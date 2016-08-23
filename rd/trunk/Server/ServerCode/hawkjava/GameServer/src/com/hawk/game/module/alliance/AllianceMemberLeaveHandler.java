package com.hawk.game.module.alliance;

import org.hawk.app.HawkAppObj;
import org.hawk.msg.HawkMsg;
import org.hawk.msg.HawkMsgHandler;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkTime;

import com.hawk.game.config.SysBasicCfg;
import com.hawk.game.entity.AllianceApplyEntity;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.AllianceTeamEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.manager.ImManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Alliance.HSAllianceLeaveRet;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.GsConst;

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
		
		boolean deleteAlliance = false;
		if (allianceEntity.getMemberList().size() > 1) {
			if (playerAllianceEntity.getPlayerId() == allianceEntity.getPlayerId()) {
				//公会有多个成员会长不能退出
				player.sendError(protocol.getType(), Status.allianceError.ALLIANCE__LEAVE_NOT_EMPTY_VALUE);
				return true;
			}

			allianceEntity.removeMember(player.getId());
		}
		else
		{
			allianceEntity.getMemberList().clear();
			allianceEntity.delete();
			deleteAlliance = true;
		}
		
		playerAllianceEntity.setPostion(GsConst.Alliance.ALLIANCE_POS_COMMON);
		playerAllianceEntity.setAllianceId(0);
		playerAllianceEntity.setPreAllianceId(allianceEntity.getId());
		playerAllianceEntity.setExitTime(HawkTime.getSeconds() + SysBasicCfg.getInstance().getAllianceFrizenTime());
		playerAllianceEntity.notifyUpdate(true);
		
		AllianceManager.getInstance().removePlayerAndAllianceMap(player.getId());
		
		AllianceTeamEntity teamEntity = allianceEntity.getTeamEntity(player.getId());
		if (teamEntity != null) {
			teamEntity.removePlayerFromTeam(player.getId());
		}

		ImManager.getInstance().quitGuild(allianceEntity.getId(), player.getId());

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
		}
		
		HSAllianceLeaveRet.Builder response = HSAllianceLeaveRet.newBuilder();
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_MEMBER_LEAVE_S_VALUE, response));
		
		
		// 广播
		//if (deleteAlliance == false) {
		//	HSMemberRemoveNotify.Builder notify = HSMemberRemoveNotify.newBuilder();
		//	notify.setPlayerId(player.getId());
		//	AllianceManager.getInstance().broadcastNotify(allianceEntity.getId(), HawkProtocol.valueOf(HS.code.ALLIANCE_MEMBER_REMOVE_N_S, response), player.getId());
		//}
		
		return true;
	}
}
