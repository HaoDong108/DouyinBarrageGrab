/**
 * 注解：串口数据过滤脚本，可用于自定义配置串口的发送报文
 * Tip: 该脚本引擎为Jint，支持的语法请参考官方文档：https://github.com/sebastienros/jint
 * 工具环境:
 *     1.支持 console.log, console.error, console.warn 这些日志将被输出到控制台, 其中error日志将被记录到日志文件
 *     2.编码工具  encoder.utf8ToBytes(str),encoder.utf8ToString   字符串 和 Uint8Array 互转
 *                encoder.utf32ToBytes(str),encoder.utf32ToString   
 *                encoder.asciiToBytes(str),encoder.asciiToString   
 */



/**
 * 这个函数由程序JS引擎调用，不要删除
 * @param {number} msgType 消息类型，如礼物，弹幕消息等，和程序内定义一致
 * @param {Object} msg 被WsBarrageServer简化后的消息对象，具体可参考Json实体文件
 * @returns {Uint8Array|string} 返回一个Uint8Array类型的数据，或者返回一个字符串，如果返回 null 则不发送数据。返回的数据将会被发送到串口
 */
function onPackData(msgType, msg) {
    let excludeMsgType = [3, 6, 0]; // 过滤掉进入直播间和统计信息
    if (excludeMsgType.includes(msgType)) return;    
    var result = msg.Content + "\r\n";    
    return result;
    // var buff = encoder.utf8ToBytes(result);
    // var newjs = encoder.utf8ToString(buff);
    // console.log(newjs);    
    // return newjs;
}
