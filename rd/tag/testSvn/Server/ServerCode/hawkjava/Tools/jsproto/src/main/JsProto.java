package main;

import java.io.BufferedInputStream;
import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.FilenameFilter;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.util.LinkedList;
import java.util.List;
import java.util.zip.ZipEntry;
import java.util.zip.ZipOutputStream;

import org.apache.oro.text.regex.MatchResult;
import org.apache.oro.text.regex.Pattern;
import org.apache.oro.text.regex.PatternCompiler;
import org.apache.oro.text.regex.PatternMatcher;
import org.apache.oro.text.regex.PatternMatcherInput;
import org.apache.oro.text.regex.Perl5Compiler;
import org.apache.oro.text.regex.Perl5Matcher;

public class JsProto {
	private static String workPath = System.getProperty("user.dir");
	private static List<String> absolutePathList = new LinkedList<>();

	private static String protoConstTemp = "var ProtoConst = {\n" + "%s" + "\n}";

	private static List<String> protoConstContent = new LinkedList<>();

	final static void showAllFiles(File dir, final String filter) throws Exception {
		FilenameFilter filenameFilter = new FilenameFilter() {
			@Override
			public boolean accept(File file, String fileName) {
				if (fileName.indexOf(filter) > 0) {
					return true;
				}
				return false;
			}
		};

		File[] fs = dir.listFiles(filenameFilter);
		for (int i = 0; i < fs.length; i++) {
			if (fs[i].isDirectory()) {
				try {
					showAllFiles(fs[i], filter);
				} catch (Exception e) {
					e.printStackTrace();
				}
			} else {
				System.out.println("[" + (i + 1) + "]" + "find proto file: " + fs[i].getAbsolutePath());
				absolutePathList.add(fs[i].getAbsolutePath());
			}
		}
	}

