package com.hawk.game.item;

import java.util.LinkedList;
import java.util.List;

public class ItemInfo {
	/**
	 * 种类
	 */
	int type;
	/**
	 * itemId
	 */
	String itemId;
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
		this.itemId = "";
		this.count = 0;
		this.stage = 0;
		this.level = 0;
	}

	public ItemInfo(int type, String itemId, int count) {
		super();
		this.type = type;
		this.itemId = itemId;
		this.count = count;
	}

	public ItemInfo(int type, String itemId, int count, int stage) {
		super();
		this.type = type;
		this.itemId = itemId;
		this.count = count;
		this.stage = stage;
	}

	public ItemInfo(int type, String itemId, int count, int stage, int level) {
		super();
		this.type = type;
		this.itemId = itemId;
		this.count = count;
		this.stage = stage;
		this.level = level;
	}

	public int getType() {
		return type;
	}

	public void setType(int type) {
		this.type = type;
	}

	public String getItemId() {
		return itemId;
	}

	public void setItemId(String itemId) {
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

	public void setStage(int stage) {
		this.stage = stage;
	}

	public int getLevel() {
		return level;
	}

	public void setLevel(int level) {
		this.level = level;
	}

	public ItemInfo clone() {
		ItemInfo ret = new ItemInfo();
		ret.itemId = this.itemId;
		ret.type = this.type;
		ret.stage = this.stage;
		ret.level = this.level;
		ret.count = this.count;
		return ret;
	}

	@Override
	public String toString() {
		return String.format("%d_%s_%d_%d_%d", type, itemId, count, stage, level);
	}

	public static String toString(List<ItemInfo> itemList) {
		if (true == itemList.isEmpty()) {
			return "";
		}
		StringBuilder builder = new StringBuilder();
		for (ItemInfo itemInfo : itemList) {
			builder.append(itemInfo.toString()).append(",");
		}
		builder.deleteCharAt(builder.length() - 1);
		return builder.toString();
	}

	public static ItemInfo valueOf(int type, String itemId, int count) {
		return new ItemInfo(type, itemId, count);
	}

	public static ItemInfo valueOf(int type, String itemId, int count, int stage) {
		return new ItemInfo(type, itemId, count, stage);
	}

	public static ItemInfo valueOf(int type, String itemId, int count, int stage, int level) {
		return new ItemInfo(type, itemId, count, stage, level);
	}

	public static ItemInfo valueOf(String info, int parseType) {
		ItemInfo itemInfo = new ItemInfo();
		if (true == itemInfo.initByString(info, parseType)) {
			return itemInfo;
		}
		return null;
	}

	public static List<ItemInfo> GetItemInfoList(String info, int parseType) {
		List<ItemInfo> result = new LinkedList<ItemInfo>();
		String[] items = info.split(",");
		for(String item : items) {
			ItemInfo itemInfo = ItemInfo.valueOf(item, parseType);
			if (itemInfo != null) {
				result.add(itemInfo);
			}
		}
		return result;
	}

	private boolean initByString(String info, int parseType) {
		if (info != null && info.length() > 0 && !info.equals("0") && !info.equals("none")) {
			String[] items = info.split("_");
			if (items.length < 3) {
				return false;
			}

			type = Integer.parseInt(items[0]);
			itemId = items[1];
			count = Integer.parseInt(items[2]);
			// if (parseType == ItemParseType.PARSE_MONSTER_STAGE) {
				if (items.length == 4)
				{
					stage = Integer.parseInt(items[3]);
				}
				else if (items.length == 5) {
					stage = Integer.parseInt(items[3]);
					level = Integer.parseInt(items[4]);
				}
			//}

			return true;
		}
		return false;
	}

}
