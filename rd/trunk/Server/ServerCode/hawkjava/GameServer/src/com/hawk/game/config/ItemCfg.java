package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.config.HawkConfigBase.Id;

import com.hawk.game.item.ItemInfo;
import com.hawk.game.protocol.Const;

import java.util.LinkedList;
import java.util.List;

@HawkConfigManager.CsvResource(file = "xml/item.csv", struct = "map")
public class ItemCfg extends HawkConfigBase {
	/**
	 * 配置id
	 */
	@Id
	protected final String id ;
	/**
	 * 1角色2怪物
	 */
	protected final int classType;
	/**
	 * 类型
	 */
	protected final int type;
	/**
	 * 品级
	 */
	protected final int grade;
	/**
	 * 等级下限
	 */
	protected final int minLevel;
	/**
	 * 使用条件
	 */
	protected final int condition;
	/**
	 * 每天的使用次数
	 */
	protected final int times;
	/**
	 * 绑定类型 
	 */
	protected final int bindType ;
	/**
	 * 出售价钱 
	 */
	protected final int sellPrice ;
	/**
	 * 货币类型
	 */
	protected final int sellType;
	/**
	 * 购买价钱 
	 */
	protected final int buyPrice ;
	/**
	 * 购买类型
	 */
	protected final int buyType;
	/**
	 * 叠加上限
	 */
	protected final int stack;
	/**
	 * 宝箱的奖励列表
	 */
	protected final String rewardId;
	/**
	 * 合成该装备需要的道具列表
	 */
	protected final String componentItem;
	/**
	 * 配对使用的道具列表
	 */
	protected final String needItem;
	/**
	 * 兑换/合成道具列表
	 */
	protected final String targetItem;
	/**
	 * 合成/兑换 需要的数量
	 */
	protected final int needCount;
	/**
	 * 增益类型
	 */
	protected final int addAttrType;
	/**
	 * 增益数值
	 */
	protected final int addAttrValue;
	/**
	 * 宝石增益属性ID
	 */
	protected final String gemId;
	/**
	 * 宝石增益属性ID
	 */
	protected final int maxGem;
	/**
	 * 装备部位
	 */
	protected final int part;
	/**
	 * 耐久度
	 */
	protected final int durability;
	/**
	 * 装备属性ID
	 */
	protected final int equipAttId;
	
	protected final String name ;
	
	protected final String asset ;
	
	protected final String tips ;

	/**
	 * 合成该装备需要的道具列表
	 */
	private List<ItemInfo> componentItemList;
	/**
	 * 匹配使用装备列表
	 */
	private List<ItemInfo> needItemList;
	/**
	 * 合成/兑换装备列表
	 */
	private List<ItemInfo> targetItemList;
	
	public ItemCfg(){				
		id  = null;
		classType = 0;
		type = 0;
		grade = 0;
		minLevel = 0;
		condition = 0;
		times = 0;
		bindType = 0;
		sellPrice = 0;
		sellType = 0;
		buyPrice = 0;
		buyType = 0;
		componentItem = null;
		needItem = null;
		stack = 0;
		rewardId = null;
		targetItem = null;
		needCount = 0;
		addAttrType = 0;
		addAttrValue = 0;
		gemId = null;
		maxGem = 0;
		part = 0;
		durability = 0;
		equipAttId = 0;
		
		name = null;
		asset = null;
		tips = null;
		
		componentItemList = new LinkedList<ItemInfo>();
		needItemList = new LinkedList<ItemInfo>();
		targetItemList = new LinkedList<ItemInfo>();
	}

	public String getId() {
		return id;
	}

	public int getClassType() {
		return classType;
	}

	public int getType() {
		return type;
	}

	public int getGrade() {
		return grade;
	}

	public int getMinLevel() {
		return minLevel;
	}

	public int getCondition() {
		return condition;
	}


	public int getTimes() {
		return times;
	}


	public int getBindType() {
		return bindType;
	}


	public int getSellPrice() {
		return sellPrice;
	}


	public int getSellType() {
		return sellType;
	}


	public int getBuyPrice() {
		return buyPrice;
	}

	public int getBuyType() {
		return buyType;
	}


	public int getStack() {
		return stack;
	}


	public String getRewardId() {
		return rewardId;
	}

	public int getNeedCount() {
		return needCount;
	}

	public int getAddAttrType() {
		return addAttrType;
	}


	public int getAddAttrValue() {
		return addAttrValue;
	}


	public String getGemId() {
		return gemId;
	}

	public int getMaxGem() {
		return maxGem;
	}

	public int getPart() {
		return part;
	}

	public int getDurability() {
		return durability;
	}


	public int getEquipAttId() {
		return equipAttId;
	}

	public String getName() {
		return name;
	}


