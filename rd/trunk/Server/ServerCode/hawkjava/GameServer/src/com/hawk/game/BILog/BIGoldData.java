package com.hawk.game.BILog;

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.player.Player;

public class BIGoldData extends BIData{
	/**
	 * 
	 * @param player
	 * @param action
	 * @param rcIn 钻石增加值
	 * @param rcOut 钻石减少值
	 * @param rcBal 事件后钻石存量
	 */
	public void log(Player player, Action action, int rcIn, int rcOut, int rcBal) {
		logBegin(player, "rc_transaction");
		jsonPropertyData.put("rc_in", rcIn);
		jsonPropertyData.put("rc_out", rcOut);
		jsonPropertyData.put("rc_bal", rcBal);
		jsonPropertyData.put("action", action.getBICode());
		logEnd();
	}
}
