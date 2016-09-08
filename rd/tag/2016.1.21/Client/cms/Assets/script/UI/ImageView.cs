using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class ImageView : MonoBehaviour,IBeginDragHandler, 
                                       IEndDragHandler, 
                                       IDragHandler
{
    [HideInInspector]
    public ImageViewModel model;

    [HideInInspector]
    public JidiPositionViewModel jidiPositionViewModel;

    private RawImage image;
    
    public BattleObject ReloadData(string monsterId)
    {
        if (model==null)
        {
            model = ImageViewModel.CreateModel();
            image = GetComponent<RawImage>();
            model.Camera.targetTexture = image.texture as RenderTexture;
        }
        return model.ReloadData(monsterId);
    }
    public void InitJidiPosoitonModel()
    {
        if (model == null)
        {
            model = ImageViewModel.CreateJidiPositionModel();
            image = GetComponent<RawImage>();
            model.Camera.targetTexture = image.texture as RenderTexture;
            jidiPositionViewModel = model.GetComponent<JidiPositionViewModel>();
        }
    }
    public BattleObject ReloadJidiPositionData(string monsterId)
    {
        InitJidiPosoitonModel();
        return model.ReloadData(monsterId);
    }
    public void CleanImageView()
    {
        if (model!=null)
        {
            model.DestroyModel();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (model!=null)
        {

        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (model!=null)
        {
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (model!=null)
        {
            model.UpdateTurn(eventData.delta);
        }
    }
}
