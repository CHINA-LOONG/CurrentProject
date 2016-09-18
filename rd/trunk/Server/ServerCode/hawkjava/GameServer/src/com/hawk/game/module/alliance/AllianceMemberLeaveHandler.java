package com.hawk.game.module.alliance;

import java.util.Arrays;

import org.hawk.app.HawkAppObj;
import org.hawk.config.HawkConfigManager;
import org.hawk.msg.HawkMsg;
import org.hawk.msg.HawkMsgHandler;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkTime;

import com.hawk.game.ServerData;
import com.hawk.game.BILog.BIGuildMemberFlowData;
import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.config.MailSysCfg;
import com.hawk.game.config.SysBasicCfg;
import com.hawk.game.entity.AllianceApplyEntity;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.AllianceTeamEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.item.ItemInfo;
import com.hawk.game.log.BILogger;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.manager.ImManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Alliance.HSAllianceLeaveRet;
import com.hawk.game.protocol.Alliance.HSAllianceTeamLeaveNotify;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.AllianceUtil;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.MailUtil;
import com.hawk.game.util.MailUtil.MailInfo;

public class AllianceMemberLeaveHandler implements HawkMsgHandler{
	/**
	 * 消息处理器
	 * 
	 * @param appObj
	 * @param msg
	 * @return
	 */
	@Override
	public boolean onMessage(HawkAppObj appObj, HawkMsg msg)
	{
		Player player = (Player) msg.getParam(0);
		HawkProtocol protocol = (HawkProtocol)msg.getParam(1);

		if (player.getAllianceId() == 0) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NOT_JOIN_VALUE);
			return true;
		}
		
		AllianceEntity allianceEntity = AllianceManager.getInstance().getAlliance(player.getAllianceId());
		if (allianceEntity == null) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NOT_JOIN_VALUE);
			return true;
		}
		
		PlayerAllianceEntity playerAllianceEntity = allianceEntity.getMember(player.getId());
		if (playerAllianceEntity == null) {
			player.sendError(protocol.getType(), Status.error.SERVER_ERROR_VALUE);
			return true;
		}
		
		boolean deleteAlliance = true;
		if (allianceEntity.getMemberList().size() > 1) {
			if (playerAllianceEntity.getPlayerId() == allianceEntity.getPlayerId()) {
				//公会有多个成员会长不能退出
				player.sendError(protocol.getType(), Status.allianceError.ALLIANCE__LEAVE_NOT_EMPTY_VALUE);
				return true;
			}
			
			deleteAlliance = false;
		}
		
		playerAllianceEntity.setPostion(GsConst.Alliance.ALLIANCE_POS_COMMON);
		playerAllianceEntity.setAllianceId(0);
		playerAllianceEntity.setPreAllianceId(allianceEntity.getId());
		playerAllianceEntity.setExitTime(HawkTime.getSeconds() + SysBasicCfg.getInstance().getAllianceFrizenTime());
		playerAllianceEntity.notifyUpdate(true);

		// 清理公会驻兵
		int totalBaseReward = AllianceUtil.getTotalBaseReward(allianceEntity, playerAllianceEntity);
		allianceEntity.clearAllianceBase(player.getId());

		// 清理队伍信息
		AllianceTeamEntity teamEntity = allianceEntity.getTeamEntity(player.getId());
		if (teamEntity != null) {
			teamEntity.removePlayerFromTeam(player.getId());
			HSAllianceTeamLeaveNotify.Builder notify = HSAllianceTeamLeaveNotify.newBuilder();
			notify.setPlayerId(player.getId());
			AllianceManager.getInstance().broadcastNotify(teamEntity, HawkProtocol.valueOf(HS.code.ALLIANCE_TEMA_LEAVE_N_S, notify), 0);
		}

		AllianceManager.getInstance().removePlayerAndAllianceMap(player.getId());
		allianceEntity.removeMember(player.getId());

		// 离开公会频道
		ImManager.getInstance().quitGuild(allianceEntity.getId(), player.getId());
		if (0 < totalBaseReward) {
			MailSysCfg mailCfg = HawkConfigManager.getInstance().getConfigByKey(MailSysCfg.class, GsConst.SysMail.ALLIANCE_LEAVE_BASE);
			if (mailCfg != null) {
				MailInfo mailInfo = new MailInfo();
				String lang = ServerData.getInstance().getPlayerLang(player.getId());
				mailInfo.subject = mailCfg.getSubject(lang);
				mailInfo.content = mailCfg.getContent(lang);
				ItemInfo item = ItemInfo.valueOf(Const.itemType.PLAYER_ATTR_VALUE, String.valueOf(Const.changeType.CHANGE_COIN_VALUE), totalBaseReward);
				mailInfo.rewardList = Arrays.asList(item);

				MailUtil.SendMail(mailInfo, player.getId(), 0, mailCfg.getSender(lang));
			}
		}

		// 清理工会数据
		if (deleteAlliance) {
			AllianceManager.getInstance().removeAlliance(allianceEntity);
			AllianceManager.getInstance().removeAllianceForSort(allianceEntity);
			AllianceManager.getInstance().getExistName().remove(allianceEntity.getName());
			for (AllianceApplyEntity apply : allianceEntity.getApplyList().values()) {
				AllianceManager.getInstance().removePlayerApply(apply.getPlayerId(), apply.getAllianceId());
				apply.delete();
			}
			if (teamEntity != null) {
				teamEntity.delete();
			}
			
			allianceEntity.delete();
		}
		
		HSAllianceLeaveRet.Builder response = HSAllianceLeaveRet.newBuilder();
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_MEMBER_LEAVE_S_VALUE, response));
		
		BILogger.getBIData(BIGuildMemberFlowData.class).logOperation(
				player, 
				Action.GUILD_LEAVE, 
				allianceEntity,
				playerAllianceEntity, 
				""
				);
		
		// 广播
		//if (deleteAlliance == false) {
		//	HSMemberRemoveNotify.Builder notify = HSMemberRemoveNotify.newBuilder();
		//	notify.setPlayerId(player.getId());
		//	AllianceManager.getInstance().broadcastNotify(allianceEntity.getId(), HawkProtocol.valueOf(HS.code.ALLIANCE_MEMBER_REMOVE_N_S, response), player.getId());
		//}
		
		return true;
	}
}
