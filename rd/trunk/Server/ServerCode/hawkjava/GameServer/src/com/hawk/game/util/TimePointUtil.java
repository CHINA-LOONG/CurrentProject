package com.hawk.game.util;

import java.util.Arrays;
import java.util.Calendar;
import java.util.List;

import org.hawk.config.HawkConfigManager;
import org.hawk.os.HawkTime;

import com.hawk.game.config.TimePointCfg;

/**
 * 通用时间点库
 * 
 * @author walker
 */
public class TimePointUtil {

	// "每月几号”风格字段队列
	public static final List<Integer> MONTH_STYLE_FIELD_QUEUE = Arrays.asList(
		Calendar.MINUTE,
		Calendar.HOUR_OF_DAY,
		Calendar.DAY_OF_MONTH,
		Calendar.MONTH,
		Calendar.YEAR
	);
	// "每周几”风格字段队列
	public static final List<Integer> WEEK_STYLE_FIELD_QUEUE = Arrays.asList(
		Calendar.MINUTE,
		Calendar.HOUR_OF_DAY,
		Calendar.DAY_OF_WEEK,
		Calendar.DAY_OF_WEEK_IN_MONTH,
		Calendar.MONTH,
		Calendar.YEAR
	);

	// 时间方向
	public static final int PAST = 1;
	public static final int FUTURE = 2;

	/**
	 * 判断时间是否在两个时间点之间
	 * @return 如果在时间点之间返回true，不在时间点之间返回false
	 */
	public static boolean isTimeInPeriod(Calendar time, int beginTimeCfgId, int endTimeCfgId) {
		TimePointCfg beginTimePoint = HawkConfigManager.getInstance().getConfigByKey(TimePointCfg.class, beginTimeCfgId);
		if (null == beginTimePoint) {
			return false;
		}
		TimePointCfg endTimePoint = HawkConfigManager.getInstance().getConfigByKey(TimePointCfg.class, endTimeCfgId);
		if (null == endTimePoint) {
			return false;
		}

		Calendar maxBeginTime = computeMaxPastTriggerTime(beginTimePoint, time);
		if (null == maxBeginTime) {
			return false;
		}
		Calendar minEndTime = computeMinFutureTriggerTime(endTimePoint, time);
		if (null == minEndTime) {
			return false;
		}

		// 时间差大于一个周期则不在时间段内
		if (beginTimePoint.getCycleField() != GsConst.UNUSABLE) {
			maxBeginTime.add(beginTimePoint.getCycleField(), 1);
			if (maxBeginTime.getTimeInMillis() < minEndTime.getTimeInMillis()) {
				return false;
			}
		}

		return true;
	}

	/**
	 * 获取配置时间点的即将刷新的时间，即：min( X >= curTime )
	 * @return 如果该时间点还可以刷新，返回即将到来的刷新时间，否则返回null
	 */
	public static Calendar getComingRefreshTime(int timeCfgId, Calendar curTime) {
		TimePointCfg timePoint = HawkConfigManager.getInstance().getConfigByKey(TimePointCfg.class, timeCfgId);
		if (null != timePoint) {
			return computeMinFutureTriggerTime(timePoint, curTime);
		}
		return null;
	}

	/**
	 * 获取配置时间点的刚刚应该刷新的时间，即：max( lastRefreshTime < X <= curTime )
	 * @return 如果该时间点需要刷新，返回预期刷新时间，否则返回null
	 */
	public static Calendar getExpectedRefreshTime(int timeCfgId, Calendar curTime, Calendar lastRefreshTime) {
		TimePointCfg timePoint = HawkConfigManager.getInstance().getConfigByKey(TimePointCfg.class, timeCfgId);
		if (null != timePoint) {
			Calendar expectedRefreshTime = computeMaxPastTriggerTime(timePoint, curTime);
			if (null != expectedRefreshTime && true == expectedRefreshTime.after(lastRefreshTime)) {
				return expectedRefreshTime;
			}
		}
		return null;
	}

	// 内部函数----------------------------------------------------------------------------------------------------------------

