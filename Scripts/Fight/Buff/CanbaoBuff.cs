using UnityEngine;
using System.Collections;

public class CanbaoBuff : Buff {
    public override void BuffStart()
    {
        target.fightAttribute.criticalImprove += buffValue;
    }

    public override void BuffEnd()
    {
        target.fightAttribute.criticalImprove -= buffValue;
    }
}
