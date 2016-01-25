using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 拖拽过程中每帧触发（用于切水果类型的响应）
/// </summary>
public class DragClick : MonoBehaviour {
    public Vector3 halfExtens;
    public List<UIWidget> UIs = new List<UIWidget>();
    public List<UIWidget> toClickUIs = new List<UIWidget>();
    bool isDraging = false;
    bool isCheckingDrag = false;
    Vector3 lastMousePos;
    Vector3 curMousePos;

    static DragClick _instance;
    public static DragClick instance
    {
        get
        {
            if(!_instance)
                _instance = new GameObject("DragClick").AddComponent<DragClick>();
            return _instance;
        }
    }

    public static bool hasInstance { get { return _instance != null; } }

    void Start()
    {
        halfExtens = new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0);
    }

    void Update()
    {
        if (Application.isEditor)
        {
            if (Input.GetMouseButtonDown(0) && !isDraging)
            {
                isCheckingDrag = true;
                lastMousePos = Input.mousePosition;
                toClickUIs.Clear();
                for (int i = 0; i < UIs.Count; i++)
                    toClickUIs.Add(UIs[i]);
            }
            if (Input.GetMouseButtonUp(0))
            {
                isDraging = false;
                isCheckingDrag = false;
            }
            curMousePos =  Input.mousePosition;
            if (isCheckingDrag && lastMousePos != curMousePos)
            {
                isCheckingDrag = false;
                lastMousePos = curMousePos;
                isDraging = true;
            }          
        }
        else
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.touches[0];
                if (touch.phase == TouchPhase.Began)
                {
                    lastMousePos = touch.position;
                    toClickUIs.Clear();
                    for (int i = 0; i < UIs.Count; i++)
                        toClickUIs.Add(UIs[i]);
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    curMousePos = touch.position;
                    isDraging = true;
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    isDraging = false;
            }
            else
                isDraging = false;
        }
        if (isDraging)
        {
            if (lastMousePos != curMousePos)
            {
                for (int i = toClickUIs.Count - 1; i >= 0; i--)
                {
                    if (toClickUIs[i] == null)
                        continue;
                    if (IsDragClick(lastMousePos, curMousePos, toClickUIs[i]))
                    {
                        toClickUIs[i].SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
                        toClickUIs.RemoveAt(i);
                    }
                }
                lastMousePos = curMousePos;
            }           
        }
    }

    public void AddListenUI(UIWidget ui)
    {
        UIs.Add(ui);
    }

    public void RemoveListenUI(UIWidget ui)
    {
        UIs.Remove(ui);
    }

    public static bool IsDragClick(Vector3 from, Vector3 to, UIWidget ui)
    {
        from -= instance.halfExtens;
        to -= instance.halfExtens;
        Bounds bd = ui.CalculateBounds(UIRoot.list[0].transform);

        //如果有一个点在矩形内就算划到
        if (bd.Contains(from) || bd.Contains(to))
            return true;
        float x0 = bd.center.x - bd.extents.x;
        float x1 = bd.center.x + bd.extents.x;
        float y0 = bd.center.y - bd.extents.y;
        float y1 = bd.center.y + bd.extents.y;
        //如果两点都在矩形的同一个边的外面，则必定没有划到
        if ((from.x < x0 && to.x < x0) ||
            (from.x > x1 && to.x > x1) ||
            (from.y < y0 && to.y < y0) ||
            (from.y > y1 && to.y > y1))
            return false;
        //如果两点不在同一边外面，则判断两点所在直线与矩形的四边所在直线的交点是否落在矩形边上
        //y = ax + b 或者 x = c
        float a, b ,c;
        if (from.x == to.x)
        {
            c = from.x;
            return c >= x0 && c <= x1;
        }
        a = (from.y - to.y) / (from.x - to.x);
        b = from.y - a * from.x;

        if (a == 0)
            return b >= y0 && b <= y1;
        float x0_y = a * x0 + b;
        if (x0_y >= y0 && x0_y <= y1)
            return true;
        float x1_y = a * x1 + b;
        if (x1_y >= y0 && x1_y <= y1)
            return true;
        float y0_x = (y0 - b) / a;
        if (y0_x >= x0 && y0_x <= x1)
            return true;
        float y1_x = (y1 - b) / a;
        if (y1_x >= x0 && y1_x <= x1)
            return true;
        return false;
    }
}
