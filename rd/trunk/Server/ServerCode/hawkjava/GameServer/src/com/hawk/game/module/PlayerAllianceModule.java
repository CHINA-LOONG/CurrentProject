package com.hawk.game.module;

import org.hawk.app.HawkApp;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkTime;
import org.hawk.xid.HawkXID;

import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.AllianceTeamEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.manager.ImManager;
import com.hawk.game.module.alliance.AllianceListHandler;
import com.hawk.game.module.alliance.AllianceNoticeHandler;
import com.hawk.game.module.alliance.AllianceSearchHandler;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.Alliance.AllianceInfo;
import com.hawk.game.protocol.Alliance.AllianceMember;
import com.hawk.game.protocol.Alliance.HSAllianceApplyList;
import com.hawk.game.protocol.Alliance.HSAllianceContribution;
import com.hawk.game.protocol.Alliance.HSAllianceDataRet;
import com.hawk.game.protocol.Alliance.HSAllianceMembers;
import com.hawk.game.protocol.Alliance.HSAllianceSelfDataRet;
import com.hawk.game.protocol.Alliance.HSAllianceSelfTeam;
import com.hawk.game.protocol.Alliance.HSAllianceSettingSyn;
import com.hawk.game.protocol.Alliance.HSAllianceSettingSynRet;
import com.hawk.game.protocol.Alliance.HSAllianceSyn;
import com.hawk.game.protocol.Alliance.HSAllianceSynRet;
import com.hawk.game.protocol.Alliance.HSAllianceTeamList;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.AllianceUtil;
import com.hawk.game.util.GsConst;

/**
 * 公会模块
 * 
 * @author zs
 */
public class PlayerAllianceModule extends PlayerModule {
	/**
	 * 构造函数
	 * 
	 * @param player
	 */
	public PlayerAllianceModule(Player player) {
		super(player);
		listenProto(HS.code.ALLIANCE_CREATE_C);
		listenProto(HS.code.ALLIANCE_LIST_C, new AllianceListHandler());
		listenProto(HS.code.ALLIANCE_SEARCH_C, new AllianceSearchHandler());
		listenProto(HS.code.ALLIANCE_NOTICE_C_VALUE, new AllianceNoticeHandler());
		listenProto(HS.code.ALLIANCE_APPLY_C_VALUE);
		listenProto(HS.code.ALLIANCE_HANDLE_APPLY_C);
		listenProto(HS.code.ALLIANCE_CHANGE_OWNER_C);
		listenProto(HS.code.ALLIANCE_CHANGE_POS_C);
		listenProto(HS.code.ALLIANCE_MEMBER_KICK_C);
		listenProto(HS.code.ALLIANCE_MEMBER_LEAVE_C);
		listenProto(HS.code.ALLIANCE_LEVEL_UP_C);
		listenProto(HS.code.ALLIANCE_PRAY_C);
		listenProto(HS.code.ALLIANCE_SYN_C_VALUE);
		listenProto(HS.code.ALLIANCE_SETTING_C);
		listenProto(HS.code.ALLIANCE_FATIGUE_GIVE_C_VALUE);
		listenProto(HS.code.ALLIANCE_MEMBERS_C_VALUE);
		listenProto(HS.code.ALLIANCE_APPLYS_C_VALUE);
		listenProto(HS.code.ALLIANCE_CANCLE_APPLY_C_VALUE);
		listenProto(HS.code.ALLIANCE_CREATE_TEAM_C_VALUE);
		listenProto(HS.code.ALLIANCE_JOIN_TEAM_C_VALUE);
		listenProto(HS.code.ALLIANCE_ACCEPT_TASK_C_VALUE);
		listenProto(HS.code.ALLIANCE_COMMIT_TASK_C_VALUE);
		listenProto(HS.code.ALLIANCE_TASK_REWARD_C_VALUE);
		listenProto(HS.code.ALLIANCE_SELF_TEAM_C_VALUE);
		listenProto(HS.code.ALLIANCE_TEAM_LIST_C_VALUE);
		listenProto(HS.code.ALLIANCE_DISSOVLE_TEAM_C_VALUE);
		listenProto(HS.code.ALLIANCE_CONTRIBUTION_C_VALUE);
		listenProto(HS.code.ALLIANCE_CONTRI_REWARD_C_VALUE);
	}

	/**
	 * 玩家上线处理
	 * 
	 * @return
	 */
	protected boolean onPlayerLogin() {
		// 加载公会数据
		PlayerAllianceEntity allianceEntity = player.getPlayerData().loadPlayerAlliance();
		allianceEntity.setLoginTime(HawkTime.getSeconds());
		allianceEntity.notifyUpdate(true);
		
		if (allianceEntity.getRefreshTime() < HawkTime.getSeconds()) {
			allianceEntity.clearFatigueCount();
			allianceEntity.clearFatigueSet();
			allianceEntity.setRefreshTime((int) HawkTime.getNextAM0Date() / 1000);
			allianceEntity.notifyUpdate(true);
		}
		
		int allianceId = allianceEntity.getAllianceId();
		if (allianceId != 0) {
			ImManager.getInstance().joinGuild(allianceId, player);
		}
		return true;
	}
	
