# 抖音弹幕监听程序

## 介绍及配置

基于系统代理抓包打造的抖音弹幕服务推送程序，与网上其他程序不同的是，本软件不是只能从浏览器获取弹幕数据，

它能够抓取电脑上所有抖音弹幕来源数据，主要包括两种来源：

1. 浏览器进程
2. 抖音直播伴侣

程序中有基本的配置可以过滤弹幕进程，弹幕数据通过Websocket服务推送，只需连接到本服务器就能接收到到弹幕数据消息

``` xml
	<appSettings>
		<!--过滤Websocket数据源进程,可用','进行分隔，程序将会监听以下进程的弹幕信息-->
		<add key="filterProcess" value="直播伴侣"/>
		<!--Websocket监听端口-->
		<add key="wsListenPort" value="8888"/>
	</appSettings>
```

## 主要依赖项

+ [FiddlerCore 4.6.2](https://www.nuget.org/packages/fiddlercore/)  (Nuget 包管理器貌似无法搜索到 只能通过命令安装或手动下载)

+ [Protobuf-net 3.1.22]([NuGet Gallery | protobuf-net 3.1.22](https://www.nuget.org/packages/protobuf-net/))

## 推送数据格式

见项目 **JsonEntity\BarrageMessages.cs**，这是默认推送格式，如需调整请参照项目 **proto\message.proto** 自行调整

## 特别注意

1. 程序只能监听到握手之后的ws数据包，例如先进入直播间或开启直播再打开本程序是无法监听到的，所以可以保持程序后台运行，避免频繁刷新直播间或者重新上下播
2. 由于打开系统代理需要自动检查/生成证书，所以程序需要管理员身份运行
3. 只有到达客户端的弹幕数据才能被接收，被抖音服务器过滤的弹幕是抓不到的

## 鸣谢

+ 特别鸣谢 [douyin_web_live](https://github.com/gll19920817/douyin_web_live) 提供的部分proto文件
