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
public class HawkTickTask extends HawkTask {
	/**
	 * tick时间（毫秒）
	 */
	long tickTime;
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
	protected HawkTickTask() {
	}

	/**
	 * 构造函数
	 * 
	 * @param xid
	 */
	protected HawkTickTask(HawkXID xid, long tickTime) {
		setParam(xid, tickTime);
	}

	/**
	 * 构造函数
	 * 
	 * @param xid
	 */
	protected HawkTickTask(List<HawkXID> xidList, long tickTime) {
		setParam(xidList, tickTime);
	}

	/**
	 * 设置任务参数
	 * 
	 * @param xid
	 * @param msg
	 */
	public void setParam(HawkXID xid, long tickTime) {
		this.tickTime = tickTime;
		this.xid = xid;
	}

	/**
	 * 设置任务参数
	 * 
	 * @param xidList
	 * @param msg
	 */
	public void setParam(List<HawkXID> xidList, long tickTime) {
		this.tickTime = tickTime;

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
		tickTime = 0;
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
		return new HawkTickTask();
	}

	/**
	 * 执行tick任务
	 */
	@Override
	protected int run() {
		if (xidList != null && xidList.size() > 0) {
			for (HawkXID xid : xidList) {
				try {
					HawkApp.getInstance().dispatchTick(xid, tickTime);
				} catch (Exception e) {
					HawkException.catchException(e);
				}
			}
		} else {
			try {
				HawkApp.getInstance().dispatchTick(xid, tickTime);
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}
		return 0;
	}

	/**
	 * 构建对象
	 */
	public static HawkTickTask valueOf(HawkXID xid, long tickTime) {
		HawkTickTask task = new HawkTickTask(xid, tickTime);
		return task;
	}

	/**
	 * 构建对象
	 */
	public static HawkTickTask valueOf(List<HawkXID> xidList, long tickTime) {
		HawkTickTask task = new HawkTickTask(xidList, tickTime);
		return task;
	}
}
