using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoTblConfig;
/// <summary>
/// 控制每个单位的攻击和行动
/// </summary>
public class FightUnit : MonoBehaviour {
    public FightGroup parentGroup;
    public FightAttribute fightAttribute = new FightAttribute();
    public int _health;
    public int health
    {
        get { return _health; }
        set 
        {            
            value = Mathf.Clamp(value,0,fightAttribute.health);
            if (_health == value) return;
            if (_health > value && OnGotHit != null) 
                //event execute _health -value (the parameter of the method is _health -value)
				// when is damaged caculate the damage
				OnGotHit(_health - value); 
            else if (_health < value && OnGotCure != null)
                OnGotCure(value - _health);
            _health = value;
        }
    }
    public float healthValue { get { if (fightAttribute.health == 0)return 0; return (float)health / fightAttribute.health; } }
    public int mana;
    public float manaValue { get { if (fightAttribute.mana == 0) return 0; return (float)mana / fightAttribute.mana; } }
    [SerializeField]
    public FightUnit targetUnit;
    public Transform orinPoint;
	public Transform birthPoint;
    public SkeletonAnimation anim;
    public Attack attack;
    public float modifiedY = 0;//Y轴修正，防止重叠
    public float modifiedZ = 0;//Z轴修正，防止重叠
    [HideInInspector]
    public Transform mTrans;

	public bool isMoveforward = true; //初始朝向
	public bool isRotate = false;  //是否需要旋转
    /// <summary>
    /// 挂载点
    /// </summary>
    [HideInInspector]
    public Transform Foot, LeftHand, LeftWeapon, RightHand, RightWeapon, Head, Body;

    public enum UnitState
    {
        MoveForward,
		MoveBack,
        MoveToTarget, //移动或者追击目标
        Fighting,
        Wait,
        MoveToNext,
        Win,
        Dead,
		GetBack, //get back to the original point
		LoseTarget,
    }
	public UnitState state = UnitState.Wait;

#region 战斗事件

	//declare the delegate class
	public delegate void FightHandler(params object[] objs);   
    public delegate bool FightCheckHandler(params object[] objs);
    public delegate int FightCaculateHandler(params object[] objs);
	//delcare the delegate variables
    public FightHandler OnNormalAttackHit; //当普通攻击命中的时候，吸血buff监听 
    public FightHandler OnNormalAttackCritil;//当普通攻击暴击的时候
    public FightHandler OnNoramlAttackFinish; //当普通攻击结束的时候，怪物AI监听

    public FightCheckHandler OnWantUseSkill; //当想使用技能的时候，麻痹buff监听   
    public FightHandler OnSkillHit; //当使用技能命中时，combo连击监听

    public FightHandler OnDodge;//当闪避攻击的时候
    public FightHandler OnGotCritil;//当受到暴击的时候
    public FightHandler OnFantan;//当受到可以反弹的伤害时（非buff攻击）
    public List<FightCaculateHandler> OnHudunList = new List<FightCaculateHandler>();//当受到可以用护盾吸收的伤害时
    public List<FightCheckHandler> OnBuffAttachList = new List<FightCheckHandler>();//当附加buff时检查是否免疫
    
    public FightHandler OnStartNextBattle;//当进入下一波战斗

    public FightHandler OnGotHit;//当受到伤害
    public FightHandler OnGotCure;//当受到治疗

    public FightHandler OnDead;//当死亡时
    public FightHandler OnKillEnemy;//当杀死目标的时候
#endregion

#region 特殊状态
    int _InvincibleStateCount = 0;//无敌状态数量
    public int InvincibleStateCount
    {
        get { return _InvincibleStateCount; }
        set { _InvincibleStateCount = value; }
    }
    public bool isInvincible{ get { return _InvincibleStateCount > 0; } }
    bool isRotated = false;//魅惑导致转向
    int _EnticeStateCount = 0;//魅惑状态的数量
    public int EnticeStateCount
    {
        get { return _EnticeStateCount; }
        set
        {
            if (value == _EnticeStateCount)
                return;
            if (value > 0)
            {
                attack.StopCurrent();
                targetUnit = FightRule.GetFightTarget(this, parentGroup);
                if (targetUnit != null && (targetUnit.mTrans.localPosition.x - mTrans.localPosition.x) * (parentGroup.group == FightGroup.GroupType.Mine ? 1 : -1) < 0)
                {
                    mTrans.Rotate(Vector3.up, 180f, Space.Self);
                    isRotated = true;
                }
                UnAbleUseSkillCount++;
            }
            else
            {
                if (isRotated)
                {
                    mTrans.Rotate(Vector3.up, 180f, Space.Self);
                    isRotated = false;
                }
                targetUnit = null;
                UnAbleUseSkillCount--;
            }
            _EnticeStateCount = value;
        }
    }
    public bool isEntice{   get { return EnticeStateCount > 0; }}

