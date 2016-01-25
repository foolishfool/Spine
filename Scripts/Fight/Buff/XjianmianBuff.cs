using UnityEngine;
using System.Collections;

public class XjianmianBuff : Buff {
    public override void BuffStart()
    {
        switch(hurtType)
        {
            case HurtType.None:
            case HurtType.True:
                target.fightAttribute.allDefenseRate += buffValue;
                break;
            case HurtType.Dark:
                target.fightAttribute.darkDefenseRate += buffValue;
                break;
            case HurtType.Fire:
                target.fightAttribute.fireDefenseRate += buffValue;
                break;
            case HurtType.Light:
                target.fightAttribute.lightDefeneseRate += buffValue;
                break;
            case HurtType.Water:
                target.fightAttribute.waterDefenseRate += buffValue;
                break;
            case HurtType.Earth:
                target.fightAttribute.eathDefenseRate += buffValue;
                break;
        }
    }
    public override void BuffEnd()
    {
        switch (hurtType)
        {
            case HurtType.None:
            case HurtType.True:
                target.fightAttribute.allDefenseRate -= buffValue;
                break;
            case HurtType.Dark:
                target.fightAttribute.darkDefenseRate -= buffValue;
                break;
            case HurtType.Fire:
                target.fightAttribute.fireDefenseRate -= buffValue;
                break;
            case HurtType.Light:
                target.fightAttribute.lightDefeneseRate -= buffValue;
                break;
            case HurtType.Water:
                target.fightAttribute.waterDefenseRate -= buffValue;
                break;
            case HurtType.Earth:
                target.fightAttribute.eathDefenseRate -= buffValue;
                break;
        }
    }
}
