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
	private final String name;

	// client only
	private final String comments = null;
	private final String icon = null;
	private final String desc = null;
	private final String speechId = null;
	private final String path = null;

	// assemble
	protected int goalTypeValue = 0;
	protected List<Object> goalParamList = null;
	protected TimePointCfg timeBeginCfg = null;
	protected TimePointCfg timeEndCfg = null;
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
		name = "";
	}

	@Override
	protected boolean assemble() {
		// 计算QuestGroup
		QuestUtil.addQuestCfg(this);

		// 解析目标参数，支持'|'分割的或关系
		String[] params = goalParam.replaceAll("\\s*", "").split("\\|+");

		Class<?> paramClass = Void.class;

		switch (goalType) {
		// 领取大冒险奖励N次
		case "adventure":
			goalTypeValue = GsConst.QuestGoalType.ADVENTURE_TIMES;
			break;
		// 通关副本N次
		case "all":
			goalTypeValue = GsConst.QuestGoalType.INSTANCE_ALL_TIMES;
			break;
		// 进行竞技场N次
		case "arena":
			goalTypeValue = GsConst.QuestGoalType.ARENA_TIMES;
			break;
		// 购买金币N次
		case "buycoin":
			goalTypeValue = GsConst.QuestGoalType.BUY_COIN_TIMES;
			break;
		// 购买礼包N次
		case "buygift":
			goalTypeValue = GsConst.QuestGoalType.BUY_GIFT_TIMES;
			break;
		// 商店购买X道具N次
		case "buyitem-x":
			goalTypeValue = GsConst.QuestGoalType.BUY_ITEM_X_TIMES;
			paramClass = String.class;
			break;
		// 抽到≥X品级的装备N个
		case "callequipstage-x":
			goalTypeValue = GsConst.QuestGoalType.CALL_EQUIP_STAGE_X_COUNT;
			paramClass = Integer.class;
			break;
		// 抽到≥X品级的宠物N个
		case "callpetstage-x":
			goalTypeValue = GsConst.QuestGoalType.CALL_PET_STAGE_X_COUNT;
			paramClass = Integer.class;
			break;
		// 领取X困难章节的满星奖励
		case "chapterhard-x":
			goalTypeValue = GsConst.QuestGoalType.CHAPTER_X_HARD;
			paramClass = Integer.class;
			break;
		// 领取X普通章节的满星奖励
		case "chapternormal-x":
			goalTypeValue = GsConst.QuestGoalType.CHAPTER_X_NORMAL;
			paramClass = Integer.class;
			break;
		// 获得公会币N个
		case "coinsociety":
			goalTypeValue = GsConst.QuestGoalType.COIN_SOCIETY_COUNT;
			break;
		// 获得通天塔币N个
		case "cointower":
			goalTypeValue = GsConst.QuestGoalType.COIN_TOWER_COUNT;
			break;
		// 抽蛋N次
		case "eggall":
			goalTypeValue = GsConst.QuestGoalType.EGG_ALL_TIMES;
			break;
		// 金币抽蛋N次
		case "eggcoin":
			goalTypeValue = GsConst.QuestGoalType.EGG_COIN_TIMES;
			break;
		// 金币十连抽N次
		case "eggcoin10":
			goalTypeValue = GsConst.QuestGoalType.EGG_COIN_10_TIMES;
			break;
		// 钻石抽蛋N次
		case "eggdiamond":
			goalTypeValue = GsConst.QuestGoalType.EGG_DIAMOND_TIMES;
			break;
		// 钻石十连抽N次
		case "eggdiamond10":
			goalTypeValue = GsConst.QuestGoalType.EGG_DIAMOND_10_TIMES;
			break;
		// 装备打孔N次
		case "equipslot":
			goalTypeValue = GsConst.QuestGoalType.EQUIP_SLOT_TIMES;
			break;
		// 同一只宠物穿过≥X品级的装备达到N个
		case "equipstage-x":
			goalTypeValue = GsConst.QuestGoalType.EQUIP_WEAR_STAGE_X_COUNT;
			paramClass = Integer.class;
			break;
		// 免费领取
		case "free":
			goalTypeValue = GsConst.QuestGoalType.FREE;
			break;
		// 通关挑战副本N次
		case "hard":
			goalTypeValue = GsConst.QuestGoalType.INSTANCE_HARD_TIMES;
			break;
		// 进入金币试炼N次
		case "holecoinenter":
			goalTypeValue = GsConst.QuestGoalType.HOLE_COIN_TIMES;
			break;
		// 进入经验试炼N次
		case "holeexpenter":
			goalTypeValue = GsConst.QuestGoalType.HOLE_EXP_TIMES;
			break;
		// 镶嵌宝石N次
		case "inlayall":
			goalTypeValue = GsConst.QuestGoalType.INLAY_ALL_TIMES;
			break;
		// 镶嵌X类型宝石N次
		case "inlaytype-x":
			goalTypeValue = GsConst.QuestGoalType.INLAY_TYPE_X_TIMES;
			paramClass = Integer.class;
			break;
		// 通关X副本N次
		case "instance-x":
			goalTypeValue = GsConst.QuestGoalType.INSTANCE_X_TIMES;
			paramClass = String.class;
			break;
		// 角色等级达到N级
		case "level":
			goalTypeValue = GsConst.QuestGoalType.LEVEL;
			break;
		// 通关普通副本N次
		case "normal":
			goalTypeValue = GsConst.QuestGoalType.INSTANCE_NORMAL_TIMES;
			break;
		// 充值钻石N个
		case "paydiamond":
			goalTypeValue = GsConst.QuestGoalType.PAY_DIAMOND_COUNT;
			break;
		// 拥有过≥X等级的宠物N个
		case "petlevel-x":
			goalTypeValue = GsConst.QuestGoalType.MONSTER_LEVEL_X_COUNT;
			paramClass = Integer.class;
			break;
		// 完成宠物合成N次
		case "petmix":
			goalTypeValue = GsConst.QuestGoalType.MONSTER_MIX_TIMES;
			break;
		// 拥有过≥X品级的宠物N个
		case "petstage-x":
			goalTypeValue = GsConst.QuestGoalType.MONSTER_STAGE_X_COUNT;
			paramClass = Integer.class;
			break;
		// 完成X任务
		case "quest-x":
			goalTypeValue = GsConst.QuestGoalType.QUEST_X;
			paramClass = Integer.class;
			break;
		// 完成X循环类任务N个
		case "questcycle-x":
			goalTypeValue = GsConst.QuestGoalType.QUEST_CYCLE_X_COUNT;
			paramClass = Integer.class;
			break;
		// 完成X类任务N个
		case "questtype-x":
			goalTypeValue = GsConst.QuestGoalType.QUEST_TYPE_X_COUNT;
			paramClass = Integer.class;
			break;
		// 手动刷新商店N次
		case "shoprefresh":
			goalTypeValue = GsConst.QuestGoalType.SHOP_REFRESH_TIMES;
			break;
		// 公会bossN次
		case "societyboss":
			goalTypeValue = GsConst.QuestGoalType.SOCIETY_BOSS_TIMES;
			break;
		// 公会体力赠送N次
		case "societyfatigue":
			goalTypeValue = GsConst.QuestGoalType.SOCIETY_FATIGUE_TIMES;
			break;
		// 加入公会N次
		case "societyjoin":
			goalTypeValue = GsConst.QuestGoalType.SOCIETY_JOIN_TIMES;
			break;
		// 公会祈福N次
		case "societypray":
			goalTypeValue = GsConst.QuestGoalType.SOCIETY_PRAY_TIMES;
			break;
		//	三星通关X副本
		case "star3-x":
			goalTypeValue = GsConst.QuestGoalType.INSTANCE_X_STAR_3;
			paramClass = String.class;
			break;
		// 合成宝石N个
		case "synall":
			goalTypeValue = GsConst.QuestGoalType.SYN_ALL_COUNT;
			break;
		// 合成X类型宝石N个
		case "syntype-x":
			goalTypeValue = GsConst.QuestGoalType.SYN_TYPE_X_COUNT;
			paramClass = Integer.class;
			break;
		// 升级装备N次
		case "upequip":
			goalTypeValue = GsConst.QuestGoalType.UP_EQUIP_TIMES;
			break;
		// 升级技能N次
		case "upskill":
			goalTypeValue = GsConst.QuestGoalType.UP_SKILL_TIMES;
			break;
		// 使用钻石N个
		case "usediamond":
			goalTypeValue = GsConst.QuestGoalType.USE_DIAMOND_COUNT;
			break;
		// 消耗活力值N个
		case "usefatigue":
			goalTypeValue = GsConst.QuestGoalType.USE_FATIGUE_COUNT;
			break;
		// 使用X物品N个
		case "useitem-x":
			goalTypeValue = GsConst.QuestGoalType.USE_ITEM_X_COUNT;
			paramClass = String.class;
			break;
		default:
			HawkLog.errPrintln(String.format("config invalid goalType : %s", goalType));
			return false;
		}

		goalParamList = new ArrayList<Object>();
		if (true == paramClass.isAssignableFrom(String.class)) {
			if (0 == params.length) {
				return false;
			}
			for (String ps : params) {
				goalParamList.add(ps);
			}
		} else if (true == paramClass.isAssignableFrom(Integer.class)) {
			if (0 == params.length) {
				return false;
			}
			for (String ps : params) {
				goalParamList.add(Integer.parseInt(ps));
			}
		}

		return true;
	}

	@Override
	protected boolean checkValid() {
		if (timeBeginId != GsConst.UNUSABLE) {
			timeBeginCfg = HawkConfigManager.getInstance().getConfigByKey(TimePointCfg.class, timeBeginId);
			if (timeBeginCfg == null) {
				HawkLog.errPrintln(String.format("config invalid TimePointCfg : %d", timeBeginId));
				return false;
			}
		}
		if (timeEndId != GsConst.UNUSABLE) {
			timeEndCfg = HawkConfigManager.getInstance().getConfigByKey(TimePointCfg.class, timeEndId);
			if (timeEndCfg == null) {
				HawkLog.errPrintln(String.format("config invalid TimePointCfg : %d", timeEndId));
				return false;
			}
		}

		rewardCfg = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, rewardId);
		if (null == rewardCfg) {
			HawkLog.errPrintln(String.format("config invalid RewardCfg : %s", rewardId));
			return false;
		}

		switch (goalTypeValue) {
		// 商店购买X道具N次
		case GsConst.QuestGoalType.BUY_ITEM_X_TIMES:
		// 使用X物品N个
		case GsConst.QuestGoalType.USE_ITEM_X_COUNT:
			for (Object itemCfgId : goalParamList) {
				if (null == HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, (String) itemCfgId)) {
					HawkLog.errPrintln(String.format("config invalid RewardCfg : %s", itemCfgId));
					return false;
				}
			}
			break;

		// 领取X困难章节的满星奖励
		case GsConst.QuestGoalType.CHAPTER_X_HARD:
		// 领取X普通章节的满星奖励
		case GsConst.QuestGoalType.CHAPTER_X_NORMAL:
			for (Object chapterId : goalParamList) {
				if (null == HawkConfigManager.getInstance().getConfigByKey(InstanceChapterCfg.class, (Integer) chapterId)) {
					HawkLog.errPrintln(String.format("config invalid InstanceChapterCfg : %d", chapterId));
					return false;
				}
			}
			break;

		// 通关X副本N次
		case GsConst.QuestGoalType.INSTANCE_X_TIMES:
		//	三星通关X副本
		case GsConst.QuestGoalType.INSTANCE_X_STAR_3:
			for (Object instanceId : goalParamList) {
				if (null == HawkConfigManager.getInstance().getConfigByKey(InstanceCfg.class, (String) instanceId)) {
					HawkLog.errPrintln(String.format("config invalid InstanceCfg : %d", instanceId));
					return false;
				}
			}
			break;

			// 完成X任务
		case GsConst.QuestGoalType.QUEST_X:
			for (Object questId : goalParamList) {
				if (null == HawkConfigManager.getInstance().getConfigByKey(QuestCfg.class, (Integer) questId)) {
					HawkLog.errPrintln(String.format("config invalid QuestCfg : %d", questId));
					return false;
				}
			}
			break;
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

	public TimePointCfg getTimeBegin() {
		return timeBeginCfg;
	}

	public TimePointCfg getTimeEnd() {
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

	public String getNameId() {
		return name;
	}
}
