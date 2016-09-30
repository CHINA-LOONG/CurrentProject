using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public interface IAdventureTeamItem
{
    void OnClickToAdventure();
    void OnClickToPaySubmit(AdventureTeam team);
    void OnClickToSubmit(AdventureTeam team);
}
public class AdventureTeamItem : MonoBehaviour
{
    public Text textState;
    public Text textName;

    public GameObject objTime;
    public Text textTime;

    public Transform memberParent;

    public Button btn_Todoit;
    public Button btn_Submit;
    public Text text_Todoit;
    public Text text_Submit;
    
    public enum State
    {
        WEIKAI,
        JINXING,
        WANCHENG
    }
    private State state;
    public State CurState
    {
        get { return state; }
        set
        {
            state = value;
            btn_Todoit.gameObject.SetActive(false);
            btn_Submit.gameObject.SetActive(false);
            switch (state)
            {
                case State.WEIKAI:
                    objTime.SetActive(false);
                    btn_Todoit.gameObject.SetActive(true);
                    text_Todoit.text = StaticDataMgr.Instance.GetTextByID("adventure_goadventure");
                    break;
                case State.JINXING:
                    objTime.SetActive(true);
                    btn_Submit.gameObject.SetActive(true);
                    text_Submit.text = StaticDataMgr.Instance.GetTextByID("adventure_complete");
                    break;
                case State.WANCHENG:
                    objTime.SetActive(false);
                    btn_Submit.gameObject.SetActive(true);
                    text_Submit.text = StaticDataMgr.Instance.GetTextByID("quest_lingqujiangli");
                    break;
            }
        }
    }
    public IAdventureTeamItem IAdventureTeamItemDelegate;
    private MonsterIcon[] monsters = new MonsterIcon[5];

    private AdventureTeam curdata;

    public AdventureTeam curData
    {
        get { return curdata; }
        set
        {
            if (curdata!=null)
            {
                if (curdata.adventure!=null&&curdata.adventure.timeEvent != null)//正在进行
                {
                    curdata.adventure.timeEvent.RemoveUpdateEvent(OnUpdateTime);
                }
            }
            curdata = value;
        }
    }

    void Start()
    {
        btn_Todoit.onClick.AddListener(OnClickToAdventureBtn);
        btn_Submit.onClick.AddListener(OnClickToSubmitBtn);
    }
    public void RefreshData(AdventureTeam team)
    {
        curData = team;
        
        textName.text = string.Format(StaticDataMgr.Instance.GetTextByID("adventure_teamname"),curData.teamId);

        if (curData.adventure != null)//大冒险中
        {
            textState.text = curData.adventure.adventureData.TypeText;
            if (curData.adventure.timeEvent != null)//正在进行
            {
                CurState = State.JINXING;
                curData.adventure.timeEvent.AddUpdateEvent(OnUpdateTime);
            }
            else//已完成
            {
                CurState = State.WANCHENG;
            }

        }
        else    //空闲中
        {
            CurState = State.WEIKAI;
            textState.text = StaticDataMgr.Instance.GetTextByID("adventure_free");
        }
        ReloadTeamMonster(curData.adventure);
    }

    void ReloadTeamMonster(AdventureInfo adventure)
    {
        if (adventure == null)
        {
            for (int i = 0; i < monsters.Length; i++)
            {
                if (monsters[i] != null)
                {
                    monsters[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < monsters.Length; i++)
            {
                if (monsters[i] == null)
                {
                    monsters[i] = MonsterIcon.CreateIcon();
                    ScrollViewEventListener.Get(monsters[i].iconButton.gameObject).onPressEnter = OnPressEnterMonsterIcon;
                    UIUtil.SetParentReset(monsters[i].transform, memberParent);
                }
                else
                {
                    monsters[i].gameObject.SetActive(true);
                    monsters[i].Init();
                }
                if (i < adventure.adventureTeam.selfIdList.Count)
                {
                    GameUnit gameUnit = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(adventure.adventureTeam.selfIdList[i]);
                    //UnitData unitData = StaticDataMgr.Instance.GetUnitRowData(gameUnit.pbUnit.id);
                    monsters[i].SetMonsterStaticId(gameUnit.pbUnit.id);
                    monsters[i].SetId(gameUnit.pbUnit.guid.ToString());
                    monsters[i].SetLevel(gameUnit.pbUnit.level);
                    monsters[i].SetStage(gameUnit.pbUnit.stage);
                }
                else if(i==adventure.adventureTeam.selfIdList.Count)
                {
                    PB.AllianceBaseMonster monster = adventure.adventureTeam.guildMonster;
                    monsters[i].SetMonsterStaticId(monster.cfgId);
                    monsters[i].SetId(monster.monsterId.ToString());
                    monsters[i].SetLevel(monster.level);
                    monsters[i].SetStage(monster.stage);
                }
                else
                {
                    Logger.LogError("大冒险宠物队员异常");
                }
            }
        }
    }
    void OnUpdateTime(int time)
    {
        textTime.text = string.Format("{0:D2}:{1:D2}:{2:D2}", time / 3600, (time % 3600) / 60, time % 60);
    }

    void OnClickToAdventureBtn()
    {
        if (IAdventureTeamItemDelegate!=null)
        {
            IAdventureTeamItemDelegate.OnClickToAdventure();
        }
    }
    void OnClickToSubmitBtn()
    {
        if (CurState==State.WANCHENG)
        {
            if (IAdventureTeamItemDelegate!=null)
            {
                IAdventureTeamItemDelegate.OnClickToSubmit(curData);
            }
        }
        else if (CurState==State.JINXING)
        {
            PrompMsgSubmitByPay.Open(StaticDataMgr.Instance.GetTextByID("adventure_completenow"),
                                     StaticDataMgr.Instance.GetTextByID("adventure_timeleft"),
                                     curData.adventure.timeEvent,
                                     PrompMsgSubmitByPayCallBack);
        }
    }
    void PrompMsgSubmitByPayCallBack(MsgBox.PrompButtonClick click)
    {
        if (click == MsgBox.PrompButtonClick.Cancle)
            return;
        if (IAdventureTeamItemDelegate != null)
        {
            IAdventureTeamItemDelegate.OnClickToPaySubmit(curData);
        }
    }

    void OnPressEnterMonsterIcon(GameObject go)
    {
        MonsterIcon micon = go.GetComponentInParent<MonsterIcon>();

        int guid = int.Parse(micon.Id);
        GameUnit unit = null;

        GameDataMgr.Instance.PlayerDataAttr.allUnitDic.TryGetValue(guid, out unit);
        UIMonsterInfo.Open(guid, micon.monsterId, unit.pbUnit.level, unit.pbUnit.stage);
    }
    public void CleanItem()
    {
        curData = null;
    }
}
