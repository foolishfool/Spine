using UnityEngine;
using System.Collections;

public class MovingBakManager : MonoBehaviour
{    
    public enum moveDirect
    {
        left,
        right,
    }
    [System.Serializable]
    public class MovingBak
    {
        public moveDirect direct;
        public float speed;
        public Transform trans;
        public float leftBorder;
        public float rightBorder;
        [HideInInspector]
        public Vector3 endPos;
        [HideInInspector]
        public Vector3 beginPos;
        public void Init()
        {
            if (direct == moveDirect.left)
            {
                endPos = new Vector3(leftBorder, trans.localPosition.y, trans.localPosition.z);
                beginPos = new Vector3(rightBorder, trans.localPosition.y, trans.localPosition.z);
            }
            else
            {
                endPos = new Vector3(rightBorder, trans.localPosition.y, trans.localPosition.z);
                beginPos = new Vector3(leftBorder, trans.localPosition.y, trans.localPosition.z);
            }
        }
    }
    public MovingBak[] movingBaks;

    void Awake()
    {
        for (int i = 0; i < movingBaks.Length; i++)
            movingBaks[i].Init();
    }

    void Update()
    {
        for (int i = 0; i < movingBaks.Length; i++)
        {
            movingBaks[i].trans.localPosition = Vector3.MoveTowards(movingBaks[i].trans.localPosition, movingBaks[i].endPos, movingBaks[i].speed * Time.deltaTime);
            if (movingBaks[i].trans.localPosition == movingBaks[i].endPos)
                movingBaks[i].trans.localPosition = movingBaks[i].beginPos;
        }
    }
}
