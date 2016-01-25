using UnityEngine;
using System.Collections;

public class FalihuifuBuff : Buff {
    public override void BuffProcess()
    {
        target.mana += (int)buffValue;
    }
	
}
