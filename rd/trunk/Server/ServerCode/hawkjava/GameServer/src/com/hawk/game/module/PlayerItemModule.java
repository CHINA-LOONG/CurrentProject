package com.hawk.game.module;

import java.util.LinkedList;
import java.util.List;

import org.hawk.config.HawkConfigManager;
import org.hawk.net.protocol.HawkProtocol;

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.config.ItemCfg;
import com.hawk.game.config.RewardCfg;
import com.hawk.game.config.StoreCfg;
import com.hawk.game.entity.ItemEntity;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.statistics.StatisticsEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Item.GemSelect;
import com.hawk.game.protocol.Item.HSGemCompose;
import com.hawk.game.protocol.Item.HSGemComposeRet;
import com.hawk.game.protocol.Item.HSItemBoxUseBatch;
import com.hawk.game.protocol.Item.HSItemBoxUseBatchRet;
import com.hawk.game.protocol.Item.HSItemBuy;
import com.hawk.game.protocol.Item.HSItemBuyAndUse;
import com.hawk.game.protocol.Item.HSItemBuyAndUseRet;
import com.hawk.game.protocol.Item.HSItemBuyRet;
import com.hawk.game.protocol.Item.HSItemCompose;
import com.hawk.game.protocol.Item.HSItemComposeRet;
import com.hawk.game.protocol.Item.HSItemSellBatch;
import com.hawk.game.protocol.Item.HSItemSellBatchRet;
import com.hawk.game.protocol.Item.HSItemUse;
import com.hawk.game.protocol.Item.HSItemUseRet;
import com.hawk.game.protocol.Item.ItemSell;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.ItemUtil;

public class PlayerItemModule extends PlayerModule{
	/**
	 * 构造函数
	 * 
	 * @param player
	 */
	public PlayerItemModule(Player player) {
		super(player);
		listenProto(HS.code.ITEM_USE_C);
		listenProto(HS.code.ITEM_BUY_C);
		listenProto(HS.code.ITEM_COMPOSE_C);
		listenProto(HS.code.ITEM_BOX_USE_BATCH_C);
		listenProto(HS.code.ITEM_SELL_BATCH_C_VALUE);
		listenProto(HS.code.GEM_COMPOSE_C_VALUE);
		listenProto(HS.code.ITEM_BUY_AND_USE_C_VALUE);
	}
	
