using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.IO;
using LitJson;
using clientTBL;
using ProtoTblConfig;


public class Util : MonoBehaviour {

    public static T Get<T>(GameObject go, string subnode) where T : Component
    {
        if (go != null)
        {
            Transform transform = go.transform.FindChild(subnode);
            if (transform != null)
            {
                return transform.GetComponent<T>();
            }
        }
        return (T)((object)null);
    }
    public static T Get<T>(Transform go, string subnode) where T : Component
    {
        if (go != null)
        {
            Transform transform = go.FindChild(subnode);
            if (transform != null)
            {
                return transform.GetComponent<T>();
            }
        }
        return (T)((object)null);
    }
    public static T Get<T>(Component go, string subnode) where T : Component
    {
        return go.transform.FindChild(subnode).GetComponent<T>();
    }

    public static T Add<T>(GameObject go) where T : Component
    {
        if (go != null)
        {
            T[] components = go.GetComponents<T>();
            for(int i = 0; i < components.Length; i++)
            {
                if (components[i] != null)
                {
                    UnityEngine.Object.Destroy(components[i]);
                }
            }
            return go.gameObject.AddComponent<T>();
        }

        return (T)((object)null);
    }

    public static T Add<T>(Transform go) where T : Component
    {
        return Util.Add<T>(go.gameObject);
    }

    public static GameObject LoadPrefab(string path)
    {
        return Resource.LoadPrefab(path);
    }

	public static AudioSource LoadAudio(string path)
	{
		return Resource.LoadAudioClip (path);
	}

    public static string LoadText(string path)
    {
        return Util.LoadTextFile(path, ".txt");
    }

    public static string LoadXml(string path)
    {
        return Util.LoadTextFile(path, ".xml");
    }

    public static Texture2D LoadTexture(string path)
    {
        return Resource.LoadTexture(path);
    }

    public static string LoadTextFile(string path, string ext)
    {
        return Resource.LoadTextFile(path, ext);
    }

    public static GameObject Child(GameObject go, string subnode)
    {
        return Util.Child(go.transform, subnode);
    }

    public static GameObject Child(Transform go, string subnode)
    {
        Transform transform = go.FindChild(subnode);
        if (transform == null)
        {
            return null;
        }
        return transform.gameObject;
    }

    public static GameObject AddChild(GameObject prefab, Transform parent)
    {
        return NGUITools.AddChild(parent.gameObject, prefab);
    }

    public static GameObject Peer(GameObject go, string subnode)
    {
        return Util.Peer(go.transform, subnode);
    }

    public static GameObject Peer(Transform go, string subnode)
    {
        Transform transform = go.parent.FindChild(subnode);
        if (transform == null)
        {
            return null;
        }
        return transform.gameObject;
    }

    public static Uri AppContentDataUri
    {
        get
        {
            string dataPath = Application.dataPath;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return new UriBuilder
                {
                    Scheme = "file",
                    Path = Path.Combine(dataPath, "Raw/")
                }.Uri;
            }
            if (Application.platform == RuntimePlatform.Android)
            {
                return new Uri("jar:file://" + dataPath + "!/assets/");
            }
            return new UriBuilder
            {
                Scheme = "file",
				Path = Path.Combine(dataPath, "StreamingAssets/")
            }.Uri;
        }
    }

    public static string GetDataDir()
    {
        string dataPath = Application.dataPath;
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            string directoryName = Path.GetDirectoryName(Path.GetDirectoryName(dataPath));
            return Path.Combine(directoryName, "Documents/aiwan/");
        }
        if (Application.platform == RuntimePlatform.Android)
        {
            return "/sdcard/aiwan/";
        }
        else
            return dataPath;
    }

    public static Uri AppPersistentDataUri
    {
        get
        {
            return new UriBuilder{
                Scheme = "file",
                Path = Application.persistentDataPath + "/",
            }.Uri;
        }
    }

    public static void SafeDestroy(GameObject go)
    {
        if (go != null)
        {
            UnityEngine.Object.Destroy(go);
        }
    }
    public static void SafeDestroy(Transform go)
    {
        if (go != null)
        {
            Util.SafeDestroy(go.gameObject);
        }
    }

    public static void ClearMemory()
    {
        GC.Collect();
        Resources.UnloadUnusedAssets();
    }

    public static Vector3 StrToVector3(string position, char c)
    {
        string[] array = position.Split(new char[]
		{
			c
		});
        float x = Util.Float(array[0]);
        float y = Util.Float(array[1]);
        float z = Util.Float(array[2]);
        return new Vector3(x, y, z);
    }

    public static Color32 StrToColor(string strColor, char c)
    {
        string[] array = strColor.Split(new char[]
		{
			c
		});
        byte r = (byte)Util.Int(array[0]);
        byte g = (byte)Util.Int(array[1]);
        byte b = (byte)Util.Int(array[2]);
        return new Color32(r, g, b, 255);
    }

    public static float Float(object o)
    {
        return (float)Math.Round((double)Convert.ToSingle(o), 2);
    }

    public static int Int(object o)
    {
        return Convert.ToInt32(o);
    }

    public static long Long(object o)
    {
        return Convert.ToInt64(o);
    }

    public static void SetBackground(string background)
    {
        GameObject gameObject = GameObject.FindWithTag("BackGround");
        if (gameObject == null)
        {
            return;
        }
        UITexture component = gameObject.GetComponent<UITexture>();
        if (!string.IsNullOrEmpty(background))
        {
            Texture2D texture2D = Util.LoadTexture(background);
            component.mainTexture = texture2D;
            //gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
        else
        {
            component.mainTexture = null;
        }
    }

    public static int Random(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static float Random(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static float Distance(Vector3 from,Vector3 to)
    {
        return Mathf.Abs(from.x - to.x);
    }

    public static void UnloadTexture(GameObject go)
    {
        if (go != null)
        {
            UITexture component = go.GetComponent<UITexture>();
            if (component != null)
            {
                Resources.UnloadAsset(component.material.mainTexture);
            }
        }
    }

    public static void UnloadTexture(UITexture texture)
    {
        if (texture == null)
        {
            return;
        }

        Resources.UnloadAsset(texture.material.mainTexture);
    }

    public static void Load<MsgT, T>()
        where MsgT : class, ProtoBuf.IExtensible, new()
        where T : class, ProtoBuf.IExtensible, new()
    {
        SingletonHolder<DataManager<MsgT, T>>.Instance.refresh("TblResource/" + typeof(T).Name);
    }

    public static List<T> GetList<MsgT, T>()
        where MsgT : class, ProtoBuf.IExtensible, new()
        where T : class, ProtoBuf.IExtensible, new()
    {
        return SingletonHolder<DataManager<MsgT, T>>.Instance.DataPool;
    }

    public static Dictionary<int,T> GetDic<MsgT, T>()
        where MsgT : class, ProtoBuf.IExtensible, new()
        where T : class, ProtoBuf.IExtensible, new()
    {
        return SingletonHolder<DataManager<MsgT, T>>.Instance.DataStore;
    }

    public static string GetConfigString(byte[] dt)
    {
        return System.Text.Encoding.Default.GetString(dt).Trim();
    }

    public static Dictionary<string, string> TextDic = new Dictionary<string, string>();

    public static string GetTextFromConfig(string id)
    {
        try
        {
            return TextDic[id];
        }
        catch
        {
            return id;
        }
        
    }
}

