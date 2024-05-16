using BarrageGrab.Forms;
using BarrageGrab.Forms.Models;
using BarrageGrab.Modles.JsonEntity;
using BarrageGrab.Proxy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BarrageGrab
{
    public partial class FormView : Form
    {
        static int printCount = 0;

        WsBarrageServer barServer = AppRuntime.WsServer;
        WssBarrageGrab grab = AppRuntime.WsServer.Grab;
        ISystemProxy proxy = AppRuntime.WsServer.Grab.Proxy;

        public FormView()
        {
            InitializeComponent();
            barServer.OnPrint += WssService_OnPrint;
            barServer.Grab.Proxy.OnProxyStatus += Proxy_OnProxyStatus;
            AppRuntime.RoomCaches.OnCache += RoomCaches_OnCache;          
        }

        private void InitTabPages()
        {
            var msgTypes = GetMsgTypes();
            foreach (TabPage page in this.tab_filters.TabPages)
            {
                page.Padding = new Padding(0)
                {
                    Left = 5
                };
                page.Margin = new Padding(0);
                foreach (Control control in page.Controls)
                {
                    var panel = control as FlowLayoutPanel;
                    if (panel == null) continue;
                    panel.Padding = new Padding(0);
                    panel.Margin = new Padding(0);

                    foreach (var label in msgTypes)
                    {
                        var ck = new CheckBox()
                        {
                            Text = label.Value,
                            Tag = label.Key,
                            Parent = panel,
                            Padding = new Padding(0),
                            Margin = new Padding(0)
                        };
                        string type = "console";
                        if (page == this.tabPage_Ws)
                        {
                            type = "ws";
                            ck.Checked = AppSetting.Current.PushFilter.Contains(label.Key);
                        }
                        if (page == this.tabPage_Console)
                        {
                            type = "console";
                            ck.Checked = AppSetting.Current.PrintFilter.Contains(label.Key);
                        }
                        if (page == this.tabPage_Log)
                        {
                            type = "log";
                            ck.Checked = AppSetting.Current.LogFilter.Contains(label.Key);
                        }
                        ck.Name = $"cbx_bartype_{type}_{label.Key}";
                        ck.CheckedChanged += Ck_CheckedChanged;
                        panel.Controls.Add(ck);
                    }
                }
            }
        }

        private void Ck_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cbx = sender as CheckBox;            
            if (cbx == null) return;            
            var selected = GetCheckedBarTypes((FlowLayoutPanel)cbx.Parent);
            if (cbx.Parent.Parent == this.tabPage_Console)
            {
                AppSetting.Current.PrintFilter = selected;
            }
            if (cbx.Parent.Parent == this.tabPage_Ws)
            {
                AppSetting.Current.PushFilter = selected;
            }
            if(cbx.Parent.Parent == this.tabPage_Log)
            {
                AppSetting.Current.LogFilter = selected;
            }
            AppSetting.Current.Save();
        }

        private int[] GetCheckedBarTypes(FlowLayoutPanel panel)
        {
            List<int> list = new List<int>();
            foreach (Control ctr in panel.Controls)
            {
                var cbk = ctr as CheckBox;
                if (cbk == null) continue;
                if (cbk.Checked && cbk.Name.StartsWith("cbx_bartype_"))
                {
                    list.Add((int)cbk.Tag);
                }                
            }
            return list.OrderBy(o => o).ToArray();
        }

        private  Dictionary<int,string> GetMsgTypes()
        {
            var dic = new Dictionary<int, string>();
            Type enumType = typeof(PackMsgType);
            FieldInfo[] fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                DescriptionAttribute attribute = (DescriptionAttribute)field.GetCustomAttribute(typeof(DescriptionAttribute), false);
                int value = (int)field.GetValue(null);
                if (attribute != null && value>0)
                {
                    dic.Add(value, attribute.Description);
                }
            }

            return dic;
        }

        private void RoomCaches_OnCache(object sender, AppRuntime.RoomCacheManager.RoomCacheEventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                if (e.Model == 0)
                {
                    var item = new RoomCacheItem(e.RoomInfo);
                    this.label3.Text = $"房间缓存列表({AppRuntime.RoomCaches.RoomInfoCache.Count})";
                    list_roomCaches.Items.Add(item);
                }
            }));            
        }

        private void FormView_Load(object sender, EventArgs e)
        {
            this.txb_wsaddr.Text = barServer.ServerLocation;
            this.txb_upstreamProxy.Text = proxy.HttpUpstreamProxy;
            this.cbx_barrageLog.Checked = AppSetting.Current.BarrageLog;
            InitTabPages();

            this.cbx_enableProxy.Checked = SystemProxy.ProxyIsOpen();
        }

        private void Proxy_OnProxyStatus(object sender, Proxy.ProxyEventArgs.SystemProxyChangeEventArgs e)
        {
            if (this.Disposing || this.IsDisposed) return;
            this.Invoke(new Action(() =>
            {
                this.cbx_enableProxy.Checked = e.Open;
            }));            
        }

        private void WssService_OnPrint(object sender, WsBarrageServer.PrintEventArgs e)
        {
            //将弹幕输出到richTextBox
            //颜色转换
            Color color = AppSetting.Current.ColorMap[e.MsgType].Item2;
            string msg = e.Message;

            this.Invoke(new Action(() =>
            {
                //输出到richTextBox
                this.rich_output.SelectionColor = color;
                this.rich_output.AppendText(msg + "\n");
                this.rich_output.ScrollToCaret();

                if (++printCount > 10000)
                {
                    this.rich_output.Clear();
                    printCount = 0;
                }
            }));          
        }

        private void cbx_enableProxy_CheckedChanged(object sender, EventArgs e)
        {
            var checker = (CheckBox)sender;
            if (checker.Checked)
            {
                barServer.Grab.Proxy.RegisterSystemProxy();
            }
            else
            {
                barServer.Grab.Proxy.CloseSystemProxy();
            }
        }

        private void list_roomCaches_DoubleClick(object sender, EventArgs e)
        {
            // 获取双击的 ListBox 中的选中项
            var selectedItem = list_roomCaches.SelectedItem as RoomCacheItem;
            if (selectedItem == null) return;

            var form = new RoomDetail(selectedItem.RoomInfo);                        
            form.ShowDialog();
        }

        private void btn_updateUpProxy_Click(object sender, EventArgs e)
        {
            var text = this.txb_upstreamProxy.Text;
            try
            {
                proxy.SetUpstreamProxy(text);
                AppSetting.Current.UpstreamProxy = text;
                AppSetting.Current.Save();
            }
            catch (Exception ex)
            {
                //弹窗提示错误
                MessageBox.Show(ex.Message,"设置失败");
            }            
        }

        private void cbx_barrageLog_CheckedChanged(object sender, EventArgs e)
        {
            var checker = (CheckBox)sender;
            AppSetting.Current.BarrageLog = checker.Checked;
            AppSetting.Current.Save();
        }
    }    
}
