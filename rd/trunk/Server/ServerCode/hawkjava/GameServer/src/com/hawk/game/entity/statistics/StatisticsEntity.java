package com.hawk.game.entity.statistics;

import java.util.Calendar;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;
import java.util.Set;

import org.hawk.config.HawkConfigManager;
import org.hawk.db.HawkDBManager;
import org.hawk.os.HawkTime;
import org.hawk.xid.HawkXID;

import com.hawk.game.config.QuestCfg;
import com.hawk.game.protocol.Const;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.QuestUtil;

/**
 * 玩家统计数据包装类
 * 
 * @author walker
 */
public class StatisticsEntity {

	private HfStatisticsEntity hfEntity;
	private MfStatisticsEntity mfEntity;
	private Mf2StatisticsEntity mf2Entity;
	private LfStatisticsEntity lfEntity;
	private Lf2StatisticsEntity lf2Entity;
	private Map<Integer, RefreshStatisticsEntity> refreshEntityMap;

	private boolean hfUpdate;
	private boolean mfUpdate;
	private boolean mf2Update;
	private boolean lfUpdate;
	private boolean lf2Update;
	private Map<Integer, Boolean> refreshUpdateMap;

	public StatisticsEntity() {
		hfUpdate = false;
		mfUpdate = false;
		mf2Update = false;
		lfUpdate = false;
		lf2Update = false;
		refreshUpdateMap = new HashMap<Integer, Boolean>();
		refreshEntityMap = new HashMap<Integer, RefreshStatisticsEntity>();
	}

	public void load(int playerId) {
		if (null == hfEntity) {
			List<HfStatisticsEntity> resultList = HawkDBManager.getInstance().query("from HfStatisticsEntity where playerId = ? and invalid = 0", playerId);
			if (null != resultList && resultList.size() > 0) {
				hfEntity = resultList.get(0);
				hfEntity.decode();
			} else {
				hfEntity = new HfStatisticsEntity(playerId);
				hfEntity.notifyCreate();
			}
		}

		if (null == mfEntity) {
			List<MfStatisticsEntity> resultList = HawkDBManager.getInstance().query("from MfStatisticsEntity where playerId = ? and invalid = 0", playerId);
			if (null != resultList && resultList.size() > 0) {
				mfEntity = resultList.get(0);
				mfEntity.decode();
			} else {
				mfEntity = new MfStatisticsEntity(playerId);
				mfEntity.notifyCreate();
			}
		}

		if (null == mf2Entity) {
			List<Mf2StatisticsEntity> resultList = HawkDBManager.getInstance().query("from Mf2StatisticsEntity where playerId = ? and invalid = 0", playerId);
			if (null != resultList && resultList.size() > 0) {
				mf2Entity = resultList.get(0);
				mf2Entity.decode();
			} else {
				mf2Entity = new Mf2StatisticsEntity(playerId);
				mf2Entity.notifyCreate();
			}
		}

		if (null == lfEntity) {
			List<LfStatisticsEntity> resultList = HawkDBManager.getInstance().query("from LfStatisticsEntity where playerId = ? and invalid = 0", playerId);
			if (null != resultList && resultList.size() > 0) {
				lfEntity = resultList.get(0);
				lfEntity.decode();
			} else {
				lfEntity = new LfStatisticsEntity(playerId);
				lfEntity.notifyCreate();
			}
		}

		if (null == lf2Entity) {
			List<Lf2StatisticsEntity> resultList = HawkDBManager.getInstance().query("from Lf2StatisticsEntity where playerId = ? and invalid = 0", playerId);
			if (null != resultList && resultList.size() > 0) {
				lf2Entity = resultList.get(0);
				lf2Entity.decode();
			} else {
				lf2Entity = new Lf2StatisticsEntity(playerId);
				lf2Entity.notifyCreate();
			}
		}

		if (true == refreshEntityMap.isEmpty()) {
			List<RefreshStatisticsEntity> resultList = HawkDBManager.getInstance().query("from RefreshStatisticsEntity where playerId = ? and invalid = 0", playerId);
			if (null != resultList && resultList.size() > 0) {
				for (RefreshStatisticsEntity entity: resultList) {
					entity.decode();
					refreshEntityMap.put(entity.timePoint, entity);
				}
			}
		}
	}

	public void notifyUpdate(boolean async) {
		if (true == hfUpdate) {
			hfUpdate = false;
			hfEntity.notifyUpdate(async);
		}
		if (true == mfUpdate) {
			mfUpdate = false;
			mfEntity.notifyUpdate(async);
		}
		if (true == mf2Update) {
			mf2Update = false;
			mf2Entity.notifyUpdate(async);
		}
		if (true == lfUpdate) {
			lfUpdate = false;
			lfEntity.notifyUpdate(async);
		}
		if (true == lf2Update) {
			lf2Update = false;
			lf2Entity.notifyUpdate(async);
		}
		for (Entry<Integer, Boolean> entry : refreshUpdateMap.entrySet()) {
			if (true == entry.getValue()) {
				refreshEntityMap.get(entry.getKey()).notifyUpdate(async);
			}
		}

		// 更新任务
		QuestUtil.postQuestDataUpdateMsg(HawkXID.valueOf( GsConst.ObjType.PLAYER, getPlayerId()));
	}

	// HF method=========================================================================

	// playerId------------------------------------------------
	public int getPlayerId() {
		return hfEntity.playerId;
	}

	public void setPlayerId(int playerId) {
		hfEntity.playerId = playerId;
		hfUpdate = true;
	}

	// fatigue-------------------------------------------------
	public int getFatigue() {
		return hfEntity.fatigue;
	}

	public void setFatigue(int fatigue) {
		hfEntity.fatigue = fatigue;
		hfUpdate = true;
	}

	public Calendar getFatigueBeginTime() {
		return hfEntity.fatigueBeginTime;
	}

	public void setFatigueBeginTime(Calendar time) {
		hfEntity.fatigueBeginTime = time;
		hfUpdate = true;
	}

	// useFatigueCount-----------------------------------------
	public int getUseFatigueCount() {
		return hfEntity.useFatigueCount;
	}

	public void increaseUseFatigueCount(int count) {
		hfEntity.useFatigueCount += count;
		hfUpdate = true;
	}

	public int getUseFatigueCountDaily() {
		return hfEntity.useFatigueCountDaily;
	}

	public void increaseUseFatigueCountDaily(int count) {
		hfEntity.useFatigueCountDaily += count;
		hfUpdate = true;
	}

	public void clearUseFatigueCountDaily() {
		hfEntity.useFatigueCountDaily = 0;
		hfUpdate = true;
	}

