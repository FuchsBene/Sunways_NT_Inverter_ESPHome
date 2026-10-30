// Decompiled with JetBrains decompiler
// Type: de.sunways.communication.protocol.nt.SWNTCommandList
// Assembly: SWCommLib, Version=1.0.2286.20848, Culture=neutral, PublicKeyToken=null
// MVID: 51F99318-6CFC-42F4-A802-BB6EE381FD69
// Assembly location: /NT_Monitor_2.0/SWCommLib.dll

using System;
using System.Collections;
using System.Text;

#nullable disable
namespace de.sunways.communication.protocol.nt;

public sealed class SWNTCommandList
{
  private SWNTCommandList()
  {
  }

  public static ArrayList GetForReadCommandArray(
    SWCommand.COMMANDS theCmd,
    int theCount,
    PROTOCOL theProtocol,
    byte theAddress,
    byte theBlocknumber,
    SWComm theOwner)
  {
    ArrayList readCommandArray = new ArrayList();
    if (SWNTCommand.IsProtocolSupported(theCmd, theProtocol))
    {
      for (int index = 0; index < theCount; ++index)
      {
        SWNTCommand swntCommand = new SWNTCommand(theCmd, theAddress, theBlocknumber, theOwner);
        readCommandArray.Add((object) swntCommand);
      }
    }
    return readCommandArray;
  }

  public static ArrayList GetForReadStatusData(
    PROTOCOL theProtocol,
    byte theAddress,
    SWComm theOwner)
  {
    ArrayList forReadStatusData = new ArrayList();
    byte num = 5;
    if (theProtocol == PROTOCOL.V12)
      num = (byte) 50;
    for (byte BN = 1; (int) BN <= (int) num; ++BN)
    {
      SWNTCommand swntCommand = new SWNTCommand(SWCommand.COMMANDS.GET_ERR, theAddress, BN, theOwner);
      swntCommand.IsReplyNeeded = true;
      forReadStatusData.Add((object) swntCommand);
    }
    return forReadStatusData;
  }

  public static ArrayList GetForReadDayValues(
    PROTOCOL theProtocol,
    byte theAddress,
    SWComm theOwner)
  {
    return SWNTCommandList.GetForReadDayValues(theProtocol, theAddress, byte.MaxValue, theOwner);
  }

  public static ArrayList GetForReadDayValues(
    PROTOCOL theProtocol,
    byte theAddress,
    byte theSubInverter,
    SWComm theOwner)
  {
    ArrayList forReadDayValues = new ArrayList();
    byte num1 = 1;
    byte num2 = 14;
    if (theProtocol == PROTOCOL.V12)
    {
      switch (theSubInverter)
      {
        case 1:
          num1 = (byte) 15;
          num2 = (byte) 28;
          break;
        case 2:
          num1 = (byte) 29;
          num2 = (byte) 42;
          break;
        case 3:
          num1 = (byte) 43;
          num2 = (byte) 56;
          break;
        case byte.MaxValue:
          break;
        default:
          throw new ArgumentException("bad subinverter number", nameof (theSubInverter));
      }
    }
    else
    {
      num1 = (byte) 1;
      num2 = (byte) 14;
    }
    for (byte BN = num1; (int) BN <= (int) num2; ++BN)
    {
      SWNTCommand swntCommand = new SWNTCommand(SWCommand.COMMANDS.GET_DAY_VAL, theAddress, BN, theOwner);
      swntCommand.IsReplyNeeded = true;
      forReadDayValues.Add((object) swntCommand);
    }
    return forReadDayValues;
  }

  public static ArrayList GetForReadMonthValues(
    PROTOCOL theProtocol,
    byte theAddress,
    SWComm theOwner)
  {
    return SWNTCommandList.GetForReadMonthValues(theProtocol, theAddress, byte.MaxValue, theOwner);
  }

