package com.hawk.game.module.alliance;

import org.hawk.app.HawkAppObj;
import org.hawk.config.HawkConfigManager;
import org.hawk.msg.HawkMsg;
import org.hawk.msg.HawkMsgHandler;
import org.hawk.net.protocol.HawkProtocol;

import com.hawk.game.config.ImSysCfg;
import com.hawk.game.config.SociatyTechnologyCfg;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.manager.ImManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Alliance.HSAllianceLevelUp;
import com.hawk.game.protocol.Alliance.HSAllianceLevelUpRet;
import com.hawk.game.protocol.Alliance.HSLevelChangeNotify;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.GsConst;

public class AllianceLevelUpHandler implements HawkMsgHandler{
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
		HSAllianceLevelUp request = protocol.parseProtocol(HSAllianceLevelUp.getDefaultInstance());
		
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
		
		int currentLevel = 0;
		switch (request.getType()) {
		case GsConst.Alliance.ALLIANCE_TEC_LEVEL:
			currentLevel = allianceEntity.getLevel();
			break;
		case GsConst.Alliance.ALLIANCE_TEC_MEMBER:
			currentLevel = allianceEntity.getMemLevel();
			break;
		case GsConst.Alliance.ALLIANCE_TEC_COIN:
			currentLevel = allianceEntity.getCoinLevel();
			break;
		case GsConst.Alliance.ALLIANCE_TEC_EXP:
			currentLevel = allianceEntity.getExpLevel();
			break;
		default:
			player.sendError(protocol.getType(), Status.error.PARAMS_INVALID_VALUE);
			return true;
		}
		
		if (SociatyTechnologyCfg.fullLevel(request.getType(), currentLevel)) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_TECH_FULL_VALUE);
			return true;
		}
		
		if (SociatyTechnologyCfg.levelLimit(request.getType(), currentLevel + 1) != -1 && 
			SociatyTechnologyCfg.levelLimit(request.getType(), currentLevel + 1) > allianceEntity.getLevel()) 
		{
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_LEVEL_LIMIT_VALUE);
			return true;
		}
		
		if (SociatyTechnologyCfg.levelUpContribution(request.getType(), currentLevel + 1) > allianceEntity.getContribution()) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_CONTRI_NOT_ENOUGH_VALUE);
			return true;
		}
		
		switch (request.getType()) {
		case GsConst.Alliance.ALLIANCE_TEC_LEVEL:
			allianceEntity.setLevel(currentLevel + 1);
			break;
		case GsConst.Alliance.ALLIANCE_TEC_MEMBER:
			allianceEntity.setMemLevel(currentLevel + 1);
			break;
		case GsConst.Alliance.ALLIANCE_TEC_COIN:
			allianceEntity.setExpLevel(currentLevel + 1);
			break;
		case GsConst.Alliance.ALLIANCE_TEC_EXP:
			allianceEntity.setCoinLevel(currentLevel + 1);
			break;
		default:
		}
		
		allianceEntity.setContribution(allianceEntity.getContribution() - SociatyTechnologyCfg.levelUpContribution(request.getType(), currentLevel + 1));
		allianceEntity.notifyUpdate(true);
		
		HSLevelChangeNotify.Builder notify = HSLevelChangeNotify.newBuilder();
		notify.setType(request.getType());
		notify.setLevel(currentLevel + 1);
		AllianceManager.getInstance().broadcastNotify(allianceEntity.getId(), HawkProtocol.valueOf(HS.code.ALLIANCE_LEVEL_CHANGE_N_S, notify), 0);

		ImSysCfg imCfg = HawkConfigManager.getInstance().getConfigByKey(ImSysCfg.class, GsConst.SysIm.ALLIANCE_LEVEL_UP);
		if (imCfg != null) {
			ImManager.getInstance().postSys(imCfg, allianceEntity.getId(), "XXXXXXX", currentLevel + 1);
		}

		HSAllianceLevelUpRet.Builder resonse = HSAllianceLevelUpRet.newBuilder();
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_LEVEL_UP_S_VALUE, resonse));
		return true;
	}
}
