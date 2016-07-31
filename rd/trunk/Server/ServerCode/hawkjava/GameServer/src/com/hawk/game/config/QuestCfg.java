package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;

import com.hawk.game.util.GsConst;
import com.hawk.game.util.QuestUtil;

@SuppressWarnings("unused")
@HawkConfigManager.CsvResource(file = "staticData/quest.csv", struct = "map")
public class QuestCfg extends HawkConfigBase {
	
	@Id
	protected final int id;
	protected final int group;
	protected final int type;
	protected final int level;
	protected final int cycle;
	protected final int timeBeginId;
	protected final int timeEndId;
	protected final String goalType;
	protected final String goalParam;
	protected final int goalCount;
	protected final String rewardId;
	protected final float expK;
	protected final float expB;

	// client only
	private final String name = null;
	private final String icon = null;
	private final String descType = null;
	private final String desc = null;
	private final String speechId = null;
	
	// assemble
	protected int goalTypeValue = 0;
	protected Object goalParamValue = null;
	protected TimeCfg timeBeginCfg = null;
	protected TimeCfg timeEndCfg = null;
	protected RewardCfg rewardCfg = null;

	public QuestCfg() {
		id = 0;
		group = 0;
		type = 0;
		level = 0;
		cycle = 0;
		timeBeginId = 0;
		timeEndId = 0;
		goalType = "";
		goalParam = "";
		goalCount = 0;
		rewardId = "";
		expK = 0;
		expB = 0;
	}

	@Override
	protected boolean assemble() {
		// 计算QuestGroup
		QuestUtil.addQuest(this);
		
		switch (goalType) {
		// 特定难度副本
		case "difficulty":
			goalTypeValue = GsConst.QuestGoalType.DIFFICULTY_GOAL;
			goalParamValue = goalParam;
			break;
		//	特定星级副本
		case "3stars":
			goalTypeValue = GsConst.QuestGoalType.STAR_GOAL;
			goalParamValue = goalParam;
			break;
		// 普通难度副本
		case "normal":
			goalTypeValue = GsConst.QuestGoalType.INSTANCE_NORMAL_GOAL;
			break;
		// 精英难度副本
		case "hard":
			goalTypeValue = GsConst.QuestGoalType.INSTANCE_HARD_GOAL;
			break;
		// 所有难度副本
		case "all":
			goalTypeValue = GsConst.QuestGoalType.INSTANCE_ALL_GOAL;
			break;
		// 角色等级
		case "level":
			goalTypeValue = GsConst.QuestGoalType.LEVEL_GOAL;
			break;
		// 宠物品级
		case "petquality":
			goalTypeValue = GsConst.QuestGoalType.MONSTER_STAGE_GOAL;
			goalParamValue = Integer.parseInt(goalParam);
			break;
		// 宠物等级
		case "petlevel":
			goalTypeValue = GsConst.QuestGoalType.MONSTER_LEVEL_GOAL;
			goalParamValue = Integer.parseInt(goalParam);
			break;
		// 竞技场次数
		case "arena":
			goalTypeValue = GsConst.QuestGoalType.ARENA_GOAL;
			break;
		//	时光之穴次数
		case "time":
			goalTypeValue = GsConst.QuestGoalType.TIMEHOLE_GOAL;
			break;
		// 炼妖炉次数
		case "petmix":
			goalTypeValue = GsConst.QuestGoalType.MONSTER_MIX_GOAL;
			break;
		// 大冒险次数
		case "adventure":
			goalTypeValue = GsConst.QuestGoalType.ADVENTURE_GOAL;
			break;
		// bossrush次数
		case "bossrush":
			goalTypeValue = GsConst.QuestGoalType.BOSSRUSH_GOAL;
			break;
		// 稀有探索次数
		case "explore":
			goalTypeValue = GsConst.QuestGoalType.EXPLORE_GOAL;
			break;
		// 升级技能次数
		case "skill":
			goalTypeValue = GsConst.QuestGoalType.SKILL_UP_GOAL;
			break;
		// 升级装备次数
		case "equip":
			goalTypeValue = GsConst.QuestGoalType.EQUIP_UP_GOAL;
			break;
		// 购买金币次数
		case "buycoin":
			goalTypeValue = GsConst.QuestGoalType.BUY_COIN_GOAL;
			break;
		// 领取体力
		case "fatigue":
			goalTypeValue = GsConst.QuestGoalType.GET_FATIGUE_GOAL;
		default:
			return false;
		}

		return true;
	}
	
	@Override
	protected boolean checkValid() {
		if (timeBeginId != 0) {
			timeBeginCfg = HawkConfigManager.getInstance().getConfigByKey(TimeCfg.class, timeBeginId);
			if (timeBeginCfg == null) {
				HawkLog.errPrintln(String.format("config invalid TimeCfg : %d", timeBeginId));
				return false;
			}
		}
		if (timeEndId != 0) {
			timeEndCfg = HawkConfigManager.getInstance().getConfigByKey(TimeCfg.class, timeEndId);
			if (timeEndCfg == null) {
				HawkLog.errPrintln(String.format("config invalid TimeCfg : %d", timeEndId));
				return false;
			}
		}
		rewardCfg = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, rewardId);
		if (null == rewardCfg) {
			HawkLog.errPrintln(String.format("config invalid RewardCfg : %s", rewardId));
			return false;
		}
		
		return true;
	}

	public int getId() {
		return id;
	}
	
	public int getGroup() {
		return group;
	}
	
	public int getType() {
		return type;
	}
	
	public int getLevel() {
		return level;
	}
	
	public int getCycle() {
		return cycle;
	}
	
	public TimeCfg getTimeBegin() {
		return timeBeginCfg;
	}
	
	public TimeCfg getTimeEnd() {
		return timeEndCfg;
	}
	
	public int getGoalType() {
		return goalTypeValue;
	}
	
	public Object getGoalParam() {
		return goalParamValue;
	}
	
	public int getGoalCount() {
		return goalCount;
	}
	
	public RewardCfg getReward() {
		return rewardCfg;
	}
	
	public int getExpReward(int level) {
		return (int)(expK * level + expB);
	}
}
