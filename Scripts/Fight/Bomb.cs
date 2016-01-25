using UnityEngine;
using System.Collections;
/// <summary>
/// 炸弹
/// </summary>
[RequireComponent(typeof(FreeFall))]
public class Bomb : MonoBehaviour {
    public float range;
    public int damage;
    /// <summary>
    /// 未爆炸炸弹特效
    /// </summary>
    public GameObject beforeCrash;
    /// <summary>
    /// 爆炸特效
    /// </summary>
    public GameObject crash;

    FreeFall freeFall;

    void Start()
    {
        freeFall = GetComponent<FreeFall>();
        freeFall.enabled = false;
        freeFall.OnHitFloor = OnHit;
        crash.SetActive(false);
        beforeCrash.SetActive(true);
    }

    void OnHit()
    {
        beforeCrash.SetActive(false);
        crash.SetActive(true);
        Invoke("DestroySelf",3f);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void FreeBomb()
    {
        freeFall.enabled = true;
        transform.parent = KongxiManager.instance.transform;
    }
}
