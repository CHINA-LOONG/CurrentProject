package com.hawk.account.zmq;

import java.util.HashMap;
import java.util.Map;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.thread.HawkTask;
import org.hawk.thread.HawkThreadPool;
import org.hawk.util.HawkTickable;
import org.hawk.zmq.HawkZmq;
import org.hawk.zmq.HawkZmq.SocketEvent;
import org.hawk.zmq.HawkZmqManager;

import com.hawk.account.AccountServices;
import com.hawk.account.httpHandler.HeartBeatHandler;
import com.hawk.account.httpHandler.RegistGameServerHandler;
import com.hawk.account.httpHandler.UnRegistGameServerHandler;
import com.hawk.account.httpHandler.UserCreateRoleHandler;
import com.hawk.account.httpHandler.UserLevelUpHandler;

public class AccountZmqServer extends HawkTickable{
	/**
	 * 服务地址
	 */
	private String zmqAddr;
	/**
	 * 服务zmq
	 */
	private HawkZmq serviceZmq;
	/**
	 * 接收数据缓冲区
	 */
	private byte[] bytes = null;
	/**
	 * monitor 数据
	 */
	StringBuilder stringBuilder = new StringBuilder();
	/**
	 * 线程池
	 */
	private HawkThreadPool threadPool;
	/**
	 * 数据库管理器单例对象
	 */
	static AccountZmqServer instance;

	/**
	 * 获取数据库管理器单例对象
	 * 
	 * @return
	 */
	public static AccountZmqServer getInstance() {
		if (instance == null) {
			instance = new AccountZmqServer();
		}
		return instance;
	}

	/**
	 * 函数
	 */
	private AccountZmqServer() {

	}

	/**
	 * 获取名字
	 */
	@Override
	public String getName() {
		return this.getClass().getSimpleName();
	}

	/**
	 * 解析zmq投递参数
	 * 
	 * @param params
	 * @return
	 */
	public static Map<String, String> parseZmqParam(String params) {
		Map<String, String> paramMap = new HashMap<String, String>();
		try {
			if (params != null && params.length() > 0) {
				HawkLog.logPrintln("ZmqReport: " + params);
				if (params != null) {
					String[] querys = params.split("&");
					for (String query : querys) {
						// param maybe empty string, use -1
						String[] pair = query.split("=", -1);
						if (pair.length == 2) {
							paramMap.put(pair[0], pair[1]);
						}
					}
				}
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
		return paramMap;
	}

	/**
	 * 开启服务
	 */
	public boolean setup(String addr, int pool) {
		// 创建通用缓冲区
		bytes = new byte[1024 * 1024 ];
		AccountServices.getInstance().addTickable(this);
		this.zmqAddr = addr;
		serviceZmq = HawkZmqManager.getInstance().createZmq(HawkZmq.ZmqType.PULL);
		if (!serviceZmq.bind(addr)) {
			HawkLog.logPrintln("service zmq bind failed......");
			return false;
		}

		if (pool > 0) {
			threadPool = new HawkThreadPool(getClass().getSimpleName());
			if (!threadPool.initPool(pool)) {
				return false;
			}

			if (!threadPool.start()) {
				return false;
			}

			HawkLog.logPrintln("zmq service thread pool running......");
		}

		serviceZmq.startMonitor(SocketEvent.CONNECTED | SocketEvent.DISCONNECTED);

		return true;
	}

	/**
	 * 停止服务
	 */
	public void stop() {
		try {
			if (serviceZmq != null) {
				HawkZmqManager.getInstance().closeZmq(serviceZmq);
				serviceZmq = null;
			}

			if (threadPool != null) {
				threadPool.close(true);
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}

	/**
	 * 获取服务地址
	 * 
	 * @return
	 */
	public String getAddr() {
		return this.zmqAddr;
	}

	/**
	 * 获取服务端口
	 * 
	 * @return
	 */
	public int getPort() {
		if (zmqAddr != null && zmqAddr.length() > 0) {
			int pos = zmqAddr.lastIndexOf(":");
			if (pos > 0) {
				String port = zmqAddr.substring(pos + 1, zmqAddr.length());
				return Integer.valueOf(port);
			}
		}
		return 0;
	}

	/**
	 * 帧更新事件
	 */
	@Override
	public void onTick() {
		while (serviceZmq.pollEvent(HawkZmq.HZMQ_EVENT_READ, 0) > 0) {
			int recvSize = serviceZmq.recv(bytes, 0);
			if (recvSize > 0) {
				final String reportPath = new String(bytes, 0, recvSize);
				if (serviceZmq.isWaitRecv()) {
					recvSize = serviceZmq.recv(bytes, 0);
					if (recvSize > 0) {
						final String params = new String(bytes, 0, recvSize);
						if (threadPool == null) {
							doUserAciton(reportPath, parseZmqParam(params));
						} else {
							threadPool.addTask(new HawkTask(true) {
								@Override
								protected int run() {
									doUserAciton(reportPath, parseZmqParam(params));
									return 0;
								}
							});
						}
					}
				}
			}
		}
	}

	/**
	 * 
	 * @param reportPath
	 * @param params
	 */
	private void doUserAciton(String reportPath, Map<String, String> params) {
		try {
			if (reportPath.equals("/regist_gameserver")) {
				RegistGameServerHandler.doReport(params);
			}else if (reportPath.equals("/unregist_gameserver")) {
				UnRegistGameServerHandler.doReport(params);
			}else if (reportPath.equals("/report_roleCreate")) {
				UserCreateRoleHandler.doReport(params);
			} else if (reportPath.equals("/report_levelUp")) {
				UserLevelUpHandler.doReport(params);
			}  else if (reportPath.equals("/heartBeat")) {
				HeartBeatHandler.doReport(params);
			} 
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}
}
