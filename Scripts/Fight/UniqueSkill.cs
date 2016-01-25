using UnityEngine;
using System.Collections;
using ProtoTblConfig;
using LitJson;
using System.Collections.Generic;
/// <summary>
/// 绝技
/// </summary>
public class UniqueSkill : Skill
{
    public SpecialSkill specialSkill;
    public SpecialSkillLvUp specialSkillLvUp;
    public bool isUsing = false;
    /// <summary>
    /// 施法动作
    /// </summary>
    string preActionName;
    /// <summary>
    /// 施法动作起效时间
    /// </summary>
    float[] preActionWorkTime; 
    /// <summary>
    /// 持续施法动作
    /// </summary>
    string actionName;
    /// <summary>
    /// 持续施法动作起效时间
    /// </summary>
    float[] actionWorkTime;
    /// <summary>
    /// 魔法特效的作用时间
    /// </summary>
    public float[] magicDelay;
    /// <summary>
    /// 是否正在施法状态（人物变亮，屏幕变黑的状态）
    /// </summary>
    bool isMagicing = false;

    public List<buffConfig> buffConfigList = new List<buffConfig>();
    public SkillAttach attachExtension;
    public JsonData attachExtensionParam;

    public override void Init()
    {       
        CEventDispatcher.GetInstance().AddEventListener(CEventType.GAME_WIN, EndSkill);
        CEventDispatcher.GetInstance().AddEventListener(CEventType.MOVE_TO_NEXT, EndSkill);
        CEventDispatcher.GetInstance().AddEventListener(CEventType.ENERGY_EMPTY, EndSkill);

        //魔法动作
        string magicDt = Util.GetConfigString(specialSkill.delay);
        JsonData jd00 = JsonMapper.ToObject(magicDt);
        if (jd00.Count > 0)
        {
            magicDelay = new float[jd00.Count];
            for (int i = 0; i < jd00.Count; i++)
                magicDelay[i] = float.Parse(jd00[i].ToString());
        }

        //施法动作
        string action01 = Util.GetConfigString(specialSkill.preAction);
        JsonData jd01 = JsonMapper.ToObject(action01);
        if (jd01.Count > 0)
        {
            preActionName = jd01[0].ToString();
            preActionWorkTime = new float[jd01[1].Count];
            for (int i = 0; i < jd01[1].Count; i++)
                preActionWorkTime[i] = float.Parse(jd01[1][i].ToString());
        }

        //持续施法动作
        string action02 = Util.GetConfigString(specialSkill.skillAction);
        JsonData jd02 = JsonMapper.ToObject(action02);
        if (jd02.Count > 0)
        {
            actionName = jd02[0].ToString();
            actionWorkTime = new float[jd02[1].Count];
            for (int i = 0; i < jd02[1].Count; i++)
                actionWorkTime[i] = float.Parse(jd02[1][i].ToString());
        }

        //buff
        string buffString = Util.GetConfigString(specialSkillLvUp.buff);
        JsonData dt_1 = JsonMapper.ToObject(buffString);
        for (int i = 0; i < dt_1.Count; i++)
        {
            JsonData dt_2 = dt_1[i];
            buffConfig bc = new buffConfig();
            bc.id = int.Parse(dt_2[0].ToString());
            bc.percent = float.Parse(dt_2[1].ToString());
            bc.value = float.Parse(dt_2[2].ToString());
            bc.time = float.Parse(dt_2[3].ToString());
            bc.level = int.Parse(dt_2[4].ToString());
            bc.bindSkill = this;
            buffConfigList.Add(bc);
        }

        //附加效果
        attachExtension = (SkillAttach)specialSkillLvUp.addEffect;
        attachExtensionParam = JsonMapper.ToObject(Util.GetConfigString(specialSkillLvUp.addEffectVal));

        base.Init();
    }

    void OnDestroy()
    {
        CEventDispatcher.GetInstance().RemoveEventListener(CEventType.GAME_WIN, EndSkill);
        CEventDispatcher.GetInstance().RemoveEventListener(CEventType.MOVE_TO_NEXT, EndSkill);
        CEventDispatcher.GetInstance().RemoveEventListener(CEventType.ENERGY_EMPTY, EndSkill);
        EndSkill();
    }

    public override void DoSkill()
    {
        if (isUsing)
            return;
        isUsing = true;
        FightEnergy.instance.EnergyUseVal += (int)specialSkillLvUp.EnergyCost;

        //把英雄头像ui上拉
        HeroFightUnit unit = (HeroFightUnit)mineUnit;
        ViewMapper<FightPanel>.instance.BringForewardHeadShot(unit);
        //增加黑幕
        FightEffectManager.instance.AddOne();
        //把人物放在变亮层
        FightEffectManager.instance.BringForeward(mineUnit);
        isMagicing = true;

        if (specialSkill.actionLimit != 1)
            StartCoroutine(StartPreSkill());
        else        
            StartCoroutine(StartChixuSkill());

        //记录使用次数
        DungeonRecord.uniqueSkillCount++;
    }

