using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InstanceChapter : MonoBehaviour 
{
	public	List<InstanceButton> instanceButtonList = new List<InstanceButton>();
	// Use this for initialization
	IEnumerator Start () 
	{
		yield return new WaitForEndOfFrame();

		InstanceButton subButton = null;
		for (int i =0; i < instanceButtonList.Count; ++i)
		{
			subButton = instanceButtonList[i];
			subButton.index = i;
			EventTriggerListener.Get(subButton.button.gameObject).onClick = OnInstanceButtonClick;
		}
	}

	void OnInstanceButtonClick(GameObject go)
	{
		var subButton = go.transform.parent.GetComponent<InstanceButton> ();
		//Debug.LogError ("Clicked button... index = " + subButton.index);
		//int index = subButton.index;
		var instanceId = subButton.instanceId;
		var data = InstanceMapService.Instance.GetRuntimeInstance (instanceId);
		/*
        UIBuild uiBuild = UIMgr.Instance.GetUI(UIBuild.ViewName) as UIBuild;
        if (uiBuild != null && uiBuild.uiInstance != null)
        {
            uiBuild.uiInstance.instanceInfo.ShowWithData(data);
        }
		*/
		UIAdjustBattleTeam adjustUi = UIMgr.Instance.OpenUI_(UIAdjustBattleTeam.ViewName) as UIAdjustBattleTeam;
		UIBuild uiBuild= UIMgr.Instance.GetUI(UIBuild.ViewName) as UIBuild;
		if (uiBuild!=null)
		{
			uiBuild.uiAdjustBattleTeam = adjustUi;
		}
		adjustUi.SetData (data.instanceId, data.staticData.enemyList,data.staticData.level);
	}
}