  public static ArrayList GetForReadMonthValues(
    PROTOCOL theProtocol,
    byte theAddress,
    byte theSubInverter,
    SWComm theOwner)
  {
    ArrayList forReadMonthValues = new ArrayList();
    byte num1 = 1;
    byte num2 = 3;
    if (theProtocol == PROTOCOL.V12)
    {
      switch (theSubInverter)
      {
        case 1:
          num1 = (byte) 4;
          num2 = (byte) 6;
          break;
        case 2:
          num1 = (byte) 7;
          num2 = (byte) 9;
          break;
        case 3:
          num1 = (byte) 10;
          num2 = (byte) 12;
          break;
        case byte.MaxValue:
          break;
        default:
          throw new ArgumentException("bad subinverter number", nameof (theSubInverter));
      }
    }
    else
    {
      num1 = (byte) 1;
      num2 = (byte) 3;
    }
    for (byte BN = num1; (int) BN <= (int) num2; ++BN)
    {
      SWNTCommand swntCommand = new SWNTCommand(SWCommand.COMMANDS.GET_MONTH_SUM, theAddress, BN, theOwner);
      swntCommand.IsReplyNeeded = true;
      forReadMonthValues.Add((object) swntCommand);
    }
    return forReadMonthValues;
  }

  public static ArrayList GetForReadAllFifteenMinutesValues(
    PROTOCOL theProtocol,
    SERIES series,
    byte theAddress,
    SWComm theOwner)
  {
    return SWNTCommandList.GetForReadAllFifteenMinutesValues(theProtocol, series, theAddress, byte.MaxValue, theOwner);
  }

  public static ArrayList GetForReadAllFifteenMinutesValues(
    PROTOCOL theProtocol,
    SERIES series,
    byte theAddress,
    byte theSubInverter,
    SWComm theOwner)
  {
    return SWNTCommandList.GetForReadTopFifteenMinutesValues(510, theProtocol, series, theAddress, theSubInverter, theOwner);
  }

  public static ArrayList GetForReadTopFifteenMinutesValues(
    int theTop,
    PROTOCOL theProtocol,
    SERIES series,
    byte theAddress,
    SWComm theOwner)
  {
    return SWNTCommandList.GetForReadTopFifteenMinutesValues(theTop, theProtocol, series, theAddress, byte.MaxValue, theOwner);
  }

  public static ArrayList GetForReadTopFifteenMinutesValues(
    int theTop,
    PROTOCOL theProtocol,
    SERIES series,
    byte theAddress,
    byte theSubInverter,
    SWComm theOwner)
  {
    ArrayList fifteenMinutesValues = new ArrayList();
    int num1 = theTop > (int) byte.MaxValue ? (int) byte.MaxValue : theTop;
    int num2 = theTop - num1;
    switch (series)
    {
      case SERIES.NT1800:
      case SERIES.NT750V:
        num1 = num1 > 120 ? 120 : num1;
        num2 = 0;
        break;
    }
    if (theTop < 1 || theTop > 510)
      return fifteenMinutesValues;
    SWCommand.COMMANDS cmd1 = SWCommand.COMMANDS.GET_MINS_B1;
    SWCommand.COMMANDS cmd2 = SWCommand.COMMANDS.GET_MINS_B2;
    if (theProtocol == PROTOCOL.V12)
    {
      switch (theSubInverter)
      {
        case 1:
          cmd1 = SWCommand.COMMANDS.GET_MINS_10K_L1_B1;
          cmd2 = SWCommand.COMMANDS.GET_MINS_10K_L1_B2;
          break;
        case 2:
          cmd1 = SWCommand.COMMANDS.GET_MINS_10K_L2_B1;
          cmd2 = SWCommand.COMMANDS.GET_MINS_10K_L2_B2;
          break;
        case 3:
          cmd1 = SWCommand.COMMANDS.GET_MINS_10K_L3_B1;
          cmd2 = SWCommand.COMMANDS.GET_MINS_10K_L3_B2;
          break;
        case byte.MaxValue:
          break;
        default:
          throw new ArgumentException("bad subinverter number", nameof (theSubInverter));
      }
    }
    else
    {
      cmd1 = SWCommand.COMMANDS.GET_MINS_B1;
      cmd2 = SWCommand.COMMANDS.GET_MINS_B2;
    }
    for (int BN = 1; BN <= num1; ++BN)
    {
      SWNTCommand swntCommand = new SWNTCommand(cmd1, theAddress, (byte) BN, theOwner);
      swntCommand.IsReplyNeeded = true;
      fifteenMinutesValues.Add((object) swntCommand);
    }
    if (num2 > 0)
    {
      for (int BN = 1; BN <= num2; ++BN)
      {
        SWNTCommand swntCommand = new SWNTCommand(cmd2, theAddress, (byte) BN, theOwner);
        swntCommand.IsReplyNeeded = true;
        fifteenMinutesValues.Add((object) swntCommand);
      }
    }
    return fifteenMinutesValues;
  }

