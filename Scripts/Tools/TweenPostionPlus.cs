using UnityEngine;
using System.Collections;
/// <summary>
/// 值为世界坐标
/// </summary>
public class TweenPostionPlus : UITweener {
	public Vector3 from;
	public Vector3 to;

	Transform mTrans;

	public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

	[System.Obsolete("Use 'value' instead")]
	public Vector3 position { get { return this.value; } set { this.value = value; } }

	/// <summary>
	/// Tween's current value.
	/// </summary>

	public Vector3 value
	{
		get
		{
            return cachedTransform.position;
		}
		set
		{
			cachedTransform.position = value;
		}
	}

    protected override void Start()
    {
        Vector3 orinPos = value;
        from = orinPos + cachedTransform.parent.TransformPoint(from);
        to = orinPos + cachedTransform.parent.TransformPoint(to);
        base.Start();
    }

	/// <summary>
	/// Tween the value.
	/// </summary>

	protected override void OnUpdate (float factor, bool isFinished) { value = from * (1f - factor) + to * factor; }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenPosition Begin (GameObject go, float duration, Vector3 pos)
	{
		TweenPosition comp = UITweener.Begin<TweenPosition>(go, duration);
		comp.from = comp.value;
		comp.to = pos;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}
}
