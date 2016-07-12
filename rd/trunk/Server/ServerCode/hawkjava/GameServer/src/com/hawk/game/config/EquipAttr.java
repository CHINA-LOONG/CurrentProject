package com.hawk.game.config;
import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;

import com.hawk.game.attr.Attribute;
import com.hawk.game.item.ItemInfo;
import com.hawk.game.protocol.Const.attr;
import com.hawk.game.util.WeightUtil;
import com.hawk.game.util.WeightUtil.WeightItem;

import java.util.LinkedHashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import javax.smartcardio.ATR;

@HawkConfigManager.CsvResource(file = "staticData/equipAttr.csv", struct = "list")
public class EquipAttr extends HawkConfigBase{

	private static Map<Integer, EquipStageItem> stageList = new LinkedHashMap<Integer, EquipStageItem>();
	private static Map<String, Map<Integer, EquipStageItem>> equipList = new LinkedHashMap<String, Map<Integer, EquipStageItem>>();

	private class EquipStageItem{
		/**
		 * 强化级别对应属性列表
		 */
		private List<EquipLevelItem> levelList;
		/**
		 * 随机属性强化列表
		 */
		private List<WeightItem<AdditionAttrItem>> weightList;
		/**
		 * roll 次数
		 */
		private int rollCount;
		
		public  EquipStageItem() {
			levelList = new LinkedList<EquipLevelItem>();
			weightList = new LinkedList<WeightItem<AdditionAttrItem>>();
			rollCount = 0;
		}
		 
		public boolean init(String additionAttr, int rollCount){
			if (additionAttr != null && additionAttr.length() > 0 && !"0".equals(additionAttr)) {
				String[] itemArrays = additionAttr.split(",");
				for (String itemArray : itemArrays) {
					String[] items = itemArray.split("_");
					if (items.length != 3) {
						return false;
					}
					AdditionAttrItem attrItem = new AdditionAttrItem();
					attrItem.setType(Integer.valueOf(items[0]));
					attrItem.setValue(Float.valueOf(items[1]));
					weightList.add(WeightItem.valueOf(attrItem, Integer.valueOf(items[2])));			
				}
			}	
			
			this.rollCount = rollCount;
			
			return true;
		}
		
		/**
		 * roll指定次数的附加属性 0代码空roll
		 */
		public Attribute rondomAddiAttr() {
			Attribute attr = new Attribute();
			List<WeightItem<AdditionAttrItem>> remaindAttr = new LinkedList<WeightUtil.WeightItem<AdditionAttrItem>>();
			remaindAttr.addAll(weightList);
			
			for (int i = 0; i < rollCount; i++) {
				AdditionAttrItem current = WeightUtil.random(remaindAttr);
				// 0 代表NULL
				if (current.getType() != 0) {
					attr.add(current.getType(), current.getValue());
					for (WeightItem<AdditionAttrItem> item : remaindAttr) {
						if (item.getValue().getType() == current.getType()) {
							remaindAttr.remove(item);
							break;
						}
					}
				}
			}
			return attr;
		}
		
		public List<EquipLevelItem> getLevelList() {
			return levelList;
		}
	}
	
	private  class AdditionAttrItem{
		/**
		 * 附加属性类型
		 */
		private int type;
		/**
		 * 附加属性值
		 */
		private float value;
		
		public AdditionAttrItem() {
			this.type = 0;
			this.value = 0;
		}
		
		public int getType() {
			return type;
		}
		public void setType(int type) {
			this.type = type;
		}
		public float getValue() {
			return value;
		}
		public void setValue(float value) {
			this.value = value;
		}
	}
	
	private class EquipLevelItem{
		/**
		 * 基础属性索引
		 */
		private int baseAttrId;
		
		/**
		 * 升级消耗
		 */
		private List<ItemInfo> demandList;
		
		public EquipLevelItem(){
			baseAttrId = 0;
			demandList = new LinkedList<ItemInfo>();
		}	
		
		public boolean init(int baseAttrID, String costList){
			this.baseAttrId = baseAttrID;
			demandList.clear();
			
			if (costList != null && costList.length() > 0 && !"0".equals(costList)) {
				String[] itemArrays = costList.split(",");
				for (String itemArray : itemArrays) {
					String[] items = itemArray.split("_");
					if (items.length != 3) {
						return false;
					}
					ItemInfo itemInfo = new ItemInfo();
					itemInfo.setType(Integer.valueOf(items[0]));
					itemInfo.setItemId(items[1]);
					itemInfo.setCount(Integer.valueOf(items[2]));
					demandList.add(itemInfo);
				}
			}	
			
			return true;
		}

