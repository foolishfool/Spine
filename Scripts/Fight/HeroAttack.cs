using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoTblConfig;
using LitJson;
/// <summary>
/// 控制所有攻击产生作用和判断条件
/// </summary>
public class HeroAttack : Attack {
    public int normalAttackId;
    public NormalAttack normalAttack;

    public int normalSkillId;
    public int normalSkillLv = 1;//假定等级为1
    public ActiveSkill normalSkill;

    public int specailSkillId;
    public int specailSkillLv = 1;//假定等级为1
    public UniqueSkill specailSkill;   

    public HeroFightUnit self;

    public FightUnit target;
    
    /// <summary>
    /// 是否自动使用技能
    /// </summary>
    bool isAutoUseSkill = false;
    /// <summary>
    /// 是否自动使用绝技
    /// </summary>
    bool isAutoUseUniqueSkill = false;

    public enum UnAbleReason
    {
        None,
        Manalack,//蓝不够
        Healthlack,//血不够
        Energylack,//能量不够
        InCD,//在cd中
        InSpecailState,//状态受限
        NotFighting,//不在战斗状态下
        UsingOtherSkill,//正在使用其他技能
    }

    public override void Init()
    {
        self = GetComponent<HeroFightUnit>();
        /* 自动使用技能，当处于自动战斗或者混乱状态下 */
        self.OnNoramlAttackFinish += ThinkUseSkill;
        /*普通攻击初始化*/
        normalAttack = NGUITools.AddChild<NormalAttack>(gameObject);
        normalAttack.attack = Util.GetDic<MsgAttackData, AttackData>()[normalAttackId];
        JsonData castEffectDt = JsonMapper.ToObject(Util.GetConfigString(normalAttack.attack.castEffect));
        for (int i = 0; i < castEffectDt.Count; i++)
            normalAttack.castEffectIds.Add((int)castEffectDt[i]);
        JsonData flyEffectDt = JsonMapper.ToObject(Util.GetConfigString(normalAttack.attack.flyEffect));
        for (int i = 0; i < flyEffectDt.Count; i++)
            normalAttack.flyEffectIds.Add((int)flyEffectDt[i]);
        JsonData hitEffectDt = JsonMapper.ToObject(Util.GetConfigString(normalAttack.attack.hitEffect));
        for (int i = 0; i < hitEffectDt.Count; i++)
            normalAttack.hitEffectIds.Add((int)hitEffectDt[i]);
        normalAttack.mineUnit = self;
        normalAttack.Init();
        /*主动技能初始化*/
        normalSkill = NGUITools.AddChild<ActiveSkill>(gameObject);
        normalSkill.normalSkill = Util.GetDic<MsgNormalSkill, NormalSkill>()[normalSkillId];
        int LvUpId = normalSkillId * 100 + normalSkillLv;
        normalSkill.normalSkillLvUp = Util.GetDic<MsgNormalSkillLvUp, NormalSkillLvUp>()[LvUpId];
        normalSkill.dmgEffectId = (int)normalSkill.normalSkill.dmgEffect;
        normalSkill.castEffectId = (int)normalSkill.normalSkill.castEffect;
        normalSkill.flyEffectId = (int)normalSkill.normalSkill.flyEffect;
        normalSkill.hitEffectId = (int)normalSkill.normalSkill.hitEffect;
        normalSkill.mineUnit = self;
        normalSkill.Init();
        /*绝技初始化*/
        specailSkill = NGUITools.AddChild<UniqueSkill>(gameObject);
        specailSkill.specialSkill = Util.GetDic<MsgSpecialSkill,SpecialSkill>()[specailSkillId];
        int specailLvId = specailSkillId * 100 + specailSkillLv;
        specailSkill.specialSkillLvUp = Util.GetDic<MsgSpecialSkillLvUp, SpecialSkillLvUp>()[specailLvId];
        specailSkill.castEffectId = (int)specailSkill.specialSkill.castEffect;
        specailSkill.dmgEffectId = (int)specailSkill.specialSkill.dmgEffect;
        specailSkill.flyEffectId = (int)specailSkill.specialSkill.flyEffect;
        specailSkill.hitEffectId = (int)specailSkill.specialSkill.hitEffect;
        specailSkill.mineUnit = self;
        specailSkill.Init();
        /*队长技能初始化*/

        CEventDispatcher.GetInstance().AddEventListener(CEventType.MOVE_TO_NEXT, ResetAutoState);
    }

    void ResetAutoState(CBaseEvent evt)
    {
        isAutoUseSkill = false;
        isAutoUseUniqueSkill = false;
    }

    void OnDestroy()
    {
        CEventDispatcher.GetInstance().RemoveEventListener(CEventType.MOVE_TO_NEXT, ResetAutoState);
    }

    /// <summary>
    /// 进入战斗
    /// </summary>
    public override void DoAttack(FightUnit t)
    {
        if (target != t)
            target = t;
        if (currentSkill != null)//如果正在释放技能
            return;
        if (isAutoUseSkill)
        {
            UseActiveSkill(normalSkill);
            isAutoUseSkill = false;
        }
        else if (isAutoUseUniqueSkill)
        {
            UseUniqueSkill();
            isAutoUseUniqueSkill = false;
        }
        else
            UseNormalAttack();
    }

