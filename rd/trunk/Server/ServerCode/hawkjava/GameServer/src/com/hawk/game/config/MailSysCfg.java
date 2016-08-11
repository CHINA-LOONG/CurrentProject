package com.hawk.game.config;

import java.util.HashMap;
import java.util.Map;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;

import com.hawk.game.util.GsConst;

@HawkConfigManager.CsvResource(file = "staticData/mailSys.csv", struct = "map")
public class MailSysCfg extends HawkConfigBase {

	@Id
	protected final int id;
	protected final String textId;
	protected final String rewardId;

	protected final String lang_1;
	protected final String sender_1;
	protected final String subject_1;
	protected final String content_1;
	protected final String lang_2;
	protected final String sender_2;
	protected final String subject_2;
	protected final String content_2;

	// assemble
	protected RewardCfg rewardCfg;
	protected Map<String, String[]> languageDataMap;

	// client only
	protected final String comments = null;

	// constant
	protected final int SENDER_INDEX = 0;
	protected final int SUBJECT_INDEX = 1;
	protected final int CONTENT_INDEX = 2;

	public MailSysCfg() {
		id = 0;
		textId = "";
		rewardId = "";
		lang_1 = lang_2 = "";
		sender_1 = sender_2 = "";
		subject_1 = subject_2 ="";
		content_1 = content_2 = "";
	}

	@Override
	protected boolean assemble() {
		languageDataMap = new HashMap<String, String[]>();
		languageDataMap.put(lang_1, new String[]{sender_1, subject_1, content_1});
		languageDataMap.put(lang_2, new String[]{sender_2, subject_2, content_2});

		return true;
	}

	@Override
	protected boolean checkValid() {
		if (rewardId != "") {
			// 检测奖励是否存在，并建立引用
			rewardCfg = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, rewardId);
			if (null == rewardCfg) {
				HawkLog.errPrintln(String.format("config invalid RewardCfg : %s", rewardId));
				return false;
			}
		}

		return true;
	}

	public int getMailSysId() {
		return id;
	}

	public String getMailSysTextId() {
		return textId;
	}

	public RewardCfg getReward() {
		return rewardCfg;
	}

	public String getSender(String lang) {
		String[] data = languageDataMap.get(lang);
		if (data != null) {
			return data[SENDER_INDEX];
		}
		return languageDataMap.get(GsConst.DEFAULT_LANGUAGE) [SENDER_INDEX];
	}

	public String getSubject(String lang) {
		String[] data = languageDataMap.get(lang);
		if (data != null) {
			return data[SUBJECT_INDEX];
		}
		return languageDataMap.get(GsConst.DEFAULT_LANGUAGE) [SUBJECT_INDEX];
	}

	public String getContent(String lang) {
		String[] data = languageDataMap.get(lang);
		if (data != null) {
			return data[CONTENT_INDEX];
		}
		return languageDataMap.get(GsConst.DEFAULT_LANGUAGE) [CONTENT_INDEX];
	}

}
