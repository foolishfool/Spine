using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoTblConfig;

public class LoadScene : MonoBehaviour {
    static string SceneName;
	static bool isAdditiveLoad = false;
    AsyncOperation async;
    public UIProgressBar bar;

    public static GameObject obj;
    void Start()
    {
        if (obj != null)
            Destroy(obj);
        obj = gameObject;        
        //AssetManager.UnloadAssets();
        async = isAdditiveLoad ? Application.LoadLevelAdditiveAsync(SceneName) : Application.LoadLevelAsync(SceneName);
    }

    void Update()
    {
        if (bar != null)
            bar.value = async.progress;
    }

    public static void LoadDungeon(int id)
    {
		isAdditiveLoad = true;
        SceneName = Const.FightLevel;
      //  SceneName = "test1";
        DungeonManager.SceneSetting = Util.GetDic<MsgDungeon, Dungeon>()[id];
        Application.LoadLevel("LoadScene");
    }

    public static void ReturnMainmenu()
    {
        isAdditiveLoad = false;
        SceneName = Const.LoginLevel;
        Application.LoadLevel("LoadScene");
    }
}
