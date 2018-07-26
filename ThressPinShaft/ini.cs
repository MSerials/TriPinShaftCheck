using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ThressPinShaft
{

        public class INI
        {

        public const int PARA_PRJ = (1 << 0);
        public const int PARA_IMAGE = (1 << 1);
        public const int PARA_IO = (1 << 2);
        public const int PARA_ALL = ((1 << 0) | (1 << 1) | (1 << 2));
            /// <summary>
            /// 写操作
            /// </summary>
            /// <param name="section">节</param>
            /// <param name="key">键</param>
            /// <param name="value">值</param>
            /// <param name="filePath">文件路径</param>
            /// <returns></returns>
            [DllImport("kernel32")]
            private static extern long WritePrivateProfileString(string section, string key, string value, string filePath);


            /// <summary>
            /// 读操作
            /// </summary>
            /// <param name="section">节</param>
            /// <param name="key">键</param>
            /// <param name="def">未读取到的默认值</param>
            /// <param name="retVal">读取到的值</param>
            /// <param name="size">大小</param>
            /// <param name="filePath">路径</param>
            /// <returns></returns>
            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

            /// <summary>
            /// 读ini文件
            /// </summary>
            /// <param name="section">节</param>
            /// <param name="key">键</param>
            /// <param name="defValue">未读取到值时的默认值</param>
            /// <param name="filePath">文件路径</param>
            /// <returns></returns>
            public static string ReadIni(string section, string key, string file_name = @"prj.ini")
            {
                StringBuilder temp = new StringBuilder(255);
                int i = GetPrivateProfileString(section, key, "", temp, 255, AppDomain.CurrentDomain.BaseDirectory + "/" + file_name);
                return temp.ToString();
            }

            /// <summary>
            /// 写入ini文件
            /// </summary>
            /// <param name="section">节</param>
            /// <param name="key">键</param>
            /// <param name="value">值</param>
            /// <param name="filePath">文件路径</param>
            /// <returns></returns>
            public static void WriteIni(string section, string key, string value, string file_name = "prj.ini")
            {
                WritePrivateProfileString(section, key, value, AppDomain.CurrentDomain.BaseDirectory + "/" + file_name);
            }
            /// <summary>
            /// 删除节
            /// </summary>
            /// <param name="section">节</param>
            /// <param name="filePath"></param>
            /// <returns></returns>
            public static long DeleteSection(string section, string file_name = @"prj.ini")
            {
                string IniFilePath = file_name;
                return WritePrivateProfileString(section, null, null, IniFilePath);
            }

            /// <summary>
            /// 删除键
            /// </summary>
            /// <param name="section">节</param>
            /// <param name="key">键</param>
            /// <param name="filePath">文件路径</param>
            /// <returns></returns>
            public static long DeleteKey(string section, string key, string file_name = @"prj.ini")
            {
                string IniFilePath = file_name;
                return WritePrivateProfileString(section, key, null, IniFilePath);
            }

            public static void writting(int sel = PARA_ALL) {
                if (PARA_PRJ == (sel & PARA_PRJ)) {

                }

                if (PARA_IMAGE == (sel & PARA_IMAGE)) {
                try {
                    WriteIni("IMAGE", "COMSEL", com_sel);
                    WriteIni("IMAGE", "BAUDRATE", BaudRate);
                    WriteIni("IMAGE", "DATABIT", DataBits);
                    WriteIni("IMAGE", "PARITY", Parity);
                    WriteIni("IMAGE", "STOPBIT", StopBits);

                    WriteIni("IMAGE", "C_X", gear_roi.center_x.ToString());
                    WriteIni("IMAGE", "C_Y", gear_roi.center_y.ToString());
                    WriteIni("IMAGE", "RADIUS", gear_roi.radius.ToString());
                    WriteIni("IMAGE", "GEAR_THRESHOLD", gear_roi.threshold.ToString());
                    WriteIni("IMAGE", "GEAR_IMGTHRESHOLD", gear_roi.imgthreshold.ToString());

                    for (int i = 0; i < 3; i++)
                    {
                        WriteIni("IMAGE", "ADJ_R1_" + i.ToString(), axis_roi[i].adjust_r1.ToString());
                        WriteIni("IMAGE", "ADJ_C1_" + i.ToString(), axis_roi[i].adjust_c1.ToString());
                        WriteIni("IMAGE", "ADJ_R2_" + i.ToString(), axis_roi[i].adjust_r2.ToString());
                        WriteIni("IMAGE", "ADJ_C2_" + i.ToString(), axis_roi[i].adjust_c2.ToString());
                        WriteIni("IMAGE", "ADJ_PHI_" + i.ToString(), axis_roi[i].adjust_phi.ToString());

                        WriteIni("IMAGE", "D1_R1_" + i.ToString(), axis_roi[i].axis_d1_r1.ToString());
                        WriteIni("IMAGE", "D1_C1_" + i.ToString(), axis_roi[i].axis_d1_c1.ToString());
                        WriteIni("IMAGE", "D1_R2_" + i.ToString(), axis_roi[i].axis_d1_r2.ToString());
                        WriteIni("IMAGE", "D1_C2_" + i.ToString(), axis_roi[i].axis_d1_c2.ToString());
                        WriteIni("IMAGE", "D1_PHI_" + i.ToString(), axis_roi[i].axis_d1_phi.ToString());
                        WriteIni("IMAGE", "D1_RELATIVE_PHI_" + i.ToString(), axis_roi[i].axis_d1_relative_phi.ToString());
                        //axis_roi[i].axis_d1_relative_phi = Convert.ToDouble(ReadIni("IMAGE", "D1_RELATIVE_PHI_" + i.ToString()));

                        WriteIni("IMAGE", "D1_MIN_" + i.ToString(), axis_roi[i].d1_min.ToString());
                        WriteIni("IMAGE", "D1_MAX_" + i.ToString(), axis_roi[i].d1_max.ToString());
                        WriteIni("IMAGE", "D1_RATIO_" + i.ToString(), axis_roi[i].d1_mmppix.ToString());
                        WriteIni("IMAGE", "D1_BIAS_" + i.ToString(), axis_roi[i].d1_bias.ToString());

                        WriteIni("IMAGE", "D2_R1_" + i.ToString(), axis_roi[i].axis_d2_r1.ToString());
                        WriteIni("IMAGE", "D2_C1_" + i.ToString(), axis_roi[i].axis_d2_c1.ToString());
                        WriteIni("IMAGE", "D2_R2_" + i.ToString(), axis_roi[i].axis_d2_r2.ToString());
                        WriteIni("IMAGE", "D2_C2_" + i.ToString(), axis_roi[i].axis_d2_c2.ToString());
                        WriteIni("IMAGE", "D2_PHI_" + i.ToString(), axis_roi[i].axis_d2_phi.ToString());
                        WriteIni("IMAGE", "D2_RELATIVE_PHI_" + i.ToString(), axis_roi[i].axis_d2_relative_phi.ToString());


                        WriteIni("IMAGE", "D2_MIN_" + i.ToString(), axis_roi[i].d2_min.ToString());
                        WriteIni("IMAGE", "D2_MAX_" + i.ToString(), axis_roi[i].d2_max.ToString());
                 //       WriteIni("IMAGE", "D2_MAX_" + i.ToString(), axis_roi[i].d2_mmppix.ToString());

                        WriteIni("IMAGE", "D3_R1_" + i.ToString(), axis_roi[i].axis_d3_r1.ToString());
                        WriteIni("IMAGE", "D3_C1_" + i.ToString(), axis_roi[i].axis_d3_c1.ToString());
                        WriteIni("IMAGE", "D3_R2_" + i.ToString(), axis_roi[i].axis_d3_r2.ToString());
                        WriteIni("IMAGE", "D3_C2_" + i.ToString(), axis_roi[i].axis_d3_c2.ToString());
                        WriteIni("IMAGE", "D3_PHI_" + i.ToString(), axis_roi[i].axis_d3_phi.ToString());
                        WriteIni("IMAGE", "D3_RELATIVE_PHI_" + i.ToString(), axis_roi[i].axis_d3_relative_phi.ToString());
                        WriteIni("IMAGE", "D3_RELATIVE_BASE_" + i.ToString(), axis_roi[i].axis_d3_relative_base.ToString());


                        WriteIni("IMAGE", "D3_MIN_" + i.ToString(), axis_roi[i].d3_min.ToString());
                        WriteIni("IMAGE", "D3_MAX_" + i.ToString(), axis_roi[i].d3_max.ToString());
                        WriteIni("IMAGE", "D3_BASE_" + i.ToString(), axis_roi[i].d3_base_h.ToString());
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                   

               
            }

                if (PARA_IO == (sel & PARA_IO)) {

                }
            }

            public static void reading(int sel = PARA_ALL) {
                if (PARA_PRJ == (sel & PARA_PRJ))
                {

                }

                if (PARA_IMAGE == (sel & PARA_IMAGE))
                {
                try
                {
                    com_sel = ReadIni("IMAGE", "COMSEL");
                    BaudRate = ReadIni("IMAGE", "BAUDRATE");
                    DataBits = ReadIni("IMAGE", "DATABIT");
                    Parity = ReadIni("IMAGE", "PARITY");
                    StopBits = ReadIni("IMAGE", "STOPBIT");



                    gear_roi.center_x = Convert.ToDouble(ReadIni("IMAGE", "C_X"));
                    gear_roi.center_y = Convert.ToDouble(ReadIni("IMAGE", "C_Y"));
                    gear_roi.radius = Convert.ToDouble(ReadIni("IMAGE", "RADIUS"));
                    gear_roi.threshold = Convert.ToDouble(ReadIni("IMAGE", "GEAR_THRESHOLD"));
                    gear_roi.imgthreshold = Convert.ToDouble(ReadIni("IMAGE", "GEAR_IMGTHRESHOLD"));   //WriteIni("IMAGE", "GEAR_IMGTHRESHOLD", .ToString());

                    for (int i = 0; i < 3; i++)
                    {
                        axis_roi[i].adjust_r1 = Convert.ToDouble(ReadIni("IMAGE", "ADJ_R1_" + i.ToString()));
                        axis_roi[i].adjust_c1 = Convert.ToDouble(ReadIni("IMAGE", "ADJ_C1_" + i.ToString()));
                        axis_roi[i].adjust_r2 = Convert.ToDouble(ReadIni("IMAGE", "ADJ_R2_" + i.ToString()));
                        axis_roi[i].adjust_c2 = Convert.ToDouble(ReadIni("IMAGE", "ADJ_C2_" + i.ToString()));
                        axis_roi[i].adjust_phi = Convert.ToDouble(ReadIni("IMAGE", "ADJ_PHI_" + i.ToString()));
                    

                        axis_roi[i].axis_d1_r1 = Convert.ToDouble(ReadIni("IMAGE", "D1_R1_" + i.ToString()));
                        axis_roi[i].axis_d1_c1 = Convert.ToDouble(ReadIni("IMAGE", "D1_C1_" + i.ToString()));
                        axis_roi[i].axis_d1_r2 = Convert.ToDouble(ReadIni("IMAGE", "D1_R2_" + i.ToString()));
                        axis_roi[i].axis_d1_c2 = Convert.ToDouble(ReadIni("IMAGE", "D1_C2_" + i.ToString()));
                        axis_roi[i].axis_d1_phi = Convert.ToDouble(ReadIni("IMAGE", "D1_PHI_" + i.ToString()));
                        axis_roi[i].axis_d1_relative_phi = Convert.ToDouble(ReadIni("IMAGE", "D1_RELATIVE_PHI_" + i.ToString()));

                        axis_roi[i].d1_min = Convert.ToDouble(ReadIni("IMAGE", "D1_MIN_" + i.ToString()));
                        axis_roi[i].d1_max = Convert.ToDouble(ReadIni("IMAGE", "D1_MAX_" + i.ToString()));
                        axis_roi[i].d1_mmppix = Convert.ToDouble(ReadIni("IMAGE", "D1_RATIO_" + i.ToString()));
                        axis_roi[i].d1_bias = Convert.ToDouble(ReadIni("IMAGE", "D1_BIAS_" + i.ToString()));

                        axis_roi[i].axis_d2_r1 = Convert.ToDouble(ReadIni("IMAGE", "D2_R1_" + i.ToString()));
                        axis_roi[i].axis_d2_c1 = Convert.ToDouble(ReadIni("IMAGE", "D2_C1_" + i.ToString()));
                        axis_roi[i].axis_d2_r2 = Convert.ToDouble(ReadIni("IMAGE", "D2_R2_" + i.ToString()));
                        axis_roi[i].axis_d2_c2 = Convert.ToDouble(ReadIni("IMAGE", "D2_C2_" + i.ToString()));
                        axis_roi[i].axis_d2_phi = Convert.ToDouble(ReadIni("IMAGE", "D2_PHI_" + i.ToString()));
                        axis_roi[i].axis_d2_relative_phi = Convert.ToDouble(ReadIni("IMAGE", "D2_RELATIVE_PHI_" + i.ToString()));
                        axis_roi[i].d2_min = Convert.ToDouble(ReadIni("IMAGE", "D2_MIN_" + i.ToString()));
                        axis_roi[i].d2_max = Convert.ToDouble(ReadIni("IMAGE", "D2_MAX_" + i.ToString()));
                  //      axis_roi[i].d2_mmppix = Convert.ToDouble(ReadIni("IMAGE", "D2_RATIO_" + i.ToString()));

                        axis_roi[i].axis_d3_r1 = Convert.ToDouble(ReadIni("IMAGE", "D3_R1_" + i.ToString()));
                        axis_roi[i].axis_d3_c1 = Convert.ToDouble(ReadIni("IMAGE", "D3_C1_" + i.ToString()));
                        axis_roi[i].axis_d3_r2 = Convert.ToDouble(ReadIni("IMAGE", "D3_R2_" + i.ToString()));
                        axis_roi[i].axis_d3_c2 = Convert.ToDouble(ReadIni("IMAGE", "D3_C2_" + i.ToString()));
                        axis_roi[i].axis_d3_phi = Convert.ToDouble(ReadIni("IMAGE", "D3_PHI_" + i.ToString()));
                        axis_roi[i].axis_d3_relative_phi = Convert.ToDouble(ReadIni("IMAGE", "D3_RELATIVE_PHI_" + i.ToString()));
                        axis_roi[i].axis_d3_relative_base = Convert.ToDouble(ReadIni("IMAGE", "D3_RELATIVE_BASE_" + i.ToString()));
                        axis_roi[i].d3_min = Convert.ToDouble(ReadIni("IMAGE", "D3_MIN_" + i.ToString()));
                        axis_roi[i].d3_max = Convert.ToDouble(ReadIni("IMAGE", "D3_MAX_" + i.ToString()));
                        axis_roi[i].d3_base_h = Convert.ToDouble(ReadIni("IMAGE", "D3_BASE_" + i.ToString()));
                        //       axis_roi[i].d3_mmppix = Convert.ToDouble(ReadIni("IMAGE", "D3_RATIO_" + i.ToString()));
                        //  public double d1_mmppix = 0.00812;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                }

                if (PARA_IO == (sel & PARA_IO))
                {

                }


            }


        public class roi{
            public roi() {
            }
            public double adjust_r1 = 0;
            public double adjust_c1 = 0;
            public double adjust_r2 = 0;
            public double adjust_c2 = 0;
            public double adjust_phi= 0;
         
            //轴1的roi
            public double axis_d1_r1 = 0;
            public double axis_d1_c1 = 0;
            public double axis_d1_r2 = 0;
            public double axis_d1_c2 = 0;
            public double axis_d1_phi = 0;
            //相对与追踪模板的角度
            public double axis_d1_relative_phi = 0;
            public double d1_min = 20;
            public double d1_max = 3000;
            public double d1_mmppix = 0.00812;
            public double d1_bias = 0;
            //轴2的roi
            public double axis_d2_r1 = 0;
            public double axis_d2_c1 = 0;
            public double axis_d2_r2 = 0;
            public double axis_d2_c2 = 0;
            public double axis_d2_phi = 0;
            public double axis_d2_relative_phi = 0;
            public double d2_min = 20;
            public double d2_max = 3000;
       //     public double d2_mmppix = 0.00812;
            //轴高度的
            public double axis_d3_r1 = 0;
            public double axis_d3_c1 = 0;
            public double axis_d3_r2 = 0;
            public double axis_d3_c2 = 0;
            public double axis_d3_phi =0;
            public double axis_d3_relative_phi = 0;
            //轴高度基准
            public double axis_d3_relative_base = 0;
            public double d3_min = 11.0;
            public double d3_max = 13.0;
            public double d3_base_h = 12.0;
       //     public double d3_mmppix = 0.00812;
        }

        public class roi_chi {
            public roi_chi() {
            }
            public double center_x = 0;
            public double center_y = 0;
            public double radius = 0;
            //相似度边界
            public double threshold = 0.78;
            public double imgthreshold = 50;
        }

        public static roi[] axis_roi= { new roi(),new roi(), new roi()};
        public static roi_chi gear_roi = new roi_chi();
        //串口选择
        public static string com_sel = "COM1";
        public static string BaudRate = "9600";
        public static string DataBits = "8";
        public static string NewLine = "\r\n";
        public static string Parity = "None";
        public static string StopBits = "1";
        }

}
