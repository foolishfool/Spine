using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineAoeMove : MonoBehaviour {
    /// <summary>
    /// 运行方向
    /// </summary>
    public Vector3 direct;
    /// <summary>
    /// 速度
    /// </summary>
    public float xSpeed;
    /// <summary>
    /// 最远距离
    /// </summary>
    public float maxLength = 500f;
    /// <summary>
    /// 未作用过的单位
    /// </summary>
    public List<FightUnit> ToHitUnits = new List<FightUnit>();

    public float detectRange = 100f;

    public delegate void HitHandler(FightUnit unit);
    public HitHandler OnHit;

    Transform mTrans;

    float S;//以运动的路程

    void Start()
    {
        mTrans = transform;
    }

    void Update()
    {
        for (int i = ToHitUnits.Count - 1; i >= 0; i--)
        {
            if (Util.Distance(mTrans.localPosition, ToHitUnits[i].mTrans.localPosition) < detectRange)
            {
                if (OnHit != null)
                    OnHit(ToHitUnits[i]);
                ToHitUnits.RemoveAt(i);
            }
        }
        float v = xSpeed * Time.deltaTime;
        mTrans.localPosition += direct * v;
        S += v;
        if (S >= maxLength)
            Destroy(gameObject);
    }

    public static GameObject CreateLineAoe(string goName, Transform parent, Vector3 goPos, float xSpeed, bool isRight, HitHandler callBack)
    {
        GameObject obj = new GameObject(goName);
        obj.transform.parent = parent;
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = goPos;
        LineAoeMove move = obj.AddComponent<LineAoeMove>();
        move.xSpeed = xSpeed;
        move.direct = isRight ? Vector3.right : -Vector3.right;
        FightGroup targetGroup = isRight ? (FightGroup)FightManager.GetInstance().enemyGroup : FightManager.GetInstance().mineGroup;
        move.ToHitUnits = new List<FightUnit>(targetGroup.fightUnits);
        move.OnHit = callBack;
        return obj;
    }


    public static GameObject CreateLineAoe(string goName, Transform parent, Vector3 goPos, float xSpeed,float maxLength, bool isRight, HitHandler callBack)
    {
        GameObject obj = new GameObject(goName);
        obj.transform.parent = parent;
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = goPos;
        LineAoeMove move = obj.AddComponent<LineAoeMove>();
        move.xSpeed = xSpeed;
        move.direct = isRight ? Vector3.right : -Vector3.right;
        FightGroup targetGroup = isRight ? (FightGroup)FightManager.GetInstance().enemyGroup : FightManager.GetInstance().mineGroup;
        move.ToHitUnits = new List<FightUnit>(targetGroup.fightUnits);
        move.OnHit = callBack;
        move.maxLength = maxLength;
        return obj;
    }
}
