package com.hawk.game.manager;

import org.hawk.app.HawkAppObj;
import org.hawk.config.HawkConfigManager;
import org.hawk.util.services.HawkOrderService;
import org.hawk.util.services.HawkReportService;
import org.hawk.xid.HawkXID;

import com.hawk.game.ServerData;
import com.hawk.game.config.RechargeCfg;
import com.hawk.game.entity.RechargeEntity;
import com.hawk.game.entity.statistics.StatisticsEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.player.Player;
import com.hawk.game.util.GsConst;

public class ShopManager extends HawkAppObj {
	/**
	 * 全局对象, 便于访问
	 */
	private static ShopManager instance = null;
	
	/**
	 * 获取全局实例对象
	 */
	public static ShopManager getInstance() {
		return instance;
	}
	
	/**
	 * 构造函数
	 */
	public ShopManager(HawkXID xid) {
		super(xid);
		if (instance == null) {
			instance = this;
		}
	}
	
	// 主线程调用
	public boolean OnOrderNotify(Player player, String puid, String orderSerial, String platform, String productId){
		RechargeCfg rechargeCfg = HawkConfigManager.getInstance().getConfigByKey(RechargeCfg.class, productId);
		if (rechargeCfg == null) {
			HawkOrderService.getInstance().responseDeliver(orderSerial, HawkOrderService.ORDER_PRODUCT_NOT_EXIST, 0, 0);
			return false;
		}
		
		int goldCount = 0;
		int giftGoldCount  = 0;
		boolean isMonthCard = productId.indexOf(GsConst.MONTH_CARD) >=0;
		// 不是月卡 检测是不是首充
		if(!isMonthCard){
			boolean isfirstRecharge = player.getPlayerData().getStatisticsEntity().getRechargeTimes(productId) == 0 ? true : false;
			giftGoldCount = isfirstRecharge ? (int)(rechargeCfg.getGold() * rechargeCfg.getGift()) : 0;
			goldCount = rechargeCfg.getGold();
		}

		RechargeEntity rechargeEntity = new RechargeEntity(orderSerial, 
														   puid,
														   player.getPlayerData().getId(),
														   productId,
														   goldCount,
														   giftGoldCount,
														   player.getLevel(),
														   platform);
		
		// 已经处理过的订单
		if (ServerData.getInstance().isExistOrder(orderSerial) == true) {
			HawkOrderService.getInstance().responseDeliver(orderSerial, HawkOrderService.ORDER_STATUS_OK, rechargeCfg.getGold(), giftGoldCount);
			return true;
		}
		
		if (rechargeEntity.notifyCreate() == false) {
			HawkOrderService.getInstance().responseDeliver(orderSerial, HawkOrderService.ORDER_STATUS_ERROR, 0, 0);
			return false;
		}
		
		ServerData.getInstance().addOrderSerial(orderSerial);
		StatisticsEntity staticsticsEntity = player.getPlayerData().getStatisticsEntity();		
		if (isMonthCard) {
			staticsticsEntity.addMonthCard();
			if (player.getSession() != null) {
				player.getPlayerData().syncStatisticsInfo();
			}
		}
		else
		{
			// 离线玩家
			if (player.getSession() == null) {
				player.increaseBuyGold(rechargeEntity.getAddGold(), Action.SHOP_RECHARGE);
				if (rechargeEntity.getGiftGold() > 0) {
					player.increaseFreeGold(rechargeEntity.getGiftGold(), Action.SHOP_RECHARGE);
				}
			}
			else {
				AwardItems reward = new AwardItems();
				reward.addFreeGold(rechargeEntity.getGiftGold());
				reward.addBuyGold(rechargeEntity.getAddGold());
				reward.rewardTakeAffectAndPush(player, Action.SHOP_RECHARGE, 0);
			}
		}

		staticsticsEntity.increaseRechargeRecord(productId);
		staticsticsEntity.notifyUpdate(false);
		HawkOrderService.getInstance().responseDeliver(orderSerial, HawkOrderService.ORDER_STATUS_OK, rechargeCfg.getGold(), giftGoldCount);
		
		// 上报充值数据
		HawkReportService.RechargeData rechargeData = new HawkReportService.RechargeData(
														  puid, 
														  "", 
														  player.getPlayerData().getId(),  
														  player.getPlayerData().getNickname(), 
														  player.getPlayerData().getLevel(),
														  orderSerial,
														  productId, 
														  0,
														  goldCount,
														  giftGoldCount,
														  "",
														  "");
		
		//HawkReportService.getInstance().report(rechargeData);
		return true;
	}
}
