package com.hawk.game.config;

import java.util.Collections;
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

	// assemble
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
		return Collections.unmodifiableList(weightList);
	}

	public ShopItemInfo getRandomItem() {
		return WeightUtil.random(weightList);
	}

	@Override
	protected boolean assemble() {
		if (itemRandom != null && itemRandom.length() > 0 && !"0".equals(itemRandom)) {
			String[] itemArrays = itemRandom.split(",");
			for (String itemArray : itemArrays) {
				String[] items = itemArray.split("_");
				if (items.length == 7) {
					ShopItemInfo item = ShopItemInfo.valueOf(Integer.valueOf(items[0]), items[1], Integer.valueOf(items[2]), Integer.valueOf(items[3]), Integer.valueOf(items[4]), Float.valueOf(items[5]));
					if (item.getType() != Const.itemType.MONSTER_VALUE && item.getType() != Const.itemType.EQUIP_VALUE)
					{
						weightList.add(WeightItem.valueOf(item, Integer.valueOf(items[6])));
					}
					else {
						return false;
					}
				}
				else if (items.length == 9) {
					ShopItemInfo item = ShopItemInfo.valueOf(Integer.valueOf(items[0]), items[1], Integer.valueOf(items[2]), Integer.valueOf(items[3]), Integer.valueOf(items[4]), Integer.valueOf(items[5]), Integer.valueOf(items[6]), Float.valueOf(items[7]));
					if (item.getType()== Const.itemType.EQUIP_VALUE)
					{
						weightList.add(WeightItem.valueOf(item, Integer.valueOf(items[8])));
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
		for (WeightItem<ShopItemInfo> weightItem : weightList) {
			if (false == ConfigUtil.checkItemInfoValid(weightItem.getValue())) {
				return false;
			}
		}
		return true;
	}
}
