using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PvpFightParam
{
    public List<int> playerTeam;
    public PB.HSPVPMatchTargetRet targetData;
}

[RequireComponent(typeof(BattlePositionAdjust))]
public class PvpAdjustBattleTeam : UIBase
{
    public static string ViewName = "PvpAdjustBattleTeam";

    public Text enemyNameText;
    public Text enemyLableText;
    public Text enemyBpLabelText;
    public Text enemBpValueText;
    public Text myPetLabelText;
    public Text myBpLabelText;
    public Text myBpValueText;

    public RectTransform[] enemyMonsterIconParentArray;

    public Text dropTitleLableText;
    public Text victoryDropLabelText;
    public Text loseDropLabelText;
    public RectTransform victoryDropParent;
    public RectTransform loseDropParent;

    public Text refreshCostCoinText;
    public Button refreshButton;
    public Button fightButton;
    public Button closeButton;

    private PB.HSPVPMatchTargetRet opponentData = null;
    private List<MonsterIcon> listOpponentMonsterIcon = new List<MonsterIcon>();
    private List<ItemIcon> listDropItemIcon = new List<ItemIcon>();


    private List<string> pvpFightTeam = new List<string>();
    private BattlePositionAdjust battlePositionAdjust = null;
    private BattlePositionAdjust BattlePositionAdjustAttr
    {
        get
        {
            if(null == battlePositionAdjust)
            {
                battlePositionAdjust = GetComponent<BattlePositionAdjust>();
            }
            return battlePositionAdjust;
        }
    }

    public static void OpenWith(PB.HSPVPMatchTargetRet opponentData)
    {
        PvpAdjustBattleTeam adjustUi = UIMgr.Instance.OpenUI_(ViewName) as PvpAdjustBattleTeam;
        adjustUi.InitWith(opponentData);
    }
    public void InitWith(PB.HSPVPMatchTargetRet opponentData)
    {
        GameDataMgr.Instance.curInstanceType = (int)InstanceType.Normal;
        this.opponentData = opponentData;
        List<string> pvpTeamList = BattleTeamManager.GetTeamWithKey(BattleTeamManager.TeamList.Pvp);
        pvpFightTeam.Clear();
        pvpFightTeam.AddRange(pvpTeamList);
        OnFightTeamChanged();
        BattlePositionAdjustAttr.InitWithDefaultPosition(ref pvpFightTeam, BattlePositionAdjust.AdjustType.PvpBattle, OnFightTeamChanged);
        InitOppenentInformation();
        InitDropInformation();
    }

	void Start ()
    {
        enemyLableText.text = StaticDataMgr.Instance.GetTextByID("instance_difangzhenrong");
        enemyBpLabelText.text = StaticDataMgr.Instance.GetTextByID("arrayselect_bp_001");
        myPetLabelText.text = StaticDataMgr.Instance.GetTextByID("instance_wofangzhenrong");
        myBpLabelText.text = StaticDataMgr.Instance.GetTextByID("arrayselect_bp_001");

        dropTitleLableText.text = StaticDataMgr.Instance.GetTextByID("pvp_rewardyulan");
        victoryDropLabelText.text = StaticDataMgr.Instance.GetTextByID("pvp_winreward");
        loseDropLabelText.text = StaticDataMgr.Instance.GetTextByID("pvp_losereward");

        UIUtil.SetButtonTitle(refreshButton.transform, StaticDataMgr.Instance.GetTextByID("shop_refresh"));
        UIUtil.SetButtonTitle(fightButton.transform, StaticDataMgr.Instance.GetTextByID("pvp_battle"));

        fightButton.onClick.AddListener(OnFightButtonClick);
        closeButton.onClick.AddListener(OnCloseButtonClick);
        refreshButton.onClick.AddListener(OnRefreshButtonClick);
	}

