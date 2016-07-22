package com.hawk.game.entity;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.HashMap;
import java.util.HashSet;
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
import com.hawk.game.util.GsConst;
import com.hawk.game.util.InstanceUtil;
import com.hawk.game.util.GsConst.Cycle;

/**
 * 玩家统计数据
 * 
 * @author walker
 * 
 */
@Entity
@Table(name = "statistics")
@SuppressWarnings("serial")
public class StatisticsEntity  extends HawkDBEntity {
	@Id
	@Column(name = "playerId", unique = true)
	private int playerId = 0;

	// 疲劳值
	@Column(name = "fatigue", nullable = false)
	private int fatigue = 0;
	
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

	// 历史时光之穴次数
	@Column(name = "timeholeCount", nullable = false)
	private int timeholeCount = 0;

	// 今日时光之穴次数
	@Column(name = "timeholeCountDaily", nullable = false)
	private int timeholeCountDaily = 0;

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

	// 刷新时间
	@Column(name = "refreshTime")
	private String refreshTime = null;

	// 签到次数
	@Column(name = "signInCount", nullable = false)
	private byte signInCount = 0;

	@Column(name = "loginCount", nullable = false)
	private int loginCount = 0;

	@Column(name = "totalOnlineTime", nullable = false)
	private long totalOnlineTime = 0;

	@Column(name = "createTime", nullable = false)
	protected Calendar createTime = null;

	@Column(name = "updateTime")
	protected Calendar updateTime = null;

	@Column(name = "invalid", nullable = false)
	protected boolean invalid = false;

	// 每一项刷新时间
	@Transient
	protected Map<Integer, Calendar> refreshTimeMap = new HashMap<Integer, Calendar>();
	@Transient
	protected Map<Integer, String> refreshStringMap = new HashMap<Integer, String>();
	@Transient
	protected Set<Integer> questCompleteSet = new HashSet<Integer>();
	@Transient
	protected Set<Integer> questCompleteDailySet = new HashSet<Integer>();
	@Transient
	protected Map<String, Integer> instanceStarMap = new HashMap<String, Integer> ();
	@Transient
	protected Map<String, Integer> instanceCountDailyMap = new HashMap<String, Integer> ();
	@Transient
	protected Set<String> monsterCollectSet = new HashSet<String> ();
	@Transient
	protected Map<Integer, Integer> monsterStageMap = new HashMap<Integer, Integer>();
	@Transient
	protected Map<Integer, Integer> monsterLevelMap = new HashMap<Integer, Integer>();
	// 最后普通副本所属章节
	@Transient
	protected int normalChapter = 0;
	// 最后困难副本所属章节
	@Transient
	protected int hardChapter = 0;
	// 最后普通副本章节索引
	@Transient
	protected int normalIndex = 0;
	// 最后困难副本章节索引
	@Transient
	protected int hardIndex = 0;
	@Transient
	SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
	
	public StatisticsEntity() {
		this.createTime = HawkTime.getCalendar();
		this.skillPointBeginTime = HawkTime.getCalendar();
	}

	public StatisticsEntity(int playerId) {
		this.playerId = playerId;
		this.createTime = HawkTime.getCalendar();
		this.skillPointBeginTime = HawkTime.getCalendar();
	}

	public int getPlayerId() {
		return playerId;
	}

	public void setPlayerId(int playerId) {
		this.playerId = playerId;
	}

	public Calendar getLastRefreshTime(int type) {
		return refreshTimeMap.get(type);
	}

	public void setRefreshTime(int type, Calendar time) {
		this.refreshTimeMap.put(type, time);
	}

	public int getFatigue() {
		return fatigue;
	}
	
