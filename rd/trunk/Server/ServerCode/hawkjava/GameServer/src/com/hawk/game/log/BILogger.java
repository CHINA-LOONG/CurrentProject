package com.hawk.game.log;

import org.apache.log4j.Logger;

import com.hawk.game.BILog.BIBehaviorAction.BIType;
import com.hawk.game.BILog.BICoinData;
import com.hawk.game.BILog.BIData;
import com.hawk.game.BILog.BIEnergyFlowData;
import com.hawk.game.BILog.BIEquipFlowData;
import com.hawk.game.BILog.BIEquipIntensifyData;
import com.hawk.game.BILog.BIGemFlowData;
import com.hawk.game.BILog.BIGoldData;
import com.hawk.game.BILog.BIGuildFlowData;
import com.hawk.game.BILog.BIGuildMemberFlowData;
import com.hawk.game.BILog.BIItemData;
import com.hawk.game.BILog.BIMissionFlowData;
import com.hawk.game.BILog.BIPVEData;
import com.hawk.game.BILog.BIPetEnhanceData;
import com.hawk.game.BILog.BIPetFlowData;
import com.hawk.game.BILog.BIPetLevelUpData;
import com.hawk.game.BILog.BIPetSkillData;
import com.hawk.game.BILog.BIPlayerLevelData;
import com.hawk.game.BILog.BIRoundFlowData;
import com.hawk.game.BILog.BITowerCoinData;

public class BILogger {
	/**
	 * BI日志，数据流变化日志记录器
	 */
	private static final Logger BI_LOGGER = Logger.getLogger("BI");
	
	/**
	 * 
	 * @param type
	 * @return
	 */
	public static <T> T getBIData(Class<T> type){
		try {
			return type.newInstance();
		} catch (Exception e) {
			return null;
		}
	}
	
	public static void log(String biData){
		BI_LOGGER.info(biData);
	}
}
