using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuffIcon : MonoBehaviour 
{
    private string curIcon;
    private Image buffIcon;

    public string IconName
    {
        get { return curIcon; }
    }

    void Awake()
    {
        buffIcon = gameObject.GetComponent<Image>();
        gameObject.SetActive(false);
    }

    public void ShowBuff(string icon)
    {
        if (curIcon != icon)
        {
            curIcon = icon;
            var image = ResourceMgr.Instance.LoadAssetType<Sprite>(curIcon) as Sprite;
            buffIcon.sprite = image;
        }
        gameObject.SetActive(true);
    }

    public void RemoveBuff()
    {
       gameObject.SetActive(false);
       curIcon = null;
    }

    public bool IsActive()
    {
        return gameObject.activeInHierarchy;
    }
}
