// Decompiled with JetBrains decompiler
// Type: de.sunways.communication.protocol.SWComm
// Assembly: SWCommLib, Version=1.0.2286.20848, Culture=neutral, PublicKeyToken=null
// MVID: 51F99318-6CFC-42F4-A802-BB6EE381FD69
// Assembly location: /NT_Monitor_2.0/SWCommLib.dll

using JH.CommBase;
using System;
using System.Threading;

#nullable disable
namespace de.sunways.communication.protocol;

public class SWComm : IDisposable
{
  private const int DEFAULT_TIMEOUT = 20;
  private ManualResetEvent m_StatusChangedFlag = new ManualResetEvent(true);
  private ManualResetEvent m_SendFlag = new ManualResetEvent(true);
  protected SWComm.SWCommBase m_CommBase = new SWComm.SWCommBase();
  protected SWCommand m_SWCommand = (SWCommand) null;
  private bool m_ReadyForSend = true;
  protected bool m_ErrorSend = false;
  protected int m_DefaultTimeout = 20;
  protected int m_PortOpeningTime = 1000;
  private DateTime m_LastReply = DateTime.Now;
  protected SWTimerThread m_TimerThread = (SWTimerThread) null;
  private object m_TimeoutLock = new object();
  private object m_CommandLock = new object();
  private bool m_Closing = false;
  private bool m_Disposing = false;

  public event ReplyHandler Reply = null;

  public event RxTimeoutHandler Timeout = null;

  public event StatusChangedHandler StatusChanged = null;

  public event SendCompleteHandler SendComplete = null;

  public event CommExceptionHandler CommException = null;

  public void ClearError()
  {
    if (!this.m_ErrorSend)
      return;
    this.m_ErrorSend = false;
  }

  public JH.CommBase.CommBase.ModemStatus GetModemStatus()
  {
    JH.CommBase.CommBase.ModemStatus modemStatus = new JH.CommBase.CommBase.ModemStatus();
    try
    {
      modemStatus = this.m_CommBase.GetModemStatus();
    }
    catch (Exception ex)
    {
      this.shutDownAfterException();
    }
    return modemStatus;
  }

  public JH.CommBase.CommBase.QueueStatus GetQueueStatus()
  {
    JH.CommBase.CommBase.QueueStatus queueStatus = new JH.CommBase.CommBase.QueueStatus();
    try
    {
      queueStatus = this.m_CommBase.GetQueueStatus();
    }
    catch (Exception ex)
    {
    }
    return queueStatus;
  }

  protected virtual void EndSend(string info, bool doSendComplete)
  {
    SWCommand swCommand;
    lock (this.m_CommandLock)
    {
      swCommand = this.m_SWCommand;
      this.m_SWCommand = (SWCommand) new SWNTTrashCommand(this);
    }
    try
    {
      if (!doSendComplete || this.SendComplete == null)
        return;
      this.SendComplete((object) this, new SWSendCompleteEventArgs(swCommand, info));
    }
    catch (Exception ex)
    {
    }
    finally
    {
      lock (this.m_CommandLock)
      {
        this.m_ReadyForSend = true;
        this.m_SendFlag.Set();
      }
    }
  }

  protected virtual void EndSend(string info) => this.EndSend(info, true);

  protected virtual void SetError() => this.m_ErrorSend = true;

  protected virtual void StartSend()
  {
    lock (this.m_CommandLock)
    {
      this.m_SendFlag.Reset();
      this.m_ReadyForSend = false;
    }
  }

