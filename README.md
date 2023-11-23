# 📺 抖音弹幕监听器
+ Github大佬请移步[Gitee](https://gitee.com/haodong108/dy-barrage-grab)，参与Issues讨论，QQ讨论群:819927029(进群前请Star，谢谢🔪)
+ 发行版下载地址在[这里](https://gitee.com/haodong108/dy-barrage-grab/releases)，别下成源码包了!!
+ 💭 需要ws多直播间直连、拿直播间/用户主页信息、自动私信、自动发弹幕、获取榜单等功能的可以找我(QQ1083092844)提供一些技术支持(不支持匿名直播间)

## ⛳近期更新
2023-11-23 v2.7.0
1. 修复了因抖音版本升级导致的WebRoomId获取不到的问题，并更近了正则表达式尽量兼容后续的变更
2. 添加了winfrom窗体，功能有限，默认隐藏
3. 支持了控制台隐藏，推送器弹幕类型过滤，日志弹幕类型过滤，弹幕文件日志  (见配置文件)
4. 支持了更多的ws命令:
  ```c#
    /*
     * 例如发送 {"Cmd":1,"Data":true} 到ws连接地址 关闭程序
     * 前往 http://wstool.jackxiang.com/ 在线ws测试
     */

    public enum CommandCode
    {
        /// <summary>
        /// 空指令
        /// </summary>
        None = 0, 

        /// <summary>
        /// 安全关闭程序
        /// </summary>
        Close = 1,

        /// <summary>
        /// 启用系统代理 Data:bool
        /// </summary>
        EnableProxy = 2,

        /// <summary>
        /// 是否显示控制台 Data:bool
        /// </summary>
        DisplayConsole = 3,
    }

    
    public class Command
    {
        /// <summary>
        /// 指令标识
        /// </summary>
        public CommandCode Cmd { get; set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        public object Data { get; set; }
    }

  ```

2023-11-04 v2.6.9

1. 修复了因抖音版本升级导致的WebRoomId获取不到的问题，并做了多重处理

2023-09-24 v2.6.8

1. 添加了上游代理支持，现在可以将无关的请求转发到其他的代理地址！详见下方配置说明`upstreamProxy`
2. 内部添加了webRoomid 的缓存映射，只要打开直播间地址即可缓存WebRoomId和Roomid的映射关系，`WebRoomId`现已添加至ws弹幕流数据中!，需要注意直播伴侣端提取的弹幕当前不会有该字段，除非在开播后打开浏览器访问一次你的直播间
3. MsgPack 现在添加了进程名字段`ProcessName`

2023-08-17 v2.6.7

1. [重要更新] 支持浏览器/客户端 http 弹幕监听，这也会解决原先版本因网络波动使抖音客户端弹幕获取方式降级，而获取不到弹幕导致必须刷新页面重新连接的问题。
2. [重要更新] 原先的web/客户端 页面无操作检测逻辑因官方js变动而失效，本次更新修复且完善了该功能，现在会定时模拟全局按键、绕过js检测逻辑、置空Websocket.close 方法三重手段尽量避免断开连接。你也可以直接修改[直播页脚本](./BarrageGrab/Scripts/inject/livePage.js)而无需重新编译源码。
3. ws连接现在默认监听在0.0.0.0地址，可以从其他客户端连接。
4. 代理端口现在可选择不占用系统代理，便于某些用户直接使用浏览器代理获取弹幕(启动时携带参数，例如:"chrome.exe --proxy-server=127.0.0.1:8827"，也可以在浏览器快捷方式-目标 引号之后附加该参数)

2023-06-19 v2.6.6

1. 修复了控制台点击后阻塞程序的问题
2. 通过篡改JS Response屏蔽了Web端定时操作检测，避免在读取web端弹幕时总被中断(本地测试通过，有问题请反馈)
3. 添加了程序关闭指令，连接ws后可向服务端发送`{"Cmd":1}`json数据包安全关闭程序和系统代理
4. 修复了客户端连接断开后未从套接字连接池删除，导致显示error的bug

## 😎介绍及配置

### 介绍

基于系统代理抓包打造的抖音弹幕服务推送程序，它能够获取电脑上所有抖音弹幕来源数据，主要包括三种来源：**浏览器进程** ，**抖音客户端**，**抖音直播伴侣**。它可以监听**弹幕**，**点赞**，**关注**，**送礼**，**进入直播间**，**直播间统计**，**粉丝团**系列消息，你可使用它做自己的直播间数据分析，以及弹幕互动游戏，语音播报等。

### <a id="tag1">配置文件</a>

程序中有基本的配置可以过滤弹幕进程，弹幕数据通过Websocket服务推送，其他程序只需接入ws服务器就能接收到到弹幕数据消息

``` xml
<!--配置更改后重启才能生效-->
<appSettings>
    <!--过滤Websocket数据源进程,可用','进行分隔，程序将会监听以下进程的弹幕信息-->
    <add key="processFilter" value="直播伴侣,douyin,chrome,msedge,QQBrowser,360se,firefox,2345explorer,iexplore"/>
    <!--Websocket监听端口-->
    <add key="wsListenPort" value="8888"/>
    <!--true:监听在0.0.0.0，接受任意Ip连接，false:监听在127.0.0.1，仅接受本机连接-->
    <add key="listenAny" value="true"/>	  
    <!--系统代理端口-->
    <add key="proxyPort" value="8827"/>
	  <!--上游代理地址，例如开启了系统代理，但是需要将其他无关请求转发到VPN工具中,例如:127.0.0.1:11223,不要带http://-->
	  <add key="upstreamProxy" value="dgproxy.qp-cn.local:3128"/>
    <!--在控制台输出弹幕-->
    <add key="printBarrage" value="true"/>
    <!--要在控制台打印的弹幕类型,可以用','隔开(空代表不过滤) 1[普通弹幕]，2[点赞消息]，3[进入直播间]，4[关注消息]，5[礼物消息]，6[统计消息]，7[粉丝团消息]，8[直播间分享]，9[下播]-->
    <add key="printFilter" value=""/>
    <!--要推送的弹幕消息类型,可以用','隔开，同上-->
    <add key="pushFilter" value=""/>
    <!--要日志记录的弹幕消息类型,可以用','隔开，同上-->
    <add key="logFilter" value="1,2,4,5,6,7,8"/>
    <!--是否启用系统代理,若设置为false 则需要在程序手动指定代理地址 -->
    <add key="usedProxy" value="true"/>
    <!--开启内置的域名过滤，设置为false会解包所有https请求，cpu占用很高，建议在无法获取弹幕数据时调整 -->
    <add key="filterHostName" value="true"/>
    <!--已知的弹幕域名列表 ','分隔  用作过滤规则中，凡是webcast开头的域名程序都会自动列入白名单-->
    <add key="hostNameFilter" value=""/>
    <!--要进行过滤的房间ID,不填代表监听所有，多项使用','分隔，浏览器进入直播间 F12 控制台输入 'window.localStorage.playRoom' 即可快速看到房间ID(不是地址栏中的那个) -->
    <add key="roomIds" value=""/>
	  <!--隐藏控制台-->
	  <add key="hideConsole" value="false"/>
    <!--弹幕文件日志-->
    <add key="barrageFileLog" value="false"/>
    <!--显示窗体-->
    <add key="showWindow" value="false"/>
  </appSettings>
```

### 推送数据格式

弹幕数据由WebSocket服务进行分发，使用Json格式进行推送，见项目  [BarrageMessages.cs](./BarrageGrab/JsonEntity/BarrageMessages.cs)，如需调整请克隆项目后参照 [message.proto](./BarrageGrab/proto/message.proto) 进行源码修改调整，文件包含所有弹幕相关数据结构，可前往[ws在线测试](http://wstool.jackxiang.com/)网站，连接 ws://127.0.0.1:8888 进行测试

### 使用方法
1. 管理员身份启动本程序，第一次启动会提示安装自签名证书，程序启动后挂在后台不要关，不然再打开会监听不到正在进行中的直播弹幕。
2. 打开浏览器进入任何直播间进行测试，没有问题再启动直播伴侣，<b>浏览器和直播伴侣同时打开时还是要注意进程过滤，不然弹幕会杂交</b>

### 启动后无法获取，排查清单

1. 先判断自己的浏览器进程名称是否在配置文件列表中

2. 程序启动后，检查系统代理有无正常打开，有可能修改注册表被其他杀毒软件拦截

3. 检查程序是否以管理员身份启动

4. 注意程序启动先后顺序(很重要)，必须保证在进入直播间之前监听程序已在运行

5. 检查代理端口是否与其他端口冲突，可尝试修改

6. 依旧不行？请前往[这里]([电脑 - 商品搜索 - 京东 (jd.com)](https://search.jd.com/Search?keyword=电脑))

   

## 🖼️控制台截图

[![控制台截图](https://s1.ax1x.com/2022/11/10/z9YYPU.png)](https://imgse.com/i/z9YYPU)



## ⚠️特别注意

1. 程序只能监听到握手之后的ws数据包，例如先进入直播间或开启直播再打开本程序是无法监听到的，所以请保持程序后台运行

2. 由于打开系统代理需要自动检查/生成证书，所以程序需要管理员身份运行

3. 只有到达客户端的弹幕数据才能被接收，被抖音服务器过滤的弹幕是抓不到的

4. 关闭程序后若有无法上网的情况请检查: 网络和internet设置-->代理-->使用代理服务器 ，关闭即可

   

## 📢鸣谢

+ 请施舍一个 ⭐Start ，现在工具处于早期版本，请及时订阅更新

  

## ⚖️免责声明

+ 本程序仅供学习参考，不得用于商业用途，不得用于恶意搜集他人直播间用户信息!

## 🍻支持一下?

开源不易，你忍心白嫖是吧🤔~

<p>
<img src="./imgs/微信.png" alt="微信支付" style="zoom:70%;border-radius: 5px;" />
</p>