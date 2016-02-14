using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using ProtoTblConfig;
using Holoville.HOTween;

//计算战斗数值的基本属性
[System.Serializable]
public class FightAttribute
{
    /*需要初始化的数据*/
    public int attack;//攻击
    public int defence;//防御
    public int health;//生命上限
    public int mana;//法力上限
    public int attackSpeed;//攻击速度   
    public int range;//攻击距离
    public int critical;//暴击   
    public int hit;//命中  
    public int dodge;//闪避   
    public int toughness;//韧性
    public float criticalImprove;//暴击伤害加成
    public HurtType elementType;//属性类型（火水土光暗）

    public float allDefenseRate;//全免伤系数
    public float fireDefenseRate;//火免伤系数
    public float waterDefenseRate;//水免伤系数
    public float eathDefenseRate;//土免伤系数
    public float lightDefeneseRate;//光免伤系数
    public float darkDefenseRate;//暗免伤系数

    public float allDamageRate;//全增伤系数

    public float cureRate;//治疗系数

    public int level;//等级

    public float NormalAttackSpace { get { return (float)Const.CONST_ATTACK_INTERVAL / Mathf.Max(1,attackSpeed); } }//攻击间隔
	public float MoveSpeed { get { return Const.MoveSpeed * Mathf.Max(0, 1 + moveSpeedBuff); } }
	public float PursueSpeed { get { return Const.PursueSpeed * Mathf.Max(0, 1 + moveSpeedBuff); } }
	public float GetBackSpeed { get { return Const.GetBackSpeed; } }
    public int Defence { get { return Mathf.RoundToInt((float)defence * Mathf.Max(0, 1 + defenceBuff)); } }
	public float alarmRange = 400f;
	public float attackRange = 300f;
	public float escapeRange = 450f;

    /*只供buff改变*/
    public float criticalRateBuff;//暴击buff修正
    public float dodgeRateBuff;//闪避buff修正
    public float hitRateBuff;//命中buff修正
    public float moveSpeedBuff;//移动速度buff修正
    public float defenceBuff;//防御buff修正
}

/// <summary>
/// 定义战斗相关计算的流程和规则
/// </summary>
public static class FightRule
{
    /// <summary>
    /// 距离修正系数
    /// </summary>
    static Dictionary<int, DistanceFactor> _DistanceFactor;
    static Dictionary<int, DistanceFactor> DistanceFactorDic
    {
        get
        {
            if (_DistanceFactor == null)
                _DistanceFactor = Util.GetDic<MsgDistanceFactor, DistanceFactor>();            
            return _DistanceFactor;
        }
    }
    /// <summary>
    /// 属性相克系数
    /// </summary>
    static Dictionary<int, ElementFactor> _ElementFactor;
    static Dictionary<int, ElementFactor> ElementFactorDic
    {
        get
        {
            if (_ElementFactor == null)
                _ElementFactor = Util.GetDic<MsgElementFactor, ElementFactor>();
            return _ElementFactor;
        }
    }
    /// <summary>
    /// buff配置
    /// </summary>
    static Dictionary<int, BuffData> _BuffData;
    static Dictionary<int, BuffData> BuffDataDic
    {
        get
        {
            if (_BuffData == null)
                _BuffData = Util.GetDic<MsgBuffData, BuffData>();
            return _BuffData;
        }
    }
    
#region 伤害计算

    /// <summary>
    /// 造成技能治疗/辅助
    /// 直接治疗量=(攻击方最终攻击*技能加成百分比+技能加成治疗量)*被治疗方受治疗加成系数*random(0.95,1.05)
    /// </summary>
    public static void FightCure(FightUnit attacker, FightUnit target, ActiveSkill skill)
    {
        if (target == null || target.health == 0)
            return;
        int cureNum = Mathf.Max(0, Mathf.RoundToInt((attacker.fightAttribute.attack * skill.normalSkillLvUp.percent + skill.normalSkillLvUp.additional) * (1 + target.fightAttribute.cureRate) * Random.Range(0.95f, 1.05f)));
        if (cureNum > 0)
        {
            HealthUIManager.instance.DisplayHurtNum(target, cureNum.ToString(), false, false);
            target.health += cureNum;
        }
        skill.StartCoroutine(skill.DisplayHitEffect(target));
        SkillAttachBuff(attacker, target, skill.buffConfigList,(HurtType)skill.normalSkill.attackAttribute);
        //附加效果
        AttachExtension(attacker, target, 0, skill.attachExtension, skill.attachExtensionParam);
    }

    public static void UniqueFightCure(FightUnit attacker, FightUnit target, UniqueSkill skill)
    {
        if (target == null || target.health == 0)
            return;
        int cureNum = Mathf.Max(0, Mathf.RoundToInt((attacker.fightAttribute.attack * skill.specialSkillLvUp.percent + skill.specialSkillLvUp.additional) * (1 + target.fightAttribute.cureRate) * Random.Range(0.95f, 1.05f)));
        if (cureNum > 0)
        {
            HealthUIManager.instance.DisplayHurtNum(target, cureNum.ToString(), false, false);
            target.health += cureNum;
        }
        skill.StartCoroutine(skill.DisplayHitEffect(target));
        SkillAttachBuff(attacker, target, skill.buffConfigList, (HurtType)skill.specialSkill.attackAttribute);
        //附加效果
        AttachExtension(attacker, target, 0, skill.attachExtension, skill.attachExtensionParam);
    }

    /// <summary>
    /// 造成吸血治疗
    /// </summary>
    public static void XixueCure(int cureNum, FightUnit target)
    {
        HealthUIManager.instance.DisplayHurtNum(target, cureNum.ToString(), false, false);
        target.health += cureNum;
    }

    /// <summary>
    /// 造成buff治疗
    /// </summary>
    public static void BuffCure(int cureNum, FightUnit target)
    {
        cureNum = Mathf.Max(0, Mathf.RoundToInt((float)cureNum * (1 + target.fightAttribute.cureRate)));
        HealthUIManager.instance.DisplayHurtNum(target, cureNum.ToString(), false, false);
        target.health += cureNum;
    }

    /// <summary>
    /// 计算护盾吸收伤害
    /// </summary>
    public static int CaculateHudun(int hurtNum,FightUnit target, HurtType hurtType)
    {
        for (int i = target.OnHudunList.Count - 1; i >= 0; i--)
        {
            if (target.OnHudunList[i] != null)
                hurtNum = target.OnHudunList[i](hurtNum, hurtType);
        }
        return hurtNum;
    }

