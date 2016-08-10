package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/store.csv")
public class StoreCfg extends HawkConfigBase{
	/**
	 * id
	 */
	@Id
	protected final int id;
	/**
	 * 物品Id
	 */
	protected final String item;
	/**
	 * 类型
	 */
	protected final int type;
	/**
	 * 品阶
	 */
	protected final int stage;
	/**
	 * 级别
	 */
	protected final int level;
	/**
	 * 数量
	 */
	protected final int count;
	/**
	 * 价钱
	 */
	protected final int price;
	/**
	 * 折扣
	 */
	protected final float discount;
	
	public StoreCfg(){
		this.id = 0;
		this.item = null;
		this.type = 0;
		this.stage = 0;
		this.level = 0;
		this.count = 0;
		this.price = 0;
		this.discount = 0.0f;
	}

	public int getId() {
		return id;
	}

	public String getItem() {
		return item;
	}

	public int getType() {
		return type;
	}

	public int getStage() {
		return stage;
	}

	public int getLevel() {
		return level;
	}

	public int getCount() {
		return count;
	}

	public int getPrice() {
		return price;
	}

	public float getDiscount() {
		return discount;
	}
	
	@Override
	protected boolean checkValid() {
		if (HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, item) == null) {
			return false;
		}
		return true;
	}
}
