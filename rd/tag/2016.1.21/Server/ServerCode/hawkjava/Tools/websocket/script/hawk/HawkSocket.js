/**
 * Created by hawk on 2014/9/11.
 */

// 构造
function HawkSocket() {
    this.url = "";
    this.socket = null;
	this.open_cb = null;
	this.message_cb = null;
	this.close_cb = null;
}

// 获取连接地址
HawkSocket.prototype.getUrl = function() {
	return this.url;
}

// 是否为激活
HawkSocket.prototype.isAlive = function () {
    return this.socket.readyState == WebSocket.OPEN;
}

// Socket打开回调
HawkSocket.prototype.onOpen = function (caller) {
	if (typeof(eval(this.open_cb)) == "function") {
		this.open_cb.apply(caller);
	}
}

// Socket接收数据
HawkSocket.prototype.onMessage = function (data, caller) {
	if (typeof(eval(this.message_cb)) == "function") {
		this.message_cb.apply(caller, data);
	}
}

// Socket断开回调
HawkSocket.prototype.onClose = function (caller) {
	if (typeof(eval(this.close_cb)) == "function") {
		this.close_cb.apply(caller);
	}
}

// 连接
HawkSocket.prototype.connect = function (url, open_cb, msg_cb, close_cb, caller) {
    var that = this;
    this.url = url;
	this.open_cb = open_cb;
	this.msg_cb = msg_cb;
	this.close_cb = close_cb;
    this.socket = new WebSocket(url, "org.hawk.protocol");

    // 注册函数
    this.socket.onopen = function (event) {
        console.log("session opened: " + that.url);
		// 回调通知
        that.onOpen(caller);
    }

    // 监听消息
    this.socket.onmessage = function (event) {
        try {
            var reader = new FileReader();
            reader.onload = function(evt){
                if(evt.target.readyState == FileReader.DONE){
                    var data = new Uint8Array(evt.target.result);
					that.onMessage(data, caller);
                    var byteBuffer = ByteBuffer.wrap(data);					
                    var protocol = new HawkProtocol();
                    if (protocol.decode(byteBuffer)) {
                        HawkProtocolManager.handlerProtocol(protocol);
                    } else {
                        console.log("protocol decode failed");
                    }
                }
            }
            reader.readAsArrayBuffer(event.data);
        } catch (e) {
            console.log("exception: " + e);
        }
    };

    // 监听Socket的关闭
    this.socket.onclose = function (event) {
        console.log("session closed: " + that.url);
        that.onClose(caller);
    };

    // Socket出错
    this.socket.onerror = function(event) {
		console.log("session error: " + that.url);
        // that.onClose(caller, event);
    };
}

// socket重连
HawkSocket.prototype.reconnect = function () {
    if (this.socket == null || this.socket.readyState != WebSocket.OPEN) {
        this.connect(this.url);
    }
}

// 发送协议
HawkSocket.prototype.sendProtocol = function (hpCode, msgBuilder) {
    if (this.socket.readyState == WebSocket.OPEN) {
        try {
            var protocol = new HawkProtocol(hpCode, msgBuilder);
            var byteBuffer = protocol.encode();
            if (byteBuffer != null) {
                this.socket.send(byteBuffer.toBuffer());
                return true;
            }
        } catch (e) {
            console.log("exception: " + e);
        }
    }
    return false;
}
