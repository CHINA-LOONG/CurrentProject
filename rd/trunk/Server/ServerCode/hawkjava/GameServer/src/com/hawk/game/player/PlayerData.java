package com.hawk.game.player;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;

import org.hawk.config.HawkConfigManager;
import org.hawk.db.HawkDBManager;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkTime;

import com.hawk.game.ServerData;
import com.hawk.game.config.ItemCfg;
import com.hawk.game.entity.EquipEntity;
import com.hawk.game.entity.ItemEntity;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.PlayerEntity;
import com.hawk.game.entity.StatisticsEntity;
import com.hawk.game.protocol.Equip.EquipInfo;
import com.hawk.game.protocol.Equip.HSEquipInfoSync;
import com.hawk.game.protocol.Item.HSItemInfoSync;
import com.hawk.game.protocol.Monster.HSMonsterInfoSync;
import com.hawk.game.protocol.Player.HSPlayerInfoSync;
import com.hawk.game.protocol.HS;
import com.hawk.game.util.BuilderUtil;
import com.hawk.game.util.ProtoUtil;

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
	private String puid = null;
	/**
	 * 玩家对象
	 */
	private Player player = null;
	
	/**
	 * 玩家基础数据
	 */
	private PlayerEntity playerEntity = null;

	/**
	 * 玩家统计数据
	 */
	private StatisticsEntity statisticsEntity = null;

	/**
	 * 怪的基础数据
	 */
	private Map<Integer, MonsterEntity> monsterEntityList = new HashMap<Integer, MonsterEntity>();


	/**
	 * 物品列表
	 */
	private List<ItemEntity> itemEntities = null;

	/**
	 * 装备列表
	 */
	private List<EquipEntity> equipEntities = null;

	/**
	 * 构造函数
	 * 
	 * @param player
	 */
	public PlayerData(Player player) {
		this.player = player;
	}

	/**
	 * 设置玩家puid
	 * 
	 * @return
	 */
	public void setPuid(String puid) {
		this.puid = puid;
	}

	/**
	 * 获取玩家puid
	 * 
	 * @return
	 */
	public String getPuid() {
		return this.puid;
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
	public Map<Integer, MonsterEntity> getMonsterEntityList() {
		 return monsterEntityList;
	}

	/**
	 * 设置玩家当前角色的怪物数据实体
	 * 
	 * @param
	 */
	public void setMonsterEntity(MonsterEntity monsterEntity) {
		monsterEntityList.put(monsterEntity.getId(), monsterEntity);
	}

	public MonsterEntity getMonsterEntity(int monsterId){
		return monsterEntityList.get(monsterId);
	}

	/**
	 * 清空当前角色的怪物数据实体
	 * 
	 * @param playerEntity
	 */
	public void clearMonsterEntity() {
		this.monsterEntityList.clear();
	}

	public StatisticsEntity getStatisticsEntity() {
		return statisticsEntity;
	}

	/**********************************************************************************************************
	 * 数据db操作区
	 **********************************************************************************************************/
	/**
	 * 加载玩家信息
	 * 
	 * @return
	 */
	public PlayerEntity loadPlayer() {	
		if (this.puid == null) {
			return null;
		}
		
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
	 * 加载玩家统计信息
	 */
	public StatisticsEntity loadStatistics() {
		if (statisticsEntity == null) {
			List<StatisticsEntity> statisticsEntitys = HawkDBManager.getInstance().query("from StatisticsEntity where playerId = ? and invalid = 0", getId());
			if (statisticsEntitys != null && statisticsEntitys.size() > 0) {
				statisticsEntity = statisticsEntitys.get(0);

				statisticsEntity.assemble();

//				// 新号上报数据
//				if (statisticsEntity.getPlatformData() != null && statisticsEntity.getPlatformData().indexOf("65535") > 0) {
//					GsApp.getInstance().reportCmActivePlayer(player);
//				}
			} else {
				statisticsEntity = new StatisticsEntity(getId());
				statisticsEntity.notifyCreate();

//				// 新号上报数据
//				GsApp.getInstance().reportCmActivePlayer(player);
			}
		}
		return statisticsEntity;
	}

	/**
	 * 加载怪物信息
	 * 
	 * @return
	 */
	public void loadAllMonster() {
		monsterEntityList.clear();
		List<MonsterEntity> monsterEntitys = HawkDBManager.getInstance().query("from MonsterEntity where playerId = ? and invalid = 0", getId());
		if (monsterEntityList != null && monsterEntityList.size() > 0) {
			for (MonsterEntity monsterEntity : monsterEntitys) {
				monsterEntity.assemble();
				monsterEntityList.put(monsterEntity.getId(), monsterEntity);
			}
		}
	}

	/**
	 * 加载物品信息
	 * 
	 * @return
	 */
	public List<ItemEntity> loadItemEntities() {
		if (itemEntities == null) {
			itemEntities = HawkDBManager.getInstance().query("from ItemEntity where playerId = ? and invalid = 0 order by id asc", playerEntity.getId());
		}
		return itemEntities;
	}

	/**
	 * 加载装备信息
	 * 
	 * @return
	 */
	public List<EquipEntity> loadEquipEntities() {
		if (equipEntities == null) {
			equipEntities = HawkDBManager.getInstance().query("from EquipEntity where playerId = ? and invalid = 0 order by id asc", playerEntity.getId());
		}
		return equipEntities;
	}

	/**********************************************************************************************************
	 * 数据同步区
	 **********************************************************************************************************/

	/**
	 * 同步玩家信息
	 */
	public void syncPlayerInfo() {
		HSPlayerInfoSync.Builder builder = HSPlayerInfoSync.newBuilder();
		builder.setInfo(BuilderUtil.genPlayerBuilder(playerEntity));
		player.sendProtocol(HawkProtocol.valueOf(HS.code.PLAYER_INFO_SYNC_S, builder));
	}

	/**
	 * 同步玩家统计信息
	 */
	public void syncStatisticsInfo() {
		player.sendProtocol(HawkProtocol.valueOf(HS.code.STATISTICS_INFO_SYNC_S, BuilderUtil.genStatisticsBuilder(statisticsEntity)));
	}

	/**
	 * 同步怪物信息(0表示同步所有)
	 */
	public void syncMonsterInfo(int id) {
		HSMonsterInfoSync.Builder builder = HSMonsterInfoSync.newBuilder();

		if (id == 0) {
			for (Entry<Integer, MonsterEntity> entry : monsterEntityList.entrySet()) {
				builder.addMonsterInfo(BuilderUtil.genMonsterBuilder(entry.getValue()));
			}
		} else {
			builder.addMonsterInfo(BuilderUtil.genMonsterBuilder(monsterEntityList.get(id)));
		}

		HawkProtocol protocol = HawkProtocol.valueOf(HS.code.MONSTER_INFO_SYNC_S, builder);
		if (id == 0) {
			player.sendProtocol(ProtoUtil.compressProtocol(protocol));
		} else {
			player.sendProtocol(protocol);
		}
	}


	/**
	 * 同步物品信息
	 */
	public void syncItemInfo(int... ids) {
		HSItemInfoSync.Builder builder = HSItemInfoSync.newBuilder();
		for (Integer id : ids) {
			for (ItemEntity itemEntity : itemEntities) {
				if ((id == 0 || id == itemEntity.getId()) && itemEntity.getCount() > 0 && !itemEntity.isInvalid()) {
					builder.addItemInfos(BuilderUtil.genItemBuilder(itemEntity));
				}
			}
		}
		HawkProtocol protocol = HawkProtocol.valueOf(HS.code.ITEM_INFO_SYNC_S, builder);
		player.sendProtocol(protocol);
	}

	/**
	 * 同步物品信息
	 */
	public void syncItemInfo() {
		HSItemInfoSync.Builder builder = HSItemInfoSync.newBuilder();
		for (ItemEntity itemEntity : itemEntities) {
			if (itemEntity.getCount() > 0 && !itemEntity.isInvalid()) {
				ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, itemEntity.getItemId());
				if (itemCfg == null) {
					continue;
				}
				builder.addItemInfos(BuilderUtil.genItemBuilder(itemEntity));
			}
		}
		HawkProtocol protocol = HawkProtocol.valueOf(HS.code.ITEM_INFO_SYNC_S, builder);
		player.sendProtocol(protocol);
	}

	/**
	 * 同步装备信息
	 */
	public void syncEquipInfo() {
		HSEquipInfoSync.Builder builder = HSEquipInfoSync.newBuilder();
		for (EquipEntity equipEntity : equipEntities) {
			if (!equipEntity.isInvalid()) {
				builder.addEquipInfos(BuilderUtil.genEquipBuilder(equipEntity));
			}
		}

		if (builder.getEquipInfosCount() > 0) {
			player.sendProtocol(HawkProtocol.valueOf(HS.code.EQUIP_INFO_SYNC_S, builder));
		}
	}

	/**********************************************************************************************************
	 * 数据查询区
	 **********************************************************************************************************/
	/**
	 * 获取物品
	 * 
	 * @return
	 */
	public ItemEntity getItemById(int id) {
		for (ItemEntity itemEntity : itemEntities) {
			if (id == itemEntity.getId()) {
				return itemEntity;
			}
		}
		return null;
	}

	/**
	 * 获取物品
	 * 
	 * @return
	 */
	public ItemEntity getItemByItemId(int itemId) {
		for (ItemEntity itemEntity : itemEntities) {
			if (itemId == itemEntity.getItemId()) {
				return itemEntity;
			}
		}
		return null;
	}

	/**
	 * 增加物品实体
	 * 
	 * @return
	 */
	public void addItemEntity(ItemEntity itemEntity) {
		itemEntities.add(itemEntity);
	}

	/**
	 * 获取装备列表
	 * 
	 * @return
	 */
	public List<EquipEntity> getEquipEntities() {
		return equipEntities;
	}

	/**
	 * 获取装备
	 * 
	 * @return
	 */
	public EquipEntity getEquipById(long id) {
		if (equipEntities != null) {
			for (EquipEntity equipEntity : equipEntities) {
				if (id == equipEntity.getId()) {
					return equipEntity;
				}
			}
		}
		return null;
	}

	/**
	 * 增加装备实体
	 * 
	 * @return
	 */
	public void addEquipEntity(EquipEntity equipEntity) {
		equipEntities.add(equipEntity);
	}

	/**
	 * 移除装备数据
	 * 
	 * @param equipEntity
	 */
	public void removeEquipEntity(EquipEntity equipEntity) {
		equipEntities.remove(equipEntity);
	}
}
