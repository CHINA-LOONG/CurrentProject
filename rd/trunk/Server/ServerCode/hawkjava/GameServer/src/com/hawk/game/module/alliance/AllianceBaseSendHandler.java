package com.hawk.game.module.alliance;

import org.hawk.app.HawkAppObj;
import org.hawk.msg.HawkMsg;
import org.hawk.msg.HawkMsgHandler;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.obj.HawkObjBase;
import org.hawk.os.HawkTime;
import org.hawk.xid.HawkXID;

import com.hawk.game.GsApp;
import com.hawk.game.entity.AllianceBaseEntity;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.Alliance.HSAllianceBaseSendMonster;
import com.hawk.game.protocol.Alliance.HSAllianceBaseSendMonsterRet;
import com.hawk.game.util.BuilderUtil;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.MonsterUtil;

public class AllianceBaseSendHandler implements HawkMsgHandler{
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
		HSAllianceBaseSendMonster request = protocol.parseProtocol(HSAllianceBaseSendMonster.getDefaultInstance());
		
		if (request.getPosition() > 2 || request.getPosition() < 0) {
			player.sendError(protocol.getType(), Status.error.PARAMS_INVALID_VALUE);
			return true;
		}
		
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
		
		// 怪物已经派出
		if (allianceEntity.getAllianceBaseEntityMap().containsKey(request.getMonsterId())) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_BASE_MONSTER_HAVE_SEND_VALUE);
			return true;
		}
		
		// 位置已经占用
		if (playerAllianceEntity.isBasePositionHasMonster(request.getPosition())) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_BASE_HAVE_MONSTER_VALUE);
			return true;
		}
		
		if ((request.getPosition() == 1 && playerAllianceEntity.getTotalContribution() <= GsConst.Alliance.SECOND_BASE_POSITION_CONTRIBUTION) || 
			(request.getPosition() == 2 && playerAllianceEntity.getTotalContribution() <= GsConst.Alliance.THIRD_BASE_POSITION_CONTRIBUTION)){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_BASE_POSITION_LOCK_VALUE);
			return true;
		}
		
		// 锁住玩家
		HawkXID xid = HawkXID.valueOf(GsConst.ObjType.PLAYER, player.getId());
		if(xid != null){
			HawkObjBase<HawkXID, HawkAppObj> objBase = GsApp.getInstance().lockObject(xid);
			try {
				if (objBase != null && objBase.isObjValid()) {		

					MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(request.getMonsterId());
					if (monsterEntity == null) {
						player.sendError(protocol.getType(), Status.monsterError.MONSTER_NOT_EXIST_VALUE);
						return true;
					}
					
					AllianceBaseEntity allianceBaseEntity = new AllianceBaseEntity();
					allianceBaseEntity.setSendTime(HawkTime.getSeconds());
					allianceBaseEntity.setAllianceId(allianceEntity.getId());
					allianceBaseEntity.setPlayerId(player.getId());
					allianceBaseEntity.setMonsterInfo(BuilderUtil.genCompleteMonsterBuilder(player, monsterEntity));
					allianceBaseEntity.setBp((int)MonsterUtil.calculateBP(allianceBaseEntity.getMonsterBuilder()));
					allianceBaseEntity.setPosition(request.getPosition());
					if (allianceBaseEntity.notifyCreate() == false) {
						player.sendError(protocol.getType(), Status.error.SERVER_ERROR_VALUE);
						return true;
					}
					
					// 添加基地驻兵
					allianceEntity.addAllianceBase(allianceBaseEntity, request.getPosition());
					
					// 回复信息
					HSAllianceBaseSendMonsterRet.Builder response = HSAllianceBaseSendMonsterRet.newBuilder();
					response.setSendTime(allianceBaseEntity.getSendTime());
					player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_BASE_SEND_S_VALUE, response));
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
