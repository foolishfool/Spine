using UnityEngine;
using System.Collections;

public class JiansuBuff : Buff {
    public override void BuffStart()
    {
        target.fightAttribute.moveSpeedBuff -= buffValue;
    }

    public override void BuffEnd()
    {
        target.fightAttribute.moveSpeedBuff += buffValue;
    }
}
