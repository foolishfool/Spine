using System;
using System.Collections;
using UnityEngine;

public class LoginButton : BaseButton
{
    void Start()
    {

    }

    private void OnClick()
    {
		ViewMapper<LoginPanel>.instance.TurnTo ();
    }

}