	protected boolean onPlayerLogout() {
		int allianceId = player.getPlayerData().getPlayerAllianceEntity().getAllianceId();
		if (allianceId != 0) {
			ImManager.getInstance().quitGuild(allianceId, player.getId());
		}
		
		player.getPlayerData().getPlayerAllianceEntity().setLogoutTime(HawkTime.getSeconds());	
		return true;
	}

	/**
	 * 消息响应
	 * 
	 * @param msg
	 * @return
	 */
	@Override
	public boolean onMessage(HawkMsg msg) {
		return super.onMessage(msg);
	}

	/**
	 * 协议响应
	 * 
	 * @param protocol
	 * @return
	 */
	@Override
	public boolean onProtocol(HawkProtocol protocol) {
		if (protocol.checkType(HS.code.ALLIANCE_CREATE_C_VALUE))
		{
	 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.ALLIANCE_CREATE, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
	 		msg.pushParam(player);
	 		msg.pushParam(protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_APPLY_C_VALUE))
		{
	 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.ALLIANCE_APPLY, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
	 		msg.pushParam(player);
	 		msg.pushParam(protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_HANDLE_APPLY_C_VALUE)){
			HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.ALLIANCE_HANDLE_APPLY, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
			msg.pushParam(player);
			msg.pushParam(protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_CHANGE_POS_C_VALUE)){
			HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.ALLIANCE_CHANGE_POS, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
			msg.pushParam(player);
			msg.pushParam(protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_CHANGE_OWNER_C_VALUE))
		{
	 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.ALLIANCE_CHANGE_OWNER, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
	 		msg.pushParam(player);
	 		msg.pushParam(protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_MEMBER_KICK_C_VALUE))
		{
	 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.ALLIANCE_KICK, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
	 		msg.pushParam(player);
	 		msg.pushParam(protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_MEMBER_LEAVE_C_VALUE))
		{
	 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.ALLIANCE_LEAVE, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
	 		msg.pushParam(player);
	 		msg.pushParam(protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_LEVEL_UP_C_VALUE))
		{
	 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.ALLIANCE_LEVEL_UP, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
	 		msg.pushParam(player);
	 		msg.pushParam(protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_PRAY_C))
		{
	 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.ALLIANCE_PRAY, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
	 		msg.pushParam(player);
	 		msg.pushParam(protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_SETTING_C_VALUE))
		{
	 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.ALLIANCE_SETTING, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
	 		msg.pushParam(player);
	 		msg.pushParam(protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_FATIGUE_GIVE_C_VALUE))
		{
	 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.ALLIANCE_FATIGUE_GIVE, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
	 		msg.pushParam(player);
	 		msg.pushParam(protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_CANCLE_APPLY_C_VALUE))
		{
	 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.ALLIANCE_CANCLE_APPLY, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
	 		msg.pushParam(player);
	 		msg.pushParam(protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_CREATE_TEAM_C_VALUE))
		{
	 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.ALLIANCE_TEAM_CREATE, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
	 		msg.pushParam(player);
	 		msg.pushParam(protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_JOIN_TEAM_C_VALUE))
		{
	 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.ALLIANCE_TEAM_JOIN, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
	 		msg.pushParam(player);
	 		msg.pushParam(protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_ACCEPT_TASK_C_VALUE))
		{
	 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.ALLIANCE_TASK_ACCEPT, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
	 		msg.pushParam(player);
	 		msg.pushParam(protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_COMMIT_TASK_C_VALUE))
		{
	 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.ALLIANCE_TASK_COMMIT, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
	 		msg.pushParam(player);
	 		msg.pushParam(protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_INSTANCE_TASK_C_VALUE))
		{
	 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.ALLIANCE_INSTANCE_TASK, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
	 		msg.pushParam(player);
	 		msg.pushParam(protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_TASK_REWARD_C_VALUE))
		{
	 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.ALLIANCE_REWARD_TASK, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
	 		msg.pushParam(player);
	 		msg.pushParam(protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_DISSOVLE_TEAM_C_VALUE))
		{
	 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.ALLIANCE_DISSOLVE_TEAM, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
	 		msg.pushParam(player);
	 		msg.pushParam(protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_CONTRI_REWARD_C_VALUE))
		{
	 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.ALLIANCE_CONTRIBUTION_REWARD, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
	 		msg.pushParam(player);
	 		msg.pushParam(protocol);
			HawkApp.getInstance().postMsg(msg);
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_SYN_C_VALUE)) {
			onAllianceSyn(HS.code.ALLIANCE_SYN_C_VALUE, protocol.parseProtocol(HSAllianceSyn.getDefaultInstance()));
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_SETTING_SYNC_C_VALUE)) {
			onAllianceSettingSyn(HS.code.ALLIANCE_SETTING_C_VALUE, protocol.parseProtocol(HSAllianceSettingSyn.getDefaultInstance()));
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_MEMBERS_C_VALUE)) {
			onAllianceMembersSyn(HS.code.ALLIANCE_MEMBERS_C_VALUE, protocol.parseProtocol(HSAllianceMembers.getDefaultInstance()));
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_APPLYS_C_VALUE)) {
			onAllianceApplysSyn(HS.code.ALLIANCE_APPLYS_C_VALUE, protocol.parseProtocol(HSAllianceApplyList.getDefaultInstance()));
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_SELF_TEAM_C_VALUE)) {
			onAllianceSelfTeamSyn(HS.code.ALLIANCE_SELF_DATA_C_VALUE, protocol.parseProtocol(HSAllianceSelfTeam.getDefaultInstance()));
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_TEAM_LIST_C_VALUE)) {
			onAllianceTeamListSyn(HS.code.ALLIANCE_TEAM_LIST_C_VALUE, protocol.parseProtocol(HSAllianceTeamList.getDefaultInstance()));
			return true;
		}
		else if (protocol.checkType(HS.code.ALLIANCE_CONTRIBUTION_C_VALUE)) {
			onAllianceContributionSyn(HS.code.ALLIANCE_CONTRIBUTION_C_VALUE, protocol.parseProtocol(HSAllianceContribution.getDefaultInstance()));
			return true;
		}
		
		return super.onProtocol(protocol);
	}
	
	/**
	 * 工会数据同步
	 * 
	 */
	public boolean onAllianceSyn(int hsCode, HSAllianceSyn protocol) {
		if (player.getAllianceId() == 0) {
			sendError(hsCode, Status.allianceError.ALLIANCE_NOT_JOIN_VALUE);
			return true;
		}
		
		AllianceEntity allianceEntity = AllianceManager.getInstance().getAlliance(player.getAllianceId());
		if (allianceEntity == null) {
			sendError(hsCode, Status.allianceError.ALLIANCE_NOT_EXIST_VALUE);
			return true;
		}
		
		// 同步工会数据
		HSAllianceDataRet.Builder allianceData = HSAllianceDataRet.newBuilder();
		AllianceInfo.Builder allianceBuilder = AllianceUtil.getAllianceInfo(allianceEntity);
		allianceData.setAllianceData(allianceBuilder);
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_DATA_S_VALUE, allianceData));

		// 同步自身数据
		HSAllianceSelfDataRet.Builder sleftData = HSAllianceSelfDataRet.newBuilder();
		AllianceMember.Builder memberBuilder = AllianceUtil.getMemberInfo(player.getPlayerData().getPlayerAllianceEntity(), null);
		memberBuilder.setPrayCount(player.getPlayerData().getStatisticsEntity().getAlliancePrayCountDaily());
		memberBuilder.setTaskCount(player.getPlayerData().getStatisticsEntity().getAllianceTaskCountDaily());
		sleftData.setSelfData(memberBuilder);
		sleftData.setContributionReward(player.getPlayerData().getStatisticsEntity().getAllianceContriRewardDaily());
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_SELF_DATA_S_VALUE, sleftData));

		// 同步成员数据
		//player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_MEMBERS_S_VALUE, AllianceUtil.getAllianceMembersInfo(allianceEntity, player.getId())));
		
		// 同步申请列表
		//player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_APPLYS_S_VALUE, AllianceUtil.getApplyList(allianceEntity)));

		// 同步完成 
		HSAllianceSynRet.Builder response = HSAllianceSynRet.newBuilder();
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_SYN_S_VALUE, response));
		return true;
	}
	
	/**
	 * 工会设置数据同步
	 * 
	 */
	public boolean onAllianceSettingSyn(int hsCode, HSAllianceSettingSyn protocol) {
		if (player.getAllianceId() == 0) {
			sendError(hsCode, Status.allianceError.ALLIANCE_NOT_JOIN_VALUE);
			return true;
		}
		
		AllianceEntity allianceEntity = AllianceManager.getInstance().getAlliance(player.getAllianceId());
		if (allianceEntity == null) {
			sendError(hsCode, Status.allianceError.ALLIANCE_NOT_EXIST_VALUE);
			return true;
		}
		
		HSAllianceSettingSynRet.Builder response = HSAllianceSettingSynRet.newBuilder();
		response.setMinLevel(allianceEntity.getMinLevel());
		response.setAutoJoin(allianceEntity.isAutoAccept());
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_SETTING_SYNC_S_VALUE, response));
		
		return true;
	}
	
	/**
	 * 工会成员数据同步
	 * 
	 */
	public boolean onAllianceMembersSyn(int hsCode, HSAllianceMembers protocol) {
		if (player.getAllianceId() == 0) {
			sendError(hsCode, Status.allianceError.ALLIANCE_NOT_JOIN_VALUE);
			return true;
		}
		
		AllianceEntity allianceEntity = AllianceManager.getInstance().getAlliance(player.getAllianceId());
		if (allianceEntity == null) {
			sendError(hsCode, Status.allianceError.ALLIANCE_NOT_EXIST_VALUE);
			return true;
		}
		
		PlayerAllianceEntity playerEntity = allianceEntity.getMember(player.getId());
		if (playerEntity == null) {
			sendError(hsCode, Status.error.SERVER_ERROR_VALUE);
			return true;
		}
		
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_MEMBERS_S_VALUE, AllianceUtil.getAllianceMembersInfo(allianceEntity, playerEntity, 0)));
		return true;
	}
	
	/**
	 * 工会成员数据同步
	 * 
	 */
	public boolean onAllianceApplysSyn(int hsCode, HSAllianceApplyList protocol) {
		if (player.getAllianceId() == 0) {
			sendError(hsCode, Status.allianceError.ALLIANCE_NOT_JOIN_VALUE);
			return true;
		}
		
		AllianceEntity allianceEntity = AllianceManager.getInstance().getAlliance(player.getAllianceId());
		if (allianceEntity == null) {
			sendError(hsCode, Status.allianceError.ALLIANCE_NOT_EXIST_VALUE);
			return true;
		}
		
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_APPLYS_S_VALUE, AllianceUtil.getApplyList(allianceEntity)));
		return true;
	}
	
	/**
	 * 队伍列表同步
	 * 
	 */
	public boolean onAllianceTeamListSyn(int hsCode, HSAllianceTeamList protocol) {
		if (player.getAllianceId() == 0) {
			sendError(hsCode, Status.allianceError.ALLIANCE_NOT_JOIN_VALUE);
			return true;
		}
		
		AllianceEntity allianceEntity = AllianceManager.getInstance().getAlliance(player.getAllianceId());
		if (allianceEntity == null) {
			sendError(hsCode, Status.allianceError.ALLIANCE_NOT_EXIST_VALUE);
			return true;
		}
		
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_TEAM_LIST_S_VALUE, AllianceUtil.getTeamList(allianceEntity)));
		return true;
	}
	
	/**
	 * 队伍列表同步
	 * 
	 */
	public boolean onAllianceContributionSyn(int hsCode, HSAllianceContribution protocol) {
		if (player.getAllianceId() == 0) {
			sendError(hsCode, Status.allianceError.ALLIANCE_NOT_JOIN_VALUE);
			return true;
		}
		
		AllianceEntity allianceEntity = AllianceManager.getInstance().getAlliance(player.getAllianceId());
		if (allianceEntity == null) {
			sendError(hsCode, Status.allianceError.ALLIANCE_NOT_EXIST_VALUE);
			return true;
		}
		
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_CONTRIBUTION_S_VALUE, AllianceUtil.getAllianceContributionInfo(allianceEntity)));
		return true;
	}
	
	/**
	 * 自身队伍数据同步
	 * 
	 */
	public boolean onAllianceSelfTeamSyn(int hsCode, HSAllianceSelfTeam protocol) {
		if (player.getAllianceId() == 0) {
			sendError(hsCode, Status.allianceError.ALLIANCE_NOT_JOIN_VALUE);
			return true;
		}
		
		AllianceEntity allianceEntity = AllianceManager.getInstance().getAlliance(player.getAllianceId());
		if (allianceEntity == null) {
			sendError(hsCode, Status.allianceError.ALLIANCE_NOT_EXIST_VALUE);
			return true;
		}
		
		PlayerAllianceEntity playerAllianceEntity = allianceEntity.getMember(player.getId());
		if (playerAllianceEntity == null) {
			player.sendError(hsCode, Status.error.SERVER_ERROR_VALUE);
			return true;
		}
		
		AllianceTeamEntity teamEntity = allianceEntity.getTeamEntity(player.getId());
		if (teamEntity == null) {
			player.sendError(hsCode, Status.allianceError.ALLIANCE_NOT_IN_TEAM_VALUE);
			return true;
		}
		
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_SELF_TEAM_S_VALUE, AllianceUtil.getSelfTeamInfo(teamEntity, allianceEntity)));
		return true;
	}
}
