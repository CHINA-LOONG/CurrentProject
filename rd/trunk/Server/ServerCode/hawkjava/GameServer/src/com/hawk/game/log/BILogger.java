package com.hawk.game.log;

import org.apache.log4j.Logger;

import com.hawk.game.BILog.BIData;

public class BILogger {
	/**
	 * BI日志，数据流变化日志记录器
	 */
	private static final Logger BI_LOGGER = Logger.getLogger("BI");

	public static void log(String biData){
		BI_LOGGER.info(biData);
	}
}
