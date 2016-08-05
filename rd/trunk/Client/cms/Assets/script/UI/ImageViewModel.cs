using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ImageViewModel : MonoBehaviour {

    public static int Count = 0;
    public static ImageViewModel CreateModel()
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("ImageViewModel");
        UIMgr.Instance.SetModelView(go.transform, Count);
        ImageViewModel model = go.GetComponent<ImageViewModel>();
        model.index = Count;
        Count++;
        return model;
    }
    public int index;
    public Transform verticalTurn;
    public Transform herizontalTurn;

    public GameObject modelPos;

    private Camera curCamera;
    public Camera Camera
    {
        get
        {
            if (curCamera == null)
                curCamera = GetComponentInChildren<Camera>();
            return curCamera;
        }
    }

    private string curMonsterId = "";
    private BattleObject curModel;



    public BattleObject ReloadData(string monsterId)
    {
        if (string.Equals(curMonsterId,monsterId))
        {
            return curModel;
        }
        if (curModel != null)
        {
            ObjectDataMgr.Instance.RemoveBattleObject(curModel.guid);
        }
        GameUnit gainPet = GameUnit.CreateFakeUnit(BattleConst.enemyStartID-index-10, monsterId);
        curModel = ObjectDataMgr.Instance.CreateBattleObject(
                            gainPet,
                            modelPos,
                            Vector3.zero,
                            Quaternion.identity
                            );
        curMonsterId = monsterId;
        ReSetTurn();
        return curModel;
    }

    /// <summary>
    /// 只在销毁界面时调用
    /// </summary>
    public void DestroyModel()
    {
        ObjectDataMgr.Instance.RemoveBattleObject(curModel.guid);
        curModel = null;
        curMonsterId = "";
        Destroy(this.gameObject);
    }

    public void ReSetTurn()
    {
        verticalTurn.localEulerAngles = Vector3.zero;
        herizontalTurn.localEulerAngles = Vector3.zero;
    }
    public void UpdateTurn(Vector2 delta)
    {
        Vector3 angle = herizontalTurn.localEulerAngles;
        angle.y -= delta.x * 0.2f;
        herizontalTurn.localEulerAngles = angle;

        angle = verticalTurn.localEulerAngles;
        angle.x = angle.x < 180f ? 360f + angle.x : angle.x;
        angle.x += delta.y * 0.2f;
        angle.x = Mathf.Clamp(angle.x, 335f, 360f);
        verticalTurn.localEulerAngles = angle;
    }



}
