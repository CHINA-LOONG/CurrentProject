package com.hawk.game.manager;

import java.util.List;

import org.hawk.app.HawkAppObj;
import org.hawk.db.HawkDBManager;
import org.hawk.log.HawkLog;
import org.hawk.msg.HawkMsg;
import org.hawk.os.HawkException;
import org.hawk.xid.HawkXID;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.googlecode.concurrentlinkedhashmap.ConcurrentLinkedHashMap;
import com.hawk.game.GsApp;
import com.hawk.game.ServerData;
import com.hawk.game.config.SysBasicCfg;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.entity.PlayerEntity;
import com.hawk.game.entity.SnapshotEntity;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Snapshot.SnapshotInfo;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.SnapshotUtil;

/**
 * 玩家数据缓存管理器
 * 
 */
public class SnapShotManager extends HawkAppObj {
	/**
	 *  快照管理器Tick周期时间
	 */
	public static int tickPeriodTime = 60 * 1000;
	private long lastTickTime = 0;
	
	/**
	 * 日志对象
	 */
	public static Logger logger = LoggerFactory.getLogger("Server");
	
	/**
	 * 全局对象, 便于访问
	 */
	private static SnapShotManager instance = null;

	private ConcurrentLinkedHashMap<Integer, SnapshotEntity> playerSnapShotMap;

	/**
	 * 获取全局实例对象
	 */
	public static SnapShotManager getInstance() {
		return instance;
	}

	public SnapShotManager(HawkXID xid) {
		super(xid);

		if (instance == null) {
			instance = this;
		}
	}
	
	public boolean init() {
		ConcurrentLinkedHashMap.Builder<Integer, SnapshotEntity> mapBuilder = new ConcurrentLinkedHashMap.Builder<Integer, SnapshotEntity>();
		mapBuilder.maximumWeightedCapacity(SysBasicCfg.getInstance().getMaxPlayerSnapShotQty());
		playerSnapShotMap = mapBuilder.build();
		return true;
	}

	@Override
	public boolean onTick(long tickTime){
		if(tickTime - lastTickTime >= tickPeriodTime){
			logger.info("snapshot resource usage : {}/{}", playerSnapShotMap.size(), SysBasicCfg.getInstance().getMaxPlayerSnapShotQty());
			lastTickTime = tickTime;
		}
		return true;
	}
	
	/**
	 * 快照管理器消息处理
	 */
	@Override
	public boolean onMessage(HawkMsg msg) {
		if(msg.getMsg() == GsConst.MsgType.ONLINE_REMOVE_OFFLINE_SNAPSHOT){
			onRemoveOfflineSnapshot(msg);
			return true;
		}
		return false;
	}
	
	/**
	 * 生成在线玩家快照对象
	 * @param playerId
	 * @return
	 */
	private SnapshotInfo.Builder getOnlinePlayerSnapshotById(int playerId) {
		try {
			Player player = GsApp.getInstance().queryPlayer(playerId);
			if(player != null && (!player.isOnline() || player.isAssembleFinish())) {
				return player.getPlayerData().getOnlinePlayerSnapshot(false);
			} else {
				HawkLog.errPrintln("query snapshot player null: " + playerId);
			}
		} 
		catch (Exception e) {
			HawkException.catchException(e);
		}
		
		return null;
	}
	
	/**
	 * 创建离线玩家快照实体
	 * @param playerId
	 * @return
	 */
	private SnapshotEntity createOfflineSnapshotEntity(int playerId) {
		SnapshotEntity snapshotEntity = new SnapshotEntity();
		snapshotEntity.setPlayerId(playerId);
		
		// 构造builder
		SnapshotInfo.Builder snapshotInfo = SnapshotInfo.newBuilder();
		
		// 拉取玩家信息
		PlayerEntity playerEntity = loadPlayer(playerId);
		if(playerEntity != null){
			snapshotInfo.setPlayerId(playerId);
			
			SnapshotUtil.attachPlayerInfo(snapshotInfo, playerEntity);
			
			// 拉起玩家工会信息
			PlayerAllianceEntity allianceEntity = loadPlayerAllianceEntity(playerId);
			if(allianceEntity != null){
				SnapshotUtil.attachAllianceInfo(snapshotInfo, allianceEntity);
			}
		
			return snapshotEntity;
		}
		return null;
	}

