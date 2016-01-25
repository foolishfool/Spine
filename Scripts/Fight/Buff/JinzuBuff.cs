using UnityEngine;
using System.Collections;

public class JinzuBuff : Buff {
    public override void BuffStart()
    {
        target.UnAbleMoveCount++;
    }

    public override void BuffEnd()
    {
        target.UnAbleMoveCount--;
    }
}
