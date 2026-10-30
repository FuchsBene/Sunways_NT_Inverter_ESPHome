// Decompiled with JetBrains decompiler
// Type: de.sunways.ntmonitor.SWNTHRData
// Assembly: SWNTMonLib, Version=1.0.2286.20848, Culture=neutral, PublicKeyToken=null
// MVID: FA1BE6D6-9789-41BC-A3D3-657A8164DEC5
// Assembly location: /NT_Monitor_2.0/SWNTMonLib.dll

using de.sunways.communication.protocol;
using de.sunways.communication.protocol.nt;
using de.sunways.ntmonitor.database;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;

#nullable disable
namespace de.sunways.ntmonitor;

public sealed class SWNTHRData : IDisposable
{
  private static SWNTHRData m_Instance = (SWNTHRData) null;
  private UI_LANG m_UILanguage = UI_LANG.GERMAN;
  private DataSet m_dsUIStatus = (DataSet) null;
  private DataSet m_dsScaling = (DataSet) null;
  private DataSet m_dsSeries = (DataSet) null;
  private readonly long[] m_lngOverflow = new long[16 /*0x10*/]
  {
    0L,
    65536L /*0x010000*/,
    131072L /*0x020000*/,
    196608L /*0x030000*/,
    262144L /*0x040000*/,
    327680L /*0x050000*/,
    393216L /*0x060000*/,
    458752L /*0x070000*/,
    524288L /*0x080000*/,
    589824L /*0x090000*/,
    655360L /*0x0A0000*/,
    720896L /*0x0B0000*/,
    786432L /*0x0C0000*/,
    851968L /*0x0D0000*/,
    917504L /*0x0E0000*/,
    983040L /*0x0F0000*/
  };

  private SWNTHRData()
  {
  }

  private void disposeDataSets()
  {
    if (this.m_dsScaling != null)
      ((MarshalByValueComponent) this.m_dsScaling).Dispose();
    this.m_dsScaling = (DataSet) null;
    if (this.m_dsUIStatus != null)
      ((MarshalByValueComponent) this.m_dsUIStatus).Dispose();
    this.m_dsUIStatus = (DataSet) null;
    if (this.m_dsSeries != null)
      ((MarshalByValueComponent) this.m_dsSeries).Dispose();
    this.m_dsSeries = (DataSet) null;
  }

  private void init()
  {
    SWNTDBAccess instance = SWNTDBAccess.GetInstance();
    if (!instance.Initialized)
      throw new Exception("SWNTDBAccess was not initialized");
    this.disposeDataSets();
    string stringSelect1 = "SELECT * FROM tbsStatus WHERE idUILanguage = " + (object) Convert.ToInt32((object) this.UILanguage);
    this.m_dsUIStatus = instance.SQLQuery(stringSelect1);
    string stringSelect2 = "SELECT * FROM tbsScaling";
    this.m_dsScaling = instance.SQLQuery(stringSelect2);
    string stringSelect3 = "SELECT * FROM tbsSeries";
    this.m_dsSeries = instance.SQLQuery(stringSelect3);
  }

  public static SWNTHRData GetInstance()
  {
    if (SWNTHRData.m_Instance == null)
    {
      SWNTHRData.m_Instance = new SWNTHRData();
      SWNTHRData.m_Instance.init();
    }
    return SWNTHRData.m_Instance;
  }

  private SERIES decodeWR(byte WR) => (SERIES) ((int) WR & 58);

  public SERIES DecodeWR(byte WR, out string Series)
  {
    Series = "";
    SERIES series = SERIES.UNKNOWN;
    if (this.m_dsSeries != null)
    {
      series = this.decodeWR(WR);
      DataRow[] dataRowArray = this.m_dsSeries.Tables[0].Select("idSeries=" + ((int) series).ToString());
      if (dataRowArray != null && dataRowArray.Length > 0)
        Series = dataRowArray[0]["strName"].ToString();
    }
    return series;
  }

