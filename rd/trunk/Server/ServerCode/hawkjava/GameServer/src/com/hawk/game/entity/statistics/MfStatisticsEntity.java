package com.hawk.game.entity.statistics;

import java.util.ArrayList;
import java.util.Calendar;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Transient;

import org.hawk.db.HawkDBEntity;
import org.hawk.os.HawkTime;
import org.hawk.util.HawkJsonUtil;

import com.google.gson.reflect.TypeToken;

/**
 * 中频更新统计数据
 * 
 * @author walker
 * 
 */
@Entity
@Table(name = "statistics_mf")
public class MfStatisticsEntity extends HawkDBEntity {
	@Id
	@Column(name = "playerId", unique = true)
	protected int playerId = 0;

	// 历史已完成的非周期任务
	@Column(name = "questComplete", nullable = false)
	protected String questCompleteJson = "";

	// 今日已完成的每日任务
	@Column(name = "questCompleteDaily", nullable = false)
	protected String questCompleteDailyJson = "";

	// 技能点
	@Column(name = "skillPoint", nullable = false)
	protected int skillPoint = 0;

	// 技能点恢复开始计时时间
	@Column(name = "skillPointBeginTime")
	protected Calendar skillPointBeginTime = null;

	// 历史达到品级X怪物数量
	@Column(name = "monsterStageXCount", nullable = false)
	protected String monsterStageXCountJson = "";

	// 历史达到等级X怪物数量
	@Column(name = "monsterLevelXCount", nullable = false)
	protected String monsterLevelXCountJson = "";

	// 历史竞技场次数
	@Column(name = "arenaTimes", nullable = false)
	protected int arenaTimes = 0;

	// 今日竞技场次数
	@Column(name = "arenaTimesDaily", nullable = false)
	protected int arenaTimesDaily = 0;

	// 历史进入洞X次数
	@Column(name = "holeXTimes", nullable = false)
	protected String holeXTimesJson = "";

	// 今日进入洞X次数
	@Column(name = "holeXTimesDaily", nullable = false)
	protected String holeXTimesDailyJson = "";

	// 塔X已完成层数
	@Column(name = "towerXFloor", nullable = false)
	protected String towerXFloorJson = "";

	// 今日已雇佣怪物
	@Column(name = "hireMonsterDaily", nullable = false)
	protected String hireMonsterDailyJson = "";

	// 历史领取大冒险奖励次数
	@Column(name = "adventureTimes", nullable = false)
	protected int adventureTimes = 0;

	// 今日领取大冒险奖励次数
	@Column(name = "adventureTimesDaily", nullable = false)
	protected int adventureTimesDaily = 0;

	// 历史提升技能次数
	@Column(name = "upSkillTimes", nullable = false)
	protected int upSkillTimes = 0;

	// 今日提升技能次数
	@Column(name = "upSkillTimesDaily", nullable = false)
	protected int upSkillTimesDaily = 0;

	// 历史升级装备次数
	@Column(name = "upEquipTimes", nullable = false)
	protected int upEquipTimes = 0;

	// 今日升级装备次数
	@Column(name = "upEquipTimesDaily", nullable = false)
	protected int upEquipTimesDaily = 0;

	// 历史使用物品X数量
	@Column(name = "useItemXCount", nullable = false)
	protected String useItemXCountJson = "";

	// 今日使用X物品数量
	@Column(name = "useItemXCountDaily", nullable = false)
	protected String useItemXCountDailyJson = "";

	// 历史获得通天塔币数量
	@Column(name = "coinTowerCount", nullable = false)
	protected int coinTowerCount = 0;

	// 历史获得公会币数量
	@Column(name = "coinAllianceCount", nullable = false)
	protected int coinAllianceCount = 0;

	// 今日获得公会币数量
	@Column(name = "coinAllianceCountDaily", nullable = false)
	protected int coinAllianceCountDaily = 0;

	// 历史购买物品X次数
	@Column(name = "buyItemXTimes", nullable = false)
	protected String buyItemXTimesJson = "";

