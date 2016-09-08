package com.hawk.game.module.alliance;

import org.hawk.app.HawkAppObj;
import org.hawk.config.HawkConfigManager;
import org.hawk.msg.HawkMsg;
import org.hawk.msg.HawkMsgHandler;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.obj.HawkObjBase;
import org.hawk.xid.HawkXID;

import com.hawk.game.GsApp;
import com.hawk.game.BILog.BIGuildFlowData;
import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.config.RewardCfg;
import com.hawk.game.config.SociatyTaskCfg;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.AllianceTeamEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.log.BILogger;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Alliance.HSAllianceTaskRewardRet;
import com.hawk.game.protocol.Alliance.HSAllianceTeamLeaveNotify;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.GsConst;

public class AllianceTaskRewardHandler implements HawkMsgHandler{
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
		
		if (teamEntity.isTaskFinish() == false) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_TASK_NOT_FINISH_VALUE);
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
					reward.addItemInfos(HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, taskCfg.getReward()).getRewardList());
					
					if (teamEntity.getCaptain() == player.getId()) {
						reward.addItemInfos(HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, taskCfg.getLeaderReward()).getRewardList());
					}
					
					reward.rewardTakeAffectAndPush(player, Action.GUILD_TASK_REWARD, protocol.getType());
					
					int contribution = reward.getRewardCount(Const.itemType.PLAYER_ATTR_VALUE, String.valueOf(Const.changeType.CHANGE_PLAYER_CONTRIBUTION));
					allianceEntity.addContribution(contribution);
					allianceEntity.notifyUpdate(true);
					
					teamEntity.removePlayerFromTeam(player.getId());
					teamEntity.getAcceptList().remove(player.getId());
					teamEntity.notifyUpdate(true);
					
					HSAllianceTeamLeaveNotify.Builder notify = HSAllianceTeamLeaveNotify.newBuilder();
					notify.setPlayerId(player.getId());
					AllianceManager.getInstance().broadcastNotify(teamEntity, HawkProtocol.valueOf(HS.code.ALLIANCE_TEMA_LEAVE_N_S, notify), 0);
					
					HSAllianceTaskRewardRet.Builder response = HSAllianceTaskRewardRet.newBuilder();
					player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_TASK_REWARD_S_VALUE, response));
					
					BILogger.getBIData(BIGuildFlowData.class).logTask(player, Action.GUILD_TASK_REWARD, allianceEntity, teamEntity, contribution, 0, 0);	
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
