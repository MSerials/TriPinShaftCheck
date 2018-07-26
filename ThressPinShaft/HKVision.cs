using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using MvCamCtrl.NET;
using HalconDotNet;

namespace ThressPinShaft
{
    class GrabImage
    {
        //static int MaxCamera = 4;
        static private MyCamera[] devices = { new MyCamera(), new MyCamera(), new MyCamera(), new MyCamera() };
        static private bool[] CameraInitOK = { false, false, false, false };
        static IntPtr[] pBufForDriver = { new IntPtr(), new IntPtr(), new IntPtr(), new IntPtr() };// Marshal.AllocHGlobal((int)nPayloadSize);
        static IntPtr[] pBufForSaveImage = { IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero };
        static MyCamera.MV_FRAME_OUT_INFO_EX[] FrameInfo = { new MyCamera.MV_FRAME_OUT_INFO_EX(), new MyCamera.MV_FRAME_OUT_INFO_EX(), new MyCamera.MV_FRAME_OUT_INFO_EX(), new MyCamera.MV_FRAME_OUT_INFO_EX() };
        static UInt32[] nPayloadSize = { 0, 0, 0, 0 };
        //初始化设备
        static public int InitCamera(out string CameraNames) {
            int CmaeraNum = 0;
            CameraNames = "";
            int nRet = MyCamera.MV_OK;
            //   MyCamera device = new MyCamera();
            // ch:枚举设备 | en:Enum device
            MyCamera.MV_CC_DEVICE_INFO_LIST stDevList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
            nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref stDevList);
            if (MyCamera.MV_OK != nRet)
            {
                Console.WriteLine("Enum device failed:{0:x8}", nRet);

            }
            Console.WriteLine("Enum device count : " + Convert.ToString(stDevList.nDeviceNum));
            if (0 == stDevList.nDeviceNum)
            {
                return 0;
            }

            MyCamera.MV_CC_DEVICE_INFO stDevInfo;                            // 通用设备信息

            int DevCounter = 0;
            List<MyCamera.MV_CC_DEVICE_INFO> DevList = new List<MyCamera.MV_CC_DEVICE_INFO>();
            // ch:打印设备信息 en:Print device info
            for (Int32 i = 0; i < stDevList.nDeviceNum; i++)
            {
                stDevInfo = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(stDevList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));

