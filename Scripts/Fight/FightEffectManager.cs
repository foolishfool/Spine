using UnityEngine;
using System.Collections;
/// <summary>
/// 控制场景层和UI层之间的特效层，用于绝技特效以及全屏特效
/// </summary>
public class FightEffectManager : MonoBehaviour {
//    public static FightEffectManager instance;
//    public UIWidget cullBackGround;
//    public static float greyScale = 1f;
//
//    int _SkillCount = 0;
//    public int SkillCount
//    {
//        get {return _SkillCount;}
//        set
//        {
//            value = Mathf.Max(0,value);
//            if(value == 0)
//                HideBlackCull();
//            else 
//                DisplayBlackCull();
//            _SkillCount = value;
//        }
//    }
//
//    Transform mTrans;
//
//    void Awake()
//    {
//        instance = this;
//        mTrans = transform;
//        if (cullBackGround == null)
//            cullBackGround = transform.Find("CullBackground").GetComponent<UIWidget>();
//    }
//
//    public void BringForeward(FightUnit unit)
//    {
//        unit.anim.isUsingSkill = true;
//    }
//
//    public void BringBack(FightUnit unit)
//    {
//        unit.anim.isUsingSkill = false;
//    }
//
//    public void DisplayBlackCull()
//    {
//        greyScale = Const.greyScale;
//        cullBackGround.enabled = true;
//    }
//
//    public void HideBlackCull()
//    {
//        greyScale = 1;
//        cullBackGround.enabled = false;
//    }
//
//    public void AddOne()
//    {
//        SkillCount++;
//    }
//
//    public void MinusOne()
//    {
//        SkillCount--;
//    }

	public static FightEffectManager instance;
	public UIWidget cullBackGround;
	public static float greyScale = 1f;
	int _SkillCount = 0;
	public int SkillCount
	{
		get{
			return _SkillCount;
		}
		set 
		{
			value = Mathf.Max(0,value);
			if (value ==0)
			{
				HideBlackCull();
			}
			else
				DisplayBlackCull();
			_SkillCount = value;
		}
	}

	Transform mTrans;
	void Awake()
	{
		instance = this;
		mTrans = transform;
		if (cullBackGround == null)
			cullBackGround = transform.Find ("CullBackGround").GetComponent<UIWidget> ();
	}

	public void BringForeward(FightUnit unit)
	{
		unit.anim.isUsingSkill = true;
	}

	public void BringBack(FightUnit unit)
	{
		unit.anim.isUsingSkill = false;
	}

	public void DisplayBlackCull()
	{
		greyScale = 1;
		cullBackGround.enabled = true;
	}

	public void HideBlackCull()
	{
		greyScale = 1;
		cullBackGround.enabled = false;	
	}

	public void AddOne()
	{
		SkillCount++;
	}

	public void MinusOne()
	{
		SkillCount--;
	}
}
