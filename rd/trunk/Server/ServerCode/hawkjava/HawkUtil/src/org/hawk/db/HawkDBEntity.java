package org.hawk.db;

import java.io.Serializable;
import java.util.Calendar;
import java.util.concurrent.atomic.AtomicLong;

import org.hawk.os.HawkTime;

/**
 * db实体对象基类, 对应sql需要对invalid字段进行索引创建
 * 
 * @author hawk
 */
@SuppressWarnings("serial")
public abstract class HawkDBEntity implements Serializable {
	/**
	 * 实体状态
	 */
	private AtomicLong entityState = new AtomicLong(0);
	
	/**
	 * 获取对象创建时间
	 * @return
	 */
	public abstract Calendar getCreateTime();

	/**
	 * 设置对象创建时间
	 * @param createTime
	 */
	public abstract void setCreateTime(Calendar createTime);

	/**
	 * 获取对象更新时间
	 * @return
	 */
	public abstract Calendar getUpdateTime();

	/**
	 * 设置对象更新时间
	 * @param updateTime
	 */
	public abstract void setUpdateTime(Calendar updateTime);

	/**
	 * 判断对象是否有效
	 * @return
	 */
	public abstract boolean isInvalid();

	/**
	 * 设置对象无效
	 * @param invalid
	 */
	public abstract void setInvalid(boolean invalid);
	
	/**
	 * 获取实体状态
	 */
	public AtomicLong getEntityState() {
		return entityState;
	}
	
	/**
	 * entity加载完成后手动调用, 进行数据解码构建
	 * 备注: 返回true表示格式正确, 否则数据发生错误
	 */
	public boolean decode() {
		return true;
	}
	
	/**
	 * entity更新或创建前自动调用, 进行数据压缩编码
	 * 备注: 返回true表示格式正确, 否则数据发生错误
	 */
	public boolean encode() {
		return true;
	}
	
	/**
	 * 通知创建到db
	 * 
	 * @return
	 */
	public boolean notifyCreate() {
		setCreateTime(HawkTime.getCalendar());
		encode();
		
		return HawkDBManager.getInstance().create(this);
	}
	
	/**
	 * 同步删除对象
	 */
	public void delete() {
		delete(true);
	}
	
	/**
	 * 删除对象(同步 or 异步)
	 */
	public void delete(boolean async) {
		setInvalid(true);
		if (async) {
			notifyUpdate(true);
		} else {
			HawkDBManager.getInstance().delete(this);
		}
	}

	/**
	 * 通知db异步更新
	 */
	public void notifyUpdate()  {
		notifyUpdate(true);
	}
	
	/**
	 * 通知db更新(同步 or 异步更新)
	 */
	public void notifyUpdate(boolean async) {
		setUpdateTime(HawkTime.getCalendar());
		encode();
		
		if (async) {
			HawkDBManager.getInstance().updateAsync(this);
		} else {
			HawkDBManager.getInstance().update(this);
		}
	}
}