	// multiExpTimes-------------------------------------------
	/**
	 * 经验加倍是否还有剩余次数
	 */
	public boolean isMultiExpLeft() {
		return hfEntity.doubleExpLeft > 0 || hfEntity.tripleExpLeft > 0;
	}

	public int getDoubleExpLeft() {
		return hfEntity.doubleExpLeft;
	}

	public int getTripleExpLeft() {
		return hfEntity.tripleExpLeft;
	}

	public void increaseDoubleExpLeft(int times) {
		hfEntity.doubleExpLeft += times;
		hfUpdate = true;
	}

	public void increaseTripleExpLeft(int times) {
		hfEntity.tripleExpLeft += times;
		hfUpdate = true;
	}

	public void decreaseDoubleExpLeft(int times) {
		hfEntity.doubleExpLeft -= times;
		hfUpdate = true;
	}

	public void decreaseTripleExpLeft(int times) {
		hfEntity.tripleExpLeft -= times;
		hfUpdate = true;
	}

	// instanceState--------------------------------------------
	public Map<String, int[]> getInstanceStateMap() {
		return hfEntity.instanceStateMap;
	}

	public void removeInstanceState(String instanceId) {
		// 只有GM使用
		hfEntity.instanceStateMap.remove(instanceId);
		hfEntity.instanceStateFlag = true;
		hfUpdate = true;
	}

	/**
	 * @return 副本完成次数，如未完成返回0
	 */
	public int getInstanceWinTimes(String instanceId) {
		int[] state = hfEntity.instanceStateMap.get(instanceId);
		if (null != state) {
			return state[GsConst.Instance.STATE_WIN_INDEX];
		}
		return 0;
	}

	public void increaseInstanceWinTimes(String instanceId, int type, int times) {
		int[] state = hfEntity.instanceStateMap.get(instanceId);
		if (null != state) {
			state[GsConst.Instance.STATE_WIN_INDEX] += times;
		} else {
			if (type == Const.InstanceType.INSTANCE_STORY_VALUE) {
				hfEntity.instanceStateMap.put(instanceId, new int[]{times, 0, 0});
			} else {
				hfEntity.instanceStateMap.put(instanceId, new int[]{times});
			}
		}
		hfEntity.instanceStateFlag = true;
		hfUpdate = true;
	}

	/**
	 * @return 副本完成星级，如未完成返回0
	 */
	public int getInstanceStar(String instanceId) {
		int[] state = hfEntity.instanceStateMap.get(instanceId);
		if (null != state) {
			return state[GsConst.Instance.STATE_STAR_INDEX];
		}
		return 0;
	}

	public void setInstanceStar(String instanceId, int star) {
		// star为0的情况正常逻辑只有最后一个副本会出现
		int[] state = hfEntity.instanceStateMap.get(instanceId);
		if (null != state) {
			state[GsConst.Instance.STATE_STAR_INDEX] = star;
		} else {
			hfEntity.instanceStateMap.put(instanceId, new int[] {0, star, 0});
		}
		hfEntity.instanceStateFlag = true;
		hfUpdate = true;
	}

	/**
	 * @return 每日副本进入次数，如未进入返回0
	 */
	public int getInstanceEnterTimesDaily(String instanceId) {
		int[] state = hfEntity.instanceStateMap.get(instanceId);
		if (null != state) {
			return state[GsConst.Instance.STATE_ENTER_INDEX];
		}
		return 0;
	}

	public void increaseInstanceEnterTimesDaily(String instanceId, int times) {
		int[] state = hfEntity.instanceStateMap.get(instanceId);
		if (null != state) {
			state[GsConst.Instance.STATE_ENTER_INDEX] += times;
		} else {
			hfEntity.instanceStateMap.put(instanceId, new int[] {0, 0, times});
		}
		hfEntity.instanceStateFlag = true;
		hfUpdate = true;
	}

	public void setInstanceEnterTimesDaily(String instanceId, int times) {
		int[] state = hfEntity.instanceStateMap.get(instanceId);
		if (null != state) {
			state[GsConst.Instance.STATE_ENTER_INDEX] = times;
		} else {
			hfEntity.instanceStateMap.put(instanceId, new int[] {0, 0, times});
		}
		hfEntity.instanceStateFlag = true;
		hfUpdate = true;
	}

	public void clearInstanceEnterTimesDaily() {
		for (int[] state : hfEntity.instanceStateMap.values()) {
			if (state.length == GsConst.Instance.STATE_STORY_SIZE) {
				state[GsConst.Instance.STATE_ENTER_INDEX] = 0;
			}
		}
		hfEntity.instanceStateFlag = true;
		hfUpdate = true;
	}

	// instanceTimes-------------------------------------------
	public int getInstanceAllTimes() {
		return hfEntity.normalTimes + hfEntity.hardTimes;
	}

	public int getInstanceAllTimesDaily() {
		return hfEntity.normalTimesDaily + hfEntity.hardTimesDaily;
	}

	public int getInstanceNormalTimes() {
		return hfEntity.normalTimes;
	}

	public void increaseInstanceNormalTimes(int times) {
		hfEntity.normalTimes += times;
		hfUpdate = true;
	}

	public int getInstanceNormalTimesDaily() {
		return hfEntity.normalTimesDaily;
	}

	public void increaseInstanceNormalTimesDaily(int times) {
		hfEntity.normalTimesDaily += times;
		hfUpdate = true;
	}

	public void clearInstanceNormalTimesDaily() {
		hfEntity.normalTimesDaily = 0;
		hfUpdate = true;
	}

	public int getInstanceHardTimes() {
		return hfEntity.hardTimes;
	}

	public void increaseInstanceHardTimes(int times) {
		hfEntity.hardTimes += times;
		hfUpdate = true;
	}

	public int getInstanceHardTimesDaily() {
		return hfEntity.hardTimesDaily;
	}

	public void increaseInstanceHardTimesDaily(int times) {
		hfEntity.hardTimesDaily += times;
		hfUpdate = true;
	}

	public void clearInstanceHardTimesDaily() {
		hfEntity.hardTimesDaily = 0;
		hfUpdate = true;
	}

	// topChapter&Index----------------------------------------
	public int getNormalTopChapter() {
		return hfEntity.normalTopChapter;
	}

	public void setNormalTopChapter(int chapter) {
		hfEntity.normalTopChapter = chapter;
		hfUpdate = true;
	}

	public int getNormalTopIndex() {
		return hfEntity.normalTopIndex;
	}

	public void setNormalTopIndex(int index) {
		hfEntity.normalTopIndex = index;
		hfUpdate = true;
	}

