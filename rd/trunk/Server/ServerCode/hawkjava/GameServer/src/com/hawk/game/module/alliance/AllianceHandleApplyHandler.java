package com.hawk.game.module.alliance;

import java.util.LinkedList;
import java.util.List;

import org.hawk.app.HawkAppObj;
import org.hawk.msg.HawkMsg;
import org.hawk.msg.HawkMsgHandler;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkTime;

import com.hawk.game.GsApp;
import com.hawk.game.config.SociatyTechnologyCfg;
import com.hawk.game.entity.AllianceApplyEntity;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Alliance.HSAllianceHanleApply;
import com.hawk.game.protocol.Alliance.HSAllianceHanleApplyRet;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.GsConst;

public class AllianceHandleApplyHandler implements HawkMsgHandler{
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
		HSAllianceHanleApply request = protocol.parseProtocol(HSAllianceHanleApply.getDefaultInstance());
		if(player.getPlayerData().getPlayerAllianceEntity().getAllianceId() == 0){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NOT_JOIN_VALUE); 	
			return true;
		}	

		if (player.getPlayerData().getPlayerAllianceEntity().getPostion() == GsConst.Alliance.ALLIANCE_POS_COMMON) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_POSITION_ERROR_VALUE);
			return true;
		}
		
		AllianceEntity allianceEntity= AllianceManager.getInstance().getAlliance(player.getPlayerData().getPlayerAllianceEntity().getAllianceId());
		if (allianceEntity == null) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NOT_EXIST_VALUE);
			return true;
		}
		
		// 没有申请
		if (request.getIsAll() == false && allianceEntity.getApplyList().get(request.getPlayerId()) == null) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_APPLY_NOT_EXIST_VALUE);
			return true;
		}
		
		// 已经在队中
		if (request.getIsAll() == false && allianceEntity.getMember(request.getPlayerId()) != null) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_TARGET_ALREADY_JOIN_VALUE);
			AllianceManager.getInstance().clearAlliancePlayerApply(allianceEntity.getId(), request.getPlayerId());
			return true;
		}

		if (request.getAccept() == true) {
			// 已经满了
			if (allianceEntity.getMemberList().size() == SociatyTechnologyCfg.getMemberPop(allianceEntity.getMemLevel())) {
				player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_ALREADY_FULL_VALUE);
				return true;
			}
			
			List<Integer> acceptList = new LinkedList<>();
			if (request.getIsAll() == true) {
				for (AllianceApplyEntity apply: allianceEntity.getApplyList().values()) {
					if (AllianceManager.getInstance().getPlayerAllinceId(apply.getPlayerId()) == 0) {
						acceptList.add(apply.getPlayerId());
					}
				}
				
				if (acceptList.size() + allianceEntity.getMemberList().size() > SociatyTechnologyCfg.getMemberPop(allianceEntity.getMemLevel())) {
					player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_CAPACITY_NOT_ENOUGH_VALUE);
					return true;
				}
				
				AllianceManager.getInstance().clearAllianceApply(allianceEntity.getId());
			}
			else {		
				// 对方已经加入其它工会
				if (AllianceManager.getInstance().getPlayerAllinceId(request.getPlayerId()) != 0) {
					AllianceManager.getInstance().clearAlliancePlayerApply(allianceEntity.getId(), request.getPlayerId());
					player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_ALREADY_IN_VALUE);
					return true;
				}
				
				// 工会已满， 申请不处理
				if (allianceEntity.getMemberList().size() == SociatyTechnologyCfg.getMemberPop(allianceEntity.getMemLevel())) {
					player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_ALREADY_FULL_VALUE);
					return true;
				}
				
				acceptList.add(request.getPlayerId());
				AllianceManager.getInstance().clearAlliancePlayerApply(allianceEntity.getId(), request.getPlayerId());
			}

			for (int targetId : acceptList) {
				PlayerAllianceEntity targetPlayerAllianceEntity = null;
				Player targetPlayer = GsApp.getInstance().queryPlayer(targetId);
				if (targetPlayer != null) {
					targetPlayerAllianceEntity = targetPlayer.getPlayerData().getPlayerAllianceEntity();
				}
				else {
					targetPlayerAllianceEntity = AllianceManager.getInstance().getOfflineAllianceEntity(targetId);
				}
				
				if (targetPlayerAllianceEntity.getExitTime() > HawkTime.getSeconds()) {
					// 单人操作直接返回错误
					if (request.getIsAll() == false) {
						player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_FRIZEN_TIME_VALUE);
						return true;
					}
				}
				else{
					targetPlayerAllianceEntity.setPostion(GsConst.Alliance.ALLIANCE_POS_COMMON);
					targetPlayerAllianceEntity.setAllianceId(allianceEntity.getId());
					targetPlayerAllianceEntity.notifyUpdate(true);
					allianceEntity.addMember(request.getPlayerId(), targetPlayerAllianceEntity);
					AllianceManager.getInstance().addPlayerAndAllianceMap(request.getPlayerId(), allianceEntity.getId());	
					// 清理该玩家在其他公会的申请
					AllianceManager.getInstance().clearPlayerApply(targetId);
				}
			}
		} 
		else {
			if (request.getIsAll()) {
				AllianceManager.getInstance().clearAllianceApply(allianceEntity.getId());
			}
			else
			{
				AllianceManager.getInstance().clearAlliancePlayerApply(allianceEntity.getId(), request.getPlayerId());
			}
		}
		
		HSAllianceHanleApplyRet.Builder response = HSAllianceHanleApplyRet.newBuilder();
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_HANDLE_APPLY_S_VALUE, response));
		
/* 广播
		if (memNotify.getMemberDataList().isEmpty() == false) {
			// 通知新成员加入
			HawkProtocol memberData =  HawkProtocol.valueOf(HS.code.ALLIANCE_MEMBER_ADD_N_S_VALUE, memNotify);
			AllianceManager.getInstance().broadcastNotify(allianceEntity.getId(), memberData,  0);
		}
		
		HSAllianceRemoveApplyNotify.Builder notify = HSAllianceRemoveApplyNotify.newBuilder();
		if (request.getIsAll() == true) {
			notify.setClear(true);
		}
		else {
			notify.setClear(false);
			notify.setPlayerId(request.getPlayerId());
		}
		
		AllianceManager.getInstance().broadcastNotify(allianceEntity.getId(), HawkProtocol.valueOf(HS.code.ALLIANCE_REMOVE_APPLY_N_S_VALUE), request.getPlayerId());
*/
		
		return true;
	}                                 
}
