package com.hawk.game;

import java.util.List;

import org.hawk.app.HawkApp;
import org.hawk.config.HawkConfigStorage;
import org.hawk.db.HawkDBManager;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;
import org.hawk.os.HawkTime;
import org.hawk.xid.HawkXID;
import org.hawk.zmq.HawkZmq;
import org.hawk.zmq.HawkZmqManager;

import com.hawk.game.entity.IpAddrEntity;

/**
 * 游戏应用
 * 
 * @author hawk
 * 
 */
public class IsApp extends HawkApp {
	/**
	 * 服务对象
	 */
	protected HawkZmq serviceZmq; 
	/**
	 * ip实体对象列表
	 */
	protected List<IpAddrEntity> ipAddrEntities;
	/**
	 * 全局静态对象
	 */
	private static IsApp instance = null;

	/**
	 * 获取全局静态对象
	 * 
	 * @return
	 */
	public static IsApp getInstance() {
		return instance;
	}

	/**
	 * 构造函数
	 */
	public IsApp() {
		super(HawkXID.valueOf(0, 0));
		if (instance == null) {
			instance = this;
		}
	}
	
	/**
	 * 从配置文件初始化
	 * 
	 * @param cfg
	 * @return
	 */
	public boolean init(String cfg) {
		IsConfig appCfg = null;
		try {
			HawkConfigStorage cfgStorgae = new HawkConfigStorage(IsConfig.class, getWorkPath());
			appCfg = (IsConfig) cfgStorgae.getConfigList().get(0);
		} catch (Exception e) {
			HawkException.catchException(e);
			return false;
		}

		// 父类初始化
		if (!super.init(appCfg)) {
			return false;
		}

		// 从db加载倒内存
		ipAddrEntities = HawkDBManager.getInstance().query("from IpAddrEntity order by beginIpInt");
		
		// 构建查询服务
		serviceZmq = HawkZmqManager.getInstance().createZmq(HawkZmq.ZmqType.REP);
		if (!serviceZmq.bind(appCfg.getAddr())) {
			HawkLog.logPrintln("service zmq bind failed......");
			return false;
		}
		
		return true;
	}

	/**
	 * 查找实体对象
	 * 
	 * @param ipVal
	 * @return
	 */
	private IpAddrEntity findIpAddrEntity(int ipVal) {
		int low = 0;
        int high = ipAddrEntities.size()-1;
        while (low <= high) {
            int mid = (low + high) >>> 1;
            IpAddrEntity midVal = ipAddrEntities.get(mid);
            if (ipVal > midVal.getEndIpInt())
                low = mid + 1;
            else if (ipVal < midVal.getBeginIpInt())
                high = mid - 1;
            else
                return midVal;
        }
		return null;
	}
	
	/**
	 * 帧更新
	 */
	@Override
	public boolean run() {
		if (!running) {
			running = true;
			HawkLog.logPrintln("server running......");
			
			byte[] recvBytes = new byte[1024];
			while (running) {
				try {
					if (serviceZmq.pollEvent(HawkZmq.HZMQ_EVENT_READ, appCfg.getTickPeriod()) > 0) {
						int recvSize = serviceZmq.recv(recvBytes, 0);
						if (recvSize >= 7) {
							IpAddrEntity entity = null;
							try {
								String ip = new String(recvBytes, 0, recvSize);
								int ipVal = HawkOSOperator.convertIp2Int(ip);
								entity = findIpAddrEntity(ipVal);
								if (entity != null) {
									HawkLog.logPrintln(HawkTime.getTimeString() + ": ipentity query, ip: " + ip + ", entity: " + entity.toPrintString());
								} else {
									HawkLog.logPrintln(HawkTime.getTimeString() + ": ipentity query, ip: " + ip + ", entity: null");
								}
							} catch (Exception e) {
								HawkException.catchException(e);
							} finally {
								if (entity == null) {
									entity = new IpAddrEntity();
								}
								serviceZmq.send(entity.toJson().getBytes(), 0);
							}
						}
					}
				} catch (Exception e) {
					HawkException.catchException(e);
				}
			}			

			onClosed();
			running = false;
			HawkLog.logPrintln("server exit......");
			return true;
		}
		return false;
	}
}
