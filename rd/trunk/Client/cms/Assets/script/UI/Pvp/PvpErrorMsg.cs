using UnityEngine;
using System.Collections;

public class PvpErrorMsg
{

    public static void ShowImWithErrorCode(int error)
    {
        //test
        switch ((PB.allianceError)error)
        {
            case PB.allianceError.ALLIANCE_LEVEL_NOT_ENOUGH:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_031"), (int)PB.ImType.PROMPT);
                break;
        }
    }
}
