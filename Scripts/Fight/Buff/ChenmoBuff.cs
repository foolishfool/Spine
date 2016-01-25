using UnityEngine;
using System.Collections;

public class ChenmoBuff : Buff {
    public override void BuffStart()
    {
        target.UnAbleUseSkillCount++;
    }

    public override void BuffEnd()
    {
        target.UnAbleUseSkillCount--;
    }
}
