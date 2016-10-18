package com.hawk.game.entity.statistics;

import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

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
 * 低频更新统计数据
 * 
 * @author walker
 * 
 */
@Entity
@Table(name = "statistics_lf")
public class LfStatisticsEntity extends HawkDBEntity {
	@Id
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private int id = 0;

	@Column(name = "playerId", unique = true)
	protected int playerId = 0;

	// 怪物图鉴
	@Column(name = "monsterCollect", nullable = false)
	protected String monsterCollectStr = "";

	// 历史合成怪物次数
	@Column(name = "monsterMixTimes", nullable = false)
	protected int monsterMixTimes = 0;

	// 今日副本挑战次数重置次数
	@Column(name = "instanceResetTimesDaily", nullable = false)
	protected int instanceResetTimesDaily = 0;

	// 普通副本章节X宝箱状态
	@Column(name = "chapterXBoxNormal", nullable = false)
	protected String chapterXBoxNormalJson = "";

	// 困难副本章节X宝箱状态
	@Column(name = "chapterXBoxHard", nullable = false)
	protected String chapterXBoxHardJson = "";

	// 历史购买金币次数
	@Column(name = "buyCoinTimes", nullable = false)
	protected int buyCoinTimes = 0;

	// 今日购买金币次数
	@Column(name = "buyCoinTimesDaily", nullable = false)
	protected int buyCoinTimesDaily = 0;

	// 历史购买礼包次数
	@Column(name = "buyGiftTimes", nullable = false)
	protected int buyGiftTimes = 0;

	// 今日购买礼包次数
	@Column(name = "buyGiftTimesDaily", nullable = false)
	protected int buyGiftTimesDaily = 0;

	// 历史充值钻石数量
	@Column(name = "payDiamondCount", nullable = false)
	protected int payDiamondCount = 0;

	// 今日充值钻石数量
	@Column(name = "payDiamondCountDaily", nullable = false)
	protected int payDiamondCountDaily = 0;

	// 历史使用钻石数量
	@Column(name = "useDiamondCount", nullable = false)
	protected int useDiamondCount = 0;

	// 今日使用钻石数量
	@Column(name = "useDiamondCountDaily", nullable = false)
	protected int useDiamondCountDaily = 0;

	// 历史同一只宠物穿过≥X品级的装备数量
	@Column(name = "equipStageXMaxCount", nullable = false)
	protected String equipStageXMaxCountJson = "";

	// 今日同一只宠物穿过≥X品级的装备数量
	@Column(name = "equipStageXMaxCountDaily", nullable = false)
	protected String equipStageXMaxCountDailyJson = "";

	// 历史打孔次数
	@Column(name = "equipPunchTimes", nullable = false)
	protected int equipPunchTimes = 0;

	// 今日打孔次数
	@Column(name = "equipPunchTimesDaily", nullable = false)
	protected int equipPunchTimesDaily = 0;

	// 历史镶嵌宝石次数
	@Column(name = "inlayAllTimes", nullable = false)
	protected int inlayAllTimes = 0;

	// 历史镶嵌类型X宝石次数
	@Column(name = "inlayTypeXTimes", nullable = false)
	protected String inlayTypeXTimesJson = "";

	// 历史合成宝石次数
	@Column(name = "synAllTimes", nullable = false)
	protected int synAllTimes = 0;

	// 今日合成宝石次数
	@Column(name = "synAllTimesDaily", nullable = false)
	protected int synAllTimesDaily = 0;

	// 历史合成类型X宝石次数
	@Column(name = "synTypeXTimes", nullable = false)
	protected String synTypeXTimesJson = "";

	@Column(name = "createTime", nullable = false)
	protected int createTime = 0;
	@Column(name = "updateTime")
	protected int updateTime = 0;
	@Column(name = "invalid", nullable = false)
	protected boolean invalid = false;

	// decode-------------------------------------------------------------------

	@Transient
	protected Set<String> monsterCollectSet = new HashSet<String>();
	@Transient
	boolean monsterCollectFlag = false;

	@Transient
	protected List<Integer> chapterBoxNormalList = new ArrayList<Integer>();
	@Transient
	boolean chapterBoxNormalFlag = false;

	@Transient
	protected List<Integer> chapterBoxHardList = new ArrayList<Integer>();
	@Transient
	boolean chapterBoxHardFlag = false;

	@Transient
	protected List<Integer> equipStageMaxCountList = new ArrayList<Integer>();
	@Transient
	boolean equipStageCountFlag = false;

	@Transient
	protected List<Integer> equipStageMaxCountDailyList = new ArrayList<Integer>();
	@Transient
	boolean equipStageCountDailyFlag = false;

