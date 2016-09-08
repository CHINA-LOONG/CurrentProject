package com.hawk.version;

import java.util.HashMap;
import java.util.Iterator;
import java.util.LinkedHashMap;
import java.util.Map;
import org.hawk.config.HawkXmlCfg;
import com.hawk.version.entity.Version;

public class VersionServices {
	/**
	 * 运行状态
	 */
	volatile boolean running = true;

	/**
	 * 版本信息
	 */
	private Map<String, Map<Integer, Version>> versionList = new HashMap<String, Map<Integer, Version>>();
	
	/**
	 * 大版本信息
	 */
	private Map<String, Integer> currentVersion = new HashMap<String, Integer>();
	
	/**
	 * 资源服务器地址
	 */
	private String resourceAddress = null;

	/**
	 * 渠道
	 */
	private String channel = null;
	
	/**
	 * 单例实例对象
	 */
	private static VersionServices instance = null;
	
	/**
	 * 获取实例对象
	 * 
	 * @return
	 */
	public static VersionServices getInstance() {
		if (instance == null) {
			instance = new VersionServices();
		}
		return instance;
	}

	public boolean init(String chanel, String resourceAddress) {
		try {
			this.channel = chanel;
			this.resourceAddress = resourceAddress;
			
			HawkXmlCfg conf = new HawkXmlCfg(System.getProperty("user.dir") + "/cfg/version.xml");	
			Iterator<String> iterator = conf.getKeys();
			while (iterator.hasNext()) {
				String key = iterator.next();
				String result[] = key.split("\\[");
			 	if (result.length != 2) {
					return false;
				}
				
			 	result = result[0].split("\\.");
			 	if (result.length != 2) {
					return false;
				}
			 	
			 	// 添加平台
			 	String platform = result[0];
			 	Map<Integer, Version> platformVersions = versionList.get(platform);
			 	if (platformVersions == null) {
			 		platformVersions = new LinkedHashMap<Integer, Version>();
			 		versionList.put(platform, platformVersions);
				}
			 	
			 	if (currentVersion.get(platform) == null) {
					currentVersion.put(platform, 0);
				}
			 	
			 	result = result[1].split("_");
			 	if (result.length != 2) {
					return false;
				}
			 	
			 	Version version = platformVersions.get(Integer.valueOf(result[1]));
			 	if (version == null) {
			 		version = new Version(Integer.valueOf(result[1]));
			 		platformVersions.put(Integer.valueOf(result[1]), version);
				}
			 	
			 	if (key.contains("[@version]")) {
					version.setVersion(conf.getInt(key));
					if (currentVersion.get(platform) < conf.getInt(key)) {
						currentVersion.put(platform, conf.getInt(key));
					}
				}
			 	else if (key.contains("[@subversion]")) {
			 		String test = conf.getString(key);
					version.setSubVersion(test);
				}
			 	else if (key.contains("[@name]")) {
					version.setName(conf.getString(key));
				}
			 	else if (key.contains("[@size]")) {
					version.setSize(conf.getFloat(key));
				}
			}
			
		} catch (Exception e) {
			System.out.println(e);
			return false;
		}

		return false;
	}	

	public Map<Integer, Version> getPlatformVersions(String platform){
		return versionList.get(platform);
	}
	
	public Integer getCurrentVersion(String platform){
		return currentVersion.get(platform);
	}
	
	public String getChanel() {
		return channel;
	}

	public void setChanel(String chanel) {
		this.channel = chanel;
	}
	
	public String getResourceAddress() {
		return resourceAddress;
	}

	public void setResourceAddress(String resourceAddress) {
		this.resourceAddress = resourceAddress;
	}

	public boolean tick() {
		return running;
	}
	
	/**
	 * 停止
	 */
	public void stop() {
		running = false;
	}
}