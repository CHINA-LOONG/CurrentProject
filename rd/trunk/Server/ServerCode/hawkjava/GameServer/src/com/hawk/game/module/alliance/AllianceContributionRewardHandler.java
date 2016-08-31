package com.hawk.game.module.alliance;

import org.hawk.app.HawkAppObj;
import org.hawk.config.HawkConfigManager;
import org.hawk.msg.HawkMsg;
import org.hawk.msg.HawkMsgHandler;
import org.hawk.net.protocol.HawkProtocol;

import com.hawk.game.config.RewardCfg;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Alliance.HSAllianceContriRewardRet;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.Alliance.HSAllianceContriReward;
import com.hawk.game.util.AllianceUtil;
import com.hawk.game.util.GsConst;

public class AllianceContributionRewardHandler implements HawkMsgHandler{
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
		HSAllianceContriReward request = protocol.parseProtocol(HSAllianceContriReward.getDefaultInstance());
	
		if(player.getAllianceId() == 0){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NOT_JOIN_VALUE);
			return true;
		}
		
		AllianceEntity allianceEntity = AllianceManager.getInstance().getAlliance(player.getAllianceId());
		if (allianceEntity == null) {
			player.sendError(protocol.getType(), Status.error.SERVER_ERROR_VALUE);
			return true;
		}
		
		PlayerAllianceEntity playerAllianceEntity = allianceEntity.getMember(player.getId());
		if (playerAllianceEntity == null) {
			player.sendError(protocol.getType(), Status.error.SERVER_ERROR_VALUE);
			return true;
		}
	
		int contributon = 0;
		String rewardId = null;
		if (GsConst.AllianceReward.FIRST_REWARD.ordinal() == request.getIndex()) {
			contributon = GsConst.AllianceReward.FIRST_REWARD.GetRewardCount();
			rewardId = GsConst.Alliance.ALLIANCE_CONTRI_REWARD1;
		}
		else if (GsConst.AllianceReward.SECOND_REWARD.ordinal() == request.getIndex()) {
			contributon = GsConst.AllianceReward.SECOND_REWARD.GetRewardCount();
			rewardId = GsConst.Alliance.ALLIANCE_CONTRI_REWARD2;
		}
		else if (GsConst.AllianceReward.THIRD_REWARD.ordinal() == request.getIndex()) {
			contributon = GsConst.AllianceReward.THIRD_REWARD.GetRewardCount();
			rewardId = GsConst.Alliance.ALLIANCE_CONTRI_REWARD3;
		}
		else {
			player.sendError(protocol.getType(), Status.error.PARAMS_INVALID_VALUE);
			return true;
		}
		
		if (allianceEntity.getContribution0() < contributon) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_CONTRI_NOT_ENOUGH_VALUE);
			return true;
		}
		
		if (AllianceUtil.isAllianceContriRewardDaily(request.getIndex()) == true) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_REWARD_ALREADY_GIVE_VALUE);
			return true;
		}
		
		RewardCfg rewardCfg = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, rewardId);
		if (rewardCfg == null) {
			player.sendError(protocol.getType(), Status.error.SERVER_ERROR_VALUE);
			return true;
		}
		
		player.getPlayerData().getStatisticsEntity().setAllianceContriRewardDaily(request.getIndex());
		player.getPlayerData().getStatisticsEntity().notifyUpdate(true);
		
		AwardItems award = new AwardItems();
		award.addItemInfos(rewardCfg.getRewardList());
		award.rewardTakeAffectAndPush(player, Action.ALLIANCE_CONTRIBUTION_REWARD, protocol.getType());
		
		HSAllianceContriRewardRet.Builder resonse = HSAllianceContriRewardRet.newBuilder();
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_CONTRI_REWARD_S_VALUE, resonse));
		
		return true;
	}
}
