using UnityEngine;
using System.Collections;
/// <summary>
/// 英雄头像UI
/// </summary>
public class HeadShotWidget : BaseWidget {
    public const string nameExtension = "_HeadShot";
    public UIProgressBar healthBar;//血条
    public UIProgressBar manaBar;//蓝条
	//public UIProgressBar energybar;// 怒气条
    public UISprite HeadshotIcon;//头像
    public UISprite[] buffIcons;//buff图标
    public UILabel cdlabel;//cd
    public UISprite shuxingIcon;//属性类型
    public TweenPosition tw;
	[HideInInspector]
	public Animation anim_headpic;
	public Color headcolor;
    public WidgetEvent onDrag;
    public WidgetEvent onClick;

	int count = 0; //用来计时
    bool isUsingUniqueSkill = false;
    void Start()
    {
        gameObject.SetActive(false);   
        if (tw == null)
            tw = GetComponent<TweenPosition>();
		anim_headpic = HeadshotIcon.GetComponent<Animation> ();
		headcolor = HeadshotIcon.color;
    }

    void OnClick()
    {
        if (onClick != null)
            onClick();
    }

    
    void OnDrag(Vector2 delta)
    {
        if (delta.y > 1f && !isUsingUniqueSkill && onDrag != null)
            onDrag();
        
    }

    public void ShowHeroState(HeroFightUnit hero)
    {
        healthBar.value = hero.healthValue;
        manaBar.value = hero.manaValue;
		if (healthBar.value <= 0.2)
			healthBar.foregroundWidget.color = Color.red;
		else if (healthBar.value > 0.2 && healthBar.value <= 0.5)
			healthBar.foregroundWidget.color = new Color (1f, 0.75f, 0f, 1f);			
		if (hero.attack != null)
		{
			HeroAttack attack = (HeroAttack)hero.attack;
            if (attack.normalSkill.time != 0)
                cdlabel.text = Mathf.RoundToInt(attack.normalSkill.time).ToString();
            else
                cdlabel.text = string.Empty;
        }
		if ( HeadshotIcon.color.g == 0f && count == 3)
		{
			
			HeadshotIcon.color = Color.white;
			count = 0;
		}
		else if(HeadshotIcon.color.g == 0f)
			count++;
    }


	IEnumerator WaitForTime(float time)
	{
		yield return new WaitForSeconds(time);
	}

		
    public void BringBack()
    {
        tw.PlayReverse();
        isUsingUniqueSkill = false;
    }

    public void BringForeward()
    {
        tw.PlayForward();
        isUsingUniqueSkill = true;
    }

    public void OnHeroDead()
    {
        /*变黑之类*/
    }

    public void SetShuxing(HurtType hurtType)
    {
        switch (hurtType)
        {
            case HurtType.Fire:
                shuxingIcon.spriteName = "shuxing_huo";
                break;
            case HurtType.Water:
                shuxingIcon.spriteName = "shuxing_shui";
                break;
            case HurtType.Earth:
                shuxingIcon.spriteName = "shuxing_tu";
                break;
            case HurtType.Light:
                shuxingIcon.spriteName = "shuxing_guong";
                break;
            case HurtType.Dark:
                shuxingIcon.spriteName = "shuxing_fa";
                break;
            default:
                shuxingIcon.enabled = false;
                break;
        }
    }

}
