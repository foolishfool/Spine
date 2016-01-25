using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssetManager{
    //正在下载的www
    public static Dictionary<string, WWW> DownLoadingDic = new Dictionary<string, WWW>();
    //已解压的资源
    public static Dictionary<string, AssetData> CachedAsset = new Dictionary<string, AssetData>();

    public struct AssetData
    {
        public Object asset;
        public bool isKeep;//是否常驻内存
    }

    public enum AssetType
    {
        Scene,
        Model,
        UI,
        Effect,
        Audio,
    }
    /// <summary>
    /// 释放所有资源
    /// </summary>
    public static void UnloadAssets()
    {
        foreach (WWW www in DownLoadingDic.Values)
            www.Dispose();
        DownLoadingDic.Clear();
        foreach (string assetName in CachedAsset.Keys)
        {
            if (!CachedAsset[assetName].isKeep)
            {
                if (CachedAsset[assetName].asset)
                {
                    Debug.LogError("assetname..... " + assetName + "    " + CachedAsset[assetName].asset);
                    Resources.UnloadAsset(CachedAsset[assetName].asset);
                    CachedAsset.Remove(assetName);
                }
            }
        }
    }

    /// <summary>
    /// 加载资源，如果正在加载则等待，如果未加载则开始加载
    /// </summary>
    /// <param name="assetName">资源名</param>
    /// <param name="type">类型</param>
    /// <param name="isKeep">是否常驻内存</param>
    /// <returns></returns>
    public static IEnumerator LoadAsset(string assetName,AssetType type,bool isKeep)
    {
        if (!CachedAsset.ContainsKey(assetName))
        {
            if (DownLoadingDic.ContainsKey(assetName))
            {
                while (!CachedAsset.ContainsKey(assetName))
                    yield return null;
            }
            else
            {
                string subPath = "";
                switch (type)
                {
                    case AssetType.Model:
                        subPath = "Model/";
                        break;
                    case AssetType.UI:
                        subPath = "UI/";
                        break;
                    case AssetType.Audio:
                        subPath = "Audio/";
                        break;
                    case AssetType.Scene:
                        subPath = "Scene/";
                        break;
                    case AssetType.Effect:
                        subPath = "Effect/";
                        break;
                    default:
                        break;
                }
                string assetPath =
//#if UNITY_EDITOR
                
				Util.AppContentDataUri + subPath + assetName + ".unity3d";
//#else 
//                Util.AppPersistentDataUri + subPath + assetName + ".unity3d";
//                Debug.LogError(Util.AppPersistentDataUri);
//#endif
                
//#if NO_STREAMINGASSETS 
#if UNITY_EDITOR

                Object asset = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Prefabs/" + subPath + assetName + ".prefab", typeof(GameObject));
                AssetData assetData = new AssetData();
                assetData.asset = asset;
                assetData.isKeep = isKeep;
                CachedAsset.Add(assetName,assetData);
#else
                lock (DownLoadingDic)
                {
                    WWW www = new WWW(assetPath);
                    DownLoadingDic.Add(assetName, www);
                    yield return www;
                    DownLoadingDic.Remove(assetName);
                    if (www.error != null)
                        Debug.LogError(www.error);
                    else
                    {
                        Object asset = www.assetBundle.mainAsset;
                        AssetData assetData = new AssetData();
                        assetData.asset = asset;
                        assetData.isKeep = isKeep;
                        CachedAsset.Add(assetName, assetData);
                        www.assetBundle.Unload(false);
                    }
                }
#endif
            }
        }
    }

    public static T GetAsset<T>(string assetName) where T:class
    {
        if (CachedAsset.ContainsKey(assetName))
            return CachedAsset[assetName].asset as T;
        Debug.LogError("Not Find Asset " + assetName + ",Ensure LoadAsset first");
        return null;
    }

    public static GameObject GetGameObject(string assetName)
    {
        return GetGameObject(assetName ,null);
    }

    public static GameObject GetGameObject(string assetName, Transform parent)
    {
        if (CachedAsset.ContainsKey(assetName) && CachedAsset[assetName].asset != null)
        {
            GameObject obj = GameObject.Instantiate(CachedAsset[assetName].asset) as GameObject;
            if (parent != null)
            {
                obj.name = assetName;
                obj.transform.parent = parent;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one;
            }
            return obj;
        }
        Debug.LogError("Not Find Asset " + assetName + ",Ensure LoadAsset first");
        return null;
    }
}
