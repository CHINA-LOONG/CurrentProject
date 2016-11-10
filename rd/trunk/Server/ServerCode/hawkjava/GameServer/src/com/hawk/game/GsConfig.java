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
	 * 活动服务器地址
	 */
	protected final String activityServerAddr;
	/**
	 * 活动服务器端口
	 */
	protected final int activityServerPort;
	/**
	 * ip代理地址
	 */
	protected final String ipProxyAddr;
	/**
	 * ip服务超时
	 */
	protected final int ipProxyTimeout;
	/**
	 * BI log
	 */
	protected final boolean logBI;
	/**
	 * 邮箱用户名
	 */
	protected final String emailUser;
	/**
	 * 邮箱密码
	 */
	protected final String emailPwd;
	/**
	 * IM-是否开启翻译
	 */
	protected final boolean imTrans;
	/**
	 * IM-世界频道消息队列释放触发容量
	 */
	protected final int imWorldQueueFreeBeginSize;
	/**
	 * IM-世界频道消息队列释放目标容量
	 */
	protected final int imWorldQueueFreeEndSize;
	/**
	 * IM-单人推送协议包含消息最大数量
	 */
	protected final int imPushPackMsgMaxCount;
	/**
	 * IM-推送线程区间
	 */
	protected final int imPushThreadMin;
	protected final int imPushThreadMax;
	/**
	 * IM-翻译线程区间
	 */
	protected final int imTransThreadMin;
	protected final int imTransThreadMax;
	/**
	 * IM-生成推送任务帧数间隔
	 */
	protected final int imPushTaskTickCount;
	/**
	 * IM-生成翻译任务帧数间隔
	 */
	protected final int imTransTaskTickCount;
	/**
	 * IM-推送任务处理消息最大数量
	 */
	protected final int imPushTaskMsgMaxCount;
	/**
	 * IM-翻译任务处理消息最大数量
	 */
	protected final int imTransTaskMsgMaxCount;

	// assemble
	protected int[] imPushThreadRange;
	protected int[] imTransThreadRange;

	/**
	 * 全局静态对象
	 */
	private static GsConfig instance = null;

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
		activityServerAddr = "";
		activityServerPort = 0;
		reportHost = "";
		reportTimeout = 1000;
		serviceDate = "20150101";
		ipProxyAddr = "";
		ipProxyTimeout = 1000;
		emailUser = "";
		emailPwd = "";
		logBI = false;
		imTrans = false;
		imWorldQueueFreeBeginSize = Integer.MAX_VALUE;
		imWorldQueueFreeEndSize = (int) (imWorldQueueFreeBeginSize * 0.8);
		imPushPackMsgMaxCount = 20;
		imPushThreadMin = 0;
		imPushThreadMax = threadNum - 1;
		imTransThreadMin = 0;
		imTransThreadMax = threadNum - 1;
		imPushTaskTickCount = 1;
		imTransTaskTickCount = 1;
		imPushTaskMsgMaxCount = Integer.MAX_VALUE;
		imTransTaskMsgMaxCount = Integer.MAX_VALUE;
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

		imPushThreadRange = new int[] {imPushThreadMin, imPushThreadMax};
		imTransThreadRange = new int[] {imTransThreadMin, imTransThreadMax};
		return true;
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

	public String getActivityServerAddr() {
		return activityServerAddr;
	}

	public int getActivityServerport() {
		return activityServerPort;
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

	public boolean isLogBI() {
		return logBI;
	}

	public String getEmailUser() {
		return emailUser;
	}

	public String getEmailPwd() {
		return emailPwd;
	}

	public boolean isImTranslate() {
		return imTrans;
	}

	public int getImWorldQueueFreeBeginSize() {
		return imWorldQueueFreeBeginSize;
	}

	public int getImWorldQueueFreeEndSize() {
		return imWorldQueueFreeEndSize;
	}

	public int getImPushPackMsgMaxCount() {
		return imPushPackMsgMaxCount;
	}

	public int[] getImPushThreadRange() {
		return imPushThreadRange;
	}

	public int[] getImTransThreadRange() {
		return imTransThreadRange;
	}

	public int getImPushTaskTickCount() {
		return imPushTaskTickCount;
	}

	public int getImTransTaskTickCount() {
		return imTransTaskTickCount;
	}

	public int getImPushTaskMsgMaxCount() {
		return imPushTaskMsgMaxCount;
	}

	public int getImTransTaskMsgMaxCount() {
		return imTransTaskMsgMaxCount;
	}
}
