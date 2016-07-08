using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class rewardItemIcon : MonoBehaviour
{
    public Image img_Item;
    public Image img_Bground;
    public Text text_Count;


    public void SetItem(RewardItemData info)
    {
        img_Item.name = "";
        text_Count.text = "*" + info.count;
    }

}