  protected virtual void OnTimer(object sender, SWTimerEventArgs e)
  {
    SWCommand theCommand = (SWCommand) null;
    lock (this.m_CommandLock)
      theCommand = this.m_SWCommand;
    switch (theCommand)
    {
      case null:
      case SWNTTrashCommand _:
        DateTime when = e.When;
        break;
      default:
        SWCommand.STATUS status = theCommand.GetStatus();
        if (status == SWCommand.STATUS.UNCOMPLETE)
          theCommand.SetStatus(SWCommand.STATUS.TIMEOUT);
        if (status != SWCommand.STATUS.UNCOMPLETE)
          break;
        this.m_LastReply = e.When;
        if (this.Timeout != null)
        {
          bool flag = false;
          try
          {
            SWRxTimeoutEventArgs e1 = new SWRxTimeoutEventArgs(theCommand);
            this.Timeout((object) this, e1);
            if (e1.Cancel)
            {
              this.ClearError();
              this.EndSend("Kommando storniert", false);
            }
            else
            {
              this.SetError();
              this.EndSend("Timout occured");
            }
            flag = true;
            break;
          }
          catch (Exception ex)
          {
            break;
          }
          finally
          {
            if (!flag)
              this.EndSend("OnTimer Fehler erzwingt EndSend");
          }
        }
        else
        {
          this.SetError();
          this.EndSend("Timout occured");
          break;
        }
    }
  }

  public virtual bool WaitOnStatusChange(int Timeout)
  {
    this.m_StatusChangedFlag.Reset();
    return ((WaitHandle) this.m_StatusChangedFlag).WaitOne(Timeout, false);
  }

  protected virtual void OnStatusChange(object sender, SWStatusChangeEventArgs e)
  {
    this.m_StatusChangedFlag.Set();
    if (this.StatusChanged == null)
      return;
    this.StatusChanged(sender, e);
  }

  protected virtual void OnRxByte(object sender, SWComm.SWRxByteEventArgs e)
  {
    byte Reply = e.Byte;
    try
    {
      SWCommand swCommand = (SWCommand) null;
      lock (this.m_CommandLock)
        swCommand = this.m_SWCommand;
      if (swCommand != null && swCommand.GetStatus() == SWCommand.STATUS.UNCOMPLETE)
      {
        swCommand.GetReplyIdx();
        this.m_TimerThread.Trigger();
        swCommand.AddReply(Reply);
        switch (swCommand.GetStatus())
        {
          case SWCommand.STATUS.COMPLETE:
          case SWCommand.STATUS.ERROR:
            this.DoReply();
            break;
        }
      }
      else
        this.SetError();
    }
    catch (Exception ex)
    {
      this.SetError();
    }
  }

  protected virtual void OnRxException(object sender, SWComm.SWRxExceptionEventArgs e)
  {
    try
    {
      SWCommand swCommand = (SWCommand) null;
      lock (this.m_CommandLock)
        swCommand = this.m_SWCommand;
      swCommand?.SetStatus(SWCommand.STATUS.ERROR);
      this.SetError();
      if (this.Online)
        this.Close();
      this.DoCommException(sender, e.Reason);
      this.DoReply();
    }
    catch (Exception ex)
    {
      this.EndSend($"{e.Reason.Message}, {ex.Message}");
    }
  }

  protected virtual void DoCommException(object origin, Exception e)
  {
    if (this.CommException == null)
      return;
    try
    {
      this.CommException((object) this, new SWCommExceptionEventArgs(origin, e));
    }
    catch (Exception ex)
    {
    }
  }

  protected virtual void DoReply()
  {
    SWCommand theCommand = (SWCommand) null;
    lock (this.m_CommandLock)
      theCommand = this.m_SWCommand;
    try
    {
      this.m_TimerThread.StopTimer();
      if (this.Reply == null)
        return;
      this.Reply((object) this, new SWReplyEventArgs(theCommand));
    }
    catch (Exception ex)
    {
    }
    finally
    {
      if (this.Reply != null)
        this.EndSend("");
      else
        this.EndSend("No current command");
      this.m_LastReply = DateTime.Now;
    }
  }

  public virtual bool IsReadyForSend() => this.m_ReadyForSend;

