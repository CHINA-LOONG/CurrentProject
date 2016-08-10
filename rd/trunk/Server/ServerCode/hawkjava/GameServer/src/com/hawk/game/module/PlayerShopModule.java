package com.hawk.game.module;

import java.util.List;

import org.hawk.annotation.MessageHandler;
import org.hawk.annotation.ProtocolHandler;
import org.hawk.config.HawkConfigManager;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkException;
import org.hawk.os.HawkRand;

import com.hawk.game.config.GoldChangeCfg;
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
import com.hawk.game.protocol.Shop.HSShopDataInitRet;
import com.hawk.game.protocol.Shop.HSShopDataSyn;
import com.hawk.game.protocol.Shop.HSShopDataSynRet;
import com.hawk.game.protocol.Shop.HSShopGold2CoinRet;
import com.hawk.game.protocol.Shop.HSShopItemBuy;
import com.hawk.game.protocol.Shop.HSShopItemBuyRet;
import com.hawk.game.protocol.Shop.HSShopRefresh;
import com.hawk.game.protocol.Shop.HSShopRefreshRet;
import com.hawk.game.protocol.Status;
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
	private boolean onShopGold2Coin(HawkProtocol cmd) throws HawkException{
		GoldChangeCfg goldChangeCfg = HawkConfigManager.getInstance().getConfigByKey(GoldChangeCfg.class, GsConst.GOLD_TO_COIN_INDEX);
		if (goldChangeCfg == null) {
			sendError(HS.code.SHOP_GOLD2COIN_C_VALUE, Status.error.CONFIG_ERROR_VALUE);
			return true;
		}

		int changeTimes = player.getPlayerData().getStatisticsEntity().getCoinOrderCountDaily();
		if (changeTimes >= goldChangeCfg.getMaxTimes()) {
			sendError(HS.code.SHOP_GOLD2COIN_C_VALUE, Status.shopError.SHOP_GOLD2COIN_MAX_COUNT_VALUE);
			return true;
		}
		
		ConsumeItems consume = new ConsumeItems();
		int consumeMutiple = (int)Math.pow(2, changeTimes / goldChangeCfg.getConsumeTimeDoubel());
		int goldCost = (goldChangeCfg.getConsume() + changeTimes * goldChangeCfg.getConsumeTimeAdd()) * consumeMutiple;
		consume.addGold(goldCost);
		
		if (consume.checkConsume(player, HS.code.SHOP_GOLD2COIN_C_VALUE) == false) {
			return true;
		}
		
		AwardItems award = new AwardItems();
		int multipleValue = 1;
		float randomValue = HawkRand.randFloat(0, 1);
		if (randomValue < goldChangeCfg.getTenMultiple()) {
			multipleValue = 10;
		}
		else if (randomValue < goldChangeCfg.getTenMultiple() + goldChangeCfg.getFiveMultiple()) {
			multipleValue = 5;	
		}
		else if (randomValue < goldChangeCfg.getTenMultiple() + goldChangeCfg.getFiveMultiple() + goldChangeCfg.getTwoMultiple()) {
			multipleValue = 2;	
		}
		
		int coinAward = (int) ((goldChangeCfg.getAward() + changeTimes * goldChangeCfg.getAwardTimeAdd()) * (1 + player.getLevel() * goldChangeCfg.getLevelAdd()));
		coinAward *= multipleValue;
		
		award.addCoin(coinAward);
		
		consume.consumeTakeAffectAndPush(player, Action.SHOP_GOLD2COIN, HS.code.SHOP_GOLD2COIN_C_VALUE);
		award.rewardTakeAffectAndPush(player, Action.SHOP_GOLD2COIN, HS.code.SHOP_GOLD2COIN_C_VALUE);
		
		player.getPlayerData().getStatisticsEntity().addCoinOrderCount();
		player.getPlayerData().getStatisticsEntity().addCoinOrderCountDaily();
		player.getPlayerData().getStatisticsEntity().notifyUpdate(true);
		
		HSShopGold2CoinRet.Builder response = HSShopGold2CoinRet.newBuilder();
		response.setChangeCount(player.getPlayerData().getStatisticsEntity().getCoinOrderCountDaily());
		response.setMultiple(multipleValue);
		response.setTotalReward(coinAward);
		sendProtocol(HawkProtocol.valueOf(HS.code.SHOP_GOLD2COIN_S_VALUE, response));
		return true;
	}
	
	@ProtocolHandler(code = HS.code.SHOP_DATA_INIT_C_VALUE)
	private boolean onShopDataInit(HawkProtocol cmd){
		HSShopDataInitRet.Builder response = HSShopDataInitRet.newBuilder();
		response.addShopDatas(ShopUtil.generateShopData(player, Const.shopType.NORMALSHOP_VALUE));
		response.addShopDatas(ShopUtil.generateShopData(player, Const.shopType.ALLIANCESHOP_VALUE));
		response.addShopDatas(ShopUtil.generateShopData(player, Const.shopType.TOWERSHOP_VALUE));
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
		int hsCode = cmd.getType();
		ShopEntity shopEntity = player.getPlayerData().getShopEntity();
		ShopItemInfo itemInfo = shopEntity.getShopItemsList(protocol.getType()).get(protocol.getSlot());

		if (itemInfo == null) {
			sendError(hsCode, Status.error.PARAMS_INVALID_VALUE);
			return true;
		}
		
		if (protocol.getShopId() != shopEntity.getShopId(protocol.getType())) {
			sendError(hsCode, Status.shopError.SHOP_REFRESH_TIMEOUT_VALUE);
			return true;
		}
		
		if (shopEntity.getShopItemsList(protocol.getType()).get(protocol.getSlot()).isHasBuy() == true) {
			sendError(hsCode, Status.shopError.SHOP_ITEM_ALREADY_BUY_VALUE);
			return true;
		}
			
		ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, shopEntity.getShopItemsList(protocol.getType()).get(protocol.getSlot()).getItemId());
		if (itemCfg == null) {
			sendError(hsCode, Status.itemError.ITEM_NOT_FOUND_VALUE);
			return true;	
		}

		if (itemCfg.getBuyPrice() == GsConst.UNUSABLE) {
			sendError(hsCode, Status.itemError.ITEM_BUY_NOT_ALLOW);
			return true;
		}

		ConsumeItems consume = new ConsumeItems();
		if (protocol.getType() == Const.shopType.NORMALSHOP_VALUE) {
			if (itemCfg.getBuyType() == Const.moneyType.MONEY_COIN_VALUE) {
				consume.addCoin((int)(itemInfo.getCount() * itemInfo.getPrice() * itemInfo.getDiscount()));
			}
			else{
				consume.addGold((int)(itemInfo.getCount() * itemInfo.getPrice() * itemInfo.getDiscount()));
			}
		}
		else if (protocol.getType() == Const.shopType.ALLIANCESHOP_VALUE) {
			consume.addContribution((int)(itemInfo.getCount() * itemInfo.getPrice() * itemInfo.getDiscount()));
		}
		else if (protocol.getType() == Const.shopType.TOWERSHOP_VALUE) {
			consume.addTowerCoin((int)(itemInfo.getCount() * itemInfo.getPrice() * itemInfo.getDiscount()));
		}

		if (consume.checkConsume(player, hsCode) == false) {
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
		
		consume.consumeTakeAffectAndPush(player, Action.SHOP_ITEM_BUY, hsCode);
		award.rewardTakeAffectAndPush(player, Action.SHOP_ITEM_BUY, hsCode);
		shopEntity.getShopItemsList(protocol.getType()).get(protocol.getSlot()).setHasBuy(true);
		shopEntity.notifyUpdate(true);

		HSShopItemBuyRet.Builder response = HSShopItemBuyRet.newBuilder();
		sendProtocol(HawkProtocol.valueOf(HS.code.SHOP_ITEM_BUY_S_VALUE, response));

		return true;
	}

	@Override
	public boolean onPlayerRefresh(List<Integer> refreshIndexList, boolean onLogin) {
		ShopEntity shopEntity = player.getPlayerData().loadShop();

		for (int index : refreshIndexList) {
			int mask = GsConst.PlayerRefreshMask[index];
			if (0 != (mask & GsConst.RefreshMask.DAILY )) {
				shopEntity.setAllianceRefreshNums(0);
				shopEntity.setNormalRefreshNums(0);
				shopEntity.setTowerRefreshNums(0);
				shopEntity.notifyUpdate(true);
				if (false == onLogin) {
					player.getPlayerData().syncShopRefreshTimeInfo();
				}

			} else if (0 != (mask & GsConst.RefreshMask.SHOP_NORMAL)) {
				ShopUtil.refreshShopData(Const.shopType.NORMALSHOP_VALUE, player);
				if (false == onLogin) {
					player.getPlayerData().syncShopRefreshInfo(Const.shopType.NORMALSHOP_VALUE);
				}

			} else if (0 != (mask & GsConst.RefreshMask.SHOP_ALLIANCE)) {
				ShopUtil.refreshShopData(Const.shopType.ALLIANCESHOP_VALUE, player);
				if (false == onLogin) {
					player.getPlayerData().syncShopRefreshInfo(Const.shopType.ALLIANCESHOP_VALUE);
				}

			} else if (0 != (mask & GsConst.RefreshMask.SHOP_TOWER)) {
				ShopUtil.refreshShopData(Const.shopType.TOWERSHOP_VALUE, player);
				if (false == onLogin) {
					player.getPlayerData().syncShopRefreshInfo(Const.shopType.TOWERSHOP_VALUE);
				}
			}
		}

		return true;
	}
}
