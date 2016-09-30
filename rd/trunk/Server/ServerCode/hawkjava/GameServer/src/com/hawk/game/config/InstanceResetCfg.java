package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/instanceReset.csv", struct = "list")
public class InstanceResetCfg extends HawkConfigBase {

	protected final int maxTimes;
	protected final int consume;
	protected final int consumeAdd;
	protected final int doubleTimes;

	// global
	protected static InstanceResetCfg singleton;
	public static InstanceResetCfg getSingleton() {
		return singleton;
	}

	public InstanceResetCfg() {
		singleton = this;
		maxTimes = 0;
		consume = 0;
		consumeAdd = 0;
		doubleTimes = 0;
	}

	public int getMaxTimes() {
		return maxTimes;
	}

	public int getConsume() {
		return consume;
	}

	public int getConsumeAdd() {
		return consumeAdd;
	}

	public int getDoubleTimes() {
		return doubleTimes;
	}
}
