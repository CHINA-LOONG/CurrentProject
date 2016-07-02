using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PetSwitchPage : MonoBehaviour
{
    public List<PetSwitchItem> items;
    // Use this for initialization
    public void Show(ShowSwitchPetUIArgs args)
    {
        int i = 0;
        foreach (var unit in args.idleUnits)
        {
            var prefab = ResourceMgr.Instance.LoadAsset("ui/battle", "petItem");
            var item = Instantiate(prefab);
            item.transform.SetParent(gameObject.transform, false);

            var trans = item.GetComponent<RectTransform>();
            trans.anchoredPosition += new Vector2(100*i++, 0);

            var com = item.GetComponent<PetSwitchItem>();
            com.targetId = args.targetId;
            com.unit = unit;

            com.text.text = string.Format("{0}:{1}\nHP:{2}/{3}", unit.name, unit.pbUnit.guid, unit.curLife, unit.maxLife);

            items.Add(com);
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (gameObject.activeSelf)
        {
            foreach (var item in items)
            {
                Destroy(item.gameObject);
            }
            items.Clear();
            gameObject.SetActive(false);
        }
    }
}
