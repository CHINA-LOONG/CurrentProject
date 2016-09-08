package com.hawk.game.item;

import com.hawk.game.protocol.Const;

public class ShopItemInfo extends ItemInfo{
	/**
	 * 排序
	 */
	int slot;
	/**
	 * 价钱
	 */
	int price;
	/**
	 * 货币类型
	 */
	int priceType;
	/**
	 * 折扣
	 */
	float discount;
	/**
	 * 是否已经购买
	 */
	boolean hasBuy;

	public ShopItemInfo () {
		super();
		this.slot = 0;
		this.price = 0;
		this.priceType = 0;
		this.discount = 1;		
		this.hasBuy = false;
	}
	
	public ShopItemInfo(int type, String itemId, int count, int priceType, int price, float discount) {
		super();
		this.type = type;
		this.itemId = itemId;
		this.count = count;
		this.priceType = priceType;
		this.price = price;
		this.discount = discount;	
	}
	
	public ShopItemInfo(int type, String itemId, int count, int stage, int level, int priceType, int price, float discount) {
		super();
		this.type = type;
		this.itemId = itemId;
		this.count = count;
		this.stage = stage;
		this.level = level;
		this.priceType = priceType;
		this.price = price;
		this.discount = discount;		
	}
	
	public ShopItemInfo clone() {
		ShopItemInfo ret = new ShopItemInfo();
		ret.type = this.type;
		ret.itemId = this.itemId;
		ret.count = this.count;
		ret.stage = this.stage;
		ret.level = this.level;
		ret.slot = this.slot;
		ret.priceType = this.priceType;
		ret.price = this.price;
		ret.discount = this.discount;	
		ret.hasBuy = this.hasBuy;
		return ret;
	}
	
	public int getSlot() {
		return slot;
	}

	public void setSlot(int slot) {
		this.slot = slot;
	}

	public float getDiscount() {
		return discount;
	}

	public void setDiscount(float discount) {
		this.discount = discount;
	}

	public int getPriceType() {
		return priceType;
	}

	public void setPriceType(int priceType) {
		this.priceType = priceType;
	}

	public int getPrice() {
		return price;
	}

	public void setPrice(int price) {
		this.price = price;
	}

	public boolean isHasBuy() {
		return hasBuy;
	}

	public void setHasBuy(boolean hasBuy) {
		this.hasBuy = hasBuy;
	}

	public String toString(){
		StringBuilder builder = new StringBuilder();
		if (this.type != Const.itemType.EQUIP_VALUE) {
			builder.append(type).append("_").append(itemId).append("_").append(count).append("_").append(priceType).append("_").append(price).append("_").append(discount).append("_").append(slot).append("_").append(hasBuy).append("_");
		}
		else
		{
			builder.append(type).append("_").append(itemId).append("_").append(count).append("_").append(stage).append("_").append(level).append("_").append(priceType).append("_").append(price).append("_").append(discount).append("_").append(slot).append("_").append(hasBuy).append("_");
		}
		
		return builder.toString();
	}
	
	public static ShopItemInfo valueOf(int type, String itemId, int count, int priceType, int price, float discount) {
		return new ShopItemInfo(type, itemId, count, priceType, price, discount);
	}

	public static ShopItemInfo valueOf(int type, String itemId, int count, int stage, int level, int priceType, int price, float discount) {
		return new ShopItemInfo(type, itemId, count, stage, level, priceType, price, discount);
	}
	
	public static ShopItemInfo generateFromDB(String info)
	{
		String[] items = info.split("_");
		ShopItemInfo shopItem = null;
		if (items.length == 8) {
			shopItem = ShopItemInfo.valueOf(Integer.valueOf(items[0]), items[1], Integer.valueOf(items[2]), Integer.valueOf(items[3]), Integer.valueOf(items[4]), Float.valueOf(items[5]));					
			shopItem.setSlot(Integer.valueOf(items[6]));
			shopItem.setHasBuy(Boolean.valueOf(items[7]));
		}
		else if (items.length == 10) {
			shopItem = ShopItemInfo.valueOf(Integer.valueOf(items[0]), items[1], Integer.valueOf(items[2]), Integer.valueOf(items[3]), Integer.valueOf(items[4]), Integer.valueOf(items[5]), Integer.valueOf(items[6]), Float.valueOf(items[7]));
			shopItem.setSlot(Integer.valueOf(items[8]));
			shopItem.setHasBuy(Boolean.valueOf(items[9]));		
		}
		return shopItem;
	}
	
	public static ShopItemInfo generateFromConfig(String info)
	{
		String[] items = info.split("_");
		ShopItemInfo shopItem = null;
		if (items.length == 6) {
			shopItem = ShopItemInfo.valueOf(Integer.valueOf(items[0]), items[1], Integer.valueOf(items[2]), Integer.valueOf(items[3]), Integer.valueOf(items[4]), Float.valueOf(items[5]));					
		}
		else if (items.length == 8) {
			shopItem = ShopItemInfo.valueOf(Integer.valueOf(items[0]), items[1], Integer.valueOf(items[2]), Integer.valueOf(items[3]), Integer.valueOf(items[4]), Integer.valueOf(items[5]), Integer.valueOf(items[6]), Float.valueOf(items[7]));
		}
		return shopItem;
	}
}
