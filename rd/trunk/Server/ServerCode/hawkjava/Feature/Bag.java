package com.feefoxes.game.entity;

import java.util.List;

import org.hawk.db.HawkDBManager;
import org.hawk.os.HawkException;

import com.feefoxes.game.protocol.Bag.BagInfo;

public class Bag extends BagInfo.Builder {
	public Bag() {
		super();
	}
	
	public Bag(String value) {
		super();
	}
	
	public byte[] toByteArray() {
		return super.build().toByteArray();
	}
	
	public boolean parseFrom(byte[] bytes) {
		try {
			super.mergeFrom(bytes);
			return true;
		} catch (Exception e) {
			HawkException.catchException(e);
			return false;
		}
	}
	
	@Override
	public String toString() {
		return super.build().toString();
	}
	
	public static void main(String args) {
		byte[] value = null;
		HawkDBManager.getInstance().getConfiguration().addClass(Bag.class);
		List<Bag> bagEntitys = HawkDBManager.getInstance().query("from Bag");
		if (bagEntitys != null && bagEntitys.size() > 0) {
			for (Bag bag : bagEntitys) {
				System.out.println(bag.toString());
				if (value == null) {
					value = bag.toByteArray();
				}
			}
		}
		Bag bag2 = new Bag();
		bag2.setPlayerid(5);
		bag2.setItemid(11);
		bag2.setCount(10);
		HawkDBManager.getInstance().create(bag2);
		
		Bag bag1 = new Bag();
		if (bag1.parseFrom(value)) {
			bag1.setCount(100);
			HawkDBManager.getInstance().update(bag1);
		}
	}
}