    /// <summary>
    /// 主动技能伤害
    ///	技能伤害=(攻击方最终攻击*技能加成百分比+技能基础值)*防御效果*攻击方伤害加成效果系数*防御方对应属性免伤系数*防御方全免伤系数*属性相克系数*暴击伤害系数*random(0.95,1.05)										
    ///		技能的属性在技能中配置									
    ///		技能加成百分比和技能基础值直接根据技能id和技能等级读表获得									
    ///											
    ///	技能真实伤害=(攻击方最终攻击*技能加成百分比+技能基础攻击)*攻击方伤害加成效果系数*防御方全免伤系数*暴击伤害系数*random(0.95,1.05)										
    /// </summary>
    public static void ActiveSkillDamage(FightUnit attacker, FightUnit target, ActiveSkill skill)
    {
        if (target == null || attacker == null || target.isInvincible)
            return;
        //判断命中
        if (IsMiss(attacker, target))
        {
            HealthUIManager.instance.DisplayHurtNum(target, "Miss", true, false);
            //当闪避攻击时
            if (target.OnDodge != null)
                target.OnDodge();
            return;
        }
        //判断无敌
        if (target.isInvincible)
        {
            HealthUIManager.instance.DisplayHurtNum(target, "Invincible", true, false);
            return;
        }
        //判断暴击
        bool isCrit = false;
        float critScale = 1f;
        if (IsCritical(attacker, target))
        {
            isCrit = true;
            critScale = Const.CONST_BASIC_CRIT_DMG + attacker.fightAttribute.criticalImprove;
            //当被暴击时
            if (target.OnGotCritil != null)
                target.OnGotCritil();
        }
        float defense = (float)1 / (1 + Const.CONST_DEF_FACTOR * target.fightAttribute.Defence);//防御效果
        defense = Mathf.Clamp(defense, Const.CONST_MIN_DEF_EFFECT, Const.CONST_MAX_DEF_EFFECT);
        int hurtNum = 1;
        HurtType hurtType = (HurtType)skill.normalSkill.attackAttribute;
        if (hurtType != HurtType.True)
        {
            float elementFactor = ElementFactorDic[skill.normalSkill.attackAttribute * 10 + (int)target.fightAttribute.elementType].factor;//属性伤害系数
            float elementDefenseFactor = 0;
            switch (hurtType)
            {
                case HurtType.Fire:
                    elementDefenseFactor = target.fightAttribute.fireDefenseRate;
                    break;
                case HurtType.Water:
                    elementDefenseFactor = target.fightAttribute.waterDefenseRate;
                    break;
                case HurtType.Earth:
                    elementDefenseFactor = target.fightAttribute.eathDefenseRate;
                    break;
                case HurtType.Light:
                    elementDefenseFactor = target.fightAttribute.lightDefeneseRate;
                    break;
                case HurtType.Dark:
                    elementDefenseFactor = target.fightAttribute.darkDefenseRate;
                    break;
            }
            hurtNum = Mathf.RoundToInt((float)(attacker.fightAttribute.attack * skill.normalSkillLvUp.percent + skill.normalSkillLvUp.additional) * defense 
                * ( 1 + attacker.fightAttribute.allDamageRate ) * ( 1 -  elementDefenseFactor) * ( 1 - target.fightAttribute.allDefenseRate ) * elementFactor * critScale * Random.Range(0.95f, 1.05f));            
        }
        else
            hurtNum = Mathf.RoundToInt((float)(attacker.fightAttribute.attack * skill.normalSkillLvUp.percent + skill.normalSkillLvUp.additional)
                * (1 + attacker.fightAttribute.allDamageRate) * (1 - target.fightAttribute.allDefenseRate) * critScale * Random.Range(0.95f, 1.05f));
        hurtNum = Mathf.Max(0,hurtNum);
        
        //受击特效
        skill.StartCoroutine(skill.DisplayHitEffect(target));
        //触发技能命中事件
        if(attacker.OnSkillHit != null)
            attacker.OnSkillHit(hurtNum);
        //触发反弹事件
        if (target.OnFantan != null)
            target.OnFantan(hurtNum, skill, hurtType);
        //护盾计算
        hurtNum = CaculateHudun(hurtNum, target, hurtType);
        //附加buff
        SkillAttachBuff(attacker, target, skill.buffConfigList, (HurtType)skill.normalSkill.attackAttribute);
        //附加效果
        AttachExtension(attacker, target, hurtNum, skill.attachExtension, skill.attachExtensionParam);
        //伤血
        if (hurtNum == 0)
            return;
        HealthUIManager.instance.DisplayHurtNum(target, hurtNum.ToString(), true, isCrit);
        target.health -= hurtNum;
        if (target.health == 0)
            FightManager.GetInstance().UnitDead(attacker, target);        
    }


