using UnityEngine;
using System.Collections;

public class ZuzhouBuff : Buff {
    public override void BuffStart()
    {
        target.fightAttribute.cureRate -= buffValue;
    }

    public override void BuffEnd()
    {
        target.fightAttribute.cureRate += buffValue;
    }
}