  public virtual bool WaitOnEndSend(int Timeout)
  {
    return ((WaitHandle) this.m_SendFlag).WaitOne(Timeout, false);
  }

  public virtual bool WaitOnEndSend() => ((WaitHandle) this.m_SendFlag).WaitOne();

  public virtual bool WaitTXFree(int iTimeout)
  {
    bool flag = false;
    double num = Convert.ToDouble(iTimeout);
    DateTime now = DateTime.Now;
    if (iTimeout <= 2)
    {
      TimeSpan timeSpan1 = new TimeSpan(0, 0, 0, 0, iTimeout);
    }
    else
    {
      TimeSpan timeSpan2 = new TimeSpan(0, 0, 0, 0, 2);
    }
    while (this.m_CommBase.IsCongested())
    {
      if (DateTime.op_Subtraction(DateTime.Now, now).TotalMilliseconds >= num)
        goto label_6;
    }
    flag = true;
label_6:
    return flag;
  }

  private void shutDownAfterException()
  {
    try
    {
      if (this.m_CommBase == null)
        return;
      lock (this.m_CommBase)
      {
        if (!this.m_CommBase.Online)
          this.m_CommBase.Open();
        if (!this.m_CommBase.Online)
          return;
        this.Close();
      }
    }
    catch (Exception ex)
    {
    }
  }

  public virtual bool Send(SWCommand Command, int iTimeout)
  {
    bool flag1 = false;
    lock (this.m_CommandLock)
      this.m_SWCommand = Command;
    bool flag2;
    if (!this.m_Disposing && this.Online && this.IsReadyForSend() && this.WaitOnEndSend(iTimeout) && !this.IsError && this.WaitTXFree(iTimeout))
    {
      this.StartSend();
      flag2 = true;
    }
    else
      flag2 = false;
    if (flag2)
    {
      flag1 = false;
      TimeSpan timeSpan1 = DateTime.op_Subtraction(DateTime.Now, this.m_LastReply);
      TimeSpan timeSpan2 = new TimeSpan(0, 0, 0, 0, 1);
      if (timeSpan1.TotalMilliseconds < timeSpan2.TotalMilliseconds)
        Thread.Sleep(timeSpan2);
      try
      {
        this.m_TimerThread.StopTimer();
        Command.ResetReply();
        Command.SetStatus(SWCommand.STATUS.UNCOMPLETE);
        ++Command.SendCount;
        byte[] cmdBytes = Command.GetCmdBytes();
        if (cmdBytes == null)
          throw new ArgumentNullException("bytes", "SWComm.Send()");
        lock (this)
          this.m_CommBase.Send(cmdBytes);
        this.WaitTXFree(Convert.ToInt32(1.0 / (double) (this.CommportBaudRate / 10) * (double) Command.GetCmdBytes().Length * 1000.0));
        if (Command.IsReplyNeeded)
        {
          this.m_TimerThread.StartTimer(iTimeout);
        }
        else
        {
          this.m_TimerThread.StopTimer();
          Command.SetStatus(SWCommand.STATUS.COMPLETE);
          this.EndSend("");
        }
        flag2 = true;
      }
      catch (Exception ex1)
      {
        this.m_TimerThread.StopTimer();
        Command.SetStatus(SWCommand.STATUS.ERROR);
        this.SetError();
        try
        {
          this.shutDownAfterException();
          this.EndSend(ex1.Message);
        }
        catch (Exception ex2)
        {
        }
        flag2 = false;
      }
    }
    return flag2;
  }

  public static JH.CommBase.CommBase.PortStatus IsPortAvailable(string strPort)
  {
    JH.CommBase.CommBase.PortStatus portStatus = JH.CommBase.CommBase.PortStatus.unavailable;
    try
    {
      portStatus = new SWComm.SWCommBase().IsPortAvailable(strPort);
    }
    catch (Exception ex)
    {
    }
    return portStatus;
  }

