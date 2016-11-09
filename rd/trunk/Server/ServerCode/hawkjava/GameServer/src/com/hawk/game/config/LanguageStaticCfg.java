package com.hawk.game.config;

import java.util.HashMap;
import java.util.Map;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

import com.hawk.game.util.GsConst;
@HawkConfigManager.CsvResource(file = "staticData/languageStatic.csv", struct = "map")
public class LanguageStaticCfg extends HawkConfigBase{

	@Id
	protected final String id;
	/**
	 * 中文
	 */
	protected final String chinese;
	/**
	 * 英文
	 */
	protected final String english;

	// assemble
	protected Map<String, String> languageDataMap;

	public LanguageStaticCfg() {
		super();
		id = null;
		chinese = null;
		english = null;
	}

	@Override
	protected boolean assemble() {
		languageDataMap = new HashMap<String, String>();
		languageDataMap.put("zh-CN", chinese);
		languageDataMap.put("en", english);

		return true;
	}

	public String getId() {
		return id;
	}

	public String getContent(String lang) {
		String data = languageDataMap.get(lang);
		if (data != null) {
			return data;
		}
		return languageDataMap.get(GsConst.DEFAULT_LANGUAGE);
	}

	/**
	 *  获取多语言英文名称
	 * @param nameId
	 * @return
	 */
	public static String getEnglishName(String nameId){
		LanguageStaticCfg languageCfg = HawkConfigManager.getInstance().getConfigByKey(LanguageStaticCfg.class, nameId);
		if (languageCfg != null) {
			return languageCfg.getContent("en");
		}

		return "";
	}
}
