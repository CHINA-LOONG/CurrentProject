using UnityEngine;
using System.Collections;

public class TimeStaticData
{
    public int type;
    public int minute;
    public int hour;
    public int dayOfWeek;
    public int dayOfMonth;
    public int month;
    public int year;
    public string comments;



    //此处重载运算符只判断当天  hour and minute
    public static bool operator ==(TimeStaticData a, TimeStaticData b)
    {
        if ((a as object) != null && (b as object) != null)
        {
            if (a.hour == b.hour && a.minute == b.minute)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if ((a as object) == null && (b as object) == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool operator !=(TimeStaticData a, TimeStaticData b)
    {
        return !(a == b);
    }

    public static bool operator >(TimeStaticData a, TimeStaticData b)
    {
        if (a.hour==b.hour)
        {
            if (a.minute>b.minute)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (a.hour>b.hour)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool operator <(TimeStaticData a, TimeStaticData b)
    {
        if (a > b || a == b)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public static bool operator <=(TimeStaticData a, TimeStaticData b)
    {
        if (a > b)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public static bool operator >=(TimeStaticData a, TimeStaticData b)
    {
        if (a > b||a == b)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
