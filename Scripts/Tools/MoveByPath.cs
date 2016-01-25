using UnityEngine;
using System.Collections;
using Holoville.HOTween;
using Holoville.HOTween.Core;
using Holoville.HOTween.Path;
using Holoville.HOTween.Plugins;
using Holoville.HOTween.Plugins.Core;
[RequireComponent(typeof(HOPath))]
public class MoveByPath : MonoBehaviour {
    public float time = 30;
    public LoopType type = LoopType.Restart;
    public EaseType easeType = EaseType.Linear;
    void Start()
    {
        HOTween.Init(true, true, true);
        HOPath path = GetComponent<HOPath>();
        TweenParms param = new TweenParms();
        param.Prop("position", path.MakePlugVector3Path(), true);
        param.Loops(-1, type);
        param.Ease(easeType);
        HOTween.To(transform, time, param);
    }
}
