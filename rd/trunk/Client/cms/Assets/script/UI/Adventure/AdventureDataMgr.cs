using UnityEngine;
using System.Collections.Generic;
using System;

public class AdventureInfo
{
    public AdventureData adventureData;

    //没有开始冒险时会有
    public List<PB.HSAdventureCondition> conditions = new List<PB.HSAdventureCondition>();
    
    //通过检测队伍是否为空来判断是否正在大冒险；
    public AdventureTeam adventureTeam;

    //private int endTime;
    public int EndTime
    {
        get
        {
            if (timeEvent!=null)
            {
                return timeEvent.endTime;
            }
            else
            {
                Logger.LogError("当前任务没有开启，没有结束时间");
                return 0;
            }
        }
        set
        {
            if (timeEvent==null)
            {
                Action endEvent = () =>
                {
                    timeEvent.RemoveTimeEvent();
                    timeEvent = null;
                    GameEventMgr.Instance.FireEvent(GameEventList.AdventureChange);
                };
                timeEvent = new TimeEventWrap(value, endEvent);
            }
            else
            {
                Logger.LogError("冒险时间计时事件异常");
            }
        }
    }
    //public int endTime;
    public TimeEventWrap timeEvent;

    public void RemoveTimeEvent()
    {
        if (timeEvent!=null)
        {
            timeEvent.RemoveTimeEvent();
            timeEvent = null;
        }
    }
    public AdventureInfo(PB.HSAdventureTeam adventure)
    {
        adventureData = StaticDataMgr.Instance.GetAdventureDataById(adventure.adventureId);

        adventureTeam = new AdventureTeam()
        {
            teamId = adventure.teamId,
            adventure = this,
            selfIdList=new List<int>(adventure.selfMonsterId),
            guildMonster = adventure.hireMonster
        };
        EndTime = adventure.endTime;
    }
    public AdventureInfo(PB.HSAdventure adventure)
    {
        adventureData = StaticDataMgr.Instance.GetAdventureDataById(adventure.adventureId);
        this.conditions = adventure.condition;
    }
    
}
public class AdventureTeam:IComparable
{
    public int teamId;
    public AdventureInfo adventure;
    public List<int> selfIdList = new List<int>();
    public PB.AllianceBaseMonster guildMonster;

    public int CompareTo(object obj)
    {
        AdventureTeam a = this;
        AdventureTeam b = (AdventureTeam)obj;
        int result = 0;
        if (a.adventure != null && b.adventure != null)
        {
            if ((a.adventure.timeEvent==null&&b.adventure.timeEvent==null)|| (a.adventure.timeEvent != null && b.adventure.timeEvent != null))
            {
                if (a.teamId < b.teamId)
                {
                    result = -1;
                }
                else
                {
                    result = 1;
                }
            }
            else
            {
                if (a.adventure.timeEvent==null)
                {
                    result = -1;
                }
                else
                {
                    result = 1;
                }
            }
        }
        else if (a.adventure == null && b.adventure == null)
        {
            if (a.teamId<b.teamId)
            {
                result = -1;
            }
            else
            {
                result = 1;
            }
        }
        else
        {
            if (a.adventure!=null)
            {
                result = -1;
            }
            else
            {
                result = 1;
            }
        }
        return result;
    }
}

public class AdventureDataMgr
{
    private static AdventureDataMgr inst;
    public static AdventureDataMgr Instance
    {
        get
        {
            if (inst==null)
            {
                inst = new AdventureDataMgr();
            }
            return inst;
        }
    }
    
    private int adventureChange;
    public int AdventureChange
    {
        get { return adventureChange; }
        set
        {
            if (adventureChange >= GameConfig.MaxAdventurePoint && adventureChange > value)
            {
                AdventureChangeBeginTime= GameTimeMgr.Instance.GetServerTimeStamp();
            }
            adventureChange = value;
        }
    }

    private int adventureChangeBeginTime;
    public int AdventureChangeBeginTime
    {
        get {return adventureChangeBeginTime; }
        set
        {
            if (adventureChangeBeginTime != value && AdventureChange < GameConfig.MaxAdventurePoint)
            {
                adventureChangeBeginTime = value;
                Action endEvent = () =>
                {
                    AdventureChange += 1;
                    AdventureChangeBeginTime = GameTimeMgr.Instance.GetServerTimeStamp();
                    GameEventMgr.Instance.FireEvent(GameEventList.AdventureConditionCountChange);
                };
                ConditionTimeEvent = new TimeEventWrap(adventureChangeBeginTime + GameConfig.AdventurePointTime, endEvent);
            }
            else if(AdventureChange >= GameConfig.MaxAdventurePoint)
            {
                ConditionTimeEvent = null;
            }
        }
    }
    
    private TimeEventWrap conditionTimeEvent; 
    public TimeEventWrap ConditionTimeEvent
    {
        get  {return conditionTimeEvent; }
        set
        {
            if (conditionTimeEvent!=null)
            {
                conditionTimeEvent.RemoveTimeEvent();
            }
            conditionTimeEvent = value;
        }
    }

    public List<int> hiredMonsterId=new List<int>();

    public int teamCount;

    public List<AdventureInfo> adventureList = new List<AdventureInfo>();

    public Dictionary<int, Dictionary<int, AdventureInfo>> adventureDict = new Dictionary<int, Dictionary<int, AdventureInfo>>();

