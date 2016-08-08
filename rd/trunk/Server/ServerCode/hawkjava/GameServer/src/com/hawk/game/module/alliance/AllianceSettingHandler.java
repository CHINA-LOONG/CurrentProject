package com.hawk.game.module.alliance;

import org.hawk.app.HawkAppObj;
import org.hawk.config.HawkConfigManager;
import org.hawk.msg.HawkMsg;
import org.hawk.msg.HawkMsgHandler;
import org.hawk.net.protocol.HawkProtocol;

import com.hawk.game.config.PlayerAttrCfg;
import com.hawk.game.config.SysBasicCfg;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.Alliance.HSAllianceSettion;
import com.hawk.game.protocol.Alliance.HSAllianceSettionRet;
import com.hawk.game.util.GsConst;

public class AllianceSettingHandler  implements HawkMsgHandler{
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
		HSAllianceSettion request = protocol.parseProtocol(HSAllianceSettion.getDefaultInstance());
		
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
		
		if (playerAllianceEntity.getPostion() == GsConst.Alliance.ALLIANCE_POS_COMMON) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_POSITION_ERROR_VALUE);
			return true;
		}
	
		if (request.getMinLevel() > HawkConfigManager.getInstance().getConfigMap(PlayerAttrCfg.class).size() ||
			request.getMinLevel() < SysBasicCfg.getInstance().getAllianceMinLevel()) {
			player.sendError(protocol.getType(), Status.error.CONFIG_ERROR_VALUE);
			return true;
		}
		
		allianceEntity.setMinLevel(request.getMinLevel());
		allianceEntity.setAutoAccept(request.getAutoJoin());
		allianceEntity.notifyUpdate(true);
		
		HSAllianceSettionRet.Builder response = HSAllianceSettionRet.newBuilder();
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_SETTING_S_VALUE, response));
		return true;
	}
}
