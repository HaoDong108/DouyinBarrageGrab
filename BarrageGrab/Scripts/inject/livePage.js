//上下文常量将在上方由程序注入
//const PROCESS_NAME = "进程名";

/**
 * Web/PC端 直播页JS注入
 * 正则: @".*:\/\/live.douyin\.com\/\d+"
 **/

/*注入脚本 定时触发全局keydown，避免直播流被无操作阻断*/

// 创建一个虚拟的按键事件
const simulatedEvent = new KeyboardEvent('keydown', {
    key: 'Virtual_Alt',
    keyCode: 18, //alt
    bubbles: true,
    cancelable: true
});

// 获取要分派事件的目标元素，例如文档的根元素
const targetElement = document.documentElement;

// 设置定时器，每隔60秒模拟一次按键事件
setInterval(() => {
    targetElement.dispatchEvent(simulatedEvent);
}, 1000 * 60);

// 订阅 keydown 事件 (Debug)
targetElement.addEventListener('keydown', event => {
    if (event.key == "Virtual_Alt") {
        console.log('[防挂机处理] 按下了', event.key);
    }
});

/*禁止关闭 Websocket*/
WebSocket.prototype.close = function () { return; }


