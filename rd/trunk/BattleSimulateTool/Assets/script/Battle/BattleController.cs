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
    public static float floorHeight = 0.0f;
    int curProcessIndex = 0;
    int maxProcessIndex = 0;
    int battleStartID = BattleConst.enemyStartID;
    BattleType battleType;
    InstanceData instanceData;
    BattleLevelData curBattleLevel = null;
    int instanceStar = 0;
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
    public bool processStart;

	private	Dictionary<string,Transform> cameraNodeDic = new Dictionary<string, Transform>();
    private bool battleSuccess;
    //---------------------------------------------------------------------------------------------
    // Use this for initialization
    public void Init()
    {
        instance = this;
        process = gameObject.AddComponent<BattleProcess>();
        process.Init();
    }
    //---------------------------------------------------------------------------------------------
	void OnDestroy()
	{
        Destroy(process);
	}
    //---------------------------------------------------------------------------------------------
    void StartProcess(int index)
    {
        Logger.Log("startProcess");
        if (index < maxProcessIndex)
        {
            string battleID;
            if (index < instanceData.battleLevelList.Count)
            {
                battleID = instanceData.battleLevelList[index];
            }
            else
            {
                battleID = instanceData.instanceProtoData.battleBoss;
            }

            curBattleLevel = StaticDataMgr.Instance.GetBattleLevelData(battleID);
            if (curBattleLevel.battleProtoData.id.Contains("boss"))
            {
                battleType = BattleType.Boss;
            }
            else
            {
                battleType = BattleType.Normal;
            }

            List<PbUnit> pbList = new List<PbUnit>();

            List<string> monsterIDList = new List<string>();
            var monsterItor = curBattleLevel.monsterList.GetEnumerator();
            while (monsterItor.MoveNext())
            {
                for (int i = 0; i < monsterItor.Current.Value; ++i)
                {
                    monsterIDList.Add(monsterItor.Current.Key);
                }
            }

            for (int i = 0; i < monsterIDList.Count; ++i)
            {
                PbUnit pbUnit = new PbUnit();
                pbUnit.guid = --battleStartID;
                pbUnit.id = monsterIDList[i];
                pbUnit.level = instanceData.instanceProtoData.level;
                pbUnit.camp = UnitCamp.Enemy;
                pbUnit.slot = i;
                pbUnit.lazy = BattleConst.defaultLazy;

                pbList.Add(pbUnit);
            }
            battleGroup.SetEnemyList(pbList);
            process.StartProcess(index, curBattleLevel);
        }
        else 
        {
            curBattleLevel = null;
            OnBattleOver(true);
        }
    }
    //---------------------------------------------------------------------------------------------
    public void StartNextProcess(float delayTime)
    {
        CurBlood(true);
        LogResult.Instance.logData[LogResult.Instance.xhNumber].logIsWin[BattleToolMain.Instance.bureauNum] = 1;
        Logger.Log("startNextProcess");
        battleGroup.DestroyEnemys();
        List<BattleObject> playerUnitList = BattleToolMain.Instance.mMainUnitList;
        var itor = playerUnitList.GetEnumerator();
        while (itor.MoveNext())
        {
            itor.Current.unit.OnStartNextProcess();
        }

        ++curProcessIndex;
        battleGroup.RefreshPlayerPos();
        StartProcess(curProcessIndex);
    }
    //---------------------------------------------------------------------------------------------
    public bool HasNextProcess()
    {
        return curProcessIndex + 1 < maxProcessIndex;
    }
    //---------------------------------------------------------------------------------------------
    int dp = 0;
    public void OnBattleOver(bool isSuccess)
    {        
        CurBlood(isSuccess);        
        battleSuccess = isSuccess;
        processStart = false;
        process.HideFireFocus();
        string iswin = "胜利";
        if (!isSuccess)
        {
            iswin = "失败";           
        }
        Logger.Log("<color=#7fff00ff>end simulate once </color>" + "<color=#FF0000FF>" + iswin + "</color>");
        LogResult.Instance.ShowLogPanel();
        ++LogResult.Instance.xhNumber;
        //unload battle scene
        battleGroup.DestroyEnemys();
        List<BattleObject> playerUnitList = BattleToolMain.Instance.mMainUnitList;
        for (int i = 0; i < playerUnitList.Count; ++i)
        {
            ObjectDataMgr.Instance.RemoveBattleObject(playerUnitList[i].guid);
        }
        battleGroup = null;
        curBattleScene = null;

        process.Clear();

        BattleToolMain.Instance.OnSimulateEnd();
    }
    //---------------------------------------------------------------------------------------------
    //for tool entry
    //---------------------------------------------------------------------------------------------
    public void StartSimulate()
    {         
        LogResult.Instance.logData[LogResult.Instance.xhNumber].logXhNumber = LogResult.Instance.xhNumber;               
        Logger.Log("<color=#7fff00ff>start simulate</color>");
        curProcessIndex = 0;
        processStart = false;
        battleStartID = BattleConst.enemyStartID;
        instanceData = StaticDataMgr.Instance.GetInstanceData(BattleToolMain.Instance.mInstanceID.text);//minghe18");//BattleToolMain.Instance.mInstanceID.text);
        maxProcessIndex = instanceData.battleLevelList.Count + 1;
        
        battleGroup = new BattleGroup();
        battleGroup.SetPlayerList();
        StartProcess(curProcessIndex);
    }
    //---------------------------------------------------------------------------------------------
    void CurBlood(bool b)
    {
        if (b)
        {
            int i = 0;
            for (; i < battleGroup.PlayerFieldList.Count; i++)
            {
                if (battleGroup.PlayerFieldList[i] != null && battleGroup.PlayerFieldList[i].unit.curLife != 0)
                {
                    LogResult.Instance.logData[LogResult.Instance.xhNumber].
                        playerData[BattleToolMain.Instance.bureauNum].monsterNumber++;
                }
                if (battleGroup.PlayerFieldList[i] == null)
                {
                    continue;                     
                }
                float blood = battleGroup.PlayerFieldList[i].unit.curLife;
                float bloodMax = battleGroup.PlayerFieldList[i].unit.maxLife;
                blood = (blood / bloodMax) * 100;
                LogResult.Instance.logData[LogResult.Instance.xhNumber].playerData[BattleToolMain.Instance.bureauNum].attBloodNumber[i] = (int)blood;                    
            }            
        }
        else
        {
            int i = 0;
            for (; i < battleGroup.EnemyFieldList.Count; i++)
            {
                if (battleGroup.EnemyFieldList[i] != null && battleGroup.EnemyFieldList[i].unit.curLife != 0)
                {
                    LogResult.Instance.logData[LogResult.Instance.xhNumber].
                      enemyData[BattleToolMain.Instance.bureauNum].monsterNumber++;
                }
                if (battleGroup.EnemyFieldList[i] == null)
                {
                    continue;                   
                }
                float blood = battleGroup.EnemyFieldList[i].unit.curLife;
                float bloodMax = battleGroup.EnemyFieldList[i].unit.maxLife;
                blood = (blood / bloodMax) * 100;
                LogResult.Instance.logData[LogResult.Instance.xhNumber].enemyData[BattleToolMain.Instance.bureauNum].attBloodNumber[i] = (int)blood;             
            }
        }
    }
    //---------------------------------------------------------------------------------------------
}
