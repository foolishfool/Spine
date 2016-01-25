using UnityEngine;
using System.Collections;
using ProtoTblConfig;

public class MianyikongzhiBuff : Buff {
    public override void BuffStart()
    {
        target.OnBuffAttachList.Add(OnBuffAttach);
        target.ClearAllDebuff();
    }
    public override void BuffEnd()
    {
        target.OnBuffAttachList.Remove(OnBuffAttach);
    }

    bool OnBuffAttach(params object[] objs)
    {
        BuffData bd = (BuffData)objs[0];
        return bd.controlDebuff != 1;
    }
}