  public SWComm(string strPort)
  {
    this.m_CommBase.Settings.port = strPort;
    this.m_SWCommand = (SWCommand) new SWNTTrashCommand(this);
    if (this.m_CommBase == null)
      this.m_CommBase = new SWComm.SWCommBase();
    this.m_TimerThread = new SWTimerThread();
  }

  protected virtual void setHandshake(JH.CommBase.CommBase.Handshake Hs)
  {
    JH.CommBase.CommBase.CommBaseSettings settings = this.m_CommBase.Settings;
    switch (Hs)
    {
      case JH.CommBase.CommBase.Handshake.none:
        settings.txFlowCTS = false;
        settings.txFlowDSR = false;
        settings.txFlowX = false;
        settings.rxFlowX = false;
        settings.useRTS = JH.CommBase.CommBase.HSOutput.online;
        settings.useDTR = JH.CommBase.CommBase.HSOutput.online;
        settings.txWhenRxXoff = true;
        settings.rxGateDSR = false;
        break;
      case JH.CommBase.CommBase.Handshake.XonXoff:
        settings.txFlowCTS = false;
        settings.txFlowDSR = false;
        settings.txFlowX = true;
        settings.rxFlowX = true;
        settings.useRTS = JH.CommBase.CommBase.HSOutput.online;
        settings.useDTR = JH.CommBase.CommBase.HSOutput.online;
        settings.txWhenRxXoff = true;
        settings.rxGateDSR = false;
        settings.XonChar = JH.CommBase.CommBase.ASCII.DC1;
        settings.XoffChar = JH.CommBase.CommBase.ASCII.DC3;
        break;
      case JH.CommBase.CommBase.Handshake.CtsRts:
        settings.txFlowCTS = true;
        settings.txFlowDSR = false;
        settings.txFlowX = false;
        settings.rxFlowX = false;
        settings.useRTS = JH.CommBase.CommBase.HSOutput.handshake;
        settings.useDTR = JH.CommBase.CommBase.HSOutput.online;
        settings.txWhenRxXoff = true;
        settings.rxGateDSR = false;
        break;
      case JH.CommBase.CommBase.Handshake.DsrDtr:
        settings.txFlowCTS = false;
        settings.txFlowDSR = true;
        settings.txFlowX = false;
        settings.rxFlowX = false;
        settings.useRTS = JH.CommBase.CommBase.HSOutput.online;
        settings.useDTR = JH.CommBase.CommBase.HSOutput.handshake;
        settings.txWhenRxXoff = true;
        settings.rxGateDSR = false;
        break;
    }
  }

