using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AttrElement : MonoBehaviour {

    public Text nameLabel;
    public Text valueLabel;


    public void SetAttr(string name, string value)
    {
        nameLabel.text = name;
        valueLabel.text = value;
    }
}
