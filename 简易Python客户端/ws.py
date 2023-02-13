from websocket import connection
import json

def ck(data):#type2
    txt=data.get("Data")
    load_data=json.loads(txt)
    print("感谢"+load_data.get("Content"))
    
def welcome(data):#type3
    json1=data.get("Data")
    Nickname=json.loads(json1)
    json2=Nickname.get("User")
    print("欢迎:"+json2["Nickname"])

def thank(data):#type5
    txt=data.get("Data")
    load_data=json.loads(txt)
    print("感谢"+load_data.get("Content"))

def check(data):#type6
    txt=data.get("Data")
    load_data=json.loads(txt)
    print(load_data.get("Content"))

def main():
    try:
        ws = create_connection("ws://127.0.0.1:8888")
        ws.send("token")
        while(True):
            result = ws.recv()
            load_data=json.loads(result)
            type=load_data.get("Type")#标签类型
            if type==1:#1用户发言
                msg(load_data)
            elif type==2:#2用户点赞
                ck(load_data)
            elif type==3:#3用户入房
                welcome(load_data)
            elif type==4:#用户关注
                print(str(load_data))
            elif type==5:#5用户礼物
                thank(load_data)
            elif type==6:#6人数统计
                check(load_data)
        ws.close()
    except:
        ws.close
main()