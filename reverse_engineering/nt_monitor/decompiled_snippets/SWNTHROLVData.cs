// Decompiled with JetBrains decompiler
// Type: de.sunways.ntmonitor.SWNTHROLVData
// Assembly: SWNTMonLib, Version=1.0.2286.20848, Culture=neutral, PublicKeyToken=null
// MVID: FA1BE6D6-9789-41BC-A3D3-657A8164DEC5
// Assembly location: /NT_Monitor_2.0/SWNTMonLib.dll

using de.sunways.communication.protocol.nt;
using System;

#nullable disable
namespace de.sunways.ntmonitor;

public class SWNTHROLVData : SWNTHRBaseResult
{
  public double UDC = double.NaN;
  public double IDC = double.NaN;
  public double UAC = double.NaN;
  public double IAC = double.NaN;
  public double Temp = double.NaN;
  public double Solar = double.NaN;
  public double PowerDay = double.NaN;
  public double PowerSUM = double.NaN;
  public int Status = -1;
  public string UIStatus = "";
  public int WRCode = -1;
  public double Overflow = double.NaN;
  public DateTime Date = DateTime.MinValue;

  public SWNTHROLVData(SWNTCommand command)
    : base(command)
  {
    this.ResultType = HR_RESULT_TYPE.ONLINE_VALUES;
  }
}
