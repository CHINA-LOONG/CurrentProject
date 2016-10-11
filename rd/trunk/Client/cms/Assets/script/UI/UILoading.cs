using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
public enum LoadingType
{ 
    loadingDefault = 1,
    loadingFb = 2,
    loadingHole = 3,
    loadingTower = 4,
    loadingGuild = 5,
    loadingPvp = 6
}

public class UILoading : UIBase
{
    public delegate IEnumerator LoadingFinishCallback();
    public static string ViewName = "loading";
    public Image loadingProgress;
    public Text loadingText;
    public LoadingFinishCallback mLoadingCallback = null;
    public Image loadingBackground;
    public Text gamePrompt;
    private bool beginLoading = false;
    private int totalAssetCount = 0;
    public Text player1Name;
    public Text player1PowerNum;
    public GameObject player1lineup;
    public Text player2Name;
    public Text player2PowerNum;
    public GameObject player2lineup;
    public Text powerText1;
    public Text powerText2;
    public Text contentName;
    public Image contentImage;
    public GameObject pvploadingG;
    public GameObject loadingContent;

    private float loadingBarSizeDelthaX = 100;
    private float loadingBarSizeDelthaY = 9;

    private RectTransform _loadingProgressRt;
    private RectTransform loadingProgressRt
    {
        get
        {
            if(null == _loadingProgressRt)
            {
                _loadingProgressRt = loadingProgress.transform as RectTransform;
                loadingBarSizeDelthaX = loadingProgressRt.sizeDelta.x;
                loadingBarSizeDelthaY = loadingProgressRt.sizeDelta.y;
            }
            return _loadingProgressRt;
        }
    }

    //-------------------------------------------------------------------------------------------
    public override void Init()
    {
        mLoadingCallback = null;
    }
    //---------------------------------------------------------------------------------------------
    public void SetLoadingCallback(LoadingFinishCallback callback)
    {
        mLoadingCallback = callback;
    }
    //---------------------------------------------------------------------------------------------
    void Update()
    {
        if (beginLoading == true)
        {
            int remainCount = ResourceMgr.Instance.GetAssetRequestCount();
            if (totalAssetCount == 0)
            {
                loadingProgressRt.sizeDelta = new Vector2(loadingBarSizeDelthaX, loadingBarSizeDelthaY);
            }
            else
            {
                loadingProgressRt.sizeDelta = new Vector2((1.0f - (float)remainCount / totalAssetCount) * loadingBarSizeDelthaX, loadingBarSizeDelthaY);
            }
            loadingText.text = (totalAssetCount - remainCount).ToString() + "/" + totalAssetCount.ToString();

            if (remainCount == 0)
            {
                beginLoading = false;
                if (mLoadingCallback != null)
                {
                    StartCoroutine(mLoadingCallback());
                }
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void UpdateTotalAssetCount()
    {
        beginLoading = true;
        loadingProgressRt.sizeDelta = new Vector2(0, loadingBarSizeDelthaY);
        totalAssetCount = ResourceMgr.Instance.GetAssetRequestCount();
    }
    //---------------------------------------------------------------------------------------------
    public void SetProgress(float ratio)
    {
        loadingProgressRt.sizeDelta = new Vector2(ratio * loadingBarSizeDelthaX, loadingBarSizeDelthaY);
    }
    //---------------------------------------------------------------------------------------------
    public void SetLoading(LoadingType loadingType)
    {
        loadingContent.SetActive(true);
        if (StaticDataMgr.Instance.GetLoadingData((int)loadingType) == null)
            loadingType = LoadingType.loadingDefault;
        LoadingData loadingData = StaticDataMgr.Instance.GetLoadingData((int)loadingType);
        int randomNum = Random.Range(0, loadingData.loadingResource.Length);
        Loadinglocation loadingLocation = StaticDataMgr.Instance.GetLoadinglocationData(loadingData.loadingResource[randomNum]);
        contentName.text = StaticDataMgr.Instance.GetTextByID(loadingLocation.tips);

        loadingBackground.sprite = ResourceMgr.Instance.LoadAssetType<Sprite>(loadingLocation.background);
        contentImage.sprite = ResourceMgr.Instance.LoadAssetType<Sprite>(loadingLocation.asset);

        contentImage.gameObject.transform.localPosition = loadingContent.transform.FindChild("contentImage" + loadingLocation.location).transform.localPosition;
        contentName.gameObject.transform.localPosition = loadingContent.transform.FindChild("contentText" + loadingLocation.location).transform.localPosition;

        randomNum = Random.Range(0, loadingData.loadingTips.Length);
        gamePrompt.text = StaticDataMgr.Instance.GetTextByID(loadingData.loadingTips[randomNum]);
    }
    //---------------------------------------------------------------------------------------------
    void OnEnable()
    {
        beginLoading = false;
        loadingProgressRt.sizeDelta = new Vector2(0, loadingBarSizeDelthaY);
        loadingText.text = string.Empty;
        pvploadingG.SetActive(false);
    }
    //---------------------------------------------------------------------------------------------    
    public void OpenPvploading(PvpFightParam pvpFinghtParam)
    {
        pvploadingG.SetActive(true);
        List<PB.HSMonster> pvpEnemyList = pvpFinghtParam.targetData.defenceData.monsterInfo;
        for (int i = 0; i < pvpEnemyList.Count; i++)
        {
            MonsterIcon icon = MonsterIcon.CreateIcon();
            icon.transform.SetParent(player2lineup.transform, false);
            icon.SetMonsterStaticId(pvpEnemyList[i].cfgId);
            icon.SetLevel(pvpEnemyList[i].level);
            icon.SetStage(pvpEnemyList[i].stage);
        }
        PbUnit pb = null;
        Dictionary<int, PbUnit> unitPbList = GameDataMgr.Instance.PlayerDataAttr.unitPbList;
        for (int i = 0; i < pvpFinghtParam.playerTeam.Count; ++i)
        {
            if (unitPbList.TryGetValue(pvpFinghtParam.playerTeam[i], out pb))
            {
                MonsterIcon icon = MonsterIcon.CreateIcon();
                icon.transform.SetParent(player1lineup.transform, false);
                icon.SetMonsterStaticId(pb.id);
                icon.SetLevel(pb.level);
                icon.SetStage(pb.stage);
            }
        }
        player1Name.text = GameDataMgr.Instance.PlayerDataAttr.nickName;
        player1PowerNum.text = pvpFinghtParam.enemyBp.ToString();
        player2Name.text = pvpFinghtParam.targetData.name;
        player2PowerNum.text = pvpFinghtParam.myBp.ToString();

    }
    //-------------------------------------------------------------------
    void Start()
    {
        powerText1.text = StaticDataMgr.Instance.GetTextByID("arrayselect_bp_001");
        powerText2.text = StaticDataMgr.Instance.GetTextByID("arrayselect_bp_001");
    }
}
