using UnityEngine;
using System.Collections;

public class BattlePositionProxy : MonoBehaviour 
{
	public enum PositionType 
	{
		Normal = 0,
		Boss
	}

	[SerializeField]
	PositionType	m_postionType = PositionType.Normal;
	public	PositionType PositionTypeAttr
	{
		set
		{
			m_postionType = value;
		}
		get
		{
			return m_postionType;
		}
	}

	[SerializeField]
	int	m_ID = 0;
	public	int	IDAttr
	{
		 set
		{
			m_ID = value;
		}
		 get
		{
			return m_ID;
		}
	}


    public UnitCamp camp;
    public int slot;
}
