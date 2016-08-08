package com.hawk.version.entity;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;

import org.hawk.db.HawkDBEntity;
import org.hibernate.annotations.GenericGenerator;

@Entity
@Table(name = "version")
public class VersionEntity extends HawkDBEntity{
	@Id
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private int id = 0;

	@Column(name = "code", nullable = false)
	private int code = 0;
	
	@Column(name = "version", nullable = false)
	private String version = null;
	
	@Column(name = "name", nullable = false)
	private String name = null;
	
	@Column(name = "platform", nullable = false)
	private String platform = null;

	@Column(name = "chanel", nullable = false)
	private String chanel = null;
	
	@Column(name = "device", nullable = true)
	private String device = null;
	
	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public int getCode() {
		return code;
	}

	public void setCode(int code) {
		this.code = code;
	}
	
	public String getVersion() {
		return version;
	}

	public void setVersion(String version) {
		this.version = version;
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public String getChanel() {
		return chanel;
	}

	public void setChanel(String chanel) {
		this.chanel = chanel;
	}
	
	public String getPlatform() {
		return platform;
	}

	public void setPlatform(String platform) {
		this.platform = platform;
	}

	public String getDevice() {
		return device;
	}

	public void setDevice(String device) {
		this.device = device;
	}

	@Override
	public int getCreateTime() {
		return 0;
	}

	@Override
	public void setCreateTime(int createTime) {
		
	}
	
	@Override
	public int getUpdateTime() {
		return 0;
	}

	@Override
	public void setUpdateTime(int updateTime) {
		
	}

	@Override
	public boolean isInvalid() {
		return true;
	}

	@Override
	public void setInvalid(boolean invalid) {
		
	}
}
