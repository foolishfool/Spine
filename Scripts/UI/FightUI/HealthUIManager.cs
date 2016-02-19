using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealthUIManager : MonoBehaviour {
    public static HealthUIManager instance;
    public Dictionary<FightUnit, HealthUI> HealthUIList = new Dictionary<FightUnit, HealthUI>();
    public GameObject myHealthBarObj;
    public GameObject enHealthBarObj;
    public GameObject hurtNumEmitter;


    public struct HealthUI
    {
        public UISlider slider;
        public DisplayHealthBar displayHealthBar;
        public Transform point;
        public Transform barTrans;
        public HurtNumEmitter emitter;
    }
    Transform mTrans;
    void Awake()
    {
        instance = this;
        mTrans = transform;
    }
    //防止UI的覆盖错乱
    int depth = 0;
    public void ResetDepth()
    {
        depth = 0;
    }


    public void AddNewHealthBar(FightUnit unit, Transform point,bool isEnemey)
    {
        if (HealthUIList.ContainsKey(unit))
            return;
        HealthUI healthUI = new HealthUI();
        healthUI.point = point;
        GameObject obj = Util.AddChild(isEnemey? enHealthBarObj:myHealthBarObj, mTrans);
        obj.SetActive(true);
        healthUI.displayHealthBar = obj.GetComponent<DisplayHealthBar>();
        UISprite background = obj.transform.GetChild(0).GetComponent<UISprite>();
        UISprite foreground = obj.transform.GetChild(0).GetChild(0).GetComponent<UISprite>();
        //写死
		background.depth = 4;
        foreground.depth = 5;
        GameObject obj2 = Util.AddChild(hurtNumEmitter, mTrans);
        obj2.SetActive(true);
        healthUI.barTrans = obj.transform;
        UISlider slider = obj.GetComponentInChildren<UISlider>();
        healthUI.slider = slider;
        HurtNumEmitter emitter = obj2.GetComponent<HurtNumEmitter>();
        healthUI.emitter = emitter;
        HealthUIList.Add(unit, healthUI);
    }

    public void RemoveHealthBar(FightUnit unit)
    {
        if (HealthUIList.ContainsKey(unit))
        {
            Destroy(HealthUIList[unit].barTrans.gameObject);
            HealthUIList[unit].emitter.TryDestroy();
            HealthUIList.Remove(unit);
        }
    }

    public void DisplayHurtNum(FightUnit unit, string num,bool isHurt,bool isCrit)
    {
        if (HealthUIList.ContainsKey(unit))
        {
            HealthUIList[unit].displayHealthBar.GetHurtDisplay();
            HealthUIList[unit].emitter.AddHurtNum(num,isHurt,isCrit);
        }
    }

    void Update()
    {
		foreach (FightUnit unit in HealthUIList.Keys)
		{
			HealthUI value = HealthUIList [unit];
			value.barTrans.position = value.point.position;
			value.slider.value = unit.healthValue;
			value.emitter.transform.position = value.point.position;
			if (value.slider.value <= 0.2)
				value.slider.foregroundWidget.color = Color.red;
			else
				value.slider.foregroundWidget.color = new Color(1f,1f,1f,1f);
		}
    }
}
