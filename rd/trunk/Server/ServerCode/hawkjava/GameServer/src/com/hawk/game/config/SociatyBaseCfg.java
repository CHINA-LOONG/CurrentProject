package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/sociatybase.csv", struct = "list")
public class SociatyBaseCfg  extends HawkConfigBase{
	/**
	 * 配置id
	 */
	@Id
	protected final int bpMax;

	protected final int coinDefend;

	protected final int coinHire;

	protected final int coinHireget;

	public SociatyBaseCfg() {
		super();
		bpMax = 0;
		coinDefend = 0;
		coinHire = 0;
		coinHireget = 0;
	}

	public int getBpMax() {
		return bpMax;
	}

	public int getCoinDefend() {
		return coinDefend;
	}

	public int getCoinHire() {
		return coinHire;
	}

	public int getCoinHireget() {
		return coinHireget;
	}
}
