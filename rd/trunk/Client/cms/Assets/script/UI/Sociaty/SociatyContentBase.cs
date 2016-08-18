using UnityEngine;
using System.Collections;

public class SociatyContentBase : MonoBehaviour
{
    public virtual void RefreshUI() { }
    public  static SociatyContentBase CreateWith(SociatyContenType contentType,RectTransform parent)
    {
        GameObject go = null;
        switch(contentType)
        {
            case SociatyContenType.Infomation:
                go = ResourceMgr.Instance.LoadAsset("SociatyContentInfomation");
                break;
            case SociatyContenType.Member:
                go = ResourceMgr.Instance.LoadAsset("SociatyContentMember");
                break;
            case SociatyContenType.Technology:
                go = ResourceMgr.Instance.LoadAsset("SociatyContentTechnology");
                break;
           // case SociatyContenType.Log:
             //   go = ResourceMgr.Instance.LoadAsset("SociatyContentLog");
            //    break;
            case SociatyContenType.OtherSociaty:
                go = ResourceMgr.Instance.LoadAsset("SociatyContentOther");
                break;
        }
        go.transform.SetParent(parent);
        go.transform.localScale = new Vector3(1, 1, 1);

        RectTransform rt = go.transform as RectTransform;
        rt.anchoredPosition = new Vector2(0, 0);

        return go.GetComponent<SociatyContentBase>();
    }
}
