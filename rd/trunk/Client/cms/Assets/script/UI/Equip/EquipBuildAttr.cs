using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EquipBuildAttr : MonoBehaviour
{
    public Text text_Attr;
    public Text textAttr;

    public Text textNextAttr;
    public Text textAddAttr;

    public Image imgAdd;

    public void Refresh(AttrType type, int attr, int nextAttr)
    {
        textAttr.text = attr.ToString();
        textNextAttr.text = nextAttr.ToString();

        imgAdd.gameObject.SetActive(nextAttr - attr > 0);
        textAddAttr.gameObject.SetActive(nextAttr - attr > 0);
        textAddAttr.text = (nextAttr - attr).ToString();

        switch (type)
        {
            case AttrType.Strength:
                text_Attr.text = StaticDataMgr.Instance.GetTextByID("common_attr_strenth");
                break;
            case AttrType.Health:
                text_Attr.text = StaticDataMgr.Instance.GetTextByID("common_attr_health");
                break;
            case AttrType.Intelligence:
                text_Attr.text = StaticDataMgr.Instance.GetTextByID("common_attr_intelligence");
                break;
            case AttrType.Defense:
                text_Attr.text = StaticDataMgr.Instance.GetTextByID("common_attr_defence");
                break;
            case AttrType.Speed:
                text_Attr.text = StaticDataMgr.Instance.GetTextByID("common_attr_speed");
                break;
            default:
                text_Attr.text = "other";
                break;
        }
    }

}
