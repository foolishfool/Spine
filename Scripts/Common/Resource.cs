﻿using System;
using System.Collections;
using System.IO;
using UnityEngine;


public class Resource {

    private static Hashtable texts = new Hashtable();
    private static Hashtable images = new Hashtable();
    private static Hashtable prefabs = new Hashtable();
	private static Hashtable audio  = new Hashtable();

    public static string LoadTextFile(string path, string ext)
    {
        object obj = Resource.texts[path];
        if (obj == null)
        {
            Resource.texts.Remove(path);
            string text = string.Empty;
#if UNITY_EDITOR
            string pathstr = Util.GetDataDir() + "/StreamingAssets/" + path + ext;
#else
            string pathstr = Util.AppContentDataUri + path + ext;
#endif
            text = File.ReadAllText(pathstr);

            Resource.texts.Add(pathstr, text);
            return text;
        }
        return obj as string;
    }

    public static Texture2D LoadTexture(string path)
    {
        object obj = Resource.images[path];
        if (obj == null)
        {
           
            Resource.images.Remove(path);
            Texture2D texture2D = (Texture2D)Resources.Load(path, typeof(Texture2D));
            Resource.images.Add(path, texture2D);
            return texture2D;
        }

        return obj as Texture2D;
    }

    public static GameObject LoadPrefab(string path)
    {
        object obj = Resource.prefabs[path];
        if (obj == null)
        {
            Resource.prefabs.Remove(path);
            GameObject gameObject = (GameObject)Resources.Load(path, typeof(GameObject));
            Resource.prefabs.Add(path, gameObject);
            return gameObject;
        }

        return obj as GameObject;
    }


	public static AudioSource LoadAudioClip(string path)
	{
		object obj = Resource.audio[path];
		if (obj == null)
		{
			Resource.audio.Remove(path);
            AudioSource audioclip = (AudioSource)Resources.Load(path, typeof(AudioSource));
			Resource.audio.Add(path, audioclip);
            return audioclip;
		}

		return obj as AudioSource;
		
	}
}
