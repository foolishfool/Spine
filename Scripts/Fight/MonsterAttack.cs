using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using ProtoTblConfig;
/// <summary>
/// 控制所有攻击产生作用和判断条件
/// </summary>
public class MonsterAttack : Attack
{
    public int normalAttackId;
    public NormalAttack normalAttack;
    public int[] autoSkillsId;
    public List<ActiveSkillStruct> autoSkills = new List<ActiveSkillStruct>();

    public class ActiveSkillStruct
    {
        public ActiveSkill autoSkill;
        public MonsterSkill autoSkillCondition;
        public SkillCondition condition;
        public int conditionVal;
        public float cd;
        public float minCd;
        public float maxCd;
        public float lastUseTime;
        public bool isLocked = true;

        public void GetCd()
        {
            cd = Random.Range(minCd,maxCd);
        }
    }

    MonsterFightUnit self;

    FightUnit target;

    ActiveSkill selectedSkill = null;

    float time;

    public override void Init()
    {
        self = GetComponent<MonsterFightUnit>();
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
        foreach (int normalSkillId in autoSkillsId)
        {
            ActiveSkill normalSkill = NGUITools.AddChild<ActiveSkill>(gameObject);
            normalSkill.normalSkill = Util.GetDic<MsgNormalSkill, NormalSkill>()[normalSkillId];
            normalSkill.dmgEffectId = (int)normalSkill.normalSkill.dmgEffect;
            normalSkill.castEffectId = (int)normalSkill.normalSkill.castEffect;
            normalSkill.flyEffectId = (int)normalSkill.normalSkill.flyEffect;
            normalSkill.hitEffectId = (int)normalSkill.normalSkill.hitEffect;
            normalSkill.mineUnit = self;
            int LvUpId = normalSkillId * 100 + 1;
            normalSkill.normalSkillLvUp = Util.GetDic<MsgNormalSkillLvUp, NormalSkillLvUp>()[LvUpId];
            normalSkill.Init();
            MonsterSkill skillCondition = null;
            Util.GetDic<MsgMonsterSkill, MonsterSkill>().TryGetValue(normalSkillId,out skillCondition);
            SkillCondition condition = SkillCondition.None;
            int conditionVal = 0;
            float minCd = 0;
            float maxCd = 0;
            if(skillCondition != null)
            {
                JsonData dt = JsonMapper.ToObject(Util.GetConfigString(skillCondition.condition));
                if (dt.Count == 2)
                {
                    condition = (SkillCondition)int.Parse(dt[0].ToString());
                    conditionVal = int.Parse(dt[1].ToString());
                }
                JsonData dt_1 = JsonMapper.ToObject(Util.GetConfigString(skillCondition.interval));
                minCd = float.Parse(dt_1[0].ToString());
                maxCd = float.Parse(dt_1[1].ToString());
            }
            ActiveSkillStruct activeSkillStruct = new ActiveSkillStruct();
            activeSkillStruct.autoSkill = normalSkill;
            activeSkillStruct.autoSkillCondition = skillCondition;
            activeSkillStruct.condition = condition;
            activeSkillStruct.conditionVal = conditionVal;
            activeSkillStruct.minCd = minCd;
            activeSkillStruct.maxCd = maxCd;
            autoSkills.Add(activeSkillStruct);
        }
        enabled = false;
    }

    void OnDestroy()
    {
        if(self != null)
            self.OnNoramlAttackFinish -= ThinkUseSkill;
    }

    /// <summary>
    /// 进入战斗
    /// </summary>
    public override void DoAttack(FightUnit t)
    {
        //进入战斗才开始解锁技能
        if (!enabled)
            enabled = true;
        if (target != t)
            target = t;
        if (currentSkill != null)
            return;
        if (selectedSkill != null)
        {
			Debug.Log (selectedSkill.flyEffectName + "!!!!!!!!!!!!");
			UseActiveSkill(selectedSkill);
            selectedSkill = null;
        }
        else
            UseNormalAttack();
    }
    /// <summary>
    /// 终止当前的技能
    /// </summary>
    public override void StopCurrent()
    {
        if (currentSkill != null && currentSkill.state != Skill.SkillState.None)
        {
            currentSkill.Stop();
            currentSkill = null;            
        }
    }
    /// <summary>
    /// 结束战斗
    /// </summary>
    public override void EndAttack()
    {

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
        if (self.beAbleUseSkill && skill.cdValue == 0 && self.health > skill.normalSkillLvUp.HPCost)
        {
            StopCurrent();
            skill.Use(target);
            currentSkill = skill;
            return true;
        }       
        return false;
    }

    /// <summary>
    /// 尝试使用绝技
    /// </summary>
    /// <returns></returns>
    public override bool UseUniqueSkill()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 每次普通攻击结束后思考一次是否使用技能
    /// </summary>
    public void ThinkUseSkill(object[] objs)
    {
        if (target == null || target.state == FightUnit.UnitState.Dead)
        {
            target = FightRule.GetFightTarget(self, self.isConfused ? self.parentGroup : self.parentGroup.targetGroup);
            if (target == null)
                return;
        }
        List<int> indexList = new List<int>();
        uint totalWeight = 0;
        float randomVal = Random.value;
        
        for (int i = 0; i < autoSkills.Count; i++)
        {
            ActiveSkillStruct skill = autoSkills[i];
            if (skill.isLocked)
            {
                if (skill.autoSkillCondition.unlockTime <= time)
                    skill.isLocked = false;
            }
            if (!skill.isLocked && skill.cd + skill.lastUseTime <= time && IsFitSkillCondition(skill))
            {
                totalWeight += skill.autoSkillCondition.weight;
                indexList.Add(i);
            }
        }
        for (int i = 0; i < indexList.Count; i++)
        {
            float val =(float)autoSkills[i].autoSkillCondition.weight / totalWeight;
            if (randomVal <= val)
            {
                selectedSkill = autoSkills[i].autoSkill;
                autoSkills[i].lastUseTime = time;
                autoSkills[i].GetCd();
                break;
            }
            else
                randomVal -= val;
        }
    }
    /// <summary>
    /// 判断是否满足技能AI条件
    /// </summary>
    /// <param name="skillStruct"></param>
    /// <returns></returns>
    bool IsFitSkillCondition(ActiveSkillStruct skillStruct)
    {
        switch (skillStruct.condition)
        {
            case SkillCondition.None:
                return true;
            case SkillCondition.HPLower:
                return self.healthValue * 100 <= skillStruct.conditionVal;
            case SkillCondition.HPHigher:
                return self.healthValue * 100 > skillStruct.conditionVal;
            case SkillCondition.FirendHPLower:
                return self.parentGroup.WeakestUnit.healthValue <= skillStruct.conditionVal;
            default:
                return true;
        }
    }

    public override void Update()
    {
        base.Update();
        time += Time.deltaTime;
    }
}
