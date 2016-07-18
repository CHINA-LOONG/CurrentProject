package com.hawk.account;

import java.io.File;
import java.io.FilenameFilter;
import java.net.InetAddress;
import java.net.URLEncoder;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;

import net.sf.json.JSONObject;

import org.apache.commons.httpclient.HttpClient;
import org.apache.commons.httpclient.HttpStatus;
import org.apache.commons.httpclient.methods.GetMethod;
import org.hawk.config.HawkXmlCfg;
import org.hawk.log.HawkLog;
import org.hawk.nativeapi.HawkNativeApi;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;
import org.hawk.util.HawkTickable;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.hawk.account.db.DBManager;
import com.hawk.account.http.AccountHttpServer;
import com.hawk.account.zmq.AccountZmqServer;

public class AccountServices {	
	
	public static class GameServer{
		public String server;
		public String hostIp;
		public int port;
		
		public GameServer(String server, String hostIp, int port) {
			this.server = server;
			this.hostIp = hostIp;
			this.port = port;
		}
	}
	
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
	 * 挂在服务器列表
	 */
	Map<String, GameServer> serverList;
	
	/**
	 * 服务器ip
	 */
	private String hostIpAddr = null;
	
	/**
	 * 单实例对象
	 */
	private static AccountServices instance = null;
	private HttpClient httpClient = null;
	private GetMethod getMethod = null;

	private static final String serverPath = "/report_accountServer";
	private static final String fetchIpPath = "/fetch_myip";
	
	private static final String serverQuery = "game=%s&platform=%s&chanel=%s&server=%s&ip=%s&zmq_port=%d&http_port=%d&dburl=%s&dbuser=%s&dbpwd=%s";
	
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
		serverList = new HashMap<String, GameServer>();
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
	public void addGameServer(GameServer gameServer) {
		serverList.put(gameServer.server, gameServer);
	}
	
	/**
	 * 移除游戏服务器对象
	 * 
	 * @param gameServer
	 */
	public void removeGameServer(String game, String platform, String server) {
		for (Map.Entry<String, GameServer> entry : serverList.entrySet()) {
			if (entry.getValue().server.equals(server)) {
				serverList.remove(server);
				break;
			}
		}
	}
	
	/**
	 * 获取游戏服务器对象
	 * 
	 * @param gameServer
	 */
	public Map<String, GameServer> getGameServer(String platform, String channel) {
		return serverList;
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
			
			// 获取账号服务器地址信息
			initOK = fetchReportInfo(conf);
			if (initOK) {
				HawkLog.logPrintln("fetch my hostIp success, " + hostIpAddr);
			} else {
				HawkLog.logPrintln("fetch my hostIp fail");
				return false;
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
			initOK &= registAccountServer(conf);
			if (initOK) {
				HawkLog.logPrintln("Regist AccountServer Success");
			} else {
				HawkLog.logPrintln("Regist AccountServer Failed");
				return false;
			}
			
		} catch (Exception e) {
			HawkException.catchException(e);
			initOK = false;
		}
		
		return initOK;
	}

	private boolean createDbSessions() {
		// 创建默认会话
		if (DBManager.getInstance().createDbSession("account") == null) {
			HawkLog.logPrintln("Create DbSession Failed, Database: oods");
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
			initHttpClient(conf);
			
			queryParam = String.format(serverQuery, conf.getString("app.game"), conf.getString("app.platform"), conf.getString("app.channel"),
					   conf.getString("app.serverId"), hostIpAddr, AccountZmqServer.getInstance().getPort(),
					   conf.getInt("httpServer.port"), conf.getString("db.dbConnUrl"), 
					   conf.getString("db.dbUserName"), conf.getString("db.dbPassWord"));
			queryParam = URLEncoder.encode(queryParam, "UTF-8");
			getMethod.setPath(serverPath);
			getMethod.setQueryString(queryParam);
			httpClient.executeMethod(getMethod);
			accountLogger.info("register account server info success: " + serverPath + "?" + queryParam);
		} catch (Exception e) {
			System.out.println(e);
			accountLogger.info("register account server info failed: " + serverPath + "?" + queryParam);
			return false;
		}
	
		return true;
	}

	/**
	 * 获取上报服务器信息
	 * 
	 * @return
	 */
	protected boolean fetchReportInfo(HawkXmlCfg conf) {			
		String reportInfo = null;
		hostIpAddr = conf.getString("app.addr");
		
		try {
			initHttpClient(conf);
			
			getMethod.setPath(fetchIpPath);
			int status = httpClient.executeMethod(getMethod);
			if (status == HttpStatus.SC_OK) {
				reportInfo = new String(getMethod.getResponseBody());
			}
		} catch (Exception e) {
			return false;
		}
		
		try {
			if (reportInfo != null && reportInfo.length() > 0) {
				accountLogger.info("fetch account service info: " + reportInfo);
				JSONObject jsonObject = JSONObject.fromObject(reportInfo);
				if (jsonObject.containsKey("myIp")) {
					hostIpAddr = (String) jsonObject.get("myIp");
					//TODO
					if (hostIpAddr.equals("127.0.0.1") || hostIpAddr.equalsIgnoreCase("123.126.3.94")) {
						hostIpAddr = InetAddress.getLocalHost().getHostAddress();
					}
				}
			}
			else
			{
				accountLogger.info("regist account service fail: ");
				return false;
			}			
		} catch (Exception e) {
			HawkException.catchException(e);
			return false;
		}
		
		return true;
	}
	
	/**
	 * 初始化httpclient
	 */
	protected void  initHttpClient(HawkXmlCfg conf) {
		if (httpClient == null) {
			httpClient = new HttpClient();
			httpClient.getHttpConnectionManager().getParams().setConnectionTimeout(conf.getInt("collectorServer.httpTimeout"));
			httpClient.getHttpConnectionManager().getParams().setSoTimeout(conf.getInt("collectorServer.httpTimeout"));
		}

		if (getMethod == null) {
			getMethod = new GetMethod(conf.getString("collectorServer.httpServer"));
		}
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
