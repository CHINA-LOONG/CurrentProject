package com.hawk.game.module.alliance;

import org.hawk.app.HawkAppObj;
import org.hawk.msg.HawkMsg;
import org.hawk.msg.HawkMsgHandler;
import org.hawk.net.protocol.HawkProtocol;

import com.hawk.game.config.SysBasicCfg;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.AllianceTeamEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Alliance.HSAllianceTaskAccept;
import com.hawk.game.protocol.Alliance.HSAllianceTaskAcceptRet;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;

public class AllianceAcceptQuestHandler implements HawkMsgHandler{
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
		HSAllianceTaskAccept request = protocol.parseProtocol(HSAllianceTaskAccept.getDefaultInstance());
		
		if(player.getAllianceId() == 0){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NOT_IN_TEAM_VALUE);
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
		
		AllianceTeamEntity teamEntity = allianceEntity.getTeamEntity(player.getId());
		if (teamEntity == null) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NOT_IN_TEAM_VALUE);
			return true;
		}
		
		if (teamEntity.getInstanceQuest1() != request.getQuestId()) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_QUEST_NOT_EXIST_VALUE);
			return true;
		}
		
		if (teamEntity.getInstanceQuest1PlayerId() != 0) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_QUEST_FINISH_VALUE);
			return true;
		}
		
		if (teamEntity.getFinishCount(player.getId()) >= SysBasicCfg.getInstance().getAllianceMaxSmallTask()) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_MAX_SMALL_TASK_VALUE);
			return true;
		}
		
		teamEntity.addAcceptMap(player.getId());
		teamEntity.notifyUpdate(true);
		
		HSAllianceTaskAcceptRet.Builder response = HSAllianceTaskAcceptRet.newBuilder();
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_ACCEPT_TASK_S_VALUE, response));
		return true;
	}
}
