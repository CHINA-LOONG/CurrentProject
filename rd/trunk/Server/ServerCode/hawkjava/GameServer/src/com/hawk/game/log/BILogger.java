package com.hawk.game.log;

import org.apache.log4j.Logger;
import org.hawk.app.HawkAppCfg;

import com.hawk.game.GsConfig;
import com.hawk.game.BILog.BIData;

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
		if (GsConfig.getInstance().isLogBI()) {
			BI_LOGGER.info(biData);
		}
	}
	
	public static void log(BIData bidata){
		if (GsConfig.getInstance().isLogBI()) {
			BI_LOGGER.info(bidata.toString());
		}	
	}
}
