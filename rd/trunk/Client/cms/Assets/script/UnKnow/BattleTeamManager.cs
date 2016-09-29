using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleTeamManager 
{
	public class	TeamList
	{
		public	static	string	Defualt = "default";
        public static string Pvp = "PvpTeam";

		public	static	string	Custome1 = "Custome1";
	}

	public static	List<string>	GetTeamWithKey(string teamKey)
	{
		List<string> teamList = new List<string> ();

		string teamString = PlayerPrefs.GetString (teamKey);
		if (! string.IsNullOrEmpty (teamString)) 
		{
			string [] iteams =  teamString.Split(',');
			if(iteams.Length ==  6)
			{
				for(int i =0;i< iteams.Length;++i)
				{
                    string subGuid = iteams[i];
                    if(!string.IsNullOrEmpty(subGuid))
                    {
                        int guid = int.Parse(subGuid);
                        GameUnit subUnit = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(guid);
                        if (subUnit == null || subUnit.pbUnit.IsInAdventure())
                        {
                            subGuid = "";
                        }
                    }
					teamList.Add(subGuid);
				}
				return teamList;
			}
		}

		for(int i =0;i<6;++i)
		{
			teamList.Add("");
		}

		return teamList;
	}

	public static	void	SetTeam(List<string> teamList,string teamKey)
	{
		if (teamList.Count != 6)
		{
			Logger.LogError("SetTeamError forkey:" + teamKey);
			return;
		}
		string teamString = "";
		string subString = "";
		for (int i = 0; i<teamList.Count; ++i)
		{
			if(i != 0)
			{
				teamString += ",";
			}

			subString = teamList[i];
			if(null == subString)
				subString = "";
			teamString += subString;
		}
		PlayerPrefs.SetString (teamKey, teamString);
	}
}
