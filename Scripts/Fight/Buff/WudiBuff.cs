using UnityEngine;
using System.Collections;
using ProtoTblConfig;

public class WudiBuff : Buff {
    public override void BuffStart()
    {
        target.InvincibleStateCount++;
        target.ClearAllDebuff();
    }

    public override void BuffEnd()
    {
        target.InvincibleStateCount--;
    }
}
