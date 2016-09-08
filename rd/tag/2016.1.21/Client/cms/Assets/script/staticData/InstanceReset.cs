using UnityEngine;
using System.Collections;

public class InstanceReset
{
    public string id;
    public int maxTimes;
    public int consume;
    public int  consumeAdd;
    public int doubleTimes;

    public int GetBaseZuanshiWithTime(int iTime)
    {
        int baseConsum = consume + (iTime - 1) * consumeAdd;
        int power = (int)System.Math.Floor((iTime - 1) / (float)doubleTimes);
        int result = (int)(baseConsum * Mathf.Pow(2, power));

        return result;
    }

}
