using UnityEngine;
using System.Collections;

public class ScrollSceneManager : MonoBehaviour {
    public ScrollScene[] scrollScenes;
    public static ScrollSceneManager instance;
	public bool isMoveToRight = false;
    void Start()
    {
        scrollScenes = GetComponentsInChildren<ScrollScene>();
        instance = this;
    }

    public void EnableScrollToLeft()
    {
        for (int i = 0; i < scrollScenes.Length; i++)
        {
            scrollScenes[i].enabled = true;
			isMoveToRight = false;
        }
    }

	public void EnableScrollToRight()
	{
		for (int i = 0; i < scrollScenes.Length; i++)
		{
			scrollScenes[i].enabled = true;
			isMoveToRight = true;
		}
	}


    public void DisableScroll()
    {
        for (int i = 0; i < scrollScenes.Length; i++)
        {
            scrollScenes[i].enabled = false;
        }
    }
}
