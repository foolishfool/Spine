using UnityEngine;
using System.Collections;

public class HunluanBuff : Buff {
    public override void Start()
    {
        //混乱buff只有50%几率产生作用
        if (Random.value > 0.5f)
        {
            Destroy(gameObject);
            return;
        }
        base.Start();
    }

    public override void BuffStart()
    {
        target.ConfusedStateCount++;
    }

    public override void BuffEnd()
    {
        target.ConfusedStateCount--;
    }
}
