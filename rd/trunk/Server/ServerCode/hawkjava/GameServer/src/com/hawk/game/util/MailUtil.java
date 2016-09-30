package com.hawk.game.util;

import java.text.MessageFormat;
import java.util.List;

import org.hawk.app.HawkApp;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;
import org.hawk.msg.HawkMsg;
import org.hawk.xid.HawkXID;

import com.hawk.game.GsApp;
import com.hawk.game.ServerData;
import com.hawk.game.config.MailSysCfg;
import com.hawk.game.config.RewardCfg;
import com.hawk.game.entity.MailEntity;
import com.hawk.game.item.ItemInfo;
import com.hawk.game.protocol.Const;

public class MailUtil {

	public static class MailInfo {
		public String subject;
		public String content;
		public List<ItemInfo> rewardList;
	}

	/**
	 * 群发系统邮件，多语言
	 */
	public static void SendSysMail(MailSysCfg mailCfg, List<Integer> receiverIdList, Object... contentArgs) {
		MailInfo mailInfo = new MailInfo();
		RewardCfg reward = mailCfg.getReward();
		if (reward != null) {
			mailInfo.rewardList = reward.getRewardList();
		}

		for (Integer receiverId : receiverIdList) {
			String lang = ServerData.getInstance().getPlayerLang(receiverId);
			mailInfo.subject = mailCfg.getSubject(lang);
			mailInfo.content = MessageFormat.format(mailCfg.getContent(lang), contentArgs);

			SendMail(mailInfo, receiverId, 0, mailCfg.getSender(lang));
		}
	}

	/**
	 * 单发系统邮件，多语言
	 */
	public static void SendSysMail(MailSysCfg mailCfg, int receiverId, Object... contentArgs) {
		MailInfo mailInfo = new MailInfo();
		String lang = ServerData.getInstance().getPlayerLang(receiverId);
		mailInfo.subject = mailCfg.getSubject(lang);
		mailInfo.content = MessageFormat.format(mailCfg.getContent(lang), contentArgs);
		RewardCfg reward = mailCfg.getReward();
		if (reward != null) {
			mailInfo.rewardList = reward.getRewardList();
		}

		SendMail(mailInfo, receiverId, 0, mailCfg.getSender(lang));
	}

	/**
	 * 单发系统邮件，多语言
	 */
	public static void SendSysMailWithReward(MailSysCfg mailCfg, int receiverId, String rewardId, Object... contentArgs) {
		MailInfo mailInfo = new MailInfo();
		String lang = ServerData.getInstance().getPlayerLang(receiverId);
		mailInfo.subject = mailCfg.getSubject(lang);
		mailInfo.content = MessageFormat.format(mailCfg.getContent(lang), contentArgs);
		RewardCfg reward = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, rewardId);
		if (reward != null) {
			mailInfo.rewardList = reward.getRewardList();
		}

		SendMail(mailInfo, receiverId, 0, mailCfg.getSender(lang));
	}
	
	/**
	 * 群发邮件，单语言
	 */
	public static void SendMail(MailInfo mailInfo, List<Integer> receiverIdList, int senderId, String senderName) {
		for (Integer receiverId : receiverIdList) {
			SendMail(mailInfo, receiverId, senderId, senderName);
		}
	}

	/**
	 * 单发邮件，单语言
	 * @return entityId, <0 表示失败
	 */
	public static int SendMail(MailInfo mailInfo, int receiverId, int senderId, String senderName) {
		if (mailInfo == null) {
			return GsConst.UNUSABLE;
		}
		if (mailInfo.subject == null) {
			mailInfo.subject = "";
		}
		if (mailInfo.content == null) {
			mailInfo.content = "";
		}
		if (senderName == null) {
			senderName = "";
		}

		MailEntity mailEntity = new MailEntity();
		mailEntity.setReceiverId(receiverId);
		mailEntity.setSenderId(senderId);
		mailEntity.setSenderName(senderName);
		mailEntity.setState((byte)Const.mailState.UNREAD_VALUE);
		mailEntity.setSubject(mailInfo.subject);
		mailEntity.setContent(mailInfo.content);
		if (mailInfo.rewardList != null) {
			mailEntity.setRewardList(mailInfo.rewardList);
		}

		if (true == mailEntity.notifyCreate()) {
			// 如果receiver在线或在缓存中，发msg
			// 忽略发送过程中receiver因超时被清理掉的错误，没有影响
			HawkXID receiverXID = HawkXID.valueOf(GsConst.ObjType.PLAYER, receiverId);
			if (null != GsApp.getInstance().queryObject(receiverXID)) {
				HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.MAIL_NEW);
				msg.pushParam(mailEntity);
				if (false == HawkApp.getInstance().postMsg(receiverXID, msg)) {
					HawkLog.errPrintln("post mail message failed, sender:" + senderName);
				}
			}
			return mailEntity.getId();
		}

		return GsConst.UNUSABLE;
	}
}
