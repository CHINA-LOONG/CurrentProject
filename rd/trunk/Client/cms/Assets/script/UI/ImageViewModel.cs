using UnityEngine;
using System.Collections;

public class ImageViewModel : MonoBehaviour {


    public static int Count = 0;

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


    public static ImageViewModel CreateModel()
    {
        ImageViewModel model = null;
        GameObject go = ResourceMgr.Instance.LoadAsset("ImageViewModel");
        UIMgr.Instance.SetModelView(go.transform, Count++);
        model = go.GetComponent<ImageViewModel>();
        return model;
    }

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
        GameUnit gainPet = GameUnit.CreateFakeUnit(BattleConst.enemyStartID-Count-10, monsterId);
        curModel = ObjectDataMgr.Instance.CreateBattleObject(
                            gainPet,
                            modelPos,
                            Vector3.zero,
                            Quaternion.identity
                            );
        curMonsterId = monsterId;

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
