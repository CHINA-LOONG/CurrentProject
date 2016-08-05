package com.hawk.game.module.alliance;

import org.hawk.app.HawkAppObj;
import org.hawk.msg.HawkMsg;
import org.hawk.msg.HawkMsgHandler;
import org.hawk.net.protocol.HawkProtocol;

import com.hawk.game.config.SociatyTechnologyCfg;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.manager.AllianceManager;
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
		
		if(player.getPlayerData().getPlayerAllianceEntity().getAllianceId() != 0){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_ALREADY_IN_VALUE);
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
		
		int currenLevel = 0;
		switch (request.getType()) {
		case GsConst.Alliance.ALLIANCE_TEC_LEVEL:
			currenLevel = allianceEntity.getLevel();
			break;
		case GsConst.Alliance.ALLIANCE_TEC_MEMBER:
			currenLevel = allianceEntity.getMemLevel();
			break;
		case GsConst.Alliance.ALLIANCE_TEC_COIN:
			currenLevel = allianceEntity.getCoinLevel();
			break;
		case GsConst.Alliance.ALLIANCE_TEC_EXP:
			currenLevel = allianceEntity.getExpLevel();
			break;
		default:
			player.sendError(protocol.getType(), Status.error.PARAMS_INVALID_VALUE);
			return true;
		}
		
		if (SociatyTechnologyCfg.fullLevel(request.getType(), currenLevel)) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_TECH_FULL_VALUE);
			return true;
		}
		
		if (SociatyTechnologyCfg.levelLimit(request.getType(), currenLevel + 1) != -1 && 
			SociatyTechnologyCfg.levelLimit(request.getType(), currenLevel + 1) < allianceEntity.getLevel()) 
		{
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_LEVEL_LIMIT_VALUE);
			return true;
		}
		
		if (SociatyTechnologyCfg.levelUpContribution(request.getType(), currenLevel + 1) < allianceEntity.getContribution()) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_CONTRI_NOT_ENOUGH_VALUE);
			return true;
		}
		
		switch (request.getType()) {
		case GsConst.Alliance.ALLIANCE_TEC_LEVEL:
			allianceEntity.setLevel(currenLevel + 1);
			break;
		case GsConst.Alliance.ALLIANCE_TEC_MEMBER:
			allianceEntity.setMemLevel(currenLevel + 1);
			break;
		case GsConst.Alliance.ALLIANCE_TEC_COIN:
			allianceEntity.setExpLevel(currenLevel + 1);
			break;
		case GsConst.Alliance.ALLIANCE_TEC_EXP:
			allianceEntity.setCoinLevel(currenLevel + 1);
			break;
		default:
		}
		
		allianceEntity.setContribution(allianceEntity.getContribution() - SociatyTechnologyCfg.levelUpContribution(request.getType(), currenLevel + 1));
		allianceEntity.notifyUpdate(true);
		
		HSAllianceLevelUpRet.Builder resonse = HSAllianceLevelUpRet.newBuilder();
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_LEVEL_UP_S_VALUE, resonse));
		
		HSLevelChangeNotify.Builder notify = HSLevelChangeNotify.newBuilder();
		notify.setType(request.getType());
		notify.setLevel(currenLevel + 1);
		AllianceManager.getInstance().broadcastNotify(allianceEntity.getId(), HawkProtocol.valueOf(HS.code.ALLIANCE_LEVEL_CHANGE_N_S, notify), player.getId());
		return true;
	}
}
