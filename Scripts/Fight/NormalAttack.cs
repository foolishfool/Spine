using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoTblConfig;
using LitJson;
/// <summary>
/// 控制普通攻击释放的表现
/// </summary>
public class NormalAttack : Skill {
    /// <summary>
    /// 普通攻击的配置
    /// </summary>
    public AttackData attack;
    /// <summary>
    /// 用于计算攻击间隔
    /// </summary>
    public float time;
    /// <summary>
    /// 攻击动作名
    /// </summary>
    public List<string> skillActionList = new List<string>();
    /// <summary>
    /// 技能生效的时间
    /// </summary>
    public List<float[]> skillTimeList = new List<float[]>();
    /// <summary>
    /// 施法特效
    /// </summary>
    public List<int> castEffectIds = new List<int>();
    public List<SpecialEffect> castEffectList = new List<SpecialEffect>();
    /// <summary>
    /// 飞行特效
    /// </summary>
    public List<int> flyEffectIds = new List<int>();
    public List<FlyingEffect> flyEffectList = new List<FlyingEffect>();
    /// <summary>
    /// 受击特效
    /// </summary>
    public List<int> hitEffectIds = new List<int>();
    public List<SpecialEffect> hitEffectList = new List<SpecialEffect>();

    /// <summary>
    /// 当前随机到的动作
    /// </summary>
    string curSkillAction;
    /// <summary>
    /// 当前随机到的攻击生效时间
    /// </summary>
    float[] curSkillTime;
    /// <summary>
    /// 当前施法特效
    /// </summary>
    public SpecialEffect curCastEffect;
    /// <summary>
    /// 当前飞行特效
    /// </summary>
    public FlyingEffect curFlyEffect;
    /// <summary>
    /// 当前受击特效
    /// </summary>
    public SpecialEffect curHitEffect;


    public override void Init()
    {
        base.Init();
        //初始化技能动作
        string actionConifg = Util.GetConfigString(attack.attackAction);
        JsonData dtList = JsonMapper.ToObject(actionConifg);
        for (int i = 0; i < dtList.Count; i++)
        {
            JsonData dt = dtList[i];//有几个动作
            string skillAction = dt["action"].ToString();
            skillActionList.Add(skillAction);
            int ic = dt["time"].Count;
            float[] skillTime = new float[ic];
            for (int j = 0; j < ic; j++)
            {
                float r;
                float.TryParse(dt["time"][j].ToString(), out r);
                skillTime[j] = r;
            }
            skillTimeList.Add(skillTime);
        }
        Dictionary<int, SpecialEffect> effectDic = Util.GetDic<MsgSpecialEffect, SpecialEffect>();
        Dictionary<int, FlyingEffect> flyEffectDic = Util.GetDic<MsgFlyingEffect, FlyingEffect>();
        for (int i = 0; i < castEffectIds.Count; i++)
        {
            if (castEffectIds[i] == 0)
                castEffectList.Add(null);
            else
                castEffectList.Add(effectDic[castEffectIds[i]]);
        }
        for (int i = 0; i < flyEffectIds.Count; i++)
        {
            if (flyEffectIds[i] == 0)
                flyEffectList.Add(null);
            else
                flyEffectList.Add(flyEffectDic[flyEffectIds[i]]);
        }
        for (int i = 0; i < hitEffectIds.Count; i++)
        {
            if (hitEffectIds[i] == 0)
                hitEffectList.Add(null);
            else
                hitEffectList.Add(effectDic[hitEffectIds[i]]);
        }
    }

   

