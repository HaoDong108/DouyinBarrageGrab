import asyncio
import json
from jsonpath import jsonpath
import websockets

# 1用户发言#2用户点赞#3用户入房#4关注主播#5用户礼物#6人数统计
'''
str(fid[0:2])=="403": #字符串提取方便自定义命令
'''


def msg(data):
    global close
    load_json_data = json.loads(data.get("Data"))
    用户名 = jsonpath(load_json_data, '$.User.Nickname')
    用户等级 = jsonpath(load_json_data, '$.User.PayLevel')
    粉丝团 = jsonpath(load_json_data, '$.User.FansClub.ClubName')
    用户发言 = jsonpath(load_json_data, '$.Content')
    if 用户发言[0] == "#关闭":
        close = False
    else:
        print(用户名[0])
        print(用户等级[0])


def praise(data):  # type2
    load_json_data = json.loads(data.get("Data"))
    用户点赞 = jsonpath(load_json_data, '$.Content')
    print("感谢" + 用户点赞[0])


def welcome(data):  # type3
    load_json_data = json.loads(data.get("Data"))
    用户名 = jsonpath(load_json_data, '$.User.Nickname')
    粉丝团 = jsonpath(load_json_data, '$.User.FansClub.ClubName')
    if 粉丝团[0] == "Reset":
        print("欢迎粉丝:" + 用户名[0])
    else:
        print("欢迎:" + 用户名[0])


def thank(data):  # type5
    load_json_data = json.loads(data.get("Data"))
    粉丝团 = jsonpath(load_json_data, '$.User.FansClub.ClubName')
    用户送礼 = jsonpath(load_json_data, '$.Content')
    # txt1 = txt.replace("主播", "黎公子") #字符串替换功能方便拓展功能
    if 粉丝团[0] == "Reset":
        print("感谢老板:" + 用户送礼[0])
    else:
        print("感谢:" + 用户送礼[0])


def check(data):  # type6
    load_json_data = json.loads(data.get("Data"))
    房间统计 = jsonpath(load_json_data, '$.Content')
    print(房间统计[0])


def check_json(json_data):
    Token = json_data.get("Type")  # 标签类型
    if Token == 1:  # 1用户发言
        msg(json_data)
    elif Token == 2:  # 2用户点赞
        praise(json_data)
    elif Token == 3:  # 3用户入房
        welcome(json_data)
    elif Token == 4:  # 感谢关注
        print(str(json_data))
    elif Token == 5:  # 5用户礼物
        thank(json_data)
    elif Token == 6:  # 6人数统计
        check(json_data)
    else:
        print(json_data)


async def main():
    global close
    async with websockets.connect("ws://127.0.0.1:8888/", ping_interval=None) as ws:
        await ws.send("token")
        close = True
        while close is True:
            result = await ws.recv()
            check_json(json.loads(result))
        await ws.close()


close = True
asyncio.run(main())