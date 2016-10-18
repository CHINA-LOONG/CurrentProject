using UnityEngine;
using System.Collections;

public class CharacterData 
{
	public int 	index;
    public string name;
	public	string desc;
    public string picture;
    public int	physicsWeight;
	public int	magicWeight;
	public int	cureMagicWeight;
	public int	gainWeight;
	public	int	negativeWeight;
	public int	defenseWeight;
	public	int	tauntDefenseWeight;

	public string DescAttr
	{
		get
		{
			return StaticDataMgr.Instance.GetTextByID(desc);
		}
	}
}
