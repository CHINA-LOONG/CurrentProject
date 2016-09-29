package com.hawk.game.manager;

import java.util.Comparator;
import java.util.HashSet;
import java.util.Iterator;
import java.util.LinkedHashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.TreeSet;
import java.util.concurrent.ConcurrentHashMap;

import org.hawk.app.HawkAppObj;
import org.hawk.db.HawkDBManager;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkTime;
import org.hawk.xid.HawkXID;

import com.hawk.game.GsApp;
import com.hawk.game.ServerData;
import com.hawk.game.entity.AllianceApplyEntity;
import com.hawk.game.entity.AllianceBaseEntity;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.AllianceTeamEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.module.alliance.AllianceAcceptQuestHandler;
import com.hawk.game.module.alliance.AllianceApplyHandler;
import com.hawk.game.module.alliance.AllianceBaseHireHandler;
import com.hawk.game.module.alliance.AllianceBaseRecallHandler;
import com.hawk.game.module.alliance.AllianceBaseSendHandler;
import com.hawk.game.module.alliance.AllianceCancleApplyHandler;
import com.hawk.game.module.alliance.AllianceChangeOwnerHandler;
import com.hawk.game.module.alliance.AllianceChangePosHandler;
import com.hawk.game.module.alliance.AllianceCommitQuestHandler;
import com.hawk.game.module.alliance.AllianceContributionRewardHandler;
import com.hawk.game.module.alliance.AllianceCreateHandler;
import com.hawk.game.module.alliance.AllianceDissolveTeamHandler;
import com.hawk.game.module.alliance.AllianceFatigueHandler;
import com.hawk.game.module.alliance.AllianceHandleApplyHandler;
import com.hawk.game.module.alliance.AllianceInstanceQuestHandler;
import com.hawk.game.module.alliance.AllianceJoinTeamHandler;
import com.hawk.game.module.alliance.AllianceLevelUpHandler;
import com.hawk.game.module.alliance.AllianceMemberKickHandler;
import com.hawk.game.module.alliance.AllianceMemberLeaveHandler;
import com.hawk.game.module.alliance.AlliancePrayHandler;
import com.hawk.game.module.alliance.AllianceSettingHandler;
import com.hawk.game.module.alliance.AllianceTaskRewardHandler;
import com.hawk.game.module.alliance.AllianceTeamCreateHandler;
import com.hawk.game.player.Player;
import com.hawk.game.util.GsConst;

/**
 * @author zs 
 * 公会管理器
 */

public class AllianceManager extends HawkAppObj {
	/**
	 * 全局对象, 便于访问
	 */
	private static AllianceManager instance = null;
	
	/**
	 * 获取全局实例对象
	 */
	public static AllianceManager getInstance() {
		return instance;
	}

	/**
	 * 公会管理对象 公会ID,对象
	 */
	private ConcurrentHashMap<Integer, AllianceEntity> allianceMap;
	
	/**
	 * 申请列表
	 */
	private ConcurrentHashMap<Integer, LinkedHashSet<Integer>> playerApplyMap;
	
	/**
	 * 玩家工会映射表
	 */
	private ConcurrentHashMap<Integer, Integer> playerAllianceMap;
	
	/**
	 * 工会有序列表
	 */
	private Set<AllianceEntity> allianceLevelSet;
	
	/**
	 * 已存在公会名字
	 */
	private Set<String> existName;
	