    void InitOppenentInformation()
    {
        enemyNameText.text = opponentData.name;
        if(listOpponentMonsterIcon.Count ==0)
        {
            for(int i =0;i<enemyMonsterIconParentArray.Length;++i)
            {
                MonsterIcon subIcon = MonsterIcon.CreateIcon();
                listOpponentMonsterIcon.Add(subIcon);

                subIcon.transform.SetParent(enemyMonsterIconParentArray[i]);
                RectTransform iconRt = subIcon.transform as RectTransform;
                float scale = enemyMonsterIconParentArray[i].rect.width / iconRt.rect.width;
                subIcon.transform.localScale = new Vector3(scale, scale, scale);
                subIcon.transform.localPosition = Vector3.zero;

                subIcon.gameObject.SetActive(false);
            }
        }

        List<PB.HSMonster> monsterList = opponentData.defenceData.monsterInfo;
        MonsterIcon mIcon = null;
        for(int i =0;i<listOpponentMonsterIcon.Count;++i)
        {
            mIcon = listOpponentMonsterIcon[i];
            if(i >= monsterList.Count)
            {
                mIcon.gameObject.SetActive(false);
                continue;
            }
            mIcon.gameObject.SetActive(true);
            var subMonster = monsterList[i];
            mIcon.SetMonsterStaticId(subMonster.cfgId);
            mIcon.SetLevel(subMonster.level);
            mIcon.SetStage(subMonster.stage);
        }

        refreshCostCoinText.text = GameDataMgr.Instance.PvpDataMgrAttr.GetRereshCost().ToString();
    }
    void InitDropInformation()
    {
        if (listDropItemIcon.Count < 1)
        {
            float parentWith = victoryDropParent.rect.width;
            for(int i =0;i< 2;++i)
            {
                ItemData subItemData = ItemData.valueof("90003", 1);
                ItemIcon subIcon = ItemIcon.CreateItemIcon(subItemData, true, false);
                listDropItemIcon.Add(subIcon);
                if(0 ==i)
                {
                    subIcon.transform.SetParent(victoryDropParent);
                }
                else
                {
                    subIcon.transform.SetParent(loseDropParent);
                }
                RectTransform iconRt = subIcon.transform as RectTransform;
                float scale = parentWith / iconRt.rect.width;
                subIcon.transform.localScale = new Vector3(scale, scale, scale);
                subIcon.transform.localPosition = Vector3.zero;
            }
        }
        ///
        PvpStaticData pvpStaticData = StaticDataMgr.Instance.GetPvpStaticDataWithStage(GameDataMgr.Instance.PvpDataMgrAttr.selfPvpStage);
        if(null != pvpStaticData)
        {
            listDropItemIcon[0].RefreshWithItemInfo(ItemData.valueof("90003", pvpStaticData.win), true,false);
            listDropItemIcon[1].RefreshWithItemInfo(ItemData.valueof("90003", pvpStaticData.lose), true, false);
        }

    }

    void OnFightTeamChanged()
    {
        myBpValueText.text = GameDataMgr.Instance.PvpDataMgrAttr.GetBpWithGuidList(pvpFightTeam).ToString();
    }
    void OnFightButtonClick()
    {
        List<int> battleTeam = SaveBattleTeam();
        if (null == battleTeam || battleTeam.Count < 1)
        {
            // MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform, StaticDataMgr.Instance.GetTextByID("tip_zhenrongError"));
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("tip_zhenrongError"), (int)PB.ImType.PROMPT);
        }
        else if (battleTeam.Count < 5)
        {
            MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform_Cancel,
                StaticDataMgr.Instance.GetTextByID("pvp_chuzhanbuman5"),
                ConformFightOrNot);
        }
        else
        {
            // GameDataMgr.Instance.OnBattleStart();
            PvpFightParam param = new PvpFightParam();
            param.playerTeam = battleTeam;
            param.targetData = opponentData;
            GameMain.Instance.LoadBattleLevelPvp(param);
        }
    }
    void ConformFightOrNot(MsgBox.PrompButtonClick click)
    {
        if(click == MsgBox.PrompButtonClick.OK)
        {
            Fight();
        }
    }
    void Fight()
    {

    }

    List<int> SaveBattleTeam()
    {
        List<string> saveTeam = new List<string>();
        List<int> battleTeam = new List<int>();
        string guid = null;

        bool lastIndexIsNull = false;

        for (int i = 0; i <pvpFightTeam.Count; ++i)
        {
            guid = pvpFightTeam[i];
            saveTeam.Add(guid);
            if (string.IsNullOrEmpty(guid))
            {
                lastIndexIsNull = true;
            }
            else
            {
                if (lastIndexIsNull)
                {
                    return null;//不允许前面有空位
                }
                battleTeam.Add(int.Parse(guid));
            }
        }
        for (int i = saveTeam.Count; i < 6; ++i)
        {
            saveTeam.Add("");
        }

        BattleTeamManager.SetTeam(saveTeam, BattleTeamManager.TeamList.Pvp);

        return battleTeam;
    }

    void OnRefreshButtonClick()
    {
        MsgBox.PrompCostMsg.Open(GameDataMgr.Instance.PvpDataMgrAttr.GetRereshCost(), 
            StaticDataMgr.Instance.GetTextByID("pvp_changecompetitor"),
            "",
            CostType.JinBiCoin,
            OnConformRefresh);
    }

    void OnConformRefresh(MsgBox.PrompButtonClick click)
    {
        if(click == MsgBox.PrompButtonClick.OK)
        {

        }
    }

   void OnRequestRefreshOpponentFinished(ProtocolMessage message)
    {

    }

    void OnCloseButtonClick()
    {
        MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform_Cancel, StaticDataMgr.Instance.GetTextByID("pvp_exit"),
            StaticDataMgr.Instance.GetTextByID("pvp_exitsure"), OnConformExit);
    }
    void OnConformExit(MsgBox.PrompButtonClick click)
    {
        if(click == MsgBox.PrompButtonClick.OK)
        {
            UIMgr.Instance.CloseUI_(this);
        }
    }
	
}
