package com.hawk.game;

import java.text.SimpleDateFormat;
import java.util.Date;

import org.hawk.app.HawkAppCfg;
import org.hawk.config.HawkConfigManager;
import org.hawk.os.HawkException;

@HawkConfigManager.KVResource(file = "cfg/gs.cfg")
public class GsConfig extends HawkAppCfg {
	/**
	 * 灰度状态
	 */
	protected final int grayState;
	/**
	 * 开服时间
	 */
	protected final String serviceDate;
	/**
	 * 开服时间
	 */
	private Date serverOpenDate;
	/**
	 * 当服最大注册数
	 */
	protected final int registerMaxSize;
	/**
	 * 玩家角色最大数
	 */
	protected final int roleMaxSize;
	/**
	 * cdk地址
	 */
	protected final String cdkHost;
	/**
	 * cdk超时
	 */
	protected final int cdkTimeout;
	/**
	 * 账号服务器地址
	 */
	protected final String accountHost;
	/**
	 * 账号服务器超时
	 */
	protected final int accountTimeout;
	/**
	 * 数据上报地址
	 */
	protected final String reportHost;
	/**
	 * 数据上报超时
	 */
	protected final int reportTimeout;
	/**
	 * ip代理地址
	 */
	protected final String ipProxyAddr;
	/**
	 * ip服务超时
	 */
	protected final int ipProxyTimeout;
	/**
	 * 是否开启翻译
	 */
	protected final boolean translate;
	/**
	 * 邮箱用户名
	 */
	protected final String emailUser;
	/**
	 * 邮箱密码
	 */
	protected final String emailPwd;
	/**
	 * 全局静态对象
	 */
	private static GsConfig instance = null;

	/**
	 * 获取全局静态对象
	 * 
	 * @return
	 */
	public static GsConfig getInstance() {
		return instance;
	}

	public GsConfig() {
		instance = this;
		registerMaxSize = 0;
		roleMaxSize = 0;
		grayState = 0;
		cdkHost = "";
		cdkTimeout = 1000;
		accountHost = "";
		accountTimeout = 1000;
		reportHost = "";
		reportTimeout = 1000;
		serviceDate = "20150101";
		ipProxyAddr = "";
		ipProxyTimeout = 1000;
		translate = false;
		emailUser = "";
		emailPwd = "";
	}

	public int getRegisterMaxSize() {
		return registerMaxSize;
	}

	public int getRoleMaxSize() {
		return roleMaxSize;
	}

	public int getGrayState() {
		return grayState;
	}

	public String getServiceDate() {
		return serviceDate;
	}

	public String getCdkHost() {
		return cdkHost;
	}

	public int getCdkTimeout() {
		return cdkTimeout;
	}

	public String getAccountHost() {
		return accountHost;
	}

	public int getAccountTimeout() {
		return accountTimeout;
	}

	public String getReportHost() {
		return reportHost;
	}

	public int getReportTimeout() {
		return reportTimeout;
	}

	public String getIpProxyAddr() {
		return ipProxyAddr;
	}

	public int getIpProxyTimeout() {
		return ipProxyTimeout;
	}

	public Date getServerOpenDate() {
		return serverOpenDate;
	}

	public boolean isTranslate() {
		return translate;
	}

	public String getEmailUser() {
		return emailUser;
	}

	public String getEmailPwd() {
		return emailPwd;
	}

	@Override
	protected boolean assemble() {
		if (this.serviceDate != null) {
			try {
				this.serverOpenDate = new SimpleDateFormat("yyyyMMdd").parse(serviceDate);
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}
		return true;
	}
}
