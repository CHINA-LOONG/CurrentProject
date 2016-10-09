package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/unitRarity.csv", struct = "map")
public class MonsterRarityCfg extends HawkConfigBase {

	@Id
	protected final int rarity;
	protected final float commonRatio;

	// client only
	protected final String desc = null;

	public MonsterRarityCfg() {
		rarity = 0;
		commonRatio = 0.0f;
	}

	@Override
	protected boolean assemble() {
		if (commonRatio < 0 || commonRatio > 1) {
			return false;
		}
		return true;
	}

	@Override
	protected boolean checkValid() {
		return true;
	}

	public int getRarity() {
		return rarity;
	}

	public float getCommonRatio() {
		return commonRatio;
	}

}
