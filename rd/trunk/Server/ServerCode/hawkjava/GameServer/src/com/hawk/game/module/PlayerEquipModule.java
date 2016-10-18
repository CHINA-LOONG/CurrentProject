package com.hawk.game.module;

import java.util.Map;

import org.hawk.config.HawkConfigManager;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkException;
import org.hawk.os.HawkRand;

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.BILog.BIEquipAdvanceFlow;
import com.hawk.game.BILog.BIEquipFlowData;
import com.hawk.game.BILog.BIEquipIntensifyData;
import com.hawk.game.BILog.BIGemFlowData;
import com.hawk.game.config.EquipForgeCfg;
import com.hawk.game.config.ItemCfg;
import com.hawk.game.config.MonsterCfg;
import com.hawk.game.entity.EquipEntity;
import com.hawk.game.entity.ItemEntity;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.statistics.StatisticsEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.item.GemInfo;
import com.hawk.game.log.BILogger;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.Const.changeType;
import com.hawk.game.protocol.Const.itemType;
import com.hawk.game.protocol.Consume.ConsumeItem;
import com.hawk.game.protocol.Equip.GemPunch;
import com.hawk.game.protocol.Equip.HSEquipBuy;
import com.hawk.game.protocol.Equip.HSEquipBuyRet;
import com.hawk.game.protocol.Equip.HSEquipDecompose;
import com.hawk.game.protocol.Equip.HSEquipGem;
import com.hawk.game.protocol.Equip.HSEquipGemRet;
import com.hawk.game.protocol.Equip.HSEquipIncreaseLevel;
import com.hawk.game.protocol.Equip.HSEquipIncreaseLevelRet;
import com.hawk.game.protocol.Equip.HSEquipIncreaseStage;
import com.hawk.game.protocol.Equip.HSEquipIncreaseStageRet;
import com.hawk.game.protocol.Equip.HSEquipMonsterDress;
import com.hawk.game.protocol.Equip.HSEquipMonsterDressRet;
import com.hawk.game.protocol.Equip.HSEquipMonsterReplace;
import com.hawk.game.protocol.Equip.HSEquipMonsterReplaceRet;
import com.hawk.game.protocol.Equip.HSEquipMonsterUndress;
import com.hawk.game.protocol.Equip.HSEquipMonsterUndressRet;
import com.hawk.game.protocol.Equip.HSEquipPunch;
import com.hawk.game.protocol.Equip.HSEquipPunchRet;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.EquipUtil;
import com.hawk.game.util.GsConst;

public class PlayerEquipModule extends PlayerModule{
	/**
	 * 构造函数
	 * 
	 * @param player
	 */
	public PlayerEquipModule(Player player) {
		super(player);
		listenProto(HS.code.EQUIP_SELL_C);
		listenProto(HS.code.EQUIP_BUY_C);
		listenProto(HS.code.EQUIP_INCREASE_LEVEL_C);
		listenProto(HS.code.EQUIP_INCREASE_STAGE_C);
		listenProto(HS.code.EQUIP_MONSTER_DRESS_C);
		listenProto(HS.code.EQUIP_MONSTER_UNDRESS_C);
		listenProto(HS.code.EQUIP_MONSTER_REPLACE_C);
		listenProto(HS.code.EQUIP_PUNCH_C);
		listenProto(HS.code.EQUIP_GEM_C);
		listenProto(HS.code.EQUIP_DECOMPOSE_C_VALUE);
	}