    public static void UniqueSkillDamage(FightUnit attacker, FightUnit target, UniqueSkill skill)
    {
        if (target == null || attacker == null || target.isInvincible)
            return;
        //判断命中
        if (IsMiss(attacker, target))
        {
            HealthUIManager.instance.DisplayHurtNum(target, "Miss", true, false);
            //当闪避攻击时
            if (target.OnDodge != null)
                target.OnDodge();
            return;
        }
        //判断无敌
        if (target.isInvincible)
        {
            HealthUIManager.instance.DisplayHurtNum(target, "Invincible", true, false);
            return;
        }
        //判断暴击
        bool isCrit = false;
        float critScale = 1f;
        if (IsCritical(attacker, target))
        {
            isCrit = true;
            critScale = Const.CONST_BASIC_CRIT_DMG + attacker.fightAttribute.criticalImprove;
            //当被暴击时
            if (target.OnGotCritil != null)
                target.OnGotCritil();
        }
        float defense = (float)1 / (1 + Const.CONST_DEF_FACTOR * target.fightAttribute.Defence);//防御效果
        defense = Mathf.Clamp(defense, Const.CONST_MIN_DEF_EFFECT, Const.CONST_MAX_DEF_EFFECT);
        int hurtNum = 1;
        HurtType hurtType = (HurtType)skill.specialSkill.attackAttribute;
        if (hurtType != HurtType.True)
        {
            float elementFactor = ElementFactorDic[skill.specialSkill.attackAttribute * 10 + (int)target.fightAttribute.elementType].factor;//属性伤害系数
            float elementDefenseFactor = 0;
            switch (hurtType)
            {
                case HurtType.Fire:
                    elementDefenseFactor = target.fightAttribute.fireDefenseRate;
                    break;
                case HurtType.Water:
                    elementDefenseFactor = target.fightAttribute.waterDefenseRate;
                    break;
                case HurtType.Earth:
                    elementDefenseFactor = target.fightAttribute.eathDefenseRate;
                    break;
                case HurtType.Light:
                    elementDefenseFactor = target.fightAttribute.lightDefeneseRate;
                    break;
                case HurtType.Dark:
                    elementDefenseFactor = target.fightAttribute.darkDefenseRate;
                    break;
            }
            hurtNum = Mathf.RoundToInt((float)(attacker.fightAttribute.attack * skill.specialSkillLvUp.percent + skill.specialSkillLvUp.additional) * defense
                * (1 + attacker.fightAttribute.allDamageRate) * (1 - elementDefenseFactor) * (1 - target.fightAttribute.allDefenseRate) * elementFactor * critScale * Random.Range(0.95f, 1.05f));
        }
        else
            hurtNum = Mathf.RoundToInt((float)(attacker.fightAttribute.attack * skill.specialSkillLvUp.percent + skill.specialSkillLvUp.additional)
                * (1 + attacker.fightAttribute.allDamageRate) * (1 - target.fightAttribute.allDefenseRate) * critScale * Random.Range(0.95f, 1.05f));
        hurtNum = Mathf.Max(0, hurtNum);

        //受击特效
        skill.StartCoroutine(skill.DisplayHitEffect(target));
        //触发技能命中事件
        if (attacker.OnSkillHit != null)
            attacker.OnSkillHit(hurtNum);
        //触发反弹事件
        if (target.OnFantan != null)
            target.OnFantan(hurtNum, skill, hurtType);
        //护盾计算
        hurtNum = CaculateHudun(hurtNum, target, hurtType);
        //附加buff
        SkillAttachBuff(attacker, target, skill.buffConfigList, (HurtType)skill.specialSkill.attackAttribute);
        //附加效果
        AttachExtension(attacker, target, hurtNum, skill.attachExtension, skill.attachExtensionParam);
        //伤血
        if (hurtNum == 0)
            return;
        HealthUIManager.instance.DisplayHurtNum(target, hurtNum.ToString(), true, isCrit);
        target.health -= hurtNum;
        if (target.health == 0)
            FightManager.GetInstance().UnitDead(attacker, target);
    }

    /// <summary>
    /// 普通攻击伤害
    /// 普通攻击伤害=攻击方最终攻击*防御效果*攻击方伤害加成效果系数*防御方对应属性免伤系数*防御方全免伤系数*属性相克系数*暴击伤害系数*random(0.95,1.05)		
    /// 防御效果=1/(1+防御系数*防御方最终防御)	
    /// 防御效果必须介于20%和100%之间，这两个参数由策划配置	
    /// 防御常数为常量，由策划配置，下同	
    /// 普通攻击的属性和单位自身的属性相同	
    /// </summary>
    public static void NormalAttackDamage(FightUnit attacker, FightUnit target, NormalAttack skill,SpecialEffect hitEffect)
    {
        if (target == null || attacker == null || target.isInvincible)
            return;

        //判断命中
        if (IsMiss(attacker, target))
        {
            HealthUIManager.instance.DisplayHurtNum(target, "Miss", true, false);
            //触发闪避事件
            if (target.OnDodge != null)
                target.OnDodge();
            return;
        }

        //判断无敌
        if (target.isInvincible)
        {
            HealthUIManager.instance.DisplayHurtNum(target, "Invincible", true, false);
            return;
        }

        //判断暴击
        bool isCrit = false;
        float critScale = 1f;
        if (IsCritical(attacker, target))
        {
            isCrit = true;
            critScale = Const.CONST_BASIC_CRIT_DMG + attacker.fightAttribute.criticalImprove;
            //当普通攻击暴击时
            if (attacker.OnNormalAttackCritil != null)
                attacker.OnNormalAttackCritil(target);
            //当被暴击时
            if (target.OnGotCritil != null)
                attacker.OnGotCritil();
        }

        //受击特效
        skill.StartCoroutine(skill.DisplayHitEffect(target, hitEffect));

        //计算数值
        float defense = (float)1 / (1 + Const.CONST_DEF_FACTOR * target.fightAttribute.Defence);//防御效果
        defense = Mathf.Clamp(defense, Const.CONST_MIN_DEF_EFFECT, Const.CONST_MAX_DEF_EFFECT);
        float elementDefenseFactor = 0;
        HurtType hurtType = (HurtType)attacker.fightAttribute.elementType;
        switch (hurtType)
        {
            case HurtType.Fire:
                elementDefenseFactor = target.fightAttribute.fireDefenseRate;
                break;
            case HurtType.Water:
                elementDefenseFactor = target.fightAttribute.waterDefenseRate;
                break;
            case HurtType.Earth:
                elementDefenseFactor = target.fightAttribute.eathDefenseRate;
                break;
            case HurtType.Light:
                elementDefenseFactor = target.fightAttribute.lightDefeneseRate;
                break;
            case HurtType.Dark:
                elementDefenseFactor = target.fightAttribute.darkDefenseRate;
                break;
        }
        float elementFactor = ElementFactorDic[(int)attacker.fightAttribute.elementType * 10 + (int)target.fightAttribute.elementType].factor;
        int hurtNum = Mathf.RoundToInt(attacker.fightAttribute.attack * defense
            * (1 + attacker.fightAttribute.allDamageRate) * (1 - elementDefenseFactor) * (1 - target.fightAttribute.allDefenseRate) * elementFactor * critScale * Random.Range(0.95f, 1.05f));
        hurtNum = Mathf.Max(0, hurtNum);

        //触发普通攻击命中事件
        if (attacker.OnNormalAttackHit != null)
            attacker.OnNormalAttackHit(hurtNum);
        //触发反弹事件
        if (target.OnFantan != null)
            target.OnFantan(hurtNum, skill, hurtType);
        //护盾计算
        hurtNum = CaculateHudun(hurtNum, target, hurtType);

        //伤血
        if (hurtNum == 0)
            return;
        HealthUIManager.instance.DisplayHurtNum(target, hurtNum.ToString(), true, isCrit);
        target.health -= hurtNum;
        if (target.health == 0)
            FightManager.GetInstance().UnitDead(attacker, target);
    }

