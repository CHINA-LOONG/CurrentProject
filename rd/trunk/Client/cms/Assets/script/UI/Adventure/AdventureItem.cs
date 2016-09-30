using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public interface IAdventureItem
{
    void openAdventureTeams();
    void openAdventureLayout(AdventureInfo info);
}

public class AdventureItem : MonoBehaviour
{
    public Text text_Msg1;  //未开始使用

    public Text text_Msg2;  //进行中和结束使用
    public Text text_Msg3;

    public Color msg2StartColor = new Color();
    public Color msg3StartColor = new Color();
    public Color msg2FinishColor = new Color();
    public Color msg3FinishColor = new Color();

    public GameObject ObjEffect;

    public Transform ImagePos;
    private string ImageName;
    private GameObject ObjImage;

    public enum State
    {
        WEIKAI,
        JINXING,
        JIESHU
    }

    private State state;
    public State CurState
    {
        get { return state; }
        set
        {
            text_Msg1.gameObject.SetActive(value == State.WEIKAI);
            text_Msg2.gameObject.SetActive(value != State.WEIKAI);
            text_Msg3.gameObject.SetActive(value != State.WEIKAI);
            state = value;
        }
    }

    private AdventureInfo curdata;
    public AdventureInfo curData
    {
        get { return curdata; }
        set
        {
            if (curdata != null)
            {
                RemoveUpdateTime(curdata);
            }
            curdata = value;
        }
    }
    [HideInInspector]
    public IAdventureItem IAdventureItemDelegate;

    void Start()
    {
        EventTriggerListener.Get(gameObject).onClick = OnClickItem;
    }

    public void ReloadData(AdventureInfo adventure)
    {
        curData = adventure;
        if (adventure.adventureTeam == null)
        {
            CurState = State.WEIKAI;   //wei开始
            text_Msg1.text = string.Format(StaticDataMgr.Instance.GetTextByID("adventure_time"), BattleConst.adventureTime[adventure.adventureData.time - 1]);

        }
        else if (adventure.timeEvent == null)
        {
            CurState = State.JIESHU;   //倒计时结束
            text_Msg2.text = StaticDataMgr.Instance.GetTextByID("adventure_alreadycom");
            text_Msg3.text = StaticDataMgr.Instance.GetTextByID("adventure_click");
            text_Msg2.color = msg2FinishColor;
            text_Msg3.color = msg3FinishColor;
        }
        else
        {
            CurState = State.JINXING;  //进行中
            text_Msg2.text = StaticDataMgr.Instance.GetTextByID("adventure_ing");
            text_Msg2.color = msg2StartColor;
            text_Msg3.color = msg3StartColor;
            curData.timeEvent.AddUpdateEvent(OnUpdateTime);
        }

        ObjEffect.SetActive(CurState == State.JIESHU);
        if (!string.Equals(ImageName,curData.adventureData.image))
        {
            //TODO：使用ResourceMgr创建，在关闭界面后没有删除；检查是否需要删除
            if (ObjImage!=null)
            {
                ResourceMgr.Instance.DestroyAsset(ObjImage);
            }
            ObjImage = ResourceMgr.Instance.LoadAsset(curData.adventureData.image);
            UIUtil.SetParentReset(ObjImage.transform, ImagePos);
        }
    }

    void OnUpdateTime(int time)
    {
        text_Msg3.text = UIUtil.Convert_hh_mm_ss(time);
    }
    void RemoveUpdateTime(AdventureInfo adventure)
    {
        if (adventure.timeEvent!=null)
        {
            adventure.timeEvent.RemoveUpdateEvent(OnUpdateTime);
        }
    }
    void OnClickItem(GameObject go)
    {
        switch (CurState)
        {
            case State.WEIKAI:
                if (AdventureDataMgr.Instance.CheckIsEnoughTeam())//有足够的队伍
                {
                    if (IAdventureItemDelegate!=null)
                    {
                        IAdventureItemDelegate.openAdventureLayout(curData);
                    }
                }
                else if(AdventureDataMgr.Instance.teamCount < BattleConst.maxAdventureTeam)
                {
                    MsgBox.PromptMsg prompt = MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform_Cancel,
                                                                  StaticDataMgr.Instance.GetTextByID("adventure_jiadui"),
                                                                  PrompButtonAddTeamCallBack);
                }
                else
                {
                    UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("adventure_record_005"), (int)PB.ImType.PROMPT);
                }
                break;
            case State.JINXING:
            case State.JIESHU:
                if (IAdventureItemDelegate != null)
                {
                    IAdventureItemDelegate.openAdventureTeams();
                }
                break;
        }
    }

    void PrompButtonAddTeamCallBack(MsgBox.PrompButtonClick click)
    {
        if (click == MsgBox.PrompButtonClick.Cancle)
            return;
        IAdventureItemDelegate.openAdventureTeams();
    }
    public void CleanItem()
    {
        curData = null;
    }
}
