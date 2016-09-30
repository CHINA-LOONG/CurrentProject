using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PetSwitchPage : MonoBehaviour
{
    int petToBeReplace;
    public List<Transform> itemPos = new List<Transform>();
    public List<PetSwitchItem> items = new List<PetSwitchItem>();
    RectTransform trans;
    int itemPosCount = 0;

    void Awake()
    {
        trans = transform as RectTransform;
        itemPosCount = itemPos.Count;
        //if (BattleConst.maxFieldUnit>itemPos.Count)
        //{
        //    Logger.LogError("error:pet count ");
        //}
        for (int i = 0; i < itemPosCount; ++i)
        {
            var item = ResourceMgr.Instance.LoadAsset("petItem");
            item.transform.SetParent(itemPos[i].transform, false);
            var itemTrans = item.GetComponent<RectTransform>();
            //itemTrans.anchoredPosition += new Vector2(100 * i, 0);
            var com = item.GetComponent<PetSwitchItem>();

            items.Add(com);
        }
    }

    public void Show(ShowSwitchPetUIArgs args)
    {
        //如果要显示的和当前打开的一样，则隐藏
        if (Hide(args.targetId))
            return;

        gameObject.SetActive(true);

        petToBeReplace = args.targetId;
        RefreshPetItems();

        BattleObject targetGO = ObjectDataMgr.Instance.GetBattleObject(args.targetId);
        Transform targetTrans = targetGO.transform;
        GameObject headNode = Util.FindChildByName(targetGO.gameObject, BattleConst.headNode);
        if (headNode != null)
        {
            targetTrans = headNode.transform;
        }
        Vector3 pt = BattleCamera.Instance.CameraAttr.WorldToScreenPoint(targetTrans.position);
        float scale = UIMgr.Instance.CanvasAttr.scaleFactor;
        trans.anchoredPosition = new Vector2(pt.x / scale, pt.y / scale);

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

    public void ForceRefresh()
    {
        RefreshPetItems(true);
    }

    void Update()
    {
        //TODO: no need to update, only refresh when show
        if (gameObject.activeSelf)
        {
            RefreshPetItems(false);
        }
    }

    void RefreshPetItems(bool forceRefresh = false)
    {
        var idleUnits = BattleController.Instance.BattleGroup.PlayerIdleList;
        int j = 0;
        GameUnit gameUnit;
        for (int i = 0; i < idleUnits.Count; ++i)
        {
            gameUnit = idleUnits[i];
            if (gameUnit != null &&
                gameUnit.backUp == true
                )
            {
                items[j].targetId = petToBeReplace;
                items[j].UpdateData(gameUnit, forceRefresh);
                ++j;
            }
        }

        for (; j < itemPosCount; ++j)
        {
            items[j].UpdateData(null, forceRefresh);
        }
    }
}
