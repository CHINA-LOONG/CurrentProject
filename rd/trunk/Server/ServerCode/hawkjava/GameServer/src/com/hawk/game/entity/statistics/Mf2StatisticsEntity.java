package com.hawk.game.entity.statistics;

import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;
import java.util.Map.Entry;

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

	// 新手引导
	@Column(name = "guideFinish", nullable = false)
	protected String guideFinishJson = "";

	// 历史使用物品X数量
	@Column(name = "useItemXCount", nullable = false)
	protected String useItemXCountStr = "";

	// 今日使用X物品数量
	@Column(name = "useItemXCountDaily", nullable = false)
	protected String useItemXCountDailyStr = "";

	// 历史购买物品X次数
	@Column(name = "buyItemXTimes", nullable = false)
	protected String buyItemXTimesStr = "";

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
	protected Set<Integer> guideFinishSet = new HashSet<Integer>();
	@Transient
	boolean guideFinishFlag = false;

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
		if (null != guideFinishJson && false == "".equals(guideFinishJson) && false == "null".equals(guideFinishJson)) {
			guideFinishSet = HawkJsonUtil.getJsonInstance().fromJson(guideFinishJson, new TypeToken<HashSet<Integer>>() {}.getType());
		}

		if (null != useItemXCountStr && false == "".equals(useItemXCountStr) && false == "null".equals(useItemXCountStr)) {
			String[] itemArray = useItemXCountStr.split(",");
			useItemCountMap.clear();
			for (int i = 0; i < itemArray.length; ++i) {
				String[] pair = itemArray[i].split(":");
				useItemCountMap.put(pair[0], Integer.parseInt(pair[1]));
			}
		}
		if (null != useItemXCountDailyStr && false == "".equals(useItemXCountDailyStr) && false == "null".equals(useItemXCountDailyStr)) {
			String[] itemArray = useItemXCountDailyStr.split(",");
			useItemCountDailyMap.clear();
			for (int i = 0; i < itemArray.length; ++i) {
				String[] pair = itemArray[i].split(":");
				useItemCountDailyMap.put(pair[0], Integer.parseInt(pair[1]));
			}
		}
		if (null != buyItemXTimesStr && false == "".equals(buyItemXTimesStr) && false == "null".equals(buyItemXTimesStr)) {
			String[] itemArray = buyItemXTimesStr.split(",");
			buyItemTimesMap.clear();
			for (int i = 0; i < itemArray.length; ++i) {
				String[] pair = itemArray[i].split(":");
				buyItemTimesMap.put(pair[0], Integer.parseInt(pair[1]));
			}
		}

		return true;
	}

	@Override
	public boolean encode() {
		if (true == guideFinishFlag) {
			guideFinishFlag = false;
			guideFinishJson = HawkJsonUtil.getJsonInstance().toJson(guideFinishSet);
		}

		if (true == useItemCountFlag) {
			useItemCountFlag = false;
			if (true == useItemCountMap.isEmpty()) {
				useItemXCountStr = "";
			} else {
				StringBuilder builder = new StringBuilder();
				for (Entry<String, Integer> entry : useItemCountMap.entrySet()) {
					builder.append(entry.getKey()).append(":").append(entry.getValue()).append(",");
				}
				builder.deleteCharAt(builder.length() - 1);
				useItemXCountStr = builder.toString();
			}
		}
		if (true == useItemCountDailyFlag) {
			useItemCountDailyFlag = false;
			if (true == useItemCountDailyMap.isEmpty()) {
				useItemXCountDailyStr = "";
			} else {
				StringBuilder builder = new StringBuilder();
				for (Entry<String, Integer> entry : useItemCountDailyMap.entrySet()) {
					builder.append(entry.getKey()).append(":").append(entry.getValue()).append(",");
				}
				builder.deleteCharAt(builder.length() - 1);
				useItemXCountDailyStr = builder.toString();
			}
		}
		if (true == buyItemTimesFlag) {
			buyItemTimesFlag = false;
			if (true == buyItemTimesMap.isEmpty()) {
				buyItemXTimesStr = "";
			} else {
				StringBuilder builder = new StringBuilder();
				for (Entry<String, Integer> entry : buyItemTimesMap.entrySet()) {
					builder.append(entry.getKey()).append(":").append(entry.getValue()).append(",");
				}
				builder.deleteCharAt(builder.length() - 1);
				buyItemXTimesStr = builder.toString();
			}
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