    /// <summary>
    /// buff造成伤害
    /// </summary>
    public static void BuffAttackDamage(Buff buff,HurtType hurtType)
    {
        if (buff.target.isInvincible)
            return;
        FightUnit target = buff.target;     
        int hurtNum = 0;
        if (hurtType == HurtType.True)
            hurtNum = Mathf.RoundToInt(buff.buffValue * (1 - target.fightAttribute.allDefenseRate));
        else
        {
            float elementDefenseFactor = 0;
            switch (hurtType)
            {
                case HurtType.Fire:
                    elementDefenseFactor = target.fightAttribute.fireDefenseRate;
                    break;
                case HurtType.Water:
                    elementDefenseFactor = target.fightAttribute.waterDefenseRate;
                    break;
                case HurtType.Earth:
                    elementDefenseFactor = target.fightAttribute.eathDefenseRate;
                    break;
                case HurtType.Light:
                    elementDefenseFactor = target.fightAttribute.lightDefeneseRate;
                    break;
                case HurtType.Dark:
                    elementDefenseFactor = target.fightAttribute.darkDefenseRate;
                    break;
            }
            hurtNum = Mathf.RoundToInt(buff.buffValue * (1 - target.fightAttribute.allDefenseRate) * (1 - elementDefenseFactor));
        }
        hurtNum = Mathf.Max(0, hurtNum);
        //护盾计算
        hurtNum = CaculateHudun(hurtNum, target, hurtType);
        //伤血
        HealthUIManager.instance.DisplayHurtNum(target, hurtNum.ToString(), true, false);
        target.health -= hurtNum;
        if (target.health == 0)
            FightManager.GetInstance().UnitDead(buff.source, target);
    }

    /// <summary>
    /// 反弹伤害
    /// </summary>
    public static void FantanDamage(int hurtNum, FightUnit attacker, FightUnit target)
    {
        if (target.isInvincible || target == null)
            return;
        //伤血
        HealthUIManager.instance.DisplayHurtNum(target, hurtNum.ToString(), true, false);
        target.health -= hurtNum;
        if (target.health == 0)
            FightManager.GetInstance().UnitDead(attacker, target);
    }

    /* 修正Miss率=基础Miss率+(被攻击方最终闪避值-攻击方最终命中值)*闪避系数				
   其中基础Miss率和闪避系数为常数，由策划配置			
   修正Miss率的值必须介于0和60%之间，这两个参数由策划配置			
				
   最终Miss率=修正Miss率+闪避率BUFF修正-命中率BUFF修正				
   同类BUFF效果按加法计算			
   最终Miss率必须介于0和100%之间			
    * */
    static bool IsMiss(FightUnit attacker, FightUnit target)
    {
        float missRate = Mathf.Clamp(Const.CONST_BASIC_MISS_RATE + (target.fightAttribute.dodge - attacker.fightAttribute.hit) * Const.CONST_DODGE_FACTOR, Const.CONST_MIN_MISS_RATE, Const.CONST_MAX_MISS_RATE);
        missRate += target.fightAttribute.dodgeRateBuff - attacker.fightAttribute.hitRateBuff;
        missRate = Mathf.Clamp01(missRate);
        return Random.value <= missRate;
    }
    /*
     * 修正暴击率=基础暴击率+(攻击方最终暴击值-被攻击方最终韧性值)*暴击系数				
	其中基础暴击率和暴击系数为常数，由策划配置			
	修正暴击率的值必须介于设定的5%和50%之间，这两个参数由策划配置			
				
    最终暴击率=修正暴击率+BUFF修正				
	同类BUFF按加法计算			
	最终暴击率必须介于0和100%之间			
				
    暴击伤害系数				
	当发生暴击时，暴击伤害系数=基础暴击伤害系数+攻击方暴击伤害加成+BUFF加成			
	未发生暴击时，暴击伤害系数=1			
	基础暴击伤害系数为常数，由策划配置			
     * */
    static bool IsCritical(FightUnit attacker, FightUnit target)
    {
        float criticalRate = Mathf.Clamp(Const.CONST_BASIC_CRIT_RATE + (target.fightAttribute.toughness - attacker.fightAttribute.critical) * Const.CONST_CRIT_FACTOR, Const.CONST_MIN_CRIT_RATE, Const.CONST_MAX_CRIT_RATE);
        criticalRate += attacker.fightAttribute.criticalRateBuff;
        criticalRate = Mathf.Clamp01(criticalRate);
        return Random.value <= criticalRate;
    }

#endregion

#region 主动技能
    /// <summary>
    /// 主动技能的入口函数，涉及到有弹道，回调
    /// </summary>
    public static void ActiveSkillWork(FightUnit attacker, FightUnit target, ActiveSkill skill)
    {
        switch ((SkillCurve)skill.normalSkill.curve)
        {
            case SkillCurve.None:
            case SkillCurve.Melee:
                ActiveSkillSelect(attacker, target, skill);
                break;
            case SkillCurve.Directional:
                GameObject obj = ProjectileMove.CreateProjectile(skill.name + "_" + attacker.mTrans.name, attacker.mTrans.parent, skill.flyEffectFromPos, skill.flyEffect.speed, target.Body, delegate(SpecialEffect fx)
                {
                    ActiveSkillSelect(attacker, target, skill);
                });
                skill.StartCoroutine(skill.DisplayFlyEffect(obj.transform));
                break;
            case SkillCurve.Parabola:
                Vector3 targetPos;
                bool isHeroSkill = attacker.parentGroup.group == FightGroup.GroupType.Mine;
                if (skill.flyEffect.distance > 0)
                    targetPos = attacker.mTrans.localPosition + (isHeroSkill? -1 : 1) * Vector3.right * skill.flyEffect.distance;
                else
                    targetPos = target.mTrans.localPosition + target.Body.localPosition;
                GameObject obj_1 = ProjectileMove.CreateProjectile(skill.name, attacker.mTrans.parent, skill.flyEffectFromPos, skill.flyEffect.speed, skill.flyEffect.height, targetPos, delegate(SpecialEffect fx)
                {
                    ActiveSkillSelect(attacker, target, skill);
                });
                if(!isHeroSkill)
                    obj_1.AddComponent<DragRecord>();
                skill.StartCoroutine(skill.DisplayFlyEffect(obj_1.transform));
                break;
            case SkillCurve.Magica:
                if (skill.dmgEffect == null)
                    return;
                if (skill.dmgEffect.target == 4 || skill.dmgEffect.target == 5)
                    skill.StartCoroutine(AcitveSkillMagic_Whole(attacker, target, skill));
                else
                    ActiveSkillSelect(attacker, target, skill);
                break;
		case SkillCurve.LineAoe:
			bool isRight = (attacker.parentGroup.group == FightGroup.GroupType.Mine && !attacker.isConfused) ||
			                            (attacker.parentGroup.group == FightGroup.GroupType.Enemy && attacker.isConfused);
			GameObject obj_2 = LineAoeMove.CreateLineAoe (skill.name + "_" + attacker.mTrans.name, attacker.mTrans.parent, skill.flyEffectFromPos, skill.flyEffect.speed, isRight, delegate(FightUnit unit)
			{
				ActiveSkillDamage (attacker, unit, skill);
			});
                skill.StartCoroutine(skill.DisplayFlyEffect(obj_2.transform));
                break;
        }
    }