                if (MyCamera.MV_GIGE_DEVICE == stDevInfo.nTLayerType)
                {
                    MyCamera.MV_GIGE_DEVICE_INFO stGigEDeviceInfo = (MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(stDevInfo.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                    uint nIp1 = ((stGigEDeviceInfo.nCurrentIp & 0xff000000) >> 24);
                    uint nIp2 = ((stGigEDeviceInfo.nCurrentIp & 0x00ff0000) >> 16);
                    uint nIp3 = ((stGigEDeviceInfo.nCurrentIp & 0x0000ff00) >> 8);
                    uint nIp4 = (stGigEDeviceInfo.nCurrentIp & 0x000000ff);
                    Console.WriteLine("\n" + i.ToString() + ": [GigE] User Define Name : " + stGigEDeviceInfo.chUserDefinedName);
                    Console.WriteLine("device IP :" + nIp1 + "." + nIp2 + "." + nIp3 + "." + nIp4);
                    DevList.Add(stDevInfo);
                    DevCounter++;
                }
                else if (MyCamera.MV_USB_DEVICE == stDevInfo.nTLayerType)
                {
                    MyCamera.MV_USB3_DEVICE_INFO stUsb3DeviceInfo = (MyCamera.MV_USB3_DEVICE_INFO)MyCamera.ByteToStruct(stDevInfo.SpecialInfo.stUsb3VInfo, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                    Console.WriteLine("\n" + i.ToString() + ": [U3V] User Define Name : " + stUsb3DeviceInfo.chUserDefinedName);
                    Console.WriteLine("\n Serial Number : " + stUsb3DeviceInfo.chSerialNumber);
                    Console.WriteLine("\n Device Number : " + stUsb3DeviceInfo.nDeviceNumber);
                }
            }

            Console.Write("\nPlease input index （0 -- {0:d}） : ", stDevList.nDeviceNum - 1);
            Int32 nDevIndex = Convert.ToInt32(Console.ReadLine());
            stDevInfo = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(stDevList.pDeviceInfo[nDevIndex], typeof(MyCamera.MV_CC_DEVICE_INFO));

            DevList.Sort((a, b) => ((MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(a.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO))).chUserDefinedName.CompareTo(
                 ((MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(b.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO))).chUserDefinedName));

            foreach (var b in DevList)
            {
                CameraNames += ((MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(b.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO))).chUserDefinedName + ",";

            }

            int minDevCounter = DevCounter < 4 ? DevCounter : 4;
            for (int i = 0; i < minDevCounter; i++) {
                stDevInfo = DevList[i];
                nRet = devices[i].MV_CC_CreateDevice_NET(ref stDevInfo);
                if (MyCamera.MV_OK != nRet)
                {
                    continue;
                }

                nRet = devices[i].MV_CC_OpenDevice_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    continue;
                }

                // ch:设置触发模式为off || en:set trigger mode as off
                if (MyCamera.MV_OK != devices[i].MV_CC_SetEnumValue_NET("TriggerMode", 0))
                {
                    continue;
                }

                //设置曝光时间
                float fValue = 5000;
                if (MyCamera.MV_OK != devices[i].MV_CC_SetExposureTime_NET(fValue))
                {
                    continue;
                }


                // ch:开启抓图 | en:start grab
                nRet = devices[i].MV_CC_StartGrabbing_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    Console.WriteLine("Start grabbing failed:{0:x8}", nRet);
                    continue;
                }

                // ch:获取包大小 || en: Get Payload Size
                MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
                nRet = devices[i].MV_CC_GetIntValue_NET("PayloadSize", ref stParam);
                if (MyCamera.MV_OK != nRet)
                {
                    Console.WriteLine("Get PayloadSize failed:{0:x8}", nRet);
                    continue;

                }
                nPayloadSize[i] = stParam.nCurValue;
                //int nCount = 0;
                pBufForDriver[i] = Marshal.AllocHGlobal((int)nPayloadSize[i]);
                CameraInitOK[i] = true;
                CmaeraNum++;
            }

            return CmaeraNum;
        }

        //抓取第几个相机的图片
        static public bool Grab(out HObject Image, int idx = 0)
        {
            Image = null;
            if (false == CameraInitOK[idx])
                return false;
            int nRet = MyCamera.MV_OK;
            try
            {
                HOperatorSet.GenEmptyObj(out Image);
                nRet = devices[idx].MV_CC_GetOneFrameTimeout_NET(pBufForDriver[idx], nPayloadSize[idx], ref FrameInfo[idx], 1000);
                if (MyCamera.MV_OK == nRet)
                {
                    //if (pBufForSaveImage[idx] == IntPtr.Zero)
                    //{
                    //    pBufForSaveImage[idx] = Marshal.AllocHGlobal((int)(FrameInfo[idx].nHeight * FrameInfo[idx].nWidth * 3 + 2048));
                    //}
                    //h FrameInfo[idx].nHeigh
                    //w FrameInfo[idx].nWidth
                    //data pBufForDriver[idx]
                    HOperatorSet.GenImage1(out Image, (HTuple)"byte", FrameInfo[idx].nWidth, FrameInfo[idx].nHeight,(new HTuple(pBufForDriver[idx])));
                }
            }
            catch (HalconException ex)
            {
                throw ex;
            }

            return false;
        }

