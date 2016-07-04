using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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


    // Use this for initialization
    public void Init(BattleProcess process)
    {
        instance = this;
        this.process = process;
        battleGroup = new BattleGroup();
		BindListener ();
    }

	void OnDestroy()
	{
		UnBindListener ();
	}
	
	void BindListener()
	{
		GameEventMgr.Instance.AddListener<Vector3>(GameEventList.MirrorClicked ,OnMirrorClilced );
	}
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener<Vector3> (GameEventList.MirrorClicked, OnMirrorClilced);
	}

	void  OnMirrorClilced(Vector3 inputPos)
	{
		RaycastBattleObject (inputPos);
	}

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

		if (Input.GetMouseButtonUp (0) && !isMouseOnUI)
		{
			RaycastBattleObject (inputPos);
		}
    }

	void RaycastBattleObject(Vector3 inputPos)
	{
		RaycastHit hit;
		Ray ray = BattleCamera.Instance.GetComponent<Camera>().ScreenPointToRay( inputPos );
		
		if (Physics.Raycast(ray, out hit, 100))
		{
			var battleGo = hit.collider.gameObject.GetComponent<BattleObject>();
			if (battleGo)
			{
				OnHitBattleObject(battleGo, GetClickedEnemyWpName(battleGo,inputPos));
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

	string GetClickedEnemyWpName(BattleObject battleObj,Vector3 inputScreenPos)
	{
		if (battleObj.camp == UnitCamp.Player) 
		{
			return null;
		}
		var gameUnit = battleObj.unit;
		
		MirrorTarget findTarget = MirrorRaycast.RaycastCanAttackWeakpoint (gameUnit, inputScreenPos, GameConfig.Instance.FireFocusWpRadius);
		if (findTarget != null)
		{
			return findTarget.WeakPointIDAttr;
		}
		return null;
	}

    void OnHitBattleObject(BattleObject battleGo, string weakpointName)
    {
        if (battleGo.camp == UnitCamp.Enemy)
        {
            //集火或者大招
			process.OnHitBattleObject(battleGo, weakpointName);
            GameEventMgr.Instance.FireEvent<int>(GameEventList.HideSwitchPetUI, BattleConst.closeSwitchPetUI);
            Logger.LogWarning("hit enemy gameobject....");
        }
        else if (battleGo.camp == UnitCamp.Player)
        {
            //换宠
            ShowSwitchPetUIArgs args = new ShowSwitchPetUIArgs();
            args.targetId = battleGo.id;
            GameEventMgr.Instance.FireEvent<EventArgs>(GameEventList.ShowSwitchPetUI, args);
        }
    }

    public void StartBattle(PbStartBattle proto)
    {
        battleType = (BattleType)proto.battleType;
        instanceData = StaticDataMgr.Instance.GetInstanceData(proto.instanceId);

        if (!InitVictorMethod())
            return;

        //设置battlegroup 并且创建模型
        battleGroup.SetEnemyList(proto.enemyList);
        battleGroup.SetPlayerList(proto.playerList);

        PlayPreStoryAnim();

        ShowUI();

        StartProcess(0);
    }

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

    void PlayPreStoryAnim()
    {
        Logger.Log("[Battle]Play Story Animation before any process...");
    }

    void ShowUI()
    {
        GameEventMgr.Instance.FireEvent(GameEventList.ShowBattleUI);
    }

    void StartProcess(int index)
    {
        var curProcess = GetProcessAtIndex(index);
        if (curProcess != null)
            process.StartProcess(curProcess, victorMethod);
        else
            OnBattleOver(true);
    }

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

    public void OnBattleOver(bool isSuccess)
    {
        Debug.LogWarning("Battle " + (isSuccess ? "Success" : "Failed"));
        //胜利失败动画
        PlayBalanceAnim();
        //后置剧情动画
        PlayPostStoryAnim();
        //结算面板UI
        ShowBalanceUI();

        //怪物全部退场
        battleGroup.AllUnitsExitField();

        //回到副本层
        GameMain.Instance.ChangeModule<BuildModule>();
        UIMgr.Instance.CloseUI(UIBattle.ViewName);
    }

    private void PlayBalanceAnim()
    {
        Logger.Log("[Battle]Playing Balance Anim...");
    }

    private void PlayPostStoryAnim()
    {
        Logger.Log("[Battle]Playing Post Story Anim...");
    }

    private void ShowBalanceUI()
    {
        Logger.Log("[Battle]Showing Balance UI...");
    }
}
