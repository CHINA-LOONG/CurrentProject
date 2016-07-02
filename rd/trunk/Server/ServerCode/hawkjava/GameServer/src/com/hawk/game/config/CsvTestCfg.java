package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "xml/csvTest.csv", struct = "map")
public class CsvTestCfg extends HawkConfigBase {
	/**
	 * csv配置测试
	 */
	@Id
	protected final String index;
	
	protected final String assetId;
	
	protected final String nickName;
	
	protected final int grade;
	
	protected final int[] numList;

	public CsvTestCfg() {
		index = "";
		assetId = null;
		nickName = null;
		grade = 0;
		numList = null;
	}

	public String getIndex() {
		return index;
	}

	@Override
	protected boolean assemble() {
		
		return true;
	}

	@Override
	protected boolean checkValid() {
		return true;
	}
}

