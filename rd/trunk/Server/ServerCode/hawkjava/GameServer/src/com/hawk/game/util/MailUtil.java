package com.hawk.game.util;

import java.util.List;

import org.hawk.app.HawkApp;
import org.hawk.log.HawkLog;
import org.hawk.msg.HawkMsg;
import org.hawk.xid.HawkXID;

import com.hawk.game.ServerData;
import com.hawk.game.config.MailSysCfg;
import com.hawk.game.entity.MailEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ItemInfo;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.log.BehaviorLogger.Source;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.Reward.RewardItem;

public class MailUtil {

	public static class MailInfo {
		public String subject;
		public String content;
		public List<ItemInfo> rewardList;
	}

	/**
	 * 群发系统邮件
	 */
	public static void SendSysMail(MailSysCfg mailCfg, List<Integer> receiverIdList, Source source, Action action) {
		MailInfo mailInfo = new MailInfo();
		mailInfo.subject = mailCfg.getSubject();
		mailInfo.content = mailCfg.getContent();
		mailInfo.rewardList = mailCfg.getReward().getRewardList();
		
		SendMail(mailInfo, receiverIdList, 0, mailCfg.getSender(), source, action);
	}
	
	/**
	 * 群发邮件
	 */
	public static void SendMail(MailInfo mailInfo, List<Integer> receiverIdList, int senderId, String senderName, Source source, Action action) {
		for (Integer receiverId : receiverIdList) {
			SendMail(mailInfo, receiverId, senderId, senderName, source, action);
		}
	}

	/**
	 * 发送邮件
	 * @return entityId, <0 表示失败
	 */
	public static int SendMail(MailInfo mailInfo, int receiverId, int senderId, String senderName, Source source, Action action) {
		MailEntity mailEntity = new MailEntity();
		mailEntity.setReceiverId(receiverId);
		mailEntity.setSenderId(senderId);
		mailEntity.setSenderName(senderName);
		mailEntity.setState((byte)Const.mailState.UNREAD_VALUE);
		mailEntity.setSubject(mailInfo.subject);
		mailEntity.setContent(mailInfo.content);
		mailEntity.setRewardList(mailInfo.rewardList);

		if (true == mailEntity.notifyCreate()) {
			// 如果receiver在线，发msg
			if (true == ServerData.getInstance().isPlayerOnline(receiverId)) {
				HawkXID receiverXID = HawkXID.valueOf(GsConst.ObjType.PLAYER, receiverId);
				HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.MAIL_NEW);
				msg.pushParam(mailEntity);
				if (false == HawkApp.getInstance().postMsg(receiverXID, msg)) {
					HawkLog.errPrintln("post mail message failed, sender:" + senderName);
				}
			}
			return mailEntity.getId();
		}

		return -1;
	}
}
