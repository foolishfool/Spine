using UnityEngine;
using System.Collections;
using ProtoTblConfig;
/// <summary>
/// 控制英雄的攻击和行动
/// </summary>
[RequireComponent(typeof(HeroAttack))]
public class HeroFightUnit : FightUnit {
    public HeroData heroData;//详细数据
    public HeroEvolution heroEvolution;//进化数据
    public int level;//英雄等级
    /// <summary>
    /// 临时，应由服务器发送
    /// </summary>
    public void InitFightAttribute()
    {
        fightAttribute.range = (int)heroData.range;
        fightAttribute.attackSpeed = (int)heroData.attackSpeed;
        fightAttribute.health = (int)heroData.healthBasic;
        fightAttribute.mana = (int)heroData.mana;
        fightAttribute.attack = (int)heroData.attackBasic;
        fightAttribute.defence = (int)heroData.defenceBasic;
        fightAttribute.critical = (int)heroData.critical;
        fightAttribute.hit = (int)heroData.hit;
        fightAttribute.dodge = (int)heroData.dodge;
        fightAttribute.toughness = (int)heroData.toughness;
        fightAttribute.elementType = (HurtType)heroData.element;  
    }

    public override void Start()
    {
        base.Start();
        Combo.GetInstance().RegesterCombo(this);
    }

	public override void Update()
    {
		base.Update ();

		//先状态 后改变状态，否则会状态冲突 改变状态无效

		if (state == UnitState.Fighting && beAbleFight)
		{
			if (modifiedY != 0 || modifiedZ != 0)
				mTrans.localPosition = Vector3.MoveTowards (mTrans.localPosition, new Vector3 (mTrans.localPosition.x, modifiedY, modifiedZ), Const.MoveSpeed * Time.deltaTime);
			if (TryAttack ())
				attack.DoAttack (targetUnit);
			else
			{
				if (attack.currentSkill != null)
					return;
				if (targetUnit == null)
					state = UnitState.Wait;
				else
					state = UnitState.MoveToTarget;
			}
				
		}
		//转向
		if (((Input.GetKeyDown (KeyCode.D) && isMoveforward == false) && !Input.GetKeyDown (KeyCode.A) || ((Input.GetKeyDown (KeyCode.A) && isMoveforward == true) && !Input.GetKeyDown(KeyCode.D))))
		{
			mTrans.Rotate (Vector3.up * 180);
		}
		//不用else if 如果转向了 这真就不走了
		if (Input.GetKey (KeyCode.D) && this.parentGroup.canMoveForward)
		{
			attack.StopCurrent ();
			state = UnitState.MoveForward;
			MoveForward();
		} 
		else if (Input.GetKey (KeyCode.D) && !this.parentGroup.canMoveForward)
		{
			attack.StopCurrent ();
			state = UnitState.MoveForward;
			anim.Play(Const.RunAction, true);
		} 

		if (Input.GetKey (KeyCode.A) && this.parentGroup.canMoveBack)
		{	
			attack.StopCurrent ();
			state = UnitState.MoveBack;
			MoveBack();
		} 
		else  if (Input.GetKey (KeyCode.A) && !this.parentGroup.canMoveBack)
		{	
			attack.StopCurrent ();
			state = UnitState.MoveBack;
			anim.Play(Const.RunAction, true);
		} 

		if (Input.GetKeyDown (KeyCode.J))
		{
			state = UnitState.Fighting;

		} 

		if(targetUnit == null && !(Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.D)||Input.GetKey(KeyCode.J)))
		{	
			state = UnitState.Wait;
			anim.Play (Const.IdleAction, true);
		}

			
    }
		
		
}