    /// <summary>
    /// 非选定目标的魔法播放过程中再进行目标选取
    /// </summary>
    public static IEnumerator AcitveSkillMagic_Whole(FightUnit attacker, FightUnit target, ActiveSkill skill)
    {
        skill.StartCoroutine(skill.DisplaydmgEffect(null));
        if (skill.magicDelay.Length > 0)
        {        
            for (int i = 0; i < skill.magicDelay.Length; i++)
            {
                yield return new WaitForSeconds(skill.magicDelay[i]);
                ActiveSkillSelect(attacker, target, skill);
            }
        }
        else
            ActiveSkillSelect(attacker, target, skill);
    }

    /// <summary>
    /// 主动技能目标选取
    /// </summary>
    public static void ActiveSkillSelect(FightUnit attacker, FightUnit target, ActiveSkill skill)
    {
        List<FightUnit> targets = new List<FightUnit>();
        switch ((SkillSelect)skill.normalSkill.selectRule)
        {
            case SkillSelect.None:
            case SkillSelect.NowTarget:
                targets.Add(target);
                break;
            case SkillSelect.LongAway:
                target = target.parentGroup.LastUnit;
                targets.Add(target);
                break;
            case SkillSelect.Weakest:
                target = target.parentGroup.WeakestUnit;
                targets.Add(target);
                break;
            case SkillSelect.Random:
                target = target.parentGroup.fightUnits[Random.Range(0, target.parentGroup.fightUnits.Count)];
                targets.Add(target);
                break;
            case SkillSelect.NowTargetNearby:
                targets = GetTargetNearby(target, skill.normalSkill.aoeRadius);
                break;
            case SkillSelect.LongAwayNearby:
                target = target.parentGroup.LastUnit;
                targets = GetTargetNearby(target, skill.normalSkill.aoeRadius);
                break;
            case SkillSelect.WeakestSeveral:
                int Count0 = Mathf.Min((int)skill.normalSkill.targetNum, target.parentGroup.fightUnits.Count);
                target.parentGroup.ResortByHP();
                for (int i = 0; i < Count0; i++)
                    targets.Add(target.parentGroup.fightUnitsSortByHP[i]);
                break;
            case SkillSelect.SelfNearby:
                targets = GetPosNearby(attacker.transform.localPosition, skill.normalSkill.aoeRadius, target.parentGroup);
                break;
            case SkillSelect.All:
                targets = target.parentGroup.fightUnits;
                break;
            case SkillSelect.Foreground:
                targets = GetPosForeground(attacker.transform.localPosition, skill.normalSkill.aoeRadius, target.parentGroup);
                break;
            case SkillSelect.RandomSeveral:
                RandomOneNoRepeat randomNoRepeat = new RandomOneNoRepeat(target.parentGroup.fightUnits.Count);
                int Count1 = Mathf.Min((int)skill.normalSkill.targetNum, target.parentGroup.fightUnits.Count);
                for (int i = 0; i < Count1; i++)
                {
                    int index = randomNoRepeat.RandomOne();
                    targets.Add(target.parentGroup.fightUnits[index]);
                }
                break;
            case SkillSelect.NearbyFriend:
                target = attacker;
                targets = GetTargetNearby(target, skill.normalSkill.aoeRadius);
                break;
            case SkillSelect.AllFriend:
                target = attacker;
                targets = attacker.parentGroup.fightUnits;
                break;
            case SkillSelect.WeakestSeveralFriend:
                target = attacker;
                int Count2 = Mathf.Min((int)skill.normalSkill.targetNum, target.parentGroup.fightUnits.Count);
                target.parentGroup.ResortByHP();
                for (int i = 0; i < Count2; i++)
                    targets.Add(target.parentGroup.fightUnitsSortByHP[i]);
                break;
            case SkillSelect.Self:
                target = attacker;
                targets.Add(target);
                break;
            case SkillSelect.Front:
                target = attacker.parentGroup.FirstUnit;
                targets.Add(target);
                break;
            default:
                break;
        }
        for (int i = 0; i < targets.Count; i++)
        {           
            //选定目标的魔法对每个作用单位都播放特效
            if (skill.dmgEffect != null && (skill.dmgEffect.target == 1 || skill.dmgEffect.target == 2))
            {
                skill.StartCoroutine(AcitveSkillMagic_Single(attacker, targets[i], skill));
            }
            else
            {
                if (skill.normalSkill.attackType == (int)SkillType.Hurt)
                    ActiveSkillDamage(attacker, targets[i], skill);
                else
                    FightCure(attacker, targets[i], skill);  
            }
        }
    }

    /// <summary>
    /// 选定目标的魔法
    /// </summary>
    public static IEnumerator AcitveSkillMagic_Single(FightUnit attacker,FightUnit target,ActiveSkill skill)
    {
        skill.StartCoroutine(skill.DisplaydmgEffect(target));
        if (skill.magicDelay.Length > 0)
        {           
            for (int i = 0; i < skill.magicDelay.Length; i++)
            {
                yield return new WaitForSeconds(skill.magicDelay[i]);
                if (skill.normalSkill.attackType == (int)SkillType.Hurt)
                    ActiveSkillDamage(attacker, target, skill);
                else
                    FightCure(attacker, target, skill);
            }
        }
        else
        {
            if (skill.normalSkill.attackType == (int)SkillType.Hurt)
                ActiveSkillDamage(attacker, target, skill);
            else
                FightCure(attacker, target, skill);  
        }
    }

#endregion

#region 绝技

