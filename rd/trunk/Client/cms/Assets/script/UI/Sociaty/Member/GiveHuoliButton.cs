using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GiveHuoliButton : MonoBehaviour
{
    public Button itemButton;
    public Transform giveFinish;
    // Use this for initialization

    public void SetIsSend(bool isSendFinish)
    {
        itemButton.gameObject.SetActive(!isSendFinish);
        giveFinish.gameObject.SetActive(isSendFinish);
    }

    public void HideAll()
    {
        itemButton.gameObject.SetActive(false);
        giveFinish.gameObject.SetActive(false);
    }
}