	public String getAsset() {
		return asset;
	}

	public String getTips() {
		return tips;
	}

	public List<ItemInfo> getComponentItemList() {
		return componentItemList;
	}

	public void setComponentItemList(List<ItemInfo> componentItemList) {
		this.componentItemList = componentItemList;
	}

	public List<ItemInfo> getNeedItemList() {
		return needItemList;
	}

	public void setNeedItemList(List<ItemInfo> needItemList) {
		this.needItemList = needItemList;
	}

	public List<ItemInfo> getTargetItemList() {
		return targetItemList;
	}

	public void setTargetItemList(List<ItemInfo> targetItemList) {
		this.targetItemList = targetItemList;
	}

	@Override
	protected boolean assemble() {
		needItemList.clear();
		targetItemList.clear();
		
		// 合成该物品需要的道具列表
		if (this.componentItem != null && this.componentItem.length() > 0 && !"0".equals(this.componentItem)) {
			String[] itemArrays = componentItem.split(",");
			for (String itemArray : itemArrays) {
				String[] items = itemArray.split("_");
				if (items.length == 3) {
					ItemInfo itemInfo = new ItemInfo();
					itemInfo.setType(Integer.valueOf(items[0]));
					itemInfo.setItemId(items[1]);
					itemInfo.setCount(Integer.valueOf(items[2]));
					componentItemList.add(itemInfo);
				}
				else if (items.length == 5){
					if (Integer.valueOf(items[0]) != Const.itemType.EQUIP_VALUE) {
						return false;
					}
					ItemInfo itemInfo = new ItemInfo();
					itemInfo.setType(Integer.valueOf(items[0]));
					itemInfo.setItemId(items[1]);
					itemInfo.setCount(Integer.valueOf(items[2]));
					itemInfo.setStage(Integer.valueOf(items[3]));
					itemInfo.setLevel(Integer.valueOf(items[4]));
					componentItemList.add(itemInfo);
				}
				else {
					return false;
				}
			}
		}
		
		// 碎片
		if (this.type == Const.toolType.FRAGMENTTOOL_VALUE) {
			if (this.targetItem != null && this.targetItem.length() > 0 && !"0".equals(this.targetItem)) {
				String[] itemArrays = targetItem.split(",");
				for (String itemArray : itemArrays) {
					String[] items = itemArray.split("_");
					if (items.length == 3) {
						ItemInfo itemInfo = new ItemInfo();
						itemInfo.setType(Integer.valueOf(items[0]));
						itemInfo.setItemId(items[1]);
						itemInfo.setCount(Integer.valueOf(items[2]));
						targetItemList.add(itemInfo);
					}
					else if (items.length == 5){
						if (Integer.valueOf(items[0]) != Const.itemType.EQUIP_VALUE) {
							return false;
						}
						ItemInfo itemInfo = new ItemInfo();
						itemInfo.setType(Integer.valueOf(items[0]));
						itemInfo.setItemId(items[1]);
						itemInfo.setCount(Integer.valueOf(items[2]));
						itemInfo.setStage(Integer.valueOf(items[3]));
						itemInfo.setLevel(Integer.valueOf(items[4]));
						targetItemList.add(itemInfo);
					}
					else {
						return false;
					}
				}
			}
		}
		// 宝箱需要配对的钥匙
		else if (this.type == Const.toolType.BOXTOOL_VALUE) {
			if (this.needItem != null && this.needItem.length() > 0 && !"0".equals(this.needItem)) {
				String[] itemArrays = needItem.split(",");
				for (String itemArray : itemArrays) {
					String[] items = itemArray.split("_");
					if (items.length == 3) {
						ItemInfo itemInfo = new ItemInfo();
						itemInfo.setType(Integer.valueOf(items[0]));
						itemInfo.setItemId(items[1]);
						itemInfo.setCount(Integer.valueOf(items[2]));
						needItemList.add(itemInfo);
					}
					else if (items.length == 5){
						if (Integer.valueOf(items[0]) != Const.itemType.EQUIP_VALUE) {
							return false;
						}
						ItemInfo itemInfo = new ItemInfo();
						itemInfo.setType(Integer.valueOf(items[0]));
						itemInfo.setItemId(items[1]);
						itemInfo.setCount(Integer.valueOf(items[2]));
						itemInfo.setStage(Integer.valueOf(items[3]));
						itemInfo.setLevel(Integer.valueOf(items[4]));
						needItemList.add(itemInfo);
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
		if (this.type == Const.toolType.FRAGMENTTOOL_VALUE && this.getNeedCount() <= 0 ) {
			return false;
		}
		else if (this.type == Const.toolType.BOXTOOL_VALUE && (this.getRewardId() == null || this.getRewardId().equals(""))) {
			return false;
		}
		return true;
	}
}