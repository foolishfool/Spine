using UnityEngine;
using System.Collections;

public class ShihuaBuff : Buff {
    public override void BuffStart()
    {
        target.fightAttribute.allDefenseRate += buffValue;
        target.UnAbleFightCount++;
        target.UnAbleMoveCount++;
        target.UnAbleUseSkillCount++;
    }

    public override void BuffEnd()
    {
        target.fightAttribute.allDefenseRate -= buffValue;
        target.UnAbleFightCount--;
        target.UnAbleMoveCount--;
        target.UnAbleUseSkillCount--;
    }
}
