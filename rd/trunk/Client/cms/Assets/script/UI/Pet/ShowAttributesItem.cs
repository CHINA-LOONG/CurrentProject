using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowAttributesItem : MonoBehaviour
{
    public Text textName;
    public Text textValue;

    public GameObject objChange;
    public Text textChange;

    public void SetValue(string name, int value, int change, int size = 18)
    {
        textName.text = name;
        textValue.text = value.ToString();
        textChange.text = change.ToString();
        objChange.SetActive(change != 0);

        textName.fontSize = size;
        textValue.fontSize = size;
        textChange.fontSize = size;
    }


}
