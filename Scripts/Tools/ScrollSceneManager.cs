using UnityEngine;
using System.Collections;

public class ScrollSceneManager : MonoBehaviour {
    public ScrollScene[] scrollScenes;
    public static ScrollSceneManager instance;

    void Start()
    {
        scrollScenes = GetComponentsInChildren<ScrollScene>();
        instance = this;
    }

    public void EnableScroll()
    {
        for (int i = 0; i < scrollScenes.Length; i++)
        {
            scrollScenes[i].enabled = true;
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
