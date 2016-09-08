package com.hawk.account;

import java.io.File;
import java.io.FilenameFilter;
import java.util.HashSet;
import java.util.Iterator;
import java.util.LinkedHashMap;
import java.util.LinkedHashSet;
import java.util.Map;
import java.util.Set;

import org.apache.http.HttpResponse;
import org.apache.http.HttpStatus;
import org.apache.http.client.HttpClient;
import org.apache.http.client.config.RequestConfig;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.utils.URIBuilder;
import org.apache.http.impl.client.HttpClients;
import org.hawk.config.HawkConfigManager;
import org.hawk.config.HawkXmlCfg;
import org.hawk.log.HawkLog;
import org.hawk.nativeapi.HawkNativeApi;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;
import org.hawk.os.HawkTime;
import org.hawk.util.HawkTickable;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.google.gson.JsonArray;
import com.google.gson.JsonObject;
import com.hawk.account.config.ServerIndexCfg;
import com.hawk.account.config.ServerInfoCfg;
import com.hawk.account.db.DBManager;
import com.hawk.account.gameserver.GameServer;
import com.hawk.account.http.AccountHttpServer;
import com.hawk.account.zmq.AccountZmqServer;

public class AccountServices {
	/**
	 * 默认心跳时间周期
	 */
	public final static int HEART_PERIOD = 300000;

	public final static String DEFAULT_LANGUAGE = "zh-CN";

	/**
	 * 调试日志对象
	 */
	static Logger accountLogger = LoggerFactory.getLogger("Account");
	/**
	 * 运行状态
	 */
	volatile boolean running = true;
	/**
	 * 可更新列表
	 */
	Set<HawkTickable> tickableSet;

	/**
	 * 挂载服务器列表
	 */
	Map<Integer, GameServer> serverList;

	/**
	 * 区间服务器列表
	 */
	Map<Integer, Set<Integer>> serverAreaList;

	/**
	 * 服务器json字符串
	 */
	JsonObject serverInfo;

	/**
	 * 服务器ip
	 */
	private String hostIpAddr = null;

	/**
	 * 单实例对象
	 */
	private static AccountServices instance = null;
	private HttpClient httpClient = null;
	private HttpGet httpGet = null;
	private URIBuilder uriBuilder = null;

	private static final String serverPath = "/report_accountServer";
	private static final String fetchIpPath = "/fetch_myip";

	private static final String serverQuery = "game=%s&platform=%s&channel=%s&server=%s&ip=%s&zmq_port=%d&http_port=%d&dburl=%s&dbuser=%s&dbpwd=%s";

	/**
	 * 获取实例对象
	 * 
	 * @return
	 */
	public static AccountServices getInstance() {
		if (instance == null) {
			instance = new AccountServices();
		}
		return instance;
	}

