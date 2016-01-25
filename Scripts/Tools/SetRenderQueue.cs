using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class SetRenderQueue : MonoBehaviour {
    public int renderQueue = 3400;
    void Awake()
    {
        Renderer[] renders = GetComponentsInChildren<Renderer>();
        foreach (Renderer render in renders)
            render.sharedMaterial.renderQueue = renderQueue;
    }	
}