	public int getHardTopChapter() {
		return hfEntity.hardTopChapter;
	}

	public void setHardTopChapter(int chapter) {
		hfEntity.hardTopChapter = chapter;
		hfUpdate = true;
	}

	public int getHardTopIndex() {
		return hfEntity.hardTopIndex;
	}

	public void setHardTopIndex(int index) {
		hfEntity.hardTopIndex = index;
		hfUpdate = true;
	}

	// MF method=========================================================================

	// questComplete-------------------------------------------
	public boolean isQuestComplete(int questId) {
		return mfEntity.questCompleteSet.contains(questId);
	}

	public Set<Integer> getQuestCompleteSet() {
		return mfEntity.questCompleteSet;
	}

	public boolean isQuestDailyComplete(int questId) {
		return mfEntity.questCompleteDailySet.contains(questId);
	}

	public Set<Integer> getQuestDailyCompleteSet() {
		return mfEntity.questCompleteDailySet;
	}

	public void addQuestComplete(int questId) {
		QuestCfg questCfg = HawkConfigManager.getInstance().getConfigByKey(QuestCfg.class, questId);
		if (null == questCfg) {
			return;
		}

		if (questCfg.getCycle() == GsConst.Quest.DAILY_CYCLE) {
			mfEntity.questCompleteDailySet.add(questId);
			mfEntity.questCompleteDailyFlag = true;
		} else {
			mfEntity.questCompleteSet.add(questId);
			mfEntity.questCompleteFlag = true;
		}
		mfUpdate = true;
	}

	public void clearQuestDailyComplete() {
		mfEntity.questCompleteDailySet.clear();
		mfEntity.questCompleteDailyFlag = true;
		mfUpdate = true;
	}

	// GM only begin/////////////////
	public void removeQuestComplete(int questId) {
		QuestCfg questCfg = HawkConfigManager.getInstance().getConfigByKey(QuestCfg.class, questId);
		if (null == questCfg) {
			return;
		}

		if (questCfg.getCycle() == GsConst.Quest.DAILY_CYCLE) {
			mfEntity.questCompleteDailySet.remove(questId);
			mfEntity.questCompleteDailyFlag = true;
		} else {
			mfEntity.questCompleteSet.remove(questId);
			mfEntity.questCompleteFlag = true;
		}
		mfUpdate = true;
	}
	public void clearQuestComplete() {
		mfEntity.questCompleteSet.clear();
		mfEntity.questCompleteFlag = true;
		mfUpdate = true;
	}
	// GM only end/////////////////////

	// skillPoint----------------------------------------------
	public int getSkillPoint() {
		return mfEntity.skillPoint;
	}

	public void setSkillPoint(int point) {
		mfEntity.skillPoint = point;
		mfUpdate = true;
	}

	public int getSkillPointBeginTime() {
		return mfEntity.skillPointBeginTime;
	}

	public void setSkillPointBeginTime(int time) {
		mfEntity.skillPointBeginTime = time;
		mfUpdate = true;
	}

	// pvpTime----------------------------------------------
	public int getPVPTime() {
		return mfEntity.pvpTime;
	}

	public void setPVPTime(int times) {
		mfEntity.pvpTime = times;
		mfUpdate = true;
	}

	public int getPVPTimeBeginTime() {
		return mfEntity.pvpTimeBeginTime;
	}

	public void setPVPTimeBeginTime(int time) {
		mfEntity.pvpTimeBeginTime = time;
		mfUpdate = true;
	}

	// monsterCount--------------------------------------------
//	public List<Integer> getMonsterCountOverStageList() {
//		return mfEntity.monsterStageCountList;
//	}

	public int getMonsterCountOverStage(int stage) {
		// 从0开始
		int index = stage;
		if (index >= mfEntity.monsterStageCountList.size()) {
			return 0;
		}
		return mfEntity.monsterStageCountList.get(index);
	}

	public void increaseMonsterCountOverStage(int stage) {
		// 从0开始
		int index = stage;
		for (int i = mfEntity.monsterStageCountList.size(); i <= index; ++i) {
			mfEntity.monsterStageCountList.add(0);
		}
		mfEntity.monsterStageCountList.set(index, mfEntity.monsterStageCountList.get(index) + 1);
		mfEntity.monsterStageCountFlag = true;
		mfUpdate = true;
	}

//	public List<Integer> getMonsterCountOverLeveList() {
//		return mfEntity.monsterLevelCountList;
//	}

	public int getMonsterCountOverLevel(int level) {
		// 从1开始
		int index = level - 1;
		if (index >= mfEntity.monsterLevelCountList.size()) {
			return 0;
		}
		return mfEntity.monsterLevelCountList.get(index);
	}

	public void increaseMonsterCountOverLevel(int level) {
		// 从1开始
		int index = level - 1;
		for (int i = mfEntity.monsterLevelCountList.size(); i <= index; ++i) {
			mfEntity.monsterLevelCountList.add(0);
		}
		mfEntity.monsterLevelCountList.set(index, mfEntity.monsterLevelCountList.get(index) + 1);
		mfEntity.monsterLevelCountFlag = true;
		mfUpdate = true;
	}

	// arenaTimes----------------------------------------------
	public int getArenaTimes() {
		return mfEntity.arenaTimes;
	}

	public void increaseArenaTimes() {
		++mfEntity.arenaTimes;
		mfUpdate = true;
	}

	public int getArenaTimesDaily() {
		return mfEntity.arenaTimesDaily;
	}

	public void increaseArenaTimesDaily() {
		++mfEntity.arenaTimesDaily;
		mfUpdate = true;
	}

	public void clearArenaTimesDaily() {
		mfEntity.arenaTimesDaily = 0;
		mfUpdate = true;
	}

	// holeTimes-----------------------------------------------
	public Map<Integer, Integer> getHoleTimesMap() {
		return mfEntity.holeTimesMap;
	}

	/**
	 * @return 洞完成次数，如未完成返回0
	 */
	public int getHoleTimes(int holeId) {
		Integer times = mfEntity.holeTimesMap.get(holeId);
		if (null != times) {
			return times;
		}
		return 0;
	}

	public void increaseHoleTimes(int holeId) {
		Integer cur = mfEntity.holeTimesMap.get(holeId);
		if (null == cur) {
			cur = 0;
		}
		mfEntity.holeTimesMap.put(holeId, cur + 1);
		mfEntity.holeTimesFlag = true;
		mfUpdate = true;
	}

//	public void setHoleTimes(int holeId, int times) {
//		mfEntity.holeTimesMap.put(holeId, times);
//		mfEntity.holeTimesFlag = true;
//		mfUpdate = true;
//	}

//	public Map<Integer, Integer> getHoleTimesDailyMap() {
//		return mfEntity.holeTimesDailyMap;
//	}

