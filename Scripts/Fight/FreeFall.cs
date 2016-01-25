using UnityEngine;
using System.Collections;
/// <summary>
/// 自由落体运动
/// </summary>
public class FreeFall : MonoBehaviour {
    public float g = 10f;//重力加速度
    public float ylimit = -0.38f;//小于这个值算是落地

    float v;
    Transform mTrans;
    public delegate void HitFloor();
    public HitFloor OnHitFloor;
    void Start()
    {
        mTrans = transform;
    }

    void Update()
    {
        v += g * Time.deltaTime;
        Vector3 pos = mTrans.localPosition;
        pos.y -= v * Time.deltaTime;
        mTrans.localPosition = pos;
        if (mTrans.position.y < ylimit)
        {
            enabled = false;
            if (OnHitFloor != null)
                OnHitFloor();
        }
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.GetComponent<FightUnit>() != null)
    //    {
    //        if (OnHitFloor != null)
    //        {
    //            enabled = false;
    //            collider.enabled = false;
    //            OnHitFloor();
    //        }
    //    }
    //}
	
}
