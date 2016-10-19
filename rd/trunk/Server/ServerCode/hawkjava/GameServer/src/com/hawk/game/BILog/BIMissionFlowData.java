package com.hawk.game.BILog;

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.config.LanguageStaticCfg;
import com.hawk.game.player.Player;
import com.hawk.game.util.GsConst;

public class BIMissionFlowData extends BIData{
	/**
	 * 
	 * @param player
	 * @param action
	 * @param maintype
	 * @param subtype
	 * @param id
	 * @param name
	 * @param progress
	 */
	public void log(Player player, Action action, int mainType, int id, String nameId, float progress){
		logBegin(player, "Mission_Flow");
		switch (mainType) {
		case GsConst.Quest.STORY_TYPE:
			jsonPropertyData.put("mission_maintype", "Mission");
			break;
		case GsConst.Quest.DAILY_TYPE:
			jsonPropertyData.put("mission_maintype", "Daily");
			break;
		case GsConst.Quest.BIOGRAPHY_TYPE:
			jsonPropertyData.put("mission_maintype", "Achievement");
			break;
		default:
			break;
		}
		jsonPropertyData.put("mission_subtype", "");
		jsonPropertyData.put("mission_id", id);
		jsonPropertyData.put("mission_name", LanguageStaticCfg.getEnglishName(nameId));
		jsonPropertyData.put("action", action.getBICode());
		jsonPropertyData.put("mission_progress", progress);
		logEnd();
	}
}
