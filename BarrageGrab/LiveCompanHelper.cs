using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using IWshRuntimeLibrary;
using File = System.IO.File;

namespace BarrageGrab
{
    public static class LiveCompanHelper
    {
        /// <summary>
        /// 获取抖音直播伴侣的exe路径
        /// </summary>
        /// <returns></returns>
        public static string GetExePath()
        {
            string exePath = "";
            //获取exe所在目录
            //1.先在开始菜单找
            var shortcutPath = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\直播伴侣.lnk";
            if (File.Exists(shortcutPath))
            {
                //获取快捷方式的目标路径
                exePath = GetInkTargetPath(shortcutPath);
            }

            return exePath;
        }

        /// <summary>
        /// 初始化直播伴侣环境
        /// </summary>
        public static void SwitchSetup()
        {
            var exePath = GetExePath();
            if (string.IsNullOrEmpty(exePath))
            {
                Logger.LogWarn("未找到直播伴侣的exe文件，跳过环境设置");
                return;
            }

            //设置index.js
            var indexJsPath = Path.Combine(Path.GetDirectoryName(exePath), "resources", "app", "index.js");
            Logger.LogInfo($"正在配置 " + indexJsPath);

            if (!File.Exists(indexJsPath))
            {
                throw new Exception("未找到直播伴侣的index.js文件");
            }
            var indexJs = File.ReadAllText(indexJsPath, Encoding.UTF8);

            CheckBackFile(indexJsPath);
            var newjs = SetIndexJsContent(indexJs);
            if (newjs != indexJs && !newjs.IsNullOrWhiteSpace())
            {
                File.WriteAllText(indexJsPath, newjs);
            }

            Logger.LogInfo("直播伴侣环境初始化完成");
        }

        //获取Ink快捷方式的目标路径
        private static string GetInkTargetPath(string shortcutPath)
        {
            // 创建 Windows Script Host 对象
            WshShell shell = new WshShell();

            // 使用 IWshShortcut 接口加载 .ink 文件
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

            // 获取目标路径
            string targetPath = shortcut.TargetPath;

            return targetPath;
        }

        //检测备份文件
        private static void CheckBackFile(string filePath)
        {
            if (!File.Exists(filePath)) return;

            var bakPath = filePath + ".bak";
            if (!File.Exists(bakPath))
            {
                //拷贝一份备份
                File.Copy(filePath, bakPath);
                Logger.LogInfo($"已备份 {filePath} -> {bakPath}");
            }
        }

        //设置index.js内容
        private static string SetIndexJsContent(string content)
        {
            //检测注入代理
            var mineProxyHost = $"127.0.0.1:{AppSetting.Current.ProxyPort},direct://";
            SetSwitch("proxy-server", mineProxyHost, ref content);

            //移除文件损坏检测校验
            var checkReg = new Regex(@"if\(\(0,\w.integrityCheckReport\)\(\w\),!\w\.ok\)");
            if (checkReg.IsMatch(content))
            {
                content = checkReg.Replace(content, "if(false)");
                Logger.LogInfo($"直播伴侣文件改动检测已拦截");
            }

            return content;
        }

        //添加 electron 启动参数
        private static void SetSwitch(string name, string value, ref string content)
        {
            if (name.IsNullOrWhiteSpace()) return;

            //检测注入
            var proxyReg = new Regex($@"(?<varname>\w+)\.commandLine\.appendSwitch\(""{name}"",""(?<value>[^""]*)""\)");
            var proxyMatch = proxyReg.Match(content);
            //检测到已经存在配置，则更新参数值
            if (proxyMatch.Success)
            {
                var matchValue = proxyMatch.Groups["value"].Value;
                if (value != matchValue)
                {
                    content = proxyReg.Replace(content, $@"${{varname}}.commandLine.appendSwitch(""{name}"",""{value}"")");
                    Logger.LogInfo($"直播伴侣成功覆盖启动参数  [{name}] = [{value}]");
                }
            }
            //否则添加新的配置
            else
            {
                var nosandboxReg = new Regex(@"(?<varname>\w+)\.commandLine\.appendSwitch\(""no-sandbox""\)");
                var match = nosandboxReg.Match(content);
                if (match.Success)
                {
                    var newvalue = $@"{match.Groups["varname"].ToString()}.commandLine.appendSwitch(""{name}"",""{value}""),";
                    content = content.Insert(match.Index, newvalue);
                    Logger.LogInfo($"直播伴侣成功添加启动参数  [{name}] = [{value}]");
                }
            }
        }

        private static void SetInnerText(HtmlNode node, string text)
        {
            node.InnerHtml = "";
            var textNode = node.OwnerDocument.CreateTextNode(text);
            node.AppendChild(textNode);
        }
    }
}
