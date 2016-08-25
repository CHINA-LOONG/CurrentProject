package com.hawk.game.module.alliance;

import org.hawk.app.HawkAppObj;
import org.hawk.msg.HawkMsg;
import org.hawk.msg.HawkMsgHandler;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.obj.HawkObjBase;
import org.hawk.os.HawkTime;
import org.hawk.xid.HawkXID;

import com.hawk.game.GsApp;
import com.hawk.game.config.SysBasicCfg;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.manager.ImManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Alliance.HSAllianceCreate;
import com.hawk.game.protocol.Alliance.HSAllianceCreateRet;
import com.hawk.game.protocol.Const.changeType;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.AllianceUtil;
import com.hawk.game.util.GsConst;

/**
 * @author zs
 *创建公会
 */
public class AllianceCreateHandler implements HawkMsgHandler {

	@Override
	public boolean onMessage(HawkAppObj appObj, HawkMsg msg) {
		Player player = (Player) msg.getParam(0);
		HawkProtocol protocol = (HawkProtocol)msg.getParam(1);
		
		if(player.getLevel() < SysBasicCfg.getInstance().getAllianceMinLevel()){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_LEVEL_NOT_ENOUGH_VALUE);
			return true;
		}
		
		if(player.getPlayerData().getPlayerAllianceEntity().getAllianceId() != 0){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_ALREADY_IN_VALUE);
			return true;
		}	
		
		if (player.getPlayerData().getPlayerAllianceEntity().getExitTime() > HawkTime.getSeconds()) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_FRIZEN_TIME_VALUE);
			return true;
		}
		
		HSAllianceCreate request = protocol.parseProtocol(HSAllianceCreate.getDefaultInstance());
		String name = request.getName();
		if(name == null){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NAME_ERROR_VALUE);
			return true;
		}
		
		name = name.trim();
		if(name.length() <= 0){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NAME_ERROR_VALUE);
			return true;
		}
		
		if(!AllianceUtil.checkName(name)){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NAME_ERROR_VALUE);
			return true;
		}

		String notice = request.getNotice();
		notice = notice.trim();
		if(notice.length() <= 0){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NOTICE_ERROR_VALUE);
			return true;
		}
		
		if(!AllianceUtil.checkNotice(notice)){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NOTICE_ERROR_VALUE);
			return true;
		}
		
		if(AllianceManager.getInstance().getExistName().contains(name)){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NAME_EXIST_VALUE);
			return true;
		}
		
		// 锁住玩家
		HawkXID xid = HawkXID.valueOf(GsConst.ObjType.PLAYER, player.getId());
		if(xid != null){
			HawkObjBase<HawkXID, HawkAppObj> objBase = GsApp.getInstance().lockObject(xid);
			try {
				if (objBase != null && objBase.isObjValid()) {				
					AllianceEntity allianceEntity = new AllianceEntity();
					allianceEntity.setPlayerId(player.getId());
					allianceEntity.setPlayerName(player.getName());
					allianceEntity.setContribution(0);
					allianceEntity.setLevel(1);
					allianceEntity.setMemLevel(1);
					allianceEntity.setExpLevel(0);
					allianceEntity.setCoinLevel(0);
					allianceEntity.setName(name);
					allianceEntity.setRefreshTime((int) (HawkTime.getNextAM0Date() / 1000));
					allianceEntity.setNotice(request.getNotice());
					allianceEntity.addMember(player.getId(), player.getPlayerData().getPlayerAllianceEntity());;
					allianceEntity.setAutoAccept(false);
					allianceEntity.setMinLevel(SysBasicCfg.getInstance().getAllianceMinLevel());
					if (allianceEntity.notifyCreate() == false) {
						player.sendError(protocol.getType(), Status.error.DATA_BASE_ERROR_VALUE);
						return true;
					}
					
					//标记为会长
					player.getPlayerData().getPlayerAllianceEntity().setPostion(GsConst.Alliance.ALLIANCE_POS_MAIN);
					player.getPlayerData().getPlayerAllianceEntity().setAllianceId(allianceEntity.getId());
					player.getPlayerData().getPlayerAllianceEntity().setTotalContribution(0);
					player.getPlayerData().getPlayerAllianceEntity().notifyUpdate(true);

					allianceEntity.addMember(player.getId(), player.getPlayerData().getPlayerAllianceEntity());
					AllianceManager.getInstance().addPlayerAndAllianceMap(player.getId(), allianceEntity.getId());
					AllianceManager.getInstance().clearPlayerApply(player.getId());
					
					// 从db创建
					AllianceManager.getInstance().addAlliance(allianceEntity);
					AllianceManager.getInstance().addAllianceForSort(allianceEntity);
					AllianceManager.getInstance().getExistName().add(name);
					
					ConsumeItems consumes =  ConsumeItems.valueOf(changeType.CHANGE_COIN, SysBasicCfg.getInstance().getAllianceCreateCoin());
					if (consumes.checkConsume(player, protocol.getType()) == false) {
						return true;
					}
					
					consumes.consumeTakeAffectAndPush(player, Action.ALLIANCE_CREATE_CONSUME, protocol.getType());

					// 加入公会频道
					ImManager.getInstance().joinGuild(allianceEntity.getId(), player);

					HSAllianceCreateRet.Builder response = HSAllianceCreateRet.newBuilder();
					response.setAllianeId(allianceEntity.getId());
					player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_CREATE_S_VALUE, response));
					
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
