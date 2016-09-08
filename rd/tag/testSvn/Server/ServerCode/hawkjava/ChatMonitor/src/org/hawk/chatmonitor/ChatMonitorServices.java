package org.hawk.chatmonitor;

import java.util.HashSet;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.concurrent.ConcurrentHashMap;

import net.sf.json.JSONArray;
import net.sf.json.JSONObject;

import org.apache.commons.configuration.XMLConfiguration;
import org.hawk.chatmonitor.http.ChatMonitorHttpServer;
import org.hawk.chatmonitor.zmq.ChatMonitorZmqServer;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;
import org.hawk.os.HawkTime;
import org.hawk.util.HawkTickable;

/**
 * 收集服务
 * 
 * @author hawk
 */
public class ChatMonitorServices {
	/**
	 * 服务器是否允许中
	 */
	volatile boolean running;
	/**
	 * 可更新列表
	 */
	Set<HawkTickable> tickableSet;
	/**
	 * 单服聊天缓存数量
	 */
	private int chatMinCache = 100;
	private int chatMaxCache = 200;
	/**
	 * 每个服的聊天信息
	 */
	Map<String, List<JSONObject>> serverChatData;
	/**
	 * 聊天信息id映射表
	 */
	Map<String, Integer> serverChatIdMap;
	
	/**
	 * 单例对象
	 */
	static ChatMonitorServices instance;

	/**
	 * 获取实例
	 */
	public static ChatMonitorServices getInstance() {
		if (instance == null) {
			instance = new ChatMonitorServices();
		}
		return instance;
	}

	/**
	 * 默认构造函数
	 */
	private ChatMonitorServices() {
		// 初始化变量
		running = true;
		tickableSet = new HashSet<HawkTickable>();
	}

	/**
	 * 是否运行中
	 * 
	 * @return
	 */
	public boolean isRunning() {
		return running;
	}

	/**
	 * 退出服务主循环
	 */
	public void breakLoop() {
		running = false;
	}

	/**
	 * 定时更新
	 */
	private void onTick() {
		for (HawkTickable tick : tickableSet) {
			try {
				tick.onTick();
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}
	}

	/**
	 * 添加可更新对象列表
	 * 
	 * @param tickable
	 */
	public void addTickable(HawkTickable tickable) {
		tickableSet.add(tickable);
	}

	/**
	 * 移除可更新对象
	 * 
	 * @param tickable
	 */
	public void removeTickable(HawkTickable tickable) {
		tickableSet.remove(tickable);
	}

	/**
	 * 使用配置文件初始化
	 * 
	 * @param cfgFile
	 * @return
	 */
	public boolean init(String cfgFile) {
		HawkOSOperator.installLibPath();
		
		// 创建服务器聊天信息列表
		serverChatData = new ConcurrentHashMap<String, List<JSONObject>>();
		serverChatIdMap = new ConcurrentHashMap<String, Integer>();
		
		boolean initOK = true;
		try {
			// 加载配置文件
			XMLConfiguration conf = new XMLConfiguration(cfgFile);

			if (conf.containsKey("basic.minCache")) {
				chatMinCache = conf.getInt("basic.minCache");
			}
			
			if (conf.containsKey("basic.maxCache")) {
				chatMaxCache = conf.getInt("basic.maxCache");
			}
			
			// 初始化http服务器
			initOK &= ChatMonitorHttpServer.getInstance().setup(conf.getString("httpserver.addr"), conf.getInt("httpserver.port"), conf.getInt("httpserver.pool"));
			if (initOK) {
				HawkLog.logPrintln("Setup HttpServer Success, " + conf.getString("httpserver.addr") + ":" + conf.getInt("httpserver.port"));
			} else {
				HawkLog.logPrintln("Setup HttpServer Failed, " + conf.getString("httpserver.addr") + ":" + conf.getInt("httpserver.port"));
			}
			
			// 初始化zmq服务器
			if (conf.containsKey("zmqserver.addr")) {
				initOK &= ChatMonitorZmqServer.getInstance().setup(conf.getString("zmqserver.addr"));
				if (initOK) {
					HawkLog.logPrintln("Setup ZmqServer Success: " + conf.getString("zmqserver.addr"));
				} else {
					HawkLog.logPrintln("Setup ZmqServer Failed: " + conf.getString("zmqserver.addr"));
				}
			}
		} catch (Exception e) {
			HawkException.catchException(e);
			initOK = false;
		}
		
		return initOK;
	}

	/**
	 * 运行服务器
	 */
	public void run() {
		HawkLog.logPrintln("MainLoop Running OK.");
		while (running) {
			try {
				onTick();

				HawkOSOperator.sleep();
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}
	}
	
	/**
	 * 添加服务器聊天信息
	 * 
	 * @param identify
	 * @param data
	 */
	public void addServerChatData(String identify, JSONObject jsonObject) {
		try {
			List<JSONObject> chatList = serverChatData.get(identify);
			if (chatList == null) {
				serverChatData.put(identify, new LinkedList<JSONObject>());
				chatList = serverChatData.get(identify);
				
				// 设置最小id
				serverChatIdMap.put(identify, new Integer(0));
			}
			
			// 超标之后删除到低水位线上
			if (chatList.size() > chatMaxCache) {
				while (chatList.size() > chatMinCache) {
					chatList.remove(0);
				}
			}
			
			// 增加秒数熟悉
			int minChatId = serverChatIdMap.get(identify);
			serverChatIdMap.put(identify, ++minChatId);
			jsonObject.put("chatId", minChatId);
			jsonObject.put("ts", HawkTime.getTimeString());
			
			// 添加到聊天列表
			chatList.add(jsonObject);
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}
	
	/**
	 * 获取服务器聊天信息
	 * 
	 * @param identify
	 * @param minChatId
	 * @param count
	 */
	public JSONArray getServerChatData(String identify, int minChatId, int count) {
		JSONArray jsonArray = new JSONArray();
		List<JSONObject> chatList = serverChatData.get(identify);
		if (chatList != null) {
			for (int i=chatList.size()-1; i>=0; i--) {
				JSONObject chatItem = chatList.get(i);
				if (chatItem.containsKey("chatId") && chatItem.getInt("chatId") > minChatId) {
					jsonArray.add(chatItem);
					if (jsonArray.size() >= count) {
						break;
					}
				}
			}
		}
		return jsonArray;
	}
}
