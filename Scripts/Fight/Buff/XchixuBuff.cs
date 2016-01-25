using UnityEngine;
using System.Collections;

public class XchixuBuff : Buff {
    public override void BuffProcess()
    {
        FightRule.BuffAttackDamage(this,hurtType);
    }	
}
