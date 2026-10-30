// Decompiled with JetBrains decompiler
// Type: de.sunways.communication.protocol.SWCommand
// Assembly: SWCommLib, Version=1.0.2286.20848, Culture=neutral, PublicKeyToken=null
// MVID: 51F99318-6CFC-42F4-A802-BB6EE381FD69
// Assembly location: /NT_Monitor_2.0/SWCommLib.dll

using System;

#nullable disable
namespace de.sunways.communication.protocol;

public abstract class SWCommand : IComparable
{
  public object UserData = (object) null;
  private SWComm m_Owner = (SWComm) null;
  public readonly Guid GUID = Guid.NewGuid();
  private DateTime m_CommandStarted = DateTime.MinValue;
  private DateTime m_CommandEnded = DateTime.MinValue;
  protected DateTime m_FirstReply = DateTime.MinValue;
  protected bool m_ReplyNeeded;
  protected SWCommand.COMMANDS m_Cmd;
  protected byte[] m_Command = (byte[]) null;
  private SWCommand.STATUS m_Status = SWCommand.STATUS.UNKNOWN;
  private int m_SendCount = 0;
  private int m_RepeatCount = 0;

  public SWCommand(SWCommand.COMMANDS cmd, SWComm Owner)
  {
    this.m_Cmd = cmd;
    this.m_Owner = Owner;
  }

  public abstract byte[] GetCmdBytes();

  public abstract byte[] GetReply();

  public abstract int GetReplyIdx();

  public virtual void ResetReply()
  {
    this.m_FirstReply = DateTime.MinValue;
    this.m_CommandStarted = DateTime.MinValue;
    this.m_CommandEnded = DateTime.MinValue;
  }

  public virtual SWCommand.STATUS GetStatus() => this.m_Status;

  public virtual void SetStatus(SWCommand.STATUS theStatus)
  {
    this.m_Status = theStatus;
    switch (this.m_Status)
    {
      case SWCommand.STATUS.UNKNOWN:
        break;
      case SWCommand.STATUS.UNCOMPLETE:
        if (!this.m_CommandStarted.Equals((object) DateTime.MinValue))
          break;
        this.m_CommandStarted = DateTime.Now;
        break;
      default:
        if (this.m_CommandStarted.Equals((object) DateTime.MinValue))
          this.m_CommandStarted = DateTime.Now;
        if (this.m_CommandEnded.Equals((object) DateTime.MinValue))
        {
          this.m_CommandEnded = DateTime.Now;
          break;
        }
        DateTime now = DateTime.Now;
        break;
    }
  }

  public virtual bool IsReplyNeeded
  {
    get => this.m_ReplyNeeded;
    set => this.m_ReplyNeeded = value;
  }

  public abstract void AddReply(byte Reply);

  public SWCommand.COMMANDS Command => this.m_Cmd;

  public DateTime CommandStarted => this.m_CommandStarted;

  public DateTime CommandEnded => this.m_CommandEnded;

  public DateTime FirstReply => this.m_FirstReply;

  public SWComm Owner => this.m_Owner;

  public int CompareTo(object obj)
  {
    int num = 0;
    if (obj.GetType().Equals(typeof (SWCommand)))
      num = ((SWCommand) obj).GUID.CompareTo((object) this.GUID);
    return num;
  }

  public int SendCount
  {
    get => this.m_SendCount;
    set => this.m_SendCount = value;
  }

  public int RepeatCount
  {
    get => this.m_RepeatCount;
    set => this.m_RepeatCount = value;
  }

  public TimeSpan CommandDuration
  {
    get => DateTime.op_Subtraction(this.CommandEnded, this.CommandStarted);
  }

