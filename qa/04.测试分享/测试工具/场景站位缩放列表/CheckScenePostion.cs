using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;

public class CheckScenePostion : MonoBehaviour 
{

	// Use this for initialization
    [MenuItem("Tools/CheckScenePosition")]
    private static void main()
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)+"/"+"ScenePositionList"+".csv";
        StreamWriter sw = new StreamWriter(path);   
        sw.WriteLine("scene,battlePosition,pos,Position_X,Position_Y,Position_Z,,Rotation_X,Rotation_Y,Rotation_Z,,Scale_X,Scale_Y,Scale_Z");
        FindAllSelect("Assets/Scene",sw);   
        sw.Close(); 
        Debug.Log("Success!"+Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)+"/"+"ScenePositionList"+".csv");
        
    }
    private static void FindAllSelect(string path,StreamWriter sw)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        GameObject go;   

        foreach (string filename in names)
        {         
            if (filename.Contains(".unity") && !filename.Contains(".meta"))
            {
                string name = filename.Replace('\\', '/');
                EditorApplication.OpenScene(name);

                for(int i =0;i<3;i++)
                {
                    go = GameObject.Find("battlePosition"+i.ToString());
                    if (go!=null)
                     {
                        if(i==0)
                        {
                            sw.WriteLine(Path.GetFileNameWithoutExtension(name)+","+go.name);
                        }
                        else
                        {
                            sw.WriteLine(","+go.name);
                        }
                        WritePosInfo(go,sw);                
                     }
                }
            }
        }
        foreach (string dir in dirs)
        {
            FindAllSelect(dir,sw);
        }
        
    }
    private static void WritePosInfo(GameObject go,StreamWriter sw)
    {
        Transform pos;
        Transform bosspos;
        Vector3 position;
        Quaternion rotation;
        Vector3 scale;

        for (int i = 0; i<6; i++)
        {
            pos = go.transform.Find("pos"+i.ToString());
            position = pos.transform.localPosition;
            rotation = pos.transform.localRotation;
            scale = pos.transform.localScale;

            sw.WriteLine(",," + pos.transform.name+","+position.x+","+position.y+","+position.z+",,"+rotation.eulerAngles.x+","+rotation.eulerAngles.y+","
                         +rotation.eulerAngles.z+",,"+scale.x+","+scale.y+","+scale.z);
        }

        bosspos = go.transform.Find("bosspos");
        position = bosspos.transform.localPosition;
        rotation = bosspos.transform.localRotation;
        scale = bosspos.transform.localScale;

        sw.WriteLine(",," + bosspos.transform.name+","+position.x+","+position.y+","+position.z+",,"+rotation.eulerAngles.x+","+rotation.eulerAngles.y+","
                     +rotation.eulerAngles.z+",,"+scale.x+","+scale.y+","+scale.z);
    }
  
}
