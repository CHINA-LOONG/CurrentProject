using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NullEquip : MonoBehaviour
{
    public static string AssetName = "NullEquip";

    public Button button;
    public Image addEquip;

    public void ShowAdd(bool showadd)
    {
        gameObject.SetActive(showadd);
    }
}
