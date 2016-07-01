package com.hawk.game.tcp;

import java.util.Deque;
import java.util.concurrent.LinkedBlockingDeque;

import org.apache.mina.core.buffer.IoBuffer;
import org.hawk.cryption.HawkMd5;
import org.hawk.net.client.HawkClientSession;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkTime;

import com.hawk.game.proxy.ProxyServer;

public class ClientSession extends HawkClientSession {
	/**
	 * 会话令牌
	 */
	private String token;
	/**
	 * 输出缓冲区
	 */
	private IoBuffer buffer;
	/**
	 * 服务器返回协议队列
	 */
	private Deque<HawkProtocol> protocolQueue;

	/**
	 * 构造
	 */
	public ClientSession() {
		this.protocolQueue = new LinkedBlockingDeque<HawkProtocol>();
		this.buffer = IoBuffer.allocate(ProxyServer.getInstance().getBufferSize());
		String identify = String.format("hash: %d, time: %d", hashCode(), HawkTime.getMillisecond());
		this.token = HawkMd5.makeMD5(identify);
	}

	/**
	 * 获取令牌
	 * 
	 * @return
	 */
	public String getToken() {
		return token;
	}

	/**
	 * 输出协议
	 * 
	 * @return
	 */
	public synchronized HawkProtocol[] flush() {
		HawkProtocol[] protocols = null;
		if (protocolQueue.size() > 0) {
			buffer.clear();
			protocols = new HawkProtocol[protocolQueue.size()];
			for (int i = 0; i < protocols.length; i++) {
				protocols[i] = protocolQueue.poll();
			}
		}
		return protocols;
	}

	/**
	 * 接收到游戏服务器协议之后添加到队列
	 */
	@Override
	protected boolean onReceived(Object message) {
		if (message instanceof HawkProtocol) {
			protocolQueue.addLast((HawkProtocol) message);
			return true;
		}
		return false;
	}
	
	/**
	 * 会话被关闭
	 */
	@Override
	protected void onClosed() {
		super.onClosed();
		// 关闭缓存的会话
		ClientManager.getInstance().removeTcpClient(token);
	}
}
