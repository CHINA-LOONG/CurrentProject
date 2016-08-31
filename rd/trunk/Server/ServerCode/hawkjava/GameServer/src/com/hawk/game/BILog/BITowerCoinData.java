package com.hawk.game.BILog;

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.player.Player;

public class BITowerCoinData extends BIData{
	/**
	 * 
	 * @param player
	 * @param action
	 * @param tcIn
	 * @param tcOut
	 * @param tcBal
	 */
	public void log(Player player, Action action, int tcIn, int tcOut, int tcBal){
		logBegin(player, "tc_transaction");
		jsonPropertyData.put("tc_in", tcIn);	
		jsonPropertyData.put("tc_out", tcOut);	
		jsonPropertyData.put("tc_bal", tcBal);	
		jsonPropertyData.put("action", action.getBICode());	
		logEnd();
	}
}