	/**
	 * @return 每日洞完成次数，如未完成返回0
	 */
	public int getHoleTimesDaily(int holeId) {
		Integer times = mfEntity.holeTimesDailyMap.get(holeId);
		if (null != times) {
			return times;
		}
		return 0;
	}

	public void increaseHoleTimesDaily(int holeId) {
		Integer cur = mfEntity.holeTimesDailyMap.get(holeId);
		if (null == cur) {
			cur = 0;
		}
		mfEntity.holeTimesDailyMap.put(holeId, cur + 1);
		mfEntity.holeTimesDailyFlag = true;
		mfUpdate = true;
	}

//	public void setHoleTimesDaily(int holeId, int times) {
//		mfEntity.holeTimesDailyMap.put(holeId, times);
//		mfEntity.holeTimesDailyFlag = true;
//		mfUpdate = true;
//	}

	public void clearHoleTimesDaily() {
		mfEntity.holeTimesDailyMap.clear();
		mfEntity.holeTimesDailyFlag = true;
		mfUpdate = true;
	}

	// towerFloor----------------------------------------------
	public Map<Integer, Integer> getTowerFloorMap() {
		return mfEntity.towerFloorMap;
	}

	/**
	 * @return 塔完成层数，如未完成返回0
	 */
	public int getTowerFloor(int towerId) {
		Integer floor = mfEntity.towerFloorMap.get(towerId);
		if (null != floor) {
			return floor;
		}
		return 0;
	}

	public void increaseTowerFloor(int towerId) {
		Integer cur = mfEntity.towerFloorMap.get(towerId);
		if (null == cur) {
			cur = 0;
		}
		mfEntity.towerFloorMap.put(towerId, cur + 1);
		mfEntity.towerFloorFlag = true;
		mfUpdate = true;
	}

//	public void setTowerFloor(int towerId, int floor) {
//		mfEntity.towerFloorMap.put(towerId, floor);
//		mfEntity.towerFloorFlag = true;
//		mfUpdate = true;
//	}

	public void clearTowerFloorMap() {
		mfEntity.towerFloorMap.clear();
		mfEntity.towerFloorFlag = true;
		mfUpdate = true;
	}

	// hireMonster------------------------------------------
	public Set<Integer> getHireMonsterDailySet() {
		return mfEntity.hireMonsterDailySet;
	}

	public void addHireMonsterDaily(int monsterId) {
		mfEntity.hireMonsterDailySet.add(monsterId);
		mfEntity.hireMonsterDailyFlag = true;
		mfUpdate = true;
	}

	public void clearHireMonsterDaily() {
		mfEntity.hireMonsterDailySet.clear();
		mfEntity.hireMonsterDailyFlag = true;
		mfUpdate = true;
	}

	// adventureTimes------------------------------------------
	public int getAdventureTimes() {
		return mfEntity.adventureTimes;
	}

	public void increaseAdventureTimes() {
		++mfEntity.adventureTimes;
		mfUpdate = true;
	}

	public int getAdventureTimesDaily() {
		return mfEntity.adventureTimesDaily;
	}

	public void increaseAdventureTimesDaily() {
		++mfEntity.adventureTimesDaily;
		mfUpdate = true;
	}

	public void clearAdventureTimesDaily() {
		mfEntity.adventureTimesDaily = 0;
		mfUpdate = true;
	}



	// coinTower-----------------------------------------------
	public int getCoinTowerCount() {
		return mfEntity.coinTowerCount;
	}

	public void increaseCoinTowerCount(int count) {
		mfEntity.coinTowerCount += count;
		mfUpdate = true;
	}

	// coinAlliance--------------------------------------------
	public int getCoinAllianceCount() {
		return mfEntity.coinAllianceCount;
	}

	public void increaseCoinAllianceCount(int count) {
		mfEntity.coinAllianceCount += count;
		mfUpdate = true;
	}

	public int getCoinAllianceCountDaily() {
		return mfEntity.coinAllianceCountDaily;
	}

	public void increaseCoinAllianceCountDaily(int count) {
		mfEntity.coinAllianceCountDaily += count;
		mfUpdate = true;
	}

	public void clearCoinAllianceCountDaily() {
		mfEntity.coinAllianceCountDaily = 0;
		mfUpdate = true;
	}

	// MF 2 method=========================================================================

	// guideFinish-------------------------------------------
	public Set<Integer> getGuideFinishSet() {
		return mf2Entity.guideFinishSet;
	}

	public void addGuideFinishList(List<Integer> guideList) {
		mf2Entity.guideFinishSet.addAll(guideList);
		mf2Entity.guideFinishFlag = true;
		mf2Update = true;
	}

	// GM only begin/////////////////
	public void clearGuideFinish() {
		mf2Entity.guideFinishSet.clear();
		mf2Entity.guideFinishFlag = true;
		mf2Update = true;
	}
	// GM only end/////////////////////

	// useItemCount--------------------------------------------
//	public Map<String, Integer> getUseItemCountMap() {
//		return mf2Entity.useItemCountMap;
//	}

	/**
	 * @return 物品使用次数，如未使用返回0
	 */
	public int getUseItemCount(String itemCfgId) {
		Integer count = mf2Entity.useItemCountMap.get(itemCfgId);
		if (null != count) {
			return count;
		}
		return 0;
	}

	public void increaseUseItemCount(String itemCfgId, int count) {
		Integer cur = mf2Entity.useItemCountMap.get(itemCfgId);
		if (null == cur) {
			cur = 0;
		}
		mf2Entity.useItemCountMap.put(itemCfgId, cur + count);
		mf2Entity.useItemCountFlag = true;
		mf2Update = true;
	}

//	public void setUseItemCount(String itemCfgId, int count) {
//		mf2Entity.useItemCountMap.put(itemCfgId, count);
//		mf2Entity.useItemCountFlag = true;
//		mf2Update = true;
//	}

	public Map<String, Integer> getUseItemCountDailyMap() {
		return mf2Entity.useItemCountDailyMap;
	}

	/**
	 * @return 物品使用次数，如未使用返回0
	 */
	public int getUseItemCountDaily(String itemCfgId) {
		Integer count = mf2Entity.useItemCountDailyMap.get(itemCfgId);
		if (null != count) {
			return count;
		}
		return 0;
	}

