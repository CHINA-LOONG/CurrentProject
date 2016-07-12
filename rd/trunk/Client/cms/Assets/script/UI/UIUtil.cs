using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIUtil  
{

	public static Vector3 GetSpacePos(RectTransform rect, Canvas canvas, Camera camera)
	{
		if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
		{
			return rect.position;
		}
		return camera.WorldToScreenPoint(rect.position);
		
	}

	public static void GetSpaceCorners(RectTransform rect, Canvas canvas, Vector3[] corners,Camera camera)
	{
		if (camera == null)
		{
			camera = Camera.main;
		}
		rect.GetWorldCorners(corners);
		if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
		{
			
		}
		else
		{
			for (var i = 0; i < corners.Length; i++)
			{
				corners[i] = camera.WorldToScreenPoint(corners[i]);
			}
		}
	}

	public static Rect GetSpaceRect(Canvas canvas, RectTransform rect, Camera camera)
	{
		Rect spaceRect = rect.rect;
		Vector3 spacePos = GetSpacePos(rect, canvas, camera);
		//lossyScale
		spaceRect.x = spaceRect.x * rect.lossyScale.x + spacePos.x;
		spaceRect.y = spaceRect.y * rect.lossyScale.y + spacePos.y;
		spaceRect.width = spaceRect.width * rect.lossyScale.x;
		spaceRect.height = spaceRect.height * rect.lossyScale.y;
		return spaceRect;
	}
	
	public static bool RectContainsScreenPoint(Vector3 point, Canvas canvas, RectTransform rect, Camera camera)
	{
		if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
		{
			return RectTransformUtility.RectangleContainsScreenPoint(rect, point, camera);
		}
		
		return GetSpaceRect(canvas, rect, camera).Contains(point);
	}

    //装换属性名称
    public static string ConvertProperty(int property)
    {
        if (property == SpellConst.propertyGold)
            return "金";
        else if (property == SpellConst.propertyWood)
            return "木";
        else if (property == SpellConst.propertyWater)
            return "水";
        else if (property == SpellConst.propertyFire)
            return "火";
        else if (property == SpellConst.propertyEarth)
            return "土";

        return "无";
    }

    public static bool NeedChangeGrade(int stage)
    {
        if (stage == 0 || stage == 2 || stage == 5 || stage == 9 || stage == 14)
        {
            return true;
        }

        return false;
    }

    public static void CalculationQuality(int stage, out int quality, out int plusQuality)
    {
        quality = 1;
        plusQuality = 0;

        if (stage < 0 || stage > 15)
            return;

        if (stage == 0)
        {
            quality = 1;
        }
        else if (stage < 3)
        {
            quality = 2;
            plusQuality = stage - 1;
        }
        else if (stage < 6)
        {
            quality = 3;
            plusQuality = stage - 3;
        }
        else if (stage < 10)
        {
            quality = 4;
            plusQuality = stage - 6;
        }
        else if (stage < 15)
        {
            quality = 5;
            plusQuality = stage - 10;
        }
        else
        {
            quality = 6;
        }
    }

    public static bool CheckIsEnoughMaterial(GameUnit unit, bool checkCoin = true)
    {
        if (unit.pbUnit.stage == GameConfig.MaxMonsterStage)
        {
            return false;
        }

        if (CheckIsEnoughLevel(unit) == false)
        {
            return false;
        }

        UnitStageData unitStageData = StaticDataMgr.Instance.getUnitStageData(unit.pbUnit.stage + 1);

        ItemData itemData = null;
        itemData = GameDataMgr.Instance.PlayerDataAttr.gameItemData.getItem(unitStageData.demandItemList[0].itemId);
        if (itemData == null || itemData.count < unitStageData.demandItemList[0].count)
        {
            return false;
        }

        itemData = GameDataMgr.Instance.PlayerDataAttr.gameItemData.getItem(unitStageData.demandItemList[1].itemId);
        if (itemData == null || itemData.count < unitStageData.demandItemList[1].count)
        {
            return false;
        }
    
        //金钱判断
        if (checkCoin == true && GameDataMgr.Instance.PlayerDataAttr.coin <= unitStageData.demandCoin)
        {
            return false;
        }


        if (NeedChangeGrade(unit.pbUnit.stage) == false)
        {
            return true;
        }

        ItemInfo itemInfo = null;
        List<GameUnit> petList = null;
        itemInfo = unitStageData.demandMonsterList[0];
        petList = GameDataMgr.Instance.PlayerDataAttr.GetAllPet(itemInfo.itemId, itemInfo.stage);
        petList.Remove(unit);
        if (petList.Count < unitStageData.demandMonsterList[0].count)
        {
            return false;
        }

        itemInfo = unitStageData.demandMonsterList[1];
        petList = GameDataMgr.Instance.PlayerDataAttr.GetAllPet(itemInfo.itemId, itemInfo.stage);
        petList.Remove(unit);
        if (petList.Count < unitStageData.demandMonsterList[1].count)
        {
            return false;
        }

        return true;
    }

    public static bool CheckIsEnoughLevel(GameUnit unit)
    {
        if (unit.pbUnit.stage == GameConfig.MaxMonsterStage)
        {
            return true;
        }

        UnitStageData unitStageData = StaticDataMgr.Instance.getUnitStageData(unit.pbUnit.stage + 1);
        return unit.pbUnit.level >= unitStageData.demandLevel;
    }

    public static void SetStageColor(Text label, GameUnit unit)
    {
        int quallity = 0;
        int plusQuality = 0;
        UIUtil.CalculationQuality(unit.pbUnit.stage, out quallity, out plusQuality);  
      
        if (quallity == 1)
        {
            label.color = Color.white;
        }
        else if (quallity == 2)
        {
            label.color = Color.green;
        }
        else if (quallity == 3)
        {
            label.color = Color.blue;
        }
        else if (quallity == 4)
        {
            label.color  = new UnityEngine.Color(1.0f, 0.0f, 1.0f);
        }
        else if (quallity == 5)
        {
            label.color = new UnityEngine.Color(1.0f, 165 / 255.0f, 0.0f); ;
        }
        else if (quallity == 6)
        {
            label.color = Color.red;
        }
        else
        {
             label.color = Color.black;
        }

        if (plusQuality == 0)
        {
            label.text = unit.name;
        }
        else
        {
            label.text = string.Format("{0} +{1}", StaticDataMgr.Instance.GetUnitRowData(unit.pbUnit.id).NickNameAttr, plusQuality);
        }

    }
}
