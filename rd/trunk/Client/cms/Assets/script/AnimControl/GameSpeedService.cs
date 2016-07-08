using UnityEngine;
using System.Collections;

public class GameSpeedService : MonoBehaviour {

    //---------------------------------------------------------------------------------------------
    //对局速度
    public float battleSpeedScale = 1.0f;
    //短暂的速度改变
    public float tmpSpeedScale = 1.0f;
    float tmpSpeedRevertTime = 0.0f;

    static GameSpeedService mInst = null;
    public static GameSpeedService Instance
    {
        get
        {
            if (mInst == null)
            {
                GameObject go = new GameObject("GameSpeedService");
                mInst = go.AddComponent<GameSpeedService>();
            }
            return mInst;
        }
    }
    //---------------------------------------------------------------------------------------------
    public void Init()
    {
        battleSpeedScale = PlayerPrefs.GetFloat("battleSpeed");
        if (battleSpeedScale == 0.0f)
        {
            SetBattleSpeed(1.0f);
        }
        else
        {
            RefreshSpeed();
        }
    }
    //---------------------------------------------------------------------------------------------
    public void SetTmpSpeed(float ratio, float len)
    {
        tmpSpeedScale = ratio;
        tmpSpeedRevertTime = len;

        RefreshSpeed();
    }
    //---------------------------------------------------------------------------------------------
    public void SetBattleSpeed(float ratio)
    {
        if (battleSpeedScale == ratio)
            return;

        battleSpeedScale = ratio;
        PlayerPrefs.SetFloat("battleSpeed", battleSpeedScale);

        RefreshSpeed();
    }
    //---------------------------------------------------------------------------------------------
    public float GetBattleSpeed()
    {
        return battleSpeedScale;
    }
    //---------------------------------------------------------------------------------------------
    public void OnModuleChange()
    {
        RefreshSpeed();
    }
    //---------------------------------------------------------------------------------------------
    void RefreshSpeed()
    {
        string curModule = GameMain.Instance.CurModuleAttr.ModuleNameAttr;
        if (curModule == "BattleModule")
        {
            Time.timeScale = battleSpeedScale * tmpSpeedScale;
        }
        else
        {
            Time.timeScale = tmpSpeedScale;
        }
    }
    //---------------------------------------------------------------------------------------------
	// Use this for initialization
	void Start () {
	
	}
    //---------------------------------------------------------------------------------------------
	
	// Update is called once per frame
	void Update () {

        if (tmpSpeedRevertTime > 0.0f)
        {
            tmpSpeedRevertTime -= Time.unscaledDeltaTime;
            if (tmpSpeedRevertTime <= 0.0f)
            {
                SetTmpSpeed(1.0f, 0.0f);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
}
