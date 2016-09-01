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
  //  public Image levelImage;
	public	Text	levelText;
	public	Text	qualityText;
	public Text 	nickNameText;

    public Image maoxianImage;
    public Image zhushouImage;
    public Image lockImage;

	private	List<Transform> listItems;

	[HideInInspector]
	public	string	Id;

	[HideInInspector]
	public string monsterId;

    private bool init = false;

    public static MonsterIcon CreateIcon()
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("monsterIcon");
        MonsterIcon micon = go.GetComponent<MonsterIcon>();
        micon.Init();
        return micon;
    }

	void Awake()
	{
		nickNameText.text = "";
	}
	// Use this for initialization

	public void Init()
	{
        if (init == true)
        {
            return;
        }
        
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

        init = true;
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
		
		Sprite headImg = ResourceMgr.Instance.LoadAssetType<Sprite>(unitData.uiAsset) as Sprite;
		
		monsterImage.sprite = headImg;

		HideItems ();
	}

	public void	SetStage(int stage)
	{
		int quallity = 0;
		int plusQuality = 0;
		UIUtil.CalculationQuality (stage, out quallity, out plusQuality);

		string assetname = "grade_" + quallity.ToString ();
		Sprite headImg = ResourceMgr.Instance.LoadAssetType<Sprite>(assetname) as Sprite;
		if (null != headImg)
			qualityImage.sprite = headImg;

		string temp = "";
		if (plusQuality > 0)
		{
			temp = "+" + plusQuality.ToString();
			Outline outLine = qualityText.gameObject.GetComponent<Outline>();
			if(null != outLine)
			{
				outLine.effectColor = ColorConst.GetStageOutLineColor(quallity);
			}
			qualityText.color = ColorConst.GetStageTextColor(quallity);
		}
		qualityText.text = temp;
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

	public	void	SetLevel(int level,bool bshow=true)
	{
      //  levelImage.gameObject.SetActive(true);
		levelText.gameObject.SetActive (bshow);
		levelText.text = level.ToString ();
	}

	public void SetName(string nickname)
	{
		nickNameText.text = nickname;
	}

    public void ShowMaoxianImage(bool bShow = true)
    {
        maoxianImage.gameObject.SetActive(bShow);
    }
    public void ShowZhushouImage(bool bShow = true)
    {
        zhushouImage.gameObject.SetActive(bShow);
    }
    public void ShowLockImage(bool bShow = true)
    {
        lockImage.gameObject.SetActive(bShow);
    }
}
