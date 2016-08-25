package com.hawk.version.entity;

public class Version {
	/**
	 * id
	 */
	int id;

	/**
	 * 大版本
	 */
	int version;

	/**
	 * 小版本
	 */
	String subVersion;
	
	/**
	 * 资源名称
	 */
	String name;
	
	/**
	 * 资源大小
	 */
	float size;
	
	public Version(int id) {
		super();
		this.id = id;
	}
	
	public Version(int id, int version, String subVersion, String name, float size) {
		super();
		this.id = id;
		this.version = version;
		this.subVersion = subVersion;
		this.name = name;
		this.size = size;
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public int getVersion() {
		return version;
	}

	public void setVersion(int version) {
		this.version = version;
	}

	public String getSubVersion() {
		return subVersion;
	}

	public void setSubVersion(String subVersion) {
		this.subVersion = subVersion;
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public float getSize() {
		return size;
	}

	public void setSize(float size) {
		this.size = size;
	}
}
