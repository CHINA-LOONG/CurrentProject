package com.hawk.game.BILog;

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.player.Player;

public class BIRoundFlowData extends BIData{
	/**
	 * 
	 * @param player
	 * @param action
	 * @param type
	 * @param id
	 * @param difficulty
	 * @param star
	 * @param time
	 * @param result
	 */
	public void log(Player player, Action action, String type, String id, String difficulty, int star, int time, int result){
		logBegin(player, "Round_Flow");
		jsonPropertyData.put("battle_type", type);
		jsonPropertyData.put("pve_id", id);
		jsonPropertyData.put("pve_difficulty", difficulty);
		jsonPropertyData.put("action", action.getBICode());
		jsonPropertyData.put("round_star", star);
		jsonPropertyData.put("round_time", time);
		jsonPropertyData.put("round_result", result);
		logEnd();
	}
}
