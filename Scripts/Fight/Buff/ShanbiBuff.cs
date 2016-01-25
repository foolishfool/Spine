using UnityEngine;
using System.Collections;

public class ShanbiBuff : Buff {
    public override void BuffStart()
    {
        target.fightAttribute.dodgeRateBuff += buffValue;
    }
    public override void BuffEnd()
    {
        target.fightAttribute.dodgeRateBuff -= buffValue;
    }
}
