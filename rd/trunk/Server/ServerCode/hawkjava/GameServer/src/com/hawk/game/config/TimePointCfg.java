package com.hawk.game.config;

import java.lang.reflect.Field;
import java.lang.reflect.Modifier;
import java.util.Arrays;
import java.util.Calendar;
import java.util.List;

import org.hawk.config.HawkConfigManager;
import org.hawk.config.HawkConfigBase;
import org.hawk.os.HawkException;
import org.hawk.os.HawkTime;

import com.hawk.game.util.GsConst;
import com.hawk.game.util.TimePointUtil;

@HawkConfigManager.CsvResource(file = "staticData/time.csv", struct = "map")
public class TimePointCfg extends HawkConfigBase {

	@Id
	protected final int type;
	protected final int minute;
	protected final int hour;
	protected final int dayOfWeek;
	protected final int dayOfWeekInMonth;
	protected final int dayOfMonth;
	protected final int month;
	protected final int year;

	// comments
	@SuppressWarnings("unused")
	private final String comments = "";

	// assemble
	// 尺度
	protected int scaleField;
	protected int scaleValue;
	// 周期
	protected int cycleField;
	// 字段列表
	protected List<Integer> fieldQueue;
	protected List<Integer> valueQueue;

	public TimePointCfg() {
		type = 0;
		minute = 0;
		hour = 0;
		dayOfWeek = 0;
		dayOfWeekInMonth = 0;
		dayOfMonth = 0;
		month = 0;
		year = 0;
	}

	@Override
	protected boolean assemble() {
		// “每周几”和“每月几号”最多配一个
		if (dayOfWeek != GsConst.UNUSABLE && dayOfMonth != GsConst.UNUSABLE) {
			return false;
		}
		// "每月第几个星期X“和“每月几号”最多配一个
		if (dayOfWeekInMonth != GsConst.UNUSABLE && dayOfMonth != GsConst.UNUSABLE) {
			return false;
		}

		// 从中国习惯值转为系统值，Calendar.SUNDAY == 1，Calendar.JANUARY ==0
		try {
			if (dayOfWeek != GsConst.UNUSABLE) {
				Field field = this.getClass().getDeclaredField("dayOfWeek");
				field.setAccessible(true);
				final Field modifiersField = Field.class.getDeclaredField("modifiers");
				modifiersField.setAccessible(true);
				modifiersField.setInt(field, field.getModifiers() & ~Modifier.FINAL);

				field.setInt(this, HawkTime.dayOfWeekChinaToGregorian(dayOfWeek));

				modifiersField.setInt(field, field.getModifiers() | Modifier.FINAL);
				modifiersField.setAccessible(false);
				field.setAccessible(false);
			}

			if (month != GsConst.UNUSABLE) {
				Field field = this.getClass().getDeclaredField("month");
				field.setAccessible(true);
				final Field modifiersField = Field.class.getDeclaredField("modifiers");
				modifiersField.setAccessible(true);
				modifiersField.setInt(field, field.getModifiers() & ~Modifier.FINAL);

				field.setInt(this, month - 1);

				modifiersField.setInt(field, field.getModifiers() | Modifier.FINAL);
				modifiersField.setAccessible(false);
				field.setAccessible(false);
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}

		// 默认"每月几号”风格字段队列
		if (dayOfWeek == GsConst.UNUSABLE && dayOfWeekInMonth == GsConst.UNUSABLE) {
			fieldQueue = TimePointUtil.MONTH_STYLE_FIELD_QUEUE;
			valueQueue = Arrays.asList(minute, hour, dayOfMonth, month, year);
		} else {
			fieldQueue = TimePointUtil.WEEK_STYLE_FIELD_QUEUE;
			valueQueue = Arrays.asList(minute, hour, dayOfWeek, dayOfWeekInMonth, month, year);
		}

		// 计算时间尺度
		if (minute == GsConst.UNUSABLE) {
			// 分是最小尺度，不可不配
			return false;

		} else if (hour == GsConst.UNUSABLE) {
			scaleValue = minute;
			scaleField = Calendar.MINUTE;
			cycleField = Calendar.HOUR_OF_DAY;

		} else if (dayOfWeek == GsConst.UNUSABLE && dayOfMonth == GsConst.UNUSABLE) {
			scaleValue = hour;
			scaleField = Calendar.HOUR_OF_DAY;
			if (dayOfWeekInMonth == GsConst.UNUSABLE) {
				cycleField = Calendar.DAY_OF_MONTH;
			} else {
				cycleField = Calendar.DAY_OF_WEEK;
			}

		} else if (dayOfWeek != GsConst.UNUSABLE && dayOfWeekInMonth == GsConst.UNUSABLE) {
			scaleValue = dayOfWeek;
			scaleField = Calendar.DAY_OF_WEEK;
			cycleField = Calendar.DAY_OF_WEEK_IN_MONTH;

		} else if (month == GsConst.UNUSABLE) {
			if (dayOfWeek != GsConst.UNUSABLE) {
				scaleValue = dayOfWeekInMonth;
				scaleField = Calendar.DAY_OF_WEEK_IN_MONTH;
				cycleField = Calendar.MONTH;

			} else if (dayOfMonth != GsConst.UNUSABLE) {
				scaleValue = dayOfMonth;
				scaleField = Calendar.DAY_OF_MONTH;
				cycleField = Calendar.MONTH;
			}

		} else if (year == GsConst.UNUSABLE) {
			scaleValue = month;
			scaleField = Calendar.MONTH;
			cycleField = Calendar.YEAR;

		} else {
			scaleValue = year;
			scaleField = Calendar.YEAR;
			cycleField = GsConst.UNUSABLE; // 年没有周期，仅发生一次
		}

		return true;
	}

	public int getType() {
		return type;
	}

	public int getMinute() {
		return minute;
	}

	public int getHour() {
		return hour;
	}

	public int getDayOfWeek() {
		return dayOfWeek;
	}

	public int getDayOfWeekInMonth() {
		return dayOfWeekInMonth;
	}

	public int getDayOfMonth() {
		return dayOfMonth;
	}

	public int getMonth() {
		return month;
	}

	public int getYear() {
		return year;
	}

	public int getScaleField() {
		return scaleField;
	}

	public int getScaleValue() {
		return scaleValue;
	}

	public int getCycleField() {
		return cycleField;
	}

	public List<Integer> getFieldQueue() {
		return fieldQueue;
	}

	public List<Integer> getValueQueue() {
		return valueQueue;
	}

}
