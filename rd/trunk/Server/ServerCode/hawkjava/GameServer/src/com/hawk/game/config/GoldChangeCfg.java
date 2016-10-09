package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/goldChange.csv", struct = "map")
public class GoldChangeCfg extends HawkConfigBase{
	/**
	 * 类别
	 */
	@Id
	protected final int id;
	/**
	 * 最大刷新次数
	 */
	protected final int maxTimes;
	/**
	 * 基础钻石消耗
	 */
	protected final int consume;
	/**
	 * 次数消耗加成
	 */
	protected final int consumeTimeAdd;
	/**
	 * 次数翻倍
	 */
	protected final int consumeTimeDoubel;
	/**
	 * 获得产物基数
	 */
	protected final int award;
	/**
	 * 次数收益加成
	 */
	protected final int awardTimeAdd;
	/**
	 * 等级系数
	 */
	protected final float levelAdd;
	/**
	 * 对应暴击概率及暴击倍数1
	 */
	protected final float twoMultiple;
	/**
	 * 对应暴击概率及暴击倍数2
	 */
	protected final float fiveMultiple;
	/**
	 * 对应暴击概率及暴击倍数3
	 */
	protected final float tenMultiple;

	public GoldChangeCfg(){
		this.id = 0;
		this.maxTimes = 0;
		this.consume = 0;
		this.consumeTimeAdd = 0;
		this.consumeTimeDoubel = 0;
		this.award = 0;
		this.awardTimeAdd = 0;
		this.levelAdd = 0;
		this.twoMultiple = 0;
		this.fiveMultiple = 0;
		this.tenMultiple = 0;
	}

	public int getId() {
		return id;
	}

	public int getMaxTimes() {
		return maxTimes;
	}

	public int getConsume() {
		return consume;
	}

	public int getConsumeTimeAdd() {
		return consumeTimeAdd;
	}

	public int getConsumeTimeDoubel() {
		return consumeTimeDoubel;
	}

	public int getAward() {
		return award;
	}

	public int getAwardTimeAdd() {
		return awardTimeAdd;
	}

	public float getLevelAdd() {
		return levelAdd;
	}

	public float getTwoMultiple() {
		return twoMultiple;
	}

	public float getFiveMultiple() {
		return fiveMultiple;
	}

	public float getTenMultiple() {
		return tenMultiple;
	}
}
