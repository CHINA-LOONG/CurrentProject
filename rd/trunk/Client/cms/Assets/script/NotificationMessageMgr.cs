//================================
//  Create by xuelong.
//  Mail:[xuelong@way4games.com]
//================================

using UnityEngine;
using System.Collections;
using System;

#if UNITY_IPHONE
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;
using LocalNotification = UnityEngine.iOS.LocalNotification;

#endif
public class NotificationMessageMgr : MonoBehaviour
{
    public enum ENotificationID
    {
        Push_LingHuoLi_1 = 10,
        Push_LingHuoLi_2,
        Push_LingHuoLi_3,

        Push_GuildTask_1 = 20,

        Push_TongTianTa_1 = 30,
        Push_TongTianTa_2,

        Push_PVP = 40,

        Push_Adventure_1 = 50,
        Push_Adventure_2,
        Push_Adventure_3,
        Push_Adventure_4,
        Push_Adventure_5,
        Push_Adventure_6,
        Push_Adventure_7,

        Push_Summon = 60,

        Push_Shop_1 = 70,
        Push_Shop_2,

        Push_HuoLi_1 = 80,
        Push_Long_1,
        Push_Long_2,
    }
    
    public static void NotificationMessage(ENotificationID id, string title, string message, int hour, bool isRepeatDay)
    {
        int year = DateTime.Now.Year;
        int month = DateTime.Now.Month;
        int day = DateTime.Now.Day;
        DateTime dateTime = new DateTime(year, month, day, hour, 0, 0);
        NotificationMessage(id, title, message, dateTime, isRepeatDay);
    }
    public static void NotificationMessage(ENotificationID id, string title, string message, DateTime dateTime, bool isRepeatDay)
    {
        if (dateTime < DateTime.Now)
        {
            if (isRepeatDay)
            {
                dateTime.AddDays(1);
            }
            else
            {
                return;
            }
        }

#if UNITY_IPHONE&&!UNITY_EDITOR
            LocalNotification localNotification = new LocalNotification();
            localNotification.fireDate = dateTime;
            localNotification.hasAction = true;
            localNotification.alertAction = title;
            localNotification.alertBody = message;
            localNotification.applicationIconBadgeNumber = 1;
            if (isRepeatDay)
            {
                localNotification.repeatCalendar = UnityEngine.iOS.CalendarIdentifier.ChineseCalendar;
                localNotification.repeatInterval = UnityEngine.iOS.CalendarUnit.Day;
            }
            localNotification.soundName = LocalNotification.defaultSoundName;
            NotificationServices.ScheduleLocalNotification(localNotification);
#elif UNITY_ANDROID&&!UNITY_EDITOR
        long delay = (long)(dateTime - DateTime.Now).TotalSeconds;
        if (isRepeatDay)
        {
            AndroidLocationNotification.SendRepeatingNotification((int)id, delay, (24 * 60 * 60), title, message, new Color32());
        }
        else
        {
            AndroidLocationNotification.SendNotification((int)id, delay, title, message, new Color32());
        }
#endif
    }
    /// <summary>
    /// 取消全部
    /// </summary>
    void CleanNotification()
    {
#if UNITY_IPHONE&&!UNITY_EDITOR
        LocalNotification localNotification = new LocalNotification();
        localNotification.applicationIconBadgeNumber = -1;
        NotificationServices.PresentLocalNotificationNow(localNotification);
        NotificationServices.CancelAllLocalNotifications();
        NotificationServices.ClearLocalNotifications();
#elif UNITY_ANDROID&&!UNITY_EDITOR
        int nums = Enum.GetValues(typeof(ENotificationID)).Length;
        for (int i = 0; i < nums; i++)
        {
            ENotificationID Id = (ENotificationID)Enum.GetValues(typeof(ENotificationID)).GetValue(i);
            AndroidLocationNotification.CancelNotification((int)Id);
        }
#endif
    }

    void Awake()
    {
        CleanNotification();
    }
    void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            OpenAllNotification();
        }
        else
        {
            CleanNotification();
        }
    }

    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(Screen.width/4,Screen.height*7/8,Screen.width/2,Screen.height/8),"test10seconds"))
    //    {
    //        NotificationMessage(ENotificationID.Push_HuoLi_1, "1111111", StaticDataMgr.Instance.GetTextByID("push_huoli"), DateTime.Now.AddSeconds(15), false);
    //    }
    //}

    void OpenAllNotification()
    {
        //1.任务
        NotificationMessage(ENotificationID.Push_LingHuoLi_1, "", StaticDataMgr.Instance.GetTextByID("push_linghuoli"), 12, true);
        NotificationMessage(ENotificationID.Push_LingHuoLi_2, "", StaticDataMgr.Instance.GetTextByID("push_linghuoli"), 18, true);
        NotificationMessage(ENotificationID.Push_LingHuoLi_3, "", StaticDataMgr.Instance.GetTextByID("push_linghuoli"), 21, true);
        //2.公会
        //NotificationMessage(ENotificationID.Push_GuildTask_1, "", StaticDataMgr.Instance.GetTextByID("push_guildtask1"), DateTime.Now.AddSeconds(0), false);
        //3.通天塔
        {
            DateTime nextMonth = DateTime.Now.AddMonths(1);
            DateTime RefreshTime = new DateTime(nextMonth.Year, nextMonth.Month, 1, 0, 0, 0);
            NotificationMessage(ENotificationID.Push_TongTianTa_1, "", StaticDataMgr.Instance.GetTextByID("push_tongtianta1"), RefreshTime.AddHours(8), false);
            NotificationMessage(ENotificationID.Push_TongTianTa_2, "", StaticDataMgr.Instance.GetTextByID("push_tongtianta2"), RefreshTime.AddHours(27), false);
        }
        //4.PVP
        //NotificationMessage(ENotificationID.Push_PVP, "", StaticDataMgr.Instance.GetTextByID("push_pvp"), DateTime.Now.AddSeconds(0), false);
        //5.大冒险
        ENotificationID AdventureID = ENotificationID.Push_Adventure_1;
        for (int i = 0; i < AdventureDataMgr.Instance.adventureTeams.Count; i++)
        {
            AdventureTeam team = AdventureDataMgr.Instance.adventureTeams[i];
            if (team.adventure!=null&&team.adventure.timeEvent!=null)
            {
                NotificationMessage(AdventureID, "", StaticDataMgr.Instance.GetTextByID("push_advanture"), DateTime.Now.AddSeconds(team.adventure.timeEvent.second), false);
                AdventureID++;
            }
        }
        //6.抽蛋
        //NotificationMessage(ENotificationID.Push_Summon, "", StaticDataMgr.Instance.GetTextByID("push_summon"), DateTime.Now.AddSeconds(0), false);
        //7.商店
        NotificationMessage(ENotificationID.Push_Shop_1, "", StaticDataMgr.Instance.GetTextByID("push_shop"), 13, true);
        NotificationMessage(ENotificationID.Push_Shop_2, "", StaticDataMgr.Instance.GetTextByID("push_shop"), 19, true);
        //8.其他
        //NotificationMessage(ENotificationID.Push_HuoLi_1, "", StaticDataMgr.Instance.GetTextByID("push_huoli"), DateTime.Now.AddSeconds(0), false);
        NotificationMessage(ENotificationID.Push_Long_1, "", StaticDataMgr.Instance.GetTextByID("push_long"), DateTime.Now.AddHours(22), false);
        NotificationMessage(ENotificationID.Push_Long_2, "", StaticDataMgr.Instance.GetTextByID("push_long"), DateTime.Now.AddHours(46), false);

    }


}