    /// <summary>
    /// 绝技的入口函数，涉及到有弹道，回调
    /// </summary>
    public static void UniqueSkillWork(FightUnit attacker, FightUnit target, UniqueSkill skill)
    {
        switch ((SkillCurve)skill.specialSkill.curve)
        {
            case SkillCurve.None:
            case SkillCurve.Melee:
                UniqueSkillSelect(attacker, target, skill);
                break;
            case SkillCurve.Directional:
                GameObject obj = ProjectileMove.CreateProjectile(skill.name + "_" + attacker.mTrans.name, attacker.mTrans.parent, skill.flyEffectFromPos, skill.flyEffect.speed, target.Body, delegate(SpecialEffect fx)
                {
                    UniqueSkillSelect(attacker, target, skill);
                });
                skill.StartCoroutine(skill.DisplayFlyEffect(obj.transform));
                break;
            case SkillCurve.Parabola:
                Vector3 targetPos;
                bool isHeroSkill = attacker.parentGroup.group == FightGroup.GroupType.Mine;
                if (skill.flyEffect.distance > 0)
                    targetPos = attacker.mTrans.localPosition + (isHeroSkill ? -1 : 1) * Vector3.right * skill.flyEffect.distance;
                else
                    targetPos = target.mTrans.localPosition + target.Body.localPosition;
                GameObject obj_1 = ProjectileMove.CreateProjectile(skill.name, attacker.mTrans.parent, skill.flyEffectFromPos, skill.flyEffect.speed, skill.flyEffect.height, targetPos, delegate(SpecialEffect fx)
                {
                    UniqueSkillSelect(attacker, target, skill);
                });
                if (!isHeroSkill)
                    obj_1.AddComponent<DragRecord>();
                skill.StartCoroutine(skill.DisplayFlyEffect(obj_1.transform));
                break;
            case SkillCurve.Magica:
                if (skill.dmgEffect == null)
                    return;
                if (skill.dmgEffect.target == 4 || skill.dmgEffect.target == 5)
                    skill.StartCoroutine(UniqueSkillMagic_Whole(attacker, target, skill));
                else
                    UniqueSkillSelect(attacker, target, skill);
                break;
            case SkillCurve.LineAoe:
                bool isRight = (attacker.parentGroup.group == FightGroup.GroupType.Mine && !attacker.isConfused) ||
                    (attacker.parentGroup.group == FightGroup.GroupType.Enemy && attacker.isConfused);
                GameObject obj_2 = LineAoeMove.CreateLineAoe(skill.name + "_" + attacker.mTrans.name, attacker.mTrans.parent, skill.flyEffectFromPos, skill.flyEffect.speed, isRight, delegate(FightUnit unit)
                {
                    UniqueSkillDamage(attacker,unit,skill);
                });
                skill.StartCoroutine(skill.DisplayFlyEffect(obj_2.transform));
                break;
        }
    }

    /// <summary>
    /// 非选定目标的魔法播放过程中再进行目标选取
    /// </summary>
    public static IEnumerator UniqueSkillMagic_Whole(FightUnit attacker, FightUnit target, UniqueSkill skill)
    {
        skill.StartCoroutine(skill.DisplaydmgEffect(null));
        if (skill.magicDelay.Length > 0)
        {
            for (int i = 0; i < skill.magicDelay.Length; i++)
            {
                yield return new WaitForSeconds(skill.magicDelay[i]);
                UniqueSkillSelect(attacker, target, skill);
            }
        }
        else
            UniqueSkillSelect(attacker, target, skill);
    }

    /// <summary>
    /// 绝技技能目标选取
    /// </summary>
    public static void UniqueSkillSelect(FightUnit attacker, FightUnit target, UniqueSkill skill)
    {
        List<FightUnit> targets = new List<FightUnit>();
        switch ((SkillSelect)skill.specialSkill.selectRule)
        {
            case SkillSelect.None:
            case SkillSelect.NowTarget:
                targets.Add(target);
                break;
            case SkillSelect.LongAway:
                target = target.parentGroup.LastUnit;
                targets.Add(target);
                break;
            case SkillSelect.Weakest:
                target = target.parentGroup.WeakestUnit;
                targets.Add(target);
                break;
            case SkillSelect.Random:
                target = target.parentGroup.fightUnits[Random.Range(0, target.parentGroup.fightUnits.Count)];
                targets.Add(target);
                break;
            case SkillSelect.NowTargetNearby:
                targets = GetTargetNearby(target, skill.specialSkill.aoeRadius);
                break;
            case SkillSelect.LongAwayNearby:
                target = target.parentGroup.LastUnit;
                targets = GetTargetNearby(target, skill.specialSkill.aoeRadius);
                break;
            case SkillSelect.WeakestSeveral:
                int Count0 = Mathf.Min((int)skill.specialSkill.targetNum, target.parentGroup.fightUnits.Count);
                target.parentGroup.ResortByHP();
                for (int i = 0; i < Count0; i++)
                    targets.Add(target.parentGroup.fightUnitsSortByHP[i]);
                break;
            case SkillSelect.SelfNearby:
                targets = GetPosNearby(attacker.transform.localPosition, skill.specialSkill.aoeRadius, target.parentGroup);
                break;
            case SkillSelect.All:
                targets = target.parentGroup.fightUnits;
                break;
            case SkillSelect.Foreground:
                targets = GetPosForeground(attacker.transform.localPosition, skill.specialSkill.aoeRadius, target.parentGroup);
                break;
            case SkillSelect.RandomSeveral:
                RandomOneNoRepeat randomNoRepeat = new RandomOneNoRepeat(target.parentGroup.fightUnits.Count);
                int Count1 = Mathf.Min((int)skill.specialSkill.targetNum, target.parentGroup.fightUnits.Count);
                for (int i = 0; i < Count1; i++)
                {
                    int index = randomNoRepeat.RandomOne();
                    targets.Add(target.parentGroup.fightUnits[index]);
                }
                break;
            case SkillSelect.NearbyFriend:
                target = attacker;
                targets = GetTargetNearby(target, skill.specialSkill.aoeRadius);
                break;
            case SkillSelect.AllFriend:
                target = attacker;
                targets = attacker.parentGroup.fightUnits;
                break;
            case SkillSelect.WeakestSeveralFriend:
                target = attacker;
                int Count2 = Mathf.Min((int)skill.specialSkill.targetNum, target.parentGroup.fightUnits.Count);
                target.parentGroup.ResortByHP();
                for (int i = 0; i < Count2; i++)
                    targets.Add(target.parentGroup.fightUnitsSortByHP[i]);
                break;
            case SkillSelect.Self:
                target = attacker;
                targets.Add(target);
                break;
            case SkillSelect.Front:
                target = attacker.parentGroup.FirstUnit;
                targets.Add(target);
                break;
            default:
                break;
        }
        for (int i = 0; i < targets.Count; i++)
        {
            //选定目标的魔法对每个作用单位都播放特效
            if (skill.dmgEffect != null && (skill.dmgEffect.target == 1 || skill.dmgEffect.target == 2))
            {
                skill.StartCoroutine(UniqueSkillMagic_Single(attacker, targets[i], skill));
            }
            else
            {
                if (skill.specialSkill.attackType == (int)SkillType.Hurt)
                    UniqueSkillDamage(attacker, targets[i], skill);
                else
                    UniqueFightCure(attacker, targets[i], skill);
            }
        }
    }

