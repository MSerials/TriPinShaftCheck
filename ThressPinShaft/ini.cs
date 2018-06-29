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
                WriteIni("IMAGE", "COMSEL", com_sel);
                for (int i = 0; i < 3; i++)
                {
                    WriteIni("IMAGE", "ADJ_C1_" + i.ToString(), axis_roi[i].adjust_c1.ToString());
                    System.Console.WriteLine(axis_roi[i].adjust_c1.ToString());
                }
            }

                if (PARA_IO == (sel & PARA_IO)) {

                }
            }

            public static void reading(int sel) {
                if (PARA_PRJ == (sel & PARA_PRJ))
                {

                }

                if (PARA_IMAGE == (sel & PARA_IMAGE))
                {
        
                    com_sel = ReadIni("IMAGE", "COMSEL");
                    for (int i = 0; i < 3; i++) {
                    axis_roi[i].adjust_c1 = Convert.ToDouble(ReadIni("IMAGE", "ADJ_C1_"+i.ToString()));
                    }
                }

                if (PARA_IO == (sel & PARA_IO))
                {

                }


            }


        public class roi{
            public roi() {
            }
            public double adjust_r1;
            public double adjust_c1;
            public double adjust_r2;
            public double adjust_c2;
            //轴1的roi
            public double axis_d1_r1;
            public double axis_d1_c1;
            public double axis_d1_r2;
            public double axis_d1_c2;
            //轴2的roi
            public double axis_d2_r1;
            public double axis_d2_c1;
            public double axis_d2_r2;
            public double axis_d2_c2;
        }

        public struct roi_chi {
            double center_x;
            double center_y;
            double radius;
        }

        public static roi[] axis_roi= { new roi(),new roi(), new roi()};
        public static roi_chi gear_roi;
        //串口选择
        public static string com_sel = "COM1";
        public static string BaudRate = "9600";
        public static string DataBits = "8";
        public static string NewLine = "\r\n";
        public static string Parity = "None";
        public static string StopBits = "1";
        }

}
