using UnityEngine;
using System.Collections;

public enum PartType
{
    Head,
    Body,
    Waist,
    Neck,
    Wrist,
    Finger,
    
    NUM_EQUIP_PART
}


public class ItemStaticData 
{
	public  string id;
	public	string	name;
	public	int	classType;
	public	string	asset;
	public	int	type;
    public int subType;

    public int level;
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
	public	int	rewardId;
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
	public	int	equipAttId;
}