	@Column(name = "createTime", nullable = false)
	protected int createTime = 0;
	@Column(name = "updateTime")
	protected int updateTime = 0;
	@Column(name = "invalid", nullable = false)
	protected boolean invalid = false;

	// decode-------------------------------------------------------------------

	@Transient
	protected Set<Integer> questCompleteSet = new HashSet<Integer>();
	@Transient
	boolean questCompleteFlag = false;

	@Transient
	protected Set<Integer> questCompleteDailySet = new HashSet<Integer>();
	@Transient
	boolean questCompleteDailyFlag = false;

	@Transient
	protected List<Integer> monsterStageCountList = new ArrayList<Integer>();
	@Transient
	boolean monsterStageCountFlag = false;

	@Transient
	protected List<Integer> monsterLevelCountList = new ArrayList<Integer>();
	@Transient
	boolean monsterLevelCountFlag = false;

	@Transient
	protected Map<Integer, Integer> holeTimesMap = new HashMap<Integer, Integer>();
	@Transient
	boolean holeTimesFlag = false;

	@Transient
	protected Map<Integer, Integer> holeTimesDailyMap = new HashMap<Integer, Integer>();
	@Transient
	boolean holeTimesDailyFlag = false;

	@Transient
	protected Map<Integer, Integer> towerFloorMap = new HashMap<Integer, Integer>();
	@Transient
	boolean towerFloorFlag = false;

	@Transient
	protected Set<Integer> hireMonsterDailySet = new HashSet<Integer>();
	@Transient
	boolean hireMonsterDailyFlag = false;

	@Transient
	protected Map<String, Integer> useItemCountMap = new HashMap<String, Integer>();
	@Transient
	boolean useItemCountFlag = false;

	@Transient
	protected Map<String, Integer> useItemCountDailyMap = new HashMap<String, Integer>();
	@Transient
	boolean useItemCountDailyFlag = false;

	@Transient
	protected Map<String, Integer> buyItemTimesMap = new HashMap<String, Integer>();
	@Transient
	boolean buyItemTimesFlag = false;

	// method-------------------------------------------------------------------

	protected MfStatisticsEntity() {
		Calendar time = HawkTime.getCalendar();
		this.skillPointBeginTime = time;
	}

	protected MfStatisticsEntity(int playerId) {
		Calendar time = HawkTime.getCalendar();
		this.playerId = playerId;
		this.skillPointBeginTime = time;
	}

