using UnityEditor;
using UnityEngine;



/// <summary>  
/// 将选中的预制分别打包  
/// </summary>  

public class ExportAssetBundles


{
	[MenuItem("Assets/Create AssetBundles By themselves")]  

	static void CreateAssetBundleThemelves()
	{


	//获取要打包的对象（在Project视图中）  
		Object[] selects = Selection.GetFiltered (typeof(Object),SelectionMode.DeepAssets);  
	//遍历选中的对象  
		foreach(Object obj in selects)
		//这里建立一个本地测试  
		//注意本地测试中可以是任意的文件，但是到了移动平台只能读取路径StreamingAssets里面的  
		//StreamingAssets是只读路径，不能写入 
		{
			string chose_path = AssetDatabase.GetAssetPath(obj);
			string targetPath = "";
			if (chose_path == "Assets/Prefabs/Effect/" + obj.name + ".prefab")
				 targetPath = "Assets/StreamingAssets/Effect/" + obj.name + ".unity3d";//文件的后缀名是assetbundle和unity都可以  
			else if (chose_path == "Assets/Prefabs/Model/"+ obj.name + ".prefab") 
				 targetPath = "Assets/StreamingAssets/Model/" + obj.name + ".unity3d"; 	
			else if (chose_path == "Assets/Prefabs/Role/"+ obj.name + ".prefab") 
				 targetPath = "Assets/StreamingAssets/Role/" + obj.name + ".unity3d";
			else if (chose_path == "Assets/Prefabs/Scene/"+ obj.name + ".prefab") 
				 targetPath = "Assets/StreamingAssets/Scene/" + obj.name + ".unity3d"; 
			else if (chose_path == "Assets/Prefabs/UI/"+ obj.name + ".prefab") 
				targetPath = "Assets/StreamingAssets/UI/" + obj.name + ".unity3d"; 
			
			if( BuildPipeline.BuildAssetBundle(obj,null,targetPath,BuildAssetBundleOptions.CollectDependencies,BuildTarget.Android)) //the buildtarget is important!!!!!
			{  
			Debug.Log(obj.name + "is packed successfully!");  
			}
			else
			{  
			Debug.Log(obj.name + "is packed failly!");  
			}  
		}  
	//刷新编辑器（不写的话要手动刷新,否则打包的资源不能及时在Project视图内显示）  
	AssetDatabase.Refresh ();  
	}
}