package com.hawk.game.entity;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;
import java.util.Set;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Transient;

import org.hawk.config.HawkConfigManager;
import org.hawk.db.HawkDBEntity;
import org.hawk.os.HawkException;
import org.hawk.os.HawkTime;
import org.hawk.util.HawkJsonUtil;

import com.google.gson.reflect.TypeToken;
import com.hawk.game.config.InstanceEntryCfg;
import com.hawk.game.config.QuestCfg;
import com.hawk.game.protocol.Const;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.GsConst.Cycle;

/**
 * 玩家统计数据
 * 
 * @author walker
 * 
 */
@Entity
@Table(name = "statistics")
public class StatisticsEntity  extends HawkDBEntity {
	@Id
	@Column(name = "playerId", unique = true)
	private int playerId = 0;

	// 活力值
	@Column(name = "fatigue", nullable = false)
	private int fatigue = 0;

	// 活力值开始计时时间
	@Column(name = "fatigueBeginTime")
	private Calendar fatigueBeginTime = null;

	// 技能点
	@Column(name = "skillPoint", nullable = false)
	private int skillPoint = 0;

	// 技能点开始计时时间
	@Column(name = "skillPointBeginTime")
	private Calendar skillPointBeginTime = null;

	// 任务状态，记录已完成任务Id
	@Column(name = "questComplete", nullable = false)
	private String questCompleteJson = "";

	// 每日任务状态，记录已完成任务Id
	@Column(name = "questCompleteDaily", nullable = false)
	private String questCompleteDailyJson = "";

	// 怪物图鉴
	@Column(name = "monsterCollect", nullable = false)
	private String monsterCollectJson = "";

	// 各副本完成星级：1-3
	@Column(name = "instanceStar", nullable = false)
	private String instanceStarJson = "";

	// 普通副本章节宝箱状态
	@Column(name = "chapterBoxNormal", nullable = false)
	private String chapterBoxNormalJson = "";

	// 困难副本章节宝箱状态
	@Column(name = "chapterBoxHard", nullable = false)
	private String chapterBoxHardJson = "";

	// 今日各副本完成次数
	@Column(name = "instanceCountDaily", nullable = false)
	private String instanceCountDailyJson = "";

	// 历史达到特定品级宠物数量
	@Column(name = "monsterStage", nullable = false)
	private String monsterStageJson = "";

	// 历史达到特定等级宠物数量
	@Column(name = "monsterLevel", nullable = false)
	private String monsterLevelJson = "";

	// 历史同时最多宠物数量
	@Column(name = "monsterMaxCount", nullable = false)
	private int monsterMaxCount = 0;

	// 历史最高宠物等级
	@Column(name = "monsterMaxLevel", nullable = false)
	private int monsterMaxLevel = 0;

	// 历史最高宠物品级
	@Column(name = "monsterMaxStage", nullable = false)
	private int monsterMaxStage = 0;

	// 今日副本挑战次数重置次数
	@Column(name = "instanceResetCountDaily", nullable = false)
	private int instanceResetCountDaily = 0;

	// 历史副本完成次数
	@Column(name = "instanceAllCount", nullable = false)
	private int instanceAllCount = 0;

	// 今日副本完成次数
	@Column(name = "instanceAllCountDaily", nullable = false)
	private int instanceAllCountDaily = 0;

	// 历史精英副本完成次数
	@Column(name = "hardCount", nullable = false)
	private int hardCount = 0;

	// 今日精英副本完成次数
	@Column(name = "hardCountDaily", nullable = false)
	private int hardCountDaily = 0;

	// 历史竞技场次数
	@Column(name = "arenaCount", nullable = false)
	private int arenaCount = 0;

	// 今日竞技场次数
	@Column(name = "arenaCountDaily", nullable = false)
	private int arenaCountDaily = 0;

	// 历史洞总次数
	@Column(name = "holeCount", nullable = false)
	private int holeCount = 0;

	// 今日各洞次数
	@Column(name = "holeCountDaily", nullable = false)
	private String holeCountDailyJson = "";

	// 各塔已完成层数
	@Column(name = "towerFloor", nullable = false)
	private String towerFloorJson = "";

	// 历史炼妖炉次数
	@Column(name = "monsterMixCount", nullable = false)
	private int monsterMixCount = 0;

	// 今日炼妖炉次数
	@Column(name = "monsterMixCountDaily", nullable = false)
	private int monsterMixCountDaily = 0;

	// 历史大冒险次数
	@Column(name = "adventureCount", nullable = false)
	private int adventureCount = 0;

	// 今日大冒险次数
	@Column(name = "adventureCountDaily", nullable = false)
	private int adventureCountDaily = 0;

	// 历史bossrush次数
	@Column(name = "bossrushCount", nullable = false)
	private int bossrushCount = 0;

