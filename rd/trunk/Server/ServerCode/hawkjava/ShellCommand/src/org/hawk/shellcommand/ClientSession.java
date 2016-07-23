package org.hawk.shellcommand;

import java.util.concurrent.atomic.AtomicInteger;

import org.hawk.net.client.HawkClientSession;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkOSOperator;

public class ClientSession extends HawkClientSession {
	AtomicInteger reqCount = new AtomicInteger(0);
	
	@Override
	protected boolean onReceived(Object message) {
		if (message instanceof HawkProtocol) {
			HawkProtocol protocol = (HawkProtocol)message;
			if (protocol.getType() == 0) {
				String result = new String(protocol.getOctets().getBuffer().array(), 0, protocol.getSize());
				System.out.println(result);
				reqCount.compareAndSet(0, 1);
			}
		}
		return true;
	}

	public void request(HawkProtocol protocol) {		
		sendProtocol(protocol);
		while(!reqCount.compareAndSet(1, 0)) {
			HawkOSOperator.osSleep(10);
		}
	}
}
