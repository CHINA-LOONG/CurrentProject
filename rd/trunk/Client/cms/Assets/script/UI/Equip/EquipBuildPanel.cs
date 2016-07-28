using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public interface IEquipPanelBaseCallBack:IBuildEquipCallBack, IMosaicEquipCallBack{ }
public abstract class EquipPanelBase : MonoBehaviour
{
    private UIEquipInlay parentNode;
    public UIEquipInlay ParentNode
    {
        get
        {
            if (parentNode == null)
            {
                parentNode = transform.GetComponentInParent<UIEquipInlay>();
            }
            return parentNode;
        }
    }
    public IEquipPanelBaseCallBack ICallBack;
    public abstract void ReloadData(EquipData data,UIEquipInlay.State type,int select=-1);
}

public interface IBuildEquipCallBack
{
    void OnBuildEquipReture();
}

public class EquipBuildPanel : EquipPanelBase
{

    public Transform transCurPos;
    public Transform transNextPos;
    private ItemIcon curItemIcon;
    private ItemIcon nextItemIcon;

    public Text text_Zhanli1;
    public Text text_Zhanli2;
    public Text textZhanli1;
    public Text textZhanli2;

    public Transform contentAttr;
    public Text text_Material;
    public Transform contentMaterial;

    public Image imgCoin;
    public Text textCoin;
    
    public Text text_Tips;
    public Button btnBuild;
    public Text text_Dazao;
    public Text text_FullTips;
    public GameObject maxHide;
    public GameObject maxShow;

    private List<EquipBuildAttr> itemAttrs = new List<EquipBuildAttr>();
    private List<EquipBuildAttr> itemAttrPool = new List<EquipBuildAttr>();

    public enum UIType 
    {
        Qianghua,
        Jinjie,
        MaxLevel
    }
    public UIType uiType;
    public IBuildEquipCallBack ICallBackDeletage { get { return ICallBack; } }

    EquipData curData;                  //基础数据
    EquipData nextData;

    private EquipProtoData curProto;    //用于获取属性
    private EquipProtoData nextProto;

    private EquipForgeData nextForge;   //用于获取材料和等级限制及成功率

    private Dictionary<AttrType, int> curAttr=new Dictionary<AttrType,int>();
    private Dictionary<AttrType, int> nextAttr=new Dictionary<AttrType,int>();
    private List<ItemInfo> curDemand = new List<ItemInfo>();

    public List<EquipMaterialItem> materialItems = new List<EquipMaterialItem>();

    void Start()
    {
        text_Material.text = StaticDataMgr.Instance.GetTextByID("equip_forge_xiaohao");
        text_Zhanli1.text = StaticDataMgr.Instance.GetTextByID("equip_forge_zhanli");
        text_Zhanli2.text = StaticDataMgr.Instance.GetTextByID("equip_forge_zhanli");
        text_FullTips.text = StaticDataMgr.Instance.GetTextByID("equip_fully_forged");

        btnBuild.onClick.AddListener(OnClickBuild);
    }