  private double decodeOverflow(byte WR)
  {
    return Convert.ToDouble(this.m_lngOverflow[(int) (byte) ((uint) (byte) ((uint) (byte) ((uint) (byte) (0U + (uint) (byte) (((int) WR & 128 /*0x80*/) >> 4)) + (uint) (byte) (((int) WR & 64 /*0x40*/) >> 4)) + (uint) (byte) (((int) WR & 4) >> 1)) + (uint) (byte) ((uint) WR & 1U))]);
  }

  private bool decodeOLV(
    SWNTCommand Command,
    PROTOCOL Protocol,
    int Series,
    SWNTHRResultData hrRes)
  {
    bool flag = true;
    byte[] reply = Command.GetReply();
    SWNTHRScaling swnthrScaling = new SWNTHRScaling(this.m_dsScaling, Series);
    double num1;
    double num2;
    if (reply[4] == (byte) 0 && reply[5] == (byte) 0 || reply[4] == (byte) 1 && reply[5] == (byte) 1)
    {
      num1 = double.NaN;
      num2 = double.NaN;
    }
    else
    {
      num1 = Convert.ToDouble(reply[4]) * swnthrScaling.dblTEMPGradient + swnthrScaling.dblTEMPOffset;
      num2 = Convert.ToDouble(reply[5]) * swnthrScaling.dblSOLARGradient + swnthrScaling.dblSOLAROffset;
    }
    if (Command.Blocknummer == byte.MaxValue && Protocol == PROTOCOL.V12)
    {
      SWNTHROLVSUMData swnthrolvsumData = new SWNTHROLVSUMData(Command)
      {
        PDC = Convert.ToDouble((int) reply[0] * 256 /*0x0100*/ + (int) reply[1]),
        PAC = Convert.ToDouble((int) reply[2] * 256 /*0x0100*/ + (int) reply[3]),
        Temp = num1,
        Solar = num2,
        PowerDay = Convert.ToDouble((int) reply[6] * 256 /*0x0100*/ + (int) reply[7]) * 10.0,
        Overflow = this.decodeOverflow(reply[11])
      };
      swnthrolvsumData.PowerSUM = Convert.ToDouble((int) reply[8] * 256 /*0x0100*/ + (int) reply[9]) + swnthrolvsumData.Overflow;
      swnthrolvsumData.PowerSUM *= 1000.0;
      swnthrolvsumData.Status = Convert.ToInt32(reply[10]);
      swnthrolvsumData.UIStatus = this.getUIStatus(swnthrolvsumData.Status, Series);
      swnthrolvsumData.Date = Command.CommandEnded;
      hrRes.ArrayResult.Add((object) swnthrolvsumData);
    }
    else
    {
      SWNTHROLVData swnthrolvData = new SWNTHROLVData(Command);
      swnthrolvData.UDC = Convert.ToDouble(reply[0]) * swnthrScaling.dblUDCGradient + swnthrScaling.dblUDCOffset;
      swnthrolvData.IDC = Convert.ToDouble(reply[1]) * swnthrScaling.dblIDCGradient + swnthrScaling.dblIDCOffset;
      swnthrolvData.UAC = Convert.ToDouble(reply[2]) * swnthrScaling.dblUACGradient + swnthrScaling.dblUACOffset;
      swnthrolvData.IAC = Convert.ToDouble(reply[3]) * swnthrScaling.dblIACGradient + swnthrScaling.dblIACOffset;
      swnthrolvData.Temp = num1;
      swnthrolvData.Solar = num2;
      swnthrolvData.PowerDay = Convert.ToDouble((int) reply[6] * 256 /*0x0100*/ + (int) reply[7]);
      if (Series == 2)
        swnthrolvData.PowerDay *= 10.0;
      swnthrolvData.Overflow = this.decodeOverflow(reply[11]);
      swnthrolvData.PowerSUM = Convert.ToDouble((int) reply[8] * 256 /*0x0100*/ + (int) reply[9]) + swnthrolvData.Overflow;
      swnthrolvData.PowerSUM *= 1000.0;
      swnthrolvData.Status = Convert.ToInt32(reply[10]);
      swnthrolvData.UIStatus = this.getUIStatus(swnthrolvData.Status, Series);
      swnthrolvData.Date = Command.CommandEnded;
      hrRes.ArrayResult.Add((object) swnthrolvData);
    }
    return flag;
  }

