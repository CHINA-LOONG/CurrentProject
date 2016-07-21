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
	public  Text	character;
	public	Image	propertyImage;
	public	Transform iconTrans;
	public	Transform	normalSkillTrans;
	public  Transform	specialSkillTrans;
	public	SkilTips	skilTips;
	public Text	haveNomralSpell;
	public Text haveSpecialSpell;
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
		haveNomralSpell.text = StaticDataMgr.Instance.GetTextByID ("spell_yongyoujineng_normal");
		haveSpecialSpell.text = StaticDataMgr.Instance.GetTextByID ("spell_yongyoujineng_special");

		SetProperty (unitData.property);

		MonsterIcon icon = MonsterIcon.CreateIcon ();
		icon.transform.SetParent (iconTrans, false);

		icon.SetMonsterStaticId (monsterid);
		icon.SetStage (stage);
		icon.SetLevel (level);
		SetSpellIcon (unitData);

		int monsterCharacter = unitData.character;
		if (guid != -1)
		{
			GameUnit pet = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(guid);
			if(pet != null)
			{
				monsterCharacter = pet.character;
			}
		}
		CharacterData cData = StaticDataMgr.Instance.GetCharacterData (monsterCharacter);
		if (null != cData)
		{
			character.text = cData.DescAttr;
		}
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
		Transform parentTrans = null;
		if (spellType.category == (int)SpellType.Spell_Type_MagicDazhao ||
			spellType.category == (int)SpellType.Spell_Type_PhyDaZhao) 
		{
			parentTrans = specialSkillTrans;
		}
		else 
		{
			parentTrans = normalSkillTrans;
		}
		var icon = SpellIcon.CreateWith (parentTrans );
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

		RectTransform iconParent = icon.transform.parent as RectTransform;
		tipsPos.y = iconParent.anchoredPosition.y + 120;

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
