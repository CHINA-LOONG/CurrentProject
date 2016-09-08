using UnityEngine;
using System.Collections;

public class SociatyErrorMsg
{
    
    //else if (error.errCode == (int)PB.allianceError.ALLIANCE_LEVEL_LIMIT)
    //{
    //    UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_046"),
    //                                   (int)PB.ImType.PROMPT);
    //}
    public static  void ShowImWithErrorCode(int error)
    {
        switch((PB.allianceError)error)
        {
            case PB.allianceError.ALLIANCE_LEVEL_NOT_ENOUGH:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_031"), (int)PB.ImType.PROMPT);
                break;
            case PB.allianceError.ALLIANCE_ALREADY_IN:
              
                break;
            case PB.allianceError.ALLIANCE_NOT_EXIST:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_010"), (int)PB.ImType.PROMPT);
                break;
            case PB.allianceError.ALLIANCE_NAME_ERROR:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_056"), (int)PB.ImType.PROMPT);
                break;
            case PB.allianceError.ALLIANCE_NAME_EXIST:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_006"), (int)PB.ImType.PROMPT);
                break;

            case PB.allianceError.ALLIANCE_NOT_JOIN:
                break;
            case PB.allianceError.ALLIANCE_NO_MAIN:
                break;
            case PB.allianceError.ALLIANCE_NOTICE_ERROR:
                //UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_006"), (int)PB.ImType.PROMPT);
                break;
            case PB.allianceError.ALLIANCE_ALREADY_APPLY:
                break;
            case PB.allianceError.ALLIANCE_ALREADY_FULL:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_002"), (int)PB.ImType.PROMPT);
                break;
            case PB.allianceError.ALLIANCE_APPLY_NOT_EXIST:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_045"), (int)PB.ImType.PROMPT);
                break;
            case PB.allianceError.ALLIANCE_POSITION_ERROR:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_018"), (int)PB.ImType.PROMPT);
            break;
            case PB.allianceError.ALLIANCE__LEAVE_NOT_EMPTY:
                break;
            case PB.allianceError.ALLIANCE_TARGET_NOT_JOIN:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_014"), (int)PB.ImType.PROMPT);
                break;
            case PB.allianceError.ALLIANCE_TARGET_ALREADY_JOIN:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_045"), (int)PB.ImType.PROMPT);
                break;

            case PB.allianceError.ALLIANCE_TECH_FULL:
                break;
            case PB.allianceError.ALLIANCE_LEVEL_LIMIT:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_031"), (int)PB.ImType.PROMPT);
                break;
            case PB.allianceError.ALLIANCE_CONTRI_NOT_ENOUGH:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_026"),
                                               (int)PB.ImType.PROMPT);
                break;
            case PB.allianceError.ALLIANCE_PRAY_MAX_COUNT:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_027"),
                                               (int)PB.ImType.PROMPT);
                break;
            case PB.allianceError.ALLIANCE_FRIZEN_TIME:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_037"), (int)PB.ImType.PROMPT);
                break;


            case PB.allianceError.ALLIANCE_MAX_FAGIGUE_COUNT:
                break;
            case PB.allianceError.ALLIANCE_FAGIGUE_GIVE_ALREADY:
                break;
            case PB.allianceError.ALLIANCE_CAPACITY_NOT_ENOUGH:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_023"), (int)PB.ImType.PROMPT);
                break;
            case PB.allianceError.ALLIANCE_SAME_POSITION:
                break;
            case PB.allianceError.ALLIANCE_MAX_APPLY:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_055"), (int)PB.ImType.PROMPT);
                break;

            case PB.allianceError.ALLIANCE_ALREADY_IN_TEAM:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_036"), (int)PB.ImType.PROMPT);
                break;
            case PB.allianceError.ALLIANCE_TEAM_NOT_EXIST:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_045"), (int)PB.ImType.PROMPT);
                break;
            case PB.allianceError.ALLIANCE_TEAM_FINISH:

                break;
            case PB.allianceError.ALLIANCE_TEAM_FULL:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_044"), (int)PB.ImType.PROMPT);
                break;
            case PB.allianceError.ALLIANCE_TASK_FINISH:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_035"), (int)PB.ImType.PROMPT);
                break;


            case PB.allianceError.ALLIANCE_NOT_IN_TEAM:
                break;
            case PB.allianceError.ALLIANCE_TASK_NOT_EXIST:
                break;
            case PB.allianceError.ALLIANCE_MAX_BIG_TASK:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_030"), (int)PB.ImType.PROMPT);
                break;
            case PB.allianceError.ALLIANCE_MAX_SMALL_TASK:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_048"), (int)PB.ImType.PROMPT);
                break;
            case PB.allianceError.ALLIANCE_QUEST_NOT_EXIST:
                break;

            case PB.allianceError.ALLIANCE_QUEST_FINISH:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_033"), (int)PB.ImType.PROMPT);
                break;
            case PB.allianceError.ALLIANCE_TASK_NOT_FINISH:
                break;
            case PB.allianceError.ALLIANCE_NOT_CAPTAIN:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_019"), (int)PB.ImType.PROMPT);
                break;
            case PB.allianceError.ALLIANCE_HAVE_MEMBER:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_060"), (int)PB.ImType.PROMPT);
                break;
            case PB.allianceError.ALLIANCE_REWARD_ALREADY_GIVE:
                break;

            case PB.allianceError.ALLIANCE_MAX_COPY_MAIN:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_040"), (int)PB.ImType.PROMPT);
                break;
            case PB.allianceError.ALLIANCE_APPLY_LIST_EMPTY:
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_045"), (int)PB.ImType.PROMPT);
                break;
        }
    }
}