	/**
	 * 协议响应
	 * 
	 * @param protocol
	 * @return
	 */
	@Override
	public boolean onProtocol(HawkProtocol protocol) {
		try {
			if (protocol.checkType(HS.code.EQUIP_SELL_C)) {
				// 卖装备
				//onItemUse(protocol.getType(),protocol.parseProtocol(HSItemUse.getDefaultInstance()));
				return true;
			} 
			else if(protocol.checkType(HS.code.EQUIP_BUY_C)) {
				//买装备
				onEquipBuy(HS.code.EQUIP_BUY_C_VALUE, protocol.parseProtocol(HSEquipBuy.getDefaultInstance()));
				return true;
			}
			else if(protocol.checkType(HS.code.EQUIP_INCREASE_LEVEL_C)) {
				//升级
				onEquipIncreaseLevel(HS.code.EQUIP_INCREASE_LEVEL_C_VALUE, protocol.parseProtocol(HSEquipIncreaseLevel.getDefaultInstance()));
				return true;
			}
			else if(protocol.checkType(HS.code.EQUIP_INCREASE_STAGE_C)) {
				//进阶
				onEquipIncreaseStage(HS.code.EQUIP_INCREASE_STAGE_C_VALUE, protocol.parseProtocol(HSEquipIncreaseStage.getDefaultInstance()));
				return true;
			}
			else if(protocol.checkType(HS.code.EQUIP_PUNCH_C)) {
				// 打孔
				onEquipPunch(HS.code.EQUIP_PUNCH_C_VALUE, protocol.parseProtocol(HSEquipPunch.getDefaultInstance()));
				return true;
			}
			else if(protocol.checkType(HS.code.EQUIP_GEM_C)) {
				// 镶嵌宝石
				onEquipGem(HS.code.EQUIP_GEM_C_VALUE, protocol.parseProtocol(HSEquipGem.getDefaultInstance()));
				return true;
			}
			else if(protocol.checkType(HS.code.EQUIP_MONSTER_DRESS_C)) {
				//穿装备
				onEquipDressOnMonster(HS.code.EQUIP_MONSTER_DRESS_C_VALUE, protocol.parseProtocol(HSEquipMonsterDress.getDefaultInstance()));
				return true;
			}
			else if(protocol.checkType(HS.code.EQUIP_MONSTER_UNDRESS_C)) {
				//脱装备
				onEquipUnDressOnMonster(HS.code.EQUIP_MONSTER_UNDRESS_C_VALUE, protocol.parseProtocol(HSEquipMonsterUndress.getDefaultInstance()));
				return true;
			}
			else if(protocol.checkType(HS.code.EQUIP_MONSTER_REPLACE_C)) {
				//替换装备
				onEquipReplaceOnMonster(HS.code.EQUIP_MONSTER_REPLACE_C_VALUE, protocol.parseProtocol(HSEquipMonsterReplace.getDefaultInstance()));
				return true;
			}
			else if(protocol.checkType(HS.code.EQUIP_DECOMPOSE_C)) {
				//分解
				onEquipDecompose(HS.code.EQUIP_DECOMPOSE_C_VALUE, protocol.parseProtocol(HSEquipDecompose.getDefaultInstance()));
				return true;
			}
		}
		catch (Exception e) {
			HawkException.catchException(e);
		}

		return false;
	}

	/**
	 * 玩家上线处理
	 * 
	 * @return
	 */
	@Override
	protected boolean onPlayerLogin() {
		player.getPlayerData().loadAllEquip();
		player.getPlayerData().syncEquipInfo();
		return true;
	}

	/*
	 * 装备购买 
	 * 废弃的接口
	 */
	public void onEquipBuy(int hsCode, HSEquipBuy protocol)
	{
		String equid = protocol.getEquipId();
		int equipCount = protocol.getEquipCount();
		if (equipCount <= 0) {
			sendError(hsCode, Status.error.PARAMS_INVALID);
			return ;
		}

		ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, equid);
		if(itemCfg == null) {
			sendError(hsCode, Status.error.CONFIG_NOT_FOUND);
			return ;
		}

		if (itemCfg.getType() != Const.toolType.EQUIPTOOL_VALUE) {
			sendError(hsCode, Status.error.PARAMS_INVALID);
			return ;
		}

		if (itemCfg.getBuyPrice() == GsConst.UNUSABLE) {
			sendError(hsCode, Status.itemError.ITEM_BUY_NOT_ALLOW);
			return ;
		}

		Const.changeType changeType = itemCfg.getBuyType() == Const.moneyType.MONEY_COIN_VALUE ? Const.changeType.CHANGE_COIN : Const.changeType.CHANGE_GOLD;
		float price = itemCfg.getBuyPrice() * equipCount * (1 + 0.2f * protocol.getStage());
		ConsumeItems consume =	ConsumeItems.valueOf(changeType, (int)price);
		if (consume.checkConsume(player, hsCode) == false) {
			return ;
		}

		consume.consumeTakeAffectAndPush(player, Action.NULL, hsCode);

		AwardItems awardItems = new AwardItems();
		awardItems.addEquip(equid, equipCount, protocol.getStage(), protocol.getLevel());
		awardItems.rewardTakeAffectAndPush(player, Action.NULL, hsCode);