	@Override
	public boolean decode() {
		if (null != questCompleteJson && false == "".equals(questCompleteJson) && false == "null".equals(questCompleteJson)) {
			questCompleteSet = HawkJsonUtil.getJsonInstance().fromJson(questCompleteJson, new TypeToken<HashSet<Integer>>() {}.getType());
		}
		if (null != questCompleteDailyJson && false == "".equals(questCompleteDailyJson) && false == "null".equals(questCompleteDailyJson)) {
			questCompleteDailySet = HawkJsonUtil.getJsonInstance().fromJson(questCompleteDailyJson, new TypeToken<HashSet<Integer>>() {}.getType());
		}
		if (null != monsterStageXCountJson && false == "".equals(monsterStageXCountJson) && false == "null".equals(monsterStageXCountJson)) {
			monsterStageCountList = HawkJsonUtil.getJsonInstance().fromJson(monsterStageXCountJson, new TypeToken<ArrayList<Integer>>() {}.getType());
		}
		if (null != monsterLevelXCountJson && false == "".equals(monsterLevelXCountJson) && false == "null".equals(monsterLevelXCountJson)) {
			monsterLevelCountList = HawkJsonUtil.getJsonInstance().fromJson(monsterLevelXCountJson, new TypeToken<ArrayList<Integer>>() {}.getType());
		}
		if (null != holeXTimesJson && false == "".equals(holeXTimesJson) && false == "null".equals(holeXTimesJson)) {
			holeTimesMap = HawkJsonUtil.getJsonInstance().fromJson(holeXTimesJson, new TypeToken<HashMap<Integer, Integer>>() {}.getType());
		}
		if (null != holeXTimesDailyJson && false == "".equals(holeXTimesDailyJson) && false == "null".equals(holeXTimesDailyJson)) {
			holeTimesDailyMap = HawkJsonUtil.getJsonInstance().fromJson(holeXTimesDailyJson, new TypeToken<HashMap<Integer, Integer>>() {}.getType());
		}
		if (null != towerXFloorJson && false == "".equals(towerXFloorJson) && false == "null".equals(towerXFloorJson)) {
			towerFloorMap = HawkJsonUtil.getJsonInstance().fromJson(towerXFloorJson, new TypeToken<HashMap<Integer, Integer>>() {}.getType());
		}
		if (null != hireMonsterDailyJson && false == "".equals(hireMonsterDailyJson) && false == "null".equals(hireMonsterDailyJson)) {
			hireMonsterDailySet = HawkJsonUtil.getJsonInstance().fromJson(hireMonsterDailyJson, new TypeToken<HashSet<Integer>>() {}.getType());
		}
		if (null != useItemXCountJson && false == "".equals(useItemXCountJson) && false == "null".equals(useItemXCountJson)) {
			useItemCountMap = HawkJsonUtil.getJsonInstance().fromJson(useItemXCountJson, new TypeToken<HashMap<String, Integer>>() {}.getType());
		}
		if (null != useItemXCountDailyJson && false == "".equals(useItemXCountDailyJson) && false == "null".equals(useItemXCountDailyJson)) {
			useItemCountDailyMap = HawkJsonUtil.getJsonInstance().fromJson(useItemXCountDailyJson, new TypeToken<HashMap<String, Integer>>() {}.getType());
		}
		if (null != buyItemXTimesJson && false == "".equals(buyItemXTimesJson) && false == "null".equals(buyItemXTimesJson)) {
			buyItemTimesMap = HawkJsonUtil.getJsonInstance().fromJson(buyItemXTimesJson, new TypeToken<HashMap<String, Integer>>() {}.getType());
		}

		return true;
	}

	@Override
	public boolean encode() {
		if (true == questCompleteFlag) {
			questCompleteFlag = false;
			questCompleteJson = HawkJsonUtil.getJsonInstance().toJson(questCompleteSet);
		}
		if (true == questCompleteDailyFlag) {
			questCompleteDailyFlag = false;
			questCompleteDailyJson = HawkJsonUtil.getJsonInstance().toJson(questCompleteDailySet);
		}
		if (true == monsterStageCountFlag) {
			monsterStageCountFlag = false;
			monsterStageXCountJson = HawkJsonUtil.getJsonInstance().toJson(monsterStageCountList);
		}
		if (true == monsterLevelCountFlag) {
			monsterLevelCountFlag = false;
			monsterLevelXCountJson = HawkJsonUtil.getJsonInstance().toJson(monsterLevelCountList);
		}
		if (true == holeTimesFlag) {
			holeTimesFlag = false;
			holeXTimesJson = HawkJsonUtil.getJsonInstance().toJson(holeTimesMap);
		}
		if (true == holeTimesDailyFlag) {
			holeTimesDailyFlag = false;
			holeXTimesDailyJson = HawkJsonUtil.getJsonInstance().toJson(holeTimesDailyMap);
		}
		if (true == towerFloorFlag) {
			towerFloorFlag = false;
			towerXFloorJson = HawkJsonUtil.getJsonInstance().toJson(towerFloorMap);
		}
		if (true == hireMonsterDailyFlag) {
			hireMonsterDailyFlag = false;
			hireMonsterDailyJson = HawkJsonUtil.getJsonInstance().toJson(hireMonsterDailySet);
		}
		if (true == useItemCountFlag) {
			useItemCountFlag = false;
			useItemXCountJson = HawkJsonUtil.getJsonInstance().toJson(useItemCountMap);
		}
		if (true == useItemCountDailyFlag) {
			useItemCountDailyFlag = false;
			useItemXCountDailyJson = HawkJsonUtil.getJsonInstance().toJson(useItemCountDailyMap);
		}
		if (true == buyItemTimesFlag) {
			buyItemTimesFlag = false;
			buyItemXTimesJson = HawkJsonUtil.getJsonInstance().toJson(buyItemTimesMap);
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