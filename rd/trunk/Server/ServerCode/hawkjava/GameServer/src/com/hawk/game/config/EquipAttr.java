package com.hawk.game.config;
import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;

import com.hawk.game.attr.Attribute;
import com.hawk.game.util.WeightUtil;
import com.hawk.game.util.WeightUtil.WeightItem;

import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

@HawkConfigManager.CsvResource(file = "staticData/equipAttr.csv", struct = "list")
public class EquipAttr extends HawkConfigBase{

	private static Map<String, Map<Integer, EquipStageItem>> equipList = new HashMap<String, Map<Integer, EquipStageItem>>();

	private static class EquipStageItem{
		/** 
  		 * 随机属性强化列表
		 */
		private List<WeightItem<AdditionAttrItem>> weightList;
		/**
		 * roll 次数
		 */
		private int rollCount;
		
		public  EquipStageItem() {
			weightList = new LinkedList<WeightItem<AdditionAttrItem>>();
			rollCount = 0;
		}
		 
		public boolean init(String stageAttrId, String levelAttrId, String additionAttr, int rollCount){
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
			return attr.getAttrMap().isEmpty() == true ? null : attr;
		}
	}
	
	private static class AdditionAttrItem{
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
		
	/**
	 * 配置id
	 */
	protected final String id ;
	/**
	 * 品级
	 */
	protected final int stage;
	/**
	 * 级别基础属性
	 */
	protected final String levelAttrId;
	/**
	 * 品级基础属性
	 */
	protected final String stageAttrId;
	/**
	 * roll次数
	 */
	protected final int rollCount;
	/**
	 * 附加属性
	 */
	protected final String additionAttr;
			
	public  EquipAttr() {
		id = null;
		stage = 0;
		levelAttrId = null;
		stageAttrId = null;
		rollCount = 0;
		additionAttr = null;
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
	 * 获取装备列表
	 */
	private static Map<String, Map<Integer, EquipStageItem>> getEquipList() {
		return equipList;
	}
	
	@Override
	protected boolean assemble() {
		Map<Integer, EquipStageItem> currentEquip = EquipAttr.getEquipList().get(this.id);
		if(currentEquip == null)
		{				
			if (this.stage != 1) 
			{
				return false;
			}
			// 新建一个装备map
			currentEquip = new HashMap<Integer, EquipAttr.EquipStageItem>();			
			EquipAttr.getEquipList().put(this.id, currentEquip);
		}
		
		EquipStageItem currentStage = currentEquip.get(this.stage);
		if (currentStage == null) 
		{			
			// 新建一个品级map
			currentStage = new EquipStageItem();
			if (currentStage.init(this.stageAttrId, this.levelAttrId, this.additionAttr, this.rollCount) == false) 
			{
				return false;		
			}
			
			currentEquip.put(this.stage, currentStage);
			return true;
		}
		
		return false;	
	} 
	
	@Override
	protected boolean checkValid() {
		if(HawkConfigManager.getInstance().getConfigByKey(BaseAttrCfg.class, this.levelAttrId) == null) {
			HawkLog.errPrintln(String.format("config invalid BaseAttrCfg : %d", this.levelAttrId));
			return false;
		}
		
		if(HawkConfigManager.getInstance().getConfigByKey(BaseAttrCfg.class, this.stageAttrId) == null) {
			HawkLog.errPrintln(String.format("config invalid BaseAttrCfg : %d", this.stageAttrId));
			return false;
		}
		
		return true;
	}
}