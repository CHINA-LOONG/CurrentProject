using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PartType
{
    Head = 1,
    Body,
    Waist,
    Neck,
    Wrist,
    Finger,

    NUM_EQUIP_PART = Finger
}


public class ItemStaticData 
{
	public  string id;
	public	string	name;
	public	int	classType;
	public	string	asset;
	public	int	type;
    public int subType;

   // public int level;
	public	int	grade;
	public	int	minLevel;
	public	int	condition;
	public	int	times; 	
	public	string	tips;

	public	int	bindType;
	public	int	sellPrice;
	public	int	sellType;
	public	int	buyPrice;
	public	int	buyType;

	public	int	stack;
    public float forgeAdjust;
    public	string	rewardId;
	public	string	componentItem;
	public	string	needItem;
	public	string	targetItem;

	public	int	needCount;
	public	int	addAttrType;
	public	int	addAttrValue;
	public	int	gemId;
    public int gemType;

	public	int	part;
	public	int	durability;
    public string itemfounds;
    private List<List<string>> foundList;
    public List<List<string>> FoundList
    {
        get
        {
            if (!string.IsNullOrEmpty(itemfounds)&&foundList==null)
            {
                foundList = new List<List<string>>();
                string[] items = itemfounds.Split(',');
                for (int i = 0; i < items.Length; i++)
                {
                    foundList.Add(new List<string>(items[i].Split('_')));
                }
            }
            return foundList;
        }
    }

    public	string	NameAttr
	{
		get
		{
            return StaticDataMgr.Instance.GetTextByID(name, LanguageType.ItemsLanguage);
		}
	}

	public string TipsAttr
	{
		get
		{
            return StaticDataMgr.Instance.GetTextByID(tips, LanguageType.ItemsLanguage);
		}
	}

    public ItemInfo TargetItem
    {
        get
        {
            return ItemInfo.valueof(targetItem, ItemParseType.DemandItemType);
        }
    }
    public string PartAttr
    {
        get
        {
            return GetEquipPart(part);
        }
    }
    public static string GetEquipPart(int part)
    {
        switch (part)
        {
            case 1:
                return StaticDataMgr.Instance.GetTextByID("equip_Weapon");
            case 2:
                return StaticDataMgr.Instance.GetTextByID("equip_Armor");
            case 3:
                return StaticDataMgr.Instance.GetTextByID("equip_Helmet");
            case 4:
                return StaticDataMgr.Instance.GetTextByID("equip_Bracer");
            case 5:
                return StaticDataMgr.Instance.GetTextByID("equip_Ring");
            case 6:
                return StaticDataMgr.Instance.GetTextByID("equip_Jewelry");
            default:
                return "";
        }
    }
}
