using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
public class SummonItem
{
    public SummonItem()
    {
        beginShow = false;
        endShow = false;
    }
    public PB.RewardItem item;
    public bool beginShow = false;
    public bool endShow = false;
}
public enum ConsumeType
{
    Consume_jinbi = 1,
    Consume_zuanshi = 2,
    Consume_Free_jinbi = 3,
    Consume_Free_zuanshi = 4,
    Num_Consume
}
public class UISummon : UIBase
{
    public static string ViewName = "UISummon2";
    public List<SummonItem> summonList = new List<SummonItem>();
    ConsumeType consum = ConsumeType.Num_Consume;
    UISumReward uiSumReward = null;
    public GameObject exit;
    public GameObject onceButton;
    public GameObject tenButton;
    public GameObject skipButton;
    public GameObject SumTypeImg;
    public GameObject SumTypeImg1;
    public GameObject ui;
    public Text freeState;
    public Text freeImage;
    public Text onceButtonText;
    public Text tenButtonText;
    public Text consumOnceText;
    public Text consumTenText;
    Animator choudanAnim;
    SummonEffects summEffect;
    TimeEventWrap timeEvent;
    Sprite jimbiIcon;
    Sprite zuanshiIcon;
    bool consumB;
    string anim = "xiaoK";
    public float endTime = 0.0f;
    public int summonNum = 0;
    static UISummon mInst = null;
    public static UISummon Instance
    {
        get
        {
            return mInst;
        }
    }
    //---------------------------------------------------------------------------
    public void OpenUISummon(bool isJinBi)
    {
        consumB = isJinBi;
        if (summEffect != null)
        {
            summEffect.SetSummonState(isJinBi);
        }
        if (jimbiIcon != null)
            SetConsumIcon();
    }
    //---------------------------------------------------------------------------
    void SetConsumIcon()//初始化设置
    {
        if (consumB)
        {
            SumTypeImg.GetComponent<Image>().sprite = jimbiIcon;
            SumTypeImg1.GetComponent<Image>().sprite = jimbiIcon;
            consumOnceText.text = GameConfig.Instance.jinBiSum.ToString();
            int jinBi = GameConfig.Instance.jinBiSum * 9;
            consumTenText.text = jinBi.ToString();
        }
        else
        {
            SumTypeImg.GetComponent<Image>().sprite = zuanshiIcon;
            SumTypeImg1.GetComponent<Image>().sprite = zuanshiIcon;
            consumOnceText.text = GameConfig.Instance.zuanShiSum.ToString();
            int zuanshi = GameConfig.Instance.zuanShiSum * 9;
            consumTenText.text = zuanshi.ToString();
        }
    }
    //---------------------------------------------------------------------------
    void SummonOnce(GameObject go)//单次召唤点击事件
    {
        ConsumeType cons = ConsumeType.Num_Consume;
        if (consumB)
        {
            cons = ConsumeType.Consume_jinbi;
            if (GameDataMgr.Instance.freeJinbiSumNum <=5)
            {
                if ((GameDataMgr.Instance.summonJinbi + GameConfig.Instance.jinBiFree) < GameTimeMgr.Instance.GetServerTimeStamp())
                {
                    cons = ConsumeType.Consume_Free_jinbi;
                    GameDataMgr.Instance.freeJinbiSumNum++;
                }
            }                
            consum = cons;
        }
        else
		{
			cons = ConsumeType.Consume_zuanshi;            
            if ((GameDataMgr.Instance.summonZuanshi + GameConfig.Instance.zuanShiFree) < GameTimeMgr.Instance.GetServerTimeStamp())
            {
				cons = ConsumeType.Consume_Free_zuanshi;
            }
            consum = cons;
        }
        showUI(false);
        SummonRequest(false);
    }
    //---------------------------------------------------------------------------
    void SummonTen(GameObject go)//十次召唤点击事件
    {
        if (consumB)
            consum = ConsumeType.Consume_jinbi;
        else
            consum = ConsumeType.Consume_zuanshi;
        showUI(false,true);
        SummonRequest(true);
    }
    //---------------------------------------------------------------------------
    void SkipClick(GameObject go)//跳过
    {
        ShowSummonReward(true);
    }
    //---------------------------------------------------------------------------
    IEnumerator BeginShowSummon()//开始召唤
    {
        choudanAnim.SetBool("nullAnim", false);
        if (PlayerPrefs.GetString("animSumBegin") != string.Empty)
            anim = PlayerPrefs.GetString("animSumBegin");
        choudanAnim.SetBool(anim, true);
        endTime = Time.time;
        if (summonNum < summonList.Count)
        {
            summonList[summonNum].beginShow = true;
        }
        yield return new WaitForSeconds(1f);
        choudanAnim.SetBool(anim, false);
        choudanAnim.SetBool("loop", true);
    }
    //---------------------------------------------------------------------------
    void ShowAnim(int stage,bool isJinbi)
    {
        if (stage > 3)
        {
            if (isJinbi)
            {
                summEffect.taiyangB2.SetActive(true);
            }
            else
            {
                summEffect.yueliangB2.SetActive(true);
            }
            summEffect.xiaoMian.SetActive(true);
            summEffect.xiaoEnd.SetActive(true);
            choudanAnim.SetBool("xiaoJ", true);
            PlayerPrefs.SetString("animSumBegin", "xiaoK");
            PlayerPrefs.SetString("animSumEnd", "xiaoJ");
        }
        else if (stage == 3)
        {
            if (isJinbi)
            {
                summEffect.taiyangB1.SetActive(true);
            }
            else
            {
                summEffect.yueliangB1.SetActive(true);
            }
            summEffect.jingMian.SetActive(true);
            summEffect.jingEnd.SetActive(true);
            choudanAnim.SetBool("jingJ", true);
            PlayerPrefs.SetString("animSumBegin", "jingyaK");
            PlayerPrefs.SetString("animSumEnd", "jingJ");
        }
        else
        {
            summEffect.kuEnd.SetActive(true);
            choudanAnim.SetBool("kuJ", true);
            PlayerPrefs.SetString("animSumBegin", "kuK");
            PlayerPrefs.SetString("animSumEnd", "kuJ");
        }
    }
    //---------------------------------------------------------------------------
    IEnumerator EndShowSummon()//召唤结束
    {
        if (summonList[summonNum].item.type == (int)PB.itemType.ITEM)
        {
            ItemStaticData itemData = StaticDataMgr.Instance.GetItemData(summonList[summonNum].item.itemId);
            ShowAnim(itemData.grade, consumB);
        }
        else if (summonList[summonNum].item.type == (int)PB.itemType.MONSTER)
        {
            PB.HSMonster monster = summonList[summonNum].item.monster;
            ShowAnim(StaticDataMgr.Instance.GetUnitRowData(monster.cfgId).rarity, consumB);
        }
        else if (summonList[summonNum].item.type == (int)PB.itemType.EQUIP)
        {
            ShowAnim(summonList[summonNum].item.stage, consumB);
        }       
        choudanAnim.SetBool("loop", false);
        yield return new WaitForSeconds(2.40f);
        choudanAnim.SetBool(PlayerPrefs.GetString("animSumEnd"), false);
        choudanAnim.SetBool("nullAnim", true);
        yield return new WaitForSeconds(1f);
        summEffect.Reset();
        ShowSummonReward();
    }
    //---------------------------------------------------------------------------
    void Update()
    {
        int count = summonList.Count;
        if (summonNum >= count && summonNum > 0)
        {
            summonNum = 0;
            count = 0;
            summonList.Clear();
        }
        if (count > 0)
        {
            //compare time
            if ((Time.time - endTime) > 4.0f &&
                summonList[summonNum].beginShow == true &&
                summonList[summonNum].endShow == false
                )
            {
                summonList[summonNum].endShow = true;
                StartCoroutine(EndShowSummon());
            }

            if (summonList[summonNum].beginShow == false)
            {
                StartCoroutine(BeginShowSummon());
            }
        }
    }
    //---------------------------------------------------------------------------
    void  ShowSummonReward(bool skip = false)//显示召唤结果
    {
        uiSumReward = UIMgr.Instance.OpenUI_(UISumReward.ViewName) as UISumReward;
        if (skip)
        {
            if (summonList.Count>1)
            uiSumReward.OpenSumReward(consum,true);
            else 
                uiSumReward.OpenSumReward(consum);
            exitClick(null);
            summonNum = 0;
            summonList.Clear();
        }
        else
        {
            //展示召唤获得
            if (summonList.Count > 1)
            {
                if (summonList.Count == (summonNum + 1))
                {
                    uiSumReward.OpenSumReward(summonList[summonNum], consum);
                }
                else
                {
                    uiSumReward.OpenSumReward(summonList[summonNum], consum, true);
                }
            }
            else
            {
                uiSumReward.OpenSumReward(summonList[summonNum], consum);
            }
        }
    }
    //---------------------------------------------------------------------------
    void SummonRequest(bool isSummonTen)//发送召唤请求
    {
        StartCoroutine(BeginShowSummon());
        if (!isSummonTen)
        {
            PB.HSSummonOne param = new PB.HSSummonOne
            {
                type = (int)consum
            };
            GameApp.Instance.netManager.SendMessage(PB.code.SUMMON_ONE_C.GetHashCode(), param, false);
        }
        else
        {
            PB.HSSummonTen param = new PB.HSSummonTen
            {
                type = (int)consum
            };
            GameApp.Instance.netManager.SendMessage(PB.code.SUMMON_TEN_C.GetHashCode(), param, false);
        }
    }
    //---------------------------------------------------------------------------
    void OnOneSummonRet(ProtocolMessage msg)//召唤单次协议返回
    {
        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode error = msg.GetProtocolBody<PB.HSErrorCode>();
            Logger.LogError("RequestShopData Error errorCode: " + error.errCode);
            choudanAnim.SetBool(PlayerPrefs.GetString("animSumBegin"), false);
            choudanAnim.SetBool("loop", false);
            showUI(true);
            return;
        }
        PB.HSSummonOneRet result = msg.GetProtocolBody<PB.HSSummonOneRet>();
        if (result.freeCoinLastTime != 0)
        {
            GameDataMgr.Instance.summonJinbi = result.freeCoinLastTime;
        }
        if (result.freeDiamondBeginTime != 0)
        {
            GameDataMgr.Instance.summonZuanshi = result.freeDiamondBeginTime;
        }
        SetFreeTime();
        SetSummonList(result.reward.RewardItems[0]);
    }
    //---------------------------------------------------------------------------
    void OnTenSummonRet(ProtocolMessage msg)//召唤十次协议返回
    {
        PB.HSSummonTenRet result = msg.GetProtocolBody<PB.HSSummonTenRet>();
        for (int i = 0; i < result.reward.Count; i++)
        {
            SetSummonList(result.reward[i].RewardItems[0]);
        }
    }
    //---------------------------------------------------------------------------
    void SetSummonList(PB.RewardItem reward)
    {
        SummonItem item = new SummonItem();
        item.item = reward;
        item.beginShow = false;
        item.endShow = false;
        summonList.Add(item);
        summonList[0].beginShow = true;
    }
    //---------------------------------------------------------------------------
    public void showUI(bool isShow,bool isTen = false)
    {
       if (isShow)
        {
            ui.SetActive(true);
            skipButton.SetActive(false);
        }
        else
        {
            ui.SetActive(false);
            skipButton.SetActive(true);
        }
       if (!isTen)
            skipButton.SetActive(false);
        else
           skipButton.SetActive(true);
    }
    //---------------------------------------------------------------------------
    void exitClick(GameObject go)
    {
        showUI(true);
        summEffect.Reset();
        if (go != null)
            UIMgr.Instance.CloseUI_(UISummon.ViewName);
        else
        {
            choudanAnim.SetBool("loop", false);
            choudanAnim.SetBool("xiaoJ", true);
        }
    }
    //---------------------------------------------------------------------------
    void Start()
    {
        mInst = this;
        ui.SetActive(true);
        skipButton.SetActive(false);
        EventTriggerListener.Get(onceButton).onClick = SummonOnce;
        EventTriggerListener.Get(tenButton).onClick = SummonTen;
        EventTriggerListener.Get(skipButton).onClick = SkipClick;
        EventTriggerListener.Get(exit).onClick = exitClick;
        summEffect = GameObject.Find("Root/choudan_biaopan").GetComponent<SummonEffects>();
        choudanAnim = summEffect.summAnim;
        onceButtonText.text = StaticDataMgr.Instance.GetTextByID("summon_onetime");
        tenButtonText.text = StaticDataMgr.Instance.GetTextByID("summon_tentime");
        summEffect.SetSummonState(consumB);
        jimbiIcon = ResourceMgr.Instance.LoadAssetType<Sprite>("icon_jinbi");
        zuanshiIcon = ResourceMgr.Instance.LoadAssetType<Sprite>("icon_zuanshi");
        SetConsumIcon();
    }
    //---------------------------------------------------------------------------
    public void SetFreeTime()
    {
        if (consumB)
        {
            if (GameDataMgr.Instance.freeJinbiSumNum >= 5)
            {
                freeState.text = StaticDataMgr.Instance.GetTextByID("summon_useout");
                freeState.gameObject.SetActive(true);
                freeImage.gameObject.SetActive(false);
                SumTypeImg.gameObject.SetActive(true);
            }
            else
            {
                if ((GameDataMgr.Instance.summonJinbi + GameConfig.Instance.jinBiFree) < GameTimeMgr.Instance.GetServerTimeStamp())
                {
                    freeImage.gameObject.SetActive(true);
                    SumTypeImg.gameObject.SetActive(false);
                    freeState.gameObject.SetActive(true);
                    freeState.text = string.Format(StaticDataMgr.Instance.GetTextByID("summon_lefttimecoin"), (5 - GameDataMgr.Instance.freeJinbiSumNum));
                }
                else
                {
                    SumTypeImg.gameObject.SetActive(true);
                    freeImage.gameObject.SetActive(false);
                    freeState.gameObject.SetActive(true);
                    timeEvent = new TimeEventWrap((GameDataMgr.Instance.summonJinbi + GameConfig.Instance.jinBiFree), endFreeTime);
                    timeEvent.AddUpdateEvent(OnUpdateTime);
                }
            }
        }
        else
        {
            if ((GameDataMgr.Instance.summonZuanshi + GameConfig.Instance.zuanShiFree) < GameTimeMgr.Instance.GetServerTimeStamp())
            {
				SumTypeImg.gameObject.SetActive(false);
                freeImage.gameObject.SetActive(true);
				freeState.gameObject.SetActive(false);
            }
            else
            {
				SumTypeImg.gameObject.SetActive(true);
				freeImage.gameObject.SetActive(false);
				freeState.gameObject.SetActive(true);
                timeEvent = new TimeEventWrap( (GameDataMgr.Instance.summonZuanshi + GameConfig.Instance.zuanShiFree), endFreeTime);
                timeEvent.AddUpdateEvent(OnUpdateTime);
            }
        }
    }
    //---------------------------------------------------------------------------
    void endFreeTime()
    {
        timeEvent.RemoveTimeEvent();
        timeEvent.RemoveUpdateEvent(OnUpdateTime);
        SetFreeTime();
    }
    //---------------------------------------------------------------------------
    void OnUpdateTime(int time)
    {
        if (consumB)
            freeState.text = string.Format(StaticDataMgr.Instance.GetTextByID("summon_recovertimecoin"), UIUtil.Convert_hh_mm_ss(time));
        else
            freeState.text = string.Format(StaticDataMgr.Instance.GetTextByID("summon_recovertimegold"), UIUtil.Convert_hh_mm_ss(time));
    }
    //---------------------------------------------------------------------------
    void OnEnable()
    {
        BindListener();
    }
    //---------------------------------------------------------------------------
    void OnDisable()
    {
        UnBindListener();
    }
    //---------------------------------------------------------------------------
    void BindListener()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.SUMMON_ONE_C.GetHashCode().ToString(), OnOneSummonRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.SUMMON_ONE_S.GetHashCode().ToString(), OnOneSummonRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.SUMMON_TEN_S.GetHashCode().ToString(), OnTenSummonRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.SUMMON_TEN_C.GetHashCode().ToString(), OnTenSummonRet);
    }
    //---------------------------------------------------------------------------
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.SUMMON_ONE_S.GetHashCode().ToString(), OnOneSummonRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.SUMMON_TEN_S.GetHashCode().ToString(), OnTenSummonRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.SUMMON_ONE_C.GetHashCode().ToString(), OnOneSummonRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.SUMMON_TEN_C.GetHashCode().ToString(), OnTenSummonRet);
    }
}
