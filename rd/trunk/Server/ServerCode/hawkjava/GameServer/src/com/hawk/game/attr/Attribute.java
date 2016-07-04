package com.hawk.game.attr;

import java.util.Iterator;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;

import com.hawk.game.protocol.Const;

/**
 * 属性定义&属性计算
 * 
 */
public class Attribute {
	/**
	 * 属性映射表
	 */
	private Map<Const.attr, Float> attrMap;

	public Attribute() {
		attrMap = new ConcurrentHashMap<Const.attr, Float>();
	}

	public Map<Const.attr, Float> getAttrMap() {
		return attrMap;
	}
	
	public void add(int attrType, float value) {
		if (attrType > 0) {
			Const.attr attrTypeEnum = Const.attr.valueOf(attrType);
			if (attrMap.containsKey(attrTypeEnum)) {
				attrMap.put(attrTypeEnum, attrMap.get(attrTypeEnum) + value);
			} else {
				attrMap.put(attrTypeEnum, value);
			}
		}
	}
	
	public void add(Const.attr attrType, float value) {
		if (attrMap.containsKey(attrType)) {
			attrMap.put(attrType, attrMap.get(attrType) + value);
		} else {
			attrMap.put(attrType, value);
		}
	}
	
	public void set(Const.attr attrType, float value) {
		attrMap.put(attrType, value);
	}

	public Attribute add(Attribute attr) {
		if (attr != null) {
			for (Map.Entry<Const.attr, Float> entry : attr.attrMap.entrySet()) {
				add(entry.getKey(), attr.getValue(entry.getKey()));
			}
		}
		return this;
	}

	public Attribute sub(Attribute attr) {
		if (attr != null) {
			for (Map.Entry<Const.attr, Float> entry : attr.attrMap.entrySet()) {
				float value = this.getValue(entry.getKey()) - attr.getValue(entry.getKey());
				set(entry.getKey(), value);
			}
		}
		return this;
	}

	public Attribute multiplicate(Attribute attr) {
		if (attr != null) {
			for (Map.Entry<Const.attr, Float> entry : attr.attrMap.entrySet()) {
				set(entry.getKey(), (entry.getValue() * attr.getValue(entry.getKey())));
			}
		}
		return this;
	}
	
	public Attribute multiplicate(float level) {
		for (Map.Entry<Const.attr, Float> entry : attrMap.entrySet()) {
			set(entry.getKey(), (entry.getValue() * level));
		}
		return this;
	}

	public Attribute additionAttr(Attribute addition) {
		if (addition != null) {
			for (Map.Entry<Const.attr, Float> entry : addition.attrMap.entrySet()) {
				float additionRatio = 0.0001f * addition.getValue(entry.getKey());
				set(entry.getKey(), (int)(this.getValue(entry.getKey()) * (1.0f + additionRatio)));
			}
		}
		return this;
	}
	
	public float getValue(int attrType) {
		if(attrType <= 0) {
			return 0;
		}
		
		Const.attr attrEnumType = Const.attr.valueOf(attrType);
		if (attrMap.containsKey(attrEnumType)) {
			return attrMap.get(attrEnumType);
		}
		return 0;
	}
	
	public float getValue(Const.attr attrType) {
		if (attrMap.containsKey(attrType)) {
			return attrMap.get(attrType);
		}
		return 0;
	}

	public boolean containsAttr(int attrType) {
		if (attrType <= 0) {
			return false;
		}
		
		return attrMap.containsKey(Const.attr.valueOf(attrType));
	}
	
	public boolean containsAttr(Const.attr attrType) {
		return attrMap.containsKey(attrType);
	}
	
	public void clear() {
		for (Map.Entry<Const.attr, Float> entry : attrMap.entrySet()) {
			this.setAttr(entry.getKey(), 0);
		}
	}

	public Attribute clone() {
		Attribute attribute = new Attribute();
		for (Map.Entry<Const.attr, Float> entry : attrMap.entrySet()) {
			attribute.setAttr(entry.getKey(), entry.getValue());
		}
		return attribute;
	}
	
	public void setAttr(int attrType, float value) {
		if (attrType <= 0) {
			return;
		}
		
		attrMap.put(Const.attr.valueOf(attrType), value);
	}
	
	public void setAttr(Const.attr attrType, float value) {
		attrMap.put(attrType, value);
	}

	public float getSumAttrValue() {
		int attrValue = 0;
		for (Map.Entry<Const.attr, Float> entry : attrMap.entrySet()) {
			attrValue += entry.getValue();
		}
		return attrValue;
	}
	
	public boolean isEmpty() {
		return this.attrMap.size() <= 0;
	}
	
	@Override
	public String toString() {
		String info = "";
		Iterator<Map.Entry<Const.attr, Float>> iterator = attrMap.entrySet().iterator();
		while (iterator.hasNext()) {
			Map.Entry<Const.attr, Float> entry = iterator.next();
			info += (entry.getKey().getNumber() + "_" + entry.getValue());
			if (iterator.hasNext()) {
				info += ",";
			}
		}
		return info;
	}

	public boolean initByString(String infos) {
		if (infos != null) {
			for (String info : infos.split(",")) {
				String[] items = info.split("_");
				if (items.length != 2) {
					return false;
				}
				add(Const.attr.valueOf(Integer.valueOf(items[0])), Float.valueOf(items[1]));
			}
		}
		return true;
	}
	
	public static Attribute valueOf(String infos) {
		Attribute attribute = new Attribute();
		if (attribute.initByString(infos)) {
			return attribute;
		}
		return null;
	}
}
