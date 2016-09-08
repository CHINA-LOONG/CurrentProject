package com.hawk.opsmanager;

import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;

import net.sf.json.JSONArray;
import net.sf.json.JSONObject;

import org.apache.commons.configuration.XMLConfiguration;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;
import org.hawk.os.HawkTime;
import org.hawk.util.HawkCmdParams;
import org.hawk.util.services.HawkOpsService;
import org.hawk.util.services.helper.HawkOpsServerInfo;
import org.hawk.zmq.HawkZmq;
import org.hawk.zmq.HawkZmqManager;

import com.hawk.opsmanager.entity.OpsServerInfo;
import com.hawk.opsmanager.handler.ManualDebugHandler;
import com.hawk.opsmanager.http.OpsHttpServer;

/**
 * 
 * @author hawk
 */
public class OpsServices {
	/**
	 * 缓冲区
	 */
	byte[] buffer = null;
	/**
	 * 服务器是否运行中
	 */
	volatile boolean running;
	/**
	 * 上次打印时间
	 */
	private long lastPrintTime;
	/**
	 * 调试模式
	 */
	private boolean debugMode = false;
	/**
	 * zmq服务通讯
	 */
	private HawkZmq agentZmq = null;
	/**
	 * 服务器信息
	 */
	Map<String, OpsServerInfo> opsServerMap;
	/**
	 * 终端信息
	 */
	Map<String, String> opsAgentMap;
	
	/**
	 * 单例对象
	 */
	static OpsServices instance;

	/**
	 * 获取实例
	 */
	public static OpsServices getInstance() {
		if (instance == null) {
			instance = new OpsServices();
		}
		return instance;
	}

