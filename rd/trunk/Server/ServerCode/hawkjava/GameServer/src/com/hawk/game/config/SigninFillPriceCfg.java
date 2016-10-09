package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/signinFillPrice.csv", struct = "list")
public class SigninFillPriceCfg extends HawkConfigBase {

	protected final int consume;
	protected final int consumeAdd;
	protected final int doubleTimes;
	protected final int ceiling;

	// global
	protected static SigninFillPriceCfg singleton;
	public static SigninFillPriceCfg getSingleton() {
		return singleton;
	}

	public SigninFillPriceCfg() {
		singleton = this;
		consume = 0;
		consumeAdd = 0;
		doubleTimes = 0;
		ceiling = 0;
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

	public int getCeiling() {
		return ceiling;
	}
}
