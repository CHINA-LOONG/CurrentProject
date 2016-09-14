package com.hawk.game.util;

import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.hawk.config.HawkConfigManager;
import org.hawk.os.HawkRand;

import com.hawk.game.config.AdventureCfg;
import com.hawk.game.config.AdventureConditionNumCfg;
import com.hawk.game.config.AdventureConditionTypeCfg;
import com.hawk.game.config.MonsterCfg;

public class AdventureUtil {

	public static class AdventureCondition {
		public int monsterCount;
		public int monsterType;
		public int monsterProperty;
		public int conditionTypeCfgId;

		public AdventureCondition(int count, AdventureConditionTypeCfg typeCfg) {
			monsterCount = count;
			monsterType = typeCfg.getMonsterType();
			monsterProperty = typeCfg.getMonsterProperty();
			conditionTypeCfgId = typeCfg.getId();
		}
	}

	// <type, <time, List<cfg>>>
	private static Map<Integer, Map<Integer, List<AdventureCfg>>> advenMap = new HashMap<>();

	// 构造阶段---------------------------------------------------------------------

	public static void addAdventureCfg(AdventureCfg advenCfg) {
		Map<Integer, List<AdventureCfg>> gearMap = advenMap.get(advenCfg.getType());
		if (null == gearMap) {
			gearMap = new HashMap<> ();
			advenMap.put(advenCfg.getType(), gearMap);
		}
		List<AdventureCfg> advenList = gearMap.get(advenCfg.getGear());
		if (null == advenList) {
			advenList = new ArrayList<> ();
			gearMap.put(advenCfg.getGear(), advenList);
		}
		advenList.add(advenCfg);
	}

	// 使用阶段----------------------------------------------------------------------

	public static Map<Integer, Map<Integer, List<AdventureCfg>>> getAdventureMap() {
		return Collections.unmodifiableMap(advenMap);
	}

	public static AdventureCfg getAdventureCfg(int type, int gear, int level) {
		Map<Integer, List<AdventureCfg>> gearMap = advenMap.get(type);
		if (null != gearMap) {
			List<AdventureCfg> advenList = gearMap.get(gear);
			if (null != advenList) {
				for (int i = 0; i < advenList.size(); ++i) {
					AdventureCfg cfg = advenList.get(i);
					if (cfg.isInLevelRange(level)) {
						return cfg;
					}
				}
			}
		}
		return null;
	}

	/**
	 * 检测怪物组是否满足条件组
	 */
	public static boolean isConditionMeet(List<AdventureCondition> conditionList, List<MonsterCfg> monsterList) {
		// conditionList为空的情况视为符合条件，不予处理
		for (AdventureCondition condition : conditionList) {
			int meetCount = 0;
			for (MonsterCfg monsterCfg : monsterList) {
				if ((GsConst.UNUSABLE == condition.monsterType || monsterCfg.getType() == condition.monsterType)
						&& (GsConst.UNUSABLE == condition.monsterProperty || monsterCfg.getProperty() == condition.monsterProperty)) {
					++meetCount;
				}
			}

			if (meetCount < condition.monsterCount) {
				return false;
			}
		}

		return true;
	}

	/**
	 * 生成玩家等级对应的条件组
	 * @return 如果level没有对应条件列表，返回null
	 */
	public static List<AdventureCondition> genConditionList(int level) {
		List<Integer> countList = getCountList(level);
		if (null == countList) {
			return null;
		}

		int conditionCount = countList.size();
		List<AdventureConditionTypeCfg> typeList = getTypeList(level, conditionCount);
		if (null == typeList) {
			return null;
		}

		HawkRand.randomOrder(countList);

		List<AdventureCondition> conditionList = new ArrayList<AdventureCondition> (conditionCount);
		for (int i = 0; i < conditionCount; ++i) {
			AdventureConditionTypeCfg typeCfg = typeList.get(i);
			AdventureCondition condition = new AdventureCondition(countList.get(i), typeCfg);
			conditionList.add(condition);
		}

		return conditionList;
	}

	// 内部方法----------------------------------------------------------------------

	/**
	 * 生成玩家等级对应的条件组数量部分
	 * @return 如果level没有对应数量列表，返回null
	 */
	private static List<Integer> getCountList(int level) {
		List<List<Integer>> objectList = new ArrayList<List<Integer>> ();
		List<Integer> weightList = new ArrayList<Integer> ();

		Map<Object, AdventureConditionNumCfg> numCfgMap = HawkConfigManager.getInstance().getConfigMap(AdventureConditionNumCfg.class);
		for (AdventureConditionNumCfg numCfg : numCfgMap.values()) {
			if (true == numCfg.isInLevelRange(level)) {
				objectList.add(numCfg.getMonsterCountList());
				weightList.add(numCfg.getWeight());
			}
		}

		if (true == objectList.isEmpty()) {
			return null;
		}
		return HawkRand.randonWeightObject(objectList, weightList);
	}

	/**
	 * 生成玩家等级对应的条件组类型部分
	 * @return 如果level没有对应类型列表，返回null
	 */
	private static List<AdventureConditionTypeCfg> getTypeList(int level, int count) {
		List<AdventureConditionTypeCfg> objectList = new ArrayList<AdventureConditionTypeCfg> ();
		List<Integer> weightList = new ArrayList<Integer> ();

		Map<Object, AdventureConditionTypeCfg> typeCfgMap = HawkConfigManager.getInstance().getConfigMap(AdventureConditionTypeCfg.class);
		for (AdventureConditionTypeCfg typeCfg : typeCfgMap.values()) {
			if (true == typeCfg.isInLevelRange(level)) {
				objectList.add(typeCfg);
				weightList.add(typeCfg.getWeight());
			}
		}

		if (true == objectList.isEmpty()) {
			return null;
		}
		return HawkRand.randonWeightObject(objectList, weightList, count);
	}

}