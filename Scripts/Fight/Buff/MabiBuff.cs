using UnityEngine;
using System.Collections;

public class MabiBuff : Buff {

    public override void BuffStart()
    {
        target.OnWantUseSkill += CheckMabi;
    }

    public override void BuffEnd()
    {
        target.OnWantUseSkill -= CheckMabi;
    }

    bool CheckMabi(params object[] objs)
    {
        if (Random.value <= buffValue)
        {
            /*播放麻痹特效*/
            return false;
        }
        return true;
    }
}
