package com.hawk.game.config;

import java.util.Collections;
import java.util.LinkedList;
import java.util.List;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

import com.hawk.game.item.ItemInfo;
import com.hawk.game.util.GsConst.ItemParseType;

@HawkConfigManager.CsvResource(file = "staticData/equipForge.csv", struct = "list")
public class EquipForgeCfg extends HawkConfigBase {
	/**
	 * 级别
	 */
	@Id
	protected final String stageLevel;
	/**
	 * 成功率
	 */
	protected final float successRate;
	/**
	 * 等级限制
	 */
	protected final int playerlevelDemand;
	/**
	 * 升级消耗列表
	 */
	protected final String levelDemand;	
	/**
	 * 打孔消耗列表
	 */
	protected final String punchDemand;	
	/**
	 * 
	 */
	protected final String decompose;	
	/**
	 * 升级消耗
	 */
	private List<ItemInfo> levelDemandList;
	/**
	 * 开孔消耗
	 */
	private List<ItemInfo> punchDemandList;
	/**
	 * 分解列表
	 */
	private List<ItemInfo> decomposeList;
	
	public  EquipForgeCfg() {
		stageLevel = null;
		successRate = .0f;
		playerlevelDemand = 0;
		levelDemand = null;
		punchDemand = null;
		decompose = null;
		levelDemandList = new LinkedList<ItemInfo>();
		punchDemandList = new LinkedList<ItemInfo>();
		decomposeList = new LinkedList<ItemInfo>();
	}
	
	public String getStageLevel() {
		return stageLevel;
	}

	public float getSuccessRate() {
		return successRate;
	}

	public int getPlayerlevelDemand() {
		return playerlevelDemand;
	}

	public String getLevelDemand() {
		return levelDemand;
	}

	public String getPunchDemand() {
		return punchDemand;
	}

	private List<ItemInfo> getLevelDemandList() {
		return levelDemandList;
	}

	private List<ItemInfo> getPunchDemandList() {
		return punchDemandList;
	}

	public List<ItemInfo> getDecomposeList() {
		return decomposeList;
	}
	
	@Override
	protected boolean assemble() {
		levelDemandList.clear();
		punchDemandList.clear();
		if (levelDemand != null && levelDemand.length() > 0 && !"0".equals(levelDemand)) {
			levelDemandList = ItemInfo.GetItemInfo(levelDemand, ItemParseType.PARSE_EQUIP_ATTR);
		}
				
		if (punchDemand != null && punchDemand.length() > 0 && !"0".equals(punchDemand)) {
			punchDemandList = ItemInfo.GetItemInfo(punchDemand, ItemParseType.PARSE_EQUIP_ATTR);
		}	

		if (decompose != null && decompose.length() > 0 && !"0".equals(decompose)) {
			decomposeList = ItemInfo.GetItemInfo(decompose, ItemParseType.PARSE_EQUIP_ATTR);
		}	
		
		return true;
	} 
	
	@Override
	protected boolean checkValid() {
		return true;
	}
	
	public static List<ItemInfo> getLevelDemandList(int stage, int level)
	{
		EquipForgeCfg equipForgeCfg = HawkConfigManager.getInstance().getConfigByIndex(EquipForgeCfg.class, (stage - 1) * 10 + level);
		if (equipForgeCfg != null) {
			return Collections.unmodifiableList(equipForgeCfg.getLevelDemandList());
		}
		return null;
	}
	
	public static List<ItemInfo> getPunchDemandList(int stage, int level)
	{
		EquipForgeCfg equipForgeCfg = HawkConfigManager.getInstance().getConfigByIndex(EquipForgeCfg.class, (stage - 1) * 10 + level );
		if (equipForgeCfg != null) {
			return Collections.unmodifiableList(equipForgeCfg.getPunchDemandList());
		}
		return null;
	}
	
	public static List<ItemInfo> getDecomposeDemandList(int stage, int level)
	{
		EquipForgeCfg equipForgeCfg = HawkConfigManager.getInstance().getConfigByIndex(EquipForgeCfg.class, (stage - 1) * 10 + level );
		if (equipForgeCfg != null) {
			return equipForgeCfg.getDecomposeList();
		}
		return null;
	}
	
	public static float getSuccessRate(int stage, int level)
	{
		EquipForgeCfg equipForgeCfg = HawkConfigManager.getInstance().getConfigByIndex(EquipForgeCfg.class, (stage - 1) * 10 + level);
		if (equipForgeCfg != null) {
			return equipForgeCfg.successRate;
		}
		return 0;
	}
	
	public static int getPlayerLevelDemand(int stage, int level)
	{
		EquipForgeCfg equipForgeCfg = HawkConfigManager.getInstance().getConfigByIndex(EquipForgeCfg.class, (stage - 1) * 10 + level);
		if (equipForgeCfg != null) {
			return equipForgeCfg.playerlevelDemand;
		}
		return 0;
	}
}
