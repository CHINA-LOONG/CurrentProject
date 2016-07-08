using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PetDetailLeft : MonoBehaviour {

    public Text     name;
    public Text     zhanli;
    public Text     level;
    public Text     property;
    public Slider   expProgress;
    public Text     expContent;

    public Text     health;
    public Text     strength;
    public Text     defense;
    public Text     intellgence;
    public Text     speed;

    public RawImage modelImage;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ReloadData(GameUnit unit)
    {     
        GameObject petCamera = GameObject.Find("petCamera");
        
        GameObject monster = Util.FindChildByName(petCamera, "petModel");
        if (monster != null)
        {
            ResourceMgr.Instance.DestroyAsset(monster);
            monster = null; 
        }

        monster = ResourceMgr.Instance.LoadAsset("monster", unit.assetID);
        monster.transform.SetParent(petCamera.transform);
        monster.name = "petModel";
        monster.transform.localPosition = new Vector3(0, -0.5f, 1.5f);
        monster.transform.localScale = Vector3.one;
        monster.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, -180, transform.localEulerAngles.z);

        modelImage.texture = petCamera.GetComponent<Camera>().targetTexture;

        name.text = unit.name + "+" + unit.pbUnit.starLevel;
        level.text = "Lv." + unit.pbUnit.level;
        property.text = "属性:" + Util.ConvertProperty(unit.property);
        int maxLevel =  StaticDataMgr.Instance.GetPlayerLevelAttr(unit.pbUnit.level).exp;
        expContent.text = unit.pbUnit.curExp + "/" + maxLevel;
        expProgress.value = unit.pbUnit.curExp * 1.0f /  maxLevel;

        health.text = unit.health.ToString();
        strength.text = unit.strength.ToString();
        defense.text = unit.defense.ToString();
        intellgence.text = unit.intelligence.ToString();
        speed.text = unit.speed.ToString();
    }
}
