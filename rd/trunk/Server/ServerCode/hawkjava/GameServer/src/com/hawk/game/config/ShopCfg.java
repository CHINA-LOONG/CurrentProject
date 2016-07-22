package com.hawk.game.config;

import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/shop.csv", struct = "map")
public class ShopCfg extends HawkConfigBase{
	/**
	 * 配置id
	 */
	@Id
	protected final String id;

	/**
	 * 商店类型
	 */
	protected final int type;
	/**
	 * 手动刷新上限
	 */
	protected final int refreshMaxNumByHand;
	/**
	 * 手动刷新上限
	 */
	protected final int maxLevel;
	/**
	 * 手动刷新上限
	 */
	protected final String sellItemId;
	
	private static Map<Integer, List<ShopCfg>> shopTypeList = new HashMap<Integer, List<ShopCfg>>();
	
	public ShopCfg() {
		id = null;
		type = 0;
		refreshMaxNumByHand = 0;
		maxLevel = 0;		
		sellItemId = null;
	}
	
	public String getId() {
		return id;
	}

	public int getType() {
		return type;
	}

	public int getRefreshMaxNumByHand() {
		return refreshMaxNumByHand;
	}

	public int getMaxLevel() {
		return maxLevel;
	}

	public String getSellItemId() {
		return sellItemId;
	}

	/**
	 * 获取商店配置信息
	 * 
	 * @param type 商店类型
	 * @param level 玩家等级
	 * @return
	 */
	public static ShopCfg getShopCfg(int type, int level){
		List<ShopCfg> shopList = shopTypeList.get(type);
		if (shopList != null) {
			for (ShopCfg element : shopList) {
				if (level <= element.getMaxLevel()) {
					return element;
				}
			}
		}
		return null;
	}
	
	@Override
	protected boolean assemble() {	
		List<ShopCfg> shopList = shopTypeList.get(this.type);
		if (shopList == null) {
			shopList = new LinkedList<ShopCfg>();
			shopTypeList.put(this.type, shopList);
		}
		shopList.add(this);
		return true;
	}

	@Override
	protected boolean checkValid() {
		if (HawkConfigManager.getInstance().getConfigByKey(SellItemCfg.class, sellItemId) == null) {
			return false;
		}
		return true;
	}
}