	/**
	 * 从数据库快照表中拉取玩家快照数据
	 * @param playerId
	 * @return
	 */
	private SnapshotEntity loadOnePlayerSnapshotDBData(int playerId){
		List<SnapshotEntity> snapshotEntities = HawkDBManager.getInstance().query("from SnapshotEntity where playerId = ?", playerId);
		if (snapshotEntities != null && snapshotEntities.size() > 0) {
			SnapshotEntity snapshotEntity = snapshotEntities.get(0);
			snapshotEntity.decode();
			return snapshotEntity;
		}
		return null;
	}
		
	
	/**
	 * 加载玩家信息
	 * 
	 * @return
	 */
	private PlayerEntity loadPlayer(int playerId) {
		PlayerEntity playerEntity = null;
		List<PlayerEntity> playerEntities = HawkDBManager.getInstance().query("from PlayerEntity where id = ?", playerId);
		if (playerEntities != null && playerEntities.size() > 0) {
			playerEntity = playerEntities.get(0);
		}
		return playerEntity;
	}
	
	/**
	 * 加载玩家工会信息
	 */
	private PlayerAllianceEntity loadPlayerAllianceEntity(int playerId){
		List<PlayerAllianceEntity> playerAlliances = HawkDBManager.getInstance().query("from PlayerAllianceEntity where playerId = ?", playerId);
		if(playerAlliances.size() > 0){
			return playerAlliances.get(0);
		}
		return null;
	
	}
	
	/**
	 * 删除在线玩家的离线快照数据
	 */
	private void onRemoveOfflineSnapshot(HawkMsg msg){
		int onlinePlayerId = msg.getParam(0);
		removePlayerSnapShot(onlinePlayerId);
	}
	
	/**
	 * 获取玩家快照信息，如果返回null则玩家不存在
	 * 
	 * @param playerId
	 * @return
	 */
	public SnapshotInfo.Builder getPlayerSnapShot(int playerId) {
		boolean isOnline = ServerData.getInstance().isPlayerOnline(playerId);
		if (isOnline) {
			// 在线玩家直接获取
			SnapshotInfo.Builder snapshotInfo = getOnlinePlayerSnapshotById(playerId);
			if(snapshotInfo != null){
				return snapshotInfo;
			}
		}
		
		// 如果不在线或者在线玩家获取失败，检测离线快照缓存中是否存在
		SnapshotEntity playerSnapshotEntity = playerSnapShotMap.get(playerId);
		if(playerSnapshotEntity != null){
			return playerSnapshotEntity.getSnapshotInfo();
		}
		
		// 如果缓存中不存在则数据库拉取，并加入快照缓存
		SnapshotEntity snapshotEntity = loadOnePlayerSnapshotDBData(playerId);
		if (snapshotEntity != null){
			// 拉取快照表，存在
			playerSnapShotMap.put(snapshotEntity.getPlayerId(), snapshotEntity);
			return snapshotEntity.getSnapshotInfo();
		}
		
		// 快照表中不存在，则分别在各个表中拉取所需数据
		snapshotEntity = createOfflineSnapshotEntity(playerId);
		if (snapshotEntity != null) {
			if(HawkDBManager.getInstance().create(snapshotEntity)){
				playerSnapShotMap.put(snapshotEntity.getPlayerId(), snapshotEntity);
			}
			return snapshotEntity.getSnapshotInfo();
		}
		
		// 数据库查询不到，说明用户不存在
		return null;
	}

	/**
	 * 缓存玩家快照
	 * @param player
	 * @return
	 */
	public boolean cacheSnapshot(int playerId, SnapshotInfo.Builder playerSnapshotInfo){
		SnapshotEntity snapshotEntity = new SnapshotEntity();
		snapshotEntity.setPlayerId(playerId);
		snapshotEntity.setSnapshotInfo(playerSnapshotInfo);
		// 保存数据到内存和数据库
		playerSnapShotMap.put(playerId, snapshotEntity);
		snapshotEntity.notifyUpdate(true);
		return true;
	}
	

	/**
	 * 修改离线玩家工会快照数据
	 */
	public boolean UpdateOfflineSnapshotAllianceData(PlayerAllianceEntity allianceEntity){
		int offlinePlayerId = allianceEntity.getPlayerId();
		if(!ServerData.getInstance().isPlayerOnline(offlinePlayerId)){
			if(playerSnapShotMap.containsKey(offlinePlayerId)){
				SnapshotEntity snapshotEntity = playerSnapShotMap.get(offlinePlayerId);
				SnapshotInfo.Builder playerSnapshotInfo = snapshotEntity.getSnapshotInfo();
				SnapshotUtil.attachAllianceInfo(playerSnapshotInfo, allianceEntity);
				snapshotEntity.notifyUpdate(true);
			}
		}
		return true;
	}
	
	/**
	 * 删除指定玩家的快照缓存数据
	 * @param playerId
	 */
	public void removePlayerSnapShot(int playerId){
		playerSnapShotMap.remove(playerId);
	}
}