	/**
	 * 构造函数
	 */
	public AllianceManager(HawkXID xid) {
		super(xid);
		if (instance == null) {
			instance = this;
		}

		listenMsg(GsConst.MsgType.ALLIANCE_CREATE, new AllianceCreateHandler());
		listenMsg(GsConst.MsgType.ALLIANCE_APPLY, new AllianceApplyHandler());
		listenMsg(GsConst.MsgType.ALLIANCE_HANDLE_APPLY, new AllianceHandleApplyHandler());
		listenMsg(GsConst.MsgType.ALLIANCE_CHANGE_POS, new AllianceChangePosHandler());
		listenMsg(GsConst.MsgType.ALLIANCE_LEVEL_UP, new AllianceLevelUpHandler());
		listenMsg(GsConst.MsgType.ALLIANCE_FATIGUE_GIVE, new AllianceFatigueHandler());
		listenMsg(GsConst.MsgType.ALLIANCE_CHANGE_OWNER, new AllianceChangeOwnerHandler());
		listenMsg(GsConst.MsgType.ALLIANCE_LEAVE, new AllianceMemberLeaveHandler());
		listenMsg(GsConst.MsgType.ALLIANCE_KICK, new AllianceMemberKickHandler());
		listenMsg(GsConst.MsgType.ALLIANCE_PRAY, new AlliancePrayHandler());
		listenMsg(GsConst.MsgType.ALLIANCE_SETTING, new AllianceSettingHandler());
		listenMsg(GsConst.MsgType.ALLIANCE_CANCLE_APPLY, new AllianceCancleApplyHandler());
		listenMsg(GsConst.MsgType.ALLIANCE_TEAM_CREATE, new AllianceTeamCreateHandler());
		listenMsg(GsConst.MsgType.ALLIANCE_TEAM_JOIN, new AllianceJoinTeamHandler());
		listenMsg(GsConst.MsgType.ALLIANCE_TASK_ACCEPT, new AllianceAcceptQuestHandler());
		listenMsg(GsConst.MsgType.ALLIANCE_TASK_COMMIT, new AllianceCommitQuestHandler());
		listenMsg(GsConst.MsgType.ALLIANCE_INSTANCE_TASK, new AllianceInstanceQuestHandler());
		listenMsg(GsConst.MsgType.ALLIANCE_REWARD_TASK, new AllianceTaskRewardHandler());
		listenMsg(GsConst.MsgType.ALLIANCE_DISSOLVE_TEAM, new AllianceDissolveTeamHandler());
		listenMsg(GsConst.MsgType.ALLIANCE_CONTRIBUTION_REWARD, new AllianceContributionRewardHandler());
		listenMsg(GsConst.MsgType.ALLIANCE_BASE_SEND, new AllianceBaseSendHandler());
		listenMsg(GsConst.MsgType.ALLIANCE_BASE_RECALL, new AllianceBaseRecallHandler());
		listenMsg(GsConst.MsgType.ALLIANCE_HIRE_REWARD, new AllianceBaseHireHandler());
		listenMsg(GsConst.MsgType.PLAYER_LEVEL_UP);
		
		allianceMap = new ConcurrentHashMap<Integer, AllianceEntity>();
		playerApplyMap = new ConcurrentHashMap<Integer, LinkedHashSet<Integer>>();
		playerAllianceMap = new ConcurrentHashMap<Integer, Integer>();
		allianceLevelSet = new TreeSet<>(new Comparator<AllianceEntity>() {
            public int compare(AllianceEntity o1, AllianceEntity o2) {
                if (o1 == null || o2 == null)
                    return 0; 
                if (o1.getLevel() != o2.getLevel()) {
                	return o2.getLevel() - o1.getLevel();
				}
                else if(o1.get3DaysContribution() != o2.get3DaysContribution() ) {
					return o2.get3DaysContribution() - o1.get3DaysContribution();
				}
                
                return o1.getId() - o2.getId();
         }});
	}
	
