package com.hawk.game.module;

import java.util.LinkedList;
import java.util.List;

import org.hawk.config.HawkConfigManager;
import org.hawk.net.protocol.HawkProtocol;

import com.hawk.game.config.ItemCfg;
import com.hawk.game.config.RewardCfg;
import com.hawk.game.entity.ItemEntity;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.item.GemInfo;
import com.hawk.game.item.ItemInfo;
import com.hawk.game.log.BehaviorLogger.Action;
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
		
		if(itemCfg.getBuyPrice() <= 0) {
			sendError(hsCode, Status.itemError.ITEM_BUY_NOT_ALLOW);
			return ;
		}
		
		Const.changeType changeType = itemCfg.getBuyType() == Const.moneyType.MONEY_COIN_VALUE ? Const.changeType.CHANGE_COIN : Const.changeType.CHANGE_GOLD;
		ConsumeItems consumeItem = ConsumeItems.valueOf(changeType, itemCfg.getBuyPrice() * itemCount);
		if (consumeItem.checkConsume(player, hsCode) == false) {
			return ;
		}
	
		consumeItem.consumeTakeAffectAndPush(player, Action.ITEM_BUY, hsCode);
		AwardItems awardItems = new AwardItems();
		awardItems.addItem(itemId, itemCount);
		awardItems.rewardTakeAffectAndPush(player, Action.ITEM_BUY, hsCode);
		
		HSItemBuyRet.Builder response = HSItemBuyRet.newBuilder();
		response.setItemId(itemId);
		response.setItemCount(itemCount);
		sendProtocol(HawkProtocol.valueOf(HS.code.ITEM_BUY_S_VALUE, response));
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
			
			if(itemCfg.getSellPrice() <= 0) {
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
		ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, protocol.getItemId());
		if(itemCfg == null) {
			sendError(hsCode, Status.error.CONFIG_NOT_FOUND);
			return ;
		}
		
		if (protocol.getItemCount() == 0 || itemCfg.getType() != Const.toolType.BOXTOOL_VALUE) {
			sendError(hsCode, Status.error.PARAMS_INVALID);
			return ;
		}
		
		List<AwardItems> awardItemsList = new LinkedList<>(); 
		ConsumeItems consumeItems = new ConsumeItems();
		consumeItems.addItem(protocol.getItemId(), protocol.getItemCount());
		
		for (int i = 0 ; i < protocol.getItemCount(); ++i) {		
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
		
		consumeItems.consumeTakeAffectAndPush(player, Action.ITEM_USE, hsCode);
		for (AwardItems awardItems : awardItemsList) {
			awardItems.rewardTakeAffectAndPush(player, Action.ITEM_USE, hsCode);
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
		int count = protocol.getItemCount();
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
		
		ConsumeItems consumeItems = new ConsumeItems();
		AwardItems awardItems = new AwardItems();
		int useType = itemCfg.getType();
		if (useType == Const.toolType.FRAGMENTTOOL_VALUE) {
			consumeItems.addItem(itemId, itemCfg.getNeedCount());
			awardItems.addItemInfos(itemCfg.getTargetItemList());
		}
		// 开宝箱走批量开接口
		else if (useType == Const.toolType.BOXTOOL_VALUE) {
			sendError(hsCode, Status.error.PARAMS_INVALID_VALUE);
			return ;
		}
		else if (useType == Const.toolType.USETOOL_VALUE) {
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
				consumeItems.addItem(itemId, count);
				awardItems.addMonsterAttr(Const.changeType.CHANGE_MONSTER_EXP_VALUE, count * itemCfg.getAddAttrValue(), protocol.getTargetID());
			}
			// 双倍经验 三倍经验
			else if (itemCfg.getSubType() == Const.UseToolSubType.USETOOLDOUBLEEXP_VALUE || itemCfg.getSubType() == Const.UseToolSubType.USETOOLTRIPLEEXP_VALUE) {
				if (player.getPlayerData().getStatisticsEntity().isExpTimeLeft() == true) {
					sendError(hsCode, Status.itemError.ITEM_EXP_LEFT_TIMES_VALUE);
					return ;
				}
				
				consumeItems.addItem(itemId, 1);
			}
			// 疲劳值
			else if (itemCfg.getSubType() == Const.UseToolSubType.USETOOLFATIGUE_VALUE) {
				awardItems.addAttr(Const.changeType.CHANGE_FATIGUE_VALUE, itemCfg.getAddAttrValue());
				consumeItems.addItem(itemId, 1);
			}			
		}		
			
		if (consumeItems.hasConsumeItem() == true) {
			if (consumeItems.checkConsume(player, hsCode) == false) {
				return;
			}
			consumeItems.consumeTakeAffectAndPush(player, Action.ITEM_USE, hsCode);
		}
		
		if (awardItems.hasAwardItem() == true) {
			if (awardItems.checkLimit(player, hsCode) == false) {
				return;
			}
			awardItems.rewardTakeAffectAndPush(player,  Action.ITEM_USE, hsCode);
		}
		
		if (itemCfg.getSubType() == Const.UseToolSubType.USETOOLDOUBLEEXP_VALUE ) {
			player.getPlayerData().getStatisticsEntity().increaseDoubleExpLeft(itemCfg.getAddAttrValue());
			player.getPlayerData().getStatisticsEntity().notifyUpdate(true);
			player.getPlayerData().syncStatisticsExpLeftInfo();
		}
		else if (itemCfg.getSubType() == Const.UseToolSubType.USETOOLTRIPLEEXP_VALUE ) {
			player.getPlayerData().getStatisticsEntity().increaseTripleExpLeft(itemCfg.getAddAttrValue());
			player.getPlayerData().getStatisticsEntity().notifyUpdate(true);
			player.getPlayerData().syncStatisticsExpLeftInfo();
		}
		
		HSItemUseRet.Builder response = HSItemUseRet.newBuilder();
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
		
		if (itemCfg.getTargetItemList() == null || itemCfg.getTargetItemList().size() == 0) {
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
		
		consumeItems.consumeTakeAffectAndPush(player, Action.ITEM_COMPOSE, hsCode);
		AwardItems awardItems = new AwardItems();
		for (int i = 0; i < composeTimes; i++) {
			awardItems.addItemInfos(itemCfg.getTargetItemList());
		}
		awardItems.rewardTakeAffectAndPush(player, Action.ITEM_COMPOSE, hsCode);
		
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
			awardItems.addItem(ItemUtil.generateGem(grade + 1).getId(), GsConst.NEXT_LEVEL_GEM_COUNT);
		}
		
		if (consumeItems.checkConsume(player, hsCode) == false) {
			return;
		}
		
		consumeItems.consumeTakeAffectAndPush(player, Action.GEM_COMPOSE, hsCode);
		awardItems.rewardTakeAffectAndPush(player, Action.GEM_COMPOSE, hsCode);
		
		HSGemComposeRet.Builder response = HSGemComposeRet.newBuilder();
		sendProtocol(HawkProtocol.valueOf(HS.code.GEM_COMPOSE_S_VALUE, response));
	}
}
