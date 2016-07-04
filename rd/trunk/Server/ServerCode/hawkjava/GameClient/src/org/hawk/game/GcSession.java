package org.hawk.game;

import org.hawk.cryption.HawkDecryption;
import org.hawk.cryption.HawkEncryption;
import org.hawk.net.client.HawkClientSession;
import org.hawk.net.protocol.HawkProtocol;

public class GcSession extends HawkClientSession {
	public GcSession(boolean cryption) {
		super();

		if (cryption) {
			setEncryption(new HawkEncryption());
			setDecryption(new HawkDecryption());
		}
	}

	@Override
	protected boolean onReceived(Object message) {
		if (message instanceof HawkProtocol) {
			HawkProtocol protocol = (HawkProtocol) message;

			return true;
		}
		return false;
	}
}
