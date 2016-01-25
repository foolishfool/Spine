using UnityEngine;
using System.Collections;

public class FightEnergy : MonoBehaviour {
    public static FightEnergy instance;

    /// <summary>
    /// 是否能量足够使用绝技
    /// </summary>
    public bool isEnergyEnough = false;
    /// <summary>
    /// 能量值
    /// </summary>
    int _EnergyVal;
    public int EnergyVal
    {
        get { return _EnergyVal; }
        set
        {
            value = Mathf.Clamp(value, 0, Const.CONST_MAX_ENERGY);
            if (value == 0)
            {
                _EnergyVal = value;
                isEnergyEnough = false;
                CEventDispatcher.GetInstance().DispatchEvent(new CBaseEvent(CEventType.ENERGY_EMPTY, null));
            }
            if (_EnergyVal < Const.CONST_MIN_ENERGY_LIMIT && value >= Const.CONST_MIN_ENERGY_LIMIT)
            {
                _EnergyVal = value;
                isEnergyEnough = true;
                CEventDispatcher.GetInstance().DispatchEvent(new CBaseEvent(CEventType.ENERGY_ENOUGH, null));
            }
            _EnergyVal = value;
        }
    }
    /// <summary>
    /// 能量消耗值
    /// </summary>
    int _EnergyUseVal;
    public int EnergyUseVal
    {
        get { return _EnergyUseVal; }
        set
        {
            if (value < 0) value = 0;
            _EnergyUseVal = value;
        }
    }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //Const.CONST_ENERGY_RECOVER_TIME = 1;
        InvokeRepeating("AddEnergy", Const.CONST_ENERGY_RECOVER_TIME, Const.CONST_ENERGY_RECOVER_TIME);
        InvokeRepeating("UseEnergy", 1, 1);
        CEventDispatcher.GetInstance().AddEventListener(CEventType.ENERGY_ENOUGH, EnergyEnoughEffect);
    }

    void OnDestroy()
    {
        CEventDispatcher.GetInstance().RemoveEventListener(CEventType.ENERGY_ENOUGH, EnergyEnoughEffect);
    }

    void AddEnergy()
    {
        if (EnergyUseVal == 0)
            EnergyVal += Const.CONST_ENERGY_RECOVER_VAL;
    }

    void UseEnergy()
    {
        if (EnergyUseVal > 0 && EnergyVal > 0)
            EnergyVal -= EnergyUseVal;
    }

    /// <summary>
    /// 展现能量足够的特效
    /// </summary>
    /// <param name="evt"></param>
    void EnergyEnoughEffect(CBaseEvent evt)
    {

    }
}