    int _ConfusedStateCount = 0;//混乱状态的数量
    public int ConfusedStateCount
    {
        get { return _ConfusedStateCount; }
        set
        {
            if (value == _ConfusedStateCount)
                return;
            if (_ConfusedStateCount == 0 && value > 0)
            {
                attack.StopCurrent();
                targetUnit = FightRule.GetFightTarget(this, parentGroup);
                if (targetUnit != null && (targetUnit.mTrans.localPosition.x - mTrans.localPosition.x) * (parentGroup.group == FightGroup.GroupType.Mine ? 1 : -1) < 0)
                {
                    mTrans.Rotate(Vector3.up, 180f, Space.Self);
                    isRotated = true;
                }
            }
            else if(_ConfusedStateCount > 0 && value == 0)
            {
                if (isRotated)
                {
                    mTrans.Rotate(Vector3.up, 180f, Space.Self);
                    isRotated = false;
                }
                targetUnit = null;
            }
            _ConfusedStateCount = value;
        }
    }
    public bool isConfused{ get { return ConfusedStateCount > 0; }}

    public uint UnAbleMoveCount;//造成无法移动的状态数量
    public uint UnAbleFightCount;//造成无法攻击的状态数量
    public uint UnAbleUseSkillCount;//造成无法使用技能的状态数量

    public bool beAbleMove { get { return UnAbleMoveCount == 0; } }
    public bool beAbleFight { get { return UnAbleFightCount == 0; } }
    public bool beAbleUseSkill { get { return UnAbleUseSkillCount == 0; } }
#endregion

    public virtual void Start()
    {
        mTrans = transform;
        attack = GetComponent<Attack>();
        _health = (int)fightAttribute.health;
        mana = (int)fightAttribute.mana;
		// 出生点， 父节点不为0

        if (GetComponent<Animation>() == null)
        {
            anim = GetComponent<SkeletonAnimation>();
            if (anim == null)
                anim = GetComponentInChildren<SkeletonAnimation>();
        }

        Foot = mTrans.Find("Foot");
        LeftHand = mTrans.Find("LeftHand");
        LeftWeapon = mTrans.Find("LeftWeapon");
        RightHand = mTrans.Find("RightHand");
        RightWeapon = mTrans.Find("RightWeapon");
        Head = mTrans.Find("Top");
        Body = mTrans.Find("Body");
        attack.Init();

    }

	public virtual void Update ()
	{


		if (state == UnitState.Wait && beAbleMove)
		{
			anim.Play (Const.IdleAction, true);
		}



//        if (state == UnitState.MoveForward && beAbleMove)
//        {
//            if (TryAttack())
//                state = UnitState.Fighting;
//            else
//                MoveForward();
//        }
//        else if (state == UnitState.MoveToTarget && beAbleMove)
//        {
//            if (TryAttack())
//            {
//                anim.Play(Const.IdleAction, true); //调整动作
//                state = UnitState.Fighting; //有目标既进入战斗状态
//            }
//            else
//            {
//                if (targetUnit == null)
//                    return;
//                else
//                    MoveTowardsTarget();
//            }
//        }
//        else if (state == UnitState.Fighting && beAbleFight)
//        {
//            if (modifiedY != 0 || modifiedZ != 0)
//                mTrans.localPosition = Vector3.MoveTowards(mTrans.localPosition, new Vector3(mTrans.localPosition.x, modifiedY, modifiedZ), Const.MoveSpeed * Time.deltaTime);
//            if (TryAttack())
//                attack.DoAttack(targetUnit);
//            else
//            {
//                //attack.StopCurrent();
//                if(attack.currentSkill != null)
//                    return;
//                if (targetUnit == null)
//                    state = UnitState.Wait;
//                else
//                    state = UnitState.MoveToTarget;
//            }
//        }       
//        else if (state == UnitState.MoveToNext)
//        {
//            //attack.StopCurrent();
//            if (attack.currentSkill != null)
//                return;
//            anim.Play(Const.RunAction, true);
//        }
//        else if (state == UnitState.Dead)
//        {
//            enabled = false;
//            attack.StopCurrent();
//            attack.EndAttack();
//        }
//        else if (state == UnitState.Win)
//        {
//            if (attack.currentSkill == null)
//            {
//                anim.isLocked = true;
//                anim.state.SetAnimation(0, Const.WinAction, false);
//                anim.state.AddAnimation(0, Const.IdleAction, true, anim.state.GetCurrent(0).animation.Duration);
//                enabled = false;
//            }
//        }
//        else
//        {
//            anim.Play(Const.IdleAction, true);
//        }
    }