	// 今日bossrush次数
	@Column(name = "bossrushCountDaily", nullable = false)
	private int bossrushCountDaily = 0;

	// 历史稀有探索次数
	@Column(name = "exploreCount", nullable = false)
	private int exploreCount = 0;

	// 今日稀有探索次数
	@Column(name = "exploreCountDaily", nullable = false)
	private int exploreCountDaily = 0;

	// 历史提升技能次数
	@Column(name = "skillUpCount", nullable = false)
	private int skillUpCount = 0;

	// 今日提升技能次数
	@Column(name = "skillUpCountDaily", nullable = false)
	private int skillUpCountDaily = 0;

	// 历史升级装备次数
	@Column(name = "equipUpCount", nullable = false)
	private int equipUpCount = 0;

	// 今日升级装备次数
	@Column(name = "equipUpCountDaily", nullable = false)
	private int equipUpCountDaily = 0;

	// 历史钻石买钱次数
	@Column(name = "coinOrderCount", nullable = false)
	private int coinOrderCount = 0;

	// 今日钻石买钱次数
	@Column(name = "coinOrderCountDaily", nullable = false)
	private int coinOrderCountDaily = 0;

	// 历史领取体力次数
	@Column(name = "fatigueClaimCount", nullable = false)
	private int fatigueClaimCount = 0;

	// 今日领取体力次数
	@Column(name = "fatigueClaimCountDaily", nullable = false)
	private int fatigueClaimCountDaily = 0;

	// 今日物品使用次数
	@Column(name = "itemUseCountDaily", nullable = false)
	private String itemUseCountDailyJson = "";

	// 今日工会祈福次数
	@Column(name = "alliancePrayCountDaily", nullable = false)
	private int alliancePrayCountDaily = 0;

	// 今日公会任务数
	@Column(name = "allianceTaskCountDaily", nullable = false)
	private int allianceTaskCountDaily = 0;

	// 商品充值次数记录
	@Column(name = "rechargeRecord", nullable = false)
	private String rechargeRecordJson = "";

	// 月卡结束时间
	@Column(name = "monthCardEndTime")
	private Calendar monthCardEndTime = null;

	// 刷新时间
	@Column(name = "refreshTime")
	private String refreshTimeJson = "";

	// 签到次数
	@Column(name = "signInCount", nullable = false)
	private byte signInCount = 0;

	// 双倍剩余经验次数
	@Column(name = "doubleExpLeft", nullable = false)
	private byte doubleExpLeft = 0;

	// 三倍剩余经验次数
	@Column(name = "tripleExpLeft", nullable = false)
	private byte tripleExpLeft = 0;

	@Column(name = "loginCount", nullable = false)
	private int loginCount = 0;

	@Column(name = "totalOnlineTime", nullable = false)
	private long totalOnlineTime = 0;

	@Column(name = "createTime", nullable = false)
	protected int createTime = 0;

	@Column(name = "updateTime")
	protected int updateTime = 0;

	@Column(name = "invalid", nullable = false)
	protected boolean invalid = false;

	// 每一项刷新时间
	@Transient
	protected Map<Integer, Calendar> refreshTimeMap = new HashMap<Integer, Calendar>();
	@Transient
	protected Map<Integer, String> refreshStringMap = new HashMap<Integer, String>();
	@Transient
	boolean refreshTimeFlag = false;
	@Transient
	protected Set<Integer> questCompleteSet = new HashSet<Integer>();
	@Transient
	boolean questCompleteFlag = false;
	@Transient
	protected Set<Integer> questCompleteDailySet = new HashSet<Integer>();
	@Transient
	boolean questCompleteDailyFlag = false;
	@Transient
	protected Map<String, Integer> instanceStarMap = new HashMap<String, Integer>();
	@Transient
	boolean instanceStarFlag = false;
	@Transient
	protected List<Integer> chapterBoxNormalList = new ArrayList<Integer>();
	@Transient
	boolean chapterBoxNormalFlag = false;
	@Transient
	protected List<Integer> chapterBoxHardList = new ArrayList<Integer>();
	@Transient
	boolean chapterBoxHardFlag = false;
	@Transient
	protected Map<String, Integer> instanceCountDailyMap = new HashMap<String, Integer>();
	@Transient
	boolean  instanceCountDailyFlag = false;
	@Transient
	protected Set<String> monsterCollectSet = new HashSet<String>();
	@Transient
	boolean monsterCollectFlag = false;
	@Transient
	protected Map<Integer, Integer> monsterStageMap = new HashMap<Integer, Integer>();
	@Transient
	boolean monsterStageFlag = false;
	@Transient
	protected Map<Integer, Integer> monsterLevelMap = new HashMap<Integer, Integer>();
	@Transient
	boolean monsterLevelFlag = false;
	@Transient
	protected Map<Integer, Integer> holeCountDailyMap = new HashMap<Integer, Integer>();
	@Transient
	boolean holeCountDailyFlag = false;
	@Transient
	protected Map<Integer, Integer> towerFloorMap = new HashMap<Integer, Integer>();
	@Transient
	boolean towerFloorFlag = false;
	@Transient
	protected Map<String, Integer> rechargeRecordMap = new HashMap<String, Integer>();
	@Transient
	boolean rechargeRecordFlag = false;
	@Transient
	protected Map<String, Integer> itemUseCountDailyMap = new HashMap<String, Integer>();
	@Transient
	boolean itemUseCountDailyFlag = false;

