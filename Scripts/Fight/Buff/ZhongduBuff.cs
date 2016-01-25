using UnityEngine;
using System.Collections;

public class ZhongduBuff : Buff {
    public override void BuffProcess()
    {
        FightRule.BuffAttackDamage(this,HurtType.True);
    }
}