	public void setFatigue(int fatigue) {
		this.fatigue = fatigue;
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
		} else {
			questCompleteSet.add(questId);
		}
	}

	public void clearQuestCompleteDaily() {
		questCompleteDailySet.clear();
	}

	public Set<String> getMonsterCollectSet() {
		return monsterCollectSet;
	}

	public void addMonsterCollect(String monsterCfgId) {
		monsterCollectSet.add(monsterCfgId);
	}

	public Map<String, Integer> getInstanceStarMap() {
		return instanceStarMap;
	}

	public void setInstanceStar(String instanceId, int starCount) {
		instanceStarMap.put(instanceId, starCount);
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
	}

	public void setInstanceCountDaily(String instanceId, int count) {
		instanceCountDailyMap.put(instanceId, count);
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

	public void clearInstanceCountDaily() {
		instanceCountDailyMap.clear();
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

	public int getTimeholeCount() {
		return timeholeCount;
	}

	public void addTimeholeCount() {
		++timeholeCount;
	}

	public int getTimeholeCountDaily() {
		return timeholeCountDaily;
	}

	public void addTimeholeCountDaily() {
		++timeholeCountDaily;
	}

	public void clearTimeholeCountDaily() {
		timeholeCountDaily = 0;
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

	public int getNormalInstanceChapter() {
		return normalChapter;
	}

	public int getNormalInstanceIndex() {
		return normalIndex;
	}

	public int getHardInstanceChapter() {
		return hardChapter;
	}

	public int getHardInstanceIndex() {
		return hardIndex;
	}

	@Override
	public boolean decode() {
		if (refreshTime != null && false == "".equals(refreshTime) && false == "null".equals(refreshTime)) {
			refreshStringMap = HawkJsonUtil.getJsonInstance().fromJson(refreshTime, new TypeToken<HashMap<Integer, String>>() {}.getType());
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
		if (instanceCountDailyJson != null && false == "".equals(instanceCountDailyJson) && false == "null".equals(instanceCountDailyJson)) {
			instanceCountDailyMap = HawkJsonUtil.getJsonInstance().fromJson(instanceCountDailyJson, new TypeToken<HashMap<String, Integer>>() {}.getType());
		}
		if (monsterStageJson != null && false == "".equals(monsterStageJson) && false == "null".equals(monsterStageJson)) {
			monsterStageMap = HawkJsonUtil.getJsonInstance().fromJson(monsterStageJson, new TypeToken<HashMap<Integer, Integer>>() {}.getType());
		}
		if (monsterLevelJson != null && false == "".equals(monsterLevelJson) && false == "null".equals(monsterLevelJson)) {
			monsterLevelMap = HawkJsonUtil.getJsonInstance().fromJson(monsterLevelJson, new TypeToken<HashMap<Integer, Integer>>() {}.getType());
		}
		
		// 0表示未开始任何章节
		normalChapter = 0;
		hardChapter = 0;
		// 0表示第一个副本
		normalIndex = 0;
		hardIndex = 0;
		for (Entry<String, Integer> entry : instanceStarMap.entrySet()) {
			InstanceEntryCfg entryCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceEntryCfg.class, entry.getKey());
			if (entryCfg != null) {
				if (entryCfg.getDifficult() == GsConst.InstanceDifficulty.NORMAL_INSTANCE) {
					int chapter = entryCfg.getChapter();
					if (chapter > normalChapter) {
						normalChapter = chapter;
						normalIndex = 0;
					}
					int index = InstanceUtil.getInstanceChapterIndex(entryCfg.getInstanceId());
					if (index > normalIndex) {
						normalIndex = index;
					}
				} else if (entryCfg.getDifficult() == GsConst.InstanceDifficulty.NORMAL_INSTANCE) {
					int chapter = entryCfg.getChapter();
					if (chapter > hardChapter) {
						hardChapter = chapter;
						hardIndex = 0;
					}
					int index = InstanceUtil.getInstanceChapterIndex(entryCfg.getInstanceId());
					if (index > hardIndex) {
						hardIndex = index;
					}
				}
			}
		}

		return true;
	}

	@Override
	public boolean encode() {
		questCompleteJson = HawkJsonUtil.getJsonInstance().toJson(questCompleteSet);
		questCompleteDailyJson = HawkJsonUtil.getJsonInstance().toJson(questCompleteDailySet);
		monsterCollectJson = HawkJsonUtil.getJsonInstance().toJson(monsterCollectSet);
		instanceStarJson = HawkJsonUtil.getJsonInstance().toJson(instanceStarMap);
		instanceCountDailyJson = HawkJsonUtil.getJsonInstance().toJson(instanceCountDailyMap);
		monsterStageJson = HawkJsonUtil.getJsonInstance().toJson(monsterStageMap);
		monsterLevelJson = HawkJsonUtil.getJsonInstance().toJson(monsterLevelMap);
		
		refreshStringMap.clear();
		for (Map.Entry<Integer, Calendar> entry : refreshTimeMap.entrySet()) {
			refreshStringMap.put(entry.getKey(), dateFormat.format(entry.getValue().getTime()));
		}
		refreshTime = HawkJsonUtil.getJsonInstance().toJson(refreshStringMap);	
		
		return true;
	}

	@Override
	public Calendar getCreateTime() {
		return createTime;
	}

	@Override
	public void setCreateTime(Calendar createTime) {
		this.createTime = createTime;
	}

	@Override
	public Calendar getUpdateTime() {
		return updateTime;
	}

	@Override
	public void setUpdateTime(Calendar updateTime) {
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