	// 最后普通副本所属章节
	@Transient
	protected int normalTopChapter = 0;
	// 最后困难副本所属章节
	@Transient
	protected int hardTopChapter = 0;
	// 最后普通副本章节索引
	@Transient
	protected int normalTopIndex = 0;
	// 最后困难副本章节索引
	@Transient
	protected int hardTopIndex = 0;
	@Transient
	SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");

	public StatisticsEntity() {
		Calendar time = HawkTime.getCalendar();
		this.fatigueBeginTime = time;
		this.skillPointBeginTime = time;
	}

	public StatisticsEntity(int playerId) {
		Calendar time = HawkTime.getCalendar();
		this.playerId = playerId;
		this.fatigueBeginTime = time;
		this.skillPointBeginTime = time;
	}

	public int getPlayerId() {
		return playerId;
	}

	public void setPlayerId(int playerId) {
		this.playerId = playerId;
	}

	public Calendar getLastRefreshTime(int timeCfgId) {
		return refreshTimeMap.get(timeCfgId);
	}

	public void setRefreshTime(int timeCfgId, Calendar time) {
		this.refreshTimeMap.put(timeCfgId, time);
		refreshTimeFlag = true;
	}

	public int getFatigue() {
		return fatigue;
	}

	public void setFatigue(int fatigue) {
		this.fatigue = fatigue;
	}

	public Calendar getFatigueBeginTime() {
		return fatigueBeginTime;
	}

	public void setFatigueBeginTime(Calendar time) {
		this.fatigueBeginTime = time;
	}

	public int getSkillPoint() {
		return skillPoint;
	}

	public void setSkillPoint(int point) {
		this.skillPoint = point;
	}

	public Calendar getSkillPointBeginTime() {
		return skillPointBeginTime;
	}

	public void setSkillPointBeginTime(Calendar time) {
		this.skillPointBeginTime = time;
	}

	public Set<Integer> getQuestCompleteSet() {
		return questCompleteSet;
	}

	public Set<Integer> getQuestCompleteDailySet() {
		return questCompleteDailySet;
	}

	public void addQuestComplete(int questId) {
		QuestCfg questCfg = HawkConfigManager.getInstance().getConfigByKey(QuestCfg.class, questId);
		if (questCfg == null) {
			return;
		}

		if (questCfg.getCycle() == Cycle.DAILY_CYCLE) {
			questCompleteDailySet.add(questId);
			questCompleteDailyFlag = true;
		} else {
			questCompleteSet.add(questId);
			questCompleteFlag = true;
		}
	}

	public void clearQuestCompleteDaily() {
		questCompleteDailySet.clear();
		questCompleteDailyFlag = true;
	}

	// GM begin--------------------------------------------------------------------
	public void removeQuestComplete(int questId) {
		QuestCfg questCfg = HawkConfigManager.getInstance().getConfigByKey(QuestCfg.class, questId);
		if (questCfg == null) {
			return;
		}

		if (questCfg.getCycle() == Cycle.DAILY_CYCLE) {
			questCompleteDailySet.remove(questId);
			questCompleteDailyFlag = true;
		} else {
			questCompleteSet.remove(questId);
			questCompleteFlag = true;
		}
	}
	public void clearQuestComplete() {
		questCompleteSet.clear();
		questCompleteFlag = true;
	}
	// Gm end------------------------------------------------------------------------

	public Set<String> getMonsterCollectSet() {
		return monsterCollectSet;
	}

	public void addMonsterCollect(String monsterCfgId) {
		monsterCollectSet.add(monsterCfgId);
		monsterCollectFlag = true;
	}

	public Map<String, Integer> getInstanceStarMap() {
		return instanceStarMap;
	}

	public void setInstanceStar(String instanceId, int starCount) {
		instanceStarMap.put(instanceId, starCount);
		instanceStarFlag = true;
	}
	/**
	 * @return 副本完成星级，如未完成返回0
	 */
	public int getInstanceStar(String instanceId) {
		Integer star = instanceStarMap.get(instanceId);
		if (null != star) {
			return star;
		}
		return 0;
	}

	public List<Integer> getNormalChapterBoxStateList() {
		return chapterBoxNormalList;
	}

	public int getNormalChapterBoxState(int chapterId) {
		if (chapterId > chapterBoxNormalList.size()) {
			return Const.ChapterBoxState.INVALID_VALUE;
		}
		return chapterBoxNormalList.get(chapterId - 1);
	}

