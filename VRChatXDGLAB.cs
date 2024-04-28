using AI绘图法典tag纠错;
using Manina.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms; 
using System.Xml.Serialization;
using uOSC;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Security.Cryptography; 
using Windows.Storage.Streams;
using Thread = System.Threading.Thread;
using 我的多功能类库;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Drawing.Drawing2D;
using Gma.System.MouseKeyHook;
using Point = System.Drawing.Point;
using System.Numerics;
using Panel = System.Windows.Forms.Panel;
using Windows.Storage;
using System.Runtime.CompilerServices;



namespace VRChatX郊狼
{
    public partial class VRChatXDGLAB : Form
    {
        DrawAudio drawAudio;
        private BluetoothLEAdvertisementWatcher Watcher = null;

        private BluetoothLEDevice CurrentDevice = null;

        private int Character_index = 0;
        public List<GattCharacteristic> CurrentCharacteristicList = new List<GattCharacteristic>();

        private const GattClientCharacteristicConfigurationDescriptorValue CHARACTERISTIC_NOTIFICATION_TYPE = GattClientCharacteristicConfigurationDescriptorValue.Notify;

        Dictionary<ulong, BluetoothLEAdvertisement> adv_datas = new Dictionary<ulong, BluetoothLEAdvertisement>(); // 
        Dictionary<ulong, BluetoothLEAdvertisement> res_datas = new Dictionary<ulong, BluetoothLEAdvertisement>(); // Scan Respones

        int listview_index = 0;
        Dictionary<ulong, int> listview_indexs = new Dictionary<ulong, int>();






        public byte[] HexStringToByteArray2(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
            {

                string bb = s.Substring(i, 2);

                buffer[i / 2] = (byte)Convert.ToByte(bb, 16);
            }
            return buffer;
        }

        public byte[] HexStringToByteArray(string s)
        {


            /*
            通道波形频率 / 通道波形强度
             
            通道波形频率长度1byte,值范围(10~240)，
            通道波形强度1byte，值范围(0 ~100)。 
            
            在B0指令中，每100ms要发送两通道各4组波形频率和波形强度，每组频率 - 强度代表了25ms的波形输出，4组数据代表了100ms数据。 在波形数据中，若某通道的输入值不在有效范围，则脉冲主机会放弃掉该通道全部4组数据。

            另外对于通道波形频率，在您的程序中可以将值范围限制在(10 ~1000),然后通过以下算法换算为要发送的通道波形频率：

             
            序列号
            序列号范围(0b0000 ~0b1111),如果输入的数据中修改了脉冲主机的通道强度，设置序列号 > 0,
            脉冲主机会通过B1回应消息以相同的序列号将修改后的通道强度从特性0x150B返回，
            如果不需要脉冲主机反馈通道强度，则序列号设置0b0000即可。 
            另外，为避免产生问题，当通过B0指令修改通道强度且序列号不为0时，
            建议等待150B返回B1且为相同序列号的信息后，再对通道强度进行修改。

            强度值解读方式
            强度值解读方式的4bits分为两个部分，高两位bits代表A通道解读方式，低两位bits代表B通道解读方式。
            解读方式:
            0b00->代表对应通道强度不做改变，对应通道的强度设定值无论是什么都无效
            0b01->代表对应通道强度相对增加变化，若A通道强度设定值为15(10进制)，那么A通道强度增加15
            0b10->代表对应通道强度相对减少变化，若A通道强度设定值为17(10进制)，那么A通道强度减少17
            0b11->代表对应通道强度绝对变化，若A通道强度设定值为32，那么A通道强度设置为32

            */
            // 0 0xB0(1byte指令HEAD) + 0xB0
            // 1    序列号(4bits) +0000 
            // 1    强度值解读方式(4bits) +0000
            // 2     A通道强度设定值(1byte) + 
            // 3     B通道强度设定值(1byte) + 
            //     A通道波形频率4条(4bytes) +
            //      A通道波形强度4条(4bytes) + 
            //      B通道波形频率4条(4bytes) + 
            //     B通道波形强度4条(4bytes)
            //   B0 BF  B1消息 BE消息
            //1F8B0800000000000000F30D092CF6CB0A2DF6752EAF44C2C63E2E81C5BE99E506BEC1400C11ABF071712D06F281B463B16F4868B13F50CCBBCAB5C437CBB1D41724E66C005467520E561BE259EE93E5580ED45302152BF70981AB43932B4652EF88D73CB07D68620098A633D5C4000000
            //   0,0,0,100,100,100,2,2,2,100,100,100,1,1,1,1,1,10,20 || 20 = 1,20 = 1 |
            byte[] buffer = new byte[20];

            string b = "B0";

            if (b == "B0")
            {
                string[] aa = s.Split(' ');


                buffer[0] = (byte)Convert.ToByte("0xB0", 16);
                buffer[1] = (byte)Convert.ToByte("00050100", 2);
                buffer[2] = Convert.ToByte(18);
                buffer[3] = Convert.ToByte(0);

                buffer[4] = Convert.ToByte(10);
                buffer[5] = Convert.ToByte(10);
                buffer[6] = Convert.ToByte(20);
                buffer[7] = Convert.ToByte(10);

                buffer[8] = Convert.ToByte(0);
                buffer[9] = Convert.ToByte(10);
                buffer[10] = Convert.ToByte(20);
                buffer[11] = Convert.ToByte(30);

                buffer[12] = Convert.ToByte(0);
                buffer[13] = Convert.ToByte(0);
                buffer[14] = Convert.ToByte(0);
                buffer[15] = Convert.ToByte(0);

                buffer[16] = Convert.ToByte(0);
                buffer[17] = Convert.ToByte(0);
                buffer[18] = Convert.ToByte(0);
                buffer[19] = Convert.ToByte(101);

            }












            //0xBF(1byte指令HEAD) + 
            // AB两通道强度软上限(2bytes) + 
            //AB两通道波形频率平衡参数(2btyes) + 
            // AB两通道波形强度平衡参数(2bytes)

            // B1消息
            //  当脉冲主机强度发生变化时，会立刻通过B1消息返回当前的强度值。如果是由于B0指令导致的强度变化，返回B1指令中序列号将会与引起此变化的命令所包含的序列号相同，否则序列号为0。
            // 0xB1(1byte指令HEAD) + 
            // 序列号(1byte) + 
            // A通道当前实际强度(1byte) + 
            //  B通道当前实际强度(1byte)

            //BE消息返回BF输入的对应设置后脉冲主机当前的AB通道强度软上限 + AB通道波形频率平衡参数 + AB通道波形强度平衡参数。
            //  0xBE(1byte指令HEAD) + 
            //  AB两通道强度软上限(2bytes) +
            //  AB两通道波形频率平衡参数(2btyes) + 
            //  AB两通道波形强度平衡参数(2bytes)




            return buffer;
        }





        public VRChatXDGLAB()
        {

            InitializeComponent();
            this.FormClosed += Form1_FormClosed;
            this.FormClosing += Form1_FormClosing;
            CheckForIllegalCrossThreadCalls = false;
            drawAudio = new DrawAudio(panel1, drawingTimer, dataTimer, textBox6);
            初始化波形队列();



            //测试.DGLab

            string pathss = System.Environment.CurrentDirectory.ToString() + "/初始化波形.DGLab";


            导入(pathss);

            this.Size = new System.Drawing.Size(311, 693);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            Watcher = new BluetoothLEAdvertisementWatcher
            {
                ScanningMode = BluetoothLEScanningMode.Active
            };
            Watcher.SignalStrengthFilter.InRangeThresholdInDBm = -100;
            Watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -100;
            Watcher.Received += OnAdvertisementReceived;

            // wait 5 seconds to make sure the device is really out of range
            Watcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(5000);
            Watcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromMilliseconds(2000);
            drawAudio.MainWindow_Load(sender, e);


            MessageBox.Show("本软件基于DG-LAB 郊狼3 开源的蓝牙协议制作，软件尚处于测试阶段，可能存在意外风险，请遵守DG-LAB 郊狼3的使用手册，不要将保护上限调到自己无法承受的程度！保护好自己的身体，愉快玩耍！", "警告！");

            加载本地目录下的玩法();
            高亮显示重要按钮();

            //更改界面和文字颜色(this, Color.FromArgb(18,18,18), Color.FromArgb(255,233,157));


        }


        void 高亮显示重要按钮()
        {

            Color a = Color.FromArgb(255, 233, 157);
            button50.BackColor = a;
            一键启动按钮.BackColor = a;
            button1.BackColor = a;
            button50.BackColor = a;
            button50.BackColor = a;
            button50.BackColor = a;
            button50.BackColor = a;



        }




        void 更改界面和文字颜色(Control a, Color 背景色, Color 文字色)
        {
            a.BackColor = 背景色;
            a.ForeColor = 文字色;

            if (a.Controls.Count > 0)
                foreach (Control ass in a.Controls)
                {
                    更改界面和文字颜色(ass, 背景色, 文字色);
                }

            if (a is GroupBox)
            {
                (a as GroupBox).Paint += groupBox_Paint;

            }

        }

        void groupBox_Paint(object sender, PaintEventArgs e)
        {
            GroupBox gBox = (GroupBox)sender;
            Pen PenColor = new Pen(gBox.ForeColor);
            Brush BrushColor = new SolidBrush(gBox.ForeColor);

            e.Graphics.Clear(gBox.BackColor);
            e.Graphics.DrawString(gBox.Text, gBox.Font, BrushColor, 10, 1);
            var vSize = e.Graphics.MeasureString(gBox.Text, gBox.Font);
            e.Graphics.DrawLine(PenColor, 1, vSize.Height / 2, 8, vSize.Height / 2);
            e.Graphics.DrawLine(PenColor, vSize.Width + 8, vSize.Height / 2, gBox.Width - 2, vSize.Height / 2);
            e.Graphics.DrawLine(PenColor, 1, vSize.Height / 2, 1, gBox.Height - 2);
            e.Graphics.DrawLine(PenColor, 1, gBox.Height - 2, gBox.Width - 2, gBox.Height - 2);
            e.Graphics.DrawLine(PenColor, gBox.Width - 2, vSize.Height / 2, gBox.Width - 2, gBox.Height - 2);
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            drawAudio.MainWindow_FormClosed(sender, e);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            关闭所有网络服务();//与 uOSC  Unity通信
            string aaa = System.Environment.CurrentDirectory.ToString() + "/" + DateTime.Now.ToString("yyyy_MM_dd_HH：mm：ss：ffff") + "自动存档.DGLab";
            if (checkBox3.Checked)
                序列化保存读取.序列化保存<郊狼存档>(a郊狼存档, aaa);
        }





        void 加载本地目录下的玩法()
        {
            listBox13.Items.Clear();
            listBox14.Items.Clear();
            DirectoryInfo aaa = new DirectoryInfo(System.Environment.CurrentDirectory.ToString());
            FileInfo[] bbb = aaa.GetFiles("*.DGLab");
            foreach (FileInfo f in bbb)
            {
                listBox13.Items.Add(f.Name);
                listBox14.Items.Add(f.Name);
            }

        }
        private void button52切换为选中玩法_Click(object sender, EventArgs e)
        {
            if (listBox14.Items.Count > 0 && listBox14.SelectedItem != null)
            {
                string aaa = System.Environment.CurrentDirectory.ToString() + "/" + listBox14.SelectedItem.ToString();

                导入(aaa);
            }
            else
            {
                MessageBox.Show("请选择一个玩法");
                return;
            }
            待执行波形列表.Clear();

        }
        private void button50_Click(object sender, EventArgs e)
        {//同步数据
            if (listBox13.Items.Count > 0 && listBox13.SelectedItem != null)
            {
                listBox14.SelectedItem = listBox13.SelectedItem;

                string aaa = System.Environment.CurrentDirectory.ToString() + "/" + listBox14.SelectedItem.ToString();

                导入(aaa);
            }
            else
            {
                MessageBox.Show("请选择一个玩法");
                return;
            }
            tabControl1.SelectedIndex = 1;

            button17_Click(sender, e);

            开启服务器接收消息button1_Click(sender, e);




        }







        #region UOSC 与Unity通讯

