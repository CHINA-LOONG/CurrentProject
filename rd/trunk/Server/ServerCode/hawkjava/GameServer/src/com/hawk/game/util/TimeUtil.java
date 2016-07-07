package com.hawk.game.util;

import java.util.Calendar;

import org.hawk.log.HawkLog;
import com.hawk.game.config.TimeCfg;

public class TimeUtil {
	
	public static final int fieldList[] = {Calendar.YEAR,
		Calendar.MONTH,
		Calendar.DAY_OF_MONTH,
		Calendar.DAY_OF_WEEK,
		Calendar.HOUR_OF_DAY,
		Calendar.MINUTE
		};
	public static final int OUT_PERIOD = 1;
	public static final int IN_PERIOD = 2;
	public static final int FURTHER= 3;
	
	public static boolean isTimeInPeriod(Calendar time, TimeCfg timeBegin, TimeCfg timeEnd) {
		int valueList[] = {time.get(Calendar.YEAR),
			time.get(Calendar.MONTH),
			time.get(Calendar.DAY_OF_MONTH),
			time.get(Calendar.DAY_OF_WEEK),
			time.get(Calendar.HOUR_OF_DAY),
			time.get(Calendar.MINUTE)
			};
		int leftList[] = {timeBegin.getYear(),
			timeBegin.getMonth(),
			timeBegin.getDayOfMonth(),
			timeBegin.getDayOfWeek(),
			timeBegin.getHour(),
			timeBegin.getMinute()
			};
		int rightList[] = {timeEnd.getYear(),
			timeEnd.getMonth(),
			timeEnd.getDayOfMonth(),
			timeEnd.getDayOfWeek(),
			timeEnd.getHour(),
			timeEnd.getMinute()
			};
		
		int resultLeft = FURTHER;
		int resultRight = FURTHER;
		
		// 从最大尺度到最小尺度一次比较
		// 当值在区间边界或区间边界未设定时，需要比较更小尺度
		// 确定在区间内的边界不再比较更小尺度
		for (int i = 0; i < valueList.length; ++i) {
			if (resultLeft == FURTHER && resultRight == FURTHER &&
					leftList[i] != TimeCfg.NO_VALUE && rightList[i] != TimeCfg.NO_VALUE &&
					leftList[i] > rightList[i]) {
				HawkLog.errPrintln(String.format("time field: %d period left: %d is after right: %d", fieldList[i], leftList[i], rightList[i]));
				return false;
			}
			if (resultLeft == FURTHER) {
				resultLeft = isFieldInPeriodLeft(valueList[i], leftList[i]);
			}
			if (resultRight == FURTHER) {
				resultRight = isFieldInPeriodRight(valueList[i], rightList[i]);
			}
			
			if (resultLeft == OUT_PERIOD || resultRight == OUT_PERIOD) {
				return false;
			} else if (resultLeft == IN_PERIOD && resultRight == IN_PERIOD) {
				return true;
			}
		}
		
		return true;
	}

	/**
	 * @param value 待测值
	 * @param left 左边界
	 */
	public static int isFieldInPeriodLeft(int value, int left) {
		if (left != TimeCfg.NO_VALUE) {
			if (value < left) {
				return OUT_PERIOD;
			} else if (value > left) {
				return IN_PERIOD;
			}
		}
		return FURTHER;
	}
	
	/**
	 * @param value 待测值
	 * @param right 右边界
	 */
	public static int isFieldInPeriodRight(int value, int right) {
		if (right != TimeCfg.NO_VALUE) {
			if (value > right) {
				return OUT_PERIOD;
			} else if (value < right) {
				return IN_PERIOD;
			}
		}
		return FURTHER;
	}
	
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

			nextRefreshTime.set(Calendar.MINUTE, minute);
			break;

		case Calendar.HOUR_OF_DAY:
			if (false == adjustRefreshOpen(Calendar.YEAR, year, curTime, lastRefreshTime, nextRefreshTime)) {
				return false;
			}
			if (false == adjustRefreshOpen(Calendar.MONTH, month, curTime, lastRefreshTime, nextRefreshTime)) {
				return false;
			}

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

			nextRefreshTime.set(Calendar.MINUTE, minute);
			nextRefreshTime.set(Calendar.HOUR_OF_DAY, hour);
			nextRefreshTime.set(Calendar.DAY_OF_WEEK, dayOfWeek);
			break;

		case Calendar.DAY_OF_MONTH:
			if (false == adjustRefreshOpen(Calendar.YEAR, year, curTime, lastRefreshTime, nextRefreshTime)) {
				return false;
			}

			nextRefreshTime.set(Calendar.MINUTE, minute);
			nextRefreshTime.set(Calendar.HOUR_OF_DAY, hour);
			nextRefreshTime.set(Calendar.DAY_OF_MONTH, dayOfMonth);
			break;

		case Calendar.MONTH:
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

		// 最小尺度为分，秒和毫秒设为0
		nextRefreshTime.set(Calendar.SECOND, 0);
		nextRefreshTime.set(Calendar.MILLISECOND, 0);
		
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
