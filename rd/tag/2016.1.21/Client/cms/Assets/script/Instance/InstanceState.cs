using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InstanceState : MonoBehaviour 
{
    /*
    //todo:delete
	public List<PB.InstanceState> listInstanceState = new List<PB.InstanceState>();
	// Use this for initialization
	void Start () 
	{
	
	}

	public void Init()
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
		GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.STATISTICS_INFO_SYNC_S.GetHashCode().ToString(), OnStatisticsInfoSyncFinished);
	}
	//---------------------------------------------------------------------------------------------
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.STATISTICS_INFO_SYNC_S.GetHashCode().ToString(), OnStatisticsInfoSyncFinished);
	}

	void OnStatisticsInfoSyncFinished(ProtocolMessage msg)
	{
		PB.HSStatisticsInfoSync  statisticsInfo = msg.GetProtocolBody<PB.HSStatisticsInfoSync> ();
		if (null == statisticsInfo)
		{
			Logger.LogError("StatisticsSync Error!");
			return;
		}

		if (statisticsInfo.instanceState != null)
		{
			listInstanceState.Clear();
			listInstanceState.AddRange(statisticsInfo.instanceState);

			InstanceMapService.Instance.RefreshInstanceMap(this);
		}

        if(statisticsInfo.chapterState != null)
        {
            InstanceMapService.Instance.chapterState = statisticsInfo.chapterState;
        }
	}*/
}