    /// <summary>
    /// 选定目标的魔法
    /// </summary>
    public static IEnumerator UniqueSkillMagic_Single(FightUnit attacker, FightUnit target, UniqueSkill skill)
    {
        skill.StartCoroutine(skill.DisplaydmgEffect(target));
        if (skill.magicDelay.Length > 0)
        {
            for (int i = 0; i < skill.magicDelay.Length; i++)
            {
                yield return new WaitForSeconds(skill.magicDelay[i]);
                if (skill.specialSkill.attackType == (int)SkillType.Hurt)
                    UniqueSkillDamage(attacker, target, skill);
                else
                    UniqueFightCure(attacker, target, skill);
            }
        }
        else
        {
            if (skill.specialSkill.attackType == (int)SkillType.Hurt)
                UniqueSkillDamage(attacker, target, skill);
            else
                UniqueFightCure(attacker, target, skill);
        }
    }

#endregion

#region 普通攻击

    /// <summary>
    /// 普通攻击的入口函数
    /// </summary>
    public static void NormalAttackWork(FightUnit attacker, FightUnit target, NormalAttack skill)
    {
        if (target == null || target.health == 0 || attacker == null || attacker.health == 0)
            return;
        switch ((SkillCurve)skill.attack.curve)
        {
            case SkillCurve.None:
            case SkillCurve.Melee:
                    NormalAttackDamage(attacker, target, skill,skill.curHitEffect);
                break;
            case SkillCurve.Directional:
                GameObject obj = ProjectileMove.CreateProjectile(skill.name + "_" + attacker.mTrans.name, attacker.mTrans.parent, skill.flyEffectFromPos, skill.curFlyEffect.speed, target.Body, delegate(SpecialEffect fx)
                {
                    if (attacker != null && target != null)
                        NormalAttackDamage(attacker, target, skill,fx);
                });
                obj.GetComponent<ProjectileMove>().hitEffect = skill.curHitEffect;
                skill.StartCoroutine(skill.DisplayFlyEffect(obj.transform));
                break;
            case SkillCurve.Parabola:
                Vector3 targetPos;
                bool isHeroSkill = attacker.parentGroup.group == FightGroup.GroupType.Mine;
                if (skill.flyEffect.distance > 0)
                    targetPos = attacker.mTrans.localPosition + (isHeroSkill? -1 : 1) * Vector3.right * skill.flyEffect.distance;
                else
                    targetPos = target.mTrans.localPosition + target.Body.localPosition;
                GameObject obj_1 = ProjectileMove.CreateProjectile(skill.name, attacker.mTrans.parent, skill.flyEffectFromPos, skill.curFlyEffect.speed, skill.curFlyEffect.height, targetPos, delegate(SpecialEffect fx)
                {
                    if(attacker != null && target != null)
                        NormalAttackDamage(attacker, target, skill,fx);
                });
                obj_1.GetComponent<ProjectileMove>().hitEffect = skill.curHitEffect;
                if(!isHeroSkill)
                    obj_1.AddComponent<DragRecord>();
                break;
        }
    }

#endregion

#region 技能额外效果

    /// <summary>
    /// 技能附加buff
    /// </summary>
    public static void SkillAttachBuff(FightUnit attacker, FightUnit target, List<Skill.buffConfig> buffConfigList,HurtType hurtType)
    {
        if (buffConfigList.Count == 0)
            return;
        for (int i = 0; i < buffConfigList.Count; i++)
        {
            ActiveSkill.buffConfig buffConfig = buffConfigList[i];
            BuffData buffData = BuffDataDic[buffConfig.id];
            //判断免疫
            if (!IsBuffAttachable(target, buffData))
                continue;
            //判断控制buff的命中
            if (buffData.controlDebuff == 1 && Random.value > 1 - Mathf.Max(0, (target.fightAttribute.level - buffConfig.level)) * Const.CONST_RESIST_FACTOR)
                continue;
            float buffValue = Mathf.RoundToInt(attacker.fightAttribute.attack * buffConfig.percent) + buffConfig.value;
            Buff buff = Buff.AddNewBuff(target, attacker, buffData, buffValue, buffConfig.time, hurtType);
            buff.bindSkill = buffConfig.bindSkill;
        }
    }

    /// <summary>
    /// 判断buff是否被目标免疫
    /// </summary>
    public static bool IsBuffAttachable(FightUnit target, BuffData buffData)
    {
        bool result = true;
        for(int i = target.OnBuffAttachList.Count - 1 ; i >= 0;i--)
        {
            if (target.OnBuffAttachList[i] != null)
                result = result && target.OnBuffAttachList[i](buffData);
        }
        return result;
    }

