using UnityEngine;
using System.Collections;

public class Combo : MonoBehaviour {
    static Combo instance;
    public static Combo GetInstance()
    {
        if (instance == null)
            instance = new GameObject("ComboManager").AddComponent<Combo>();
        return instance;
    }

    int combo = 0;
    float lastHitTime;
    bool isDisplaying = false;
    FightPanel fightPanel;

    void Start()
    {
        fightPanel = ViewMapper<FightPanel>.instance;
    }

    public void RegesterCombo(HeroFightUnit hero)
    {
        hero.OnSkillHit += AddCombo;
    }

    public void RemoveCombo(HeroFightUnit hero)
    {
        hero.OnSkillHit -= AddCombo;
    }
    
    void AddCombo(params object[] objs)
    {
        combo++;
        fightPanel.SetCombo(combo);
        if (!isDisplaying && combo > 1)
        {
            fightPanel.DisplayComboLabel();
            isDisplaying = true;
            if (DungeonRecord.maxCombo < combo)
                DungeonRecord.maxCombo = combo;
        }
        lastHitTime = Time.time;
    }

    void Update()
    {
        if (isDisplaying && Time.time > lastHitTime + Const.CONST_COMBO_TIME)
        {
            fightPanel.HideComboLabel();
            isDisplaying = false;
            combo = 0;
        }
    }
}
