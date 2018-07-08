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
using System.Text.RegularExpressions;
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
using static HDevelopExportDisp;

namespace ThressPinShaft
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 
#if DEBUG
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

#endif



    public class global
    {
        public int CamSel = 0;
        public int[] GotImage = { 0, 0, 0, 0 };
        public string[] res = { "22","22","22","22"};
        public HTuple[] Window = { null, null, null, null };
        public string FromPLC = "";
        public string[] sArray;

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
        //public event RaiseEventHandler RaiseEvent;
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
           // a.RaiseEvent += new RaiseEventHandler(a_RaiseEvent);
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
        string[] camera_name = { "CameraA","CameraB","CameraC","CameraD"};
  
        List<string> lstCOM = new List<string>();
        delegate string TestImage(HObject ho_Image, HTuple WindowHandle);

       // static HObject Obj[0]=null, Obj[1]=null, Obj[2]=null, Obj[3]=null, ho_Rectange_Again;
        HObject[] Obj = new HObject[4];

        static HDevelopExportGrab CameraA = new HDevelopExportGrab("A");
        static HDevelopExportDisp CameraADisp = new HDevelopExportDisp();

        static HDevelopExportGrab CameraB = new HDevelopExportGrab("B");
        static HDevelopExportDisp CameraBDisp = new HDevelopExportDisp();

        static HDevelopExportGrab CameraC = new HDevelopExportGrab("C");
        static HDevelopExportDisp CameraCDisp = new HDevelopExportDisp();

        static HDevelopExportGrab CameraD = new HDevelopExportGrab("D");
         static HDevelopExportDisp CameraDDisp = new HDevelopExportDisp();

        HDevelopExportDisp ImageOperate = new HDevelopExportDisp();

        HDevelopExportDisp CameraSettingDisp = new HDevelopExportDisp();
       // HTuple hv_Exception;

        HDevelopExport hd = new HDevelopExport();
        private Mutex mutex;


        private  void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("是否关闭程序?", "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            try
            {
                serial_port.Close();
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        // 主界面完成关闭
        private void Window_Closed(object sender, System.EventArgs e)
        {
            //Application.Current.Shutdown(); // 正常结束
            Environment.Exit(0);              // 强制结束
        }

        private void Button_Click_Get_Image(object sender, RoutedEventArgs e)
        {
            // 在WPF中， OpenFileDialog位于Microsoft.Win32名称空间
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "图像文件(*.jpg; *.jpeg;*.bmp;*.png)| *.jpg; *.jpeg;*.bmp;*.png | 所有文件 | *.* ";
            if (dialog.ShowDialog() != true)
                return;
            Console.WriteLine("第" + global.GetIns().CamSel.ToString() + "相机被选择");
            try
            {
                switch (global.GetIns().CamSel)
                {
                    case 0: Obj[0].Dispose(); HOperatorSet.ReadImage(out Obj[0], dialog.FileName); CameraADisp.RunHalcon(this.CamSetting.HalconID, Obj[0]); break;
                    case 1: Obj[1].Dispose(); HOperatorSet.ReadImage(out Obj[1], dialog.FileName); CameraBDisp.RunHalcon(this.CamSetting.HalconID, Obj[1]); break;
                    case 2: Obj[2].Dispose(); HOperatorSet.ReadImage(out Obj[2], dialog.FileName); CameraCDisp.RunHalcon(this.CamSetting.HalconID, Obj[2]); break;
                    case 3: Obj[3].Dispose(); HOperatorSet.ReadImage(out Obj[3], dialog.FileName); CameraDDisp.RunHalcon(this.CamSetting.HalconID, Obj[3]); break;
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
            try
            {
                switch (global.GetIns().CamSel)
                {
                    case 0: Obj[0].Dispose(); CameraA.Grab(out Obj[0]); CameraADisp.RunHalcon(this.CamSetting.HalconID, Obj[0]); break;
                    case 1: Obj[1].Dispose(); CameraB.Grab(out Obj[1]); CameraBDisp.RunHalcon(this.CamSetting.HalconID, Obj[1]); break;
                    case 2: Obj[2].Dispose(); CameraC.Grab(out Obj[2]); CameraCDisp.RunHalcon(this.CamSetting.HalconID, Obj[2]); break;
                    case 3: Obj[3].Dispose(); CameraD.Grab(out Obj[3]); CameraDDisp.RunHalcon(this.CamSetting.HalconID, Obj[3]); break;
                    default: break;
                }
            }
            catch (HalconException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }



        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
    
            try
            {
                Obj[0].Dispose();
                Obj[1].Dispose();
                Obj[2].Dispose();
                Obj[3].Dispose();
                CameraA.Grab(out Obj[0]);
                CameraB.Grab(out Obj[1]);
                CameraC.Grab(out Obj[2]);
                CameraD.Grab(out Obj[3]);

            }
            catch (HalconException HDevExpDefaultException)
            {

                HOperatorSet.ReadImage(out Obj[0], @"D:\img\1.bmp");
                CameraADisp.RunHalcon(this.Cam1_Disp.HalconID, Obj[0]);
                HOperatorSet.ReadImage(out Obj[1], @"D:\img\2.bmp");
                CameraADisp.RunHalcon(this.Cam2_Disp.HalconID, Obj[1]);
                HOperatorSet.ReadImage(out Obj[2], @"D:\img\3.bmp");
                CameraADisp.RunHalcon(this.Cam3_Disp.HalconID, Obj[2]);
                HOperatorSet.ReadImage(out Obj[3], @"D:\img\4.bmp");
                CameraADisp.RunHalcon(this.Cam4_Disp.HalconID, Obj[3]);
                HTuple hv_exception = null;
                HDevExpDefaultException.ToHTuple(out hv_exception);
            }

            try
            {
                CameraADisp.check_axis(Obj[0], 0, Cam1_Disp.HalconID);
                CameraBDisp.check_axis(Obj[1], 0, Cam2_Disp.HalconID);
                CameraCDisp.check_axis(Obj[2], 0, Cam3_Disp.HalconID);
                CameraDDisp.check_axis(Obj[3], 0, Cam4_Disp.HalconID);

            }
            catch (HalconException HDevExpDefaultException)
            {
                Console.WriteLine(HDevExpDefaultException.ToString());
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
                    case 0: CameraADisp.Disp_Adjust_Line(Obj[0], hv_r2r, hv_r2c, hv_r2phi, hv_r2w, hv_r2h, this.CamSetting.HalconID); break;
                    case 1: CameraBDisp.Disp_Adjust_Line(Obj[1], hv_r2r, hv_r2c, hv_r2phi, hv_r2w, hv_r2h, this.CamSetting.HalconID); break;
                    case 2: CameraCDisp.Disp_Adjust_Line(Obj[2], hv_r2r, hv_r2c, hv_r2phi, hv_r2w, hv_r2h, this.CamSetting.HalconID); break;
                    case 3: CameraDDisp.Disp_Adjust_Line(Obj[3], hv_r2r, hv_r2c, hv_r2phi, hv_r2w, hv_r2h, this.CamSetting.HalconID); break;
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
            HObject ho_Rectange_Again = null;
            HOperatorSet.SetDraw(this.CamSetting.HalconID, "margin");
            HOperatorSet.SetColor(this.CamSetting.HalconID, "#FF00FF");
            HTuple hv_r2r = null, hv_r2c = null, hv_r2phi = null, hv_r2w = null, hv_r2h = null;
            HOperatorSet.DrawRectangle2(this.CamSetting.HalconID, out hv_r2r, out hv_r2c, out hv_r2phi, out hv_r2w, out hv_r2h);
            HOperatorSet.GenRectangle2(out ho_Rectange_Again, hv_r2r, hv_r2c, hv_r2phi, hv_r2w, hv_r2h);
            HTuple hv_c = null, hv_r = null;
            try
            {
                /*
                switch (global.GetIns().CamSel)
                {
                    case 0: CameraADisp.Measure_Diameter(Obj[0], hv_r2r, hv_r2c, hv_r2phi, hv_r2w, hv_r2h, out hv_c, out hv_r, this.CamSetting.HalconID); break;
                    case 1: CameraBDisp.Measure_Diameter(Obj[1], hv_r2r, hv_r2c, hv_r2phi, hv_r2w, hv_r2h, out hv_c, out hv_r, this.CamSetting.HalconID); break;
                    case 2: CameraCDisp.Measure_Diameter(Obj[2], hv_r2r, hv_r2c, hv_r2phi, hv_r2w, hv_r2h, out hv_c, out hv_r, this.CamSetting.HalconID); break;
                   // case 3: CameraDDisp.Measure_Diameter(Obj[3], hv_r2r, hv_r2c, hv_r2phi, hv_r2w, hv_r2h, this.CamSetting.HalconID); break;
                    default: break;
                }
                */
                Info_Ctrl Infc = new Info_Ctrl();
                ImageOperate.Measure_Diameter(Obj[global.GetIns().CamSel], hv_r2r, hv_r2c, hv_r2phi, hv_r2w, hv_r2h, out Infc, this.CamSetting.HalconID);
                INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d1_r1 = hv_r2r - Infc.pos_y;
                INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d1_c1 = hv_r2c - Infc.pos_x;
                INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d1_r2 = hv_r2w;
                INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d1_c2 = hv_r2h;
                INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d1_phi = hv_r2phi - Infc.pos_angle;
                try
                {
                    HTuple hv_Angle = null;
                    HOperatorSet.AngleLx(Infc.pos_y, Infc.pos_x, hv_r2r, hv_r2c, out hv_Angle);
                    INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d1_relative_phi = hv_Angle;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                

                HOperatorSet.DispObj(ho_Rectange_Again, this.CamSetting.HalconID);
                INI.writting();
            }
            catch (Exception ex)
            {
                history.HistoryNotify += ex.ToString() + "\r\n";
                return;
            }


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



        private void Button_Click_Test_Image(object sender, RoutedEventArgs e)
        {
            int idx = global.GetIns().CamSel;
            if (ImageOperate.isEmpty(Obj[idx]))
                return;
            try
            {
                if (idx > 2)
                {
                    ImageOperate.Check_gear(Obj[idx], this.CamSetting.HalconID,INI.gear_roi.imgthreshold,INI.gear_roi.threshold);
                }
                else
                {
                    string res = ImageOperate.check_axis(Obj[idx], idx, this.CamSetting.HalconID);
                    if ("00" == res)
                    {
                        ImageOperate.disp_message(this.CamSetting.HalconID, res, "window", 20, 20, "green", "true", this.CamSetting.HalconID);
                    }
                    else {
                        ImageOperate.disp_message(this.CamSetting.HalconID, res, "window", 20, 20, "red", "true", this.CamSetting.HalconID);
                    }
                }
            }
            catch {
                HOperatorSet.SetColor(this.CamSetting.HalconID, "red");
                CameraADisp.disp_message(this.CamSetting.HalconID, "检测失败", "window",20, 20, "red", "true", this.CamSetting.HalconID);
            }
        }



        private void Button_Click_Slot_Measure(object sender, RoutedEventArgs e)
        {
            HObject ho_Rectange_Again = null;
            HOperatorSet.SetDraw(this.CamSetting.HalconID, "margin");
            HOperatorSet.SetColor(this.CamSetting.HalconID, "green");
            HTuple hv_r2r = null, hv_r2c = null, hv_r2phi = null, hv_r2w = null, hv_r2h = null;
            HOperatorSet.DrawRectangle2(this.CamSetting.HalconID, out hv_r2r, out hv_r2c, out hv_r2phi, out hv_r2w, out hv_r2h);
            HOperatorSet.GenRectangle2(out ho_Rectange_Again, hv_r2r, hv_r2c, hv_r2phi, hv_r2w, hv_r2h);
            HTuple hv_r = null, hv_c = null;
            try
            {
                Info_Ctrl Infc = new Info_Ctrl();
                ImageOperate.Measure_Diameter(Obj[global.GetIns().CamSel], hv_r2r, hv_r2c, hv_r2phi, hv_r2w, hv_r2h, out Infc, this.CamSetting.HalconID);
                
                INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d2_r1 = hv_r2r - Infc.pos_y;
                INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d2_c1 = hv_r2c - Infc.pos_x;
                INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d2_r2 = hv_r2w;
                INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d2_c2 = hv_r2h;
                INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d2_phi = hv_r2phi - Infc.pos_angle;

                try
                {
                    HTuple hv_Angle = null;
                    HOperatorSet.AngleLx(Infc.pos_y, Infc.pos_x, hv_r2r, hv_r2c, out hv_Angle);
                    INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d2_relative_phi = hv_Angle;
                }
                catch (Exception exe)
                {
                    MessageBox.Show(exe.ToString());
                }



                HOperatorSet.DispObj(ho_Rectange_Again, this.CamSetting.HalconID);
                INI.writting();
            }
            catch (HalconException ex)
            {
                history.HistoryNotify += ex.ToString() + "\r\n";
                return;
            }


        }

        private void Button_Click_Gear_Measure(object sender, RoutedEventArgs e)
        {
            HObject Ho_Circle = null;
            HOperatorSet.GenEmptyObj(out Ho_Circle);
            try
            {
                HOperatorSet.SetDraw(this.CamSetting.HalconID, "margin");
                HOperatorSet.SetColor(this.CamSetting.HalconID, "yellow");
                HTuple hv_x = null, hv_y = null, hv_r = null;
                HOperatorSet.DrawCircle(this.CamSetting.HalconID, out hv_y, out hv_x, out hv_r);
                HOperatorSet.GenCircle(out Ho_Circle, hv_y,hv_x,hv_r);
                HOperatorSet.DispObj(Ho_Circle, this.CamSetting.HalconID);
                INI.gear_roi.center_x = hv_x;
                INI.gear_roi.center_x = hv_y;
                INI.gear_roi.center_x = hv_r;
                INI.writting();
                Ho_Circle.Dispose();
            }
            catch (HalconException ex)
            {
                Ho_Circle.Dispose();
                Console.WriteLine(ex.ToString());
            }
      
        }


        private void DrawCircle() {
            HTuple hv_y = null, hv_x = null, hv_r = null;
            HObject Ho_Circle = null;
            HOperatorSet.GenEmptyObj(out Ho_Circle);
            try
            {
                HOperatorSet.SetDraw(this.CamSetting.HalconID, "margin");
                HOperatorSet.SetColor(this.CamSetting.HalconID, "yellow");
                HOperatorSet.DrawCircle(this.CamSetting.HalconID, out hv_y, out hv_x, out hv_r);
                HOperatorSet.GenCircle(out Ho_Circle, hv_y, hv_x, hv_r);
                HOperatorSet.DispObj(Ho_Circle, this.CamSetting.HalconID);
                INI.gear_roi.center_x = hv_x;
                INI.gear_roi.center_y = hv_y;
                INI.gear_roi.radius = hv_r;
                INI.writting();
                Ho_Circle.Dispose();
            }
            catch (HalconException ex)
            {
                INI.gear_roi.center_x = 0;
                INI.gear_roi.center_y = 0;
                INI.gear_roi.radius = 0;
                Ho_Circle.Dispose();
                Console.WriteLine(ex.ToString());
            }
        }

        private void Button_Click_Gear_Get(object sender, RoutedEventArgs e)
        {
#if DEBUG
            Console.WriteLine("drawing circle");
#endif
            int idx = 3;
            if (idx != global.GetIns().CamSel)
                return;
            if (false == Obj[idx].IsInitialized())
                return;


            HObject Ho_Circle = null;
            HTuple hv_y = null, hv_x = null, hv_r = null, threshold_var = null;
            HOperatorSet.GenEmptyObj(out Ho_Circle);
            try
            {
                HOperatorSet.SetDraw(this.CamSetting.HalconID, "margin");
                HOperatorSet.SetColor(this.CamSetting.HalconID, "yellow");
                HOperatorSet.DispObj(Obj[idx], this.CamSetting.HalconID);
                HOperatorSet.DrawCircle(this.CamSetting.HalconID, out hv_y, out hv_x, out hv_r);
                HOperatorSet.GenCircle(out Ho_Circle, hv_y, hv_x, hv_r);
            }
            catch (HalconException ex)
            {
                Ho_Circle.Dispose();
                MessageBox.Show("画搜索框异常");
                return;
            }
            


#if DEBUG
            Console.WriteLine("drawing end");
#endif
 
            try
            {
                threshold_var = INI.gear_roi.imgthreshold;
                CameraDDisp.create_gear_model(Obj[idx],hv_y,hv_x,hv_r,threshold_var, this.CamSetting.HalconID);
                INI.gear_roi.center_x = hv_x;
                INI.gear_roi.center_x = hv_y;
                INI.gear_roi.center_x = hv_r;
                INI.writting();
            }
            catch (HalconException ex)
            {
             
                Console.WriteLine(ex.ToString());
                MessageBox.Show("设置失败，可能图像效果不好");
            }
            Ho_Circle.Dispose();


            INI.gear_roi.center_x = hv_x;
            INI.gear_roi.center_y = hv_y;
            INI.gear_roi.radius = hv_r;
            INI.writting();
        }

        private void Button_Click_Height_Measure(object sender, RoutedEventArgs e)
        {
            /*
            //eerror
            int Cam_idx = global.GetIns().CamSel;
            Info_Ctrl Infc = new Info_Ctrl();
            try
            {
                HObject ho_Rectange_Again = null;
                HOperatorSet.SetDraw(this.CamSetting.HalconID, "margin");
                HOperatorSet.SetColor(this.CamSetting.HalconID, "green");
                HTuple hv_r2r = null, hv_r2c = null, hv_r2phi = null, hv_r2w = null, hv_r2h = null;
                HOperatorSet.DrawRectangle2(this.CamSetting.HalconID, out hv_r2r, out hv_r2c, out hv_r2phi, out hv_r2w, out hv_r2h);
                HOperatorSet.GenRectangle2(out ho_Rectange_Again, hv_r2r, hv_r2c, hv_r2phi, hv_r2w, hv_r2h);
              
                
                if (false == ImageOperate.FindTrackPos(Obj[global.GetIns().CamSel], this.CamSetting.HalconID, out Infc))
                {
                    return ;
                }

                INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d3_r1 = hv_r2r - Infc.pos_y;
                INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d3_c1 = hv_r2c - Infc.pos_x;
                INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d3_r2 = hv_r2w;
                INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d3_c2 = hv_r2h;
                INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d3_phi = hv_r2phi - Infc.pos_angle;

                try
                {
                    HTuple hv_Angle = null;
                    HOperatorSet.AngleLx(Infc.pos_y, Infc.pos_x, hv_r2r, hv_r2c, out hv_Angle);
                    INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d3_relative_phi = hv_Angle;
                }
                catch (Exception exe)
                {
                    MessageBox.Show(exe.ToString());
                }

                HOperatorSet.DispObj(ho_Rectange_Again, this.CamSetting.HalconID);
                INI.writting();
            }
            catch (HalconException ex)
            {
                Console.WriteLine(ex.ToString());
            }

            HObject Ho_Image;
            HTuple hv_c=null, hv_r = null;
            HOperatorSet.GenEmptyObj(out Ho_Image);
            try
            {
                double Angle = CameraADisp.Disp_Adjust_Line(Obj[Cam_idx], INI.axis_roi[Cam_idx].adjust_r1, INI.axis_roi[Cam_idx].adjust_c1, INI.axis_roi[Cam_idx].adjust_phi, INI.axis_roi[Cam_idx].adjust_r2,
                INI.axis_roi[Cam_idx].adjust_c2, this.CamSetting.HalconID, false);
                HOperatorSet.RotateImage(Obj[Cam_idx], out Ho_Image, Angle, "constant");
                HOperatorSet.DispObj(Ho_Image, this.CamSetting.HalconID);

                double y_bias = Infc.pos_y;
                double x_bias = Infc.pos_x;
                double x_center = 0;
                double y_center = 0;
                ImageOperate.GetRelativePos(INI.axis_roi[Cam_idx].axis_d1_r1, INI.axis_roi[Cam_idx].axis_d1_c1, INI.axis_roi[Cam_idx].axis_d1_relative_phi + Angle, out x_center, out y_center);
                y_center = y_bias - y_center;
                x_center = x_bias + x_center;

                ImageOperate.Measure_Diameter(Obj[Cam_idx], y_center, x_center, INI.axis_roi[Cam_idx].axis_d1_phi + Angle, INI.axis_roi[Cam_idx].axis_d1_r2, INI.axis_roi[Cam_idx].axis_d1_c2, out Infc, this.CamSetting.HalconID, false);
                
                CameraADisp.check_height(Obj[Cam_idx], INI.axis_roi[Cam_idx].axis_d3_r1, INI.axis_roi[Cam_idx].axis_d3_c1, Angle, INI.axis_roi[Cam_idx].axis_d3_r2, INI.axis_roi[Cam_idx].axis_d3_c2,Infc.c_x,Infc.c_y, this.CamSetting.HalconID, false);
            }
            catch (HalconException ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Ho_Image.Dispose();
            */


            int Cam_idx = global.GetIns().CamSel;

            Info_Ctrl Infc = new Info_Ctrl();
            if (false == ImageOperate.FindTrackPos(Obj[Cam_idx], this.CamSetting.HalconID, out Infc, false))
            {
                return;
            }
           
            try
            {
                HObject ho_Rectange_Again = null;
                HOperatorSet.SetDraw(this.CamSetting.HalconID, "margin");
                HOperatorSet.SetColor(this.CamSetting.HalconID, "green");
                HTuple hv_r2r = null, hv_r2c = null, hv_r2phi = null, hv_r2w = null, hv_r2h = null;
                HOperatorSet.DrawRectangle2(this.CamSetting.HalconID, out hv_r2r, out hv_r2c, out hv_r2phi, out hv_r2w, out hv_r2h);
                HOperatorSet.GenRectangle2(out ho_Rectange_Again, hv_r2r, hv_r2c, hv_r2phi, hv_r2w, hv_r2h);
                INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d3_r1 = hv_r2r;
                INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d3_c1 = hv_r2c;
                INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d3_r2 = hv_r2w;
                INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d3_c2 = hv_r2h;
                INI.axis_roi.ElementAt(global.GetIns().CamSel).axis_d3_phi = hv_r2phi;
                HOperatorSet.DispObj(ho_Rectange_Again, this.CamSetting.HalconID);
                INI.writting();
            }
            catch (HalconException ex)
            {
                Console.WriteLine(ex.ToString());
            }

            HObject Ho_Image;
            HTuple hv_c = null, hv_r = null;
            HOperatorSet.GenEmptyObj(out Ho_Image);
            try
            {
                double Angle = CameraADisp.Disp_Adjust_Line(Obj[Cam_idx], INI.axis_roi[Cam_idx].adjust_r1, INI.axis_roi[Cam_idx].adjust_c1, INI.axis_roi[Cam_idx].adjust_phi, INI.axis_roi[Cam_idx].adjust_r2,
                INI.axis_roi[Cam_idx].adjust_c2, this.CamSetting.HalconID, false);
                HOperatorSet.RotateImage(Obj[Cam_idx], out Ho_Image, Angle, "constant");
                HOperatorSet.DispObj(Ho_Image, this.CamSetting.HalconID);

                //恢复信息
                double r1 = INI.axis_roi[Cam_idx].axis_d1_r1 + Infc.pos_y;
                double c1 = INI.axis_roi[Cam_idx].axis_d1_c1 + Infc.pos_x;
                CameraADisp.Measure_Diameter(Ho_Image, r1, c1, 0, INI.axis_roi[Cam_idx].axis_d1_r2, INI.axis_roi[Cam_idx].axis_d1_c2, out hv_c, out hv_r, this.CamSetting.HalconID, false);
                CameraADisp.check_height(Ho_Image, INI.axis_roi[Cam_idx].axis_d3_r1, INI.axis_roi[Cam_idx].axis_d3_c1, 0, INI.axis_roi[Cam_idx].axis_d3_r2, INI.axis_roi[Cam_idx].axis_d3_c2, hv_c, hv_r, this.CamSetting.HalconID, false);
            }
            catch (HalconException ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Ho_Image.Dispose();
        }

        private void Button_Click_Save_INI(object sender, RoutedEventArgs e)
        {
            try
            {
                int idx = global.GetIns().CamSel;
                if (idx > 2)
                {
                    INI.gear_roi.threshold = Convert.ToDouble(this.Gear_Threshold.Text);
                    INI.gear_roi.imgthreshold = Convert.ToDouble(this.Image_Threshold.Text);
                }
                else
                {
                    INI.axis_roi[idx].d1_min = Convert.ToDouble(this.D1_Min.Text);
                    INI.axis_roi[idx].d1_max = Convert.ToDouble(this.D1_Max.Text);
                    INI.axis_roi[idx].d1_mmppix = Convert.ToDouble(this.Ratio.Text);


                    INI.axis_roi[idx].d2_min = Convert.ToDouble(this.D2_Min.Text);
                    INI.axis_roi[idx].d2_max = Convert.ToDouble(this.D2_Max.Text);


                    INI.axis_roi[idx].d3_min = Convert.ToDouble(this.D3_Min.Text);
                    INI.axis_roi[idx].d3_max = Convert.ToDouble(this.D3_Max.Text);
                }
                INI.writting();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show("保存失败，可能参数不对,或者删除原有的prj文件重新调整参数");
            }
        }

        private void Button_Click_Save_Cam_Image(object sender, RoutedEventArgs e)
        {
            int idx = global.GetIns().CamSel;
            try
            {
                string AppPath = AppDomain.CurrentDomain.BaseDirectory + "/";
                System.IO.Directory.CreateDirectory(AppPath + "Cam"+(idx+1).ToString());
                string file_name = AppPath + "Cam" + (idx + 1).ToString() +"/"+ DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + "Cam1.bmp";
                HOperatorSet.WriteImage(Obj[idx],"bmp",0, file_name);
            }
            catch (HalconException ex)
            {
                MessageBox.Show("保存失败");
            }

        }

        public void UpdateUI(int sel = 0) {
            if (sel > 2)
            {
                this.Bt_Adjust.Visibility = Visibility.Hidden;
                
                this.Bt_Height.Visibility = Visibility.Hidden;
                this.Bt_Slot.Visibility = Visibility.Hidden;
                this.Bt_D.Visibility = Visibility.Hidden;

                this.D1_Min.Visibility = Visibility.Hidden;
                this.D1_Max.Visibility = Visibility.Hidden;
                this.D2_Min.Visibility = Visibility.Hidden;
                this.D2_Max.Visibility = Visibility.Hidden;
                this.D3_Min.Visibility = Visibility.Hidden;
                this.D3_Max.Visibility = Visibility.Hidden;

                this.L_D1_Min.Visibility = Visibility.Hidden;
                this.L_D1_Max.Visibility = Visibility.Hidden;
                this.L_D2_Min.Visibility = Visibility.Hidden;
                this.L_D2_Max.Visibility = Visibility.Hidden;
                this.L_D3_Min.Visibility = Visibility.Hidden;
                this.L_D3_Max.Visibility = Visibility.Hidden;


                this.Bt_Track.Visibility = Visibility.Hidden;

                this.Ratio.Visibility = Visibility.Hidden;
                this.L_Ratio.Visibility = Visibility.Hidden;
                this.L_Ratio_.Visibility = Visibility.Hidden;

                this.Bt_Gear.Visibility = Visibility.Hidden;
                this.Gear_Threshold.Visibility = Visibility.Visible;
                this.L_Gear_Threshold.Visibility = Visibility.Visible;

                this.Bt_ImgThreshold.Visibility = Visibility.Visible;
                this.Image_Threshold.Visibility = Visibility.Visible;
                this.L_Image_Threshold.Visibility = Visibility.Visible;

                this.Image_Threshold.Text = Convert.ToInt32(INI.gear_roi.imgthreshold.ToString()).ToString();
                this.Gear_Threshold.Text = INI.gear_roi.threshold.ToString();


               
            }
            else
            {
                this.Bt_Adjust.Visibility = Visibility.Visible;
                
                this.Bt_Height.Visibility = Visibility.Visible;
                this.Bt_Slot.Visibility = Visibility.Visible;
                this.Bt_D.Visibility = Visibility.Visible;

                this.D1_Min.Visibility = Visibility.Visible;
                this.D1_Max.Visibility = Visibility.Visible;
                this.D2_Min.Visibility = Visibility.Visible;
                this.D2_Max.Visibility = Visibility.Visible;
                this.D3_Min.Visibility = Visibility.Visible;
                this.D3_Max.Visibility = Visibility.Visible;

                this.L_D1_Min.Visibility = Visibility.Visible;
                this.L_D1_Max.Visibility = Visibility.Visible;
                this.L_D2_Min.Visibility = Visibility.Visible;
                this.L_D2_Max.Visibility = Visibility.Visible;
                this.L_D3_Min.Visibility = Visibility.Visible;
                this.L_D3_Max.Visibility = Visibility.Visible;

                this.Bt_Track.Visibility = Visibility.Visible;

                this.Ratio.Visibility = Visibility.Visible;
                this.L_Ratio.Visibility = Visibility.Visible;
                this.L_Ratio_.Visibility = Visibility.Visible;



                this.Bt_Gear.Visibility = Visibility.Hidden;
          
                this.Gear_Threshold.Visibility = Visibility.Hidden;
                this.L_Gear_Threshold.Visibility = Visibility.Hidden;

                this.Bt_ImgThreshold.Visibility = Visibility.Hidden;
                this.Image_Threshold.Visibility = Visibility.Hidden;
                this.L_Image_Threshold.Visibility = Visibility.Hidden;

  
                this.D1_Min.Text = INI.axis_roi[sel].d1_min.ToString();
                this.D1_Max.Text = INI.axis_roi[sel].d1_max.ToString();

                this.D2_Min.Text = INI.axis_roi[sel].d2_min.ToString();
                this.D2_Max.Text = INI.axis_roi[sel].d2_max.ToString();

                this.D3_Min.Text = INI.axis_roi[sel].d3_min.ToString();
                this.D3_Max.Text = INI.axis_roi[sel].d3_max.ToString();
                this.Ratio.Text = INI.axis_roi[sel].d1_mmppix.ToString();
            }



            INI.writting();

        }

        public void CamSel_DropDownClosedClick(object sender, EventArgs e)
        {
            ComboBox l_CmbBox = sender as ComboBox;
            int sel = Convert.ToInt32(l_CmbBox.SelectedValue.ToString());
            global.GetIns().CamSel = sel;
            UpdateUI(sel);
        }


        [STAThread]
        void App_Startup()
        {
            bool ret;
            mutex = new System.Threading.Mutex(true, "ThreePinShaft", out ret);

            if (!ret)
            {
                MessageBox.Show("程序已经打开");
                Environment.Exit(0);
            }

        }




        //异步抓取图片

        public MainWindow()
        {
            InitializeComponent();
            App_Startup();
            HOperatorSet.GenEmptyObj(out Obj[0]);
            HOperatorSet.GenEmptyObj(out Obj[1]);
            HOperatorSet.GenEmptyObj(out Obj[2]);
            HOperatorSet.GenEmptyObj(out Obj[3]);
            INI.reading();

#if DEBUG
            ConsoleManager.Show();
            System.Console.WriteLine("程序已经启动...");
            
#endif
                int width = (int)this.Cam1_Disp.Width;
                int height = (int)this.Cam1_Disp.Height;
                HOperatorSet.GenEmptyObj(out Obj[0]);                
                CameraADisp.InitHalcon(width, height);

                width = (int)this.Cam2_Disp.Width;
                height = (int)this.Cam2_Disp.Height;
                HOperatorSet.GenEmptyObj(out Obj[1]);                
                CameraBDisp.InitHalcon(width, height);

                width = (int)this.Cam3_Disp.Width;
                height = (int)this.Cam3_Disp.Height;
                HOperatorSet.GenEmptyObj(out Obj[2]);
                CameraCDisp.InitHalcon(width, height);

                width = (int)this.Cam4_Disp.Width;
                height = (int)this.Cam4_Disp.Height;
                HOperatorSet.GenEmptyObj(out Obj[3]);
                CameraDDisp.InitHalcon(width, height);

                width = (int)this.CamSetting.Width;
                height = (int)this.CamSetting.Height;
                CameraSettingDisp.InitHalcon(width, height);




            //halcon 参数载入初始化
            try
            {
                HOperatorSet.ReadShapeModel(AppDomain.CurrentDomain.BaseDirectory + "/" + ImageOperate.Model_File_Name, out ImageOperate.Gear_Model);
            }
            catch (HalconException ex)
            {
                
                Console.WriteLine(ex.ToString());
                MessageBox.Show("模板文件载入失败，无法检测内齿");
            }

            try
            {
                HOperatorSet.ReadShapeModel(AppDomain.CurrentDomain.BaseDirectory + "/" + ImageOperate.Track_Model_Name, out ImageOperate.Track_Model);
            }
            catch (HalconException ex)
            {

                Console.WriteLine(ex.ToString());
                MessageBox.Show("模板文件载入失败，无法检追踪图像");
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



     

            bd.Source = history;
            bd.Path = new PropertyPath("HistoryNotify");
            BindingOperations.SetBinding(this.pTextBoxHistory, TextBox.TextProperty, bd);
            string c_ = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss ") + "启动程序...\r\n";
            history.HistoryNotify += c_;

            new Thread(new ThreadStart(HardwareInit)).Start();
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
                else
                {
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
                else
                {
                    pStopBitsSel.SelectedItem = INI.StopBits;
                }

                List<string> _Parity = new List<string>();
                _Parity.Add("None");
                _Parity.Add("Odd");
                _Parity.Add("Even");
                pParitySel.ItemsSource = _Parity;
                if (!_Parity.Contains(INI.Parity))
                {
                    pParitySel.SelectedItem = "None";
                    ToWrite = true;
                }
                else
                {
                    pParitySel.SelectedItem = INI.Parity;
                }

                if (ToWrite)
                {
                    throw new Exception("未知的串口参数");
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
                    INI.Parity = "None";
                    INI.StopBits = "1";
                    OpenSerialPort();
                    INI.writting();
                }
                catch (Exception exception_info)
                {
                    history.HistoryNotify += DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss ") + "没有找到任何串口,会导致无法通信PLC\r\n";
                }
            }



            global.GetIns().Window[0] = this.Cam1_Disp.HalconID;
            global.GetIns().Window[1] = this.Cam2_Disp.HalconID;
            global.GetIns().Window[2] = this.Cam3_Disp.HalconID;
            global.GetIns().Window[3] = this.Cam4_Disp.HalconID;

            new Thread(new ThreadStart(CameraADeal)).Start();
            new Thread(new ThreadStart(CameraBDeal)).Start();
            new Thread(new ThreadStart(CameraCDeal)).Start();
            new Thread(new ThreadStart(CameraDDeal)).Start();
            new Thread(new ThreadStart(CameraControl)).Start();
            
            UpdateUI();

            this.TextBt.Visibility = Visibility.Hidden;
        }


        void HardwareInit()
        {

            string error_init = "";
            try
            {
                CameraA.InitHalcon(camera_name[0]);
            }
            catch (HalconException HDevExpDefaultException)
            {
                HTuple hv_exception = null;
                HDevExpDefaultException.ToHTuple(out hv_exception);
#if DEBUG
                Console.WriteLine("初始化相机一失败");
#else
                error_init += " 初始化相机一失败";

#endif
            }

            try
            {
                CameraB.InitHalcon(camera_name[1]);
            }
            catch (HalconException HDevExpDefaultException)
            {
                HTuple hv_exception = null;
                HDevExpDefaultException.ToHTuple(out hv_exception);
#if DEBUG
                Console.WriteLine("初始化相机二失败");
#else
                error_init += " 初始化相机二失败";
#endif
            }
            try
            {
                CameraC.InitHalcon(camera_name[2]);
            }
            catch (HalconException HDevExpDefaultException)
            {
                HTuple hv_exception = null;
                HDevExpDefaultException.ToHTuple(out hv_exception);
#if DEBUG
                Console.WriteLine("初始化相机三失败");
#else
                error_init += " 初始化相机三失败";
#endif
            }
            try
            {
                CameraD.InitHalcon(camera_name[3]);
            }
            catch (HalconException HDevExpDefaultException)
            {
                HTuple hv_exception = null;
                HDevExpDefaultException.ToHTuple(out hv_exception);
#if DEBUG
                Console.WriteLine("初始化相机四失败");
#else
                error_init += " 初始化相机四失败";
#endif
            }

            if (error_init.Length > 0)
            {
                MessageBox.Show(error_init);
            }



        }

        private void Button_Click_Find_Track(object sender, RoutedEventArgs e)
        {
            int Cam_idx = global.GetIns().CamSel;
            if (ImageOperate.isEmpty(Obj[Cam_idx]))
                return;

            HObject Ho_Image = null;
            HOperatorSet.GenEmptyObj(out Ho_Image);
            
            try
            {
                //   double Angle = ImageOperate.Disp_Adjust_Line(Obj[Cam_idx], INI.axis_roi[Cam_idx].adjust_r1, INI.axis_roi[Cam_idx].adjust_c1, INI.axis_roi[Cam_idx].adjust_phi, INI.axis_roi[Cam_idx].adjust_r2, INI.axis_roi[Cam_idx].adjust_c2, this.CamSetting.HalconID);
                //    HOperatorSet.RotateImage(Obj[Cam_idx], out Ho_Image, Angle, "constant");

                HOperatorSet.DispObj(Obj[Cam_idx], this.CamSetting.HalconID);
                ImageOperate.CreateTraceModel(Obj[Cam_idx], this.CamSetting.HalconID, out ImageOperate.ctrl_info);
            }
            catch (HalconException ex)
            {

                Ho_Image.Dispose();
                MessageBox.Show("设置失败 " +ex.ToString());
                return;
            }
            Ho_Image.Dispose();
        }
    }


}
