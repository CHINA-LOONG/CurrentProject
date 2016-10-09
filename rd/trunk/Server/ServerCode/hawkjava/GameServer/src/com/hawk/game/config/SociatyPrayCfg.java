package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/sociatyPray.csv", struct = "map")
public class SociatyPrayCfg extends HawkConfigBase{
	/**
	 * 配置id
	 */
	@Id
	protected final int id;

	protected final int memberReward;

	protected final int allianceReward;

	protected final int coinConsume;

	protected final int goldConsume;

	public SociatyPrayCfg(){
		id = 0;
		memberReward = 0;
		allianceReward = 0;
		goldConsume = 0;
		coinConsume = 0;
	}

	public int getId() {
		return id;
	}

	public int getMemberReward() {
		return memberReward;
	}

	public int getAllianceReward() {
		return allianceReward;
	}

	public int getCoinConsume() {
		return coinConsume;
	}

	public int getGoldConsume() {
		return goldConsume;
	}


	@Override
	protected boolean assemble() {
		return true;
	}
}
