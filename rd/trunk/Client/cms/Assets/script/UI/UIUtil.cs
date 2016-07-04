using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

}
