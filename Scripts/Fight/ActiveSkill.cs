using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoTblConfig;
using LitJson;

public class ActiveSkill : Skill {
	public NormalSkill normalSkill;
	public NormalSkillLvUp normalSkillLvUp;

	public float time;
	public float cdValue
	{
		get
		{
			if(normalSkillLvUp.cooldown == 0)
				return 0;
			else
				return time / normalSkillLvUp.cooldown;
		}
	}

	public string skillAction;
	public float[] skillTime;
	public float[] magicDelay;

	public List<buffConfig> buffConfigList = new List<buffConfig>();
	public SkillAttach attachExtension;

	public JsonData attachExtensionParam;

	public  override void Init()
	{
		base.Init ();
		string actionConfig = Util.GetConfigString (normalSkill.skillAction);
		JsonData dt = JsonMapper.ToObject (actionConfig);
		skillAction = dt ["action"].ToString ();
		int ic = dt ["time"].Count;
		skillTime = new float[ic];
		for (int i = 0; i<ic; i++) 
		{
			skillTime[i] = float.Parse(dt["time"][i].ToString());
		}
		string strMagicDelay = Util.GetConfigString (normalSkill.delay);
		JsonData magicDt = JsonMapper.ToObject (strMagicDelay);
		magicDelay = new float[magicDt.Count];

		for (int i = 0; i< magicDelay.Length; i++) 
		{
			magicDelay[i] = float.Parse(magicDt[i].ToString());
		}
		//buff 
		string buffString = Util.GetConfigString (normalSkillLvUp.buff);
		JsonData dt_1 = JsonMapper.ToObject (buffString);
		for (int i=0; i<dt_1.Count; i++) 
		{
			JsonData dt_2 = dt_1[i];
			buffConfig bc = new buffConfig();
			bc.id = int.Parse(dt_2[0].ToString());
			bc.percent = float.Parse(dt_2[1].ToString());
			bc.value = float.Parse(dt_2[2].ToString());
			bc.time = float.Parse(dt_2[3].ToString());
			bc.level = int.Parse(dt_2[4].ToString());
			buffConfigList.Add(bc);
		}

		attachExtension = (SkillAttach)normalSkillLvUp.addEffect;
		attachExtensionParam = JsonMapper.ToObject (Util.GetConfigString (normalSkillLvUp.addEffectVal));
	}

	void Update()
	{
		if (time > 0) 
		{
			time = Time.deltaTime;
			if(time <= 0)
			{
				time = 0;
			}
		}
	}

	public override void DoSkill()
	{
		time = normalSkillLvUp.cooldown;
		mineUnit.health -= (int)normalSkillLvUp.HPCost;
		if (mineUnit.parentGroup.group == FightGroup.GroupType.Mine)
		{
			mineUnit.mana -= (int)normalSkillLvUp.MPCost;
		}
		if(mineUnit.OnWantUseSkill != null && !mineUnit.OnWantUseSkill(this))
			return;
		OnBefore ();
		StartCoroutine (SkillWork());
	}

	public override void Stop()
	{
		OnNone ();
		StopCoroutine("SkillWork");
	}



	 IEnumerator SkillWork()
	{
		for (int i=0; i<skillTime.Length; i++) 
		{
			yield return new WaitForSeconds(skillTime[i]);
		}
		yield return 1;
		OnAfter ();
	}

	public override void OnBefore()
	{
		base.OnBefore ();
		mineUnit.anim.Play (skillAction, false);
		StartCoroutine (DisplayPreEffect ());
	}

	public override void OnIng()
	{
		base.OnIng ();
		if (!targetUnit) 
		{
			if(mineUnit.TryAttack())
			{
				targetUnit = mineUnit.targetUnit;
			}
			else
				return;
		}
		FightRule.ActiveSkillWork (mineUnit, targetUnit, this);
	}

	public override void BindAnim()
	{
		mineUnit.anim.state.End += (state,trackIndex) =>
		{
			if (state.GetCurrent (trackIndex).animation.name.Equals (skillAction))
				OnNone ();
		};

	}

}
