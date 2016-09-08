package com.hawk.game.proxy;

import java.util.Map;

import org.apache.mina.core.buffer.IoBuffer;
import org.apache.mina.core.session.IoSession;
import org.apache.mina.filter.codec.ProtocolDecoderAdapter;
import org.apache.mina.filter.codec.ProtocolDecoderOutput;
import org.hawk.cryption.HawkBase64;
import org.hawk.log.HawkLog;
import org.hawk.net.HawkSession;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;

/**
 * 协议解码器
 * 
 * @author hawk
 */
public class ProxyDecoder extends ProtocolDecoderAdapter {
	/**
	 * 参数格式: /{send|recv}?token=%s&data=%s
	 */
	private static String SEND_PROTOCOL_HEADER = "GET /send?";
	private static String RECV_PROTOCOL_HEADER = "GET /recv?";

	/**
	 * 协议解码
	 */
	@Override
	public void decode(IoSession session, IoBuffer buffer, ProtocolDecoderOutput output) throws Exception {
		// 协议解码
		Object attrObject = session.getAttribute(HawkSession.SESSION_ATTR);
		if (attrObject instanceof HawkSession) {
			HawkSession hawkSession = (HawkSession) attrObject;
			decodeHttpProtocol(hawkSession, buffer, output);
		}
	}

	/**
	 * 服务器接收到协议进行解码
	 * 
	 * @param session
	 * @param buffer
	 * @param output
	 * @throws Exception
	 */
	private boolean decodeHttpProtocol(HawkSession session, IoBuffer buffer, ProtocolDecoderOutput output) {
		try {
			if (session != null) {
				String httpHeader = new String(buffer.array());
				HawkLog.debugPrintln(httpHeader);

				ProxyProtocol proxyProtocol = new ProxyProtocol(0, "", null);
				if (buffer.get(0) == 'G') {
					int beginPos = httpHeader.indexOf("?");
					int endPos = httpHeader.indexOf(" ", beginPos);
					if (beginPos > 0 && endPos > beginPos) {
						String params = httpHeader.substring(beginPos + 1, endPos).trim();
						Map<String, String> paramMap = HawkOSOperator.parseHttpParam(params);

						if (paramMap.containsKey("token")) {
							proxyProtocol.setToken(paramMap.get("token"));
						}

						if (paramMap.containsKey("data")) {
							byte[] bytes = HawkBase64.decode(paramMap.get("data"));
							HawkProtocol protocol = HawkProtocol.valueOf();
							if (protocol.decode(IoBuffer.wrap(bytes))) {
								proxyProtocol.setProtocols(new HawkProtocol[] { protocol });
							}
						}

						// 模式
						if (httpHeader.indexOf(SEND_PROTOCOL_HEADER) >= 0) {
							proxyProtocol.setType(ProxyProtocol.SEND_MODE);
						} else if (httpHeader.indexOf(RECV_PROTOCOL_HEADER) >= 0) {
							proxyProtocol.setType(ProxyProtocol.RECV_MODE);
						}
					}
				}

				if (proxyProtocol.isValid()) {
					output.write(proxyProtocol);
					return true;
				}
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		} finally {
			buffer.position(buffer.limit());
		}
		return false;
	}
}
