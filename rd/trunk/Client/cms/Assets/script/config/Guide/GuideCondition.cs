using UnityEngine;
using System.Collections;

public class GuideCondition
{
    public static bool IsConditionOK(int guideGroupId)
    {
        switch(guideGroupId)
        {
            case 1:
                return guide1IsOk(true);
            case 2:
                return guide1IsOk(false);
            case 3:
                return guide3_4IsOk(true);
            case 4:
                return guide3_4IsOk(false);
            case 5:
                return guide5IsOk(true);
            case 6:
                return guide6IsOk(true);
            case 7:
                return guide7IsOk(true);
            case 8:
                return guide7IsOk(false);
            case 9:
                return guide9IsOk(true);
            case 10:
                return guide10IsOk(true);
            case 11:
                return guide11IsOk(true);
            case 12:
                return guide12IsOk(true);
            case 13:
                return guide13IsOk(true);
            case 14:
                return guide14IsOk(true);
            case 15:
                return guide15IsOk();
            case 16:
                return guide16_17IsOK(true);
            case 17:
                return guide16_17IsOK(false);
            case 18:
                return guide18_19_20IsOK();
            case 19:
                return guide18_19_20IsOK();
            case 20:
                return guide18_19_20IsOK();
            case 21:
                return guide21IsOK();

            default:
                Logger.LogErrorFormat("新手引导groupId 为 {0},这个谁配置的，需要在GuideCondition中增加条件！FF ", guideGroupId);
                break;
        }
        return false;
    }

    static bool  guide1IsOk(bool moreButtonIsOpen)
    {
        if (GuideHelp.Instance.IsSigninAutoOpen())
            return false;

        return GuideHelp.Instance.IsBuidMoreButtonExpand() == moreButtonIsOpen;
    }

    static  bool guide3_4IsOk(bool moreButtonIsOpen)
    {
        if (GuideHelp.Instance.IsSigninAutoOpen())
            return false;

		return GuideHelp.Instance.IsBuidMoreButtonExpand() == moreButtonIsOpen;
	}
	static bool guide5IsOk(bool moreButtonIsOpen)
    {
        if (GuideHelp.Instance.IsSigninAutoOpen())
            return false;

        if (GuideHelp.Instance.IsBuidMoreButtonExpand() != moreButtonIsOpen)
            return false;

        return GuideHelp.Instance.IsPetFragmentEnough();
    }

	static bool guide6IsOk(bool moreButtonIsOpen)
	{
		if (GuideHelp.Instance.IsSigninAutoOpen())
			return false;
		
		if (GuideHelp.Instance.IsBuidMoreButtonExpand() != moreButtonIsOpen && GuideHelp.Instance.IsPetFragmentEnough())
			return true;
		
		return false;
	}
	
	static bool guide7IsOk(bool moreButtonIsOpen)
    {
        if (GuideHelp.Instance.IsSigninAutoOpen())
            return false;

        if (GuideHelp.Instance.IsBuidMoreButtonExpand() != moreButtonIsOpen)
            return false;
        return true;
    }
    static bool guide9IsOk(bool moreButtonIsOpen)
    {
        if (GuideHelp.Instance.IsSigninAutoOpen())
            return false;

        if (GuideHelp.Instance.IsBuidMoreButtonExpand() != moreButtonIsOpen)
            return false;
		if (GuideHelp.Instance.GetExpItemCount() > 0 && GuideHelp.Instance.GetExpUseCount() == 0 && GuideHelp.Instance.GetPlayerLevel() > GuideHelp.Instance.GetPetMaxLevel())
			return true;
        else
            return false;
    }
	static bool guide10IsOk(bool moreButtonIsOpen)
	{
		if (GuideHelp.Instance.IsSigninAutoOpen())
			return false;
		
		if (GuideHelp.Instance.IsBuidMoreButtonExpand() != moreButtonIsOpen && GuideHelp.Instance.GetExpItemCount() > 0 && GuideHelp.Instance.GetExpUseCount() == 0 && GuideHelp.Instance.GetPlayerLevel() > GuideHelp.Instance.GetPetMaxLevel())
			return true;
		else
			return false;
	}
	
	static bool guide11IsOk(bool moreButtonIsOpen)
    {
        if (GuideHelp.Instance.IsSigninAutoOpen())
            return false;

        if (GuideHelp.Instance.IsBuidMoreButtonExpand() != moreButtonIsOpen)
            return false;

        if (GuideHelp.Instance.GetPetNumberWithLevelMore(1) >= 1 && GuideHelp.Instance.GetPetSkilledTimes() == 0)
            return true;
        else
            return false;
    }

	static bool guide12IsOk(bool moreButtonIsOpen)
	{
		if (GuideHelp.Instance.IsSigninAutoOpen())
			return false;
		
		if (GuideHelp.Instance.IsBuidMoreButtonExpand() != moreButtonIsOpen && GuideHelp.Instance.GetPetNumberWithLevelMore(1) >= 1 && GuideHelp.Instance.GetPetSkilledTimes() == 0)
			return true;
		else
			return false;
	}

	static bool guide13IsOk(bool moreButtonIsOpen)
	{
		if (GuideHelp.Instance.IsSigninAutoOpen())
            return false;

        if (GuideHelp.Instance.IsBuidMoreButtonExpand() != moreButtonIsOpen)
            return false;
        if (!GuideHelp.Instance.IsPetHasWear() && GuideHelp.Instance.GetCanWeaponPetNumber() > 0)
            return true;
        else
            return false;
    }
	static bool guide14IsOk(bool moreButtonIsOpen)
	{
		if (GuideHelp.Instance.IsSigninAutoOpen())
			return false;
		
		if (GuideHelp.Instance.IsBuidMoreButtonExpand() != moreButtonIsOpen && !GuideHelp.Instance.IsPetHasWear() && GuideHelp.Instance.GetCanWeaponPetNumber() > 0)
			return true;
		else
			return false;
	}
	static bool guide15IsOk()
	{
		if (GuideHelp.Instance.IsSigninAutoOpen())
			return false;
		if (GuideHelp.Instance.GetPlayerLevel() >= 6)
			return true;
		return false;
    }
    static bool guide16_17IsOK(bool moreButtonIsOpen)
    {
		if (GuideHelp.Instance.IsSigninAutoOpen())
			return false;
		
		if (GuideHelp.Instance.IsBuidMoreButtonExpand() != moreButtonIsOpen)
			return false;
		if (GuideHelp.Instance.GetOpenShopTimes() > 0 && GuideHelp.Instance.GetPlayerLevel() >= 8)
			return true;
		return false;
    }
    static bool guide18_19_20IsOK()
    {
		if (GuideHelp.Instance.IsSigninAutoOpen())
			return false;
		if (GuideHelp.Instance.GetStrongStoneCount() > 0)
			return true;
        return false;
    }
    static bool guide21IsOK()
    {
		if (GuideHelp.Instance.IsSigninAutoOpen())
			return false;
		if (GuideHelp.Instance.GetInstance1SuccCount() > 0 && GuideHelp.Instance.GetSignTimesThisMonth() == 0)
			return true;
        return false;
    }
}
