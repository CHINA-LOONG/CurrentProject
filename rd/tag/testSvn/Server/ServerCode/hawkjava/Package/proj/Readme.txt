version:
	zeromq: 4.0.4
	jzmq:   https://github.com/zeromq/jzmq

changes:
	jzmq:   1. Socket.cpp添加Java_org_zeromq_ZMQ_00024Socket_monitor函数, 让java能使用zmq的monitor功能
			2. ZMQ.java添加 public native boolean monitor(String addr, int events);
	