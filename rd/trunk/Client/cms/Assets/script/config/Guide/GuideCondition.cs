using UnityEngine;
using System.Collections;

public class GuideCondition
{
    public static bool IsConditionOK(int guideGroupId)
    {
        switch(guideGroupId)
        {
            case 1:
                return guide1_2IsOk(true);
            case 2:
                return guide1_2IsOk(false);
            case 3:
                return guide3_4IsOk(true);
            case 4:
                return guide3_4IsOk(false);
            case 5:
                return guide5_6IsOk(true);
            case 6:
                return guide5_6IsOk(false);
            case 7:
                return guide7_8IsOk(true);
            case 8:
                return guide7_8IsOk(false);
            case 9:
                return guide9_10IsOk(true);
            case 10:
                return guide9_10IsOk(false);
            case 11:
                return guide11_12IsOk(true);
            case 12:
                return guide11_12IsOk(false);
            case 13:
                return guide13_14IsOk(true);
            case 14:
                return guide13_14IsOk(false);
            default:
                Logger.LogErrorFormat("新手引导groupId 为 {0},这个谁配置的，需要在GuideCondition中增加条件！FF ");
                break;
        }
        return false;
    }

    static bool  guide1_2IsOk(bool moreButtonIsOpen)
    {
        return GuideHelp.Instance.IsBuidMoreButtonExpand() == moreButtonIsOpen;
    }

    static  bool guide3_4IsOk(bool moreButtonIsOpen)
    {
        if (GuideHelp.Instance.IsBuidMoreButtonExpand() != moreButtonIsOpen)
            return false;

        return true;
    }
    static bool guide5_6IsOk(bool moreButtonIsOpen)
    {
        if (GuideHelp.Instance.IsBuidMoreButtonExpand() != moreButtonIsOpen)
            return false;
        return true;
    }

    static bool guide7_8IsOk(bool moreButtonIsOpen)
    {
        if (GuideHelp.Instance.IsBuidMoreButtonExpand() != moreButtonIsOpen)
            return false;
        return true;
    }
    static bool guide9_10IsOk(bool moreButtonIsOpen)
    {
        if (GuideHelp.Instance.IsBuidMoreButtonExpand() != moreButtonIsOpen)
            return false;
        return true;
    }

    static bool guide11_12IsOk(bool moreButtonIsOpen)
    {
        if (GuideHelp.Instance.IsBuidMoreButtonExpand() != moreButtonIsOpen)
            return false;
        return true;
    }
    static bool guide13_14IsOk(bool moreButtonIsOpen)
    {
        if (GuideHelp.Instance.IsBuidMoreButtonExpand() != moreButtonIsOpen)
            return false;
        return true;
    }
}
