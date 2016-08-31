package com.hawk.game.BILog;

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.player.Player;

public class BICoinData extends BIData{
	/**
	 * 
	 * @param player
	 * @param action
	 * @param coinIn 金币增加值
	 * @param coinOut 金币减少值
	 * @param coinBal 事件后金币存量
	 */
	public void log(Player player, Action action, long coinIn, long coinOut, long coinBal) {
		logBegin(player, "coins_transaction");
		jsonPropertyData.put("coins_in", coinIn);
		jsonPropertyData.put("coins_out", coinOut);
		jsonPropertyData.put("coins_bal", coinBal);
		jsonPropertyData.put("action", action.getBICode());
		logEnd();
	}
}
