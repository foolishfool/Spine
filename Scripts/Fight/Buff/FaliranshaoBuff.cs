using UnityEngine;
using System.Collections;

public class FaliranshaoBuff : Buff {
    public override void BuffProcess()
    {
        target.mana -= (int)buffValue;
    }
	
}
