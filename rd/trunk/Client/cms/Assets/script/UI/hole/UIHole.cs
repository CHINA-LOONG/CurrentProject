using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
public class UIHole : UIBase
{
    public static string ViewName = "UISelectDifficulty";
    public static UIHole OpenHole(int holeType)
    {
        UIHole hole = UIMgr.Instance.OpenUI_(UIHole.ViewName) as UIHole;
		hole.ShowHole(holeType);
        return hole;
    }
    public Transform fatherBox;
    public GameObject Close;//关闭
    public Text selectDifficultyTitle;
    List<GameObject> difficultyObjs = new List<GameObject>();
    HoleData holeData;
    int[] vitalityNum;//活力消耗
    private int mCurrentHoleType;
    //---------------------------------------------------------------------------------------------------------------------------------------
    // Use this for initialization
    void Start()
    {
        selectDifficultyTitle.text = StaticDataMgr.Instance.GetTextByID("tower_difficulty_title");
        EventTriggerListener.Get(Close).onClick = ExitClick;
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
    public int GetSelectHoleType()
    {
        return mCurrentHoleType;
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
    private void ShowHole(int holeType)
    {
        mCurrentHoleType = holeType;
        InstanceEntry instEntry;
        HoleItemData holeItemData;
        InstanceData instanceData;
        Sprite difficultyImageAsset = null;
        holeData = StaticDataMgr.Instance.GetHoleData(holeType);
        vitalityNum = new int[holeData.difficultyList.Count];
        for (int i = 0; i < holeData.difficultyList.Count; i++)
        {
            instEntry = StaticDataMgr.Instance.GetInstanceEntry(holeData.difficultyList[i]);
            vitalityNum[i] = instEntry.fatigue;
            difficultyObjs.Add(ResourceMgr.Instance.LoadAsset("holeItem"));
            ScrollViewEventListener.Get(difficultyObjs[i]).onClick = HoleItemClick;
            difficultyObjs[i].transform.SetParent(fatherBox, false);
            holeItemData = difficultyObjs[i].GetComponent<HoleItemData>();
            if (i >= 0 && i < 4)
            {
                for (int j = 0; j < 3 - i; j++)
                {
                    holeItemData.difficulty[j].SetActive(false);
                }
                difficultyImageAsset = ResourceMgr.Instance.LoadAssetType<Sprite>("tongtianta_nandukulou1");
            }
            else if (i >= 4 && i < 8)
            {
                for (int j = 0; j < 7 - i; j++)
                {
                    holeItemData.difficulty[j].SetActive(false);
                }
                difficultyImageAsset = ResourceMgr.Instance.LoadAssetType<Sprite>("tongtianta_nandukulou2");
            }
            else if (i >= 8)
            {
                for (int j = 0; j < 11 - i; j++)
                {
                    holeItemData.difficulty[j].SetActive(false);
                }
                difficultyImageAsset = ResourceMgr.Instance.LoadAssetType<Sprite>("tongtianta_nandukulou3");
            }
            holeItemData.difficultyImage.sprite = difficultyImageAsset;
            holeItemData.difficultyImage.SetNativeSize();
            holeItemData.fbID = holeData.difficultyList[i];
            holeItemData.difficultyText.text = StaticDataMgr.Instance.GetTextByID(instEntry.name);//难度
            holeItemData.vitalityNumText.text = vitalityNum[i].ToString();
            instanceData = StaticDataMgr.Instance.GetInstanceData(holeData.difficultyList[i]);
            holeItemData.holeLevel = instanceData.instanceProtoData.level;
            if (GameDataMgr.Instance.PlayerDataAttr.LevelAttr >= instanceData.instanceProtoData.level)
                holeItemData.leveLimit.SetActive(false);
            else
                holeItemData.consume.SetActive(false);
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
        if (GameDataMgr.Instance.PlayerDataAttr.HuoliAttr < int.Parse(holeItemData.vitalityNumText.text))
        {
			UseHuoLi.Open();
            return;
        }

        int holeCount = GameDataMgr.Instance.mHoleStateList.Count;
        for (int i = 0; i < holeCount; ++i)
        {
            if (GameDataMgr.Instance.mHoleStateList[i].holeId == mCurrentHoleType)
            {
                if (GameDataMgr.Instance.mHoleStateList[i].isOpen == false)
                {
                    UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("tower_record_004"), (int)PB.ImType.PROMPT);
                    return;
                }
            }
        }

        if (holeItemData.holeLevel > GameDataMgr.Instance.PlayerDataAttr.LevelAttr)
        {
            UIIm.Instance.ShowSystemHints(string.Format(StaticDataMgr.Instance.GetTextByID("tower_record_003"), holeItemData.holeLevel), (int)PB.ImType.PROMPT);
        }
        else
        {
            UIAdjustBattleTeam.OpenWith(holeItemData.fbID, 0,false, InstanceType.Hole);
        }
    }
    //---------------------------------------------------------------------------------------------------------------------------------------
}
