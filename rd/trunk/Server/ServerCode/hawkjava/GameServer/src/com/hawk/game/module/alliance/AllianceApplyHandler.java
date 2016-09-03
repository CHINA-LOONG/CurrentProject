package com.hawk.game.module.alliance;

import org.hawk.app.HawkAppObj;
import org.hawk.config.HawkConfigManager;
import org.hawk.msg.HawkMsg;
import org.hawk.msg.HawkMsgHandler;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkTime;

import com.hawk.game.BILog.BIGuildMemberFlowData;
import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.config.ImSysCfg;
import com.hawk.game.config.SociatyTechnologyCfg;
import com.hawk.game.config.SysBasicCfg;
import com.hawk.game.entity.AllianceApplyEntity;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.log.BILogger;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.manager.ImManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Alliance.HSAllianceJoinNotify;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.Alliance.HSAllianceApply;
import com.hawk.game.protocol.Alliance.HSAllianceApplyRet;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.QuestUtil;

public class AllianceApplyHandler implements HawkMsgHandler{
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
		HSAllianceApply request = protocol.parseProtocol(HSAllianceApply.getDefaultInstance());

		if(player.getPlayerData().getPlayerAllianceEntity().getAllianceId() != 0){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_ALREADY_IN_VALUE);
			return true;
		}

		AllianceEntity allianceEntity= AllianceManager.getInstance().getAlliance(request.getAllianceId());
		if (allianceEntity == null) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_NOT_EXIST_VALUE);
			return true;
		}

		if (allianceEntity.getApplyList().get(player.getId()) != null) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_ALREADY_APPLY_VALUE);
			return true;
		}

		if (player.getPlayerData().getPlayerAllianceEntity().getExitTime() > HawkTime.getSeconds()) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_FRIZEN_TIME_VALUE);
			return true;
		}

		if (player.getLevel() < allianceEntity.getMinLevel()){
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_LEVEL_NOT_ENOUGH_VALUE);
			return true;
		}

		if (allianceEntity.getMemberList().size() ==  SociatyTechnologyCfg.getMemberPop(allianceEntity.getMemLevel())) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_ALREADY_FULL_VALUE);
			return true;
		}

		//自动加入
		if (allianceEntity.isAutoAccept() == true) {
			player.getPlayerData().getPlayerAllianceEntity().setPostion(GsConst.Alliance.ALLIANCE_POS_COMMON);
			player.getPlayerData().getPlayerAllianceEntity().setAllianceId(allianceEntity.getId());
			player.getPlayerData().getPlayerAllianceEntity().notifyUpdate(true);
			if (player.getPlayerData().getPlayerAllianceEntity().getPreAllianceId() != allianceEntity.getId()) {
				player.getPlayerData().getPlayerAllianceEntity().setTotalContribution(0);
			}
			allianceEntity.addMember(player.getId(), player.getPlayerData().getPlayerAllianceEntity());
			AllianceManager.getInstance().addPlayerAndAllianceMap(player.getId(), allianceEntity.getId());
			AllianceManager.getInstance().clearPlayerApply(player.getId());

			// 加入公会频道
			ImManager.getInstance().joinGuild(allianceEntity.getId(), player);
			ImSysCfg imCfg = HawkConfigManager.getInstance().getConfigByKey(ImSysCfg.class, GsConst.SysIm.ALLIANCE_JOIN);
			if (imCfg != null) {
				ImManager.getInstance().postSys(imCfg, allianceEntity.getId(), player.getName());
			}

			// 更新加入公会任务
			QuestUtil.postQuestDataUpdateMsg(player.getXid());

			HSAllianceApplyRet.Builder response = HSAllianceApplyRet.newBuilder();
			response.setAllianceId(allianceEntity.getId());
			player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_APPLY_S_VALUE, response));

			//HawkProtocol memberData =  HawkProtocol.valueOf(HS.code.ALLIANCE_MEMBER_ADD_N_S_VALUE, AllianceUtil.getMemberInfo(player.getPlayerData().getPlayerAllianceEntity()));
			//AllianceManager.getInstance().broadcastNotify(allianceEntity.getId(), memberData,  0);
			HawkProtocol notify = HawkProtocol.valueOf(HS.code.ALLIANCE_JOIN_N_S_VALUE, HSAllianceJoinNotify.newBuilder().setAllianceId(allianceEntity.getId()));
			player.sendProtocol(notify);
			
			BILogger.getBIData(BIGuildMemberFlowData.class).log(
					player, 
					Action.GUILD_JOIN,
					allianceEntity, 
					player.getPlayerData().getPlayerAllianceEntity(), 
					0, 
					0, 
					player.getPlayerData().getPlayerAllianceEntity().getPostion()
					);
		} 
		else 
		{
			if (AllianceManager.getInstance().getPlayerApplyCount(player.getId()) >= SysBasicCfg.getInstance().getAllianceMaxApply()) {
				player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_MAX_APPLY_VALUE);
				return true;
			}

			AllianceApplyEntity applyEntity = new AllianceApplyEntity();
			applyEntity.setPlayerId(player.getId());
			applyEntity.setName(player.getName());
			applyEntity.setAllianceId(request.getAllianceId());
			applyEntity.setLevel(player.getLevel());

			if (applyEntity.notifyCreate() == false) {
				player.sendError(protocol.getType(), Status.error.DATA_BASE_ERROR_VALUE);
				return true;
			}

			allianceEntity.addApply(applyEntity);
			AllianceManager.getInstance().addPlayerApply(player.getId(), allianceEntity.getId());

			HSAllianceApplyRet.Builder response = HSAllianceApplyRet.newBuilder();
			response.setAllianceId(0);
			player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_APPLY_S_VALUE, response));

		//	HSAllianceApplyNotify.Builder builder =  HSAllianceApplyNotify.newBuilder();
		//	builder.setApply(AllianceUtil.getApplyNotify(applyEntity));
		//	AllianceManager.getInstance().broadcastNotify(allianceEntity.getId(), HawkProtocol.valueOf(HS.code.ALLIANCE_APPLY_N_S_VALUE, builder), 0);
		}

		return true;
	}
}
