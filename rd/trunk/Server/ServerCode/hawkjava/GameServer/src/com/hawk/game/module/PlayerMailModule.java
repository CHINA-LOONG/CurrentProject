package com.hawk.game.module;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

import org.hawk.annotation.MessageHandler;
import org.hawk.annotation.ProtocolHandler;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;

import com.hawk.game.entity.MailEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ItemInfo;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Mail.HSMailDelete;
import com.hawk.game.protocol.Mail.HSMailNew;
import com.hawk.game.protocol.Mail.HSMailRead;
import com.hawk.game.protocol.Mail.HSMailReceive;
import com.hawk.game.protocol.Mail.HSMailReceiveAllRet;
import com.hawk.game.protocol.Mail.HSMailReceiveRet;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.BuilderUtil;
import com.hawk.game.util.GsConst;

public class PlayerMailModule extends PlayerModule {

	public PlayerMailModule(Player player) {
		super(player);
	}

	/**
	 * 收到新邮件
	 */
	@MessageHandler(code = GsConst.MsgType.MAIL_NEW)
	private boolean onNewMail(HawkMsg msg) {
		MailEntity mailEntity = msg.getParam(0);

		player.getPlayerData().addMailEntity(mailEntity);

		HSMailNew.Builder mailBuilder = HSMailNew.newBuilder();
		mailBuilder.setMail(BuilderUtil.genMailBuilder(mailEntity));

		List<Integer> overflowList = player.RemoveOverflowMail();
		if (false == overflowList.isEmpty()) {
			mailBuilder.setOverflowMailId(overflowList.get(0));
		}

		sendProtocol(HawkProtocol.valueOf(HS.code.MAIL_NEW_S, mailBuilder));
		return true;
	}

	/**
	 * 阅读邮件
	 */
	@ProtocolHandler(code = HS.code.MAIL_READ_C_VALUE)
	private boolean onMailRead(HawkProtocol cmd) {
		HSMailRead protocol = cmd.parseProtocol(HSMailRead.getDefaultInstance());
		int hsCode = cmd.getType();
		int mailId = protocol.getMailId();

		MailEntity mailEntity = player.getPlayerData().getMailEntity(mailId);
		if (mailEntity == null) {
			sendError(hsCode, Status.mailError.MAIL_NOT_EXIST);
			return true;
		}

		boolean update = false;
		if (mailEntity.getState() == Const.mailState.UNREAD_VALUE) {
			mailEntity.setState((byte)Const.mailState.READ_VALUE);
			update = true;
		}

		if (true == mailEntity.getRewardList().isEmpty()) {
			mailEntity.setInvalid(true);
			player.getPlayerData().removeMailEntity(mailEntity);
			update = true;
		}

		if (true == update) {
			mailEntity.notifyUpdate(true);
		}
		return true;
	}

	/**
	 * 删除邮件
	 */
	@ProtocolHandler(code = HS.code.MAIL_DELETE_C_VALUE)
	private boolean onMailDelete(HawkProtocol cmd) {
		HSMailDelete protocol = cmd.parseProtocol(HSMailDelete.getDefaultInstance());
		int hsCode = cmd.getType();
		int mailId = protocol.getMailId();

		MailEntity mailEntity = player.getPlayerData().getMailEntity(mailId);
		if (mailEntity == null) {
			sendError(hsCode, Status.mailError.MAIL_NOT_EXIST);
			return true;
		}

		mailEntity.setState((byte)Const.mailState.DELETE_VALUE);
		mailEntity.setInvalid(true);
		mailEntity.notifyUpdate(true);
		player.getPlayerData().removeMailEntity(mailEntity);
		return true;
	}

