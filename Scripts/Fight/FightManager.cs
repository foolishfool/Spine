using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoTblConfig;

/// <summary>
/// 一波战斗的统一管理
/// </summary>
public class FightManager : MonoBehaviour{
    static FightManager instance;
    public static FightManager GetInstance()
    {
        return instance;
    }

    public HeroFightGroup mineGroup;
    public MonsterFightGroup enemyGroup;
    public GameObject dieEffect;

    /// <summary>
    /// 是否处于自动战斗状态
    /// </summary>
    public bool isAutoFight = false;
    /// <summary>
    /// 自动战斗使用绝技所需最低能量
    /// </summary>
    public int needEnergy;
    /// <summary>
    /// 决策系数
    /// </summary>
    Dictionary<int, DecisionFactor> DecisionFactorDic;

    void Awake()
    {
        instance = this;
        //needEnergy = 10;
        needEnergy = Random.Range(Const.CONST_SPECIALSKILL_RNDMIN, Const.CONST_SPECIALSKILL_RNDMAX);
        DecisionFactorDic = Util.GetDic<MsgDecisionFactor,DecisionFactor>();
    }

    /// <summary>
    /// a杀死b
    /// </summary>
    /// <param name="a"></param>
    /// <param name="d"></param>
    public void UnitDead(FightUnit a, FightUnit d)
    {
        d.parentGroup.fightUnits.Remove(d);
        if(d.parentGroup.fightUnitsBak.Count > 0)
        {
            FightUnit b = d.parentGroup.fightUnitsBak[0];
            d.parentGroup.fightUnitsBak.RemoveAt(0);
            d.parentGroup.fightUnits.Add(b);
            b.gameObject.SetActive(true);
            b.transform.localPosition = d.transform.localPosition;
        }       
        if (d.parentGroup.fightUnits.Count == 0)
        {
            if (d.parentGroup.group == FightGroup.GroupType.Mine)
                GameOver();
            else
            {
                if (enemyGroup.wave == DungeonManager.enemyWave.Count)
                    Win();
                else
                    MoveToNext();
            }
        }     
        GameObject obj = Instantiate(dieEffect) as GameObject;
        obj.transform.position = d.transform.position;
        obj.transform.parent = a.mTrans.parent;
        HealthUIManager.instance.RemoveHealthBar(d);
        StartCoroutine(DestroyUnit(d));
        if (d.OnDead != null)
            d.OnDead(a);
        if (a.OnKillEnemy != null)
            a.OnKillEnemy(d);
    }

    IEnumerator DestroyUnit(FightUnit unit)
    {
        if (unit.state != FightUnit.UnitState.Dead)
        {
            if (unit.parentGroup.group == FightGroup.GroupType.Mine)
                DungeonRecord.heroDieCount++;
            unit.state = FightUnit.UnitState.Dead;  
            unit.anim.Play(Const.IdleAction, true);
            yield return new WaitForSeconds(0.2f);
            Destroy(unit.gameObject);
        }
    }

    public void GameOver()
    {
        CEventDispatcher.GetInstance().DispatchEvent(new CBaseEvent(CEventType.GAME_OVER, this));
    }

    public void Win()
    {
        mineGroup.Win();
        CEventDispatcher.GetInstance().DispatchEvent(new CBaseEvent(CEventType.GAME_WIN, this));
    }

    public void MoveToNext()
    {
        StartCoroutine(StartMoveToNext());
        CEventDispatcher.GetInstance().DispatchEvent(new CBaseEvent(CEventType.MOVE_TO_NEXT, this));
    }

    IEnumerator StartMoveToNext()
    {
        yield return new WaitForSeconds(Const.DieTime);
        ScrollSceneManager.instance.EnableScroll();
        mineGroup.MoveToNext();       
 
    }

    public void EndMoveToNext()
    {
        ScrollSceneManager.instance.DisableScroll();
        enemyGroup.InstUnits();
        mineGroup.MoveForward();
        CEventDispatcher.GetInstance().DispatchEvent(new CBaseEvent(CEventType.NEXT_BATTALE_START, this));
    }

    /// <summary>
    /// 激活自动战斗
    /// </summary>
    public void EnableAutoFight()
    {
        isAutoFight = true;
        InvokeRepeating("TryUseUniqueSkill",0,3f);
    }
    /// <summary>
    /// 结束自动战斗
    /// </summary>
    public void DisableAutoFight()
    {
        isAutoFight = false;
        CancelInvoke("TryUseUniqueSkill");
    }
    /// <summary>
    /// 自动使用绝技
    /// </summary>
    public void TryUseUniqueSkill()
    {
        if (FightEnergy.instance.EnergyVal < needEnergy || FightEnergy.instance.EnergyUseVal != 0 || mineGroup.fightUnits.Count == 0)
            return;
        List<HeroAttack> heroAttacks = new List<HeroAttack>();
        for (int i = 0; i < mineGroup.fightUnits.Count; i++)
        {
            HeroAttack attack = (HeroAttack)mineGroup.fightUnits[i].attack;
            if (attack.specailSkill == null || attack.specailSkill.specialSkill.skillType == (uint)SkillType.Cure)
                continue;
            heroAttacks.Add(attack);
        }
        heroAttacks.Sort(delegate(HeroAttack a, HeroAttack b){ return a.specailSkill.specialSkill.weight.CompareTo(b.specailSkill.specialSkill.weight);});
        int usedNum = 0;
        for (int i = heroAttacks.Count - 1; i >= 0; i--)
        {
            if (Random.value * 10000 <= heroAttacks[i].specailSkill.specialSkill.weight * DecisionFactorDic[usedNum + 1].factor)
            {
                if (heroAttacks[i].self.state == FightUnit.UnitState.Fighting)
                {
                    usedNum++;
                    heroAttacks[i].AutoUseUniqueSkill();
                }
            }
        }
        if (usedNum == 0 && heroAttacks.Count > 0 && heroAttacks[0].self.state == FightUnit.UnitState.Fighting)
        {
            heroAttacks[0].AutoUseUniqueSkill();
        }
    }
}
