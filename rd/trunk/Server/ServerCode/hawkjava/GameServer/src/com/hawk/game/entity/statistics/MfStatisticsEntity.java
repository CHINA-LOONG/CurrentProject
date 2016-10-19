package com.hawk.game.entity.statistics;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Transient;

import org.hawk.db.HawkDBEntity;
import org.hawk.os.HawkTime;
import org.hawk.util.HawkJsonUtil;
import org.hibernate.annotations.GenericGenerator;

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
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private int id = 0;

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
	protected int skillPointBeginTime = 0;

	// pvp次数
	@Column(name = "pvpTime", nullable = false)
	protected int pvpTime = 0;

	// pvp次数开始计时时间
	@Column(name = "pvpTimeBeginTime")
	protected int pvpTimeBeginTime = 0;

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

	// 历史获得通天塔币数量
	@Column(name = "coinTowerCount", nullable = false)
	protected int coinTowerCount = 0;

	// 历史获得公会币数量
	@Column(name = "coinAllianceCount", nullable = false)
	protected int coinAllianceCount = 0;

	// 今日获得公会币数量
	@Column(name = "coinAllianceCountDaily", nullable = false)
	protected int coinAllianceCountDaily = 0;

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

	// method-------------------------------------------------------------------

	protected MfStatisticsEntity() {
		this.skillPointBeginTime = HawkTime.getSeconds();
	}

	protected MfStatisticsEntity(int playerId) {
		this.playerId = playerId;
		this.skillPointBeginTime = HawkTime.getSeconds();
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