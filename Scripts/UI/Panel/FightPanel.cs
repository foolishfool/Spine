using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 战斗场景中的主UI
/// </summary>
public class FightPanel: BaseView {

    public override void Regester()
    {
        ViewMapper<FightPanel>.instance = this;
    }

    void Start()
    {
        BindAutoFight();
    }

    void Update()
    {
        UpdateHeadShot();
        UpdateEnergy();
    }

#region 头像
    public Dictionary<HeroFightUnit, HeadShotWidget> HeadShotMapper = new Dictionary<HeroFightUnit, HeadShotWidget>();
    /// <summary>
    /// 绑定英雄头像控件
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="i"></param>
    public void RegesterHeadShot(HeroFightUnit unit,int i)
    {
        string key = i + HeadShotWidget.nameExtension;
        HeadShotWidget headShot = (HeadShotWidget)widgetsMap[key];
        HeadShotMapper.Add(unit,headShot);
        headShot.HeadshotIcon.spriteName = Util.GetConfigString(unit.heroData.icon);
        headShot.SetShuxing((HurtType)unit.heroData.element);
        HeroAttack attack = unit.GetComponent<HeroAttack>();
        headShot.onClick = attack.UI_UseActiveSkill;
        headShot.onDrag = attack.UI_UseUniqueSkill;
        headShot.gameObject.SetActive(true);
    }
    /// <summary>
    /// 替换头像控件绑定
    /// </summary>
    /// <param name="oldHero"></param>
    /// <param name="newHero"></param>
    public void ReplaceHeadShot(HeroFightUnit oldHero,HeroFightUnit newHero)
    {
        HeadShotWidget headShot = HeadShotMapper[oldHero];
        HeadShotMapper.Remove(oldHero);
        HeadShotMapper.Add(newHero,headShot);
        headShot.HeadshotIcon.spriteName = Util.GetConfigString(newHero.heroData.icon);
        headShot.SetShuxing((HurtType)newHero.heroData.element);
        HeroAttack attack = newHero.GetComponent<HeroAttack>();
        headShot.onClick = attack.UI_UseActiveSkill;
        headShot.onDrag = attack.UI_UseUniqueSkill;
    }
    /// <summary>
    /// 头像回退
    /// </summary>
    /// <param name="hero"></param>
    public void BringBackHeadShot(HeroFightUnit hero)
    {
        HeadShotWidget headShot = HeadShotMapper[hero];
        headShot.BringBack();
    }
    /// <summary>
    /// 头像拉上
    /// </summary>
    /// <param name="hero"></param>
    public void BringForewardHeadShot(HeroFightUnit hero)
    {
        HeadShotWidget headShot = HeadShotMapper[hero];
        headShot.BringForeward();
    }


    void UpdateHeadShot()
    {
        foreach (var element in HeadShotMapper)
        {
            HeroFightUnit unit = element.Key;
//			Debug.Log ("!!!!!!!!!!!!" + unit.name);
            HeadShotWidget ui = element.Value;
            ui.ShowHeroState(unit);
        }
    }
#endregion

#region 能量
    void UpdateEnergy()
    {
        FightEnergyWidget ui = (FightEnergyWidget)widgetsMap[FightEnergyWidget.nameExtension];
        ui.Value = (float)FightEnergy.instance.EnergyVal / Const.CONST_MAX_ENERGY;
    }
#endregion

#region Combo连击
    LabelWidget comboLabel;
    LabelWidget ComboLabel
    {
        get
        {
            if(comboLabel == null)
                comboLabel = (LabelWidget)widgetsMap["Combo_Label"];
            return comboLabel;
        }
    }
    public void SetCombo(int combo)
    {
        ComboLabel.Value = "×" + combo;
    }

    public void HideComboLabel()
    {
        TweenAlpha.Begin(ComboLabel.gameObject, 0.2f, 0);
        TweenScale.Begin(ComboLabel.gameObject, 0.2f, Vector3.one);
    }

    public void DisplayComboLabel()
    {
        TweenAlpha.Begin(ComboLabel.gameObject, 0.2f, 1);
        TweenScale.Begin(ComboLabel.gameObject, 0.2f, Vector3.one * 1.2f);
    }
#endregion

#region 自动战斗
    /// <summary>
    /// 绑定自动战斗按钮的事件
    /// </summary>
    void BindAutoFight()
    {
        ButtonWidget button = (ButtonWidget)widgetsMap["AutoFight_Button"];
        button.onClick = OnClick_AutoFight;
    }
    void OnClick_AutoFight(params object[] objs)
    {
        if (FightManager.GetInstance().isAutoFight)
            FightManager.GetInstance().DisableAutoFight();
        else
            FightManager.GetInstance().EnableAutoFight();
    }

#endregion
}
