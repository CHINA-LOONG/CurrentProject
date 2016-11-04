package com.hawk.game.util;

import com.hawk.game.protocol.Status;

/**
 * 文本合法检测与过滤
 * @author walker
 */
public class TextUtil {

	public static int checkNickname(String nickname) {
		if (nickname == GsConst.UNCOMPLETE_NICKNAME) {
			return Status.PlayerError.NICKNAME_INVALID_VALUE;
		}

		return Status.error.NONE_ERROR_VALUE;
	}
}
