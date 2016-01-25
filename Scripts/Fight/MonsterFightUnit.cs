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
        base.Update();
    }
}
