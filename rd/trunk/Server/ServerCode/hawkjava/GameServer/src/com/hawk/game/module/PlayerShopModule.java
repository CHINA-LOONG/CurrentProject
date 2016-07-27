package com.hawk.game.module;

import java.util.List;

import org.hawk.annotation.MessageHandler;
import org.hawk.annotation.ProtocolHandler;
import org.hawk.config.HawkConfigManager;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkTime;

import com.hawk.game.config.ItemCfg;
import com.hawk.game.config.ShopCfg;
import com.hawk.game.config.SysBasicCfg;
import com.hawk.game.entity.ShopEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.item.ShopItemInfo;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Shop.HSShopDataInit;
import com.hawk.game.protocol.Shop.HSShopDataInitRet;
import com.hawk.game.protocol.Shop.HSShopDataSynRet;
import com.hawk.game.protocol.Shop.HSShopGold2Coin;
import com.hawk.game.protocol.Shop.HSShopGold2CoinRet;
import com.hawk.game.protocol.Shop.HSShopRefreshRet;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.Shop.HSShopDataSyn;
import com.hawk.game.protocol.Shop.HSShopItemBuy;
import com.hawk.game.protocol.Shop.HSShopItemBuyRet;
import com.hawk.game.protocol.Shop.HSShopRefresh;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.ShopUtil;
public class PlayerShopModule extends PlayerModule{
	/**
	 * 构造函数
	 * 
	 * @param player
	 */
	public PlayerShopModule(Player player) {
		super(player);
	}

	@Override
	public boolean onProtocol(HawkProtocol protocol) {
		return super.onProtocol(protocol);
	}

	@Override
	protected boolean onPlayerLogin() {
		player.getPlayerData().loadShop();
		
		return true;
	}

	@ProtocolHandler(code = HS.code.SHOP_GOLD2COIN_C_VALUE)
	private boolean onShopGold2Coin(HawkProtocol cmd){
		HSShopGold2Coin protocol = cmd.parseProtocol(HSShopGold2Coin.getDefaultInstance());
		ConsumeItems consume = new ConsumeItems();
		consume.addGold(100);
		
		if (consume.checkConsume(player, HS.code.SHOP_GOLD2COIN_C_VALUE) == false) {
			return true;
		}
		
		AwardItems award = new AwardItems();
		award.addCoin(10000);
		consume.consumeTakeAffectAndPush(player, Action.SHOP_GOLD2COIN, HS.code.SHOP_GOLD2COIN_C_VALUE);
		award.rewardTakeAffectAndPush(player, Action.SHOP_GOLD2COIN, HS.code.SHOP_GOLD2COIN_C_VALUE);
		
		player.getPlayerData().getStatisticsEntity().addCoinOrderCount();
		player.getPlayerData().getStatisticsEntity().addCoinOrderCountDaily();
		player.getPlayerData().getStatisticsEntity().notifyUpdate(true);
		
		HSShopGold2CoinRet.Builder response = HSShopGold2CoinRet.newBuilder();
		response.setChangeCount(player.getPlayerData().getStatisticsEntity().getCoinOrderCountDaily());
		sendProtocol(HawkProtocol.valueOf(HS.code.SHOP_GOLD2COIN_S_VALUE, response));
		return true;
	}
	
	@ProtocolHandler(code = HS.code.SHOP_DATA_INIT_C_VALUE)
	private boolean onShopDataInit(HawkProtocol cmd){
		HSShopDataInit protocol = cmd.parseProtocol(HSShopDataInit.getDefaultInstance());
		HSShopDataInitRet.Builder response = HSShopDataInitRet.newBuilder();
		response.addShopDatas(ShopUtil.generateShopData(player, Const.shopType.NORMALSHOP_VALUE));
		response.addShopDatas(ShopUtil.generateShopData(player, Const.shopType.ALLIANCESHOP_VALUE));
		response.addShopDatas(ShopUtil.generateShopData(player, Const.shopType.OTHERSHOP_VALUE));
		sendProtocol(HawkProtocol.valueOf(HS.code.SHOP_DATA_INIT_S_VALUE, response));
		return true;
	} 
	
	@ProtocolHandler(code = HS.code.SHOP_DATA_SYN_C_VALUE)
	private boolean onShopDataSyn(HawkProtocol cmd){
		HSShopDataSyn protocol = cmd.parseProtocol(HSShopDataSyn.getDefaultInstance());
		HSShopDataSynRet.Builder response = HSShopDataSynRet.newBuilder();
		response.setShopData(ShopUtil.generateShopData(player, protocol.getType()));
		sendProtocol(HawkProtocol.valueOf(HS.code.SHOP_DATA_SYN_S_VALUE, response));
		return true;
	} 
	
	@ProtocolHandler(code = HS.code.SHOP_REFRESH_C_VALUE)
	private boolean onShopRefresh(HawkProtocol cmd){
		HSShopRefresh protocol = cmd.parseProtocol(HSShopRefresh.getDefaultInstance());
		ShopEntity shopEntity = player.getPlayerData().getShopEntity();
		ShopCfg shopCfg = ShopCfg.getShopCfg(protocol.getType(), player.getLevel());
		if (shopEntity.getShopRefreshNum(protocol.getType()) >= shopCfg.getRefreshMaxNumByHand()) {
			sendError(HS.code.SHOP_REFRESH_C_VALUE, Status.shopError.SHOP_REFRESH_MAX_COUNT_VALUE);
			return true;
		}
		
		ConsumeItems consume = new ConsumeItems();
		consume.addGold(SysBasicCfg.getInstance().getShopRefreshGold());
		if (consume.checkConsume(player, HS.code.SHOP_REFRESH_C_VALUE) == false) {
			return true;
		}
		
		ShopUtil.refreshShopData(protocol.getType(), player);
		consume.consumeTakeAffectAndPush(player, Action.SHOP_REFRESH, HS.code.SHOP_REFRESH_C_VALUE);
		shopEntity.increaseShopRefreshNum(protocol.getType());
		shopEntity.notifyUpdate(true);
		
		HSShopRefreshRet.Builder response = HSShopRefreshRet.newBuilder();
		response.setShopData(ShopUtil.generateShopData(player, protocol.getType()));
		sendProtocol(HawkProtocol.valueOf(HS.code.SHOP_REFRESH_S_VALUE, response));

		return true;
	}
	
