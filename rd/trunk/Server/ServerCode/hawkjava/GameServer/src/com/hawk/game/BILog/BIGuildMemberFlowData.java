package com.hawk.game.BILog;

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.player.Player;

public class BIGuildMemberFlowData extends BIData{
	/**
	 * 
	 * @param player
	 * @param action
	 * @param guildId
	 * @param guildName
	 * @param guildLevel
	 * @param afterGuildLevel
	 * @param contributionIn
	 * @param contributionOut
	 * @param contributionBal
	 */
	public void log(Player player, Action action, int guildId, String guildName, int guildLevel, int afterGuildLevel, int contributionIn, int contributionOut, int contributionBal){
		logBegin(player, "Guild_Memberflow");
		jsonPropertyData.put("guild_id", guildId);
		jsonPropertyData.put("guild_name", guildName);
		jsonPropertyData.put("guild_level", guildLevel);
		jsonPropertyData.put("after_guild_level", afterGuildLevel);
		jsonPropertyData.put("contribution_in", contributionIn);
		jsonPropertyData.put("contribution_out", contributionOut);
		jsonPropertyData.put("contribution_bal", contributionBal);
		jsonPropertyData.put("action", action.getBICode());
		logEnd();
	}
}
