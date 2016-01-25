using UnityEngine;
using System.Collections;

public class ZhimangBuff : Buff {
    public override void BuffStart()
    {
        target.fightAttribute.hitRateBuff -= buffValue;
    }
    public override void BuffEnd()
    {
        target.fightAttribute.hitRateBuff += buffValue;
    }
}