  private bool decodeDayVal(
    SWNTCommand Command,
    DateTime LastDate,
    PROTOCOL Protocol,
    int Series,
    SWNTHRResultData hrRes)
  {
    bool flag = true;
    byte[] reply = Command.GetReply();
    DateTime dateTime = new DateTime(LastDate.Year, LastDate.Month, LastDate.Day, 0, 0, 0);
    for (int index1 = 0; index1 < 3; ++index1)
    {
      SWNTHRDayVal swnthrDayVal = new SWNTHRDayVal(Command);
      int index2 = 4 * index1;
      int num1 = (int) reply[index2];
      int num2 = (int) reply[index2 + 1];
      int num3 = 0;
      int num4 = 0;
      int year = dateTime.Year;
      if (num1 > 0 && num2 > 0)
      {
        if (new DateTime(year, num1, num2, num3, num4, 0).CompareTo((object) dateTime) > 0)
          --year;
        swnthrDayVal.Date = new DateTime(year, num1, num2, num3, num4, 0);
        swnthrDayVal.Power = Convert.ToDouble((int) reply[index2 + 2] * 256 /*0x0100*/ + (int) reply[index2 + 3]);
        if (Series == 2)
          swnthrDayVal.Power *= 10.0;
        hrRes.ArrayResult.Add((object) swnthrDayVal);
      }
      else
      {
        Trace.WriteLine("Ungültiges Datum");
        flag = false;
      }
    }
    return flag;
  }

  private bool decodeMonthVal(
    SWNTCommand Command,
    DateTime LastDate,
    PROTOCOL Protocol,
    SWNTHRResultData hrRes)
  {
    bool flag = true;
    int blocknummer = (int) Command.Blocknummer;
    byte[] reply = Command.GetReply();
    DateTime dateTime;
    if ((int) Command.Blocknummer % 3 == 1)
    {
      int num1 = (int) reply[0] + 2000;
      int num2 = (int) reply[1];
      int num3 = 1;
      int num4 = 0;
      int num5 = 0;
      if (num2 < 1 || num2 > 12)
        return false;
      dateTime = new DateTime(num1, num2, num3, num4, num5, 0);
    }
    else
    {
      if (!DateTime.op_Inequality(LastDate, DateTime.MinValue))
        return false;
      dateTime = LastDate.AddMonths(-1);
    }
    int num6;
    int num7;
    if ((int) Command.Blocknummer % 3 == 0)
    {
      num6 = 0;
      num7 = 3;
    }
    else
    {
      num6 = 2;
      num7 = 11;
    }
    for (int index = num6; index < num7; index += 2)
    {
      SWNTHRMonthVal swnthrMonthVal = new SWNTHRMonthVal(Command);
      if (reply[index] != byte.MaxValue || reply[index + 1] != byte.MaxValue)
      {
        swnthrMonthVal.Date = dateTime;
        swnthrMonthVal.Power = Convert.ToDouble((int) reply[index] * 256 /*0x0100*/ + (int) reply[index + 1]) * 100.0;
        hrRes.ArrayResult.Add((object) swnthrMonthVal);
      }
      dateTime = dateTime.AddMonths(-1);
    }
    if ((int) Command.Blocknummer % 3 == 0)
    {
      SWNTHRSumOfMonths swnthrSumOfMonths = new SWNTHRSumOfMonths(Command);
      swnthrSumOfMonths.Date = DateTime.Now;
      swnthrSumOfMonths.Power = Convert.ToDouble((int) reply[10] * 256 /*0x0100*/ + (int) reply[11]) + this.decodeOverflow(reply[9]);
      swnthrSumOfMonths.Power *= 1000.0;
      hrRes.ArrayResult.Add((object) swnthrSumOfMonths);
    }
    return flag;
  }

