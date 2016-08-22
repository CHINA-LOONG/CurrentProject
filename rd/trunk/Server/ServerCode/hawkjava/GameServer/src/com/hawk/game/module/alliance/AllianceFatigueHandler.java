package com.hawk.game.module.alliance;

import org.hawk.app.HawkAppObj;
import org.hawk.config.HawkConfigManager;
import org.hawk.msg.HawkMsg;
import org.hawk.msg.HawkMsgHandler;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkTime;

import com.hawk.game.config.MailSysCfg;
import com.hawk.game.config.SysBasicCfg;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Alliance.HSAllianceFatigueGive;
import com.hawk.game.protocol.Alliance.HSAllianceFatigueGiveRet;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.MailUtil;

public class AllianceFatigueHandler  implements HawkMsgHandler{
	/**
	 * 消息处理器   体力值的处理方式可能导致工会管理器和玩家同时清理赠送体力值的情况，但是影响不大
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
		HSAllianceFatigueGive request = protocol.parseProtocol(HSAllianceFatigueGive.getDefaultInstance());
		
		if (player.getAllianceId() == 0) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NOT_JOIN_VALUE);
			return true;
		}
		
		AllianceEntity allianceEntity = AllianceManager.getInstance().getAlliance(player.getAllianceId());
		if (allianceEntity == null) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NOT_JOIN_VALUE);
			return true;
		}
		
		PlayerAllianceEntity selfPlayerAllianceEntity = allianceEntity.getMember(player.getId());
		if (selfPlayerAllianceEntity == null) {
			// 不应该出现的情况
			player.sendError(protocol.getType(), Status.error.SERVER_ERROR_VALUE);
			return true;
		}
		
		PlayerAllianceEntity targetPlayerAllianceEntity = allianceEntity.getMember(request.getTargetId());
		if (targetPlayerAllianceEntity == null ) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_TARGET_NOT_JOIN_VALUE);
			return true;
		}
		
		// 刷新体力赠送
		if (selfPlayerAllianceEntity.getRefreshTime() < HawkTime.getSeconds()) {
			selfPlayerAllianceEntity.clearFatigueCount();
			selfPlayerAllianceEntity.clearFatigueSet();
			selfPlayerAllianceEntity.setRefreshTime((int) (HawkTime.getNextAM0Date() / 1000));
		}
		
		if (selfPlayerAllianceEntity.getFatigueSet().contains(request.getTargetId())) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_FAGIGUE_GIVE_ALREADY_VALUE);
			return true;
		}
		
		selfPlayerAllianceEntity.addFatigueSet(request.getTargetId());
		selfPlayerAllianceEntity.notifyUpdate(true);
		
		if (targetPlayerAllianceEntity.getRefreshTime() < HawkTime.getSeconds()) {
			targetPlayerAllianceEntity.clearFatigueCount();
			targetPlayerAllianceEntity.clearFatigueSet();
			targetPlayerAllianceEntity.setRefreshTime((int) (HawkTime.getNextAM0Date() / 1000));
			targetPlayerAllianceEntity.notifyUpdate(true);
		}
		
		if (targetPlayerAllianceEntity.getFatigueCount() < SysBasicCfg.getInstance().getAllianceMaxFatigueReceived()) {
			targetPlayerAllianceEntity.addFatigueCount();
			targetPlayerAllianceEntity.notifyUpdate(true);
			
			MailSysCfg mailCfg = HawkConfigManager.getInstance().getConfigByKey(MailSysCfg.class, GsConst.SysMail.ALLIANCE_FATIGUE);
			if (mailCfg != null) {
				MailUtil.SendSysMail(mailCfg, request.getTargetId(), selfPlayerAllianceEntity.getName(), 888888888);
			}
		}
		
		HSAllianceFatigueGiveRet.Builder repsonse = HSAllianceFatigueGiveRet.newBuilder();
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_FATIGUE_GIVE_S_VALUE, repsonse));
		return true;
	}		
}
