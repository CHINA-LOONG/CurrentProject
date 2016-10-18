package org.hawk.util.services;

import java.util.LinkedList;
import java.util.List;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

import org.apache.http.HttpStatus;
import org.hawk.app.HawkApp;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkTime;
import org.hawk.util.HawkTickable;
import org.hawk.zmq.HawkZmq;
import org.hawk.zmq.HawkZmqManager;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class HawkAccountService extends HawkTickable {

	/**
	 * 默认心跳时间周期
	 */
	public final static int HEART_PERIOD = 60000;

	public static class RegitsterGameServer {
		String gameServerHost;
		int port;
		int scriptPort;
		public RegitsterGameServer (String gameServerHost, int port, int scriptPort) {
			this.gameServerHost = gameServerHost;
			this.port = port;
			this.scriptPort = scriptPort;
		}
	}

	public static class UnRegitsterGameServer {

	}

	public static class HeartBeatData {

	}

	public static class CreateRoleData {
		public String puid;
		public int playerId;
		public String nickname;

		public CreateRoleData (String puid, int playerId, String nickname) {
			this.puid = puid;
			this.playerId = playerId;
			this.nickname = nickname;
		}
	}

	public static class LevelUpData {
		public String puid;
		public int playerId;
		int level;
		public LevelUpData (String puid, int playerId, int level) {
			this.puid = puid;
			this.playerId = playerId;
			this.level = level;
		}
	}

	public static class RenameRoleData {
		public String puid;
		public int playerId;
		public String newName;
		public RenameRoleData(String puid, int playerId, String newName) {
			this.puid = puid;
			this.playerId = playerId;
			this.newName = newName;
		}
	}

	/**
	 * 调试日志对象
	 */
	static Logger accountLogger = LoggerFactory.getLogger("Account");

	/**
	 * zmq对象
	 */
	private HawkZmq accountZmq = null;

	/**
	 * 汇报数据
	 */
	private Lock reportLock = null;
	List<Object> reportDatas = null;

	/**
	 * 连接成功
	 */

	private boolean connectOK = false;

	/**
	 * 上次beat周期时间
	 */
	private long lastBeatTime = 0;

	// 账号上报路径
	private static final String roleCreatePath      = "/report_roleCreate";
	private static final String roleRenamePath      = "/report_roleRename";
	private static final String levelUpPath         = "/report_levelUp";
	private static final String serverRegistPath    = "/regist_gameserver";
	private static final String serverUnRegistPath  = "/unregist_gameserver";
	private static final String heartBeatPath       = "/heartBeat";

	private static final String roleCreateParam     = "game=%s&platform=%s&channel=%s&server=%d&puid=%s&playerid=%d&nickname=%s";
	private static final String roleRenameParam     = "game=%s&platform=%s&channel=%s&server=%d&puid=%s&playerid=%d&nickname=%s";
	private static final String levelUpParam        = "game=%s&platform=%s&channel=%s&server=%d&puid=%s&playerid=%d&level=%d";
	private static final String serverRegistParam   = "game=%s&platform=%s&channel=%s&server=%d&ip=%s&port=%d&scriptPort=%d";
	private static final String serverUnRegistParam = "game=%s&platform=%s&channel=%s&server=%d";
	private static final String heartBeatParam      = "game=%s&platform=%s&channel=%s&server=%d";

	/**
	 * 服务器信息
	 */
	private String accountZmqHost = "";
	private String gameName = "";
	private String platform = "";
	private String channel = "";
	private int serverId = 0;

	/**
	 * 实例对象
	 */
	private static HawkAccountService instance = null;

	/**
	 * 获取全局实例对象
	 * 
	 * @return
	 */
	public static HawkAccountService getInstance() {
		if (instance == null) {
			instance = new HawkAccountService();
		}
		return instance;
	}

	private HawkAccountService () {
		reportLock = new ReentrantLock();
		reportDatas = new LinkedList<Object>();

		if (HawkApp.getInstance() != null) {
			HawkApp.getInstance().addTickable(this);
		}
	}

	/**
	 * 初始化账号服务
	 * 
	 * @return
	 */
	public boolean install(String gameName, String platform, String channel, int serverId, int timeout, String zmqAddress) {
		try {
			this.gameName = gameName;
			this.platform = platform;
			this.channel = channel;
			this.serverId = serverId;
			this.accountZmqHost = zmqAddress;
			lastBeatTime = HawkTime.getMillisecond();

			// 可重复调用
			HawkZmqManager.getInstance().init(HawkZmq.HZMQ_CONTEXT_THREAD);
			if (createAccountZmq(accountZmqHost) == false) {
				accountLogger.info("create account server fail");
			}
			else{
				accountLogger.info("create account server success");
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}

		if (accountZmq == null) {
			HawkLog.errPrintln("install account service failed.");
			return false;
		}

		return true;
	}

	/**
	 * 执行http请求
	 * 
	 * @param path
	 * @param params
	 * @return
	 */
	public synchronized int executeMethod(String path, String params) {
		if (accountZmq != null) {
			try {
				if (!accountZmq.send(path.getBytes(), HawkZmq.HZMQ_SNDMORE)) {
					return -1;
				}

				if (!accountZmq.send(params.getBytes(), 0)) {
					return -1;
				}

				return 0;
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		} 

		return -1;
	}

	/**
	 * 创建zmq对象
	 * 
	 * @param addr
	 * @return
	 */
	protected boolean createAccountZmq(String addr) {
		if (accountZmq == null) {
			accountZmq = HawkZmqManager.getInstance().createZmq(HawkZmq.ZmqType.PUSH);
			accountZmq.startMonitor(HawkZmq.SocketEvent.CONNECTED 
									| HawkZmq.SocketEvent.DISCONNECTED 
									| HawkZmq.SocketEvent.CONNECT_RETRIED 
									| HawkZmq.SocketEvent.CONNECT_DELAYED
									);
			if (!accountZmq.connect(addr)) {
				accountZmq = null;
				return false;
			}
		}
		return true;
	}

	/**
	 * 注册游戏服务器
	 * 
	 * @return
	 */
	public void report(RegitsterGameServer registerData) {
		reportLock.lock();
		try {
			reportDatas.add(registerData);
		} finally {
			reportLock.unlock();
		}
	}

	/**
	 * 卸载游戏服务器
	 * 
	 * @return
	 */
	public void report(UnRegitsterGameServer unRegisterData) {
		reportLock.lock();
		try {
			reportDatas.add(unRegisterData);
		} finally {
			reportLock.unlock();
		}
	}

	/**
	 * 心跳检测
	 * 
	 * @return
	 */
	public void report(HeartBeatData heartBeatData) {
		reportLock.lock();
		try {
			reportDatas.add(heartBeatData);
		} finally {
			reportLock.unlock();
		}
	}

	/**
	 * 上报创建角色
	 * 
	 * @return
	 */
	public void report(CreateRoleData createRoleData) {
		reportLock.lock();
		try {
			reportDatas.add(createRoleData);
		} finally {
			reportLock.unlock();
		}
	}

	/**
	 * 上报角色升级
	 * 
	 * @return
	 */
	public void report(LevelUpData levelUpData) {
		reportLock.lock();
		try {
			reportDatas.add(levelUpData);
		} finally {
			reportLock.unlock();
		}
	}

	/**
	 * 上报角色修改昵称
	 */
	public void report(RenameRoleData renameRoleData) {
		reportLock.lock();
		try {
			reportDatas.add(renameRoleData);
		} finally {
			reportLock.unlock();
		}
	}

	/**
	 * 执行上报
	 */
	private boolean doReport(RegitsterGameServer gameServerData) {
		if (accountZmq != null) {
			try {
				String queryParam = String.format(serverRegistParam, gameName, platform, channel, serverId, gameServerData.gameServerHost, gameServerData.port, gameServerData.scriptPort);
				accountLogger.info("report: " + serverRegistPath + "?" + queryParam);

				int status = executeMethod(serverRegistPath, queryParam);
				if (status == HttpStatus.SC_OK) {
					return true;
				}
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}
		return false;
	}

	/**
	 * 执行上报
	 */
	private boolean doReport(UnRegitsterGameServer gameServerData) {
		if (accountZmq != null) {
			try {
				String queryParam = String.format(serverUnRegistParam, gameName, platform, channel, serverId);
				accountLogger.info("report: " + serverUnRegistPath + "?" + queryParam);

				int status = executeMethod(serverUnRegistPath, queryParam);
				if (status == HttpStatus.SC_OK) {
					return true;
				}
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}
		return false;
	}

	/**
	 * 执行上报
	 */
	private boolean doReport(HeartBeatData heartBeatData) {
		if (accountZmq != null) {
			try {
				String queryParam = String.format(heartBeatParam, gameName, platform, channel, serverId);

				accountLogger.info("report:"  + queryParam);

				int status = executeMethod(heartBeatPath, queryParam);
				if (status == HttpStatus.SC_OK) {
					return true;
				}
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}
		return false;
	}

	/**
	 * 执行上报
	 */
	private boolean doReport(CreateRoleData createRoleData) {
		if (accountZmq != null) {
			try {
				String queryParam = String.format(roleCreateParam, gameName, platform, channel, serverId, createRoleData.puid, 
						createRoleData.playerId, createRoleData.nickname);

				accountLogger.info("report: " + roleCreatePath + "?" + queryParam);

				int status = executeMethod(roleCreatePath, queryParam);
				if (status == HttpStatus.SC_OK) {
					return true;
				}
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}
		return false;
	}

	/**
	 * 执行上报
	 */
	private boolean doReport(LevelUpData levelUpData) {
		if (accountZmq != null) {
			try {
				String queryParam = String.format(levelUpParam, gameName, platform, channel, serverId, levelUpData.puid, levelUpData.playerId, levelUpData.level);
				accountLogger.info("report: " + levelUpPath + "?" + queryParam);

				int status = executeMethod(levelUpPath, queryParam);
				if (status == HttpStatus.SC_OK) {
					return true;
				}
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}
		return false;
	}

	/**
	 * 执行上报
	 */
	private boolean doReport(RenameRoleData renameRoleData) {
		if (accountZmq != null) {
			try {
				String queryParam = String.format(roleRenameParam, gameName, platform, channel, serverId, renameRoleData.puid, renameRoleData.playerId, renameRoleData.newName);
				accountLogger.info("report: " + roleRenamePath + "?" + queryParam);

				int status = executeMethod(roleRenamePath, queryParam);
				if (status == HttpStatus.SC_OK) {
					return true;
				}
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}
		return false;
	}

	/**
	 * 停止服务，把当前数据全部上报
	 */
	public void stop() {
		if (reportDatas.size() > 0) {
			Object reportData = null;
			reportLock.lock();
			try {
				while (reportDatas.size() > 0) {
					reportData = reportDatas.remove(0);
					if (gameName.length() > 0 && platform.length() > 0) {
						if (reportData instanceof CreateRoleData) {
							doReport((CreateRoleData) reportData);
						} else if (reportData instanceof RenameRoleData) {
							doReport((RenameRoleData) reportData);
						} else if (reportData instanceof LevelUpData) {
							doReport((LevelUpData) reportData);
						} else if (reportData instanceof RegitsterGameServer) {
							doReport((RegitsterGameServer) reportData);
						} else if (reportData instanceof UnRegitsterGameServer) {
							doReport((UnRegitsterGameServer) reportData);
						} else if (reportData instanceof HeartBeatData) {
							doReport((HeartBeatData) reportData);
						}
					}
				}
			}catch (Exception e) {
				HawkException.catchException(e);
			}finally {
				reportLock.unlock();
			}

		}
	}

	/**
	 * 帧更新上报数据
	 */
	@Override
	public void onTick() {
		// 检测连接是否正常
		try {
			int events = accountZmq.updateMonitor(0);
			if ((events & HawkZmq.SocketEvent.CONNECTED) > 0) {
				connectOK = true;
				HawkLog.logPrintln("account zmq client connected: " + this.accountZmqHost);
			} else if ((events & HawkZmq.SocketEvent.DISCONNECTED) > 0) {
				connectOK = false;
				HawkLog.logPrintln("account zmq client disconnected: " + this.accountZmqHost);
			}
			else if ((events & HawkZmq.SocketEvent.CONNECT_RETRIED) > 0) {
				HawkLog.logPrintln("account zmq client connect retried: " + this.accountZmqHost);
			}
			else if ((events & HawkZmq.SocketEvent.CONNECT_DELAYED) > 0) {
				HawkLog.logPrintln("account zmq client connect delayed: " + this.accountZmqHost);
			}

		} catch (Exception e) {
			HawkException.catchException(e);
		}

		if (connectOK == true) {
			// 心跳检测
			long curTime = HawkTime.getMillisecond();
			if (curTime - lastBeatTime >= HEART_PERIOD) {
				lastBeatTime = curTime;
				report(new HeartBeatData());
			}

			if (reportDatas.size() > 0) {
				// 取出队列首个上报数据对象
				Object reportData = null;
				reportLock.lock();
				try {
					reportData = reportDatas.remove(0);
				} finally {
					//和stop操作产生线程安全问题 忽略
					reportLock.unlock();
				}

				// 数据上报操作
				try {
					if (gameName.length() > 0 && platform.length() > 0) {
						if (reportData instanceof CreateRoleData) {
							doReport((CreateRoleData) reportData);
						} else if (reportData instanceof RenameRoleData) {
							doReport((RenameRoleData) reportData);
						} else if (reportData instanceof LevelUpData) {
							doReport((LevelUpData) reportData);
						} else if (reportData instanceof RegitsterGameServer) {
							doReport((RegitsterGameServer) reportData);
						} else if (reportData instanceof UnRegitsterGameServer) {
							doReport((UnRegitsterGameServer) reportData);
						} else if (reportData instanceof HeartBeatData) {
							doReport((HeartBeatData) reportData);
						}
					}
				} catch (Exception e) {
					HawkException.catchException(e);

					// 上报失败重新放回
					reportLock.lock();
					try {
						reportDatas.add(reportData);
					} finally {
						reportLock.unlock();
					}
				}
		}

		}
	}

	@Override
	public String getName() {
		return this.getClass().getSimpleName();
	}
}
