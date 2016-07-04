using UnityEngine;
using System.Collections;

public class GameDataMgr : MonoBehaviour 
{
	static GameDataMgr mInst = null;
	public static GameDataMgr Instance
	{
		get
		{
			if (mInst == null)
			{
				GameObject go = new GameObject("GameDataMgr");
				mInst = go.AddComponent<GameDataMgr>();
			}
			return mInst;
		}
	}

	[SerializeField]
	PlayerData m_PlayerData;
	public PlayerData PlayerDataAttr
	{
		get
		{
			return m_PlayerData;
		}
		set
		{
			m_PlayerData = value;
		}
	}

	[SerializeField]
	UserData userData;
	public UserData UserDataAttr
	{
		get
		{
			return userData;
		}
	}

	public void Init()
	{
		DontDestroyOnLoad(gameObject);
		InitData();
	}

	void InitData()
	{
		GameObject playerData = new GameObject("PlayerData");
		playerData.transform.SetParent (transform);
		m_PlayerData = playerData.AddComponent<PlayerData>();
		m_PlayerData.Init ();

		GameObject userDataGo = new GameObject("UserData");
		userDataGo.transform.SetParent (transform);
		userData = userDataGo.AddComponent<UserData>();
		userData.Init ();
	}
}
