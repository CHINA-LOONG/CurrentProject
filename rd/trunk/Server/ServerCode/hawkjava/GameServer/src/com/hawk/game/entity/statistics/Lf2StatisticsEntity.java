package com.hawk.game.entity.statistics;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Transient;

import org.hawk.db.HawkDBEntity;
import org.hawk.os.HawkException;
import org.hawk.os.HawkTime;
import org.hawk.util.HawkJsonUtil;

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
	@Column(name = "playerId", unique = true)
	protected int playerId = 0;

	// 刷新时间点X上次刷新时间
	@Column(name = "refreshTimeX")
	protected String refreshTimeXJson = "";

	// 大冒险条件刷新次数
	@Column(name = "adventureChange", nullable = false)
	protected int adventureChange = 0;

	// 大冒险条件刷新次数开始计时时间
	@Column(name = "adventureChangeBeginTime")
	protected Calendar adventureChangeBeginTime = null;

	// 历史金币抽蛋次数
	@Column(name = "eggCoinTimes", nullable = false)
	protected int eggCoinTimes = 0;

	// 今日金币抽蛋次数
	@Column(name = "eggCoinTimesDaily", nullable = false)
	protected int eggCoinTimesDaily = 0;

	// 今日免费金币抽蛋次数
	@Column(name = "eggCoinFreeTimesDaily", nullable = false)
	protected int eggCoinFreeTimesDaily = 0;

	// 历史钻石免费单抽次数
	@Column(name = "eggDiamondFreeTimes", nullable = false)
	protected int eggDiamondFreeTimes = 0;

	// 历史钻石收费单抽次数
	@Column(name = "eggDiamondOneTimes", nullable = false)
	protected int eggDiamondOneTimes = 0;

	// 历史钻石十连抽次数
	@Column(name = "eggDiamondTenTimes", nullable = false)
	protected int eggDiamondTenTimes = 0;

	// 免费钻石抽蛋点数
	@Column(name = "eggDiamondPoint", nullable = false)
	protected int eggDiamondPoint = 0;

	// 免费钻石抽蛋点数开始计时时间
	@Column(name = "eggDiamondPointBeginTime")
	protected Calendar eggPointBeginTime = null;

	// 上次免费金币抽蛋时间
	@Column(name = "eggCoinFreeLastTime")
	protected Calendar eggCoinFreeLastTime = null;

	// 历史抽到品级X宠物次数
	@Column(name = "callMonsterStageXTimes", nullable = false)
	protected String callMonsterStageXTimesJson = "";

	// 历史抽到品级X装备次数
	@Column(name = "callEquipStageXTimes", nullable = false)
	protected String callEquipStageXTimesJson = "";

	// 历史抽到物品X次数
	@Column(name = "callItemXTimes", nullable = false)
	protected String callItemXTimesJson = "";

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

	// 历史刷新商店次数
	@Column(name = "shopRefreshTimes", nullable = false)
	protected int shopRefreshTimes = 0;

	// 商品充值次数记录
	@Column(name = "rechargeRecord", nullable = false)
	protected String rechargeRecordJson = "";

	// 月卡结束时间
	@Column(name = "monthCardEndTime")
	protected Calendar monthCardEndTime = null;

	// 签到次数
	@Column(name = "signInTimes", nullable = false)
	protected int signInTimes = 0;

	// 登录次数
	@Column(name = "loginTimes", nullable = false)
	protected int loginTimes = 0;

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
	SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");

	// 每一项刷新时间
	@Transient
	protected Map<Integer, Calendar> refreshTimeMap = new HashMap<Integer, Calendar>();
	@Transient
	protected Map<Integer, String> refreshDateMap = new HashMap<Integer, String>();
	@Transient
	boolean refreshTimeFlag = false;

	@Transient
	protected List<Integer> callMonsterStageTimesList = new ArrayList<Integer>();
	@Transient
	boolean callMonsterStageTimesFlag = false;

	@Transient
	protected List<Integer> callEquipStageTimesList = new ArrayList<Integer>();
	@Transient
	boolean callEquipStageTimesFlag = false;

	@Transient
	protected Map<String, Integer> callItemTimesMap = new HashMap<String, Integer>();
	@Transient
	boolean callItemTimesFlag = false;

	@Transient
	protected Map<String, Integer> rechargeRecordMap = new HashMap<String, Integer>();
	@Transient
	boolean rechargeRecordFlag = false;

	// method-------------------------------------------------------------------

	protected Lf2StatisticsEntity() {
		Calendar time = HawkTime.getCalendar();
		this.adventureChangeBeginTime = time;
		this.monthCardEndTime = time;
	}

	protected Lf2StatisticsEntity(int playerId) {
		Calendar time = HawkTime.getCalendar();
		this.playerId = playerId;
		this.adventureChangeBeginTime = time;
		this.monthCardEndTime = time;
	}

	@Override
	public boolean decode() {
		if (null != refreshTimeXJson && false == "".equals(refreshTimeXJson) && false == "null".equals(refreshTimeXJson)) {
			refreshDateMap = HawkJsonUtil.getJsonInstance().fromJson(refreshTimeXJson, new TypeToken<HashMap<Integer, String>>() {}.getType());
			for (Map.Entry<Integer, String> entry : refreshDateMap.entrySet()) {
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

		if (null != callMonsterStageXTimesJson && false == "".equals(callMonsterStageXTimesJson) && false == "null".equals(callMonsterStageXTimesJson)) {
			callMonsterStageTimesList = HawkJsonUtil.getJsonInstance().fromJson(callMonsterStageXTimesJson, new TypeToken<ArrayList<Integer>>() {}.getType());
		}
		if (null != callEquipStageXTimesJson && false == "".equals(callEquipStageXTimesJson) && false == "null".equals(callEquipStageXTimesJson)) {
			callEquipStageTimesList = HawkJsonUtil.getJsonInstance().fromJson(callEquipStageXTimesJson, new TypeToken<ArrayList<Integer>>() {}.getType());
		}
		if (null != callItemXTimesJson && false == "".equals(callItemXTimesJson) && false == "null".equals(callItemXTimesJson)) {
			callItemTimesMap = HawkJsonUtil.getJsonInstance().fromJson(callItemXTimesJson, new TypeToken<HashMap<String, Integer>>() {}.getType());
		}
		if (null != rechargeRecordJson && false == "".equals(rechargeRecordJson) && false == "null".equals(rechargeRecordJson)) {
			rechargeRecordMap = HawkJsonUtil.getJsonInstance().fromJson(rechargeRecordJson, new TypeToken<HashMap<String, Integer>>() {}.getType());
		}

		return true;
	}

	@Override
	public boolean encode() {
		if (true == refreshTimeFlag) {
			refreshTimeFlag = false;
			refreshDateMap.clear();
			for (Map.Entry<Integer, Calendar> entry : refreshTimeMap.entrySet()) {
				refreshDateMap.put(entry.getKey(), dateFormat.format(entry.getValue().getTime()));
			}
			refreshTimeXJson = HawkJsonUtil.getJsonInstance().toJson(refreshDateMap);
		}

		if (true == callMonsterStageTimesFlag) {
			callMonsterStageTimesFlag = false;
			callMonsterStageXTimesJson = HawkJsonUtil.getJsonInstance().toJson(callMonsterStageTimesList);
		}
		if (true == callEquipStageTimesFlag) {
			callEquipStageTimesFlag = false;
			callEquipStageXTimesJson = HawkJsonUtil.getJsonInstance().toJson(callEquipStageTimesList);
		}
		if (true == callItemTimesFlag) {
			callItemTimesFlag = false;
			callItemXTimesJson = HawkJsonUtil.getJsonInstance().toJson(callItemTimesMap);
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