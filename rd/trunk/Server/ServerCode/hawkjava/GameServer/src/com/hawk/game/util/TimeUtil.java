package com.hawk.game.util;

import java.util.Calendar;

import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkTime;

import com.hawk.game.config.TimeCfg;

public class TimeUtil {

	public static final int fieldList[] = {
		Calendar.YEAR,
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
		int valueList[] = {
			time.get(Calendar.YEAR),
			time.get(Calendar.MONTH),
			time.get(Calendar.DAY_OF_MONTH),
			time.get(Calendar.DAY_OF_WEEK),
			time.get(Calendar.HOUR_OF_DAY),
			time.get(Calendar.MINUTE)
			};
		int leftList[] = {
			timeBegin.getYear(),
			timeBegin.getMonth(),
			timeBegin.getDayOfMonth(),
			timeBegin.getDayOfWeek(),
			timeBegin.getHour(),
			timeBegin.getMinute()
			};
		int rightList[] = {
			timeEnd.getYear(),
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
					leftList[i] != GsConst.UNUSABLE && rightList[i] != GsConst.UNUSABLE &&
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
	private static int isFieldInPeriodLeft(int value, int left) {
		if (left != GsConst.UNUSABLE) {
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
	private static int isFieldInPeriodRight(int value, int right) {
		if (right != GsConst.UNUSABLE) {
			if (value > right) {
				return OUT_PERIOD;
			} else if (value < right) {
				return IN_PERIOD;
			}
		}
		return FURTHER;
	}

	/**
	 * 获取配置时间点的未来刷新时间（大于等于当前时间的刷新时间）
	 * @return 如果该时间点还能刷新，返回未来刷新时间，否则返回null
	 */
	public static Calendar getComingRefreshTime(int timeCfgId, Calendar curTime) {
		TimeCfg timeCfg = HawkConfigManager.getInstance().getConfigByKey(TimeCfg.class, timeCfgId);
		if (null != timeCfg) {
			Calendar comingRefreshTime = HawkTime.getCalendar();

			final int scaleField = timeCfg.getScaleField();
			final int cycleField = timeCfg.getCycleField();
			final int year = timeCfg.getYear();
			final int month = timeCfg.getMonth();
			final int dayOfMonth = timeCfg.getDayOfMonth();
			final int dayOfWeek = timeCfg.getDayOfWeek();
			final int hour = timeCfg.getHour();
			final int minute = timeCfg.getMinute();

			// 设置为当前时间，进行推演
			comingRefreshTime.setTimeInMillis(curTime.getTimeInMillis());

			// 尺度和尺度以前的时间设为配置，周期及以后使用当前时间
			switch (scaleField) {
			case Calendar.MINUTE:
				comingRefreshTime.set(Calendar.MINUTE, minute);
				break;

			case Calendar.HOUR_OF_DAY:
				comingRefreshTime.set(Calendar.MINUTE, minute);
				comingRefreshTime.set(Calendar.HOUR_OF_DAY, hour);
				break;

			case Calendar.DAY_OF_WEEK:
				comingRefreshTime.set(Calendar.MINUTE, minute);
				comingRefreshTime.set(Calendar.HOUR_OF_DAY, hour);
				comingRefreshTime.set(Calendar.DAY_OF_WEEK, dayOfWeek);
				break;

			case Calendar.DAY_OF_MONTH:
				comingRefreshTime.set(Calendar.MINUTE, minute);
				comingRefreshTime.set(Calendar.HOUR_OF_DAY, hour);
				comingRefreshTime.set(Calendar.DAY_OF_MONTH, dayOfMonth);
				break;

			case Calendar.MONTH:
				comingRefreshTime.set(Calendar.MINUTE, minute);
				comingRefreshTime.set(Calendar.HOUR_OF_DAY, hour);
				if (dayOfWeek != GsConst.UNUSABLE) {
					comingRefreshTime.set(Calendar.DAY_OF_WEEK, dayOfWeek);
				} else if (dayOfMonth != GsConst.UNUSABLE) {
					comingRefreshTime.set(Calendar.DAY_OF_MONTH, dayOfMonth);
				}
				comingRefreshTime.set(Calendar.MONTH, month);
				break;

			case Calendar.YEAR:
				comingRefreshTime.set(Calendar.MINUTE, minute);
				comingRefreshTime.set(Calendar.HOUR_OF_DAY, hour);
				if (dayOfWeek != GsConst.UNUSABLE) {
					comingRefreshTime.set(Calendar.DAY_OF_WEEK, dayOfWeek);
				} else if (dayOfMonth != GsConst.UNUSABLE) {
					comingRefreshTime.set(Calendar.DAY_OF_MONTH, dayOfMonth);
				}
				comingRefreshTime.set(Calendar.MONTH, month);
				comingRefreshTime.set(Calendar.YEAR, year);
				break;

			default:
				break;
			}

			// 最小尺度为分，秒和毫秒设为0
			comingRefreshTime.set(Calendar.SECOND, 0);
			comingRefreshTime.set(Calendar.MILLISECOND, 0);

			// 如果下次刷新时间<当前时间，加1个周期
			if (comingRefreshTime.compareTo(curTime) < 0) {
				// 固定时间没有周期
				if (cycleField == GsConst.UNUSABLE) {
					return null;
				}
				comingRefreshTime.add(cycleField, 1);
			}

			return comingRefreshTime;

			// TODO
		}
		return null;
	}

	/**
	 * 获取配置时间点的预期刷新时间
	 * @return 如果该时间点需要刷新，返回预期刷新时间，否则返回null
	 */
	public static Calendar getExpectedRefreshTime(int timeCfgId, Calendar curTime, Calendar lastRefreshTime) {
		TimeCfg timeCfg = HawkConfigManager.getInstance().getConfigByKey(TimeCfg.class, timeCfgId);
		if (null != timeCfg) {
			try {
				Calendar expectedRefreshTime = HawkTime.getCalendar();

				if (null == lastRefreshTime) {
					lastRefreshTime = HawkTime.getCalendar();
					lastRefreshTime.setTimeInMillis(0);
				}

				boolean shouldRefresh = checkAndCalcExpectedRefreshTime(timeCfg, curTime, lastRefreshTime, expectedRefreshTime);
				if (true == shouldRefresh) {
					return expectedRefreshTime;
				}
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}

		return null;
	}

	/**
	 * 检查是否需要刷新。如是，计算预期刷新时间
	 * @param cfg 时间配置
	 * @param curTime 输入当前时间
	 * @param lastRefreshTime 输入上次刷新时间
	 * @param expectedRefreshTime 输出预期刷新时间
	 * @return 是否刷新
	 */
	private static boolean checkAndCalcExpectedRefreshTime(TimeCfg cfg, Calendar curTime, Calendar lastRefreshTime, Calendar expectedRefreshTime) {

		final int scaleField = cfg.getScaleField();
		final int cycleField = cfg.getCycleField();
		final int year = cfg.getYear();
		final int month = cfg.getMonth();
		final int dayOfMonth = cfg.getDayOfMonth();
		final int dayOfWeek = cfg.getDayOfWeek();
		final int hour = cfg.getHour();
		final int minute = cfg.getMinute();

		// 设置为当前时间，进行回溯
		expectedRefreshTime.setTimeInMillis(curTime.getTimeInMillis());

		// 尺度和尺度以前的时间设为配置，周期及以后使用当前时间
		switch (scaleField) {
		case Calendar.MINUTE:
			if (false == adjustRefreshOpen(Calendar.YEAR, year, curTime, lastRefreshTime, expectedRefreshTime)) {
				return false;
			}
			if (false == adjustRefreshOpen(Calendar.MONTH, month, curTime, lastRefreshTime, expectedRefreshTime)) {
				return false;
			}
			if (false == adjustRefreshOpen(Calendar.DAY_OF_MONTH, dayOfMonth, curTime, lastRefreshTime, expectedRefreshTime)) {
				return false;
			}
			if (false == adjustRefreshOpen(Calendar.DAY_OF_WEEK, dayOfWeek, curTime, lastRefreshTime, expectedRefreshTime)) {
				return false;
			}

			expectedRefreshTime.set(Calendar.MINUTE, minute);
			break;

		case Calendar.HOUR_OF_DAY:
			if (false == adjustRefreshOpen(Calendar.YEAR, year, curTime, lastRefreshTime, expectedRefreshTime)) {
				return false;
			}
			if (false == adjustRefreshOpen(Calendar.MONTH, month, curTime, lastRefreshTime, expectedRefreshTime)) {
				return false;
			}

			expectedRefreshTime.set(Calendar.MINUTE, minute);
			expectedRefreshTime.set(Calendar.HOUR_OF_DAY, hour);
			break;

		case Calendar.DAY_OF_WEEK:
			if (false == adjustRefreshOpen(Calendar.YEAR, year, curTime, lastRefreshTime, expectedRefreshTime)) {
				return false;
			}
			if (false == adjustRefreshOpen(Calendar.MONTH, month, curTime, lastRefreshTime, expectedRefreshTime)) {
				return false;
			}

			expectedRefreshTime.set(Calendar.MINUTE, minute);
			expectedRefreshTime.set(Calendar.HOUR_OF_DAY, hour);
			expectedRefreshTime.set(Calendar.DAY_OF_WEEK, dayOfWeek);
			break;

		case Calendar.DAY_OF_MONTH:
			if (false == adjustRefreshOpen(Calendar.YEAR, year, curTime, lastRefreshTime, expectedRefreshTime)) {
				return false;
			}

			expectedRefreshTime.set(Calendar.MINUTE, minute);
			expectedRefreshTime.set(Calendar.HOUR_OF_DAY, hour);
			expectedRefreshTime.set(Calendar.DAY_OF_MONTH, dayOfMonth);
			break;

		case Calendar.MONTH:
			expectedRefreshTime.set(Calendar.MINUTE, minute);
			expectedRefreshTime.set(Calendar.HOUR_OF_DAY, hour);
			if (dayOfWeek != GsConst.UNUSABLE) {
				expectedRefreshTime.set(Calendar.DAY_OF_WEEK, dayOfWeek);
			} else if (dayOfMonth != GsConst.UNUSABLE) {
				expectedRefreshTime.set(Calendar.DAY_OF_MONTH, dayOfMonth);
			}
			expectedRefreshTime.set(Calendar.MONTH, month);
			break;

		case Calendar.YEAR:
			expectedRefreshTime.set(Calendar.MINUTE, minute);
			expectedRefreshTime.set(Calendar.HOUR_OF_DAY, hour);
			if (dayOfWeek != GsConst.UNUSABLE) {
				expectedRefreshTime.set(Calendar.DAY_OF_WEEK, dayOfWeek);
			} else if (dayOfMonth != GsConst.UNUSABLE) {
				expectedRefreshTime.set(Calendar.DAY_OF_MONTH, dayOfMonth);
			}
			expectedRefreshTime.set(Calendar.MONTH, month);
			expectedRefreshTime.set(Calendar.YEAR, year);
			break;

		default:
			break;
		}

		// 最小尺度为分，秒和毫秒设为0
		expectedRefreshTime.set(Calendar.SECOND, 0);
		expectedRefreshTime.set(Calendar.MILLISECOND, 0);

		// 如果预期刷新时间>当前时间，减1个周期
		if (expectedRefreshTime.compareTo(curTime) > 0) {
			// 固定时间没有周期
			if (cycleField == GsConst.UNUSABLE) {
				return false;
			}
			expectedRefreshTime.add(cycleField, -1);
		}

		if (expectedRefreshTime.compareTo(lastRefreshTime) <= 0) {
			return false;
		}

		return true;
	}

	/**
	 * 检查刷新开启状态并调整预期刷新时间
	 * @return 是否可刷新
	 */
	private static boolean adjustRefreshOpen(int scaleField, int value, Calendar curTime, Calendar lastRefreshTime, Calendar expectedRefreshTime) {
		if (value != GsConst.UNUSABLE) {
			// 现在已关闭
			if (curTime.get(scaleField) > value) {
				// 上次刷新时就已关闭
				if (lastRefreshTime.get(scaleField) > value) {
					return false;
				}
				// 上次刷新时正开启，或还未开启
				else {
					expectedRefreshTime.set(scaleField, value);
				}
			}
			// 还未开启
			else if (curTime.get(scaleField) < value) {
				return false;
			}
			// 现在正开启，不用变
		}
		return true;
	}
}
