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
    public int bp;//敌方阵容战力
    public  string reward;
    public string bgzhuangshi;
    public string instanceSpell;

    public List<string> enemyList;

    private Dictionary<string, int> enmeyStateDic;



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
        enmeyStateDic = new Dictionary<string, int>();

        if (!string.IsNullOrEmpty(enemy1))
            AddEnemy(enemy1);
		
		if(!string.IsNullOrEmpty(enemy2))
            AddEnemy(enemy2);
		
		if(!string.IsNullOrEmpty(enemy3))
            AddEnemy(enemy3);
		
		if(!string.IsNullOrEmpty(enemy4))
            AddEnemy(enemy4);
		
		if(!string.IsNullOrEmpty(enemy5))
            AddEnemy(enemy5);
		
		if(!string.IsNullOrEmpty(enemy6))
            AddEnemy(enemy6);
	}

    void AddEnemy(string enemy)
    {
        string[] szData = enemy.Split(',');
        if(null!=szData && szData.Length ==2)
        {
            enemyList.Add(szData[0]);
            enmeyStateDic.Add(szData[0], int.Parse(szData[1]));
        }
    }

    public  int GetEnemyState(string enemyId)
    {
        int stage = 0;
        if(enmeyStateDic.TryGetValue(enemyId,out stage))
        {
            return stage;
        }
        return 1;
    }
}
