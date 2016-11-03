package com.hawk.game.player;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;

import org.hawk.config.HawkConfigManager;
import org.hawk.db.HawkDBManager;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkException;

import com.hawk.game.config.ItemCfg;
import com.hawk.game.entity.AdventureEntity;
import com.hawk.game.entity.AdventureTeamEntity;
import com.hawk.game.entity.EquipEntity;
import com.hawk.game.entity.ItemEntity;
import com.hawk.game.entity.MailEntity;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.PVPDefenceEntity;
import com.hawk.game.entity.PVPDefenceRecordEntity;
import com.hawk.game.entity.PVPRankEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.entity.PlayerEntity;
import com.hawk.game.entity.ShopEntity;
import com.hawk.game.entity.statistics.StatisticsEntity;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.manager.PVPManager;
import com.hawk.game.protocol.Adventure.HSAdventureInfoSync;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.Equip.HSEquipInfoSync;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Item.HSItemInfoSync;
import com.hawk.game.protocol.Mail.HSMailInfoSync;
import com.hawk.game.protocol.Monster.HSMonster;
import com.hawk.game.protocol.Monster.HSMonsterInfoSync;
import com.hawk.game.protocol.PVP.HSGetPVPDefenceMonsterRet;
import com.hawk.game.protocol.Player.HSPlayerInfoSync;
import com.hawk.game.protocol.Quest.HSQuest;
import com.hawk.game.protocol.Quest.HSQuestInfoSync;
import com.hawk.game.protocol.Setting.HSSetting;
import com.hawk.game.protocol.Setting.HSSettingInfoSync;
import com.hawk.game.protocol.Snapshot.SnapshotInfo;
import com.hawk.game.util.BuilderUtil;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.ShopUtil;
import com.hawk.game.util.SnapshotUtil;

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
	private Map<Integer, MonsterEntity> monsterEntityMap = null;

	/**
	 * 物品列表
	 */
	private Map<String, ItemEntity> itemEntityMap = null;

	/**
	 * 装备列表
	 */
	private Map<Long, EquipEntity> equipEntityMap = null;

	/**
	 * 玩家数据快照
	 */
	private SnapshotInfo.Builder onlinePlayerSnapshot = null;

	/**
	 * 穿戴装备列表
	 */
	private Map<Integer, Map<Integer, Long>> dressedEquipMap = null;

	/**
	 * 邮件列表
	 */
	private List<MailEntity> mailEntityList = null;

	/**
	 * 任务列表
	 */
	private Map<Integer, HSQuest> questMap = new HashMap<>();

	/**
	 * 角色商城数据
	 */
	private Map<Integer, ShopEntity> shopEntityMap = null;

	/**
	 * 公会基本信息
	 */
	private PlayerAllianceEntity playerAllianceEntity = null;

	/**
	 * 大冒险列表
	 */
	private List<AdventureEntity> adventureEntityList = null;

	/**
	 * 大冒险队伍列表
	 */
	private Map<Integer, AdventureTeamEntity> adventureTeamEntityMap = null;

	/**
	 * pvp 防守阵容数据
	 */
	private PVPDefenceEntity pvpDefenceEntity = null;

	/**
	 * pvp 防守记录
	 */
	private LinkedList<PVPDefenceRecordEntity> pvpDefenceRecordEntityList = new LinkedList<PVPDefenceRecordEntity>();

	/**
	 * 刷新时间缓存（毫秒）
	 */
	private long[] refreshTimeCache = new long[GsConst.Refresh.PlayerTimePointArray.length];

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
	 */
	public int getId() {
		return playerEntity.getId();
	}

	/**
	 * 获取玩家昵称
	 */
	public String getNickname() {
		return playerEntity.getNickname();
	}

	/**
	 * 获取玩家等级
	 */
	public int getLevel() {
		return playerEntity.getLevel();
	}

	/**
	 * 获取玩家基础数据
	 */
	public PlayerEntity getPlayerEntity() {
		return playerEntity;
	}

	/**
	 * 设置玩家数据实体
	 */
	public void setPlayerEntity(PlayerEntity playerEntity) {
		this.playerEntity = playerEntity;
	}

	/**
	 * 获取当前角色的怪物数据实体列表
	 */
	public Map<Integer, MonsterEntity> getMonsterEntityMap() {
		 return monsterEntityMap;
	}

	/**
	 * 获取当前角色的怪物数据实体
	 */
	public MonsterEntity getMonsterEntity(int monsterId){
		return monsterEntityMap.get(monsterId);
	}

	/**
	 * 设置当前角色的怪物数据实体
	 */
	public void setMonsterEntity(MonsterEntity monsterEntity) {
		monsterEntityMap.put(monsterEntity.getId(), monsterEntity);
	}

	/**
	 * 移除当前角色的怪物数据实体
	 */
	public boolean removeMonsterEntity(int monsterId) {
		if (null == monsterEntityMap.remove(monsterId)) {
			return false;
		}
		return true;
	}

	/**
	 * 清空当前角色的怪物数据实体
	 */
	public void clearMonsterEntity() {
		this.monsterEntityMap.clear();
	}

	/**
	 * 获取商店实体对象
	 * @return
	 */
	public ShopEntity getShopEntity(int type) {
		// 避免登陆调用refresh时数据没有assemble完成
		if (shopEntityMap == null) {
			loadShop();
		}

		return shopEntityMap.get(type);
	}

	/**
	 * 获取玩家公会实体
	 * 
	 * @return
	 */
	public PlayerAllianceEntity getPlayerAllianceEntity() {
		return playerAllianceEntity;
	}

	/**
	 * 获取当前角色的统计数据实体
	 */
	public StatisticsEntity getStatisticsEntity() {
		return statisticsEntity;
	}

	/**
	 * 初始化怪物装备信息
	 */
	public void initMonsterDressedEquip() {
		dressedEquipMap = new HashMap<>();
		for (Map.Entry<Long, EquipEntity> entry : equipEntityMap.entrySet()) {
			if (entry.getValue().getMonsterId() != GsConst.EQUIP_NOT_DRESS) {
				int monsterId = entry.getValue().getMonsterId();
				ItemCfg itemcfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, entry.getValue().getItemId());
				if (itemcfg != null) {
					addMonsterEquip(monsterId, entry.getValue(), itemcfg.getPart());
				}
			}
		}
	}

	/**
	 * 获取某件怪物身上所有装备
	 */
	public Map<Integer, Long> getMonsterEquips(int monsterId) {
		return dressedEquipMap.get(monsterId);
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

		monsterDressedMap.remove(part);

		if (monsterDressedMap.isEmpty() == true) {
			dressedEquipMap.remove(equipEntity.getMonsterId());
		}

		return true;
	}

	/**
	 * 替换装备
	 * 不修改entity
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
	 * 怪物是否在公会基地中
	 * @param monsterId
	 * @return
	 */
	public boolean isMonsterInBase(int monsterId){
		if (playerAllianceEntity.getAllianceId() != 0) {
			return playerAllianceEntity.ismonsterSendtoBase(monsterId);
		}

		return false;
	}

	/**
	 * 获取某一个位置装备Id
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
	 */
	public String monsterEquipsToString(int monsterId) {
		StringBuilder builder = new StringBuilder();
		Map<Integer, Long> monsterDressedMap = dressedEquipMap.get(monsterId);
		if (monsterDressedMap == null) {
			return builder.toString();
		}

		for (Map.Entry<Integer, Long> entry : monsterDressedMap.entrySet()) {
			builder.append(String.format("%d : %d; ", entry.getKey(), entry.getValue()));
		}

		return builder.toString();
	}

	/**
	 * 获取达到某等级怪物数量
	 */
	public int getMonsterCountOverLevel(int level) {
		int count = 0;
		for (Entry<Integer,MonsterEntity> entry : monsterEntityMap.entrySet()) {
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
		for (Entry<Integer,MonsterEntity> entry : monsterEntityMap.entrySet()) {
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

	public HSQuest getQuest(int questId){
		return questMap.get(questId);
	}

	public void addQuest(HSQuest quest) {
		questMap.put(quest.getQuestId(), quest);
	}

	/**
	 * 获取道具列表
	 * 
	 * @return
	 */
	public Map<String, ItemEntity> getItemEntityMap() {
		return itemEntityMap;
	}

	/**
	 * 获取物品
	 * 
	 * @return
	 */
	public ItemEntity getItemById(int id) {
		for (Map.Entry<String, ItemEntity> entry : itemEntityMap.entrySet()) {
			if (id == entry.getValue().getId()) {
				return entry.getValue();
			}
		}
		return null;
	}

	/**
	 * 获取物品
	 * 
	 * @return
	 */
	public ItemEntity getItemByItemId(String itemId) {
		return itemEntityMap.get(itemId);
	}

	/**
	 * 增加物品实体
	 * 
	 * @return
	 */
	public void addItemEntity(ItemEntity itemEntity) {
		itemEntityMap.put(itemEntity.getItemId(), itemEntity);
	}

	/**
	 * 获取装备列表
	 * 
	 * @return
	 */
	public Map<Long, EquipEntity> getEquipEntityMap() {
		return equipEntityMap;
	}

	/**
	 * 获取装备
	 * 
	 * @return
	 */
	public EquipEntity getEquipById(long id) {
		return equipEntityMap.get(id);
	}

	/**
	 * 增加装备实体
	 * 
	 * @return
	 */
	public void addEquipEntity(EquipEntity equipEntity) {
		equipEntityMap.put(equipEntity.getId(), equipEntity);
	}

	/**
	 * 移除装备数据
	 * 
	 * @param equipEntity
	 */
	public void removeEquipEntity(EquipEntity equipEntity) {
		equipEntityMap.remove(equipEntity.getId());
	}

	/**
	 * 获取邮件实体列表
	 */
	public List<MailEntity> getMailEntityList() {
		return mailEntityList;
	}

	public MailEntity getMailEntity(int id) {
		for (MailEntity mailEntity : mailEntityList) {
			if (id == mailEntity.getId()) {
				return mailEntity;
			}
		}
		return null;
	}

	/**
	 * 增加邮件实体
	 */
	public void addMailEntity(MailEntity mailEntity) {
		mailEntityList.add(mailEntity);
	}

	/**
	 * 移除邮件实体
	 */
	public void removeMailEntity(MailEntity mailEntity) {
		mailEntityList.remove(mailEntity);
	}

	/**
	 * 获取大冒险列表
	 */
	public List<AdventureEntity> getAdventureEntityList() {
		return adventureEntityList;
	}

	public AdventureEntity getAdventureEntity(int advenId) {
		for (AdventureEntity entity : adventureEntityList) {
			if (advenId == entity.getAdventureId()) {
				return entity;
			}
		}
		return null;
	}

	public AdventureEntity getAdventureEntity(int type, int gear) {
		for (AdventureEntity entity : adventureEntityList) {
			if (type == entity.getType() && gear == entity.getGear()) {
				return entity;
			}
		}
		return null;
	}

	public void addAdventureEntity(AdventureEntity adventureEntity) {
		adventureEntityList.add(adventureEntity);
	}

	/**
	 * 获取大冒险队伍列表
	 */
	public Map<Integer, AdventureTeamEntity> getAdventureTeamEntityMap() {
		return adventureTeamEntityMap;
	}

	public AdventureTeamEntity getAdventureTeamEntity(int teamId) {
		return adventureTeamEntityMap.get(teamId);
	}

	public void addAdventureTeamEntity(AdventureTeamEntity teamEntity) {
		adventureTeamEntityMap.put(teamEntity.getTeamId(), teamEntity);
	}

	public PVPDefenceEntity getPVPDefenceEntity(){
		return pvpDefenceEntity;
	}

	public List<PVPDefenceRecordEntity> getPVPDefenceRecordList(){
		return pvpDefenceRecordEntityList;
	}

	public void addPVPDefenceRecord(PVPDefenceRecordEntity pvpDefenceRecordEntity){
		pvpDefenceRecordEntityList.addFirst(pvpDefenceRecordEntity);
		while (pvpDefenceRecordEntityList.size() > GsConst.PVP.PVP_DEFENCE_RECORD_SIZE) {
			pvpDefenceRecordEntityList.removeLast().delete(true);
		}
	}

	public long getCacheRefreshTime(int refreshIndex) {
		return refreshTimeCache[refreshIndex];
	}

	public void setCacheRefreshTime(int refreshIndex, long refreshTime) {
		refreshTimeCache[refreshIndex] = refreshTime;
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
			List<PlayerEntity> resultList = HawkDBManager.getInstance().query("from PlayerEntity where puid = ? and invalid = 0", puid);
			if (resultList != null && resultList.size() > 0) {
				playerEntity = resultList.get(0);
				playerEntity.decode();
//				try {
//					if (playerEntity.getSilentTime() != null && playerEntity.getSilentTime().getTime() > HawkTime.getMillisecond()) {
//						ServerData.getInstance().addDisablePhone(playerEntity.getPhoneInfo());
//					}
//				} catch (Exception e) {
//				}
			}
			else {
				try {
					throw new HawkException(" test null exception");
				} catch (Exception e) {
					HawkException.catchException(e);
				}
			}
		}
		return playerEntity;
	}

	/**
	 * 加载玩家统计信息
	 */
	public StatisticsEntity loadStatistics() {
		if (statisticsEntity == null) {
			statisticsEntity = new StatisticsEntity();
			statisticsEntity.load(getId());
		}
		return statisticsEntity;
	}

	/**
	 * 加载怪物信息
	 * 
	 * @return
	 */
	public void loadAllMonster() {
		if (monsterEntityMap == null) {
			monsterEntityMap = new HashMap<>();
			List<MonsterEntity> resultList = HawkDBManager.getInstance().query("from MonsterEntity where playerId = ? and invalid = 0", getId());
			if (resultList != null && resultList.size() > 0) {
				for (MonsterEntity monsterEntity : resultList) {
					monsterEntity.decode();
					monsterEntityMap.put(monsterEntity.getId(), monsterEntity);
				}
			}
		}
	}

	/**
	 * 加载物品信息
	 * 
	 * @return
	 */
	public void loadAllItem() {
		if (itemEntityMap == null) {
			itemEntityMap = new HashMap<>();
			List<ItemEntity> resultList = HawkDBManager.getInstance().query("from ItemEntity where playerId = ? and invalid = 0 order by id asc", getId());
			if (resultList != null && resultList.size() > 0) {
				for (ItemEntity itemEntity : resultList) {
					itemEntity.decode();
					itemEntityMap.put(itemEntity.getItemId(), itemEntity);
				}
			}
		}
	}

	/**
	 * 加载装备信息
	 * 
	 * @return
	 */
	public void loadAllEquip() {
		if (equipEntityMap == null) {
			equipEntityMap = new HashMap<>();
			List<EquipEntity> resultList = HawkDBManager.getInstance().query("from EquipEntity where playerId = ? and invalid = 0 order by id asc", getId());
			if (resultList != null && resultList.size() > 0) {
				for (EquipEntity equipEntity : resultList) {
					equipEntity.decode();
					equipEntityMap.put(equipEntity.getId(), equipEntity);
				}
			}
		}
	}

	/**
	 * 加载邮件信息
	 */
	public void loadAllMail() {
		if (mailEntityList == null) {
			mailEntityList = HawkDBManager.getInstance().query("from MailEntity where receiverId = ? and invalid = 0 order by id asc", getId());
			if (mailEntityList == null) {
				mailEntityList = new ArrayList<>();
				return;
			}
			if (mailEntityList.size() > 0) {
				for (MailEntity mailEntity : mailEntityList) {
					mailEntity.decode();
				}
			}
		}
	}

	/**
	 * 加载角色商城
	 */
	public void loadShop() {
		if (shopEntityMap == null) {
			shopEntityMap = new HashMap<>();
			List<ShopEntity> resultList = HawkDBManager.getInstance().query("from ShopEntity where playerId = ? and invalid = 0", getId());
			if (resultList != null && resultList.size() > 0) {
				for (ShopEntity shopEntity : resultList) {
					shopEntity.decode();
					shopEntityMap.put(shopEntity.getType(), shopEntity);
				}
			}
		}

		for (int i = Const.shopType.NORMALSHOP_VALUE; i <= Const.shopType.SHOPNUM_VALUE; i++) {
			if (!shopEntityMap.containsKey(i)) {
				ShopEntity shopEntity= new ShopEntity();
				shopEntity.setPlayerId(player.getId());
				shopEntity.setType(i);
				shopEntity.notifyCreate();
				shopEntity.decode();
				shopEntityMap.put(i, shopEntity);
				ShopUtil.refreshShopData(i, player);
			}
		}
	}

	/**
	 * 加载公会个人信息
	 */
	public PlayerAllianceEntity loadPlayerAlliance() {
		// 先从工会里面取
		playerAllianceEntity = AllianceManager.getInstance().getPlayerAllianceEntity(player.getId());

		if (playerAllianceEntity == null) {
			List<PlayerAllianceEntity> resultList = HawkDBManager.getInstance().query("from PlayerAllianceEntity where playerId = ? and invalid = 0", getId());
			if (resultList != null && resultList.size() > 0) {
				playerAllianceEntity = resultList.get(0);
				playerAllianceEntity.decode();
			}
			if (playerAllianceEntity == null) {
				playerAllianceEntity = new PlayerAllianceEntity();
				playerAllianceEntity.setPlayerId(playerEntity.getId());
				playerAllianceEntity.setLevel(playerEntity.getLevel());
				playerAllianceEntity.setName(playerEntity.getNickname());
				playerAllianceEntity.notifyCreate();
			}
		}
		return playerAllianceEntity;
	}

	public void loadAllAdventure() {
		if (adventureEntityList == null) {
			adventureEntityList = HawkDBManager.getInstance().query("from AdventureEntity where playerId = ? and invalid = 0 order by id asc", getId());
			if (adventureEntityList == null) {
				adventureEntityList = new ArrayList<>();
				return;
			}
			if (adventureEntityList.size() > 0) {
				for (AdventureEntity entity : adventureEntityList) {
					entity.decode();
				}
			}
		}
	}

	public void loadAllAdventureTeam() {
		if (adventureTeamEntityMap == null) {
			adventureTeamEntityMap = new HashMap<>();
			List<AdventureTeamEntity> resultList = HawkDBManager.getInstance().query("from AdventureTeamEntity where playerId = ? and invalid = 0 order by id asc", getId());
			if (resultList != null && resultList.size() > 0) {
				for (AdventureTeamEntity teamEntity : resultList) {
					teamEntity.decode();
					adventureTeamEntityMap.put(teamEntity.getTeamId(), teamEntity);
				}
			}
		}
	}

	public void loadPVPDefenceData(){
		if (pvpDefenceEntity == null) {
			pvpDefenceEntity = PVPManager.getInstance().getPVPDefenceEntity(getId(), getNickname(), getLevel(), true);
		}
	}

	public PVPRankEntity loadPVPRankData(){
		PVPRankEntity rankEntity = null;
		List<PVPRankEntity> resultList = HawkDBManager.getInstance().query("from PVPRankEntity where playerId = ? and invalid = 0", getId());
		if (resultList != null && resultList.size() > 0) {
			rankEntity = resultList.get(0);
		}
		if (rankEntity == null) {
			rankEntity = new PVPRankEntity();
			rankEntity.setPlayerId(getId());
			rankEntity.setName(player.getName());
			rankEntity.setLevel(getLevel());
			rankEntity.setPoint(GsConst.PVP.PVP_DEFAULT_POINT);
			rankEntity.setInRank(true);
			rankEntity.notifyCreate();
		}

		return rankEntity;
	}

	public void loadPVPDefenceRecordData(){
		if (pvpDefenceRecordEntityList == null || pvpDefenceRecordEntityList.isEmpty()) {
			List<PVPDefenceRecordEntity> resultList = HawkDBManager.getInstance().query("from PVPDefenceRecordEntity where playerId = ? and invalid = 0 order by id DESC ", getId());
			if (resultList != null && resultList.size() > 0) {
				pvpDefenceRecordEntityList.addAll(resultList);
				while (pvpDefenceRecordEntityList.size() > GsConst.PVP.PVP_DEFENCE_RECORD_SIZE) {
					PVPDefenceRecordEntity pvpDefenceRecordEntity = pvpDefenceRecordEntityList.removeLast();
					pvpDefenceRecordEntity.delete(true);
				}
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
		builder.setInfo(BuilderUtil.genPlayerBuilder(playerEntity));
		player.sendProtocol(HawkProtocol.valueOf(HS.code.PLAYER_INFO_SYNC_S, builder));
	}

	/**
	 * 同步玩家统计信息
	 */
	public void syncStatisticsInfo() {
		player.sendProtocol(HawkProtocol.valueOf(HS.code.STATISTICS_SYNC_PART1_S, BuilderUtil.genStatisticsPart1Builder(statisticsEntity)));
		player.sendProtocol(HawkProtocol.valueOf(HS.code.STATISTICS_SYNC_PART2_S, BuilderUtil.genStatisticsPart2Builder(statisticsEntity)));
		player.sendProtocol(HawkProtocol.valueOf(HS.code.STATISTICS_SYNC_PART3_S, BuilderUtil.genStatisticsPart3Builder(statisticsEntity)));
		player.sendProtocol(HawkProtocol.valueOf(HS.code.STATISTICS_SYNC_GUIDE_S, BuilderUtil.genStatisticsGuideBuilder(statisticsEntity)));
	}

	/**
	 * 同步新手引导信息
	 */
	public void syncStatisticsGuideInfo() {
		player.sendProtocol(HawkProtocol.valueOf(HS.code.STATISTICS_SYNC_GUIDE_S, BuilderUtil.genStatisticsGuideBuilder(statisticsEntity)));
	}

	/**
	 * 同步经验次数信息
	 */
	public void syncStatisticsExpLeftInfo() {
		player.sendProtocol(HawkProtocol.valueOf(HS.code.SYNC_EXP_LEFT_TIMES_S, BuilderUtil.genSyncExpLeftTimesBuilder(statisticsEntity)));
	}

	/**
	 * 同步商店刷新次数信息
	 */
	public void syncShopRefreshTimeInfo() {
		player.sendProtocol(HawkProtocol.valueOf(HS.code.SHOP_REFRESH_TIMES, BuilderUtil.genShopRefreshTimeBuilder(player)));
	}

	/**
	 * 刷新商店数据
	 */
	public void syncShopRefreshInfo(int shopType) {
		player.sendProtocol(HawkProtocol.valueOf(HS.code.SYNC_SHOP_REFRESH_S, BuilderUtil.genSyncShopRefreshBuilder(shopType)));
	}

	/**
	 * 同步每日刷新数据
	 */
	public void syncDailyRefreshInfo() {
		player.sendProtocol(HawkProtocol.valueOf(HS.code.SYNC_DAILY_REFRESH_S, BuilderUtil.genSyncDailyRefreshBuilder()));
	}

	/**
	 * 同步每月刷新数据
	 */
	public void syncMonthlyRefreshInfo() {
		player.sendProtocol(HawkProtocol.valueOf(HS.code.SYNC_MONTHLY_REFRESH_S, BuilderUtil.genSyncMonthlyRefreshBuilder()));
	}

	/**
	 * 同步系统设置信息
	 */
	public void syncSettingInfo() {
		HSSettingInfoSync.Builder builder = HSSettingInfoSync.newBuilder();

		HSSetting.Builder setting = HSSetting.newBuilder();
		setting.setLanguage(playerEntity.getLanguage());
		setting.addAllBlockPlayerId(playerEntity.getBlockPlayerList());
		builder.setSetting(setting);

		HawkProtocol protocol = HawkProtocol.valueOf(HS.code.SETTING_INFO_SYNC_S, builder);
		player.sendProtocol(protocol);
	}

	/**
	 * 同步怪物信息(0表示同步所有)
	 */
	public void syncMonsterInfo(int id) {
		HSMonsterInfoSync.Builder builder = HSMonsterInfoSync.newBuilder();
		// 判断宠物是否在基地中
		PlayerAllianceEntity playerAllianceEntity = AllianceManager.getInstance().getPlayerAllianceEntity(player.getId());
		if (id == 0) {
			for (Entry<Integer, MonsterEntity> entry : monsterEntityMap.entrySet()) {
				builder.addMonsterInfo(BuilderUtil.genMonsterBuilder(entry.getValue(), playerAllianceEntity));
				// 分批发送
				if (builder.getMonsterInfoCount() >= 10) {
					player.sendProtocol(HawkProtocol.valueOf(HS.code.MONSTER_INFO_SYNC_S, builder));
					builder = HSMonsterInfoSync.newBuilder();
				}
			}
		}
		else {
			builder.addMonsterInfo(BuilderUtil.genMonsterBuilder(monsterEntityMap.get(id), playerAllianceEntity));
		}

		if (builder.getMonsterInfoCount() > 0) {
			player.sendProtocol(HawkProtocol.valueOf(HS.code.MONSTER_INFO_SYNC_S, builder));
		}
	}

	/**
	 * 同步物品信息
	 */
	public void syncItemInfo() {
		HSItemInfoSync.Builder builder = HSItemInfoSync.newBuilder();
		for (Map.Entry<String, ItemEntity> entry : itemEntityMap.entrySet()) {
			if (entry.getValue().getCount() > 0 && !entry.getValue().isInvalid()) {
				ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, entry.getValue().getItemId());
				if (itemCfg == null) {
					continue;
				}
				builder.addItemInfos(BuilderUtil.genItemBuilder(entry.getValue()));
				// 分批发送
				if (builder.getItemInfosCount() >= 10) {
					player.sendProtocol(HawkProtocol.valueOf(HS.code.ITEM_INFO_SYNC_S, builder));
					builder = HSItemInfoSync.newBuilder();
				}
			}
		}

		if (builder.getItemInfosCount() > 0) {
			player.sendProtocol(HawkProtocol.valueOf(HS.code.ITEM_INFO_SYNC_S, builder));
		}
	}

	/**
	 * 同步装备信息
	 */
	public void syncEquipInfo() {
		HSEquipInfoSync.Builder builder = HSEquipInfoSync.newBuilder();
		for (Map.Entry<Long, EquipEntity> entry : equipEntityMap.entrySet()) {
			if (!entry.getValue().isInvalid()) {
				builder.addEquipInfos(BuilderUtil.genEquipBuilder(entry.getValue()));
				// 分批发送
				if (builder.getEquipInfosCount() >= 10) {
					player.sendProtocol(HawkProtocol.valueOf(HS.code.EQUIP_INFO_SYNC_S, builder));
					builder = HSEquipInfoSync.newBuilder(); 
				}
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

	/**
	 * 同步邮件信息
	 */
	public void syncMailInfo() {
		HSMailInfoSync.Builder builder = HSMailInfoSync.newBuilder();

		for (MailEntity mailEntity : mailEntityList) {
			builder.addMailInfo(BuilderUtil.genMailBuilder(mailEntity));
		}

		HawkProtocol protocol = HawkProtocol.valueOf(HS.code.MAIL_INFO_SYNC_S, builder);
		player.sendProtocol(protocol);
	}

	/**
	 * 同步大冒险信息
	 */
	public void syncAdventureInfo() {
		HSAdventureInfoSync.Builder builder = HSAdventureInfoSync.newBuilder();
		builder.setTeamCount(adventureTeamEntityMap.size());

		List<Integer> busyAdventureList = new ArrayList<Integer> ();

		for (AdventureTeamEntity teamEntity : adventureTeamEntityMap.values()) {
			if (0 != teamEntity.getAdventureId()) {
				builder.addBusyTeam(BuilderUtil.genAdventureTeamBuilder(teamEntity));
				busyAdventureList.add(teamEntity.getAdventureId());
			}
		}

		for (AdventureEntity advenEntity : adventureEntityList) {
			if (false == busyAdventureList.contains(advenEntity.getAdventureId())) {
				builder.addIdleAdventure(BuilderUtil.genAdventureBuilder(advenEntity));
			}
		}

		HawkProtocol protocol = HawkProtocol.valueOf(HS.code.ADVENTURE_INFO_SYNC_S, builder);
		player.sendProtocol(protocol);
	}

	/**
	 * 同步PVP防守阵容
	 */
	public void syncPVPDefenceInfo(){
		HSGetPVPDefenceMonsterRet.Builder response = HSGetPVPDefenceMonsterRet.newBuilder();
		for (HSMonster.Builder monster : getPVPDefenceEntity().getMonsterDefenceBuilder().getMonsterInfoBuilderList()) {
			response.addMonsterId(monster.getMonsterId());
		}

		player.sendProtocol(HawkProtocol.valueOf(HS.code.PVP_DEFENCE_SYNC_S_VALUE, response));
	}

	/**
	 * 获取在线玩家快照数据
	 * @return
	 */
	public SnapshotInfo.Builder getOnlinePlayerSnapshot(boolean refresh) {
		if(refresh || this.onlinePlayerSnapshot == null){
			refreshOnlinePlayerSnapshot();
		}
		return onlinePlayerSnapshot;
	}

	/**
	 * 刷新在线玩家快照数据
	 */
	public void refreshOnlinePlayerSnapshot(){
		this.onlinePlayerSnapshot = SnapshotUtil.genOnlineQuickPhoto(this);
	}
}
