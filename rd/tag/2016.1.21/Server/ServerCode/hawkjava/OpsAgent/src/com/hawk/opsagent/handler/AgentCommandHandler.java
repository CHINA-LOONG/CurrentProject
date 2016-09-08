package com.hawk.opsagent.handler;

import java.util.List;

import net.sf.json.JSONArray;
import net.sf.json.JSONObject;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;
import org.hawk.os.HawkTime;
import org.hawk.util.HawkCmdParams;
import org.hawk.util.services.helper.HawkOpsServerInfo;

import com.hawk.opsagent.OpsAgentServices;

public class AgentCommandHandler {
	public static String onAgentCommand(String cmd) {
		HawkLog.logPrintln("AgentCommand: " + cmd);
		
		final JSONObject jsonObject = new JSONObject();
		final JSONArray resultArray = new JSONArray();
		jsonObject.put("identify", OpsAgentServices.getInstance().getAgentIdentify());
		jsonObject.put("ip", OpsAgentServices.getInstance().getMyIp());
		try {
			final HawkCmdParams params = HawkCmdParams.valueOf(cmd);
			if (params != null) {
				if (params.cmd.equals("sh") || params.cmd.equals("update")) {
					// 系统命令执行
					String result = SystemCommandHandler.onCommand(params);
					if (result == null || result.length() <= 0) {
						result = "failed";
					}
					
					// 给当前机器每个服相同的返回值
					List<HawkOpsServerInfo> serverInfos = OpsAgentServices.getInstance().getServerInfo(params.getParam("game"), params.getParam("serverids"));
					for (HawkOpsServerInfo serverInfo : serverInfos) {
						JSONObject serverResult = new JSONObject();
						serverResult.put("identify", serverInfo.getIdentify());
						serverResult.put("result", result);
						resultArray.add(serverResult);
					}
				} else if (params.existParam("game")) {
					List<HawkOpsServerInfo> serverInfos = OpsAgentServices.getInstance().getServerInfo(params.getParam("game"), params.getParam("serverids"));
					for (final HawkOpsServerInfo serverInfo : serverInfos) {
						new Thread(new Runnable() {
							@Override
							public void run() {
								String result = ServerCommandHandler.onCommand(serverInfo, params);
								if (result == null || result.length() <= 0) {
									result = "failed";
								}
								// 构建结果集信息
								JSONObject serverResult = new JSONObject();
								serverResult.put("identify", serverInfo.getIdentify());
								serverResult.put("result", result);
								resultArray.add(serverResult);
							}
						}).start();
					}
					
					long timeout = 300000;
					if (params.existParam("timeout")) {
						timeout = Long.valueOf(params.getParam("timeout"));
					}

					// 等待线程执行完毕
					long beginTime = HawkTime.getMillisecond();
					while (resultArray.size() < serverInfos.size()) {
						HawkOSOperator.sleep();
						// 超时
						if (HawkTime.getMillisecond() > beginTime + timeout) {
							break;
						}
					}
				}
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
		jsonObject.put("results", resultArray);
		return jsonObject.toString();
	}
}
