var HawkDebug = HawkDebug || {}

HawkDebug.log = function () {
    if (window.navigator.appVersion.indexOf("Win") < 0) {
        return;
    } else {
        var debugPanel = document.getElementById('debugPanel');
        debugPanel.style.display = "";
    }
    var args = Array.prototype.slice.call(arguments);
    if (args.length > 1) {
        var i = 1, hasstyle = false;
        if (args[0].indexOf("%c") == 0) {
            args[0] = args[0].replace(/%c/, "");
            i = 2;
            hasstyle = true;
        }
        for (; i < args.length; i++) {
            if (/%s|%d|%i|%o/.test(args[0])) {
                args[0] = args[0].replace(/%s|%d|%i|%o/, args[i]);
            } else {
                break;
            }
        }
        if (i < args.length) {
            args[0] = args[0] + " " + args.slice(i).join(" ");
        }
        if (hasstyle) {
            this.showLog(args[0], args[1]);
        } else {
            this.showLog(args[0]);
        }
    } else if (args.length == 1) {
        if (arguments[0] instanceof Array) {
            this.showLog("[" + args[0] + "]");
        } else {
            this.showLog(args[0]);
        }
    } else {
        this.showLog("");
    }
}

HawkDebug.showLog = function (log) {
    if (document.getElementById("logPanel")) {
        var div = document.createElement("div");
        var oText = document.createTextNode(log);
        div.appendChild(oText);
        document.getElementById("logPanel").appendChild(div);
    }
};

window.onload = function () {
    var debugPanel = document.getElementById('debugPanel');
    var headPanel = document.getElementById('headPanel');
    var disX = 0, disY = 0;
    headPanel.onmousedown = function (ev) {
        var oEvent = ev || event;
        disX = oEvent.clientX - debugPanel.offsetLeft;
        disY = oEvent.clientY - debugPanel.offsetTop;
        document.onmousemove = function (ev) {
            var oEvent = ev || event;
            var l = oEvent.clientX - disX;
            var t = oEvent.clientY - disY;
            if (l < 0) {
                l = 0;
            } else if (l > document.documentElement.clientWidth - debugPanel.offsetWidth) {
                l = document.documentElement.clientWidth - debugPanel.offsetWidth;
            }
            if (t < 0) {
                t = 0;
            } else if (t > document.documentElement.clientHeight - debugPanel.offsetHeight) {
                t = document.documentElement.clientHeight - debugPanel.offsetHeight;
            }
            debugPanel.style.left = l + 'px';
            debugPanel.style.top = t + 'px';
        }
        document.onmouseup = function () {
            document.onmousemove = null;
            document.onmouseup = null;
        }
        return false;
    }
}

HawkDebug.log("screenWidth："+ window.screen.width);
HawkDebug.log("screenHeight："+ window.screen.height);
HawkDebug.log("windowWidth："+ window.innerWidth);
HawkDebug.log("windowHeight："+ window.innerHeight);

function onSessionOpen() {
	HawkDebug.log("session open.");
}

function onSessionMsg() {
	HawkDebug.log("session msg.");
}

function onSessionClose() {
	HawkDebug.log("session close.");
}

// 注册协议
HawkProtocolManager.registerProtocol("protocol/JsProto.jspb");

HawkProtocolManager.registerHandler(1001, "JsProto.HPLoginRet", 
	function(hpCode, msgBuilder) {
		HawkDebug.log("protocol: " + hpCode);
		HawkDebug.log(msgBuilder.toString());
	});
	
HawkProtocolManager.registerHandler(102, "JsProto.HPPlayerInfoSync", 
	function(hpCode, msgBuilder) {
		HawkDebug.log("protocol: " + hpCode);
		HawkDebug.log(msgBuilder.toString());
	});

// 创建套接字
var socket = new HawkSocket();
socket.connect("ws://127.0.0.1:9595", onSessionOpen, onSessionMsg, onSessionClose, null);

function sendProtocol() {
    var msg = HawkProtocolManager.newBuilder("JsProto.HPLogin");
    msg.setPuid("" + parseInt(Math.random() * 100000));
    msg.setDeviceId("123-456-789");
    socket.sendProtocol(1000, msg);
}