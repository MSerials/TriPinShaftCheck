//
//  File generated by HDevelop for HALCON/DOTNET (C#) Version 12.0
//
//  This file is intended to be used with the HDevelopTemplate or
//  HDevelopTemplateWPF projects located under %HALCONEXAMPLES%\c#

using System;
using HalconDotNet;

public partial class HDevelopExport
{
  public HTuple hv_ExpDefaultWinHandle;

  // Procedures 
  // External procedures 
  // Chapter: Graphics / Text
  // Short Description: This procedure displays 'Click 'Run' to continue' in the lower right corner of the screen. 
  public void disp_continue_message (HTuple hv_WindowHandle, HTuple hv_Color, HTuple hv_Box)
  {


  
    // Local iconic variables 

    // Local control variables 

    HTuple hv_ContinueMessage = null, hv_Row = null;
    HTuple hv_Column = null, hv_Width = null, hv_Height = null;
    HTuple hv_Ascent = null, hv_Descent = null, hv_TextWidth = null;
    HTuple hv_TextHeight = null;
    // Initialize local and output iconic variables 
    //This procedure displays 'Press Run (F5) to continue' in the
    //lower right corner of the screen.
    //It uses the procedure disp_message.
    //
    //Input parameters:
    //WindowHandle: The window, where the text shall be displayed
    //Color: defines the text color.
    //   If set to '' or 'auto', the currently set color is used.
    //Box: If set to 'true', the text is displayed in a box.
    //
    hv_ContinueMessage = "Press Run (F5) to continue";
    HOperatorSet.GetWindowExtents(hv_ExpDefaultWinHandle, out hv_Row, out hv_Column, 
        out hv_Width, out hv_Height);
    HOperatorSet.GetStringExtents(hv_ExpDefaultWinHandle, (" "+hv_ContinueMessage)+" ", 
        out hv_Ascent, out hv_Descent, out hv_TextWidth, out hv_TextHeight);
    disp_message(hv_ExpDefaultWinHandle, hv_ContinueMessage, "window", (hv_Height-hv_TextHeight)-12, 
        (hv_Width-hv_TextWidth)-12, hv_Color, hv_Box);

    return;
  }

