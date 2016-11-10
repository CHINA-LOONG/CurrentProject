using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;

public class CheckModelScale : EditorWindow 
{


    [MenuItem("Tools/CheckModelScale")]
    private static void main()
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)+"/"+"checkList"+".csv";
        StreamWriter sw = new StreamWriter(path); 
        sw.WriteLine("Perfab,ScaleX,ScaleY,ScaleZ,Material,colorH,colorS,colorV,colorA");
        FindAllSelect("Assets/Prefabs/monsters",sw);   
        sw.Close(); 
        Debug.Log("Success!"+Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)+"/"+"checkList"+".csv");
       
    }
    private static void FindAllSelect(string path,StreamWriter sw)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        //SkinnedMeshRenderer[] skin;
        Material ModelMaterial;
        Color color;
        float color_H;
        float color_S;
        float color_V;
        float color_A;

        string Material_name;

        foreach (string filename in names)
        {
          //  string ext = Path.GetExtension(filename);
            if (filename.Contains(".prefab") && !filename.Contains(".meta"))
            {
                string name = filename.Replace('\\', '/');
                GameObject curObj = AssetDatabase.LoadAssetAtPath(name, typeof(GameObject)) as GameObject;
                if (curObj != null)
                {                
                    ModelMaterial = curObj.transform.Find("mesh_body").GetComponentsInChildren<SkinnedMeshRenderer>(true)[0].sharedMaterial;  

                    if (ModelMaterial != null)
                    {

                        color= ModelMaterial.color;
                        color_A = color.a;
                        Material_name = ModelMaterial.name;
                       // Debug.LogError(ModelMaterial.mainTexture.name);
                        EditorGUIUtility.RGBToHSV(color,out color_H,out color_S,out color_V);
                    }
                    else
                    {                       
                        Material_name ="Missing";
                        color_H = 9999;
                        color_S = 9999;
                        color_V = 9999;
                        color_A = 9999;
                    }
                                    
                    //Debug.LogError(curObj.name+"____"+skin[0].sharedMaterial.name+"____"+color_H+","+color_S+","+color_V);
                    sw.WriteLine(curObj.name.ToString()+","+curObj.transform.localScale.x+","+curObj.transform.localScale.y+","+curObj.transform.localScale.z
                                 +","+Material_name+","+colorToStr(color_H)+","+colorToStr(color_S)+","+colorToStr(color_V)+","+colorToStr(color_A));
                   sw.Flush(); 
                }            
            }
        }
       foreach (string dir in dirs)
       {
           FindAllSelect(dir,sw);
       }
       
    }
    private static string colorToStr(float color)
    {
        if (color.ToString().Equals("9999"))
        {
            return "Missing";
        } 
        else
        {
            return Math.Floor((color*255)+0.5).ToString();
        }
    }

       
}