	/**
	 * 协议响应
	 * 
	 * @param protocol
	 * @return
	 */
	@Override
	public boolean onProtocol(HawkProtocol protocol) {
		if (protocol.checkType(HS.code.ITEM_USE_C)) {
			// 使用道具
			onItemUse(protocol.getType(), protocol.parseProtocol(HSItemUse.getDefaultInstance()));
			return true;
		}
		else if(protocol.checkType(HS.code.ITEM_SELL_BATCH_C_VALUE)) {
			//批量出售道具出售
			onItemSell(protocol.getType(), protocol.parseProtocol(HSItemSellBatch.getDefaultInstance()));
			return true;
		}
		else if(protocol.checkType(HS.code.ITEM_BUY_C)) {
			//道具购买
			onItemBuy(protocol.getType(),protocol.parseProtocol(HSItemBuy.getDefaultInstance()));
			return true;
		}
		else if(protocol.checkType(HS.code.ITEM_BUY_AND_USE_C)) {
			//道具购买并使用
			onItemBuyAndUse(protocol.getType(),protocol.parseProtocol(HSItemBuyAndUse.getDefaultInstance()));
			return true;
		}
		else if(protocol.checkType(HS.code.ITEM_COMPOSE_C)) {
			//道具合成
			onItemCompose(protocol.getType(),protocol.parseProtocol(HSItemCompose.getDefaultInstance()));
			return true;
		}
		else if(protocol.checkType(HS.code.ITEM_BOX_USE_BATCH_C)) {
			//批量开宝箱
			onItemBoxUseBatch(protocol.getType(),protocol.parseProtocol(HSItemBoxUseBatch.getDefaultInstance()));
			return true;
		}
		else if(protocol.checkType(HS.code.GEM_COMPOSE_C)) {
			//合成宝石
			onGemCompose(protocol.getType(),protocol.parseProtocol(HSGemCompose.getDefaultInstance()));
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
		player.getPlayerData().loadAllItem();
		player.getPlayerData().syncItemInfo();
		return true;
	}
	
	/**
	 * 组装完成
	 */
	@Override
	protected boolean onPlayerAssemble() {
		return true;
	}

	/**
	 * 购买
	 * @param hsCode
	 * @param protocol
	 */
	private void onItemBuy(int hsCode, HSItemBuy protocol) {
		String itemId = protocol.getItemId();
		int itemCount = protocol.getItemCount();
		if (itemCount <= 0) {
			sendError(hsCode, Status.error.PARAMS_INVALID);
			return ;
		}

		ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, itemId);
		if(itemCfg == null) {
			sendError(hsCode, Status.error.CONFIG_NOT_FOUND);
			return ;
		}
		
		if (itemCfg.getType() == Const.toolType.EQUIPTOOL_VALUE) {
			sendError(hsCode, Status.error.PARAMS_INVALID);
			return ;
		}
		
		StoreCfg storeCfg = HawkConfigManager.getInstance().getConfigByKey(StoreCfg.class, itemId);
		if (storeCfg == null) {
			sendError(hsCode, Status.error.CONFIG_ERROR_VALUE);
			return ;
		}
		
		ConsumeItems consumeItem = new ConsumeItems();
		int price = (int)Math.ceil(storeCfg.getPrice() * storeCfg.getDiscount());
		consumeItem.addGold((int) (price * itemCount));
		if (consumeItem.checkConsume(player, hsCode) == false) {
			return ;
		}
		
		Action action = Action.NULL;
		if (itemCfg.getSubType() == Const.UseToolSubType.USETOOLFATIGUE_VALUE) {
			action = Action.ENERGY_BUY;
		}
		else if (itemCfg.getId().equals(GsConst.SWEEP_TICKET)) {
			action = Action.RAID_TICKEY_BUY;
		}
		else if (itemCfg.getSubType() == Const.UseToolSubType.USETOOLDOUBLEEXP_VALUE ||
				 itemCfg.getSubType() == Const.UseToolSubType.USETOOLTRIPLEEXP_VALUE) {
			action = Action.EXP_SCROLL_USE;
		}
		else if (itemCfg.getSubType() == Const.UseToolSubType.USETOOLEXP_VALUE) {
			action = Action.EXP_POTION_USE;
		}
		
		consumeItem.consumeTakeAffectAndPush(player, action, hsCode);

		AwardItems awardItems = new AwardItems();
		awardItems.addItem(itemId, itemCount);
		awardItems.rewardTakeAffectAndPush(player, action, hsCode);

		HSItemBuyRet.Builder response = HSItemBuyRet.newBuilder();
		response.setItemId(itemId);
		response.setItemCount(itemCount);
		sendProtocol(HawkProtocol.valueOf(HS.code.ITEM_BUY_S_VALUE, response));
	}

	/**
	 * 购买并使用
	 * @param hsCode
	 * @param protocol
	 */
	private void onItemBuyAndUse(int hsCode, HSItemBuyAndUse protocol) {
		String itemId = protocol.getItemId();
		int itemCount = protocol.getItemCount();
		if (itemCount <= 0) {
			sendError(hsCode, Status.error.PARAMS_INVALID);
			return;
		}

		ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, itemId);
		if(itemCfg == null) {
			sendError(hsCode, Status.error.CONFIG_NOT_FOUND);
			return;
		}

		// 只处理体力药一种情况
		if (itemCfg.getSubType() != Const.UseToolSubType.USETOOLFATIGUE_VALUE) {
			sendError(hsCode, Status.error.PARAMS_INVALID);
			return;
		}

		StoreCfg storeCfg = HawkConfigManager.getInstance().getConfigByKey(StoreCfg.class, itemId);
		if (storeCfg == null) {
			sendError(hsCode, Status.error.CONFIG_ERROR_VALUE);
			return ;
		}
		
		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		int maxUseCount = itemCfg.getTimes();
		int useCount = GsConst.UNUSABLE;
		if (maxUseCount != GsConst.UNUSABLE) {
			useCount = statisticsEntity.getUseItemCountDaily(itemId) + itemCount;
			if (useCount > maxUseCount) {
				sendError(hsCode, Status.itemError.ITEM_USE_COUNT);
				return;
			}
		}
		
