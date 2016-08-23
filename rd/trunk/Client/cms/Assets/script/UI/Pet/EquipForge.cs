using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class EquipForge : EquipInfoBase
{
    public Button btnForge;
    public Text textForge;
    public Text textBaseAttr;
    public Text textNotForge;
    public GameObject objForge;

    public Transform BasePos;

    private ShowAttributesItem[] AttrList = new ShowAttributesItem[5];
    private EquipData curData;
    private EquipData nextData;
    
    public enum ForgeType
    {
        Qianghua,
        Jinjie,
        MaxLevel
    }
    public ForgeType uiType;

    public Transform targetPos;
    private ItemIcon targetIcon;
    public List<EquipMaterialItem> materialItems = new List<EquipMaterialItem>();
    public List<GameObject> objLine = new List<GameObject>();

    public Image imgCoin;
    public Text textCoin;

    void Start()
    {
        btnForge.onClick.AddListener(OnClickForgeBtn);
        textBaseAttr.text = StaticDataMgr.Instance.GetTextByID("pet_detail_stage_attr");
    }

    public override void ReloadData(EquipData data)
    {
        curData = data;
        nextData = null;

        #region 设置当前装备的状态
        if (curData.level >= BattleConst.maxEquipLevel && curData.stage >= BattleConst.maxEquipStage)
        {
            uiType = ForgeType.MaxLevel;
            nextData = EquipData.valueof(curData.id, curData.equipId, curData.stage, curData.level, BattleConst.invalidMonsterID, null);
        }
        else if (curData.level < BattleConst.maxEquipLevel)
        {
            uiType = ForgeType.Qianghua;
            nextData = EquipData.valueof(curData.id, curData.equipId, curData.stage, curData.level + 1, BattleConst.invalidMonsterID, null);
        }
        else if (curData.stage < BattleConst.maxEquipStage)
        {
            uiType = ForgeType.Jinjie;
            nextData = EquipData.valueof(curData.id, curData.equipId, curData.stage + 1, BattleConst.minEquipLevel, BattleConst.invalidMonsterID, null);
        }

        EquipForgeData nextForge = StaticDataMgr.Instance.GetEquipForgeData(nextData.stage, nextData.level);
        if (nextForge.playerlevelDemand > GameDataMgr.Instance.PlayerDataAttr.LevelAttr)
        {
            uiType = ForgeType.MaxLevel;
        }
        #endregion

        #region 设置显示属性变化
        EquipProtoData curProto = StaticDataMgr.Instance.GetEquipProtoData(curData.equipId, curData.stage);
        EquipProtoData nextProto = StaticDataMgr.Instance.GetEquipProtoData(nextData.equipId, nextData.stage);
        
        Dictionary<AttrType, int> curAttr = curProto.leveAttribute(curData.level);
        Dictionary<AttrType, int> nextAttr = nextProto.leveAttribute(nextData.level);

        for (int i = 0; i < AttrList.Length; i++)
        {
            if (AttrList[i] != null)
            {
                AttrList[i].gameObject.SetActive(false);
            }
        }
        Action<int, string, float, float> action = (index, name, value, change) =>
        {
            if (AttrList[index] == null)
            {
                GameObject go = ResourceMgr.Instance.LoadAsset("ShowAttributesItem");
                UIUtil.SetParentReset(go.transform, BasePos);
                AttrList[index] = go.GetComponent<ShowAttributesItem>();
            }
            else
            {
                AttrList[index].gameObject.SetActive(true);
            }
            AttrList[index].SetValue(name, (int)value, (int)change);
        };
        int count = 0;
        if (nextAttr.ContainsKey(AttrType.Health) && nextAttr[AttrType.Health] > 0.0f)
        {
            action(count, StaticDataMgr.Instance.GetTextByID("common_attr_health"), curAttr[AttrType.Health], nextAttr[AttrType.Health]-curAttr[AttrType.Health]);
            count++;
        }
        if (nextAttr.ContainsKey(AttrType.Strength) && nextAttr[AttrType.Strength] > 0.0f)
        {
            action(count, StaticDataMgr.Instance.GetTextByID("common_attr_strenth"), curAttr[AttrType.Strength], nextAttr[AttrType.Strength] - curAttr[AttrType.Strength]);
            count++;
        }
        if (nextAttr.ContainsKey(AttrType.Intelligence) && nextAttr[AttrType.Intelligence] > 0.0f)
        {
            action(count, StaticDataMgr.Instance.GetTextByID("common_attr_intelligence"), curAttr[AttrType.Intelligence], nextAttr[AttrType.Intelligence] - curAttr[AttrType.Intelligence]);
            count++;
        }
        if (nextAttr.ContainsKey(AttrType.Defense) && nextAttr[AttrType.Defense] > 0.0f)
        {
            action(count, StaticDataMgr.Instance.GetTextByID("common_attr_defence"), curAttr[AttrType.Defense], nextAttr[AttrType.Defense] - curAttr[AttrType.Defense]);
            count++;
        }
        if (nextAttr.ContainsKey(AttrType.Speed) && nextAttr[AttrType.Speed] > 0.0f)
        {
            action(count, StaticDataMgr.Instance.GetTextByID("common_attr_speed"), curAttr[AttrType.Speed], nextAttr[AttrType.Speed] - curAttr[AttrType.Speed]);
            count++;
        }
        #endregion

        if(uiType==ForgeType.MaxLevel)
        { 
            textNotForge.gameObject.SetActive(true);
            objForge.gameObject.SetActive(false);
            return;
        }
        else
        {
            textNotForge.gameObject.SetActive(false);
            objForge.gameObject.SetActive(true);
        }
        

        if (targetIcon==null)
        {
            targetIcon = ItemIcon.CreateItemIcon(nextData);
            UIUtil.SetParentReset(targetIcon.transform, targetPos);
        }
        else
        {
            targetIcon.RefreshWithEquipInfo(nextData);
        }

        #region 设置需求的材料

        List<ItemInfo> curDemand = new List<ItemInfo>();
        nextForge.GetLevelDemand(ref curDemand);
        materialItems.ForEach(delegate (EquipMaterialItem item) { item.gameObject.SetActive(false); });
        int materialIndex = 0;
        for (int i = 0; i < curDemand.Count; i++)
        {
            if (curDemand[i].type == (int)PB.itemType.ITEM && materialIndex < materialItems.Count)
            {
                materialItems[materialIndex].gameObject.SetActive(true);
                materialItems[materialIndex].Refresh(curDemand[i].itemId, curDemand[i].count);
                materialIndex++;
            }
            else if (curDemand[i].type == (int)PB.itemType.PLAYER_ATTR)
            {
                if (curDemand[i].itemId.Equals(((int)PB.changeType.CHANGE_COIN).ToString()))
                {
                    imgCoin.sprite = ResourceMgr.Instance.LoadAssetType<Sprite>(BattleConst.icon_jinbi);
                    if (GameDataMgr.Instance.PlayerDataAttr.coin < curDemand[i].count)
                    {
                        textCoin.color = ColorConst.text_color_nReq;
                    }
                    else
                    {
                        textCoin.color = ColorConst.text_color_Req;
                    }
                    textCoin.text = curDemand[i].count.ToString();
                }
                else
                {
                    Logger.LogError("xiao hao jin bi pei zhi cuo wu !!!!!!!!!!!!!!!!!!");
                }
            }
            else
            {
                Logger.LogError("xiao hao wu pin pei zhi cuo wu!!!!!!!!!!");
            }
        }
        for (int i = 0; i < objLine.Count; i++)
        {
            objLine[i].SetActive(i == materialIndex - 1);
        }
        #endregion

        if (UIUtil.CheckIsEnoughMaterial(curDemand) && UIUtil.CheckIsEnoughPlayerLevel(nextForge.playerlevelDemand))
        {
            btnForge.interactable = true;
        }
        else
        {
            btnForge.interactable = false;
        }

    }

    void OnClickForgeBtn()
    {
        if (uiType == ForgeType.Qianghua)
        {
            PB.HSEquipIncreaseLevel param = new PB.HSEquipIncreaseLevel();
            param.id = curData.id;
            GameApp.Instance.netManager.SendMessage(PB.code.EQUIP_INCREASE_LEVEL_C.GetHashCode(), param);
        }
        else if(uiType == ForgeType.Jinjie)
        {
            PB.HSEquipIncreaseStage param = new PB.HSEquipIncreaseStage();
            param.id = curData.id;
            GameApp.Instance.netManager.SendMessage(PB.code.EQUIP_INCREASE_STAGE_C.GetHashCode(), param);
        }
    }

    //void ReloadEquipNotify(EquipData equipData)
    //{
    //    if (curData==equipData)
    //    {
    //        ReloadData(curData);
    //    }
    //}
    void OnEnable()
    {
        BindListener();
    }
    void OnDisable()
    {
        UnBindListener();
    }

    void BindListener()
    {
        //GameEventMgr.Instance.AddListener<EquipData>(GameEventList.ReloadEquipForgeNotify, ReloadEquipNotify);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_INCREASE_LEVEL_C.GetHashCode().ToString(), OnEquipForgeLevelRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_INCREASE_LEVEL_S.GetHashCode().ToString(), OnEquipForgeLevelRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_INCREASE_STAGE_C.GetHashCode().ToString(), OnEquipForgeStageRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_INCREASE_STAGE_S.GetHashCode().ToString(), OnEquipForgeStageRet);
    }

    void UnBindListener()
    {
        //GameEventMgr.Instance.AddListener<EquipData>(GameEventList.ReloadEquipForgeNotify, ReloadEquipNotify);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_INCREASE_LEVEL_C.GetHashCode().ToString(), OnEquipForgeLevelRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_INCREASE_LEVEL_S.GetHashCode().ToString(), OnEquipForgeLevelRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_INCREASE_STAGE_C.GetHashCode().ToString(), OnEquipForgeStageRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_INCREASE_STAGE_S.GetHashCode().ToString(), OnEquipForgeStageRet);
    }

    void OnEquipForgeLevelRet(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg == null || msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        PB.HSEquipIncreaseLevelRet result = msg.GetProtocolBody<PB.HSEquipIncreaseLevelRet>();

        OnEquipForgeReslut(result.id, result.stage, result.level);
    }

    void OnEquipForgeStageRet(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg == null || msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        PB.HSEquipIncreaseStageRet result = msg.GetProtocolBody<PB.HSEquipIncreaseStageRet>();

        OnEquipForgeReslut(result.id, result.stage, result.level);
    }
    //此处只给强化升级"返回成功""并不是升级成功"后使用
    void OnEquipForgeReslut(long id, int stage, int level)
    {
        if (curData.id == id)
        {
            if ((curData.stage == stage) && (curData.level == level))
            {//失败
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("im_forge_failed"), (int)PB.ImType.PROMPT);
            }
            else
            {//成功
                // update local data
                {
                    curData.id = id;
                    curData.SetStageLvl(stage);
                    curData.SetStrengthLvl(level);
                }
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("im_forge_success"), (int)PB.ImType.PROMPT);
            }
        }
        else
        {
            Logger.LogError("强化物品出现id不匹配异常！");
        }
        GameEventMgr.Instance.FireEvent<EquipData>(GameEventList.ReloadEquipForgeNotify, curData);
    }
}
