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

}