		/**
		 * 获取基础属性
		 */
		public BaseAttrCfg getBaseAttr() {
			return HawkConfigManager.getInstance().getConfigByKey(BaseAttrCfg.class, this.baseAttrId);
		}

		/**
		 * 获取消耗列表
		 */
		public List<ItemInfo> getDemandList() {
			return demandList;
		}
	}
		
	/**
	 * 配置id
	 */
	protected final String id ;
	/**
	 * 品级
	 */
	protected final int stage;
	/**
	 * 级别
	 */
	protected final int level;
	/**
	 * 基础属性ID
	 */
	protected final int baseAttrId;
	/**
	 * roll次数
	 */
	protected final int rollCount;
	/**
	 * 附加属性
	 */
	protected final String additionAttr;
	/**
	 * 消耗列表
	 */
	protected final String demand;				

	public  EquipAttr() {
		id = null;
		stage = 0;
		level = 0;
		baseAttrId = 0;
		rollCount = 0;
		additionAttr = null;
		demand = null;
	}
	
	/**
	 * 随机附加属性
	 */
	public static Attribute getAdditionAttr(String equip, int stage){
		if (stage <= 1) {
			return null;
		}
		
		Map<Integer, EquipStageItem> equipStage = equipList.get(equip);
		if(equipStage == null)
		{
			HawkLog.errPrintln("invalid equip id:" + equip);
			return null;
		}
		
		EquipStageItem stageItem = equipStage.get(stage);
		if(stageItem == null)
		{
			HawkLog.errPrintln("invalid stage id: " + stage);
			return null;
		}
		
		return stageItem.rondomAddiAttr();
	}
	
	/**
	 * 获取消耗列表
	 */
	public static List<ItemInfo> getDemandList(String equip, int stage, int level){
		List<EquipLevelItem> levelList = equipList.get(equip).get(stage).getLevelList();
		if (level >= levelList.size()) {
			return null;
		}
		return levelList.get(level).getDemandList();
	}

	/**
	 * 获取基础属性列表
	 */
	public static BaseAttrCfg getBaseAttrCfg(String equip, int stage, int level){		
		List<EquipLevelItem> levelList = equipList.get(equip).get(stage).getLevelList();
		if (level >= levelList.size()) {
			return null;
		}
		return levelList.get(level).getBaseAttr();
	}
	
	/**
	 * 获取品级下级别个数
	 */
	public static int getLevelSize(String equip, int stage){
		List<EquipLevelItem> levelList = equipList.get(equip).get(stage).getLevelList();
		return levelList.size();
	}
	
	/**
	 * 获取品级下级别个数
	 */
	public static int getStageSize(String equip){
		return equipList.get(equip).size();
	}
	
	/**
	 * 获取品级列表
	 */
	public static Map<Integer, EquipStageItem> getStageList() {
		return stageList;
	}

	/**
	 * 设置品级列表
	 */
	public static void setStageList(Map<Integer, EquipStageItem> stageList) {
		EquipAttr.stageList = stageList;
	}

	/**
	 * 获取装备列表
	 */
	public static Map<String, Map<Integer, EquipStageItem>> getEquipList() {
		return equipList;
	}

	/**
	 * 设置装备列表
	 */
	public static void setEquipList(Map<String, Map<Integer, EquipStageItem>> equipList) {
		EquipAttr.equipList = equipList;
	}
	
	@Override
	protected boolean assemble() {
		Map<Integer, EquipStageItem> currentEquip = EquipAttr.getEquipList().get(this.id);
		if(currentEquip == null)
		{				
			if (this.stage != 1 || this.level != 0) 
			{
				return false;
			}
			// 新建一个装备map
			currentEquip = new LinkedHashMap<Integer, EquipAttr.EquipStageItem>();			
			EquipAttr.getEquipList().put(this.id, currentEquip);
		}
		
		EquipStageItem currentStage = currentEquip.get(this.stage);
		if (currentStage == null) 
		{
			if (this.level != 0) 
			{
				return false;
			}
			
			// 新建一个品级map
			currentStage = new EquipStageItem();
			if (currentStage.init(this.additionAttr, this.rollCount) == false) 
			{
				return false;		
			}
			
			currentEquip.put(this.stage, currentStage);
		}
		
		EquipLevelItem newItem  = new EquipLevelItem();
		newItem.init(baseAttrId,demand);
		currentStage.getLevelList().add(newItem);
		return true;
	} 
	
	@Override
	protected boolean checkValid() {
		if(HawkConfigManager.getInstance().getConfigByKey(BaseAttrCfg.class, this.baseAttrId) == null)
			return false;
		
		return true;
	}
}