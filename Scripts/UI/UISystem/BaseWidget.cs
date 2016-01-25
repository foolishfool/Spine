using UnityEngine;
using System.Collections;
/// <summary>
/// 控件基类，控件只处理显示，禁止直接与逻辑交互，交互的代码统一写在控件的视图组件上
/// </summary>
public class BaseWidget : MonoBehaviour {
    /// <summary>
    /// 控件命名的结尾
    /// </summary>
    public const string nameExtension = "";
    /// <summary>
    /// 控件事件定义
    /// </summary>
    /// <param name="objs"></param>
    public delegate void WidgetEvent(params object[] objs);
    public delegate bool WidgetCheckEvent(params object[] objs);

    void Awake()
    {
        Transform trans = transform;
        while (trans.parent != null)
        {
            trans = trans.parent;
            BaseView v = trans.GetComponent<BaseView>();
            if (v != null)
            {
                v.widgetsMap.Add(gameObject.name,this);
                break;
            }
        }
#if UNITY_EDITOR
        if (!gameObject.name.Contains(nameExtension))
            Debug.LogError("gameobject..." + gameObject.name + "'s   nameExtension is wrong!");
#endif
    }
}
