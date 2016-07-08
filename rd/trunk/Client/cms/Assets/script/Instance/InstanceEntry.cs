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
	public	int	reward1;
	public	int	reward2;
	public	int	reward3;
	public	int	reward4;
	public	int	reward5;
	public	int	reward6;

	public List<string> enemyList;
	public List<int> rewardList;

	public void AdaptData()
	{
		enemyList = new List<string>();
		rewardList = new List<int>();
		
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
		
		if(reward1 > 0)
			rewardList.Add(reward1);
		
		if(reward2 > 0)
			rewardList.Add(reward2);
		
		if(reward3 > 0)
			rewardList.Add(reward3);
		
		if(reward4 > 0)
			rewardList.Add(reward4);
		
		if(reward5 > 0)
			rewardList.Add(reward5);
		
		if(reward6 > 0)
			rewardList.Add(reward6);

	}
}