  // Chapter: Graphics / Text
  // Short Description: This procedure writes a text message. 
  public void disp_message (HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem, 
      HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
  {



      // Local iconic variables 

      // Local control variables 

      HTuple hv_Red = null, hv_Green = null, hv_Blue = null;
      HTuple hv_Row1Part = null, hv_Column1Part = null, hv_Row2Part = null;
      HTuple hv_Column2Part = null, hv_RowWin = null, hv_ColumnWin = null;
      HTuple hv_WidthWin = new HTuple(), hv_HeightWin = null;
      HTuple hv_MaxAscent = null, hv_MaxDescent = null, hv_MaxWidth = null;
      HTuple hv_MaxHeight = null, hv_R1 = new HTuple(), hv_C1 = new HTuple();
      HTuple hv_FactorRow = new HTuple(), hv_FactorColumn = new HTuple();
      HTuple hv_UseShadow = null, hv_ShadowColor = null, hv_Exception = new HTuple();
      HTuple hv_Width = new HTuple(), hv_Index = new HTuple();
      HTuple hv_Ascent = new HTuple(), hv_Descent = new HTuple();
      HTuple hv_W = new HTuple(), hv_H = new HTuple(), hv_FrameHeight = new HTuple();
      HTuple hv_FrameWidth = new HTuple(), hv_R2 = new HTuple();
      HTuple hv_C2 = new HTuple(), hv_DrawMode = new HTuple();
      HTuple hv_CurrentColor = new HTuple();
      HTuple   hv_Box_COPY_INP_TMP = hv_Box.Clone();
      HTuple   hv_Color_COPY_INP_TMP = hv_Color.Clone();
      HTuple   hv_Column_COPY_INP_TMP = hv_Column.Clone();
      HTuple   hv_Row_COPY_INP_TMP = hv_Row.Clone();
      HTuple   hv_String_COPY_INP_TMP = hv_String.Clone();

      // Initialize local and output iconic variables 
    //This procedure displays text in a graphics window.
    //
    //Input parameters:
    //WindowHandle: The WindowHandle of the graphics window, where
    //   the message should be displayed
    //String: A tuple of strings containing the text message to be displayed
    //CoordSystem: If set to 'window', the text position is given
    //   with respect to the window coordinate system.
    //   If set to 'image', image coordinates are used.
    //   (This may be useful in zoomed images.)
    //Row: The row coordinate of the desired text position
    //   If set to -1, a default value of 12 is used.
    //Column: The column coordinate of the desired text position
    //   If set to -1, a default value of 12 is used.
    //Color: defines the color of the text as string.
    //   If set to [], '' or 'auto' the currently set color is used.
    //   If a tuple of strings is passed, the colors are used cyclically
    //   for each new textline.
    //Box: If Box[0] is set to 'true', the text is written within an orange box.
    //     If set to' false', no box is displayed.
    //     If set to a color string (e.g. 'white', '#FF00CC', etc.),
    //       the text is written in a box of that color.
    //     An optional second value for Box (Box[1]) controls if a shadow is displayed:
    //       'true' -> display a shadow in a default color
    //       'false' -> display no shadow (same as if no second value is given)
    //       otherwise -> use given string as color string for the shadow color
    //
    //Prepare window
    HOperatorSet.GetRgb(hv_ExpDefaultWinHandle, out hv_Red, out hv_Green, out hv_Blue);
    HOperatorSet.GetPart(hv_ExpDefaultWinHandle, out hv_Row1Part, out hv_Column1Part, 
        out hv_Row2Part, out hv_Column2Part);
    HOperatorSet.GetWindowExtents(hv_ExpDefaultWinHandle, out hv_RowWin, out hv_ColumnWin, 
        out hv_WidthWin, out hv_HeightWin);
    HOperatorSet.SetPart(hv_ExpDefaultWinHandle, 0, 0, hv_HeightWin-1, hv_WidthWin-1);
    //
    //default settings
    if ((int)(new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(-1))) != 0)
    {
      hv_Row_COPY_INP_TMP = 12;
    }
    if ((int)(new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(-1))) != 0)
    {
      hv_Column_COPY_INP_TMP = 12;
    }
    if ((int)(new HTuple(hv_Color_COPY_INP_TMP.TupleEqual(new HTuple()))) != 0)
    {
      hv_Color_COPY_INP_TMP = "";
    }
    //
    hv_String_COPY_INP_TMP = (((""+hv_String_COPY_INP_TMP)+"")).TupleSplit("\n");
    //
    //Estimate extentions of text depending on font size.
    HOperatorSet.GetFontExtents(hv_ExpDefaultWinHandle, out hv_MaxAscent, out hv_MaxDescent, 
        out hv_MaxWidth, out hv_MaxHeight);
    if ((int)(new HTuple(hv_CoordSystem.TupleEqual("window"))) != 0)
    {
      hv_R1 = hv_Row_COPY_INP_TMP.Clone();
      hv_C1 = hv_Column_COPY_INP_TMP.Clone();
    }
    else
    {
      //Transform image to window coordinates
      hv_FactorRow = (1.0*hv_HeightWin)/((hv_Row2Part-hv_Row1Part)+1);
      hv_FactorColumn = (1.0*hv_WidthWin)/((hv_Column2Part-hv_Column1Part)+1);
      hv_R1 = ((hv_Row_COPY_INP_TMP-hv_Row1Part)+0.5)*hv_FactorRow;
      hv_C1 = ((hv_Column_COPY_INP_TMP-hv_Column1Part)+0.5)*hv_FactorColumn;
    }
    //
    //Display text box depending on text size
    hv_UseShadow = 1;
    hv_ShadowColor = "gray";
    if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(0))).TupleEqual("true"))) != 0)
    {
      if (hv_Box_COPY_INP_TMP == null)
        hv_Box_COPY_INP_TMP = new HTuple();
      hv_Box_COPY_INP_TMP[0] = "#fce9d4";
      hv_ShadowColor = "#f28d26";
    }
    if ((int)(new HTuple((new HTuple(hv_Box_COPY_INP_TMP.TupleLength())).TupleGreater(
        1))) != 0)
    {
      if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(1))).TupleEqual("true"))) != 0)
      {
        //Use default ShadowColor set above
      }
      else if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(1))).TupleEqual(
          "false"))) != 0)
      {
        hv_UseShadow = 0;
      }
      else
      {
        hv_ShadowColor = hv_Box_COPY_INP_TMP[1];
        //Valid color?
        try
        {
          HOperatorSet.SetColor(hv_ExpDefaultWinHandle, hv_Box_COPY_INP_TMP.TupleSelect(
              1));
        }
        // catch (Exception) 
        catch (HalconException HDevExpDefaultException1)
        {
          HDevExpDefaultException1.ToHTuple(out hv_Exception);
          hv_Exception = "Wrong value of control parameter Box[1] (must be a 'true', 'false', or a valid color string)";
          throw new HalconException(hv_Exception);
        }
      }
    }
    if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(0))).TupleNotEqual("false"))) != 0)
    {
      //Valid color?
      try
      {
        HOperatorSet.SetColor(hv_ExpDefaultWinHandle, hv_Box_COPY_INP_TMP.TupleSelect(
            0));
      }
      // catch (Exception) 
      catch (HalconException HDevExpDefaultException1)
      {
        HDevExpDefaultException1.ToHTuple(out hv_Exception);
        hv_Exception = "Wrong value of control parameter Box[0] (must be a 'true', 'false', or a valid color string)";
        throw new HalconException(hv_Exception);
      }
      //Calculate box extents
      hv_String_COPY_INP_TMP = (" "+hv_String_COPY_INP_TMP)+" ";
      hv_Width = new HTuple();
      for (hv_Index=0; (int)hv_Index<=(int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
          ))-1); hv_Index = (int)hv_Index + 1)
      {
        HOperatorSet.GetStringExtents(hv_ExpDefaultWinHandle, hv_String_COPY_INP_TMP.TupleSelect(
            hv_Index), out hv_Ascent, out hv_Descent, out hv_W, out hv_H);
        hv_Width = hv_Width.TupleConcat(hv_W);
      }
      hv_FrameHeight = hv_MaxHeight*(new HTuple(hv_String_COPY_INP_TMP.TupleLength()
          ));
      hv_FrameWidth = (((new HTuple(0)).TupleConcat(hv_Width))).TupleMax();
      hv_R2 = hv_R1+hv_FrameHeight;
      hv_C2 = hv_C1+hv_FrameWidth;
      //Display rectangles
      HOperatorSet.GetDraw(hv_ExpDefaultWinHandle, out hv_DrawMode);
      HOperatorSet.SetDraw(hv_ExpDefaultWinHandle, "fill");
      //Set shadow color
      HOperatorSet.SetColor(hv_ExpDefaultWinHandle, hv_ShadowColor);
      if ((int)(hv_UseShadow) != 0)
      {
        HOperatorSet.DispRectangle1(hv_ExpDefaultWinHandle, hv_R1+1, hv_C1+1, hv_R2+1, 
            hv_C2+1);
      }
      //Set box color
      HOperatorSet.SetColor(hv_ExpDefaultWinHandle, hv_Box_COPY_INP_TMP.TupleSelect(
          0));
      HOperatorSet.DispRectangle1(hv_ExpDefaultWinHandle, hv_R1, hv_C1, hv_R2, hv_C2);
      HOperatorSet.SetDraw(hv_ExpDefaultWinHandle, hv_DrawMode);
    }
    //Write text.
    for (hv_Index=0; (int)hv_Index<=(int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
        ))-1); hv_Index = (int)hv_Index + 1)
    {
      hv_CurrentColor = hv_Color_COPY_INP_TMP.TupleSelect(hv_Index%(new HTuple(hv_Color_COPY_INP_TMP.TupleLength()
          )));
      if ((int)((new HTuple(hv_CurrentColor.TupleNotEqual(""))).TupleAnd(new HTuple(hv_CurrentColor.TupleNotEqual(
          "auto")))) != 0)
      {
        HOperatorSet.SetColor(hv_ExpDefaultWinHandle, hv_CurrentColor);
      }
      else
      {
        HOperatorSet.SetRgb(hv_ExpDefaultWinHandle, hv_Red, hv_Green, hv_Blue);
      }
      hv_Row_COPY_INP_TMP = hv_R1+(hv_MaxHeight*hv_Index);
      HOperatorSet.SetTposition(hv_ExpDefaultWinHandle, hv_Row_COPY_INP_TMP, hv_C1);
      HOperatorSet.WriteString(hv_ExpDefaultWinHandle, hv_String_COPY_INP_TMP.TupleSelect(
          hv_Index));
    }
    //Reset changed window settings
    HOperatorSet.SetRgb(hv_ExpDefaultWinHandle, hv_Red, hv_Green, hv_Blue);
    HOperatorSet.SetPart(hv_ExpDefaultWinHandle, hv_Row1Part, hv_Column1Part, hv_Row2Part, 
        hv_Column2Part);

    return;
  }

  // Main procedure 
  private void action()
  {


    // Stack for temporary objects 
    HObject[] OTemp = new HObject[20];

    // Local iconic variables 

    HObject ho_Image1, ho_red, ho_green, ho_blue;
    HObject ho_ImageGray, ho_ROI_0, ho_TMP_Region, ho_Rectangle;
    HObject ho_EdgeFirst=null, ho_EdgeSecond=null;

    // Local control variables 

    HTuple hv_Width = null, hv_Height = null, hv_WindowHandle = new HTuple();
    HTuple hv_ROI_Num = null, hv_row = null, hv_column = null;
    HTuple hv_angle = null, hv_length1 = null, hv_length2 = null;
    HTuple hv_MeasureHandle = null, hv_RowEdgeFirst = null;
    HTuple hv_ColumnEdgeFirst = null, hv_AmplitudeFirst = null;
    HTuple hv_RowEdgeSecond = null, hv_ColumnEdgeSecond = null;
    HTuple hv_AmplitudeSecond = null, hv_IntraDistance = null;
    HTuple hv_InterDistance = null, hv_i = null;
    // Initialize local and output iconic variables 
    HOperatorSet.GenEmptyObj(out ho_Image1);
    HOperatorSet.GenEmptyObj(out ho_red);
    HOperatorSet.GenEmptyObj(out ho_green);
    HOperatorSet.GenEmptyObj(out ho_blue);
    HOperatorSet.GenEmptyObj(out ho_ImageGray);
    HOperatorSet.GenEmptyObj(out ho_ROI_0);
    HOperatorSet.GenEmptyObj(out ho_TMP_Region);
    HOperatorSet.GenEmptyObj(out ho_Rectangle);
    HOperatorSet.GenEmptyObj(out ho_EdgeFirst);
    HOperatorSet.GenEmptyObj(out ho_EdgeSecond);
    try
    {
      //测量手机电池的高度有多少个像素

      //
      //读取图像
      //
      //Code generated by Image Acquisition 01
      ho_Image1.Dispose();
      HOperatorSet.ReadImage(out ho_Image1, "D:/Users/Lux/Desktop/草稿/halcon/distance.jpg");

      HOperatorSet.GetImageSize(ho_Image1, out hv_Width, out hv_Height);

      //dev_close_window(...);
      //dev_open_window(...);
      //设置所画的区域是一个面还是轮廓
      HOperatorSet.SetDraw(hv_ExpDefaultWinHandle, "margin");
      //设置画的颜色
      HOperatorSet.SetColor(hv_ExpDefaultWinHandle, "black");
      //因为我使用的彩色RGB图像，先使他变成灰度图像
      ho_red.Dispose();ho_green.Dispose();ho_blue.Dispose();
      HOperatorSet.Decompose3(ho_Image1, out ho_red, out ho_green, out ho_blue);
      ho_ImageGray.Dispose();
      HOperatorSet.Rgb3ToGray(ho_red, ho_green, ho_blue, out ho_ImageGray);
      HOperatorSet.DispObj(ho_ImageGray, hv_ExpDefaultWinHandle);

      //









      ho_ROI_0.Dispose();
      HOperatorSet.GenRectangle2(out ho_ROI_0, 224.5, 521.5, (new HTuple(-90)).TupleRad()
          , 102, 8);
      ho_TMP_Region.Dispose();
      HOperatorSet.GenRectangle2(out ho_TMP_Region, 232.5, 777.5, (new HTuple(-90)).TupleRad()
          , 107, 8);
      {
      HObject ExpTmpOutVar_0;
      HOperatorSet.Union2(ho_ROI_0, ho_TMP_Region, out ExpTmpOutVar_0);
      ho_ROI_0.Dispose();
      ho_ROI_0 = ExpTmpOutVar_0;
      }
      ho_TMP_Region.Dispose();
      HOperatorSet.GenRectangle2(out ho_TMP_Region, 387.5, 277.5, (new HTuple(-34.1597)).TupleRad()
          , 101.514, 8);
      {
      HObject ExpTmpOutVar_0;
      HOperatorSet.Union2(ho_ROI_0, ho_TMP_Region, out ExpTmpOutVar_0);
      ho_ROI_0.Dispose();
      ho_ROI_0 = ExpTmpOutVar_0;
      }
      ho_TMP_Region.Dispose();
      HOperatorSet.GenRectangle2(out ho_TMP_Region, 385.5, 540.5, (new HTuple(-39.9364)).TupleRad()
          , 112.161, 8);
      {
      HObject ExpTmpOutVar_0;
      HOperatorSet.Union2(ho_ROI_0, ho_TMP_Region, out ExpTmpOutVar_0);
      ho_ROI_0.Dispose();
      ho_ROI_0 = ExpTmpOutVar_0;
      }
      ho_TMP_Region.Dispose();
      HOperatorSet.GenRectangle2(out ho_TMP_Region, 520.5, 428.5, (new HTuple(-28.9677)).TupleRad()
          , 64.0078, 8);
      {
      HObject ExpTmpOutVar_0;
      HOperatorSet.Union2(ho_ROI_0, ho_TMP_Region, out ExpTmpOutVar_0);
      ho_ROI_0.Dispose();
      ho_ROI_0 = ExpTmpOutVar_0;
      }
      ho_TMP_Region.Dispose();
      HOperatorSet.GenRectangle2(out ho_TMP_Region, 476.5, 746.5, (new HTuple(-30.1735)).TupleRad()
          , 99.4786, 8);
      {
      HObject ExpTmpOutVar_0;
      HOperatorSet.Union2(ho_ROI_0, ho_TMP_Region, out ExpTmpOutVar_0);
      ho_ROI_0.Dispose();
      ho_ROI_0 = ExpTmpOutVar_0;
      }
      ho_TMP_Region.Dispose();
      HOperatorSet.GenRectangle2(out ho_TMP_Region, 551.5, 302.5, (new HTuple(-94.4556)).TupleRad()
          , 77.2334, 8);
      {
      HObject ExpTmpOutVar_0;
      HOperatorSet.Union2(ho_ROI_0, ho_TMP_Region, out ExpTmpOutVar_0);
      ho_ROI_0.Dispose();
      ho_ROI_0 = ExpTmpOutVar_0;
      }



      HOperatorSet.CountObj(ho_ROI_0, out hv_ROI_Num);
      //设置ROI
      //
      //注意row和column是矩形的中心
      hv_row = 238;
      hv_column = 300;
      //这个是矩形旋转的角度，角度是正的按逆时针转，负的按顺时针转
      hv_angle = (new HTuple(90)).TupleRad();
      //在length1<length2的情况下，measure——pairs没有值，不管怎样调整参数？？？
      //length1和length2是矩形的两个半轴的长，明白了这个下面在计算多边形轮廓的时候要用到
      hv_length1 = 200;
      hv_length2 = 10;
      //注意这里的矩形框的参数是自己调整的
      ho_Rectangle.Dispose();
      HOperatorSet.GenRectangle2(out ho_Rectangle, hv_row, hv_column, hv_angle, hv_length1, 
          hv_length2);
      //这里的测量矩形框也就是上面显示的部分
      HOperatorSet.GenMeasureRectangle2(hv_row, hv_column, hv_angle, hv_length1, 
          hv_length2, hv_Width, hv_Height, "nearest_neighbor", out hv_MeasureHandle);

      disp_continue_message(hv_ExpDefaultWinHandle, "black", "true");



      //注意这个算子可以计算许多组边缘，然后将获得的边缘组放在RowEdgeFirst, ColumnEdgeFirst, RowEdgeSecond, ColumnEdgeSecond中
      //根据Thansition的不同值，所获得的边缘组的先后顺序不同，例如为'negative'意思是将‘从白至黑’的边缘 中心点 的坐标放在RowEdgeFirst,注意这里的参考方向，是角度为0时，是从左往右方向
      //ColumnEdgeFirst中，‘从黑到白’放在RowEdgeSecond, ColumnEdgeSecond中
      HOperatorSet.MeasurePairs(ho_Image1, hv_MeasureHandle, 0.9, 30, "negative", 
          "all", out hv_RowEdgeFirst, out hv_ColumnEdgeFirst, out hv_AmplitudeFirst, 
          out hv_RowEdgeSecond, out hv_ColumnEdgeSecond, out hv_AmplitudeSecond, 
          out hv_IntraDistance, out hv_InterDistance);

      disp_continue_message(hv_ExpDefaultWinHandle, "black", "true");
      //
      //可视化结果
      //
      //根据分组的个数，来画线，这个个数就是RowEdgeFirst组的个数，也就是边缘的个数
      for (hv_i=0; (int)hv_i<=(int)((new HTuple(hv_RowEdgeFirst.TupleLength()))-1); hv_i = (int)hv_i + 1)
      {
        //这个算子是画出多边形的亚像素轮廓，其中第二个和第三个参数可以是元素，即表示有多个点，两个点组成一条直线
        //至于每一个点是怎么计算的，我们已经知道了旋转的角度和每一条边的中心点，这个大家自己就可以在纸上用三角函数得出来每一个开始结束点
        //的坐标了
        ho_EdgeFirst.Dispose();
        HOperatorSet.GenContourPolygonXld(out ho_EdgeFirst, (((hv_RowEdgeFirst.TupleSelect(
            hv_i))-((((((new HTuple(90)).TupleRad())-hv_angle)).TupleSin())*hv_length2))).TupleConcat(
            (hv_RowEdgeFirst.TupleSelect(hv_i))+((((((new HTuple(90)).TupleRad())-hv_angle)).TupleSin()
            )*hv_length2)), (((hv_ColumnEdgeFirst.TupleSelect(hv_i))-((((((new HTuple(90)).TupleRad()
            )-hv_angle)).TupleCos())*hv_length2))).TupleConcat((hv_ColumnEdgeFirst.TupleSelect(
            hv_i))+((((((new HTuple(90)).TupleRad())-hv_angle)).TupleCos())*hv_length2)));
        ho_EdgeSecond.Dispose();
        HOperatorSet.GenContourPolygonXld(out ho_EdgeSecond, (((hv_RowEdgeSecond.TupleSelect(
            hv_i))-((((((new HTuple(90)).TupleRad())-hv_angle)).TupleSin())*hv_length2))).TupleConcat(
            (hv_RowEdgeSecond.TupleSelect(hv_i))+((((((new HTuple(90)).TupleRad()
            )-hv_angle)).TupleSin())*hv_length2)), (((hv_ColumnEdgeFirst.TupleSelect(
            hv_i))-((((((new HTuple(90)).TupleRad())-hv_angle)).TupleCos())*hv_length2))).TupleConcat(
            (hv_ColumnEdgeFirst.TupleSelect(hv_i))+((((((new HTuple(90)).TupleRad()
            )-hv_angle)).TupleCos())*hv_length2)));
        HOperatorSet.SetColor(hv_ExpDefaultWinHandle, "cyan");
        HOperatorSet.DispObj(ho_EdgeFirst, hv_ExpDefaultWinHandle);
        HOperatorSet.SetColor(hv_ExpDefaultWinHandle, "red");
        HOperatorSet.DispObj(ho_EdgeSecond, hv_ExpDefaultWinHandle);
        HOperatorSet.SetColor(hv_ExpDefaultWinHandle, "blue");
        //这是设置文本在那里显示
        if ((int)(new HTuple(hv_i.TupleEqual(0))) != 0)
        {
          HOperatorSet.SetTposition(hv_ExpDefaultWinHandle, (hv_RowEdgeFirst.TupleSelect(
              hv_i))+5, (hv_ColumnEdgeFirst.TupleSelect(hv_i))+20);
        }
        else
        {
          HOperatorSet.SetTposition(hv_ExpDefaultWinHandle, (hv_RowEdgeFirst.TupleSelect(
              hv_i))-40, (hv_ColumnEdgeFirst.TupleSelect(hv_i))+20);
        }
        //从文本显示的地方写入字符串，写出像素
        HOperatorSet.WriteString(hv_ExpDefaultWinHandle, ("width: "+(hv_IntraDistance.TupleSelect(
            hv_i)))+" pix");
      }
      disp_continue_message(hv_ExpDefaultWinHandle, "black", "true");

      //
      //销毁
      //
      HOperatorSet.CloseMeasure(hv_MeasureHandle);

    }
    catch (HalconException HDevExpDefaultException)
    {
      ho_Image1.Dispose();
      ho_red.Dispose();
      ho_green.Dispose();
      ho_blue.Dispose();
      ho_ImageGray.Dispose();
      ho_ROI_0.Dispose();
      ho_TMP_Region.Dispose();
      ho_Rectangle.Dispose();
      ho_EdgeFirst.Dispose();
      ho_EdgeSecond.Dispose();

      throw HDevExpDefaultException;
    }
    ho_Image1.Dispose();
    ho_red.Dispose();
    ho_green.Dispose();
    ho_blue.Dispose();
    ho_ImageGray.Dispose();
    ho_ROI_0.Dispose();
    ho_TMP_Region.Dispose();
    ho_Rectangle.Dispose();
    ho_EdgeFirst.Dispose();
    ho_EdgeSecond.Dispose();

  }

  public void InitHalcon(int width = 512, int height = 512)
  {
    // Default settings used in HDevelop 
    HOperatorSet.SetSystem("width", width);
    HOperatorSet.SetSystem("height", height);
  }

  public void RunHalcon(HTuple Window)
  {
    hv_ExpDefaultWinHandle = Window;
    action();
  }

}

