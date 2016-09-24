using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum InstanceDifficulty :int
{
	Normal = 0,
	Hard
}

public	class InstanceEntryRuntimeData
{
	public	string	instanceId;
	public	int		star;
	public	int	countDaily;
    public bool isOpen = true;
	public	InstanceEntry	staticData;
}
public enum ChapterBoxState
{
   CanNotReceiv = -1,
   CanReceiv,
   HasReceiv
}
public class InstanceMapService : MonoBehaviour 
{
	public	int	openedMaxNormlChapter = 1;
	public	int	openedMaxHardChapter = 0;
	public	List<InstanceEntryRuntimeData>	openChapterInstanceList = new List<InstanceEntryRuntimeData> ();

    public PB.ChapterState chapterState = null;
    public int instanceResetTimes = 0;

	static InstanceMapService mInst = null;
	public static InstanceMapService Instance
	{
		get
		{
			if (mInst == null)
			{
				GameObject go = new GameObject("InstanceMapService");
                mInst = go.AddComponent<InstanceMapService>();

                //TODO: move to ManagerInit?
                DontDestroyOnLoad(go);
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

	public void RefreshInstanceMap(List<PB.InstanceState> listState)
	{
		openChapterInstanceList.Clear ();
       
        AddOpenChapeterInstance (listState);
	}

    public  void ResetCountDaily()
    {
        foreach(var subData in openChapterInstanceList)
        {
            subData.countDaily = 0;
        }
    }

    public void ResetCountDaily(string instanceid)
    {
        var realInstance = GetRuntimeInstance(instanceid);
        if (null != realInstance)
        {
            realInstance.countDaily = 0;
        }
    }
    public void AddCountDaily(string instanceid, int addCount)
    {
        var realInstance = GetRuntimeInstance(instanceid);
        if (null != realInstance)
        {
            realInstance.countDaily += addCount;
        }
    }

    private	void	AddOpenChapeterInstance(List<PB.InstanceState> finishedInstance)
	{
		openedMaxNormlChapter = 1;
		openedMaxHardChapter = 0;

        finishedInstance.Sort(SortFinishInstance);

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
            subRuntimeData.isOpen = true;

			openChapterInstanceList.Add(subRuntimeData);

			//统计
			if(subRuntimeData.staticData.difficulty == (int) InstanceDifficulty.Normal)
			{
				if(openedMaxNormlChapter < subRuntimeData.staticData.chapter)
				{
					openedMaxNormlChapter = subRuntimeData.staticData.chapter;
				}
			}
			else if(subRuntimeData.staticData.difficulty == (int) InstanceDifficulty.Hard)
			{
				if(openedMaxHardChapter < subRuntimeData.staticData.chapter)
				{
					openedMaxHardChapter = subRuntimeData.staticData.chapter;
				}
			}
		}

		AddLeftInstanceInChapter ();
	}

    int SortFinishInstance(PB.InstanceState itemA,PB.InstanceState itemB)
    {
        InstanceEntry insA = StaticDataMgr.Instance.GetInstanceEntry(itemA.instanceId);
        InstanceEntry insB = StaticDataMgr.Instance.GetInstanceEntry(itemB.instanceId);
        if (insA.chapter > insB.chapter)
        {
            return 1;
        }
        else if (insA.chapter<insB.chapter)
        {
            return -1;
        }
        else
        {
            if (insA.difficulty > insB.difficulty)
            {
                return 1;
            }
            else if (insA.difficulty < insB.difficulty)
            {
                return -1;
            }
            else
            {
                if (insA.index > insB.index)
                {
                    return 1;
                }
                else if (insA.index < insB.index)
                {
                    return -1;
                }
            }
        }
        return 1;
    }

	private	void	AddLeftInstanceInChapter()
	{
		AddLeftInstance (InstanceDifficulty.Normal, openedMaxNormlChapter);
		AddLeftInstance (InstanceDifficulty.Hard, openedMaxHardChapter);
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
            bool isOpen = true;
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
					AddNoFinishedInstance(subStaticData,isOpen);
                    isOpen = false;
				}
			}
		}
	}

	private bool CheckAndOpenNextChapter(InstanceDifficulty difftype,int nextChapter)
	{
        if (!IsChapterFinished(nextChapter - 1, difftype))
        {
            return false;
        }
        List<InstanceEntry> listStaticInstance = GetStaticInstance (difftype, nextChapter);
		if (listStaticInstance.Count == 0)
			return false;

        if(difftype == InstanceDifficulty.Hard)
        {
            //检测对应普通副本是否通关
            List<InstanceEntryRuntimeData> listNormalInstance = GetRuntimeInstance(InstanceDifficulty.Normal, nextChapter);
            foreach(var subNormalInstance in listNormalInstance)
            {
                if(!subNormalInstance.isOpen)
                {
                    return false;//对应普通副本未通关，则不能开通困难副本
                }
            }
        }

        Chapter chapter = StaticDataMgr.Instance.GetChapterData(nextChapter);
        if(null == chapter)
        {
            return false;
        }
        int openNeedLevel = 1;
        int openedMaxChapter = 0;
        if(difftype == InstanceDifficulty.Normal)
        {
            openedMaxChapter = openedMaxNormlChapter;
            openNeedLevel = chapter.normalLevel;
        }
        else
        {
            openedMaxChapter = openedMaxHardChapter;
            openNeedLevel = chapter.hardLevel;
        }
        if(openedMaxChapter >= nextChapter)
        {
            return false;//已经开启
        }

		if (GameDataMgr.Instance.PlayerDataAttr.LevelAttr < openNeedLevel)
		{
			return false;//等级不够
		}
        bool isOpen = true;
		foreach (InstanceEntry subEntry in listStaticInstance) 
		{
			AddNoFinishedInstance(subEntry,isOpen);
            isOpen = false;
		}

		if (difftype == InstanceDifficulty.Normal) 
		{
			openedMaxNormlChapter = nextChapter;
		}
		else if (difftype == InstanceDifficulty.Hard)
		{
			openedMaxHardChapter = nextChapter;
		}
		return true;
	}

	private	void AddNoFinishedInstance(InstanceEntry staticData,bool isOpen = false)
	{
		InstanceEntryRuntimeData runtimeData = new InstanceEntryRuntimeData();
		runtimeData.instanceId = staticData.id;
		runtimeData.countDaily = 0;
		runtimeData.star =0;
        runtimeData.isOpen = isOpen;
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
    //------------------------------------------------------------------------------------------------
    public InstanceEntryRuntimeData GetNextRuntimeInstance(string instanceId)
    {
        InstanceEntryRuntimeData curInstance = GetRuntimeInstance(instanceId);
        int nextIndex  = curInstance.staticData.index + 1;

        InstanceEntryRuntimeData subEntry = null;
        for (int i = 0; i < openChapterInstanceList.Count; ++i)
        {
            subEntry = openChapterInstanceList[i];
            if(nextIndex == subEntry.staticData.index &&
                subEntry.staticData.chapter == curInstance.staticData.chapter&&
                subEntry.staticData.difficulty == curInstance.staticData.difficulty)
            {
                return subEntry;
            }
        }
        return null;
    }
    //------------------------------------------------------------------------------------------------

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

    public bool IsChapterFinished(int chapter, InstanceDifficulty diff)
    {
        if (chapter < 1)
            return true;
        List<InstanceEntryRuntimeData> listChapter = GetRuntimeInstance(diff, chapter);
        if(null == listChapter || listChapter.Count < 1)
            return false;
        foreach (var subData in listChapter)
        {
            if(subData.star < 1)
            {
                return false;
            }
        }
        return true;
    }

    public bool IsChapterOpened(int chapter)
    {
        if (chapter <= openedMaxNormlChapter)
            return true;

        if(chapter == openedMaxNormlChapter + 1)
        {
            if (IsChapterFinished(chapter - 1, InstanceDifficulty.Normal))
            {
                CheckAndOpenNextChapter(InstanceDifficulty.Normal, chapter);
            }
        }
        return chapter <= openedMaxNormlChapter;
    }

    public  bool    IsHardChapterOpend(int chapter)
    {
        if(chapter <= openedMaxHardChapter)
         return true;
        if (!IsChapterOpened(chapter))
            return false;
        if(chapter == openedMaxHardChapter + 1)
        {
            if (IsChapterFinished(chapter - 1, InstanceDifficulty.Hard))
            {
                CheckAndOpenNextChapter(InstanceDifficulty.Hard, chapter);
            }
        }
        return chapter <= openedMaxHardChapter;
    }

    public  ChapterBoxState GetChapterBoxState(int chapter,InstanceDifficulty  diffType)
    {
        if(null == chapterState || chapter < 1)
            return ChapterBoxState.CanNotReceiv;
        List<int> stateList = null;
        if (InstanceDifficulty.Normal == diffType)
        {
            stateList = chapterState.normalBoxState;
        }
        else
        {
            stateList = chapterState.hardBoxState;
        }
        if(chapter > stateList.Count)
        {
            return ChapterBoxState.CanNotReceiv;
        }
        return (ChapterBoxState)stateList[chapter-1];
    }

    public  void    SetChapterBoxState(int chapter,InstanceDifficulty diffType,ChapterBoxState state)
    {
        List<int> stateList = null;
        if (InstanceDifficulty.Normal == diffType)
        {
            stateList = chapterState.normalBoxState;
        }
        else
        {
            stateList = chapterState.hardBoxState;
        }

        if(chapter <= stateList.Count)
        {
            stateList[chapter - 1] = (int)state;
        }
        else
        {
            for(int i =0;i<chapter - stateList.Count -1;++i)
            {
                stateList.Add((int)ChapterBoxState.CanNotReceiv);
            }
            stateList.Add((int)state);
        }
    }


	#region  Events

	void	OnFinishedInstnace(int star, string instanceID)
	{
		if (star < 1)
			return;
		InstanceEntryRuntimeData runtimeInstance = GetRuntimeInstance (instanceID);
		if (null == runtimeInstance) 
		{
			Logger.LogError("Error:can't find instanceid in runtime  Id = " + instanceID);
			return;
		}
		if (runtimeInstance.star < star) 
		{
			runtimeInstance.star = star;
		}
		runtimeInstance.countDaily ++;

        //next instanceid
        InstanceEntryRuntimeData nextInstance = GetNextRuntimeInstance(instanceID);
        if(nextInstance != null)
        {
            nextInstance.isOpen = true;
        }
        else
        {
            //开启下一章节
            int nextChapter = runtimeInstance.staticData.chapter + 1;
            bool openNextChapter = CheckAndOpenNextChapter((InstanceDifficulty)runtimeInstance.staticData.difficulty, nextChapter);
            if(openNextChapter)
            {
                GameEventMgr.Instance.FireEvent<int>(GameEventList.OpenNewChapter, nextChapter);
            }
        }
	}

	#endregion

}
