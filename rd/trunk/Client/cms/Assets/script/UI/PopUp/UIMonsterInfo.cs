using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIMonsterInfo : UIBase
{

	public static string ViewName = "UIMonsterInfo";
	public static string AssertName = "ui/monsterinfo";

	public static void Open(int guid, string monsterid)
	{
	 	GameObject go =	UIMgr.Instance.OpenUI (UIMonsterInfo.AssertName, UIMonsterInfo.ViewName);
		UIMonsterInfo mInfo = go.GetComponent<UIMonsterInfo> ();
		mInfo.ShowWithData (guid, monsterid);
	}


	public Button closeButton;
	public	Text	name;
	public	Image	propertyImage;
	public	Transform iconTrans;
	public	Transform	skillTrans;

	// Use this for initialization
	void Start ()
	{
		EventTriggerListener.Get (closeButton.gameObject).onClick = OnCloseButtonClick;
		name.text = null; 
	}

	void	OnCloseButtonClick(GameObject go)
	{
		UIMgr.Instance.CloseUI (this);
	}

	public	void	ShowWithData(int guid, string monsterid)
	{
		UnitData unitData = StaticDataMgr.Instance.GetUnitRowData (monsterid);
		if (null == unitData) 
		{
			Logger.LogError("Error:instance info , monsterId config error :" + monsterid);
			return;
		}
		name.text = unitData.nickName;
		SetProperty (unitData.property);

		MonsterIcon icon = MonsterIcon.CreateIcon ();
		icon.transform.SetParent (iconTrans, false);

		icon.SetMonsterStaticId (monsterid);

		SetSpellIcon (unitData);
	}

	private void SetProperty(int property)
	{
		var image = ResourceMgr.Instance.LoadAssetType<Sprite>("ui/property", "property_" + property) as Sprite;
		if(image != null)
		{
			propertyImage.sprite = image;
		}
	}

	private void SetSpellIcon(UnitData unitData)
	{
		bool isBoss = unitData.assetID.Contains ("boss_");
		Spell tempSpell = null;

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
			   spellPt.category == (int)SpellType.Spell_Type_PhyAttack ||
			   spellPt.category == (int)SpellType.Spell_Type_Beneficial||
			   spellPt.category == (int)SpellType.Spell_Type_Negative)
			{
				AddIcon(spellPt);
			}
			else if(spellPt.category == (int)SpellType.Spell_Type_MagicDazhao ||
			        spellPt.category == (int)SpellType.Spell_Type_PhyDaZhao ||
			        spellPt.category == (int)SpellType.Spell_Type_PrepareDazhao)
			{
				if(!isBoss)
				{
					AddIcon(spellPt);
				}
			}
		}	
	}

	private void AddIcon(SpellProtoType spellType)
	{
		var icon = SpellIcon.CreateWith (skillTrans );
		icon.SetData (1, spellType.id);
	}
}
