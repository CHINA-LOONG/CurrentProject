package com.hawk.game.module.alliance;

import org.hawk.app.HawkAppObj;
import org.hawk.config.HawkConfigManager;
import org.hawk.msg.HawkMsg;
import org.hawk.msg.HawkMsgHandler;
import org.hawk.net.protocol.HawkProtocol;

import com.hawk.game.ServerData;
import com.hawk.game.BILog.BIGuildMemberFlowData;
import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.config.MailSysCfg;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.AllianceTeamEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.log.BILogger;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.manager.ImManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.Alliance.HSAllianceLeave;
import com.hawk.game.protocol.Alliance.HSAllianceMemKick;
import com.hawk.game.protocol.Alliance.HSAllianceMemKickRet;
import com.hawk.game.protocol.Alliance.HSAllianceTeamLeaveNotify;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.MailUtil;

public class AllianceMemberKickHandler implements HawkMsgHandler{
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
		HSAllianceMemKick request = protocol.parseProtocol(HSAllianceMemKick.getDefaultInstance());

		if (player.getAllianceId() == 0) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NOT_JOIN_VALUE);
			return true;
		}
		
		AllianceEntity allianceEntity = AllianceManager.getInstance().getAlliance(player.getAllianceId());
		if (allianceEntity == null) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NOT_JOIN_VALUE);
			return true;
		}
		
		PlayerAllianceEntity selfPlayerAllianceEntity = allianceEntity.getMember(player.getId());
		if (selfPlayerAllianceEntity == null) {
			// 不应该出现的情况
			player.sendError(protocol.getType(), Status.error.SERVER_ERROR_VALUE);
			return true;
		}
		
		PlayerAllianceEntity targetPlayerAllianceEntity = allianceEntity.getMember(request.getTargetId());
		if (targetPlayerAllianceEntity == null ) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_TARGET_NOT_JOIN_VALUE);
			return true;
		}
		
		if (selfPlayerAllianceEntity.getPostion() <= targetPlayerAllianceEntity.getPostion()) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_POSITION_ERROR_VALUE);
			return true;
		}
		
		targetPlayerAllianceEntity.setPostion(GsConst.Alliance.ALLIANCE_POS_COMMON);
		targetPlayerAllianceEntity.setAllianceId(0);
		targetPlayerAllianceEntity.setPreAllianceId(allianceEntity.getId());
		targetPlayerAllianceEntity.notifyUpdate(true);

		// 清理公会驻兵
		allianceEntity.clearAllianceBase(request.getTargetId());
		
		AllianceTeamEntity teamEntity = allianceEntity.getTeamEntity(request.getTargetId());
		if (teamEntity != null) {
			teamEntity.removePlayerFromTeam(request.getTargetId());
			HSAllianceTeamLeaveNotify.Builder notify = HSAllianceTeamLeaveNotify.newBuilder();
			notify.setPlayerId(request.getTargetId());
			AllianceManager.getInstance().broadcastNotify(teamEntity, HawkProtocol.valueOf(HS.code.ALLIANCE_TEMA_LEAVE_N_S, notify), 0);	
		}

		AllianceManager.getInstance().removePlayerAndAllianceMap(request.getTargetId());
		allianceEntity.removeMember(request.getTargetId());
		
		// 离开公会频道
		ImManager.getInstance().quitGuild(allianceEntity.getId(), request.getTargetId());
		MailSysCfg mailCfg = HawkConfigManager.getInstance().getConfigByKey(MailSysCfg.class, GsConst.SysMail.ALLIANCE_KICK);
		if (mailCfg != null) {
			MailUtil.SendSysMail(mailCfg, request.getTargetId(), allianceEntity.getName());
		}

		HSAllianceMemKickRet.Builder response = HSAllianceMemKickRet.newBuilder();
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_MEMBER_KCIK_S_VALUE, response));
		
		HawkProtocol notify = HawkProtocol.valueOf(HS.code.ALLIANCE_LEAVE_N_S_VALUE, HSAllianceLeave.newBuilder());
		AllianceManager.getInstance().memberNotify(targetPlayerAllianceEntity.getPlayerId(), notify);
		
		BILogger.getBIData(BIGuildMemberFlowData.class).logOperation(
				player, 
				Action.GUILD_KICK, 
				allianceEntity,
				selfPlayerAllianceEntity, 
				ServerData.getInstance().getPuidByPlayerId(targetPlayerAllianceEntity.getPlayerId())
				);
		
		// 广播
		//HSMemberRemoveNotify.Builder notify = HSMemberRemoveNotify.newBuilder();
		//notify.setPlayerId(request.getTargetId());
		//AllianceManager.getInstance().broadcastNotify(allianceEntity.getId(), HawkProtocol.valueOf(HS.code.ALLIANCE_MEMBER_REMOVE_N_S, response), player.getId());
		return true;
	}
}
