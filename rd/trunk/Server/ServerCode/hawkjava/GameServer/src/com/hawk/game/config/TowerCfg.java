package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;
import org.hawk.util.HawkJsonUtil;

import com.google.gson.reflect.TypeToken;

@HawkConfigManager.CsvResource(file = "staticData/tower.csv", struct = "map")
public class TowerCfg extends HawkConfigBase {

	@Id
	protected final int id;
	protected final String time;
	protected final int level;
	protected final String floor;

	// client only
	private final String textId = null;

	// assemble
	protected int[] refreshTimeList;
	protected String[] instanceList;

	public TowerCfg() {
		id = 0;
		time = "";
		level = 0;
		floor = "";
	}

	@Override
	protected boolean assemble() {
		refreshTimeList = HawkJsonUtil.getJsonInstance().fromJson(time, new TypeToken<int[]>() {}.getType());
		instanceList = HawkJsonUtil.getJsonInstance().fromJson(floor, new TypeToken<String[]>() {}.getType());
		return true;
	}

	@Override
	protected boolean checkValid() {
		for (int id : refreshTimeList) {
			TimeCfg timeCfg = HawkConfigManager.getInstance().getConfigByKey(TimeCfg.class, id);
			if (timeCfg == null) {
				HawkLog.errPrintln(String.format("config invalid TimeCfg : %d", id));
				return false;
			}
		}

		for (String id : instanceList) {
			InstanceCfg instanceCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceCfg.class, id);
			if (instanceCfg == null) {
				HawkLog.errPrintln(String.format("config invalid InstanceCfg : %s", id));
				return false;
			}
		}

		return true;
	}

	public int getId() {
		return id;
	}

	public int getLevel() {
		return level;
	}

	public int[] getRefreshTimeList() {
		return refreshTimeList;
	}

	public String[] getInstanceList() {
		return instanceList;
	}
}