package com.hawk.game.module;

import java.util.List;

import org.hawk.config.HawkConfigManager;
import org.hawk.net.protocol.HawkProtocol;

import com.hawk.game.config.ItemCfg;
import com.hawk.game.config.RewardCfg;
import com.hawk.game.entity.ItemEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.item.ItemInfo;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerData;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Item.HSItemBuy;
import com.hawk.game.protocol.Item.HSItemBuyRet;
import com.hawk.game.protocol.Item.HSItemCompose;
import com.hawk.game.protocol.Item.HSItemComposeRet;
import com.hawk.game.protocol.Item.HSItemUse;
import com.hawk.game.protocol.Item.HSItemUseRet;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.ConfigUtil;
import com.hawk.game.util.GsConst;

public class PlayerItemModule extends PlayerModule{
	/**
	 * 构造函数
	 * 
	 * @param player
	 */
	public PlayerItemModule(Player player) {
		super(player);
		listenProto(HS.code.ITEM_USE_C);
		listenProto(HS.code.ITEM_SELL_C);
		listenProto(HS.code.ITEM_BUY_C);
		listenProto(HS.code.ITEM_COMPOSE_C);
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
			onItemUse(protocol.getType(),protocol.parseProtocol(HSItemUse.getDefaultInstance()));
			return true;
		}
		else if(protocol.checkType(HS.code.ITEM_SELL_C)) {
			//道具出售
			//onItemSell(protocol.parseProtocol(hsItemSell.getDefaultInstance()));
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
		return false;
	}
	
	/**
	 * 玩家上线处理
	 * 
	 * @return
	 */
	@Override
	protected boolean onPlayerLogin() {
		player.getPlayerData().loadItemEntities();
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
		int itemId = protocol.getItemId();
		int itemCount = protocol.getItemCount();
		if (itemId <= 0 || itemCount <= 0) {
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
	
		consumeItem.consumeTakeAffectAndPush(player, Action.ITEM_BUY);
		AwardItems awardItems = new AwardItems();
		awardItems.addItem(itemId, itemCount);
		awardItems.rewardTakeAffectAndPush(player, Action.ITEM_BUY);
		
		HSItemBuyRet.Builder response = HSItemBuyRet.newBuilder();
		response.setItemId(itemId);
		response.setItemCount(itemCount);
		sendProtocol(HawkProtocol.valueOf(HS.code.ITEM_BUY_S_VALUE, response));
	}
	
	/**
	 * 使用道具
	 * @param hsCode
	 * @param protocol
	 */
	private void onItemUse(int hsCode, HSItemUse protocol) {
		int itemId = protocol.getItemId();
		
		if (itemId <= 0 ) {
			sendError(hsCode, Status.error.PARAMS_INVALID);
			return ;
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
		
		ConsumeItems consumeItems = ConsumeItems.valueOf();
		AwardItems awardItems = new AwardItems();
		
		int useType = itemCfg.getType();
		
		if (useType == Const.toolType.FRAGMENTTOOL_VALUE) {
			consumeItems.addItem(itemId, itemCfg.getNeedCount());
			if (consumeItems.checkConsume(player, hsCode) == false) {
				return;
			}
			consumeItems.consumeTakeAffectAndPush(player, Action.TOOL_USE);
			
			List<ItemInfo> targetList = itemCfg.getTargetItemList();
			awardItems.addItemInfos(targetList);
			awardItems.rewardTakeAffectAndPush(player,  Action.TOOL_USE);
		}
		else if (useType == Const.toolType.BOXTOOL_VALUE) {
			consumeItems.addItem(itemId, 1);
			consumeItems.addItemInfos(itemCfg.getNeedItemList());
			if (consumeItems.checkConsume(player, hsCode) == false) {
				return;
			}
			
			consumeItems.consumeTakeAffectAndPush(player, Action.TOOL_USE);
			
			RewardCfg reward = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, itemCfg.getRewardId());
			if (reward == null) {
				return;
			}
			awardItems.addItemInfos(reward.getRewardList());
			awardItems.rewardTakeAffect(player, Action.TOOL_USE);
		}
		
		
		HSItemUseRet.Builder response = HSItemUseRet.newBuilder();
		response.setItemId(itemId);
		sendProtocol(HawkProtocol.valueOf(HS.code.ITEM_BUY_S_VALUE, response));
	}
	
	/**
	 * 合成道具
	 * @param hsCode
	 * @param protocol
	 */
	private void onItemCompose(int hsCode, HSItemCompose protocol) {
		int itemId = protocol.getItemId();
		if (itemId <= 0 ) {
			sendError(hsCode, Status.error.PARAMS_INVALID);
			return ;
		}
		
		ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, itemId);
		if(itemCfg == null) {
			sendError(hsCode, Status.error.CONFIG_NOT_FOUND);
			return ;
		}
		
		if (itemCfg.getComponentItemList() == null || itemCfg.getComponentItemList().size() == 0) {
			sendError(hsCode, Status.itemError.ITEM_NOT_ENOUGH);
			return ;
		}		
		
		ConsumeItems consumeItems = ConsumeItems.valueOf();
		consumeItems.addItemInfos(itemCfg.getNeedItemList());
		if (consumeItems.checkConsume(player, hsCode) == false) {
			return;
		}
		
		consumeItems.consumeTakeAffectAndPush(player, Action.TOOL_USE);
		
		AwardItems awardItems = new AwardItems();
		awardItems.addItem(itemId, 1);
		awardItems.rewardTakeAffect(player, Action.TOOL_USE);
		
		HSItemComposeRet.Builder response = HSItemComposeRet.newBuilder();
		response.setItemId(itemId);
		sendProtocol(HawkProtocol.valueOf(HS.code.ITEM_COMPOSE_S_VALUE, response));
	}
}
