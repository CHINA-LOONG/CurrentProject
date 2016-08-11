package com.hawk.game.config;

import java.util.HashMap;
import java.util.Map;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

import com.hawk.game.util.GsConst;

@HawkConfigManager.CsvResource(file = "staticData/imSys.csv", struct = "map")
public class ImSysCfg extends HawkConfigBase {

	@Id
	protected final int id;
	protected final String textId;
	protected final int type;
	protected final int channel;
	protected final String expansion;

	protected final String lang_1;
	protected final String sender_1;
	protected final String content_1;
	protected final String lang_2;
	protected final String sender_2;
	protected final String content_2;

	// assemble
	protected Map<String, String[]> languageDataMap;

	// client only
	protected final String comments = null;

	// constant
	protected final int SENDER_INDEX = 0;
	protected final int CONTENT_INDEX = 1;

	public ImSysCfg() {
		id = 0;
		textId = "";
		type = 0;
		channel = 0;
		expansion = "";
		lang_1 = lang_2 = "";
		sender_1 = sender_2 = "";
		content_1 = content_2 = "";
	}

	@Override
	protected boolean assemble() {
		languageDataMap = new HashMap<String, String[]>();
		languageDataMap.put(lang_1, new String[]{sender_1, content_1});
		languageDataMap.put(lang_2, new String[]{sender_2, content_2});

		return true;
	}

	@Override
	protected boolean checkValid() {
		return true;
	}

	public int getImSysId() {
		return id;
	}

	public String getImSysTextId() {
		return textId;
	}

	public int getType() {
		return type;
	}

	public int getChannel() {
		return channel;
	}

	public String getExpansion() {
		return expansion;
	}

	public String getSender(String lang) {
		String[] data = languageDataMap.get(lang);
		if (data != null) {
			return data[SENDER_INDEX];
		}
		return languageDataMap.get(GsConst.DEFAULT_LANGUAGE) [SENDER_INDEX];
	}

	public String getContent(String lang) {
		String[] data = languageDataMap.get(lang);
		if (data != null) {
			return data[CONTENT_INDEX];
		}
		return languageDataMap.get(GsConst.DEFAULT_LANGUAGE) [CONTENT_INDEX];
	}

}
