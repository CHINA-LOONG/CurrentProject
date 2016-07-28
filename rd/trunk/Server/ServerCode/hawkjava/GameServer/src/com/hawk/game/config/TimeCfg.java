package com.hawk.game.config;

import java.lang.reflect.Field;
import java.lang.reflect.Modifier;
import java.util.Calendar;

import org.hawk.config.HawkConfigManager;
import org.hawk.config.HawkConfigBase;
import org.hawk.os.HawkException;
import org.hawk.os.HawkTime;

@HawkConfigManager.CsvResource(file = "staticData/time.csv", struct = "map")
public class TimeCfg extends HawkConfigBase {

	public static final int NO_VALUE = -1;

	@Id
	protected final int type;
	protected final int minute;
	protected final int hour;
	protected final int dayOfWeek;
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

	public TimeCfg() {
		type = 0;
		minute = 0;
		hour = 0;
		dayOfWeek = 0;
		dayOfMonth = 0;
		month = 0;
		year = 0;
	}

	@Override
	protected boolean assemble() {
//		if (type <= GsConst.RefreshType.GLOBAL_REFRESH_BEGIN ||
//			type >= GsConst.RefreshType.GLOBAL_REFRESH_END && type <= GsConst.RefreshType.PERS_REFRESH_BEGIN ||
//			type >= GsConst.RefreshType.PERS_REFRESH_END) {
//			return false;
//		}

		// “每周几”和“每月几号”最多配一个
		if (dayOfWeek != NO_VALUE  && dayOfMonth != NO_VALUE) {
			return false;
		}

		// 从中国习惯值转为系统值，Calendar.SUNDAY == 1，Calendar.JANUARY ==0
		try {
			if (dayOfWeek != NO_VALUE) {
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

			if (month != NO_VALUE) {
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

		// 计算时间尺度
		if (minute == NO_VALUE) {
			// 分是最小尺度，不可不配
			return false;

		} else if (hour == NO_VALUE) {
			scaleField = Calendar.MINUTE;
			cycleField = Calendar.HOUR_OF_DAY;
			scaleValue = minute;

		} else if (dayOfWeek == NO_VALUE && dayOfMonth == NO_VALUE) {
			scaleField = Calendar.HOUR_OF_DAY;
			cycleField = Calendar.DAY_OF_YEAR;
			scaleValue = hour;

		} else if (month == NO_VALUE) {
			if (dayOfWeek != NO_VALUE) {
				scaleField = Calendar.DAY_OF_WEEK;
				cycleField = Calendar.DAY_OF_WEEK_IN_MONTH;
				scaleValue = dayOfWeek;

			} else if (dayOfMonth != NO_VALUE) {
				scaleField = Calendar.DAY_OF_MONTH;
				cycleField = Calendar.MONTH;
				scaleValue = dayOfMonth;
			}

		} else if (year == NO_VALUE) {
			scaleField = Calendar.MONTH;
			cycleField = Calendar.YEAR;
			scaleValue = month;

		} else {
			scaleField = Calendar.YEAR;
			cycleField = NO_VALUE; // 年没有周期，仅发生一次
			scaleValue = year;
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
}
