package com.hawk.game.module.alliance;

import org.hawk.app.HawkAppObj;
import org.hawk.msg.HawkMsg;
import org.hawk.msg.HawkMsgHandler;
import org.hawk.net.protocol.HawkProtocol;

import com.hawk.game.manager.AllianceManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.Alliance.HSAllianceCancleApply;
import com.hawk.game.protocol.Alliance.HSAllianceCancleApplyRet;

public class AllianceCancleApplyHandler implements HawkMsgHandler{
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
		HSAllianceCancleApply request = protocol.parseProtocol(HSAllianceCancleApply.getDefaultInstance());
		
		if(player.getPlayerData().getPlayerAllianceEntity().getAllianceId() != 0){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_ALREADY_IN_VALUE);
			return true;
		}	
		
		AllianceManager.getInstance().clearAlliancePlayerApply(request.getAllianceId(), player.getId());
		
		HSAllianceCancleApplyRet.Builder response = HSAllianceCancleApplyRet.newBuilder();
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_CANCLE_APPLY_S_VALUE, response));
		return true;
	}
}
