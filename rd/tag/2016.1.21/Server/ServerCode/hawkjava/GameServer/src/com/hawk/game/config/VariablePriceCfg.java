package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/variablePrice.csv", struct = "map")
public class VariablePriceCfg extends HawkConfigBase {

	@Id
	protected final String times;
	protected final int resetInstanceCoin;
	protected final int fatigueCoin;

	public VariablePriceCfg() {
		times = "";
		resetInstanceCoin = 0;
		fatigueCoin = 0;
	}

	public String getTimes() {
		return times;
	}

	public int getResetInstanceCoin() {
		return resetInstanceCoin;
	}

	public int getResetFatigueCoin() {
		return fatigueCoin;
	}

}
