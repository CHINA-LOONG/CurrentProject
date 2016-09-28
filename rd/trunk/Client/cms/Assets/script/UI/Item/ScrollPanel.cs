using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollPanel : MonoBehaviour {

    public void AddContent(string assetname, int count)
    {
        var go = ResourceMgr.Instance.LoadAsset(assetname);
        for (int i = 0; i < count; ++i)
        {
            GameObject bagItem = GameObject.Instantiate(go);
            if (bagItem != null)
            {
                bagItem.transform.SetParent(gameObject.transform, false);
                bagItem.transform.localScale = Vector3.one;
                bagItem.transform.localPosition = Vector3.zero;
            }
        }

        //float cellX = gameObject.GetComponent<GridLayoutGroup>().cellSize.x;
        //float cellY = gameObject.GetComponent<GridLayoutGroup>().cellSize.y;
        //float spacingX = gameObject.GetComponent<GridLayoutGroup>().spacing.x;
        //float spacingY = gameObject.GetComponent<GridLayoutGroup>().spacing.y;

        //// 设置padding为spacing一半大小
        //gameObject.GetComponent<GridLayoutGroup>().padding = new RectOffset((int)spacingX / 2, (int)spacingX / 2, (int)spacingY / 2, (int)spacingY / 2);
        //float width = gameObject.GetComponent<RectTransform>().rect.width;
        //float height = gameObject.GetComponent<RectTransform>().rect.height;
        //int capacityPerLine = (int)(width / (cellX + spacingX));

        //int rowCount = (int)((gameObject.transform.childCount + capacityPerLine - 1) / capacityPerLine);
        //float totalHeight = rowCount * (cellY + spacingY);

       // gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        //gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(0, height - totalHeight);
    }
}
