using UnityEngine;
using System.Collections;

public class DisplayHealthBar : MonoBehaviour {
    float time;

    void Start()
    {
        TweenAlpha.Begin(gameObject, 0, 0);
    }

    void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            if (time <= 0)
                Hide();
        }
    }
    /// <summary>
    /// 受到攻击显现血条，过段时间消失
    /// </summary>
    public void GetHurtDisplay()
    {
        time = Const.HealthBarDisplayTime;
        Display();
    }

    public void Display()
    {
        TweenAlpha.Begin(gameObject, 0.2f, 1);
    }

    public void Hide()
    {
        TweenAlpha.Begin(gameObject, 0.2f, 0);
    }
}