	public void setNormalChapterBoxState(int chapterId, int state) {
		for (int i = chapterBoxNormalList.size(); i < chapterId; ++i) {
			chapterBoxNormalList.add(Const.ChapterBoxState.INVALID_VALUE);
		}
		chapterBoxNormalList.set(chapterId - 1, state);
		chapterBoxNormalFlag = true;
	}

	public List<Integer> getHardChapterBoxStateList() {
		return chapterBoxHardList;
	}

	public int getHardChapterBoxState(int chapterId) {
		if (chapterId > chapterBoxHardList.size()) {
			return Const.ChapterBoxState.INVALID_VALUE;
		}
		return chapterBoxHardList.get(chapterId - 1);
	}

	public void setHardChapterBoxState(int chapterId, int state) {
		for (int i = chapterBoxHardList.size(); i < chapterId; ++i) {
			chapterBoxHardList.add(Const.ChapterBoxState.INVALID_VALUE);
		}
		chapterBoxHardList.set(chapterId - 1, state);
		chapterBoxHardFlag = true;
	}

	/**
	 * @return 副本完成次数，如未完成返回0
	 */
	public int getInstanceCountDaily(String instanceId) {
		Integer count = instanceCountDailyMap.get(instanceId);
		if (null != count) {
			return count;
		}
		return 0;
	}

	public Map<String, Integer> getInstanceCountDailyMap() {
		return instanceCountDailyMap;
	}

	public void addInstanceCountDaily(String instanceId, int addCount) {
		int curCount = 0;
		Integer oldCount = instanceCountDailyMap.get(instanceId);
		if (null != oldCount) {
			curCount = oldCount;
		}
		instanceCountDailyMap.put(instanceId, curCount + addCount);
		instanceCountDailyFlag = true;
	}

	public void setInstanceCountDaily(String instanceId, int count) {
		instanceCountDailyMap.put(instanceId, count);
		instanceCountDailyFlag = true;
	}

	public void clearInstanceCountDaily() {
		instanceCountDailyMap.clear();
		instanceCountDailyFlag = true;
	}

	public Map<Integer, Integer> getMonsterCountOverStageMap() {
		return monsterStageMap;
	}

	public int getMonsterCountOverStage(int stage) {
		Integer count = monsterStageMap.get(stage);
		if (null != count) {
			return count;
		}
		return 0;
	}

	public void setMonsterCountOverStage(int stage, int count) {
		monsterStageMap.put(stage, count);
		monsterStageFlag = true;
	}

	public Map<Integer, Integer> getMonsterCountOverLevelMap() {
		return monsterLevelMap;
	}

	public int getMonsterCountOverLevel(int level) {
		Integer count = monsterLevelMap.get(level);
		if (null != count) {
			return count;
		}
		return 0;
	}

	public void setMonsterCountOverLevel(int level, int count) {
		monsterLevelMap.put(level, count);
		monsterLevelFlag = true;
	}

	/**
	 * @return 洞完成次数，如未完成返回0
	 */
	public int getHoleCountDaily(int holeId) {
		Integer count = holeCountDailyMap.get(holeId);
		if (null != count) {
			return count;
		}
		return 0;
	}

	public Map<Integer, Integer> getHoleCountDailyMap() {
		return holeCountDailyMap;
	}

	public void addHoleCountDaily(int holeId, int addCount) {
		int curCount = 0;
		Integer oldCount = holeCountDailyMap.get(holeId);
		if (null != oldCount) {
			curCount = oldCount;
		}
		holeCountDailyMap.put(holeId, curCount + addCount);
		holeCountDailyFlag = true;
	}

	public void setHoleCountDaily(int holeId, int count) {
		holeCountDailyMap.put(holeId, count);
		holeCountDailyFlag = true;
	}

	public void clearHoleCountDaily() {
		holeCountDailyMap.clear();
		holeCountDailyFlag = true;
	}

	/**
	 * @return 塔完成层数，如未完成返回0
	 */
	public int getTowerFloor(int towerId) {
		Integer index = towerFloorMap.get(towerId);
		if (null != index) {
			return index;
		}
		return 0;
	}

	public Map<Integer, Integer> getTowerFloorMap() {
		return towerFloorMap;
	}

	public void addTowerFloor(int towerId, int addCount) {
		int curIndex = 0;
		Integer oldIndex = towerFloorMap.get(towerId);
		if (null != oldIndex) {
			curIndex = oldIndex;
		}
		towerFloorMap.put(towerId, curIndex + addCount);
		towerFloorFlag = true;
	}

	public void setTowerFloor(int towerId, int index) {
		towerFloorMap.put(towerId, index);
		towerFloorFlag = true;
	}

	public void clearTowerFloorMap() {
		towerFloorMap.clear();
		towerFloorFlag = true;
	}

