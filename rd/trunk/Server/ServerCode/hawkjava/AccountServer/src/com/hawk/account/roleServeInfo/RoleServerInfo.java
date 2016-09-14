package com.hawk.account.roleServeInfo;

public class RoleServerInfo {
	public int server;
	public String nickname;
	public int level;

	public RoleServerInfo(int server, int level, String nickname){
		this.server = server;
		this.nickname = nickname;
		this.level = level;
	}

	@Override
	public String toString() {
		return super.toString() + " " + server + " "  + nickname + " "  + level;
	}
}