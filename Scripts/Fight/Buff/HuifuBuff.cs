using UnityEngine;
using System.Collections;

public class HuifuBuff : Buff {
    public override void BuffProcess()
    {
        FightRule.BuffCure((int)buffValue,target);
    }
	
}