	@Transient
	protected List<Integer> inlayTypeTimesList = new ArrayList<Integer>();
	@Transient
	boolean inlayTypeTimesFlag = false;

	@Transient
	protected List<Integer> synTypeTimesList = new ArrayList<Integer>();
	@Transient
	boolean synTypeTimesFlag = false;

	// method-------------------------------------------------------------------

	protected LfStatisticsEntity() {

	}

	protected LfStatisticsEntity(int playerId) {
		this.playerId = playerId;
	}

	@Override
	public boolean decode() {
		if (null != monsterCollectStr && false == "".equals(monsterCollectStr) && false == "null".equals(monsterCollectStr)) {
			String[] monsterArray = monsterCollectStr.split(",");
			monsterCollectSet.clear();
			for (int i = 0; i < monsterArray.length; ++i) {
				monsterCollectSet.add(monsterArray[i]);
			}
		}
		if (null != chapterXBoxNormalJson && false == "".equals(chapterXBoxNormalJson) && false == "null".equals(chapterXBoxNormalJson)) {
			chapterBoxNormalList = HawkJsonUtil.getJsonInstance().fromJson(chapterXBoxNormalJson, new TypeToken<ArrayList<Integer>>() {}.getType());
		}
		if (null != chapterXBoxHardJson && false == "".equals(chapterXBoxHardJson) && false == "null".equals(chapterXBoxHardJson)) {
			chapterBoxHardList = HawkJsonUtil.getJsonInstance().fromJson(chapterXBoxHardJson, new TypeToken<ArrayList<Integer>>() {}.getType());
		}
		if (null != equipStageXMaxCountJson && false == "".equals(equipStageXMaxCountJson) && false == "null".equals(equipStageXMaxCountJson)) {
			equipStageMaxCountList = HawkJsonUtil.getJsonInstance().fromJson(equipStageXMaxCountJson, new TypeToken<ArrayList<Integer>>() {}.getType());
		}
		if (null != equipStageXMaxCountDailyJson && false == "".equals(equipStageXMaxCountDailyJson) && false == "null".equals(equipStageXMaxCountDailyJson)) {
			equipStageMaxCountDailyList = HawkJsonUtil.getJsonInstance().fromJson(equipStageXMaxCountDailyJson, new TypeToken<ArrayList<Integer>>() {}.getType());
		}
		if (null != inlayTypeXTimesJson && false == "".equals(inlayTypeXTimesJson) && false == "null".equals(inlayTypeXTimesJson)) {
			inlayTypeTimesList = HawkJsonUtil.getJsonInstance().fromJson(inlayTypeXTimesJson, new TypeToken<ArrayList<Integer>>() {}.getType());
		}
		if (null != synTypeXTimesJson && false == "".equals(synTypeXTimesJson) && false == "null".equals(synTypeXTimesJson)) {
			synTypeTimesList = HawkJsonUtil.getJsonInstance().fromJson(synTypeXTimesJson, new TypeToken<ArrayList<Integer>>() {}.getType());
		}

		return true;
	}

	@Override
	public boolean encode() {
		if (true == monsterCollectFlag) {
			monsterCollectFlag = false;
			if (true == monsterCollectSet.isEmpty()) {
				monsterCollectStr = "";
			} else {
				StringBuilder builder = new StringBuilder();
				for (String monster : monsterCollectSet) {
					builder.append(monster).append(",");
				}
				builder.deleteCharAt(builder.length() - 1);
				monsterCollectStr = builder.toString();
			}
		}
		if (true == chapterBoxNormalFlag) {
			chapterBoxNormalFlag = false;
			chapterXBoxNormalJson = HawkJsonUtil.getJsonInstance().toJson(chapterBoxNormalList);
		}
		if (true == chapterBoxHardFlag) {
			chapterBoxHardFlag = false;
			chapterXBoxHardJson = HawkJsonUtil.getJsonInstance().toJson(chapterBoxHardList);
		}
		if (true == equipStageCountFlag) {
			equipStageCountFlag = false;
			equipStageXMaxCountJson = HawkJsonUtil.getJsonInstance().toJson(equipStageMaxCountList);
		}
		if (true == equipStageCountDailyFlag) {
			equipStageCountDailyFlag = false;
			equipStageXMaxCountDailyJson = HawkJsonUtil.getJsonInstance().toJson(equipStageMaxCountDailyList);
		}
		if (true == inlayTypeTimesFlag) {
			inlayTypeTimesFlag = false;
			inlayTypeXTimesJson = HawkJsonUtil.getJsonInstance().toJson(inlayTypeTimesList);
		}
		if (true == synTypeTimesFlag) {
			synTypeTimesFlag = false;
			synTypeXTimesJson = HawkJsonUtil.getJsonInstance().toJson(synTypeTimesList);
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