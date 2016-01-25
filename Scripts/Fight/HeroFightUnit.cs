using UnityEngine;
using System.Collections;
using ProtoTblConfig;
/// <summary>
/// 控制英雄的攻击和行动
/// </summary>
[RequireComponent(typeof(HeroAttack))]
public class HeroFightUnit : FightUnit {
    public HeroData heroData;//详细数据
    public HeroEvolution heroEvolution;//进化数据
    public int level;//英雄等级

    /// <summary>
    /// 临时，应由服务器发送
    /// </summary>
    public void InitFightAttribute()
    {
        fightAttribute.range = (int)heroData.range;
        fightAttribute.attackSpeed = (int)heroData.attackSpeed;
        fightAttribute.health = (int)heroData.healthBasic;
        fightAttribute.mana = (int)heroData.mana;
        fightAttribute.attack = (int)heroData.attackBasic;
        fightAttribute.defence = (int)heroData.defenceBasic;
        fightAttribute.critical = (int)heroData.critical;
        fightAttribute.hit = (int)heroData.hit;
        fightAttribute.dodge = (int)heroData.dodge;
        fightAttribute.toughness = (int)heroData.toughness;
        fightAttribute.elementType = (HurtType)heroData.element;  
    }

    public override void Start()
    {
        base.Start();
        Combo.GetInstance().RegesterCombo(this);
    }

    public override void Update()
    {
        base.Update();
    }
}
