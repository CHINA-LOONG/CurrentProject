package com.hawk.game.entity.statistics;

import java.util.Calendar;
import java.util.HashMap;
import java.util.Map;
import java.util.Map.Entry;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Transient;

import org.hawk.config.HawkConfigManager;
import org.hawk.db.HawkDBEntity;
import org.hawk.os.HawkTime;
import org.hawk.util.HawkJsonUtil;

import com.google.gson.reflect.TypeToken;
import com.hawk.game.config.InstanceEntryCfg;
import com.hawk.game.protocol.Const;
import com.hawk.game.util.GsConst;

/**
 * 高频更新统计数据
 * 
 * @author walker
 * 
 */
@Entity
@Table(name = "statistics_hf")
public class HfStatisticsEntity extends HawkDBEntity {
	@Id
	@Column(name = "playerId", unique = true)
	protected int playerId = 0;

	// 活力值
	@Column(name = "fatigue", nullable = false)
	protected int fatigue = 0;

	// 活力值开始计时时间
	@Column(name = "fatigueBeginTime")
	protected Calendar fatigueBeginTime = null;

	// 历史使用活力值数量
	@Column(name = "useFatigueCount", nullable = false)
	protected int useFatigueCount = 0;

	// 今日使用活力值数量
	@Column(name = "useFatigueCountDaily", nullable = false)
	protected int useFatigueCountDaily = 0;

	// 双倍剩余经验次数
	@Column(name = "doubleExpLeft", nullable = false)
	protected byte doubleExpLeft = 0;

	// 三倍剩余经验次数
	@Column(name = "tripleExpLeft", nullable = false)
	protected byte tripleExpLeft = 0;

	// 副本X状态[历史通关次数，星级0-3，今日进入次数]
	@Column(name = "instanceXState", nullable = false)
	protected String instanceXStateJson = "";

	// 今日普通副本完成次数
	@Column(name = "normalTimesDaily", nullable = false)
	protected int normalTimesDaily = 0;

	// 今日精英副本完成次数
	@Column(name = "hardTimesDaily", nullable = false)
	protected int hardTimesDaily = 0;

	@Column(name = "createTime", nullable = false)
	protected int createTime = 0;
	@Column(name = "updateTime")
	protected int updateTime = 0;
	@Column(name = "invalid", nullable = false)
	protected boolean invalid = false;

	// decode-------------------------------------------------------------------

	@Transient
	protected Map<String, int[]> instanceStateMap = new HashMap<>();
	@Transient
	boolean instanceStateFlag = false;

	@ Transient
	protected int normalTimes = 0;
	@ Transient
	protected int hardTimes = 0;

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

	// method-------------------------------------------------------------------

	protected HfStatisticsEntity() {
		Calendar time = HawkTime.getCalendar();
		this.fatigueBeginTime = time;
	}

	protected HfStatisticsEntity(int playerId) {
		Calendar time = HawkTime.getCalendar();
		this.playerId = playerId;
		this.fatigueBeginTime = time;
	}

	@Override
	public boolean decode() {
		if (null != instanceXStateJson && false == "".equals(instanceXStateJson) && false == "null".equals(instanceXStateJson)) {
			instanceStateMap = HawkJsonUtil.getJsonInstance().fromJson(instanceXStateJson, new TypeToken<HashMap<String, int[]>>() {}.getType());
		}

		normalTimes = 0;
		hardTimes = 0;
		// 0表示未开始任何章节
		normalTopChapter = 0;
		hardTopChapter = 0;
		// 0表示第一个副本
		normalTopIndex = 0;
		hardTopIndex = 0;

		// 计算章节、索引和完成次数
		for (Entry<String, int[]> entry : instanceStateMap.entrySet()) {
			int[] state = entry.getValue();
			if (state.length != GsConst.Instance.STATE_STORY_SIZE || state[GsConst.Instance.STATE_STAR_INDEX] <= 0) {
				continue;
			}

			InstanceEntryCfg entryCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceEntryCfg.class, entry.getKey());
			if (entryCfg != null && entryCfg.getType() == Const.InstanceType.INSTANCE_STORY_VALUE) {
				int chapter = entryCfg.getChapter();
				int index = entryCfg.getIndex();
				if (entryCfg.getDifficult() == GsConst.Instance.NORMAL) {
					if (chapter > normalTopChapter) {
						normalTopChapter = chapter;
						normalTopIndex = index;
					} else if (chapter == normalTopChapter && index > normalTopIndex) {
						normalTopIndex = index;
					}
					normalTimes += state[GsConst.Instance.STATE_WIN_INDEX];

				} else if (entryCfg.getDifficult() == GsConst.Instance.HARD) {
					if (chapter > hardTopChapter) {
						hardTopChapter = chapter;
						hardTopIndex = index;
					} else if (chapter == hardTopChapter && index > hardTopIndex) {
						hardTopIndex = index;
					}
					hardTimes += state[GsConst.Instance.STATE_WIN_INDEX];
				}
			}
		}

		return true;
	}

	@Override
	public boolean encode() {
		if (true == instanceStateFlag) {
			instanceStateFlag = false;
			instanceXStateJson = HawkJsonUtil.getJsonInstance().toJson(instanceStateMap);
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

