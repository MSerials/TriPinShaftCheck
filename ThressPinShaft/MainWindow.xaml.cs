using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HalconDotNet;



namespace ThressPinShaft
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    [SuppressUnmanagedCodeSecurity]
    public static class ConsoleManager
    {
        private const string Kernel32_DllName = "kernel32.dll";

        [DllImport(Kernel32_DllName)]
        private static extern bool AllocConsole();

        [DllImport(Kernel32_DllName)]
        private static extern bool FreeConsole();

        [DllImport(Kernel32_DllName)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport(Kernel32_DllName)]
        private static extern int GetConsoleOutputCP();

        public static bool HasConsole
        {
            get { return GetConsoleWindow() != IntPtr.Zero; }
        }

        /// <summary>  
        /// Creates a new console instance if the process is not attached to a console already.  
        /// </summary>  
        public static void Show()
        {
#if DEBUG
            if (!HasConsole)
            {
                AllocConsole();
                InvalidateOutAndError();
            }
#endif
        }

        /// <summary>  
        /// If the process has a console attached to it, it will be detached and no longer visible. Writing to the System.Console is still possible, but no output will be shown.  
        /// </summary>  
        public static void Hide()
        {
#if DEBUG
            if (HasConsole)
            {
                SetOutAndErrorNull();
                FreeConsole();
            }
#endif
        }

        public static void Toggle()
        {
            if (HasConsole)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        static void InvalidateOutAndError()
        {
            Type type = typeof(System.Console);

            System.Reflection.FieldInfo _out = type.GetField("_out",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            System.Reflection.FieldInfo _error = type.GetField("_error",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            System.Reflection.MethodInfo _InitializeStdOutError = type.GetMethod("InitializeStdOutError",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            Debug.Assert(_out != null);
            Debug.Assert(_error != null);

            Debug.Assert(_InitializeStdOutError != null);

            _out.SetValue(null, null);
            _error.SetValue(null, null);

            _InitializeStdOutError.Invoke(null, new object[] { true });
        }

        static void SetOutAndErrorNull()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }
        static void SetOut(string A)
        {

        }
    }



    public class global
    {
        public int CamSel = 0;



        private static global uniqueInstance;
        private global() { }
        public static global GetIns() {
            // 如果类的实例不存在则创建，否则直接返回
            if (uniqueInstance == null)
            {
                uniqueInstance = new global();
            }
            return uniqueInstance;
        }
    }


  
    public delegate void RaiseEventHandler(int CamSel);

    public class ComboxBind
    {
        public event RaiseEventHandler RaiseEvent;
        //构造函数
        public ComboxBind(string _cmbText, int _cmdsel)
        {
            this.cmbText = _cmbText; 
            this.CmbSel = _cmdsel;
        }

        //用于显示值
        private string cmbText;
        public string CmbText
        {
            get { return cmbText; }
            set { cmbText = value; }
        }

        //用于实际选取的值
        private int cmbSel;
        public int CmbSel
        {
            get { return cmbSel; }
            set { cmbSel = value; }
        }
    }

    public class Cam1Setting{
        ComboxBind a;
        public Cam1Setting(ComboxBind a) {
            this.a = a;
            a.RaiseEvent += new RaiseEventHandler(a_RaiseEvent);
        }

        void a_RaiseEvent(int CamSel)
        {
            global.GetIns().CamSel = CamSel;
        }

    }

   


    public partial class isRunnalbe {
        void get() { return; }
        void set() { return; }
        void set(HObject ho_Image) { return; }
        void Runnable() {
        }
    }

    public partial class findBoader : isRunnalbe {
        HObject Image;
        void set(HObject ho_Image)
        {
            Image = ho_Image;
        }
        void Runnable() {

        }
    }

    public class History : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private string historyNotify;
        public string HistoryNotify
        {
            get { return historyNotify; }
            set
            {
                historyNotify = value;
                if (this.PropertyChanged != null) {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("HistoryNotify"));
                }
            }
        }

    }

    public partial class MainWindow : Window
    {
        History history = new History();
        Binding bd = new Binding();
  
        List<string> lstCOM = new List<string>();

        HObject Obj_A, Obj_B, Obj_C, Obj_D, ho_Rectange_Again;

        HDevelopExportGrab CameraA = new HDevelopExportGrab("A");
        HDevelopExportDisp CameraADisp = new HDevelopExportDisp();

        HDevelopExportGrab CameraB = new HDevelopExportGrab("B");
        HDevelopExportDisp CameraBDisp = new HDevelopExportDisp();

        HDevelopExportGrab CameraC = new HDevelopExportGrab("C");
        HDevelopExportDisp CameraCDisp = new HDevelopExportDisp();

        HDevelopExportGrab CameraD = new HDevelopExportGrab("D");
        HDevelopExportDisp CameraDDisp = new HDevelopExportDisp();

        HDevelopExportDisp CameraSettingDisp = new HDevelopExportDisp();
        HTuple hv_Exception;

        private void Button_Click_Get_Image(object sender, RoutedEventArgs e)
        {
            // 在WPF中， OpenFileDialog位于Microsoft.Win32名称空间
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "图像文件(*.jpg; *.jpeg;*.bmp;*.png)| *.jpg; *.jpeg;*.bmp;*.png | 所有文件 | *.* ";
            if (dialog.ShowDialog() != true)
                return;
            try
            {
                switch (global.GetIns().CamSel)
                {
                    case 0: HOperatorSet.ReadImage(out Obj_A, dialog.FileName); CameraADisp.RunHalcon(this.CamSetting.HalconID, Obj_A); break;
                    case 1: HOperatorSet.ReadImage(out Obj_B, dialog.FileName); CameraBDisp.RunHalcon(this.CamSetting.HalconID, Obj_B); break;
                    case 2: HOperatorSet.ReadImage(out Obj_C, dialog.FileName); CameraCDisp.RunHalcon(this.CamSetting.HalconID, Obj_C); break;
                    case 3: HOperatorSet.ReadImage(out Obj_D, dialog.FileName); CameraDDisp.RunHalcon(this.CamSetting.HalconID, Obj_D); break;
                    default: break;
                }

            }
            catch (HalconException HDevExpDefaultException)
            {
                HTuple hv_exception = null;
                HDevExpDefaultException.ToHTuple(out hv_exception);
                return;
            }
        }

        private void Button_Click_Get_Cam_Image(object sender, RoutedEventArgs e)
        {
            switch (global.GetIns().CamSel) {
                case 0: CameraA.Grab(out Obj_A, out hv_Exception); CameraADisp.RunHalcon(this.CamSetting.HalconID, Obj_A); break;
                case 1: CameraB.Grab(out Obj_B, out hv_Exception); CameraBDisp.RunHalcon(this.CamSetting.HalconID, Obj_B); break;
                case 2: CameraC.Grab(out Obj_C, out hv_Exception); CameraCDisp.RunHalcon(this.CamSetting.HalconID, Obj_C); break;
                case 3: CameraD.Grab(out Obj_D, out hv_Exception); CameraDDisp.RunHalcon(this.CamSetting.HalconID, Obj_D); break;
                default:break;
            }
        }



        //private void camera_setting_sel() {

        //}



        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            try
            {
                CameraA.Grab(out Obj_A, out hv_Exception);
                CameraADisp.RunHalcon(this.Cam1_Disp.HalconID, Obj_A);
                hd.RunHalcon(this.Cam2_Disp.HalconID);
                hd.RunHalcon(this.Cam3_Disp.HalconID);
                hd.RunHalcon(this.Cam4_Disp.HalconID);
            }
            catch (HalconException HDevExpDefaultException)
            {

                HOperatorSet.ReadImage(out Obj_A, @"D:\img\1.bmp");
                CameraADisp.RunHalcon(this.Cam1_Disp.HalconID, Obj_A);
                HOperatorSet.ReadImage(out Obj_B, @"D:\img\2.bmp");
                CameraADisp.RunHalcon(this.Cam2_Disp.HalconID, Obj_B);
                HOperatorSet.ReadImage(out Obj_C, @"D:\img\3.bmp");
                CameraADisp.RunHalcon(this.Cam3_Disp.HalconID, Obj_C);
                HOperatorSet.ReadImage(out Obj_D, @"D:\img\4.bmp");
                CameraADisp.RunHalcon(this.Cam4_Disp.HalconID, Obj_D);
                HTuple hv_exception = null;
                HDevExpDefaultException.ToHTuple(out hv_exception);
            }
        }

        private void Button_Click_Find_Boder(object sender, RoutedEventArgs e)
        {
            HOperatorSet.SetDraw(this.CamSetting.HalconID, "margin");
            HOperatorSet.SetColor(this.CamSetting.HalconID, "blue");
            HTuple hv_r2r = null, hv_r2c = null, hv_r2phi = null, hv_r2w = null, hv_r2h = null;
            HOperatorSet.DrawRectangle2(this.CamSetting.HalconID, out hv_r2r, out hv_r2c, out hv_r2phi, out hv_r2w, out hv_r2h);
            try
            {
                switch (global.GetIns().CamSel)
                {
                    case 0: CameraADisp.Disp_Adjust_Line(Obj_A, hv_r2r, hv_r2c, hv_r2phi, hv_r2w, hv_r2h, this.CamSetting.HalconID); break;
                    case 1: CameraBDisp.Disp_Adjust_Line(Obj_B, hv_r2r, hv_r2c, hv_r2phi, hv_r2w, hv_r2h, this.CamSetting.HalconID); break;
                    case 2: CameraCDisp.Disp_Adjust_Line(Obj_C, hv_r2r, hv_r2c, hv_r2phi, hv_r2w, hv_r2h, this.CamSetting.HalconID); break;
                    case 3: CameraDDisp.Disp_Adjust_Line(Obj_D, hv_r2r, hv_r2c, hv_r2phi, hv_r2w, hv_r2h, this.CamSetting.HalconID); break;
                    default: break;
                }
            }
            catch (HalconException ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }

            INI.axis_roi.ElementAt(global.GetIns().CamSel).adjust_c1 = hv_r2c;
            INI.axis_roi.ElementAt(global.GetIns().CamSel).adjust_r1 = hv_r2r;
            INI.axis_roi.ElementAt(global.GetIns().CamSel).adjust_c2 = hv_r2h;
            INI.axis_roi.ElementAt(global.GetIns().CamSel).adjust_r2 = hv_r2w;
            INI.axis_roi.ElementAt(global.GetIns().CamSel).adjust_phi = hv_r2phi;



            INI.writting();

#if DEBUG
            //Console.Write("abcdefg\n");
            System.Console.WriteLine("w {0:G3}, h {1:G3}", hv_r2w, hv_r2h);
#endif
        }


        private void Button_Click_D_Measure(object sender, RoutedEventArgs e)
        {
            HOperatorSet.SetDraw(this.CamSetting.HalconID, "margin");
            HOperatorSet.SetColor(this.CamSetting.HalconID, "yellow");
            HTuple hv_r2r = null, hv_r2c = null, hv_r2phi = null, hv_r2w = null, hv_r2h = null;
            HOperatorSet.DrawRectangle2(this.CamSetting.HalconID, out hv_r2r, out hv_r2c, out hv_r2phi, out hv_r2w, out hv_r2h);
            HOperatorSet.GenRectangle2(out ho_Rectange_Again, hv_r2r, hv_r2c, hv_r2phi, hv_r2w, hv_r2h);

            try
            {
                switch (global.GetIns().CamSel)
                {
                    case 0: CameraADisp.Measure_Diameter(Obj_A, hv_r2r, hv_r2c, hv_r2phi, hv_r2w, hv_r2h, this.CamSetting.HalconID,20,200); break;
                    case 1: CameraBDisp.Measure_Diameter(Obj_B, hv_r2r, hv_r2c, hv_r2phi, hv_r2w, hv_r2h, this.CamSetting.HalconID); break;
                    case 2: CameraCDisp.Measure_Diameter(Obj_C, hv_r2r, hv_r2c, hv_r2phi, hv_r2w, hv_r2h, this.CamSetting.HalconID); break;
                   // case 3: CameraDDisp.Measure_Diameter(Obj_D, hv_r2r, hv_r2c, hv_r2phi, hv_r2w, hv_r2h, this.CamSetting.HalconID); break;
                    default: break;
                }
            }
            catch (HalconException ex)
            {
                history.HistoryNotify += ex.ToString() + "\r\n";
                return;
            }


            INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d1_r1 = hv_r2r;
            INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d1_c1 = hv_r2c;
            INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d1_r2 = hv_r2w;
            INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d1_c2 = hv_r2h;
            INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d1_phi = hv_r2phi;




            HOperatorSet.DispObj(ho_Rectange_Again, this.CamSetting.HalconID);
            INI.writting();
        }

        private void Button_Click_Send_Data(object sender, RoutedEventArgs e)
        {
                try
                {
                    byte[] bytesSend = System.Text.Encoding.Default.GetBytes(SendData.Text);
                    serial_port.Write(bytesSend, 0, bytesSend.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    history.HistoryNotify += "串口被关闭" + "\r\n";
                }
        }

        HDevelopExport hd = new HDevelopExport();
        public void UpdateUI() {
            Console.WriteLine("UpdateUI");
            return;
        }

        public void CamSel_DropDownClosedClick(object sender, EventArgs e)
        {
            ComboBox l_CmbBox = sender as ComboBox;
            global.GetIns().CamSel = Convert.ToInt32(l_CmbBox.SelectedValue.ToString());
            UpdateUI();
        }



        


        //异步抓取图片

        public MainWindow()
        {
                InitializeComponent();
                INI.reading();

#if DEBUG
            ConsoleManager.Show();
            System.Console.WriteLine("程序已经启动...");
            
#endif
                int width = (int)this.Cam1_Disp.Width;
                int height = (int)this.Cam1_Disp.Height;
                HOperatorSet.GenEmptyObj(out Obj_A);                
                CameraADisp.InitHalcon(width, height);

                width = (int)this.Cam2_Disp.Width;
                height = (int)this.Cam2_Disp.Height;
                HOperatorSet.GenEmptyObj(out Obj_B);                
                CameraBDisp.InitHalcon(width, height);

                width = (int)this.Cam3_Disp.Width;
                height = (int)this.Cam3_Disp.Height;
                HOperatorSet.GenEmptyObj(out Obj_C);
                CameraCDisp.InitHalcon(width, height);

                width = (int)this.Cam4_Disp.Width;
                height = (int)this.Cam4_Disp.Height;
                HOperatorSet.GenEmptyObj(out Obj_D);
                CameraDDisp.InitHalcon(width, height);

                width = (int)this.CamSetting.Width;
                height = (int)this.CamSetting.Height;
                CameraSettingDisp.InitHalcon(width, height);

            try
            {
                CameraA.InitHalcon("A");
            } catch (HalconException HDevExpDefaultException)
             {
                HTuple hv_exception = null;
                HDevExpDefaultException.ToHTuple(out hv_exception);
#if DEBUG
#else
                MessageBox.Show("初始化相机一失败");
#endif
            }

            try
            {
                CameraB.InitHalcon("B");
            }
            catch (HalconException HDevExpDefaultException)
            {
                HTuple hv_exception = null;
                HDevExpDefaultException.ToHTuple(out hv_exception);
#if DEBUG
#else
                MessageBox.Show("初始化相机二失败");
#endif
            }
            try
            {
                CameraC.InitHalcon("C");
            }
            catch (HalconException HDevExpDefaultException)
            {
                HTuple hv_exception = null;
                HDevExpDefaultException.ToHTuple(out hv_exception);
#if DEBUG
#else
                MessageBox.Show("初始化相机三失败");
#endif
            }
            try
            {
                CameraD.InitHalcon("D");
            }
            catch (HalconException HDevExpDefaultException)
            {
                HTuple hv_exception = null;
                HDevExpDefaultException.ToHTuple(out hv_exception);
#if DEBUG
#else
                MessageBox.Show("初始化相机四失败");
#endif
            }

            hd.InitHalcon(width, height);

            //相机的combox选择
            List<ComboxBind> lstCmbBind = new List<ComboxBind>();
            lstCmbBind.Add(new ComboxBind("三销轴检测相机1", 0));
            lstCmbBind.Add(new ComboxBind("三销轴检测相机2", 1));
            lstCmbBind.Add(new ComboxBind("三销轴检测相机3", 2));
            lstCmbBind.Add(new ComboxBind("漏拉复拉检测相机", 3));

            this.pCamSel.ItemsSource = lstCmbBind;
            pCamSel.DisplayMemberPath = "CmbText";//类ComboxBind中的属性
            pCamSel.SelectedValuePath = "CmbSel";//类ComboxBind中的属性
            pCamSel.SelectedIndex = 0;



            ;
            try
            {
                bool ToWrite = false;
                List<string> Baund = new List<string>();
                Baund.Add("9600");
                pBaundSel.ItemsSource = Baund;
                if (!Baund.Contains(INI.BaudRate))
                {
                    pBaundSel.SelectedItem = "9600";
                    ToWrite = true;
                }
                else
                    pBaundSel.SelectedItem = INI.BaudRate;

                List<string> DataBits = new List<string>();
                DataBits.Add("8");
                pDataBitsSel.ItemsSource = DataBits;
                if (!DataBits.Contains(INI.DataBits))
                {
                    pDataBitsSel.SelectedItem = "8";
                    ToWrite = true;
                }
                else {
                    pDataBitsSel.SelectedItem = INI.DataBits;
                }

                List<string> StopBits = new List<string>();
                StopBits.Add("1");
                StopBits.Add("1.5");
                StopBits.Add("2");
                pStopBitsSel.ItemsSource = StopBits;
                if (!StopBits.Contains(INI.StopBits))
                {
                    pStopBitsSel.SelectedItem = "1";
                    ToWrite = true;
                }
                else {
                    pStopBitsSel.SelectedItem = INI.StopBits;
                }

                List<string> Parity = new List<string>();
                Parity.Add("None");
                Parity.Add("Odd");
                Parity.Add("Even");
                pParitySel.ItemsSource = Parity;
                if (!Parity.Contains(INI.Parity))
                {
                    pParitySel.ItemsSource = "None";
                    ToWrite = true;
                }
                else {
                    pParitySel.SelectedItem = INI.Parity;
                }

                if (ToWrite)
                {
                    throw new Exception("未知的串口参数") ;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("串口参数选择错误，请在串口设置里重新选择!" + e.ToString());
            }

            //串口的combox选择
            string[] sAllPort = null;
            try
            {
                sAllPort = SerialPort.GetPortNames();
            }
            catch (Exception ex)
            {
                throw new Exception("获取计算机COM口列表失败!\r\n错误信息:" + ex.Message);
            }

            foreach (var name in sAllPort)
            {
                lstCOM.Add(name);
            }
            this.pComSel.ItemsSource = lstCOM;
            if (lstCOM.Contains(INI.com_sel))
            {
                pComSel.SelectedItem = INI.com_sel;
            }


            try
            {
                OpenSerialPort();
            }
            catch (Exception e)
            {
                try
                {
                    pComSel.SelectedItem = sAllPort.ElementAt(0);
                    INI.com_sel = sAllPort.ElementAt(0);
                    INI.BaudRate = "9600";
                    INI.DataBits = "8";
                    OpenSerialPort();
                    INI.writting();
                }
                catch (Exception exception_info)
                {
                    MessageBox.Show("没有找到任何串口,会导致无法通信PLC " + exception_info.ToString() +" " + e.ToString());
                }
            }


            bd.Source = history;
            bd.Path = new PropertyPath("HistoryNotify");
            BindingOperations.SetBinding(this.pTextBoxHistory, TextBox.TextProperty, bd);
            string c_ = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss ") + "启动程序...\r\n";
            history.HistoryNotify += c_;
        }
       

    }


}
