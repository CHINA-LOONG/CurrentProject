using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
public class CheckImage 
{
    private static string motionNameList;
    private static List<string> paths = new List<string>();
    private static float maxNum = 0;
    private static float minNum = 10;
    [MenuItem("Builder/Check Image")]
    private static void CheckImageSize()
    {
        Recursive("Assets/SourceAsset/texture/UI");
    }
    private static void Recursive(string path)//battle_speed_num1
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (ext.Equals(".png") || ext.Equals(".tga"))
            {
                if (filename.Contains("meta") == true)
                    continue;
                Sprite sprite = AssetDatabase.LoadAssetAtPath(filename, typeof(Sprite)) as Sprite;
                if (null == sprite)
                    return;
                FileInfo fileInfoByte = new FileInfo(filename);
                float fileByte = (((float)fileInfoByte.Length / 1024) / 1024) * 1024;
                float spriteHW = (sprite.rect.height * sprite.rect.width) / fileByte;
                if (spriteHW < (256 - 70))
                {
                    //if (spriteHW > maxNum)
                    //    maxNum = spriteHW;
                    //if (spriteHW < minNum)
                    //{
                    //    minNum = spriteHW;
                    //    motionNameList = filename;
                    //}
                    Debug.Log("文件路径: " + filename + " 值:  " + spriteHW);
                }
            }
        }
        foreach (string dir in dirs)
        {
            paths.Add(dir.Replace('\\', '/'));
            Recursive(dir);
        }
    }
}
