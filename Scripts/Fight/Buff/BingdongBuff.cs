using UnityEngine;
using System.Collections;

public class BingdongBuff : Buff {
    public override void BuffStart()
    {
        target.fightAttribute.defence -= (int)buffValue;
        target.UnAbleFightCount++;
        target.UnAbleMoveCount++;
        target.UnAbleUseSkillCount++;
    }

    public override void BuffEnd()
    {
        target.fightAttribute.defence += (int)buffValue;
        target.UnAbleFightCount--;
        target.UnAbleMoveCount--;
        target.UnAbleUseSkillCount--;
    }
}
