package com.hawk.opsagent;

import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;
import java.util.UUID;

import net.sf.json.JSONArray;
import net.sf.json.JSONObject;

import org.apache.commons.configuration.XMLConfiguration;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;
import org.hawk.os.HawkTime;
import org.hawk.util.services.HawkOpsService;
import org.hawk.util.services.helper.HawkOpsServerInfo;
import org.hawk.zmq.HawkZmq;
import org.hawk.zmq.HawkZmqManager;

import com.google.gson.JsonArray;
import com.google.gson.JsonObject;
import com.hawk.opsagent.handler.AgentCommandHandler;
import com.hawk.opsagent.handler.ManualDebugHandler;

/**
 * 
 * @author hawk
 */
public class OpsAgentServices {
	/**
	 * 缓冲区
	 */
	byte[] buffer = null;
	/**
	 * 服务器是否运行中
	 */
	volatile boolean running;
	/**
	 * 上次的同步时间
	 */
	private long lastSyncTime;
	/**
	 * 本机ip地址
	 */
	private String serverIp;
	/**
	 * 代理端标识
	 */
	private String agentIdentify;
	/**
	 * zmq服务通讯
	 */
	private HawkZmq opsZmq = null;
	/**
	 * zmq服务通讯
	 */
	private HawkZmq agentZmq = null;
	/**
	 * 调试模式
	 */
	private boolean debugMode = false;
	/**
	 * 本地服务器信息
	 */
	Map<String, HawkOpsServerInfo> opsServerMap;
	
	/**
	 * 单例对象
	 */
	static OpsAgentServices instance;

	/**
	 * 获取实例
	 */
	public static OpsAgentServices getInstance() {
		if (instance == null) {
			instance = new OpsAgentServices();
		}
		return instance;
	}