        void 关闭所有网络服务()
        {
            try
            {
                if (监听服务器 != null && !监听服务器.关闭)
                {
                    监听服务器.关闭线程();
                }

                if (服务端监听 != null)
                {
                    if (服务端监听.isStarted_)
                    {

                        服务端监听.StopServer();

                    }
                }
                if (客户端发送 != null)
                {
                    if (客户端发送.isStarted_)
                    {
                        客户端发送.StopClient();

                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        ServerUOSC 服务端监听;
        ClientUOSC 客户端发送;
        class 监听类
        {
            public bool 关闭 = false;
            System.Threading.Thread 监听;
            public 监听类(System.Threading.ThreadStart 服务端监听Update)
            {
                监听 = new System.Threading.Thread(服务端监听Update);
                监听.Start();
            }
            public void 关闭线程()
            {
                关闭 = true;
                监听.Abort();

            }

        }

        监听类 监听服务器;
        private void 开启服务器接收消息button1_Click(object sender, EventArgs e)
        {

            if (服务端监听 == null)
            {
                服务端监听 = new ServerUOSC(int.Parse(textBox服务器监听端口号.Text));
                服务端监听.DataReceiveEventLog += 服务端监听_DataReceiveEventLog;
                服务端监听.ServerStartEventLog += 服务端监听_ServerStartEventLog;
                服务端监听.ServerStopEventLog += 服务端监听_ServerStopEventLog;

            }

            if (服务端监听.isStarted_)
            {//已经开启服务，返回
                return;
            }
            else
            {
                服务端监听.StartServer();

            }


            DG启用Vrc = true;
          //  timer波形连续输出.Enabled = true;
            timer新定义波形输出控制.Enabled = true;
            groupBox36.Visible = true;
            groupBox107.Visible = true;

            button1.Visible = false;
            button3.Visible = true;
            timer写入蓝牙.Enabled = true;


        }
        private void 开启客户端发送消息服务button_Click(object sender, EventArgs e)
        {
            if (客户端发送 == null)
            {
                客户端发送 = new ClientUOSC(textBox客户端服务IP地址.Text, int.Parse(textbox客户端服务端口号.Text));
                客户端发送.ClientStartEventLog += 客户端发送_ClientStartEventLog;
                客户端发送.ClientStopEventLog += 客户端发送_ClientStopEventLog;
            }

            if (客户端发送.isStarted_)
            {//已经开启服务，返回
                return;
            }
            else
            {
                客户端发送.StartClient();
            }
        }
        private void 停止服务器接收消息button4_Click(object sender, EventArgs e)
        {
            服务端监听.StopServer();
            button1.Visible = true;
            button3.Visible = false;
            groupBox36.Visible = false;
            groupBox107.Visible = false;
            DG启用Vrc = false;
            //timer波形连续输出.Enabled = false;

            timer新定义波形输出控制.Enabled = false;
            待执行波形列表.Clear();
            timer写入蓝牙.Enabled = true;
        }

        private void 服务端监听_ServerStopEventLog(int e)
        {
            更新服务器消息文本框("Stop Server:" + e.ToString());
            监听服务器.关闭线程();
        }

        private void 服务端监听_ServerStartEventLog(int e)
        {
            更新服务器消息文本框("Start Server:" + e.ToString());
            监听服务器 = new 监听类(服务端监听Update);

        }
        void 服务端监听Update()
        {
            while (!监听服务器.关闭)
            {
                if (服务端监听 != null)
                {
                    if (服务端监听.isStarted_)
                    {//已经开启服务， 
                        服务端监听.Update();
                    }
                }
                Thread.Sleep(10);
            }


        }
        Dictionary<string, string> data = new Dictionary<string, string>();

        List<string> dataKeys = new List<string>();

        private void 服务端监听_DataReceiveEventLog(uOSC.Message e)
        {

            if (服务端监听 != null && 服务端监听.isStarted_)
            {
                // var msg = e.address + ": ";

                // timestamp
                // msg += "(" + e.timestamp.ToLocalTime() + ") ";

                // values
                // foreach (var value in e.values)
                //  {
                //      msg += value.GetString() + " ";
                //  }



                if (!监听服务器.关闭 && e.address != null && e.values != null)
                {
                    if (data.Keys.Contains(e.address))
                    {
                        data[e.address] = e.values[0].ToString();

                    }
                    else
                    {
                        data.Add(e.address, e.values[0].ToString());
                        dataKeys.Add(e.address);
                    }

                }
                // 更新服务器message包文本框(e.address, e.values[0].ToString ());
                //Thread.Sleep(sleeptime);
            }

        }

        private void 客户端发送_ClientStopEventLog(string ip, int p)
        {
            更新服务器消息文本框("Stop Client:" + ip + ":" + p.ToString());
        }

        private void 客户端发送_ClientStartEventLog(string ip, int p)
        {
            更新服务器消息文本框("Start Client:" + ip + ":" + p.ToString());
        }

        private void 客户端发送消息button2_Click(object sender, EventArgs e)
        {
            // SCM.SendMsg(textBox3.Text);
            if (客户端发送 != null)
            {
                if (客户端发送.isStarted_)
                {//已经开启服务， 

                    客户端发送.Send("/uOSC/test", 10, "hoge", "hogehoge", 1.234f, 123f, true, false);

                }

            }



        }


        void 更新服务器消息文本框(string a)
        {
            try
            {
                if (textBox服务器收到的消息.IsHandleCreated)
                {
                    textBox服务器收到的消息.Invoke((Action)(() =>
                    {

                        textBox服务器收到的消息.Text += a;
                    }
                    ), null);


                }


            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "textBox服务器收到的消息(string a)"); }



        }


        void 更新服务器message包文本框(string a, string b)
        {
            try
            {
                if (监听服务器 != null && !监听服务器.关闭 && textBox服务器收到的消息.IsHandleCreated)
                {
                    textBox服务器收到的消息.Invoke((Action)(() =>
                    {


                        识别参数并更新到参数列表(a, b);


                    }
                ), null);
                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message+ "textBox8Add(string a)");
            }



        }
        string parameters = "";
        string value = "";

        Match m;
        string assd = "";


        Dictionary<string, string> vrc参数 = new Dictionary<string, string>();

        private void 识别参数并更新到参数列表(string a, string b)
        {
            if (
                a.Contains("VelocityX") ||
                a.Contains("VelocityZ") ||
                a.Contains("VelocityY") ||
                a.Contains("AngularY") ||
                a.Contains("AngularX") ||
                a.Contains("AngularZ")
                )
            {
                return;
            }





            // 限制文本框最大行数(textBox服务器收到的消息, 50, a);

            //osc vrchat参数 VelocityX

            // / avatar / parameters / ClothesSwitch: (1900 / 1 / 1 8:00:00) 0
            // / avatar / parameters / ClothesSwitch: (1900 / 1 / 1 8:00:00) 1
            // / avatar / parameters / BreastsBig: (1900 / 1 / 1 8:00:00) 0
            // / avatar / parameters / BreastsBig: (1900 / 1 / 1 8:00:00) 0.005928122


            parameters = a.Replace("/avatar/parameters/", "");

            value = b;


            if (parameters != "" && value != "" && vrc参数.Count > 0)
            {
                if (vrc参数.Keys.Contains<string>(parameters))
                {
                    vrc参数[parameters] = value;
                }
                else
                {
                    vrc参数.Add(parameters, value);
                }

            }
            else
            {
                vrc参数.Add(parameters, value);
            }


            textBox5.Clear();
            foreach (string aaa in vrc参数.Keys)
            {
                textBox5.AppendText(aaa + ":" + vrc参数[aaa] + "\r\n");


                获得参数变更(aaa, vrc参数[aaa]);
            }

        }
        private void 限制文本框最大行数(TextBox myTextBox, int maxLines, string a)
        {
            string[] b = myTextBox.Lines;

            if (myTextBox.Lines.Length + 1 >= maxLines)
            {
                myTextBox.Text = a + "\r\n";

                for (int i = 1; i < maxLines; i++)
                {
                    myTextBox.Text += b[i - 1] + "\r\n";
                }


            }
            else
                myTextBox.Text = a + "\r\n" + myTextBox.Text;

        }

        #endregion

        #region 蓝牙于郊狼通讯
        private void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs eventArgs)
        {
            Int16 rssi = eventArgs.RawSignalStrengthInDBm;
            ulong address = eventArgs.BluetoothAddress;
            String name = eventArgs.Advertisement.LocalName.ToString();
            string addr_str = BitConverter.ToString(BitConverter.GetBytes(address), 0, 6).Replace('-', ':').ToLower();

            if (eventArgs.IsScanResponse == true)
            {
                if (res_datas.ContainsKey(address)) res_datas.Remove(address);
                res_datas.Add(address, eventArgs.Advertisement);
            }
            else
            {
                if (adv_datas.ContainsKey(address)) adv_datas.Remove(address);
                adv_datas.Add(address, eventArgs.Advertisement);
            }

            if (None_Name_checkBox.Checked)
            {
                if (name.Length == 0) { return; }
                if (name.Contains(Name_Filter_textBox.Text) == false) { return; }
            }

            if (listview_indexs.TryGetValue(address, out int i))
            {
                if (BLE_Device_listView.Items.Count == 0) return;
                BLE_Device_listView.Items[i].SubItems[1].Text = rssi.ToString();
                if (name.Length > 0) BLE_Device_listView.Items[i].SubItems[3].Text = name;
                return;
            }

            ListViewItem item = new ListViewItem();
            item.Text = addr_str;
            item.Tag = address;
            item.SubItems.Add(rssi.ToString());
            item.SubItems.Add(eventArgs.IsConnectable ? "是" : "否");
            item.SubItems.Add(name);
            BLE_Device_listView.Items.Add(item);

            listview_indexs.Add(address, listview_index);
            listview_index++;
        }

        public delegate void Gatt_Tree_Add_Server_delegate(string s);
        public void Gatt_Tree_Add_Server(string s)
        {
            TreeNode newNode2 = new TreeNode("服务:" + s);
            newNode2.Tag = -1;
            Gatt_treeView.Nodes.Add(newNode2);
            Gatt_treeView.Select();
        }

        public delegate void Gatt_Tree_Add_Character_delegate(string s, int tag);
        public void Gatt_Tree_Add_Character(string s, int tag)
        {
            int i = this.Gatt_treeView.Nodes.Count - 1;
            TreeNode selectedNode = null;

            if (i < 0)
                selectedNode = Gatt_treeView.TopNode;
            else selectedNode = this.Gatt_treeView.Nodes[i];

            if (selectedNode == null)
            {
                Console.WriteLine("Nul;l");
                return;
            }
            TreeNode newNode2 = new TreeNode("特性:" + s);
            newNode2.Tag = tag;
            selectedNode.Nodes.Add(newNode2);
            selectedNode.Expand();
        }

        private void Connect_Device(BluetoothLEDevice device)
        {
            device.GetGattServicesAsync().Completed = (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    CurrentDevice = device;
                    var services = asyncInfo.GetResults().Services;
                    Console.WriteLine("GattServices size=" + services.Count);
                    foreach (GattDeviceService ser in services)
                    {
                        Console.WriteLine(ser.Uuid.ToString());
                        Gatt_treeView.Invoke(new Gatt_Tree_Add_Server_delegate(Gatt_Tree_Add_Server), ser.Uuid.ToString());
                        FindCharacteristic(ser);
                    }
                    Connect_Btn.BackColor = Color.LightGreen;
                    Connect_Btn.Enabled = false;
                }
            };
        }

        private void FindCharacteristic(GattDeviceService gattDeviceService)
        {
            //this.CurrentService = gattDeviceService;
            gattDeviceService.GetCharacteristicsAsync().Completed = (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    var services = asyncInfo.GetResults().Characteristics;
                    foreach (var c in services)
                    {
                        Console.WriteLine(c.Uuid.ToString() + "," + c.GetAllDescriptors().ToString() + c.CharacteristicProperties.ToString());

                        Gatt_treeView.Invoke(new Gatt_Tree_Add_Character_delegate(Gatt_Tree_Add_Character), new object[] { c.Uuid.ToString() + " (" + c.CharacteristicProperties.ToString() + ")", Character_index });
                        CurrentCharacteristicList.Add(c);
                        Character_index++;
                    }
                }
            };
        }

        private void write(byte[] data)
        {
            int index = (int)Gatt_treeView.SelectedNode.Tag;
            Console.WriteLine("Select:" + Gatt_treeView.SelectedNode.Tag.ToString());

            if (index == -1)
            {
                Log_textBox.AppendText("\r\n你选择的是服务，请选择一个可以Write的特性~");
                return;
            }
            if ((CurrentCharacteristicList[index].CharacteristicProperties & (GattCharacteristicProperties.Write | GattCharacteristicProperties.WriteWithoutResponse)) == 0)
            {
                Log_textBox.AppendText("\r\n你选择的特性不可写，请选择一个可以Write的特性~");
                return;
            }
            CurrentCharacteristicList[index].WriteValueAsync(CryptographicBuffer.CreateFromByteArray(data), GattWriteOption.WriteWithResponse);
            string str = CurrentCharacteristicList[index].Uuid.ToString() + " Tx:\r\n" + BitConverter.ToString(data) + "\r\n";
            Log_textBox.AppendText(str);
            string[] hexs = BitConverter.ToString(data).Split('-');
            foreach (string a in hexs)
            {
                textBox1.AppendText(int.Parse(a, System.Globalization.NumberStyles.HexNumber).ToString() + " ");
            }
            textBox1.AppendText("\r\n");


            foreach (byte aa in data)
            {
                textBox2.AppendText(aa.ToString() + " ");
            }

            textBox2.AppendText("\r\n");

            蓝牙消息识别(data);

        }

        void 蓝牙消息识别(byte[] data)
        {
            string a = Convert.ToString(data[0], 16).ToLower();


            /*
            序列号
            序列号范围(0b0000 ~0b1111),如果输入的数据中修改了脉冲主机的通道强度，设置序列号 > 0,
            脉冲主机会通过B1回应消息以相同的序列号将修改后的通道强度从特性0x150B返回，
            如果不需要脉冲主机反馈通道强度，则序列号设置0b0000即可。 
            另外，为避免产生问题，当通过B0指令修改通道强度且序列号不为0时，
            建议等待150B返回B1且为相同序列号的信息后，再对通道强度进行修改。

            强度值解读方式
            强度值解读方式的4bits分为两个部分，高两位bits代表A通道解读方式，低两位bits代表B通道解读方式。
            解读方式:
            0b00->代表对应通道强度不做改变，对应通道的强度设定值无论是什么都无效
            0b01->代表对应通道强度相对增加变化，若A通道强度设定值为15(10进制)，那么A通道强度增加15
            0b10->代表对应通道强度相对减少变化，若A通道强度设定值为17(10进制)，那么A通道强度减少17
            0b11->代表对应通道强度绝对变化，若A通道强度设定值为32，那么A通道强度设置为32

            */


            if (a == "b0")
            {
                textBox7.AppendText(a + "B0指令 长度：" + data.Length.ToString() + "\r\n");
                // 0 0xB0(1byte指令HEAD) + 0xB0
                // 1    序列号(4bits) +0000 
                // 1    强度值解读方式(4bits) +0000
                // 2     A通道强度设定值(1byte) + 
                // 3     B通道强度设定值(1byte) + 
                //     A通道波形频率4条(4bytes) +
                //      A通道波形强度4条(4bytes) + 
                //      B通道波形频率4条(4bytes) + 
                //     B通道波形强度4条(4bytes)
                //   B0 BF  B1消息 BE消息
                //1F8B0800000000000000F30D092CF6CB0A2DF6752EAF44C2C63E2E81C5BE99E506BEC1400C11ABF071712D06F281B463B16F4868B13F50CCBBCAB5C437CBB1D41724E66C005467520E561BE259EE93E5580ED45302152BF70981AB43932B4652EF88D73CB07D68620098A633D5C4000000
                //   0,0,0,100,100,100,2,2,2,100,100,100,1,1,1,1,1,10,20 || 20 = 1,20 = 1 |


            }
            else if (a == "bf")
            {
                textBox7.AppendText(a + "BF指令 长度：" + data.Length.ToString() + "\r\n");
                //0xBF(1byte指令HEAD) + 
                // AB两通道强度软上限(2bytes) + 
                //AB两通道波形频率平衡参数(2btyes) + 
                // AB两通道波形强度平衡参数(2bytes)

            }
            else if (a == "b1")
            {
                textBox7.AppendText("序列号：" + Convert.ToString(data[1], 2) + "A强度：" + Convert.ToString(data[2], 10) + "B强度：" + Convert.ToString(data[3], 10) + "\r\n");
                textBox7.AppendText(a + "B1指令 长度：" + data.Length.ToString() + "\r\n");
                // B1消息
                //  当脉冲主机强度发生变化时，会立刻通过B1消息返回当前的强度值。如果是由于B0指令导致的强度变化，返回B1指令中序列号将会与引起此变化的命令所包含的序列号相同，否则序列号为0。
                // 0xB1(1byte指令HEAD) + 
                // 序列号(1byte) + 
                // A通道当前实际强度(1byte) + 
                //  B通道当前实际强度(1byte)
                if(DG启用Vrc)
                   收到单次回应 = true;
                if (!DG启用Vrc)
                   波形循环触发测试();

            }
            else if (a == "be")
            {
                textBox7.AppendText(a + "BE指令 长度：" + data.Length.ToString() + "\r\n");
                //BE消息返回BF输入的对应设置后脉冲主机当前的AB通道强度软上限 + AB通道波形频率平衡参数 + AB通道波形强度平衡参数。
                //  0xBE(1byte指令HEAD) + 
                //  AB两通道强度软上限(2bytes) +
                //  AB两通道波形频率平衡参数(2btyes) + 
                //  AB两通道波形强度平衡参数(2bytes)

            }














        }


        #endregion

        #region 蓝牙郊狼方法


        private void BLE_Scan_Start_Btn_Click(object sender, EventArgs e)
        {
            listview_indexs.Clear();
            listview_index = 0;
            adv_datas.Clear();
            res_datas.Clear();
            BLE_Device_listView.Items.Clear();
            Watcher.Start();
            BLE_Scan_Start_Btn.Enabled = false;
        }

        private void BLE_Scan_Stop_Btn_Click(object sender, EventArgs e)
        {
            BLE_Scan_Start_Btn.Enabled = true;
            Watcher.Stop();
        }

        private void Connect_Btn_Click(object sender, EventArgs e)
        {
            Watcher.Stop();

            if (BLE_Device_listView.SelectedItems.Count == 0)
            {
                Log_textBox.AppendText("请先选着一个设备！！！\r\n");
                return;
            }
            ulong address = (ulong)BLE_Device_listView.SelectedItems[0].Tag;

            CurrentCharacteristicList.Clear();
            Character_index = 0;
            Gatt_treeView.Nodes.Clear();

            BluetoothLEDevice.FromBluetoothAddressAsync(address).Completed = (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    if (asyncInfo.GetResults() == null) { Log_textBox.AppendText("连接设备失败！！！\r\n"); }
                    else { Connect_Device(asyncInfo.GetResults()); }
                }
                else
                {
                    Log_textBox.AppendText("连接设备失败！！！\r\n");
                }
            };
        }

        private void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            byte[] data;
            CryptographicBuffer.CopyToByteArray(args.CharacteristicValue, out data);
            string str = BitConverter.ToString(data).Replace('-', ' ');
            Console.WriteLine(str);

            Log_textBox.AppendText(sender.Uuid.ToString() + " Rx: \r\n");
            Log_textBox.AppendText(str + "\r\n");

            string[] hexs = str.Split(' ');
            foreach (string a in hexs)
            {
                textBox1.AppendText(int.Parse(a, System.Globalization.NumberStyles.HexNumber).ToString() + " ");
            }

            textBox1.AppendText("\r\n");


            foreach (byte aa in data)
            {

                textBox2.AppendText(aa.ToString() + " ");

            }

