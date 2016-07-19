package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/mailSys.csv", struct = "map")
public class MailSysCfg extends HawkConfigBase {

	@Id
	protected final String id;
	protected final String reason;
	protected final String sender;
	protected final String subject;
	protected final String content;
	protected final String rewardId;

	// assemble
	protected RewardCfg rewardCfg;

	public MailSysCfg() {
		id = "";
		reason = "";
		sender = "";
		subject = "";
		content = "";
		rewardId = "";
	}

	@Override
	protected boolean assemble() {
		return true;
	}

	@Override
	protected boolean checkValid() {
		// 检测奖励是否存在，并建立引用
		rewardCfg = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, rewardId);
		if (null == rewardCfg) {
			return false;
		}
		return true;
	}

	public String getMailSysId() {
		return id;
	}

	public String getReason() {
		return reason;
	}

	public String getSender() {
		return sender;
	}

	public String getSubject() {
		return subject;
	}

	public String getContent() {
		return content;
	}

	public RewardCfg getReward() {
		return rewardCfg;
	}

}
