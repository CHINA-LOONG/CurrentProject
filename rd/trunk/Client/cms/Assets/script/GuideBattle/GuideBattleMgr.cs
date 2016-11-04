using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuideLevelParam
{
    public List<string> selfIdList = new List<string>();
    public List<string> enemyIdList = new List<string>();
    public string instanceId;
}


public class GuideBattleMgr : MonoBehaviour
{
    public GuideBattleData mCurGuideData;
    public int mCurStepIndex;
    public GuideBattleStepData mCurStep;

    //---------------------------------------------------------------------------------------------
    public void StartGuideBattle(int id)
    {
        mCurGuideData = StaticDataMgr.Instance.GetGuideBattleData(id);
        mCurStepIndex = 0;
    }
    //---------------------------------------------------------------------------------------------
    void ActiveStep()
    {
        if (
            mCurGuideData != null && 
            mCurStepIndex >= 0 &&
            mCurStepIndex < mCurGuideData.battleStepList.Count
            )
        {
            
        }
    }
    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------
}
