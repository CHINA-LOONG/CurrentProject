package com.hawk.game.player;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.hawk.db.HawkDBManager;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkTime;

import com.hawk.game.ServerData;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.PlayerEntity;
import com.hawk.game.protocol.Player.HSPlayerInfoSync;
import com.hawk.game.protocol.HS;
import com.hawk.game.util.BuilderUtil;

/**
 * 管理所有玩家数据集合
 * 
 * @author hawk
 * 
 */
public class PlayerData {
	/**
	 * 玩家对象
	 */
	private Player player = null;
	
	/**
	 * 玩家基础数据
	 */
	private PlayerEntity playerEntity = null;

	/**
	 * 怪的基础数据
	 */
	private Map<Integer, MonsterEntity> monsterEntities = new HashMap<Integer, MonsterEntity>();

	/**
	 * 构造函数
	 * 
	 * @param player
	 */
	public PlayerData(Player player) {
		this.player = player;
	}

	/**
	 * 获取数据对应玩家对象
	 * 
	 * @return
	 */
	public Player getPlayer() {
		return player;
	}

	/**********************************************************************************************************
	 * 数据查询区
	 **********************************************************************************************************/
	/**
	 * 获取玩家ID
	 * 
	 * @return
	 */
	public int getId() {
		return playerEntity.getId();
	}

	/**
	 * 获取玩家基础数据
	 * 
	 * @return
	 */
	public PlayerEntity getPlayerEntity() {
		return playerEntity;
	}
	
	/**
	 * 设置玩家数据实体
	 * 
	 * @param playerEntity
	 */
	public void setPlayerEntity(PlayerEntity playerEntity) {
		this.playerEntity = playerEntity;
	}

	/**
	 * 获取玩家当前角色的怪物数据实体
	 * 
	 * @return
	 */
	public Map<Integer, MonsterEntity> getMonsterEntities() {
		 return monsterEntities;
	}
	

	/**
	 * 设置玩家当前角色的怪物数据实体
	 * 
	 * @param playerEntity
	 */
	public void setMonsterEntity(MonsterEntity monsterEntity) {
		monsterEntities.put(monsterEntity.getId(), monsterEntity);
	}

	public MonsterEntity getMonsterEntity(int monsterID){
		return monsterEntities.get(monsterID);
	}
	
	/**
	 * 清空当前角色的怪物数据实体
	 * 
	 * @param playerEntity
	 */
	public void clearMonsterEntity() {
		this.monsterEntities.clear();
	}

	/**********************************************************************************************************
	 * 数据db操作区
	 **********************************************************************************************************/
	/**
	 * 加载玩家信息
	 * 
	 * @return
	 */
	public PlayerEntity loadPlayer(String puid) {
		if (playerEntity == null) {
			List<PlayerEntity> playerEntitys = HawkDBManager.getInstance().query("from PlayerEntity where puid = ? and invalid = 0", puid);
			if (playerEntitys != null && playerEntitys.size() > 0) {
				playerEntity = playerEntitys.get(0);
//				try {
//					if (playerEntity.getSilentTime() != null && playerEntity.getSilentTime().getTime() > HawkTime.getMillisecond()) {
//						ServerData.getInstance().addDisablePhone(playerEntity.getPhoneInfo());
//					}
//				} catch (Exception e) {
//				}
			}
		}
		return playerEntity;
	}
	
	/**
	 * 加载怪物信息
	 * 
	 * @return
	 */
	public void loadAllMonsters(int playerID) {	
		monsterEntities.clear();
		List<MonsterEntity> monsterEntityList = HawkDBManager.getInstance().query("from MonsterEntity where roleId = ? and invalid = 0", playerID);
		if (monsterEntityList != null && monsterEntityList.size() > 0) {
			for (MonsterEntity monsterEntity : monsterEntityList) {
				monsterEntities.put(monsterEntity.getId(), monsterEntity);
			}
		}
	}
	
	/**********************************************************************************************************
	 * 数据同步区
	 **********************************************************************************************************/
	
	/**
	 * 同步玩家信息
	 */
	public void syncPlayerInfo() {
		HSPlayerInfoSync.Builder builder = HSPlayerInfoSync.newBuilder();
		builder.addPlayerInfos(BuilderUtil.genPlayerBuilder(playerEntity));
		player.sendProtocol(HawkProtocol.valueOf(HS.code.PLAYER_INFO_SYNC_S, builder));
	}

	/**
	 * 同步角色信息(0表示同步所有)
	 * 
	 * @param roleId
	 */
	public void syncRoleInfo(int id) {

	}
}
