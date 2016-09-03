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
	 * @param oldPosition
	 */
	public void log(Player player, Action action, AllianceEntity allianceEntity, PlayerAllianceEntity playerAllianceEntity, int contributionIn, int contributionOut, int oldPosition){
		logBegin(player, "Guild_Memberflow");
		jsonPropertyData.put("guild_id", allianceEntity.getId());
		jsonPropertyData.put("guild_name", allianceEntity.getName());
		jsonPropertyData.put("guild_level", oldPosition);
		jsonPropertyData.put("after_guild_level", playerAllianceEntity.getPostion());
		jsonPropertyData.put("contribution_in", contributionIn);
		jsonPropertyData.put("contribution_out", contributionOut);
		jsonPropertyData.put("contribution_bal", playerAllianceEntity.getContribution());
		jsonPropertyData.put("action", action.getBICode());
		logEnd();
	}
}
