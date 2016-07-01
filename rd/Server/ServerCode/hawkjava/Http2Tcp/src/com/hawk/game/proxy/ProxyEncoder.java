package com.hawk.game.proxy;

import java.nio.ByteBuffer;

import org.apache.mina.core.buffer.IoBuffer;
import org.apache.mina.core.session.IoSession;
import org.apache.mina.filter.codec.ProtocolEncoderAdapter;
import org.apache.mina.filter.codec.ProtocolEncoderOutput;
import org.hawk.cryption.HawkBase64;
import org.hawk.net.HawkNetStatistics;
import org.hawk.net.HawkSession;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkException;

import com.google.gson.JsonObject;

/**
 * 协议编码器
 * 
 * @author hawk
 */
public class ProxyEncoder extends ProtocolEncoderAdapter {
	/**
	 * 开始编码输出
	 */
	@Override
	public void encode(IoSession session, Object message, ProtocolEncoderOutput output) throws Exception {
		Object attrObject = session.getAttribute(HawkSession.SESSION_ATTR);
		// 协议编码
		IoBuffer buffer = null;
		if (attrObject instanceof HawkSession) {
			HawkSession hawkSession = (HawkSession) attrObject;
			buffer = encodeProtocol(hawkSession, message);
		}

		if (buffer != null) {
			// 统计更新信息
			HawkNetStatistics.getInstance().onSendBytes(buffer.remaining());
			output.write(buffer);
		}
	}

	/**
	 * 服务器模式编码
	 * 
	 * @param session
	 * @param message
	 * @throws Exception
	 */
	private IoBuffer encodeProtocol(HawkSession session, Object message) throws Exception {
		// 写会话锁定
		session.lock();
		try {
			JsonObject jsonObject = new JsonObject();
			if (message instanceof ProxyProtocol) {
				ProxyProtocol proxyProtocol = (ProxyProtocol) message;
				if (proxyProtocol.isValid()) {
					if (proxyProtocol.getToken() != null) {
						jsonObject.addProperty("token", proxyProtocol.getToken());
					} else {
						jsonObject.addProperty("token", "");
					}

					HawkProtocol[] protocols = proxyProtocol.getProtocols();
					if (protocols != null && protocols.length > 0) {
						IoBuffer outBuffer = session.getOutBuffer().clear();
						for (HawkProtocol protocol : protocols) {
							if (!protocol.encode(outBuffer)) {
								throw new HawkException("protocol encode failed");
							}
						}
						outBuffer.flip();

						// base64编码
						String protocolBase64 = HawkBase64.encode(outBuffer.array(), outBuffer.position(), outBuffer.remaining());
						jsonObject.addProperty("data", protocolBase64);
						outBuffer.clear();
					}
				}
			} else {
				throw new HawkException("protocol message illegality");
			}

			String content = jsonObject.toString();
			String respHeader = String.format(
					"HTTP/1.1 200 OK\r\n" + 
					"Connection: Keep-Alive\r\n" + 
					"Content-Type: text/html\r\n" + 
					"Content-Length: %d\r\n\r\n", content.length());

			ByteBuffer byteBuffer = ByteBuffer.allocate(respHeader.length() + content.length());
			byteBuffer.put(respHeader.getBytes());
			byteBuffer.put(content.getBytes());
			byteBuffer.flip();
			return IoBuffer.wrap(byteBuffer);
		} finally {
			session.unlock();
		}
	}
}
