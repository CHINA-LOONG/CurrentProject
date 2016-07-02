using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PetSwitchPage : MonoBehaviour
{
    public List<PetSwitchItem> items;

    public void Show(ShowSwitchPetUIArgs args)
    {
        int i = 0;
        var prefab = ResourceMgr.Instance.LoadAsset("ui/battle", "petItem");
        foreach (var unit in args.idleUnits)
        {
            var item = Instantiate(prefab);
            item.transform.SetParent(gameObject.transform, false);

            var trans = item.GetComponent<RectTransform>();
            trans.anchoredPosition += new Vector2(100*i++, 0);

            var com = item.GetComponent<PetSwitchItem>();
            com.Show(args.targetId, unit);

            items.Add(com);
        }

        gameObject.SetActive(true);
        var targetGO = BattleController.Instance.BattleGroup.GetUnitByGuid(args.targetId);
        transform.position = RectTransformUtility.WorldToScreenPoint(BattleCamera.Instance.CameraAttr, targetGO.gameObject.transform.position);
        transform.position += new Vector3(0, 200, 0);
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
