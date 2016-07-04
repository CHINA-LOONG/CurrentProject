package com.hawk.game.util;

import java.util.Calendar;

import com.hawk.game.config.TimeCfg;

public class RefreshTime {

	/**
	 * 检查是否现在刷新，如是，计算下次刷新时间
	 * @param cfg 时间配置
	 * @param curTime 输入当前时间
	 * @param lastRefreshTime 输入上次刷新时间
	 * @param nextRefreshTime 输出下次刷新时间
	 * @return 是否刷新
	 */
	public static boolean getNextRefreshTime(TimeCfg cfg, Calendar curTime, Calendar lastRefreshTime, Calendar nextRefreshTime) {

		final int scaleField = cfg.getScaleField();
		final int cycleField = cfg.getCycleField();
		final int year = cfg.getYear();
		final int month = cfg.getMonth();
		final int dayOfMonth = cfg.getDayOfMonth();
		final int dayOfWeek = cfg.getDayOfWeek();
		final int hour = cfg.getHour();
		final int minute = cfg.getMinute();

		// 设置为当前时间，进行回溯
		nextRefreshTime.setTimeInMillis(curTime.getTimeInMillis());

		// 尺度和尺度以前的时间设为配置，周期及以后使用当前时间
		switch (scaleField) {
		case Calendar.MINUTE:
			if (false == adjustRefreshOpen(Calendar.YEAR, year, curTime, lastRefreshTime, nextRefreshTime)) {
				return false;
			}
			if (false == adjustRefreshOpen(Calendar.MONTH, month, curTime, lastRefreshTime, nextRefreshTime)) {
				return false;
			}
			if (false == adjustRefreshOpen(Calendar.DAY_OF_MONTH, dayOfMonth, curTime, lastRefreshTime, nextRefreshTime)) {
				return false;
			}
			if (false == adjustRefreshOpen(Calendar.DAY_OF_WEEK, dayOfWeek, curTime, lastRefreshTime, nextRefreshTime)) {
				return false;
			}

			nextRefreshTime.set(Calendar.SECOND, 0);
			nextRefreshTime.set(Calendar.MINUTE, minute);
			break;

		case Calendar.HOUR_OF_DAY:
			if (false == adjustRefreshOpen(Calendar.YEAR, year, curTime, lastRefreshTime, nextRefreshTime)) {
				return false;
			}
			if (false == adjustRefreshOpen(Calendar.MONTH, month, curTime, lastRefreshTime, nextRefreshTime)) {
				return false;
			}

			nextRefreshTime.set(Calendar.SECOND, 0);
			nextRefreshTime.set(Calendar.MINUTE, minute);
			nextRefreshTime.set(Calendar.HOUR_OF_DAY, hour);
			break;

		case Calendar.DAY_OF_WEEK:
			if (false == adjustRefreshOpen(Calendar.YEAR, year, curTime, lastRefreshTime, nextRefreshTime)) {
				return false;
			}
			if (false == adjustRefreshOpen(Calendar.MONTH, month, curTime, lastRefreshTime, nextRefreshTime)) {
				return false;
			}

			nextRefreshTime.set(Calendar.SECOND, 0);
			nextRefreshTime.set(Calendar.MINUTE, minute);
			nextRefreshTime.set(Calendar.HOUR_OF_DAY, hour);
			nextRefreshTime.set(Calendar.DAY_OF_WEEK, dayOfWeek);
			break;

		case Calendar.DAY_OF_MONTH:
			if (false == adjustRefreshOpen(Calendar.YEAR, year, curTime, lastRefreshTime, nextRefreshTime)) {
				return false;
			}

			nextRefreshTime.set(Calendar.SECOND, 0);
			nextRefreshTime.set(Calendar.MINUTE, minute);
			nextRefreshTime.set(Calendar.HOUR_OF_DAY, hour);
			nextRefreshTime.set(Calendar.DAY_OF_MONTH, dayOfMonth);
			break;

		case Calendar.MONTH:
			nextRefreshTime.set(Calendar.SECOND, 0);
			nextRefreshTime.set(Calendar.MINUTE, minute);
			nextRefreshTime.set(Calendar.HOUR_OF_DAY, hour);
			if (dayOfWeek != TimeCfg.NO_VALUE) {
				nextRefreshTime.set(Calendar.DAY_OF_WEEK, dayOfWeek);
			} else if (dayOfMonth != TimeCfg.NO_VALUE) {
				nextRefreshTime.set(Calendar.DAY_OF_MONTH, dayOfMonth);
			}
			nextRefreshTime.set(Calendar.MONTH, month);
			break;

		case Calendar.YEAR:
			nextRefreshTime.set(Calendar.SECOND, 0);
			nextRefreshTime.set(Calendar.MINUTE, minute);
			nextRefreshTime.set(Calendar.HOUR_OF_DAY, hour);
			if (dayOfWeek != TimeCfg.NO_VALUE) {
				nextRefreshTime.set(Calendar.DAY_OF_WEEK, dayOfWeek);
			} else if (dayOfMonth != TimeCfg.NO_VALUE) {
				nextRefreshTime.set(Calendar.DAY_OF_MONTH, dayOfMonth);
			}
			nextRefreshTime.set(Calendar.MONTH, month);
			nextRefreshTime.set(Calendar.YEAR, year);
			break;

		default:
			break;
		}

		// 如果下次刷新时间>当前时间，减1个周期
		if (nextRefreshTime.compareTo(curTime) >= 0) {
			// 固定时间没有周期
			if (cycleField == TimeCfg.NO_VALUE) {
				return false;
			}
			nextRefreshTime.add(cycleField, -1);
		}

		if (nextRefreshTime.compareTo(lastRefreshTime) <= 0) {
			return false;
		}

		return true;
	}

	/**
	 * 检查更新开启状态并调整下个更新时间
	 * @return 是否可更新
	 */
	private static boolean adjustRefreshOpen(int scaleField, int value, Calendar curTime, Calendar lastRefreshTime, Calendar nextRefreshTime) {
		if (value != TimeCfg.NO_VALUE) {
			// 现在已关闭
			if (curTime.get(scaleField) > value) {
				// 上次更新就已关闭
				if (lastRefreshTime.get(scaleField) > value) {
					return false;
				}
				// 上次更新正开启 或 还未开启
				else {
					nextRefreshTime.set(scaleField, value);
				}
			}
			// 还未开启
			else if (value > curTime.get(scaleField)) {
				return false;
			}
			// 现在正开启，不用变
		}
		return true;
	}
}
