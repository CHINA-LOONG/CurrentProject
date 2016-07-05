using UnityEngine;
using System.Collections;

public class DazhaofocusController : MonoBehaviour 
{

	private GameObject 		focusSceneGo;
	private	Transform		focusCameraTrans;
	private	Transform		focusUnitTrans;
	private BattleObject 	battleObject;

	private	Vector3 unitOldPosition;
	private	Vector3 unitOldScale;
	private	Vector3 unitOldEulerAngles;

	private Vector3 cameraOldPosition;
	private Vector3 cameraOldScale;
	private Vector3 cameraOldEulerAngles;

	static DazhaofocusController instance = null;
	public static DazhaofocusController Instance 
	{
		get
		{
			return instance;
		}
	}

	// Use this for initialization
	void Start ()
	{
		instance = this;
		GameObject prefab = ResourceMgr.Instance.LoadAsset ("battlescene/dazhaofocus", "dazhaoFocusScene");
		focusSceneGo = Instantiate (prefab) as GameObject;

		focusUnitTrans = Util.FindChildByName (focusSceneGo, "monsterPosition").transform;
		MeshRenderer tempMesh = focusUnitTrans.GetComponent<MeshRenderer> ();
		if (null != tempMesh) 
		{
			tempMesh.enabled = false;
		}

		focusCameraTrans = Util.FindChildByName (focusSceneGo, "cameraPosition").transform;
		tempMesh = focusCameraTrans.GetComponent<MeshRenderer> ();
		if (null != tempMesh) 
		{
			tempMesh.enabled = false;
		}

		focusSceneGo.SetActive (false);
	}
	
	public void ShowoffDazhao(BattleObject battleObj)
	{
		battleObject = battleObj;
		SetMonster ();
		SetCamera ();
		focusSceneGo.SetActive (true);
		StartCoroutine (MonsterShowoffCo ());

		UIBattle.Instance.gameObject.SetActive (false);
	}

	IEnumerator MonsterShowoffCo()
	{
		//monstershowoff

		yield return new WaitForSeconds (1.5f);
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
		monsterTrans.localEulerAngles = focusUnitTrans.localEulerAngles;
	}

	void SetCamera()
	{
		Transform cameraTrans = BattleCamera.Instance.transform;

		//old
		cameraOldPosition = new Vector3 (cameraTrans.position.x, cameraTrans.position.y, cameraTrans.position.z);
		cameraOldScale = new Vector3 (cameraTrans.localScale.x, cameraTrans.localScale.y, cameraTrans.localScale.z);
		cameraOldEulerAngles = new Vector3 (cameraTrans.localEulerAngles.x, cameraTrans.localEulerAngles.y, cameraTrans.localEulerAngles.z);
		
		//new
		cameraTrans.position = focusCameraTrans.position;
		//cameraTrans.localScale = focusCameraTrans.localScale;
		cameraTrans.eulerAngles = focusCameraTrans.eulerAngles;
	}

	private void RestoreBattle()
	{
		Transform monsterTrans = battleObject.transform;
		monsterTrans.position = unitOldPosition;
		monsterTrans.localScale = unitOldScale;
		monsterTrans.localEulerAngles = unitOldEulerAngles;

		Transform cameraTrans = BattleCamera.Instance.transform;
		cameraTrans.position = cameraOldPosition;
		cameraTrans.localScale = cameraOldScale;
		cameraTrans.localEulerAngles = cameraOldEulerAngles;

		focusSceneGo.SetActive (false);

		GameEventMgr.Instance.FireEvent (GameEventList.MonsterShowoffOver);

		UIBattle.Instance.gameObject.SetActive (true);
	}
	

}
