package org.hawk.chatmonitor.zmq;

public class ChatSendItem {
	public String identify;
	public String msg;
	
	public ChatSendItem() {
	}
	
	public ChatSendItem(String identify, String msg) {
		this.identify = identify;
		this.msg = msg;
	}
}
