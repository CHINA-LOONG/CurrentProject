package com.hawk.game.module;

import java.util.Calendar;

import org.hawk.app.HawkApp;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkTime;
import org.hawk.xid.HawkXID;

import com.hawk.game.ServerData;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.PVPDefenceRecordEntity;
import com.hawk.game.entity.PVPRankEntity;
import com.hawk.game.manager.PVPManager;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.PVP.HSGetPVPDefenceMonsterRet;
import com.hawk.game.protocol.PVP.HSPVPDefenceRecordRet;
import com.hawk.game.protocol.PVP.HSPVPInfoRet;
import com.hawk.game.protocol.PVP.PVPDefenceRecordData;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.Monster.HSMonster;
import com.hawk.game.protocol.Monster.HSMonsterDefence;
import com.hawk.game.protocol.PVP.HSSetPVPDefenceMonster;
import com.hawk.game.protocol.PVP.HSSetPVPDefenceMonsterRet;
import com.hawk.game.util.BuilderUtil;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.MonsterUtil;
import com.hawk.game.util.TimePointUtil;

public class PlayerPVPModule extends PlayerModule{

	public PlayerPVPModule(Player player) {
		super(player);
		listenProto(HS.code.PVP_MATCH_TARGET_C_VALUE);
		listenProto(HS.code.PVP_ENTER_ROOM_C_VALUE);
		listenProto(HS.code.PVP_SETTLE_C_VALUE);
		listenProto(HS.code.PVP_SET_DEFENCE_MONSTERS_C_VALUE);
		listenProto(HS.code.PVP_GET_DEFENCE_MONSTERS_C_VALUE);
		listenProto(HS.code.PVP_RANK_LIST_C_VALUE);
		listenProto(HS.code.PVP_DEFENCE_RECORD_C_VALUE);
		listenProto(HS.code.PVP_GET_MY_INFO_C_VALUE);
		listenProto(HS.code.PVP_GET_RANK_DEFENCE_C_VALUE);
		listenMsg(GsConst.MsgType.PVP_RECORD);
	}

	/**
	 * 玩家上线处理
	 * 
	 * @return
	 */
	protected boolean onPlayerLogin() {
		player.getPlayerData().loadPVPDefenceData();
		player.getPlayerData().loadPVPDefenceRecordData();
		player.getPlayerData().syncPVPDefenceInfo();
		return true;
	}

