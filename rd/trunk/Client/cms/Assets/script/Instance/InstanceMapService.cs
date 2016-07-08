using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum InstanceDifficulty :int
{
	Normal = 0,
	Difficult
}

public	class InstanceEntryRuntimeData
{
	public	string	instanceId;
	public	int		star;
	public	int	countDaily;

	public	InstanceEntry	staticData;
}
public class InstanceMapService : MonoBehaviour 
{
	public	int	openedMaxNormlChapter = 1;
	public	int	openedMaxDifficultyChapter = 1;
	public	List<InstanceEntryRuntimeData>	openChapterInstanceList = new List<InstanceEntryRuntimeData> ();

	static InstanceMapService mInst = null;
	public static InstanceMapService Instance
	{
		get
		{
			if (mInst == null)
			{
				GameObject go = new GameObject("InstanceMapService");
				mInst = go.AddComponent<InstanceMapService>();
			}
			return mInst;
		}
	}

	void Start()
	{
		BindListener ();
	}

	void OnDestroy()
	{
		UnBindListener();
	}
	//---------------------------------------------------------------------------------------------
	void BindListener()
	{
		GameEventMgr.Instance.AddListener<int,string> (GameEventList.FinishedInstance, OnFinishedInstnace);
	}
	//---------------------------------------------------------------------------------------------
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener<int,string> (GameEventList.FinishedInstance, OnFinishedInstnace);
	}

	public void RefreshInstanceMap(InstanceState instanceState)
	{
		openChapterInstanceList.Clear ();

		AddOpenChapeterInstance (instanceState.listInstanceState);
	}

	private	void	AddOpenChapeterInstance(List<PB.InstanceState> finishedInstance)
	{
		openedMaxNormlChapter = 1;
		openedMaxDifficultyChapter = 1;

		PB.InstanceState subState = null;
		for (int i = 0; i< finishedInstance.Count; ++i) 
		{
			subState = finishedInstance [i];
			InstanceEntryRuntimeData subRuntimeData = new InstanceEntryRuntimeData();

			subRuntimeData.staticData = StaticDataMgr.Instance.GetInstanceEntry(subState.instanceId);
			if(null == subRuntimeData.staticData)
			{
				Logger.LogError("Error:instanceEntry can't have the recored instanceId = " + subState.instanceId);
				continue;
			}
			subRuntimeData.instanceId = subState.instanceId;
			subRuntimeData.star = subState.star;
			subRuntimeData.countDaily = subState.countDaily;

			openChapterInstanceList.Add(subRuntimeData);

			//统计
			if(subRuntimeData.staticData.difficulty == (int) InstanceDifficulty.Normal)
			{
				if(openedMaxNormlChapter < subRuntimeData.staticData.chapter)
				{
					openedMaxNormlChapter = subRuntimeData.staticData.chapter;
				}
			}
			else if(subRuntimeData.staticData.difficulty == (int) InstanceDifficulty.Difficult)
			{
				if(openedMaxDifficultyChapter < subRuntimeData.staticData.chapter)
				{
					openedMaxDifficultyChapter = subRuntimeData.staticData.chapter;
				}
			}
		}

		AddLeftInstanceInChapter ();
	}

	private	void	AddLeftInstanceInChapter()
	{
		AddLeftInstance (InstanceDifficulty.Normal, openedMaxNormlChapter);
		AddLeftInstance (InstanceDifficulty.Difficult, openedMaxDifficultyChapter);
	}

	private	void	AddLeftInstance(InstanceDifficulty diffType,int maxChapter)
	{
		List<InstanceEntryRuntimeData> listRunTimeInstance = GetRuntimeInstance (diffType, maxChapter);
		List<InstanceEntry> listStaticInstance = GetStaticInstance (diffType, maxChapter);

		if (listRunTimeInstance.Count == listStaticInstance.Count)
		{
			//下一章是否开启
			CheckAndOpenNextChapter(diffType,maxChapter + 1);
		}
		else
		{
			InstanceEntry subStaticData = null;
			for(int i =0 ; i < listStaticInstance.Count ; ++ i)
			{
				subStaticData = listStaticInstance[i];

				bool isExist = false;
				foreach(InstanceEntryRuntimeData subRuntime in listRunTimeInstance)
				{
					if(subRuntime.instanceId.EndsWith(subStaticData.id))
					{
						isExist =true;
						break;
					}
				}
				if(!isExist)
				{
					AddNoFinishedInstance(subStaticData);
				}
			}
		}
	}

	private bool CheckAndOpenNextChapter(InstanceDifficulty difftype,int nextChapter)
	{
		List<InstanceEntry> listStaticInstance = GetStaticInstance (difftype, nextChapter);
		if (listStaticInstance.Count == 0)
			return false;

		int openLevel = 1;
		foreach (InstanceEntry subEntry in listStaticInstance) 
		{
			if(openLevel < subEntry.level)
			{
				openLevel = subEntry.level;
			}
		}
		if (GameDataMgr.Instance.PlayerDataAttr.level < openLevel)
		{
			return false;
		}

		foreach (InstanceEntry subEntry in listStaticInstance) 
		{
			AddNoFinishedInstance(subEntry);
		}

		if (difftype == InstanceDifficulty.Normal) 
		{
			openedMaxNormlChapter = nextChapter;
		}
		else if (difftype == InstanceDifficulty.Difficult)
		{
			openedMaxDifficultyChapter = nextChapter;
		}
		return true;
	}

	private	void AddNoFinishedInstance(InstanceEntry staticData)
	{
		InstanceEntryRuntimeData runtimeData = new InstanceEntryRuntimeData();
		runtimeData.instanceId = staticData.id;
		runtimeData.countDaily = 0;
		runtimeData.star =0;
		runtimeData.staticData = staticData;
		
		openChapterInstanceList.Add(runtimeData);
	}

	//------------------------------------------------------------------------------------------------

	public InstanceEntryRuntimeData	GetRuntimeInstance(string instanceId)
	{
		InstanceEntryRuntimeData subEntry = null;
		for (int i =0; i < this.openChapterInstanceList.Count; ++i) 
		{
			subEntry = openChapterInstanceList[i];
			if(subEntry.instanceId.EndsWith(instanceId))
				return subEntry;
		}
		return null;
	}

	public	List<InstanceEntryRuntimeData> GetRuntimeInstance(InstanceDifficulty diffType,int chapter)
	{
		List<InstanceEntryRuntimeData> listReturn = new List<InstanceEntryRuntimeData> ();
		
		InstanceEntryRuntimeData subEntry = null;
		for (int i =0; i < this.openChapterInstanceList.Count; ++i) 
		{
			subEntry = openChapterInstanceList[i];
			
			if((int) diffType == subEntry.staticData.difficulty  &&
			   chapter == subEntry.staticData.chapter)
			{
				listReturn.Add(subEntry);
			}
		}
		return listReturn;
	}
	
	public List<InstanceEntry> GetStaticInstance(InstanceDifficulty diffType,int chapter)
	{
		return StaticDataMgr.Instance.GetInstanceEntryList((int) diffType,chapter);
	}

	#region  Events

	void	OnFinishedInstnace(int star, string instanceID)
	{
		if (star < 1)
			return;
		InstanceEntryRuntimeData runtimeInstance = GetRuntimeInstance (instanceID);
		if (null == runtimeInstance) 
		{
			Debug.LogError("Error:can't find instanceid in runtime  Id = " + instanceID);
			return;
		}
		if (runtimeInstance.star < star) 
		{
			runtimeInstance.star = star;
		}
		runtimeInstance.countDaily ++;

		//todo:other
	}

	#endregion

}
