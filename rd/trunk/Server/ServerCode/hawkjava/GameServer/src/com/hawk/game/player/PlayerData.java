package com.hawk.game.player;

import java.util.Collection;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;
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
import com.hawk.game.protocol.Quest.HSQuest;
import com.hawk.game.protocol.Quest.HSQuestInfoSync;
import com.hawk.game.util.BuilderUtil;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.ProtoUtil;
import com.sun.xml.internal.stream.Entity;

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
	 * 穿戴装备列表
	 */
	private Map<Integer, Map<Integer, Long>> dressedEquipMap = null;
	
	/**
	 * 任务列表
	 */
	private Map<Integer, HSQuest> questMap = new HashMap<Integer, HSQuest>();
	
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

	/**
	 * 初始化怪物装备信息
	 * 
	 */
	public void initMonsterDressedEquip() {
		dressedEquipMap = new HashMap<>();
		for (EquipEntity equip : equipEntities) {
			if (equip.getMonsterId() != GsConst.EQUIPNOTDRESS) {
				int monsterId = equip.getMonsterId();
				ItemCfg itemcfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, equip.getItemId());
				if (itemcfg != null) {
					addMonsterEquip(monsterId, equip, itemcfg.getPart());
				}
			}
		}
	}
	
	/**
	 * 添加装备  不修改entity
	 * 
	 * @param monsterID 怪物ID
	 * @param id 装备唯一id
	 */
	public boolean addMonsterEquip(int monsterId, EquipEntity equipEntity, int part) {
		Map<Integer, Long> monsterDressedMap = null;
		if (dressedEquipMap.containsKey(monsterId)) {
			monsterDressedMap = dressedEquipMap.get(monsterId);
		}
		else {
			monsterDressedMap = new HashMap<>();
			dressedEquipMap.put(monsterId, monsterDressedMap);
		}
		
		if (monsterDressedMap.containsKey(part)) {
			return false;
		}
		
		monsterDressedMap.put(part, equipEntity.getId());
		return true;
	}
	
	/**
	 * 脱掉装备
	 * 
	 */
	public boolean removeMonsterEquip(EquipEntity equipEntity, int part) {
		Map<Integer, Long> monsterDressedMap = null;
		if (!dressedEquipMap.containsKey(equipEntity.getMonsterId())) {
			return false;
		}
		monsterDressedMap = dressedEquipMap.get(equipEntity.getMonsterId());
		
		if (monsterDressedMap.get(part) != equipEntity.getId()) {
			return false;
		}
		
		if (monsterDressedMap.size() == 0) {
			dressedEquipMap.remove(equipEntity.getMonsterId());
		}
		
		return true;
	}
	
	/**
	 * 替换装备
	 * 不修改entity
	 * 
	 */
	public boolean replaceMonsterEquip(int monsterId, EquipEntity oldEquip, EquipEntity newEquip, int part) {	
		if (newEquip.getMonsterId() != -1 || oldEquip.getMonsterId() != monsterId) {
			return false;
		}
		
		Map<Integer, Long> monsterDressedMap = null;
		if (!dressedEquipMap.containsKey(monsterId)) {
			return false;
		}
		
		monsterDressedMap = dressedEquipMap.get(monsterId);		
		if (monsterDressedMap.get(part) != oldEquip.getId()) {
			return false;
		}
		
		monsterDressedMap.put(part, newEquip.getId());
		return true;	
	}
	
	/**
	 * 指定位置是否有装备
	 * 
	 */
	public boolean isMonsterEquipOnPart(int monsterId, int part) {	
		Map<Integer, Long> monsterDressedMap = null;
		if (!dressedEquipMap.containsKey(monsterId)) {
			return false;
		}
		
		monsterDressedMap = dressedEquipMap.get(monsterId);		
		if (!monsterDressedMap.containsKey(part)) {
			return false;
		}
		
		return true;	
	}
	
	/**
	 * 指定位置是否有装备id
	 * 
	 */
	public boolean isMonsterEquipOnPart(int monsterId, int part, long id) {	
		Map<Integer, Long> monsterDressedMap = null;
		if (!dressedEquipMap.containsKey(monsterId)) {
			return false;
		}
		
		monsterDressedMap = dressedEquipMap.get(monsterId);		
		if (monsterDressedMap.get(part) != id) {
			return false;
		}
		
		return true;	
	}

	/**
	 * 获取某一个位置装备Id
	 * 
	 */
	public long getMonsterEquipIdOnPart(int monsterId, int part) {	
		Map<Integer, Long> monsterDressedMap = null;
		if (!dressedEquipMap.containsKey(monsterId)) {
			return 0;
		}
		
		monsterDressedMap = dressedEquipMap.get(monsterId);		
		if (monsterDressedMap.containsKey(part) == false) {
			return 0;
		}
		
		return monsterDressedMap.get(part);	
	}
	
	/**
	 * 获取怪兽装备列表字符串用于日志
	 * 
	 */
	public String monsterEquipsToString(int monsterId) {
		StringBuilder builder = new StringBuilder();
		Map<Integer, Long> monsterDressedMap = dressedEquipMap.get(monsterId);
		for (Map.Entry<Integer, Long> entry : monsterDressedMap.entrySet()) {
			builder.append(String.format("%d : %lld; ", entry.getKey(), entry.getValue()));
		}
		return builder.toString();
	}

	/**
	 * 获取达到某等级怪物数量
	 */
	public int getMonsterCountOverLevel(int level) {
		int count = 0;
		for (Entry<Integer,MonsterEntity> entry : monsterEntityList.entrySet()) {
			if (entry.getValue().getLevel() >= level) {
				++count;
			}
		}
		return count;
	}
	
	/**
	 * 获取达到某品级怪物数量
	 */
	public int getMonsterCountOverStage(int stage) {
		int count = 0;
		for (Entry<Integer,MonsterEntity> entry : monsterEntityList.entrySet()) {
			if (entry.getValue().getStage() >= stage) {
				++count;
			}
		}
		return count;
	}
	
	/**
	 * 获取当前任务列表
	 */
	public Map<Integer, HSQuest> getQuestMap() {
		 return questMap;
	}

	public void setQuest(HSQuest quest) {
		questMap.put(quest.getQuestId(), quest);
	}

	public HSQuest getQuest(int questId){
		return questMap.get(questId);
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
				playerEntity.assemble();
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
		if (monsterEntitys != null && monsterEntitys.size() > 0) {
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
		
		if (itemEntities != null && itemEntities.size() > 0) {
			for (ItemEntity itemEntity : itemEntities) {
				itemEntity.assemble();
			}
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
		
		if (equipEntities != null && equipEntities.size() > 0) {
			for (EquipEntity equipEntity : equipEntities) {
				equipEntity.assemble();
			}
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
		player.sendProtocol(protocol);
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
	
	/**
	 * 同步任务信息
	 */
	public void syncQuestInfo() {
		HSQuestInfoSync.Builder builder = HSQuestInfoSync.newBuilder();

		for (Entry<Integer, HSQuest> entry : questMap.entrySet()) {
			builder.addQuestInfo(entry.getValue());
		}

		HawkProtocol protocol = HawkProtocol.valueOf(HS.code.QUEST_INFO_SYNC_S, builder);
		player.sendProtocol(protocol);
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
