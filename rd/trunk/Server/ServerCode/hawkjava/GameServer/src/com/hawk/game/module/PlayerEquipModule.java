package com.hawk.game.module;

import org.hawk.config.HawkConfigManager;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkException;

import com.hawk.game.config.EquipAttr;
import com.hawk.game.config.ItemCfg;
import com.hawk.game.entity.EquipEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.log.BehaviorLogger;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.log.BehaviorLogger.Params;
import com.hawk.game.log.BehaviorLogger.Source;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.Equip.HSEquipBuy;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.Equip.HSEquipBuyRet;
import com.hawk.game.protocol.Equip.HSEquipIncreaseLevel;
import com.hawk.game.protocol.Equip.HSEquipIncreaseLevelRet;
import com.hawk.game.protocol.Equip.HSEquipIncreaseStage;
import com.hawk.game.protocol.Equip.HSEquipIncreaseStageRet;
import com.hawk.game.protocol.Equip.HSEquipMonsterDress;
import com.hawk.game.protocol.Equip.HSEquipMonsterDressRet;
import com.hawk.game.protocol.Equip.HSEquipMonsterDressRetOrBuilder;
import com.hawk.game.protocol.Equip.HSEquipMonsterReplace;
import com.hawk.game.protocol.Equip.HSEquipMonsterReplaceRet;
import com.hawk.game.protocol.Equip.HSEquipMonsterUndress;
import com.hawk.game.protocol.Equip.HSEquipMonsterUndressRet;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
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
	}
	
	/**
	 * 协议响应
	 * 
	 * @param protocol
	 * @return
	 */
	@Override
	public boolean onProtocol(HawkProtocol protocol) {
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
			//买装备
			onEquipIncreaseLevel(HS.code.EQUIP_INCREASE_LEVEL_C_VALUE, protocol.parseProtocol(HSEquipIncreaseLevel.getDefaultInstance()));
			return true;
		}
		else if(protocol.checkType(HS.code.EQUIP_INCREASE_STAGE_C)) {
			//买装备
			onEquipIncreaseStage(HS.code.EQUIP_INCREASE_STAGE_C_VALUE, protocol.parseProtocol(HSEquipIncreaseStage.getDefaultInstance()));
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

		return false;
	}
	
	/**
	 * 玩家上线处理
	 * 
	 * @return
	 */
	@Override
	protected boolean onPlayerLogin() {
		player.getPlayerData().loadEquipEntities();
		player.getPlayerData().syncEquipInfo();
		return true;
	}

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
		
		if(itemCfg.getBuyPrice() <= 0) {
			sendError(hsCode, Status.itemError.ITEM_BUY_NOT_ALLOW);
			return ;
		}
		
		Const.changeType changeType = itemCfg.getBuyType() == Const.moneyType.MONEY_COIN_VALUE ? Const.changeType.CHANGE_COIN : Const.changeType.CHANGE_GOLD;
		float price = itemCfg.getBuyPrice() * equipCount * (1 + 0.2f * protocol.getStage());
		ConsumeItems consume =	ConsumeItems.valueOf(changeType, (int)price);
		if (consume.checkConsume(player, hsCode) == false) {
			return ;
		}
		consume.consumeTakeAffectAndPush(player, Action.EQUIP_BUY);
		
		AwardItems awardItems = new AwardItems();
		for (int i = 0; i < equipCount; i++) {
			awardItems.addEquip(equid, protocol.getStage(), equipCount, protocol.getLevel());
		}
		awardItems.rewardTakeAffectAndPush(player, Action.EQUIP_BUY);
		
		HSEquipBuyRet.Builder response = HSEquipBuyRet.newBuilder();
		response.setEquipCount(equipCount);
		response.setEquipId(equid);
		sendProtocol(HawkProtocol.valueOf(HS.code.EQUIP_BUY_S_VALUE, response));
	}

	public void onEquipIncreaseLevel(int hsCode, HSEquipIncreaseLevel protocol)
	{
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
		
		// level 从0开始
		int maxLevel = EquipAttr.getLevelSize(equipEntity.getItemId(), equipEntity.getStage());
		if (equipEntity.getLevel() >= maxLevel - 1) {
			sendError(hsCode, Status.itemError.EQUIP_MAX_LEVEL_ALREADY);
			return ;
		}
	
		ConsumeItems consume = new ConsumeItems();
		consume.addItemInfos(EquipAttr.getDemandList(equipEntity.getItemId(), equipEntity.getStage(), equipEntity.getLevel() + 1));
		if (consume.checkConsume(player, hsCode) == false) {
			return;
		}
	
		consume.consumeTakeAffectAndPush(player, Action.EQUIP_EHANCE);
		equipEntity.setLevel(equipEntity.getLevel() + 1);
		equipEntity.notifyUpdate(true);
		
		HSEquipIncreaseLevelRet.Builder response = HSEquipIncreaseLevelRet.newBuilder();
		response.setId(id);
		response.setStage(equipEntity.getStage());
		response.setLevel(equipEntity.getLevel());
		sendProtocol(HawkProtocol.valueOf(HS.code.EQUIP_INCREASE_LEVEL_S, response));
		
		BehaviorLogger.log4Service(player, Source.USER_OPERATION, Action.EQUIP_EHANCE,
				Params.valueOf("itemId", equipEntity.getItemId()),
				Params.valueOf("equipId", equipEntity.getId()),
				Params.valueOf("after", equipEntity.getLevel()));	
	}
	
	public void onEquipIncreaseStage(int hsCode, HSEquipIncreaseStage protocol)
	{
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
		
		int maxLevel = EquipAttr.getLevelSize(equipEntity.getItemId(), equipEntity.getStage());
		if (equipEntity.getLevel() < maxLevel - 1) {
			sendError(hsCode, Status.itemError.EQUIP_LEVEL_NOT_ENOUGH_VALUE);
			return ;
		}	
		
		int maxStage = EquipAttr.getStageSize(equipEntity.getItemId());
		if (equipEntity.getStage() >= maxStage) {
			sendError(hsCode, Status.itemError.EQUIP_MAX_STAGE_ALREADY_VALUE);
			return ;
		}
		
		ConsumeItems consume = new ConsumeItems();
		consume.addItemInfos(EquipAttr.getDemandList(equipEntity.getItemId(), equipEntity.getStage() + 1, 0));
		if (consume.checkConsume(player, hsCode) == false) {
			return;
		}
	
		consume.consumeTakeAffectAndPush(player, Action.EQUIP_ADVANCE);
		equipEntity.setLevel(0);
		equipEntity.setStage(equipEntity.getStage() + 1);
		equipEntity.notifyUpdate(true);
		
		HSEquipIncreaseStageRet.Builder response = HSEquipIncreaseStageRet.newBuilder();
		response.setId(id);
		response.setStage(equipEntity.getStage());
		response.setLevel(equipEntity.getLevel());
		sendProtocol(HawkProtocol.valueOf(HS.code.EQUIP_INCREASE_STAGE_S, response));	
		
		BehaviorLogger.log4Service(player, Source.USER_OPERATION, Action.EQUIP_ADVANCE,
				Params.valueOf("itemId", equipEntity.getItemId()),
				Params.valueOf("equipId", equipEntity.getId()),
				Params.valueOf("after", equipEntity.getStage()));		
	}
	
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
		
		if (equipEntity.getMonsterId() != GsConst.EQUIPNOTDRESS) {
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
		
		HSEquipMonsterDressRet.Builder response = HSEquipMonsterDressRet.newBuilder();
		response.setId(id);
		response.setMonsterId(monsterId);
		sendProtocol(HawkProtocol.valueOf(HS.code.EQUIP_MONSTER_DRESS_S_VALUE, response));
		
		BehaviorLogger.log4Service(player, Source.USER_OPERATION, Action.EQUIP_DRESS,
				Params.valueOf("itemId", equipEntity.getItemId()),
				Params.valueOf("equipId", equipEntity.getId()),
				Params.valueOf("after", player.getPlayerData().monsterEquipsToString(monsterId)));
	}
	
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
		equipEntity.setMonsterId(GsConst.EQUIPNOTDRESS);
		equipEntity.notifyUpdate(true);
		
		HSEquipMonsterUndressRet.Builder response = HSEquipMonsterUndressRet.newBuilder();
		response.setId(id);
		sendProtocol(HawkProtocol.valueOf(HS.code.EQUIP_MONSTER_UNDRESS_S_VALUE, response));

		BehaviorLogger.log4Service(player, Source.USER_OPERATION, Action.EQUIP_UNDRESS,
				Params.valueOf("itemId", equipEntity.getItemId()),
				Params.valueOf("equipId", equipEntity.getId()),
				Params.valueOf("after", player.getPlayerData().monsterEquipsToString(monsterId)));
	}
	
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
		
		if (newEntity.getMonsterId() != GsConst.EQUIPNOTDRESS) {
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
		
		oldEntity.setMonsterId(GsConst.EQUIPNOTDRESS);
		oldEntity.notifyUpdate(true);
		
		HSEquipMonsterReplaceRet.Builder response = HSEquipMonsterReplaceRet.newBuilder();
		response.setId(id);
		response.setMonsterId(monsterId);
		sendProtocol(HawkProtocol.valueOf(HS.code.EQUIP_MONSTER_UNDRESS_S_VALUE, response));
	
		BehaviorLogger.log4Service(player, Source.USER_OPERATION, Action.EQUIP_UNDRESS,
				Params.valueOf("newItemId", newEntity.getItemId()),
				Params.valueOf("newEquipId", newEntity.getId()),
				Params.valueOf("oldItemId", oldEntity.getItemId()),
				Params.valueOf("oldEquipId", oldEntity.getId()),
				Params.valueOf("after", player.getPlayerData().monsterEquipsToString(monsterId)));
	}
}