	/**
	 * 默认构造函数
	 */
	private OpsAgentServices() {
		running = true;
		buffer = new byte[16 * 1024 * 1024];
		opsServerMap = new HashMap<String, HawkOpsServerInfo>();
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
	 * 获取代理端标识
	 * 
	 * @return
	 */
	public String getAgentIdentify() {
		return agentIdentify;
	}
	
	/**
	 * 获取自己的ip地址
	 * 
	 * @return
	 */
	public String getMyIp() {
		return serverIp;
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
			String opsAddr = conf.getString("basic.ops");
			opsZmq = HawkZmqManager.getInstance().createZmq(HawkZmq.ZmqType.DEALER);
			agentIdentify = UUID.randomUUID().toString().toLowerCase().replace("-", "");
			opsZmq.setIdentity(agentIdentify.getBytes());
			if (!opsZmq.connect(opsAddr)) {
				HawkZmqManager.getInstance().closeZmq(opsZmq);
				opsZmq = null;
				return false;
			}
			HawkLog.logPrintln("Init OpsZmq Success: " + opsAddr);
			
			// 跟本地游戏服务器通讯对象初始化
			String agentAddr = conf.getString("basic.agent");
			agentZmq = HawkZmqManager.getInstance().createZmq(HawkZmq.ZmqType.PULL);
			if (!agentZmq.bind(agentAddr)) {
				HawkZmqManager.getInstance().closeZmq(agentZmq);
				agentZmq = null;
				return false;
			}
			HawkLog.logPrintln("Init AgentZmq Success: " + agentAddr);
			
			try {
				String svrCfg = System.getProperty("user.dir") + "/servers.json";
				loadServerMap(HawkOSOperator.readTextFile(svrCfg));
			} catch (Exception e) {
				HawkException.catchException(e);
			}
			
			// 是否为调试模式
			if (conf.containsKey("basic.debug") && Integer.valueOf(conf.getString("basic.debug")) != 0) {
				debugMode = true;
				new ManualDebugHandler().start();
			}
			// 获取自己的ip
			serverIp = HawkOSOperator.getMyIp(5000);
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
	public List<HawkOpsServerInfo> getServerInfo(String game, String serverids) {
		List<HawkOpsServerInfo> serverList = new LinkedList<HawkOpsServerInfo>();
		for (Entry<String, HawkOpsServerInfo> entry : opsServerMap.entrySet()) {
			if (entry.getValue().meetConditions(game, serverids)) {
				serverList.add(entry.getValue());
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
				// 更新本地事件
				updateLocalEvent();
				
				// 更新代理端事件
				updateAgentEvent();

				// 更新本地服务器信心给终端控制中心
				syncToOpsCenter();
				
				// 空闲控制帧
				HawkOSOperator.sleep();
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}
	}

	/**
	 * 检测本地通讯事件
	 */
	private void updateLocalEvent() {
		try {
			if (agentZmq.pollEvent(HawkZmq.HZMQ_EVENT_READ, 0) > 0) {
				int size = agentZmq.recv(buffer, 0);
				HawkOpsServerInfo serverInfo = new HawkOpsServerInfo();
				if (size > 0 && serverInfo.fromJson(JSONObject.fromObject(new String(buffer, 0, size, "UTF-8")))) {
					String identify = serverInfo.getIdentify();
					if (!opsServerMap.containsKey(identify) || opsServerMap.get(identify).hasChanged(serverInfo)) {
						// 添加到服务器列表
						opsServerMap.put(identify, serverInfo);
						// 日志记录
						HawkLog.logPrintln(serverInfo.toString());
					}
				}
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}

	/**
	 * 更新控制端通讯事件
	 */
	private void updateAgentEvent() {
		try {
			if (opsZmq.pollEvent(HawkZmq.HZMQ_EVENT_READ, 0) > 0) {
				int size = opsZmq.recv(buffer, 0);
				if (size > 0) {
					String result = AgentCommandHandler.onAgentCommand(new String(buffer, 0, size, "UTF-8"));
					if (result != null) {
						HawkLog.logPrintln("response: " + result);
						opsZmq.send(result.getBytes(), 0);
					}
				}
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}
	
	private void loadServerMap(String serverInfos) {
		try {
			if (serverInfos != null && serverInfos.length() > 0) {
				JSONArray serverArray = JSONArray.fromObject(serverInfos);
				for (int i=0; i<serverArray.size(); i++) {
					HawkOpsServerInfo serverInfo = new HawkOpsServerInfo();
					try {
						serverInfo.fromJson(serverArray.getJSONObject(i));
						serverInfo.setPid("0");
						// 添加到服务器列表
						opsServerMap.put(serverInfo.getIdentify(), serverInfo);
						// 日志记录
						HawkLog.logPrintln(serverInfo.toString());
					} catch (Exception e) {
						HawkException.catchException(e);
					}
				}
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}
	
	/**
	 * 同步本地服务器信心到运维中心
	 */
	private void syncToOpsCenter() {
		if (HawkTime.getMillisecond() >= lastSyncTime + HawkOpsService.SYNC_PERIOD) {
			lastSyncTime = HawkTime.getMillisecond();
			// 构建服务器信息
			JsonArray serverArray = new JsonArray();
			for (Entry<String, HawkOpsServerInfo> entry : opsServerMap.entrySet()) {
				try {
					HawkOpsServerInfo serverInfo = entry.getValue();
					serverArray.add(serverInfo.toJson());
				} catch (Exception e) {
					HawkException.catchException(e);
				}
			}
			// 同步给运维中心
			JsonObject jsonObject = new JsonObject();
			jsonObject.addProperty("identify", getAgentIdentify());
			jsonObject.addProperty("ip", getMyIp());
			jsonObject.add("servers", serverArray);
			opsZmq.send(jsonObject.toString().getBytes(), 0);
			
			// 保存服务器信息
			String svrCfg = System.getProperty("user.dir") + "/servers.json";
			HawkOSOperator.saveAsFile(serverArray.toString(), svrCfg);
			
			// 日志打印记录信息
			HawkLog.logPrintln(jsonObject.toString());
		}
	}
}
