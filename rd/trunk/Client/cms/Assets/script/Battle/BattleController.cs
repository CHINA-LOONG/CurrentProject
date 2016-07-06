using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

public enum BattleType
{
    Normal,
    Boss,
    Rare,
}

public class BattleController : MonoBehaviour
{
    BattleType battleType;
    InstanceData instanceData;
    public InstanceData InstanceData
    {
        get { return instanceData; }
    }
    BattleProcess process;
    public BattleProcess Process
    {
        get { return process; }
    }
    bool isMouseOnUI = false;

    BattleGroup battleGroup;
    public BattleGroup BattleGroup
    {
        get { return battleGroup; }
    }

    //战斗胜利method
    MethodInfo victorMethod = null;

    static BattleController instance;
    public static BattleController Instance
    {
        get { return instance; }
    }

    public BattleObject curBattleScene
    {
        set;
        get;
    }
    private UIBattle uiBattle;

	private	Dictionary<string,Transform> cameraNodeDic = new Dictionary<string, Transform>();

    //---------------------------------------------------------------------------------------------
    // Use this for initialization
    public void Init()
    {
        instance = this;
        BindListener();
        process = gameObject.AddComponent<BattleProcess>();
        process.Init();
        //battleGroup = new BattleGroup();
    }
    //---------------------------------------------------------------------------------------------
	void OnDestroy()
	{
        Destroy(process);
		UnBindListener ();
	}
    //---------------------------------------------------------------------------------------------
	void BindListener()
	{
		GameEventMgr.Instance.AddListener<Vector3>(GameEventList.MirrorClicked ,OnMirrorClilced );
	}
    //---------------------------------------------------------------------------------------------
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener<Vector3> (GameEventList.MirrorClicked, OnMirrorClilced);
	}
    //---------------------------------------------------------------------------------------------
	void  OnMirrorClilced(Vector3 inputPos)
	{
		RaycastBattleObject (inputPos);
	}
    //---------------------------------------------------------------------------------------------
    void Update()
    {
		Vector3 inputPos = Input.mousePosition;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            isMouseOnUI = EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }  
        else if (Input.GetMouseButtonDown(0))
        {
            isMouseOnUI = EventSystem.current.IsPointerOverGameObject();
        }

		if (Input.GetMouseButtonUp (0))
		{
            if (!isMouseOnUI)
            {
                if (!LastEvenType.Instance.IsDrag())
                {
                    RaycastBattleObject(inputPos);
                }
            }
            else 
            {
                GameObject curGo = EventSystem.current.currentSelectedGameObject;
                if (curGo != null && curGo.GetComponent<PetSwitchItem>() == null)
                {
                    GameEventMgr.Instance.FireEvent<int>(GameEventList.HideSwitchPetUI, BattleConst.closeSwitchPetUI);
                }
            }
		}
    }
    //---------------------------------------------------------------------------------------------
	void RaycastBattleObject(Vector3 inputPos)
	{
		RaycastHit hit;
		Ray ray = BattleCamera.Instance.GetComponent<Camera>().ScreenPointToRay( inputPos );
		
		if (Physics.Raycast(ray, out hit, 100))
		{
			var battleGo = hit.collider.gameObject.GetComponent<BattleObject>();
			if (battleGo)
			{
				if(battleGo.unit.isVisible)
				{
					OnHitBattleObject(battleGo, GetClickedEnemyWpName(battleGo,inputPos));
				}
			}
			else
			{
                GameEventMgr.Instance.FireEvent<int>(GameEventList.HideSwitchPetUI, BattleConst.closeSwitchPetUI);
				Logger.LogWarning("Hit something but not a battle object!");
			}
		}
		else
		{
            GameEventMgr.Instance.FireEvent<int>(GameEventList.HideSwitchPetUI, BattleConst.closeSwitchPetUI);
		}
	}
    //---------------------------------------------------------------------------------------------
	string GetClickedEnemyWpName(BattleObject battleObj,Vector3 inputScreenPos)
	{
		if (battleObj.camp == UnitCamp.Player) 
		{
			return null;
		}
		var gameUnit = battleObj.unit;
		MirrorTarget findTarget = MirrorRaycast.RaycastFirFocusWeakpoint (gameUnit, inputScreenPos, GameConfig.Instance.FireFocusWpRadius);
		if (findTarget != null)
		{
			return findTarget.WeakPointIDAttr;
		}
		return null;
	}
    //---------------------------------------------------------------------------------------------
    void OnHitBattleObject(BattleObject battleGo, string weakpointName)
    {
        if (battleGo.camp == UnitCamp.Enemy)
        {
            //集火或者大招
			process.OnHitBattleObject(battleGo, weakpointName);
            GameEventMgr.Instance.FireEvent<int>(GameEventList.HideSwitchPetUI, BattleConst.closeSwitchPetUI);
            Logger.LogWarning("hit enemy gameobject....");
        }
        else if (battleGo.camp == UnitCamp.Player && process.SwitchingPet == false)
        {
            //换宠
            ShowSwitchPetUIArgs args = new ShowSwitchPetUIArgs();
            args.targetId = battleGo.guid;
            GameEventMgr.Instance.FireEvent<EventArgs>(GameEventList.ShowSwitchPetUI, args);
        }
    }
    //---------------------------------------------------------------------------------------------
    public void StartBattle(PbStartBattle proto)
    {
        battleType = (BattleType)proto.battleType;
        instanceData = StaticDataMgr.Instance.GetInstanceData(proto.instanceId);

        if (!InitVictorMethod())
            return;

        //加载场景
        LoadBattleScene(instanceData.sceneBattle);

        //设置battlegroup 并且创建模型
        battleGroup.SetEnemyList(proto.enemyList);
        battleGroup.SetPlayerList();

        PlayPreStoryAnim();

        var ui = UIMgr.Instance.OpenUI(UIBattle.AssertName, UIBattle.ViewName);
        uiBattle = ui.GetComponent<UIBattle>();
        uiBattle.Init();

        StartProcess(0);
    }
    //---------------------------------------------------------------------------------------------
    public UIBattle GetUIBattle()
    {
        return uiBattle;
    }
    //---------------------------------------------------------------------------------------------
    void LoadBattleScene(string sceneName)
    {
        battleGroup = new BattleGroup();
        int index = sceneName.LastIndexOf('/');
        string assetbundle = sceneName.Substring(0, index);
        string assetname = sceneName.Substring(index + 1, sceneName.Length - index - 1);
        curBattleScene = ObjectDataMgr.Instance.CreateSceneObject(BattleConst.battleSceneGuid, assetbundle, assetname);

        Transform defaultTrans = GetDefaultCameraNode();
        BattleCamera.Instance.transform.localPosition = defaultTrans.position;
        BattleCamera.Instance.transform.localRotation = defaultTrans.rotation;
        BattleCamera.Instance.transform.localScale = Vector3.Scale(transform.localScale, defaultTrans.localScale);
    }
    //---------------------------------------------------------------------------------------------
    public Transform GetDefaultCameraNode()
    {
        string cameraSlotName = "cameraNormal";
        if (battleType == BattleType.Boss)
        {
            cameraSlotName = "cameraBoss";
        }
		Transform defaultTrans = null;
		if (cameraNodeDic.TryGetValue (cameraSlotName, out defaultTrans))
		{
			return defaultTrans;
		}

        GameObject cameraNode = Util.FindChildByName(curBattleScene.gameObject, cameraSlotName);
		cameraNodeDic.Add (cameraSlotName, cameraNode.transform);
		return cameraNode.transform;
    }
	//---------------------------------------------------------------------------------------------
	public	Transform GetPhyDazhaoCameraNode()
	{
		string name = "cameraPhyDazhao";
		Transform dazhaoTrans = null;
		if (cameraNodeDic.TryGetValue (name, out dazhaoTrans))
		{
			return dazhaoTrans;
		}

		GameObject cameraNode = Util.FindChildByName(curBattleScene.gameObject,name);
		cameraNodeDic.Add (name, cameraNode.transform);
		return cameraNode.transform;
	}
    //---------------------------------------------------------------------------------------------
    void UnLoadBattleScene()
    {
        battleGroup.DestroyEnemys();
        ObjectDataMgr.Instance.RemoveBattleObject(BattleConst.battleSceneGuid);
        //Destroy(curBattleScene);
        List<BattleObject> playerUnitList = GameDataMgr.Instance.PlayerDataAttr.GetMainUnits();
        for (int i = 0; i < playerUnitList.Count; ++i)
        {
            playerUnitList[i].gameObject.SetActive(false);
        }

        curBattleScene = null;
    }
    //---------------------------------------------------------------------------------------------
    public GameObject GetSlotNode(UnitCamp camp, int slotID, bool isBoss)
    {
        if (curBattleScene == null)
        {
            Logger.LogError("battle scene is null");
            return GameMain.Instance.gameObject;
        }

        string nodeName = "pos";
        if (isBoss)
        {
            nodeName = "bosspos";
        }
        else 
        {
            if (camp == UnitCamp.Enemy)
            {
                slotID = slotID + BattleConst.slotIndexMax + 1;
            }

            nodeName = nodeName + slotID.ToString();
        }

        GameObject slotNode = Util.FindChildByName(curBattleScene.gameObject, nodeName);
        if (slotNode != null)
        {
            return slotNode;
        }

        return GameMain.Instance.gameObject;
    }
    //---------------------------------------------------------------------------------------------
    /// <summary>
    /// 反射获取战斗/进程胜利的方法
    /// </summary>
    bool InitVictorMethod()
    {
        //获取战斗胜利的func
        switch (battleType)
        {
            case BattleType.Normal:
                {
                    if (instanceData.normalValiVicMethod != null)
                    {
                        victorMethod = instanceData.normalValiVicMethod;
                        break;
                    }

                    string funcName = instanceData.normalValiVic;
                    var cls = typeof(NormalScript);
                    victorMethod = cls.GetMethod(funcName);
                    if (victorMethod == null)
                    {
                        Logger.LogErrorFormat("Instance {0}'s normalValiVic #{1}# can not find! Exit battle!", instanceData.id, funcName);
                        return false;
                    }
                    instanceData.normalValiVicMethod = victorMethod;
                    break;
                }
            case BattleType.Boss:
                {
                    if (instanceData.bossValiVicMethod != null)
                    {
                        victorMethod = instanceData.bossValiVicMethod;
                        break;
                    }

                    string funcName = instanceData.bossValiVic;
                    var cls = typeof(BossScript);
                    victorMethod = cls.GetMethod(funcName);
                    if (victorMethod == null)
                    {
                        Logger.LogErrorFormat("Instance {0}'s bossValiVic #{1}# can not find! Exit battle!", instanceData.id, funcName);
                        return false;
                    }
                    instanceData.bossValiVicMethod = victorMethod;

                    foreach (var item in instanceData.bossProcess)
                    {
                        if (item.method != null)
                            break;

                        funcName = item.func;
                        var method = cls.GetMethod(funcName);
                        if (method == null)
                        {
                            Logger.LogErrorFormat("Instance {0}'s BossProcessValiVic #{1}# can not find! Exit battle!", instanceData.id, funcName);
                            return false;
                        }
                        item.method = method;
                    }
                    break;
                }
            case BattleType.Rare:
                {
                    if (instanceData.rareValiVicMethod != null)
                    {
                        victorMethod = instanceData.rareValiVicMethod;
                        break;
                    }

                    string funcName = instanceData.rareValiVic;
                    var cls = typeof(RareScript);
                    victorMethod = cls.GetMethod(funcName);
                    if (victorMethod == null)
                    {
                        Logger.LogErrorFormat("Instance {0}'s rareValiVic #{1}# can not find! Exit battle!", instanceData.id, funcName);
                        return false;
                    }
                    instanceData.rareValiVicMethod = victorMethod;

                    foreach (var item in instanceData.rareProcess)
                    {
                        if (item.method != null)
                            break;

                        funcName = item.func;
                        var method = cls.GetMethod(funcName);
                        if (method == null)
                        {
                            Logger.LogErrorFormat("Instance {0}'s RareProcessValiVic #{1}# can not find! Exit battle!", instanceData.id, funcName);
                            return false;
                        }
                        item.method = method;
                    }
                    break;
                }
            default:
                Logger.LogError("Battle type error" + battleType);
                return false;
        }

        return true;
    }
    //---------------------------------------------------------------------------------------------
    void PlayPreStoryAnim()
    {
        Logger.Log("[Battle]Play Story Animation before any process...");
    }
    //---------------------------------------------------------------------------------------------
    //void ShowUI()
    //{
    //    GameEventMgr.Instance.FireEvent(GameEventList.ShowBattleUI);
    //}
    //---------------------------------------------------------------------------------------------
    void StartProcess(int index)
    {
        var curProcess = GetProcessAtIndex(index);
        if (curProcess != null)
            process.StartProcess(curProcess, victorMethod);
        else
            OnBattleOver(true);
    }
    //---------------------------------------------------------------------------------------------
    ProcessData GetProcessAtIndex(int index)
    {
        if (battleType == BattleType.Normal)
        {
            var pProcess = new ProcessData();
            pProcess.method = victorMethod;
            return pProcess;
        }

        if (battleType == BattleType.Boss)
        {
            int count = instanceData.bossProcess.Count;
            if (index < 0 || index >= count)
            {
                Logger.LogError("Boss process error index: " + index);
                return null;
            }
            else
                return instanceData.bossProcess[index];
        }

        if (battleType == BattleType.Rare)
        {
            int count = instanceData.rareProcess.Count;
            if (index < 0 || index >= count)
            {
                Logger.LogError("Rare process error index: " + index);
                return null;
            }
            else
                return instanceData.rareProcess[index];
        }

        Logger.LogError("BattleType error" + battleType);
        return null;
    }
    //---------------------------------------------------------------------------------------------
    public void OnProcessSwitch(int gotoVal)
    {
        //检测下一个process的条件，避免出现跳进程的情况，如：从>50%，一刀砍到<30%
        var next = GetProcessAtIndex(gotoVal - 1);
        while (true)
        {
            var processRet = (int)next.method.Invoke(null, null);
            if (next.rets.ContainsKey(processRet))
            {
                gotoVal = next.rets[processRet];
                next = GetProcessAtIndex(gotoVal - 1);
            }
            else
                break;
        }

        Logger.LogWarning("Switch Process to: " + gotoVal);
        //进程切换条件达成后
        StartProcess(gotoVal - 1);
    }
    //---------------------------------------------------------------------------------------------
    public void OnBattleOver(bool isSuccess)
    {
        Debug.LogWarning("Battle " + (isSuccess ? "Success" : "Failed"));
        StartCoroutine(ProcessBattleOver(isSuccess));
    }
    //---------------------------------------------------------------------------------------------
    IEnumerator ProcessBattleOver(bool isSuccess)
    {
        //yield return new WaitForSeconds(5.0f);
        //胜利失败动画
        yield return StartCoroutine(PlayBalanceAnim(isSuccess));
        //后置剧情动画
        yield return StartCoroutine(PlayPostStoryAnim());
        //结算面板UI
        ShowBalanceUI();

        //怪物全部退场
        //battleGroup.AllUnitsExitField();//do this in UnLoadBattleScen

        //回到副本层
        UnLoadBattleScene();
        process.Clear();
        GameMain.Instance.ChangeModule<BuildModule>();
        UIMgr.Instance.CloseUI(UIBattle.ViewName);

		UIMgr.Instance.CloseUI (UIFazhen.ViewName);
    }
    //---------------------------------------------------------------------------------------------
    IEnumerator PlayBalanceAnim(bool isSuccess)
    {
        uiBattle.ShowEndBattleUI(isSuccess);
        string eventName = isSuccess ? "win" : "failed";
        float curTime = Time.time;
        for (int i = 0; i < battleGroup.EnemyFieldList.Count; ++i)
        {
            BattleObject bo = battleGroup.PlayerFieldList[i];
            if (bo != null && bo.unit.curLife > 0)
            {
                battleGroup.PlayerFieldList[i].TriggerEvent(eventName, curTime, null);
            }
        }
        yield return new WaitForSeconds(3.0f);
        uiBattle.DestroyEndBattleUI();
        yield return null;
    }
    //---------------------------------------------------------------------------------------------
    IEnumerator PlayPostStoryAnim()
    {
        Logger.Log("[Battle]Playing Post Story Anim...");
        yield return null;
    }
    //---------------------------------------------------------------------------------------------
    IEnumerator ShowBalanceUI()
    {
        Logger.Log("[Battle]Showing Balance UI...");
        yield return null;
    }
    //---------------------------------------------------------------------------------------------
}
