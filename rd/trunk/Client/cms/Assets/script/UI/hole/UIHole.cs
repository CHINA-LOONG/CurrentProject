using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
public class UIHole : UIBase
{
    public static string ViewName = "UISelectDifficulty";
    public static UIHole OpenHole(int holeType)
    {
        UIHole hole = UIMgr.Instance.OpenUI_(UIHole.ViewName, false) as UIHole;
		hole.ShowHole(holeType);
        return hole;
    }
    public Transform fatherBox;
    public GameObject Close;//关闭
    public Text selectDifficultyTitle;
    List<GameObject> difficultyObjs = new List<GameObject>();
    HoleData holeData;
    int[] vitalityNum;//活力消耗
    //---------------------------------------------------------------------------------------------------------------------------------------
    // Use this for initialization
    void Start()
    {
        selectDifficultyTitle.text = StaticDataMgr.Instance.GetTextByID("tower_difficulty_title");
        EventTriggerListener.Get(Close).onClick = ExitClick;
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
    private void ShowHole(int holeType)
    {
        InstanceEntry instEntry;
        HoleItemData holeItemData;
        InstanceData instanceData;
        holeData = StaticDataMgr.Instance.GetHoleData(holeType);
        vitalityNum = new int[holeData.difficultyList.Count];
        for (int i = 0; i < holeData.difficultyList.Count; i++)
        {
            instEntry = StaticDataMgr.Instance.GetInstanceEntry(holeData.difficultyList[i]);
            vitalityNum[i] = instEntry.fatigue;
            difficultyObjs.Add(ResourceMgr.Instance.LoadAsset("holeItem"));
            EventTriggerListener.Get(difficultyObjs[i]).onClick = HoleItemClick;
			difficultyObjs[i].transform.SetParent(fatherBox, false);
            holeItemData = difficultyObjs[i].GetComponent<HoleItemData>();
            holeItemData.fbID = holeData.difficultyList[i];
            holeItemData.difficultyText.text = StaticDataMgr.Instance.GetTextByID(instEntry.name);//难度
            holeItemData.vitalityNumText.text = vitalityNum[i].ToString();
            instanceData = StaticDataMgr.Instance.GetInstanceData(holeData.difficultyList[i]);
            holeItemData.holeLevel = instanceData.instanceProtoData.level;
            if (GameDataMgr.Instance.PlayerDataAttr.LevelAttr >= instanceData.instanceProtoData.level)
            {
                holeItemData.leveLimit.SetActive(false);
            }
        }
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
    void ExitClick(GameObject bnt)
    {
        for (int i = 0; i < difficultyObjs.Count; i++)
        {
            ResourceMgr.Instance.DestroyAsset(difficultyObjs[i]);
        }
        difficultyObjs.Clear();
        UIMgr.Instance.DestroyUI(transform.GetComponent<UIHole>());
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
    void HoleItemClick(GameObject bnt)
    {
		HoleItemData holeItemData = bnt.GetComponent<HoleItemData>();
        if (holeItemData.holeLevel > GameDataMgr.Instance.PlayerDataAttr.LevelAttr)
        {
            UIIm.Instance.ShowSystemHints(string.Format(StaticDataMgr.Instance.GetTextByID("tower_record_003"), holeItemData.holeLevel), (int)PB.ImType.PROMPT);
        }
        else
        {
			holeItemData.RequestEnterHole();
        }
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
	void OnRequestEnterHoleFinished(ProtocolMessage msg)
	{
		
	}
}
