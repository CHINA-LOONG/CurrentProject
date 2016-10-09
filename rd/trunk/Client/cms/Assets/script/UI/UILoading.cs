using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
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
    public GameObject pvploadingG;
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
                loadingProgress.fillAmount = 1.0f;
            }
            else
            {
                loadingProgress.fillAmount = 1.0f - (float)remainCount / totalAssetCount;
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
        loadingProgress.fillAmount = 0.0f;
        totalAssetCount = ResourceMgr.Instance.GetAssetRequestCount();
    }
    //---------------------------------------------------------------------------------------------
    public void SetProgress(float ratio)
    {
        loadingProgress.fillAmount = ratio;
    }
    //---------------------------------------------------------------------------------------------
    public void SetLoading(int loadingType = 1000)
    {
        LoadingData loadingData = StaticDataMgr.Instance.GetLoadingData(loadingType);
        int randomNum = Random.Range(0, loadingData.imageResource.Length);
        loadingBackground.sprite = ResourceMgr.Instance.LoadAssetType<Sprite>(loadingData.imageResource[randomNum]);
        randomNum = Random.Range(0, loadingData.content.Length);
        gamePrompt.text = StaticDataMgr.Instance.GetTextByID(loadingData.content[randomNum]);
    }
    //---------------------------------------------------------------------------------------------
    void OnEnable()
    {
        beginLoading = false;
        loadingProgress.fillAmount = 0.0f;
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
