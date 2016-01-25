using UnityEngine;
using System.Collections;
using ProtoTblConfig;
/// <summary>
/// 投射物运动（直线或者抛物线）
/// </summary>
public class ProjectileMove : MonoBehaviour {
    public Vector3 _targetPos;
    public Transform _targetTrans;
    Vector3 TargetPos
    {
        get
        {
            if (_targetTrans == null)
                return _targetPos;
            else
                return _targetTrans.parent.localPosition + _targetTrans.localPosition;
        }
    }
    Vector3 orinPos;
    float orinyV0;
    //水平方向的位移
    public float xSpeed;
    public Vector3 xPos;

    //竖直方向的位移
    public float yLength;
    public float a;//加速度
    public float yV0;//初始速度 
    public float yPos;
    public void CaculateA()
    {
        float xS = Mathf.Abs(transform.localPosition.x - _targetPos.x);
        a = 8 * yLength * xSpeed * xSpeed / (xS * xS);
        yV0 = a * xS / (2 * xSpeed);
    }

    public SpecialEffect hitEffect;

    Transform mTrans;

    float time;

    public delegate void ProjectileHandler(SpecialEffect effect);
    public ProjectileHandler OnProjectileReach;

    void Start()
    {
        mTrans = transform;
        time = Time.time;
        xPos = mTrans.localPosition;
        orinPos = xPos;
        Vector3 direct = TargetPos - orinPos;
        orinyV0 = direct.y / Mathf.Abs(direct.x) * xSpeed;
    }

    void Update()
    {
        //计算y方向位置
        if (yLength > 0)
        {
            yV0 -= a * Time.deltaTime;
            yPos += yV0 * Time.deltaTime;
        }
        //计算x方向位置
        xPos = Vector3.MoveTowards(xPos,TargetPos,xSpeed * Time.deltaTime);

        mTrans.localPosition = new Vector3(xPos.x, xPos.y + yPos, xPos.z);

        //Vector3 lookDire = new Vector3(TargetPos.x > orinPos.x ? xSpeed : -xSpeed, yV0 + orinyV0, 0);
        float angle = Mathf.Atan((yV0 + orinyV0) / xSpeed) / Mathf.Deg2Rad;

        mTrans.localRotation = Quaternion.Euler(0, TargetPos.x > orinPos.x ? 0 : 180, angle);

        if (xPos == TargetPos)
        {
            if (OnProjectileReach != null)
                OnProjectileReach(hitEffect);
            Destroy(gameObject);
        }
        if (Time.time  - time > 20f)
            Destroy(gameObject);
    }


    /// <summary>
    /// 抛物线投射物
    /// </summary>

    public static GameObject CreateProjectile(string goName,Transform parent,Vector3 goPos,float xSpeed,float yLength,Vector3 targetPos,ProjectileHandler callBack)
	{
        GameObject obj = new GameObject(goName);
        obj.transform.parent = parent;
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = goPos;
        ProjectileMove move = obj.AddComponent<ProjectileMove>();
        move._targetPos = targetPos;
        move.xSpeed = xSpeed;
        move.yLength = yLength;
        move.OnProjectileReach = callBack;
        move.CaculateA();
        return obj;
    }

    /// <summary>
    /// 直线投射物
    /// </summary>

    public static GameObject CreateProjectile(string goName, Transform parent, Vector3 goPos, float xSpeed, Transform targetTrans, ProjectileHandler callBack)
    {
        GameObject obj = new GameObject(goName);
        obj.transform.parent = parent;
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = goPos;
        ProjectileMove move = obj.AddComponent<ProjectileMove>();
        move._targetTrans = targetTrans;
        move._targetPos = targetTrans.parent.localPosition + targetTrans.localPosition;
        move.xSpeed = xSpeed;
        move.yLength = 0;
        move.OnProjectileReach = callBack;
        return obj;
    }
}
