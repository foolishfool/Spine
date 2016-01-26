using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;
using LitJson;
using System.Text;
public class EditObjectPos : Editor
{

    [MenuItem("Custom Editor/ReadJson")]
    static public void ReadJson()
    {
        string filedirName = "Assets/StreamingAssets/file/";
        string filename = "";

        UnityEngine.Object[] objects = Selection.objects;
        for (int iter = 0; iter < objects.Length; ++iter)
        {
            filename = objects[iter].name;
            string path = filedirName + filename + ".txt";
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset;
            Debug.Log("change..... " + textAsset.text);
            JsonData[] data = JsonMapper.ToObject<JsonData[]>(textAsset.text);
            int i = 0;

            GameObject parentobj = new GameObject("PosObj");
            parentobj.name = "PosObj";
            foreach (JsonData jd in data)
            {
                Vector3 vec = Util.StrToVector3(jd["pos_"+ i.ToString()].ToString(), ',');
                GameObject obj = new GameObject("pos_" + i.ToString(),typeof(GameObject));
                obj.transform.parent = parentobj.transform;
                obj.transform.position = vec;
                obj.name = "pos_" + i.ToString();
                NGUIDebug.Log("vec   " + vec.ToString());
                i++;
            }
        }
    }

    [MenuItem("Custom Editor/SaveJSON")]
	static void SaveJSON ()
    {

        string filePath = Application.dataPath + @"/StreamingAssets/file/pos.txt";
        FileInfo t = new FileInfo(filePath);
        if (!File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        StreamWriter sw = t.CreateText();

        StringBuilder sb = new StringBuilder();
        JsonWriter writer = new JsonWriter(sb);

        foreach (UnityEditor.EditorBuildSettingsScene s in UnityEditor.EditorBuildSettings.scenes)
        {
            if (s.enabled)
            {
                string name = s.path;
                EditorApplication.OpenScene(name);
                GameObject parent = GameObject.Find("PosObj");
                if (parent)
                {
                    writer.WriteArrayStart();
                    for (int i = 0; i < parent.transform.childCount; i++)
                    {
                        
                        Transform obj = parent.transform.GetChild(i);
                        Debug.Log("objtr....  " + obj.name + "   " + obj.position);
                        //writer.WriteObjectStart();  
                        writer.WriteObjectStart();
                        writer.WritePropertyName(obj.name);

                        writer.Write(obj.position.x.ToString() + "," + obj.position.y.ToString() + "," + obj.position.z.ToString());
                        writer.WriteObjectEnd();
                        //writer.WriteObjectEnd();
                        
                    }
                    writer.WriteArrayEnd();
                }
                
            }

        }
        sw.WriteLine(sb.ToString());
        sw.Close();
        sw.Dispose();
        AssetDatabase.Refresh();
    }
}
