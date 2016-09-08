package com.hawk.game.util;

import java.util.LinkedList;
import java.util.List;

import org.hawk.os.HawkException;
import org.hawk.os.HawkRand;

/**
 * 概率计算工具
 * 
 */
public class RatioUtil {
	/**
	 * 概率项
	 * 
	 * @author zs
	 * @param <T>
	 */
	public static class RatioItem<T> {
		private T value;
		private float ratio;

		public static <T> RatioItem<T> valueOf(T value, float ratio) {
			RatioItem<T> ratioItem = new RatioItem<>();
			ratioItem.ratio = ratio;
			ratioItem.value = value;
			return ratioItem;
		}

		public float getRatio() {
			return ratio;
		}

		public void setRatio(float ratio) {
			this.ratio = ratio;
		}

		public T getValue() {
			return value;
		}

		public void setValue(T value) {
			this.value = value;
		}

		@Override
		public String toString() {
			return "value:" + value + ",ratio:" + ratio;
		}
	}

	/**
	 * 根据概率列表获得东西
	 * 
	 * @param itemList
	 * @return
	 */
	public static <T> List<T> random(List<RatioItem<T>> itemList) {
		List<T> result = new LinkedList<>();
		
		try {
			for (int i = 0; i < itemList.size(); i++) {
				float randomRatio = HawkRand.randInt(1, 100) / 100.f;
				float ratio = itemList.get(i).getRatio();
				if (randomRatio <= ratio) {
					result.add(itemList.get(i).getValue());
				}
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}

		return result;
	}
	
	/**
	 * 根据概率列表获得Id
	 * 
	 * @param itemList
	 * @return
	 */
	public static List<Integer> random(String idRatios) {
		return random(convertToList(idRatios));
	}

	/**
	 * 字符串转换为id概率项列表
	 * 
	 * @param idRatios
	 * @return
	 */
	public static List<RatioItem<Integer>> convertToList(String idRatios) {
		List<RatioItem<Integer>> itemList = new LinkedList<>();
		String items[] = idRatios.split(",");
		if (items.length > 0) {
			for (String item : items) {
				String[] idRatio = item.split("_");
				itemList.add(RatioItem.valueOf(Integer.valueOf(idRatio[0]), Float.valueOf(idRatio[1])));
			}
		}
		return itemList;
	}
}
