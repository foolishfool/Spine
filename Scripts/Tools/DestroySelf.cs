using UnityEngine;
using System.Collections;

public class DestroySelf : MonoBehaviour {
    public float DestroyTime = 1.5f;

    public static void DestroyAfterTime(GameObject obj,float time)
    {
        DestroySelf ins = obj.AddComponent<DestroySelf>();
        ins.DestroyTime = time;
    }

    void Awake()
    {
        Invoke("DoDestroy",DestroyTime);
    }
	
    void DoDestroy()
    {
        Destroy(gameObject);
    }
}
