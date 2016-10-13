package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;
import org.hawk.util.HawkJsonUtil;

import com.google.gson.reflect.TypeToken;

@HawkConfigManager.CsvResource(file = "staticData/hole.csv", struct = "map")
public class HoleCfg extends HawkConfigBase {

	@Id
	protected final int id;
	protected final String time;
	protected final int count;
	protected final String difficulty;

	// client only
	protected final String textId = null;
	protected final String openId = null;
	protected final String dropId = null;

	// assemble
	protected int[] openTimeList;
	protected int[] closeTimeList;
	protected String[] instanceList;

	public HoleCfg() {
		id = 0;
		time = "";
		count = 0;
		difficulty = "";
	}

	@Override
	protected boolean assemble() {
		int[][] timePairList = HawkJsonUtil.getJsonInstance().fromJson(time, new TypeToken<int[][]>() {}.getType());
		openTimeList = new int[timePairList.length];
		closeTimeList = new int[timePairList.length];
		for (int i = 0; i < timePairList.length; ++i) {
			int[] pair = timePairList[i];
			if (pair.length != 2) {
				HawkLog.errPrintln(String.format("config invalid time"));
				return false;
			}
			openTimeList[i] = pair[0];
			closeTimeList[i] = pair[1];
		}

		instanceList = HawkJsonUtil.getJsonInstance().fromJson(difficulty, new TypeToken<String[]>() {}.getType());
		return true;
	}

	@Override
	protected boolean checkValid() {
		for (int id : openTimeList) {
			TimePointCfg timeCfg = HawkConfigManager.getInstance().getConfigByKey(TimePointCfg.class, id);
			if (timeCfg == null) {
				HawkLog.errPrintln(String.format("config invalid TimePointCfg : %d", id));
				return false;
			}
		}

		for (int id : closeTimeList) {
			TimePointCfg timeCfg = HawkConfigManager.getInstance().getConfigByKey(TimePointCfg.class, id);
			if (timeCfg == null) {
				HawkLog.errPrintln(String.format("config invalid TimePointCfg : %d", id));
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

	public int getCount() {
		return count;
	}

	public boolean isOpenTime(int timeCfgId) {
		for (int id : openTimeList) {
			if (id == timeCfgId) {
				return true;
			}
		}
		return false;
	}

	public boolean isCloseTime(int timeCfgId) {
		for (int id : closeTimeList) {
			if (id == timeCfgId) {
				return true;
			}
		}
		return false;
	}

	public String[] getInstanceList() {
		return instanceList;
	}
}
