package com.hawk.game.module;

import java.util.List;

import org.hawk.annotation.MessageHandler;
import org.hawk.annotation.ProtocolHandler;
import org.hawk.config.HawkConfigManager;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkException;
import org.hawk.os.HawkRand;

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.config.GoldChangeCfg;
import com.hawk.game.config.ItemCfg;
import com.hawk.game.config.ShopCfg;
import com.hawk.game.config.StoreCfg;
import com.hawk.game.config.SysBasicCfg;
import com.hawk.game.entity.ShopEntity;
import com.hawk.game.entity.statistics.StatisticsEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.item.ShopItemInfo;
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
import com.hawk.game.protocol.Shop.HSStoreItemBuy;
import com.hawk.game.protocol.Shop.HSStoreItemBuyRet;
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

		int changeTimes = player.getPlayerData().getStatisticsEntity().getBuyCoinTimesDaily();
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
		
		consume.consumeTakeAffectAndPush(player, Action.COIN_CHANGE, HS.code.SHOP_GOLD2COIN_C_VALUE);
		award.rewardTakeAffectAndPush(player, Action.COIN_CHANGE, HS.code.SHOP_GOLD2COIN_C_VALUE);
		
		HSShopGold2CoinRet.Builder response = HSShopGold2CoinRet.newBuilder();
		response.setChangeCount(player.getPlayerData().getStatisticsEntity().getBuyCoinTimesDaily());
		response.setMultiple(multipleValue);
		response.setTotalReward(coinAward);
		sendProtocol(HawkProtocol.valueOf(HS.code.SHOP_GOLD2COIN_S_VALUE, response));
		return true;
	}
	
	@ProtocolHandler(code = HS.code.SHOP_DATA_INIT_C_VALUE)
	private boolean onShopDataInit(HawkProtocol cmd){
		HSShopDataInitRet.Builder response = HSShopDataInitRet.newBuilder();
		for (int i = Const.shopType.NORMALSHOP_VALUE; i < Const.shopType.SHOPNUM_VALUE; i++) {
			response.addShopDatas(ShopUtil.generateShopData(player, i));
		}
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
		ShopEntity shopEntity = player.getPlayerData().getShopEntity(protocol.getType());
		ShopCfg shopCfg = ShopCfg.getShopCfg(protocol.getType(), player.getLevel());
		if (shopCfg.getRefreshMaxNumByHand() != GsConst.UNUSABLE && shopEntity.getRefreshNums() >= shopCfg.getRefreshMaxNumByHand()) {
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
		shopEntity.increaseShopRefreshNum();
		shopEntity.notifyUpdate(true);

		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		statisticsEntity.increaseShopRefreshTimes();
		statisticsEntity.notifyUpdate(true);

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
		ShopEntity shopEntity = player.getPlayerData().getShopEntity(protocol.getType());
		ShopItemInfo itemInfo = shopEntity.getShopItemsList().get(protocol.getSlot());

		if (itemInfo == null) {
			sendError(hsCode, Status.error.PARAMS_INVALID_VALUE);
			return true;
		}
		
		if (protocol.getShopId() != shopEntity.getShopId()) {
			sendError(hsCode, Status.shopError.SHOP_REFRESH_TIMEOUT_VALUE);
			return true;
		}
		
		if (shopEntity.getShopItemsList().get(protocol.getSlot()).isHasBuy() == true) {
			sendError(hsCode, Status.shopError.SHOP_ITEM_ALREADY_BUY_VALUE);
			return true;
		}
			
		ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, shopEntity.getShopItemsList().get(protocol.getSlot()).getItemId());
		if (itemCfg == null) {
			sendError(hsCode, Status.itemError.ITEM_NOT_FOUND_VALUE);
			return true;	
		}

		if (protocol.getType() == Const.shopType.ALLIANCESHOP_VALUE && player.getAllianceId() == 0) {
			sendError(hsCode, Status.allianceError.ALLIANCE_NOT_JOIN_VALUE);
			return true;
		}
		
		int discountPrice = (int)Math.ceil(itemInfo.getPrice() * itemInfo.getDiscount());
		
		ConsumeItems consume = new ConsumeItems();
		if (protocol.getType() == Const.shopType.NORMALSHOP_VALUE) {
			if (itemInfo.getPriceType() == Const.moneyType.MONEY_COIN_VALUE) {
				consume.addCoin((itemInfo.getCount() * discountPrice));
			}
			else if (itemInfo.getPriceType() == Const.moneyType.MONEY_GOLD_VALUE) {
				consume.addGold((itemInfo.getCount() * discountPrice));
			}
		}
		else if (protocol.getType() == Const.shopType.ALLIANCESHOP_VALUE) {
			consume.addContribution((itemInfo.getCount() * discountPrice));
		}
		else if (protocol.getType() == Const.shopType.TOWERSHOP_VALUE) {
			consume.addTowerCoin((itemInfo.getCount() * discountPrice));
		}
		else if (protocol.getType() == Const.shopType.PVPSHOP_VALUE) {
			consume.addHonorPoint((itemInfo.getCount() * discountPrice));
		}

		if (consume.checkConsume(player, hsCode) == false) {
			return true;
		}
		
		AwardItems award = new AwardItems();
		if (itemCfg.getType() == Const.toolType.EQUIPTOOL_VALUE) {
			award.addEquip(itemInfo.getItemId(), itemInfo.getCount(), itemInfo.getStage(), itemInfo.getLevel());
		}
		else{
			award.addItem(itemInfo.getItemId(), itemInfo.getCount());
		}
		
		Action action = Action.NULL;
		if (protocol.getType() == Const.shopType.NORMALSHOP_VALUE) {
			action = Action.NORMAL_SHOP_BUY;
		}
		else if (protocol.getType() == Const.shopType.ALLIANCESHOP_VALUE) {
			action = Action.GUILD_SHOP_BUY;
		}
		else {
			action = Action.TOWER_SHOP_BUY;
		}
		
		consume.consumeTakeAffectAndPush(player, action, hsCode);
		award.rewardTakeAffectAndPush(player, action, hsCode);
		shopEntity.getShopItemsList().get(protocol.getSlot()).setHasBuy(true);
		shopEntity.notifyUpdate(true);

		HSShopItemBuyRet.Builder response = HSShopItemBuyRet.newBuilder();
		sendProtocol(HawkProtocol.valueOf(HS.code.SHOP_ITEM_BUY_S_VALUE, response));

		return true;
	}
	
	@ProtocolHandler(code = HS.code.SHOP_STORE_BUY_C_VALUE)
	private boolean onStoreItemBuy(HawkProtocol cmd){
		HSStoreItemBuy protocol = cmd.parseProtocol(HSStoreItemBuy.getDefaultInstance());
		int hsCode = cmd.getType();
		StoreCfg storeCfg = HawkConfigManager.getInstance().getConfigByKey(StoreCfg.class, protocol.getItemId());

		if (storeCfg == null) {
			sendError(hsCode, Status.error.CONFIG_ERROR_VALUE);
			return true;
		}

		ConsumeItems consume = new ConsumeItems();
		int discountPrice = (int)Math.ceil(storeCfg.getPrice() * storeCfg.getDiscount());
		consume.addGold((int)(protocol.getCount() * discountPrice));
		
		if (consume.checkConsume(player, hsCode) == false) {
			return true;
		}
		
		AwardItems award = new AwardItems();
		award.addItem(storeCfg.getItemId(), protocol.getCount());
		
		consume.consumeTakeAffectAndPush(player, Action.STORE_BUY, hsCode);
		award.rewardTakeAffectAndPush(player, Action.STORE_BUY, hsCode);

		HSStoreItemBuyRet.Builder response = HSStoreItemBuyRet.newBuilder();
		sendProtocol(HawkProtocol.valueOf(HS.code.SHOP_STORE_BUY_S_VALUE, response));
		return true;
	}
	
	@Override
	public boolean onPlayerRefresh(List<Integer> refreshIndexList, boolean onLogin) {
		for (int index : refreshIndexList) {
			int mask = GsConst.PlayerRefreshMask[index];
			if (0 != (mask & GsConst.RefreshMask.DAILY )) {

				for (int i = Const.shopType.NORMALSHOP_VALUE; i < Const.shopType.SHOPNUM_VALUE; i++) {
					ShopEntity shopEntity = player.getPlayerData().getShopEntity(i);
					shopEntity.notifyUpdate(true);
				}
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
			} else if (0 != (mask & GsConst.RefreshMask.SHOP_PVP)) {
				ShopUtil.refreshShopData(Const.shopType.PVPSHOP_VALUE, player);
				if (false == onLogin) {
					player.getPlayerData().syncShopRefreshInfo(Const.shopType.PVPSHOP_VALUE);
				}
			}
		}

		return true;
	}
}