	/**
	 * 构造函数
	 */
	public AccountServices() {
		// 初始化变量
		running = true;
		tickableSet = new HashSet<HawkTickable>();
		serverList = new LinkedHashMap<Integer, GameServer>();
		serverAreaList = new LinkedHashMap<>();
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
	 * 添加游戏服务器对象
	 * 
	 * @param gameServer
	 */
	private void addGameServer(GameServer gameServer) {
		serverList.put(gameServer.getIndex(), gameServer);

		// 添加服务器区间映射
		Set<Integer> areaServers = serverAreaList.get(gameServer.getArea());
		if (areaServers == null) {
			areaServers = new LinkedHashSet<Integer>();
			serverAreaList.put(gameServer.getArea(), areaServers);
		}

		areaServers.add(gameServer.getIndex());
	}

	/**
	 * 挂载游戏服务器
	 * 
	 * @param serverIndex
	 * @param hostIp
	 * @param port
	 */
	public void registGameServer(int serverIndex, String hostIp, int port) {
		GameServer gameServer = getGameServer(serverIndex);
		ServerInfoCfg serverInfo = HawkConfigManager.getInstance().getConfigByKey(ServerInfoCfg.class, serverIndex);
		if (gameServer != null && serverInfo != null) {
			gameServer.setHostIp(hostIp);
			gameServer.setPort(port);
			gameServer.setState(serverInfo.getState());
			gameServer.setHeartBeatTime(HawkTime.getMillisecond());
			updateServerInfo();
		}
	}

	/**
	 * 卸载游戏服务器
	 * 
	 * @param serverIndex
	 * @param hostIp
	 * @param port
	 */
	public void unregistGameServer(int serverIndex) {
		GameServer gameServer = getGameServer(serverIndex);
		if (gameServer != null ) {
			gameServer.setHostIp("");
			gameServer.setPort(0);
			gameServer.setState(GameServer.SERVER_STATE_MAINTAIN);
			updateServerInfo();
		}
	}

	/**
	 * 获取区域服务器列表
	 * 
	 * @param gameServer
	 */
	public Map<Integer, Set<Integer>> getAreaServerList() {
		return serverAreaList;
	}

	/**
	 * 获取游戏服务器对象
	 * 
	 * @param gameServer
	 */
	public GameServer getGameServer(int index) {
		return serverList.get(index);
	}

	/**
	 * 使用配置文件初始化
	 * 
	 * @param cfgFile
	 * @return
	 */
	public boolean init(String cfgFile) {
		boolean initOK = true;
		try {
			// 添加库加载目录
			HawkOSOperator.addUsrPath(System.getProperty("user.dir") + "/lib");
			HawkOSOperator.addUsrPath(System.getProperty("user.dir") + "/hawk");
			new File(".").list(new FilenameFilter() {
				@Override
				public boolean accept(File dir, String name) {
					if (dir.isDirectory() && name.endsWith("lib")) {
						HawkOSOperator.addUsrPath(System.getProperty("user.dir") + "/" + name);
						return true;
					}
					return false;
				}
			});

			try {
				// 初始化
				System.loadLibrary("hawk");
				if (!HawkNativeApi.initHawk()) {
					return false;
				}
			} catch (Exception e) {
				HawkException.catchException(e);
			}

			// 加载配置文件
			HawkXmlCfg conf = new HawkXmlCfg(System.getProperty("user.dir") + "/cfg/config.xml");

			// 初始化配置
			if (conf.getString("app.configPath") != null && conf.getString("app.configPath").length() > 0) {
				String workPath = System.getProperty("user.dir") + File.separator;
				if (!HawkConfigManager.getInstance().init(conf.getString("app.configPath"), workPath)) {
					System.err.println("config error");
					return false;
				}
			}

			// 初始化http服务器
			initOK &= AccountHttpServer.getInstance().setup(conf.getString("httpServer.addr"), conf.getInt("httpServer.port"), conf.getInt("httpServer.pool"));
			if (initOK) {
				HawkLog.logPrintln("Setup HttpServer Success, " + conf.getString("httpServer.addr") + ":" + conf.getInt("httpServer.port"));
			} else {
				HawkLog.logPrintln("Setup HttpServer Failed, " + conf.getString("httpServer.addr") + ":" + conf.getInt("httpServer.port"));
				return false;
			}

			// 线程池大小
			int threadPool = 4;
			if (conf.containsKey("database.threads")) {
				threadPool = conf.getInt("database.threads");
			}

			// 初始化数据库
			initOK &= DBManager.getInstance().init(conf.getString("db.dbConnUrl"), conf.getString("db.dbUserName"), conf.getString("db.dbPassWord"), threadPool);
			if (initOK) {
				HawkLog.logPrintln("Init DBManager Success, dbConnUrl: " + conf.getString("db.dbConnUrl"));
			} else {
				HawkLog.logPrintln("Init DBManager Failed, dbConnUrl: " + conf.getString("db.dbConnUrl"));
				return false;
			}

			// 创建db会话
			initOK &= createDbSessions();
			if (initOK) {
				HawkLog.logPrintln("Create Database Session Success");
			} else {
				HawkLog.logPrintln("Create Database Session Failed");
				return false;
			}

			// 启动zmq服务器
			initOK &= AccountZmqServer.getInstance().setup(conf.getString("zmqServer.addr"), conf.getInt("zmqServer.pool"));
			if (initOK) {
				HawkLog.logPrintln("Setup Zmqserver Success: " + conf.getString("zmqServer.addr"));
			} else {
				HawkLog.logPrintln("Setup Zmqserver Failed: " + conf.getString("zmqServer.addr"));
				return false;
			}

			// 注册账号服务器
			//initOK &= registAccountServer(conf);
			//if (initOK) {
			//	HawkLog.logPrintln("Regist AccountServer Success");
			//} else {
			//  HawkLog.logPrintln("Regist AccountServer Failed");
			//	return false;
			//}

			// 加载全部服务器
			for (ServerInfoCfg serverInfo : HawkConfigManager.getInstance().getConfigMap(ServerInfoCfg.class).values()) {
				GameServer gameServer = new GameServer();
				gameServer.setName(serverInfo.getName(DEFAULT_LANGUAGE));
				gameServer.setArea(serverInfo.getArea());
				gameServer.setIndex(serverInfo.getIndex());
				gameServer.setState(GameServer.SERVER_STATE_MAINTAIN);
				addGameServer(gameServer);
			}

			updateServerInfo();

		} catch (Exception e) {
			HawkException.catchException(e);
			initOK = false;
		}

		return initOK;
	}

	/**
	 * 生成服务器json数据
	 * 
	 */
	private void updateServerInfo() {
		JsonObject serverListInfo = new JsonObject();
		for (int area : AccountServices.getInstance().getAreaServerList().keySet()) {
			Set<Integer> serverList = AccountServices.getInstance().getAreaServerList().get(area);
			JsonArray jsonArray = new JsonArray();
			for (Integer serverIndex : serverList) {
				GameServer gameServer = AccountServices.getInstance().getGameServer(serverIndex);
				JsonObject jsonObject = new JsonObject();
				jsonObject.addProperty("serverIndex", serverIndex);
				jsonObject.addProperty("serverName", gameServer.getName());
				jsonObject.addProperty("hostIp", gameServer.getHostIp());
				jsonObject.addProperty("port", gameServer.getPort());
				jsonObject.addProperty("state", gameServer.getState());
				jsonArray.add(jsonObject);
			}

			JsonObject areaInfo = new JsonObject();
			areaInfo.addProperty("name", HawkConfigManager.getInstance().getConfigByKey(ServerIndexCfg.class, area).getName(AccountServices.DEFAULT_LANGUAGE));
			areaInfo.add("serverList", jsonArray);
			serverListInfo.add(String.valueOf(area), areaInfo);
		}

		serverInfo = serverListInfo;
	}

	public JsonObject getServerInfo() {
		return serverInfo;
	}

	private boolean createDbSessions() {
		// 创建默认会话
		if (DBManager.getInstance().createDbSession("account") == null) {
			HawkLog.logPrintln("Create DbSession Failed, Database: account");
			return false;
		}
		return true;
	}

	/**
	 * 获取hostIp地址
	 * 
	 * @return
	 */
	public String getHostIpAddr() {
		return hostIpAddr;
	}

	/**
	 * 注册账号服务器
	 * 
	 * @return
	 */
	protected boolean registAccountServer(HawkXmlCfg conf) {
		String queryParam = "";
		try {
			if (false == initHttpClient(conf)) {
				return false;
			}

			queryParam = String.format(serverQuery, conf.getString("app.game"), conf.getString("app.platform"), conf.getString("app.channel"),
					   conf.getString("app.serverId"), hostIpAddr, AccountZmqServer.getInstance().getPort(),
					   conf.getInt("httpServer.port"), conf.getString("db.dbConnUrl"), 
					   conf.getString("db.dbUserName"), conf.getString("db.dbPassWord"));

			uriBuilder.setPath(serverPath);
			uriBuilder.setCustomQuery(queryParam);
			httpGet.setURI(uriBuilder.build());
			HttpResponse httpResponse = httpClient.execute(httpGet);
			int status =  httpResponse.getStatusLine().getStatusCode();
			if (status != HttpStatus.SC_OK) {
				accountLogger.info("register account server info failed status : " + status );
				return false;
			}
			accountLogger.info("register account server info success: " + serverPath + "?" + queryParam);
		} catch (Exception e) {
			System.out.println(e);
			accountLogger.info("register account server info failed: " + serverPath + "?" + queryParam);
			return false;
		}

		return true;
	}

	/**
	 * 初始化httpclient
	 */
	protected boolean initHttpClient(HawkXmlCfg conf) {
		if (httpClient == null) {
			RequestConfig requestConfig = RequestConfig.custom()
					.setConnectTimeout(conf.getInt("collectorServer.httpTimeout"))
					.setSocketTimeout(conf.getInt("collectorServer.httpTimeout"))
					.build();
			httpClient = HttpClients.custom()
					.setDefaultRequestConfig(requestConfig)
					.build();
		}

		if (httpGet == null) {
			httpGet = new HttpGet();
		}

		try {
			if (uriBuilder == null) {
				uriBuilder = new URIBuilder(conf.getString("collectorServer.httpServer"));
			}
		} catch (Exception e) {
			HawkException.catchException(e);
			return false;
		}

		return true;
	}

	/**
	 * 运行服务器
	 */
	public void run() {
		HawkLog.logPrintln("MainLoop Running OK.");
		while (running) {
			try {
				onTick();

				// 心跳检测
				long curTime = HawkTime.getMillisecond();
				Iterator<Map.Entry<Integer, GameServer>> it = serverList.entrySet().iterator();  
				while(it.hasNext()){
					Map.Entry<Integer, GameServer> entry = it.next(); 
					if (entry.getValue().getState() != GameServer.SERVER_STATE_MAINTAIN && entry.getValue().heartBeatTime + HEART_PERIOD < curTime) {
						entry.getValue().setState(GameServer.SERVER_STATE_MAINTAIN);
					}
				}

				HawkOSOperator.sleep();
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}

		AccountHttpServer.getInstance().stop();
		AccountZmqServer.getInstance().stop();
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
	 * 停止服务器
	 */
	public void stop() {
		running = false;
	}
}
