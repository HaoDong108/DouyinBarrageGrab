# ruff: noqa: F401,F403,F405,E402,F541,E722
import asyncio
import websockets
import json
import os
import ctypes
import sys
import time
import winreg
import tkinter as tk
from tkinter import ttk
import customtkinter
import subprocess
import random
import threading
import atexit
import psutil
import queue

# 创建一个队列
gui_queue = queue.Queue()

# 创建一个线程安全的标志
stop_flag = threading.Event()


def is_admin():
    try:
        return ctypes.windll.shell32.IsUserAnAdmin()
    except:
        return False


def disable_proxy():
    internet_settings = winreg.OpenKey(
        winreg.HKEY_CURRENT_USER,
        r"Software\Microsoft\Windows\CurrentVersion\Internet Settings",
        0,
        winreg.KEY_ALL_ACCESS,
    )

    # 设置代理使能为0（禁用）
    winreg.SetValueEx(internet_settings, "ProxyEnable", 0, winreg.REG_DWORD, 0)
    # 清空代理服务器信息
    winreg.SetValueEx(internet_settings, "ProxyServer", 0, winreg.REG_SZ, "")

    winreg.CloseKey(internet_settings)
    print("Proxy disabled.")


def initGame():
    print("Init game.")
    pass


class App(customtkinter.CTk):
    def __init__(self):
        super().__init__()

        self.title("dy-barrage-grab-log-window")
        self.geometry("800x600")
        self.grid_columnconfigure((0, 1), weight=1)
        self.grid_rowconfigure(0, weight=1)
        self.LogFrame = LogFrame(self)
        self.LogFrame.grid(row=0, column=0, sticky="nsew")


class LogFrame(customtkinter.CTkFrame):
    def __init__(self, master):
        super().__init__(master)

        # 创建一个Frame来包含treeview和滚动条
        self.tree_frame = ttk.Frame(self)
        self.tree_frame.grid(row=0, column=0, sticky="nsew", padx=50, pady=50)

        # 创建一个滚动条
        self.scrollbar = customtkinter.CTkScrollbar(self.tree_frame)
        self.scrollbar.grid(row=0, column=1, sticky="ns")

        self.treeview = ttk.Treeview(
            self.tree_frame,
            columns=("Type", "User ID", "User Nickname", "Content"),
            show="headings",
            height=20,
            yscrollcommand=self.scrollbar.set,  # 将滚动条与treeview关联
        )
        self.treeview.grid(row=0, column=0, sticky="nsew")

        # 设置滚动条的命令为移动treeview
        self.scrollbar.configure(command=self.treeview.yview)

        # 设置列宽
        self.treeview.column("Type", width=50)
        self.treeview.column("User ID", width=100)
        self.treeview.column("User Nickname", width=100)
        self.treeview.column("Content", width=450)

        self.treeview.heading("Type", text="Type")
        self.treeview.heading("User ID", text="User ID")
        self.treeview.heading("User Nickname", text="User Nickname")
        self.treeview.heading("Content", text="Content")

        # 设置不同类型的颜色
        self.treeview.tag_configure("type1", foreground="black")
        self.treeview.tag_configure("type2", foreground="cyan")
        self.treeview.tag_configure("type3", foreground="gold")
        self.treeview.tag_configure("type4", foreground="blue")
        self.treeview.tag_configure("type5", foreground="red")
        self.treeview.tag_configure("type6", foreground="purple")
        self.treeview.tag_configure("type7", foreground="green")
        self.treeview.tag_configure("type8", foreground="orange")
        self.treeview.tag_configure("type9", foreground="gray")
        # 添加更多类型的颜色配置...

    def display_message(self, message):
        parsed_message = json.loads(message)
        message_type = parsed_message.get("Type")

        data_str = parsed_message.get("Data", "{}")
        try:
            data = json.loads(data_str)
        except json.JSONDecodeError:
            print(f"Cannot parse 'Data' as JSON: {data_str}")
            return
        user = data.get("User", {})
        if user is None:
            # print(f"'User' is None in 'Data': {data_str}")
            user = {"Id": "无", "Nickname": "无"}
        user_id = user.get("Id", "")
        user_nickname = user.get("Nickname", "")
        content = data.get("Content", "")
        # 根据类型设置颜色
        color_tag = f"type{message_type}"
        message_type_str = [
            "无",
            "弹幕",
            "点赞",
            "进房",
            "关注",
            "礼物",
            "统计",
            "粉团",
            "分享",
            "下播",
        ]
        self.treeview.insert(
            "",
            "end",
            values=(
                message_type_str[message_type],
                user_id,
                user_nickname,
                content,
            ),
            tags=(color_tag,),
        )


