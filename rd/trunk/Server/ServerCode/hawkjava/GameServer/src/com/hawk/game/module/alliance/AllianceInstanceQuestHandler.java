package com.hawk.game.module.alliance;

import org.hawk.app.HawkAppObj;
import org.hawk.config.HawkConfigManager;
import org.hawk.msg.HawkMsg;
import org.hawk.msg.HawkMsgHandler;
import org.hawk.obj.HawkObjBase;
import org.hawk.xid.HawkXID;

import com.hawk.game.GsApp;
import com.hawk.game.config.RewardCfg;
import com.hawk.game.config.SociatyQuestCfg;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.AllianceTeamEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Const;
import com.hawk.game.util.GsConst;

public class AllianceInstanceQuestHandler implements HawkMsgHandler{
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
		String instanceId = (String) msg.getParam(1);
		
		if(player.getAllianceId() == 0){
			return true;
		}
		
		AllianceEntity allianceEntity = AllianceManager.getInstance().getAlliance(player.getAllianceId());
		if (allianceEntity == null) {
			return true;
		}
		
		PlayerAllianceEntity playerAllianceEntity = allianceEntity.getMember(player.getId());
		if (playerAllianceEntity == null) {
			return true;
		}
		
		AllianceTeamEntity teamEntity = allianceEntity.getTeamEntity(player.getId());
		if (teamEntity == null) {
			return true;
		}
		
		if (teamEntity.getInstanceQuest1PlayerId() != 0) {
			return true;
		}
		
		SociatyQuestCfg questCfg = HawkConfigManager.getInstance().getConfigByKey(SociatyQuestCfg.class, teamEntity.getInstanceQuest1());
		if (questCfg == null) {
			return true;
		}
		
		if (!questCfg.getGoalParam().equals(instanceId)) {
			return true;
		}
		
		// 锁住玩家
		HawkXID xid = HawkXID.valueOf(GsConst.ObjType.PLAYER, player.getId());
		if(xid != null){
			HawkObjBase<HawkXID, HawkAppObj> objBase = GsApp.getInstance().lockObject(xid);
			try {
				if (objBase != null && objBase.isObjValid()) {				
					teamEntity.setQuestFinish(teamEntity.getInstanceQuest1(), player.getId());
					teamEntity.notifyUpdate(true);
					AwardItems reward = new AwardItems();
					reward.addItemInfos(HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, questCfg.getRewardId()).getRewardList());
					int contribution = reward.getRewardCount(Const.itemType.ALLIANCE_VALUE, String.valueOf(Const.changeType.CHANGE_PLAYER_CONTRIBUTION_VALUE)) ;
					if (contribution != 0) {
						allianceEntity.addContribution(contribution);
						allianceEntity.notifyUpdate(true);
					}
					
					reward.rewardTakeAffectAndPush(player, Action.ALLIANCE_QUEST_REWARD, 0);
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
