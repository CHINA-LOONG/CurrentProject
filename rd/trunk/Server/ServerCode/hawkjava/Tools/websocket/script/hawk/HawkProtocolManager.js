/**
 * Created by hawk on 2014/9/13.
 */

// 定义全局对象
var ProtoBuf = dcodeIO.ProtoBuf;
var ByteBuffer = dcodeIO.ByteBuffer;

// 协议操作封装
var HawkProtocolManager = {}

// 文件builder表
HawkProtocolManager.builderMap = {};

// 协议回调注册表
HawkProtocolManager.handlerMap = {};

// 协议头大小
HawkProtocolManager.HEADER_SIZE = 16;

// 协议注册(简短文件名格式: test.proto)
HawkProtocolManager.registerProtocol = function(protocolFile) {
    try {        
        var builder = ProtoBuf.loadProtoFile(protocolFile);
        if (!builder) {
            console.log("load proto file failed: " + protocolFile);
            return null;
        }

		var pathArray = protocolFile.split("/");
		var simpleName = protocolFile;
		if (pathArray && pathArray.length > 0) {
			simpleName = pathArray[pathArray.length-1];
		}
        var protocolName = simpleName.split(".")[0];
        HawkProtocolManager.builderMap[protocolName] = builder;
        console.log("load proto file ok: " + protocolFile);
        return builder;
    } catch (e) {
        console.log("exception: " + e);
    }
    return null;
}

// 获取pb的message类
HawkProtocolManager.getBuilderClass = function(messageName) {
    var nameArray = messageName.split(".");
    var builder = HawkProtocolManager.builderMap[nameArray[0]];
    if (builder != null) {
        return builder.build(nameArray[1]);
    }
    return null;
}

// 生成message对象
HawkProtocolManager.newBuilder = function(messageName) {
    var builder = HawkProtocolManager.getBuilderClass(messageName);
    if (builder != null) {
        return new builder();
    }
    return null;
}

// 注册协议处理器
HawkProtocolManager.registerHandler = function (hpCode, messageName, handler, caller) {
    var handlerInfo = HawkProtocolManager.handlerMap[hpCode];
    if (handlerInfo) {
        console.log("protocol handler is exist: " + hpCode + ", message: " + messageName);
        return false;
    }
    HawkProtocolManager.handlerMap[hpCode] = {};
    HawkProtocolManager.handlerMap[hpCode]["messageName"] = messageName;
    HawkProtocolManager.handlerMap[hpCode]["handler"] = handler;
	HawkProtocolManager.handlerMap[hpCode]["caller"] = caller;
    return true;
}

// 处理协议
HawkProtocolManager.handlerProtocol = function (protocol) {
    try {
        var handlerInfo = HawkProtocolManager.handlerMap[protocol.getType()];
        if (!handlerInfo) {
            console.log("protocol handler is null: " + protocol.getType());
            return false;
        }

        var builderClass = HawkProtocolManager.getBuilderClass(handlerInfo["messageName"]);
        if (!builderClass) {
            console.log("protocol class is null: " + protocol.getType());
            return false;
        }

        var msgBuilder = null;
		try {
			msgBuilder = builderClass.decode(protocol.getMsgBuffer());
		} catch (e) {
			console.log("protocol decode failed: " + protocol.getType() + ", exception: " + e);
		}
		
        if (msgBuilder) {
			console.log("recv protocol: " + protocol.getType() + ", msg: " + msgBuilder.toString());
			
            var handler = handlerInfo["handler"];
			var caller = handlerInfo["caller"];
            if (!handler) {
                console.log("protocol handler is null: " + protocol.getType());
                return false;
            }
			
			if (caller) {
				handler.apply(caller, [protocol.getType(), msgBuilder]);
			} else {
				handler(protocol.getType(), msgBuilder);
			}
			
            return true;
        } else {
            console.log("protocol decode failed: " + protocol.getType());
        }
    } catch (e) {
        console.log("exception: " + e);
    }
    return false;
}

// 计算crc
HawkProtocolManager.calcCrc = function(byteBuffer) {
    var hash = 0;
	if (byteBuffer) {
		var offset = byteBuffer.offset;
		for (var i = byteBuffer.offset; i < byteBuffer.limit; i++) {
			var value = byteBuffer.readByte();
			hash ^= ((i & 1) == 0) ? ((hash << 7) ^ (value & 0xff) ^ (hash >>> 3)) : (~((hash << 11) ^ (value & 0xff) ^ (hash >>> 5)));
		}
		byteBuffer.offset = offset;
	}
    return hash;
}