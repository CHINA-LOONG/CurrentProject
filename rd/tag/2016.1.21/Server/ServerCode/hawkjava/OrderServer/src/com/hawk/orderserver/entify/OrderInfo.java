package com.hawk.orderserver.entify;

import org.hawk.util.services.helper.HawkOrderEntity;

public class OrderInfo extends HawkOrderEntity {
	/**
	 * 通知时间
	 */
	protected long notifyTime;

	public long getNotifyTime() {
		return notifyTime;
	}

	public void setNotifyTime(long notifyTime) {
		this.notifyTime = notifyTime;
	}

	@Override
	public boolean equals(Object o) {
		if (o instanceof OrderInfo) {
			OrderInfo orderInfo = (OrderInfo) o;
			if (myOrder.equals(orderInfo.getMyOrder())) {
				return true;
			}
		}
		return false;
	}
}