	/**
	 * 默认构造函数
	 */
	private OpsServices() {
		running = true;
		buffer = new byte[16 * 1024 * 1024];
		opsAgentMap = new HashMap<String, String>();
		opsServerMap = new HashMap<String, OpsServerInfo>();
		lastPrintTime = HawkTime.getMillisecond();
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
	 * 是否为调试模式
	 */
	public boolean isDebugMode() {
		return debugMode;
	}
	
	/**
	 * 使用配置文件初始化
	 * 
	 * @param cfgFile
	 * @return
	 */
	public boolean init(String cfgFile) {
		// 添加库加载目录
		HawkOSOperator.installLibPath();
		try {
			// 加载配置文件
			XMLConfiguration conf = new XMLConfiguration(cfgFile);

			// 跟控制服务器通讯对象初始化
			String agentAddr = conf.getString("basic.addr");
			agentZmq = HawkZmqManager.getInstance().createZmq(HawkZmq.ZmqType.ROUTER);
			if (!agentZmq.bind(agentAddr)) {
				HawkZmqManager.getInstance().closeZmq(agentZmq);
				agentZmq = null;
				return false;
			}
			HawkLog.logPrintln("Init AgentZmq Success: " + agentAddr);
			
			// 初始化http的回调服务器
			if (conf.containsKey("httpserver.addr")) {
				if (!OpsHttpServer.getInstance().setup(conf.getString("httpserver.addr"), conf.getInt("httpserver.port"), conf.getInt("httpserver.pool"))) {
					HawkLog.logPrintln("Setup Ops HttpServer Failed: " + conf.getString("httpserver.addr") + ":" + conf.getInt("httpserver.port"));
					return false;
				}
			}
			
			// 是否为调试模式
			if (conf.containsKey("basic.debug") && Integer.valueOf(conf.getString("basic.debug")) != 0) {
				debugMode = true;
				new ManualDebugHandler().start();
			}
			
			return true;
		} catch (Exception e) {
			HawkException.catchException(e);
		}
		return false;
	}

	/**
	 * 获取满足条件的服务器信息 
	 * 
	 * @param game
	 * @param serverids
	 * @return
	 */
	public List<OpsServerInfo> getServerInfo(String game, String serverids) {
		List<OpsServerInfo> serverList = new LinkedList<OpsServerInfo>();
		synchronized (opsServerMap) {
			for (Entry<String, OpsServerInfo> entry : opsServerMap.entrySet()) {
				if (entry.getValue().getServerInfo().meetConditions(game, serverids)) {
					serverList.add(entry.getValue());
				}
			}
		}
		return serverList;
	}
	
	/**
	 * 获取服务器列表信息 
	 * 
	 * @param game
	 * @return
	 */
	public List<OpsServerInfo> getServerInfo(String game) {
		List<OpsServerInfo> serverList = new LinkedList<OpsServerInfo>();
		synchronized (opsServerMap) {
			for (Entry<String, OpsServerInfo> entry : opsServerMap.entrySet()) {
				if (game.equals(entry.getValue().getServerInfo().getGame())) {
					serverList.add(entry.getValue());
				}
			}
		}
		return serverList;
	}
	
	/**
	 * 运行服务器
	 */
	public void run() {
		HawkLog.logPrintln("MainLoop Running OK.");
		while (running) {
			try {
				// 更新代理端事件
				updateAgentEvent();

				// 打印服务器信息
				printServerInfos();
				
				// 空闲控制帧
				HawkOSOperator.sleep();
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}
	}

	/**
	 * 更新控制端通讯事件
	 */
	private void updateAgentEvent() {
		try {
			synchronized (agentZmq) {
				if (agentZmq.pollEvent(HawkZmq.HZMQ_EVENT_READ, 0) > 0) {
					int size = agentZmq.recv(buffer, 0);
					if (size <= 0) {
						return;
					}
					
					// 终端标识符
					String agentIdentify = new String(buffer, 0, size, "UTF-8");
					if (!agentZmq.isWaitRecv()) {
						return;
					}
					
					// 接收数据体
					size = agentZmq.recv(buffer, 0);
					if (size <= 0) {
						return;
					}
					
					String data = new String(buffer, 0, size, "UTF-8");
					JSONObject jsonObject = JSONObject.fromObject(data);
					if (!jsonObject.containsKey("servers")) {
						return;
					}

					// 逐个处理服务器信息， 遍历终端所有服务器
					opsAgentMap.put(agentIdentify, agentIdentify);
					JSONArray serverArray = jsonObject.getJSONArray("servers");
					for (int i=0; i<serverArray.size(); i++) {
						HawkOpsServerInfo serverInfo = new HawkOpsServerInfo();
						if (serverInfo.fromJson(serverArray.getJSONObject(i))) {
							if (!opsServerMap.containsKey(serverInfo.getIdentify())) {
								// 添加到服务器列表或者更新
								OpsServerInfo opsServerInfo = new OpsServerInfo();
								opsServerInfo.setAgentIdentify(agentIdentify);
								opsServerInfo.setServerInfo(serverInfo);
								synchronized (opsServerMap) {
									opsServerMap.put(serverInfo.getIdentify(), opsServerInfo);
								}
								// 日志记录
								HawkLog.logPrintln(serverInfo.toString());
							} else {
								// 更新信息
								opsServerMap.get(serverInfo.getIdentify()).update(agentIdentify, serverInfo);
							}
						}
					}
				}
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}
	
	/**
	 * 打印服务器信息
	 */
	private void printServerInfos() {
		if (HawkTime.getMillisecond() > lastPrintTime + HawkOpsService.SYNC_PERIOD) {
			lastPrintTime = HawkTime.getMillisecond();
			if (opsServerMap.size() > 0) {
				synchronized (opsServerMap) {
					for (Entry<String, OpsServerInfo> entry : opsServerMap.entrySet()) {
						HawkLog.logPrintln(entry.getValue().getServerInfo().toString());
					}
				}
			} else {
				HawkLog.logPrintln("ops agent server empty.");
			}
		}
	}

	/**
	 * 发起请求
	 * 
	 * @param params
	 * @param game
	 * @param platform
	 * @param serverids
	 * @return
	 */
	public JSONArray doRequest(HawkCmdParams params, String game, String serverids, long timeout) {
		JSONArray jsonArray = new JSONArray();
		List<OpsServerInfo> serverInfos = OpsServices.getInstance().getServerInfo(game, serverids);
		Map<String, String> agentResponse = new HashMap<String, String>();
		synchronized (agentZmq) {
			for (OpsServerInfo serverInfo : serverInfos) {
				if (!agentResponse.containsKey(serverInfo.getAgentIdentify())) {
					HawkLog.logPrintln(String.format("NotifyAgent: %s, Params: %s", serverInfo.getAgentIdentify(), params.toString()));
					agentZmq.send(serverInfo.getAgentIdentify().getBytes(), HawkZmq.HZMQ_SNDMORE);
					agentZmq.send(params.toString().getBytes(), 0);
					agentResponse.put(serverInfo.getAgentIdentify(), "failed");
				}
			}
			
			int responseCount = 0;
			long beginTime = HawkTime.getMillisecond();
			do {
				if (agentZmq.pollEvent(HawkZmq.HZMQ_EVENT_READ, 0) > 0) {
					try {
						int size = agentZmq.recv(buffer, 0);
						if (size <= 0) {
							break;
						}
						
						// 终端标识符
						String agentIdentify = new String(buffer, 0, size, "UTF-8");
						if (!agentZmq.isWaitRecv()) {
							break;
						}
						
						// 接收数据体
						size = agentZmq.recv(buffer, 0);
						if (size <= 0) {
							break;
						}
						
						String data = new String(buffer, 0, size, "UTF-8");
						JSONObject jsonObject = JSONObject.fromObject(data);
						if (jsonObject.containsKey("results")) {
							if (agentResponse.containsKey(agentIdentify)) {
								responseCount ++;
								String results = jsonObject.getString("results");
								HawkLog.logPrintln("agent: " + agentIdentify + ", results: " + results);
								agentResponse.put(agentIdentify, results);
							}
						}
					} catch (Exception e) {
						HawkException.catchException(e);
					}
				} else {
					HawkOSOperator.sleep();
				}
				
				// 接收到所有消息
				if (responseCount >= agentResponse.size()) {
					break;
				}
			} while (timeout > 0 && HawkTime.getMillisecond() <= beginTime + timeout);
			
			// 填充到返回结果中
			for (Entry<String, String> entry : agentResponse.entrySet()) {
				try {
					if (!entry.getValue().equals("failed")) {
						JSONArray resultArray = JSONArray.fromObject(entry.getValue());
						for (int i=0; i<resultArray.size(); i++) {
							JSONObject serverResult = resultArray.getJSONObject(i);
							jsonArray.add(serverResult);
						}
					} else {
						HawkLog.logPrintln("agent recv response failed: " + entry.getKey() + ", timeout: " + timeout);
					}
				} catch (Exception e) {
					HawkException.catchException(e);
				}
			}
		}
		return jsonArray;
	}
}