# async def send_ping(websocket):
#     while True:
#         try:
#             if websocket.open:
#                 await websocket.ping()
#                 await asyncio.sleep(5)
#             else:
#                 break
#         except websockets.exceptions.ConnectionClosedOK:
#             # 连接已关闭，尝试重新连接
#             websocket = await websockets.connect("ws://127.0.0.1:8888")


async def receive_messages():
    while True:
        try:
            async with websockets.connect(
                "ws://127.0.0.1:8888", ping_timeout=None, ping_interval=None
            ) as websocket:
                while True:
                    message = await websocket.recv()
                    if message is not None:
                        try:
                            json.loads(message)
                            app.LogFrame.display_message(message)
                        except json.JSONDecodeError:
                            print(
                                f"Received a message that could not be parsed as JSON: {message}"
                            )
        except (
            websockets.ConnectionClosed,
            websockets.exceptions.ConnectionClosedError,
            websockets.exceptions.ConnectionClosedOK,
        ) as e:
            print(f"Connection closed, retrying...{e}")
            continue
        except Exception as e:
            print(f"An error occurred on ws server: {e}")
            continue


def main():
    global listener_process
    global app
    # Check if the process is already running
    for proc in psutil.process_iter():
        if proc.name() == "WssBarrageServer.exe":
            print("The process is already running.")
            proc.kill()
            time.sleep(1)

    # Start a new process
    # 将WssBarrageServer.exe放至同目录下dy-barrage-grab文件夹
    # 获取当前脚本的绝对路径
    script_dir = os.path.dirname(os.path.realpath(__file__))
    # 构建WssBarrageServer.exe的绝对路径
    exe_path = os.path.join(script_dir, "dy-barrage-grab", "WssBarrageServer.exe")

    # 使用绝对路径启动进程
    listener_process = subprocess.Popen(exe_path)
    # 创建一个新的事件循环
    loop = asyncio.new_event_loop()
    # 设置这个事件循环为当前线程的事件循环
    asyncio.set_event_loop(loop)
    app = App()

    # 点击关闭按钮时调用的函数
    def on_close():
        print("Closing...")
        # 设置停止标志
        stop_flag.set()
        # 关闭WssBarrageServer
        listener_process.terminate()
        # 关闭代理
        disable_proxy()
        # 退出程序
        os._exit(0)

    # 设置关闭按钮的回调函数
    app.protocol("WM_DELETE_WINDOW", on_close)
    # 在事件循环中创建新的异步任务
    loop.create_task(receive_messages())

    def update():
        # 更新Tkinter的界面
        app.update()
        # 在事件循环中定期调用update方法
        loop.call_soon(update)

        # 检查队列中是否有任务
        while not gui_queue.empty():
            # 获取任务并执行
            task = gui_queue.get()
            task()

    # 在事件循环中定期调用update方法
    loop.call_soon(update)
    try:
        # 运行事件循环
        loop.run_forever()
    except Exception as e:
        print(f"An error occurred in loop: {e}")
    finally:
        # 关闭事件循环
        loop.close()


if __name__ == "__main__":
    if is_admin():
        # 如果已经是管理员权限，那么直接运行你的代码
        print("Running as administrator.")
        pass
    else:
        # 如果不是管理员权限，那么以管理员权限重新启动程序
        print("Running as non-administrator.")
        ctypes.windll.shell32.ShellExecuteW(
            None, "runas", sys.executable, " ".join(sys.argv), None, 1
        )
    # try:
    main()
# except Exception as e:
# print(f"An error occurred: {e}")
# finally:
#     # 这里结束进程
#     listener_process.terminate()


# 注册一个函数，在程序结束时调用
def on_exit():
    listener_process.terminate()


atexit.register(on_exit)
