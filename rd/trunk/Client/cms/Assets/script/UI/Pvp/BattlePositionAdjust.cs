using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BattlePositionAdjust : MonoBehaviour
{
    public enum AdjustType:int
    {
        PvpDefensse = 0,
        PvpBattle
    }
    public delegate void AdjustPositionDelegate();

    public ScrollView myPetScrollView;

    public List<MonsterIconBg> playerTeamBg = new List<MonsterIconBg>();
    private List<MonsterIcon> playerIcons = new List<MonsterIcon>();

    private List<string> teamList;
    private int prepareIndex = -1;//准备上阵的空位索引
    private List<GameUnit> mPetList = new List<GameUnit>();
    private Dictionary<string, MonsterIcon> playerAllIconDic = new Dictionary<string, MonsterIcon>();

    private AdjustType adjustType = AdjustType.PvpDefensse;
    private AdjustPositionDelegate positionChangedCallback = null;

    private float   monsterIconScale = -1;

    public void InitWithDefaultPosition(ref List<string> defalutTeam, AdjustType adjustType, AdjustPositionDelegate changedCallback)
    {
        teamList = defalutTeam;
        this.adjustType = adjustType;
        positionChangedCallback = changedCallback;

        InitPlayerTeamIcons();
        InitAllPlayerPetsIcons();
    }

    void InitPlayerTeamIcons()
    {
        prepareIndex = -1;
        CleanAllPlayersIcons();

        MonsterIconBg subBg = null;
        RectTransform rectTrans = null;
        for (int i = 0; i < 5; ++i)
        {
            subBg = playerTeamBg[i];
            subBg.SetEffectShow(false);

            string guid = teamList[i];
            GameUnit unit = null;

            if (!string.IsNullOrEmpty(guid))
            {
                unit = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(int.Parse(guid));
            }
            if (unit == null)
            {
                if (prepareIndex == -1)
                {
                    prepareIndex = i;
                    subBg.SetEffectShow(true);
                }
            }
            else
            {
                MonsterIcon subIcon = MonsterIcon.CreateIcon();
                playerIcons.Add(subIcon);//添加列表用于维护

                

                subIcon.transform.SetParent(subBg.transform, false);

                rectTrans = subIcon.transform as RectTransform;
                rectTrans.anchoredPosition = new Vector2(0, 0);

                if (monsterIconScale < 0)
                {
                    RectTransform parentWith = subBg.transform as RectTransform;
                    monsterIconScale = parentWith.rect.width / rectTrans.rect.width;
                }
                rectTrans.localScale = new Vector3(monsterIconScale, monsterIconScale, monsterIconScale);

                subIcon.SetId(guid);
                subIcon.SetMonsterStaticId(unit.pbUnit.id);
                subIcon.SetLevel(unit.pbUnit.level);
                subIcon.SetStage(unit.pbUnit.stage);

                EventTriggerListener.Get(subIcon.iconButton.gameObject).onClick = OnPlayerTeamIconClick;
            }
        }
    }
    void CleanAllPlayersIcons()
    {
        for (int i = 0; i < playerIcons.Count; i++)
        {
            ResourceMgr.Instance.DestroyAsset(playerIcons[i].gameObject);
        }
        playerIcons.Clear();
    }

    void InitAllPlayerPetsIcons()
    {
        //清理所有宠物Icon
        CleanAllPlayerPetsIcons();

        GameDataMgr.Instance.PlayerDataAttr.GetAllPet(ref mPetList);
        mPetList.Sort();
        GameUnit subUnit = null;
        for (int i = 0; i < mPetList.Count; ++i)
        {
            subUnit = mPetList[i];
            string monsterId = subUnit.pbUnit.id;

            MonsterIcon icon = MonsterIcon.CreateIcon();

            ScrollViewEventListener.Get(icon.iconButton.gameObject).onClick = OnPlayerWarehouseIconClick;
            ScrollViewEventListener.Get(icon.iconButton.gameObject).onPressEnter = OnPlayerWarehouseIconPressEnter;
            myPetScrollView.AddElement(icon.gameObject);
            icon.SetMonsterStaticId(monsterId);
            icon.SetId(subUnit.pbUnit.guid.ToString());
            icon.SetLevel(subUnit.pbUnit.level);
            icon.SetStage(subUnit.pbUnit.stage);
            icon.ShowMaoxianImage(subUnit.pbUnit.IsInAdventure());
            icon.ShowMaskImage(subUnit.pbUnit.IsInAdventure());
            icon.ShowLockImage(subUnit.pbUnit.IsInAdventure());

            playerAllIconDic.Add(icon.Id, icon);

            if (teamList.Contains(subUnit.pbUnit.guid.ToString()))
            {
                icon.ShowSelectImage(true);
                icon.ShowMaskImage();
            }
        }
    }
    void CleanAllPlayerPetsIcons()
    {
        foreach (var item in playerAllIconDic)
        {
            ResourceMgr.Instance.DestroyAsset(item.Value.gameObject);
        }
        playerAllIconDic.Clear();
    }

    void OnPlayerTeamIconClick(GameObject go)
    {
        MonsterIcon subIcon = go.GetComponentInParent<MonsterIcon>();
        string guid = subIcon.Id;
        BattleTeamToPlayerWarehouse(guid);
    }
    void OnPlayerWarehouseIconClick(GameObject go)
    {
        MonsterIcon micon = go.GetComponentInParent<MonsterIcon>();
        GameUnit gameUnit = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(int.Parse(micon.Id));
        if (null != gameUnit)
        {
            if (gameUnit.pbUnit.IsInAdventure())
            {
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("arrayselect_count_005"), (int)PB.ImType.PROMPT);
                return;
            }
        }

        bool isSel = micon.IsSelected();

        if (isSel)
        {
            BattleTeamToPlayerWarehouse(micon.Id);
        }
        else
        {
            if (prepareIndex == -1)
            {
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("arrayselect_count_003"), (int)PB.ImType.PROMPT);
                return;
            }
            PlayerWarehouseToBattleTeam(micon.Id);
        }
        micon.ShowSelectImage(!isSel);
        micon.ShowMaskImage(!isSel);
    }

    void OnPlayerWarehouseIconPressEnter(GameObject go)
    {
        MonsterIcon micon = go.GetComponentInParent<MonsterIcon>();

        int guid = int.Parse(micon.Id);
        GameUnit unit = null;

        GameDataMgr.Instance.PlayerDataAttr.allUnitDic.TryGetValue(guid, out unit);
        UIMonsterInfo.Open(guid, micon.monsterId, unit.pbUnit.level, unit.pbUnit.stage);
    }
    void PlayerWarehouseToBattleTeam(string guid)
    {
        var iconBg = playerTeamBg[prepareIndex];
        iconBg.SetEffectShow(false);

        MonsterIcon subIcon = iconBg.GetComponentInChildren<MonsterIcon>();
        if (null == subIcon)
        {
            subIcon = MonsterIcon.CreateIcon();
            playerIcons.Add(subIcon);
            subIcon.transform.SetParent(iconBg.transform, false);

            RectTransform rectTrans = subIcon.transform as RectTransform;
            rectTrans.anchoredPosition = new Vector2(0, 0);

            if (monsterIconScale < 0)
            {
                RectTransform parentWith = iconBg.transform as RectTransform;
                monsterIconScale = parentWith.rect.width / rectTrans.rect.width;
            }
            rectTrans.localScale = new Vector3(monsterIconScale, monsterIconScale, monsterIconScale);
            EventTriggerListener.Get(subIcon.iconButton.gameObject).onClick = OnPlayerTeamIconClick;
        }
        subIcon.gameObject.SetActive(true);
        subIcon.SetId(guid);
        teamList[prepareIndex] = guid;
        if(positionChangedCallback != null)
        {
            positionChangedCallback();
        }
        GameUnit unit = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(int.Parse(guid));
        subIcon.SetMonsterStaticId(unit.pbUnit.id);
        subIcon.SetLevel(unit.pbUnit.level);
        subIcon.SetStage(unit.pbUnit.stage);
        //新的prepareindex
        UpdatePrepareIndex();
    }

    void BattleTeamToPlayerWarehouse(string guid)
    {
        MonsterIcon playerIcon = playerAllIconDic[guid];
        playerIcon.ShowSelectImage(false);
        playerIcon.ShowMaskImage(false);

        MonsterIconBg subBg = null;
        for (int i = 0; i < 5; ++i)
        {
            subBg = playerTeamBg[i];
            MonsterIcon subIcon = subBg.GetComponentInChildren<MonsterIcon>();
            if (null != subIcon && subIcon.gameObject.activeSelf)
            {
                if (guid == subIcon.Id)
                {
                    playerIcons.Remove(subIcon);
                    ResourceMgr.Instance.DestroyAsset(subIcon.gameObject);
                    GameUnit unit = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(int.Parse(guid));
                    teamList[i] = "";
                    if(positionChangedCallback != null)
                    {
                        positionChangedCallback();
                    }
                    break;
                }
            }
        }
        StartCoroutine(_UpdatePrepareIndex());
    }
    IEnumerator _UpdatePrepareIndex()
    {
        yield return new WaitForEndOfFrame();
        MonsterIconBg subBg = null;
        if (prepareIndex != -1)
        {
            subBg = playerTeamBg[prepareIndex];
            subBg.SetEffectShow(false);
        }
        UpdatePrepareIndex();

        subBg = playerTeamBg[prepareIndex];
        subBg.SetEffectShow(true);
    }

    //---------------------------------------------------------------------------------------------------------------------
    void UpdatePrepareIndex()
    {
        prepareIndex = -1;
        MonsterIconBg subIconBg = null;
        for (int i = 0; i < 5; ++i)
        {
            subIconBg = playerTeamBg[i];
            MonsterIcon tempIcon = subIconBg.GetComponentInChildren<MonsterIcon>();
            if (null == tempIcon || !tempIcon.gameObject.activeSelf)
            {
                prepareIndex = i;
                subIconBg.SetEffectShow(true);
                break;
            }
        }
    }
}
