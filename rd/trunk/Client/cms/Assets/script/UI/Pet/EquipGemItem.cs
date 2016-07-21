using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EquipGemItem : MonoBehaviour
{
    public Image imgIcon;

    public void Refresh(GemInfo info)
    {
        //TODO;
        if (string.IsNullOrEmpty(info.gemId))
        {
            switch (info.type)
            {
                case 1:
                    imgIcon.color = Color.red;
                    break;
                case 2:
                    imgIcon.color = Color.green;
                    break;
                case 3:
                    imgIcon.color = Color.blue;
                    break;
                default:
                    Logger.LogError("error: gemtype error;");
                    break;
            }
        }
        else
        {
            //TODO:
            imgIcon.color = Color.black;
        }
    }

}
