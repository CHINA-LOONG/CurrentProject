using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using System.IO;
using BestHTTP;

public class UIUpdate : MonoBehaviour ,IPointerUpHandler
{

	private bool userCanRequestUpdate = false;

	public	Text	msgText;
	// Use this for initialization
	IEnumerator Start () 
	{
		msgText.text = null;

		yield return new WaitForEndOfFrame ();
		CheckExtractResource ();
	}

	/// <summary>
	/// 释放资源
	/// </summary>
	public void CheckExtractResource()
	{
		bool isExists = Directory.Exists(Util.ResPath) && File.Exists(Path.Combine(Util.ResPath, "files.txt"));
		//
		if (Const.DebugMode || isExists)
		{
			StartCoroutine(OnUpdateResource());
			return;   //文件已经解压过了，自己可添加检查文件列表逻辑
		}
		StartCoroutine(OnExtractResource());    //启动释放协成 
	}
	
	IEnumerator OnExtractResource()
	{
		string dataPath = Util.ResPath;  //数据目录
		string resPath = Util.AppContentPath(); //游戏包资源目录
		Logger.Log(Util.ResPath);
		
		if (Directory.Exists(dataPath))
			Directory.Delete(dataPath, true);
		Directory.CreateDirectory(dataPath);
		
		string infile = Path.Combine(resPath, "files.txt");
		string outfile = Path.Combine(dataPath, "files.txt");
		if (File.Exists(outfile))
			File.Delete(outfile);
		
		Logger.Log("正在解包文件:>files.txt\n");
		if (Application.platform == RuntimePlatform.Android)
		{
			WWW www = new WWW(infile);
			yield return www;
			
			if (www.isDone)
			{
				File.WriteAllBytes(outfile, www.bytes);
			}
			yield return 0;
		}
		else File.Copy(infile, outfile, true);
		yield return new WaitForEndOfFrame();
		
		//释放所有文件到数据目录
		string[] files = File.ReadAllLines(outfile);
		int count = 1;
		foreach (var file in files)
		{
			infile = resPath + file;
			outfile = dataPath + file;
			Logger.Log("正在解包文件:>" + file);
			msgText.text = string.Format("正在解包 第{0}个，共{1}个",count ++,files.Length);
			string dir = Path.GetDirectoryName(outfile);
			if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
			
			if (Application.platform == RuntimePlatform.Android)
			{
				WWW www = new WWW(infile);
				yield return www;
				
				if (www.isDone)
				{
					//Logger.LogError(infile + " ## " + www.bytes.Length + " ## " + outfile);
					File.WriteAllBytes(outfile, www.bytes);
				}
				yield return null;
			}
			else
				File.Copy(infile, outfile, true);
			
			yield return new WaitForEndOfFrame();
		}
		Logger.Log("解包完成!!!");
		msgText.text = "资源包抽取抽取完成";
		yield return new WaitForSeconds(0.1f);
		
		//释放完成，开始启动更新资源
		StartCoroutine(OnUpdateResource());
	}

	/// <summary>
	/// 启动更新下载
	/// </summary>
	IEnumerator OnUpdateResource()
	{
		//不更新模式下直接添加ResourceManager
		if (!Const.UpdateMode)
		{
			OnUpdateFinished();
			yield break;
		}
		
		HTTPRequest httpRquest = null;
		string dataPath = Util.ResPath;  //数据目录
		string url = Const.UpdateUrl;
		
		//获取本地版本
		string versionFilePath = Path.Combine(dataPath, "version");
		string localVersion = string.Empty;
		if (File.Exists(versionFilePath))
			localVersion = File.ReadAllText(versionFilePath).Trim();

		
		httpRquest = new HTTPRequest (new Uri(url),HTTPMethods.Post);
		httpRquest.AddField ("channel", Const.channel);
		httpRquest.AddField ("device", Const.platform);
		httpRquest.AddField ("resourceId", Const.ResouceCodeAttr.ToString());
		httpRquest.AddField ("versionCode", Const.versionCode.ToString());
		httpRquest.Send ();
		yield return StartCoroutine (httpRquest);
		if (!httpRquest.Response.IsSuccess)
		{
			OnUpdateFailed(string.Empty);
			yield break;
		}
		//将version list文件解析为列表
		string remoteVersion = httpRquest.Response.DataAsText;

	   Hashtable ht = MiniJsonExtensions.hashtableFromJson (remoteVersion);
		if (null == ht )
		{
			Logger.LogError("更新错误，解析数据出错: " + remoteVersion);
			OnUpdateFailed("");
			yield break;
		}
		if(int.Parse(ht["status"].ToString()) != 0)
		{
			Logger.LogError("更新错误，数据error: " + remoteVersion);
			OnUpdateFailed("");
			yield break;
		}

		string resServer = ht ["resourceServer"] as String;
		ArrayList versionList = ht ["resources"] as ArrayList;
		int tempIndex = 1;
		foreach(Hashtable subVersion in versionList)
		{
			int verId = int.Parse(subVersion["resourceId"].ToString());
			string verZip = string.Format("{0}.zip",subVersion["resourceName"].ToString());

			msgText.text = string.Format("更新资源{0},共有{1}个资源", tempIndex ++ ,versionList.Count );
			httpRquest = new HTTPRequest( new Uri( string.Format("{0}/{1}",resServer,verZip)));
			httpRquest.Send();
			yield return StartCoroutine(httpRquest);
			if (!httpRquest.Response.IsSuccess)
			{
				OnUpdateFailed("");
				yield break;
			}
			
			//解压zip，写入本地目录
			Util.UnZipFromBytes(httpRquest.Response.Data, dataPath);
			
			//更新成功后写入客户端的version文件
			File.WriteAllText(versionFilePath, verId.ToString());

			Const.SetResouceCode(verId);

			yield return new WaitForEndOfFrame();
		}
		Logger.Log("更新完成!!");
		OnUpdateFinished ();
	}
	
	void OnUpdateFailed(string file)
	{
		Logger.LogError("更新失败!>" + file);
		msgText.text = "资源更新失败，todo：该怎么办呢！";
		userCanRequestUpdate = true;
	}

	void OnUpdateFinished()
	{	
		InitMgr ();
	}

	void InitMgr()
	{
		msgText.text = "正在初始化组件...";
		GameEventMgr.Instance.Init();
		ResourceMgr.Instance.Init();
		
		StaticDataMgr.Instance.Init();
		ObjectDataMgr.Instance.Init();
		GameDataMgr.Instance.Init();
		StatisticsDataMgr.Instance.Init();
		LayerConst.Init ();

		
		UIMgr.Instance.Init();
		GameMain.Instance.Init();
		SpellService.Instance.Init();
		ActorEventService.Instance.Init();
		GameSpeedService.Instance.Init();

        ResourceMgr.Instance.LoadLevelAsyn("firstScene", false);
		Destroy (gameObject);
	}

	//--------------Event------------------------
	public void OnPointerUp (PointerEventData eventData)
	{
		userCanRequestUpdate = false;
		StartCoroutine(OnUpdateResource());
	}
}
