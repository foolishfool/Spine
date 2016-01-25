using UnityEngine;
using System.Collections;

public class XhudunBuff : Buff {
    public int healthHudun;

    public override void BuffStart()
    {
        healthHudun = (int)buffValue;
        target.OnHudunList.Add(OnHudun);
    }

    public override void BuffEnd()
    {
        target.OnHudunList.Remove(OnHudun);
    }

    int OnHudun(params object[] objs)
    {
        int hurtNum = (int)objs[0];
        HurtType damageType = (HurtType)objs[1];
        if (damageType != hurtType || hurtNum == 0)
            return hurtNum;
        healthHudun -= hurtNum;
        if (healthHudun <= 0)
        {
            Clear();
            return -healthHudun;
        }
        else
            return 0;
    }
}
