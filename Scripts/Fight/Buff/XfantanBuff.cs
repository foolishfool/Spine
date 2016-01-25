using UnityEngine;
using System.Collections;

public class XfantanBuff : Buff {
    public override void BuffStart()
    {
        target.OnFantan += OnFantan;
    }

    public override void BuffEnd()
    {
        target.OnFantan -= OnFantan;
    }

    void OnFantan(params object[] objs)
    {
        int hurtNum = (int)objs[0];
        Skill skill = (Skill)objs[1];
        HurtType damangeType = (HurtType)objs[2];
        if (damangeType != hurtType || skill == null)
            return;
        int fantanNum = Mathf.Max(1, Mathf.RoundToInt(hurtNum * buffValue));
        FightRule.FantanDamage(fantanNum, target, skill.mineUnit);
    }
}
