package com.hawk.game.module;

import org.hawk.app.HawkApp;
import org.hawk.config.HawkConfigManager;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;

import com.hawk.game.config.ItemCfg;
import com.hawk.game.entity.ItemEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.Consume;
import com.hawk.game.protocol.Consume.ConsumeItem;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Item.HSItemBuy;
import com.hawk.game.protocol.Item.HSItemUse;
import com.hawk.game.protocol.Player.HSAssembleFinish;
import com.hawk.game.protocol.Status;
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
			//onItemSell(protocol.parseProtocol(HPItemSell.getDefaultInstance()));
			return true;
		}
		else if(protocol.checkType(HS.code.ITEM_BUY_C)) {
			//道具购买
			onItemBuy(protocol.getType(),protocol.parseProtocol(HSItemBuy.getDefaultInstance()));
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
	 * @param hpCode
	 * @param protocol
	 */
	private void onItemBuy(int hpCode, HSItemBuy protocol) {
		int itemId = protocol.getItemId();
		int itemCount = protocol.getItemCount();
		if (itemId <= 0 || itemCount <= 0) {
			sendError(hpCode, Status.error.PARAMS_INVALID);
			return ;
		}
		
		ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, itemId);
		if(itemCfg == null) {
			sendError(hpCode, Status.error.CONFIG_NOT_FOUND);
			return ;
		}
		
		if (itemCfg.getType() == Const.toolType.EQUIPTOOL_VALUE) {
			sendError(hpCode, Status.error.PARAMS_INVALID);
			return ;
		}
		
		if(itemCfg.getBuyPrice() <= 0) {
			sendError(hpCode, Status.itemError.ITEM_BUY_NOT_ALLOW);
			return ;
		}
		
		Const.changeType changeType = itemCfg.getBuyType() == Const.moneyType.MONEY_COIN_VALUE ? Const.changeType.CHANGE_COIN : Const.changeType.CHANGE_GOLD;
		ConsumeItems.valueOf(changeType, itemCfg.getBuyPrice() * itemCount).consumeTakeAffectAndPush(player, Action.ITEM_BUY);
		AwardItems awardItems = new AwardItems();
		awardItems.addItem(itemId, itemCount);
		awardItems.rewardTakeAffectAndPush(player, Action.ITEM_BUY);
	}
	
	/**
	 * 使用道具
	 * @param hpCode
	 * @param protocol
	 */
	private void onItemUse(int hpCode, HSItemUse protocol) {
		int itemId = protocol.getItemId();
		if (itemId <= 0 ) {
			sendError(hpCode, Status.error.PARAMS_INVALID);
			return ;
		}
		 
		ItemEntity itemEntity = player.getPlayerData().getItemByItemId(itemId);
		if(itemEntity == null) {
			sendError(hpCode, Status.itemError.ITEM_NOT_FOUND);
			return ;
		}
		
		ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, itemEntity.getItemId());
		if(itemCfg == null) {
			sendError(hpCode, Status.error.CONFIG_NOT_FOUND);
			return ;
		}
		
		ConsumeItems consumeItems = ConsumeItems.valueOf();
		AwardItems awardItems = new AwardItems();
		
		int useType = itemCfg.getType();
		if (useType == Const.toolType.FRAGMENTTOOL_VALUE) {
			consumeItems.addChangeItem(itemId, itemCfg.getNeedCount());
			if (consumeItems.checkConsume(player, hpCode) == false) {
				return;
			}
			consumeItems.consumeTakeAffectAndPush(player, Action.TOOL_USE);

		
		}
		
	}
}
