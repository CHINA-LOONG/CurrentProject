package com.hawk.game.proxy;

import org.apache.mina.core.service.IoHandlerAdapter;
import org.apache.mina.core.session.IdleStatus;
import org.apache.mina.core.session.IoSession;
import org.hawk.app.HawkApp;
import org.hawk.log.HawkLog;
import org.hawk.net.HawkNetStatistics;
import org.hawk.net.HawkSession;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkException;

import com.hawk.game.tcp.ClientSession;
import com.hawk.game.tcp.ClientManager;

/**
 * mina对应的io处理句柄
 * 
 * @author hawk
 */
public class ProxyIoHandler extends IoHandlerAdapter {
	/**
	 * 默认构造
	 */
	protected ProxyIoHandler() {
	}

	/**
	 * 会话创建
	 */
	@Override
	public void sessionCreated(IoSession session) {
		HawkNetStatistics.getInstance().onSessionCreated();

		// 获取ip信息
		String ipaddr = "0.0.0.0";
		try {
			ipaddr = session.getRemoteAddress().toString().split(":")[0].substring(1);
		} catch (Exception e) {
			HawkException.catchException(e);
		}

		try {
			HawkSession hawkSession = new HawkSession();
			if (!hawkSession.onOpened(session)) {
				session.close(false);
				return;
			}

			if (HawkApp.getInstance().getAppCfg().isDebug()) {
				HawkLog.logPrintln(String.format("session opened, ipaddr: %s, total: %d", ipaddr, HawkNetStatistics.getInstance().getCurSession()));
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}

	/**
	 * 开启回调
	 */
	@Override
	public void sessionOpened(IoSession session) throws Exception {
	}

	/**
	 * 消息接收回调
	 */
	@Override
	public void messageReceived(IoSession session, Object message) throws Exception {
		try {
			if (message instanceof ProxyProtocol) {
				ProxyProtocol proxyProtocol = (ProxyProtocol) message;
				
				ClientSession client = null;
				if (proxyProtocol.getToken() == null || proxyProtocol.getToken().length() <= 0) {
					client = ClientManager.getInstance().createTcpClient();
					HawkLog.logPrintln("create tcp client, token: " + client.getToken());
				} else {
					client = ClientManager.getInstance().getTcpClient(proxyProtocol.getToken());
				}
				
				if (client != null && proxyProtocol.isValid()) {
					// 发送模式: 发送给游戏服务器, 并给客户端同步当前令牌
					if (proxyProtocol.getType() == ProxyProtocol.SEND_MODE) {
						HawkProtocol[] protocols = proxyProtocol.getProtocols();
						if (protocols != null && protocols.length > 0) {
							for (HawkProtocol protocol : protocols) {
								client.sendProtocol(protocol);
							}
						}
						
						// 发送完之后同步令牌信息给客户端
						session.write(new ProxyProtocol(ProxyProtocol.RESP_MODE, client.getToken(), null));
					} 

					// 接收模式: 获取当前所有服务器协议缓存, 若不存在, 返回空
					if (proxyProtocol.getType() == ProxyProtocol.RECV_MODE) {
						HawkProtocol[] protocols = client.flush();
						if (protocols != null && protocols.length > 0) {
							session.write(new ProxyProtocol(ProxyProtocol.RESP_MODE, client.getToken(), protocols));
						} else {
							session.write(new ProxyProtocol(ProxyProtocol.RESP_MODE, client.getToken(), null));
						}
					}
				} else {
					session.write(new ProxyProtocol(ProxyProtocol.RESP_MODE, null, null));
				}
				
				// 通知接收到协议对象
				HawkNetStatistics.getInstance().onRecvProto();
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}

	/**
	 * 消息发送成功
	 */
	@Override
	public void messageSent(IoSession session, Object message) {
		// 通知已发送协议对象
		HawkNetStatistics.getInstance().onSendProto();
	}

	/**
	 * 空闲回调
	 */
	@Override
	public void sessionIdle(IoSession session, IdleStatus status) throws Exception {
		try {
			HawkSession hawkSession = (HawkSession) session.getAttribute(HawkSession.SESSION_ATTR);
			if (hawkSession != null) {
				hawkSession.onIdle(status);
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}

	/**
	 * 异常回调
	 */
	@Override
	public void exceptionCaught(IoSession session, Throwable throwable) throws Exception {
		try {
			session.close(false);
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}

	/**
	 * 关闭回调
	 */
	@Override
	public void sessionClosed(IoSession session) throws Exception {
		HawkNetStatistics.getInstance().onSessionClosed();

		if (HawkApp.getInstance().getAppCfg().isDebug()) {
			String ipaddr = session.getRemoteAddress().toString().split(":")[0].substring(1);
			HawkLog.logPrintln(String.format("session closed, ipaddr: %s, total: %d", ipaddr, HawkNetStatistics.getInstance().getCurSession()));
		}

		try {
			HawkSession hawkSession = (HawkSession) session.getAttribute(HawkSession.SESSION_ATTR);
			if (hawkSession != null) {
				hawkSession.onClosed();
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}
}
