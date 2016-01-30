using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;

public class BatchExportSpine : Editor
{
    [MenuItem("Custom Editor/BatchCreateSpineData")]
    static public void BatchCreateSpineData()
    {
        //string dirName = Application.dataPath + "/Arts/Spine/";
        string filedirName = "Assets/Arts/Spine/";
        
        string spineFileName = "";
        //选择多个对象批量生成
        UnityEngine.Object[] objects = Selection.objects;
        for (int iter = 0; iter < objects.Length; ++iter)
        {
            spineFileName = objects[iter].name;
            string path = filedirName + spineFileName + "/" + spineFileName;
            string textureName = path + ".png";
            string jsonFileName = path + ".json.txt";
            string atlasFileName = path + ".atlas.txt";

            Material mat;
            ///1、 创建材质，并指贴图和shader和设置图片格式
            {
                Shader shader = Shader.Find("Spine/FadeInOut"); //默认的shader，这个可以修改

                mat = new Material(shader);
                Texture tex = AssetDatabase.LoadAssetAtPath(textureName, typeof(Texture)) as Texture;
                TextureImporter textureImporter = AssetImporter.GetAtPath(textureName) as TextureImporter;
                textureImporter.textureType = TextureImporterType.Advanced;
                textureImporter.mipmapEnabled = false;
                textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
                
                mat.SetTexture("_MainTex", tex);

                AssetDatabase.CreateAsset(mat, path + ".mat");
                AssetDatabase.SaveAssets();
            }

            ///2、 创建atlas，并指xx
            AtlasAsset m_AtlasAsset = AtlasAsset.CreateInstance<AtlasAsset>();
            AssetDatabase.CreateAsset(m_AtlasAsset, path + ".asset");
            Selection.activeObject = m_AtlasAsset;

            TextAsset textAsset = AssetDatabase.LoadAssetAtPath(atlasFileName, typeof(TextAsset)) as TextAsset;
            m_AtlasAsset.atlasFile = textAsset;
            m_AtlasAsset.materials = new Material[1];
            m_AtlasAsset.materials[0] = mat;
            AssetDatabase.SaveAssets();


            ///3、 创建SkeletonDataAsset，并指相关
            SkeletonDataAsset m_skeltonDataAsset = SkeletonDataAsset.CreateInstance<SkeletonDataAsset>();
            AssetDatabase.CreateAsset(m_skeltonDataAsset, path + "_SkeletonData.asset");
            Selection.activeObject = m_skeltonDataAsset;

            m_skeltonDataAsset.atlasAsset = m_AtlasAsset;
            TextAsset m_jsonAsset = AssetDatabase.LoadAssetAtPath(jsonFileName, typeof(TextAsset)) as TextAsset;
            m_skeltonDataAsset.skeletonJSON = m_jsonAsset;
            m_skeltonDataAsset.scale = 0.01f;
            AssetDatabase.SaveAssets();


            /// 创建场景物件
            GameObject gameObject = new GameObject(spineFileName, typeof(SkeletonAnimation));
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = gameObject;

            SkeletonAnimation m_skelAnim = gameObject.GetComponent<SkeletonAnimation>();
            m_skelAnim.skeletonDataAsset = m_skeltonDataAsset;
        }
    }
}

