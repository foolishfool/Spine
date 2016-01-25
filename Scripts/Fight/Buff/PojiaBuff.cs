using UnityEngine;
using System.Collections;

public class PojiaBuff : Buff {
    public override void BuffStart()
    {
        target.fightAttribute.defenceBuff -= buffValue;
    }
    public override void BuffEnd()
    {
        target.fightAttribute.defenceBuff += buffValue;
    }
}