    /// <summary>
    /// 开始施法动作
    /// </summary>
    IEnumerator StartPreSkill()
    {            
        if (!string.IsNullOrEmpty(preActionName))
            mineUnit.anim.Play(preActionName, false);
        OnBefore();
        yield return new WaitForSeconds(preActionWorkTime[0]);
        while (true)
        {
            OnIng();
            yield return new WaitForSeconds(1f);
        }
    }

    /// <summary>
    /// 开始持续施法动作
    /// </summary>
    IEnumerator StartChixuSkill()
    {
        if (!string.IsNullOrEmpty(actionName))
            mineUnit.anim.Play(actionName, true);
        OnBefore();
        while(true)
        {
            yield return new WaitForSeconds(actionWorkTime[0]);
            OnIng();
        }  
    }

    public override void OnBefore()
    {
        base.OnBefore();
        StartCoroutine(DisplayPreEffect());
    }

    /// <summary>
    /// 结束绝技过程
    /// </summary>
    public void EndSkill()
    {
        if (!isUsing)
            return;
        isUsing = false;
              
        FightEnergy.instance.EnergyUseVal -= (int)specialSkillLvUp.EnergyCost;

        //把英雄头像ui放回
        HeroFightUnit unit = (HeroFightUnit)mineUnit;
        ViewMapper<FightPanel>.instance.BringBackHeadShot(unit);

        if (isMagicing)
        {
            //减少黑幕
            FightEffectManager.instance.MinusOne();
            //把人物放回原层
            FightEffectManager.instance.BringBack(mineUnit);
            isMagicing = false;
        }

        if (cachedEffect != null)
            Destroy(cachedEffect);

        StopAllCoroutines();
        OnNone();
    }

    /// <summary>
    /// 战斗结束事件
    /// </summary>
    /// <param name="evt"></param>
    void EndSkill(CBaseEvent evt)
    {
        if(isUsing)
            EndSkill();
    }

    /// <summary>
    /// 强行停止
    /// </summary>
    public override void Stop()
    {
        EndSkill();
    }  

    public override void OnIng()
    {
        base.OnIng();
        FightRule.UniqueSkillWork(mineUnit,targetUnit,this);
    }

    public override void BindAnim()
    {
        mineUnit.anim.state.End += (state, trackIndex) =>
        {
            if (state.GetCurrent(trackIndex).animation.name.Equals(preActionName))
            {
                if (isMagicing)
                {
                    //减少黑幕
                    FightEffectManager.instance.MinusOne();
                    //把人物放回原层
                    FightEffectManager.instance.BringBack(mineUnit);
                    isMagicing = false;
                }
                OnNone();
            }
        };
    }

    /// <summary>
    /// 某些特效只生成一次，然后循环播放，直至绝技结束
    /// </summary>
    GameObject cachedEffect;
    /// <summary>
    /// 展现魔法特效
    /// </summary>
    public override IEnumerator DisplaydmgEffect(FightUnit dmgEffectTarget)
    {
        if (dmgEffect != null && !string.IsNullOrEmpty(dmgEffectName) && cachedEffect == null)
        {
            EffectTarget effectTarget = (EffectTarget)dmgEffect.target;
            Vector3 effectLocalPos = Vector3.zero;
            Transform dmgEffectTrans = null;
            if (effectTarget == EffectTarget.Screen)
                dmgEffectTrans = FightEffectManager.instance.transform;
            else if (effectTarget == EffectTarget.MineCenter)
            {
                dmgEffectTrans = mineUnit.mTrans.parent;
                effectLocalPos = mineUnit.parentGroup.Center;
            }
            else if (effectTarget == EffectTarget.EnemyCenter)
            {
                dmgEffectTrans = mineUnit.mTrans.parent;
                effectLocalPos = targetUnit.parentGroup.Center;
            }
            else if (dmgEffectTarget != null)
                dmgEffectTrans = dmgEffectTarget.GetEffectPoint((EffectPoint)dmgEffect.bone);

            yield return StartCoroutine(AssetManager.LoadAsset(dmgEffectName, AssetManager.AssetType.Effect, false));
            GameObject obj = AssetManager.GetGameObject(dmgEffectName, dmgEffectTrans);
            obj.transform.localPosition = effectLocalPos;

            if (effectTarget != EffectTarget.Enemy)
                cachedEffect = obj;
        }
    }
}