	@MessageHandler(code = GsConst.MsgType.REFRESH_SHOP)
	private boolean onShopRefresh(HawkMsg msg){
		int shopType = msg.getParam(0);
		ShopUtil.refreshShopData(shopType, player);
		return true;
	}
	
	@ProtocolHandler(code = HS.code.SHOP_ITEM_BUY_C_VALUE)
	private boolean onShopItemBuy(HawkProtocol cmd){
		HSShopItemBuy protocol = cmd.parseProtocol(HSShopItemBuy.getDefaultInstance());
		ShopEntity shopEntity = player.getPlayerData().getShopEntity();
		ShopItemInfo itemInfo = shopEntity.getShopItemsList(protocol.getType()).get(protocol.getSlot());

		if (itemInfo == null) {
			sendError(HS.code.SHOP_ITEM_BUY_C_VALUE, Status.error.PARAMS_INVALID_VALUE);
			return true;
		}
		
		if (protocol.getShopId() != shopEntity.getShopId(protocol.getType())) {
			sendError(HS.code.SHOP_ITEM_BUY_C_VALUE, Status.shopError.SHOP_REFRESH_TIMEOUT_VALUE);
			return true;
		}
		
		if (shopEntity.getShopItemsList(protocol.getType()).get(protocol.getSlot()).isHasBuy() == true) {
			sendError(HS.code.SHOP_ITEM_BUY_C_VALUE, Status.shopError.SHOP_ITEM_ALREADY_BUY_VALUE);
			return true;
		}
			
		ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, shopEntity.getShopItemsList(protocol.getType()).get(protocol.getSlot()).getItemId());
		if (itemCfg == null) {
			sendError(HS.code.SHOP_ITEM_BUY_C_VALUE, Status.itemError.ITEM_NOT_FOUND_VALUE);
			return true;	
		}
		
		ConsumeItems consume = new ConsumeItems();
		if (itemCfg.getBuyType() == Const.moneyType.MONEY_COIN_VALUE) {
			consume.addCoin((int)(itemInfo.getCount() * itemCfg.getBuyPrice() * itemInfo.getDiscount()));
		}
		else{
			consume.addGold((int)(itemInfo.getCount() * itemCfg.getBuyPrice() * itemInfo.getDiscount()));
		}
		
		if (consume.checkConsume(player, HS.code.SHOP_ITEM_BUY_C_VALUE) == false) {
			return true;
		}
		
		AwardItems award = new AwardItems();
		if (itemCfg.getType() == Const.toolType.EQUIPTOOL_VALUE) {
			award.addEquip(itemInfo.getItemId(), itemInfo.getCount(), itemInfo.getStage(), itemInfo.getLevel());
		}
		else
		{
			award.addItem(itemInfo.getItemId(), itemInfo.getCount());
		}
		
		consume.consumeTakeAffectAndPush(player, Action.SHOP_ITEM_BUY, HS.code.SHOP_ITEM_BUY_C_VALUE);
		award.rewardTakeAffectAndPush(player, Action.SHOP_ITEM_BUY, HS.code.SHOP_ITEM_BUY_C_VALUE);
		shopEntity.getShopItemsList(protocol.getType()).get(protocol.getSlot()).setHasBuy(true);
		shopEntity.notifyUpdate(true);

		HSShopItemBuyRet.Builder response = HSShopItemBuyRet.newBuilder();
		sendProtocol(HawkProtocol.valueOf(HS.code.SHOP_ITEM_BUY_S_VALUE, response));

		return true;
	}
	
	@Override
	protected boolean onRefresh(List<Integer> refreshTypeList) {	
		if (refreshTypeList.contains(GsConst.RefreshType.DAILY_PERS_REFRESH)) {
			ShopEntity shopEntity = player.getPlayerData().getShopEntity();
			shopEntity.setAllianceRefreshNums(0);
			shopEntity.setNormalRefreshNums(0);
			shopEntity.setOtherRefreshNums(0);
			player.getPlayerData().syncShopRefreshTimeInfo();
		}
		
		if (refreshTypeList.contains(GsConst.RefreshType.SHOP_REFRESH_TIME_FIRST) || 
			refreshTypeList.contains(GsConst.RefreshType.SHOP_REFRESH_TIME_SECOND)||
			refreshTypeList.contains(GsConst.RefreshType.SHOP_REFRESH_TIME_THIRD ))
		{
			ShopUtil.refreshShopData(Const.shopType.NORMALSHOP_VALUE, player);
		}
		else if (refreshTypeList.contains(GsConst.RefreshType.ALLIANCE_REFRESH_TIME_FIRST) || 
			     refreshTypeList.contains(GsConst.RefreshType.ALLIANCE_REFRESH_TIME_SECOND)||
			     refreshTypeList.contains(GsConst.RefreshType.ALLIANCE_REFRESH_TIME_THIRD ))
		{
			ShopUtil.refreshShopData(Const.shopType.ALLIANCESHOP_VALUE, player);
		}
		
		return true;
	}
}