	public int getRechargeTime(String productId){
		if (rechargeRecordMap.containsKey(productId)) {
			return rechargeRecordMap.get(productId);
		}
		return 0;
	}

	public Map<String, Integer> getRechargeRecordMap() {
		return rechargeRecordMap;
	}

	public void AddRechargeRecord(String productId) {
		if (rechargeRecordMap.containsKey(productId)) {
			rechargeRecordMap.put(productId, rechargeRecordMap.get(productId) + 1);
		}
		else{
			rechargeRecordMap.put(productId, 1);
		}
		rechargeRecordFlag = true;
	}

	public Calendar getMonthCardEndTime() {
		return monthCardEndTime;
	}

	public void addMonthCard() {
		if (this.monthCardEndTime == null || HawkTime.getCalendar().compareTo(this.monthCardEndTime) > 0) {
			this.monthCardEndTime = HawkTime.getCalendar();
		}
		this.monthCardEndTime.add(Calendar.DATE, GsConst.MONTH_CARD_TIME);
	}

	public int getMonsterMaxCount() {
		return monsterMaxCount;
	}

	public void setMonsterMaxCount(int monsterMaxCount) {
		this.monsterMaxCount = monsterMaxCount;
	}

	public int getMonsterMaxLevel() {
		return monsterMaxLevel;
	}

	public void setMonsterMaxLevel(int monsterMaxLevel) {
		this.monsterMaxLevel = monsterMaxLevel;
	}

	public int getMonsterMaxStage() {
		return monsterMaxStage;
	}

	public void setMonsterMaxStage(int monsterMaxStage) {
		this.monsterMaxStage = monsterMaxStage;
	}

	public int getInstanceResetCountDaily() {
		return instanceResetCountDaily;
	}

	public void addInstanceResetCountDaily() {
		++instanceResetCountDaily;
	}

	public void clearInstanceResetCountDaily() {
		instanceResetCountDaily = 0;
	}

	public int getInstanceAllCount() {
		return instanceAllCount;
	}

	public void addInstanceAllCount() {
		++instanceAllCount;
	}

	public int getInstanceAllCountDaily() {
		return instanceAllCountDaily;
	}

	public void addInstanceAllCountDaily() {
		++instanceAllCountDaily;
	}

	public void clearInstanceAllCountDaily() {
		instanceAllCountDaily = 0;
	}

	public int getHardCount() {
		return hardCount;
	}

	public void addHardCount() {
		++hardCount;
	}

	public int getHardCountDaily() {
		return hardCountDaily;
	}

	public void addHardCountDaily() {
		++hardCountDaily;
	}

	public void clearHardCountDaily() {
		hardCountDaily = 0;
	}

	public int getArenaCount() {
		return arenaCount;
	}

	public void addArenaCount() {
		++arenaCount;
	}

	public int getArenaCountDaily() {
		return arenaCountDaily;
	}

	public void addArenaCountDaily() {
		++arenaCountDaily;
	}

	public void clearArenaCountDaily() {
		arenaCountDaily = 0;
	}

	public int getHoleCount() {
		return holeCount;
	}

	public void addHoleCount() {
		++holeCount;
	}

	public int getMonsterMixCount() {
		return monsterMixCount;
	}

	public void addMonsterMixCount() {
		++monsterMixCount;
	}

	public int getMonsterMixCountDaily() {
		return monsterMixCountDaily;
	}

	public void addMonsterMixCountDaily() {
		++monsterMixCountDaily;
	}

	public void clearMonsterMixCountDaily() {
		monsterMixCountDaily = 0;
	}

	public int getAdventureCount() {
		return adventureCount;
	}

	public void addAdventureCount() {
		++this.adventureCount;
	}

	public int getAdventureCountDaily() {
		return adventureCountDaily;
	}

	public void addAdventureCountDaily() {
		++adventureCountDaily;
	}

	public void clearAdventureCountDaily() {
		adventureCountDaily = 0;
	}

	public int getBossrushCount() {
		return bossrushCount;
	}

	public void addBossrushCount() {
		++bossrushCount;
	}

	public int getBossrushCountDaily() {
		return bossrushCountDaily;
	}

	public void addBossrushCountDaily() {
		++bossrushCountDaily;
	}

	public void clearBossrushCountDaily() {
		bossrushCountDaily = 0;
	}

	public int getExploreCount() {
		return exploreCount;
	}

	public void addExploreCount() {
		++exploreCount;
	}

	public int getExploreCountDaily() {
		return exploreCountDaily;
	}

	public void addExploreCountDaily() {
		++exploreCountDaily;
	}

	public void clearExploreCountDaily() {
		exploreCountDaily = 0;
	}

	public int getSkillUpCount() {
		return skillUpCount;
	}

	public void addSkillUpCount() {
		++skillUpCount;
	}