  private bool decodeMinuteVal(
    SWNTCommand Command,
    DateTime LastDate,
    PROTOCOL Protocol,
    int Series,
    SWNTHRResultData hrRes)
  {
    bool flag = true;
    byte[] reply = Command.GetReply();
    try
    {
      int num1 = (int) reply[0];
      int num2 = (int) reply[1];
      int num3 = (int) reply[2];
      if (num1 != 0)
      {
        DateTime dateTime1 = LastDate;
        DateTime dateTime2 = new DateTime(dateTime1.Year, dateTime1.Month, num1, num2, num3, 0);
        if (DateTime.op_GreaterThan(dateTime2, dateTime1))
          dateTime2 = dateTime2.AddMonths(-1);
        int year = dateTime2.Year;
        int month = dateTime2.Month;
        SWNTHRScaling swnthrScaling = new SWNTHRScaling(this.m_dsScaling, Series);
        SWNTHRMinuteVal swnthrMinuteVal = new SWNTHRMinuteVal(Command);
        swnthrMinuteVal.Date = new DateTime(year, month, num1, num2, num3, 0);
        if (Series != 2 || Command.Command != SWCommand.COMMANDS.GET_MINS_B1 && Command.Command != SWCommand.COMMANDS.GET_MINS_B2)
        {
          swnthrMinuteVal.UDC = Convert.ToDouble(reply[3]) * swnthrScaling.dblUDCGradient + swnthrScaling.dblUDCOffset;
          swnthrMinuteVal.IDC = Convert.ToDouble(reply[4]) * swnthrScaling.dblIDCGradient + swnthrScaling.dblIDCOffset;
          swnthrMinuteVal.UAC = Convert.ToDouble(reply[5]) * swnthrScaling.dblUACGradient + swnthrScaling.dblUACOffset;
          swnthrMinuteVal.IAC = Convert.ToDouble(reply[6]) * swnthrScaling.dblIACGradient + swnthrScaling.dblIACOffset;
        }
        if (reply[7] == (byte) 0 && reply[8] == (byte) 0 || reply[7] == (byte) 1 && reply[8] == (byte) 1)
        {
          swnthrMinuteVal.Temp = double.NaN;
          swnthrMinuteVal.Solar = double.NaN;
        }
        else
        {
          swnthrMinuteVal.Temp = Convert.ToDouble(reply[7]) * swnthrScaling.dblTEMPGradient + swnthrScaling.dblTEMPOffset;
          swnthrMinuteVal.Solar = Convert.ToDouble(reply[8]) * swnthrScaling.dblSOLARGradient + swnthrScaling.dblSOLAROffset;
        }
        swnthrMinuteVal.Power = Convert.ToDouble((int) reply[9] * 256 /*0x0100*/ + (int) reply[10]);
        hrRes.ArrayResult.Add((object) swnthrMinuteVal);
      }
      else
        flag = false;
    }
    catch (Exception ex)
    {
      Trace.WriteLine(ex.Message);
      flag = false;
    }
    return flag;
  }

  public SWNTHRTime DecodeTime(SWNTCommand Command)
  {
    SWNTHRTime swnthrTime = (SWNTHRTime) null;
    try
    {
      if (Command.Command == SWCommand.COMMANDS.GET_DATETIME)
      {
        swnthrTime = new SWNTHRTime(Command);
        byte[] reply = Command.GetReply();
        swnthrTime.Date = new DateTime((int) reply[0] + 2000, (int) reply[1], (int) reply[2], (int) reply[3], (int) reply[4], 0);
      }
    }
    catch (Exception ex)
    {
      swnthrTime = (SWNTHRTime) null;
    }
    return swnthrTime;
  }

  private bool decodeTime(SWNTCommand Command, SWNTHRResultData hrRes)
  {
    bool flag = true;
    SWNTHRTime swnthrTime = this.DecodeTime(Command);
    if (swnthrTime != null)
      hrRes.ArrayResult.Add((object) swnthrTime);
    else
      flag = false;
    return flag;
  }

  public SWNTHRFWProtVer DecodeFWProtVer(SWNTCommand Command)
  {
    SWNTHRFWProtVer swnthrfwProtVer = (SWNTHRFWProtVer) null;
    try
    {
      if (Command.Command == SWCommand.COMMANDS.GET_FW_PROT_VER)
      {
        swnthrfwProtVer = new SWNTHRFWProtVer(Command);
        byte[] reply = Command.GetReply();
        try
        {
          int int32 = Convert.ToInt32(SWUtil.BytesToString(reply, 0, 1));
          swnthrfwProtVer.Protocol = (PROTOCOL) int32;
        }
        catch (Exception ex)
        {
          swnthrfwProtVer.Protocol = PROTOCOL.V10;
        }
        swnthrfwProtVer.FWVersion = SWUtil.BytesToString(reply, 2, 11).Replace('\r', ' ').Trim();
      }
    }
    catch (Exception ex)
    {
      swnthrfwProtVer = new SWNTHRFWProtVer(Command);
    }
    return swnthrfwProtVer;
  }

