package com.hawk.game.entity.statistics;

import java.util.ArrayList;
import java.util.Calendar;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

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
 * 低频更新统计数据
 * 
 * @author walker
 * 
 */
@Entity
@Table(name = "statistics_lf_2")
public class Lf2StatisticsEntity extends HawkDBEntity {
	@Id
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private int id = 0;

	@Column(name = "playerId", unique = true)
	protected int playerId = 0;

	// 大冒险条件刷新次数
	@Column(name = "adventureChange", nullable = false)
	protected int adventureChange = 0;

	// 大冒险条件刷新次数开始计时时间
	@Column(name = "adventureChangeBeginTime")
	protected Calendar adventureChangeBeginTime = null;

	// 历史金币单抽次数（免费+收费）
	@Column(name = "eggCoin1Times", nullable = false)
	protected int eggCoin1Times = 0;

	// 今日金币免费单抽次数
	@Column(name = "eggCoin1FreeTimesDaily", nullable = false)
	protected int eggCoin1FreeTimesDaily = 0;

	// 今日金币收费单抽次数
	@Column(name = "eggCoin1PayTimesDaily", nullable = false)
	protected int eggCoin1PayTimesDaily = 0;

	// 历史金币十连抽次数
	@Column(name = "eggCoin10Times", nullable = false)
	protected int eggCoin10Times = 0;

	// 今日金币十连抽次数
	@Column(name = "eggCoin10TimesDaily", nullable = false)
	protected int eggCoin10TimesDaily = 0;

	// 上次免费金币抽蛋时间
	@Column(name = "eggCoin1FreeLastTime")
	protected Calendar eggCoin1FreeLastTime = null;

	// 历史钻石免费单抽次数
	@Column(name = "eggDiamond1FreeTimes", nullable = false)
	protected int eggDiamond1FreeTimes = 0;

	// 历史钻石收费单抽次数
	@Column(name = "eggDiamond1PayTimes", nullable = false)
	protected int eggDiamond1PayTimes = 0;

	// 历史钻石十连抽次数
	@Column(name = "eggDiamond10Times", nullable = false)
	protected int eggDiamond10Times = 0;

	// 免费钻石抽蛋点数
	@Column(name = "eggDiamond1FreePoint", nullable = false)
	protected int eggDiamond1FreePoint = 0;

	// 免费钻石抽蛋点数开始计时时间
	@Column(name = "eggDiamond1FreePointBeginTime")
	protected Calendar eggDiamond1FreePointBeginTime = null;

	// 历史抽到品级X宠物次数
	@Column(name = "callMonsterStageXTimes", nullable = false)
	protected String callMonsterStageXTimesJson = "";

	// 历史抽到品级X装备次数
	@Column(name = "callEquipStageXTimes", nullable = false)
	protected String callEquipStageXTimesJson = "";

	// 历史公会祈福次数
	@Column(name = "alliancePrayTimes", nullable = false)
	protected int alliancePrayTimes = 0;

	// 今日公会祈福次数
	@Column(name = "alliancePrayTimesDaily", nullable = false)
	protected int alliancePrayTimesDaily = 0;

	// 历史公会boss次数
	@Column(name = "allianceBossTimes", nullable = false)
	protected int allianceBossTimes = 0;

	// 历史公会赠送体力次数
	@Column(name = "allianceFatigueTimes", nullable = false)
	protected int allianceFatigueTimes = 0;

	// 今日公会赠送体力次数
	@Column(name = "allianceFatigueTimesDaily", nullable = false)
	protected int allianceFatigueTimesDaily = 0;

	// 今日公会任务数
	@Column(name = "allianceTaskCountDaily", nullable = false)
	protected int allianceTaskCountDaily = 0;

	// 今日公会贡献值领奖数
	@Column(name = "allianceContriRewardDaily", nullable = false)
	protected int allianceContriRewardDaily = 0;

	// 历史手动刷新商店次数
	@Column(name = "shopRefreshTimes", nullable = false)
	protected int shopRefreshTimes = 0;

	// 商品充值次数记录
	@Column(name = "rechargeRecord", nullable = false)
	protected String rechargeRecordJson = "";

