using UnityEngine;
using System.Collections;

public class EvaluatePanel : BaseView {
    public override void Regester()
    {
        ViewMapper<EvaluatePanel>.instance = this;
    }

    public struct EvaluateData
    {
        public int timeScore;
        public int healthScore;
        public int comboScore;
        public int skillScore;
        public int deathScore;
        public int uniqueSkillScore;
        public int totalScore{ get { return timeScore + healthScore + comboScore + skillScore + deathScore + uniqueSkillScore; }}
    }

    public static EvaluateData data;

    int totalScore;

    void Start()
    {
        DisplayPanel();
        totalScore = data.totalScore;
        StartCoroutine(WaitToDuplicate());
    }

    void DisplayPanel()
    {
        LabelWidget shijianLabel = (LabelWidget)widgetsMap["shijian_Label"];
        shijianLabel.Value = data.timeScore.ToString();
        LabelWidget lianjiLabel = (LabelWidget)widgetsMap["lianji_Label"];
        lianjiLabel.Value = data.comboScore.ToString();
        LabelWidget xueliangLabel = (LabelWidget)widgetsMap["xueliang_Label"];
        xueliangLabel.Value = data.healthScore.ToString();
        LabelWidget siwangLabel = (LabelWidget)widgetsMap["siwang_Label"];
        siwangLabel.Value = data.deathScore.ToString();
        LabelWidget jiqiaoLabel = (LabelWidget)widgetsMap["jiqiao_Label"];
        jiqiaoLabel.Value = data.skillScore.ToString();
        LabelWidget juejiLabel = (LabelWidget)widgetsMap["jueji_Label"];
        juejiLabel.Value = data.uniqueSkillScore.ToString();
    }

    public void ScrollTotalScore()
    {
        LabelWidget scoreLabel = (LabelWidget)widgetsMap["score_Label"];
        scoreLabel.Value = data.totalScore.ToString();
    }

    IEnumerator WaitToDuplicate()
    {
        yield return new WaitForSeconds(1.0f);
    }
}