		ConsumeItems consumeItem = new ConsumeItems();
		consumeItem.addGold((int) (storeCfg.getPrice() * storeCfg.getDiscount() * itemCount));
		
		AwardItems awardItems = AwardItems.valueOf();
		awardItems.addAttr(Const.changeType.CHANGE_FATIGUE_VALUE, itemCfg.getAddAttrValue() * itemCount);

		// 检测消耗数量和奖励上限
		if (consumeItem.checkConsume(player, hsCode) == false) {
			return;
		}
		if (awardItems.checkLimit(player, hsCode) == false) {
			return;
		}

		consumeItem.consumeTakeAffectAndPush(player, Action.ENERGY_BUY, hsCode);
		awardItems.rewardTakeAffectAndPush(player, Action.ENERGY_COOKIES_USE, hsCode);

		// 特殊情况，直接在此统计
		statisticsEntity.increaseBuyItemTimes(itemId);
		statisticsEntity.increaseUseItemCount(itemId, itemCount);
		statisticsEntity.increaseUseItemCountDaily(itemId, itemCount);
		statisticsEntity.notifyUpdate(true);

		HSItemBuyAndUseRet.Builder response = HSItemBuyAndUseRet.newBuilder();
		response.setItemId(itemId);
		response.setUseCountDaily(useCount);
		sendProtocol(HawkProtocol.valueOf(HS.code.ITEM_BUY_AND_USE_S_VALUE, response));
	}

	/**
	 * 出售
	 * @param hsCode
	 * @param protocol
	 */
	private void onItemSell(int hsCode, HSItemSellBatch protocol) {
		if (protocol.getItemsList().isEmpty() == true) {
			sendError(hsCode, Status.error.PARAMS_INVALID);
			return ;
		}
		
		AwardItems award = new AwardItems();
		ConsumeItems consume = new ConsumeItems();
		for (ItemSell element : protocol.getItemsList()) {
			String itemId = element.getItemId();
			int itemCount = element.getCount();
	
			if (itemCount <= 0) {
				sendError(hsCode, Status.error.PARAMS_INVALID);
				return ;
			}

			ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, itemId);
			if(itemCfg == null) {
				sendError(hsCode, Status.error.CONFIG_NOT_FOUND);
				return ;
			}
			
			if (itemCfg.getType() == Const.toolType.EQUIPTOOL_VALUE) {
				sendError(hsCode, Status.error.PARAMS_INVALID);
				return ;
			}
			
			if(itemCfg.getSellPrice() == GsConst.UNUSABLE) {
				sendError(hsCode, Status.itemError.ITEM_SELL_NOT_ALLOW);
				return ;
			}
			
			Const.changeType changeType = itemCfg.getSellType() == Const.moneyType.MONEY_COIN_VALUE ? Const.changeType.CHANGE_COIN : Const.changeType.CHANGE_GOLD;
			award.addAttr(changeType.getNumber(), itemCfg.getSellPrice() * itemCount);
		
		    consume.addItem(itemId, itemCount);
		}
		
		if (consume.checkConsume(player, hsCode) == false) {
			return ;
		}
	
		award.rewardTakeAffectAndPush(player, Action.ITEM_SELL, hsCode);
		consume.consumeTakeAffectAndPush(player, Action.ITEM_SELL, hsCode);
		
		HSItemSellBatchRet.Builder response = HSItemSellBatchRet.newBuilder();
		sendProtocol(HawkProtocol.valueOf(HS.code.ITEM_SELL_BATCH_S_VALUE, response));
	}
	
	/**
	 * 批量开宝箱
	 * @param hsCode
	 * @param protocol
	 */
	private void onItemBoxUseBatch(int hsCode, HSItemBoxUseBatch protocol) {
		String itemId = protocol.getItemId();
		int itemCount = protocol.getItemCount();

		ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, itemId);
		if(itemCfg == null) {
			sendError(hsCode, Status.error.CONFIG_NOT_FOUND);
			return ;
		}

		if (itemCount == 0 || itemCfg.getType() != Const.toolType.BOXTOOL_VALUE) {
			sendError(hsCode, Status.error.PARAMS_INVALID);
			return ;
		}

		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		int maxUseCount = itemCfg.getTimes();
		if (maxUseCount != GsConst.UNUSABLE) {
			if (statisticsEntity.getUseItemCountDaily(itemId) + itemCount > maxUseCount) {
				sendError(hsCode, Status.itemError.ITEM_USE_COUNT);
				return;
			}
		}

		List<AwardItems> awardItemsList = new LinkedList<>(); 
		ConsumeItems consumeItems = new ConsumeItems();
		consumeItems.addItem(itemId, itemCount);

		for (int i = 0 ; i < itemCount; ++i) {
			consumeItems.addItemInfos(itemCfg.getNeedItemList());
			RewardCfg reward = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, itemCfg.getRewardId());
			if (reward == null) {
				return;
			}
			AwardItems awardItems = new AwardItems();
			awardItems.addItemInfos(reward.getRewardList());
			awardItemsList.add(awardItems);
		}

		if (consumeItems.checkConsume(player, hsCode) == false) {
			return;
		}

		consumeItems.consumeTakeAffectAndPush(player, Action.BOX_USE, hsCode);
		for (AwardItems awardItems : awardItemsList) {
			awardItems.rewardTakeAffectAndPush(player, Action.BOX_USE, hsCode);
		}

		HSItemBoxUseBatchRet.Builder response = HSItemBoxUseBatchRet.newBuilder();
		sendProtocol(HawkProtocol.valueOf(HS.code.ITEM_BOX_USE_BATCH_S_VALUE, response));
	}
	
	/**
	 * 使用道具
	 * @param hsCode
	 * @param protocol
	 */
	private void onItemUse(int hsCode, HSItemUse protocol) {
		String itemId = protocol.getItemId();
		int count = 0;
		if (true == protocol.hasItemCount()) {
			count = protocol.getItemCount();
		}

		ItemEntity itemEntity = player.getPlayerData().getItemByItemId(itemId);
		if(itemEntity == null) {
			sendError(hsCode, Status.itemError.ITEM_NOT_FOUND);
			return ;
		}

		ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, itemEntity.getItemId());
		if(itemCfg == null) {
			sendError(hsCode, Status.error.CONFIG_NOT_FOUND);
			return ;
		}

		int useType = itemCfg.getType();
		if (useType == Const.toolType.USETOOL_VALUE) {
			if(count <= 0){
				sendError(hsCode, Status.error.PARAMS_INVALID_VALUE);
				return ;
			}
			// 经验药水
			if (itemCfg.getSubType() == Const.UseToolSubType.USETOOLEXP_VALUE) {
				if (itemCfg.getAddAttrType() != Const.changeType.CHANGE_MONSTER_EXP_VALUE || protocol.getTargetID() == 0) {
					sendError(hsCode, Status.error.PARAMS_INVALID_VALUE);
					return ;
				}

				MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(protocol.getTargetID());
				if (monsterEntity == null) {
					sendError(hsCode, Status.error.PARAMS_INVALID);
					return ;
				}

				count = ItemUtil.checkExpItemCount(monsterEntity, count, itemCfg.getAddAttrValue());
			}
		}

		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		ConsumeItems consumeItems = new ConsumeItems();
		AwardItems awardItems = new AwardItems();

		// 开宝箱走批量开接口
		if (useType == Const.toolType.BOXTOOL_VALUE) {
			sendError(hsCode, Status.error.PARAMS_INVALID_VALUE);
			return ;
		}
		else if (useType == Const.toolType.FRAGMENTTOOL_VALUE) {
			// 怪物合成走怪物合成接口
			if (itemCfg.getSubType() == Const.FragSubType.FRAG_MONSTER_VALUE) {
				sendError(hsCode, Status.error.PARAMS_INVALID_VALUE);
				return ;
			}
			// 物品合成
			count = itemCfg.getNeedCount();
			consumeItems.addItem(itemId, count);
			awardItems.addItemInfos(itemCfg.getTargetItemList());
		}
		else if (useType == Const.toolType.USETOOL_VALUE) {
			// 经验药水
			if (itemCfg.getSubType() == Const.UseToolSubType.USETOOLEXP_VALUE) {
				consumeItems.addItem(itemId, count);
				awardItems.addMonsterAttr(Const.changeType.CHANGE_MONSTER_EXP_VALUE, count * itemCfg.getAddAttrValue(), protocol.getTargetID());
			}
			// 双倍经验 三倍经验
			else if (itemCfg.getSubType() == Const.UseToolSubType.USETOOLDOUBLEEXP_VALUE || itemCfg.getSubType() == Const.UseToolSubType.USETOOLTRIPLEEXP_VALUE) {
				if (statisticsEntity.isMultiExpLeft() == true) {
					sendError(hsCode, Status.itemError.ITEM_EXP_LEFT_TIMES_VALUE);
					return ;
				}
				count = 1;
				consumeItems.addItem(itemId, count);
			}
			// 疲劳值
			else if (itemCfg.getSubType() == Const.UseToolSubType.USETOOLFATIGUE_VALUE) {
				count = 1;
				consumeItems.addItem(itemId, count);
				awardItems.addAttr(Const.changeType.CHANGE_FATIGUE_VALUE, itemCfg.getAddAttrValue());
			}
		}

		// 每日使用次数限制
		int maxUseCount = itemCfg.getTimes();
		int useCount = GsConst.UNUSABLE;
		if (maxUseCount != GsConst.UNUSABLE) {
			useCount = statisticsEntity.getUseItemCountDaily(itemId) + count;
			if (useCount > maxUseCount) {
				sendError(hsCode, Status.itemError.ITEM_USE_COUNT);
				return;
			}
		}

		Action action = Action.NULL;
		if (itemCfg.getSubType() == Const.UseToolSubType.USETOOLFATIGUE_VALUE) {
			action = Action.ENERGY_COOKIES_USE;
		}
		else if (itemCfg.getId().equals(GsConst.SWEEP_TICKET)) {
			action = Action.RAID_TICKET_USE;
		}
		else if (itemCfg.getSubType() == Const.UseToolSubType.USETOOLDOUBLEEXP_VALUE ||
				 itemCfg.getSubType() == Const.UseToolSubType.USETOOLTRIPLEEXP_VALUE) {
			action = Action.EXP_SCROLL_USE;
		}
		else if (itemCfg.getSubType() == Const.UseToolSubType.USETOOLEXP_VALUE) {
			action = Action.EXP_POTION_USE;
		}
		
		// 检测消耗数量和奖励上限
		if (consumeItems.hasConsumeItem() == true && consumeItems.checkConsume(player, hsCode) == false) {
			return;
		}
		if (awardItems.hasAwardItem() == true && awardItems.checkLimit(player, hsCode) == false) {
			return;
		}

		if (consumeItems.hasConsumeItem() == true) {
			consumeItems.consumeTakeAffectAndPush(player, action, hsCode);
		}
		if (awardItems.hasAwardItem() == true) {
			awardItems.rewardTakeAffectAndPush(player, action, hsCode);
		}

		if (itemCfg.getSubType() == Const.UseToolSubType.USETOOLDOUBLEEXP_VALUE ) {
			statisticsEntity.increaseDoubleExpLeft(itemCfg.getAddAttrValue());
			statisticsEntity.notifyUpdate(true);
			player.getPlayerData().syncStatisticsExpLeftInfo();
		}
		else if (itemCfg.getSubType() == Const.UseToolSubType.USETOOLTRIPLEEXP_VALUE ) {
			statisticsEntity.increaseTripleExpLeft(itemCfg.getAddAttrValue());
			statisticsEntity.notifyUpdate(true);
			player.getPlayerData().syncStatisticsExpLeftInfo();
		}
		
		HSItemUseRet.Builder response = HSItemUseRet.newBuilder();
		response.setItemId(itemId);
		response.setUseCountDaily(useCount);
		sendProtocol(HawkProtocol.valueOf(HS.code.ITEM_USE_S_VALUE, response));
	}
	
	/**
	 * 合成道具
	 * @param hsCode
	 * @param protocol
	 */
	private void onItemCompose(int hsCode, HSItemCompose protocol) {
		String itemId = protocol.getItemId();
		boolean composeAll = protocol.getComposeAll();
		
		ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, itemId);
		if(itemCfg == null) {
			sendError(hsCode, Status.error.CONFIG_NOT_FOUND);
			return ;
		}
		
		if (itemCfg.getTargetItemList() == null || itemCfg.getTargetItemList().isEmpty() == true || itemCfg.getNeedCount() == GsConst.UNUSABLE) {
			sendError(hsCode, Status.error.CONFIG_ERROR);
			return ;
		}
		
		ItemEntity itemEnitity = player.getPlayerData().getItemByItemId(itemId);
		if (itemEnitity == null || itemEnitity.getCount() <= itemCfg.getNeedCount()) {
			sendError(hsCode, Status.itemError.ITEM_NOT_ENOUGH_VALUE);
			return ;
		}
		
		int composeTimes = 1;
		if (composeAll) {
			composeTimes = itemEnitity.getCount() / itemCfg.getNeedCount();
			composeTimes = composeTimes > GsConst.COMPOSE_MAX_COUNT ? GsConst.COMPOSE_MAX_COUNT : composeTimes;
		}
		
		ConsumeItems consumeItems = ConsumeItems.valueOf();
		consumeItems.addItem(itemId, itemCfg.getNeedCount() * composeTimes);
		if (consumeItems.checkConsume(player, hsCode) == false) {
			return;
		}
		
		consumeItems.consumeTakeAffectAndPush(player, Action.MATERIAL_COMBINE, hsCode);
		AwardItems awardItems = new AwardItems();
		for (int i = 0; i < composeTimes; i++) {
			awardItems.addItemInfos(itemCfg.getTargetItemList());
		}
		awardItems.rewardTakeAffectAndPush(player, Action.MATERIAL_COMBINE, hsCode);
		
		HSItemComposeRet.Builder response = HSItemComposeRet.newBuilder();
		sendProtocol(HawkProtocol.valueOf(HS.code.ITEM_COMPOSE_S_VALUE, response));
	}
	
	/**
	 * 宝石合成
	 * @param hsCode
	 * @param protocol
	 */
	private void onGemCompose(int hsCode, HSGemCompose protocol){
		int count = 0;
		int grade = 0;
		int composeTimes = protocol.getComposeAll() ? GsConst.COMPOSE_MAX_COUNT : 1;
		for (GemSelect gem : protocol.getGemsList()) {
			ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, gem.getGemId());
			if(gem.getCount() == 0 || itemCfg == null || itemCfg.getType() != Const.toolType.GEMTOOL_VALUE) {
				sendError(hsCode, Status.error.PARAMS_INVALID_VALUE);
				return ;
			}
			
			if (grade == 0) {
				grade = itemCfg.getGrade();
			}
			
			if (grade != itemCfg.getGrade()) {
				sendError(hsCode, Status.error.PARAMS_INVALID_VALUE);
				return ;
			}
			
			ItemEntity itemEntity = player.getPlayerData().getItemByItemId(gem.getGemId());
			int maxCount = itemEntity.getCount() / gem.getCount();
			if (maxCount < composeTimes) {
				composeTimes = maxCount;
			}
			
			count += gem.getCount();
		}
		
		if (composeTimes <= 0) {
			sendError(hsCode, Status.itemError.ITEM_NOT_ENOUGH_VALUE);
			return ;
		}
		if (count != GsConst.GEM_COMPOSE_COUNT || grade == 0 || grade == GsConst.GEM_MAX_STAGE) {
			sendError(hsCode, Status.error.PARAMS_INVALID_VALUE);
			return ;
		}

		ConsumeItems consumeItems = ConsumeItems.valueOf();
		AwardItems awardItems = AwardItems.valueOf();
	
		for (int i = 0; i < composeTimes; i++) {
			for (GemSelect gem : protocol.getGemsList()) {
				consumeItems.addItem(gem.getGemId(), gem.getCount());
			}		
		}
		
		if (consumeItems.checkConsume(player, hsCode) == false) {
			return;
		}

		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		for (int i = 0; i < composeTimes; i++) {
			ItemCfg gemCfg = ItemUtil.generateGem(grade + 1);
			awardItems.addItem(gemCfg.getId(), GsConst.NEXT_LEVEL_GEM_COUNT);

			statisticsEntity.increaseSynAllTimes();
			statisticsEntity.increaseSynAllTimesDaily();
			statisticsEntity.increaseSynTypeTimes(gemCfg.getGemType());
		}
		statisticsEntity.notifyUpdate(true);

		consumeItems.consumeTakeAffectAndPush(player, Action.GEM_COMBINE, hsCode);
		awardItems.rewardTakeAffectAndPush(player, Action.GEM_COMBINE, hsCode);

		HSGemComposeRet.Builder response = HSGemComposeRet.newBuilder();
		sendProtocol(HawkProtocol.valueOf(HS.code.GEM_COMPOSE_S_VALUE, response));
	}
}
