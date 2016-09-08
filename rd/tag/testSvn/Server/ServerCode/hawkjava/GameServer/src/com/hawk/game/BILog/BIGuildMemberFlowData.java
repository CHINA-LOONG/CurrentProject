package com.hawk.game.BILog;

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.player.Player;

public class BIGuildMemberFlowData extends BIData{
	/**
	 * 
	 * @param player
	 * @param action
	 * @param allianceEntity
	 * @param playerAllianceEntity
	 * @param contributionIn
	 * @param contributionOut
	 * @param targetId
	 */
	public void logContribution(Player player, Action action, AllianceEntity allianceEntity, PlayerAllianceEntity playerAllianceEntity, int contributionIn, int contributionOut){
		logBegin(player, "Guild_Memberflow");
		jsonPropertyData.put("guild_id", allianceEntity.getId());
		jsonPropertyData.put("guild_name", allianceEntity.getName());
		jsonPropertyData.put("member_id", "");
		jsonPropertyData.put("after_guild_level", playerAllianceEntity.getPostion());
		jsonPropertyData.put("contribution_in", contributionIn);
		jsonPropertyData.put("contribution_out", contributionOut);
		jsonPropertyData.put("contribution_bal", playerAllianceEntity.getContribution());
		jsonPropertyData.put("action", action.getBICode());
		logEnd();
	}
	
	/**
	 * 
	 * @param player
	 * @param action
	 * @param allianceEntity
	 * @param playerAllianceEntity
	 * @param targetId
	 */
	public void logOperation(Player player, Action action, AllianceEntity allianceEntity, PlayerAllianceEntity playerAllianceEntity, String targetId){
		logBegin(player, "Guild_Memberflow");
		jsonPropertyData.put("guild_id", allianceEntity.getId());
		jsonPropertyData.put("guild_name", allianceEntity.getName());
		jsonPropertyData.put("member_id", targetId);
		jsonPropertyData.put("after_guild_level", playerAllianceEntity.getPostion());
		jsonPropertyData.put("contribution_in", 0);
		jsonPropertyData.put("contribution_out", 0);
		jsonPropertyData.put("contribution_bal", playerAllianceEntity.getContribution());
		jsonPropertyData.put("action", action.getBICode());
		logEnd();
	}
}