	public int getSkillUpCountDaily() {
		return skillUpCountDaily;
	}

	public void addSkillUpCountDaily() {
		++skillUpCountDaily;
	}

	public void clearSkillUpCountDaily() {
		skillUpCountDaily = 0;
	}

	public int getEquipUpCount() {
		return equipUpCount;
	}

	public void addEquipUpCount() {
		++equipUpCount;
	}

	public int getEquipUpCountDaily() {
		return equipUpCountDaily;
	}

	public void addEquipUpCountDaily() {
		++equipUpCountDaily;
	}

	public void clearEquipUpCountDaily() {
		equipUpCountDaily = 0;
	}

	public int getCoinOrderCount() {
		return coinOrderCount;
	}

	public void addCoinOrderCount() {
		++coinOrderCount;
	}

	public int getCoinOrderCountDaily() {
		return coinOrderCountDaily;
	}

	public void addCoinOrderCountDaily() {
		++coinOrderCountDaily;
	}

	public void clearCoinOrderCountDaily() {
		coinOrderCountDaily = 0;
	}

	public int getFatigueClaimCount() {
		return fatigueClaimCount;
	}

	public void addFatigueClaimCount() {
		++fatigueClaimCount;
	}

	public int getFatigueClaimCountDaily() {
		return fatigueClaimCountDaily;
	}

	public void addFatigueClaimCountDaily() {
		++fatigueClaimCountDaily;
	}

	public void clearFatigueClaimCountDaily() {
		fatigueClaimCountDaily = 0;
	}

	public int getAlliancePrayCountDaily() {
		return alliancePrayCountDaily;
	}

	public void addAlliancePrayCountDaily() {
		++alliancePrayCountDaily;
	}

	public void clearAlliancePrayCountDaily() {
		alliancePrayCountDaily = 0;
	}

	public int getAllianceTaskCountDaily() {
		return allianceTaskCountDaily;
	}

	public void addAllianceTaskCountDaily() {
		++allianceTaskCountDaily;
	}

	public void clearAllianceTaskCountDaily() {
		allianceTaskCountDaily = 0;
	}

	public Map<String, Integer> getItemUseCountDailyMap() {
		return itemUseCountDailyMap;
	}

	/**
	 * @return 物品使用次数，如未使用返回0
	 */
	public int getItemUseCountDaily(String itemId) {
		Integer count = itemUseCountDailyMap.get(itemId);
		if (null != count) {
			return count;
		}
		return 0;
	}

	public void addItemUseCountDaily(String itemId, int addCount) {
		int curCount = 0;
		Integer oldCount = itemUseCountDailyMap.get(itemId);
		if (null != oldCount) {
			curCount = oldCount;
		}
		itemUseCountDailyMap.put(itemId, curCount + addCount);
		itemUseCountDailyFlag = true;
	}

	public void setItemUseCountDaily(String itemId, int count) {
		itemUseCountDailyMap.put(itemId, count);
		itemUseCountDailyFlag = true;
	}

	public void clearItemUseCountDaily() {
		itemUseCountDailyMap.clear();
		itemUseCountDailyFlag = true;
	}

	public byte getSignInCount() {
		return signInCount;
	}

	public void setSignInCount(byte count) {
		this.signInCount = count;
	}

	public int getLoginCount() {
		return loginCount;
	}

	public void addLoginCount() {
		++loginCount;
	}

	public long getTotalOnlineTime() {
		return totalOnlineTime;
	}

	public void addTotalOnlineTime(long onlineTime) {
		totalOnlineTime += onlineTime;
	}

	public int getNormalTopChapter() {
		return normalTopChapter;
	}

	public void setNormalTopChapter(int chapter) {
		this.normalTopChapter = chapter;
	}

	public int getNormalTopIndex() {
		return normalTopIndex;
	}

	public void setNormalTopIndex(int index) {
		this.normalTopIndex = index;
	}

	public int getHardTopChapter() {
		return hardTopChapter;
	}

	public void setHardTopChapter(int chapter) {
		this.hardTopChapter = chapter;
	}

	public int getHardTopIndex() {
		return hardTopIndex;
	}

	public void setHardTopIndex(int index) {
		this.hardTopIndex = index;
	}

	/*
	 * 经验加倍是否还有剩余次数
	 */
	public boolean isExpTimeLeft()
	{
		return doubleExpLeft > 0 || tripleExpLeft > 0;
	}

	public void increaseDoubleExpLeft(int times) {
		doubleExpLeft += times;
	}

	public void increaseTripleExpLeft(int times) {
		tripleExpLeft += times;
	}

	public void decreaseDoubleExpLeft(int times) {
		doubleExpLeft -= times;
	}

	public void decreaseTripleExpLeft(int times) {
		tripleExpLeft -= times;
	}

	public int getDoubleExpLeftTimes() {
		return doubleExpLeft;
	}

