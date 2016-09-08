package com.hawk.game.proxy;

import java.net.InetSocketAddress;

import org.apache.mina.core.session.IdleStatus;
import org.apache.mina.filter.codec.ProtocolCodecFilter;
import org.apache.mina.filter.executor.ExecutorFilter;
import org.apache.mina.filter.executor.OrderedThreadPoolExecutor;
import org.apache.mina.transport.socket.nio.NioSocketAcceptor;
import org.hawk.config.HawkXmlCfg;
import org.hawk.log.HawkLog;
import org.hawk.net.HawkNetStatistics;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;

import com.hawk.game.tcp.ClientManager;

/**
 * 
 * @author hawk
 */
public class ProxyServer {
	/**
	 * 运行状态
	 */
	private volatile boolean running;
	/**
	 * 对外http端口
	 */
	private int httpPort = 80;
	/**
	 * http服务线程数
	 */
	private int filterChain = 0;
	/**
	 * 会话缓冲区大小
	 */
	private int bufferSize = 8192;
	/**
	 * 空闲超时
	 */
	private int idleTimeout = 300000;
	/**
	 * 内网服ip地址
	 */
	private String serverIp = "127.0.0.1";
	/**
	 * 内网服端口
	 */
	private int serverPort = 9595;
	/**
	 * 内网服连接超时
	 */
	private int connTimeout = 1000;
	/**
	 * 网络接收器
	 */
	private NioSocketAcceptor acceptor;

	/**
	 * 全局实例对象
	 */
	private static ProxyServer instance = null;

	/**
	 * 获取全局实例单例对象
	 * 
	 * @return
	 */
	public static ProxyServer getInstance() {
		if (instance == null) {
			instance = new ProxyServer();
		}
		return instance;
	}

	/**
	 * 默认构造函数
	 */
	private ProxyServer() {
		running = false;
	}

	/**
	 * 获取游戏服务器ip
	 * @return
	 */
	public String getServerIp() {
		return serverIp;
	}

	/**
	 * 获取游戏服务器端口
	 * @return
	 */
	public int getServerPort() {
		return serverPort;
	}

	/**
	 * 获取缓冲区大小
	 * @return
	 */
	public int getBufferSize() {
		return bufferSize;
	}
	
	/**
	 * 获取连接超时时间
	 * @return
	 */
	public int getConnTimeout() {
		return connTimeout;
	}
	
	/**
	 * 获取空闲超时
	 * @return
	 */
	public int getIdleTimeout() {
		return idleTimeout;
	}
	
	/**
	 * 初始化服务
	 * 
	 * @param xmlCfg
	 */
	public boolean init(String xmlCfg) {
		try {
			// 读取配置文件
			HawkXmlCfg proxyCfg = new HawkXmlCfg(System.getProperty("user.dir") + "/cfg/config.xml");

			// 初始化proxy服务
			httpPort = proxyCfg.getInt("http.port");
			
			if (proxyCfg.containsKey("http.filterChain")) {
				filterChain = proxyCfg.getInt("http.filterChain");
			}
			
			if (proxyCfg.containsKey("http.bufferSize")) {
				bufferSize = proxyCfg.getInt("http.bufferSize");
			}
			
			if (proxyCfg.containsKey("http.idleTimeout")) {
				idleTimeout = proxyCfg.getInt("http.idleTimeout");
			}
			
			if (proxyCfg.containsKey("tcp.ip")) {
				serverIp = proxyCfg.getString("tcp.ip");
			}
			
			if (proxyCfg.containsKey("tcp.port")) {
				serverPort = proxyCfg.getInt("tcp.port");
			}
			
			if (proxyCfg.containsKey("tcp.connTimeout")) {
				connTimeout = proxyCfg.getInt("tcp.connTimeout");
			}
			
			// 初始化网络统计信息
			HawkNetStatistics.getInstance().onTick();

			// 服务端的实例
			acceptor = new NioSocketAcceptor();
			// 地址重用
			acceptor.getSessionConfig().setReuseAddress(true);
			acceptor.getSessionConfig().setSoLinger(0);
			// 设置编码器&解码器
			acceptor.getFilterChain().addLast("codec", new ProtocolCodecFilter(ProxyEncoder.class, ProxyDecoder.class));
			// 设置读取数据的缓冲区大小
			if (bufferSize > 0) {
				acceptor.getSessionConfig().setReadBufferSize(bufferSize);
			}
			// 读写通道无操作进入空闲状态
			if (idleTimeout > 0) {
				acceptor.getSessionConfig().setIdleTime(IdleStatus.BOTH_IDLE, idleTimeout / 1000);
			}
			// 添加IoFilterChain线程池
			if (filterChain > 0) {
				OrderedThreadPoolExecutor executor = new OrderedThreadPoolExecutor(filterChain);
				acceptor.getFilterChain().addLast("threadPool", new ExecutorFilter(executor));
			}
			// 设置服务端的handler
			acceptor.setHandler(new ProxyIoHandler());
			// 绑定ip
			acceptor.bind(new InetSocketAddress(httpPort));
			return true;
		} catch (Exception e) {
			HawkException.catchException(e);
		}
		return false;
	}

	/**
	 * 运行proxy服务
	 */
	public void run() {
		if (running == false) {
			HawkLog.logPrintln("http2tcp proxy running");

			running = true;
			while (running) {
				try {
					// 更新统计
					HawkNetStatistics.getInstance().onTick();
	
					// 更新会话管理器
					ClientManager.getInstance().onTick();
					
					// 控制帧率
					HawkOSOperator.osSleep(10);
				} catch (Exception e) {
					HawkException.catchException(e);
				}
			}
			close();
			HawkLog.logPrintln("http2tcp proxy exit");
		}
	}

	/**
	 * 通知停止proxy服务
	 */
	public void stop() {
		running = false;
	}

	/**
	 * 关闭服务器
	 */
	public void close() {
		if (acceptor != null) {
			try {
				acceptor.unbind();
				acceptor = null;
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}
	}
}
