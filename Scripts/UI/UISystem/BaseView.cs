using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 视图基类
/// </summary>
public class BaseView : MonoBehaviour {

    /// <summary>
    /// 视图地下的控件的索引
    /// </summary>
    public Dictionary<string, BaseWidget> widgetsMap = new Dictionary<string, BaseWidget>();

    /// <summary>
    /// 父视图
    /// </summary>
    public BaseView parentView;

    /// <summary>
    /// 子视图
    /// </summary>
    public Dictionary<string, BaseView> childViewsMap = new Dictionary<string, BaseView>();

    void Awake()
    {
        Transform trans = transform;
        while (trans.parent != null)
        {
            trans = trans.parent;
            BaseView v = trans.GetComponent<BaseView>();
            if (v != null)
            {
                parentView = v;
                v.childViewsMap.Add(gameObject.name, this);
                break;
            }
        }
        Regester();
    }

    /// <summary>
    /// 注册视图索引
    /// </summary>
    public virtual void Regester() { }
}
