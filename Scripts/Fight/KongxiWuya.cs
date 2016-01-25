using UnityEngine;
using System.Collections;
[RequireComponent(typeof(UIWidget))]
public class KongxiWuya : MonoBehaviour{
    /// <summary>
    /// 乌鸦携带的炸弹
    /// </summary>
    Bomb bomb;
    /// <summary>
    /// 目标扔炸弹点
    /// </summary>
    public Vector3 target;
    /// <summary>
    /// 飞行起始点
    /// </summary>
    public Vector3 from;
    /// <summary>
    /// 飞行终点
    /// </summary>
    public Vector3 to;
    /// <summary>
    /// 飞行速度
    /// </summary>
    public float speed;

    Transform mTrans;
    bool hasFreeBomb = false;
    UIWidget ui;

    void Start()
    {
        bomb = GetComponentInChildren<Bomb>();
        mTrans = transform;
        mTrans.localPosition = from;
        CEventDispatcher.GetInstance().AddEventListener(CEventType.MOVE_TO_NEXT, OnMoveToNext);
        ui = GetComponent<UIWidget>();
        DragClick.instance.AddListenUI(ui);
    }

    void OnDestroy()
    {
        if (DragClick.hasInstance)
            DragClick.instance.RemoveListenUI(ui);
        CEventDispatcher.GetInstance().RemoveEventListener(CEventType.MOVE_TO_NEXT, OnMoveToNext);
        if (!hasFreeBomb)
            DungeonRecord.unKillWuya++;
    }

    void OnMoveToNext(CBaseEvent evt)
    {
        Destroy(gameObject);
    }

    void OnClick()
    {
        if (!hasFreeBomb)
        {
            hasFreeBomb = true;
            bomb.FreeBomb();
        }
    }

    void Update()
    {
        if (hasFreeBomb)
        {
            mTrans.localPosition = Vector3.MoveTowards(mTrans.localPosition, to, speed * Time.deltaTime);
            if (mTrans.localPosition == to)
                Destroy(gameObject);
        }
        else
        {
            mTrans.localPosition = Vector3.MoveTowards(mTrans.localPosition, target, speed * Time.deltaTime);
            if (mTrans.localPosition == target)
            {
                hasFreeBomb = true;
                bomb.FreeBomb();
            }
        }
    }
}
