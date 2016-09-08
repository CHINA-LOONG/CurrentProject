package org.hawk.app.task;

import java.util.LinkedList;
import java.util.List;

import org.hawk.app.HawkApp;
import org.hawk.cache.HawkCacheObj;
import org.hawk.os.HawkException;
import org.hawk.thread.HawkTask;
import org.hawk.xid.HawkXID;

/**
 * 更新类型任务
 * 
 * @author hawk
 */
public class HawkRefreshTask extends HawkTask {
	/**
	 * refresh时间（毫秒）
	 */
	long refreshTime;
	/**
	 * 对象id
	 */
	HawkXID xid;
	/**
	 * 对象id列表
	 */
	List<HawkXID> xidList;

	/**
	 * 构造函数
	 * 
	 */
	protected HawkRefreshTask() {
	}

	/**
	 * 构造函数
	 * 
	 * @param xid
	 */
	protected HawkRefreshTask(HawkXID xid, long refreshTime) {
		setParam(xid, refreshTime);
	}

	/**
	 * 构造函数
	 * 
	 * @param xid
	 */
	protected HawkRefreshTask(List<HawkXID> xidList, long refreshTime) {
		setParam(xidList, refreshTime);
	}

	/**
	 * 设置任务参数
	 * 
	 * @param xid
	 * @param msg
	 */
	public void setParam(HawkXID xid, long refreshTime) {
		this.refreshTime = refreshTime;
		this.xid = xid;
	}

	/**
	 * 设置任务参数
	 * 
	 * @param xidList
	 * @param msg
	 */
	public void setParam(List<HawkXID> xidList, long refreshTime) {
		this.refreshTime = refreshTime;

		if (this.xidList == null) {
			this.xidList = new LinkedList<HawkXID>();
		}
		this.xidList.clear();
		this.xidList.addAll(xidList);
	}

	/**
	 * 缓存对象清理
	 */
	@Override
	protected void clear() {
		refreshTime = 0;
		xid = null;
		if (xidList != null) {
			xidList.clear();
		}
	}

	/**
	 * 对象克隆
	 */
	@Override
	protected HawkCacheObj clone() {
		return new HawkRefreshTask();
	}

	/**
	 * 执行refresh任务
	 */
	@Override
	protected int run() {
		if (xidList != null && xidList.size() > 0) {
			for (HawkXID xid : xidList) {
				try {
					HawkApp.getInstance().dispatchRefresh(xid, refreshTime);
				} catch (Exception e) {
					HawkException.catchException(e);
				}
			}
		} else {
			try {
				HawkApp.getInstance().dispatchRefresh(xid, refreshTime);
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}
		return 0;
	}

	/**
	 * 构建对象
	 */
	public static HawkRefreshTask valueOf(HawkXID xid, long refreshTime) {
		HawkRefreshTask task = new HawkRefreshTask(xid, refreshTime);
		return task;
	}

	/**
	 * 构建对象
	 */
	public static HawkRefreshTask valueOf(List<HawkXID> xidList, long refreshTime) {
		HawkRefreshTask task = new HawkRefreshTask(xidList, refreshTime);
		return task;
	}
}
