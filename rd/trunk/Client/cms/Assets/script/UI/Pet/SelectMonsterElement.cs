using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectMonsterElement : MonoBehaviour {

    public Text nameLabel;
    public Text characterLabel;
    public Text battleLabel;
    public GameObject maskView;
    public GameObject eventObject;
    public MonsterIcon avatar;

    [HideInInspector]
    public int guid;

    [HideInInspector]
    public bool showMask
    {
        set
        {
            maskView.SetActive(value);
        }
    }

    private bool isSelect = false;
    [HideInInspector]
    public bool select
    {
        get
        {
            return isSelect;
        }
        set
        {
            avatar.ShowSelectImage(value);
            isSelect = value;
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ReloadData(GameUnit unit, bool isSelect)
    {
        this.select = isSelect;
        guid = unit.pbUnit.guid;
        nameLabel.text = unit.name;
        characterLabel.text = unit.character.ToString();
        battleLabel.text = unit.attackCount.ToString();

        avatar.Init();
        avatar.SetMonsterStaticId(unit.pbUnit.id);
        avatar.SetStage(unit.pbUnit.stage);
        avatar.SetLevel(unit.pbUnit.level);
        avatar.iconButton.gameObject.SetActive(false);
        avatar.ShowSelectImage(isSelect);
    }
}
