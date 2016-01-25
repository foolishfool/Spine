using UnityEngine;
using System.Collections;

public class MeihuoBuff : Buff {
    public override void BuffStart()
    {
        target.EnticeStateCount++;
    }

    public override void BuffEnd()
    {
        target.EnticeStateCount--;
    }
}
