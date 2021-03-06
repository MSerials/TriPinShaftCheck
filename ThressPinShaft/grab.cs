//
//  File generated by HDevelop for HALCON/DOTNET (C#) Version 12.0
//
//  This file is intended to be used with the HDevelopTemplate or
//  HDevelopTemplateWPF projects located under %HALCONEXAMPLES%\c#

using System;
using System.Threading;
using HalconDotNet;

public partial class HDevelopExportGrab
{
    public HDevelopExportGrab(String UID)
    {
        UserID = UID;
    }
  // public HTuple hv_ExpDefaultWinHandle;
  private HTuple hv_AcqHandle = null;
  private String UserID = "C"; 
  // Main procedure 
  private void action(HTuple UID)
  {
        // HTuple hv_exception = new HTuple();
        // Initialize local and output iconic variables 
        //HOperatorSet.GenEmptyObj(out ho_Image);
#if DEBUG
        Console.WriteLine("���ڳ�ʼ��" + UID.ToString());
#endif
     try
    {
            HOperatorSet.OpenFramegrabber("MVision", 0, 0, 0, 0, 0, 0, "default", -1,

//            HOperatorSet.OpenFramegrabber("GigEVision", 0, 0, 0, 0, 0, 0, "default", -1,
                "default", -1, "false", "default", UID, 0, -1, out hv_AcqHandle);
            HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "ExposureTime", 5000);
            HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "Gain", 0);
            HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerMode", "On");
            HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerSource", "Software");
            HOperatorSet.GrabImageStart(hv_AcqHandle, -1);
    }
    catch (HalconException HDevExpDefaultException)
    {
      throw HDevExpDefaultException;
    }


  }

  public void InitHalcon(HTuple str_UserID)
  {
        try
        {
            action(str_UserID);
        }
        catch (HalconException ex)
        {
            throw ex;
        }
  }

    public void Grab(out HObject ho_Image)
    {
        try
        {
            HOperatorSet.GrabImageStart(hv_AcqHandle, -1);
        }
        catch (HalconException ex)
        {

            Console.WriteLine("alread opened" + ex.ToString());
        }
        try
        {
            HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerSoftware", -1);
            Console.WriteLine("triiger");
            HOperatorSet.GenEmptyObj(out ho_Image);
            ho_Image.Dispose();
            HOperatorSet.GrabImageAsync(out ho_Image, hv_AcqHandle, 500);
        }
        catch (HalconException HDevExpDefaultException)
        {
            throw HDevExpDefaultException;
        }
    }
    /*

    public void Grab(out HObject ho_Image, out HTuple hv_exception) {
        hv_exception = null;
        ho_Image = null;
        try
        {
            
            HOperatorSet.GenEmptyObj(out ho_Image);
            ho_Image.Dispose();
            HOperatorSet.GrabImageAsync(out ho_Image, hv_AcqHandle, 1000);
        }
        catch (HalconException HDevExpDefaultException1)
        {
            HDevExpDefaultException1.ToHTuple(out hv_exception);
            throw HDevExpDefaultException1;
        }
    }
    */
  public void RunHalcon()
  {
     
        //action();
        //HOperatorSet.CloseFramegrabber(hv_AcqHandle);
    }

}