	private static String packetProtoFile(String filePath) {
		try {
			StringBuilder builder = new StringBuilder(16 * 1024);
			String line = null;
			String total = "";
			BufferedReader reader = new BufferedReader(new InputStreamReader(new FileInputStream(filePath), "UTF-8"));
			boolean segmentDisable = false;
			while ((line = reader.readLine()) != null) {
				total += line + "\n";
				if (segmentDisable) {
					if (line.indexOf("*/") >= 0) {
						segmentDisable = false;
					}
					continue;
				} else if (line.indexOf("/*") >= 0) {
					segmentDisable = true;
					if (line.indexOf("*/") >= 0) {
						segmentDisable = false;
					}
					continue;
				}

				line = line.replace("\r", "").replace("\n", "").replace("\t", " ").trim();
				while (line.indexOf("  ") >= 0) {
					line = line.replace("  ", " ");
				}
				line = line.replace(" = ", "=");
				line = line.replace(" =",  "=");
				line = line.replace("= ",  "=");
				line = line.trim();
				
				if (line.length() <= 0 || line.indexOf("java_package") > 0 || line.indexOf("import ") >= 0 || line.startsWith("//")) {
					continue;
				}

				int pos = line.indexOf("//");
				if (pos >= 0) {
					line = line.substring(0, pos);
				}
				line = line.trim();
				
				if (line.length() > 0) {
					builder.append(line);
				}
			}
			reader.close();
			reader.close();

			String regEx = "enum[^}]*}";
			PatternCompiler compiler = new Perl5Compiler();
			Pattern pattern = null;
			try {
				pattern = compiler.compile(regEx, Perl5Compiler.MULTILINE_MASK);
				PatternMatcher matcher = new Perl5Matcher();
				PatternMatcherInput input = new PatternMatcherInput(total);
				while (matcher.contains(input, pattern)) {
					MatchResult result = matcher.getMatch();

					String value = result.toString();
					System.out.println(value);
					protoConstContent.add(dealWith(value));
				}
			} catch (Exception e) {
				e.printStackTrace();
			}

			if (builder.length() > 0) {
				builder.append("\r\n");
				return builder.toString();
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
		return "";
	}

	private static String dealWith(String value) {
		value = value.replace("enum ", "");
		String regEx = ";[^;}]*}";
		PatternCompiler compiler = new Perl5Compiler();
		Pattern pattern = null;
		try {
			pattern = compiler.compile(regEx, Perl5Compiler.MULTILINE_MASK);
			PatternMatcher matcher = new Perl5Matcher();
			PatternMatcherInput input = new PatternMatcherInput(value);
			while (matcher.contains(input, pattern)) {
				MatchResult result = matcher.getMatch();
				String v = result.toString();
				value = value.replace(v, "\n}");
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
		value = value.replace("\n{", ":{");
		value = value.replace("=", ":");
		value = value.replace(";", ",");
		return value;
	}

	private static void zip(String zipFileName, String srcFileName) {
		try {
			// 压缩文件名
			File zipFile = new File(zipFileName);
			ZipOutputStream zos = new ZipOutputStream(new FileOutputStream(zipFile));

			File srcFile = new File(srcFileName);
			ZipEntry entry = null;
			entry = new ZipEntry(srcFile.getName());
			entry.setSize(srcFile.length());
			entry.setTime(srcFile.lastModified());
			zos.putNextEntry(entry);

			InputStream is = new BufferedInputStream(new FileInputStream(srcFile));
			int readLen = -1;
			byte[] buf = new byte[4096];
			while ((readLen = is.read(buf, 0, 4096)) != -1) {
				zos.write(buf, 0, readLen);
			}
			is.close();
			zos.close();
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	public static void main(String[] args) {
		if (args.length > 0) {
			workPath = args[0];
			if (!workPath.endsWith("/") && !workPath.endsWith("\\")) {
				workPath += File.separator;
			}
		}

		// 路径
		File srcFile = new File(workPath);
		if (!srcFile.exists()) {
			System.err.println("workpath not exist: " + workPath);
			return;
		}

		// 是否为目录
		if (!srcFile.isDirectory()) {
			System.err.println("workpath not be directory: " + workPath);
			return;
		}

		System.out.println("workpath: " + workPath);

		// 遍历
		try {
			showAllFiles(srcFile, ".proto");
		} catch (Exception e) {
			e.printStackTrace();
		}

		try {
			FileOutputStream fos = new FileOutputStream(new File(workPath + "/JsProto.jspb"));
			try {
				for (int i = 0; i < absolutePathList.size(); i++) {
					String filePath = absolutePathList.get(i);
					System.out.println("[" + (i + 1) + "]" + "packet proto file: " + filePath);

					String packetData = packetProtoFile(filePath);
					if (packetData.length() > 0) {
						fos.write(packetData.getBytes());
					}
				}
			} finally {
				fos.close();
			}

			// 压缩为zip格式
			zip(workPath + "JsProto.jspb.zip", workPath + "/JsProto.jspb");
			System.out.println("compress successful: " + workPath + "/JsProto.jspb.zip");

			// 常量定义过滤
			String constDef = "";
			for (int i = 0; i < protoConstContent.size(); i++) {
				if (i != protoConstContent.size() - 1) {
					constDef += protoConstContent.get(i) + ",\n\n";
				} else {
					constDef += protoConstContent.get(i) + "\n";
				}
			}
			write(String.format(protoConstTemp, constDef), workPath + "/ProtoConst.js", "UTF-8");

			// 提示退出
			System.out.println("press any key to exit.");
			System.in.read();
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	public static void write(String fileContent, String fileName, String encoding) {
		OutputStreamWriter osw = null;
		try {
			osw = new OutputStreamWriter(new FileOutputStream(fileName), encoding);
			osw.write(fileContent);
			osw.flush();
		} catch (Exception e) {
			e.printStackTrace();
		} finally {
			if (osw != null) {
				try {
					osw.close();
				} catch (IOException e) {
					e.printStackTrace();
				}
			}
		}
	}
}
