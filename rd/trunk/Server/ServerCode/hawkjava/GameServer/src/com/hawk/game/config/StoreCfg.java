package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/store.csv")
public class StoreCfg extends HawkConfigBase{
	/**
	 * itemId
	 */
	@Id
	protected final String itemId;
	/**
	 * 价钱
	 */
	protected final int price;
	/**
	 * 折扣
	 */
	protected final float discount;
	
	public StoreCfg(){
		this.itemId = null;
		this.price = 0;
		this.discount = 0.0f;
	}

	public String getItemId() {
		return itemId;
	}

	public int getPrice() {
		return price;
	}

	public float getDiscount() {
		return discount;
	}
	
	@Override
	protected boolean checkValid() {
		if (HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, itemId) == null) {
			return false;
		}
		return true;
	}
}
