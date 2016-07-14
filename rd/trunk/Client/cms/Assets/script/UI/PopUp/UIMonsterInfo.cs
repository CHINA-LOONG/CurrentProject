using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIMonsterInfo : UIBase
{

	public static string ViewName = "UIMonsterInfo";

	public static void Open(int guid, string monsterid,int level,int stage)
	{
	 	GameObject go =	UIMgr.Instance.OpenUI (UIMonsterInfo.ViewName);
		UIMonsterInfo mInfo = go.GetComponent<UIMonsterInfo> ();
		mInfo.ShowWithData (guid, monsterid,level,stage);
	}

	public Button closeButton;
	public	Text	name;
	public	Image	propertyImage;
	public	Transform iconTrans;
	public	Transform	skillTrans;
	public	SkilTips	skilTips;
	public Text	haveSpell;

	// Use this for initialization
	void Awake ()
	{
		EventTriggerListener.Get (closeButton.gameObject).onClick = OnCloseButtonClick;
		name.text = ""; 
		skilTips.gameObject.SetActive (false);
	}

	void	OnCloseButtonClick(GameObject go)
	{
		UIMgr.Instance.CloseUI (this);
	}

	public	void	ShowWithData(int guid, string monsterid,int level,int stage)
	{
		UnitData unitData = StaticDataMgr.Instance.GetUnitRowData (monsterid);
		if (null == unitData) 
		{
			Logger.LogError("Error:instance info , monsterId config error :" + monsterid);
			return;
		}
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
		bool isBoss = unitData.assetID.Contains ("boss_");
		//Spell tempSpell = null;

		ArrayList spellArrayList = MiniJsonExtensions.arrayListFromJson(unitData.spellIDList);
		for (int i = 0; i < spellArrayList.Count; ++i)
		{
			string spellID = spellArrayList[i] as string;
			SpellProtoType spellPt = StaticDataMgr.Instance.GetSpellProtoData(spellID);
			if (spellPt == null)
			{
				continue;
			}
			if(spellPt.category == (int)SpellType.Spell_Type_MgicAttack ||
			   spellPt.category == (int)SpellType.Spell_Type_Cure||
			   spellPt.category == (int)SpellType.Spell_Type_PhyAttack ||
			   spellPt.category == (int)SpellType.Spell_Type_Beneficial||
			   spellPt.category == (int)SpellType.Spell_Type_Negative)
			{
				AddIcon(spellPt);
			}
			else if(spellPt.category == (int)SpellType.Spell_Type_MagicDazhao ||
			        spellPt.category == (int)SpellType.Spell_Type_PhyDaZhao)
			{
				if(!isBoss)
				{
					AddIcon(spellPt);
				}
			}

		}	
		if (isBoss)
		{
			var icon = SpellIcon.CreateWith (skillTrans );
			icon.SetBoss();
		}
	}

	private void AddIcon(SpellProtoType spellType)
	{
		var icon = SpellIcon.CreateWith (skillTrans );
		icon.SetData (1, spellType.id);

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