	public void increaseUseItemCountDaily(String itemCfgId, int count) {
		Integer curCount = mf2Entity.useItemCountDailyMap.get(itemCfgId);
		if (null == curCount) {
			curCount = 0;
		}
		mf2Entity.useItemCountDailyMap.put(itemCfgId, curCount + count);
		mf2Entity.useItemCountDailyFlag = true;
		mf2Update = true;
	}

//	public void setUseItemCountDaily(String itemCfgId, int count) {
//		mf2Entity.useItemCountDailyMap.put(itemCfgId, count);
//		mf2Entity.useItemCountDailyFlag = true;
//		mf2Update = true;
//	}

	public void clearUseItemCountDaily() {
		mf2Entity.useItemCountDailyMap.clear();
		mf2Entity.useItemCountDailyFlag = true;
		mf2Update = true;
	}

	// buyItemTimes--------------------------------------------
//	public Map<String, Integer> getBuyItemTimesMap() {
//		return mf2Entity.buyItemTimesMap;
//	}

	/**
	 * @return 物品购买次数，如未购买返回0
	 */
	public int getBuyItemTimes(String itemCfgId) {
		Integer times = mf2Entity.buyItemTimesMap.get(itemCfgId);
		if (null != times) {
			return times;
		}
		return 0;
	}

	public void increaseBuyItemTimes(String itemCfgId) {
		Integer cur = mf2Entity.buyItemTimesMap.get(itemCfgId);
		if (null == cur) {
			cur = 0;
		}
		mf2Entity.buyItemTimesMap.put(itemCfgId, cur + 1);
		mf2Entity.buyItemTimesFlag = true;
		mf2Update = true;
	}

	// upSkillTimes--------------------------------------------
	public int getUpSkillTimes() {
		return mf2Entity.upSkillTimes;
	}

	public void increaseUpSkillTimes() {
		++mf2Entity.upSkillTimes;
		mf2Update = true;
	}

	public int getUpSkillTimesDaily() {
		return mf2Entity.upSkillTimesDaily;
	}

	public void increaseUpSkillTimesDaily() {
		++mf2Entity.upSkillTimesDaily;
		mf2Update = true;
	}

	public void clearUpSkillTimesDaily() {
		mf2Entity.upSkillTimesDaily = 0;
		mf2Update = true;
	}

	// upEquipTimes--------------------------------------------
	public int getUpEquipTimes() {
		return mf2Entity.upEquipTimes;
	}

	public void increaseUpEquipTimes() {
		++mf2Entity.upEquipTimes;
		mf2Update = true;
	}

	public int getUpEquipTimesDaily() {
		return mf2Entity.upEquipTimesDaily;
	}

	public void increaseUpEquipTimesDaily() {
		++mf2Entity.upEquipTimesDaily;
		mf2Update = true;
	}

	public void clearUpEquipTimesDaily() {
		mf2Entity.upEquipTimesDaily = 0;
		mf2Update = true;
	}

	// LF method=========================================================================

	// monsterCollect------------------------------------------
	public Set<String> getMonsterCollectSet() {
		return lfEntity.monsterCollectSet;
	}

	public void addMonsterCollect(String monsterCfgId) {
		lfEntity.monsterCollectSet.add(monsterCfgId);
		lfEntity.monsterCollectFlag = true;
		lfUpdate = true;
	}

	// monsterMixTimes-----------------------------------------
	public int getMonsterMixTimes() {
		return lfEntity.monsterMixTimes;
	}

	public void increaseMonsterMixTimes() {
		++lfEntity.monsterMixTimes;
		lfUpdate = true;
	}

	// instanceResetTimes--------------------------------------
	public int getInstanceResetTimesDaily() {
		return lfEntity.instanceResetTimesDaily;
	}

	public void increaseInstanceResetTimesDaily() {
		++lfEntity.instanceResetTimesDaily;
		lfUpdate = true;
	}

	public void clearInstanceResetTimesDaily() {
		lfEntity.instanceResetTimesDaily = 0;
		lfUpdate = true;
	}

	// chapterBox----------------------------------------------
	public List<Integer> getNormalChapterBoxStateList() {
		return lfEntity.chapterBoxNormalList;
	}

	public int getNormalChapterBoxState(int chapterId) {
		// 从1开始
		int index = chapterId - 1;
		if (index >= lfEntity.chapterBoxNormalList.size()) {
			return Const.ChapterBoxState.INVALID_VALUE;
		}
		return lfEntity.chapterBoxNormalList.get(index);
	}

	public void setNormalChapterBoxState(int chapterId, int state) {
		// 从1开始
		int index = chapterId - 1;
		for (int i = lfEntity.chapterBoxNormalList.size(); i <= index; ++i) {
			lfEntity.chapterBoxNormalList.add(Const.ChapterBoxState.INVALID_VALUE);
		}
		lfEntity.chapterBoxNormalList.set(index, state);
		lfEntity.chapterBoxNormalFlag = true;
		lfUpdate = true;
	}

	public List<Integer> getHardChapterBoxStateList() {
		return lfEntity.chapterBoxHardList;
	}

	public int getHardChapterBoxState(int chapterId) {
		// 从1开始
		int index = chapterId - 1;
		if (index >= lfEntity.chapterBoxHardList.size()) {
			return Const.ChapterBoxState.INVALID_VALUE;
		}
		return lfEntity.chapterBoxHardList.get(index);
	}

	public void setHardChapterBoxState(int chapterId, int state) {
		// 从1开始
		int index = chapterId - 1;
		for (int i = lfEntity.chapterBoxHardList.size(); i <= index; ++i) {
			lfEntity.chapterBoxHardList.add(Const.ChapterBoxState.INVALID_VALUE);
		}
		lfEntity.chapterBoxHardList.set(index, state);
		lfEntity.chapterBoxHardFlag = true;
		lfUpdate = true;
	}

	// buyCoin-------------------------------------------------
	public int getBuyCoinTimes() {
		return lfEntity.buyCoinTimes;
	}

	public void increaseBuyCoinTimes() {
		++lfEntity.buyCoinTimes;
		lfUpdate = true;
	}

	public int getBuyCoinTimesDaily() {
		return lfEntity.buyCoinTimesDaily;
	}

	public void increaseBuyCoinTimesDaily() {
		++lfEntity.buyCoinTimesDaily;
		lfUpdate = true;
	}

	public void clearBuyCoinTimesDaily() {
		lfEntity.buyCoinTimesDaily = 0;
		lfUpdate = true;
	}

	// buyGift-------------------------------------------------
	// TODO 大礼包功能未开发，开发后统计数据
	public int getBuyGiftTimes() {
		return lfEntity.buyGiftTimes;
	}

	public void increaseBuyGiftTimes() {
		++lfEntity.buyGiftTimes;
		lfUpdate = true;
	}