	/**
	 * 数据加载
	 */
	public boolean init() {		
		List<AllianceEntity> allianceEntities = HawkDBManager.getInstance().query("from AllianceEntity where invalid = 0");
		List<PlayerAllianceEntity> allianceMembers = HawkDBManager.getInstance().query("from PlayerAllianceEntity where allianceId > 0 and invalid = 0");
		List<AllianceApplyEntity> allianceApplyEnties = HawkDBManager.getInstance().query("from AllianceApplyEntity where invalid = 0");
		List<AllianceTeamEntity> allianceTeams = HawkDBManager.getInstance().query("from AllianceTeamEntity where invalid = 0");
		List<AllianceBaseEntity> allianceBases = HawkDBManager.getInstance().query("from AllianceBaseEntity where invalid = 0");
		
		if (allianceEntities.isEmpty() == false) {
			for (AllianceEntity allianceEntity : allianceEntities) {
				addAlliance(allianceEntity);
				addAllianceForSort(allianceEntity);
			}
		}

		// 填充成员
		for (PlayerAllianceEntity playerAllianceEntity : allianceMembers) {
			playerAllianceEntity.decode();
			AllianceEntity allianceEntity = getAlliance(playerAllianceEntity.getAllianceId());
			if (allianceEntity != null ) {
				allianceEntity.addMember(playerAllianceEntity.getPlayerId(), playerAllianceEntity);
				playerAllianceMap.put(playerAllianceEntity.getPlayerId(), playerAllianceEntity.getAllianceId());
			}
		}

		if (allianceApplyEnties.isEmpty() == false) {
			for (AllianceApplyEntity applyEntity : allianceApplyEnties) {
				AllianceEntity alliance = allianceMap.get(applyEntity.getAllianceId());
				if (alliance != null) {
					alliance.addApply(applyEntity);
					addPlayerApply(applyEntity.getPlayerId(), applyEntity.getAllianceId());
				}
				else {
					applyEntity.delete(true);
				}
			}
		}
 
		// 组队信息
		for (AllianceTeamEntity teamEntity : allianceTeams) {
			AllianceEntity allianceEntity = getAlliance(teamEntity.getAllianceId());
			if (allianceEntity != null ) {
				teamEntity.setAllianceEntity(allianceEntity);
				teamEntity.decode();
				allianceEntity.addAllianceTeamEntity(teamEntity);
			}
		}
		
		//基地派兵信息
		for (AllianceBaseEntity allianceBase : allianceBases) {
			AllianceEntity allianceEntity = getAlliance(allianceBase.getAllianceId());
			if (allianceEntity != null) {
				PlayerAllianceEntity playerAllianceEntity = allianceEntity.getMember(allianceBase.getPlayerId());
				if (playerAllianceEntity != null) {
					allianceBase.decode();
					allianceEntity.getAllianceBaseEntityMap().put(allianceBase.getMonsterBuilder().getMonsterId(), allianceBase);
				}
			}
		}
		
		//校验数据
		for (AllianceEntity allianceEntity : allianceMap.values()) {			
			Iterator<Map.Entry<Integer, AllianceBaseEntity>> iterator = allianceEntity.getAllianceBaseEntityMap().entrySet().iterator();
			while (iterator.hasNext()) {
				AllianceBaseEntity baseEntity = iterator.next().getValue();
				PlayerAllianceEntity playerAllianceEntity = allianceEntity.getMember(baseEntity.getPlayerId());
				if (playerAllianceEntity == null || playerAllianceEntity.getBaseMonsterInfo().get(baseEntity.getPosition()) == null) {
					baseEntity.delete(true);
					iterator.remove();
				}
			}
		}
		
		List<String> allianeNames = HawkDBManager.getInstance().query("select name from AllianceEntity");
		existName = new HashSet<>();
		if (allianeNames != null)
			existName.addAll(allianeNames);
		
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
		if(msg.getMsg() == GsConst.MsgType.PLAYER_LEVEL_UP){
			onPlayerLevelChange(msg);
			return true;
		}
		
		return super.onMessage(msg);
	}
	
	/**
	 * 线程主执行函数
	 */
	@Override
	public boolean onTick(long tickTime) {
		int nowSeconds = (int) (tickTime / 1000);
		for (AllianceEntity alliance : allianceMap.values()) {
			if (nowSeconds > alliance.getRefreshTime()) {
				alliance.dailyRefresh();
				alliance.setRefreshTime((int) (HawkTime.getNextAM0Date() / 1000));
				alliance.notifyUpdate(true);
			}

			alliance.refreshTeamEntity(nowSeconds);
		}
		
		return true;
	}

	/**
	 * 获取所有公会
	 * @return
	 */
	public ConcurrentHashMap<Integer, AllianceEntity> getAllianceMap() {
		return allianceMap;
	}

	public Set<AllianceEntity> getAllianceLevelSet() {
		return allianceLevelSet;
	}

	/**
	 * 增加公会
	 * @param allianceEntity
	 */
	public void addAlliance(AllianceEntity allianceEntity) {
		allianceMap.put(allianceEntity.getId(), allianceEntity);
	}
	
