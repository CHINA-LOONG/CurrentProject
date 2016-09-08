/**
 * Created by hawk on 2014/9/13.
 */

// 构造
function HawkProtocol(type, msgBuilder) {
    if (type && msgBuilder) {
        this.type = type;
		try {
			this.msgBuffer = msgBuilder.encode();
		} catch (e) {
			console.log("builder encode failed: " + type + ", exception: " + e);
		}		
        this.size = this.msgBuffer.remaining();
        this.reserve = 0;
        this.crc = HawkProtocolManager.calcCrc(this.msgBuffer);
    } else {
        this.type = 0;
        this.size = 0;
        this.reserve = 0;
        this.crc = 0;
        this.msgBuffer = null;
    }
}

HawkProtocol.prototype.getType = function () {
    return this.type;
}

HawkProtocol.prototype.getSize = function () {
    return this.size;
}

HawkProtocol.prototype.getReserve = function () {
    return this.reserve;
}

HawkProtocol.prototype.getCrc = function () {
    return this.crc;
}

HawkProtocol.prototype.getMsgBuffer = function () {
    return this.msgBuffer;
}

// 协议编码, 返回ByteBuffer
HawkProtocol.prototype.encode = function() {
    try {
        var byteBuffer = new ByteBuffer(1024);
        byteBuffer.writeInt32(this.type);
        byteBuffer.writeInt32(this.size);
        byteBuffer.writeInt32(this.reserve);
        byteBuffer.writeInt32(this.crc);
        byteBuffer.append(this.msgBuffer);
        byteBuffer.flip();
        return byteBuffer;
    } catch (e) {
		console.log("protocol encode failed: " + type + ", exception: " + e);
    }
    return null;
}

// 协议解码
HawkProtocol.prototype.decode = function(byteBuffer) {
    if (byteBuffer.remaining() < HawkProtocolManager.HEADER_SIZE) {
        return false;
    }
    byteBuffer.mark();
    try {
        this.type = byteBuffer.readInt32();
        this.size = byteBuffer.readInt32();
        this.reserve = byteBuffer.readInt32();
        this.crc = byteBuffer.readInt32();
        if (byteBuffer.remaining() >= this.size) {
            // 读协议数据
			this.msgBuffer = new ByteBuffer(this.size);
            if (this.size > 0) {
				byteBuffer.copyTo(this.msgBuffer, 0, byteBuffer.offset, byteBuffer.offset + this.size);			
            }
        } else {
            // 数据包不完全,回滚
            byteBuffer.reset();
            return false;
        }
    } catch (e) {
        // 数据包不完全,回滚
        byteBuffer.reset();
        console.log("exception: " + e);
        return false;
    }

    // crc校验
    var calcCrc = HawkProtocolManager.calcCrc(this.msgBuffer);
    if (calcCrc != this.crc) {
        return false;
    }
    return true;
}