  private bool decodeFWProtVer(SWNTCommand Command, SWNTHRResultData hrRes)
  {
    bool flag = true;
    SWNTHRFWProtVer swnthrfwProtVer = this.DecodeFWProtVer(Command);
    if (swnthrfwProtVer != null)
      hrRes.ArrayResult.Add((object) swnthrfwProtVer);
    else
      flag = false;
    return flag;
  }

  private bool decodeDisplayLang(SWNTCommand Command, SWNTHRResultData hrRes)
  {
    bool flag = true;
    byte[] reply = Command.GetReply();
    hrRes.ArrayResult.Add((object) new SWNTHRDisplayLang(Command)
    {
      Lang = (DISPLAY_LANG) reply[0]
    });
    return flag;
  }

  private bool decodeInverterParameter(SWNTCommand Command, SWNTHRResultData hrRes)
  {
    bool flag = true;
    byte[] reply = Command.GetReply();
    SWNTHRParameterOffset swnthrParameterOffset = new SWNTHRParameterOffset(Command);
    SWNTHRParameterGradient parameterGradient = new SWNTHRParameterGradient(Command);
    switch (Command.Blocknummer)
    {
      case 1:
      case 2:
      case 3:
        swnthrParameterOffset.UDCOffset = Convert.ToDouble((int) reply[0] * 256 /*0x0100*/ + (int) reply[1]);
        swnthrParameterOffset.IDCOffset = Convert.ToDouble((int) reply[2] * 256 /*0x0100*/ + (int) reply[3]);
        swnthrParameterOffset.UACOffset = Convert.ToDouble((int) reply[4] * 256 /*0x0100*/ + (int) reply[5]);
        swnthrParameterOffset.InverterOffset = Convert.ToDouble((int) reply[6] * 256 /*0x0100*/ + (int) reply[7]);
        hrRes.ArrayResult.Add((object) swnthrParameterOffset);
        break;
      case 4:
      case 5:
      case 6:
        parameterGradient.UDCGradient = Convert.ToDouble((int) reply[0] * 65536 /*0x010000*/ + (int) reply[1] * 256 /*0x0100*/ + (int) reply[2]);
        parameterGradient.IDCGradient = Convert.ToDouble((int) reply[3] * 65536 /*0x010000*/ + (int) reply[4] * 256 /*0x0100*/ + (int) reply[5]);
        hrRes.ArrayResult.Add((object) parameterGradient);
        break;
    }
    return flag;
  }

  private SWNTHRResultData decodeNTResult(
    SWNTCommand Command,
    SWNTInverter Inverter,
    SWNTHRResultData hrRes)
  {
    if (Command.GetStatus() == SWCommand.STATUS.COMPLETE)
    {
      switch (Command.Command)
      {
        case SWCommand.COMMANDS.GET_ERR:
          this.decodeGetErr(Command, Inverter.Protocol, Inverter.SeriesID, hrRes);
          break;
        case SWCommand.COMMANDS.GET_OLV:
          this.decodeOLV(Command, Inverter.Protocol, Inverter.SeriesID, hrRes);
          break;
        case SWCommand.COMMANDS.GET_DATETIME:
          this.decodeTime(Command, hrRes);
          break;
        case SWCommand.COMMANDS.GET_FW_PROT_VER:
          this.decodeFWProtVer(Command, hrRes);
          break;
        case SWCommand.COMMANDS.GET_PARAMETER:
          this.decodeInverterParameter(Command, hrRes);
          break;
        case SWCommand.COMMANDS.GET_DISPLAY_LANG:
          this.decodeDisplayLang(Command, hrRes);
          break;
        default:
          throw new Exception("not implemented");
      }
    }
    return hrRes;
  }

