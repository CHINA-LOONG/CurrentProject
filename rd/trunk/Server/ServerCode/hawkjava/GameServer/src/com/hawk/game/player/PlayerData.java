package com.hawk.game.player;

import java.util.HashMap;
import java.util.LinkedList;
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
	 * 当前选择角色基础数据
	 */
	private RoleEntity selectRoleEntity = null;

	/**
	 * 角色列表数据
	 */
	private List<RoleEntity> roleEntities = new LinkedList<RoleEntity>();
	
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
	public RoleEntity getSelectRoleEntity() {
		return selectRoleEntity;
	}

	/**
	 * 设置角色数据实体
	 * 
	 * @param playerEntity
	 */
	public void setRoleEntity(RoleEntity roleEntity){
		this.selectRoleEntity = roleEntity;
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
	 * 加载所有角色信息
	 * 
	 * @return
	 */
	public List<RoleEntity> loadAllRoles() {	
		roleEntities = HawkDBManager.getInstance().query("from RoleEntity where playerId = ? and invalid = 0 order by id asc ", playerEntity.getId());
		return roleEntities;
	}
	
	/**
	 * 加载角色信息
	 * 
	 * @return
	 */
	public boolean selectRole(int roleId) {
		selectRoleEntity = null;
		
		for (RoleEntity roleEntity : roleEntities) {
			if (roleId == roleEntity.getRoleID()) {
				selectRoleEntity = roleEntity;
			}
		}
		
		if (selectRoleEntity == null) {
			return false;
		}
		
		loadMonster(roleId);
		return true;
	}

	/**
	 * 加载怪物信息
	 * 
	 * @return
	 */
	private void loadMonster(int roleId) {	
		monsterEntities.clear();
		List<MonsterEntity> monsterEntityList = HawkDBManager.getInstance().query("from MonsterEntity where roleId = ? and invalid = 0", roleId);
		if (monsterEntityList != null && monsterEntityList.size() > 0) {
			for (MonsterEntity monsterEntity : monsterEntityList) {
				monsterEntities.put(monsterEntity.getId(), monsterEntity);
			}
		}
	}
}
