using UnityEngine;
using System.Collections;
using ProtoTblConfig;
public class ScrollScene : MonoBehaviour {
    UIScrollView scrollView;
    public float speed = 180f;
//	public HeroFightGroup heroFightGroup;

    void Start()
    {
        scrollView = GetComponent<UIScrollView>();
        this.enabled = false;
    }

    void Update()
    {
		if (ScrollSceneManager.instance.isMoveToRight == false)
			scrollView.MoveRelative (Vector3.right * Mathf.RoundToInt (speed * Time.deltaTime));
		else if (ScrollSceneManager.instance.isMoveToRight == true)
			scrollView.MoveRelative (Vector3.left * Mathf.RoundToInt (speed * Time.deltaTime));
    }	


}