    public override void ReloadData(EquipData data, UIEquipInlay.State type, int select = -1)
    {
        curData = data;
        nextData = null;

        #region SetNext Equip and (Jinjie or Qianghua)

        if (curData.level >= BattleConst.maxEquipLevel && curData.stage >= BattleConst.maxEquipStage)
        {
            uiType = UIType.MaxLevel;
            nextData = new EquipData()
            {
                equipId = curData.equipId,
                stage = curData.stage,
                level = curData.level
            };
        }
        else if (curData.level < BattleConst.maxEquipLevel)
        {
            uiType = UIType.Qianghua;
            nextData = new EquipData()
            {
                equipId=curData.equipId,
                stage = curData.stage,
                level = curData.level + 1
            };
        }
        else
        {
            uiType = UIType.Jinjie;
            nextData = new EquipData
            {
                equipId = curData.equipId,
                stage = curData.stage + 1,
                level = BattleConst.minEquipLevel
            };
        }


        #endregion

        //TODO: 设置战力
        textZhanli1.text = "0";
        textZhanli2.text = "0";

        #region SetIcon by (curData and nextData)
        if (curItemIcon==null)
        {
            curItemIcon = ItemIcon.CreateItemIcon(curData);
            UIUtil.SetParentReset(curItemIcon.transform, transCurPos);
        }
        else
        {
            curItemIcon.RefreshWithEquipInfo(curData);
        }

        if (nextItemIcon==null)
        {
            nextItemIcon = ItemIcon.CreateItemIcon(nextData);
            UIUtil.SetParentReset(nextItemIcon.transform, transNextPos);
        }
        else
        {
            nextItemIcon.RefreshWithEquipInfo(nextData);
        }
        #endregion

        #region Set Attr (curProto and nextProto)
        
        curProto = StaticDataMgr.Instance.GetEquipProtoData(curData.equipId, curData.stage);
        nextProto = StaticDataMgr.Instance.GetEquipProtoData(nextData.equipId, nextData.stage);
        

        curAttr= curProto.leveAttribute(curData.level);
        nextAttr= nextProto.leveAttribute(nextData.level);
        if (curAttr.Count!=nextAttr.Count)
        {
            Logger.LogError("equip attr errror");
        }

        RemoveAllElement();
        foreach (var attr in curAttr)
        {
            AttrType attrType = attr.Key;
            EquipBuildAttr itemAttr = GetElement();
            itemAttr.Refresh(attrType, attr.Value, nextAttr[attrType]);
        }
        #endregion

        #region Get materials
        nextForge = StaticDataMgr.Instance.GetEquipForgeData(nextData.stage, nextData.level);

        if (uiType == UIType.MaxLevel)
        {
            maxShow.SetActive(true);
            maxHide.SetActive(false);
            return;
        }
        else
        {
            maxShow.SetActive(false);
            maxHide.SetActive(true);
        }

        curDemand.Clear();
        nextForge.GetLevelDemand(ref curDemand);
        materialItems.ForEach(delegate(EquipMaterialItem item) { item.gameObject.SetActive(false); });
        int materialIndex = 0;
        for (int i = 0; i < curDemand.Count; i++)
        {
            if (curDemand[i].type == (int)PB.itemType.ITEM && materialIndex < materialItems.Count)
            {
                materialItems[materialIndex].gameObject.SetActive(true);
                materialItems[materialIndex].Refresh(curDemand[i].itemId, curDemand[i].count);
                materialIndex++;
            }
            else if(curDemand[i].type==(int)PB.itemType.PLAYER_ATTR)
            {
                if (curDemand[i].itemId.Equals(((int)PB.playerAttr.COIN).ToString()))
                {
                    imgCoin.sprite = ResourceMgr.Instance.LoadAssetType<Sprite>(BattleConst.icon_jinbi);
                    if (GameDataMgr.Instance.PlayerDataAttr.coin<curDemand[i].count)
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
        #endregion

        if (uiType == UIType.Jinjie)
        {
            text_Dazao.text = StaticDataMgr.Instance.GetTextByID("equip_forge_XXdazao");
            text_Tips.text = StaticDataMgr.Instance.GetTextByID("equip_forge_stateTips");
        }
        else
        {
            text_Dazao.text = StaticDataMgr.Instance.GetTextByID("equip_forge_dazao");
            text_Tips.text = StaticDataMgr.Instance.GetTextByID("equip_forge_levelTips");
        }

        if (!UIUtil.CheckIsEnoughPlayerLevel(nextForge.playerlevelDemand))
        {
            text_Tips.text = "<color=" + ColorConst.colorTo_Hstr(ColorConst.text_color_nReq) + ">" + StaticDataMgr.Instance.GetTextByID("equip_level_limits") + "</color>";
        }
        if (UIUtil.CheckIsEnoughMaterial(curDemand) && UIUtil.CheckIsEnoughPlayerLevel(nextForge.playerlevelDemand))
        {
            btnBuild.interactable = true;
        }
        else
        {
            btnBuild.interactable = false;
        }
    }

    public EquipBuildAttr GetElement()
    {
        EquipBuildAttr item = null;
        if (itemAttrPool.Count <= 0)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset("EquipBuildAttr");
            if (go != null)
            {
                go.transform.localScale = Vector3.one;
                go.transform.SetParent(contentAttr, false);
                item = go.GetComponent<EquipBuildAttr>();
            }
        }
        else
        {
            item = itemAttrPool[itemAttrPool.Count - 1];
            item.gameObject.SetActive(true);
            itemAttrPool.Remove(item);
        }
        itemAttrs.Add(item);
        item.transform.SetAsLastSibling();
        return item;
    }
    public void RemoveElement(EquipBuildAttr item)
    {
        if (itemAttrs.Contains(item))
        {
            item.gameObject.SetActive(false);
            itemAttrs.Remove(item);
            itemAttrPool.Add(item);
        }
    }
    public void RemoveAllElement()
    {
        itemAttrs.ForEach(delegate(EquipBuildAttr item) { item.gameObject.SetActive(false); });
        itemAttrPool.AddRange(itemAttrs);
        itemAttrs.Clear();
    }


    void OnClickBuild()
    {
        if (uiType==UIType.Qianghua)
        {
            PB.HSEquipIncreaseLevel param = new PB.HSEquipIncreaseLevel();
            param.id = curData.id;
            GameApp.Instance.netManager.SendMessage(PB.code.EQUIP_INCREASE_LEVEL_C.GetHashCode(), param);
        }
        else
        {
            PB.HSEquipIncreaseStage param = new PB.HSEquipIncreaseStage();
            param.id = curData.id;
            GameApp.Instance.netManager.SendMessage(PB.code.EQUIP_INCREASE_STAGE_C.GetHashCode(), param);
        }

    }
    void OnEquipIncreaseLevelRet(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg == null || msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        PB.HSEquipIncreaseLevelRet result = msg.GetProtocolBody<PB.HSEquipIncreaseLevelRet>();

        EquipIncreaseReslut(result.id, result.stage, result.level);

        if (ICallBackDeletage!=null)
        {
            ICallBackDeletage.OnBuildEquipReture();
        }
    }

    void OnEquipIncreaseStageRet(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg == null || msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        PB.HSEquipIncreaseStageRet result = msg.GetProtocolBody<PB.HSEquipIncreaseStageRet>();

        EquipIncreaseReslut(result.id, result.stage, result.level);

        if (ICallBackDeletage != null)
        {
            ICallBackDeletage.OnBuildEquipReture();
        }
    }
    //此处只给强化升级返回成功后使用
    void EquipIncreaseReslut(long id, int stage, int level)
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
    }


    #region Bind And UnBund

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
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_INCREASE_LEVEL_C.GetHashCode().ToString(), OnEquipIncreaseLevelRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_INCREASE_LEVEL_S.GetHashCode().ToString(), OnEquipIncreaseLevelRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_INCREASE_STAGE_C.GetHashCode().ToString(), OnEquipIncreaseStageRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_INCREASE_STAGE_S.GetHashCode().ToString(), OnEquipIncreaseStageRet);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_INCREASE_LEVEL_C.GetHashCode().ToString(), OnEquipIncreaseLevelRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_INCREASE_LEVEL_S.GetHashCode().ToString(), OnEquipIncreaseLevelRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_INCREASE_STAGE_C.GetHashCode().ToString(), OnEquipIncreaseStageRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_INCREASE_STAGE_S.GetHashCode().ToString(), OnEquipIncreaseStageRet);
    }

    #endregion

}
