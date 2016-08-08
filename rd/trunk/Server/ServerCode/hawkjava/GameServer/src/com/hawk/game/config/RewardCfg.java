package com.hawk.game.config;

import java.util.Collections;
import java.util.LinkedList;
import java.util.List;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

import com.hawk.game.item.ItemInfo;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.Const.itemType;
import com.hawk.game.util.RatioUtil;
import com.hawk.game.util.RatioUtil.RatioItem;

@HawkConfigManager.CsvResource(file = "staticData/reward.csv", struct = "map")
public class RewardCfg extends HawkConfigBase  {
	/**
	 * 配置id
	 */
	@Id
	protected final String id ;
	/**
	 * 奖励配置
	 */
	protected final String reward;
	/**
	 * 奖励概率列表
	 */
	private List<RatioItem<ItemInfo>> ratioList;
	
	public RewardCfg() {
		id = null;
		reward = null;
		ratioList = new LinkedList<RatioItem<ItemInfo>>();
	}

	public List<RatioItem<ItemInfo>> getRatioList() {
		return Collections.unmodifiableList(ratioList);
	}
	
	public  List<ItemInfo> getRewardList() {
//		RewardCfg reward = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, id);
//		if (reward == null) {
//			return null;
//		}
//		List<ItemInfo> result = RatioUtil.random(reward.getRatioList());
		List<ItemInfo> result = RatioUtil.random(getRatioList());
		for (int i = 0; i < result.size();) {
			ItemInfo item = result.get(i);
			if (item.getType() == Const.itemType.GROUP_VALUE) {
				RewardGroupCfg group = HawkConfigManager.getInstance().getConfigByKey(RewardGroupCfg.class, item.getItemId());
				result.remove(i);
				if (group != null) {
					for (int j = 0; j < item.getCount(); j++) {
						ItemInfo weightItem = group.getRewardItem();
						if (weightItem.getType() != itemType.NONE_ITEM_VALUE) {
							result.add(weightItem);
						}
					}
				}
			}	
			else {
				i++;
			}			
		}
		return result;
	}  

	@Override
	protected boolean assemble() {
		ratioList.clear();
		
		if (reward != null && reward.length() > 0 && !"0".equals(reward)) {
			String[] itemArrays = reward.split(",");
			for (String itemArray : itemArrays) {
				String[] items = itemArray.split("_");
				if (items.length == 4) {
					ItemInfo item = ItemInfo.valueOf(Integer.valueOf(items[0]), items[1], Integer.valueOf(items[2]));
					if (item.getType() != Const.itemType.EQUIP_VALUE && item.getType() != Const.itemType.MONSTER_VALUE)
					{
						ratioList.add(RatioItem.valueOf(item, Float.valueOf(items[3])));
					}
					else {
						return false;
					}	
				}
				else if (items.length == 5) {
					ItemInfo item = ItemInfo.valueOf(Integer.valueOf(items[0]), items[1], Integer.valueOf(items[2]), Integer.valueOf(items[3]));
					if (item.getType() == Const.itemType.MONSTER_VALUE)
					{
						ratioList.add(RatioItem.valueOf(item, Float.valueOf(items[4])));
					}
					else {
						return false;
					}
				}
				else if (items.length == 6) {
					ItemInfo item = ItemInfo.valueOf(Integer.valueOf(items[0]), items[1], Integer.valueOf(items[2]), Integer.valueOf(items[3]), Integer.valueOf(items[4]));
					if (item.getType() == Const.itemType.EQUIP_VALUE)
					{
						ratioList.add(RatioItem.valueOf(item, Float.valueOf(items[5])));
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
