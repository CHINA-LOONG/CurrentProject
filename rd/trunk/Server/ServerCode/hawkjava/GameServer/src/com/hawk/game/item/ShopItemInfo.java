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
		this.discount = 1;		
		this.hasBuy = false;
	}
	
	public ShopItemInfo(int type, String itemId, int count, int price, float discount) {
		super();
		this.type = type;
		this.itemId = itemId;
		this.count = count;
		this.discount = discount;	
		this.price = price;
	}
	
	public ShopItemInfo(int type, String itemId, int count, int stage, int level, int price, float discount) {
		super();
		this.type = type;
		this.itemId = itemId;
		this.count = count;
		this.stage = stage;
		this.level = level;
		this.discount = discount;		
		this.price = price;
	}
	
	public ShopItemInfo clone() {
		ShopItemInfo ret = new ShopItemInfo();
		ret.type = this.type;
		ret.itemId = this.itemId;
		ret.count = this.count;
		ret.stage = this.stage;
		ret.level = this.level;
		ret.slot = this.slot;
		ret.discount = this.discount;	
		ret.price = this.price;
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
			builder.append(type).append("_").append(itemId).append("_").append(count).append("_").append(price).append("_").append(discount);
		}
		else
		{
			builder.append(type).append("_").append(itemId).append("_").append(count).append("_").append(stage).append("_").append(level).append("_").append(price).append("_").append(discount);
		}
		
		return builder.toString();
	}
	
	public static ShopItemInfo valueOf(int type, String itemId, int count, int price, float discount) {
		return new ShopItemInfo(type, itemId, count, price, discount);
	}

	public static ShopItemInfo valueOf(int type, String itemId, int count, int stage, int level, int price, float discount) {
		return new ShopItemInfo(type, itemId, count, stage, level, price, discount);
	}
	
	public static ShopItemInfo valueOf(String info)
	{
		String[] items = info.split("_");
		ShopItemInfo shopItem = null;
		if (items.length == 5) {
			shopItem = ShopItemInfo.valueOf(Integer.valueOf(items[0]), items[1], Integer.valueOf(items[2]), Integer.valueOf(items[3]), Float.valueOf(items[4]));					
		}
		else if (items.length == 7) {
			shopItem = ShopItemInfo.valueOf(Integer.valueOf(items[0]), items[1], Integer.valueOf(items[2]), Integer.valueOf(items[3]), Integer.valueOf(items[4]), Integer.valueOf(items[5]), Float.valueOf(items[6]));
		}
		return shopItem;
	}
	
}
