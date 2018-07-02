using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using HalconDotNet;

namespace ThressPinShaft
{
    public partial class MainWindow
    {
        //串口设置
        SerialPort serial_port = new SerialPort();
        //private bool serial_port_opened = false;
        delegate void HandleInterfaceUpdateDelagate(string text);//委托       
        HandleInterfaceUpdateDelagate interfaceUpdateHandle;
      //  static Thread threadReceive = null;

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
                    serial_port.StopBits = StopBits.One;
                }
                serial_port.Open();
                serial_port.DataReceived += new SerialDataReceivedEventHandler(serialport_DataReceived);
            }
            catch (Exception e)
            {
                Console.WriteLine("串口打开失败");

                throw e;
            }


        }

        public void serialport_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort Port = (SerialPort)sender;
            int SeriaDataLength = Port.BytesToRead;              //得到缓冲区数据长度
            byte[] SeriaData = new byte[SeriaDataLength];         //设置数组
            Port.Read(SeriaData, 0, SeriaDataLength);           //读取缓冲区
            interfaceUpdateHandle = new HandleInterfaceUpdateDelagate(UpdateTextBox);//实例化委托对象
            Dispatcher.Invoke(interfaceUpdateHandle, new string[] { Encoding.ASCII.GetString(SeriaData) });
        }


        void UpdateTextBox(string text)
        {
            RecivedData.Text += text;
            Regex r = new Regex("[\r\n|\r|\n]");
            MatchCollection mc = r.Matches(RecivedData.Text);
            if (mc.ToString().Length > 0)
            {
                string Text_  = System.Text.RegularExpressions.Regex.Replace(RecivedData.Text, "[\r\n|\r|\n]", "");
                string[] sArray = Text_.Split(',');
                RecivedData.Text = "";
                if (sArray.Contains("T1"))
                {
                    try
                    {
                       CameraA.Grab(out Obj_A); CameraADisp.RunHalcon(this.Cam1_Disp.HalconID, Obj_A);
                       CameraADisp.check_axis(Obj_A, this.Cam1_Disp.HalconID);


                    }
                    catch (HalconException ex)
                    {
                        Console.WriteLine(ex.ToString());
                        history.HistoryNotify += DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss ") + "相机一抓图失败...\r\n";
                    }
                }

                if (sArray.Contains("T2"))
                {

                    try
                    {
                       CameraB.Grab(out Obj_B); CameraBDisp.RunHalcon(this.Cam2_Disp.HalconID, Obj_B);
                       CameraBDisp.check_axis(Obj_B, this.Cam2_Disp.HalconID);
                    }
                    catch (HalconException ex)
                    {
                        Console.WriteLine(ex.ToString());
                        history.HistoryNotify += DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss ") + "相机二抓图失败...\r\n";
                    }
                }
                
                if (sArray.Contains("T3"))
                {
                    try
                    {
                        CameraC.Grab(out Obj_C); CameraCDisp.RunHalcon(this.Cam3_Disp.HalconID, Obj_C);
                        CameraCDisp.check_axis(Obj_C, this.Cam3_Disp.HalconID);
                    }
                    catch (HalconException ex)
                    {
                        Console.WriteLine(ex.ToString());
                        history.HistoryNotify += DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss ") + "相机三抓图失败...\r\n";
                    }
                }

                if (sArray.Contains("T4"))
                {
                    try
                    {
                        CameraD.Grab(out Obj_D);
                        CameraDDisp.check_axis(Obj_D, this.Cam4_Disp.HalconID);
                    }
                    catch (HalconException ex)
                    {
                        Console.WriteLine(ex.ToString());
                        history.HistoryNotify += DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss ") + "相机四抓图失败...\r\n";
                    }
                    
                }
                


            }
        }
    }

}
