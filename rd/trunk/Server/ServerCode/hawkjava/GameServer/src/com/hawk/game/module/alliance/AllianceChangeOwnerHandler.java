package com.hawk.game.module.alliance;

import org.hawk.app.HawkAppObj;
import org.hawk.msg.HawkMsg;
import org.hawk.msg.HawkMsgHandler;
import org.hawk.net.protocol.HawkProtocol;

import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Alliance.HSAllianceChangeOwnerRet;
import com.hawk.game.protocol.Alliance.HSChangeOwnerNotify;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.Alliance.HSAllianceChangeOwner;
import com.hawk.game.util.GsConst;

public class AllianceChangeOwnerHandler implements HawkMsgHandler{
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
		HSAllianceChangeOwner request = protocol.parseProtocol(HSAllianceChangeOwner.getDefaultInstance());
		
		
		if (player.getAllianceId() == 0) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NOT_JOIN_VALUE);
			return true;
		}
		
		AllianceEntity allianceEntity = AllianceManager.getInstance().getAlliance(player.getAllianceId());
		if (allianceEntity == null) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NOT_EXIST_VALUE);
			return true;
		}
		
		PlayerAllianceEntity playerAllianceEntity = allianceEntity.getMember(player.getId());
		if (playerAllianceEntity == null) {
			player.sendError(protocol.getType(), Status.error.SERVER_ERROR_VALUE);
			return true;
		}
		
		if (playerAllianceEntity.getPostion() != GsConst.Alliance.ALLIANCE_POS_MAIN) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_POSITION_ERROR_VALUE);
			return true;
		}
		
		PlayerAllianceEntity targetPlayerAllianceEntity = allianceEntity.getMember(request.getTargetId());
		if (targetPlayerAllianceEntity == null) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_TARGET_NOT_JOIN_VALUE);
			return true;
		}
		
		playerAllianceEntity.setPostion(GsConst.Alliance.ALLIANCE_POS_COMMON);
		playerAllianceEntity.notifyUpdate(true);
		
		targetPlayerAllianceEntity.setPostion(GsConst.Alliance.ALLIANCE_POS_MAIN);
		targetPlayerAllianceEntity.notifyUpdate(true);

		allianceEntity.setPlayerId(targetPlayerAllianceEntity.getPlayerId());
		allianceEntity.setPlayerName(targetPlayerAllianceEntity.getName());
		allianceEntity.notifyUpdate(true);
		
		HSAllianceChangeOwnerRet.Builder response = HSAllianceChangeOwnerRet.newBuilder();
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_CHANGE_OWNER_S_VALUE, response));
		
		HSChangeOwnerNotify.Builder notify = HSChangeOwnerNotify.newBuilder();
		notify.setOwnerId(request.getTargetId());
		AllianceManager.getInstance().broadcastNotify(allianceEntity.getId(), HawkProtocol.valueOf(HS.code.ALLIANCE_CHANGE_OWNER_N_S_VALUE), player.getId());
		
		return true;
	}
}