    public bool TryAttack()
    {
       if(targetUnit == null || targetUnit.state == UnitState.Dead)
           targetUnit = FightRule.GetFightTarget(this,isEntice || isConfused? parentGroup : parentGroup.targetGroup);
       if (targetUnit == null)
           return false;
       return Util.Distance(targetUnit.transform.localPosition, mTrans.localPosition) <= fightAttribute.range;
    }
		

    public void MoveForward()
    {
		isMoveforward = true;
		mTrans.localPosition += (parentGroup.group == FightGroup.GroupType.Mine ? 1 : -1) * (isEntice ? -1 : 1) * Vector3.right * fightAttribute.MoveSpeed * Time.deltaTime;
        anim.Play(Const.RunAction, true);
    }	

	public void MoveBack()
	{
		isMoveforward = false;
		mTrans.localPosition += (parentGroup.group == FightGroup.GroupType.Mine ? 1 : -1) * (isEntice ? -1 : 1) * Vector3.left * fightAttribute.MoveSpeed * Time.deltaTime;
		anim.Play(Const.RunAction, true);
	}	
	//返回出生地
	public void GetBack()
	{
		
		mTrans.localPosition = Vector3.MoveTowards(mTrans.localPosition, birthPoint.localPosition, fightAttribute.GetBackSpeed * Time.deltaTime);
		anim.Play(Const.RunAction, true);
	}
	//追击目标
    public void MoveTowardsTarget()
    {
        float x = mTrans.localPosition.x < targetUnit.transform.localPosition.x ? targetUnit.transform.localPosition.x - fightAttribute.range : targetUnit.transform.localPosition.x + fightAttribute.range;
		mTrans.localPosition = Vector3.MoveTowards(mTrans.localPosition, new Vector3(x, modifiedY == 0 ? mTrans.localPosition.y : modifiedY, mTrans.localPosition.z), fightAttribute.PursueSpeed * Time.deltaTime);
        anim.Play(Const.RunAction, true);
    }


	public void ChangeOrientation()
	{
		//如果在目标活着出生点右侧
		if (((this.targetUnit != null && mTrans.localPosition.x < this.targetUnit.mTrans.localPosition.x ) || (this.targetUnit == null && mTrans.localPosition.x < this.birthPoint.localPosition.x)) && isRotate == false  )
		{
			mTrans.Rotate (Vector3.up * 180);
			isRotate = true;

		}

		if (((this.targetUnit != null && mTrans.localPosition.x > this.targetUnit.mTrans.localPosition.x ) || (this.targetUnit == null && mTrans.localPosition.x > this.birthPoint.localPosition.x)) && isRotate == true  )
		{
			mTrans.Rotate (Vector3.up * 180);
			isRotate = false;
			Debug.Log ("@@@@@@@@@@@@@@"  );
		}

	}


    public Transform GetEffectPoint(EffectPoint point)
    {
        switch (point)
        {
            case EffectPoint.Body:
                return Body;
            case EffectPoint.Foot:
                return Foot;
            case EffectPoint.Head:
                return Head;
            case EffectPoint.LeftHand:
                return LeftHand;
            case EffectPoint.LeftWeapon:
                return LeftWeapon;
            case EffectPoint.RightHand:
                return RightHand;
            case EffectPoint.RightWeapon:
                return RightWeapon;
            default:
                return mTrans;
        }
    }

#region Buff

    public List<Buff> buffList = new List<Buff>();

    public void RegisterBuff(Buff newBuff)
    {
        if(newBuff.buffData.mutexType == 1)
        {
            for (int i = buffList.Count - 1; i >= 0; i--)
            {
                if (newBuff.id == buffList[i].id)
                    buffList[i].Clear();
            }
        }
        buffList.Add(newBuff);
    }

    public void LogoffBuff(Buff buff)
    {
        buffList.Remove(buff);
    }

    /// <summary>
    /// 清除所有buff
    /// </summary>
    public void ClearAllBuff()
    {
        for (int i = buffList.Count - 1; i >= 0; i--)
        {
            buffList[i].Clear();
        }
    }
    /// <summary>
    /// 清除所有可清除的debuff
    /// </summary>
    public void ClearAllDebuff()
    {
        for (int i = buffList.Count - 1; i >= 0; i--)
        {
            if (buffList[i].buffData.buffType == 1 && buffList[i].buffData.dispel == 1)
                buffList[i].Clear();
        }
    }

#endregion
}
