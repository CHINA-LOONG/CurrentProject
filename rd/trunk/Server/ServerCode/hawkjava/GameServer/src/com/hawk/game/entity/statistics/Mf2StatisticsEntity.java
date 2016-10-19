package com.hawk.game.entity.statistics;

import java.util.HashMap;
import java.util.Map;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Transient;

import org.hawk.db.HawkDBEntity;
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
@Table(name = "statistics_mf_2")
public class Mf2StatisticsEntity extends HawkDBEntity {
	@Id
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private int id = 0;

	@Column(name = "playerId", unique = true)
	protected int playerId = 0;

	// 历史使用物品X数量
	@Column(name = "useItemXCount", nullable = false)
	protected String useItemXCountJson = "";

	// 今日使用X物品数量
	@Column(name = "useItemXCountDaily", nullable = false)
	protected String useItemXCountDailyJson = "";

	// 历史购买物品X次数
	@Column(name = "buyItemXTimes", nullable = false)
	protected String buyItemXTimesJson = "";

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

	@Column(name = "createTime", nullable = false)
	protected int createTime = 0;
	@Column(name = "updateTime")
	protected int updateTime = 0;
	@Column(name = "invalid", nullable = false)
	protected boolean invalid = false;

	// decode-------------------------------------------------------------------

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

	protected Mf2StatisticsEntity() {
	}

	protected Mf2StatisticsEntity(int playerId) {
		this.playerId = playerId;
	}

	@Override
	public boolean decode() {
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