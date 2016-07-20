using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIMonsterInfo : UIBase
{

	public static string ViewName = "UIMonsterInfo";

	public static void Open(int guid, string monsterid,int level,int stage)
	{
        UIMonsterInfo mInfo = UIMgr.Instance.OpenUI_(UIMonsterInfo.ViewName) as UIMonsterInfo;
		mInfo.ShowWithData (guid, monsterid,level,stage);
	}

	public Button closeButton;
	public	Text	name;
	public	Image	propertyImage;
	public	Transform iconTrans;
	public	Transform	skillTrans;
	public	SkilTips	skilTips;
	public Text	haveSpell;
	private int guid = -1;

	private int level = 1;

	// Use this for initialization
	void Awake ()
	{
		EventTriggerListener.Get (closeButton.gameObject).onClick = OnCloseButtonClick;
	}

    public override void Init()
    {
		name.text = ""; 
		skilTips.gameObject.SetActive (false);
    }
    public override void Clean()
    {
        //TODO: destroy MonsterIcon
    }

	void	OnCloseButtonClick(GameObject go)
	{
		UIMgr.Instance.DestroyUI (this);
	}

	public	void	ShowWithData(int guid, string monsterid,int level,int stage)
	{
		UnitData unitData = StaticDataMgr.Instance.GetUnitRowData (monsterid);
		if (null == unitData) 
		{
			Logger.LogError("Error:instance info , monsterId config error :" + monsterid);
			return;
		}
		this.level = level;
		this.guid = guid;
		name.text = unitData.NickNameAttr;
		haveSpell.text = StaticDataMgr.Instance.GetTextByID ("spell_yongyoujineng");
		SetProperty (unitData.property);

		MonsterIcon icon = MonsterIcon.CreateIcon ();
		icon.transform.SetParent (iconTrans, false);

		icon.SetMonsterStaticId (monsterid);
		icon.SetStage (level);
		icon.SetLevel (stage);
		SetSpellIcon (unitData);
	}

	private void SetProperty(int property)
	{
		var image = ResourceMgr.Instance.LoadAssetType<Sprite>("property_" + property) as Sprite;
		if(image != null)
		{
			propertyImage.sprite = image;
		}
	}

	private void SetSpellIcon(UnitData unitData)
	{
		ArrayList spellArrayList = MiniJsonExtensions.arrayListFromJson(unitData.spellIDList);
		for (int i = 0; i < spellArrayList.Count; ++i) 
		{
			string spellID = spellArrayList [i] as string;
			SpellProtoType spellPt = StaticDataMgr.Instance.GetSpellProtoData (spellID);
			if (spellPt == null) 
			{
				continue;
			}
			if (string.IsNullOrEmpty (spellPt.tips)) 
			{
				continue;
			}
			AddIcon (spellPt);
		}
	}

	private void AddIcon(SpellProtoType spellType)
	{
		var icon = SpellIcon.CreateWith (skillTrans );
		int spellLevel = this.level;
		if (guid != -1)
		{
			GameUnit pet = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(guid);
			if(pet != null)
			{
				Spell sp = pet.GetSpell(spellType.id);
				if(sp!=null)
				{
					spellLevel = sp.level;
				}
			}
		}
		icon.SetData (spellLevel, spellType.id);

		EventTriggerListener.Get (icon.iconButton.gameObject).onEnter = OnPointerEnter;
		EventTriggerListener.Get (icon.iconButton.gameObject).onExit = OnPointerExit;
	}

	int test = 0;
	public	void OnPointerEnter(GameObject go)
	{
		//Logger.LogError ("--------" + test++);
		SpellIcon icon =  go.GetComponentInParent<SpellIcon> ();
		icon.SetMask (true);
		skilTips.gameObject.SetActive (true);
		skilTips.SetSpellId (icon.spellId, icon.level);

		RectTransform iconTrans =	icon.transform as RectTransform;
		RectTransform tipsTrans = skilTips.transform as RectTransform;

		Vector2 iconPos = iconTrans.anchoredPosition;
		Vector2 tipsPos = tipsTrans.anchoredPosition;
		tipsPos.x = iconPos.x + 120;

		tipsTrans.anchoredPosition = tipsPos;
	}

	public	void OnPointerExit(GameObject go)
	{
		//Logger.LogError (test++ + "--------"  );
		SpellIcon icon =  go.GetComponentInParent<SpellIcon> ();
		icon.SetMask (false);
		skilTips.gameObject.SetActive (false);
	}
}
