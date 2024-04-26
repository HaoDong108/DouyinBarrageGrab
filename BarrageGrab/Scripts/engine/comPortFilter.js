/*
 * 注解：串口数据过滤脚本，可用于自定义配置串口的发送报文
 * Tip: 该脚本引擎为Jint，支持的语法请参考官方文档：https://github.com/sebastienros/jint
 * 为了 工具环境:
 *     1.控制台:   console: 控制台输出
 *                console.log, console.error, console.warn  这些日志将被输出到控制台, 其中error日志将被记录到日志文件
 * 
 *     2.编码器:   encoder:  字符串 和 Uint8Array 互转
 *                encoder.utf8ToBytes(str):Unit8Array ,encoder.utf8ToString(buff):String   
 *                encoder.utf32ToBytes(str):Unit8Array ,encoder.utf32ToString(buff):String
 *                encoder.asciiToBytes(str):Unit8Array ,encoder.asciiToString(buff):String
 * 
 *     3.Bit转换:  bitConvert: 支持 Number,Boolean 与 Uint8Array 互转
 *                bitConvert.toNumber(bytes, offset):Number
 *                bitConvert.toBoolean(bytes, offset):Boolean
 *                bitConvert.toString(bytes, offset, length):String
 *                bitConvert.toString(bytes, offset):String
 *                bitConvert.toString(bytes):String
 *                bitConvert.getBytes(value):Unit8Array  支持传入 Number,Boolean  String请使用 encoder.utf8ToBytes
 */



/**
 * 这个函数由程序JS引擎调用，不要删除
 * @param {number} msgType 消息类型，如礼物，弹幕消息等，和程序内定义一致 见 Modles\JsonEntity\BarrageMessages.cs  PackMsgType 枚举
 * @param {Object} msg 被WsBarrageServer简化后的消息对象，具体可参考Json实体文件 见 Modles\JsonEntity\BarrageMessages.cs
 * @param {Object} roomInfo 当前消息对应的房间信息 见 Modles\RoomInfo.cs 结构
 * @returns {Uint8Array|string} 返回一个Uint8Array类型的数据，或者返回一个字符串(将自动进行utf8编码)，如果返回 null 则不发送数据。返回的数据将会被发送到串口
 */
function onPackData(msgType, msg , roomInfo) {
    let json = JSON.stringify(msg);    
    return json+"\n";
    // let excludeMsgType = [3, 6, 0]; // 过滤掉进入直播间和统计信息
    // if (excludeMsgType.includes(msgType)) return;
    // var result = msg.Content + "\r\n";    
    // return result;
}
