using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Jint;
using Newtonsoft.Json;
using Jint.Native;

namespace BarrageGrab
{
    public static class JsEngine
    {
        /// <summary>
        /// 创建一个Js引擎
        /// </summary>
        /// <returns></returns>
        public static Engine CreateNewEngine()
        {
            var jsonParse = new Func<object, string>((obj) =>
             {
                 try
                 {
                     return JsonConvert.SerializeObject(JsonConvert.SerializeObject(obj));
                 }
                 catch (Exception ex)
                 {
                     Logger.LogError("JS:" + ex.Message);
                     return null;
                 }
             });

            var engine = new Engine();

            engine.Execute(@"
                byteToUint8Array = function(arr){
                    return new Uint8Array(arr);
                }");

            engine.SetValue("console", new JsConsole());
            engine.SetValue("encoder", new JsEncoder(engine));
            engine.SetValue("bitConvert", new JsBitConvert(engine));

            return engine;
        }

        /// <summary>
        /// 从engine 目录获取js 文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetJsFile(string fileName)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Scripts", "engine", fileName);
            if (!File.Exists(path)) throw new FileNotFoundException("不存在的文件", path);
            return File.ReadAllText(path, Encoding.UTF8);
        }

        class JsConsole
        {
            public void log(object obj)
            {
                if (obj == null) return;
                Logger.PrintColor("JS: " + obj.ToString());
            }

            public void error(object obj)
            {
                if (obj == null) return;
                Logger.LogError("JS: " + obj.ToString());
            }

            public void warn(object obj)
            {
                if (obj == null) return;
                Logger.LogWarn("JS: " + obj.ToString());
            }
        }

        class JsEncoder
        {
            Engine eng;
            public JsEncoder(Engine eng)
            {
                this.eng = eng;               
            }

            private JsValue ByteConvert(byte[] buff)
            {
                return eng.Invoke("byteToUint8Array", buff);
            }

            public JsValue utf8ToBytes(string str)
            {
                byte[] res = new byte[0];
                if (str.IsNullOrWhiteSpace())
                {
                    res = new byte[0];
                }
                else
                {
                    res = Encoding.UTF8.GetBytes(str);
                }
                return ByteConvert(res);
            }

            public JsValue utf32ToBytes(string str)
            {
                byte[] res = new byte[0];
                if (str.IsNullOrWhiteSpace())
                {
                    res = new byte[0];
                }
                else
                {
                    res = Encoding.UTF32.GetBytes(str);
                }
                return ByteConvert(res);
            }

            public JsValue asciiToBytes(string str)
            {
                byte[] res = new byte[0];
                if (str.IsNullOrWhiteSpace())
                {
                    res = new byte[0];
                }
                else
                {
                    res = Encoding.ASCII.GetBytes(str);
                }
                return ByteConvert(res);
            }

            public string utf8ToString(byte[] bytes)
            {
                if (bytes == null) return null;
                return Encoding.UTF8.GetString(bytes);
            }

            public string utf32ToString(byte[] bytes)
            {
                if (bytes == null) return null;
                return Encoding.UTF32.GetString(bytes);
            }

            public string asciiToString(byte[] bytes)
            {
                if (bytes == null) return null;
                return Encoding.ASCII.GetString(bytes);
            }
        }

        class JsBitConvert
        {
            Engine eng;
            public JsBitConvert(Engine eng)
            {
                this.eng = eng;
            }

            private JsValue ByteConvert(byte[] buff)
            {
                return eng.Invoke("byteToUint8Array", buff);
            }

            public JsValue toNumber(byte[] buff,int startIndex = 0)
            {
                return BitConverter.ToDouble(buff, startIndex);
            }
            
            public bool toBoolean(byte[] buff, int startIndex = 0)
            {
                return BitConverter.ToBoolean(buff, startIndex);                
            }

            public string toString(byte[] buff, int startIndex = 0)
            {
                return BitConverter.ToString(buff, startIndex);
            }

            public string toString(byte[] buff, int startIndex, int length)
            {
                return BitConverter.ToString(buff, startIndex, length);                
            }

            public JsValue getBytes(JsValue value)
            {
                byte[] buff = new byte[0];
                switch (value.Type)
                {
                    case Jint.Runtime.Types.Empty:                        
                    case Jint.Runtime.Types.Undefined:
                    case Jint.Runtime.Types.Null:
                        break;
                    case Jint.Runtime.Types.Boolean:
                        buff = BitConverter.GetBytes(value.AsBoolean());
                        break;                    
                    case Jint.Runtime.Types.Number:
                        buff = BitConverter.GetBytes(value.AsNumber());
                        break;                                          
                    case Jint.Runtime.Types.BigInt:
                        buff = BitConverter.GetBytes((long)value.AsNumber());
                        break;                   
                    default:
                        throw new Exception($"不支持该类型转换:" + value.Type);                        
                }

                return ByteConvert(buff);
            }
        }
    }
}
