using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SinglePet : MonoBehaviour {

    public Image avatar;
    public Text stageLable;
    public Text nameLable;
    public Text propretyLable;
    public Text typeLable;

    [HideInInspector]
    public int guid;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReloadPatData(GameUnit unit)
    {
        nameLable.text = unit.name;
        avatar.sprite = unit.headImg;
        propretyLable.text = Util.ConvertProperty(unit.property);
    }
}
