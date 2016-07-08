using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class rewardItemIcon : MonoBehaviour
{
    public Image img_Item;
    public Image img_Bground;
    public Text text_Count;

    void SetItem(string name, int count)
    {
        img_Item.name = name;
        text_Count.text = "*" + count;
    }

    public void SetItem(RewardItemData info, int k = 1, int b = 0)
    {
        SetItem("", info.count * k + b);
    }

}
