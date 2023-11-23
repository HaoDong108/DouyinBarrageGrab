using BarrageGrab.Modles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace BarrageGrab.Forms
{
    public partial class RoomDetail : Form
    {
        public RoomDetail(RoomInfo info)
        {
            InitializeComponent();
            //小驼峰
            var json = JsonConvert.SerializeObject(info,Formatting.Indented,new JsonSerializerSettings()
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });

            this.textBox1.Text = json;
        }

        private void RoomDetail_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //复制到剪切板
            Clipboard.SetText(this.textBox1.Text);
            MessageBox.Show("复制成功");
        }
    }
}
