using UnityEngine;
using System.Collections;

public class PvpErrorMsg
{

    public static void ShowImWithErrorCode(int error)
    {
        switch ((PB.pvpError)error)
        {
            case PB.pvpError.PVP_TIMES_NOT_ENOUGH:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("pvp_record_002"), (int)PB.ImType.PROMPT);
                break;
            case PB.pvpError.PVP_NOT_MATCH_BEFORE:
                break;
            case PB.pvpError.PVP_NOT_MATCH_TARGET:
                MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform, StaticDataMgr.Instance.GetTextByID("pvp_first"));
                break;
        }
    }
}