    public List<AdventureTeam> adventureTeams = new List<AdventureTeam>();
    public void AdvestureInfoSync(PB.HSAdventureInfoSync syncInfo)
    {
        CleanAdventure();
        teamCount = syncInfo.teamCount;
        for (int i = 1; i <= teamCount; i++)
        {
            adventureTeams.Add(new AdventureTeam() { teamId = i });
        }

        #region 添加冒险到列表

        AdventureInfo adventureInfo;
        foreach (var item in syncInfo.busyTeam)
        {
            adventureInfo = new AdventureInfo(item);
            adventureList.Add(adventureInfo);
            AdventureTeamUpdate(adventureInfo.adventureTeam);
        }
        foreach (var item in syncInfo.idleAdventure)
        {
            adventureInfo = new AdventureInfo(item);
            adventureList.Add(adventureInfo);
        }
        #endregion

        #region 添加冒险到字典

        Dictionary<int, AdventureInfo> adventureGear = null;
        for (int i = 0; i < adventureList.Count; i++)
        {
            //Logger.Log(adventureList[i].adventureData.type +"     "+ adventureList[i].adventureData.time +"    "+ adventureList[i].adventureData.id);
            int type = adventureList[i].adventureData.type;
            int gear = adventureList[i].adventureData.time;

            if (!adventureDict.TryGetValue(type,out adventureGear))
            {
                adventureGear = new Dictionary<int, AdventureInfo>();
                adventureDict.Add(type, adventureGear);
            }
            if (adventureGear.ContainsKey(gear))
            {
                adventureGear[gear] = adventureList[i];
            }
            else
            {
                adventureGear.Add(gear, adventureList[i]);
            }
        }
        #endregion

        GameEventMgr.Instance.FireEvent(GameEventList.AdventureChange);
    }

    public void AdvestureInfoUpdate(PB.HSAdventureConditionPush updateInfo)
    {
        foreach (var item in updateInfo.idleAdventure)
        {
            AdventureInfoUpdate(item);
        }
        GameEventMgr.Instance.FireEvent(GameEventList.AdventureChange);
    }
    public void AdventureInfoUpdate(PB.HSAdventure adventure)
    {
        AdventureData data = StaticDataMgr.Instance.GetAdventureDataById(adventure.adventureId);
        //删旧
        AdventureInfo info = adventureDict[data.type][data.time];
        info.RemoveTimeEvent();
        adventureList.Remove(info);
        //加新
        info = new AdventureInfo(adventure);
        adventureList.Add(info);
        adventureDict[data.type][data.time] = info;
    }

    public void CleanAdventure()
    {
        teamCount = 0;
        adventureList.Clear();
        adventureDict.Clear();
        adventureTeams.Clear();
    }
    
    public bool CheckIsEnoughTeam()
    {
        int usedTeam = 0;
        for (int i = 0; i < adventureList.Count; i++)
        {
            if (adventureList[i].adventureTeam!=null)
            {
                usedTeam++;
                if (usedTeam>=teamCount)
                {
                    return false;
                }
            }
        }
        return true;
    }
    public void AdventureAddTeam(int teamId)
    {
        if (teamId == (teamCount + 1))
        {
            teamCount += 1;
            adventureTeams.Add(new AdventureTeam() { teamId = teamId });
        }
        else
        {
            Logger.LogError("添加大冒险队伍异常");
        }
    }
    public void AdventureTeamUpdate(int teamId)
    {
        for (int i = 0; i < adventureTeams.Count; i++)
        {
            if (adventureTeams[i].teamId == teamId)
            {
                AdventureTeam team = adventureTeams[i];
                for (int j = 0; j < team.selfIdList.Count; j++)
                {
                    GameUnit unit = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(team.selfIdList[j]);
                    if (unit != null)
                    {
                        unit.pbUnit.SetInAdventure(false);
                    }
                }
                adventureTeams.RemoveAt(i);
                adventureTeams.Add(new AdventureTeam() { teamId = teamId });
            }
        }
    }
    public void AdventureTeamUpdate(AdventureTeam team)
    {
        for (int i = 0; i < adventureTeams.Count; i++)
        {
            if (adventureTeams[i].teamId==team.teamId)
            {
                adventureTeams.RemoveAt(i);
                adventureTeams.Add(team);
            }
        }
    }
    public int GetUnusedAdventureTeamId()
    {
        int index = 0;
        for (int i = 0; i < adventureTeams.Count; i++)
        {
            if (adventureTeams[i].adventure==null)
            {
                if ((index == 0) || (index != 0 && adventureTeams[i].teamId < index))
                {
                    index = adventureTeams[i].teamId;
                }
            }
        }
        return index;
    }

    public void AddHiredMonster(int id)
    {
        if (hiredMonsterId.Contains(id))
        {
            Logger.LogError("上阵公会宠物出现错误");
        }
        hiredMonsterId.Add(id);
    }
    public void CleanHiredMonster()
    {
        hiredMonsterId.Clear();
    }
    public bool CheckMonsterIsHired(PB.AllianceBaseMonster monster)
    {
        for (int i = 0; i < hiredMonsterId.Count; i++)
        {
            if (monster.monsterId == hiredMonsterId[i])
            {
                return true;
            }
        }
        return false;
    }
}
