// Decompiled with JetBrains decompiler
// Type: de.sunways.communication.protocol.nt.SWNTCommand
// Assembly: SWCommLib, Version=1.0.2286.20848, Culture=neutral, PublicKeyToken=null
// MVID: 51F99318-6CFC-42F4-A802-BB6EE381FD69
// Assembly location: /NT_Monitor_2.0/SWCommLib.dll

using System;

#nullable disable
namespace de.sunways.communication.protocol.nt;

public class SWNTCommand : SWCommand
{
  private const int KNPOS = 2;
  private const int BNPOS = 3;
  private const int ADPOS = 1;
  private byte[] m_Reply = new byte[13];
  private int m_ReplyIdx = 0;
  private byte m_AD = 0;
  private byte m_BN = 0;
  private readonly byte[] CMD_BUF = new byte[5];

  public static bool IsProtocolSupported(SWCommand.COMMANDS cmd, PROTOCOL protocol)
  {
    int num1 = (int) protocol;
    int num2;
    switch (cmd)
    {
      case SWCommand.COMMANDS.GET_ERR:
      case SWCommand.COMMANDS.GET_OLV:
      case SWCommand.COMMANDS.GET_DAY_VAL:
      case SWCommand.COMMANDS.GET_MONTH_SUM:
      case SWCommand.COMMANDS.GET_MINS_B1:
      case SWCommand.COMMANDS.GET_DATETIME:
      case SWCommand.COMMANDS.GET_MINS_B2:
        num2 = 10;
        break;
      case SWCommand.COMMANDS.GET_SERNR:
      case SWCommand.COMMANDS.GET_FW_PROT_VER:
        num2 = 10;
        break;
      case SWCommand.COMMANDS.GET_MINS_10K_L1_B1:
      case SWCommand.COMMANDS.GET_MINS_10K_L1_B2:
      case SWCommand.COMMANDS.GET_MINS_10K_L2_B1:
      case SWCommand.COMMANDS.GET_MINS_10K_L2_B2:
      case SWCommand.COMMANDS.GET_MINS_10K_L3_B1:
      case SWCommand.COMMANDS.GET_MINS_10K_L3_B2:
      case SWCommand.COMMANDS.GET_PARAMETER:
      case SWCommand.COMMANDS.GET_DISPLAY_LANG:
        num2 = 12;
        break;
      case SWCommand.COMMANDS.SET_DATETIME_YEAR:
      case SWCommand.COMMANDS.SET_DATETIME_MONTH:
      case SWCommand.COMMANDS.SET_DATETIME_DAY:
      case SWCommand.COMMANDS.SET_DATETIME_HOUR:
      case SWCommand.COMMANDS.SET_DATETIME_MIN:
      case SWCommand.COMMANDS.SET_INVERTER_ADR_S1:
      case SWCommand.COMMANDS.SET_INVERTER_ADR_S2:
      case SWCommand.COMMANDS.SET_INVERTER_ADR_S3:
        num2 = 10;
        break;
      case SWCommand.COMMANDS.SET_DISPLAY_LANG_S1TO3:
        num2 = 12;
        break;
      case SWCommand.COMMANDS.SET_SERNR_POS_1:
      case SWCommand.COMMANDS.SET_SERNR_POS_2:
      case SWCommand.COMMANDS.SET_SERNR_POS_3:
      case SWCommand.COMMANDS.SET_SERNR_POS_4:
      case SWCommand.COMMANDS.SET_SERNR_POS_5:
      case SWCommand.COMMANDS.SET_SERNR_POS_6:
      case SWCommand.COMMANDS.SET_SERNR_POS_7:
      case SWCommand.COMMANDS.SET_SERNR_POS_8:
      case SWCommand.COMMANDS.SET_SERNR_POS_9:
      case SWCommand.COMMANDS.SET_SERNR_POS_10:
      case SWCommand.COMMANDS.SET_SERNR_POS_11:
      case SWCommand.COMMANDS.SET_SERNR_POS_12:
      case SWCommand.COMMANDS.SET_SERNR_POS_13:
      case SWCommand.COMMANDS.SET_SERNR_POS_14:
      case SWCommand.COMMANDS.SET_SERNR_POS_15:
      case SWCommand.COMMANDS.SET_SERNR_POS_16:
      case SWCommand.COMMANDS.SET_SERNR_POS_17:
      case SWCommand.COMMANDS.SET_SERNR_POS_18:
      case SWCommand.COMMANDS.SET_SERNR_POS_19:
      case SWCommand.COMMANDS.SET_SERNR_POS_20:
      case SWCommand.COMMANDS.SET_SERNR_POS_21:
      case SWCommand.COMMANDS.SET_SERNR_POS_22:
      case SWCommand.COMMANDS.SET_SERNR_POS_23:
      case SWCommand.COMMANDS.SET_SERNR_POS_24:
      case SWCommand.COMMANDS.SET_TOTAL_OUTPUT_MSB_S1:
      case SWCommand.COMMANDS.SET_TOTAL_OUTPUT_MID_S1:
      case SWCommand.COMMANDS.SET_TOTAL_OUTPUT_LSB_S1:
      case SWCommand.COMMANDS.SET_TOTAL_OUTPUT_MSB_S2:
      case SWCommand.COMMANDS.SET_TOTAL_OUTPUT_MID_S2:
      case SWCommand.COMMANDS.SET_TOTAL_OUTPUT_LSB_S2:
        num2 = 11;
        break;
      case SWCommand.COMMANDS.SET_PARAMETER_MSB:
      case SWCommand.COMMANDS.SET_PARAMETER_BYTE2:
      case SWCommand.COMMANDS.SET_PARAMETER_BYTE3:
      case SWCommand.COMMANDS.SET_PARAMETER_LSB:
      case SWCommand.COMMANDS.SET_PARAMETER_KEY:
        num2 = 12;
        break;
      default:
        num2 = 99;
        break;
    }
    return num2 <= num1;
  }