	/**
	 * 移除公会
	 * @param allianceEntity
	 */
	public void removeAlliance(AllianceEntity allianceEntity) {
		allianceMap.remove(allianceEntity.getId());
	}

	/**
	 * 增加公会排序表
	 * @param allianceEntity
	 */
	public void addAllianceForSort(AllianceEntity allianceEntity){
		synchronized (allianceLevelSet) {
			allianceLevelSet.add(allianceEntity);
		}
	}
	
	/**
	 * 删除公会排序表
	 * @param allianceEntity
	 */
	public void removeAllianceForSort(AllianceEntity allianceEntity) {
		synchronized (allianceLevelSet) {
			allianceLevelSet.remove(allianceEntity);
		}
	}
	
	/**
	 * 升级
	 * @param allianceEntity
	 */
	public void reSortAllianceForSort(AllianceEntity allianceEntity) {
		synchronized (allianceLevelSet) {
			allianceLevelSet.remove(allianceEntity);
			allianceLevelSet.add(allianceEntity);
		}
	}
	
	/**
	 * 添加工会和玩家映射
	 * @param allianceEntity
	 */
	public void addPlayerAndAllianceMap(int playerId, int allianceid) {
		playerAllianceMap.put(playerId, allianceid);
	}
	
	/**
	 * 移除工会和玩家映射
	 * @param playerId
	 */
	public void removePlayerAndAllianceMap(int playerId) {
		playerAllianceMap.remove(playerId);
	}
	
	/**
	 * 玩家是否在公会中
	 * @param playerId
	 */
	public int getPlayerAllinceId(int playerId) {
		Integer allianceId = playerAllianceMap.get(playerId);
		if (allianceId != null) {
			return allianceId;
		}
		return 0;
	}
	
	/**
	 * 通过公会Id获取公会数据
	 * @param allianceId
	 * @return
	 */
	public AllianceEntity getAlliance(int allianceId) {
		return allianceMap.get(allianceId);
	}

	/**
	 * 通过玩家id获取playerAllianceEntity
	 * @param playerId
	 * @return
	 */
	public PlayerAllianceEntity getPlayerAllianceEntity(int playerId){
		Integer allianceId = playerAllianceMap.get(playerId);
		if (allianceId != null) {
			AllianceEntity allianceEntity = allianceMap.get(allianceId);
			if (allianceEntity != null) {
				return allianceMap.get(allianceId).getMember(playerId);
			}
		}
		return null;
	}
	
	
	/**
	 * 获取所有已经存在的公会名
	 * @return
	 */
	public Set<String> getExistName() {
		return existName;
	}	
	
	/**
	 * 添加申请列表
	 */
	public void addPlayerApply(int playerId, int allianceId) {
		LinkedHashSet<Integer> allianceSet = playerApplyMap.get(playerId);
		if (allianceSet == null) {
			allianceSet = new LinkedHashSet<Integer>();
			playerApplyMap.put(playerId, allianceSet);	
		}
		 
		allianceSet.add(allianceId);
	}
	
	/**
	 * 获取申请列表
	 */
	public Set<Integer> getPlayerApplyList(int playerId) {
		return playerApplyMap.get(playerId);
	}
	
	/**
	 * 获取申请数量
	 */
	public int getPlayerApplyCount(int playerId) {
		HashSet<Integer> allianceSet = playerApplyMap.get(playerId);
		if (allianceSet != null) {
			return allianceSet.size();
		}
		
		return 0;
	}
	
	/**
	 * 删除申请列表
	 */
	public void removePlayerApply(int playerId, int allianceId) {
		HashSet<Integer> allianceSet = playerApplyMap.get(playerId);
		if (allianceSet != null) {
			allianceSet.remove(allianceId);
		}
	}
	
	/**
	 * 是否申请过公会
	 */
	public boolean isPlayerApply(int playerId, int allianceId) {
		HashSet<Integer> allianceSet = playerApplyMap.get(playerId);
		if (allianceSet != null) {
			return allianceSet.contains(allianceId);
		}
		
		return false;
	}
	
