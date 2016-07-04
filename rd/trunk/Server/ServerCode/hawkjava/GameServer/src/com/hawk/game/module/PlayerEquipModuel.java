package com.hawk.game.module;

import org.hawk.config.HawkConfigManager;
import org.hawk.net.protocol.HawkProtocol;

import com.hawk.game.config.ItemCfg;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.Equip.HSEquipBuy;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.HS.code;
import com.hawk.game.protocol.Item.HSItemBuy;
import com.hawk.game.protocol.Item.HSItemUse;

public class PlayerEquipModuel extends PlayerModule{
	/**
	 * 构造函数
	 * 
	 * @param player
	 */
	public PlayerEquipModuel(Player player) {
		super(player);
		listenProto(HS.code.EQUIP_SELL_C);
		listenProto(HS.code.EQUIP_BUY_C);
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

	public void onEquipBuy(int hpCode, HSEquipBuy protocol)
	{
		int equip = protocol.getEquipId();
		int equipCount = protocol.getEquipCount();
		if (equip <= 0 || equipCount <= 0) {
			sendError(hpCode, Status.error.PARAMS_INVALID);
			return ;
		}
		
		ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, equip);
		if(itemCfg == null) {
			sendError(hpCode, Status.error.CONFIG_NOT_FOUND);
			return ;
		}
		
		if (itemCfg.getType() != Const.toolType.EQUIPTOOL_VALUE) {
			sendError(hpCode, Status.error.PARAMS_INVALID);
			return ;
		}
		
		if(itemCfg.getBuyPrice() <= 0) {
			sendError(hpCode, Status.itemError.ITEM_BUY_NOT_ALLOW);
			return ;
		}
		
		Const.changeType changeType = itemCfg.getBuyType() == Const.moneyType.MONEY_COIN_VALUE ? Const.changeType.CHANGE_COIN : Const.changeType.CHANGE_GOLD;
		float price = itemCfg.getBuyPrice() * equipCount * (1 + 0.2f * protocol.getStage());
		ConsumeItems consume =	ConsumeItems.valueOf(changeType, (int)price);
		if (consume.checkConsume(player, hpCode) == false) {
			return ;
		}
		consume.consumeTakeAffectAndPush(player, Action.ITEM_BUY);
		
		AwardItems awardItems = new AwardItems();
		for (int i = 0; i < equipCount; i++) {
			awardItems.addEquip(equip, protocol.getStage(), protocol.getLevel());
		}
		awardItems.rewardTakeAffectAndPush(player, Action.ITEM_BUY);
	}

}
