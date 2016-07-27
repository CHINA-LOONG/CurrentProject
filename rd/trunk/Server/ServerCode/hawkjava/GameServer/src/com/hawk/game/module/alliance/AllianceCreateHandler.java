package com.hawk.game.module.alliance;

import org.hawk.app.HawkAppObj;
import org.hawk.config.HawkConfigManager;
import org.hawk.db.HawkDBManager;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.net.protocol.HawkProtocolHandler;

import com.hawk.game.config.AllianceCfg;
import com.hawk.game.config.SysBasicCfg;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Alliance.HSAllianceCreateC;
import com.hawk.game.protocol.Alliance.HSAllianceCreateS;
import com.hawk.game.protocol.Const.changeType;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.AllianceUtil;
import com.hawk.game.util.GsConst;

/**
 * @author zs
 *创建公会
 */
public class AllianceCreateHandler implements HawkProtocolHandler {

	@Override
	public boolean onProtocol(HawkAppObj appObj, HawkProtocol protocol) {
		Player player = (Player) appObj;
		if(player.getLevel() < SysBasicCfg.getInstance().getAllianceMinLevel()){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_LEVEL_NOT_ENOUGH_VALUE);
			return true;
		}
		if(player.getGold() < SysBasicCfg.getInstance().getAllianceCreateCoin()){
			player.sendError(protocol.getType(), Status.PlayerError.GOLD_NOT_ENOUGH_VALUE);
			return true;
		}
		if(player.getPlayerData().getPlayerAllianceEntity().getAllianceId()!=0){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_ALREADY_IN_VALUE);
			return true;
		}	
		
		HSAllianceCreateC par = protocol.parseProtocol(HSAllianceCreateC.getDefaultInstance());
		String name = par.getName();
		if(name == null){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NAME_ERROR_VALUE);
			return true;
		}
		name = name.trim();
		if(name.length()<=0){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NAME_ERROR_VALUE);
			return true;
		}
		
		if(!AllianceUtil.checkName(name)){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NAME_ERROR_VALUE);
			return true;
		}
		
		if(AllianceManager.getInstance().getExistName().contains(name)){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NAME_EXIST_VALUE);
			return true;
		}
			
		AllianceCfg allianceCfg = HawkConfigManager.getInstance().getConfigByKey(AllianceCfg.class, 1);
		if(allianceCfg == null){
			player.sendError(protocol.getType(), Status.error.CONFIG_ERROR_VALUE);
			return true;
		}
		
		AllianceEntity allianceEntity = new AllianceEntity();
		allianceEntity.setPlayerId(player.getId());
		allianceEntity.setPlayerName(player.getName());
		allianceEntity.setExp(0);
		allianceEntity.setLevel(allianceCfg.getLevel());
		allianceEntity.setName(name);
		allianceEntity.setCreateAllianceTime(System.currentTimeMillis());
		allianceEntity.getMemberList().add(player.getId());
		allianceEntity.notifyCreate();
		//标记为会长
		player.getPlayerData().getPlayerAllianceEntity().setPostion(GsConst.Alliance.ALLIANCE_POS_MAIN);
		player.getPlayerData().getPlayerAllianceEntity().setAllianceId(allianceEntity.getId());
		player.getPlayerData().getPlayerAllianceEntity().notifyUpdate(true);
		// 从db创建
		AllianceManager.getInstance().addAlliance(allianceEntity);
		
		AllianceManager.getInstance().getExistName().add(name);
		
		ConsumeItems.valueOf(changeType.CHANGE_COIN, SysBasicCfg.getInstance().getAllianceCreateCoin()).consumeTakeAffect(player, Action.ALLIANCE_CREATE_CONSUME);
		
		PlayerAllianceEntity myPlayerAllianceEntity = player.getPlayerData().getPlayerAllianceEntity();

		HSAllianceCreateS.Builder response = HSAllianceCreateS.newBuilder();
		response.setAllianceInfo(AllianceManager.getInstance().getAllianceInfo(allianceEntity, player.getId(), player.getGold()));
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_CREATE_S_VALUE, response));
		return true;
	}
}
