using UnityEngine;
using System.Collections;

public class BaseButton : MonoBehaviour {

    private Transform trans;

    public void OnClick()
    {
       
    }
	// Use this for initialization
	void Start ()
    {
        this.trans = base.transform;
    }
}