  public virtual bool Open()
  {
    bool flag = false;
    lock (this)
    {
      if (this.m_Disposing)
        return false;
    }
    try
    {
      if (this.m_CommBase != null)
      {
        try
        {
          if (this.Online)
            this.Close();
          lock (this)
            flag = this.m_CommBase.Open();
        }
        catch (Exception ex)
        {
          flag = false;
        }
        finally
        {
          if (flag)
            Thread.Sleep(this.PortOpeningTime);
        }
        if (flag)
        {
          this.m_CommBase.RxByte += new SWComm.RxByteHandler(this.OnRxByte);
          this.m_CommBase.RxException += new SWComm.RxExceptionHandler(this.OnRxException);
          this.m_CommBase.StatusChanged += new StatusChangedHandler(this.OnStatusChange);
          this.m_TimerThread.Timer += new TimerHandler(this.OnTimer);
          this.m_ReadyForSend = true;
        }
        this.m_SendFlag.Set();
        this.m_StatusChangedFlag.Set();
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error [Open ({this.Port})]:{ex.Message}");
      flag = false;
    }
    return flag;
  }

  public virtual void Close()
  {
    bool flag = false;
    lock (this)
    {
      if (this.m_Disposing || this.m_Closing)
        return;
      this.m_Closing = true;
    }
    try
    {
      if (this.m_TimerThread != null)
      {
        this.m_TimerThread.StopTimer();
        try
        {
          this.m_TimerThread.Join();
        }
        catch (ThreadStateException ex)
        {
        }
        catch (ThreadInterruptedException ex)
        {
        }
      }
      if (this.m_CommBase == null)
        throw new SWCommException("No CommBase");
      try
      {
        lock (this)
        {
          flag = this.m_CommBase.Online;
          if (flag)
          {
            Thread.Sleep(this.PortOpeningTime / 2);
            this.m_CommBase.Close();
          }
        }
      }
      catch (Exception ex)
      {
        this.SetError();
      }
      finally
      {
        if (flag)
          Thread.Sleep(this.PortOpeningTime / 2);
      }
      if (this.m_TimerThread != null)
        this.m_TimerThread.Timer -= new TimerHandler(this.OnTimer);
      if (this.m_CommBase == null)
        return;
      this.m_CommBase.RxByte -= new SWComm.RxByteHandler(this.OnRxByte);
      this.m_CommBase.RxException -= new SWComm.RxExceptionHandler(this.OnRxException);
      this.m_CommBase.StatusChanged -= new StatusChangedHandler(this.OnStatusChange);
    }
    catch (Exception ex)
    {
      this.SetError();
    }
    finally
    {
      try
      {
        if (this.IsReadyForSend())
        {
          if (((WaitHandle) this.m_SendFlag).WaitOne(0, false))
            goto label_30;
        }
        this.EndSend("Erzwungenes Ende in Close", false);
      }
      catch (Exception ex)
      {
      }
label_30:
      lock (this)
        this.m_Closing = false;
    }
  }

  public bool Online
  {
    get
    {
      bool online = false;
      try
      {
        if (this.m_CommBase != null)
        {
          lock (this)
            online = this.m_CommBase.Online;
        }
      }
      catch (CommPortException ex)
      {
        this.shutDownAfterException();
        online = false;
      }
      catch (Exception ex)
      {
        this.shutDownAfterException();
        online = false;
      }
      return online;
    }
    set
    {
      try
      {
        if (value)
          this.Open();
        else
          this.Close();
      }
      catch (CommPortException ex)
      {
      }
      catch (Exception ex)
      {
      }
    }
  }

  public void Dispose()
  {
    try
    {
      lock (this)
      {
        this.m_Disposing = true;
        if (this.m_TimerThread != null)
          this.m_TimerThread.Dispose();
        this.Close();
        this.m_TimerThread = (SWTimerThread) null;
        lock (this.m_CommandLock)
          this.m_SWCommand = (SWCommand) null;
        this.m_CommBase.Dispose();
        this.m_CommBase = (SWComm.SWCommBase) null;
        this.m_SendFlag = (ManualResetEvent) null;
        this.m_StatusChangedFlag = (ManualResetEvent) null;
      }
    }
    catch (Exception ex)
    {
    }
  }

  public bool IsError => this.m_ErrorSend;

  public string Port
  {
    get => this.m_CommBase.Settings.port;
    set => this.m_CommBase.Settings.port = value;
  }

  public int DefaultTimeout
  {
    get => this.m_DefaultTimeout;
    set => this.m_DefaultTimeout = value;
  }

  public int CommportBaudRate
  {
    get => this.m_CommBase.Settings.baudRate;
    set => this.m_CommBase.Settings.baudRate = value;
  }

  public virtual bool Break
  {
    get
    {
      bool flag = false;
      try
      {
        flag = this.m_CommBase.Break;
      }
      catch (Exception ex)
      {
      }
      return flag;
    }
    set
    {
      try
      {
        this.m_CommBase.Break = value;
      }
      catch (Exception ex)
      {
      }
    }
  }

  public virtual bool Connected => this.Online;

  public int PortOpeningTime
  {
    get => this.m_PortOpeningTime;
    set => this.m_PortOpeningTime = value;
  }

  public SWNTTrashCommand Trash
  {
    get
    {
      SWCommand swCommand = (SWCommand) null;
      lock (this.m_CommandLock)
        swCommand = this.m_SWCommand;
      SWNTTrashCommand trash = new SWNTTrashCommand(this);
      if (swCommand != null && swCommand is SWNTTrashCommand)
        trash = swCommand as SWNTTrashCommand;
      return trash;
    }
  }

  protected delegate void RxByteHandler(object sender, SWComm.SWRxByteEventArgs e);

  protected delegate void RxExceptionHandler(object sender, SWComm.SWRxExceptionEventArgs e);

  protected class SWRxExceptionEventArgs : EventArgs
  {
    public Exception Reason;

    public SWRxExceptionEventArgs(Exception theException) => this.Reason = theException;

    public virtual string ToString() => this.Reason.Message;
  }

  protected class SWRxByteEventArgs : EventArgs
  {
    public byte Byte = 0;

    public SWRxByteEventArgs(byte theByte) => this.Byte = theByte;

    public virtual string ToString() => $"{this.Byte}";
  }

  protected class SWCommBase : JH.CommBase.CommBase
  {
    public uint TxTimeout = 20;
    private JH.CommBase.CommBase.CommBaseSettings m_settings = (JH.CommBase.CommBase.CommBaseSettings) null;
    private object lockModemStatus = new object();

    public event SWComm.RxByteHandler RxByte = null;

    public event SWComm.RxExceptionHandler RxException = null;

    public event StatusChangedHandler StatusChanged = null;

    public SWCommBase()
    {
      this.m_settings = new JH.CommBase.CommBase.CommBaseSettings();
      this.m_settings.SetStandard("COM1", 9600, JH.CommBase.CommBase.Handshake.none);
      this.m_settings.sendTimeoutConstant = this.TxTimeout;
      this.m_settings.sendTimeoutMultiplier = 1U;
    }

    public new bool IsCongested() => base.IsCongested();

    public new void Send(byte[] Bytes)
    {
      lock (Bytes)
        base.Send(Bytes);
    }

    public new JH.CommBase.CommBase.QueueStatus GetQueueStatus() => base.GetQueueStatus();

    public new JH.CommBase.CommBase.ModemStatus GetModemStatus()
    {
      lock (this.lockModemStatus)
      {
        JH.CommBase.CommBase.ModemStatus modemStatus = new JH.CommBase.CommBase.ModemStatus();
        if (this.Online)
        {
          try
          {
            lock (this)
              modemStatus = base.GetModemStatus();
          }
          catch (Exception ex)
          {
          }
        }
        return modemStatus;
      }
    }

    protected override JH.CommBase.CommBase.CommBaseSettings CommSettings() => this.m_settings;

    protected override void OnRxChar(byte ch)
    {
      if (this.RxByte == null)
        return;
      SWComm.SWRxByteEventArgs e = new SWComm.SWRxByteEventArgs(ch);
      try
      {
        this.RxByte((object) this, e);
      }
      catch (Exception ex)
      {
      }
    }

    protected override void OnStatusChange(JH.CommBase.CommBase.ModemStatus mask, JH.CommBase.CommBase.ModemStatus state)
    {
      if (this.StatusChanged == null)
        return;
      this.StatusChanged((object) this, new SWStatusChangeEventArgs(mask, state));
    }

    protected override void OnRxException(Exception e)
    {
      if (this.RxException == null)
        return;
      SWComm.SWRxExceptionEventArgs e1 = new SWComm.SWRxExceptionEventArgs(e);
      try
      {
        this.RxException((object) this, e1);
      }
      catch (Exception ex)
      {
      }
    }

    public JH.CommBase.CommBase.CommBaseSettings Settings
    {
      get => this.m_settings;
      set => this.m_settings = value;
    }

    public new bool Break
    {
      get => base.Break;
      set => base.Break = value;
    }
  }
}
