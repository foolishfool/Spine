using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ProtoTblConfig;

public class HeroFightGroup : FightGroup {
    Dictionary<int, HeroData> heroDataDic = new Dictionary<int,HeroData>();
    Dictionary<int, HeroEvolution> heroEvolution = new Dictionary<int, HeroEvolution>();

	public Transform middle_point;
	public Transform cinemaLEndPos;
	public Transform cinemaREndPos;
	public Transform heroLStopPos;
	public Transform heroRStopPos;
	public Transform backGroundLEndPos;
	public Transform backGroundREndPos;



	bool MovingToNext = false;  

	[HideInInspector]
	Vector3 firstEndPos;
	[HideInInspector]
	Animation anim;
	[HideInInspector]
	Vector3 middlepointpos;
	Color headcolor;
	FightUnit firstUnit;
	HeroFightUnit unit;

//	public ScrollSceneManager scrollSceneManager;
    void Awake()
    {
        group = GroupType.Mine;
        FightManager.GetInstance().mineGroup = this;
        heroDataDic = Util.GetDic<MsgHeroData, HeroData>();
        heroEvolution = Util.GetDic<MsgHeroEvolution, HeroEvolution>();
    }

    public override void Start()
    {
        base.Start();
        targetGroup = FightManager.GetInstance().enemyGroup;
        InstUnits();        
    }

    /// <summary>
    /// 实例化所有作战单位
    /// </summary>
    public void InstUnits()
    {
        HealthUIManager.instance.ResetDepth();
        for (int i = 0; i < PlayerData.GetInstance().CurrentFightHero.Count; i++)
        {
            int id = PlayerData.GetInstance().CurrentFightHero[i];
            string heroModelName = Encoding.Default.GetString(heroDataDic[id].model);
            GameObject child = AssetManager.GetGameObject(heroModelName, battle_points[i]);
            if (heroDataDic[id].direction == 1)
                child.transform.Rotate(new Vector3(0,180,0));
            child.transform.parent = transform.parent;
            child.layer = gameObject.layer;
            unit = child.AddComponent<HeroFightUnit>();
            fightUnits.Add(unit);
            unit.parentGroup = this;
            unit.orinPoint = battle_points[i];
            //赋值战斗属性
            unit.heroData = heroDataDic[id];
            unit.heroEvolution = heroEvolution[110];//假定品质1，星级0
            unit.level = 1;//假定等级是1
            unit.InitFightAttribute();            
            //赋值技能id
            HeroAttack heroAttack = child.GetComponent<HeroAttack>();
            heroAttack.normalAttackId = (int)heroDataDic[id].attackID;
            heroAttack.normalSkillId = (int)heroDataDic[id].normalSkill;
            heroAttack.specailSkillId = (int)heroDataDic[id].specialSkill;
            //生成血条UI
            Transform healthBarPos = child.transform.Find("blood");
            HealthUIManager.instance.AddNewHealthBar(fightUnits[i],healthBarPos,group == GroupType.Enemy);
            //绑定人物头像
            ViewMapper<FightPanel>.instance.RegesterHeadShot(unit,i);
			unit.OnGotHit += OnHeadPicShake;
			anim = ViewMapper<FightPanel>.instance.HeadShotMapper[unit].anim_headpic;
			headcolor = ViewMapper<FightPanel>.instance.HeadShotMapper [unit].headcolor;
            //记录初始数值
            DungeonRecord.totalHp += unit.fightAttribute.health;
        }
        /* 赋值备战单位
         * 记录初始数值*/
    }

    /// <summary>
    /// 战斗结束，返回屏幕左边，背景卷屏
    /// 恢复全体血和蓝
    /// 清除所有buff
    /// </summary>
  
    public void MoveToNext()
    {
        firstUnit = FirstUnit;
        float recover = DungeonManager.SceneSetting.recover;
        for (int i = 0; i < fightUnits.Count; i++)
        {
            fightUnits[i].state = FightUnit.UnitState.MoveToNext;
            fightUnits[i].health += Mathf.FloorToInt(fightUnits[i].fightAttribute.health * recover);
            fightUnits[i].mana += Mathf.FloorToInt(fightUnits[i].fightAttribute.mana * recover);
            fightUnits[i].ClearAllBuff();
        }
        firstEndPos = new Vector3(transform.position.x, firstUnit.transform.position.y, firstUnit.transform.position.z);
        MovingToNext = true;        
    }

    /// <summary>
    /// 通关播放胜利动画
    /// 记录通关数值
    /// </summary>
    public void Win()
    {
        for (int i = 0; i < fightUnits.Count; i++)
        {
            fightUnits[i].state = FightUnit.UnitState.Win;
            fightUnits[i].ClearAllBuff();
            DungeonRecord.allHP += fightUnits[i].health;            
        }
        for (int i = 0; i < fightUnitsBak.Count; i++)
        {
            DungeonRecord.allHP += fightUnitsBak[i].fightAttribute.health;
        }
    }

    void Update()
    {
        if (MovingToNext)
        {
            firstUnit.transform.position = Vector3.MoveTowards(firstUnit.transform.position, firstEndPos, Const.MoveToNextSpeed * Time.deltaTime);
            for (int i = 0; i < fightUnits.Count; i++)
            {
                if (fightUnits[i] == firstUnit)
                    continue;
                fightUnits[i].transform.position += Const.MoveToNextSpeed * Time.deltaTime * Vector3.left;
            }
            if (firstUnit.transform.position == firstEndPos)
            {	
                MovingToNext = false;
                FightManager.GetInstance().EndMoveToNext();
            }
        }

		if (this.FirstUnit.mTrans.localPosition.x >= (this.heroRStopPos.localPosition.x + this.heroRStopPos.parent.localPosition.x))
			this.FirstUnit.parentGroup.canMoveForward = false;
		else
			this.FirstUnit.parentGroup.canMoveForward = true;
		if (this.FirstUnit.mTrans.localPosition.x <= (this.heroLStopPos.localPosition.x + this.heroLStopPos.parent.localPosition.x))
			this.FirstUnit.parentGroup.canMoveBack = false;
		else
			this.FirstUnit.parentGroup.canMoveBack = true;
	}
	// 头像变红摇晃
	 void OnHeadPicShake(params object[] objs)
	{
		ViewMapper<FightPanel>.instance.HeadShotMapper [unit].HeadshotIcon.color = Color.red;
		anim.Play();
	}


}