using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class ExoportPackage : Editor
{	
	/*[MenuItem("Custom Editor/Create AssetBunldes Main")]
	static void CreateAssetBunldesMain ()
	{
        //获取在Project视图中选择的所有游戏对象
		Object[] SelectedAsset = Selection.GetFiltered (typeof(Object), SelectionMode.DeepAssets);
 
        //遍历所有的游戏对象
		foreach (Object obj in SelectedAsset) 
		{
			//本地测试：建议最后将Assetbundle放在StreamingAssets文件夹下，如果没有就创建一个，因为移动平台下只能读取这个路径
			//StreamingAssets是只读路径，不能写入
			//服务器下载：就不需要放在这里，服务器上客户端用www类进行下载。
			string targetPath = Application.dataPath + "/StreamingAssets/" + obj.name + ".assetbundle";
			if (BuildPipeline.BuildAssetBundle (obj, null, targetPath, BuildAssetBundleOptions.CollectDependencies)) {
  				Debug.Log(obj.name +"资源打包成功");
			} 
			else 
			{
 				Debug.Log(obj.name +"资源打包失败");
			}
		}
		//刷新编辑器
		AssetDatabase.Refresh ();	
		
	}*/
	
	[MenuItem("Custom Editor/Create Unity3D")]
	static void CreateAssetBunldesALL ()
	{		
		Caching.CleanCache ();
		Object[] SelectedAsset = Selection.GetFiltered (typeof(Object), SelectionMode.Unfiltered);
        string Path = Application.streamingAssetsPath;
        Path = EditorUtility.OpenFolderPanel("save", Path, "");
        if (string.IsNullOrEmpty(Path))
            return;
		foreach (Object obj in SelectedAsset) 
		{
			Debug.Log ("Create AssetBunldes name :" + obj);                      
            BuildPipeline.BuildAssetBundle(obj, null, Path + "/" + obj.name + ".unity3d", BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, BuildTarget.Android);
		}		
        AssetDatabase.Refresh();
	}
    [MenuItem("Custom Editor/Create Single Unity3D")]
    static void CreateSingleAssetBundle()
    {
        Caching.CleanCache();

        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        foreach (Object obj in SelectedAsset)
        {
            Debug.Log("Create AssetBunldes name :" + obj);
        }
        string Path = Application.streamingAssetsPath;
        Path = EditorUtility.SaveFilePanel("save", Path, "", "unity3d");

        //这里注意第二个参数就行
        if (BuildPipeline.BuildAssetBundle(null, SelectedAsset, Path, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, BuildTarget.Android))
        {
            AssetDatabase.Refresh();
        }
    }
    //[MenuItem("Custom Editor/Create Scene")]
    //static void CreateSceneALL ()
    //{
    //    //清空一下缓存
    //    Caching.CleanCache();
    //    string Path = Application.dataPath + "/MyScene.unity3d";
    //    string[] levels = { "Assets/Scenes/Level.unity", "Assets/Scenes/Level2.unity" };
    //    //打包场景
    //    //BuildPipeline.BuildPlayer( levels, Path,BuildTarget.WebPlayer, BuildOptions.BuildAdditionalStreamedScenes);
    //    BuildPipeline.BuildStreamedSceneAssetBundle(levels, Path, BuildTarget.WebPlayer);//BuildTarget.Andrdoid
    //    AssetDatabase.Refresh ();
    //}

    //[MenuItem("Custom Editor/Build AssetBundles From Directory of Files")]
    //static void ExportAssetBundles()
    //{
    //    string path = AssetDatabase.GetAssetPath(Selection.activeObject);

    //    if (path.Length != 0)
    //    {

    //        path = path.Replace("Assets/", "");

    //        string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);

    //        foreach (string fileName in fileEntries)
    //        {

    //            string filePath = fileName.Replace("\\", "/");

    //            int index = filePath.LastIndexOf("/");

    //            filePath = filePath.Substring(index + 1);

    //            string localPath = "Assets/" + path + "/";

    //            if (index > 0)

    //                localPath += filePath;

    //            Object t = AssetDatabase.LoadMainAssetAtPath(localPath);

    //            if (t != null)
    //            {
    //                string bundlePath = "Assets/" + "StreamingAssets/" + t.name + ".unity3d";
    //                //从激活的选择编译资源文件  

    //                BuildPipeline.BuildAssetBundle

    //                (t, null, bundlePath, BuildAssetBundleOptions.CompleteAssets,BuildTarget.WebPlayer );
    //            }
    //        }
    //    }
    //}

    //[MenuItem("Custom Editor/Build AssetBundle From Selection")]
    //static void  ExportResource () {
    // // Bring up save panel
    // string path = EditorUtility.SaveFilePanel ("Save Resource", "", "test", "unity3d");
    // if (path.Length != 0)
    // {
    //      // Build the resource file from the active selection.
    //      var selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
  
    //      BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets); 
    //      Selection.objects = selection;
    // }
    //}
}
