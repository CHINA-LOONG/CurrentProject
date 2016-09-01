package com.hawk.game.module.alliance;

import org.hawk.app.HawkAppObj;
import org.hawk.msg.HawkMsg;
import org.hawk.msg.HawkMsgHandler;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.obj.HawkObjBase;
import org.hawk.os.HawkTime;
import org.hawk.xid.HawkXID;

import com.hawk.game.GsApp;
import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.entity.AllianceBaseEntity;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Alliance.HSAllianceBaseRecallMonster;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.Alliance.HSAllianceBaseRecallMonsterRet;
import com.hawk.game.util.AllianceUtil;
import com.hawk.game.util.GsConst;

public class AllianceBaseRecallHandler implements HawkMsgHandler{
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
		HSAllianceBaseRecallMonster request = protocol.parseProtocol(HSAllianceBaseRecallMonster.getDefaultInstance());
		
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
		
		// 基地对应位置没有派兵
		if (!playerAllianceEntity.isBasePositionHasMonster(request.getPosition())) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_BASE_HAVE_NO_MONSTER_VALUE);
			return true;
		}
		
		// 基地宠物最小时间是一个小时
		AllianceBaseEntity baseEntity = allianceEntity.getAllianceBaseEntity(player.getId(), request.getPosition());
		if (baseEntity.getSendTime() + GsConst.Alliance.BASE_MIN_TIME > HawkTime.getSeconds()) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_BASE_TIME_LIMIT_VALUE);
			return true;
		}
		
		// 锁住玩家
		HawkXID xid = HawkXID.valueOf(GsConst.ObjType.PLAYER, player.getId());
		if(xid != null){
			HawkObjBase<HawkXID, HawkAppObj> objBase = GsApp.getInstance().lockObject(xid);
			try {
				if (objBase != null && objBase.isObjValid()) {		
					int bp = baseEntity.getBp();
					
					AwardItems award = new AwardItems();
					int rewardCoin = AllianceUtil.getAllianceBaseConfig(bp).getCoinDefend() * (HawkTime.getSeconds()- baseEntity.getSendTime());
					int hireCoin = playerAllianceEntity.getBaseMonsterInfo(request.getPosition()).getReward();
					award.addCoin(rewardCoin + hireCoin);
					award.rewardTakeAffectAndPush(player, Action.GUILD_BASE_REWARD, protocol.getType());
					
					// 移除基地驻兵
					allianceEntity.removeAllianceBase(player.getId(), request.getPosition());
					baseEntity.delete();
					
					// 回复信息
					HSAllianceBaseRecallMonsterRet.Builder response = HSAllianceBaseRecallMonsterRet.newBuilder();
					response.setCoinDefend(rewardCoin);
					response.setCoinHire(hireCoin);
					player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_BASE_RECALL_S_VALUE, response));
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
