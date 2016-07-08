using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AttrElement : MonoBehaviour {

    public Text nameLabel;
    public Text valueLabel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetAttr(string name, string value)
    {
        nameLabel.text = name;
        valueLabel.text = value;
    }
}
