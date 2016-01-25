using UnityEngine;
using System.Collections;

public class ScrollScene : MonoBehaviour {
    UIScrollView scrollView;
    public float speed = 100f;
    void Start()
    {
        scrollView = GetComponent<UIScrollView>();
        this.enabled = false;
    }

    void Update()
    {
       scrollView.MoveRelative(Vector3.left * Mathf.RoundToInt(speed * Time.deltaTime));
    }	
}