	/**
	 * 处理玩家下线
	 */
	protected boolean onPlayerLogout() {
		// 更新宠物信息
		HSMonsterDefence.Builder monstersBuilder = HSMonsterDefence.newBuilder();
		if (player.getPlayerData().getPVPDefenceEntity().hasSetDefenceMonsters()) {
			for (HSMonster monster : player.getPlayerData().getPVPDefenceEntity().getMonsterDefenceBuilder().getMonsterInfoList()) {
				MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(monster.getMonsterId());
				if (monsterEntity != null) {
					monstersBuilder.addMonsterInfo(BuilderUtil.genCompleteMonsterBuilder(player, monsterEntity));
				}
			}
			
			player.getPlayerData().getPVPDefenceEntity().setBP((int)(MonsterUtil.calculateBP(monstersBuilder.getMonsterInfoBuilderList())));
			player.getPlayerData().getPVPDefenceEntity().setLevel(player.getLevel());
			player.getPlayerData().getPVPDefenceEntity().setMonsterDefenceBuilder(monstersBuilder);
			player.getPlayerData().getPVPDefenceEntity().notifyUpdate(true);

	 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.PVP_LOGOUT, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.PVP));
	 		msg.pushParam(player);
			HawkApp.getInstance().postMsg(msg);
		}

		return true;
	}

	/**
	 * 协议响应
	 * 
	 * @param protocol
	 * @return
	 */
	@Override
	public boolean onProtocol(HawkProtocol protocol) {
		if (protocol.getType() == HS.code.PVP_MATCH_TARGET_C_VALUE) {
	 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.PVP_MATCH_TARGET, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.PVP));
	 		msg.pushParam(player, protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.getType() == HS.code.PVP_ENTER_ROOM_C_VALUE) {
	 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.PVP_ENTER_ROOM, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.PVP));
	 		msg.pushParam(player, protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.getType() == HS.code.PVP_SETTLE_C_VALUE) {
	 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.PVP_SETTLE, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.PVP));
	 		msg.pushParam(player, protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.getType() == HS.code.PVP_RANK_LIST_C_VALUE) {
			HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.PVP_RANK_LIST, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.PVP));
			msg.pushParam(player, protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.getType() == HS.code.PVP_GET_RANK_DEFENCE_C_VALUE) {
			HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.PVP_RANK_DEFENCE, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.PVP));
			msg.pushParam(player, protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.getType() == HS.code.PVP_SET_DEFENCE_MONSTERS_C_VALUE) {
			onSetDefenceMonsters(protocol);
		}
		else if (protocol.getType() == HS.code.PVP_GET_DEFENCE_MONSTERS_C_VALUE) {
			onGetDefenceMonsters(protocol);
		}
		else if (protocol.getType() == HS.code.PVP_DEFENCE_RECORD_C_VALUE) {
			onGetPVPDefenceRecord(protocol);
		}
		else if (protocol.getType() == HS.code.PVP_GET_MY_INFO_C_VALUE) {
			onGetPVPMyInfo(protocol);
		}

		return false;
	}

	/**
	 * 设置防守阵容
	 * @param cmd
	 * @return
	 */
	private boolean onSetDefenceMonsters(HawkProtocol cmd) {
		HSSetPVPDefenceMonster protocol = cmd.parseProtocol(HSSetPVPDefenceMonster.getDefaultInstance());

		HSMonsterDefence.Builder monstersBuilder = HSMonsterDefence.newBuilder();
		for (int monsterId : protocol.getMonsterIdList()) {
			MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(monsterId);
			if (monsterEntity != null) {
				monstersBuilder.addMonsterInfo(BuilderUtil.genCompleteMonsterBuilder(player, monsterEntity));
			}
			else {
				sendError(HS.code.PVP_SET_DEFENCE_MONSTERS_C_VALUE, Status.monsterError.MONSTER_NOT_EXIST_VALUE);
				return true;
			}
		}

		if (player.getPlayerData().getPVPDefenceEntity().hasSetDefenceMonsters()) {
			for (HSMonster monster : player.getPlayerData().getPVPDefenceEntity().getMonsterDefenceBuilder().getMonsterInfoList()) {
				MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(monster.getMonsterId());
				if (monsterEntity != null) {
					monsterEntity.removeState(Const.MonsterState.IN_PVP_DEFENCE_VALUE);
					monsterEntity.notifyUpdate(true);
				}
			}
		}

		for (int monsterId : protocol.getMonsterIdList()) {
			MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(monsterId);
			if (monsterEntity != null) {
				monsterEntity.addState(Const.MonsterState.IN_PVP_DEFENCE_VALUE);
				monsterEntity.notifyUpdate(true);
			}
		}

		player.getPlayerData().getPVPDefenceEntity().setBP((int)(MonsterUtil.calculateBP(monstersBuilder.getMonsterInfoBuilderList())));
		player.getPlayerData().getPVPDefenceEntity().setMonsterDefenceBuilder(monstersBuilder);
		player.getPlayerData().getPVPDefenceEntity().notifyUpdate(true);

		HSSetPVPDefenceMonsterRet.Builder response = HSSetPVPDefenceMonsterRet.newBuilder();
		sendProtocol(HawkProtocol.valueOf(HS.code.PVP_SET_DEFENCE_MONSTERS_S_VALUE, response));

		return true;
	}

	/**
	 * 获取防守阵容
	 * @param cmd
	 * @return
	 */
	private boolean onGetDefenceMonsters(HawkProtocol cmd) {
		HSGetPVPDefenceMonsterRet.Builder response = HSGetPVPDefenceMonsterRet.newBuilder();
		for (HSMonster.Builder monster : player.getPlayerData().getPVPDefenceEntity().getMonsterDefenceBuilder().getMonsterInfoBuilderList()) {
			response.addMonsterId(monster.getMonsterId());
		}

		sendProtocol(HawkProtocol.valueOf(HS.code.PVP_GET_DEFENCE_MONSTERS_S_VALUE, response));

		return true;
	}

	/**
	 * 获取防守记录
	 * @param cmd
	 * @return
	 */
	private boolean onGetPVPDefenceRecord(HawkProtocol cmd){
		HSPVPDefenceRecordRet.Builder response = HSPVPDefenceRecordRet.newBuilder();

		for (PVPDefenceRecordEntity pvpDefenceRecordEntity : player.getPlayerData().getPVPDefenceRecordList()) {
			PVPDefenceRecordData.Builder pvpDefenceRecordData = PVPDefenceRecordData.newBuilder();
			pvpDefenceRecordData.setAttacker(pvpDefenceRecordEntity.getAttackerName());
			pvpDefenceRecordData.setChangePoint(pvpDefenceRecordEntity.getChangePoint());
			pvpDefenceRecordData.setPoint(pvpDefenceRecordEntity.getAttackerPoint());
			pvpDefenceRecordData.setLevel(pvpDefenceRecordEntity.getAttackerLevel());
			pvpDefenceRecordData.setResult(pvpDefenceRecordEntity.getResult());

			response.addPvpDefenceRecordList(pvpDefenceRecordData);

			if (response.getPvpDefenceRecordListCount() >= GsConst.PVP.PVP_DEFENCE_RECORD_SIZE) {
				break;
			}
		}

		sendProtocol(HawkProtocol.valueOf(HS.code.PVP_DEFENCE_RECORD_S_VALUE, response));
		return true;
	}

	private boolean onGetPVPMyInfo(HawkProtocol cmd){;
		PVPRankEntity rankEntity = PVPManager.getInstance().getPVPRankEntity(player.getId());
		HSPVPInfoRet.Builder response = HSPVPInfoRet.newBuilder();

		Calendar refreshTime = TimePointUtil.getComingRefreshTime(GsConst.PVP.PVP_WEEK_REFRESH_TIME_ID, HawkTime.getCalendar());
		int leftTime = (3 - ServerData.getInstance().getPVPWeekRewardCount()) * GsConst.WEEK_SECOND;
		if (refreshTime != null) {
			leftTime += (int)((refreshTime.getTimeInMillis() - HawkTime.getMillisecond()) / 1000);
		}

		if (rankEntity != null) {
			// 排行榜之内的需要考虑并列情况
			if (rankEntity.getRank() > GsConst.PVP.PVP_RANK_SIZE + 10) {
				response.setPvpPoint(rankEntity.getPoint());
				response.setPvpRank(rankEntity.getRank() + 1);
			}
			else{
				HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.PVP_MY_INFO, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.PVP));
		 		msg.pushParam(player, leftTime);
				HawkApp.getInstance().postMsg(msg);
				return true;
			}
		}
		else{
			response.setPvpPoint(GsConst.PVP.PVP_DEFAULT_POINT);
			response.setPvpRank(0);
		}

		response.setMonthRewardTimeLeft(leftTime);

		sendProtocol(HawkProtocol.valueOf(HS.code.PVP_GET_MY_INFO_S_VALUE, response));
		return true;
	}

	@Override
	public boolean onMessage(HawkMsg msg) {
		if(msg.getMsg() == GsConst.MsgType.PVP_RECORD){
			onPVPRecord(msg);
			return true;
		}

		return super.onMessage(msg);
	}

	private boolean onPVPRecord(HawkMsg msg) {
		PVPDefenceRecordEntity pvpDefenceRecordEntity = msg.getParam(0);
		player.getPlayerData().addPVPDefenceRecord(pvpDefenceRecordEntity);
		return true;
	}
}
