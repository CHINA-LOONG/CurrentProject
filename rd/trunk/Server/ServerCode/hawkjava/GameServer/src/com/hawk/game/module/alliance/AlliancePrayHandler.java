package com.hawk.game.module.alliance;

import org.hawk.app.HawkAppObj;
import org.hawk.config.HawkConfigManager;
import org.hawk.msg.HawkMsg;
import org.hawk.msg.HawkMsgHandler;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.obj.HawkObjBase;
import org.hawk.xid.HawkXID;

import com.hawk.game.GsApp;
import com.hawk.game.config.SociatyPrayCfg;
import com.hawk.game.config.SysBasicCfg;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.Alliance.HSAlliancePray;
import com.hawk.game.protocol.Alliance.HSAlliancePrayRet;
import com.hawk.game.util.GsConst;

public class AlliancePrayHandler implements HawkMsgHandler{
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
		HSAlliancePray request = protocol.parseProtocol(HSAlliancePray.getDefaultInstance());
	
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
		
		SociatyPrayCfg prayCfg = HawkConfigManager.getInstance().getConfigByKey(SociatyPrayCfg.class, request.getPrayIndex());
		if (prayCfg == null) {
			player.sendError(protocol.getType(), Status.error.CONFIG_ERROR_VALUE);
			return true;
		}
		
		// 锁住玩家
		HawkXID xid = HawkXID.valueOf(GsConst.ObjType.PLAYER, player.getId());
		if(xid != null){
			HawkObjBase<HawkXID, HawkAppObj> objBase = GsApp.getInstance().lockObject(xid);
			try {
				if (objBase != null && objBase.isObjValid()) {
	
					if (player.getPlayerData().getStatisticsEntity().getAlliancePrayTimesDaily() >= SysBasicCfg.getInstance().getAlliancePrayCount()) {
						player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_PRAY_MAX_COUNT_VALUE);
						return true;
					}	
					
					ConsumeItems consume = new ConsumeItems();
					if (prayCfg.getCoinConsume() != 0) {
						consume.addCoin(prayCfg.getCoinConsume());
					}
					else {
						consume.addGold(prayCfg.getGoldConsume());
					}
					
					if (consume.checkConsume(player, protocol.getType()) == false) {
						return true;
					}
					
					AwardItems reward = new AwardItems();
					reward.addContribution(prayCfg.getMemberReward());
					
					allianceEntity.addContribution(prayCfg.getAllianceReward());
					allianceEntity.notifyUpdate(true);
					
					player.getPlayerData().getStatisticsEntity().increaseAlliancePrayTimesDaily();
					player.getPlayerData().getStatisticsEntity().notifyUpdate(true);
					
					consume.consumeTakeAffectAndPush(player, Action.ALLIANCE_PRAY, protocol.getType());
					reward.rewardTakeAffectAndPush(player, Action.ALLIANCE_PRAY, protocol.getType());
					
					HSAlliancePrayRet.Builder response = HSAlliancePrayRet.newBuilder();
					response.setSelfContribution(playerAllianceEntity.getContribution());
					response.setAllianceContribution(allianceEntity.getContribution());
					player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_PRAY_S_VALUE, response));
				}
			} finally {
				if (objBase != null) {
					objBase.unlockObj();
				}
			}
		}
		
		return true;
	}
}
