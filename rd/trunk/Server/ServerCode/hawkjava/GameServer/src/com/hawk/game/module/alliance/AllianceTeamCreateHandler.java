package com.hawk.game.module.alliance;

import java.util.LinkedList;
import java.util.List;

import org.hawk.app.HawkAppObj;
import org.hawk.config.HawkConfigManager;
import org.hawk.msg.HawkMsg;
import org.hawk.msg.HawkMsgHandler;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.obj.HawkObjBase;
import org.hawk.os.HawkException;
import org.hawk.os.HawkRand;
import org.hawk.xid.HawkXID;

import com.hawk.game.GsApp;
import com.hawk.game.config.SociatyTaskCfg;
import com.hawk.game.config.SysBasicCfg;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.AllianceTeamEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Alliance.HSAllianceCreateTeamRet;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.Alliance.HSAllianceCreateTeam;
import com.hawk.game.util.GsConst;

public class AllianceTeamCreateHandler implements HawkMsgHandler{

	/**
	 * 消息处理器
	 * 
	 * @param appObj
	 * @param msg
	 * @return
	 */
	@Override
	public boolean onMessage (HawkAppObj appObj, HawkMsg msg)
	{
		Player player = (Player) msg.getParam(0);
		HawkProtocol protocol = (HawkProtocol)msg.getParam(1);
		HSAllianceCreateTeam request = protocol.parseProtocol(HSAllianceCreateTeam.getDefaultInstance());
		
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
		
		if (allianceEntity.getTeamEntity(player.getId()) != null) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_ALREADY_IN_TEAM_VALUE);
			return true;
		}
		
		if (player.getPlayerData().getStatisticsEntity().getAllianceTaskCountDaily() >= SysBasicCfg.getInstance().getAllianceMaxBigTask()) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_MAX_BIG_TASK_VALUE);
			return true;
		}
		
		SociatyTaskCfg task = HawkConfigManager.getInstance().getConfigByKey(SociatyTaskCfg.class, request.getTaskId());
		if (task == null) {
			player.sendError(protocol.getType(), Status.error.CONFIG_ERROR_VALUE);
			return true;
		}
		
		if (task.getMinLevel() > playerAllianceEntity.getLevel()) {
			player.sendError(protocol.getType(), Status.allianceError.ALLIANCE_LEVEL_LIMIT_VALUE);
			return true;
		}
		
		// 锁住玩家
		HawkXID xid = HawkXID.valueOf(GsConst.ObjType.PLAYER, player.getId());
		if(xid != null){
			HawkObjBase<HawkXID, HawkAppObj> objBase = GsApp.getInstance().lockObject(xid);
			try {
				if (objBase != null && objBase.isObjValid()) {		
					ConsumeItems consume = new ConsumeItems();
					consume.addGold(task.getTaskStart());
					if (consume.checkConsume(player, protocol.getType()) == false) {
						return true;
					}
					
					AllianceTeamEntity teamEntity = new AllianceTeamEntity();
					teamEntity.setAllianceEntity(allianceEntity);
					teamEntity.setCaptain(player.getId());	
					teamEntity.setTaskId(request.getTaskId());
					teamEntity.setAllianceId(allianceEntity.getId());
					
					try {
						List<Integer> taskList = new LinkedList<>();
						
						// 3个物品任务
						taskList.addAll(task.getItemTaskSet());
						int taskIndex = HawkRand.randInt(0, taskList.size() - 1);
						teamEntity.setItemQuest1(taskList.remove(taskIndex));
						taskIndex = HawkRand.randInt(0, taskList.size() - 1);
						teamEntity.setItemQuest2(taskList.remove(taskIndex));
						taskIndex = HawkRand.randInt(0, taskList.size() - 1);
						teamEntity.setItemQuest3(taskList.remove(taskIndex));
						taskList.clear();
						
						// 2个金币任务
						taskList.addAll(task.getCoinTaskSet());
						taskIndex = HawkRand.randInt(0, taskList.size() - 1);
						teamEntity.setCoinQuest1(taskList.remove(taskIndex));
						taskIndex = HawkRand.randInt(0, taskList.size() - 1);
						teamEntity.setCoinQuest2(taskList.remove(taskIndex));
						taskList.clear();
						
						// 1个副本任务
						taskList.addAll(task.getInstanceTaskSet());
						taskIndex = HawkRand.randInt(0, taskList.size() - 1);
						teamEntity.setInstanceQuest1(taskList.remove(taskIndex));
						taskList.clear();		

					} catch (Exception e) {
						HawkException.catchException(e);
						player.sendError(protocol.getType(), Status.error.SERVER_ERROR_VALUE);
						return true;
					}
					
					teamEntity.setOverTime(task.getTime() * 60);
					
					if (teamEntity.notifyCreate() == false) {
						player.sendError(protocol.getType(), Status.error.SERVER_ERROR_VALUE);
						return true;
					}
					
					allianceEntity.addAllianceTeamEntity(teamEntity);
					consume.consumeTakeAffectAndPush(player, Action.ALLIANCE_TASK_CONSUME, protocol.getType());
					
					HSAllianceCreateTeamRet.Builder response = HSAllianceCreateTeamRet.newBuilder();
					response.setTeamId(teamEntity.getId());
					player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_CREATE_TEAM_S_VALUE, response));	
					
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
