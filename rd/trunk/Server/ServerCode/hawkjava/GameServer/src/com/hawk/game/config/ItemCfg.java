package com.hawk.game.config;

import java.util.Collections;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;

import com.hawk.game.item.ItemInfo;
import com.hawk.game.protocol.Const;
import com.hawk.game.util.ConfigUtil;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.GsConst.ItemParseType;

@HawkConfigManager.CsvResource(file = "staticData/item.csv", struct = "map")
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
	 * 子类型
	 */
	protected final int subType;
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
	 * 锻造系数
	 */
	protected final float forgeAdjust;
	/**
	 * 宝箱的奖励id
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
	 * 宝石形状
	 */
	protected final int gemType;
	/**
	 * 装备部位
	 */
	protected final int part;
	/**
	 * 耐久度
	 */
	protected final int durability;

	// client only
	protected final String name = null;
	protected final String asset = null;
	protected final String tips = null;
	protected final String itemfounds = null;

	// assemble
	protected RewardCfg rewardCfg;
	protected List<ItemInfo> componentItemList;
	protected List<ItemInfo> needItemList;
	protected List<ItemInfo> targetItemList;

	// global
	/**
	 * 宝石配置
	 */
	private static Map<Integer, Map<Integer, ItemCfg>> gemList = new HashMap<Integer, Map<Integer, ItemCfg>>();

	public ItemCfg(){
		id  = null;
		classType = 0;
		type = 0;
		subType = 0;
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
		forgeAdjust = 0;
		rewardId = null;
		targetItem = null;
		needCount = 0;
		addAttrType = 0;
		addAttrValue = 0;
		gemId = null;
		gemType = 0;
		part = 0;
		durability = 0;
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

	public int getSubType() {
		return subType;
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

	public float getForgeAdjust() {
		return forgeAdjust;
	}

	public String getRewardId() {
		return rewardId;
	}

	public String getComponentItem() {
		return componentItem;
	}

	public String getNeedItem() {
		return needItem;
	}

	public String getTargetItem() {
		return targetItem;
	}

	public String getItemfounds() {
		return itemfounds;
	}

	public RewardCfg getRewardCfg() {
		return rewardCfg;
	}

	public static Map<Integer, Map<Integer, ItemCfg>> getGemList() {
		return gemList;
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

	public int getGemType() {
		return gemType;
	}

	public int getMaxType() {
		return gemType;
	}

	public int getPart() {
		return part;
	}

	public int getDurability() {
		return durability;
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

	public RewardCfg getReward() {
		return rewardCfg;
	}

	public List<ItemInfo> getComponentItemList() {
		return Collections.unmodifiableList(componentItemList);
	}

	public List<ItemInfo> getNeedItemList() {
		return Collections.unmodifiableList(needItemList);
	}

	public List<ItemInfo> getTargetItemList() {
		return Collections.unmodifiableList(targetItemList);
	}

	public static ItemCfg getGemCfg(int level, int type) {
		return gemList.get(level).get(type);
	}

	@Override
	protected boolean assemble() {
		componentItemList = new LinkedList<ItemInfo>();
		needItemList = new LinkedList<ItemInfo>();
		targetItemList = new LinkedList<ItemInfo>();

		// 合成该物品需要的道具列表
		if (this.componentItem != null && this.componentItem.length() > 0 && !"0".equals(this.componentItem)) {
			componentItemList = ItemInfo.GetItemInfoList(this.componentItem, ItemParseType.PARSE_EQUIP_ATTR);
		}

		// 合成列表
		if (this.targetItem != null && this.targetItem.length() > 0 && !"0".equals(this.targetItem)) {
			targetItemList = ItemInfo.GetItemInfoList(this.targetItem, ItemParseType.PARSE_EQUIP_ATTR);
		}

		// 宝箱需要配对的钥匙
		if (this.type == Const.toolType.BOXTOOL_VALUE) {
			if (this.needItem != null && this.needItem.length() > 0 && !"0".equals(this.needItem)) {
				needItemList = ItemInfo.GetItemInfoList(this.needItem, ItemParseType.PARSE_EQUIP_ATTR);
			}
		} else if (this.type == Const.toolType.GEMTOOL_VALUE) {
			if (gemList.get(grade) == null) {
				Map<Integer, ItemCfg> gradeList = new HashMap<Integer, ItemCfg>();
				gradeList.put(gemType, this);
				gemList.put(grade, gradeList);
			}
			else {
				gemList.get(grade).put(gemType, this);
			}
		}

		return true;
	}

	@Override
	protected boolean checkValid() {
		if (this.type == Const.toolType.FRAGMENTTOOL_VALUE ) {
			if (this.subType == Const.FragSubType.FRAG_MONSTER_VALUE) {
				if (this.needCount != GsConst.UNUSABLE) {
					HawkLog.errPrintln(String.format("config invalid ItemCfg : %s", this.id));
					return false;
				}
			} else if (this.subType == Const.FragSubType.FRAG_TOOL_VALUE ) {
				if (this.needCount <= 0) {
					HawkLog.errPrintln(String.format("config invalid ItemCfg : %s", this.id));
					return false;
				}
			} else {
				HawkLog.errPrintln(String.format("config invalid ItemCfg : %s", this.id));
				return false;
			}
		} else if (this.type == Const.toolType.BOXTOOL_VALUE && (this.rewardId == null || this.rewardId.equals(""))) {
			HawkLog.errPrintln(String.format("config invalid ItemCfg : %s", this.id));
			return false;
		}

		if (this.sellType == GsConst.UNUSABLE && this.sellPrice != GsConst.UNUSABLE) {
			HawkLog.errPrintln(String.format("config invalid ItemCfg : %s", this.id));
			return false;
		} else if (this.sellType != GsConst.UNUSABLE && this.sellPrice == GsConst.UNUSABLE) {
			HawkLog.errPrintln(String.format("config invalid ItemCfg : %s", this.id));
			return false;
		}

		if (this.buyType == GsConst.UNUSABLE && this.buyPrice != GsConst.UNUSABLE) {
			HawkLog.errPrintln(String.format("config invalid ItemCfg : %s", this.id));
			return false;
		} else if (this.buyType != GsConst.UNUSABLE && this.buyPrice == GsConst.UNUSABLE) {
			HawkLog.errPrintln(String.format("config invalid ItemCfg : %s", this.id));
			return false;
		}

		if (this.targetItemList.isEmpty() == true && this.needCount != GsConst.UNUSABLE) {
			HawkLog.errPrintln(String.format("config invalid ItemCfg : %s", this.id));
			return false;
		} else if (this.targetItemList.isEmpty() == false && this.needCount == GsConst.UNUSABLE) {
			HawkLog.errPrintln(String.format("config invalid ItemCfg : %s", this.id));
			return false;
		}

		if (this.addAttrType == GsConst.UNUSABLE && this.addAttrValue != GsConst.UNUSABLE) {
			HawkLog.errPrintln(String.format("config invalid ItemCfg : %s", this.id));
			return false;
		} else if (this.addAttrType != GsConst.UNUSABLE && this.addAttrValue == GsConst.UNUSABLE) {
			HawkLog.errPrintln(String.format("config invalid ItemCfg : %s", this.id));
			return false;
		}

		if (this.gemId != null && this.gemId.length() > 0) {
			if (this.gemType == GsConst.UNUSABLE) {
				HawkLog.errPrintln(String.format("config invalid ItemCfg : %s", this.id));
				return false;
			}

			BaseAttrCfg attrCfg = HawkConfigManager.getInstance().getConfigByKey(BaseAttrCfg.class, this.gemId);
			if (attrCfg == null) {
				HawkLog.errPrintln(String.format("gemID not found in BaseAttrCfg : %s %s", this.id, this.gemId));
				return false;
			}

		} else if (this.gemType != GsConst.UNUSABLE) {
			HawkLog.errPrintln(String.format("config invalid ItemCfg : %s", this.id));
			return false;
		}

		// 检测奖励是否存在，并建立引用
		if (rewardId != null && rewardId.equals("") == false) {
			rewardCfg = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, rewardId);
			if (null == rewardCfg) {
				HawkLog.errPrintln(String.format("config invalid RewardCfg : %s", rewardId));
				return false;
			}
		}
		for (ItemInfo itemInfo: componentItemList) {
			if (false == ConfigUtil.checkItemInfoValid(itemInfo)) {
				return false;
			}
		}
		for (ItemInfo itemInfo: needItemList) {
			if (false == ConfigUtil.checkItemInfoValid(itemInfo)) {
				return false;
			}
		}
		for (ItemInfo itemInfo: targetItemList) {
			if (false == ConfigUtil.checkItemInfoValid(itemInfo)) {
				return false;
			}
		}

		return true;
	}
}