	public int getBuyGiftTimesDaily() {
		return lfEntity.buyGiftTimesDaily;
	}

	public void increaseBuyGiftTimesDaily() {
		++lfEntity.buyGiftTimesDaily;
		lfUpdate = true;
	}

	public void clearBuyGiftTimesDaily() {
		lfEntity.buyGiftTimesDaily = 0;
		lfUpdate = true;
	}

	// payDiamond----------------------------------------------
	public int getPayDiamondCount() {
		return lfEntity.payDiamondCount;
	}

	public void increasePayDiamondCount(int count) {
		lfEntity.payDiamondCount += count;
		lfUpdate = true;
	}

	public int getPayDiamondCountDaily() {
		return lfEntity.payDiamondCountDaily;
	}

	public void increasePayDiamondCountDaily(int count) {
		lfEntity.payDiamondCountDaily += count;
		lfUpdate = true;
	}

	public void clearPayDiamondCountDaily() {
		lfEntity.payDiamondCountDaily = 0;
		lfUpdate = true;
	}

	// useDiamond----------------------------------------------
	public int getUseDiamondCount() {
		return lfEntity.useDiamondCount;
	}

	public void increaseUseDiamondCount(int count) {
		lfEntity.useDiamondCount += count;
		lfUpdate = true;
	}

	public int getUseDiamondCountDaily() {
		return lfEntity.useDiamondCountDaily;
	}

	public void increaseUseDiamondCountDaily(int count) {
		lfEntity.useDiamondCountDaily += count;
		lfUpdate = true;
	}

	public void clearUseDiamondCountDaily() {
		lfEntity.useDiamondCountDaily = 0;
		lfUpdate = true;
	}

	// equipStageCount-----------------------------------------
//	public List<Integer> getEquipMaxCountOverStageList() {
//		return lfEntity.equipStageCountList;
//	}

	public int getEquipMaxCountOverStage(int stage) {
		// 从1开始
		int index = stage - 1;
		if (index >= lfEntity.equipStageMaxCountList.size()) {
			return 0;
		}
		return lfEntity.equipStageMaxCountList.get(index);
	}

	public void setEquipMaxCountOverStage(int stage, int count) {
		// 从1开始
		int index = stage - 1;
		for (int i = lfEntity.equipStageMaxCountList.size(); i <= index; ++i) {
			lfEntity.equipStageMaxCountList.add(0);
		}
		lfEntity.equipStageMaxCountList.set(index, count);
		lfEntity.equipStageCountFlag = true;
		lfUpdate = true;
	}

//	public List<Integer> getEquipMaxCountOverStageDailyList() {
//		return lfEntity.equipStageCountDailyList;
//	}

	public int getEquipMaxCountOverStageDaily(int stage) {
		// 从1开始
		int index = stage - 1;
		if (index >= lfEntity.equipStageMaxCountDailyList.size()) {
			return 0;
		}
		return lfEntity.equipStageMaxCountDailyList.get(index);
	}

	public void setEquipMaxCountOverStageDaily(int stage, int count) {
		// 从1开始
		int index = stage - 1;
		for (int i = lfEntity.equipStageMaxCountDailyList.size(); i <= index; ++i) {
			lfEntity.equipStageMaxCountDailyList.add(0);
		}
		lfEntity.equipStageMaxCountDailyList.set(index, count);
		lfEntity.equipStageCountDailyFlag = true;
		lfUpdate = true;
	}

	public void clearEquipStageMaxCountDaily() {
		lfEntity.equipStageMaxCountDailyList.clear();
		lfEntity.equipStageCountDailyFlag = true;
		lfUpdate = true;
	}

	// equipPunch----------------------------------------------
	public int getEquipPunchTimes() {
		return lfEntity.equipPunchTimes;
	}

	public void increaseEquipPunchTimes() {
		++lfEntity.equipPunchTimes;
		lfUpdate = true;
	}

	public int getEquipPunchTimesDaily() {
		return lfEntity.equipPunchTimesDaily;
	}

	public void increaseEquipPunchTimesDaily() {
		++lfEntity.equipPunchTimesDaily;
		lfUpdate = true;
	}

	public void clearEquipPunchTimesDaily() {
		lfEntity.equipPunchTimesDaily = 0;
		lfUpdate = true;
	}

	// inlayTimes----------------------------------------------
	public int getInlayAllTimes() {
		return lfEntity.inlayAllTimes;
	}

	public void increaseInlayAllTimes() {
		++lfEntity.inlayAllTimes;
		lfUpdate = true;
	}

//	public List<Integer> getInlayTypeTimesList() {
//		return lfEntity.inlayTypeTimesList;
//	}

	public int getInlayTypeTimes(int type) {
		// 从1开始
		int index = type - 1;
		if (index >= lfEntity.inlayTypeTimesList.size()) {
			return 0;
		}
		return lfEntity.inlayTypeTimesList.get(index);
	}

	public void increaseInlayTypeTimes(int type) {
		// 从1开始
		int index = type - 1;
		for (int i = lfEntity.inlayTypeTimesList.size(); i <= index; ++i) {
			lfEntity.inlayTypeTimesList.add(0);
		}
		lfEntity.inlayTypeTimesList.set(index, lfEntity.inlayTypeTimesList.get(index) + 1);
		lfEntity.inlayTypeTimesFlag = true;
		lfUpdate = true;
	}

	// synTimes------------------------------------------------
	public int getSynAllTimes() {
		return lfEntity.synAllTimes;
	}

	public void increaseSynAllTimes() {
		++lfEntity.synAllTimes;
		lfUpdate = true;
	}

	public int getSynAllTimesDaily() {
		return lfEntity.synAllTimesDaily;
	}

	public void increaseSynAllTimesDaily() {
		++lfEntity.synAllTimesDaily;
		lfUpdate = true;
	}

	public void clearSynAllTimesDaily() {
		lfEntity.synAllTimesDaily = 0;
		lfUpdate = true;
	}

//	public List<Integer> getSynTypeTimesList() {
//		return lfEntity.synTypeTimesList;
//	}

	public int getSynTypeTimes(int type) {
		// 从1开始
		int index = type - 1;
		if (index >= lfEntity.synTypeTimesList.size()) {
			return 0;
		}
		return lfEntity.synTypeTimesList.get(index);
	}

	public void increaseSynTypeTimes(int type) {
		// 从1开始
		int index = type - 1;
		for (int i = lfEntity.synTypeTimesList.size(); i <= index; ++i) {
			lfEntity.synTypeTimesList.add(0);
		}
		lfEntity.synTypeTimesList.set(index, lfEntity.synTypeTimesList.get(index) + 1);
		lfEntity.synTypeTimesFlag = true;
		lfUpdate = true;
	}

