package com.hawk.orderserver.entify;

import java.util.Map;

import org.hawk.os.HawkException;

import com.google.gson.JsonObject;

public class CallbackInfo {
	protected int id = 0;
	protected String tid = "";
	protected String uid = "";
	protected String product_id = "";
	protected String through_cargo = "";
	protected String type = "";
	protected int callbackStatus = 1;
	protected int status = 0;
	protected int ts = 0;
	/**
	 * 通知时间
	 */
	protected long notifyTime = 0;
	
	public JsonObject toJson() {
		JsonObject jsonObject = new JsonObject();
		jsonObject.addProperty("id", id);
		jsonObject.addProperty("tid", tid);
		jsonObject.addProperty("uid", uid);
		jsonObject.addProperty("product_id", product_id);
		jsonObject.addProperty("through_cargo", through_cargo);
		jsonObject.addProperty("type", type);
		jsonObject.addProperty("status", status);
		jsonObject.addProperty("ts", ts);
		return jsonObject;
	}

	public boolean fromMap(Map<String, String> params) {
		try {
			tid = params.get("tid");
			uid = params.get("uid");
			type = params.get("type");
			through_cargo = params.get("through_cargo");
			product_id = params.get("product_id");
			callbackStatus = Integer.valueOf(params.get("status"));
			ts = Integer.valueOf(params.get("ts"));
			
			return true;
		} catch (Exception e) {
			HawkException.catchException(e);
		}
		return false;
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public String getTid() {
		return tid;
	}

	public void setTid(String tid) {
		this.tid = tid;
	}

	public String getUid() {
		return uid;
	}

	public void setUid(String uid) {
		this.uid = uid;
	}

	public String getProduct_id() {
		return product_id;
	}

	public void setProduct_id(String product_id) {
		this.product_id = product_id;
	}

	public String getThrough_cargo() {
		return through_cargo;
	}

	public void setThrough_cargo(String through_cargo) {
		this.through_cargo = through_cargo;
	}

	public String getType() {
		return type;
	}

	public void setType(String type) {
		this.type = type;
	}

	public int getCallbackStatus() {
		return callbackStatus;
	}

	public void setCallbackStatus(int callbackStatus) {
		this.callbackStatus = callbackStatus;
	}

	public int getStatus() {
		return status;
	}

	public void setStatus(int status) {
		this.status = status;
	}

	public int getTs() {
		return ts;
	}

	public void setTs(int ts) {
		this.ts = ts;
	}

	public long getNotifyTime() {
		return notifyTime;
	}

	public void setNotifyTime(long notifyTime) {
		this.notifyTime = notifyTime;
	}
	
}