	/**
	 * 获取配置时间点的即将触发的时间，即：min( X >= curTime )
	 * @return 如果该时间点还可以触发，返回即将到来的触发时间，否则返回null
	 */
	private static Calendar computeMinFutureTriggerTime(TimePointCfg timePoint, Calendar curTime) {
		// 计算基准触发时间
		Calendar minFutureTriggerTime = computeBaseTriggerTime(timePoint, curTime, FUTURE);
		if (null == minFutureTriggerTime) {
			return null;
		}

		// 基准触发时间从周期以上字段开始向未来推演
		// 1. 单次循环中途不可重计算calendar，保证字段的一致性
		// 2. DAY_OF_WEEK最小值取每一个DAY_OF_WEEK_IN_MONTH的第一天，设置对应DAY_OF_MONTH，保证时间参考系的绝对性
		if (timePoint.getCycleField() != GsConst.UNUSABLE) {
			final List<Integer> cfgFieldQueue = timePoint.getFieldQueue();
			final List<Integer> cfgValueQueue = timePoint.getValueQueue();
			int lowerSetBegin = 0;
			boolean isDayOfWeekMin = false;

			for (int i = cfgFieldQueue.indexOf(timePoint.getCycleField()) + 1; i < cfgFieldQueue.size(); ++i) {
				int cfgValue = cfgValueQueue.get(i);
				if (cfgValue == GsConst.UNUSABLE) {
					continue;
				}

				int field = cfgFieldQueue.get(i);
				int value = minFutureTriggerTime.get(field);

				// 相等，比较更高字段
				if (value == cfgValue) {
					continue;
				}

				// 不相等，向未来推演
				// 记录DAY_OF_WEEK_IN_MONTH
				int inMonthValue = 0;

				// 如果大于，高字段进位
				if (value > cfgValue) {
					// 最高字段，不能进位
					if (i == cfgFieldQueue.size() - 1) {
						return null;
					}
					// 进位
					minFutureTriggerTime.add(cfgFieldQueue.get(i + 1), 1);
				}

				// 设为配置值
				minFutureTriggerTime.set(field, cfgValue);
				if (field == Calendar.DAY_OF_WEEK_IN_MONTH) {
					inMonthValue = cfgValue;
				}

				// 更低字段设为配置值，没有配置值设为最小值
				for (int j = lowerSetBegin; j < i; ++j) {
					int lowerField = cfgFieldQueue.get(j);
					int lowerCfgValue = cfgValueQueue.get(j);

					if (lowerCfgValue != GsConst.UNUSABLE) {
						minFutureTriggerTime.set(lowerField, lowerCfgValue);
						if (lowerField == Calendar.DAY_OF_WEEK_IN_MONTH) {
							inMonthValue = lowerCfgValue;
						}
					} else {
						int min = minFutureTriggerTime.getActualMinimum(lowerField);
						minFutureTriggerTime.set(lowerField, min);
						if (lowerField == Calendar.DAY_OF_WEEK_IN_MONTH) {
							inMonthValue = min;
						} else if (lowerField == Calendar.DAY_OF_WEEK) {
							isDayOfWeekMin = true;
						}
					}
				}
				lowerSetBegin = i;

				// 转换DAY_OF_WEEK最小值为DAY_OF_MONTH，利用set的最近优先原则
				if (true == isDayOfWeekMin) {
					if (true == minFutureTriggerTime.isSet(Calendar.DAY_OF_WEEK_IN_MONTH)) {
						minFutureTriggerTime.set(Calendar.DAY_OF_MONTH, inMonthValue * 7 - 6);
					} else if (true == minFutureTriggerTime.isSet(Calendar.DAY_OF_WEEK)) {
						inMonthValue = minFutureTriggerTime.get(Calendar.DAY_OF_WEEK_IN_MONTH);
						minFutureTriggerTime.set(Calendar.DAY_OF_MONTH, inMonthValue * 7 - 6);
					}
				}
			}
		}

		// 重计算calendar
		minFutureTriggerTime.getTimeInMillis();
		return minFutureTriggerTime;
	}

