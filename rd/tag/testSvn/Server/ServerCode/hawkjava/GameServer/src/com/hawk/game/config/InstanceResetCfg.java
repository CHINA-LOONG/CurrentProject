package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/instanceReset.csv", struct = "map")
public class InstanceResetCfg extends HawkConfigBase {

	@Id
	protected final String id;
	protected final int maxTimes;
	protected final int consume;
	protected final int consumeAdd;
	protected final int doubleTimes;

	public InstanceResetCfg() {
		id = "";
		maxTimes = 0;
		consume = 0;
		consumeAdd = 0;
		doubleTimes = 0;
	}

	public String getId() {
		return id;
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