		HSEquipBuyRet.Builder response = HSEquipBuyRet.newBuilder();
		response.setEquipCount(equipCount);
		response.setEquipId(equid);
		sendProtocol(HawkProtocol.valueOf(HS.code.EQUIP_BUY_S_VALUE, response));
	}

	/*
	 * 装备升级
	 */
	public void onEquipIncreaseLevel(int hsCode, HSEquipIncreaseLevel protocol) throws HawkException
	{
		EquipEntity equipEntity = player.getPlayerData().getEquipById(protocol.getId());
		if (equipEntity == null) {
			sendError(hsCode, Status.itemError.ITEM_NOT_FOUND_VALUE);
			return ;
		}

		ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, equipEntity.getItemId());
		if(itemCfg == null) {
			sendError(hsCode, Status.error.CONFIG_NOT_FOUND);
			return ;
		}

		// level 从0开始
		if (equipEntity.getLevel() >= GsConst.EQUIP_MAX_LEVEL) {
			sendError(hsCode, Status.itemError.EQUIP_MAX_LEVEL_ALREADY);
			return ;
		}

		if (player.getLevel() < EquipForgeCfg.getPlayerLevelDemand(equipEntity.getStage(), equipEntity.getLevel() + 1)) {
			sendError(hsCode, Status.itemError.EQUIP_PLAYER_LEVEL_DEMAND_VALUE);
			return ;
		}

		ConsumeItems consume = new ConsumeItems();
		consume.addItemInfos(EquipForgeCfg.getLevelDemandList(equipEntity.getStage(), equipEntity.getLevel() + 1));
		
		// 调整金币
		ConsumeItem.Builder consumeItem = consume.getConsumeItem(itemType.PLAYER_ATTR_VALUE, String.valueOf(changeType.CHANGE_ARENA_COIN_VALUE));
		if (consumeItem != null) {
			consumeItem.setCount((int)(consumeItem.getCount() * itemCfg.getForgeAdjust()));
		}
		if (consume.checkConsume(player, hsCode) == false) {
			return;
		}

		consume.consumeTakeAffectAndPush(player, Action.EQUIP_FORGE, hsCode);
		if (EquipForgeCfg.getSuccessRate(equipEntity.getStage(), equipEntity.getLevel() + 1) >= HawkRand.randFloat(0, 1)) {
			equipEntity.setLevel(equipEntity.getLevel() + 1);
			equipEntity.notifyUpdate(true);
		}

		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		statisticsEntity.increaseUpEquipTimes();
		statisticsEntity.increaseUpEquipTimesDaily();
		statisticsEntity.notifyUpdate(true);

		HSEquipIncreaseLevelRet.Builder response = HSEquipIncreaseLevelRet.newBuilder();
		response.setId(equipEntity.getId());
		response.setStage(equipEntity.getStage());
		response.setLevel(equipEntity.getLevel());
		sendProtocol(HawkProtocol.valueOf(HS.code.EQUIP_INCREASE_LEVEL_S, response));

		MonsterCfg monsterCfg = null;
		if (equipEntity.getMonsterId() != GsConst.EQUIP_NOT_DRESS) {
			MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(equipEntity.getMonsterId());
			monsterCfg = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, monsterEntity.getCfgId());
		}

		BILogger.getBIData(BIEquipIntensifyData.class).log(
				player, 
				itemCfg, 
				protocol.getId(), 
				equipEntity.getStage(), 
				monsterCfg, 
				equipEntity.getMonsterId(), 
				equipEntity.getLevel() - 1, 
				equipEntity.getLevel()
				);
	}

	/*
	 * 装备进阶
	 */
	public void onEquipIncreaseStage(int hsCode, HSEquipIncreaseStage protocol) throws HawkException
	{
		EquipEntity equipEntity = player.getPlayerData().getEquipById(protocol.getId());
		if (equipEntity == null) {
			sendError(hsCode, Status.itemError.ITEM_NOT_FOUND_VALUE);
			return ;
		}

		ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, equipEntity.getItemId());
		if(itemCfg == null) {
			sendError(hsCode, Status.error.CONFIG_NOT_FOUND);
			return ;
		}

		if (equipEntity.getLevel() < GsConst.EQUIP_MAX_LEVEL) {
			sendError(hsCode, Status.itemError.EQUIP_LEVEL_NOT_ENOUGH_VALUE);
			return ;
		}

		if (equipEntity.getStage() >= GsConst.EQUIP_MAX_STAGE) {
			sendError(hsCode, Status.itemError.EQUIP_MAX_STAGE_ALREADY_VALUE);
			return ;
		}

		if (player.getLevel() < EquipForgeCfg.getPlayerLevelDemand(equipEntity.getStage() + 1, 0)) {
			sendError(hsCode, Status.itemError.EQUIP_PLAYER_LEVEL_DEMAND_VALUE);
			return ;
		}

		ConsumeItems consume = new ConsumeItems();
		consume.addItemInfos(EquipForgeCfg.getLevelDemandList(equipEntity.getStage() + 1, 0));
		
		// 调整金币
		ConsumeItem.Builder consumeItem = consume.getConsumeItem(itemType.PLAYER_ATTR_VALUE, String.valueOf(changeType.CHANGE_ARENA_COIN_VALUE));
		if (consumeItem != null) {
			consumeItem.setCount((int)(consumeItem.getCount() * itemCfg.getForgeAdjust()));
		}
		
		if (consume.checkConsume(player, hsCode) == false) {
			return;
		}

		int oldLevel = equipEntity.getLevel();
		consume.consumeTakeAffectAndPush(player, Action.EQUIP_ADVANCE, hsCode);
		if (EquipForgeCfg.getSuccessRate(equipEntity.getStage() + 1, 0) >= HawkRand.randFloat(0, 1)) {
			equipEntity.setLevel(0);
			equipEntity.setStage(equipEntity.getStage() + 1);
			equipEntity.notifyUpdate(true);
		}

		HSEquipIncreaseStageRet.Builder response = HSEquipIncreaseStageRet.newBuilder();
		response.setId(equipEntity.getId());
		response.setStage(equipEntity.getStage());
		response.setLevel(equipEntity.getLevel());
		sendProtocol(HawkProtocol.valueOf(HS.code.EQUIP_INCREASE_STAGE_S, response));

		MonsterCfg monsterCfg = null;
		if (equipEntity.getMonsterId() != GsConst.EQUIP_NOT_DRESS) {
			MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(equipEntity.getMonsterId());
			monsterCfg = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, monsterEntity.getCfgId());

			updateEquipStageStatistics(equipEntity.getMonsterId(), equipEntity.getStage());
		}

		BILogger.getBIData(BIEquipAdvanceFlow.class).log(
				player,
				itemCfg, 
				protocol.getId(), 
				monsterCfg, 
				equipEntity.getStage(), 
				equipEntity.getMonsterId(), 
				oldLevel, 
				equipEntity.getLevel()
				);
	}

	/*
	 * 镶嵌宝石
	 */
	public void onEquipGem(int hsCode, HSEquipGem protocol)
	{
		EquipEntity equipEntity = player.getPlayerData().getEquipById(protocol.getId());
		if (equipEntity == null) {
			sendError(hsCode, Status.itemError.ITEM_NOT_FOUND_VALUE);
			return ;
		}

		ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, equipEntity.getItemId());
		if(itemCfg == null) {
			sendError(hsCode, Status.error.CONFIG_NOT_FOUND);
			return ;
		}

		if (protocol.getOldGem().equals("") && protocol.getNewGem().equals("")) {
			sendError(hsCode, Status.error.PARAMS_INVALID_VALUE);
			return ;
		}

		if (equipEntity.GetGemDressList().get(protocol.getSlot()) == null) {
			sendError(hsCode, Status.itemError.EQUIP_SLOT_NOT_PUNCH_VALUE);
			return ;
		}

		AwardItems award = new AwardItems();
		ConsumeItems consume = new ConsumeItems();
		if (protocol.getNewGem().equals("") == false) {
			ItemEntity itemEntity = player.getPlayerData().getItemByItemId(protocol.getNewGem());
			if (itemEntity == null) {
				sendError(hsCode, Status.itemError.ITEM_NOT_FOUND_VALUE);
				return ;
			}

			ItemCfg gemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, protocol.getNewGem());
			if (gemCfg == null) {
				sendError(hsCode, Status.error.CONFIG_NOT_FOUND);
				return ;
			}

			if (gemCfg.getType() != Const.toolType.GEMTOOL_VALUE || gemCfg.getGemType() != protocol.getType()) {
				sendError(hsCode, Status.error.PARAMS_INVALID_VALUE);
				return ;
			}

			if (equipEntity.GetGemDressList().get(protocol.getSlot()).getType() != gemCfg.getGemType()) {
				sendError(hsCode, Status.error.PARAMS_INVALID_VALUE);
				return ;
			}

			consume.addItem(protocol.getNewGem(), 1);
		}

		if (protocol.getOldGem().equals("") == false) {
			if (equipEntity.GetGemDressList().get(protocol.getSlot()) == null || equipEntity.GetGemDressList().get(protocol.getSlot()).getGemId().equals(GsConst.EQUIP_GEM_NONE)) {
				sendError(hsCode, Status.itemError.EQUIP_SLOT_EMPTY);
				return ;
			}

			if (!equipEntity.GetGemDressList().get(protocol.getSlot()).getGemId().equals(protocol.getOldGem())) {
				sendError(hsCode, Status.itemError.EQUIP_GEM_MISMATCH);
				return ;
			}

			award.addItem(protocol.getOldGem(), 1);
		}
		else {
			if (equipEntity.GetGemDressList().get(protocol.getSlot()) != null && !equipEntity.GetGemDressList().get(protocol.getSlot()).getGemId().equals(GsConst.EQUIP_GEM_NONE)) {
				sendError(hsCode, Status.itemError.EQUIP_SLOT_NOT_EMPTY);
				return ;
			}
		}

		if (!protocol.getNewGem().equals("") && !protocol.getOldGem().equals("")) {
			equipEntity.replaceGem(protocol.getSlot(), protocol.getOldGem(), protocol.getNewGem());

			StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
			statisticsEntity.increaseInlayAllTimes();
			statisticsEntity.increaseInlayTypeTimes(protocol.getType());
			statisticsEntity.notifyUpdate(true);
		}
		else if(!protocol.getNewGem().equals("")) {
			equipEntity.addGem(protocol.getSlot(), protocol.getType(), protocol.getNewGem());

			StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
			statisticsEntity.increaseInlayAllTimes();
			statisticsEntity.increaseInlayTypeTimes(protocol.getType());
			statisticsEntity.notifyUpdate(true);
		}
		else {
			equipEntity.removeGem(protocol.getSlot(), protocol.getOldGem());
		}

		equipEntity.notifyUpdate(true);
		award.rewardTakeAffectAndPush(player, Action.EQUIP_GEM, hsCode);
		consume.consumeTakeAffectAndPush(player, Action.EQUIP_GEM, hsCode);

		HSEquipGemRet.Builder response = HSEquipGemRet.newBuilder();
		for (Map.Entry<Integer, GemInfo> entry : equipEntity.GetGemDressList().entrySet()) {
			GemPunch.Builder gemPunch = GemPunch.newBuilder();
			gemPunch.setSlot(entry.getKey());
			gemPunch.setType(entry.getValue().getType());
			gemPunch.setGemItemId(entry.getValue().getGemId());
			response.addGemItems(gemPunch);
		}

		sendProtocol(HawkProtocol.valueOf(HS.code.EQUIP_GEM_S_VALUE, response));

		MonsterCfg monsterCfg = null;
		if (equipEntity.getMonsterId() != GsConst.EQUIP_NOT_DRESS) {
			MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(equipEntity.getMonsterId());
			monsterCfg = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, monsterEntity.getCfgId());
		}

		// 装备BI
		if (!protocol.getNewGem().equals("")) {
			ItemCfg gemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, protocol.getNewGem());
			BILogger.getBIData(BIGemFlowData.class).log(player, itemCfg, equipEntity.getId(), monsterCfg, gemCfg.getGrade(), true);
		}

		// 卸载BI
		if (!protocol.getOldGem().equals("")) {
			ItemCfg gemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, protocol.getOldGem());
			BILogger.getBIData(BIGemFlowData.class).log(player, itemCfg, equipEntity.getId(), monsterCfg, gemCfg.getGrade(), true);
		}
	}

	/*
	 * 装备打孔
	 */
	public void onEquipPunch(int hsCode, HSEquipPunch protocol)
	{
		EquipEntity equipEntity = player.getPlayerData().getEquipById(protocol.getId());
		if (equipEntity == null) {
			sendError(hsCode, Status.itemError.ITEM_NOT_FOUND_VALUE);
			return ;
		}

		ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, equipEntity.getItemId());
		if(itemCfg == null) {
			sendError(hsCode, Status.error.CONFIG_NOT_FOUND);
			return ;
		}

		if (EquipUtil.getPunchCount(equipEntity) == 0) {
			sendError(hsCode, Status.itemError.EQUIP_CAN_NOT_OPEN_SLOT);
			return ;
		}

		AwardItems rewardItems = null;
		if (equipEntity.GetGemDressList().isEmpty() == false) {
			rewardItems = new AwardItems();
			for (Map.Entry<Integer, GemInfo> entry : equipEntity.GetGemDressList().entrySet()) {
				rewardItems.addItem(entry.getValue().getGemId(), 1);
			}
		}

		ConsumeItems consume = new ConsumeItems();
		consume.addItemInfos(EquipForgeCfg.getPunchDemandList(equipEntity.getStage(), equipEntity.getLevel()));
		if (consume.checkConsume(player, hsCode) == false) {
			return;
		}

		HSEquipPunchRet.Builder response = HSEquipPunchRet.newBuilder();
		equipEntity.GetGemDressList().clear();
		try {
			int newPunchCount = HawkRand.randInt(1, EquipUtil.getPunchCount(equipEntity));
			for (int i = 0; i < newPunchCount; i++) {
				int type = HawkRand.randInt(1, GsConst.GEM_MAX_TYPE);
				equipEntity.addGem(i, type);
				GemPunch.Builder gemInfo = GemPunch.newBuilder();
				gemInfo.setSlot(i);
				gemInfo.setType(type);
				gemInfo.setGemItemId(GsConst.EQUIP_GEM_NONE);
				response.addGemItems(gemInfo);
			}
		}
		catch (HawkException e) {
			HawkException.catchException(e);
			return;
		}

		equipEntity.notifyUpdate(true);
		if (rewardItems != null) {
			rewardItems.rewardTakeAffectAndPush(player, Action.EQUIP_OPEN_SLOTS, hsCode);
		}
		consume.consumeTakeAffectAndPush(player, Action.EQUIP_OPEN_SLOTS, hsCode);

		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		statisticsEntity.increaseEquipPunchTimes();
		statisticsEntity.increaseEquipPunchTimesDaily();
		statisticsEntity.notifyUpdate(true);

		sendProtocol(HawkProtocol.valueOf(HS.code.EQUIP_PUNCH_S_VALUE, response));

	}

	/*
	 * 穿装备
	 */
	public void onEquipDressOnMonster(int hsCode, HSEquipMonsterDress protocol) {
		long id = protocol.getId();
		int monsterId = protocol.getMonsterId();

		EquipEntity equipEntity = player.getPlayerData().getEquipById(id);
		if (equipEntity == null) {
			sendError(hsCode, Status.itemError.ITEM_NOT_FOUND_VALUE);
			return ;
		}

		ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, equipEntity.getItemId());
		if(itemCfg == null) {
			sendError(hsCode, Status.error.CONFIG_NOT_FOUND);
			return ;
		}

		if (equipEntity.getMonsterId() != GsConst.EQUIP_NOT_DRESS) {
			sendError(hsCode, Status.itemError.EQUIP_DRESS_ALREADY_VALUE);
			return ;
		}

		if (player.getPlayerData().isMonsterEquipOnPart(monsterId, itemCfg.getPart()) == true) {
			sendError(hsCode, Status.itemError.EQUIP_DRESS_OTHER_ALREADY_VALUE);
			return ;
		}

		if (player.getPlayerData().addMonsterEquip(monsterId, equipEntity, itemCfg.getPart()) == false) {
			sendError(hsCode, Status.error.SERVER_ERROR);
			return ;
		}

		equipEntity.setMonsterId(monsterId);
		equipEntity.notifyUpdate(true);

		updateEquipStageStatistics(monsterId, equipEntity.getStage());

		HSEquipMonsterDressRet.Builder response = HSEquipMonsterDressRet.newBuilder();
		response.setId(id);
		response.setMonsterId(monsterId);
		sendProtocol(HawkProtocol.valueOf(HS.code.EQUIP_MONSTER_DRESS_S_VALUE, response));

		MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(monsterId);
		if (monsterEntity == null) {
			throw new RuntimeException("monster not found " + monsterId);
		}

		MonsterCfg monsterCfg = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, monsterEntity.getCfgId());
		if (monsterCfg == null) {
			throw new RuntimeException("monster config not found " + monsterEntity.getCfgId());
		}

		BILogger.getBIData(BIEquipFlowData.class).log(player, itemCfg, protocol.getId(), monsterCfg, monsterId, equipEntity.getStage(), equipEntity.getLevel(), true);
	}

	/*
	 * 脱装备
	 */
	public void onEquipUnDressOnMonster(int hsCode, HSEquipMonsterUndress protocol) {
		long id = protocol.getId();
		EquipEntity equipEntity = player.getPlayerData().getEquipById(id);
		if (equipEntity == null) {
			sendError(hsCode, Status.itemError.ITEM_NOT_FOUND_VALUE);
			return ;
		}

		ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, equipEntity.getItemId());
		if(itemCfg == null) {
			sendError(hsCode, Status.error.CONFIG_NOT_FOUND);
			return ;
		}

		if (player.getPlayerData().isMonsterEquipOnPart(equipEntity.getMonsterId(), itemCfg.getPart(), id) == false) {
			sendError(hsCode, Status.itemError.EQUIP_NOT_DRESSED);
			return ;
		}

		if (player.getPlayerData().removeMonsterEquip(equipEntity, itemCfg.getPart()) == false) {
			sendError(hsCode, Status.error.SERVER_ERROR);
			return ;
		}

		int monsterId = equipEntity.getMonsterId();
		equipEntity.setMonsterId(GsConst.EQUIP_NOT_DRESS);
		equipEntity.notifyUpdate(true);

		HSEquipMonsterUndressRet.Builder response = HSEquipMonsterUndressRet.newBuilder();
		response.setId(id);
		response.setMonsterId(monsterId);
		sendProtocol(HawkProtocol.valueOf(HS.code.EQUIP_MONSTER_UNDRESS_S_VALUE, response));

		MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(monsterId);
		if (monsterEntity == null) {
			throw new RuntimeException("monster not found " + monsterId);
		}

		MonsterCfg monsterCfg = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, monsterEntity.getCfgId());
		if (monsterCfg == null) {
			throw new RuntimeException("monster config not found " + monsterEntity.getCfgId());
		}

		BILogger.getBIData(BIEquipFlowData.class).log(player, itemCfg, protocol.getId(), monsterCfg, monsterId, equipEntity.getStage(), equipEntity.getLevel(), false);
	}

	/*
	 * 替换装备
	 */
	public void onEquipReplaceOnMonster(int hsCode, HSEquipMonsterReplace protocol) {
		long id = protocol.getId();
		int monsterId = protocol.getMonsterId();

		EquipEntity newEntity = player.getPlayerData().getEquipById(id);
		if (newEntity == null) {
			sendError(hsCode, Status.itemError.ITEM_NOT_FOUND_VALUE);
			return ;
		}

		ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, newEntity.getItemId());
		if(itemCfg == null) {
			sendError(hsCode, Status.error.CONFIG_NOT_FOUND);
			return ;
		}

		if (newEntity.getMonsterId() != GsConst.EQUIP_NOT_DRESS) {
			sendError(hsCode, Status.itemError.EQUIP_DRESS_ALREADY);
			return ;
		}

		if (player.getPlayerData().isMonsterEquipOnPart(monsterId, itemCfg.getPart()) == false) {
			sendError(hsCode, Status.itemError.EQUIP_NOT_DRESS_OTHER_VALUE);
			return ;
		}

		long oldId = player.getPlayerData().getMonsterEquipIdOnPart(monsterId, itemCfg.getPart());

		EquipEntity oldEntity = player.getPlayerData().getEquipById(oldId);
		if (oldEntity == null) {
			sendError(hsCode, Status.itemError.ITEM_NOT_FOUND_VALUE);
			return ;
		}

		if (player.getPlayerData().replaceMonsterEquip(monsterId, oldEntity, newEntity, itemCfg.getPart()) == false) {
			sendError(hsCode, Status.error.SERVER_ERROR);
			return ;
		}

		newEntity.setMonsterId(monsterId);
		newEntity.notifyUpdate(true);

		oldEntity.setMonsterId(GsConst.EQUIP_NOT_DRESS);
		oldEntity.notifyUpdate(true);

		updateEquipStageStatistics(monsterId, newEntity.getStage());

		HSEquipMonsterReplaceRet.Builder response = HSEquipMonsterReplaceRet.newBuilder();
		response.setId(id);
		response.setMonsterId(monsterId);
		sendProtocol(HawkProtocol.valueOf(HS.code.EQUIP_MONSTER_REPLACE_S_VALUE, response));

		MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(monsterId);
		if (monsterEntity == null) {
			throw new RuntimeException("monster not found " + monsterId);
		}

		MonsterCfg monsterCfg = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, monsterEntity.getCfgId());
		if (monsterCfg == null) {
			throw new RuntimeException("monster config not found " + monsterEntity.getCfgId());
		}

		BILogger.getBIData(BIEquipFlowData.class).log(player, itemCfg, protocol.getId(), monsterCfg, monsterId, oldEntity.getStage(), oldEntity.getLevel(), false);

		BILogger.getBIData(BIEquipFlowData.class).log(player, itemCfg, protocol.getId(), monsterCfg, monsterId, newEntity.getStage(), newEntity.getLevel(), true);

	}

	/*
	 * 装备分解
	 */
	public void onEquipDecompose(int hsCode, HSEquipDecompose protocol) {
		ConsumeItems consume = new ConsumeItems();
		AwardItems award = new AwardItems();
		for(long equipId : protocol.getEquipIdList()){
			EquipEntity equipEntity = player.getPlayerData().getEquipById(equipId); 
			if (equipEntity == null) {
				sendError(HS.code.EQUIP_DECOMPOSE_C_VALUE, Status.itemError.EQUIP_NOT_FOUND_VALUE);
				return ;
			}

			// 宝石
			if (equipEntity.GetGemDressList().isEmpty() == false) {
				for (Map.Entry<Integer, GemInfo> entry : equipEntity.GetGemDressList().entrySet()) {
					award.addItem(entry.getValue().getGemId(), 1);
				}
			}

			consume.addEquip(equipId, equipEntity.getItemId());
			award.addItemInfos(EquipForgeCfg.getDecomposeDemandList(equipEntity.getStage(), equipEntity.getLevel()));
		}


		if (consume.checkConsume(player, HS.code.MONSTER_DECOMPOSE_C_VALUE) == false) {
			return ;
		}

		consume.consumeTakeAffectAndPush(player, Action.EQUIP_DISENCHANT, hsCode);
		award.rewardTakeAffectAndPush(player, Action.EQUIP_DISENCHANT, hsCode);

		HSEquipDecompose.Builder response = HSEquipDecompose.newBuilder();
		sendProtocol(HawkProtocol.valueOf(HS.code.EQUIP_DECOMPOSE_S_VALUE, response));
		return ;
	}

	/**
	 * 穿新装备或已穿装备stage发生变化后，更新统计数据
	 * @param monsterId 穿装备的怪物id
	 * @param newStage 发生变化的装备的品级
	 */
	private void updateEquipStageStatistics(int monsterId, int newStage) {
		// 某只怪身上装备，从1~newStage，>=每个品级的装备数量
		// 与statistics比较，如果更大，则更新
		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		Map<Integer, Long> monsterEquipMap = player.getPlayerData().getMonsterEquips(monsterId);
		int[] countOverStageList = new int[newStage + 1];

		for (Map.Entry<Integer, Long> entry : monsterEquipMap.entrySet()) {
			EquipEntity monsterEquip = player.getPlayerData().getEquipById(entry.getValue());
			for (int i = 1; i <= newStage; ++i) {
				if (monsterEquip.getStage() >= i) {
					countOverStageList[i] += 1;
				}
			}
		}
		boolean update = false;
		for (int i = 1; i <= newStage; ++i) {
			if (countOverStageList[i] > statisticsEntity.getEquipMaxCountOverStage(i)) {
				statisticsEntity.setEquipMaxCountOverStage(i, countOverStageList[i]);
				update = true;
			}
			if (countOverStageList[i] > statisticsEntity.getEquipMaxCountOverStageDaily(i)) {
				statisticsEntity.setEquipMaxCountOverStageDaily(i, countOverStageList[i]);
				update = true;
			}
		}
		if (true == update) {
			statisticsEntity.notifyUpdate(true);
		}
	}
}
