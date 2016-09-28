using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class UIGainPet : UIBase
{
    public static string ViewName = "UIGainPet";
    public Button mConfirmBtn;
    public Text mConfirmBtnText;
    public Text mGainPetText;

    private GameObject mGainPetRender;
    private BattleObject mGainPetBo;
    private float mGainPetEndTime;
    private Fade mFadeWhite;
    //---------------------------------------------------------------------------------------------
    void Awake()
    {
        //minus time means invalidate time
        mGainPetEndTime = -1.0f;
        mConfirmBtnText.text = StaticDataMgr.Instance.GetTextByID("ui_queding");
        mConfirmBtn.gameObject.SetActive(false);
        mGainPetText.gameObject.SetActive(false);
    }
    //---------------------------------------------------------------------------------------------
    void Update()
    {
        if (mGainPetEndTime > 0.0f && Time.time >= mGainPetEndTime)
        {
            //minus time means invalidate time
            mGainPetEndTime = -1.0f;

            mConfirmBtn.gameObject.SetActive(true);
            mGainPetText.gameObject.SetActive(true);
            //mGainPetBo.TriggerEvent("chuchang", Time.time, null);
        }
    }
    //---------------------------------------------------------------------------------------------
    void OnDestroy()
    {
        if (mGainPetRender != null)
        {
            ResourceMgr.Instance.DestroyAsset(mGainPetRender);
        }

        if (mGainPetBo != null)
        {
            ObjectDataMgr.Instance.RemoveBattleObject(mGainPetBo.guid);
        }

        mGainPetRender = null;
        mGainPetBo = null;
    }
    //---------------------------------------------------------------------------------------------
    public void SetConfirmCallback(EventTriggerListener.VoidDelegate callBack)
    {
        EventTriggerListener.Get(mConfirmBtn.gameObject).onClick = callBack;
    }
    //---------------------------------------------------------------------------------------------
    public void ShowGainPet(string monsterID)
    {
        //UICamera.Instance.CameraAttr.enabled = true;
        GameUnit gainPet = GameUnit.CreateFakeUnit(BattleConst.enemyStartID, monsterID);
        ShowGainPetInternal(gainPet);
        //StartCoroutine(ShowGainPetFlash(monsterID));
    }
    //---------------------------------------------------------------------------------------------
    IEnumerator ShowGainPetFlash(string monsterID)
    {
        UICamera.Instance.CameraAttr.enabled = false;
        mFadeWhite = Camera.main.GetComponent<Fade>();
        mFadeWhite.FadeOut(0.5f);
        yield return new WaitForSeconds(1.0f);
        mFadeWhite.FadeIn(2.0f);
        UICamera.Instance.CameraAttr.enabled = true;
        GameUnit gainPet = GameUnit.CreateFakeUnit(BattleConst.enemyStartID, monsterID);
        ShowGainPetInternal(gainPet);
    }
    //---------------------------------------------------------------------------------------------
    public void ShowGainPet(int guid)
    {
        GameUnit gainPet = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(guid);
        ShowGainPetInternal(gainPet);
    }
    //---------------------------------------------------------------------------------------------
    void ShowGainPetInternal(GameUnit gainPet)
    {
        if (mGainPetRender != null || mGainPetBo != null)
        {
            Logger.LogError("the gain pet camera already created!");
        }

        mGainPetText.text = StaticDataMgr.Instance.GetTextByID("gain_pet") + gainPet.name;

        mGainPetRender = ResourceMgr.Instance.LoadAsset("GainPetCamera");
        if (mGainPetRender != null)
        {
            //GameObject startObj = Util.FindChildByName(mGainPetRender, "PetStartRoot");
            GameObject endObj = Util.FindChildByName(mGainPetRender, "PetEndRoot");
            Quaternion localRot = Quaternion.identity;
            mGainPetBo = ObjectDataMgr.Instance.CreateBattleObject(
                                        gainPet,
                                        mGainPetRender,
                                        endObj.transform.localPosition,
                                        endObj.transform.localRotation
                                        );
            mGainPetBo.SetTargetRotate(endObj.transform.localRotation, false);

            //mGainPetBo.transform.DOMove(endObj.transform.position, BattleConst.battleEndDelay);
            mGainPetEndTime = Time.time + BattleConst.battleEndDelay;
            mGainPetBo.TriggerEvent("gainUnitMove", Time.time, null);
        }
    }
    //---------------------------------------------------------------------------------------------
    public override void Clean()
    {
        ObjectDataMgr.Instance.RemoveBattleObject(mGainPetBo.guid);
        ResourceMgr.Instance.DestroyAsset(mGainPetRender);
        mGainPetBo = null;
        mGainPetRender = null;
        //minus time means invalidate time
        mGainPetEndTime = -1.0f;
        //mFadeWhite.FadeIn(0.0f);
    }
    //---------------------------------------------------------------------------------------------
}
