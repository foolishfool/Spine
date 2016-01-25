using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoTblConfig;

// effect and is used in loading effect

public class Skill : MonoBehaviour {

	public enum SkillState
	{
		None,
		Ing,
		Before,
		After,
	}

	public class buffConfig
	{
		public int id;
		public float percent;
		public float value;
		public float time;
		public int level; // hit level
		public UniqueSkill bindSkill;
	}

	public SkillState state = SkillState.None;
	public FightUnit mineUnit;
	public FightUnit targetUnit;

	public int dmgEffectId; //magic effect id
	public SpecialEffect dmgEffect;
	public string dmgEffectName;

	public int castEffectId;
	public SpecialEffect castEffect;
	public string castEffectName;
	public Transform castEffectTrans;

	public int flyEffectId;
	public FlyingEffect flyEffect;
	public string flyEffectName;
	public Transform flyEffectTrans;
	public Vector3 flyEffectFromPos
	{
		get
		{
			Vector3 offset = flyEffectTrans.localPosition;
			if(mineUnit.mTrans.localRotation.eulerAngles.y != 0 )
			{
				offset.x *= -1;
				return mineUnit.mTrans.localPosition + offset;
			}
			else
				return offset;
		}
	}

	public int hitEffectId;
	public SpecialEffect hitEffect;
	public string hitEffectName;
	public Transform hitEffectTrans;

	public virtual void Init()

	{
		BindAnim ();
		Dictionary<int,SpecialEffect> effectDic = Util.GetDic<MsgSpecialEffect,SpecialEffect> ();
		if (dmgEffectId != 0) 
		{
			dmgEffect = effectDic[dmgEffectId];
			dmgEffectName  = Util.GetConfigString(dmgEffect.name);	
		}
		if (castEffectId != 0) 
		{
			castEffect = effectDic[castEffectId];
			castEffectName = Util.GetConfigString(castEffect.name);
		}
		if (flyEffectId != 0) 
		{
			flyEffect = Util.GetDic<MsgFlyingEffect,FlyingEffect>()[flyEffectId];
			flyEffectName = Util.GetConfigString(flyEffect.name);
			flyEffectTrans = mineUnit.GetEffectPoint((EffectPoint)flyEffect.bone);
		}

		if (hitEffectId != 0) 
		{
			hitEffect = effectDic[hitEffectId];
			hitEffectName = Util.GetConfigString(hitEffect.name);
			
		}

		StartCoroutine (LoadEffect ());
	}

	IEnumerator LoadEffect()
	{
		if (!string.IsNullOrEmpty (dmgEffectName))
			yield return StartCoroutine (AssetManager.LoadAsset (dmgEffectName, AssetManager.AssetType.Effect, false));
		if (!string.IsNullOrEmpty (castEffectName))
			yield return StartCoroutine (AssetManager.LoadAsset (castEffectName, AssetManager.AssetType.Effect, false));
		if (!string.IsNullOrEmpty (flyEffectName))
			yield return StartCoroutine (AssetManager.LoadAsset (flyEffectName, AssetManager.AssetType.Effect, false));
		if (!string.IsNullOrEmpty (hitEffectName))
			yield return StartCoroutine (AssetManager.LoadAsset (hitEffectName, AssetManager.AssetType.Effect, false));

	}

	public void Use(FightUnit target)
	{
		if (targetUnit == null || targetUnit != target) 
			targetUnit = target;
		DoSkill ();
	}

	public virtual void Stop()
	{
		OnNone ();
		StopAllCoroutines ();
	}

	/// <summary>
	/// interrupt the skill being preparing to cast
	/// </summary> 
	/// 
	public void StopBefore()
	{
		if (state == SkillState.None || state == SkillState.Before) 
		{
			StopAllCoroutines();
		}
	}

