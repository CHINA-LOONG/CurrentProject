package com.hawk.game.util;

import java.util.LinkedList;
import java.util.List;

import org.hawk.os.HawkException;
import org.hawk.os.HawkRand;

/**
 * 权重计算工具
 * 
 */
public class WeightUtil {
	/**
	 * 权重项
	 * 
	 * @author hawk
	 * @param <T>
	 */
	public static class WeightItem<T> {
		private T value;
		private int weight;

		public static <T> WeightItem<T> valueOf(T value, int weight) {
			WeightItem<T> weightItem = new WeightItem<>();
			weightItem.weight = weight;
			weightItem.value = value;
			return weightItem;
		}

		public int getWeight() {
			return weight;
		}

		public void setWeight(int weight) {
			this.weight = weight;
		}

		public T getValue() {
			return value;
		}

		public void setValue(T value) {
			this.value = value;
		}

		@Override
		public String toString() {
			return "value:" + value + ",weight:" + weight;
		}
	}

	/**
	 * 根据权重列表获得东西
	 * 
	 * @param itemList
	 * @return
	 */
	public static <T> T random(List<WeightItem<T>> itemList) {
		int totalWeight = 0;
		for (WeightItem<T> item : itemList) {
			totalWeight += item.getWeight();
		}

		try {
			int accumulative = 0;
			int randomWeight = HawkRand.randInt(1, totalWeight);
			for (int i = 0; i < itemList.size(); i++) {
				accumulative += itemList.get(i).weight;
				if (randomWeight <= accumulative) {
					return itemList.get(i).getValue();
				}
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}

		return null;
	}
	
	/**
	 * 根据权重列表获得Id
	 * 
	 * @param itemList
	 * @return
	 */
	public static Integer random(String idWeights) {
		return random(convertToList(idWeights));
	}

	/**
	 * 字符串转换为id权重项列表
	 * 
	 * @param idWeights
	 * @return
	 */
	public static List<WeightItem<Integer>> convertToList(String idWeights) {
		List<WeightItem<Integer>> itemList = new LinkedList<>();
		String items[] = idWeights.split(",");
		if (items.length > 0) {
			for (String item : items) {
				String[] idWeight = item.split("_");
				itemList.add(WeightItem.valueOf(Integer.valueOf(idWeight[0]), Integer.valueOf(idWeight[1])));
			}
		}
		return itemList;
	}
}
