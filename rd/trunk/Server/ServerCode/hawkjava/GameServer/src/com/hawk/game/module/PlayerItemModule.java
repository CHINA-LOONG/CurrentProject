package com.hawk.game.module;

import org.hawk.app.HawkApp;
import org.hawk.config.HawkConfigManager;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;

import com.hawk.game.config.ItemCfg;
import com.hawk.game.entity.ItemEntity;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.HS;
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
		} else if(protocol.checkType(HS.code.ITEM_SELL_C)) {
			//道具出售
			//onItemSell(protocol.parseProtocol(HPItemSell.getDefaultInstance()));
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
	 * 使用道具
	 * @param hpCode
	 * @param protocol
	 */
	private void onItemUse(int hpCode, HSItemUse protocol) {
		int itemId = protocol.getItemId();
		int itemCount = protocol.getItemCount();
		if (itemId <= 0 || itemCount <= 0) {
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
		
		if(itemEntity.getCount() < itemCount) {
			sendError(hpCode, Status.itemError.ITEM_NOT_ENOUGH);
			return ;
		}
		
		int useType = itemCfg.getType();
		if (useType == Const.toolType.FRAGMENT_VALUE) {
			
		}
		
	}
}