	// LF 2 method=========================================================================

	// adventureChange----------------------------------------------
	public int getAdventureChange() {
		return lf2Entity.adventureChange;
	}

	public void setAdventureChange(int times) {
		lf2Entity.adventureChange = times;
		lf2Update = true;
	}

	public void increaseAdventureChange() {
		++lf2Entity.adventureChange;
		lf2Update = true;
	}

	public Calendar getAdventureChangeBeginTime() {
		return lf2Entity.adventureChangeBeginTime;
	}

	public void setAdventureChangeBeginTime(Calendar time) {
		lf2Entity.adventureChangeBeginTime = time;
		lf2Update = true;
	}

	// eggTimes------------------------------------------------
	public int getEggCoin1Times() {
		return lf2Entity.eggCoin1Times;
	}

	public void increaseEggCoin1Times() {
		++lf2Entity.eggCoin1Times;
		lf2Update = true;
	}

	public int getEggCoin1FreeTimesDaily() {
		return lf2Entity.eggCoin1FreeTimesDaily;
	}

	public void increaseEggCoin1FreeTimesDaily() {
		++lf2Entity.eggCoin1FreeTimesDaily;
		lf2Update = true;
	}

	public void clearEggCoin1FreeTimesDaily() {
		lf2Entity.eggCoin1FreeTimesDaily = 0;
		lf2Update = true;
	}

	public int getEggCoin1PayTimesDaily() {
		return lf2Entity.eggCoin1PayTimesDaily;
	}

	public void increaseEggCoin1PayTimesDaily() {
		++lf2Entity.eggCoin1PayTimesDaily;
		lf2Update = true;
	}

	public void clearEggCoin1PayTimesDaily() {
		lf2Entity.eggCoin1PayTimesDaily = 0;
		lf2Update = true;
	}

	public int getEggCoin10Times() {
		return lf2Entity.eggCoin10Times;
	}

	public void increaseEggCoin10Times() {
		++lf2Entity.eggCoin10Times;
		lf2Update = true;
	}

	public int getEggCoin10TimesDaily() {
		return lf2Entity.eggCoin10TimesDaily;
	}

	public void increaseEggCoin10TimesDaily() {
		++lf2Entity.eggCoin10TimesDaily;
		lf2Update = true;
	}

	public void clearEggCoin10TimesDaily() {
		lf2Entity.eggCoin10TimesDaily = 0;
		lf2Update = true;
	}

	public Calendar getEggCoin1FreeLastTime() {
		return lf2Entity.eggCoin1FreeLastTime;
	}

	public void setEggCoin1FreeLastTime(Calendar time) {
		lf2Entity.eggCoin1FreeLastTime = time;
		lf2Update = true;
	}

	public int getEggDiamond1FreeTimes() {
		return lf2Entity.eggDiamond1FreeTimes;
	}

	public void increaseEggDiamond1FreeTimes() {
		++lf2Entity.eggDiamond1FreeTimes;
		lf2Update = true;
	}

	public int getEggDiamond1PayTimes() {
		return lf2Entity.eggDiamond1PayTimes;
	}

	public void increaseEggDiamond1PayTimes() {
		++lf2Entity.eggDiamond1PayTimes;
		lf2Update = true;
	}

	public int getEggDiamond10Times() {
		return lf2Entity.eggDiamond10Times;
	}

	public void increaseEggDiamond10Times() {
		++lf2Entity.eggDiamond10Times;
		lf2Update = true;
	}

	public int getEggDiamond1FreePoint() {
		return lf2Entity.eggDiamond1FreePoint;
	}

	public void setEggDiamond1FreePoint(int point) {
		lf2Entity.eggDiamond1FreePoint = point;
		lf2Update = true;
	}

	public Calendar getEggDiamond1FreePointBeginTime() {
		return lf2Entity.eggDiamond1FreePointBeginTime;
	}

	public void setEggDiamond1FreePointBeginTime(Calendar time) {
		lf2Entity.eggDiamond1FreePointBeginTime = time;
		lf2Update = true;
	}

	// callMonsterStageTimes----------------------------------------
	public List<Integer> getCallMonsterStageTimesList() {
		return lf2Entity.callMonsterStageTimesList;
	}

	public int getCallMonsterStageTimes(int stage) {
		// 从0开始
		int index = stage;
		if (index >= lf2Entity.callMonsterStageTimesList.size()) {
			return 0;
		}
		return lf2Entity.callMonsterStageTimesList.get(index);
	}

	public void increaseCallMonsterStageTimes(int stage) {
		// 从0开始
		int index = stage;
		for (int i = lf2Entity.callMonsterStageTimesList.size(); i <= index; ++i) {
			lf2Entity.callMonsterStageTimesList.add(0);
		}
		lf2Entity.callMonsterStageTimesList.set(index, lf2Entity.callMonsterStageTimesList.get(index) + 1);
		lf2Entity.callMonsterStageTimesFlag = true;
		lf2Update = true;
	}

	// callEquipStageTimes------------------------------------------
	public List<Integer> getCallEquipStageTimesList() {
		return lf2Entity.callEquipStageTimesList;
	}

	public int getCallEquipStageTimes(int stage) {
		// 从1开始
		int index = stage - 1;
		if (index >= lf2Entity.callEquipStageTimesList.size()) {
			return 0;
		}
		return lf2Entity.callEquipStageTimesList.get(index);
	}

	public void increaseCallEquipStageTimes(int stage) {
		// 从1开始
		int index = stage - 1;
		for (int i = lf2Entity.callEquipStageTimesList.size(); i <= index; ++i) {
			lf2Entity.callEquipStageTimesList.add(0);
		}
		lf2Entity.callEquipStageTimesList.set(index, lf2Entity.callEquipStageTimesList.get(index) + 1);
		lf2Entity.callEquipStageTimesFlag = true;
		lf2Update = true;
	}

	// alliance------------------------------------------------
	public int getAlliancePrayTimes() {
		return lf2Entity.alliancePrayTimes;
	}

	public void increaseAlliancePrayTimes() {
		++lf2Entity.alliancePrayTimes;
		lf2Update = true;
	}

	public int getAlliancePrayTimesDaily() {
		return lf2Entity.alliancePrayTimesDaily;
	}

	public void increaseAlliancePrayTimesDaily() {
		++lf2Entity.alliancePrayTimesDaily;
		lf2Update = true;
	}

	public void clearAlliancePrayTimesDaily() {
		lf2Entity.alliancePrayTimesDaily = 0;
		lf2Update = true;
	}

