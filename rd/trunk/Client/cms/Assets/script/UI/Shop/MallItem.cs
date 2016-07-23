using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BestHTTP;

public class MallItem : MonoBehaviour 
{
	public	Image	zuanshiTypeImage;
	public	Image	moneyIcon;

	public	Text	zuanshiText;
	public	Text	moneyText;

	public	Transform	firstbuyPanel;

	public	Button	itemButton;

	//data
	private	RechargeStaticData	rechargeStaticData = null;


	public	static MallItem	Create(RechargeStaticData staticData)
	{
		GameObject go = ResourceMgr.Instance.LoadAsset ("MallItem");

		var mitem = go.GetComponent<MallItem> ();
		mitem.RefreshWithRechargeData (staticData);

		return mitem;
	}
	// Use this for initialization
	void Start ()
	{
		ScrollViewEventListener.Get (itemButton.gameObject).onClick = OnItemButtonClicked;
	}
	
	public	void	RefreshWithRechargeData(RechargeStaticData rsdata)
	{
		rechargeStaticData = rsdata;
		zuanshiText.text = rsdata.gold.ToString();

	}

	void OnItemButtonClicked(GameObject go)
	{
		Logger.Log ("Store buy item!!!");
		StartCoroutine (BuyItem ());

	}

	IEnumerator  BuyItem()
	{
		string url = "http://123.59.45.55:9600/test";
		//string url = "http://192.168.199.177:9600/test";

		HTTPRequest httpRquest = null;
		
		httpRquest = new HTTPRequest (new System.Uri(url),HTTPMethods.Post);
		httpRquest.AddField ("product_id",rechargeStaticData.id);
		httpRquest.AddField ("through_cargo", GameDataMgr.Instance.UserDataAttr.orderServerKey);
		httpRquest.AddField ("puid", GameDataMgr.Instance.UserDataAttr.guid);
		httpRquest.Send ();
		
		yield return StartCoroutine (httpRquest);
		if (!httpRquest.Response.IsSuccess)
		{
			Logger.LogError("buy http request error..");
			yield break;
		}
		//get http
		string retjson = httpRquest.Response.DataAsText;
		Logger.LogError ("buy item returnjson = " + retjson);
		Hashtable ht = MiniJsonExtensions.hashtableFromJson (retjson);
	}

}