	/**
	 * 获取配置时间点的刚刚触发的时间，即：max( X <= curTime )
	 * @return 如果该时间点触发过，返回刚刚过去的触发时间，否则返回null
	 */
	private static Calendar computeMaxPastTriggerTime(TimePointCfg timePoint, Calendar curTime) {
		// 计算基准触发时间
		Calendar maxPastTriggerTime = computeBaseTriggerTime(timePoint, curTime, PAST);
		if (null == maxPastTriggerTime) {
			return null;
		}

		// 基准触发时间从周期以上字段开始向过去回溯
		// 1. 单次循环中途不可重计算calendar，保证字段的一致性
		// 2. DAY_OF_WEEK最大值取每一个DAY_OF_WEEK_IN_MONTH的最后一天，设置对应DAY_OF_MONTH，保证时间参考系的绝对性
		if (timePoint.getCycleField() != GsConst.UNUSABLE) {
			final List<Integer> cfgFieldQueue = timePoint.getFieldQueue();
			final List<Integer> cfgValueQueue = timePoint.getValueQueue();
			int lowerSetBegin = 0;
			boolean isDayOfWeekMax = false;

			for (int i = cfgFieldQueue.indexOf(timePoint.getCycleField()) + 1; i < cfgFieldQueue.size(); ++i) {
				int cfgValue = cfgValueQueue.get(i);
				if (cfgValue == GsConst.UNUSABLE) {
					continue;
				}

				int field = cfgFieldQueue.get(i);
				int value = maxPastTriggerTime.get(field);

				// 相等，比较更高字段
				if (value == cfgValue) {
					continue;
				}

				// 不相等，向过去回溯
				// 记录DAY_OF_WEEK_IN_MONTH
				int inMonthValue = 0;

				// 如果小于，高字段退位
				if (value < cfgValue) {
					// 最高字段，不能退位
					if (i == cfgFieldQueue.size() - 1) {
						return null;
					}
					// 退位
					maxPastTriggerTime.add(cfgFieldQueue.get(i + 1), -1);
				}

				// 设为配置值
				maxPastTriggerTime.set(field, cfgValue);
				if (field == Calendar.DAY_OF_WEEK_IN_MONTH) {
					inMonthValue = cfgValue;
				}

				// 更低字段设为配置值，没有配置值设为最大值
				for (int j = lowerSetBegin; j < i; ++j) {
					int lowerField = cfgFieldQueue.get(j);
					int lowerCfgValue = cfgValueQueue.get(j);

					if (lowerCfgValue != GsConst.UNUSABLE) {
						maxPastTriggerTime.set(lowerField, lowerCfgValue);
						if (lowerField == Calendar.DAY_OF_WEEK_IN_MONTH) {
							inMonthValue = lowerCfgValue;
						}
					} else {
						int max = maxPastTriggerTime.getActualMaximum(lowerField);
						maxPastTriggerTime.set(lowerField, max);
						if (lowerField == Calendar.DAY_OF_WEEK_IN_MONTH) {
							inMonthValue = max;
						} else if (lowerField == Calendar.DAY_OF_WEEK) {
							isDayOfWeekMax = true;
						}
					}
				}
				lowerSetBegin = i;

				// 转换DAY_OF_WEEK最大值为DAY_OF_MONTH，利用set的最近优先原则
				if (true == isDayOfWeekMax) {
					if (true == maxPastTriggerTime.isSet(Calendar.DAY_OF_WEEK_IN_MONTH)) {
						maxPastTriggerTime.set(Calendar.DAY_OF_MONTH, inMonthValue * 7);
					} else if (true == maxPastTriggerTime.isSet(Calendar.DAY_OF_WEEK)) {
						inMonthValue = maxPastTriggerTime.get(Calendar.DAY_OF_WEEK_IN_MONTH);
						maxPastTriggerTime.set(Calendar.DAY_OF_MONTH, inMonthValue * 7);
					}
				}
			}
		}

		// 重计算calendar
		maxPastTriggerTime.getTimeInMillis();
		return maxPastTriggerTime;
	}

	/**
	 * 计算基准触发时间，即在调整方向上最接近当前时间的触发时间
	 * @param timePoint 时间点配置
	 * @param curTime 当前时间
	 * @param direction 时间调整方向: PAST/FUTURE
	 * @return 基准触发时间
	 */
	private static Calendar computeBaseTriggerTime(TimePointCfg timePoint, Calendar curTime, int direction) {
		// 计算完成前不可重计算calendar，保证字段的一致性
		Calendar baseTriggerTime = HawkTime.getCalendar();
		baseTriggerTime.setLenient(true);

		// 周期及更高字段设为当前时间
		baseTriggerTime.setTimeInMillis(curTime.getTimeInMillis());

		// 最小尺度为分，秒和毫秒设为0
		baseTriggerTime.set(Calendar.MILLISECOND, 0);
		baseTriggerTime.set(Calendar.SECOND, 0);

		// 尺度及更低字段设为配置
		List<Integer> fieldQueue = timePoint.getFieldQueue();
		List<Integer> valueQueue = timePoint.getValueQueue();
		for (int i = 0; i <= fieldQueue.size(); ++i) {
			baseTriggerTime.set(fieldQueue.get(i), valueQueue.get(i));
			if (fieldQueue.get(i) == timePoint.getScaleField()) {
				break;
			}
		}

		// 使用克隆防止重计算，并且避免after/before内部的克隆
		Calendar cloneTime = (Calendar) baseTriggerTime.clone();
		int cycleField = timePoint.getCycleField();

		switch (direction) {
		case PAST:
			// 如果基准触发时间>当前时间，减1个周期
			if (cloneTime.getTimeInMillis() > curTime.getTimeInMillis()) {
				// 固定时间没有周期
				if (cycleField == GsConst.UNUSABLE) {
					return null;
				}
				baseTriggerTime.set(cycleField, cloneTime.get(cycleField) - 1);
			}
			break;

		case FUTURE:
			// 如果基准触发时间<当前时间，加1个周期
			if (cloneTime.getTimeInMillis() < curTime.getTimeInMillis()) {
				// 固定时间没有周期
				if (cycleField == GsConst.UNUSABLE) {
					return null;
				}
				baseTriggerTime.set(cycleField, cloneTime.get(cycleField) + 1);
			}
			break;
		}

		// 重计算calendar
		baseTriggerTime.getTimeInMillis();
		return baseTriggerTime;
	}

}