	/**
	 * 获取离线玩家公会数据
	 * @param playerId
	 * @param postion
	 */
	public PlayerAllianceEntity getOfflineAllianceEntity(int playerId) {
		List<PlayerAllianceEntity> playerEntitys = HawkDBManager.getInstance().query("from PlayerAllianceEntity where invalid = 0 and playerId = ?", playerId);
		if (playerEntitys != null && playerEntitys.size() > 0) {
			PlayerAllianceEntity playerAllianceEntity = playerEntitys.get(0);
			return playerAllianceEntity;
		}
		
		return null;
	}
	
	/**
	 * 清理玩家所有公会申请
	 * @param playerId
	 * @param postion
	 */
	public void clearPlayerApply(int playerId){
		if (playerApplyMap.get(playerId) != null) {
			for (int allianceId : playerApplyMap.get(playerId)){
				AllianceEntity allianceEntity = getAlliance(allianceId);
				if (allianceEntity != null) {
					AllianceApplyEntity applyEntity = allianceEntity.removeApply(playerId);
					if (applyEntity != null) {
						applyEntity.delete();
					}
				}
			}
			playerApplyMap.remove(playerId);
		}
	}
	
	/**
	 * 清理工会申请
	 * @param playerId
	 * @param postion
	 */
	public void clearAllianceApply(int allianceId){
		AllianceEntity allianeEntity = getAlliance(allianceId);
		if (allianeEntity != null) {
			for (AllianceApplyEntity applyEntity : allianeEntity.getApplyList().values()) {
				applyEntity.delete();
				removePlayerApply(applyEntity.getPlayerId(), allianceId);
			}
		}
		
		allianeEntity.getApplyList().clear();
	}
	
	/**
	 * 清理公会中某个玩家申请
	 * @param playerId
	 * @param postion
	 */
	public void clearAlliancePlayerApply(int allianceId, int playerId){
		AllianceEntity allianeEntity = getAlliance(allianceId);
		if (allianeEntity != null) {
			AllianceApplyEntity applyEntity = allianeEntity.removeApply(playerId);
			if (applyEntity != null) {
				applyEntity.delete();
			}
			removePlayerApply(playerId, allianceId);
		}
	}
	
	/**
	 * 玩家等级变换
	 * @param playerId
	 * @param postion
	 */
	public void onPlayerLevelChange(HawkMsg msg){
		Player player = (Player) msg.getParam(0);
		player.getPlayerData().getPlayerAllianceEntity().setLevel(player.getLevel());
		player.getPlayerData().getPlayerAllianceEntity().notifyUpdate(true);
	}
	
	/*
	 * 广播
	 * @param allianceId
	 * @param protocol
	 * @param filterId
	 */
	public void broadcastNotify(int allianceId, HawkProtocol protocol, int filterId) {
		AllianceEntity allianceEntity = allianceMap.get(allianceId);
		if (allianceEntity != null) {
			for (int playerId : allianceEntity.getMemberList().keySet()) {
				if (playerId != filterId && ServerData.getInstance().isPlayerOnline(playerId) == true) {
					memberNotify(playerId, protocol);
				}	
			}
		}
	}
	
	/*
	 * 队伍广播
	 * @param teamEntity
	 * @param protocol
	 * @param filterId
	 */
	public void broadcastNotify(AllianceTeamEntity teamEntity, HawkProtocol protocol, int filterId) {	
		if (teamEntity.getCaptain() != 0 && teamEntity.getCaptain() != filterId) {
			memberNotify(teamEntity.getCaptain(), protocol);
		}
		if (teamEntity.getMember1() != 0 && teamEntity.getMember1() != filterId) {
			memberNotify(teamEntity.getMember1(), protocol);
		}
		if (teamEntity.getMember2() != 0 && teamEntity.getMember2() != filterId) {
			memberNotify(teamEntity.getMember1(), protocol);
		}
		if (teamEntity.getMember3() != 0 && teamEntity.getMember3() != filterId) {
			memberNotify(teamEntity.getMember3(), protocol);
		}
	}
	
	/*
	 * 单播
	 * @param allianceId
	 * @param protocol
	 * @param filterId
	 */
	public void memberNotify(int playerId, HawkProtocol protocol) {
		Player member = GsApp.getInstance().queryPlayer(playerId);
		if (member != null && member.isOnline() && member.isAssembleFinish()) {
			member.sendProtocol(protocol);
		}
	}
}


