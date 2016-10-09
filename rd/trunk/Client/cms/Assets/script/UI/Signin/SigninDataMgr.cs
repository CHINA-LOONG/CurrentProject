//================================
//  Create by xuelong.
//  Mail:[xuelong@way4games.com]
//================================


using System;
using System.Collections.Generic;

public class SigninDataMgr
{
    private static SigninDataMgr inst;
    public static SigninDataMgr Instance
    {
        get
        {
            if (inst == null)
            {
                inst = new SigninDataMgr();
            }
            return inst;
        }
    }

    public const int maxCount = 30;
    public bool isPopup = false;//首次登陆弹出

    private int month;
    private int day;
    public int CurMonth
    {
        get { return month; }
        set
        {
            month = value;
            SigninList = GetCurSigninList(month);
        }
    }
    public int CurDay
    {
        get { return day; }
        set { day = value; }
    }

    // 本月签到次数（包括补签）
    public int signinTimesMonthly;
	// 本月补签次数
    public int signinFillTimesMonthly;
	// 今日是否已签到
    public bool isSigninDaily;
    //今日登陆次数
    public int loginTimesDaily = 0;

    private List<string> signinList;
    //当月签到列表
    public List<string> SigninList
    {
        get
        {
            if (signinList == null)
            {
                signinList = GetCurSigninList(CurMonth);
            }
            return signinList;
        }
        set
        {
            signinList = value;
        }
    }

    List<string> GetCurSigninList(int month)
    {
        List<SigninData> list = StaticDataMgr.Instance.GetSigninDataByMonth(month);
        List<string> signinList = new List<string>();
        if (list != null)
        {
            SigninData data;
            for (int i = 0; i < list.Count; i++)
            {
                data = list[i];
                if (!string.IsNullOrEmpty(data.col1))
                    signinList.Add(data.col1);
                if (!string.IsNullOrEmpty(data.col2))
                    signinList.Add(data.col2);
                if (!string.IsNullOrEmpty(data.col3))
                    signinList.Add(data.col3);
                if (!string.IsNullOrEmpty(data.col4))
                    signinList.Add(data.col4);
                if (!string.IsNullOrEmpty(data.col5))
                    signinList.Add(data.col5);
                if (!string.IsNullOrEmpty(data.col6))
                    signinList.Add(data.col6);
            }
        }
        if (signinList.Count > 30 || signinList.Count < 28)
        {
            Logger.LogError("配置可能存在异常");
        }
        return signinList;
    }

    //可补签次数
    public int canSigninFillTimes
    {
        get
        {
            return Math.Min(CurDay, SigninList.Count) - signinTimesMonthly - (isSigninDaily ? 0 : 1);
        }
    }

    public void ReloadDateInfo(int signinTimes, int signinFillTimes, bool isSignin, int loginTimes = 0)
    {
        DateTime curDateTime = GameTimeMgr.Instance.GetServerDateTime();
        CurMonth = curDateTime.Month;

        CurDay = Math.Min(curDateTime.Day, maxCount);//最多30个奖励

        signinTimesMonthly = signinTimes;
        signinFillTimesMonthly = signinFillTimes;
        isSigninDaily = (isSignin || (signinTimesMonthly >= SigninList.Count));
        loginTimesDaily = loginTimes == 0 ? loginTimesDaily : loginTimes;
        GameEventMgr.Instance.FireEvent(GameEventList.SignInDataChange);
        GameEventMgr.Instance.FireEvent(GameEventList.SignInChange);
    }
    
}
