package com.hawk.game.BILog;

import net.sf.json.JSONObject;

import org.hawk.os.HawkTime;

import com.hawk.game.GsConfig;
import com.hawk.game.log.BILogger;
import com.hawk.game.player.Player;
import com.hawk.game.util.GsConst;

public class BIData {
	/**
	 * 最终数据
	 */
	private JSONObject jsonData = new JSONObject();
	
	/**
	 * 属性数据
	 */
	protected JSONObject jsonPropertyData = new JSONObject();
	
	/**
	 * 构造函数
	 */
	public BIData() {

	}

	/**
	 * 组装玩家数据
	 */
	protected void logBegin(Player player, String event)
	{
		if (player != null) {
			// 头数据
			jsonData.put("data_version", GsConst.BIVersion);
			jsonData.put("app_id", GsConfig.getInstance().getGameId());
			jsonData.put("user_id", player.getPuid());
			jsonData.put("session_id", "");
			jsonData.put("ts", HawkTime.getMillisecond());
			jsonData.put("event", event);
			
			// property内相关数据
			jsonPropertyData.put("os", player.getOsName());
			jsonPropertyData.put("os_version", player.getOsVersion());
			jsonPropertyData.put("ip", player.getIp());
			jsonPropertyData.put("lang", player.getLanguage());
			jsonPropertyData.put("device", player.getDevice());
			jsonPropertyData.put("level", player.getLevel());
		}
	}
	
	/**
	 * 组装属性数据
	 */
	protected void logEnd()
	{
		// 组装json
		jsonData.put("properties", jsonPropertyData);
		// 输出
		BILogger.log(jsonData.toString());
	}
}
