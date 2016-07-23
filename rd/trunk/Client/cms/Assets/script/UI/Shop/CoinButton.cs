using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CoinButton : MonoBehaviour
{
	public	enum CoinType
	{
		Zuanshi = 0,
		Jinbi,
		GonghuiBi
	}

	public	static CoinButton CreateWithType(CoinType cType)
	{
		GameObject go = ResourceMgr.Instance.LoadAsset ("CoinButton");
		var c = go.GetComponent<CoinButton> ();
		c.CoinTypeAttr = cType;
		return c;
	}

	// Use this for initialization
	void Start () 
	{
		BindListener ();
		RefreshIcon ();
	}
	void OnDestroy()
	{
		UnBindListener ();
	}

	void BindListener()
	{
		GameEventMgr.Instance.AddListener<int> (GameEventList.ZuanshiChanged,OnZuanshiChanged);
		GameEventMgr.Instance.AddListener<long> (GameEventList.CoinChanged,OnJinbiChanged);
	}
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener<int> (GameEventList.ZuanshiChanged,OnZuanshiChanged);
		GameEventMgr.Instance.RemoveListener<long> (GameEventList.CoinChanged,OnJinbiChanged);
	}
	[SerializeField]
	private	CoinType	coinType = CoinType.Jinbi;

	public	Image	coinImage = null;
	public	Text	coinCount = null;
	public	Image	plusImage = null;
	public	Button	addCoinButton;

	public	CoinType	CoinTypeAttr
	{
		get
		{
			return coinType;
		}
		set
		{
			coinType = value;
			RefreshIcon();
		}
	}

	void RefreshIcon()
	{
		string coinImg = null;
		HideAddCoinButton (false);
		if(coinType == CoinType.Zuanshi)
		{
			OnZuanshiChanged(GameDataMgr.Instance.PlayerDataAttr.gold);
			coinImg = "icon_zuanshi";
		}
		else if (coinType == CoinType.Jinbi)
		{
			OnJinbiChanged(GameDataMgr.Instance.PlayerDataAttr.coin);
			coinImg = "icon_jinbi";
		}
		else if(coinType == CoinType.GonghuiBi)
		{
			OnGonghuiBiChanged(1);
			coinImg = "icon_gonghuibi";
			HideAddCoinButton(true);
		}
		
		if(!string.IsNullOrEmpty(coinImg))
		{
			Sprite coinSp = ResourceMgr.Instance.LoadAssetType<Sprite>(coinImg) as Sprite;
			if(null != coinSp)
			{
				coinImage.sprite = coinSp;
			}
		}
	}

	void	HideAddCoinButton( bool isHiede)
	{
		plusImage.gameObject.SetActive (!isHiede);
		addCoinButton.gameObject.SetActive (!isHiede);
	}

	void OnZuanshiChanged(int zuanshi)
	{
		if (coinType == CoinType.Zuanshi)
		{
			coinCount.text = string.Format("{0:N0}",zuanshi);
		}
	}

	void OnJinbiChanged(long jinbi)
	{
		if (coinType == CoinType.Jinbi)
		{
			coinCount.text = string.Format("{0:N0}",jinbi);
		}
	}

	void OnGonghuiBiChanged(int gonghuibi)
	{
		if (coinType == CoinType.GonghuiBi) 
		{
			coinCount.text = string.Format("{0:N0}",gonghuibi);
		}
	}
}
