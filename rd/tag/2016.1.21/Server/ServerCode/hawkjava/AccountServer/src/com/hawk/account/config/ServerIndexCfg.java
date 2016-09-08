package com.hawk.account.config;

import java.util.HashMap;
import java.util.Map;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

import com.hawk.account.AccountServices;

@HawkConfigManager.CsvResource(file = "cfg/index.csv", struct = "map")
public class ServerIndexCfg extends HawkConfigBase{
	/**
	 * 区间
	 */
	@Id
	final protected int area;
	/**
	 * 服务器多语言名称
	 */
	protected final String lang_1;
	protected final String name_1;
	protected final String lang_2;
	protected final String name_2;

	protected Map<String, String> languageDataMap;

	public ServerIndexCfg(){
		area = 0;
		lang_1 = "";
		lang_2 = "";
		name_1 = "";
		name_2 = "";
	}

	public int getArea() {
		return area;
	}

	public String getName(String lang) {
		String name = languageDataMap.get(lang);
		if (name != null) {
			return name;
		}
		return languageDataMap.get(AccountServices.DEFAULT_LANGUAGE);
	}

	@Override
	protected boolean assemble() {
		languageDataMap = new HashMap<String, String>();
		languageDataMap.put(lang_1, name_1);
		languageDataMap.put(lang_2, name_2);
		return true;
	}
}
