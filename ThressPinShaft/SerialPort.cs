using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace ThressPinShaft
{
        public partial class MainWindow
        {
            //串口设置
            SerialPort serial_port = new SerialPort();
            //private bool serial_port_opened = false;
            delegate void HandleInterfaceUpdateDelagate(string text);//委托       
            HandleInterfaceUpdateDelagate interfaceUpdateHandle;
            static Thread threadReceive = null;

            private void ComSel_DropOpened(object sender, EventArgs e)
            {
            
            string[] sAllPort = null;
            try
            {
                sAllPort = SerialPort.GetPortNames();
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误 " +ex.ToString());
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
             } catch (Exception except) {
                MessageBox.Show("没有选择任何串口 "+ except.ToString());
                }

           

        }

        public void BaundSel_DropDownClosedClick(object sender, EventArgs e) {

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

 

        private void Button_Click_Send_SerialData(object sender, RoutedEventArgs e)
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
            try
            {
               // int i = 0;
                /*
                if (null != threadReceive)
                {
                    threadReceive.Abort();

                    while (threadReceive.ThreadState != ThreadState.Aborted)
                    {
                        //当调用Abort方法后，如果thread线程的状态不为Aborted，主线程就一直在这里做循环，直到thread线程的状态变为Aborted为止
                        Thread.Sleep(500);
                        i++;
                        Console.WriteLine("times to abort " + i.ToString() + "state "+ threadReceive.ThreadState.ToString());
                    }
                    threadReceive = null;
                }
                */
                serial_port.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine("failed to close serial port "+e.ToString());
            }

            try
            {
                serial_port.PortName = INI.com_sel;
                serial_port.BaudRate = Convert.ToInt32(INI.BaudRate);
                serial_port.DataBits = Convert.ToInt32(INI.DataBits);
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
                
                if (null == threadReceive)
                {
                    threadReceive = new Thread(new ParameterizedThreadStart(AsyReceiveData));
                    threadReceive.Start(serial_port);
                }
               // else
               // {
               //     MessageBox.Show("已经打开了一个串口");
               // }
              
            }
            catch (Exception e)
            {
                //serial_port_opened = false;

                throw e;
            }


        }

        void ReceiveData(SerialPort _sp)
        {
            Thread threadReceive = new Thread(new ParameterizedThreadStart(AsyReceiveData));
            threadReceive.Start(_sp);
        }

        //异步读取
        private void AsyReceiveData(object serialPortobj)
        {
            SerialPort serialPort = (SerialPort)serialPortobj;
            System.Threading.Thread.Sleep(500);
            try
            {
                Console.WriteLine("start read");
                int n = serialPort.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致  
                Console.WriteLine("end read");
                byte[] buf = new byte[n];//声明一个临时数组存储当前来的串口数据  
                //received_count += n;//增加接收计数  
                serialPort.Read(buf, 0, n);//读取缓冲数据  
                //因为要访问ui资源，所以需要使用invoke方式同步ui。
                interfaceUpdateHandle = new HandleInterfaceUpdateDelagate(UpdateTextBox);//实例化委托对象
                Dispatcher.Invoke(interfaceUpdateHandle, new string[] { Encoding.ASCII.GetString(buf) });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                //处理错误
            }
            serialPort.Close();
            
        }

        /*
        void SynReceiveData(object _spObj)
        {
            SerialPort _sp = (SerialPort)_spObj;
            System.Threading.Thread.Sleep(0);
            _sp.ReadTimeout = 1000;
            try
            {
                int n = _sp.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致  
                byte[] buf = new byte[n];//声明一个临时数组存储当前来的串口数据  
                                         //received_count += n;//增加接收计数  
                _sp.Read(buf, 0, n);//读取缓冲数据  
                                    //因为要访问ui资源，所以需要使用invoke方式同步ui
                interfaceUpdateHandle = new HandleInterfaceUpdateDelagate(UpdateTextBox);//实例化委托对象
                Dispatcher.Invoke(interfaceUpdateHandle, new string[] { Encoding.ASCII.GetString(buf) });
            }
            catch (Exception e)
            {
           
                    Console.WriteLine("接收数据失败 " + e.Message);
                //处理超时错误
            }
        }
        */
        void UpdateTextBox(string text)
        {
            RecivedData.Text = text;
        }
    }

}
