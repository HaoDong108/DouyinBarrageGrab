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


/*判断是否自动关闭 */
//获取路由参数
function getQueryString(name) {
    let reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    let r = window.location.search.substr(1).match(reg); //search,查询？后面的参数，并匹配正则
    if (r != null) return unescape(r[2]);
    return null;
}

if (getQueryString("pause") === "true" || AUTOPAUSE === true) {
    //自动暂停一次播放
    var pauseIdx = setInterval(() => {
        let playbtn = document.querySelector(".xgplayer-play");
        if (!playbtn) return;
        let status = playbtn.getAttribute("data-state");
        if (status !== "play") return;
        playbtn.click();
        clearInterval(pauseIdx);
        console.log("已自动暂停播放");
    }, 500);
}


