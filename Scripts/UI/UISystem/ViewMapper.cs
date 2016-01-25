using UnityEngine;
using System.Collections;
/// <summary>
/// 视图索引类
/// </summary>
/// <typeparam name="T"></typeparam>
public class ViewMapper<T> where T : BaseView
{
    static T _instance;

    public static T instance
    {
        get
        {
            return _instance;
        }
        set
        {
            if (_instance != null)
                GameObject.Destroy(_instance.gameObject);
            _instance = value;
        }
    }
}
