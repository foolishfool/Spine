using UnityEngine;
using System.Collections;
[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class FrameAnimator : MonoBehaviour {
    /// <summary>
    /// 延时
    /// </summary>
    public float delay = 0;
    /// <summary>
    /// 行
    /// </summary>
    public int X = 1;
    /// <summary>
    /// 列
    /// </summary>
    public int Y = 1;
    /// <summary>
    /// 总帧数
    /// </summary>
    public int frameCount;
    /// <summary>
    /// 面片宽度
    /// </summary>
    public int width = 0;
    /// <summary>
    /// 面片高度
    /// </summary>
    public int height = 0;
    /// <summary>
    /// 面片偏移量
    /// </summary>
    public Vector3 center;
    /// <summary>
    /// 一秒播放几帧
    /// </summary>
    public int frameSpeed = 30;
    /// <summary>
    /// 是否循环播放
    /// </summary>
    public bool isLoop = false;
    /// <summary>
    /// 是否保持自身旋转
    /// </summary>
    public bool isKeepRotation = false;
    /// <summary>
    /// 当前帧数，0开始
    /// </summary>
    public int frame = 0;

    float spaceTime;
    float lastFrameTime;
    int texWidth;
    int texHeight;
    int unitX;
    int unitY;

    MeshFilter filter;

    MeshRenderer render;

    Material material;

    Quaternion rotation;

    public void Start()
    {
        filter = GetComponent<MeshFilter>();
        render = GetComponent<MeshRenderer>();
        material = render.material;
        material.renderQueue = 3400;
        texWidth = material.mainTexture.width;
        texHeight = material.mainTexture.height;
        unitX = texWidth / X;
        unitY = texHeight / Y;
        if (width == 0)
            width = unitX;
        if (height == 0)
            height = unitY;
        if (frameCount == 0)
            frameCount = X * Y;
        spaceTime = (float)1 / frameSpeed;
        lastFrameTime = Time.time;
        material.SetTextureScale("_MainTex",new Vector2(1f / X,1f / Y));
        rotation = transform.rotation;
        ManageMesh();
        UpdateFrame();
        if (delay != 0)
        {
            enabled = false;
            render.enabled = false;
            Invoke("EnableAnim",delay);
        }
    }

    void EnableAnim()
    {
        enabled = true;
        render.enabled = true;
        lastFrameTime = Time.time;
    }

    void ManageMesh()
    {
        Vector3[] mVertices = new Vector3[]
        {
            center + new Vector3(-width * 0.5f,height * 0.5f,0),
            center + new Vector3(-width * 0.5f,-height * 0.5f,0),
            center + new Vector3(width * 0.5f,height * 0.5f,0),
            center + new Vector3(width * 0.5f,-height * 0.5f,0)
        };
        Vector2[] mUVs = new Vector2[]
        {
            new Vector2(0,1),
            new Vector2(0,0),
            new Vector2(1,1),
            new Vector2(1,0),
        };
        int[] mTriangles = new int[]
        {
            1,0,2,
            1,2,3
        };
        Vector3[] mNormals = new Vector3[]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        Mesh mMesh = new Mesh();
        mMesh.vertices = mVertices;
        mMesh.uv = mUVs;
        mMesh.triangles = mTriangles;
        mMesh.normals = mNormals;
        filter.mesh = mMesh;
    }
  
    void Update()
    {
        if (Time.time - lastFrameTime > spaceTime)
        {
            frame++;
            lastFrameTime = Time.time;  
            if (frame == frameCount)
            {
                if (isLoop)
                    frame = 0;
                else
                {
                    enabled = false;
                    render.enabled = false;
                    return;
                }
            }                     
            UpdateFrame();                        
        }
        if (isKeepRotation)
            transform.rotation = rotation;
    }

    void UpdateFrame()
    {
        float xOffset = (float)frame % X / X;
        float yOffset = 1f - (float)(frame / X + 1) / Y;
        material.SetTextureOffset("_MainTex",new Vector2(xOffset,yOffset));
    }
}