  public enum COMMANDS
  {
    GET_ERR = 1,
    GET_OLV = 2,
    GET_DAY_VAL = 3,
    GET_MONTH_SUM = 4,
    GET_MINS_B1 = 5,
    GET_DATETIME = 6,
    GET_MINS_B2 = 7,
    GET_SERNR = 8,
    GET_FW_PROT_VER = 9,
    GET_MINS_10K_L1_B1 = 10, // 0x0000000A
    GET_MINS_10K_L1_B2 = 11, // 0x0000000B
    GET_MINS_10K_L2_B1 = 12, // 0x0000000C
    GET_MINS_10K_L2_B2 = 13, // 0x0000000D
    GET_MINS_10K_L3_B1 = 14, // 0x0000000E
    GET_MINS_10K_L3_B2 = 15, // 0x0000000F
    GET_PARAMETER = 16, // 0x00000010
    GET_DISPLAY_LANG = 17, // 0x00000011
    SET_DATETIME_YEAR = 50, // 0x00000032
    SET_DATETIME_MONTH = 51, // 0x00000033
    SET_DATETIME_DAY = 52, // 0x00000034
    SET_DATETIME_HOUR = 53, // 0x00000035
    SET_DATETIME_MIN = 54, // 0x00000036
    SET_INVERTER_ADR_S1 = 55, // 0x00000037
    SET_INVERTER_ADR_S2 = 56, // 0x00000038
    SET_INVERTER_ADR_S3 = 57, // 0x00000039
    SET_DISPLAY_LANG_S1TO3 = 58, // 0x0000003A
    SET_SERNR_POS_1 = 100, // 0x00000064
    SET_SERNR_POS_2 = 101, // 0x00000065
    SET_SERNR_POS_3 = 102, // 0x00000066
    SET_SERNR_POS_4 = 103, // 0x00000067
    SET_SERNR_POS_5 = 104, // 0x00000068
    SET_SERNR_POS_6 = 105, // 0x00000069
    SET_SERNR_POS_7 = 106, // 0x0000006A
    SET_SERNR_POS_8 = 107, // 0x0000006B
    SET_SERNR_POS_9 = 108, // 0x0000006C
    SET_SERNR_POS_10 = 109, // 0x0000006D
    SET_SERNR_POS_11 = 110, // 0x0000006E
    SET_SERNR_POS_12 = 111, // 0x0000006F
    SET_SERNR_POS_13 = 112, // 0x00000070
    SET_SERNR_POS_14 = 113, // 0x00000071
    SET_SERNR_POS_15 = 114, // 0x00000072
    SET_SERNR_POS_16 = 115, // 0x00000073
    SET_SERNR_POS_17 = 116, // 0x00000074
    SET_SERNR_POS_18 = 117, // 0x00000075
    SET_SERNR_POS_19 = 118, // 0x00000076
    SET_SERNR_POS_20 = 119, // 0x00000077
    SET_SERNR_POS_21 = 120, // 0x00000078
    SET_SERNR_POS_22 = 121, // 0x00000079
    SET_SERNR_POS_23 = 122, // 0x0000007A
    SET_SERNR_POS_24 = 123, // 0x0000007B
    SET_TOTAL_OUTPUT_MSB_S1 = 150, // 0x00000096
    SET_TOTAL_OUTPUT_MID_S1 = 151, // 0x00000097
    SET_TOTAL_OUTPUT_LSB_S1 = 152, // 0x00000098
    SET_TOTAL_OUTPUT_MSB_S2 = 153, // 0x00000099
    SET_TOTAL_OUTPUT_MID_S2 = 154, // 0x0000009A
    SET_TOTAL_OUTPUT_LSB_S2 = 155, // 0x0000009B
    SET_PARAMETER_MSB = 156, // 0x0000009C
    SET_PARAMETER_BYTE2 = 157, // 0x0000009D
    SET_PARAMETER_BYTE3 = 158, // 0x0000009E
    SET_PARAMETER_LSB = 159, // 0x0000009F
    SET_PARAMETER_KEY = 160, // 0x000000A0
    SPECIAL_CMD = 999, // 0x000003E7
  }

  public enum STATUS
  {
    UNKNOWN = 0,
    UNCOMPLETE = 1,
    COMPLETE = 2,
    TIMEOUT = 3,
    ERROR = 99, // 0x00000063
  }
}