	// 月卡结束时间
	@Column(name = "monthCardEndTime")
	protected Calendar monthCardEndTime = null;

	// 本月签到次数（包括补签）
	@Column(name = "signinTimesMonthly", nullable = false)
	protected int signinTimesMonthly = 0;

	// 本月补签次数
	@Column(name = "signinFillTimesMonthly", nullable = false)
	protected int signinFillTimesMonthly = 0;

	// 今日是否已签到
	@Column(name = "signinDaily", nullable = false)
	protected boolean signinDaily = false;

	// 历史登录次数
	@Column(name = "loginTimes", nullable = false)
	protected int loginTimes = 0;

	// 今日登录次数
	@Column(name = "loginTimesDaily", nullable = false)
	protected int loginTimesDaily = 0;

	// 禁言时间
	@Column(name = "dumpTime", nullable = false)
	protected int dumpTime = 0;

	// 在线时间
	@Column(name = "totalOnlineTime", nullable = false)
	protected long totalOnlineTime = 0;

	@Column(name = "createTime", nullable = false)
	protected int createTime = 0;
	@Column(name = "updateTime")
	protected int updateTime = 0;
	@Column(name = "invalid", nullable = false)
	protected boolean invalid = false;

	// decode-------------------------------------------------------------------

	@Transient
	protected List<Integer> callMonsterStageTimesList = new ArrayList<Integer>();
	@Transient
	boolean callMonsterStageTimesFlag = false;

	@Transient
	protected List<Integer> callEquipStageTimesList = new ArrayList<Integer>();
	@Transient
	boolean callEquipStageTimesFlag = false;

	@Transient
	protected Map<String, Integer> rechargeRecordMap = new HashMap<String, Integer>();
	@Transient
	boolean rechargeRecordFlag = false;

	// method-------------------------------------------------------------------

	protected Lf2StatisticsEntity() {
		Calendar time = HawkTime.getCalendar();
		this.adventureChangeBeginTime = time;
		this.eggDiamond1FreePointBeginTime = time;
		this.eggCoin1FreeLastTime = time;
		this.monthCardEndTime = time;
	}

	protected Lf2StatisticsEntity(int playerId) {
		Calendar time = HawkTime.getCalendar();
		this.playerId = playerId;
		this.adventureChangeBeginTime = time;
		this.eggDiamond1FreePointBeginTime = time;
		this.eggCoin1FreeLastTime = time;
		this.monthCardEndTime = time;
	}

	@Override
	public boolean decode() {
		if (null != callMonsterStageXTimesJson && false == "".equals(callMonsterStageXTimesJson) && false == "null".equals(callMonsterStageXTimesJson)) {
			callMonsterStageTimesList = HawkJsonUtil.getJsonInstance().fromJson(callMonsterStageXTimesJson, new TypeToken<ArrayList<Integer>>() {}.getType());
		}
		if (null != callEquipStageXTimesJson && false == "".equals(callEquipStageXTimesJson) && false == "null".equals(callEquipStageXTimesJson)) {
			callEquipStageTimesList = HawkJsonUtil.getJsonInstance().fromJson(callEquipStageXTimesJson, new TypeToken<ArrayList<Integer>>() {}.getType());
		}
		if (null != rechargeRecordJson && false == "".equals(rechargeRecordJson) && false == "null".equals(rechargeRecordJson)) {
			rechargeRecordMap = HawkJsonUtil.getJsonInstance().fromJson(rechargeRecordJson, new TypeToken<HashMap<String, Integer>>() {}.getType());
		}

		return true;
	}

	@Override
	public boolean encode() {
		if (true == callMonsterStageTimesFlag) {
			callMonsterStageTimesFlag = false;
			callMonsterStageXTimesJson = HawkJsonUtil.getJsonInstance().toJson(callMonsterStageTimesList);
		}
		if (true == callEquipStageTimesFlag) {
			callEquipStageTimesFlag = false;
			callEquipStageXTimesJson = HawkJsonUtil.getJsonInstance().toJson(callEquipStageTimesList);
		}
		if (true == rechargeRecordFlag) {
			rechargeRecordFlag = false;
			rechargeRecordJson = HawkJsonUtil.getJsonInstance().toJson(rechargeRecordMap);
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