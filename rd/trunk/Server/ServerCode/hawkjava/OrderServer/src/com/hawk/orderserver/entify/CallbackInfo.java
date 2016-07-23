package com.hawk.orderserver.entify;

import org.hawk.os.HawkException;

import com.google.gson.JsonObject;

public class CallbackInfo {
	protected int id = 0;
	protected String myOrder = "";
	protected String payPf = "";
	protected String pfOrder = "";
	protected int payMoney = 0;
	protected String userData = "";
	protected int status = 0;
	
	public JsonObject toJson() {
		JsonObject jsonObject = new JsonObject();
		jsonObject.addProperty("id", id);
		jsonObject.addProperty("myOrder", myOrder);
		jsonObject.addProperty("payPf", payPf);
		jsonObject.addProperty("pfOrder", pfOrder);
		jsonObject.addProperty("payMoney", payMoney);
		jsonObject.addProperty("userData", userData);
		jsonObject.addProperty("status", status);
		return jsonObject;
	}
	
	public boolean fromJson(JsonObject jsonObject) {
		try {
			myOrder = jsonObject.get("myOrder").getAsString();
			pfOrder = jsonObject.get("pfOrder").getAsString();
			payMoney = jsonObject.get("payMoney").getAsInt();
			if (jsonObject.get("payPf") != null) {
				payPf = jsonObject.get("payPf").getAsString();
			}
			if (jsonObject.get("userData") != null) {
				userData = jsonObject.get("userData").getAsString();
			}
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
	
	public String getMyOrder() {
		return myOrder;
	}

	public void setMyOrder(String myOrder) {
		this.myOrder = myOrder;
	}

	public String getPayPf() {
		return payPf;
	}

	public void setPayPf(String payPf) {
		this.payPf = payPf;
	}
	
	public String getPfOrder() {
		return pfOrder;
	}

	public void setPfOrder(String pfOrder) {
		this.pfOrder = pfOrder;
	}

	public int getPayMoney() {
		return payMoney;
	}

	public void setPayMoney(int payMoney) {
		this.payMoney = payMoney;
	}

	public String getUserData() {
		return userData;
	}

	public void setUserData(String userData) {
		this.userData = userData;
	}
	
	public int getStatus() {
		return status;
	}

	public void setStatus(int status) {
		this.status = status;
	}
}
