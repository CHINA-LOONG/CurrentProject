package com.hawk.game.config;

import org.hawk.config.HawkConfigBase; 
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/pvp.csv", struct = "list")
public class PVPCfg extends HawkConfigBase{
	/**
	 * 段位
	 */
	@Id
	final protected int stage ;

	final protected int point ;

	final protected int win ;

	final protected int lose ;

	final protected int draw ;

	final protected int K ;
	
	public PVPCfg() {
		super();
		stage = 0;
		point = 0;
		win = 0;
		lose = 0;
		draw = 0;
		K = 0;
	}

	public int getStage() {
		return stage;
	}

	public int getPoint() {
		return point;
	}

	public int getWin() {
		return win;
	}

	public int getLose() {
		return lose;
	}

	public int getDraw() {
		return draw;
	}

	public int getK() {
		return K;
	}
	
	/**
	 * 获取PVP cfg
	 * @param point
	 * @return
	 */
	public static PVPCfg getPVPStageCfg(int point){
		for (PVPCfg pvpCfg : HawkConfigManager.getInstance().getConfigList(PVPCfg.class)) {
			if (point <= pvpCfg.point || pvpCfg.point == 0) {
				return pvpCfg;
			}
		}
		return null;
	}
}
