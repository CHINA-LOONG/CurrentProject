package com.hawk.game.config;

import java.util.List;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.util.HawkJsonUtil;

import com.google.gson.reflect.TypeToken;

@HawkConfigManager.CsvResource(file = "staticData/hole.csv", struct = "map")
public class HoleCfg extends HawkConfigBase {

	@Id
	protected final String id;
	protected final String time;
	protected final int count;
	protected final String difficulty;

	// client only
	private final String openId = null;
	private final String dropId = null;

	// assemble
	protected List<String[]> timePairList;
	protected List<String> instanceList;

	public HoleCfg() {
		id = "";
		time = "";
		count = 0;
		difficulty = "";
	}

	@Override
	protected boolean assemble() {
		timePairList = HawkJsonUtil.getJsonInstance().fromJson(time, new TypeToken<List<String[]>>() {}.getType());
		return true;
	}

	@Override
	protected boolean checkValid() {
		return true;
	}

	public String getId() {
		return id;
	}

	public int getCount() {
		return count;
	}

}
