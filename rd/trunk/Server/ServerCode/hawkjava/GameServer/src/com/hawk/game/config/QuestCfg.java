package com.hawk.game.config;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

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
	private final String comments = null;
	private final String name = null;
	private final String icon = null;
	private final String desc = null;
	private final String speechId = null;
	private final String path = null;

	// assemble
	protected int goalTypeValue = 0;
	protected List<Object> goalParamList = null;
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

		// 解析目标参数，支持'|'分割的或关系
		String[] params = goalParam.replaceAll("\\s*", "").split("\\|+");

		Class<?> paramClass = Void.class;

		switch (goalType) {
		// 通关X副本
		case "instance":
			goalTypeValue = GsConst.QuestGoalType.INSTANCE;
			paramClass = String.class;
			break;
		//	满星通关X副本
		case "star3":
			goalTypeValue = GsConst.QuestGoalType.INSTANCE_STAR3;
			paramClass = String.class;
			break;
		// 通关普通副本次数
		case "normal":
			goalTypeValue = GsConst.QuestGoalType.INSTANCE_NORMAL_TIMES;
			break;
		// 通关精英副本次数
		case "hard":
			goalTypeValue = GsConst.QuestGoalType.INSTANCE_HARD_TIMES;
			break;
		// 通关副本次数
		case "all":
			goalTypeValue = GsConst.QuestGoalType.INSTANCE_ALL_TIMES;
			break;
		// 满星通关X章节
		case "chapter":
			goalTypeValue = GsConst.QuestGoalType.CHAPTER;
			paramClass = Integer.class;
			break;
		// 达到X角色等级
		case "level":
			goalTypeValue = GsConst.QuestGoalType.LEVEL;
			break;
		// 达到X品级宠物数量
		case "petstage":
			goalTypeValue = GsConst.QuestGoalType.MONSTER_STAGE_COUNT;
			paramClass = Integer.class;
			break;
		// 达到X等级宠物数量
		case "petlevel":
			goalTypeValue = GsConst.QuestGoalType.MONSTER_LEVEL_COUNT;
			paramClass = Integer.class;
			break;
		// 合成X宠物次数
		case "petmix":
			goalTypeValue = GsConst.QuestGoalType.MONSTER_MIX_TIMES;
			paramClass = String.class;
			break;
		// 完成竞技场次数
		case "arena":
			goalTypeValue = GsConst.QuestGoalType.ARENA_TIMES;
			break;
		// 完成金币试炼次数
		case "holecoin":
			goalTypeValue = GsConst.QuestGoalType.HOLE_COIN_TIMES;
			break;
		// 完成经验试炼次数
		case "holeexp":
			goalTypeValue = GsConst.QuestGoalType.HOLE_EXP_TIMES;
			break;
		// 完成通天塔次数
		case "tower":
			goalTypeValue = GsConst.QuestGoalType.TOWER_TIMES;
			break;
		// 完成大冒险次数
		case "adventure":
			goalTypeValue = GsConst.QuestGoalType.ADVENTURE_TIMES;
			break;
		// 升级技能次数
		case "upskill":
			goalTypeValue = GsConst.QuestGoalType.UP_SKILL_TIMES;
			break;
		// 升级装备次数
		case "upequip":
			goalTypeValue = GsConst.QuestGoalType.UP_EQUIP_TIMES;
			break;
		// 购买金币次数
		case "buycoin":
			goalTypeValue = GsConst.QuestGoalType.BUY_COIN_TIMES;
			break;
		// 购买礼包次数
		case "buygift":
			goalTypeValue = GsConst.QuestGoalType.BUY_GIFT_TIMES;
			break;
		// 购买X物品次数
		case "buyitem":
			goalTypeValue = GsConst.QuestGoalType.BUY_ITEM_TIMES;
			paramClass = String.class;
			break;
		// 充值钻石数量
		case "paydiamond":
			goalTypeValue = GsConst.QuestGoalType.PAY_DIAMOND_COUNT;
			break;
		// 消耗活力值数量
		case "usefatigue":
			goalTypeValue = GsConst.QuestGoalType.USE_FATIGUE_COUNT;
			break;
		// 使用X物品数量
		case "useitem":
			goalTypeValue = GsConst.QuestGoalType.USE_ITEM_COUNT;
			paramClass = String.class;
			break;
		// 使用钻石数量
		case "usediamond":
			goalTypeValue = GsConst.QuestGoalType.USE_DIAMOND_COUNT;
			break;
		// 镶嵌宝石次数
		case "inlayall":
			goalTypeValue = GsConst.QuestGoalType.INLAY_ALL_TIMES;
			//goalParamValue = Integer.parseInt(goalParam);
			break;
		// 镶嵌X类型宝石次数
		case "inlaytype":
			goalTypeValue = GsConst.QuestGoalType.INLAY_TYPE_TIMES;
			paramClass = Integer.class;
			break;
		// 合成宝石次数
		case "synall":
			goalTypeValue = GsConst.QuestGoalType.SYN_ALL_TIMES;
			break;
		// 合成X类型宝石次数
		case "syntype":
			goalTypeValue = GsConst.QuestGoalType.SYN_TYPE_TIMES;
			paramClass = Integer.class;
			break;
		// 金币抽蛋次数
		case "eggcoin":
			goalTypeValue = GsConst.QuestGoalType.EGG_COIN_TIMES;
			break;
		// 钻石抽蛋次数
		case "eggdiamond":
			goalTypeValue = GsConst.QuestGoalType.EGG_DIAMOND_TIMES;
			break;
		// 抽蛋次数
		case "eggall":
			goalTypeValue = GsConst.QuestGoalType.EGG_ALL_TIMES;
			break;
		// 抽到X品级宠物次数
		case "callpetstage":
			goalTypeValue = GsConst.QuestGoalType.CALL_PET_STAGE_TIMES;
			paramClass = Integer.class;
			break;
		// 抽到X品级装备次数
		case "callequipstage":
			goalTypeValue = GsConst.QuestGoalType.CALL_EQUIP_STAGE_TIMES;
			paramClass = Integer.class;
			break;
		// 抽到X物品次数
		case "callitem":
			goalTypeValue = GsConst.QuestGoalType.CALL_ITEM_TIMES;
			paramClass = String.class;
			break;
		// 公会加入次数
		case "societyjoin":
			goalTypeValue = GsConst.QuestGoalType.SOCIETY_JOIN_TIMES;
			break;
		// 公会退出次数
		case "societyleave":
			goalTypeValue = GsConst.QuestGoalType.SOCIETY_LEAVE_TIMES;
			break;
		// 公会祈福次数
		case "societypray":
			goalTypeValue = GsConst.QuestGoalType.SOCIETY_PRAY_TIMES;
			break;
		// 公会boss次数
		case "societyboss":
			goalTypeValue = GsConst.QuestGoalType.SOCIETY_BOSS_TIMES;
			break;
		// 公会体力赠送次数
		case "societyfatigue":
			goalTypeValue = GsConst.QuestGoalType.SOCIETY_FATIGUE_TIMES;
			break;
		// 刷新商店次数
		case "shoprefresh":
			goalTypeValue = GsConst.QuestGoalType.SHOP_REFRESH_TIMES;
			break;
		// 获得竞技场币数量
		case "coinarena":
			goalTypeValue = GsConst.QuestGoalType.COIN_ARENA_COUNT;
			break;
		// 获得公会币数量
		case "coinsociety":
			goalTypeValue = GsConst.QuestGoalType.COIN_SOCIETY_COUNT;
			break;
		// 获得通天塔币数量
		case "cointower":
			goalTypeValue = GsConst.QuestGoalType.COIN_TOWER_COUNT;
			break;
		// 完成X任务
		case "quest":
			goalTypeValue = GsConst.QuestGoalType.QUEST;
			paramClass = Integer.class;
			break;
		// 完成X类型任务数量
		case "questtype":
			goalTypeValue = GsConst.QuestGoalType.QUEST_TYPE_COUNT;
			paramClass = Integer.class;
			break;
		// 完成X循环性任务数量
		case "questcycle":
			goalTypeValue = GsConst.QuestGoalType.QUEST_CYCLE_COUNT;
			paramClass = Integer.class;
			break;
		// 携带过X品级装备数量
		case "equipstage":
			goalTypeValue = GsConst.QuestGoalType.EQUIP_STAGE_COUNT;
			paramClass = Integer.class;
			break;
		// 打孔次数
		case "equipslot":
			goalTypeValue = GsConst.QuestGoalType.EQUIP_SLOT_TIMES;
			break;	
		// 免费领取
		case "free":
			goalTypeValue = GsConst.QuestGoalType.FREE_GOAL;
			break;
		default:
			HawkLog.errPrintln(String.format("config invalid goalType : %s", goalType));
			return false;
		}

		goalParamList = new ArrayList<Object>();
		if (true == paramClass.isAssignableFrom(String.class)) {
			for (String ps : params) {
				goalParamList.add(ps);
			}
		} else if (true == paramClass.isAssignableFrom(Integer.class)) {
			for (String ps : params) {
				goalParamList.add(Integer.parseInt(ps));
			}
		}

		return true;
	}

	@Override
	protected boolean checkValid() {
		if (timeBeginId != GsConst.UNUSABLE) {
			timeBeginCfg = HawkConfigManager.getInstance().getConfigByKey(TimeCfg.class, timeBeginId);
			if (timeBeginCfg == null) {
				HawkLog.errPrintln(String.format("config invalid TimeCfg : %d", timeBeginId));
				return false;
			}
		}
		if (timeEndId != GsConst.UNUSABLE) {
			timeEndCfg = HawkConfigManager.getInstance().getConfigByKey(TimeCfg.class, timeEndId);
			if (timeEndCfg == null) {
				HawkLog.errPrintln(String.format("config invalid TimeCfg : %d", timeEndId));
				return false;
			}
		}
// 临时注释
//		rewardCfg = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, rewardId);
//		if (null == rewardCfg) {
//			HawkLog.errPrintln(String.format("config invalid RewardCfg : %s", rewardId));
//			return false;
//		}

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

	public List<Object> getGoalParamList() {
		return goalParamList;
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
