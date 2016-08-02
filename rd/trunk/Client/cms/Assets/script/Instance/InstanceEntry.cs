using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InstanceEntry 
{
	public	string	id;
	public	string	name;
	public	int	chapter;
    public int index;
	public	int	type;
	//public	string	icon;
	public	int	difficulty;
	public	int	fatigue;
	public	int	count;
	//public	int 	level;
	public	string	desc;
	public	string	enemy1;
	public	string	enemy2;
	public	string	enemy3;
	public	string	enemy4;
	public	string	enemy5;
	public	string	enemy6;
    public  string reward;
    public string bgzhuangshi;
    public string instanceSpell;

    public List<string> enemyList;
	//public List<string> rewardList;


	public string NameAttr
	{
		get
		{
			return StaticDataMgr.Instance.GetTextByID(name);
		}
	}

	public string DescAttr
	{
		get
		{
			return StaticDataMgr.Instance.GetTextByID(desc);
		}
	}

	public void AdaptData()
	{
		enemyList = new List<string>();
       // rewardList = new List<string>();
		
		if(!string.IsNullOrEmpty(enemy1))
			enemyList.Add(enemy1);
		
		if(!string.IsNullOrEmpty(enemy2))
			enemyList.Add(enemy2);
		
		if(!string.IsNullOrEmpty(enemy3))
			enemyList.Add(enemy3);
		
		if(!string.IsNullOrEmpty(enemy4))
			enemyList.Add(enemy4);
		
		if(!string.IsNullOrEmpty(enemy5))
			enemyList.Add(enemy5);
		
		if(!string.IsNullOrEmpty(enemy6))
			enemyList.Add(enemy6);
	}
}
