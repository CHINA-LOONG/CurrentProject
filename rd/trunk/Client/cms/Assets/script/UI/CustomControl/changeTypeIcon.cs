using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class changeTypeIcon : MonoBehaviour
{
    public Image backGround;
    public Image frameImage;
    public Image itemImage;

    public Text itemCountText;
    public Button iconButton;

    public PB.changeType type;
    public int count;

    public static changeTypeIcon CreateIcon(PB.changeType type, int count)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("changeTypeIcon");
        if (go != null)
        {
            changeTypeIcon icon = go.GetComponent<changeTypeIcon>();
            icon.RefreshWithInfo(type, count);
            return icon;
        }
        else
        {
            return null;
        }
    }


    public void RefreshWithInfo(PB.changeType type, int count)
    {
        this.type = type;
        this.count = count;

        SetIconImage(type);
        SetFrameImage(1);
        SetCountText(count);
    }

    private void SetIconImage(PB.changeType type)
    {
        string iconName = "";
        switch (type)
        {
            case PB.changeType.CHANGE_GOLD:
                iconName = "icon_zuanshi2";
                break;
            case PB.changeType.CHANGE_COIN:
                iconName = "icon_jinbi2";
                break;
            case PB.changeType.CHANGE_PLAYER_EXP:
                iconName = "icon_exp2";
                break;
            default:
                break;
        }
        Sprite iconImg = ResourceMgr.Instance.LoadAssetType<Sprite>(iconName) as Sprite;
        if (null != iconImg)
            itemImage.sprite = iconImg;
    }
    private void SetFrameImage(int stage)
    {
        string assetname = "grade_" + stage.ToString();
        Sprite headImg = ResourceMgr.Instance.LoadAssetType<Sprite>(assetname) as Sprite;
        if (null != headImg)
            frameImage.sprite = headImg;
    }
    private void SetCountText(int itemCount)
    {
        string strCount = "";
        if (itemCount > 1)
        {
            strCount = string.Format("{0}", itemCount);
        }
        itemCountText.text = strCount;
    }
}
