using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ProtoTblConfig;

public class DungeonManager : MonoBehaviour {
    public HeroFightGroup mineGroup;
    public MonsterFightGroup enemyGroup;
    public Transform sceneParent;
    //敌方怪物id（临时，应由服务器传过来）
    public static List<int[]> enemyWave = new List<int[]>();
    //当前副本的配置
    public static Dungeon SceneSetting;
    
    /// <summary>
    /// 副本时间
    /// </summary>
    public int DungeonTime{
        get { return Mathf.FloorToInt(Time.time - DungeonRecord.dungeonStartTime); }
    }

    public int dungeonEndTime;

    public static DungeonManager instance;
    void Awake()
    {
        instance = this;
        DungeonRecord.Clear();
        StartCoroutine(Init());
    }
    /// <summary>
    /// 优化。。。在选择关卡的时候就开始加载，读取进度条的时候完成剩下的加载
    /// </summary>
    /// <returns></returns>
    IEnumerator Init()
    {
        /*加载场景资源*/
        string sceneAssetName = Encoding.Default.GetString(SceneSetting.scene);
        yield return StartCoroutine(AssetManager.LoadAsset(sceneAssetName,AssetManager.AssetType.Scene,false));
        AssetManager.GetGameObject(sceneAssetName,sceneParent);       
        /* 加载所有用到的模型资源 */
        Dictionary<int, HeroData> HeroDataDic = Util.GetDic<MsgHeroData, HeroData>();
        for (int i = 0; i < PlayerData.GetInstance().CurrentFightHero.Count; i++)
        {
            int id = PlayerData.GetInstance().CurrentFightHero[i];
            string heroModelName = Encoding.Default.GetString(HeroDataDic[id].model);
            yield return StartCoroutine(AssetManager.LoadAsset(heroModelName,AssetManager.AssetType.Model,false));
        }
        Dictionary<int, MonsterData> MonsterDataDic = Util.GetDic<MsgMonsterData, MonsterData>();
        for (int i = 0; i < enemyWave.Count; i++)
        {
            for (int j = 0; j < enemyWave[i].Length; j++)
            {
                MonsterData monsterData = MonsterDataDic[enemyWave[i][j]];
                string monsterModelName = Encoding.Default.GetString(monsterData.model);
                yield return StartCoroutine(AssetManager.LoadAsset(monsterModelName, AssetManager.AssetType.Model, false));
            }
        }
        /* 加载UI界面 */
        yield return StartCoroutine(AssetManager.LoadAsset(Const.FightPanel, AssetManager.AssetType.UI, true));
        GameObject panel = AssetManager.GetGameObject(Const.FightPanel, UIContainer.instance.transform);
        if (UIContainer.instance != null)
            UIContainer.instance.fightPanel = panel;

        /*全部加载完成后才刷兵*/
        mineGroup.gameObject.SetActive(true);
        enemyGroup.gameObject.SetActive(true);

        //清除加载界面
        if (LoadScene.obj != null)
            Destroy(LoadScene.obj);
        //初始化乌鸦空袭
        KongxiManager.instance.Init(Util.GetConfigString(SceneSetting.airRaid));
        //赋值副本开启时间
        DungeonRecord.dungeonStartTime = Time.time;
    }

    void Start()
    {
        CEventDispatcher.GetInstance().AddEventListener(CEventType.GAME_WIN, Evaluate);
    }

    void OnDestroy()
    {
        CEventDispatcher.GetInstance().RemoveEventListener(CEventType.GAME_WIN, Evaluate);
    }

    /// <summary>
    /// 进行结算
    /// </summary>
    public void Evaluate(CBaseEvent evt)
    {
        dungeonEndTime = DungeonTime;
        EvaluatePanel.EvaluateData data = new EvaluatePanel.EvaluateData();
        //时间分
        data.timeScore = Const.CONST_DUNGEON_MAX_TIME_POINT - Mathf.Max(0, Mathf.RoundToInt((float)(dungeonEndTime - SceneSetting.standardTime) * Const.CONST_DUNGEON_TIME_FACTOR));
        //血量分
        data.healthScore = Mathf.FloorToInt( 
            Mathf.Min(1f, (float)DungeonRecord.allHP / (DungeonRecord.totalHp * Const.CONST_DUNGEON_LIFE_PCT)) 
            * Const.CONST_DUNGEON_MAX_LIFE_POINT);
        //连击分
        data.comboScore = Mathf.Clamp(Mathf.RoundToInt((float)DungeonRecord.maxCombo * Const.CONST_DUNGEON_COMBO_FACTOR - Const.CONST_DUNGEON_COMBO_BASIC), 0, Const.CONST_DUNGEON_MAX_COMBO_POINT);
        //技巧分
        data.skillScore = Const.CONST_DUNGEON_TECH_BASIC - Mathf.RoundToInt((float)(DungeonRecord.unKillDragObj + DungeonRecord.unKillWuya) * Const.CONST_DUNGEON_TECH_FACTOR);
        //死亡分
        data.deathScore = Const.CONST_DUNGEON_DEATH_BASIC - Mathf.RoundToInt(Const.CONST_DUNGEON_DEATH_FACTOR * DungeonRecord.heroDieCount);
        //绝技分
        data.uniqueSkillScore = Mathf.Clamp(DungeonRecord.uniqueSkillCount * Const.CONST_DUNGEON_SPECSKILL_FACTOR - Const.CONST_DUNGEON_SPECSKILL_BASIC, 0, Const.CONST_DUNGEON_MAX_SPECSKILL_POINT);
        EvaluatePanel.data = data;

        /*发送服务器分数，得到结果后才显示结算界面*/
        StartCoroutine(LoadEvaluatePanel());
    }

    /// <summary>
    /// 加载结算界面
    /// </summary>
    IEnumerator LoadEvaluatePanel()
    {
        float time = Time.time;
        yield return StartCoroutine(AssetManager.LoadAsset(Const.EvaluatePanel, AssetManager.AssetType.UI, false));
        yield return new WaitForSeconds(Mathf.Max(0,2.5f - Time.time + time));
        AssetManager.GetGameObject(Const.EvaluatePanel, UIContainer.instance.transform);
    }
}


/// <summary>
/// 用于计算通关结算分数
/// </summary>
public static class DungeonRecord
{
    public static float dungeonStartTime;// 副本开始时间
    public static int allHP;//通关时所有英雄的生命总和
    public static int totalHp;//初始所有英雄的生命总和
    public static int maxCombo;//最高连击数
    public static int unKillWuya;//未击落的乌鸦
    public static int unKillDragObj;//未划掉的抛掷物
    public static int heroDieCount;//英雄死亡数
    public static int uniqueSkillCount;//绝技使用次数

    public static void Clear()
    {
        dungeonStartTime = 0;
        allHP = 0;
        totalHp = 0;
        maxCombo = 0;
        unKillWuya = 0;
        unKillDragObj = 0;
        heroDieCount = 0;
        uniqueSkillCount = 0;
    }
}