	public int getTripleExpLeftTimes() {
		return tripleExpLeft;
	}

	@Override
	public boolean decode() {
		if (refreshTimeJson != null && false == "".equals(refreshTimeJson) && false == "null".equals(refreshTimeJson)) {
			refreshStringMap = HawkJsonUtil.getJsonInstance().fromJson(refreshTimeJson, new TypeToken<HashMap<Integer, String>>() {}.getType());
			for (Map.Entry<Integer, String> entry : refreshStringMap.entrySet()) {
				try {
					Date date = dateFormat.parse(entry.getValue());
					Calendar calendar = Calendar.getInstance();
					calendar.setTime(date);
					refreshTimeMap.put(entry.getKey(), calendar);
				} catch (ParseException e) {
					HawkException.catchException(e);
					return false;
				}
			}
		}
		if (questCompleteJson != null && false == "".equals(questCompleteJson) && false == "null".equals(questCompleteJson)) {
			questCompleteSet = HawkJsonUtil.getJsonInstance().fromJson(questCompleteJson, new TypeToken<HashSet<Integer>>() {}.getType());
		}
		if (questCompleteDailyJson != null && false == "".equals(questCompleteDailyJson) && false == "null".equals(questCompleteDailyJson)) {
			questCompleteDailySet = HawkJsonUtil.getJsonInstance().fromJson(questCompleteDailyJson, new TypeToken<HashSet<Integer>>() {}.getType());
		}
		if (monsterCollectJson != null && false == "".equals(monsterCollectJson) && false == "null".equals(monsterCollectJson)) {
			monsterCollectSet = HawkJsonUtil.getJsonInstance().fromJson(monsterCollectJson, new TypeToken<HashSet<String>>() {}.getType());
		}
		if (instanceStarJson != null && false == "".equals(instanceStarJson) && false == "null".equals(instanceStarJson)) {
			instanceStarMap = HawkJsonUtil.getJsonInstance().fromJson(instanceStarJson, new TypeToken<HashMap<String, Integer>>() {}.getType());
		}
		if (chapterBoxNormalJson != null && false == "".equals(chapterBoxNormalJson) && false == "null".equals(chapterBoxNormalJson)) {
			chapterBoxNormalList = HawkJsonUtil.getJsonInstance().fromJson(chapterBoxNormalJson, new TypeToken<ArrayList<Integer>>() {}.getType());
		}
		if (chapterBoxHardJson != null && false == "".equals(chapterBoxHardJson) && false == "null".equals(chapterBoxHardJson)) {
			chapterBoxHardList = HawkJsonUtil.getJsonInstance().fromJson(chapterBoxHardJson, new TypeToken<ArrayList<Integer>>() {}.getType());
		}
		if (instanceCountDailyJson != null && false == "".equals(instanceCountDailyJson) && false == "null".equals(instanceCountDailyJson)) {
			instanceCountDailyMap = HawkJsonUtil.getJsonInstance().fromJson(instanceCountDailyJson, new TypeToken<HashMap<String, Integer>>() {}.getType());
		}
		if (monsterStageJson != null && false == "".equals(monsterStageJson) && false == "null".equals(monsterStageJson)) {
			monsterStageMap = HawkJsonUtil.getJsonInstance().fromJson(monsterStageJson, new TypeToken<HashMap<Integer, Integer>>() {}.getType());
		}
		if (monsterLevelJson != null && false == "".equals(monsterLevelJson) && false == "null".equals(monsterLevelJson)) {
			monsterLevelMap = HawkJsonUtil.getJsonInstance().fromJson(monsterLevelJson, new TypeToken<HashMap<Integer, Integer>>() {}.getType());
		}
		if (holeCountDailyJson != null && false == "".equals(holeCountDailyJson) && false == "null".equals(holeCountDailyJson)) {
			holeCountDailyMap = HawkJsonUtil.getJsonInstance().fromJson(holeCountDailyJson, new TypeToken<HashMap<Integer, Integer>>() {}.getType());
		}
		if (towerFloorJson != null && false == "".equals(towerFloorJson) && false == "null".equals(towerFloorJson)) {
			towerFloorMap = HawkJsonUtil.getJsonInstance().fromJson(towerFloorJson, new TypeToken<HashMap<Integer, Integer>>() {}.getType());
		}
		if (rechargeRecordJson != null && false == "".equals(rechargeRecordJson) && false == "null".equals(rechargeRecordJson)) {
			rechargeRecordMap = HawkJsonUtil.getJsonInstance().fromJson(rechargeRecordJson, new TypeToken<HashMap<String, Integer>>() {}.getType());
		}
		if (itemUseCountDailyJson != null && false == "".equals(itemUseCountDailyJson) && false == "null".equals(itemUseCountDailyJson)) {
			itemUseCountDailyMap = HawkJsonUtil.getJsonInstance().fromJson(itemUseCountDailyJson, new TypeToken<HashMap<String, Integer>>() {}.getType());
		}

		// 0表示未开始任何章节
		normalTopChapter = 0;
		hardTopChapter = 0;
		// 0表示第一个副本
		normalTopIndex = 0;
		hardTopIndex = 0;

		for (Entry<String, Integer> entry : instanceStarMap.entrySet()) {
			InstanceEntryCfg entryCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceEntryCfg.class, entry.getKey());
			if (entryCfg != null) {
				int chapter = entryCfg.getChapter();
				int index = entryCfg.getIndex();
				if (entryCfg.getDifficult() == GsConst.InstanceDifficulty.NORMAL_INSTANCE) {
					if (chapter > normalTopChapter) {
						normalTopChapter = chapter;
						normalTopIndex = index;
					} else if (chapter == normalTopChapter && index > normalTopIndex) {
						normalTopIndex = index;
					}
				} else if (entryCfg.getDifficult() == GsConst.InstanceDifficulty.HARD_INSTANCE) {
					if (chapter > hardTopChapter) {
						hardTopChapter = chapter;
						hardTopIndex = index;
					} else if (chapter == hardTopChapter && index > hardTopIndex) {
						hardTopIndex = index;
					}
				}
			}
		}