  public SWNTCommand(SWCommand.COMMANDS cmd, byte AD, byte BN, SWComm Owner)
    : base(cmd, Owner)
  {
    this.m_Command = new byte[this.CMD_BUF.Length];
    this.CMD_BUF.CopyTo((Array) this.m_Command, 0);
    this.m_Command[2] = (byte) cmd;
    this.m_Command[3] = BN;
    this.m_Command[1] = AD;
    if ((byte) cmd > (byte) 17)
      this.IsReplyNeeded = false;
    else
      this.IsReplyNeeded = true;
    this.m_ReplyIdx = 0;
    this.Adresse = AD;
    this.Blocknummer = BN;
    if (this.IsReplyNeeded)
      return;
    this.SetStatus(SWCommand.STATUS.COMPLETE);
  }

  private byte checkSum(byte[] Bytes)
  {
    int num1 = Bytes.Length - 1;
    long num2 = 0;
    for (int index = 0; index < num1; ++index)
      num2 += (long) Bytes[index];
    return (byte) ((ulong) num2 % 256UL /*0x0100*/);
  }

  public override byte[] GetCmdBytes() => this.m_Command;

  public override byte[] GetReply() => this.m_Reply;

  public override int GetReplyIdx() => this.m_ReplyIdx;

  public override void ResetReply()
  {
    lock (this)
    {
      this.m_ReplyIdx = 0;
      this.m_Reply.Initialize();
      base.ResetReply();
    }
  }

  public override void AddReply(byte Reply)
  {
    lock (this)
    {
      if (this.m_ReplyIdx == 0)
        this.m_FirstReply = DateTime.Now;
      if (this.m_ReplyIdx < this.m_Reply.Length)
      {
        this.m_Reply[this.m_ReplyIdx++] = Reply;
        this.SetStatus(SWCommand.STATUS.UNCOMPLETE);
      }
      else
        this.SetStatus(SWCommand.STATUS.ERROR);
      if (this.m_ReplyIdx != this.m_Reply.Length)
        return;
      if ((int) this.m_Reply[this.m_Reply.Length - 1] == (int) this.checkSum(this.m_Reply))
      {
        this.SetStatus(SWCommand.STATUS.COMPLETE);
      }
      else
      {
        this.m_ReplyIdx = 0;
        this.m_Reply.Initialize();
        this.SetStatus(SWCommand.STATUS.UNCOMPLETE);
      }
    }
  }

  public byte Adresse
  {
    get => this.m_AD;
    set
    {
      this.m_AD = value;
      this.UpdateCmd();
    }
  }

  private void UpdateCmd()
  {
    this.m_Command[1] = this.Adresse;
    this.m_Command[3] = this.Blocknummer;
    this.m_Command[4] = this.checkSum(this.m_Command);
    this.m_ReplyIdx = 0;
    this.m_Reply.Initialize();
  }

  public byte Blocknummer
  {
    get => this.m_BN;
    set
    {
      this.m_BN = value;
      this.UpdateCmd();
    }
  }
}
