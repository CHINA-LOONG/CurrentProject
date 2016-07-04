package com.hawk.game.item;
/**
 * 物品结构体定义
 */
public class ItemInfo {
	/**
	 * 种类
	 */
	int type;
	/**
	 * itemId
	 */
	int itemId;
	/**
	 * 数量
	 */
	int count;
	/**
	 * 品级
	 */
	int stage;
	/**
	 * 级别
	 */
	int level;

	public ItemInfo() {
		super();
		this.type = 0;
		this.itemId = 0;
		this.count = 0;
		this.stage = 0;
		this.level = 0;
	}

	public ItemInfo(int type, int itemId, int count) {
		super();
		this.type = type;
		this.itemId = itemId;
		this.count = count;
	}

	public ItemInfo(int type, int itemId, int stage, int level) {
		super();
		this.type = type;
		this.itemId = itemId;
		this.stage = stage;
		this.level = level;
	}
	
	public ItemInfo(String info) {
		initByString(info);
	}

	public int getType() {
		return type;
	}

	public void setType(int type) {
		this.type = type;
	}

	public int getItemId() {
		return itemId;
	}

	public void setItemId(int itemId) {
		this.itemId = itemId;
	}

	public int getCount() {
		return count;
	}

	public void setCount(int count) {
		this.count = count;
	}

	public int getStage() {
		return stage;
	}

	public void Stage(int stage) {
		this.stage = stage;
	}

	public int getLevel() {
		return level;
	}

	public void setLevel(int level) {
		this.level = level;
	}

	public ItemInfo clone() {
		ItemInfo ret = new ItemInfo(type, itemId, count);
		return ret;
	}

	@Override
	public String toString() {
		return String.format("%d_%d_%d", type, itemId, count);
	}

	public static ItemInfo valueOf(int type, int itemId, int quantity) {
		return new ItemInfo(type, itemId, quantity);
	}

	public boolean initByString(String info) {
		if (info != null && info.length() > 0 && !info.equals("0") && !info.equals("none")) {
			String[] items = info.split("_");
			if (items.length < 3) {
				return false;
			}
			type = Integer.parseInt(items[0]);
			itemId = Integer.parseInt(items[1]);
			count = Integer.parseInt(items[2]);
			return true;
		}
		return false;
	}

	public static ItemInfo valueOf(String info) {
		ItemInfo itemInfo = new ItemInfo();
		if (itemInfo.initByString(info)) {
			return itemInfo;
		}
		return null;
	}
}
