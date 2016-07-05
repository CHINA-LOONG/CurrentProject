using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PetSwitchPage : MonoBehaviour
{
    int petToBeReplace;
    public List<PetSwitchItem> items;
    RectTransform trans;

    void Awake()
    {
        trans = transform as RectTransform;
    }
    /// <summary>
    /// 创建UI item，创建三个，只需要创建一次
    /// </summary>
    void CreateItems()
    {
        if (items.Count != 3)
        {
            var prefab = ResourceMgr.Instance.LoadAsset("ui/battle", "petItem");

            for (int i = 0; i < 3; i++)
            {
                var item = Instantiate(prefab);
                item.transform.SetParent(gameObject.transform, false);

                var trans = item.GetComponent<RectTransform>();
                trans.anchoredPosition += new Vector2(100 * i, 0);

                var com = item.GetComponent<PetSwitchItem>();

                items.Add(com);
            }
        }
    }

    public void Show(ShowSwitchPetUIArgs args)
    {
        //如果要显示的和当前打开的一样，则隐藏
        if (Hide(args.targetId))
            return;

        gameObject.SetActive(true);

        //查看是否需要创建UI items
        CreateItems();

        petToBeReplace = args.targetId;
        var idleUnits = BattleController.Instance.BattleGroup.PlayerIdleList;

        for (int i = 0; i < items.Count; i++)
        {
            var com = items[i];
            if (i < idleUnits.Count)
                com.Show(args.targetId, idleUnits[i]);
            else
                com.ShowEmpty(false);
        }


        BattleObject targetGO = ObjectDataMgr.Instance.GetBattleObject(args.targetId);
        Vector3 pt = BattleCamera.Instance.CameraAttr.WorldToScreenPoint(targetGO.gameObject.transform.position);
        float scale = UIMgr.Instance.CanvasAttr.scaleFactor;
        //TODO; change to child node, when gd changes the prefab
        trans.anchoredPosition = new Vector2(pt.x / scale, pt.y / scale + 100/scale);

        //var targetGO = ObjectDataMgr.Instance.GetBattleObject(args.targetId);
        //var viewPos = BattleCamera.Instance.CameraAttr.WorldToViewportPoint(targetGO.gameObject.transform.position);
        //var pos = UICamera.Instance.CameraAttr.ViewportToScreenPoint(viewPos);
        //var trans = transform as RectTransform;
        //trans.anchoredPosition = pos;
        //trans.anchoredPosition += new Vector2(0, Screen.height * 0.4f);
    }

    public bool Hide(int id)
    {
        if (id == BattleConst.closeSwitchPetUI || id == petToBeReplace)
        {
            if (gameObject.activeSelf)
                gameObject.SetActive(false);

            petToBeReplace = 0;
            return true;
        }

        return false;
    }

    void Update()
    {
        //TODO: no need to update, only refresh when show
        if (gameObject.activeSelf)
        {
            var idleUnits = BattleController.Instance.BattleGroup.PlayerIdleList;
            for (int i = 0; i < items.Count; i++)
            {
                var com = items[i];
                if (i < idleUnits.Count)
                    com.UpdateData(idleUnits[i]);
            }
        }
    }
}
