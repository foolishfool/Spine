using UnityEngine;
using System.Collections;
/// <summary>
/// 控制所有攻击产生作用和判断条件
/// </summary>
public abstract class Attack : MonoBehaviour{
    public Skill currentSkill;

    public abstract void Init();

    /// <summary>
    /// 使用技能，进行战斗
    /// </summary>
    public abstract void DoAttack(FightUnit t);

    /// <summary>
    /// 终止当前的技能
    /// </summary>
    public abstract void StopCurrent();

    /// <summary>
    /// 尝试进行普通攻击
    /// </summary>
    /// <returns></returns>
    public abstract bool UseNormalAttack();

    /// <summary>
    /// 尝试使用技能
    /// </summary>
    /// <param name="skill"></param>
    /// <returns></returns>
    public abstract bool UseActiveSkill(ActiveSkill skill);

    /// <summary>
    /// 尝试使用绝技
    /// </summary>
    /// <returns></returns>
    public abstract bool UseUniqueSkill();

    /// <summary>
    /// 结束战斗
    /// </summary>
    public abstract void EndAttack();

    public virtual void Update()
    {
        if (currentSkill != null && currentSkill.state == Skill.SkillState.None)
            currentSkill = null;
    }
	


}
