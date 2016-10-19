package com.hawk.game.module.alliance;

import org.hawk.app.HawkAppObj;
import org.hawk.config.HawkConfigManager;
import org.hawk.msg.HawkMsg;
import org.hawk.msg.HawkMsgHandler;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.obj.HawkObjBase;
import org.hawk.xid.HawkXID;

import com.hawk.game.GsApp;
import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.BILog.BIGuildFlowData;
import com.hawk.game.config.SociatyQuestCfg;
import com.hawk.game.config.SysBasicCfg;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.AllianceTeamEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.log.BILogger;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Alliance.HSAllianceTaskCommit;
import com.hawk.game.protocol.Alliance.HSAllianceTaskCommitRet;
import com.hawk.game.protocol.Alliance.HSAllianceTeamQuestFinishNotify;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.GsConst;

public class AllianceCommitQuestHandler implements HawkMsgHandler{
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
		HSAllianceTaskCommit request = protocol.parseProtocol(HSAllianceTaskCommit.getDefaultInstance());

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

		AllianceTeamEntity teamEntity = allianceEntity.getTeamEntity(player.getId());
		if (teamEntity == null) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NOT_IN_TEAM_VALUE);
			return true;
		}

		SociatyQuestCfg questCfg = HawkConfigManager.getInstance().getConfigByKey(SociatyQuestCfg.class, request.getQuestId());
		if (questCfg == null) {
			player.sendError(protocol.getType(), Status.error.CONFIG_ERROR_VALUE);
			return true;
		}

		// 锁住玩家
		HawkXID xid = HawkXID.valueOf(GsConst.ObjType.PLAYER, player.getId());
		if(xid != null){
			HawkObjBase<HawkXID, HawkAppObj> objBase = GsApp.getInstance().lockObject(xid);
			try {
				if (objBase != null && objBase.isObjValid()) {

					ConsumeItems consume = new ConsumeItems();
					if (questCfg.getGoalType() == GsConst.Alliance.COIN_QUEST) {
						if (teamEntity.getCoinQuest1() == request.getQuestId()) {
							if (teamEntity.getCoinQuest1PlayerId() != 0) {
								player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_QUEST_FINISH_VALUE);
								return true;
							}
						}
						else if (teamEntity.getCoinQuest2() == request.getQuestId()) {
							if (teamEntity.getCoinQuest2PlayerId() != 0) {
								player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_QUEST_FINISH_VALUE);
								return true;
							}
						}
						else{
							player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_QUEST_NOT_EXIST_VALUE);
							return true;
						}

						consume.addCoin(questCfg.getGoalCount());
					}
					else if (questCfg.getGoalType() == GsConst.Alliance.ITEM_QUEST){
						if (teamEntity.getItemQuest1() == request.getQuestId()) {
							if (teamEntity.getItemQuest1PlayerId() != 0) {
								player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_QUEST_FINISH_VALUE);
								return true;
							}
						}
						else if (teamEntity.getItemQuest2() == request.getQuestId()) {
							if (teamEntity.getItemQuest2PlayerId() != 0) {
								player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_QUEST_FINISH_VALUE);
								return true;
							}
						}
						else if (teamEntity.getItemQuest3() == request.getQuestId()) {
							if (teamEntity.getItemQuest3PlayerId() != 0) {
								player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_QUEST_FINISH_VALUE);
								return true;
							}
						}
						else{
							player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_QUEST_NOT_EXIST_VALUE);
							return true;
						}

						consume.addItem(questCfg.getGoalParam(), questCfg.getGoalCount());
					}
					else{
						player.sendError(protocol.getType(), Status.error.PARAMS_INVALID_VALUE);
						return true;
					}

					if (consume.checkConsume(player, protocol.getType()) == false) {
						return true;
					}

					if (teamEntity.getFinishCount(player.getId()) >= SysBasicCfg.getInstance().getAllianceMaxSmallTask()) {
						player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_MAX_SMALL_TASK_VALUE);
						return true;
					}

					teamEntity.setQuestFinish(request.getQuestId(), player.getId());
					teamEntity.notifyUpdate(true);

					consume.consumeTakeAffectAndPush(player, Action.GUILD_SUB_MISSION, protocol.getType());

					AwardItems reward = new AwardItems();
					reward.addItemInfos(questCfg.getReward().getRewardList());
					reward.rewardTakeAffectAndPush(player, Action.GUILD_SUB_MISSION, protocol.getType());

					int contribution = (int) reward.getRewardCount(Const.itemType.PLAYER_ATTR_VALUE, String.valueOf(Const.changeType.CHANGE_PLAYER_CONTRIBUTION));
					if (contribution != 0) {
						allianceEntity.addContribution(contribution);
						allianceEntity.notifyUpdate(true);
					}

					HSAllianceTeamQuestFinishNotify.Builder notify = HSAllianceTeamQuestFinishNotify.newBuilder();
					notify.setQuestId(request.getQuestId());
					notify.setTeamId(teamEntity.getId());
					notify.setPlayerId(player.getId());
					AllianceManager.getInstance().broadcastNotify(teamEntity, HawkProtocol.valueOf(HS.code.ALLIANCE_QUEST_FINISH_N_S_VALUE, notify), 0);

					HSAllianceTaskCommitRet.Builder response = HSAllianceTaskCommitRet.newBuilder();
					player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_COMMIT_TASK_S_VALUE, response));

					BILogger.getBIData(BIGuildFlowData.class).logTask(
							player, 
							Action.GUILD_SUB_MISSION, 
							allianceEntity, 
							teamEntity,
							request.getQuestId(),
							contribution, 
							0
							);

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
