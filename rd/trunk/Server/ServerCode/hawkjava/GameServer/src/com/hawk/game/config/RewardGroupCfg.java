package com.hawk.game.config;

import java.util.LinkedList;
import java.util.List;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.config.HawkConfigBase.Id;

import com.hawk.game.item.ItemInfo;
import com.hawk.game.util.ConfigUtil;
import com.hawk.game.util.WeightUtil;
import com.hawk.game.util.WeightUtil.WeightItem;

@HawkConfigManager.CsvResource(file = "xml/rewardGroup.csv", struct = "map")
public class RewardGroupCfg extends HawkConfigBase  {
	/**
	 * 配置id
	 */
	@Id
	protected final int id ;
	/**
	 * 奖励配置
	 */
	protected final String rewardGroup;
	/**
	 * 奖励权重列表列表
	 */
	private List<WeightItem<ItemInfo>> weightList;
	
	public RewardGroupCfg() {
		id = 0;
		rewardGroup = null;
		weightList = new LinkedList<WeightItem<ItemInfo>>();
	}

	public List<WeightItem<ItemInfo>> getWeightList() {
		return weightList;
	}

	public void setWeightList(List<WeightItem<ItemInfo>> weightList) {
		this.weightList = weightList;
	}

	public ItemInfo getRewardItem() {
		return WeightUtil.random(weightList);
	}
	
	@Override
	protected boolean assemble() {
		weightList.clear();
		
		if (rewardGroup != null && rewardGroup.length() > 0 && !"0".equals(rewardGroup)) {
			String[] itemArrays = rewardGroup.split(",");
			for (String itemArray : itemArrays) {
				String[] items = itemArray.split("_");
				if (items.length == 4) {
					ItemInfo item = ItemInfo.valueOf(Integer.valueOf(items[0]), Integer.valueOf(items[1]), Integer.valueOf(items[2]));
					if (ConfigUtil.checkIsNotEquip(item.getType()))
					{
						weightList.add(WeightItem.valueOf(item, Integer.valueOf(items[3])));
					}
					else {
						return false;
					}	
				}
				else if (items.length == 6) {
					ItemInfo item = ItemInfo.valueOf(Integer.valueOf(items[0]), Integer.valueOf(items[1]), Integer.valueOf(items[2]), Integer.valueOf(items[3]), Integer.valueOf(items[4]));
					if (ConfigUtil.checkIsNotEquip(item.getType()))
					{
						weightList.add(WeightItem.valueOf(item, Integer.valueOf(items[5])));
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
	
		
		
		return true;
	}
}
