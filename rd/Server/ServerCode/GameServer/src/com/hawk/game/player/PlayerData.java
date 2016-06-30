package com.hawk.game.player;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.hawk.db.HawkDBManager;
import org.hawk.os.HawkTime;

import com.hawk.game.ServerData;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.PlayerEntity;
import com.hawk.game.entity.RoleEntity;

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
	 * 角色基础数据
	 */
	private RoleEntity roleEntity = null;

	private Map<Integer, MonsterEntity> monsterEntityMap = new HashMap<Integer, MonsterEntity>();

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

	/**
	 * 获取玩家数据实体
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
	 * 获取角色数据实体
	 * 
	 * @return
	 */
	public RoleEntity getRoleEntity() {
		return roleEntity;
	}

	/**
	 * 设置角色数据实体
	 * 
	 * @param playerEntity
	 */
	public void setRoleEntity(RoleEntity roleEntity){
		this.roleEntity = roleEntity;
	}

	/**
	 * 获取玩家当前角色的怪物数据实体
	 * 
	 * @return
	 */
	public Map<Integer, MonsterEntity> getMonsterEntity() {
		 return monsterEntityMap;
	}
	public MonsterEntity getMonsterEntity(int monsterId) {
		return monsterEntityMap.get(monsterId);
	}

	/**
	 * 设置玩家当前角色的怪物数据实体
	 * 
	 * @param playerEntity
	 */
	public void setMonsterEntity(MonsterEntity monsterEntity) {
		this.monsterEntityMap.put(monsterEntity.getId(), monsterEntity);
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
				try {
					if (playerEntity.getSilentTime() != null && playerEntity.getSilentTime().getTime() > HawkTime.getMillisecond()) {
						ServerData.getInstance().addDisablePhone(playerEntity.getPhoneInfo());
					}
				} catch (Exception e) {
				}
			}
		}
		return playerEntity;
	}
	
	/**
	 * 加载角色信息
	 * 
	 * @return
	 */
	public RoleEntity loadRole(int roleId) {	
		List<RoleEntity> roleEntitys = HawkDBManager.getInstance().query("from RoleEntity where id = ? and invalid = 0", roleId);
		if (roleEntitys != null && roleEntitys.size() > 0) {
			RoleEntity roleEntity = roleEntitys.get(0);
			return roleEntity;
		}
		return null;
	}


	/**
	 * 加载怪物信息
	 * 
	 * @return
	 */
	private void loadMonster() {
		if (monsterEntityMap.isEmpty() == true) {
			// TODO: roleId
			List<MonsterEntity> monsterEntitys = HawkDBManager.getInstance().query("from MonsterEntity where roleId = ? and invalid = 0", 1);
			if (monsterEntitys != null && monsterEntitys.size() > 0) {
				for (MonsterEntity monsterEntity : monsterEntitys) {
					monsterEntityMap.put(monsterEntity.getId(), monsterEntity);
				}
			}
		}
	}
}