    void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            if (time <= 0)           
                time = 0;                
        }
    }

    public override void DoSkill()
    {
        time = mineUnit.fightAttribute.NormalAttackSpace;
        //随机一个动作
        int index = Random.Range(0, skillActionList.Count);
        curSkillAction = skillActionList[index];
        curSkillTime = skillTimeList[index];
        curCastEffect = castEffectList[index];
        curFlyEffect = flyEffectList[index];
        if(curFlyEffect != null)
            flyEffectTrans = mineUnit.GetEffectPoint((EffectPoint)curFlyEffect.bone);
        curHitEffect = hitEffectList[index];
        StartCoroutine(SkillWork());
    }

    public override void Stop()
    {
        OnNone();
        StopCoroutine("SkillWork");
    }

    IEnumerator SkillWork()
    {
        OnBefore();
        for (int i = 0; i < curSkillTime.Length; i++)
        {
            yield return new WaitForSeconds(curSkillTime[i]);
            OnIng();
        }
        yield return 1;
        OnAfter();
    }

    public override void OnBefore()
    {
        base.OnBefore();
        mineUnit.anim.Play(curSkillAction, false);
        StartCoroutine(DisplayPreEffect());
    }

    public override void OnIng()
    {
        base.OnIng();
        if (!targetUnit)
        {
            targetUnit = FightRule.GetFightTarget(mineUnit, mineUnit.parentGroup.targetGroup);
            if (!targetUnit)
                return;
        }
        FightRule.NormalAttackWork(mineUnit, targetUnit, this);
    }

    public override void OnAfter()
    {
        base.OnAfter();
    }

    public override void OnNone()
    {
        base.OnNone();
    }   

    public override void BindAnim()
    {
        mineUnit.anim.state.End += (state, trackIndex) =>
        {
            if (state.GetCurrent(trackIndex).animation.name.Equals(curSkillAction))
            {
                OnNone();
				//当普通攻击结束时，尝试运用技能 OnNoramlAttackFinish()  是thinkuseskill
                if (mineUnit.OnNoramlAttackFinish != null)
                   mineUnit.OnNoramlAttackFinish();
            }
        };
    }

    /// <summary>
    /// 展现施法特效
    /// </summary>
    public override IEnumerator DisplayPreEffect()
    {
        if (curCastEffect != null)
        {
            EffectTarget effectTarget = (EffectTarget)curCastEffect.target;
            if (effectTarget == EffectTarget.Enemy)
                castEffectTrans = targetUnit.GetEffectPoint((EffectPoint)curCastEffect.bone);
            else if (effectTarget == EffectTarget.Self)
                castEffectTrans = mineUnit.GetEffectPoint((EffectPoint)curCastEffect.bone);
            else if (effectTarget == EffectTarget.Screen)
                castEffectTrans = FightEffectManager.instance.transform;
            castEffectName = Util.GetConfigString(curCastEffect.name);
			castEffectAudioName = Util.GetConfigString (curCastEffect.audio);
			castEffectAudioDelay = curCastEffect.audioDelay;
            if (!string.IsNullOrEmpty(castEffectName))
            {
                yield return StartCoroutine(AssetManager.LoadAsset(castEffectName, AssetManager.AssetType.Effect, false));
				GameObject obj = AssetManager.GetGameObject(castEffectName, castEffectTrans);
				yield return StartCoroutine(AssetManager.LoadAsset(castEffectAudioName, AssetManager.AssetType.Audio, false));
				AudioClip audio = AssetManager.GetAudio (castEffectAudioName);
				AudioSource audiosource =  obj.AddComponent<AudioSource> ();
				audiosource.clip = audio; 
				StartCoroutine (WaitAudioDelay (castEffectAudioDelay));
				obj.GetComponent<AudioSource> ().Play();
            }
        }
    }
		


    /// <summary>
    /// 展现飞行特效(挂接到飞行物下)
    /// </summary>
    public override IEnumerator DisplayFlyEffect(Transform point)
    {
        if (curFlyEffect != null)
        {
            flyEffectName = Util.GetConfigString(curFlyEffect.name);
			flyEffectAudioName = Util.GetConfigString(curFlyEffect.audio);
			flyEffectAudioDelay = curFlyEffect.audioDelay;
            if (!string.IsNullOrEmpty(flyEffectName))
            {
                yield return StartCoroutine(AssetManager.LoadAsset(flyEffectName, AssetManager.AssetType.Effect, false));
				yield return StartCoroutine(AssetManager.LoadAsset(flyEffectAudioName, AssetManager.AssetType.Audio, false));
                if (point != null)
                {
                    GameObject obj = AssetManager.GetGameObject(flyEffectName, point);
					AudioClip audio = AssetManager.GetAsset<AudioClip> (flyEffectAudioName);
					obj.AddComponent<AudioSource> ().clip = audio;
					StartCoroutine (WaitAudioDelay (flyEffectAudioDelay));
					obj.GetComponent<AudioSource> ().Play();
                }
            }
        }
    }

    /// <summary>
    /// 展现受击特效
    /// </summary>
    public IEnumerator DisplayHitEffect(FightUnit tg,SpecialEffect fx)
    {
        if (fx != null)
        {
            Transform hitEffectTrans = tg.GetEffectPoint((EffectPoint)fx.bone);
            Vector3 pos = hitEffectTrans.position;
            hitEffectName = Util.GetConfigString(fx.name);
			hitEffectAudioName = Util.GetConfigString (fx.audio);
			hitEffectAudioDelay = fx.audioDelay;
            if (!string.IsNullOrEmpty(hitEffectName))
            {
                yield return StartCoroutine(AssetManager.LoadAsset(hitEffectName, AssetManager.AssetType.Effect, false));
				yield return StartCoroutine(AssetManager.LoadAsset(hitEffectAudioName, AssetManager.AssetType.Audio, false));
                GameObject obj = AssetManager.GetGameObject(hitEffectName);
                obj.transform.parent = mineUnit.mTrans.parent;
                obj.transform.localScale = Vector3.one;
                obj.transform.position = pos;
				AudioClip audio = AssetManager.GetAsset <AudioClip>(hitEffectAudioName);
				AudioSource audiosouce = obj.AddComponent<AudioSource> ();
				audiosouce.clip = audio;
				StartCoroutine ( WaitAudioDelay(hitEffectAudioDelay));
				audiosouce.Play();
            }
        }
    }

}