	/**
	 * 收取邮件
	 */
	@ProtocolHandler(code = HS.code.MAIL_RECEIVE_C_VALUE)
	private boolean onMailReceive(HawkProtocol cmd) {
		HSMailReceive protocol = cmd.parseProtocol(HSMailReceive.getDefaultInstance());
		int hsCode = cmd.getType();
		int mailId = protocol.getMailId();

		MailEntity mailEntity = player.getPlayerData().getMailEntity(mailId);
		if (mailEntity == null) {
			sendError(hsCode, Status.mailError.MAIL_NOT_EXIST);
			return true;
		}

		AwardItems sumAwardItems = AwardItems.valueOf();
		int status = receiveMail(mailEntity, sumAwardItems);
		if (status != Status.error.NONE_ERROR_VALUE) {
			sendError(hsCode, status);
			return true;
		}
		sumAwardItems.rewardTakeAffectAndPush(player,  Action.MAIL_REWARD, HS.code.MAIL_RECEIVE_C_VALUE);

		HSMailReceiveRet.Builder response = HSMailReceiveRet.newBuilder();
		response.setMailId(mailId);
		sendProtocol(HawkProtocol.valueOf(HS.code.MAIL_RECEIVE_S, response));
		return true;
	}

	/**
	 * 收取全部邮件
	 */
	@ProtocolHandler(code = HS.code.MAIL_RECEIVE_ALL_C_VALUE)
	private boolean onMailReceiveAll(HawkProtocol cmd) {
		int hsCode = cmd.getType();

		List<MailEntity> mailList = player.getPlayerData().getMailEntityList();
		if (true == mailList.isEmpty()) {
			sendError(hsCode, Status.mailError.MAIL_NONE);
			return true;
		}

		// 排序
		List<MailEntity> orderedMailList = new ArrayList<>(player.getPlayerData().getMailEntityList());
		Collections.sort(orderedMailList, new Comparator<MailEntity>() {
			public int compare(MailEntity arg0, MailEntity arg1) {
				// 1.优先显示“未读信件”，再显示“已读信件”
				if (arg0.getState() != arg1.getState()) {
					if (arg0.getState() == Const.mailState.UNREAD_VALUE) {
						return -1;
					} else if (arg1.getState() == Const.mailState.UNREAD_VALUE) {
						return 1;
					}
				}
				// 2.按接收邮件的时间排序，新邮件在前
				// mailId递增，顺序与时间一致，直接比较检查Id
				return Integer.compare(arg1.getId(), arg0.getId());
			}
		});

		AwardItems sumAwardItems = AwardItems.valueOf();
		int status = Status.error.NONE_ERROR_VALUE;
		List<Integer> receiveMailIdList = new ArrayList<>();
		for (MailEntity mailEntity : orderedMailList) {
			status = receiveMail(mailEntity, sumAwardItems);

			if (status == Status.error.NONE_ERROR_VALUE) {
				receiveMailIdList.add(mailEntity.getId());
			} else {
				break;
			}
		}
		sumAwardItems.rewardTakeAffectAndPush(player, Action.MAIL_REWARD, HS.code.MAIL_RECEIVE_ALL_C_VALUE);

		HSMailReceiveAllRet.Builder response = HSMailReceiveAllRet.newBuilder();
		response.setStatus(status);
		response.addAllReceiveMailId(receiveMailIdList);
		sendProtocol(HawkProtocol.valueOf(HS.code.MAIL_RECEIVE_ALL_S, response));
		return true;
	}

	// 内部函数------------------------------------------------------------------------------------------

	/**
	 * 收取邮件内部函数
	 * @return 错误码
	 */
	private int receiveMail(MailEntity mailEntity, AwardItems sumAwardItems) {
		List<ItemInfo> rewardList = mailEntity.getRewardList();
		if (false == rewardList.isEmpty()) {
			sumAwardItems.addItemInfos(rewardList);
			// TODO: 检测金币、钻石上限
		}

		mailEntity.setState((byte)Const.mailState.RECEIVE_VALUE);
		mailEntity.setInvalid(true);
		mailEntity.notifyUpdate(true);
		player.getPlayerData().removeMailEntity(mailEntity);

		return Status.error.NONE_ERROR_VALUE;
	}

	@Override
	protected boolean onPlayerLogin() {
		// 加载所有邮件数据
		player.getPlayerData().loadAllMail();

		// 删除多余邮件
		player.RemoveOverflowMail();

		// 同步邮件信息
		player.getPlayerData().syncMailInfo();

		return true;
	}

	@Override
	protected boolean onPlayerLogout() {
		// 重要数据下线就存储
		for (MailEntity entity : player.getPlayerData().getMailEntityList()) {
			entity.notifyUpdate(false);
		}
		return true;
	}
}
