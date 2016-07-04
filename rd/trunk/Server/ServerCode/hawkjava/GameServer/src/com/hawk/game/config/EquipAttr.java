package com.hawk.game.config;
import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

import com.hawk.game.item.ItemInfo;
import com.hawk.game.util.WeightUtil.WeightItem;

import java.util.LinkedHashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

@HawkConfigManager.CsvResource(file = "xml/equipAttr.csv", struct = "list")
public class EquipAttr extends HawkConfigBase{

	private static Map<Integer, EquipStageItem> stageList = new LinkedHashMap<Integer, EquipStageItem>();
	private static Map<Integer, Map<Integer, EquipStageItem>> equipList = new LinkedHashMap<Integer, Map<Integer, EquipStageItem>>();

	private static class EquipStageItem{
		/**
		 * 强化级别对应属性列表
		 */
		List<EquipLevelItem> levelList;
		List<WeightItem<AdditionAttrItem>> weightList;
		
		public  EquipStageItem() {
			levelList = new LinkedList<EquipLevelItem>();
			weightList = new LinkedList<WeightItem<AdditionAttrItem>>();
		}
		
		public boolean init(String additionAttr){
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
			
			return true;
		}

		public List<EquipLevelItem> getLevelList() {
			return levelList;
		}

		public void setLevelList(List<EquipLevelItem> levelList) {
			this.levelList = levelList;
		}

		public List<WeightItem<AdditionAttrItem>> getWeightList() {
			return weightList;
		}

		public void setWeightList(List<WeightItem<AdditionAttrItem>> weightList) {
			this.weightList = weightList;
		}
	}
	
	private static class AdditionAttrItem{
		/**
		 * 附加属性类型
		 */
		int type;
		/**
		 * 附加属性值
		 */
		float value;
		
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
	
	private static class EquipLevelItem{
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
					itemInfo.setItemId(Integer.valueOf(items[1]));
					itemInfo.setCount(Integer.valueOf(items[2]));
					demandList.add(itemInfo);
				}
			}	
			
			return true;
		}

		public BaseAttrCfg getBaseAttr() {
			return HawkConfigManager.getInstance().getConfigByKey(BaseAttrCfg.class, this.baseAttrId);
		}

		public List<ItemInfo> getDemandList() {
			return demandList;
		}
	}
		
	/**
	 * 配置id
	 */
	protected final int id ;
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
		id = 0;
		stage = 0;
		level = 0;
		baseAttrId = 0;
		rollCount = 0;
		additionAttr = null;
		demand = null;
	}
	
	public static Map<Integer, EquipStageItem> getStageList() {
		return stageList;
	}

	public static void setStageList(Map<Integer, EquipStageItem> stageList) {
		EquipAttr.stageList = stageList;
	}

	public static Map<Integer, Map<Integer, EquipStageItem>> getEquipList() {
		return equipList;
	}

	public static void setEquipList(Map<Integer, Map<Integer, EquipStageItem>> equipList) {
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
			if (currentStage.init(this.additionAttr) == false) 
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