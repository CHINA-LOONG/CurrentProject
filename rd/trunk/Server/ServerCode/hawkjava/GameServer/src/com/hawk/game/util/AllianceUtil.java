package com.hawk.game.util;

import java.util.Comparator;
import java.util.HashMap;
import java.util.Map;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.hawk.config.HawkConfigManager;

import com.hawk.game.config.AllianceCfg;
import com.hawk.game.entity.AllianceEntity;

public class AllianceUtil {
	/**
	 * 检测公会名称是否合法
	 * @param name
	 * @return
	 */
	public static boolean checkName(String name) {  
		String regEx = "^[0-9A-Za-z\u4e00-\u9fa5]{2,6}$";  
		Pattern pat = Pattern.compile(regEx);  
		Matcher mat = pat.matcher(name);  
		return mat.find();     
    }
	
	/**
	 * 公会排行比较器
	 */
	public final static Comparator<AllianceEntity> SORTALLIANCE = new Comparator<AllianceEntity>() {
		@Override
		public int compare(AllianceEntity o1, AllianceEntity o2) {
			if(o1.getId() == o2.getId()) return 0;
			if (o1.getLevel() < o2.getLevel()) {
				return 1;
			} else if (o1.getLevel() == o2.getLevel()) {
				if (o1.getExp() == o2.getExp()) {
					if (o1.getId() > o2.getId())
						return 1;
					else 
						return -1;
				}
				if (o1.getExp() < o2.getExp()) {
					return 1;
				} else {
					return -1;
				}
			} else {
				return -1;
			}
		}
	};
}
