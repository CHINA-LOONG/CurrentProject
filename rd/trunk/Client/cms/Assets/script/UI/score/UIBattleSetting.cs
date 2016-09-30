using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class UIBattleSetting : UIBase 
{
    public static string ViewName = "UIBattleSetting";
    public Button mExitBtn;
    public Button mContinueBtn;
    public Text mExitTxt;
    public Text mContinueTxt;
    private MsgBox.PromptMsg mExitEnsureWnd;
    //---------------------------------------------------------------------------------------------
    void Start()
    {
        mExitBtn.onClick.AddListener(OnExitBattle);
        mContinueBtn.onClick.AddListener(OnContinueBtn);
        mExitTxt.text = StaticDataMgr.Instance.GetTextByID("setting_exit");
        mContinueTxt.text = StaticDataMgr.Instance.GetTextByID("setting_continue");
    }
    //---------------------------------------------------------------------------------------------
    void OnExitBattle()
    {
        mExitEnsureWnd = MsgBox.PromptMsg.Open(
            MsgBox.MsgBoxType.Conform_Cancel,
            StaticDataMgr.Instance.GetTextByID("setting_exit_tips"),
            EnsureExit
            );
    }
    //---------------------------------------------------------------------------------------------
    public void CloseEnsureExitWnd()
    {
        if (mExitEnsureWnd != null)
        {
            mExitEnsureWnd.Close();
        }
    }
    //---------------------------------------------------------------------------------------------
    private void OnContinueBtn()
    {
        BattleController.Instance.Process.Pause(false, false);
    }
    //---------------------------------------------------------------------------------------------
    private void EnsureExit(MsgBox.PrompButtonClick state)
    {
        if (state == MsgBox.PrompButtonClick.OK)
        {
            BattleController.Instance.Process.Pause(false, true);
        }
    }
    //---------------------------------------------------------------------------------------------
}
