using UnityEngine;
using System.Collections;

public class HurtNumEmitter : MonoBehaviour {
    public GameObject hurtLabelObj;
    public Color hurtColor = Color.red;
    public Color cureColor = Color.green;
    public int normalSize = 30;
    public int critSize = 40;
    public Queue queue = new Queue();
    public struct HurtNum
    {
        public string txt;//数字文本
        public bool isHurt;//是否是伤害数字
        public bool isCrit;//是否是暴击
    }
    bool bDestroy = false;
    float time;
    Transform mTrans;

    void Start()
    {
        mTrans = transform;
        time = Const.HurtNumEmitterTime;
    }

    public void AddHurtNum(string num,bool isHurt,bool isCrit)
    {
        HurtNum hurtNum = new HurtNum();
        hurtNum.txt = num;
        hurtNum.isHurt = isHurt;
        hurtNum.isCrit = isCrit;
        queue.Enqueue(hurtNum);
    }

    public void TryDestroy()
    {
        bDestroy = true;
    }

    void Update()
    {
        if (queue.Count > 0)
        {
            time += Time.deltaTime;
            if (time >= Const.HurtNumEmitterTime)
            {
                time = 0;
                HurtNum hurtNum = (HurtNum)queue.Dequeue();
                GameObject obj = Util.AddChild(hurtLabelObj, mTrans.parent);
                obj.transform.position = mTrans.position;
                obj.SetActive(true);
                UILabel label = obj.GetComponent<UILabel>();
                label.text = hurtNum.txt;
                if (hurtNum.isHurt)
                    label.color = hurtColor;
                else
                    label.color = cureColor;
                if (hurtNum.isCrit)
                    label.fontSize = critSize;
                else
                    label.fontSize = normalSize;
            }            
        }
        else if (bDestroy)
        {
            Destroy(gameObject);
        }
    }
}
