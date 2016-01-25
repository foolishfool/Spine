using UnityEngine;
using System.Collections;

public class YingyongBuff : Buff {
    public override void BuffStart()
    {
        target.fightAttribute.attackSpeed += (int)buffValue;
    }
    public override void BuffEnd()
    {
        target.fightAttribute.attackSpeed -= (int)buffValue;
    }
}
