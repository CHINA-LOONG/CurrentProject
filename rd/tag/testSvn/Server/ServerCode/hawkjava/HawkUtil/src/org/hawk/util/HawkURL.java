package org.hawk.util;

import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

/**
 * URL encode
 * 
 * @author walker
 */
public class HawkURL {

	/**
	 * Encode a string for use in the path of a URL; uses URLEncoder.encode,
	 * (which encodes a string for use in the query portion of a URL), then
	 * applies some postfilters to fix things up per the RFC. Can optionally
	 * handle strings which are meant to encode a path (ie include '/'es
	 * which should NOT be escaped).
	 *
	 * @param value the value to encode
	 * @param path true if the value is intended to represent a path
	 * @return the encoded value
	 */
	public static final String urlEncodeRFC3986(final String value, final boolean isPath) {
		if (value == null) {
			return "";
		}

		try {
			String encoded = URLEncoder.encode(value, "UTF-8");

			Matcher matcher = ENCODED_CHARACTERS_PATTERN_RFC3986.matcher(encoded);
			StringBuffer buffer = new StringBuffer(encoded.length());

			while (matcher.find()) {
				String replacement = matcher.group(0);

				if ("+".equals(replacement)) {
					replacement = "%20";
				} else if ("*".equals(replacement)) {
					replacement = "%2A";
				} else if ("%7E".equals(replacement)) {
					replacement = "~";
				} else if (isPath && "%2F".equals(replacement)) {
					replacement = "/";
				}

				matcher.appendReplacement(buffer, replacement);
			}

			matcher.appendTail(buffer);
			return buffer.toString();

		} catch (UnsupportedEncodingException e) {
			throw new RuntimeException(e);
		}
	}

	/**
	 * Regex which matches any of the sequences that should be fixed up after
	 * URLEncoder.encode() based on rfc3986.
	 */
	private static final Pattern ENCODED_CHARACTERS_PATTERN_RFC3986;
	static {
		StringBuilder pattern = new StringBuilder();

		pattern
			.append(Pattern.quote("+"))
			.append("|")
			.append(Pattern.quote("*"))
			.append("|")
			.append(Pattern.quote("%7E"))
			.append("|")
			.append(Pattern.quote("%2F"));

		ENCODED_CHARACTERS_PATTERN_RFC3986 = Pattern.compile(pattern.toString());
	}

}