  public SWNTHRResultData DecodeNTResult(SWNTCommand Command, SWNTInverter Inverter)
  {
    SWNTHRResultData hrRes = new SWNTHRResultData();
    return this.decodeNTResult(Command, Inverter, hrRes);
  }

  public bool DecodeSerNr(ArrayList CommandList, out string SerNr)
  {
    bool flag = false;
    SWNTHRResultData swnthrResultData = this.DecodeNTCommandListResult(CommandList, (SWNTInverter) null);
    SerNr = "";
    if (swnthrResultData.Count > 0 && swnthrResultData[0] is SWNTHRSerNr swnthrSerNr)
    {
      SerNr = swnthrSerNr.SerNr;
      flag = true;
    }
    return flag;
  }

  public SWNTHRResultData DecodeNTCommandListResult(ArrayList CommandList, SWNTInverter Inverter)
  {
    SWNTHRResultData hrRes = new SWNTHRResultData();
    if (CommandList.Count > 0)
    {
      SWNTCommand command1 = (SWNTCommand) CommandList[0];
      if (command1.GetStatus() == SWCommand.STATUS.COMPLETE)
      {
        switch (command1.Command)
        {
          case SWCommand.COMMANDS.GET_DAY_VAL:
            DateTime LastDate1 = SWUtil.DayEnd(DateTime.Now);
            if (this.decodeDayVal(command1, LastDate1, Inverter.Protocol, Inverter.SeriesID, hrRes))
            {
              if (hrRes[hrRes.Count - 1] is SWNTHRDayVal swnthrDayVal1)
                LastDate1 = swnthrDayVal1.Date;
              for (int index = 1; index < CommandList.Count; ++index)
              {
                SWNTCommand command2 = (SWNTCommand) CommandList[index];
                if (command2.GetStatus() == SWCommand.STATUS.COMPLETE && this.decodeDayVal(command2, LastDate1, Inverter.Protocol, Inverter.SeriesID, hrRes) && hrRes[hrRes.Count - 1] is SWNTHRDayVal swnthrDayVal2)
                  LastDate1 = swnthrDayVal2.Date;
              }
              break;
            }
            break;
          case SWCommand.COMMANDS.GET_MONTH_SUM:
            DateTime LastDate2 = DateTime.MinValue;
            if (this.decodeMonthVal(command1, LastDate2, Inverter.Protocol, hrRes))
            {
              if (hrRes[hrRes.Count - 1] is SWNTHRMonthVal swnthrMonthVal1)
                LastDate2 = swnthrMonthVal1.Date;
              for (int index = 1; index < CommandList.Count; ++index)
              {
                SWNTCommand command3 = (SWNTCommand) CommandList[index];
                if (command3.GetStatus() == SWCommand.STATUS.COMPLETE && this.decodeMonthVal(command3, LastDate2, Inverter.Protocol, hrRes) && hrRes[hrRes.Count - 1] is SWNTHRMonthVal swnthrMonthVal2)
                  LastDate2 = swnthrMonthVal2.Date;
              }
              break;
            }
            break;
          case SWCommand.COMMANDS.GET_MINS_B1:
          case SWCommand.COMMANDS.GET_MINS_B2:
          case SWCommand.COMMANDS.GET_MINS_10K_L1_B1:
          case SWCommand.COMMANDS.GET_MINS_10K_L1_B2:
          case SWCommand.COMMANDS.GET_MINS_10K_L2_B1:
          case SWCommand.COMMANDS.GET_MINS_10K_L2_B2:
          case SWCommand.COMMANDS.GET_MINS_10K_L3_B1:
          case SWCommand.COMMANDS.GET_MINS_10K_L3_B2:
            DateTime LastDate3 = SWUtil.DayEnd(DateTime.Now);
            if (this.decodeMinuteVal(command1, LastDate3, Inverter.Protocol, Inverter.SeriesID, hrRes))
            {
              if (hrRes[hrRes.Count - 1] is SWNTHRMinuteVal swnthrMinuteVal1)
                LastDate3 = swnthrMinuteVal1.Date;
              for (int index = 1; index < CommandList.Count; ++index)
              {
                SWNTCommand command4 = (SWNTCommand) CommandList[index];
                if (command4.GetStatus() == SWCommand.STATUS.COMPLETE && this.decodeMinuteVal(command4, LastDate3, Inverter.Protocol, Inverter.SeriesID, hrRes) && hrRes[hrRes.Count - 1] is SWNTHRMinuteVal swnthrMinuteVal2)
                  LastDate3 = swnthrMinuteVal2.Date;
              }
              break;
            }
            break;
          case SWCommand.COMMANDS.GET_SERNR:
            SWNTCommand command5 = (SWNTCommand) CommandList[0];
            if (command5.GetStatus() == SWCommand.STATUS.COMPLETE)
            {
              SWNTHRSerNr swnthrSerNr = new SWNTHRSerNr(command5);
              string str1 = SWUtil.BytesToString(command5.GetReply(), 0, 11).Replace('\r', ' ');
              swnthrSerNr.SerNr = str1.Replace(char.MinValue, ' ');
              SWNTCommand command6 = (SWNTCommand) CommandList[1];
              if (command6.GetStatus() == SWCommand.STATUS.COMPLETE)
              {
                string str2 = SWUtil.BytesToString(command6.GetReply(), 0, 11).Replace('\r', ' ');
                swnthrSerNr.SerNr += str2.Replace(char.MinValue, ' ');
                hrRes.ArrayResult.Add((object) swnthrSerNr);
                break;
              }
              break;
            }
            break;
          default:
            for (int index = 0; index < CommandList.Count; ++index)
            {
              SWNTCommand command7 = (SWNTCommand) CommandList[index];
              if (command7.GetStatus() == SWCommand.STATUS.COMPLETE)
                this.decodeNTResult(command7, Inverter, hrRes);
            }
            break;
        }
      }
    }
    return hrRes;
  }

