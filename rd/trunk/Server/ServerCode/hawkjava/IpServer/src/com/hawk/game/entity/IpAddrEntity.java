package com.hawk.game.entity;

import java.io.Serializable;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;

import org.hibernate.annotations.GenericGenerator;

@Entity
@Table(name = "ip_addr")
public class IpAddrEntity implements Serializable {
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	@Id
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private int id;

	@Column(name = "beginIp")
	private String beginIp;
	
	@Column(name = "endIp")
	private String endIp;

	@Column(name = "beginIpInt")
	private int beginIpInt;
	
	@Column(name = "endIpInt")
	private int endIpInt;
	
	@Column(name = "position")
	private String position;
	
	@Column(name = "province")
	private int province;
	
	@Column(name = "city")
	private int city;

	@Override
	public boolean equals(Object o) {
		if(o instanceof IpAddrEntity) {
			return this.beginIpInt == ((IpAddrEntity)o).beginIpInt && this.endIpInt == ((IpAddrEntity)o).endIpInt;
		}
		return false;
	}
	
	@Override
	public int hashCode() {
		return beginIpInt + endIpInt;
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public String getBeginIp() {
		return beginIp;
	}

	public void setBeginIp(String beginIp) {
		this.beginIp = beginIp;
	}

	public String getEndIp() {
		return endIp;
	}

	public void setEndIp(String endIp) {
		this.endIp = endIp;
	}

	public int getBeginIpInt() {
		return beginIpInt;
	}

	public void setBeginIpInt(int beginIpInt) {
		this.beginIpInt = beginIpInt;
	}

	public int getEndIpInt() {
		return endIpInt;
	}

	public void setEndIpInt(int endIpInt) {
		this.endIpInt = endIpInt;
	}

	public String getPosition() {
		return position;
	}

	public void setPosition(String position) {
		this.position = position;
	}

	public int getProvince() {
		return province;
	}

	public void setProvince(int province) {
		this.province = province;
	}

	public int getCity() {
		return city;
	}

	public void setCity(int city) {
		this.city = city;
	}

	public String toPrintString() {
		return String.format("%s-%s[%s]", beginIp, endIp, position);
	}
}
