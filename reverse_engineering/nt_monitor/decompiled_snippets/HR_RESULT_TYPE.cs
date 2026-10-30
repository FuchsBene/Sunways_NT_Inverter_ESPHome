// Decompiled with JetBrains decompiler
// Type: de.sunways.ntmonitor.HR_RESULT_TYPE
// Assembly: SWNTMonLib, Version=1.0.2286.20848, Culture=neutral, PublicKeyToken=null
// MVID: FA1BE6D6-9789-41BC-A3D3-657A8164DEC5
// Assembly location: /NT_Monitor_2.0/SWNTMonLib.dll

#nullable disable
namespace de.sunways.ntmonitor;

public enum HR_RESULT_TYPE
{
  STATUS_DATA = 1,
  ONLINE_VALUES = 2,
  DAY_VALUES = 3,
  MONTH_VALUES = 4,
  MINS_VALUES = 5,
  INVERTER_DATE_TIME = 6,
  SER_NR = 8,
  FW_PROT_VER = 9,
  DISPLAY_LANG = 17, // 0x00000011
  UNKNOWN = 99, // 0x00000063
  ONLINE_VALUES_SUM = 102, // 0x00000066
  MONTH_SUM = 104, // 0x00000068
  INVERTER_PARAMETER_GRADIENT = 116, // 0x00000074
  INVERTER_PARAMETER_OFFSET = 116, // 0x00000074
}