  private string getUIStatus(int Status, int Series)
  {
    string uiStatus = "";
    try
    {
      DataRow[] dataRowArray = this.m_dsUIStatus.Tables[0].Select($"idStatus = {Status} AND idSeries = {Series} AND idUILanguage = {(int) this.UILanguage}");
      uiStatus = dataRowArray.Length <= 0 ? "?" : dataRowArray[0]["strStatus"].ToString();
    }
    catch (Exception ex)
    {
      Trace.WriteLine(ex.Message);
    }
    return uiStatus;
  }

  private bool decodeGetErr(
    SWNTCommand Command,
    PROTOCOL Protocol,
    int Series,
    SWNTHRResultData Result)
  {
    bool err = true;
    byte[] reply = Command.GetReply();
    DateTime now = DateTime.Now;
    for (int index1 = 0; index1 < 2; ++index1)
    {
      SWNTHRStatusData swnthrStatusData = new SWNTHRStatusData(Command);
      int index2 = 6 * index1;
      int num1 = (int) reply[index2];
      int num2 = (int) reply[index2 + 1];
      int num3 = (int) reply[index2 + 2];
      int num4 = (int) reply[index2 + 3];
      int Status = (int) reply[index2 + 4];
      int num5 = (int) reply[index2 + 5] + 2000;
      if (num1 < 1 || num2 < 1)
        err = false;
      if (err)
      {
        try
        {
          if (Protocol == PROTOCOL.V10)
          {
            num5 = DateTime.Now.Year;
            if (new DateTime(num5, num1, num2, num3, num4, 0).CompareTo((object) now) > 0)
              --num5;
          }
          swnthrStatusData.DateStart = new DateTime(num5, num1, num2, num3, num4, 0);
          swnthrStatusData.DateEnd = swnthrStatusData.DateStart;
          swnthrStatusData.Status = Status;
          swnthrStatusData.UIStatus = this.getUIStatus(Status, Series);
          Result.ArrayResult.Add((object) swnthrStatusData);
        }
        catch (Exception ex)
        {
          err = false;
          Trace.WriteLine(ex.Message);
        }
      }
    }
    return err;
  }

  public UI_LANG UILanguage
  {
    get => this.m_UILanguage;
    set
    {
      this.m_UILanguage = value;
      this.init();
    }
  }

  public void Dispose()
  {
    this.disposeDataSets();
    SWNTHRData.m_Instance = (SWNTHRData) null;
  }
}
