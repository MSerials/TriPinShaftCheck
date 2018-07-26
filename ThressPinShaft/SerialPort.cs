using HalconDotNet;
using System;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace ThressPinShaft
{
    public partial class MainWindow
    {

        class MyPort
        {
            public MyPort(object port_, object str_)
            {
                port__ = (SerialPort)port_;
                str__ = (string)str_;
            }
            private SerialPort port__;
            public SerialPort port
            {
                set { port__ = value; }
                get { return port__; }
            }

            private string str__;
            public string str
            {
                set { str__ = value; }
                get { return str__; }
            }
        }
        //串口设置
        SerialPort serial_port = new SerialPort();
        //private bool serial_port_opened = false;
        delegate void HandleInterfaceUpdateDelagate(string text);//委托       
        HandleInterfaceUpdateDelagate interfaceUpdateHandle;
        AutoResetEvent[] mEvt_Cam = { new AutoResetEvent(false), new AutoResetEvent(false), new AutoResetEvent(false), new AutoResetEvent(false) };
        AutoResetEvent mEvt_CamCtrl = new AutoResetEvent(false);
        Object thisLock = new Object();

        private void ComSel_DropOpened(object sender, EventArgs e)
        {

            string[] sAllPort = null;
            try
            {
                sAllPort = SerialPort.GetPortNames();
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误 " + ex.ToString());
            }
            lstCOM.Clear();
            foreach (var name in sAllPort)
            {
                lstCOM.Add(name);
            }
            pComSel.ItemsSource = lstCOM;
        }

        public void ComSel_DropDownClosedClick(object sender, EventArgs e)
        {

            try
            {
                string selected_value = pComSel.SelectedValue.ToString();
                INI.com_sel = selected_value;
            }
            catch (Exception except)
            {
                MessageBox.Show("没有选择任何串口 " + except.ToString());
            }



        }

        public void BaundSel_DropDownClosedClick(object sender, EventArgs e)
        {

        }

        public void DataBitSel_DropDownClosedClick(object sender, EventArgs e)
        {

        }

        public void StopBitSel_DropDownClosedClick(object sender, EventArgs e)
        {

        }

        public void ParitySel_DropDownClosedClick(object sender, EventArgs e)
        {

        }


        //按钮点击打开窗口
        private void Button_Click_OpenSP(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenSerialPort();
            }
            catch (Exception _e)
            {
                MessageBox.Show("打开串口失败 " + _e.ToString());
            }
        }

        //打开串口
        void OpenSerialPort()
        {
            Thread.Sleep(500);
            try
            {
                serial_port.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine("failed to close serial port " + e.ToString());
            }

            try
            {
                serial_port.PortName = INI.com_sel;
                serial_port.BaudRate = Convert.ToInt32(INI.BaudRate);
                serial_port.DataBits = Convert.ToInt32(INI.DataBits);
                serial_port.NewLine = "\r\n";

                Console.Write("COM " + INI.com_sel + " BD " + INI.BaudRate + " DataBIt " + INI.DataBits + " Pa " + INI.Parity + " Stopbit " + INI.StopBits);

                if (INI.Parity == "Odd")
                {
                    serial_port.Parity = Parity.Odd;
                }
                else if (INI.Parity == "Even")
                {
                    serial_port.Parity = Parity.Even;
                }
                else
                {
                  //  INI.Parity = "None";
                    serial_port.Parity = Parity.None;
                }

                if ("2" == INI.StopBits)
                {
                    serial_port.StopBits = StopBits.One;
                }
                else if ("1.5" == INI.StopBits)
                {
                    serial_port.StopBits = StopBits.OnePointFive;
                }
                else
                {
                //    INI.StopBits = "One";
                    serial_port.StopBits = StopBits.One;
                }
                serial_port.Open();
                INI.writting();

                // Port_Data.port = serial_port;
                //Port_Data.str = "";


                serial_port.DataReceived += new SerialDataReceivedEventHandler(serialport_DataReceived);
            }
            catch (Exception e)
            {
                Console.WriteLine("串口打开失败");

                throw e;
            }


        }




        //接受串口数据
        public void serialport_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort Port = (SerialPort)sender;
            int SeriaDataLength = Port.BytesToRead;              //得到缓冲区数据长度
            byte[] SeriaData = new byte[SeriaDataLength];         //设置数组
            Port.Read(SeriaData, 0, SeriaDataLength);           //读取缓冲区
            interfaceUpdateHandle = new HandleInterfaceUpdateDelagate(UpdateTextBox);//实例化委托对象
            Dispatcher.Invoke(interfaceUpdateHandle, new string[] { Encoding.ASCII.GetString(SeriaData) });
        }


        //发送数据给PLC
        private void Sender_data_to_plc(string content)
        {
            try
            {
                byte[] bytesSend = System.Text.Encoding.Default.GetBytes(content);
                serial_port.Write(bytesSend, 0, bytesSend.Length);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                history.HistoryNotify += "串口被关闭" + "\r\n";
            }
        }

        //四个相机的线程，由于WPF不允许跨线程更新界面。所以,更新界面的检查就暂时不写了
        void CameraADeal()
        {
            int idx = 0;
          //  HObject Obj;
           // HOperatorSet.GenEmptyObj(out Obj);

            while (true)
            {
                mEvt_Cam[idx].WaitOne();


                try
                {
                    Obj[idx].Dispose();
                    GrabImage.Grab(out Obj[idx],idx);
                    //CameraA.Grab(out Obj[idx]);
                    global.GetIns().GotImage[idx] = 1;
                }
                catch (HalconException ex)
                {
                    try
                    {
                        HOperatorSet.ReadImage(out Obj[idx], @"D:\img\1.bmp");
                        global.GetIns().GotImage[idx] = 1;

                    }
                    catch (HalconException exe)
                    {
                        Console.WriteLine(ex.ToString());
                        global.GetIns().res[idx] = "T1," + "22,";
                        global.GetIns().GotImage[idx] = 0;
                    }
                }
            }

        }


        void CameraBDeal()
        {

            int idx = 1;
           // HObject Obj;
           // HOperatorSet.GenEmptyObj(out Obj);

            while (true)
            {
                mEvt_Cam[idx].WaitOne();

                try
                {
                    Obj[idx].Dispose();
                    GrabImage.Grab(out Obj[idx], idx);
                    //Thread.Sleep(5);
                    global.GetIns().GotImage[idx] = 1;
                }
                catch (HalconException ex)
                {
                    try
                    {
                        HOperatorSet.ReadImage(out Obj[idx], @"D:\img\2.bmp");
                        global.GetIns().GotImage[idx] = 1;

                    }
                    catch (HalconException exe)
                    {
                        global.GetIns().res[idx] = "T2," + "22,";
                        global.GetIns().GotImage[idx] = 0;
                        Console.WriteLine(ex.ToString());
                    }
                }
            }


        }

        void CameraCDeal()
        {

            int idx = 2;
           // HObject Obj;
           // HOperatorSet.GenEmptyObj(out Obj);

            while (true)
            {
                mEvt_Cam[idx].WaitOne();


                try
                {
                    Obj[idx].Dispose();
                    GrabImage.Grab(out Obj[idx], idx);
                    global.GetIns().GotImage[idx] = 1;
                }
                catch (HalconException ex)
                {
                    try
                    {
                        HOperatorSet.ReadImage(out Obj[idx], @"D:\img\3.bmp");
                        global.GetIns().GotImage[idx] = 1;

                    }
                    catch (HalconException exe)
                    {
                        global.GetIns().res[idx] = "T3," + "22,";
                        global.GetIns().GotImage[idx] = 0;
                        Console.WriteLine(exe.ToString());
                    }
                }
            }


        }

        void CameraDDeal()
        {
            int idx = 3;

          //  HObject Obj;
          //  HOperatorSet.GenEmptyObj(out Obj);

            while (true)
            {
                mEvt_Cam[idx].WaitOne();
                try
                {
                    Obj[idx].Dispose();
                    GrabImage.Grab(out Obj[idx], idx);
                    global.GetIns().res[idx] = "T4," + ImageOperate.Check_gear(Obj[idx], this.Cam4_Disp.HalconID, INI.gear_roi.imgthreshold, INI.gear_roi.threshold,ImageOperate.Gear_Model) + ",";
                    global.GetIns().GotImage[idx] = 1;
                }
                catch (HalconException ex)
                {
                    try
                    {
                        HOperatorSet.ReadImage(out Obj[idx], @"D:\img\4.bmp");
                        global.GetIns().GotImage[idx] = 1;

                    }
                    catch (HalconException exe)
                    {
                        global.GetIns().res[idx] = "T4," + "22,";
                        global.GetIns().GotImage[idx] = 0;
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
        }

       void CameraCtrl(object obj)
        {
            string text = (string)obj;
            // interface_camera = ;//实例化委托对象



        }

        // private static MainWindow instance = 
        void CameraControl()
        {
            //halcon支持多线程更新？所以这个地方将就不卡界面？？？
            bool self_lock = false;
            while (true)
            {
                mEvt_CamCtrl.WaitOne();

                while(self_lock)
                {
                    Thread.Sleep(5);
                }

                self_lock = true;
#if DEBUG
                global.GetIns().FromPLC = "T1,T2,T3,T4,\r\n";
#endif
                string[] sArray = global.GetIns().FromPLC.Split(',');// CamSel.Split(',');
                global.GetIns().FromPLC = "";
                bool isFound = false;
                
                if (sArray.Contains("T1"))
                {
                    isFound = true;
                    int idx = 0;
                    global.GetIns().GotImage[idx] = -1;
                    mEvt_Cam[idx].Set();
                }

                if (sArray.Contains("T2"))
                {
                    isFound = true;
                    int idx = 1;
                    global.GetIns().GotImage[idx] = -1;
                    mEvt_Cam[idx].Set();
                }

                if (sArray.Contains("T3"))
                {
                    isFound = true;
                    int idx = 2;
                    global.GetIns().GotImage[idx] = -1;
                    mEvt_Cam[idx].Set();
                }

                if (sArray.Contains("T4"))
                {
                    isFound = true;
                    int idx = 3;
                    global.GetIns().GotImage[idx] = -1;
                    mEvt_Cam[idx].Set();
                }

                string toPLCdata = "";
                if (sArray.Contains("T1"))
                {
                    int idx = 0;
                    int tick = Environment.TickCount;
                    while (global.GetIns().GotImage[idx] == -1)
                    {
                        int CTick = Environment.TickCount;
                        if ((CTick - tick) > 1000)
                        {
                            global.GetIns().GotImage[idx] = 0;
                        }
                       
                    }

                    if (0 == global.GetIns().GotImage[idx])
                    {
                        history.HistoryNotify += DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss ") + "相机一抓图失败...\r\n";
                        toPLCdata += "T1," + "22,";
                    }
                    else
                    {
                        try
                        {
                            global.GetIns().res[idx] = "T1," + ImageOperate.check_axis(Obj[idx], idx, ImageOperate.Track_Model, this.Cam1_Disp.HalconID) + ",";
                            toPLCdata += global.GetIns().res[idx];
                        }
                        catch (Exception ex)
                        {
                            history.HistoryNotify += DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss ") + "相机一参数错误...\r\n";
                            toPLCdata += "T1," + "22,";
                        }
                    }

                }
                else {
                    toPLCdata += "T1," + "99,";
                }

                if (sArray.Contains("T2"))
                {
                    int idx = 1;
                    int tick = Environment.TickCount;
                    while (global.GetIns().GotImage[idx] == -1)
                    {
                        int CTick = Environment.TickCount;

                        if ((CTick - tick) > 1000)
                        {
                            global.GetIns().GotImage[idx] = 0;
                        }
                        Thread.Sleep(5);
                    }

                    if (0 == global.GetIns().GotImage[idx])
                    {
                        history.HistoryNotify += DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss ") + "相机二抓图失败...\r\n";
                        toPLCdata += "T2," + "22,";
                    }
                    else
                    {
                        try
                        {
                            global.GetIns().res[idx] = "T2," + ImageOperate.check_axis(Obj[idx], idx, ImageOperate.Track_Model, this.Cam2_Disp.HalconID) + ",";

                            toPLCdata += global.GetIns().res[idx];
                        }
                        catch (Exception ex)
                        {
                            history.HistoryNotify += DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss ") + "相机二参数错误...\r\n";
                            toPLCdata += "T2," + "22,";
                        }
                        // toPLCdata += "T2," + CameraBDisp.check_axis(Obj[1], this.Cam2_Disp.HalconID) + ",";
                    }

                }
                else {
                    toPLCdata += "T2," + "99,";
                }

                if (sArray.Contains("T3"))
                {
                    int idx = 2;
                    int tick = Environment.TickCount;
                    while (global.GetIns().GotImage[idx] == -1)
                    {
                        int CTick = Environment.TickCount;
                        if ((CTick - tick) > 1000)
                        {
                            global.GetIns().GotImage[idx] = 0;
                        }
                        Thread.Sleep(5);
                    }

                    if (0 == global.GetIns().GotImage[idx])
                    {
                        history.HistoryNotify += DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss ") + "相机三抓图失败...\r\n";
                        toPLCdata += "T3," + "22,";
                    }
                    else
                    {
                        try
                        {
                            global.GetIns().res[idx] = "T3," + ImageOperate.check_axis(Obj[idx], idx, ImageOperate.Track_Model, this.Cam3_Disp.HalconID) + ",";
                            toPLCdata += global.GetIns().res[idx];
                        }
                        catch (Exception ex)
                        {
                            history.HistoryNotify += DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss ") + "相机三抓数错误...\r\n";
                            toPLCdata += "T3," + "22,";
                        }
                    }
                }
                else {
                    //占位符号
                    toPLCdata += "T3," + "99,";
                }

                if (sArray.Contains("T4"))
                {
                    int idx = 3;
                    int tick = Environment.TickCount;
                    while (global.GetIns().GotImage[idx] == -1)
                    {
                        int CTick = Environment.TickCount;
                        if ((CTick - tick) > 1000)
                        {
                            global.GetIns().GotImage[idx] = 0;

                        }
                        Thread.Sleep(5);
                    }

                    if (0 == global.GetIns().GotImage[idx])
                    {
                        history.HistoryNotify += DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss ") + "相机四抓图失败或者参数错误...\r\n";
                        toPLCdata += "T4," + "22,";
                        
                    }
                    else
                    {
                        toPLCdata += global.GetIns().res[idx];
                       // history.HistoryNotify += DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss ") + "相机四参数错误...\r\n";
                       // toPLCdata += "T4," + "22,";
                        /*
                        try
                        {
                           
                            toPLCdata += global.GetIns().res[idx];
                            
                        }
                        catch (Exception)
                        {

                        }
                        */
                    }

                }
                else {
                    toPLCdata += "T4," + "99,";
                }



                toPLCdata += "\r\n";
                if (isFound)
                    Sender_data_to_plc(toPLCdata);
                self_lock = false;
            }

          
            /*

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (ThreadStart)delegate ()
                    {
                        ParserStringCheck("T4");
                    }
                    );
                    */
            //instance.
            // Dispatcher.Invoke(new HandleInterfaceUpdateDelagate(ParserStringCheck), text);
            //  Dispatcher.Invoke(new HandleInterfaceUpdateDelagate(Dispatcher.Invoke(new HandleInterfaceUpdateDelagate(ParserStringCheck), text);), " ");

        }


        
        //接受PLC命令相机抓图
        void UpdateTextBox(string text)
        {
            RecivedData.Text += text;
            Regex r = new Regex("[\r\n|\r|\n]");
            MatchCollection mc = r.Matches(RecivedData.Text);
            if (mc.Count > 0)
            {
                if (global.GetIns().FromPLC.Length > 0)
                {
                    RecivedData.Text = "";
                }
                else
                {
                    global.GetIns().FromPLC = System.Text.RegularExpressions.Regex.Replace(RecivedData.Text, "[\r\n|\r|\n]", "");
                    RecivedData.Text = "";
                    //激活线程去检测，否则可能卡死界面
                    mEvt_CamCtrl.Set();
                }
            }
        }

          
    }
        
}
