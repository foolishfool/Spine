using UnityEngine;
using System.Collections;

public class tmpKongxi : MonoBehaviour {
    public GameObject KongxiObj;
    public TweenPostionPlus tw;
    public float delay = 20f;
    public float space = 20f;
    public float moveTime = 10f;
    public Transform left;
    public Transform Right;


    void Start()
    {
        InvokeRepeating("Kongxi", delay, space);
    }

    void Kongxi()
    {
        GameObject obj = Util.AddChild(KongxiObj,transform);
        obj.SetActive(true);
        tw = obj.GetComponentInChildren<TweenPostionPlus>();
        if (Random.value <= 0.5f)
        {
            tw.from = left.transform.localPosition;
            tw.to = Right.transform.localPosition;
        }
        else
        {
            tw.from = Right.transform.localPosition;
            tw.to = left.transform.localPosition;
            obj.transform.localRotation = Quaternion.Euler(0,-180,0);
        }
        tw.duration = moveTime;
        tw.enabled = true;       
    }

    public void OnClick(GameObject obj)
    {
        Bomb bomb = obj.GetComponentInChildren<Bomb>();
        bomb.FreeBomb();
    }

    public void OnFinish(GameObject obj)
    {
        Destroy(obj);
    }
}
