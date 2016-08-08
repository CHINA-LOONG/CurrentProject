package com.hawk.game.module.alliance;

import org.hawk.app.HawkAppObj;
import org.hawk.config.HawkConfigManager;
import org.hawk.msg.HawkMsg;
import org.hawk.msg.HawkMsgHandler;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.obj.HawkObjBase;
import org.hawk.xid.HawkXID;

import com.hawk.game.GsApp;
import com.hawk.game.config.SociatyTaskCfg;
import com.hawk.game.config.SysBasicCfg;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.AllianceTeamEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Alliance.HSAllianceDissolveTeamRet;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.Alliance.HSAllianceDissolveTeam;
import com.hawk.game.util.GsConst;

public class AllianceDissolveTeamHandler implements HawkMsgHandler{
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
		
		if (teamEntity.getCaptain() != player.getId()) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NOT_CAPTAIN_VALUE);
			return true;
		}
		
		if (teamEntity.getCaptain() != player.getId()) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NOT_CAPTAIN_VALUE);
			return true;
		}
		
		if (teamEntity.hasTeamMember() == true) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_HAVE_MEMBER_VALUE);
			return true;
		}
		
		if (teamEntity.isTaskFinish() == true) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_TASK_FINISH_VALUE);
			return true;
		}
		
		SociatyTaskCfg taskCfg = HawkConfigManager.getInstance().getConfigByKey(SociatyTaskCfg.class, teamEntity.getTaskId());
		if (taskCfg == null) {
			player.sendError(protocol.getType(), Status.error.CONFIG_ERROR_VALUE);
			return true;
		}
		
		// 锁住玩家
		HawkXID xid = HawkXID.valueOf(GsConst.ObjType.PLAYER, player.getId());
		if(xid != null){
			HawkObjBase<HawkXID, HawkAppObj> objBase = GsApp.getInstance().lockObject(xid);
			try {
				if (objBase != null && objBase.isObjValid()) {
					AwardItems reward = new AwardItems();
					reward.addFreeGold((int)(SysBasicCfg.getInstance().getAllianceDissoveGoldRate() * taskCfg.getTaskStart()));
					teamEntity.removePlayerFromTeam(player.getId());
					reward.rewardTakeAffectAndPush(player, Action.ALLIANCE_DISSOLVE_TEAM_REWARD, protocol.getType());
					
					HSAllianceDissolveTeamRet.Builder response = HSAllianceDissolveTeamRet.newBuilder();
					player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_DISSOVLE_TEAM_S_VALUE, response));				
					return true;
				}
			} 
			finally {
				if (objBase != null) {
					objBase.unlockObj();
				}
			}
		}
		return true;
	}
}
