using UnityEngine;
using System.Collections;

public class GameDataMgr : MonoBehaviour 
{
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
	public void Init()
	{
		DontDestroyOnLoad(gameObject);
		InitData();
		BindListener();
	}

	void InitData()
	{
		GameObject playerData = new GameObject("PlayerData");
		m_PlayerData = playerData.AddComponent<PlayerData>();
		m_PlayerData.transform.parent = transform;
	}
	void BindListener()
	{
		//GameEventMgr.Instance.AddListener<Coin>(GameEventList.EatCoin, OnEatCoin);
	}

	void UnBindListener()
	{
		//GameEventMgr.Instance.RemoveListener<Coin>(GameEventList.EatCoin, OnEatCoin);
	}

}