		return true;
	}

	@Override
	public boolean encode() {
		if (true == refreshTimeFlag) {
			refreshTimeFlag = false;
			refreshStringMap.clear();
			for (Map.Entry<Integer, Calendar> entry : refreshTimeMap.entrySet()) {
				refreshStringMap.put(entry.getKey(), dateFormat.format(entry.getValue().getTime()));
			}
			refreshTimeJson = HawkJsonUtil.getJsonInstance().toJson(refreshStringMap);
		}
		if (true == questCompleteFlag) {
			questCompleteFlag = false;
			questCompleteJson = HawkJsonUtil.getJsonInstance().toJson(questCompleteSet);
		}
		if (true == questCompleteDailyFlag) {
			questCompleteDailyFlag = false;
			questCompleteDailyJson = HawkJsonUtil.getJsonInstance().toJson(questCompleteDailySet);
		}
		if (true == instanceStarFlag) {
			instanceStarFlag = false;
			instanceStarJson = HawkJsonUtil.getJsonInstance().toJson(instanceStarMap);
		}
		if (true == chapterBoxNormalFlag) {
			chapterBoxNormalFlag = false;
			chapterBoxNormalJson = HawkJsonUtil.getJsonInstance().toJson(chapterBoxNormalList);
		}
		if (true == chapterBoxHardFlag) {
			chapterBoxHardFlag = false;
			chapterBoxHardJson = HawkJsonUtil.getJsonInstance().toJson(chapterBoxHardList);
		}
		if (true == instanceCountDailyFlag) {
			instanceCountDailyFlag = false;
			instanceCountDailyJson = HawkJsonUtil.getJsonInstance().toJson(instanceCountDailyMap);
		}
		if (true == monsterCollectFlag) {
			monsterCollectFlag = false;
			monsterCollectJson = HawkJsonUtil.getJsonInstance().toJson(monsterCollectSet);
		}
		if (true == monsterStageFlag) {
			monsterStageFlag = false;
			monsterStageJson = HawkJsonUtil.getJsonInstance().toJson(monsterStageMap);
		}
		if (true == monsterLevelFlag) {
			monsterLevelFlag = false;
			monsterLevelJson = HawkJsonUtil.getJsonInstance().toJson(monsterLevelMap);
		}
		if (true == holeCountDailyFlag) {
			holeCountDailyFlag = false;
			holeCountDailyJson = HawkJsonUtil.getJsonInstance().toJson(holeCountDailyMap);
		}
		if (true == towerFloorFlag) {
			towerFloorFlag = false;
			towerFloorJson = HawkJsonUtil.getJsonInstance().toJson(towerFloorMap);
		}
		if (true == rechargeRecordFlag) {
			rechargeRecordFlag = false;
			rechargeRecordJson = HawkJsonUtil.getJsonInstance().toJson(rechargeRecordMap);
		}
		if (true == itemUseCountDailyFlag) {
			itemUseCountDailyFlag = false;
			itemUseCountDailyJson = HawkJsonUtil.getJsonInstance().toJson(itemUseCountDailyMap);
		}

		return true;
	}

	@Override
	public int getCreateTime() {
		return createTime;
	}

	@Override
	public void setCreateTime(int createTime) {
		this.createTime = createTime;
	}

	@Override
	public int getUpdateTime() {
		return updateTime;
	}

	@Override
	public void setUpdateTime(int updateTime) {
		this.updateTime = updateTime;
	}

	@Override
	public boolean isInvalid() {
		return invalid;
	}

	@Override
	public void setInvalid(boolean invalid) {
		this.invalid = invalid;
	}

}