	public virtual IEnumerator DisplaydmgEffect(FightUnit dmgEffectTarget)
	{
		if (dmgEffect != null && !string.IsNullOrEmpty (dmgEffectName)) 
		{
			EffectTarget effectTarget = (EffectTarget)dmgEffect.target;
			Transform dmgEffectTrans = null;
			Vector3 effectLocalPos = Vector3.zero;
			if(effectTarget ==EffectTarget.Screen )
			{
				dmgEffectTrans = FightEffectManager.instance.transform;
			}
			else if(effectTarget == EffectTarget.MineCenter)
			{
				dmgEffectTrans = mineUnit.mTrans.parent;
				effectLocalPos = mineUnit.parentGroup.Center; 
			}
			else if (effectTarget == EffectTarget.EnemyCenter)
			{
				dmgEffectTrans = mineUnit.mTrans.parent;
				effectLocalPos = targetUnit.parentGroup.Center;
			}
			else if (effectTarget != null)
			{
				dmgEffectTrans = dmgEffectTarget.GetEffectPoint((EffectPoint)dmgEffect.bone);
			}

			yield return StartCoroutine(AssetManager.LoadAsset(dmgEffectName,AssetManager.AssetType.Effect,false));
			GameObject obj = AssetManager.GetGameObject(dmgEffectName,dmgEffectTrans);
			obj.transform.localPosition = effectLocalPos; 
		}
	}

	public virtual IEnumerator DisplayPreEffect()
	{
		if (castEffect != null) 
		{
			EffectTarget effectTarget = (EffectTarget)castEffect.target;
			if(effectTarget == EffectTarget.Enemy)
				castEffectTrans = targetUnit.GetEffectPoint((EffectPoint)castEffect.bone);
			else if (effectTarget == EffectTarget.Self)
				castEffectTrans = mineUnit.GetEffectPoint((EffectPoint)castEffect.bone);
			else if (effectTarget == EffectTarget.Screen)
				castEffectTrans = FightEffectManager.instance.transform;
			if(!string.IsNullOrEmpty(castEffectName))
			{
				yield return StartCoroutine(AssetManager.LoadAsset(castEffectName,AssetManager.AssetType.Effect,false));
				GameObject obj = AssetManager.GetGameObject(castEffectName,castEffectTrans);
			}

		}
	}

	public virtual IEnumerator DisplayFlyEffect(Transform point)
	{
		if (flyEffect != null) 
		{
			if(!string.IsNullOrEmpty(flyEffectName))
			{
				yield return StartCoroutine(AssetManager.LoadAsset(flyEffectName,AssetManager.AssetType.Effect,false));
				if(point!= null)
				{
					GameObject obj = AssetManager.GetGameObject(flyEffectName,point);
				}
			}
		}
	}

	public virtual IEnumerator DisplayHitEffect(FightUnit tg)
	{
		if (hitEffect != null)
		{
			Transform hitEffectTrans = tg.GetEffectPoint((EffectPoint)hitEffect.bone);	
			Vector3 pos = hitEffectTrans.position;
			if(!string.IsNullOrEmpty(hitEffectName))
			{
				yield return StartCoroutine(AssetManager.LoadAsset(hitEffectName,AssetManager.AssetType.Effect,false));
				GameObject obj = AssetManager.GetGameObject(hitEffectName);
				obj.transform.parent = mineUnit.mTrans.parent;
				obj.transform.localScale = Vector3.one;
				obj.transform.position = pos;
			}
		}
	}






	public virtual void BindAnim()
	{

	}
	public virtual void DoSkill()
	{	
		OnBefore();
	}
	public virtual void OnBefore()
	{
		state = SkillState.Before;
	}

	public virtual void OnIng()
	{
		if (state == SkillState.Ing)
			state = SkillState.After;
	}
	public virtual void OnNone()
	{
		state = SkillState.None;
		if (mineUnit.attack.currentSkill == this)
			mineUnit.anim.Play (Const.IdleAction, true);
	}
	

	public virtual void OnAfter() 
	{ 
		if (state == SkillState.Ing) 
			state = SkillState.After; 
	}

}
