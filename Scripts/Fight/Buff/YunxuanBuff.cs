using UnityEngine;
using System.Collections;

public class YunxuanBuff : Buff {
    public override void BuffStart()
    {
        target.attack.StopCurrent();
        target.UnAbleFightCount++;
        target.UnAbleMoveCount++;
        target.UnAbleUseSkillCount++;
    }

    public override void BuffEnd()
    {
        target.UnAbleFightCount--;
        target.UnAbleMoveCount--;
        target.UnAbleUseSkillCount--;
    }
}
