package com.hawk.game.item;

public class ShopItemInfo extends ItemInfo{
	/**
	 * 排序
	 */
	int slot;
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
		this.discount = 1;		
		this.hasBuy = false;
	}
	
	public ShopItemInfo(int type, String itemId, int count) {
		super();
		this.type = type;
		this.itemId = itemId;
		this.count = count;
		this.slot = 0;
		this.discount = 1;		
		this.hasBuy = false;
	}
	
	public ShopItemInfo(int type, String itemId, int count, int stage, int level) {
		super();
		this.type = type;
		this.itemId = itemId;
		this.count = count;
		this.stage = stage;
		this.level = level;
		this.slot = 0;
		this.discount = 1;		
		this.hasBuy = false;
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

	public boolean isHasBuy() {
		return hasBuy;
	}

	public void setHasBuy(boolean hasBuy) {
		this.hasBuy = hasBuy;
	}

	public static ShopItemInfo valueOf(int type, String itemId, int count) {
		return new ShopItemInfo(type, itemId, count);
	}

	public static ShopItemInfo valueOf(int type, String itemId, int count, int stage, int level) {
		return new ShopItemInfo(type, itemId, count, stage, level);
	}
}
