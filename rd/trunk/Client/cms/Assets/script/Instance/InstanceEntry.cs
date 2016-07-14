using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InstanceEntry 
{
	public	string	id;
	public	string	name;
	public	int	chapter;
	public	int	type;
	public	string	icon;
	public	int	difficulty;
	public	int	fatigue;
	public	int	count;
	public	int 	level;
	public	string	desc;
	public	string	enemy1;
	public	string	enemy2;
	public	string	enemy3;
	public	string	enemy4;
	public	string	enemy5;
	public	string	enemy6;
    public  string reward1;
    public  string reward2;
    public  string reward3;
    public  string reward4;
    public  string reward5;
	public	string	reward6;

	public List<string> enemyList;
	public List<string> rewardList;

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
        rewardList = new List<string>();
		
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


		if (!string.IsNullOrEmpty (reward1))
			rewardList.Add(reward1);

		if (!string.IsNullOrEmpty (reward2))
			rewardList.Add(reward2);

		if (!string.IsNullOrEmpty (reward3))
			rewardList.Add(reward3);

		if (!string.IsNullOrEmpty (reward4))
			rewardList.Add(reward4);

		if (!string.IsNullOrEmpty (reward5))
			rewardList.Add(reward5);

		if (!string.IsNullOrEmpty (reward6))
			rewardList.Add(reward6);

	}
}
