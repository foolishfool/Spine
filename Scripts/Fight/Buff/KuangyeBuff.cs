using UnityEngine;
using System.Collections;

public class KuangyeBuff : Buff {
    public override void BuffStart()
    {
        target.fightAttribute.criticalRateBuff += buffValue;
    }
    public override void BuffEnd()
    {
        target.fightAttribute.criticalRateBuff -= buffValue;
    }
}
