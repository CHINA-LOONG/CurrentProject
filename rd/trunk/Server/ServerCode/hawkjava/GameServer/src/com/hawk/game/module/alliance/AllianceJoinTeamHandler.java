package com.hawk.game.module.alliance;

import org.hawk.app.HawkAppObj;
import org.hawk.config.HawkConfigManager;
import org.hawk.msg.HawkMsg;
import org.hawk.msg.HawkMsgHandler;
import org.hawk.net.protocol.HawkProtocol;

import com.hawk.game.config.SociatyTaskCfg;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.AllianceTeamEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Alliance.HSAllianceJoinTeam;
import com.hawk.game.protocol.Alliance.HSAllianceJoinTeamRet;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;

public class AllianceJoinTeamHandler implements HawkMsgHandler{
	/**
	 * 消息处理器
	 * 
	 * @param appObj
	 * @param msg
	 * @return
	 */
	@Override
	public boolean onMessage (HawkAppObj appObj, HawkMsg msg)
	{
		Player player = (Player) msg.getParam(0);
		HawkProtocol protocol = (HawkProtocol)msg.getParam(1);
		HSAllianceJoinTeam request = protocol.parseProtocol(HSAllianceJoinTeam.getDefaultInstance());
		
		if(player.getPlayerData().getPlayerAllianceEntity().getAllianceId() == 0){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NOT_JOIN_VALUE);
			return true;
		}	

		AllianceEntity allianceEntity= AllianceManager.getInstance().getAlliance(player.getPlayerData().getPlayerAllianceEntity().getAllianceId());
		if (allianceEntity == null) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NOT_EXIST_VALUE);
			return true;
		}
		
		PlayerAllianceEntity playerAllianceEntity = allianceEntity.getMember(player.getId());
		if (playerAllianceEntity == null) {
			player.sendError(protocol.getType(), Status.error.SERVER_ERROR_VALUE);
			return true;
		}
	
		if (allianceEntity.getPlayerTeamMap().get(player.getId()) != null ) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_ALREADY_IN_TEAM_VALUE);
			return true;
		}
		
		AllianceTeamEntity teamEntity = allianceEntity.getFinishTeamList().get(request.getTeamId());
		if (teamEntity != null) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_TEAM_FINISH_VALUE);
			return true;
		}
		
		teamEntity = allianceEntity.getUnfinishTeamList().get(request.getTeamId());
		if (teamEntity == null) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_TEAM_NOT_EXIST_VALUE);
			return true;
		}
		
		SociatyTaskCfg taskCfg = HawkConfigManager.getInstance().getConfigByKey(SociatyTaskCfg.class, teamEntity.getTaskId());
		if (taskCfg == null) {
			player.sendError(protocol.getType(), Status.error.CONFIG_ERROR_VALUE);
			return true;
		}
		
		if (taskCfg.getMinLevel() > player.getLevel()) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_LEVEL_LIMIT_VALUE);
			return true;
		}
		
		if (teamEntity.isTeamFull() == true) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_TEAM_FULL_VALUE);
			return true;
		}
		
		teamEntity.addPlayerToTeam(request.getTeamId(), player.getId());
		
		teamEntity.notifyUpdate(true);
		allianceEntity.addPlayerTeamMap(player.getId(), teamEntity.getId());
		
		HSAllianceJoinTeamRet.Builder response = HSAllianceJoinTeamRet.newBuilder();
		response.setTeamId(teamEntity.getId());
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_JOIN_TEAM_S_VALUE, response));
		return true;
	}
}
