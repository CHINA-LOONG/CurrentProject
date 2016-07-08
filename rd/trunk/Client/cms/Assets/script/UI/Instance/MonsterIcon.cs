using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MonsterIcon : MonoBehaviour 
{
	public	Button	iconButton;
	public	Image	qualityImage;
	public	Image	monsterImage;

	public	Transform	itemsParent;
	public	Image	bossImage;
	public	Image	friendImage;
	public	Image	selectImage;
	public  Image  	maskImage;
	public	Text	levelText;
	public	Text	qualityText;
	public Text 	nickNameText;

	private	List<Transform> listItems;

	[HideInInspector]
	public	string	Id;

	[HideInInspector]
	public string monsterId;

	public	static MonsterIcon	CreateIcon()
	{
		GameObject go = ResourceMgr.Instance.LoadAsset ("ui/monstericon", "monsterIcon");
		MonsterIcon micon =  go.GetComponent<MonsterIcon>();
		micon.Init ();
		return micon;
	}

	void Awake()
	{
		nickNameText.text = "";
	}
	// Use this for initialization

	public void Init()
	{
		listItems = new List<Transform> ();
		
		Transform[] subItems = itemsParent.GetComponentsInChildren<Transform> ();
		foreach (Transform subitem in subItems)
		{
			if(subitem != itemsParent)
			{
				listItems.Add(subitem);
				subitem.gameObject.SetActive(false);
			}
		}
	}
	
	//--------------------------------------------------------------------------------------------------------------------
	public	void	SetId(string id)
	{
		Id = id;
	}
	
	public void SetMonsterStaticId(string unitId)
	{
		monsterId = unitId;
		UnitData unitData = StaticDataMgr.Instance.GetUnitRowData (unitId);
		if (null == unitData) 
		{
			Logger.LogError("Error:instance info , monsterId config error :" + unitId);
			return;
		}
		
		var headPath = unitData.uiAsset;
		var k = headPath.LastIndexOf('/');
		var assetbundle = headPath.Substring(0, k);
		var assetname = headPath.Substring(k + 1, headPath.Length - k - 1);
		Sprite headImg = ResourceMgr.Instance.LoadAssetType<Sprite>(assetbundle, assetname) as Sprite;
		
		monsterImage.sprite = headImg;

		HideItems ();
	}

	public void	SetStage(int stage)
	{
		int quallity = 0;
		int plusQuality = 0;
		CalculationQuality (stage, out quallity, out plusQuality);

		string assetbundle = "ui/grade";
		string assetname = "grade_" + quallity.ToString ();
		Sprite headImg = ResourceMgr.Instance.LoadAssetType<Sprite>(assetbundle, assetname) as Sprite;
		if (null != headImg)
			qualityImage.sprite = headImg;

		string temp = "";
		if (plusQuality > 0)
		{
			temp = "+" + plusQuality.ToString();
		}
		qualityText.text = temp;
	}

	private	void	CalculationQuality(int stage, out int quality ,out int plusQuality )
	{
		quality = 1;
		plusQuality = 0;

		if (stage < 0 || stage > 15)
			return;

		if (stage == 0)
		{
			quality = 1;
		} 
		else if (stage < 3)
		{
			quality = 2;
			plusQuality = stage - 1;
		} 
		else if (stage < 6)
		{
			quality = 3;
			plusQuality = stage - 3;
		} 
		else if (stage < 10)
		{
			quality = 4;
			plusQuality = stage - 6;
		} 
		else if (stage < 15)
		{
			quality = 5;
			plusQuality = stage - 10;
		}
		else 
		{
			quality = 6;
		}
	}

	//--------------------------------------------------------------------------------------------------------------------
	private	void	HideItems()
	{
		foreach (Transform subitem in listItems)
		{
			subitem.gameObject.SetActive(false);
		}
	}

	public void ShowBossItem(bool bshow = true)
	{
		bossImage.gameObject.SetActive (bshow);
	}

	public	void	ShowFriendItem(bool bshow = true)
	{
		friendImage.gameObject.SetActive (bshow);
	}

	public	void	ShowSelectImage(bool bshow = true)
	{
		selectImage.gameObject.SetActive (bshow);
	}

	public void ShowMaskImage(bool bshow = true)
	{
		maskImage.gameObject.SetActive (bshow);
	}

	public	bool	IsSelected()
	{
		return selectImage.gameObject.activeSelf;
	}

	public	void	SetLevel(int level)
	{
		levelText.gameObject.SetActive (true);
		levelText.text = level.ToString ();
	}

	public void SetName(string nickname)
	{
		nickNameText.text = nickname;
	}
}
