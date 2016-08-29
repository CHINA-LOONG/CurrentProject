package com.hawk.game.BILog;

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.player.Player;

public class BIPlayerLevelData extends BIData{
	/**
	 * 
	 * @param player
	 * @param action
	 * @param level
	 * @param afterLevel
	 * @param expNum
	 * @param playerExp
	 * @param afterPlayerExp
	 */
	public void log(Player player, Action action, int level, int afterLevel, int expNum, int playerExp, int afterPlayerExp){
		logBegin(player, "Player_levelup");
		jsonPropertyData.put("playerlevel", level);
		jsonPropertyData.put("after_playerlevel", afterLevel);
		jsonPropertyData.put("exp_num", expNum);
		jsonPropertyData.put("playerexp", playerExp);
		jsonPropertyData.put("after_playerexp", afterPlayerExp);
		jsonPropertyData.put("action", action.getBICode());	
		logEnd();
	}
}
