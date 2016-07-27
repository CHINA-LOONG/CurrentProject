package com.hawk.game.config;

import java.util.ArrayList;
import java.util.LinkedList;
import java.util.List;

import com.hawk.game.item.ShopItemInfo;
import com.hawk.game.protocol.Const;
import com.hawk.game.util.ConfigUtil;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.os.HawkRand;

@HawkConfigManager.CsvResource(file = "staticData/sellItem.csv", struct = "map")
public class SellItemCfg extends HawkConfigBase{
 
	/**
	 * 配置id
	 */
	@Id
	protected final String id;
	
	/**
	 * 配置id
	 */
	protected final String sellFixed;
	
	/**
	 * 配置id
	 */
	protected final String sellRandom;
	
	private List<ShopItemInfo> fixItems = new LinkedList<>();
	private List<String> randomItemList = new LinkedList<>();
	
	public SellItemCfg() {
		id = null;
		sellFixed = null;
		sellRandom = null;
	}
	
	public List<ShopItemInfo> getItemList(){
		List<ShopItemInfo> shopItems = new LinkedList<>();
		for (ShopItemInfo element : fixItems) {
			shopItems.add(element);
		}
		
		List<ShopItemInfo> randomShopItems = new LinkedList<>();
		for (String element : randomItemList) {
			randomShopItems.add(HawkConfigManager.getInstance().getConfigByKey(ItemRandomCfg.class, element).getRandomItem());
		}

		HawkRand.randomOrder(randomShopItems);
		shopItems.addAll(randomShopItems);
		return shopItems;
	}
	
	@Override 
	protected boolean assemble(){
		fixItems.clear();
		randomItemList.clear();
		
		if (sellFixed != null && sellFixed.length() > 0 && !"0".equals(sellFixed)) {
			String[] itemArrays = sellFixed.split(",");
			for (String itemArray : itemArrays) {
				String[] items = itemArray.split("_");
				if (items.length == 3) {
					ShopItemInfo item = ShopItemInfo.valueOf(Integer.valueOf(items[0]), items[1], Integer.valueOf(items[2]));
					if (item.getType() != Const.itemType.EQUIP_VALUE)
					{
						fixItems.add(item);
					}
					else {
						return false;
					}
				}
				else if (items.length == 5) {
					ShopItemInfo item = ShopItemInfo.valueOf(Integer.valueOf(items[0]), items[1], Integer.valueOf(items[2]), Integer.valueOf(items[3]), Integer.valueOf(items[4]));
					if (item.getType() == Const.itemType.EQUIP_VALUE)
					{
						fixItems.add(item);
					}
					else {
						return false;
					}
				}			
			}
		}	
		
		if (sellRandom != null && sellRandom.length() > 0 && !"0".equals(sellRandom)) {
			String[] itemIds = sellRandom.split("_");
			for (String itemId : itemIds) {
				randomItemList.add(itemId);
			}
		}
		
		return true;
	}
	
	@Override 
	protected boolean checkValid(){
		for (String itemId : randomItemList) {
			if (HawkConfigManager.getInstance().getConfigByKey(ItemRandomCfg.class, itemId) == null) {
				return false;
			}
		}
		return true;
	}
}
