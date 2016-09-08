package org.hawk.dbmerge;

import java.io.File;
import java.io.FileWriter;
import java.io.PrintWriter;
import java.util.LinkedList;
import java.util.List;

import net.sf.json.JSONArray;
import net.sf.json.JSONObject;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;
import org.hawk.thread.HawkThreadPool;

/**
 * 合服 注意合服之后的服务器启动的时候会根据新数据重新创建快照数据
 * 
 * @author xulinqs
 * 
 */
public class MergeMain {
	/**
	 * 程序执行目录
	 */
	public static String APP_DIR = "";
	/**
	 * sql单条语句大小
	 */
	public static int sqlPacketSize = 262144;
	/**
	 * 线程池
	 */
	public static HawkThreadPool threadPool;

	public static void main(String[] args) {
		APP_DIR = System.getProperty("user.dir") + File.separator;
		HawkOSOperator.installLibPath();
		MergeMain merge = new MergeMain();
		if (!merge.init()) {
			HawkLog.logPrintln("merge init failed.");
			return;
		}
		merge.doMerge();
	}

	/**
	 * 写文件
	 * 
	 * @param content
	 * @param filePath
	 */
	public static void writeFile(String content, String filePath) {
		try (FileWriter fileWriter = new FileWriter(new File(filePath), true)) {
			// 如果文件存在，则追加内容；如果文件不存在，则创建文件
			try (PrintWriter printWriter = new PrintWriter(fileWriter)) {
				printWriter.println(content);
				printWriter.flush();
				fileWriter.flush();
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}

	/**
	 * 字段映射表
	 */
	private MapperCfg mapperCfg;
	/**
	 * 合服的配置文件
	 */
	private JSONObject mergeCfgJson;
	/**
	 * 字段映射的配置文件
	 */
	private JSONObject fieldMapJson;
	/**
	 * 目标服务器
	 */
	private ServerInfo targetServerInfo = null;
	/**
	 * 分区服务器列表
	 */
	private List<ServerInfo> needMergeServers = new LinkedList<ServerInfo>();

	/**
	 * 初始化
	 */
	private boolean init() {
		try {
			String mergeCfgContent = HawkOSOperator.readTextFile(APP_DIR + "cfg/merge.json");
			sqlPacketSize = Integer.valueOf(mergeCfgJson.getString("sqlPacketSize"));
			mergeCfgJson = JSONObject.fromObject(mergeCfgContent);
		} catch (Exception e) {
			HawkException.catchException(e);
			HawkLog.logPrintln("read cfg/merge.json failed");
		}

		try {
			String mapCfgContent = HawkOSOperator.readTextFile(APP_DIR + "cfg/map.json");
			fieldMapJson = JSONObject.fromObject(mapCfgContent);
			mapperCfg = new MapperCfg(fieldMapJson);
		} catch (Exception e) {
			HawkException.catchException(e);
			HawkLog.logPrintln("read cfg/map.json failed");
		}

		// 初始化各个服务器的链接
		return initServersConnection();
	}

	/**
	 * 初始化MYSQL的链接
	 */
	private boolean initServersConnection() {
		JSONArray servers = mergeCfgJson.getJSONArray("servers");
		for (int i = 0; i < servers.size(); i++) {
			JSONObject jsonObject = servers.getJSONObject(i);
			ServerInfo serverInfo = new ServerInfo(jsonObject.getInt("serverId"), jsonObject.getString("dbUrl"), jsonObject.getString("dbUserName"), jsonObject.getString("dbPassword"));

			if (!serverInfo.connect()) {
				HawkLog.logPrintln("mysql connect failed: " + serverInfo.toString());
				return false;
			}
			needMergeServers.add(serverInfo);
			HawkLog.logPrintln("mysql connect success: " + serverInfo.toString());
		}

		JSONObject targetJson = mergeCfgJson.getJSONObject("target");
		targetServerInfo = new ServerInfo(targetJson.getInt("serverId"), targetJson.getString("dbUrl"), targetJson.getString("dbUserName"), targetJson.getString("dbPassword"));

		if (!targetServerInfo.connect()) {
			HawkLog.logPrintln("mysql connect failed: " + targetServerInfo.toString());
			return false;
		}
		HawkLog.logPrintln("mysql connect success: " + targetServerInfo.toString());
		return true;
	}

	/**
	 * 开始合服
	 */
	public void doMerge() {
		JSONArray srcJsonArr = mergeCfgJson.getJSONArray("srcPrepareSql");
		if (srcJsonArr != null) {
			for (ServerInfo serverInfo : needMergeServers) {
				HawkLog.logPrintln("run source prepare sql: " + serverInfo.getServerId());
				for (int i = 0; i < srcJsonArr.size(); i++) {
					serverInfo.executeSql(srcJsonArr.getString(i));
					HawkLog.logPrintln("execute source prepare sql: " + srcJsonArr.getString(i));
				}
				HawkLog.logPrintln("execute source prepare sql finish: " + serverInfo.getServerId());
			}
		}

		JSONArray jsonArr = mergeCfgJson.getJSONArray("dstPrepareSql");
		if (jsonArr != null) {
			for (int i = 0; i < jsonArr.size(); i++) {
				targetServerInfo.executeSql(jsonArr.getString(i));
				HawkLog.logPrintln("execute dest prepare sql: " + jsonArr.getString(i));
			}
			HawkLog.logPrintln("execute dest prepare sql finish.");
		}

		for (ServerInfo server : needMergeServers) {
			new MergeChannel(server, targetServerInfo, mapperCfg).doMerge();
		}
	}
}
