using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class FightGroup : MonoBehaviour {
	// when the unit come to the battle_point, they can begin to attack
	public Transform[] battle_points; 
	[HideInInspector]
	public List<FightUnit> fightUnits =  new List<FightUnit>(); 
	[HideInInspector]
	public List<FightUnit> fightUnitsBak = new List<FightUnit>();

	//define the new enum Type
	public enum GroupType
	{
		Mine,
		Enemy,
	}

	public GroupType group;
	[HideInInspector]
	public FightGroup targetGroup;
	public bool canMoveForward = true;
	public bool canMoveBack = true;
	public FightUnit FirstUnit
	{
		get
		{
			ResortByPos();
			return fightUnitsSortByPos[0];
		}
	}

	public FightUnit LastUnit
	{
		get
		{
			ResortByPos();
			return fightUnitsSortByPos[fightUnitsSortByPos.Count - 1];
		}
	}
	// sort the render orders of different fight units

	public List<FightUnit> fightUnitsSortByPos = new List<FightUnit>();

	public void ResortByPos()	
	{
		fightUnitsSortByPos = new List<FightUnit> (fightUnits);
		fightUnitsSortByPos.Sort (delegate(FightUnit x, FightUnit y) {
			return group == GroupType.Mine ? -(x.transform.localPosition.x.CompareTo (y.transform.localPosition.x)) : x.transform.localPosition.x.CompareTo (y.transform.localPosition.x);
		}
		);
	}

	public FightUnit WeakestUnit
	{
		get
		{
			ResortByHP();
			return fightUnitsSortByHP[0];
		}
	}

	public List<FightUnit> fightUnitsSortByHP = new List<FightUnit>();
	public void ResortByHP()
	{	
		fightUnitsSortByHP = new List<FightUnit> (fightUnits);
		//委托排序
		fightUnitsSortByHP.Sort (delegate(FightUnit x, FightUnit y) {
			return x.healthValue.CompareTo (y.healthValue);
		});
	}
	// the central point's position of group
	public Vector3 Center
	{
		get
		{
			ResortByPos();
			return (fightUnitsSortByPos[0].transform.localPosition + fightUnitsSortByPos[fightUnitsSortByPos.Count-1].transform.localPosition)*0.5f;
		}

	}

	/// <summary>
	/// 战斗开始前向前移动
	/// </summary>

	public void MoveForward()
	{
		for (int i = 0; i< fightUnits.Count; i++) 
		{
			fightUnits[i].state = FightUnit.UnitState.MoveForward;
		}
	}




	public virtual void Start()
	{

	}

}