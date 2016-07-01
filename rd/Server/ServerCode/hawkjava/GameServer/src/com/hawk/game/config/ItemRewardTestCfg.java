package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.XmlResource(file = "xml/item_reward_nest.xml", struct = "map")
@HawkConfigManager.XMLNestResource(nestValueType = ItemRewardTestCfg.RewardItem.class, struct = "map")
public class ItemRewardTestCfg extends HawkConfigBase  {

    public static class RewardItemChild extends HawkConfigBase {  	
    	@Id
    	protected final String id;
    	protected final float ratio;
    	protected final int min;
    	protected final int max;
    	
    	public RewardItemChild()
    	{
    		id = "";
    		ratio = 1.f;;
    		min = 0;
    		max = 0;		
    	}	
   } 
    
    @HawkConfigManager.XMLNestResource(nestValueType = ItemRewardTestCfg.RewardItemChild.class, struct = "map")
    public static class RewardItem extends HawkConfigBase { 
    	@Id
    	protected final String type;
    	protected final float ratio;
    	protected final int min;
    	protected final int max;
    	
    	public RewardItem()
    	{
    		type = "";
    		ratio = 1.f;;
    		min = 0;
    		max = 0;		
    	} 	
   } 
    
    @Id
	protected final String id;
   
	public ItemRewardTestCfg() {
		id = "";
	}
}
