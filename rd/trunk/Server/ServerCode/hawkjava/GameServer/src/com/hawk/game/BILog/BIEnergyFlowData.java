package com.hawk.game.BILog;

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.player.Player;

public class BIEnergyFlowData extends BIData{
	/**
	 * 
	 * @param player
	 * @param action
	 * @param type : battle_type
	 * @param id : pve_id
	 * @param difficulty : pve_difficulty
	 * @param energyIn
	 * @param energyOut
	 * @param energyBal
	 */
	public void log(Player player, Action action, String type, String id, String difficulty, int energyIn, int energyOut, int energyBal){
		logBegin(player, "Energy_Flow");
		jsonPropertyData.put("reason", action.getBICode());
		jsonPropertyData.put("battle_type", type);
		jsonPropertyData.put("pve_id", id);
		jsonPropertyData.put("pve_difficulty", difficulty);
		jsonPropertyData.put("energy_in", energyIn);
		jsonPropertyData.put("energy_out", energyOut);
		jsonPropertyData.put("energy_bal", energyBal);
		logEnd();
	}
}