    /// <summary>
    /// 终止当前的技能
    /// </summary>
    public override void StopCurrent()
    {
        if (currentSkill != null)
        {
            currentSkill.Stop();
            currentSkill = null;          
        }
        if (specailSkill.isUsing)
            specailSkill.Stop();
    }

    /// <summary>
    /// 结束战斗
    /// </summary>
    public override void EndAttack()
    {
        StopCurrent();
        enabled = false;
		self.anim.Play (Const.IdleAction, true);
    }

    /// <summary>
    /// 尝试普通攻击
    /// </summary>
    /// <returns></returns>
    public override bool UseNormalAttack()
    {
        if (self.beAbleFight && normalAttack.time == 0)
        {
            normalAttack.Use(target);
            currentSkill = normalAttack;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 尝试使用技能
    /// </summary>
    /// <param name="skill"></param>
    /// <returns></returns>
    public override bool UseActiveSkill(ActiveSkill skill)
    {
        UnAbleReason reason = UnAbleReason.None;
        if (!self.beAbleUseSkill)
            reason = UnAbleReason.InSpecailState;
        else if (skill.cdValue != 0)
            reason = UnAbleReason.InCD;
        else if (self.mana < skill.normalSkillLvUp.MPCost)
            reason = UnAbleReason.Manalack;
        else if (self.health < skill.normalSkillLvUp.HPCost)
            reason = UnAbleReason.Healthlack;
        if (reason != UnAbleReason.None)
        {
            if (!isAutoUseSkill)
                TipReason(reason);
            return false;
        }
        else
        {
            StopCurrent();
            skill.Use(target);
            currentSkill = skill;
            return true;
        }
    }

    /// <summary>
    /// 尝试使用绝技
    /// </summary>
    /// <returns></returns>
    public override bool UseUniqueSkill()
    {
        UnAbleReason reason = UnAbleReason.None;
        if (!self.beAbleUseSkill)
            reason = UnAbleReason.InSpecailState;
        else if (!FightEnergy.instance.isEnergyEnough)
            reason = UnAbleReason.Energylack;
        if (reason != UnAbleReason.None)
        {
            if (!isAutoUseUniqueSkill)
                TipReason(reason);
            return false;
        }
        else
        {
            StopCurrent();
            specailSkill.Use(target);
            currentSkill = specailSkill;
            return true;
        }
    }

    /// <summary>
    /// 提示错误信息
    /// </summary>
    /// <param name="reason"></param>
    void TipReason(UnAbleReason reason)
    {

    }

    /// <summary>
    /// 使用主动技能,UI调用
    /// </summary>
    public void UI_UseActiveSkill(params object[] objs)
    {       
        UnAbleReason reason = UnAbleReason.None;
        if (self.state != FightUnit.UnitState.Fighting)
            reason = UnAbleReason.NotFighting;
        else if (self.isConfused)
            reason = UnAbleReason.InSpecailState;
        if (currentSkill != null && currentSkill == specailSkill)
            reason = UnAbleReason.UsingOtherSkill;
        if (reason != UnAbleReason.None)
            TipReason(reason);
        else
        {
            if (target == null || target.state == FightUnit.UnitState.Dead)
            {
                target = FightRule.GetFightTarget(self, self.parentGroup.targetGroup);
                if (target == null)
                    return;
            }
            UseActiveSkill(normalSkill);
        }
    }

    /// <summary>
    /// 使用绝技技能,HeroUnitUI调用
    /// </summary>
    /// <returns></returns>
    public void UI_UseUniqueSkill(params object[] objs)
    {
        UnAbleReason reason = UnAbleReason.None;
        if (self.state != FightUnit.UnitState.Fighting)
            reason = UnAbleReason.NotFighting;
        else if (self.isConfused)
            reason = UnAbleReason.InSpecailState;
        if (reason != UnAbleReason.None)
        {
            TipReason(reason);
            return;
        }
        else
        {
            if (target == null || target.state == FightUnit.UnitState.Dead)
            {
                target = FightRule.GetFightTarget(self, self.parentGroup.targetGroup);
                if (target == null)
                    return;
            }
            UseUniqueSkill();
        }
    }

    /// <summary>
    /// 终止绝技技能,HeroUnitUI调用
    /// </summary>
    /// <returns></returns>
    public void EndUniqueSkill()
    {
        specailSkill.EndSkill();
    }

     /// <summary>
    /// 每次普通攻击结束后思考一次是否使用技能
    /// </summary>
    public void ThinkUseSkill(object[] objs)
    {
        if (self.isConfused)
        {
            if(FightManager.GetInstance().needEnergy <= FightEnergy.instance.EnergyVal)
                isAutoUseUniqueSkill = true;
            else if(normalSkill.cdValue != 0)
                isAutoUseSkill = true;
        }
        else if (FightManager.GetInstance().isAutoFight && normalSkill.cdValue == 0 && Random.value * 10000 <= normalSkill.normalSkill.weight)
            isAutoUseSkill = true;
    }

    /// <summary>
    /// 使用绝技,自动战斗时fightmanager调用
    /// </summary>
    public void AutoUseUniqueSkill()
    {
        isAutoUseUniqueSkill = true;
    }


}
	
