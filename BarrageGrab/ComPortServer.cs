using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using Jint;
using Jint.Runtime;
using System.Windows.Forms;
using Jint.Native;

namespace BarrageGrab
{
    /// <summary>
    /// 串口服务
    /// </summary>
    public class ComPortServer : IDisposable
    {
        WsBarrageServer server;
        SerialPort sendPort;
        static Engine jsEngine = null;

        static ComPortServer()
        {
            if (AppSetting.Current.UseComPortFilter)
            {
                jsEngine = JsEngine.CreateNewEngine();
                var jsFile = JsEngine.GetJsFile("comPortFilter.js");
                try
                {
                    jsEngine.Execute(jsFile);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"comPortFilter.js 执行错误: {ex.Message}");
                    throw;
                }                
            }
        }

        public ComPortServer(WsBarrageServer server)
        {
            string portName = AppSetting.Current.ComPort;
            int baudRate = AppSetting.Current.ComBaudRate;
            if (portName.IsNullOrWhiteSpace())
            {
                return;
                //throw new ArgumentNullException(nameof(portName));
            }
            this.server = server;
            server.OnPackMessage += Server_OnPackMessage;
            server.OnClose += Server_OnClose;
            sendPort = new SerialPort(portName, baudRate);
        }

        private void Server_OnClose(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void Server_OnPackMessage(WsBarrageServer sender, WsBarrageServer.PackMsgEventArgs e)
        {
            if (!sendPort.IsOpen) return;
            if (e == null) return;
            if (e.Message == null) return;

            if (!AppSetting.Current.UseComPortFilter)
            {
                var json = e.ToJson() + "\r\n";
                var jbuff = Encoding.UTF8.GetBytes(json);
                Send(jbuff);
                return;
            }

            var roomInfo = AppRuntime.RoomCaches.GetCachedWebRoomInfo(e.Message.RoomId.ToString());
            JsValue result = null;
            try
            {
                result = jsEngine.Invoke("onPackData", e.MsgType.GetHashCode(), e.Message, roomInfo);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"串口过滤器 onPackData 执行错误:{ex.Message}");
                return;                                
            }
            if (result == null) return;

            var nocanTypes = new Types[]
            {
                Types.Null,
                Types.Undefined,
                Types.Empty,
                Types.Symbol
            };
            if (nocanTypes.Contains(result.Type)) return;
            Func<string, byte[]> encode = (str) => Encoding.UTF8.GetBytes(str);

            byte[] buff = new byte[0];
            switch (result.Type)
            {
                case Types.Boolean:
                    buff = encode(result.AsBoolean().ToString().ToLower());
                    break;
                case Types.String:
                    buff = encode(result.AsString());
                    break;
                case Types.BigInt:
                case Types.Number:
                    buff = BitConverter.GetBytes(result.AsNumber());
                    break;
                case Types.Object:
                    if (result.ToObject() is byte[])
                    {
                        buff = result.AsArrayBuffer();
                    }
                    break;
                default:
                    break;
            }

            if (buff.Length == 0) return;
            Send(buff);
        }

        //发送数据包
        private void Send(byte[] buff)
        {
            lock (this.sendPort)
            {
                sendPort.Write(buff, 0, buff.Length);
            }
        }

        /// <summary>
        /// 打开串口并开始接收写入数据
        /// </summary>
        public void OpenStart()
        {
            if (sendPort == null) throw new Exception("未成功初始化串口对象，请检查配置文件");
            try
            {
                sendPort.Open();
            }
            catch (Exception ex)
            {
                sendPort.Close();
                sendPort.Dispose();
                sendPort = null;
                throw ex;
            }
        }

        public void Dispose()
        {
            if (sendPort == null) return;
            sendPort.Close();
            sendPort.Dispose();
            sendPort = null;
            jsEngine.Dispose();
        }
    }
}