  public static ArrayList GetForReadSerNr(PROTOCOL theProtocol, byte theAddress, SWComm theOwner)
  {
    ArrayList forReadSerNr = (ArrayList) null;
    if (theProtocol != PROTOCOL.V10)
    {
      forReadSerNr = new ArrayList();
      SWNTCommand swntCommand1 = new SWNTCommand(SWCommand.COMMANDS.GET_SERNR, theAddress, (byte) 1, theOwner);
      swntCommand1.IsReplyNeeded = true;
      forReadSerNr.Add((object) swntCommand1);
      SWNTCommand swntCommand2 = new SWNTCommand(SWCommand.COMMANDS.GET_SERNR, theAddress, (byte) 2, theOwner);
      swntCommand2.IsReplyNeeded = true;
      forReadSerNr.Add((object) swntCommand2);
    }
    return forReadSerNr;
  }

  public static ArrayList GetForWriteDateTime(
    PROTOCOL theProtocol,
    DateTime theDateTime,
    byte theAddress,
    SWComm theOwner)
  {
    ArrayList forWriteDateTime = (ArrayList) null;
    DateTime dateTime = theDateTime;
    if (dateTime.Year > 1999 && dateTime.Year < 2256)
    {
      int BN = dateTime.Year - 2000;
      forWriteDateTime = new ArrayList();
      forWriteDateTime.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_DATETIME_YEAR, theAddress, (byte) BN, theOwner));
      forWriteDateTime.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_DATETIME_MONTH, theAddress, (byte) dateTime.Month, theOwner));
      forWriteDateTime.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_DATETIME_DAY, theAddress, (byte) dateTime.Day, theOwner));
      forWriteDateTime.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_DATETIME_HOUR, theAddress, (byte) (dateTime.Hour + 1), theOwner));
      forWriteDateTime.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_DATETIME_MIN, theAddress, (byte) (dateTime.Minute + 1), theOwner));
      foreach (SWCommand swCommand in forWriteDateTime)
        swCommand.IsReplyNeeded = false;
    }
    return forWriteDateTime;
  }

  public static ArrayList GetForWriteDateTime(
    PROTOCOL theProtocol,
    DateTime theDateTime,
    byte theAddress)
  {
    return SWNTCommandList.GetForWriteDateTime(theProtocol, theDateTime, theAddress, (SWComm) null);
  }

  public static ArrayList GetForWriteInverterAdr(
    PROTOCOL theProtocol,
    byte theNewAddress,
    byte theOldAddress,
    SWComm theOwner)
  {
    ArrayList writeInverterAdr = (ArrayList) null;
    if ((int) theNewAddress != (int) theOldAddress)
    {
      writeInverterAdr = new ArrayList();
      if (theProtocol == PROTOCOL.V10)
      {
        SWNTCommand swntCommand = new SWNTCommand(SWCommand.COMMANDS.SET_INVERTER_ADR_S1, theOldAddress, theNewAddress, theOwner);
        swntCommand.IsReplyNeeded = false;
        writeInverterAdr.Add((object) swntCommand);
      }
      else
      {
        writeInverterAdr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_INVERTER_ADR_S1, theOldAddress, theNewAddress, theOwner));
        writeInverterAdr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_INVERTER_ADR_S2, theOldAddress, theNewAddress, theOwner));
        writeInverterAdr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_INVERTER_ADR_S3, theOldAddress, theNewAddress, theOwner));
        foreach (SWCommand swCommand in writeInverterAdr)
          swCommand.IsReplyNeeded = false;
      }
    }
    return writeInverterAdr;
  }

  public static ArrayList GetForWriteInverterAdr(
    PROTOCOL theProtocol,
    byte theNewAddress,
    byte theOldAddress)
  {
    return SWNTCommandList.GetForWriteInverterAdr(theProtocol, theNewAddress, theOldAddress, (SWComm) null);
  }

  public static ArrayList GetForWriteDisplayLang(
    PROTOCOL theProtocol,
    byte theAddress,
    DISPLAY_LANG theLang,
    SWComm theOwner)
  {
    ArrayList writeDisplayLang = (ArrayList) null;
    if (theProtocol == PROTOCOL.V12)
    {
      writeDisplayLang.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_DISPLAY_LANG_S1TO3, theAddress, (byte) theLang, theOwner));
      writeDisplayLang.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_DISPLAY_LANG_S1TO3, theAddress, (byte) theLang, theOwner));
      writeDisplayLang.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_DISPLAY_LANG_S1TO3, theAddress, (byte) theLang, theOwner));
      foreach (SWCommand swCommand in writeDisplayLang)
        swCommand.IsReplyNeeded = false;
    }
    return writeDisplayLang;
  }

  public static ArrayList GetForWriteDisplayLang(
    PROTOCOL theProtocol,
    byte theAddress,
    DISPLAY_LANG theLang)
  {
    return SWNTCommandList.GetForWriteDisplayLang(theProtocol, theAddress, theLang, (SWComm) null);
  }

  public static ArrayList GetForWriteTotalOutput(
    PROTOCOL theProtocol,
    byte theAddress,
    int theKWH,
    SWComm theOwner)
  {
    ArrayList writeTotalOutput = (ArrayList) null;
    if (theProtocol != PROTOCOL.V10)
    {
      byte BN1 = (byte) ((theKWH & 16711680 /*0xFF0000*/) >> 16 /*0x10*/);
      byte BN2 = (byte) ((theKWH & 65280) >> 8);
      byte BN3 = (byte) (theKWH & (int) byte.MaxValue);
      writeTotalOutput.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_TOTAL_OUTPUT_MSB_S1, theAddress, BN1, theOwner));
      writeTotalOutput.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_TOTAL_OUTPUT_MID_S1, theAddress, BN2, theOwner));
      writeTotalOutput.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_TOTAL_OUTPUT_LSB_S1, theAddress, BN3, theOwner));
      writeTotalOutput.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_TOTAL_OUTPUT_MSB_S2, theAddress, BN1, theOwner));
      writeTotalOutput.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_TOTAL_OUTPUT_MID_S2, theAddress, BN2, theOwner));
      writeTotalOutput.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_TOTAL_OUTPUT_LSB_S2, theAddress, BN3, theOwner));
      foreach (SWCommand swCommand in writeTotalOutput)
        swCommand.IsReplyNeeded = false;
    }
    return writeTotalOutput;
  }

  public static ArrayList GetForWriteTotalOutput(PROTOCOL theProtocol, byte theAddress, int theKWH)
  {
    return SWNTCommandList.GetForWriteTotalOutput(theProtocol, theAddress, theKWH, (SWComm) null);
  }

  public static ArrayList GetForWriteSerNr(
    PROTOCOL theProtocol,
    byte theAddress,
    string theSerNr,
    SWComm theOwner)
  {
    ArrayList forWriteSerNr = (ArrayList) null;
    if (theProtocol != PROTOCOL.V10)
    {
      byte[] numArray = new byte[24];
      ((Encoding) new ASCIIEncoding()).GetBytes(theSerNr, 0, theSerNr.Length - 1, numArray, 0);
      for (int length = theSerNr.Length; length < numArray.Length; ++length)
        numArray[length] = (byte) 13;
      forWriteSerNr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_SERNR_POS_1, theAddress, (byte) 0, theOwner));
      forWriteSerNr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_SERNR_POS_2, theAddress, (byte) 0, theOwner));
      forWriteSerNr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_SERNR_POS_3, theAddress, (byte) 0, theOwner));
      forWriteSerNr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_SERNR_POS_4, theAddress, (byte) 0, theOwner));
      forWriteSerNr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_SERNR_POS_5, theAddress, (byte) 0, theOwner));
      forWriteSerNr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_SERNR_POS_6, theAddress, (byte) 0, theOwner));
      forWriteSerNr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_SERNR_POS_7, theAddress, (byte) 0, theOwner));
      forWriteSerNr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_SERNR_POS_8, theAddress, (byte) 0, theOwner));
      forWriteSerNr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_SERNR_POS_9, theAddress, (byte) 0, theOwner));
      forWriteSerNr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_SERNR_POS_10, theAddress, (byte) 0, theOwner));
      forWriteSerNr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_SERNR_POS_11, theAddress, (byte) 0, theOwner));
      forWriteSerNr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_SERNR_POS_12, theAddress, (byte) 0, theOwner));
      forWriteSerNr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_SERNR_POS_13, theAddress, (byte) 0, theOwner));
      forWriteSerNr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_SERNR_POS_14, theAddress, (byte) 0, theOwner));
      forWriteSerNr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_SERNR_POS_15, theAddress, (byte) 0, theOwner));
      forWriteSerNr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_SERNR_POS_16, theAddress, (byte) 0, theOwner));
      forWriteSerNr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_SERNR_POS_17, theAddress, (byte) 0, theOwner));
      forWriteSerNr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_SERNR_POS_18, theAddress, (byte) 0, theOwner));
      forWriteSerNr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_SERNR_POS_19, theAddress, (byte) 0, theOwner));
      forWriteSerNr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_SERNR_POS_20, theAddress, (byte) 0, theOwner));
      forWriteSerNr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_SERNR_POS_21, theAddress, (byte) 0, theOwner));
      forWriteSerNr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_SERNR_POS_22, theAddress, (byte) 0, theOwner));
      forWriteSerNr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_SERNR_POS_23, theAddress, (byte) 0, theOwner));
      forWriteSerNr.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_SERNR_POS_24, theAddress, (byte) 0, theOwner));
      for (int index = 0; index < forWriteSerNr.Count; ++index)
      {
        SWNTCommand swntCommand = (SWNTCommand) forWriteSerNr[index];
        swntCommand.IsReplyNeeded = false;
        swntCommand.Blocknummer = numArray[index];
      }
    }
    return forWriteSerNr;
  }

  public static ArrayList GetForWriteSerNr(PROTOCOL theProtocol, byte theAddress, string theSerNr)
  {
    return SWNTCommandList.GetForWriteSerNr(theProtocol, theAddress, theSerNr, (SWComm) null);
  }

  public static ArrayList GetForWriteParameter(
    PROTOCOL theProtocol,
    byte theAddress,
    INVERTER_PARAMETER theParameter,
    int theValue,
    SWComm theOwner)
  {
    ArrayList forWriteParameter = (ArrayList) null;
    if (theProtocol != PROTOCOL.V10 && theProtocol != PROTOCOL.V11)
    {
      byte BN1 = (byte) (((long) theValue & 4278190080L /*0xFF000000*/) >> 24);
      byte BN2 = (byte) ((theValue & 16711680 /*0xFF0000*/) >> 16 /*0x10*/);
      byte BN3 = (byte) ((theValue & 65280) >> 8);
      byte BN4 = (byte) (theValue & (int) byte.MaxValue);
      forWriteParameter.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_PARAMETER_MSB, theAddress, BN1, theOwner));
      forWriteParameter.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_PARAMETER_BYTE2, theAddress, BN2, theOwner));
      forWriteParameter.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_PARAMETER_BYTE3, theAddress, BN3, theOwner));
      forWriteParameter.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_PARAMETER_LSB, theAddress, BN4, theOwner));
      forWriteParameter.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_PARAMETER_KEY, theAddress, (byte) theParameter, theOwner));
      forWriteParameter.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_PARAMETER_MSB, theAddress, BN1, theOwner));
      forWriteParameter.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_PARAMETER_BYTE2, theAddress, BN2, theOwner));
      forWriteParameter.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_PARAMETER_BYTE3, theAddress, BN3, theOwner));
      forWriteParameter.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_PARAMETER_LSB, theAddress, BN4, theOwner));
      forWriteParameter.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_PARAMETER_KEY, theAddress, (byte) theParameter, theOwner));
      forWriteParameter.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_PARAMETER_MSB, theAddress, BN1, theOwner));
      forWriteParameter.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_PARAMETER_BYTE2, theAddress, BN2, theOwner));
      forWriteParameter.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_PARAMETER_BYTE3, theAddress, BN3, theOwner));
      forWriteParameter.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_PARAMETER_LSB, theAddress, BN4, theOwner));
      forWriteParameter.Add((object) new SWNTCommand(SWCommand.COMMANDS.SET_PARAMETER_KEY, theAddress, (byte) theParameter, theOwner));
      foreach (SWCommand swCommand in forWriteParameter)
        swCommand.IsReplyNeeded = false;
    }
    return forWriteParameter;
  }

  public static ArrayList GetForWriteParameter(
    PROTOCOL theProtocol,
    byte theAddress,
    INVERTER_PARAMETER theParameter,
    int theValue)
  {
    return SWNTCommandList.GetForWriteParameter(theProtocol, theAddress, theParameter, theValue, (SWComm) null);
  }
}
