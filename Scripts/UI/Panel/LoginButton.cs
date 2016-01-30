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

		ViewMapper<LoginPanel>.instance.anim.Play("ValkyrieD_Attack2",false);
		NGUITools.PlaySound(ViewMapper<LoginPanel>.instance.audio.audioClip, 1, 1);
		ViewMapper<LoginPanel>.instance.anim_letter.Play();
		StartCoroutine (Wait (1f));


	
    }




	IEnumerator Wait(float waitTime)  
	{  
		
		yield return new WaitForSeconds(waitTime); 
		ViewMapper<LoginPanel>.instance.anim_letter.Play();
		ViewMapper<LoginPanel>.instance.TurnTo();

	}    
}
