package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
@HawkConfigManager.CsvResource(file = "staticData/languageStatic.csv", struct = "map")
public class LanguageStaticCfg extends HawkConfigBase{
	/**
	 * 配置id
	 */
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

	public LanguageStaticCfg() {
		super();
		id = null;
		chinese = null;
		english = null;
	}

	public String getId() {
		return id;
	}

	public String getChinese() {
		return chinese;
	}

	public String getEnglish() {
		return english;
	}
	
	/**
	 *  获取多语言英文名称
	 * @param nameId
	 * @return
	 */
	public static String getEnglishName(String nameId){
		LanguageStaticCfg languageCfg = HawkConfigManager.getInstance().getConfigByKey(LanguageStaticCfg.class, nameId);
		if (languageCfg != null) {
			return languageCfg.getEnglish();
		}
		
		return "";
	}
}
