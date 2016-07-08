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
	public	Text	levelText;

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

	// Use this for initialization
	void Start ()
	{

	}

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

		SetQuality (unitData.grade);

		HideItems ();
	}

	private void	SetQuality(int quallity)
	{
		if (quallity < 1 || quallity > 6)
			return;
		string assetbundle = "ui/grade";
		string assetname = "grade_" + quallity.ToString ();
		Sprite headImg = ResourceMgr.Instance.LoadAssetType<Sprite>(assetbundle, assetname) as Sprite;
		if (null != headImg)
			qualityImage.sprite = headImg;
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
		friendImage.gameObject.SetActive (bshow);
	}

	public	bool	IsSelected()
	{
		return friendImage.gameObject.activeSelf;
	}

	public	void	SetLevel(int level)
	{
		levelText.gameObject.SetActive (true);
		levelText.text = level.ToString ();
	}
}
