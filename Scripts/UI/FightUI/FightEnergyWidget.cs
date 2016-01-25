using UnityEngine;
using System.Collections;
/// <summary>
/// 能量控件
/// </summary>
public class FightEnergyWidget : BaseWidget {
    public const string nameExtension = "FightEnergy";

    public Transform zhizheng;

    float percent;
    public float Value
    {
        get
        {
            return percent;
        }
        set
        {           
            value = Mathf.Clamp01(value);
            percent = value;
            zhizheng.localRotation = Quaternion.Euler(0, 0, -360f * percent);
        }
    }

    void Start()
    {
        if (!zhizheng)
            zhizheng = transform.Find("Energy/zhizheng");
    }
}
