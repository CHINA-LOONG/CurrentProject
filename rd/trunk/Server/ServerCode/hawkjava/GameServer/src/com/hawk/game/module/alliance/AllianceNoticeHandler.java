package com.hawk.game.module.alliance;

import org.hawk.app.HawkAppObj;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.net.protocol.HawkProtocolHandler;

import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Alliance.HSAllianceNotice;
import com.hawk.game.protocol.Alliance.HSAllianceNoticeRet;
import com.hawk.game.protocol.Alliance.HSAllianceNoticeRetOrBuilder;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.AllianceUtil;

/**
 * @author zhenghuangfei
 * 设置公告内容
 */
public class AllianceNoticeHandler implements HawkProtocolHandler {
	@Override
	public boolean onProtocol(HawkAppObj appObj, HawkProtocol protocol) {
		Player player = (Player) appObj;
		HSAllianceNotice request = protocol.parseProtocol(HSAllianceNotice.getDefaultInstance());
		if(request.getNotice().length() <= 0){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NOTICE_ERROR_VALUE);
			return true;
		}
		if(!AllianceUtil.checkNotice(request.getNotice())){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NOTICE_ERROR_VALUE);
			return true;
		}
		
		int allianceId = player.getPlayerData().getPlayerAllianceEntity().getAllianceId(); 
		AllianceEntity allianceEntity = AllianceManager.getInstance().getAlliance(allianceId);
		if(allianceEntity == null){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NOT_EXIST_VALUE);
			return true;
		}
		
		if(player.getPlayerData().getId() != allianceEntity.getPlayerId()){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NO_MAIN_VALUE);
			return true;
		}
		
		allianceEntity.setNotice(request.getNotice());
		allianceEntity.notifyUpdate(true);
		
		HSAllianceNoticeRet.Builder response = HSAllianceNoticeRet.newBuilder();
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_NOTICE_S_VALUE, response));
		return true;
	}
  
}
