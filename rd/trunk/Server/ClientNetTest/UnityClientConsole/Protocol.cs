using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ProtoBuf;
using ProtoBuf.Meta;

using System.Net;
using System.Net.Sockets;

namespace UnityClientConsole
{
    class Protocol
    {
        public const int HEAD_SIZE = 16;

        public class ProtocolHeader {
		/**
		 * 类型
		 */
		public int type;
		/**
		 * 字节数
		 */
		public int size;
		/**
		 * 保留字段
		 */
		public int reserve;
		/**
		 * 校验码
		 */
		public int crc;


		public ProtocolHeader(int type) {
			this.type = type;
		}

		public void clear() {
			type = 0;
			size = 0;
			reserve = 0;
			crc = 0;
		}
		
		public String toString() {
			return String.Format("[type: %d, size: %d, reserve: %d, crc: %d]", type, size, reserve, crc);
		}
	}

        private ProtocolHeader header;
        private MemoryStream octets;

	    protected Protocol() 
        {
		    header = new ProtocolHeader(0);
	    }

        protected Protocol(int type)
        {
		    header = new ProtocolHeader(type);
	    }

        public bool checkType(int type)
        {
            if (header != null)
            {
                return header.type == type;
            }
            return false;
        }

        public T GetProtocolBody<T>()
        {
            try
            {
                octets.Position = 0;
                T body = Serializer.Deserialize<T>(octets);
                return body;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return default(T);
        }

        public static Protocol valueOf()
        {
            return new Protocol();
        }

        public static Protocol valueOf(int type)
        {
            Protocol protocol = valueOf();
            protocol.setType(type);
            return protocol;
        }

        public static Protocol valueOf(int type, ProtoBuf.IExtensible builder)
        {
            Protocol protocol = valueOf(type);
            try
            {
                protocol.writeBuilder(builder);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            return protocol;
        }

        public Protocol setType(int type)
        {
            if (header != null)
            {
                header.type = type;
            }
            return this;
        }

        public void calcOctets()
        {
            if (octets != null)
            {
                header.size = (int)octets.Length;
                header.crc  = 0;
            }
        }

        public bool writeBuilder(ProtoBuf.IExtensible builder)
        {
            if (builder == null)
            {
                return true;
            }

            try
            {
               if (octets == null)
               {
                    octets = new MemoryStream();
               }
                
               octets.SetLength(0);
               Serializer.Serialize(octets, builder);

               calcOctets();
               return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

        public bool encode(MemoryStream buffer)
        {
            // 协议编码
            try
            {
                // 有效协议头
                buffer.Write(System.BitConverter.GetBytes(IPAddress.HostToNetworkOrder(header.type)), 0, sizeof(int));
                buffer.Write(System.BitConverter.GetBytes(IPAddress.HostToNetworkOrder(header.size)), 0, sizeof(int));
                buffer.Write(System.BitConverter.GetBytes(IPAddress.HostToNetworkOrder(header.reserve)), 0, sizeof(int));
                buffer.Write(System.BitConverter.GetBytes(IPAddress.HostToNetworkOrder(header.crc)), 0, sizeof(int));

                if (header.size > 0)
                {
                    octets.WriteTo(buffer);
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

	    public bool decode(MemoryStream buffer) {
		    
		    if (buffer.Length < HEAD_SIZE) {
		    	return false;
		    }

		    header.clear();
		    int crc = 0;

		    try {

                buffer.Position = 0;
                Byte[] tempBuffer = new Byte[sizeof(int)];
                buffer.Read(tempBuffer, 0, sizeof(int));
                header.type = IPAddress.NetworkToHostOrder(System.BitConverter.ToInt32(tempBuffer, 0));
                buffer.Read(tempBuffer, 0, sizeof(int));
                header.size = IPAddress.NetworkToHostOrder(System.BitConverter.ToInt32(tempBuffer, 0));
                buffer.Read(tempBuffer, 0, sizeof(int));
                header.reserve = IPAddress.NetworkToHostOrder(System.BitConverter.ToInt32(tempBuffer, 0));
                buffer.Read(tempBuffer, 0, sizeof(int));
                header.crc = IPAddress.NetworkToHostOrder(System.BitConverter.ToInt32(tempBuffer, 0));
		    	
		    	if ((buffer.Length - buffer.Position) >= header.size) {
  		
		    		if (header.size >= 0) {

                        if (octets == null)
                        {
                            octets = new MemoryStream(header.size);
                        }

                        octets.SetLength(0);
                        octets.Write(buffer.GetBuffer(), (int)buffer.Position, header.size);
                        crc = calcCrc(octets.ToArray(), 0, (int)octets.Length, 0);
                        if (header.reserve == 1)
                        {
                            byte[] result = null;
                            ZlibUtil.DecompressData(octets.ToArray(), out result);
                            octets.SetLength(0);
                            octets.Write(result, 0, result.Length);
                        }

                        Buffer.BlockCopy(buffer.GetBuffer(), HEAD_SIZE + header.size, buffer.GetBuffer(), 0, (int)(buffer.Length - HEAD_SIZE - header.size));                
                        buffer.SetLength(buffer.Length - header.size - HEAD_SIZE); 
		    		}
		    	}
                else{
                    return false;
		    	}
		    } catch (Exception e) {
		    	
		    	// 捕获异常
                Console.WriteLine(e.Message);
                return false;
            }
            finally
            {
                buffer.Seek(0, SeekOrigin.Begin);
            }

            // crc校验
            if (crc != header.crc)
            {
                return false;
            }

		    return true;
	    }


        public static int calcCrc(byte[] bytes, int offset, int size, int crc)
        {
            int hash = crc;
            for (int i = offset; i < offset + size; i++)
            {
                hash ^= ((i & 1) == 0) ? ((hash << 7) ^ (bytes[i] & 0xff) ^ (MoveByte(hash, 3))) : (~((hash << 11) ^ (bytes[i] & 0xff) ^ (MoveByte(hash, 5))));
            }

            return hash;
        }

        public static int MoveByte(int value, int pos)
        {
            if (value < 0)
            {
                string s = Convert.ToString(value, 2);
                for (int i = 0; i < pos; i++)
                {
                    s = "0" + s.Substring(0, 31);
                }
                return Convert.ToInt32(s, 2);
            }
            else
            {
                return value >> pos;
            }
        }
    }
}