	// TODO 公会boss功能未开发，开发后统计数据
	public int getAllianceBossTimes() {
		return lf2Entity.allianceBossTimes;
	}

	public void increaseAllianceBossTimes() {
		++lf2Entity.allianceBossTimes;
		lf2Update = true;
	}

	public int getAllianceFatigueTimes() {
		return lf2Entity.allianceFatigueTimes;
	}

	public void increaseAllianceFatigueTimes() {
		++lf2Entity.allianceFatigueTimes;
		lf2Update = true;
	}

	public int getAllianceFatigueTimesDaily() {
		return lf2Entity.allianceFatigueTimesDaily;
	}

	public void increaseAllianceFatigueTimesDaily() {
		++lf2Entity.allianceFatigueTimesDaily;
		lf2Update = true;
	}

	public void clearAllianceFatigueTimesDaily() {
		lf2Entity.allianceFatigueTimesDaily = 0;
		lf2Update = true;
	}

	public int getAllianceTaskCountDaily() {
		return lf2Entity.allianceTaskCountDaily;
	}

	public void increaseAllianceTaskCountDaily() {
		++lf2Entity.allianceTaskCountDaily;
		lf2Update = true;
	}

	public void clearAllianceTaskCountDaily() {
		lf2Entity.allianceTaskCountDaily = 0;
		lf2Update = true;
	}

	public int getAllianceContriRewardDaily() {
		return lf2Entity.allianceContriRewardDaily;
	}

	public void setAllianceContriRewardDaily(int index) {
		if (index == 0) {
			lf2Entity.allianceContriRewardDaily |= 1;
		} else if (index == 1) {
			lf2Entity.allianceContriRewardDaily |= 2;
		} else if (index == 2) {
			lf2Entity.allianceContriRewardDaily |= 4;
		} else {
			return;
		}
		lf2Update = true;
	}

	public void clearAllianceContriRewardDaily() {
		lf2Entity.allianceContriRewardDaily = 0;
		lf2Update = true;
	}

	// shopRefresh---------------------------------------------
	public int getShopRefreshTimes() {
		return lf2Entity.shopRefreshTimes;
	}

	public void increaseShopRefreshTimes() {
		++lf2Entity.shopRefreshTimes;
		lf2Update = true;
	}

	// rechargeTimes-------------------------------------------
	public Map<String, Integer> getRechargeRecordMap() {
		return lf2Entity.rechargeRecordMap;
	}

	public int getRechargeTimes(String productId){
		Integer times = lf2Entity.rechargeRecordMap.get(productId);
		if (null != times) {
			return times;
		}
		return 0;
	}

	public void increaseRechargeRecord(String productId) {
		Integer cur = lf2Entity.rechargeRecordMap.get(productId);
		if (null == cur) {
			cur = 0;
		}
		lf2Entity.rechargeRecordMap.put(productId, cur + 1);
		lf2Entity.rechargeRecordFlag = true;
		lf2Update = true;
	}

	// monthCard-----------------------------------------------
	public Calendar getMonthCardEndTime() {
		return lf2Entity.monthCardEndTime;
	}

	public void addMonthCard() {
		if (null == lf2Entity.monthCardEndTime || HawkTime.getCalendar().compareTo(lf2Entity.monthCardEndTime) > 0) {
			lf2Entity.monthCardEndTime = HawkTime.getCalendar();
		}
		lf2Entity.monthCardEndTime.add(Calendar.DATE, GsConst.MONTH_CARD_TIME);
		lf2Update = true;
	}

	// signInTimes---------------------------------------------
	public int getSigninTimesMonthly() {
		return lf2Entity.signinTimesMonthly;
	}

	public void increaseSigninTimesMonthly() {
		++lf2Entity.signinTimesMonthly;
		lf2Update = true;
	}

	public void clearSigninTimesMonthly() {
		lf2Entity.signinTimesMonthly = 0;
		lf2Update = true;
	}

	// signInFillTimes-----------------------------------------
	public int getSigninFillTimesMonthly() {
		return lf2Entity.signinFillTimesMonthly;
	}

	public void increaseSigninFillTimesMonthly() {
		++lf2Entity.signinFillTimesMonthly;
		lf2Update = true;
	}

	public void clearSigninFillTimesMonthly() {
		lf2Entity.signinFillTimesMonthly = 0;
		lf2Update = true;
	}

	// signinDaily----------------------------------------------
	public boolean isSigninDaily() {
		return lf2Entity.signinDaily;
	}

	public void setSigninDaily(boolean isSignin) {
		lf2Entity.signinDaily = isSignin;
		lf2Update = true;
	}

	// loginTimes----------------------------------------------
	public int getLoginTimes() {
		return lf2Entity.loginTimes;
	}

	public void increaseLoginTimes() {
		++lf2Entity.loginTimes;
		lf2Update = true;
	}

	public int getLoginTimesDaily() {
		return lf2Entity.loginTimesDaily;
	}

	public void increaseLoginTimesDaily() {
		++lf2Entity.loginTimesDaily;
		lf2Update = true;
	}

	public void clearLoginTimesDaily() {
		lf2Entity.loginTimesDaily = 0;
		lf2Update = true;
	}

	// onlineTime----------------------------------------------
	public long getTotalOnlineTime() {
		return lf2Entity.totalOnlineTime;
	}

	public void increaseTotalOnlineTime(long onlineTime) {
		lf2Entity.totalOnlineTime += onlineTime;
		lf2Update = true;
	}

	// dumpTime----------------------------------------------
	public int getDumpTime() {
		return lf2Entity.dumpTime;
	}

	public void setDumpTime(int dumpTime) {
		lf2Entity.dumpTime = dumpTime;
		lf2Update = true;
	}

	// Refresh method=========================================================================

	// refreshTime---------------------------------------------
	public Calendar getLastRefreshTime(int timeCfgId) {
		RefreshStatisticsEntity refreshEntity = refreshEntityMap.get(timeCfgId);
		if (null != refreshEntity) {
			return refreshEntity.refreshTime;
		}
		return null;
	}

	public void setLastRefreshTime(int timeCfgId, Calendar time) {
		RefreshStatisticsEntity refreshEntity = refreshEntityMap.get(timeCfgId);
		if (null == refreshEntity) {
			refreshEntity = new RefreshStatisticsEntity(getPlayerId(),timeCfgId);
			refreshEntity.refreshTime = time;
			refreshEntity.notifyCreate();
			refreshEntityMap.put(timeCfgId, refreshEntity);
		} else {
			refreshEntity.refreshTime = time;
			refreshUpdateMap.put(timeCfgId, true);
		}
	}
}