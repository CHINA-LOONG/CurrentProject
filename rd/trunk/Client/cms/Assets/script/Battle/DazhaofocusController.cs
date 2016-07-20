using UnityEngine;
using System.Collections;

public class DazhaofocusController : MonoBehaviour 
{
	private	Transform		focusCameraTrans;
	private	Transform		focusUnitTrans;
	private BattleObject 	battleObject;

	private	Vector3 unitOldPosition;
	private	Vector3 unitOldScale;
	private	Vector3 unitOldEulerAngles;

	private Vector3 cameraOldPosition;
	private Vector3 cameraOldScale;
	private Vector3 cameraOldEulerAngles;
	private bool isMagicDazhao = false;

	public static DazhaofocusController Create(bool isMagicDazhao, string testname = "")
	{
		GameObject go = null;
		if (isMagicDazhao)
		{
            go = ResourceMgr.Instance.LoadAsset("dazhaoFocusSceneMagic");
		}
		else 
		{
			go =  ResourceMgr.Instance.LoadAsset("dazhaoFocusScene");
		}
        //NOTE: gamemain is set as don't destroy, the controller should not destroy
        go.transform.SetParent(GameMain.Instance.gameObject.transform, false);

		DazhaofocusController focus = go.AddComponent<DazhaofocusController> ();
		focus.InitWithType (isMagicDazhao);

		go.SetActive (false);

        if (string.IsNullOrEmpty(testname) == false)
            go.name = testname;

		return focus;
	}

	// Use this for initialization
	void Start ()
	{
	}

	public void InitWithType(bool isMagicDazhao)
	{
		this.isMagicDazhao = isMagicDazhao;
		focusUnitTrans = Util.FindChildByName (gameObject, "monsterPosition").transform;
		MeshRenderer tempMesh = focusUnitTrans.GetComponent<MeshRenderer> ();
		if (null != tempMesh) 
		{
			tempMesh.enabled = false;
		}
		
		focusCameraTrans = Util.FindChildByName (gameObject, "cameraPosition").transform;
		tempMesh = focusCameraTrans.GetComponent<MeshRenderer> ();
		if (null != tempMesh) 
		{
			tempMesh.enabled = false;
		}
	}
	
	public void ShowoffDazhao(BattleObject battleObj)
	{
		battleObject = battleObj;
		SetMonster ();
		SetCamera ();
		gameObject.SetActive (true);
		StartCoroutine (MonsterShowoffCo ());

		UIBattle.Instance.ShowUI (false);
	}

	IEnumerator MonsterShowoffCo()
	{
		//monstershowoff
		if (isMagicDazhao)
		{
			battleObject.TriggerEvent ("dazhaoxuanyao_wuli", Time.time, null);
		}
		else 
		{
			battleObject.TriggerEvent ("dazhaoxuanyao_fashu", Time.time, null);
		}
        if (string.IsNullOrEmpty(battleObject.unit.closeUp) == false)
        {
            battleObject.TriggerEvent(battleObject.unit.closeUp, Time.time, null);
        }

		yield return new WaitForSeconds (BattleConst.dazhaoShowOffTime);
		RestoreBattle ();
	}

	void SetMonster()
	{
		Transform monsterTrans = battleObject.transform;
		//old
		unitOldPosition = new Vector3 (monsterTrans.position.x, monsterTrans.position.y, monsterTrans.position.z);
		unitOldScale = new Vector3 (monsterTrans.localScale.x, monsterTrans.localScale.y, monsterTrans.localScale.z);
		unitOldEulerAngles = new Vector3 (monsterTrans.localEulerAngles.x, monsterTrans.localEulerAngles.y, monsterTrans.localEulerAngles.z);

		//new
		monsterTrans.position = focusUnitTrans.position;
		monsterTrans.localScale = focusUnitTrans.localScale;
        monsterTrans.localRotation = focusUnitTrans.localRotation;

        battleObject.SetTargetRotate(battleObject.transform.localRotation, false);
	}

	void SetCamera()
	{
		Transform cameraTrans = BattleCamera.Instance.transform;

		//old
		cameraOldPosition = new Vector3 (cameraTrans.position.x, cameraTrans.position.y, cameraTrans.position.z);
		cameraOldScale = new Vector3 (cameraTrans.localScale.x, cameraTrans.localScale.y, cameraTrans.localScale.z);
		cameraOldEulerAngles = new Vector3 (cameraTrans.localEulerAngles.x, cameraTrans.localEulerAngles.y, cameraTrans.localEulerAngles.z);
		
		//new
		//cameraTrans.position = focusCameraTrans.position;
		//cameraTrans.localScale = focusCameraTrans.localScale;
		//cameraTrans.eulerAngles = focusCameraTrans.eulerAngles;

		BattleCamera.Instance.cameraAni.MotionTo (focusCameraTrans, 0, false);
	}

	private void RestoreBattle()
	{
		Transform monsterTrans = battleObject.transform;
		monsterTrans.position = unitOldPosition;
		monsterTrans.localScale = unitOldScale;
		monsterTrans.localEulerAngles = unitOldEulerAngles;

		battleObject.SetTargetRotate(battleObject.transform.localRotation, true);
		//Transform cameraTrans = BattleCamera.Instance.transform;
		//cameraTrans.position = cameraOldPosition;
		//cameraTrans.localScale = cameraOldScale;
		//cameraTrans.localEulerAngles = cameraOldEulerAngles;

		BattleCameraAni.SetDefaultNoAni ();

		gameObject.SetActive (false);

		GameEventMgr.Instance.FireEvent (GameEventList.MonsterShowoffOver);

        UIBattle.Instance.ShowUI(true);
        UIBattle.Instance.gameObject.BroadcastMessage("OnAnimationFinish");
		GameEventMgr.Instance.FireEvent<UIBattle.UiState> (GameEventList.ChangeUIBattleState, UIBattle.UiState.Dazhao);
	}
}