            textBox2.AppendText("\r\n");
            蓝牙消息识别(data);

        }

        private void Notify_Btn_Click(object sender, EventArgs e)
        {
            int index = (int)Gatt_treeView.SelectedNode.Tag;
            Console.WriteLine("Select:" + Gatt_treeView.SelectedNode.Tag.ToString());
            CurrentCharacteristicList[index].ValueChanged += Characteristic_ValueChanged;
            CurrentCharacteristicList[index].WriteClientCharacteristicConfigurationDescriptorAsync(CHARACTERISTIC_NOTIFICATION_TYPE).Completed = async (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    GattCommunicationStatus status = asyncInfo.GetResults();
                    if (status == GattCommunicationStatus.Unreachable)
                    {
                        Console.WriteLine("设备不可用");
                    }
                }
            };
        }

        private void Gatt_treeView_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if ((e.State & TreeNodeStates.Selected) != 0)
            {
                Rectangle rect = e.Bounds;
                rect.Width = rect.Width + 10;
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0, 122, 204)), rect);
                Font nodeFont = e.Node.NodeFont;
                if (nodeFont == null) nodeFont = ((TreeView)sender).Font;
                e.Graphics.DrawString(e.Node.Text, nodeFont, Brushes.White, Rectangle.Inflate(rect, 2, 0));
            }
            else
            {
                e.DrawDefault = true;
            }
        }

        private void Send_Btn_Click(object sender, EventArgs e)
        {
            byte[] data = null;

            if (HEX_checkBox.Checked)
            {
                data = HexStringToByteArray(Send_textBox.Text);

            }
            else
            {
                data = System.Text.Encoding.UTF8.GetBytes(Send_textBox.Text);
            }
            write(data);
        }

        private void button5B1_Click(object sender, EventArgs e)
        {
            byte[] buffer = new byte[20];

            buffer[0] = (byte)Convert.ToByte(B0指令头.Text, 16);
            buffer[1] = (byte)Convert.ToByte(B0序列号.Text + A强.Text.Split('|')[1] + B强.Text.Split('|')[1], 2);
            buffer[2] = Convert.ToByte(A值.Value);
            buffer[3] = Convert.ToByte(B值.Value);

            buffer[4] = Convert.ToByte(A1_1.Value);
            buffer[5] = Convert.ToByte(A1_2.Value);
            buffer[6] = Convert.ToByte(A1_3.Value);
            buffer[7] = Convert.ToByte(A1_4.Value);

            buffer[8] = Convert.ToByte(A2_1.Value);
            buffer[9] = Convert.ToByte(A2_2.Value);
            buffer[10] = Convert.ToByte(A2_3.Value);
            buffer[11] = Convert.ToByte(A2_4.Value);

            buffer[12] = Convert.ToByte(B1_1.Value);
            buffer[13] = Convert.ToByte(B1_2.Value);
            buffer[14] = Convert.ToByte(B1_3.Value);
            buffer[15] = Convert.ToByte(B1_4.Value);

            buffer[16] = Convert.ToByte(B2_1.Value);
            buffer[17] = Convert.ToByte(B2_2.Value);
            buffer[18] = Convert.ToByte(B2_3.Value);
            buffer[19] = Convert.ToByte(B2_4.Value);




            write(buffer);


        }

        private void button6BF_Click(object sender, EventArgs e)
        {
            byte[] buffer = new byte[7];
            //0xBF(1byte指令HEAD) + 
            // AB两通道强度软上限(2bytes) + 
            //AB两通道波形频率平衡参数(2btyes) + 
            // AB两通道波形强度平衡参数(2bytes)
            //  通道强度软上限
            //  通道强度软上限可以限制脉冲主机通道强度能达到的最大值，并且该设置断电保存，值范围(0~200)，
            //  输入范围外的值则不会修改软上限。 假设设置AB通道软上限为150和30，
            //  那么通过拨动滚轮或B0指令无论如何修改强度，
            //  A通道的通道强度只会在范围(0 ~150),
            //  B通道的通道强度只会在范围(0 ~30)，
            //  脉冲主机的通道强度一定不会超过软上限。

            //   频率平衡参数1
            //   波形频率平衡参数会调整波形高低频的感受，并且该设置断电保存，值范围(0 ~255)
            //   本参数控制固定通道强度下，不同频率波形的相对体感强度。值越大，低频波形冲击感越强。

            //   频率平衡参数2
            //   波形强度平衡参数会调整波形脉冲宽度，并且该设置断电保存，值范围(0 ~255)
            //   本参数控制固定通道强度下，不同频率波形的相对体感强度。值越大，低频波形刺激越强。

            buffer[0] = (byte)Convert.ToByte("0xBF", 16);
            buffer[1] = Convert.ToByte(A软上限设置.Value);
            buffer[2] = Convert.ToByte(B软上限设置.Value);
            buffer[3] = Convert.ToByte(A频率平衡.Value);
            buffer[4] = Convert.ToByte(B频率平衡.Value);
            buffer[5] = Convert.ToByte(A强度平衡.Value);
            buffer[6] = Convert.ToByte(B强度平衡.Value);


            write(buffer);
        }

        private void Clear_Log_Btn_Click(object sender, EventArgs e)
        {
            Log_textBox.Clear();
        }

        private void RSSI_trackBar_Scroll(object sender, EventArgs e)
        {
            int rssi_threshold = -140 - RSSI_trackBar.Value;
            RSSI_Lable.Text = rssi_threshold.ToString();
        }

        private void BLE_Device_listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (BLE_Device_listView.SelectedItems.Count == 0) return;
            ulong address = (ulong)BLE_Device_listView.SelectedItems[0].Tag;
            string addr_str = BitConverter.ToString(BitConverter.GetBytes(address), 0, 6).Replace('-', ':').ToLower();
            Log_textBox.AppendText("\r\n设备(" + addr_str + ")的原始广播数据：\r\n");
            Log_textBox.AppendText("广播数据:");
            if (adv_datas.TryGetValue(address, out var adv_data))
            {
                foreach (BluetoothLEAdvertisementDataSection ds in adv_data.DataSections)
                {
                    CryptographicBuffer.CopyToByteArray(ds.Data, out var context_buff);
                    string context = BitConverter.ToString(context_buff).Replace("-", " ").ToUpper() + " ";
                    Log_textBox.AppendText((context_buff.Length).ToString("X2") + " " + ds.DataType.ToString("X2") + " " + context);
                }
            }
            Log_textBox.AppendText("\r\n扫描响应:");
            if (res_datas.TryGetValue(address, out var res_data))
            {
                foreach (BluetoothLEAdvertisementDataSection ds in res_data.DataSections)
                {
                    CryptographicBuffer.CopyToByteArray(ds.Data, out var context_buff);
                    string context = BitConverter.ToString(context_buff).Replace("-", " ").ToUpper() + " ";
                    Log_textBox.AppendText((context_buff.Length).ToString("X2") + " " + ds.DataType.ToString("X2") + " " + context);
                }
            }
            Log_textBox.AppendText("\r\n");
        }

        private void RSSI_trackBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (BLE_Scan_Start_Btn.Enabled) return;
            int rssi_threshold = -140 - RSSI_trackBar.Value;
            Watcher.Stop();
            while (Watcher.Status != BluetoothLEAdvertisementWatcherStatus.Stopped) { Thread.Sleep(100); }
            Thread.Sleep(100);
            Watcher.SignalStrengthFilter.InRangeThresholdInDBm = (short)rssi_threshold;
            Watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = (short)(rssi_threshold - 10);
            BLE_Scan_Start_Btn_Click(null, null);
        }

        private void None_Name_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (BLE_Scan_Start_Btn.Enabled) return;
            Watcher.Stop();
            while (Watcher.Status != BluetoothLEAdvertisementWatcherStatus.Stopped) { Thread.Sleep(100); }
            Thread.Sleep(100);
            BLE_Scan_Start_Btn_Click(null, null);
        }

        private void Name_Filter_textBox_TextChanged(object sender, EventArgs e)
        {
            if (None_Name_checkBox.Checked == false) return;
            None_Name_checkBox_CheckedChanged(null, null);
        }

        private void Disconnect_Btn_Click(object sender, EventArgs e)
        {
            if (CurrentDevice == null) return;
            CurrentDevice.Dispose();
            Connect_Btn.Enabled = true;
        }


        #endregion

        #region 波形设置  滑块控制

        private void trackBar8_Scroll(object sender, EventArgs e)
        {
            label12.Text = A值.Value.ToString();
        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            label11.Text = B值.Value.ToString();
        }

        private void A强设置_Scroll(object sender, EventArgs e)
        {
            label3.Text = A强设置.Value.ToString();
        }

        private void B强设置_Scroll(object sender, EventArgs e)
        {
            label8.Text = B强设置.Value.ToString();
        }

        private void A软上限设置_Scroll(object sender, EventArgs e)
        {
            label6.Text = A软上限设置.Value.ToString();
        }

        private void B软上限设置_Scroll(object sender, EventArgs e)
        {
            label7.Text = B软上限设置.Value.ToString();
        }

        private void A频率平衡_Scroll(object sender, EventArgs e)
        {
            label16.Text = A频率平衡.Value.ToString();
        }

        private void A强度平衡_Scroll(object sender, EventArgs e)
        {
            label15.Text = A强度平衡.Value.ToString();
        }

        private void B频率平衡_Scroll(object sender, EventArgs e)
        {
            label18.Text = B频率平衡.Value.ToString();
        }

        private void B强度平衡_Scroll(object sender, EventArgs e)
        {
            label17.Text = B强度平衡.Value.ToString();

        }



        private void A1_1_Scroll(object sender, EventArgs e)
        {
            label19.Text = A1_1.Value.ToString();
        }

        private void A1_2_Scroll(object sender, EventArgs e)
        {
            label20.Text = A1_2.Value.ToString();
        }

        private void A1_3_Scroll(object sender, EventArgs e)
        {
            label21.Text = A1_3.Value.ToString();
        }

        private void A1_4_Scroll(object sender, EventArgs e)
        {
            label22.Text = A1_4.Value.ToString();
        }

        private void A2_1_Scroll(object sender, EventArgs e)
        {
            label26.Text = A2_1.Value.ToString();
        }

        private void A2_2_Scroll(object sender, EventArgs e)
        {
            label25.Text = A2_2.Value.ToString();
        }

        private void A2_3_Scroll(object sender, EventArgs e)
        {
            label24.Text = A2_3.Value.ToString();
        }

        private void A2_4_Scroll(object sender, EventArgs e)
        {
            label23.Text = A2_4.Value.ToString();
        }

        private void B1_1_Scroll(object sender, EventArgs e)
        {
            label30.Text = B1_1.Value.ToString();
        }

        private void B1_2_Scroll(object sender, EventArgs e)
        {
            label29.Text = B1_2.Value.ToString();
        }

        private void B1_3_Scroll(object sender, EventArgs e)
        {
            label28.Text = B1_3.Value.ToString();
        }

        private void B1_4_Scroll(object sender, EventArgs e)
        {
            label27.Text = B1_4.Value.ToString();
        }

        private void B2_1_Scroll(object sender, EventArgs e)
        {
            label34.Text = B2_1.Value.ToString();
        }
        private void B2_2_Scroll(object sender, EventArgs e)
        {
            label33.Text = B2_2.Value.ToString();
        }

        private void B2_3_Scroll(object sender, EventArgs e)
        {
            label32.Text = B2_3.Value.ToString();
        }

        private void B2_4_Scroll(object sender, EventArgs e)
        {
            label31.Text = B2_4.Value.ToString();
        }
        #endregion

        #region 绘制音乐图谱


        private void 发送音乐波形()
        {

            string a = textBox6.Text;//提取音量最高的波形前4后3 强度波形一致
            int maxindex = 0;
            int count = 0;
            int max = 0;
            foreach (char c in a)
            {

                if (int.Parse(c.ToString()) > max)
                {
                    max = int.Parse(c.ToString());
                    maxindex = count;
                }
                count++;
            }


            int v1 = 0;
            int v2 = 0;
            int v3 = 0;
            int v4 = 0;
            //0~200<0~9
            if (maxindex < 4)
            {
                v1 = int.Parse(a[maxindex].ToString());
                v2 = int.Parse(a[maxindex + 1].ToString());
                v3 = int.Parse(a[maxindex + 2].ToString());
                v4 = int.Parse(a[maxindex + 3].ToString());
            }
            else
            {
                v1 = int.Parse(a[maxindex - 1].ToString());
                v2 = int.Parse(a[maxindex].ToString());
                v3 = int.Parse(a[maxindex + 1].ToString());
                v4 = int.Parse(a[maxindex + 2].ToString());
            }

            int v12 = 限制频率范围(v1 * 18);
            int v22 = 限制频率范围(v2 * 18);
            int v32 = 限制频率范围(v3 * 18);
            int v42 = 限制频率范围(v4 * 18);

            byte[] buffer = new byte[20];

            buffer[0] = (byte)Convert.ToByte(B0指令头.Text, 16);
            buffer[1] = (byte)Convert.ToByte(B0序列号.Text + A强.Text.Split('|')[1] + B强.Text.Split('|')[1], 2);
            buffer[2] = Convert.ToByte(A值.Value);
            buffer[3] = Convert.ToByte(B值.Value);

            buffer[4] = Convert.ToByte(v12);
            buffer[5] = Convert.ToByte(v22);
            buffer[6] = Convert.ToByte(v32);
            buffer[7] = Convert.ToByte(v42);



            v12 = 限制强度范围(v1 * 6);
            v22 = 限制强度范围(v2 * 6);
            v32 = 限制强度范围(v3 * 6);
            v42 = 限制强度范围(v4 * 6);



            buffer[8] = Convert.ToByte(v12);
            buffer[9] = Convert.ToByte(v22);
            buffer[10] = Convert.ToByte(v32);
            buffer[11] = Convert.ToByte(v42);

            buffer[12] = Convert.ToByte(B1_1.Value);
            buffer[13] = Convert.ToByte(B1_2.Value);
            buffer[14] = Convert.ToByte(B1_3.Value);
            buffer[15] = Convert.ToByte(B1_4.Value);

            buffer[16] = Convert.ToByte(B2_1.Value);
            buffer[17] = Convert.ToByte(B2_2.Value);
            buffer[18] = Convert.ToByte(B2_3.Value);
            buffer[19] = Convert.ToByte(B2_4.Value);

            write(buffer);
        }


        private void dataTimer_Tick(object sender, EventArgs e)
        {
            drawAudio.DataTimer_Tick(sender, e);
        }

        private void drawingTimer_Tick(object sender, EventArgs e)
        {
            drawAudio.倍率 = trackBar1.Value;
            drawAudio.微调倍率 = trackBar2.Value;
            drawAudio.DrawingTimer_Tick(sender, e);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            BLE_Scan_Start_Btn_Click(sender, e);



        }
        bool 使用音乐波形 = false;
        private void button8_Click(object sender, EventArgs e)
        {
           //timer波形连续输出.Enabled = true;
            timer新定义波形输出控制.Enabled = true;
            使用音乐波形 = true;



        }
        #endregion

        int 测试时间 = 0;
        int 最大测试时间 = 30;
        bool 开始测试 = false;


        byte[] 测试波形 = null;



        bool DG启用Vrc执行完毕 = true;
        string DG启用Vrc名字 = "";
        string DG启用Vrc名字2 = "";




        DateTime timeA = DateTime.Now;
        DateTime timeB = DateTime.Now;
        private void timer波形连续输出_Tick(object sender, EventArgs e)
        {
            if (DG启用Vrc)
            {   //vrc启用中不可测试

                if(待执行波形列表.Count >200)
                {//防止过多命令堆积
                    待执行波形列表.RemoveRange(0,160); 
                }
                 

                DG启用Vrc名字 = 依次获取DG启用Vrc名字();//每轮更新


                if (DG启用Vrc名字 != "" && DG启用Vrc名字 != "停止" && listBox2.Items.Contains(DG启用Vrc名字))
                {

                    if (待执行波形列表.Count == 0)
                    {
                        byte[] datago;
                        for (int i = 0; i < a郊狼存档.DG波形队列[DG启用Vrc名字].波形队列User包含全部格式的英文Tag.Count; i++)
                        {
                            //更新AB通道强度
                            datago = string转byte波形(
                                    a郊狼存档.DG波形队列[DG启用Vrc名字].波形队列User包含全部格式的英文Tag[i]
                                    );

                            if (checkBox2.Checked)
                            {
                                datago = 实时更新强度(datago, float.Parse(textBoxA通强度值.Text), float.Parse(textBoxB通强度值.Text));
                            }



                            待执行波形列表.Add(datago);

                        }
                        波形循环触发测试();


                    }


                }

                if (DG启用Vrc名字 == "停止")
                {
                    待执行波形列表.Clear();
                    DG启用Vrc = false;
                    groupBox36.Visible = false;

                    button19.Visible = false;
                    button15.Visible = true;
                }


            }
            else
            {
                //可用测试

                if (开始测试)
                {
                    if (测试时间 > 最大测试时间)
                    {
                        测试时间 = 0;
                        开始测试 = false;
                    }
                    write(测试波形);
                    测试时间++;
                }
                else if (DG播放队列)
                {
                    if (listBox2.SelectedItems.Count <= 0)
                    {
                        MessageBox.Show("请选择一个队列");
                        return;
                    }


                    if (测试时间 >= a郊狼存档.DG波形队列[listBox2.SelectedItem.ToString()].波形队列User包含全部格式的英文Tag.Count)
                    {
                        测试时间 = 0;
                        DG播放队列 = false;
                    }


                    write(
                        string转byte波形(
                            a郊狼存档.DG波形队列[listBox2.SelectedItem.ToString()].波形队列User包含全部格式的英文Tag[测试时间]
                            )
                        );




                    测试时间++;



                }
                else if (DG播放波形)
                {

                    if (listBox1.SelectedItems.Count <= 0)
                    {
                        MessageBox.Show("请选择一个波形");
                        return;
                    }

                    if (测试时间 > 最大测试时间)
                    {
                        测试时间 = 0;
                        DG播放波形 = false;
                    }
                    write(string转byte波形(a郊狼存档.DG波形列表[listBox1.SelectedItem.ToString()]));
                    测试时间++;
                }
                else if (使用音乐波形)
                {
                    发送音乐波形();
                }



            }


        }

        public static string GetTime(DateTime timeA)
        {
            //timeA 表示需要计算
            DateTime timeB = DateTime.Now;	//获取当前时间
            TimeSpan ts = timeB - timeA;	//计算时间差
            string time = ts.TotalSeconds.ToString();	//将时间差转换为秒
            return time;
        }

        private string 依次获取DG启用Vrc名字()
        {
            //玩法触发参数预览
            //首先全部清除，依照触发再切
            checkBox配1.Checked = false;
            checkBox配2.Checked = false;
            checkBox配3.Checked = false;
            checkBox配4.Checked = false;
            checkBox配5.Checked = false;


            if (符合触发条件(comboBox配置条件1, textBox触发值1))
            {
                checkBox配1.Checked = true;
                return comboBox配置1.Text;
            }
            else if (符合触发条件(comboBox配置条件2, textBox触发值2))
            {
                checkBox配2.Checked = true;
                return comboBox配置2.Text;
            }
            else if (符合触发条件(comboBox配置条件3, textBox触发值3))
            {
                checkBox配3.Checked = true;
                return comboBox配置3.Text;
            }
            else if (符合触发条件(comboBox配置条件4, textBox触发值4))
            {
                checkBox配4.Checked = true;
                return comboBox配置4.Text;
            }
            else if (符合触发条件(comboBox配置条件5, textBox触发值5))
            {
                checkBox配5.Checked = true;
                return comboBox配置5.Text;
            }

            return "";

        }

        private string 依次获取DG启用Vrc名字2()
        {
            //玩法触发参数预览
            //首先全部清除，依照触发再切
            checkBox配12.Checked = false;
            checkBox配22.Checked = false;
            checkBox配32.Checked = false;
            checkBox配42.Checked = false;
            checkBox配52.Checked = false;


            if (符合触发条件(comboBox配置条件12, textBox触发值12))
            {
                checkBox配12.Checked = true;
                return comboBox配置12.Text;
            }
            else if (符合触发条件(comboBox配置条件22, textBox触发值22))
            {
                checkBox配22.Checked = true;
                return comboBox配置22.Text;
            }
            else if (符合触发条件(comboBox配置条件32, textBox触发值32))
            {
                checkBox配32.Checked = true;
                return comboBox配置32.Text;
            }
            else if (符合触发条件(comboBox配置条件42, textBox触发值42))
            {
                checkBox配42.Checked = true;
                return comboBox配置42.Text;
            }
            else if (符合触发条件(comboBox配置条件52, textBox触发值52))
            {
                checkBox配52.Checked = true;
                return comboBox配置52.Text;
            }

            return "";

        }



        private bool 符合触发条件(ComboBox comboBox配置条件1, TextBox 触发值)
        {
            float cc = 0.0f;
            switch (comboBox配置条件1.Text.Trim())
            {
                case "true/1执行":

                    if (触发值.Text.Trim() != "" && (触发值.Text.Trim().ToLower() == "true" || 触发值.Text.Trim() == "1" || 触发值.Text.Trim().StartsWith("1.0")))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case "false/0执行":

                    if (触发值.Text.Trim() != "" && (触发值.Text.Trim().ToLower() == "false" || 触发值.Text.Trim() == "0" || 触发值.Text.Trim().StartsWith("0.0")))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case "大于0.5执行":
                    cc = 0.0f;
                    if (float.TryParse(触发值.Text.Trim(), out cc))
                    {
                        if (触发值.Text.Trim() != "" && cc > 0.5)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                case "小于0.5执行":
                    cc = 0.0f;
                    if (float.TryParse(触发值.Text.Trim(), out cc))
                    {
                        if (触发值.Text.Trim() != "" && float.Parse(触发值.Text.Trim()) < 0.5)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    }
                    else
                    {
                        return false;
                    }
                case "大于0.25执行":
                    cc = 0.0f;
                    if (float.TryParse(触发值.Text.Trim(), out cc))
                    {
                        if (触发值.Text.Trim() != "" && float.Parse(触发值.Text.Trim()) > 0.25)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                case "小于0.25执行":
                    cc = 0.0f;
                    if (float.TryParse(触发值.Text.Trim(), out cc))
                    {
                        if (触发值.Text.Trim() != "" && float.Parse(触发值.Text.Trim()) < 0.25)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                case "大于0.75执行":
                    cc = 0.0f;
                    if (float.TryParse(触发值.Text.Trim(), out cc))
                    {
                        if (触发值.Text.Trim() != "" && float.Parse(触发值.Text.Trim()) > 0.75)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                case "小于0.75执行":
                    cc = 0.0f;
                    if (float.TryParse(触发值.Text.Trim(), out cc))
                    {
                        if (触发值.Text.Trim() != "" && float.Parse(触发值.Text.Trim()) < 0.75)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                default:
                    return false;
            }

        }

        int 限制频率范围(int inv)
        {
            if (inv < 10)
            {
                inv = 10;
            }
            else if (inv > 240)
            {
                inv = 240;
            }
            return inv;

        }
        int 限制强度范围(int inv)
        {
            if (inv < 0)
            {
                inv = 0;
            }
            else if (inv > 100)
            {
                inv = 100;
            }
            return inv;

        }

        private void button13_Click(object sender, EventArgs e)
        {
            设置测试波形();

            待执行波形列表.Add(测试波形);
            波形循环触发测试();


        }


        void 设置测试波形()
        {
            byte[] buffer = new byte[20];

            buffer[0] = (byte)Convert.ToByte(B0指令头.Text, 16);
            buffer[1] = (byte)Convert.ToByte(B0序列号.Text + A强.Text.Split('|')[1] + B强.Text.Split('|')[1], 2);
            buffer[2] = Convert.ToByte(A值.Value);
            buffer[3] = Convert.ToByte(B值.Value);

            buffer[4] = Convert.ToByte(A1_1.Value);
            buffer[5] = Convert.ToByte(A1_2.Value);
            buffer[6] = Convert.ToByte(A1_3.Value);
            buffer[7] = Convert.ToByte(A1_4.Value);

            buffer[8] = Convert.ToByte(A2_1.Value);
            buffer[9] = Convert.ToByte(A2_2.Value);
            buffer[10] = Convert.ToByte(A2_3.Value);
            buffer[11] = Convert.ToByte(A2_4.Value);

            buffer[12] = Convert.ToByte(B1_1.Value);
            buffer[13] = Convert.ToByte(B1_2.Value);
            buffer[14] = Convert.ToByte(B1_3.Value);
            buffer[15] = Convert.ToByte(B1_4.Value);

            buffer[16] = Convert.ToByte(B2_1.Value);
            buffer[17] = Convert.ToByte(B2_2.Value);
            buffer[18] = Convert.ToByte(B2_3.Value);
            buffer[19] = Convert.ToByte(B2_4.Value);


            测试波形 = buffer;


        }
        int 第二阶段 = 0;

        private void button17_Click(object sender, EventArgs e)
        {
            一键启动按钮.Visible = false;
            BLE_Scan_Start_Btn_Click(sender, e);
            timer一键启动用.Enabled = true;

            第二阶段 = 0;

        }
        private TreeNode FindNode(TreeNode tnParent, string strValue)

        {

            if (tnParent == null) return null;

            if (tnParent.Text == strValue) return tnParent;



            TreeNode tnRet = null;

            foreach (TreeNode tn in tnParent.Nodes)

            {

                tnRet = FindNode(tn, strValue);

                if (tnRet != null) break;

            }

            return tnRet;

        }
        private void timer一键启动用_Tick(object sender, EventArgs e)
        {
            if (第二阶段 == 0)
            {
                for (int i = 0; i < BLE_Device_listView.Items.Count; i++)
                {
                    foreach (ListViewItem.ListViewSubItem a in BLE_Device_listView.Items[i].SubItems)
                    {
                        if (a.Text == "47L121000")
                        {
                            BLE_Device_listView.SelectedIndices.Add(i);
                            Connect_Btn_Click(sender, e);


                            第二阶段 = 1;
                            // MessageBox.Show("连接成功"); 
                            return;






                        }
                    }
                }

            }
            else if (第二阶段 == 1)
            {
                if (Gatt_treeView.Nodes.Count > 5)
                {
                    TreeNode tnRet = null;

                    foreach (TreeNode tn in Gatt_treeView.Nodes)

                    {

                        tnRet = FindNode(tn, "服务:0000180c-0000-1000-8000-00805f9b34fb\\特性:0000150b-0000-1000-8000-00805f9b34fb (Notify)");
                        tnRet = FindNode(tn, "特性:0000150b-0000-1000-8000-00805f9b34fb (Notify)");
                        // tnRet = FindNode(tn, "服务:0000180c-0000-1000-8000-00805f9b34fb");

                        if (tnRet != null) break;

                    }

                    if (tnRet != null)
                    {
                        Gatt_treeView.SelectedNode = tnRet;
                        Notify_Btn_Click(sender, e);




                        第二阶段 = 2;







                    }

                }

            }
            else if (第二阶段 == 2)
            {

                TreeNode tnRet = null;

                foreach (TreeNode tn in Gatt_treeView.Nodes)

                {

                    tnRet = FindNode(tn, "特性:0000150a-0000-1000-8000-00805f9b34fb (WriteWithoutResponse)");
                    // tnRet = FindNode(tn, "服务:0000180c-0000-1000-8000-00805f9b34fb");

                    if (tnRet != null) break;

                }

                if (tnRet != null)
                {
                    Gatt_treeView.SelectedNode = tnRet;

                    timer一键启动用.Enabled = false;
                    tabControl1.SelectedIndex = 2;


                    //测试.DGLab

                    // string pathss = System.Environment.CurrentDirectory.ToString() + "/测试.DGLab";

                    //导入(pathss);
                    一键启动设置上限();
                    刷新最大值最小值();
                    一键启动按钮.Enabled = false;
                    richTextBox1.Text = "链接成功！";


                    return;

                }

            }






        }
        void 一键启动设置上限()
        {
            byte[] buffer = new byte[7];
            //0xBF(1byte指令HEAD) + 
            // AB两通道强度软上限(2bytes) + 
            //AB两通道波形频率平衡参数(2btyes) + 
            // AB两通道波形强度平衡参数(2bytes)
            //  通道强度软上限
            //  通道强度软上限可以限制脉冲主机通道强度能达到的最大值，并且该设置断电保存，值范围(0~200)，
            //  输入范围外的值则不会修改软上限。 假设设置AB通道软上限为150和30，
            //  那么通过拨动滚轮或B0指令无论如何修改强度，
            //  A通道的通道强度只会在范围(0 ~150),
            //  B通道的通道强度只会在范围(0 ~30)，
            //  脉冲主机的通道强度一定不会超过软上限。

            //   频率平衡参数1
            //   波形频率平衡参数会调整波形高低频的感受，并且该设置断电保存，值范围(0 ~255)
            //   本参数控制固定通道强度下，不同频率波形的相对体感强度。值越大，低频波形冲击感越强。

            //   频率平衡参数2
            //   波形强度平衡参数会调整波形脉冲宽度，并且该设置断电保存，值范围(0 ~255)
            //   本参数控制固定通道强度下，不同频率波形的相对体感强度。值越大，低频波形刺激越强。

            buffer[0] = (byte)Convert.ToByte("0xBF", 16);
            buffer[1] = Convert.ToByte(int.Parse(textBoxA软上限.Text));

            labelA通软上限.Text = textBoxA软上限.Text;

            buffer[2] = Convert.ToByte(int.Parse(textBoxB软上限.Text));

            labelB通软上限.Text = textBoxB软上限.Text;

            buffer[3] = Convert.ToByte(int.Parse(textBoxA频率平衡参数.Text));
            buffer[4] = Convert.ToByte(int.Parse(textBoxB频率平衡参数.Text));
            buffer[5] = Convert.ToByte(int.Parse(textBoxA强度平衡参数.Text));
            buffer[6] = Convert.ToByte(int.Parse(textBoxB强度平衡参数.Text));
            write(buffer);
        }

        void 确认非空(ComboBox a, string b)
        {
            if (b != null && b.Trim() != "")
            {
                a.Text = b;
            }
        }

        void 确认非空(TextBox a, string b)
        {
            if (b != null && b.Trim() != "")
            {
                a.Text = b;
            }
        }





        控制ImageListView vv;




        private void 初始化波形队列()
        {

            vv = new 控制ImageListView(imageListView1);
            控制ImageListView.刷新选中tag缩略图 = 刷新选中tag缩略图;
            控制ImageListView.刷新选中ListBox缩略图 = 预览图ListBox刷新;


        }
        private void 刷新选中tag缩略图(ImageListViewItem path)
        {


        }
        private void 预览图ListBox刷新(string path)
        {
            string backstring = "";
            string aa = "";
            string selectedItem = "";
            // string filePath = "";
            string imageFilePath = "";

        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count <= 0)
            {
                MessageBox.Show("请选择一个波形");
                return;
            }



            Random ss = new Random();
            string bb = ss.Next(0, 9999).ToString();
            ImageListViewItem a = new ImageListViewItem("不使用" + vv.tag栏位列表ImageListView.Items.Count.ToString() + bb, listBox1.SelectedItem.ToString() + vv.tag栏位列表ImageListView.Items.Count.ToString() + bb, a郊狼存档.DG波形列表[listBox1.SelectedItem.ToString()], "不使用");
            vv.tag栏位列表ImageListView.Items.Add(a);
        }







        private void button9依照当前波形设置创建波形_Click(object sender, EventArgs e)
        {
            if (textBox8新建波形命名.Text.Trim() == "")
            {
                MessageBox.Show("请填写波形名称");
                return;
            }
            if (a郊狼存档.DG波形列表.Keys.Contains(textBox8新建波形命名.Text.Trim()))
            {
                MessageBox.Show("波形名称已存在，请更换一个");
                return;
            }
            a郊狼存档.DG波形列表.Add(textBox8新建波形命名.Text.Trim(), byte波形转string(获取波形设置()));
            listBox1.Items.Clear();
            foreach (string a in a郊狼存档.DG波形列表.Keys)
            {
                listBox1.Items.Add(a);
            }
        }


        byte[] 获取波形设置()
        {
            byte[] buffer = new byte[20];

            buffer[0] = (byte)Convert.ToByte(B0指令头.Text, 16);
            buffer[1] = (byte)Convert.ToByte(B0序列号.Text + A强.Text.Split('|')[1] + B强.Text.Split('|')[1], 2);
            buffer[2] = Convert.ToByte(A值.Value);
            buffer[3] = Convert.ToByte(B值.Value);

            buffer[4] = Convert.ToByte(A1_1.Value);
            buffer[5] = Convert.ToByte(A1_2.Value);
            buffer[6] = Convert.ToByte(A1_3.Value);
            buffer[7] = Convert.ToByte(A1_4.Value);

            buffer[8] = Convert.ToByte(A2_1.Value);
            buffer[9] = Convert.ToByte(A2_2.Value);
            buffer[10] = Convert.ToByte(A2_3.Value);
            buffer[11] = Convert.ToByte(A2_4.Value);

            buffer[12] = Convert.ToByte(B1_1.Value);
            buffer[13] = Convert.ToByte(B1_2.Value);
            buffer[14] = Convert.ToByte(B1_3.Value);
            buffer[15] = Convert.ToByte(B1_4.Value);

            buffer[16] = Convert.ToByte(B2_1.Value);
            buffer[17] = Convert.ToByte(B2_2.Value);
            buffer[18] = Convert.ToByte(B2_3.Value);
            buffer[19] = Convert.ToByte(B2_4.Value);


            return buffer;


        }



        string byte波形转string(byte[] a)
        {
            string returnss = "";
            foreach (byte b in a)
            {
                returnss += b.ToString() + " ";
            }
            return returnss.Trim();
        }
        byte[] string转byte波形(string a)
        {
            string[] b = a.Split(' ');
            byte[] bytes = new byte[b.Length];
            int i = 0;
            foreach (string b2 in b)
            {
                bytes[i] = Convert.ToByte(b2);
                i++;
            }
            return bytes;

        }
        byte[] string转byte波形AB(string a,string b, float A强度百分比, float B强度百分比)
        {

            if (a=="")
            {
                 
                string[] bb = b.Split(' ');
                byte[] bytesb = new byte[bb.Length];
                int ib = 0;
                foreach (string bbb in bb)
                {
                    bytesb[ib] = Convert.ToByte(bbb);
                    ib++;
                }
                bytesb[2] = Convert.ToByte(0);
                bytesb[3] = 同步强度B(bytesb[3], B强度百分比);
                bytesb[7] = Convert.ToByte(241);
                bytesb[8] = Convert.ToByte(101);
                 

                return bytesb;


            }
            else if (b== "")
            {

              
                string[] aa = a.Split(' ');
                byte[] bytes = new byte[aa.Length];
                int i = 0;
                foreach (string aaa in aa)
                {
                    bytes[i] = Convert.ToByte(aaa);
                    i++;
                }
                bytes[2] = 同步强度A(bytes[2], A强度百分比);
                bytes[3] = Convert.ToByte(0);
                bytes[15] = Convert.ToByte(241);
                bytes[16] = Convert.ToByte(101);  
                return bytes;



            }
            else
            {
                string[] aa = a.Split(' ');
                byte[] bytes = new byte[aa.Length];
                int i = 0;
                foreach (string aaa in aa)
                {
                    bytes[i] = Convert.ToByte(aaa);
                    i++;
                }


                string[] bb = b.Split(' ');
                byte[] bytesb = new byte[bb.Length];
                int ib = 0;
                foreach (string bbb in bb)
                {
                    bytesb[ib] = Convert.ToByte(bbb);
                    ib++;
                }
                bytes[2]= 同步强度A(bytes[2], A强度百分比);
                bytes[3] = 同步强度B(bytesb[3], B强度百分比);

                for (int iii = 12; iii < 20; iii++)
                {
                    bytes[iii] = bytesb[iii];
                }
                return bytes;
            }
 

        }


        byte 同步强度A(byte a, float A强度百分比)
        {
            textBoxA前.Text = Convert.ToInt32(a).ToString();

            int gg = (int)((float)Convert.ToInt32(a) * A强度百分比 * float.Parse(textBoxA通倍率.Text));

            if (gg > int.Parse(labelA通软上限.Text))
            {
                gg = int.Parse(labelA通软上限.Text);
            }
            textBoxA后.Text = gg.ToString();

            return Convert.ToByte(gg);

        }
        byte 同步强度B(byte b, float  B强度百分比)
        {
            textBoxB前.Text = Convert.ToInt32(b).ToString();
            int gg = (int)((float)Convert.ToInt32(b) * B强度百分比 * float.Parse(textBoxB通倍率.Text));

            if (gg > int.Parse(labelB通软上限.Text))
            {
                gg = int.Parse(labelB通软上限.Text);
            }

            textBoxB后.Text = gg.ToString();
            return Convert.ToByte(gg);

        }




        郊狼存档 a郊狼存档 = new 郊狼存档();

        private void button以以上名字新建队列_Click(object sender, EventArgs e)
        {
            if (textBox9波形队列名称.Text.Trim() == "")
            {
                MessageBox.Show("请填写波形队列名称");
                return;
            }
            if (a郊狼存档.DG波形队列.Keys.Contains(textBox9波形队列名称.Text.Trim()))
            {
                MessageBox.Show("波形队列名称已存在，请更换一个");
                return;
            }

            a郊狼存档.DG波形队列.Add(textBox9波形队列名称.Text.Trim(), new 波形队列序列化(vv));
            listBox2.Items.Clear();
            foreach (string a in a郊狼存档.DG波形队列.Keys)
            {
                listBox2.Items.Add(a);
            }




            填充vrc波形配置(true);


        }

        void 填充波形设置(byte[] buffer)
        {



            B0指令头.Text = Convert.ToString(buffer[0], 16);
            B0序列号.Text = Convert.ToString(buffer[1], 2).Substring(0, 4);
            string tst = Convert.ToString(buffer[1], 2).PadLeft(8, '0');
            string tstA = tst.Substring(4, 2);
            string tstB = tst.Substring(6, 2);
            for (int i = 0; i < A强.Items.Count; i++)
            {

                if (A强.Items[i].ToString().Contains(tstA))
                {
                    A强.Text = A强.Items[i].ToString();
                }


            }

            for (int i = 0; i < B强.Items.Count; i++)
            {
                if (B强.Items[i].ToString().Contains(tstB))
                {
                    B强.Text = B强.Items[i].ToString();
                }
            }
            A值.Value = Convert.ToInt32(buffer[2]);//  
            B值.Value = Convert.ToInt32(buffer[3]);//  

            A1_1.Value = Convert.ToInt32(buffer[4]);//  
            A1_2.Value = Convert.ToInt32(buffer[5]);//  
            A1_3.Value = Convert.ToInt32(buffer[6]);//  
            A1_4.Value = Convert.ToInt32(buffer[7]);//  


            A2_1.Value = Convert.ToInt32(buffer[8]);//  
            A2_2.Value = Convert.ToInt32(buffer[9]);//  
            A2_3.Value = Convert.ToInt32(buffer[10]);//  
            A2_4.Value = Convert.ToInt32(buffer[11]);//  

            B1_1.Value = Convert.ToInt32(buffer[12]);//  
            B1_2.Value = Convert.ToInt32(buffer[13]);//  
            B1_3.Value = Convert.ToInt32(buffer[14]);//  
            B1_4.Value = Convert.ToInt32(buffer[15]);//  


            B2_1.Value = Convert.ToInt32(buffer[16]);//  
            B2_2.Value = Convert.ToInt32(buffer[17]);//  
            B2_3.Value = Convert.ToInt32(buffer[18]);//  
            B2_4.Value = Convert.ToInt32(buffer[19]);//   

        }
        void 填充vrc波形配置(bool 仅填充波形列表)
        {
            comboBox配置1.Items.Clear();
            comboBox配置2.Items.Clear();
            comboBox配置3.Items.Clear();
            comboBox配置4.Items.Clear();
            comboBox配置5.Items.Clear();

            if (listBox2.Items.Count > 0)
            {
                foreach (string a in listBox2.Items)
                {
                    comboBox配置1.Items.Add(a);
                    comboBox配置2.Items.Add(a);
                    comboBox配置3.Items.Add(a);
                    comboBox配置4.Items.Add(a);
                    comboBox配置5.Items.Add(a);
                }
            }
            comboBox配置12.Items.Clear();
            comboBox配置22.Items.Clear();
            comboBox配置32.Items.Clear();
            comboBox配置42.Items.Clear();
            comboBox配置52.Items.Clear();

            if (listBox2.Items.Count > 0)
            {
                foreach (string a in listBox2.Items)
                {
                    comboBox配置12.Items.Add(a);
                    comboBox配置22.Items.Add(a);
                    comboBox配置32.Items.Add(a);
                    comboBox配置42.Items.Add(a);
                    comboBox配置52.Items.Add(a);
                }
            }





            if (仅填充波形列表)
                return;

            if (!comboBox配置1.Items.Contains(comboBox配置1.Text) && comboBox配置1.Items.Count > 0)
            {
                comboBox配置1.Text = comboBox配置1.Items[0].ToString();
            }
            if (!comboBox配置2.Items.Contains(comboBox配置2.Text) && comboBox配置2.Items.Count > 0)
            {
                comboBox配置2.Text = comboBox配置2.Items[0].ToString();
            }
            if (!comboBox配置3.Items.Contains(comboBox配置3.Text) && comboBox配置3.Items.Count > 0)
            {
                comboBox配置3.Text = comboBox配置3.Items[0].ToString();
            }
            if (!comboBox配置4.Items.Contains(comboBox配置4.Text) && comboBox配置4.Items.Count > 0)
            {
                comboBox配置4.Text = comboBox配置4.Items[0].ToString();
            }
            if (!comboBox配置5.Items.Contains(comboBox配置5.Text) && comboBox配置5.Items.Count > 0)
            {
                comboBox配置5.Text = comboBox配置5.Items[0].ToString();
            }
        }

        byte[] 实时更新强度(byte[] buffer, float A强度百分比, float B强度百分比)
        {
            byte[] bufferout = new byte[buffer.Length];

            for (int i = 0; i < buffer.Length; i++)
            {
                if (i == 2)
                {//A通强度
                    textBoxA前.Text = Convert.ToInt32(buffer[2]).ToString();

                    int gg = (int)((float)Convert.ToInt32(buffer[2]) * A强度百分比 * float.Parse(textBoxA通倍率.Text));

                    if (gg > int.Parse(labelA通软上限.Text))
                    {
                        gg = int.Parse(labelA通软上限.Text);
                    }
                    textBoxA后.Text = gg.ToString();

                    bufferout[2] = Convert.ToByte(gg);



                }
                else if (i == 3)
                {//B通强度
                    textBoxB前.Text = Convert.ToInt32(buffer[3]).ToString();
                    int gg = (int)((float)Convert.ToInt32(buffer[3]) * B强度百分比 * float.Parse(textBoxB通倍率.Text));

                    if (gg > int.Parse(labelB通软上限.Text))
                    {
                        gg = int.Parse(labelB通软上限.Text);
                    }

                    textBoxB后.Text = gg.ToString();
                    bufferout[3] = Convert.ToByte(gg);

                }
                else
                    bufferout[i] = buffer[i];
            }


            return bufferout;



        }

       

        bool DG播放队列 = false;
        bool DG播放波形 = false;

        private void button10_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count > 0 && listBox1.SelectedItem != null)
            {
                write(string转byte波形(a郊狼存档.DG波形列表[listBox1.SelectedItem.ToString()])); 
            }

        }

        private void button29_Click(object sender, EventArgs e)
        {
            if (listBox2.Items.Count > 0 && listBox2.SelectedItem != null)
            {
                待执行波形列表.Clear();

                for (int i = 0; i < a郊狼存档.DG波形队列[listBox2.SelectedItem.ToString()].波形队列User包含全部格式的英文Tag.Count; i++)
                {
                    待执行波形列表.Add(string转byte波形(
                       a郊狼存档.DG波形队列[listBox2.SelectedItem.ToString()].波形队列User包含全部格式的英文Tag[i]
                       ));
                }
                timer新定义波形输出控制.Enabled = true;
            }
        }

        private void button19全部导出_Click(object sender, EventArgs e)
        {
            初次写入();

            System.Windows.Forms.SaveFileDialog dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.InitialDirectory = System.Environment.CurrentDirectory.ToString();
            dialog.Filter = "郊狼存档|*.DGLab";
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string newFilepath = dialog.FileName;

                序列化保存读取.序列化保存<郊狼存档>(a郊狼存档, dialog.FileName);
            }

            加载本地目录下的玩法();

        }

        private void button20仅导入波形设置_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.InitialDirectory = System.Environment.CurrentDirectory.ToString();
            dialog.Filter = "郊狼存档|*.DGLab";
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {

                仅导入波形(dialog.FileName);

            }




        }

        private void button9仅导入VRChat玩法_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.InitialDirectory = System.Environment.CurrentDirectory.ToString();
            dialog.Filter = "郊狼存档|*.DGLab";
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {

                仅导入玩法(dialog.FileName);

            }

        }

        void 导入(string pathss)
        {

            if (!File.Exists(pathss))
            {
                return;
            }


            a郊狼存档 = 序列化保存读取.序列化读取<郊狼存档>(pathss);

            listBox1.Items.Clear();
            foreach (string a in a郊狼存档.DG波形列表.Keys)
            {
                listBox1.Items.Add(a);
            }
            listBox2.Items.Clear();
            foreach (string a in a郊狼存档.DG波形队列.Keys)
            {
                listBox2.Items.Add(a);
            }


            确认非空(textBox配置名1C, a郊狼存档.通道A.textBox配置名1C);
            确认非空(textBox配置名2C, a郊狼存档.通道A.textBox配置名2C);
            确认非空(textBox配置名3C, a郊狼存档.通道A.textBox配置名3C);
            确认非空(textBox配置名4C, a郊狼存档.通道A.textBox配置名4C);
            确认非空(textBox配置名5C, a郊狼存档.通道A.textBox配置名5C);




            确认非空(comboBox配置条件1, a郊狼存档.通道A.textBox配置条件1C);
            确认非空(comboBox配置条件2, a郊狼存档.通道A.textBox配置条件2C);
            确认非空(comboBox配置条件3, a郊狼存档.通道A.textBox配置条件3C);
            确认非空(comboBox配置条件4, a郊狼存档.通道A.textBox配置条件4C);
            确认非空(comboBox配置条件5, a郊狼存档.通道A.textBox配置条件5C);



            确认非空(textBox配置名1C2, a郊狼存档.通道B.textBox配置名1C);
            确认非空(textBox配置名2C2, a郊狼存档.通道B.textBox配置名2C);
            确认非空(textBox配置名3C2, a郊狼存档.通道B.textBox配置名3C);
            确认非空(textBox配置名4C2, a郊狼存档.通道B.textBox配置名4C);
            确认非空(textBox配置名5C2, a郊狼存档.通道B.textBox配置名5C);




            确认非空(comboBox配置条件12, a郊狼存档.通道B.textBox配置条件1C);
            确认非空(comboBox配置条件22, a郊狼存档.通道B.textBox配置条件2C);
            确认非空(comboBox配置条件32, a郊狼存档.通道B.textBox配置条件3C);
            确认非空(comboBox配置条件42, a郊狼存档.通道B.textBox配置条件4C);
            确认非空(comboBox配置条件52, a郊狼存档.通道B.textBox配置条件5C);

















            确认非空(textBoxA通强度参数C, a郊狼存档.通道A.textBoxA通强度参数C);
            确认非空(textBoxB通强度参数C, a郊狼存档.通道A.textBoxB通强度参数C);

            确认非空(textBoxA通倍率, a郊狼存档.通道A.textBoxA通倍率C);
            确认非空(textBoxB通倍率, a郊狼存档.通道A.textBoxB通倍率C);





            确认非空(comboBox配置1, a郊狼存档.通道A.comboBox配置1C);
            确认非空(comboBox配置2, a郊狼存档.通道A.comboBox配置2C);
            确认非空(comboBox配置3, a郊狼存档.通道A.comboBox配置3C);
            确认非空(comboBox配置4, a郊狼存档.通道A.comboBox配置4C);
            确认非空(comboBox配置5, a郊狼存档.通道A.comboBox配置5C);



            确认非空(comboBox配置12, a郊狼存档.通道B.comboBox配置1C);
            确认非空(comboBox配置22, a郊狼存档.通道B.comboBox配置2C);
            确认非空(comboBox配置32, a郊狼存档.通道B.comboBox配置3C);
            确认非空(comboBox配置42, a郊狼存档.通道B.comboBox配置4C);
            确认非空(comboBox配置52, a郊狼存档.通道B.comboBox配置5C);



            填充vrc波形配置(false);

        }


        void 仅导入玩法(string pathss)
        {

           if (!File .Exists (pathss))
            {
                return;
            }


            a郊狼存档 = 序列化保存读取.序列化读取<郊狼存档>(pathss);




            确认非空(textBox配置名1C, a郊狼存档.通道A.textBox配置名1C);
            确认非空(textBox配置名2C, a郊狼存档.通道A.textBox配置名2C);
            确认非空(textBox配置名3C, a郊狼存档.通道A.textBox配置名3C);
            确认非空(textBox配置名4C, a郊狼存档.通道A.textBox配置名4C);
            确认非空(textBox配置名5C, a郊狼存档.通道A.textBox配置名5C);




            确认非空(comboBox配置条件1, a郊狼存档.通道A.textBox配置条件1C);
            确认非空(comboBox配置条件2, a郊狼存档.通道A.textBox配置条件2C);
            确认非空(comboBox配置条件3, a郊狼存档.通道A.textBox配置条件3C);
            确认非空(comboBox配置条件4, a郊狼存档.通道A.textBox配置条件4C);
            确认非空(comboBox配置条件5, a郊狼存档.通道A.textBox配置条件5C);




            确认非空(textBox配置名1C2, a郊狼存档.通道B.textBox配置名1C);
            确认非空(textBox配置名2C2, a郊狼存档.通道B.textBox配置名2C);
            确认非空(textBox配置名3C2, a郊狼存档.通道B.textBox配置名3C);
            确认非空(textBox配置名4C2, a郊狼存档.通道B.textBox配置名4C);
            确认非空(textBox配置名5C2, a郊狼存档.通道B.textBox配置名5C);




            确认非空(comboBox配置条件12, a郊狼存档.通道B.textBox配置条件1C);
            确认非空(comboBox配置条件22, a郊狼存档.通道B.textBox配置条件2C);
            确认非空(comboBox配置条件32, a郊狼存档.通道B.textBox配置条件3C);
            确认非空(comboBox配置条件42, a郊狼存档.通道B.textBox配置条件4C);
            确认非空(comboBox配置条件52, a郊狼存档.通道B.textBox配置条件5C);




            确认非空(textBoxA通强度参数C, a郊狼存档.通道A.textBoxA通强度参数C);
            确认非空(textBoxB通强度参数C, a郊狼存档.通道A.textBoxB通强度参数C);

            确认非空(textBoxA通倍率, a郊狼存档.通道A.textBoxA通倍率C);
            确认非空(textBoxB通倍率, a郊狼存档.通道A.textBoxB通倍率C);





            确认非空(comboBox配置1, a郊狼存档.通道A.comboBox配置1C);
            确认非空(comboBox配置2, a郊狼存档.通道A.comboBox配置2C);
            确认非空(comboBox配置3, a郊狼存档.通道A.comboBox配置3C);
            确认非空(comboBox配置4, a郊狼存档.通道A.comboBox配置4C);
            确认非空(comboBox配置5, a郊狼存档.通道A.comboBox配置5C);



            确认非空(comboBox配置12, a郊狼存档.通道B.comboBox配置1C);
            确认非空(comboBox配置22, a郊狼存档.通道B.comboBox配置2C);
            确认非空(comboBox配置32, a郊狼存档.通道B.comboBox配置3C);
            确认非空(comboBox配置42, a郊狼存档.通道B.comboBox配置4C);
            确认非空(comboBox配置52, a郊狼存档.通道B.comboBox配置5C);





            填充vrc波形配置(false);

        }

        void 仅导入波形(string pathss)
        {

            if (!File.Exists(pathss))
            {
                return;
            }


            a郊狼存档 = 序列化保存读取.序列化读取<郊狼存档>(pathss);

            listBox1.Items.Clear();
            foreach (string a in a郊狼存档.DG波形列表.Keys)
            {
                listBox1.Items.Add(a);
            }
            listBox2.Items.Clear();
            foreach (string a in a郊狼存档.DG波形队列.Keys)
            {
                listBox2.Items.Add(a);
            }


            填充vrc波形配置(true);

        }








        bool DG启用Vrc = false;
        private void button15_Click(object sender, EventArgs e)
        {
            DG启用Vrc = true;

           // timer波形连续输出.Enabled = true;

            timer新定义波形输出控制.Enabled = true;
            groupBox36.Visible = true;

            button19.Visible = true;
            button15.Visible = false;
        }

        private void button19_Click(object sender, EventArgs e)
        {
            DG启用Vrc = false;
            groupBox36.Visible = false;

            button19.Visible = false;
            button15.Visible = true;
        }



        void 尝试追加参数名(ComboBox a, string 参数名)
        {
            if (!a.Items.Contains(参数名))
            {
                a.Items.Add(参数名);
            }
        }


        void 获得参数变更(string 参数名, string 值)
        {
            //更新列表和所有combobox

            if (!listBox5.Items.Contains(参数名))
            {
                listBox5.Items.Add(参数名);
                listBox3.Items.Add(值);

            }
            else
            {
                listBox3.Items[listBox5.Items.IndexOf(参数名)] = 值;
            }


            尝试追加参数名(textBox配置名1C, 参数名);
            尝试追加参数名(textBox配置名2C, 参数名);
            尝试追加参数名(textBox配置名3C, 参数名);
            尝试追加参数名(textBox配置名4C, 参数名);
            尝试追加参数名(textBox配置名5C, 参数名);
            尝试追加参数名(textBoxA通强度参数C, 参数名);
            尝试追加参数名(textBoxB通强度参数C, 参数名);

            尝试追加参数名(textBox配置名1C2, 参数名);
            尝试追加参数名(textBox配置名2C2, 参数名);
            尝试追加参数名(textBox配置名3C2, 参数名);
            尝试追加参数名(textBox配置名4C2, 参数名);
            尝试追加参数名(textBox配置名5C2, 参数名);



            if (textBox配置名1C.Text.Trim() == 参数名.Trim())
            {
                textBox触发值1.Text = 值.Trim();
            }
            else if (textBox配置名2C.Text.Trim() == 参数名.Trim())
            {
                textBox触发值2.Text = 值.Trim();
            }
            else if (textBox配置名3C.Text.Trim() == 参数名.Trim())
            {
                textBox触发值3.Text = 值.Trim();
            }
            else if (textBox配置名4C.Text.Trim() == 参数名.Trim())
            {
                textBox触发值4.Text = 值.Trim();
            }
            else if (textBox配置名5C.Text.Trim() == 参数名.Trim())
            {
                textBox触发值5.Text = 值.Trim();
            }






            if (textBox配置名1C2.Text.Trim() == 参数名.Trim())
            {
                textBox触发值12.Text = 值.Trim();
            }
            else if (textBox配置名2C2.Text.Trim() == 参数名.Trim())
            {
                textBox触发值22.Text = 值.Trim();
            }
            else if (textBox配置名3C2.Text.Trim() == 参数名.Trim())
            {
                textBox触发值32.Text = 值.Trim();
            }
            else if (textBox配置名4C2.Text.Trim() == 参数名.Trim())
            {
                textBox触发值42.Text = 值.Trim();
            }
            else if (textBox配置名5C2.Text.Trim() == 参数名.Trim())
            {
                textBox触发值52.Text = 值.Trim();
            }






             

            if (textBoxA通强度参数C.Text.Trim() == 参数名.Trim())
            {
                textBoxA通强度值.Text = 值.Trim();
            }
             if (textBoxB通强度参数C.Text.Trim() == 参数名.Trim())
            {
                textBoxB通强度值.Text = 值.Trim();
            }



        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //恢复

            if (listBox2.SelectedItem != null)
            {
                a郊狼存档.DG波形队列[listBox2.SelectedItem.ToString()].重设控制ImageListView(ref vv);
            }
            else if (listBox2.Items.Count > 0)
            {
                a郊狼存档.DG波形队列[listBox2.Items[0].ToString()].重设控制ImageListView(ref vv);
            }
        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {//将设置还原回去
            if (listBox1.SelectedItem != null)
            {
                byte[] A = string转byte波形(a郊狼存档.DG波形列表[listBox1.SelectedItem.ToString()]);
                填充波形设置(A);

            }



        }

        int sleeptime = 0;
        string changedata = "";
        private void timerOSC_Tick(object sender, EventArgs e)
        {
            if (data.Count > 0)
            {
                changedata = "";
                for (int i = 0; i < data.Count; i++)
                {
                    string a = dataKeys[i].Replace("/avatar/parameters/", "");
                    string b = data[dataKeys[i]];


                    changedata += (a + ":" + b + "\r\n");
                    获得参数变更(a, b);
                }

                if (textBox5.Text != changedata)
                {
                    textBox5.Text = changedata;
                }
                /*
                 * AngularY:0
VelocityX:0
VelocityZ:0
VelocityMagnitude:0
Voice:0
Viseme:0
AUF_Angle:0
CTR_Angle:0
CTR_IsGrabbed:False
CTR_Stretch:0
CTR_IsPosed:False
AUF_IsGrabbed:False
AUF_IsPosed:True
VelocityY:0
Grounded:True

                 * */

            }
        }

        private void button30_Click(object sender, EventArgs e)
        {
            float bb = float.Parse(textBoxA通倍率.Text) - 0.1f;
            if (bb < 0.1)
            {
                bb = 0.1f;
            }
            textBoxA通倍率.Text = bb.ToString();
        }

        private void button33_Click(object sender, EventArgs e)
        {
            float bb = float.Parse(textBoxA通倍率.Text) + 0.1f;
            if (bb > 5)
            {
                bb = 5;
            }
            textBoxA通倍率.Text = bb.ToString();
        }

        private void button31_Click(object sender, EventArgs e)
        {
            float bb = float.Parse(textBoxB通倍率.Text) - 0.1f;
            if (bb < 0.1)
            {
                bb = 0.1f;
            }
            textBoxB通倍率.Text = bb.ToString();
        }

        private void button32_Click(object sender, EventArgs e)
        {
            float bb = float.Parse(textBoxB通倍率.Text) + 0.1f;
            if (bb > 5)
            {
                bb = 5;
            }
            textBoxB通倍率.Text = bb.ToString();
        }

        private void textBoxA通强度值_TextChanged(object sender, EventArgs e)
        {
            float bb = 0; 
            if (float.TryParse(textBoxA通强度值.Text, out bb))
            { 
                if (bb < set数值最小值)
                {
                    bb = set数值最小值;
                }
                if (bb > set数值最大值)
                {
                    bb = set数值最大值;
                }
                textBoxA通强度值.Text = bb.ToString();
                textBoxA参.Text = bb.ToString();
            }
        }

        private void textBoxB通强度值_TextChanged(object sender, EventArgs e)
        {

            float bb = 0;

            if (float.TryParse (textBoxB通强度值.Text,out bb))
            {

                if (bb < set数值最小值)
                {
                    bb = set数值最小值;
                }
                if (bb > set数值最大值)
                {
                    bb = set数值最大值;
                }
                textBoxB通强度值.Text = bb.ToString();
                textBoxB参.Text = bb.ToString();

            }    
          
        }

        private void button34_Click(object sender, EventArgs e)
        {
            if (data.Keys.Contains(textBox11.Text))
                data[textBox11.Text] = textBox10.Text;
            else
            {
                data.Add(textBox11.Text, textBox10.Text);
                dataKeys.Add(textBox11.Text);

            }

        }

        private void button35_Click(object sender, EventArgs e)
        {
            data.Clear();
        }

        private void button36_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count > 0 && listBox1.SelectedItem != null)
            {

                待执行波形列表.Add(string转byte波形(a郊狼存档.DG波形列表[listBox1.SelectedItem.ToString()]));
                波形循环触发测试();
            }


        }

        int 循环次数 = 3;
        byte[] 执行波形 = null;


        List<byte[]> 待执行波形列表 = new List<byte[]>();



        int 毫秒消息延迟 = 0;
        void 波形循环触发测试()
        {
             
            if (待执行波形列表.Count > 0)
            {
                //write(待执行波形列表[0]);
                写入蓝牙列表.Add(待执行波形列表[0]);
                待执行波形列表.RemoveAt(0);
            }

        }

        private void button37_Click(object sender, EventArgs e)
        {
            if (listBox2.Items.Count > 0 && listBox2.SelectedItem != null)
            {


                for (int i = 0; i < a郊狼存档.DG波形队列[listBox2.SelectedItem.ToString()].波形队列User包含全部格式的英文Tag.Count; i++)
                {
                    待执行波形列表.Add(string转byte波形(
                       a郊狼存档.DG波形队列[listBox2.SelectedItem.ToString()].波形队列User包含全部格式的英文Tag[i]
                       ));
                }

                波形循环触发测试();
            }
        }

        private void button38_Click(object sender, EventArgs e)
        {
            待执行波形列表.Clear();
        }

        private void button11_Click(object sender, EventArgs e)
        {

            if (listBox1.SelectedItem != null)
            {
                a郊狼存档.DG波形列表.Remove(listBox1.SelectedItem.ToString());

                listBox1.Items.Remove(listBox1.SelectedItem);

            }
        }

        private void button22_Click(object sender, EventArgs e)
        {


            if (listBox2.SelectedItem != null)
            {
                a郊狼存档.DG波形队列.Remove(listBox2.SelectedItem.ToString());

                listBox2.Items.Remove(listBox2.SelectedItem);

            }




        }

        private void textBox配置名1C_TextChanged(object sender, EventArgs e)
        {
            a郊狼存档.通道A.textBox配置名1C = textBox配置名1C.Text;
            groupBox配1.Text = textBox配置名1C.Text;
        }

        private void textBox配置名2C_TextChanged(object sender, EventArgs e)
        {
            a郊狼存档.通道A.textBox配置名2C = textBox配置名2C.Text;
            groupBox配2.Text = textBox配置名2C.Text;
        }

        private void textBox配置名3C_TextChanged(object sender, EventArgs e)
        {
            a郊狼存档.通道A.textBox配置名3C = textBox配置名3C.Text;
            groupBox配3.Text = textBox配置名3C.Text;
        }


        private void textBox配置名4C_TextChanged(object sender, EventArgs e)
        {
            a郊狼存档.通道A.textBox配置名4C = textBox配置名4C.Text;
            groupBox配4.Text = textBox配置名4C.Text;
        }

        private void textBox配置名5C_TextChanged(object sender, EventArgs e)
        {
            a郊狼存档.通道A.textBox配置名5C = textBox配置名5C.Text;
            groupBox配5.Text = textBox配置名5C.Text;
        }

        private void comboBox配置条件1_TextChanged(object sender, EventArgs e)
        {
            a郊狼存档.通道A.textBox配置条件1C = comboBox配置条件1.Text;
        }

        private void comboBox配置条件2_TextChanged(object sender, EventArgs e)
        {
            a郊狼存档.通道A.textBox配置条件2C = comboBox配置条件2.Text;
        }

        private void comboBox配置条件3_TextChanged(object sender, EventArgs e)
        {
            a郊狼存档.通道A.textBox配置条件3C = comboBox配置条件3.Text;
        }

        private void comboBox配置条件4_TextChanged(object sender, EventArgs e)
        {
            a郊狼存档.通道A.textBox配置条件4C = comboBox配置条件4.Text;
        }

        private void comboBox配置条件5_TextChanged(object sender, EventArgs e)
        {
            a郊狼存档.通道A.textBox配置条件5C = comboBox配置条件5.Text;
        }

        private void textBoxA通强度参数C_TextChanged(object sender, EventArgs e)
        {
            a郊狼存档.通道A.textBoxA通强度参数C = textBoxA通强度参数C.Text;

            groupBoxA强.Text = textBoxA通强度参数C.Text;
        }

        private void textBoxB通强度参数C_TextChanged(object sender, EventArgs e)
        {
            a郊狼存档.通道A.textBoxB通强度参数C = textBoxB通强度参数C.Text;

            groupBoxB强.Text = textBoxB通强度参数C.Text;
        }

        private void textBoxA通倍率_TextChanged(object sender, EventArgs e)
        {
            float bb = float.Parse(textBoxA通倍率.Text);
            if (bb < 0.1)
            {
                bb = 0.1f;
            }
            if (bb > 5)
            {
                bb = 5;
            }
            textBoxA通倍率.Text = bb.ToString();
            a郊狼存档.通道A.textBoxA通倍率C = textBoxA通倍率.Text;
            刷新最大值最小值();
        }

        private void textBoxB通倍率_TextChanged(object sender, EventArgs e)
        {
            float bb = float.Parse(textBoxB通倍率.Text);
            if (bb < 0.1)
            {
                bb = 0.1f;
            }
            if (bb > 5)
            {
                bb = 5;
            }
            textBoxB通倍率.Text = bb.ToString();
            a郊狼存档.通道A.textBoxB通倍率C = textBoxB通倍率.Text;
            刷新最大值最小值();
        }
        void 初次写入()
        {
            a郊狼存档.通道A.textBox配置名1C = textBox配置名1C.Text;
            a郊狼存档.通道A.textBox配置名2C = textBox配置名2C.Text;
            a郊狼存档.通道A.textBox配置名3C = textBox配置名3C.Text;
            a郊狼存档.通道A.textBox配置名4C = textBox配置名4C.Text;
            a郊狼存档.通道A.textBox配置名5C = textBox配置名5C.Text;
            a郊狼存档.通道A.textBox配置条件1C = comboBox配置条件1.Text;
            a郊狼存档.通道A.textBox配置条件2C = comboBox配置条件2.Text;
            a郊狼存档.通道A.textBox配置条件3C = comboBox配置条件3.Text;
            a郊狼存档.通道A.textBox配置条件4C = comboBox配置条件4.Text;
            a郊狼存档.通道A.textBox配置条件5C = comboBox配置条件5.Text;
            a郊狼存档.通道A.textBoxA通强度参数C = textBoxA通强度参数C.Text;
            a郊狼存档.通道A.textBoxB通强度参数C = textBoxB通强度参数C.Text;
            a郊狼存档.通道A.textBoxA通倍率C = textBoxA通倍率.Text;
            a郊狼存档.通道A.textBoxB通倍率C = textBoxB通倍率.Text;

            a郊狼存档.通道A.comboBox配置1C= comboBox配置1.Text;
            a郊狼存档.通道A.comboBox配置2C = comboBox配置2.Text;
            a郊狼存档.通道A.comboBox配置3C = comboBox配置3.Text;
            a郊狼存档.通道A.comboBox配置4C = comboBox配置4.Text;
            a郊狼存档.通道A.comboBox配置5C = comboBox配置5.Text;






            a郊狼存档.通道B.textBox配置名1C = textBox配置名1C2.Text;
            a郊狼存档.通道B.textBox配置名2C = textBox配置名2C2.Text;
            a郊狼存档.通道B.textBox配置名3C = textBox配置名3C2.Text;
            a郊狼存档.通道B.textBox配置名4C = textBox配置名4C2.Text;
            a郊狼存档.通道B.textBox配置名5C = textBox配置名5C2.Text;
            a郊狼存档.通道B.textBox配置条件1C = comboBox配置条件12.Text;
            a郊狼存档.通道B.textBox配置条件2C = comboBox配置条件22.Text;
            a郊狼存档.通道B.textBox配置条件3C = comboBox配置条件32.Text;
            a郊狼存档.通道B.textBox配置条件4C = comboBox配置条件42.Text;
            a郊狼存档.通道B.textBox配置条件5C = comboBox配置条件52.Text;
            a郊狼存档.通道B.textBoxA通强度参数C = textBoxA通强度参数C.Text;
            a郊狼存档.通道B.textBoxB通强度参数C = textBoxB通强度参数C.Text;
            a郊狼存档.通道B.textBoxA通倍率C = textBoxA通倍率.Text;
            a郊狼存档.通道B.textBoxB通倍率C = textBoxB通倍率.Text;


            a郊狼存档.通道B.comboBox配置1C = comboBox配置12.Text;
            a郊狼存档.通道B.comboBox配置2C = comboBox配置22.Text;
            a郊狼存档.通道B.comboBox配置3C = comboBox配置32.Text;
            a郊狼存档.通道B.comboBox配置4C = comboBox配置42.Text;
            a郊狼存档.通道B.comboBox配置5C = comboBox配置52.Text;




        }

        private void button12_Click(object sender, EventArgs e)
        {
            初次写入();
            System.Windows.Forms.SaveFileDialog dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.InitialDirectory = System.Environment.CurrentDirectory.ToString();
            dialog.Filter = "郊狼存档|*.DGLab";
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string newFilepath = dialog.FileName;

                序列化保存读取.序列化保存<郊狼存档>(a郊狼存档, dialog.FileName);
            }
        }


        int mousex = 0;
        int mousey = 0;
        private void GetMousePose()
        {
            System.Drawing.Point mp = System.Windows.Forms.Control.MousePosition;
            mousex = mp.X;  //鼠标当前X坐标
            mousey = mp.Y;  //鼠标当前Y坐标
        }

        private void A1_1_MouseEnter(object sender, EventArgs e)
        {
            GetMousePose();
            if (checkBox4.Checked)
            {
                A1_1.Value = mousex;
            }

        }
        IKeyboardMouseEvents gmouse;

        private void button17_Click_1(object sender, EventArgs e)
        {
            gmouse = Hook.GlobalEvents();
            gmouse.MouseDownExt += Gmouse_MouseDownExt;
            gmouse.MouseUpExt += Gmouse_MouseUpExt;
        }

        bool mousedown = false;
        private void Gmouse_MouseUpExt(object sender, MouseEventExtArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mousedown = false;
                checkBox4.Checked = false;
            }
        }

        private void Gmouse_MouseDownExt(object sender, MouseEventExtArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mousedown = true;
                checkBox4.Checked = true;
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            gmouse.MouseUpExt -= Gmouse_MouseDownExt;
            gmouse.MouseDownExt -= Gmouse_MouseDownExt;
            gmouse.Dispose();
        }











        bool G_MouseFlag;
        Pen pen = new Pen(Color.Black);
        Pen pen2 = new Pen(Color.Gray);
        Point lastPoint;



        void 控制滑块(int 每层高度, int width, int X, int Y, TrackBar a, TrackBar b, TrackBar c, TrackBar d)
        {


            // 303, 179


            int aa = (int)((float)a.Maximum * ((float)X / (float)width));
            if (aa > a.Maximum)
            {
                aa = a.Maximum;
            }

            if (aa < a.Minimum)
            {
                aa = a.Minimum;
            }

            if (Math.Abs(Y - 每层高度) < 10)
            {
                a.Value = aa;
            }
            else if (Math.Abs(Y - 每层高度 * 2) < 10)
            {
                b.Value = aa;
            }
            else if (Math.Abs(Y - 每层高度 * 3) < 10)
            {
                c.Value = aa;
            }
            else if (Math.Abs(Y - 每层高度 * 4) < 10)
            {
                d.Value = aa;
            }

        }


        private void _018_MouseMove(object sender, MouseEventArgs e)
        {
            label36.Text = e.X.ToString();
            label37.Text = e.Y.ToString();
            Panel aa = sender as Panel;
            Graphics graphics = aa.CreateGraphics();


            int 每层高度 = (int)((float)aa.Height / 5f);



            if (lastPoint.Equals(Point.Empty))//判断绘图开始点是否为空
            {
                lastPoint = new Point(e.X, e.Y);//记录鼠标当前位置
            }
            if (G_MouseFlag)//开始绘图
            {
                Point currentPoint = new Point(e.X, e.Y);//获取鼠标当前位置
                graphics.DrawLine(pen, currentPoint, lastPoint);//绘图


                if (aa.Name == "panel3")
                    控制滑块(每层高度, aa.Width, e.X, e.Y, A1_1, A1_2, A1_3, A1_4);
                if (aa.Name == "panel4")
                    控制滑块(每层高度, aa.Width, e.X, e.Y, B1_1, B1_2, B1_3, B1_4);
                if (aa.Name == "panel5")
                    控制滑块(每层高度, aa.Width, e.X, e.Y, A2_1, A2_2, A2_3, A2_4);
                if (aa.Name == "panel6")
                    控制滑块(每层高度, aa.Width, e.X, e.Y, B2_1, B2_2, B2_3, B2_4);


            }



            Point S01A = new Point(0, 每层高度);
            Point S01B = new Point(aa.Width, 每层高度);
            Point S02A = new Point(0, 每层高度 * 2);
            Point S02B = new Point(aa.Width, 每层高度 * 2);
            Point S03A = new Point(0, 每层高度 * 3);
            Point S03B = new Point(aa.Width, 每层高度 * 3);
            Point S04A = new Point(0, 每层高度 * 4);
            Point S04B = new Point(aa.Width, 每层高度 * 4);



            graphics.DrawLine(pen2, S01A, S01B);//绘图
            graphics.DrawLine(pen2, S02A, S02B);//绘图
            graphics.DrawLine(pen2, S03A, S03B);//绘图
            graphics.DrawLine(pen2, S04A, S04B);//绘图




            lastPoint = new Point(e.X, e.Y);//记录鼠标当前位置
        }

        private void _018_MouseDown(object sender, MouseEventArgs e)
        {
            label36.Text = e.X.ToString();
            label37.Text = e.Y.ToString();

            if (!G_MouseFlag)
            {
                Panel aa = sender as Panel;
                Graphics graphics = aa.CreateGraphics();
                graphics.Clear(Color.White);
            }


            G_MouseFlag = true;//开始绘图标识设置为true

        }

        private void _018_MouseUp(object sender, MouseEventArgs e)
        {
            G_MouseFlag = false;//开始绘图标识设置为false
        }





        List<Point> pp = new List<Point>();

        List<int> ppA通频率 = new List<int>();
        List<int> ppA通强度 = new List<int>();
        List<int> ppB通频率 = new List<int>();
        List<int> ppB通强度 = new List<int>();
        int 每段宽度 = 0;
        private void _018_MouseMove队列(object sender, MouseEventArgs e)
        {
            label36.Text = e.X.ToString();
            label37.Text = e.Y.ToString();


            label39.Text = e.X.ToString();
            label40.Text = e.Y.ToString();


            Panel aa = sender as Panel;
            Graphics graphics = aa.CreateGraphics();
            if (lastPoint.Equals(Point.Empty))//判断绘图开始点是否为空
            {
                lastPoint = new Point(e.X, e.Y);//记录鼠标当前位置
            }
            if (G_MouseFlag)//开始绘图
            {
                Point currentPoint = new Point(e.X, e.Y);//获取鼠标当前位置
                graphics.DrawLine(pen, currentPoint, lastPoint);//绘图
            }

            //依照切分数量划线
            //依照切分判定获取点位。？直接获取值

            int a = 4;
            if (int.TryParse(textBox14.Text, out a))
            {
                每段宽度 = (int)((float)aa.Width / (float)(a + 1));

                if (ppA通频率.Count < a)
                {

                    ppA通频率.Clear();
                    ppA通强度.Clear();
                    ppB通频率.Clear();
                    ppB通强度.Clear();

                    for (int i = 0; i <= a; i++)
                    {
                        ppA通频率.Add(0);
                        ppA通强度.Add(0);
                        ppB通频率.Add(0);
                        ppB通强度.Add(0);

                    }
                }


                //549, 120

                for (int i = 1; i <= a; i++)
                {
                    if (Math.Abs(e.X - 每段宽度 * i) < (549f / (float)a))
                    {
                        if (aa.Name == "panel7")
                            ppA通频率[i] = e.Y;
                        if (aa.Name == "panel8")
                            ppA通强度[i] = e.Y;
                        if (aa.Name == "panel9")
                            ppB通频率[i] = e.Y;
                        if (aa.Name == "panel10")
                            ppB通强度[i] = e.Y;
                    }




                }
                for (int i = 1; i <= a + 1; i++)
                {
                    Point S01A = new Point(每段宽度 * i, 0);
                    Point S01B = new Point(每段宽度 * i, aa.Height);
                    graphics.DrawLine(pen2, S01A, S01B);//绘图

                }
            }



            lastPoint = new Point(e.X, e.Y);//记录鼠标当前位置
        }

        private void _018_MouseDown队列(object sender, MouseEventArgs e)
        {
            label36.Text = e.X.ToString();
            label37.Text = e.Y.ToString();

            if (!G_MouseFlag)
            {
                Panel aa = sender as Panel;
                Graphics graphics = aa.CreateGraphics();
                graphics.Clear(Color.White);
            }


            G_MouseFlag = true;//开始绘图标识设置为true

        }

        private void _018_MouseUp队列(object sender, MouseEventArgs e)
        {
            // 549, 120
            Panel aa = sender as Panel;
            G_MouseFlag = false;//开始绘图标识设置为false 

            if (aa.Name == "panel7")
            {
                listBox15.Items.Clear();
                foreach (int a in ppA通频率)
                {
                    int aaa = (int)((float)241 * ((float)a / (float)aa.Height));
                    if (aaa > 240)
                    {
                        aaa = 240;
                    }

                    if (aaa < 10)
                    {
                        aaa = 10;
                    }

                    listBox15.Items.Add(aaa);
                }
                listBox15.Items.RemoveAt(0);
            }
            else if (aa.Name == "panel8")
            {

                listBox16.Items.Clear();
                foreach (int a in ppA通强度)
                {
                    int aaa = (int)((float)101 * ((float)a / (float)aa.Height));
                    aaa = 101 - aaa;

                    if (aaa > 100)
                    {
                        aaa = 100;
                    }

                    if (aaa < 0)
                    {
                        aaa = 0;
                    }


                    listBox16.Items.Add(aaa);
                }

                listBox16.Items.RemoveAt(0);
            }
            else if (aa.Name == "panel9")
            {
                listBox17.Items.Clear();
                foreach (int a in ppB通频率)
                {
                    int aaa = (int)((float)241 * ((float)a / (float)aa.Height));

                    if (aaa > 240)
                    {
                        aaa = 240;
                    }

                    if (aaa < 10)
                    {
                        aaa = 10;
                    }

                    listBox17.Items.Add(aaa);
                }
                listBox17.Items.RemoveAt(0);

            }
            else if (aa.Name == "panel10")
            {
                listBox18.Items.Clear();
                foreach (int a in ppB通强度)
                {
                    int aaa = (int)((float)101 * ((float)a / (float)aa.Height));
                    aaa = 101 - aaa;

                    if (aaa > 100)
                    {
                        aaa = 100;
                    }

                    if (aaa < 0)
                    {
                        aaa = 0;
                    }

                    listBox18.Items.Add(aaa);
                }
                listBox18.Items.RemoveAt(0);
            }



        }























        //画圆
        private void button1_Click(object sender, EventArgs e)
        {
            Graphics graphics = this.CreateGraphics();
            Rectangle gle = new Rectangle(20, 20, 200, 200);
            graphics.DrawEllipse(pen, gle);
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;



            Debug蓝牙链接.Visible = checkBox.Checked;
            DebugVRChat.Visible = checkBox.Checked;

            checkBox5.Checked = checkBox.Checked;
            checkBox6.Checked = checkBox.Checked;



            //1749, 1047
            if (checkBox6.Checked)
            {
                this.Size = new System.Drawing.Size(1749, 1047);
            }
            else
            {
                this.Size = new System.Drawing.Size(311, 693);

            }

            // 1749, 1047  专家模式
            // 314, 625    简单模式



        }



        void 刷新最大值最小值()
        {
            //A通
            float A通软上限 = float.Parse(labelA通软上限.Text);

            float A通倍率 = float.Parse(textBoxA通倍率.Text);

            float A通最小值 = set数值最小值 * 1f * (float)A通倍率;
            labelA通最小值.Text = A通最小值.ToString("0.0");

            float A通波形预设强度 = float.Parse(textBoxA前.Text);


            float A通最大值 = set数值最大值 * A通波形预设强度 * (float)A通倍率;


            labelA通最大值.Text = A通最大值.ToString("0.0") + "(软上限" + labelA通软上限.Text + ")";


            //B通
            float B通软上限 = float.Parse(labelB通软上限.Text);
            float B通倍率 = float.Parse(textBoxB通倍率.Text);
            float B通最小值 = set数值最小值 * 1f * (float)B通倍率;
            labelB通最小值.Text = B通最小值.ToString("0.0");

            float B通波形预设强度 = float.Parse(textBoxB前.Text);



            float B通最大值 = set数值最大值 * B通波形预设强度 * (float)B通倍率;
            labelB通最大值.Text = B通最大值.ToString("0.0") + "(软上限" + labelB通软上限.Text + ")";

        }



        #region 超链接打开

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            System.Diagnostics.Process.Start("  https://github.com/amoeet/VRChat_X_DGLAB");

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://t.me/VRChatDGLAB");
        }



        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://space.bilibili.com/89856");
        }



        #endregion



        float set数值最小值 = 0.1f;
        float set数值最大值 = 1;

        private void textBox12_TextChanged(object sender, EventArgs e)
        {//最小值设置
            float tt = 0;
            if (float.TryParse(textBox12.Text, out tt))
            {

                set数值最小值 = tt;
            }
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {//最大值设置
            float tt = 0;
            if (float.TryParse(textBox13.Text, out tt))
            {

                set数值最大值 = tt;
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            byte[] buffer = new byte[7];
            //0xBF(1byte指令HEAD) + 
            // AB两通道强度软上限(2bytes) + 
            //AB两通道波形频率平衡参数(2btyes) + 
            // AB两通道波形强度平衡参数(2bytes)
            //  通道强度软上限
            //  通道强度软上限可以限制脉冲主机通道强度能达到的最大值，并且该设置断电保存，值范围(0~200)，
            //  输入范围外的值则不会修改软上限。 假设设置AB通道软上限为150和30，
            //  那么通过拨动滚轮或B0指令无论如何修改强度，
            //  A通道的通道强度只会在范围(0 ~150),
            //  B通道的通道强度只会在范围(0 ~30)，
            //  脉冲主机的通道强度一定不会超过软上限。

            //   频率平衡参数1
            //   波形频率平衡参数会调整波形高低频的感受，并且该设置断电保存，值范围(0 ~255)
            //   本参数控制固定通道强度下，不同频率波形的相对体感强度。值越大，低频波形冲击感越强。

            //   频率平衡参数2
            //   波形强度平衡参数会调整波形脉冲宽度，并且该设置断电保存，值范围(0 ~255)
            //   本参数控制固定通道强度下，不同频率波形的相对体感强度。值越大，低频波形刺激越强。

            buffer[0] = (byte)Convert.ToByte("0xBF", 16);
            buffer[1] = Convert.ToByte(A软上限设置.Value);
            buffer[2] = Convert.ToByte(B软上限设置.Value);
            buffer[3] = Convert.ToByte(A频率平衡.Value);
            buffer[4] = Convert.ToByte(B频率平衡.Value);
            buffer[5] = Convert.ToByte(A强度平衡.Value);
            buffer[6] = Convert.ToByte(B强度平衡.Value);


            write(buffer);
        }

        private void button51_Click(object sender, EventArgs e)
        {
            一键启动设置上限();
            刷新最大值最小值();
        } 

        private void textBoxA前_TextChanged(object sender, EventArgs e)
        {
            刷新最大值最小值();
        }

        private void textBoxB前_TextChanged(object sender, EventArgs e)
        {
            刷新最大值最小值();
        }
        private void 禁止非数字_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }







        byte[] 按照参数组合(int A值, int B值, int[] A1, int[] A2, int[] B1, int[] B2, int 开始点, int 获取数量)
        {
            byte[] buffer = new byte[20];

            buffer[0] = (byte)Convert.ToByte("0xB0", 16);
            buffer[1] = (byte)Convert.ToByte("01011111", 2);
            buffer[2] = Convert.ToByte(A值);
            buffer[3] = Convert.ToByte(B值);

            buffer[4] = Convert.ToByte(A1[开始点 * 获取数量 + 0]);
            buffer[5] = Convert.ToByte(A1[开始点 * 获取数量 + 1]);
            buffer[6] = Convert.ToByte(A1[开始点 * 获取数量 + 2]);
            buffer[7] = Convert.ToByte(A1[开始点 * 获取数量 + 3]);

            buffer[8] = Convert.ToByte(A2[开始点 * 获取数量 + 0]);
            buffer[9] = Convert.ToByte(A2[开始点 * 获取数量 + 1]);
            buffer[10] = Convert.ToByte(A2[开始点 * 获取数量 + 2]);
            buffer[11] = Convert.ToByte(A2[开始点 * 获取数量 + 3]);

            buffer[12] = Convert.ToByte(B1[开始点 * 获取数量 + 0]);
            buffer[13] = Convert.ToByte(B1[开始点 * 获取数量 + 1]);
            buffer[14] = Convert.ToByte(B1[开始点 * 获取数量 + 2]);
            buffer[15] = Convert.ToByte(B1[开始点 * 获取数量 + 3]);

            buffer[16] = Convert.ToByte(B2[开始点 * 获取数量 + 0]);
            buffer[17] = Convert.ToByte(B2[开始点 * 获取数量 + 1]);
            buffer[18] = Convert.ToByte(B2[开始点 * 获取数量 + 2]);
            buffer[19] = Convert.ToByte(B2[开始点 * 获取数量 + 3]);

            return buffer;
        }


        bool 检查listBox(ListBox a, int 切分数量)
        {
            if (a.Items.Count > 0 && a.Items.Count == 切分数量)
            {
                return true;
            }

            return false;
        }

        private void button24_Click(object sender, EventArgs e)
        {
            int 切分段数 = int.Parse(textBox14.Text);



            //获取切分的值，并进行组合
            if (!检查listBox(listBox15, 切分段数))
            {
                MessageBox.Show("A通 频率未绘制或绘制错误，缺少点");
                return;
            }
            if (!检查listBox(listBox16, 切分段数))
            {
                MessageBox.Show("A通 强度未绘制或绘制错误，缺少点");
                return;
            }
            if (!检查listBox(listBox17, 切分段数))
            {
                MessageBox.Show("B通 频率未绘制或绘制错误，缺少点");
                return;
            }
            if (!检查listBox(listBox18, 切分段数))
            {
                MessageBox.Show("B通 强度未绘制或绘制错误，缺少点");
                return;
            }

            //切分并追加
            int[] A1 = new int[listBox15.Items.Count];
            int[] A2 = new int[listBox16.Items.Count];
            int[] B1 = new int[listBox17.Items.Count];
            int[] B2 = new int[listBox18.Items.Count];


            for (int i = 0; i < listBox15.Items.Count; i++)
            {
                A1[i] = int.Parse(listBox15.Items[i].ToString());
            }
            for (int i = 0; i < listBox16.Items.Count; i++)
            {
                A2[i] = int.Parse(listBox16.Items[i].ToString());
            }
            for (int i = 0; i < listBox17.Items.Count; i++)
            {
                B1[i] = int.Parse(listBox17.Items[i].ToString());
            }
            for (int i = 0; i < listBox18.Items.Count; i++)
            {
                B2[i] = int.Parse(listBox18.Items[i].ToString());
            }
            Random ss = new Random();
            vv.tag栏位列表ImageListView.Items.Clear();
            for (int 计数器 = 0; 计数器 < listBox18.Items.Count / 4; 计数器++)
            {
                string bb = ss.Next(0, 9999).ToString();
                string cc = byte波形转string(按照参数组合(int.Parse(textBox15.Text), int.Parse(textBox16.Text), A1, A2, B1, B2, 计数器, 4));
                ImageListViewItem a = new ImageListViewItem("不使用" + 计数器.ToString() + "个" + bb, "自绘制第" + 计数器.ToString() + "个" + bb, cc, "不使用");
                vv.tag栏位列表ImageListView.Items.Add(a);


            }




            //队列操控UI追加方法：












        }

        private void textBoxA后_TextChanged(object sender, EventArgs e)
        {
            textBoxA强.Text = textBoxA后.Text;
        }

        private void textBoxB后_TextChanged(object sender, EventArgs e)
        {
            textBoxB强.Text = textBoxB后.Text;
        }

        private void button53_Click(object sender, EventArgs e)
        {

            int 切分段数 = int.Parse(textBox14.Text);



            //获取切分的值，并进行组合
            if (!检查listBox(listBox15, 切分段数))
            {
                MessageBox.Show("A通 频率未绘制或绘制错误，缺少点");
                return;
            }
            if (!检查listBox(listBox16, 切分段数))
            {
                MessageBox.Show("A通 强度未绘制或绘制错误，缺少点");
                return;
            }
            if (!检查listBox(listBox17, 切分段数))
            {
                MessageBox.Show("B通 频率未绘制或绘制错误，缺少点");
                return;
            }
            if (!检查listBox(listBox18, 切分段数))
            {
                MessageBox.Show("B通 强度未绘制或绘制错误，缺少点");
                return;
            }

            //切分并追加
            int[] A1 = new int[listBox15.Items.Count];
            int[] A2 = new int[listBox16.Items.Count];
            int[] B1 = new int[listBox17.Items.Count];
            int[] B2 = new int[listBox18.Items.Count];


            for (int i = 0; i < listBox15.Items.Count; i++)
            {
                A1[i] = int.Parse(listBox15.Items[i].ToString());
            }
            for (int i = 0; i < listBox16.Items.Count; i++)
            {
                A2[i] = int.Parse(listBox16.Items[i].ToString());
            }
            for (int i = 0; i < listBox17.Items.Count; i++)
            {
                B1[i] = int.Parse(listBox17.Items[i].ToString());
            }
            for (int i = 0; i < listBox18.Items.Count; i++)
            {
                B2[i] = int.Parse(listBox18.Items[i].ToString());
            }
            Random ss = new Random();
            for (int 计数器 = 0; 计数器 < listBox18.Items.Count / 4; 计数器++)
            {
                待执行波形列表.Add(按照参数组合(int.Parse(textBox15.Text), int.Parse(textBox16.Text), A1, A2, B1, B2, 计数器, 4));
            }


            //队列操控UI追加方法： 

            // 波形循环触发测试();
            timer新定义波形输出控制.Enabled = true;

        }

        private void button54_Click(object sender, EventArgs e)
        {

            if (textBox9波形队列名称.Text.Trim() == "")
            {
                MessageBox.Show("请填写波形队列名称");
                return;
            }
            if (a郊狼存档.DG波形队列.Keys.Contains(textBox9波形队列名称.Text.Trim()))
            {
                MessageBox.Show("波形队列名称已存在，请更换一个");
                return;
            } 

            


            if (listBox2.Items.Count > 0 && listBox2.SelectedItem != null)
            {
                string 待改名 = listBox2.SelectedItem.ToString ();
                string 新名 = textBox9波形队列名称.Text.Trim();
                波形队列序列化 档案 = a郊狼存档.DG波形队列[listBox2.SelectedItem.ToString()]; 
                a郊狼存档.DG波形队列.Remove(待改名); 
                a郊狼存档.DG波形队列.Add(新名, new 波形队列序列化(vv));
                listBox2.Items.Clear();
                foreach (string a in a郊狼存档.DG波形队列.Keys)
                {
                    listBox2.Items.Add(a);
                } 
            }
            else
            {
                MessageBox.Show("请选择一个需要重命名的项目");
                return;

            }


        }


        List<byte[]> 写入蓝牙列表=new List<byte[]>();

        private void timer写入蓝牙_Tick(object sender, EventArgs e)
        {
            if ( !DG启用Vrc)
            {
                if (写入蓝牙列表.Count > 500)
                {
                    写入蓝牙列表.RemoveRange(0, 400);
                } 
            }
            else
            {
                if (写入蓝牙列表.Count > 5)
                {
                    写入蓝牙列表.RemoveRange(0, 4);
                } 
            }

           



            if (写入蓝牙列表.Count > 0)
            {
                write(写入蓝牙列表[0]);

                写入蓝牙列表.RemoveAt(0);
            }


        }



        string 正在执行中的A通 = "";
        string 正在执行中的B通 = "";
        int index正在执行中的A通 = 0;
        int index正在执行中的B通 = 0;

        int 计数器新 = 0;
        bool 收到单次回应=false;

        int 单次回应等待时间上限 = 5;
        int 单次回应等待时间 =0;

        private void timer新定义波形输出控制_Tick(object sender, EventArgs e)
        {

            if(DG启用Vrc)
            {
                //负责检测参数是否触发。
                if (收到单次回应)
                {   //vrc启用中不可测试

                    DG启用Vrc名字 = 依次获取DG启用Vrc名字();//每轮更新A通//如果不为空则找到激活参数
                    DG启用Vrc名字2 = 依次获取DG启用Vrc名字2();//每轮更新B通//如果不为空则找到激活参数
                    string getA = "";
                    string getB = "";

                    //获取当前进度
                    if (DG启用Vrc名字 != "")
                    {
                        if (DG启用Vrc名字 == 正在执行中的A通)
                        {//执行中 
                         //获得A通道当前执行强度
                            getA = a郊狼存档.DG波形队列[DG启用Vrc名字].波形队列User包含全部格式的英文Tag[index正在执行中的A通];
                            index正在执行中的A通++;

                            if (index正在执行中的A通 >= a郊狼存档.DG波形队列[DG启用Vrc名字].波形队列User包含全部格式的英文Tag.Count)
                            {
                                index正在执行中的A通 = 0;
                            }
                        }
                        else
                        {//切换中 
                            正在执行中的A通 = DG启用Vrc名字;
                            index正在执行中的A通 = 0;
                            getA = a郊狼存档.DG波形队列[DG启用Vrc名字].波形队列User包含全部格式的英文Tag[index正在执行中的A通];
                        }
                    }
                    else
                    {
                        正在执行中的A通 = "";
                        index正在执行中的A通 = 0;
                    }
                    if (DG启用Vrc名字2 != "")
                    {
                        if (DG启用Vrc名字2 == 正在执行中的B通)
                        {//执行中 
                            getB = a郊狼存档.DG波形队列[DG启用Vrc名字2].波形队列User包含全部格式的英文Tag[index正在执行中的B通];
                            index正在执行中的B通++;
                            if (index正在执行中的B通 >= a郊狼存档.DG波形队列[DG启用Vrc名字2].波形队列User包含全部格式的英文Tag.Count)
                            {
                                index正在执行中的B通 = 0;
                            }
                        }
                        else
                        {//切换中 
                            正在执行中的B通 = DG启用Vrc名字2;
                            index正在执行中的B通 = 0;
                            getB = a郊狼存档.DG波形队列[DG启用Vrc名字2].波形队列User包含全部格式的英文Tag[index正在执行中的B通];

                        }
                    }
                    else
                    {
                        正在执行中的B通 = "";
                        index正在执行中的B通 = 0;
                    }
                    if (getA != "" || getB != "")
                    {
                        计数器新++;
                        label53.Text = 计数器新.ToString();
                        byte[] data = string转byte波形AB(getA, getB, float.Parse(textBoxA通强度值.Text), float.Parse(textBoxB通强度值.Text));
                        write(data);
                        收到单次回应 = false;
                    }



                }
                else
                {
                    单次回应等待时间++;
                    if (单次回应等待时间 > 单次回应等待时间上限)
                    {
                        收到单次回应 = true;
                    }
                }


            }
            else
            {
                //测试用触发
                //负责检测参数是否触发。
                if (收到单次回应&& 待执行波形列表.Count >0)
                {   //vrc启用中不可测试

                    if (index正在执行中的A通 >= 待执行波形列表.Count)
                    {
                        index正在执行中的A通 = 0;
                        待执行波形列表.Clear();
                        timer新定义波形输出控制.Enabled = false;
                        return ;    
                    }


                    write(待执行波形列表[index正在执行中的A通]);
                    收到单次回应 = false;
                    index正在执行中的A通++;

                  
                        

                   
                     计数器新++;
                     label53.Text = 计数器新.ToString();
                    
                    



                }
                else
                {
                    单次回应等待时间++;
                    if (单次回应等待时间 > 单次回应等待时间上限)
                    {
                        收到单次回应 = true;
                    }
                }





            }
           

        }

        private void timer新定义写入蓝牙_Tick(object sender, EventArgs e)
        {
            //负责依照固定频率将数据发送给蓝牙

        }

        private void textBox配置名1C2_TextChanged(object sender, EventArgs e)
        {
            a郊狼存档.通道B.textBox配置名1C = textBox配置名1C2.Text;
            groupBox配12.Text = textBox配置名1C2.Text;
        }

        private void textBox配置名2C2_TextChanged(object sender, EventArgs e)
        {
            a郊狼存档.通道B.textBox配置名2C = textBox配置名2C2.Text;
            groupBox配22.Text = textBox配置名2C2.Text;
        }

        private void textBox配置名3C2_TextChanged(object sender, EventArgs e)
        {
            a郊狼存档.通道B.textBox配置名3C = textBox配置名3C2.Text;
            groupBox配32.Text = textBox配置名3C2.Text;
        }

        private void textBox配置名4C2_TextChanged(object sender, EventArgs e)
        {
            a郊狼存档.通道B.textBox配置名4C = textBox配置名4C2.Text;
            groupBox配42.Text = textBox配置名4C2.Text;
        }

        private void textBox配置名5C2_TextChanged(object sender, EventArgs e)
        {
            a郊狼存档.通道B.textBox配置名5C = textBox配置名5C2.Text;
            groupBox配52.Text = textBox配置名5C2.Text;
        }
    } 
}

