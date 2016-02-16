using UnityEngine;
using System.Collections;

public class ScrollSceneManager : MonoBehaviour {
    public ScrollScene[] scrollScenes;
    public static ScrollSceneManager instance;
	public bool isMoveToRight = false;
//	public Transform BackGround;
//	public float distance;
//	public Vector3 backgroundPos;

    void Start()
	{
		instance = this;
		UISprite background = instance.transform.GetChild(0).GetComponent<UISprite>();
        scrollScenes = GetComponentsInChildren<ScrollScene>();    
		background.depth = -1;
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

	void Update()
	{
		UISprite background = instance.transform.GetChild(0).GetComponent<UISprite>();
	}
	//计算当前位置差距
//	public void CaculateBackgroundParameter()
//	{
//		distance = Util.Distance (FightManager.GetInstance().mineGroup.FirstUnit.mTrans.localPosition, FightManager.GetInstance().mineGroup.middle_point.parent.localPosition);
//		backgroundPos = BackGround.localPosition;
//	}
//
//	//背景如何运动	
//	public void BackGroundMoveToward()
//	{
//		if ((FightManager.GetInstance().mineGroup.FirstUnit.mTrans.localPosition.x <= (FightManager.GetInstance().mineGroup.middle_point.localPosition.x + FightManager.GetInstance().mineGroup.middle_point.parent.localPosition.x)
//			&& !(BackGround.localPosition.x < FightManager.GetInstance().mineGroup.backGroundLEndPos.localPosition.x)))
//		{
//			backgroundPos.x -= distance;
//			//平滑运动
//			float posX = Mathf.Lerp (BackGround.localPosition.x, backgroundPos.x, 0.05f);
//			float posY = BackGround.localPosition.y;
//			float posZ = BackGround.localPosition.z;
//			BackGround.localPosition = new Vector3 (posX, posY, posZ);			
//		}
//
//	}
//
//	public void BackGroundMoveBack()
//	{
//		if (FightManager.GetInstance().mineGroup.FirstUnit.mTrans.localPosition.x > (FightManager.GetInstance().mineGroup.middle_point.localPosition.x + FightManager.GetInstance().mineGroup.middle_point.parent.localPosition.x))
//		{
//			if (!(BackGround.localPosition.x > FightManager.GetInstance().mineGroup.backGroundREndPos.localPosition.x))
//			{
//				backgroundPos.x += distance;
//				float posX = Mathf.Lerp (BackGround.localPosition.x, backgroundPos.x, 0.05f);
//				float posY = BackGround.localPosition.y;
//				float posZ = BackGround.localPosition.z;
//				BackGround.localPosition = new Vector3 (posX, posY, posZ);
//			}
//		}	
//	}

}
