using UnityEngine;
using System.Collections;
using ProtoTblConfig;
/// <summary>
/// 控制怪物的攻击和行动
/// </summary>
[RequireComponent(typeof(MonsterAttack))]
public class MonsterFightUnit : FightUnit {
    public MonsterData monsterData;

    public void InitFightAttribute()
    {
        fightAttribute.range = (int)monsterData.range;
        fightAttribute.attackSpeed = (int)monsterData.attackSpeed;
        fightAttribute.health = (int)monsterData.health;
        fightAttribute.attack = (int)monsterData.attack;
        fightAttribute.defence = (int)monsterData.defence;
        fightAttribute.critical = (int)monsterData.critical;
        fightAttribute.hit = (int)monsterData.hit;
        fightAttribute.dodge = (int)monsterData.dodge;
        fightAttribute.toughness = (int)monsterData.toughness;
        fightAttribute.elementType = (HurtType)monsterData.element;
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
		base.Update ();
		if (state == UnitState.Wait && FightRule.GetAlarmedFightTarget (this, this.parentGroup.targetGroup))
			state = UnitState.MoveToTarget;
		if (state == UnitState.MoveToTarget && beAbleMove)
		{
			if (TryAttack ())
			{
				anim.Play (Const.IdleAction, true); //调整动作
				state = UnitState.Fighting; //有目标既进入战斗状态
			} else
			{
				if (targetUnit == null)
					return;
				else
					MoveTowardsTarget ();
			}
		} else if (state == UnitState.Fighting && beAbleFight)
		{
			if (modifiedY != 0 || modifiedZ != 0)
				mTrans.localPosition = Vector3.MoveTowards (mTrans.localPosition, new Vector3 (mTrans.localPosition.x, modifiedY, modifiedZ), Const.MoveSpeed * Time.deltaTime);
			if (TryAttack ())
				attack.DoAttack (targetUnit);
			else
			{
				if (attack.currentSkill != null)
					return;
				if (targetUnit == null)
					state = UnitState.Wait;
				else
					state = UnitState.MoveToTarget;
			}
		}   
    }
}