        static public void close()
        {
            int nRet = MyCamera.MV_OK;
            for (int i=0;i<4;i++)
            {
                try
                {
                    Marshal.FreeHGlobal(pBufForDriver[i]);
                    Marshal.FreeHGlobal(pBufForSaveImage[i]);

                    // ch:停止抓图 | en:Stop grab image
                    nRet = devices[i].MV_CC_StopGrabbing_NET();
                    if (MyCamera.MV_OK != nRet)
                    {
                        Console.WriteLine("Stop grabbing failed{0:x8}", nRet);
                        continue;
                    }

                    // ch:关闭设备 | en:Close device
                    nRet = devices[i].MV_CC_CloseDevice_NET();
                    if (MyCamera.MV_OK != nRet)
                    {
                        Console.WriteLine("Close device failed{0:x8}", nRet);
                        continue;
                    }

                    // ch:销毁设备 | en:Destroy device
                    nRet = devices[i].MV_CC_DestroyDevice_NET();
                    if (MyCamera.MV_OK != nRet)
                    {
                        Console.WriteLine("Destroy device failed:{0:x8}", nRet);
                        continue;
                    }

                    nRet = devices[i].MV_CC_DestroyDevice_NET();
                    if (MyCamera.MV_OK != nRet)
                    {
                        Console.WriteLine("Destroy device failed:{0:x8}", nRet);
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
        } 


        }
        /*
                   
                    // ch:获取一帧图像 | en:Get one image
                    if (MyCamera.MV_OK == nRet)
                    {
                        Console.WriteLine("Get One Frame:" + "Width[" + Convert.ToString(FrameInfo.nWidth) + "] , Height[" + Convert.ToString(FrameInfo.nHeight)
                                        + "] , FrameNum[" + Convert.ToString(FrameInfo.nFrameNum) + "]");

                        if (pBufForSaveImage == IntPtr.Zero)
                        {
                            pBufForSaveImage = Marshal.AllocHGlobal((int)(FrameInfo.nHeight * FrameInfo.nWidth * 3 + 2048));
                        }

                        MyCamera.MV_SAVE_IMAGE_PARAM_EX stSaveParam = new MyCamera.MV_SAVE_IMAGE_PARAM_EX();
                        stSaveParam.enImageType = MyCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Bmp;
                        stSaveParam.enPixelType = FrameInfo.enPixelType;
                        stSaveParam.pData = pBufForDriver;
                        stSaveParam.nDataLen = FrameInfo.nFrameLen;
                        stSaveParam.nHeight = FrameInfo.nHeight;
                        stSaveParam.nWidth = FrameInfo.nWidth;
                        stSaveParam.pImageBuffer = pBufForSaveImage;
                        stSaveParam.nBufferSize = (uint)(FrameInfo.nHeight * FrameInfo.nWidth * 3 + 2048);
                        stSaveParam.nJpgQuality = 80;
                        nRet = device.MV_CC_SaveImageEx_NET(ref stSaveParam);
                        if (MyCamera.MV_OK != nRet)
                        {
                            Console.WriteLine("Save Image failed:{0:x8}", nRet);
                            continue;
                        }

                        // ch:将图像数据保存到本地文件 | en:Save image data to local file
                        byte[] data = new byte[stSaveParam.nImageLen];
                        Marshal.Copy(pBufForSaveImage, data, 0, (int)stSaveParam.nImageLen);
                        FileStream pFile = new FileStream("frame" + nCount.ToString() + ".bmp", FileMode.Create);
                        pFile.Write(data, 0, data.Length);
                        pFile.Close();
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("No data:{0:x8}", nRet);
                    }
                }


                Marshal.FreeHGlobal(pBufForDriver);
                Marshal.FreeHGlobal(pBufForSaveImage);

                // ch:停止抓图 | en:Stop grab image
                nRet = device.MV_CC_StopGrabbing_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    Console.WriteLine("Stop grabbing failed{0:x8}", nRet);
                    break;
                }

                // ch:关闭设备 | en:Close device
                nRet = device.MV_CC_CloseDevice_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    Console.WriteLine("Close device failed{0:x8}", nRet);
                    break;
                }

                // ch:销毁设备 | en:Destroy device
                nRet = device.MV_CC_DestroyDevice_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    Console.WriteLine("Destroy device failed:{0:x8}", nRet);
                    break;
                }
            } while (false);

            if (MyCamera.MV_OK != nRet)
            {
                // ch:销毁设备 | en:Destroy device
                nRet = device.MV_CC_DestroyDevice_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    Console.WriteLine("Destroy device failed:{0:x8}", nRet);
                }
            }

            Console.WriteLine("Press enter to exit");
            Console.ReadKey();
        }
    */
    }
}
