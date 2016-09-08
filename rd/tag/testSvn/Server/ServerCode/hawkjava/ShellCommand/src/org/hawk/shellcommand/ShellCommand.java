package org.hawk.shellcommand;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;

import org.hawk.log.HawkLog;
import org.hawk.net.protocol.HawkProtocol;

import com.google.gson.JsonObject;

public class ShellCommand {
	public static void main(String[] args) {
		try {
			HawkLog.logPrintln("==============================================");
			
			// 接收输入
			System.out.print("请输入对端服务器IP地址: ");
			BufferedReader br = new BufferedReader(new InputStreamReader(System.in));
			String[] items = br.readLine().split(":");
			String ip = items[0];
			int port = Integer.valueOf(items[1]);
			
			// 连接服务器
			ClientSession session = new ClientSession();
			if (session.connect(ip, port, 1000)) {
				System.out.println("Shell连接建立成功.");
			} else {
				System.out.println("Shell连接建立失败.");
				return;
			}
			
			// Shell操作
			do {
				System.out.print("\r\n>");
				String line = br.readLine().trim();
				if (line.equals("exit")) {
					break;
				}
				
				JsonObject json = new JsonObject();
				json.addProperty("user", "hawk");
				if (line.equals("list_script")) {
					json.addProperty("action", "list_script");
				} else {
					json.addProperty("action", "run_shell");
					json.addProperty("cmd", line);
				}
				
				HawkProtocol protocol = HawkProtocol.valueOf(0, json.toString().getBytes());
				session.request(protocol);
			} while (true);			
			
			session.close();
			System.out.println("Shell连接已关闭.");
			// 随意键盘输入即可退出
			br.read();
		} catch (IOException e) {
			e.printStackTrace();
		}
	}
}
