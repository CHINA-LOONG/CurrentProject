package com.hawk.game.item;

public class GemInfo {
	/**
	 * 宝石种类
	 */
	private int type;
	/**
	 * 宝石Id
	 */
	private String gemId;
	
	public GemInfo(int type, String gemId) {
		this.gemId = gemId;
		this.type = type;
	}

	public int getType() {
		return type;
	}

	public void setType(int type) {
		this.type = type;
	}

	public String getGemId() {
		return gemId;
	}

	public void setGemId(String gemId) {
		this.gemId = gemId;
	}
}
