using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class makeWar : MonoBehaviour {
    public GameObject startButton;
    public InputField fbId;//副本ID
    public InputField operationID;//玩家操作id
    public InputField xhNum;//循环次数
    public List<GameObject> gwList;
    GwData gwData = new GwData();
    AttData attData;
	// Use this for initialization
    void Awake()
    {
        StaticDataMgr.Instance.InitData();
    }

	void Start () {
        EventTriggerListener.Get(startButton).onClick = BattleBegins;
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
    //------------------------------------------------------
    void BattleBegins(GameObject start)//开始战斗
    {
        for (int i = 0; i < gwList.Count; i++)
        {
            attData = gwList[i].GetComponent<AttData>();
            if (attData.gwID != null)
            {
                gwData.gwIdList.Add(attData.gwID.text);
                gwData.gwLvList.Add(attData.gwLv.text);
                gwData.gwCharacterList.Add(attData.gwCharacter.text);
            }
        }
        UnitData uData = StaticDataMgr.Instance.GetUnitRowData(gwData.gwIdList[0]);
        Debug.Log("名字: " + uData.nickName);
    }
}