    /// <summary>
    /// 技能附加效果
    /// </summary>
    public static void AttachExtension(FightUnit attacker, FightUnit target, int hurtNum, SkillAttach attachType, JsonData param)
    {
        if (attachType == SkillAttach.None)
            return;
        switch (attachType)
        {
            case SkillAttach.Beatback:
                Beatback(attacker, target, int.Parse(param[0].ToString()),float.Parse(param[1].ToString()));
                break;
            case SkillAttach.ManaRecover:
                target.mana += int.Parse(param[0].ToString());
                /* -- 特效 -- */
                break;
            case SkillAttach.EnegyRecover:
                FightEnergy.instance.EnergyVal += int.Parse(param[0].ToString());
                /* -- 特效 -- */
                break;
            case SkillAttach.Taunt:
                target.attack.StopCurrent();
                target.targetUnit = attacker;
                /* -- 特效 -- */
                break;
            case SkillAttach.Dispel:
                target.ClearAllDebuff();
                /* -- 特效 -- */
                break;
            case SkillAttach.CallTotem:
                /* -- 图腾功能 -- */
                break;
            case SkillAttach.GetBlood:
                float rate = float.Parse(param[0].ToString());
                XixueCure(Mathf.RoundToInt(hurtNum * rate),attacker);
                /* -- 特效 -- */
                break;
            case SkillAttach.LoseMana:
                int manalose = int.Parse(param[0].ToString());
                target.mana -= manalose;
                /* -- 特效 -- */
                break;
        }
    }

    /// <summary>
    /// 击退
    /// </summary>
    public static void Beatback(FightUnit attacker, FightUnit target, int distance,float time)
    {
        //击退不叠加
        if (!target.enabled)
            return;
        Vector3 direction = target.mTrans.localPosition - attacker.mTrans.localPosition;
        direction.y = 0;
        direction.z = 0;
        direction = direction.normalized;
        target.attack.StopCurrent();
        target.enabled = false;
        HOTween.To(target.mTrans, time, new TweenParms()
            .Prop("position", target.mTrans.parent.TransformPoint(target.mTrans.localPosition + direction * distance), false)
            .Ease(EaseType.EaseOutSine)
            .OnComplete(delegate() { target.enabled = true; }));
    }

#endregion

#region 辅助函数
    /// <summary>
    /// 取得目标附近单位（与目标相同阵营）
    /// </summary>
    public static List<FightUnit> GetTargetNearby(FightUnit target, float range)
    {
        List<FightUnit> targets = new List<FightUnit>();
        if (target == null || target.state == FightUnit.UnitState.Dead)
            return targets;
        for (int i = 0; i < target.parentGroup.fightUnits.Count; i++)
        {
            if (target.parentGroup.fightUnits[i] == null || target.parentGroup.fightUnits[i].state == FightUnit.UnitState.Dead)
                continue;
            if (Util.Distance(target.transform.localPosition, target.parentGroup.fightUnits[i].transform.localPosition) <= range)
                targets.Add(target.parentGroup.fightUnits[i]);
        }
        return targets;
    }

    /// <summary>
    /// 取得某个坐标附近的单位（指定阵营或全阵营）
    /// </summary>
    public static List<FightUnit> GetPosNearby(Vector3 pos, float range, FightGroup targetGroup)
    {
        List<FightUnit> targets = new List<FightUnit>();
        if (targetGroup != null)
        {
            for (int i = 0; i < targetGroup.fightUnits.Count; i++)
            {
                if (Util.Distance(pos, targetGroup.fightUnits[i].transform.localPosition) <= range)
                    targets.Add(targetGroup.fightUnits[i]);
            }
        }
        else
        {
            for (int i = 0; i < FightManager.GetInstance().mineGroup.fightUnits.Count; i++)
            {
                if (Util.Distance(pos, FightManager.GetInstance().mineGroup.fightUnits[i].transform.localPosition) <= range)
                    targets.Add(FightManager.GetInstance().mineGroup.fightUnits[i]);
            }
            for (int i = 0; i < FightManager.GetInstance().enemyGroup.fightUnits.Count; i++)
            {
                if (Util.Distance(pos, FightManager.GetInstance().enemyGroup.fightUnits[i].transform.localPosition) <= range)
                    targets.Add(FightManager.GetInstance().enemyGroup.fightUnits[i]);
            }
        }
        return targets;
    }

    /// <summary>
    /// 取得向前一段距离内的目标（需指定起点和阵营）
    /// </summary>
    public static List<FightUnit> GetPosForeground(Vector3 pos, float range, FightGroup targetGroup)
    {
        bool isRight = targetGroup.group == FightGroup.GroupType.Enemy;
        List<FightUnit> targets = new List<FightUnit>();
        for (int i = 0; i < targetGroup.fightUnits.Count; i++)
        {
            if ((isRight && targetGroup.fightUnits[i].transform.localPosition.x - pos.x <= range)
                || (!isRight && pos.x - targetGroup.fightUnits[i].transform.localPosition.x <= range))
                targets.Add(targetGroup.fightUnits[i]);
        }
        return targets;
    }

    /// <summary>
    /// 获取攻击目标
    /// </summary>
    public static FightUnit GetFightTarget(FightUnit self, FightGroup targetGroup )
    {
        FightUnit target = null;
		for (int i = 0; i < targetGroup.fightUnits.Count; i++)
		{
			if (targetGroup.fightUnits[i] == self)
				continue; // does not find self
			//在攻击距离内的才进入目标列表
			if (target == null &&  Distance (self, targetGroup.fightUnits [i]) <= self.fightAttribute.range  ) 
			{
				target = targetGroup.fightUnits[i];
				continue;
			}
			//更换目标
			if (target != null && Distance(self, targetGroup.fightUnits[i]) < Distance(self, target))
				target = targetGroup.fightUnits[i];
		}
		return target;
    }

	public static FightUnit GetAlarmedFightTarget(FightUnit self, FightGroup targetGroup )
	{
		FightUnit target = null;
		for (int i = 0; i < targetGroup.fightUnits.Count; i++)
		{
			if (targetGroup.fightUnits[i] == self)
				continue; // does not find self
			if (target == null &&  Distance (self, targetGroup.fightUnits [i]) <= self.fightAttribute.alarmRange  ) 
			{
//				Debug.Log (Distance (self, targetGroup.fightUnits [i]) + "###############");
				target = targetGroup.fightUnits[i];
				continue;
			}
			if (target != null && Distance(self, targetGroup.fightUnits[i]) < Distance(self, target))
				target = targetGroup.fightUnits[i];
		}
		return target;
	}

    /// <summary>
    /// 算出修正距离
    /// </summary>
    public static float Distance(FightUnit from, FightUnit to)
    {
        float factor = DistanceFactorDic[(int)from.fightAttribute.elementType * 10 + (int)to.fightAttribute.elementType].factor; //不同的属性之间攻击距离不同
        //float factor = 1;
        return Util.Distance(from.transform.localPosition, to.transform.localPosition) * factor;
    }

    #endregion

}
