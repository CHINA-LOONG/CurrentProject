using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class EncryptResource : EditorWindow
{
    static  EncryptResource window = null;

    public string showMsg = null;

    [MenuItem("Builder/Encrypt/rc4_test")]
    private static void rc4Test()
    {
        string text = "我爱你，北京！I love Beijing! 1949.10.1,#@lldh,很久以前烤串!OK";
        string key = "iloveybyf999";
        Logger.LogErrorFormat("待机密文本:{0}",text);
        string  encode = Encrypt.encode(key,text);
        Logger.LogErrorFormat("加密后文本:{0}", encode);
        text = Encrypt.encode(key, encode);
        Logger.LogErrorFormat("机密文本:{0}", text);
    }
    /*
    [MenuItem("Builder/Encrypt/加密CsvAndXML")]
    private static void encodeCsvAndXml()
    {
        bool isEncry = Encrypt.IsSpecialFileEncode();
        Logger.LogErrorFormat("specialFile state = {0}", isEncry);
        if (isEncry && window == null)
        {
            Rect wr = new Rect(200, 200, 500, 500);
            window = (EncryptResource)EditorWindow.GetWindowWithRect(typeof(EncryptResource), wr, true, "资源加密");
            window.showMsg = "CSV 和XML 已经加密!";
            window.Show();
            return;
        }
        EncodeOrDecodeCSVAndXml();
    }

    [MenuItem("Builder/Encrypt/解密CsvAndXML")]
    private static void decodeCsvAndXml()
    {
        bool isEncry = Encrypt.IsSpecialFileEncode();
        Logger.LogErrorFormat("specialFile state = {0}", isEncry);
       
        if (!isEncry && window == null)
        {
            Rect wr = new Rect(0, 0, 500, 500);
            window = (EncryptResource)EditorWindow.GetWindowWithRect(typeof(EncryptResource), wr, true, "资源解密");
            window.showMsg = "CSV 和XML 未加密，不需解密!";
            window.Show();
            return;
        }
        EncodeOrDecodeCSVAndXml();
    }
    */
    [MenuItem("Builder/Encrypt/加密OR解密CsvAndXML")]
    private static void EncodeOrDecodeCSVAndXml()
    {
        string staticPah = Path.Combine(Util.ResPath, "staticData");

        if(!Directory.Exists(staticPah))
        {
            Rect wr = new Rect(0, 0, 500, 500);
            window = (EncryptResource)EditorWindow.GetWindowWithRect(typeof(EncryptResource), wr, true, "资源加密/解密");
            window.showMsg = string.Format("不存在静态资源目录",staticPah);
            window.Show();
            return;
        }
        string[] files = Directory.GetFiles(staticPah);
        string fileExtension;
        foreach (string subFile in files)
        {
            if (subFile.Contains("assetMap"))
                continue;
            fileExtension = Path.GetExtension(subFile);
            if (string.IsNullOrEmpty(fileExtension))
                continue;
            fileExtension = fileExtension.ToLower();
            if (fileExtension.Contains("csv") || fileExtension.Contains("xml"))
            {
                Encrypt.encodeFileAndSave(Const.CSVENCRYKEY, subFile);
               // fileList.Add(subFile);
            }
        }
        Logger.Log("Encrpy Finished!");
        //window = (EncryptResource)EditorWindow.GetWindowWithRect(typeof(EncryptResource), new Rect(0, 0, 400, 400), true, "资源加密/解密");
       // window.showMsg = "Success!";
       // window.Show();
        return;
    }

    void OnGUI()
    {
        if(!string.IsNullOrEmpty(showMsg))
        {
            this.ShowNotification(new GUIContent(showMsg));
        }
    }
    void OnInspectorUpdate()
    {
        //这里开启窗口的重绘，不然窗口信息不会刷新
        this.Repaint();
    }
}
