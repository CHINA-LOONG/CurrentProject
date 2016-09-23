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
    [HideInInspector]
    public AdventureInfo curData;
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
        }
        else
        {
            CurState = State.JINXING;  //进行中
            text_Msg2.text = StaticDataMgr.Instance.GetTextByID("adventure_ing");
            curData.timeEvent.AddUpdateEvent(OnUpdateTime);
        }
    }

    void OnUpdateTime(int time)
    {
        text_Msg3.text = UIUtil.Convert_hh_mm_ss(time);
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
                else //队伍已满
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

}
