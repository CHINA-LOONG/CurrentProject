package com.hawk.game.config;

import java.util.LinkedList;
import java.util.List;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

import com.hawk.game.item.ShopItemInfo;
import com.hawk.game.protocol.Const;
import com.hawk.game.util.ConfigUtil;
import com.hawk.game.util.WeightUtil;
import com.hawk.game.util.WeightUtil.WeightItem;

@HawkConfigManager.CsvResource(file = "staticData/itemRandom.csv", struct = "map")
public class ItemRandomCfg extends HawkConfigBase{
	/**
	 * 配置id
	 */
	@Id
	protected final String id;

	/**
	 * 随机商品列表
	 */
	protected final String itemRandom;
	
	/**
	 * 奖励权重列表
	 */
	private List<WeightItem<ShopItemInfo>> weightList;
	
	public ItemRandomCfg(){
		this.id = null;
		this.itemRandom = null;
		weightList = new LinkedList<WeightItem<ShopItemInfo>>();
	}

	public String getId() {
		return id;
	}

	public String getItemRandom() {
		return itemRandom;
	}

	public List<WeightItem<ShopItemInfo>> getWeightList() {
		return weightList;
	}

	public ShopItemInfo getRandomItem() {
		return WeightUtil.random(weightList);
	}
	
	@Override
	protected boolean assemble() {
		weightList.clear();
		
		if (itemRandom != null && itemRandom.length() > 0 && !"0".equals(itemRandom)) {
			String[] itemArrays = itemRandom.split(",");
			for (String itemArray : itemArrays) {
				String[] items = itemArray.split("_");
				if (items.length == 4) {
					ShopItemInfo item = ShopItemInfo.valueOf(Integer.valueOf(items[0]), items[1], Integer.valueOf(items[2]));
					if (item.getType() != Const.itemType.MONSTER_VALUE && item.getType() != Const.itemType.EQUIP_VALUE)
					{
						weightList.add(WeightItem.valueOf(item, Integer.valueOf(items[3])));
					}
					else {
						return false;
					}	
				}
				else if (items.length == 6) {
					ShopItemInfo item = ShopItemInfo.valueOf(Integer.valueOf(items[0]), items[1], Integer.valueOf(items[2]), Integer.valueOf(items[3]), Integer.valueOf(items[4]));
					if (item.getType()== Const.itemType.EQUIP_VALUE)
					{
						weightList.add(WeightItem.valueOf(item, Integer.valueOf(items[5])));
					}
					else {
						return false;
					}
				}			
			}
		}	
		return true;
	}
	
	@Override
	protected boolean checkValid() {
	
		return true;
	}
}
