// Decompiled with JetBrains decompiler
// Type: de.sunways.ntmonitor.DataSetOLV
// Assembly: SWNTMonLib, Version=1.0.2286.20848, Culture=neutral, PublicKeyToken=null
// MVID: FA1BE6D6-9789-41BC-A3D3-657A8164DEC5
// Assembly location: /NT_Monitor_2.0/SWNTMonLib.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;

#nullable disable
namespace de.sunways.ntmonitor;

[DebuggerStepThrough]
[DesignerCategory("code")]
[ToolboxItem(true)]
[Serializable]
public class DataSetOLV : DataSet
{
  private DataSetOLV.tbwOnlineDataTable tabletbwOnline;

  public DataSetOLV()
  {
    this.InitClass();
    CollectionChangeEventHandler changeEventHandler = new CollectionChangeEventHandler(this.SchemaChanged);
    this.Tables.CollectionChanged += changeEventHandler;
    this.Relations.CollectionChanged += changeEventHandler;
  }

  protected DataSetOLV(SerializationInfo info, StreamingContext context)
  {
    string str = (string) info.GetValue("XmlSchema", typeof (string));
    if (str != null)
    {
      DataSet dataSet = new DataSet();
      dataSet.ReadXmlSchema((XmlReader) new XmlTextReader((TextReader) new StringReader(str)));
      if (dataSet.Tables[nameof (tbwOnline)] != null)
        this.Tables.Add((DataTable) new DataSetOLV.tbwOnlineDataTable(dataSet.Tables[nameof (tbwOnline)]));
      this.DataSetName = dataSet.DataSetName;
      this.Prefix = dataSet.Prefix;
      this.Namespace = dataSet.Namespace;
      this.Locale = dataSet.Locale;
      this.CaseSensitive = dataSet.CaseSensitive;
      this.EnforceConstraints = dataSet.EnforceConstraints;
      this.Merge(dataSet, false, (MissingSchemaAction) 1);
      this.InitVars();
    }
    else
      this.InitClass();
    this.GetSerializationData(info, context);
    CollectionChangeEventHandler changeEventHandler = new CollectionChangeEventHandler(this.SchemaChanged);
    this.Tables.CollectionChanged += changeEventHandler;
    this.Relations.CollectionChanged += changeEventHandler;
  }

  [DesignerSerializationVisibility]
  [Browsable(false)]
  public DataSetOLV.tbwOnlineDataTable tbwOnline => this.tabletbwOnline;

  public virtual DataSet Clone()
  {
    DataSetOLV dataSetOlv = (DataSetOLV) base.Clone();
    dataSetOlv.InitVars();
    return (DataSet) dataSetOlv;
  }

  protected virtual bool ShouldSerializeTables() => false;

  protected virtual bool ShouldSerializeRelations() => false;

  protected virtual void ReadXmlSerializable(XmlReader reader)
  {
    this.Reset();
    DataSet dataSet = new DataSet();
    dataSet.ReadXml(reader);
    if (dataSet.Tables["tbwOnline"] != null)
      this.Tables.Add((DataTable) new DataSetOLV.tbwOnlineDataTable(dataSet.Tables["tbwOnline"]));
    this.DataSetName = dataSet.DataSetName;
    this.Prefix = dataSet.Prefix;
    this.Namespace = dataSet.Namespace;
    this.Locale = dataSet.Locale;
    this.CaseSensitive = dataSet.CaseSensitive;
    this.EnforceConstraints = dataSet.EnforceConstraints;
    this.Merge(dataSet, false, (MissingSchemaAction) 1);
    this.InitVars();
  }

  protected virtual XmlSchema GetSchemaSerializable()
  {
    MemoryStream memoryStream = new MemoryStream();
    this.WriteXmlSchema((XmlWriter) new XmlTextWriter((Stream) memoryStream, (Encoding) null));
    ((Stream) memoryStream).Position = 0L;
    return XmlSchema.Read((XmlReader) new XmlTextReader((Stream) memoryStream), (ValidationEventHandler) null);
  }

  internal void InitVars()
  {
    this.tabletbwOnline = (DataSetOLV.tbwOnlineDataTable) this.Tables["tbwOnline"];
    if (this.tabletbwOnline == null)
      return;
    this.tabletbwOnline.InitVars();
  }

  private void InitClass()
  {
    this.DataSetName = nameof (DataSetOLV);
    this.Prefix = "";
    this.Namespace = "http://tempuri.org/DatasetOLV.xsd";
    this.Locale = new CultureInfo("en-US");
    this.CaseSensitive = false;
    this.EnforceConstraints = true;
    this.tabletbwOnline = new DataSetOLV.tbwOnlineDataTable();
    this.Tables.Add((DataTable) this.tabletbwOnline);
  }

  private bool ShouldSerializetbwOnline() => false;

  private void SchemaChanged(object sender, CollectionChangeEventArgs e)
  {
    if (e.Action != 2)
      return;
    this.InitVars();
  }

  public delegate void tbwOnlineRowChangeEventHandler(
    object sender,
    DataSetOLV.tbwOnlineRowChangeEvent e);

  [DebuggerStepThrough]
  public class tbwOnlineDataTable : DataTable, IEnumerable
  {
    private DataColumn columnidOnline;
    private DataColumn columnidInverter;
    private DataColumn columndtmDate;
    private DataColumn columndblUDC;
    private DataColumn columndblIDC;
    private DataColumn columndblUAC;
    private DataColumn columndblIAC;
    private DataColumn columndblPDC;
    private DataColumn columndblPAC;
    private DataColumn columndblTEMP;
    private DataColumn columndblSOLAR;
    private DataColumn columndblPowerDay;
    private DataColumn columndblPowerSUM;
    private DataColumn columndblOverflow;
    private DataColumn columnidStatus;
    private DataColumn columnstrUIStatus;
    private DataColumn columnstrInverterName;

    internal tbwOnlineDataTable()
      : base("tbwOnline")
    {
      this.InitClass();
    }

    internal tbwOnlineDataTable(DataTable table)
      : base(table.TableName)
    {
      if (table.CaseSensitive != table.DataSet.CaseSensitive)
        this.CaseSensitive = table.CaseSensitive;
      if (string.op_Inequality(table.Locale.ToString(), table.DataSet.Locale.ToString()))
        this.Locale = table.Locale;
      if (string.op_Inequality(table.Namespace, table.DataSet.Namespace))
        this.Namespace = table.Namespace;
      this.Prefix = table.Prefix;
      this.MinimumCapacity = table.MinimumCapacity;
      this.DisplayExpression = table.DisplayExpression;
    }

    [Browsable(false)]
    public int Count => ((InternalDataCollectionBase) this.Rows).Count;

    internal DataColumn idOnlineColumn => this.columnidOnline;

    internal DataColumn idInverterColumn => this.columnidInverter;

    internal DataColumn dtmDateColumn => this.columndtmDate;

    internal DataColumn dblUDCColumn => this.columndblUDC;

    internal DataColumn dblIDCColumn => this.columndblIDC;

    internal DataColumn dblUACColumn => this.columndblUAC;

    internal DataColumn dblIACColumn => this.columndblIAC;

    internal DataColumn dblPDCColumn => this.columndblPDC;

    internal DataColumn dblPACColumn => this.columndblPAC;

    internal DataColumn dblTEMPColumn => this.columndblTEMP;

    internal DataColumn dblSOLARColumn => this.columndblSOLAR;

    internal DataColumn dblPowerDayColumn => this.columndblPowerDay;

    internal DataColumn dblPowerSUMColumn => this.columndblPowerSUM;

    internal DataColumn dblOverflowColumn => this.columndblOverflow;

    internal DataColumn idStatusColumn => this.columnidStatus;

    internal DataColumn strUIStatusColumn => this.columnstrUIStatus;

    internal DataColumn strInverterNameColumn => this.columnstrInverterName;

    public DataSetOLV.tbwOnlineRow this[int index] => (DataSetOLV.tbwOnlineRow) this.Rows[index];

    public event DataSetOLV.tbwOnlineRowChangeEventHandler tbwOnlineRowChanged;

    public event DataSetOLV.tbwOnlineRowChangeEventHandler tbwOnlineRowChanging;

    public event DataSetOLV.tbwOnlineRowChangeEventHandler tbwOnlineRowDeleted;

    public event DataSetOLV.tbwOnlineRowChangeEventHandler tbwOnlineRowDeleting;

    public void AddtbwOnlineRow(DataSetOLV.tbwOnlineRow row) => this.Rows.Add((DataRow) row);

    public DataSetOLV.tbwOnlineRow AddtbwOnlineRow(
      int idInverter,
      DateTime dtmDate,
      double dblUDC,
      double dblIDC,
      double dblUAC,
      double dblIAC,
      double dblPDC,
      double dblPAC,
      double dblTEMP,
      double dblSOLAR,
      double dblPowerDay,
      double dblPowerSUM,
      double dblOverflow,
      int idStatus,
      string strUIStatus,
      string strInverterName)
    {
      DataSetOLV.tbwOnlineRow tbwOnlineRow = (DataSetOLV.tbwOnlineRow) this.NewRow();
      tbwOnlineRow.ItemArray = new object[17]
      {
        null,
        (object) idInverter,
        (object) dtmDate,
        (object) dblUDC,
        (object) dblIDC,
        (object) dblUAC,
        (object) dblIAC,
        (object) dblPDC,
        (object) dblPAC,
        (object) dblTEMP,
        (object) dblSOLAR,
        (object) dblPowerDay,
        (object) dblPowerSUM,
        (object) dblOverflow,
        (object) idStatus,
        (object) strUIStatus,
        (object) strInverterName
      };
      this.Rows.Add((DataRow) tbwOnlineRow);
      return tbwOnlineRow;
    }

    public DataSetOLV.tbwOnlineRow FindByidOnline(int idOnline)
    {
      return (DataSetOLV.tbwOnlineRow) this.Rows.Find(new object[1]
      {
        (object) idOnline
      });
    }

    public IEnumerator GetEnumerator() => ((InternalDataCollectionBase) this.Rows).GetEnumerator();

    public virtual DataTable Clone()
    {
      DataSetOLV.tbwOnlineDataTable tbwOnlineDataTable = (DataSetOLV.tbwOnlineDataTable) base.Clone();
      tbwOnlineDataTable.InitVars();
      return (DataTable) tbwOnlineDataTable;
    }

    protected virtual DataTable CreateInstance() => (DataTable) new DataSetOLV.tbwOnlineDataTable();

    internal void InitVars()
    {
      this.columnidOnline = this.Columns["idOnline"];
      this.columnidInverter = this.Columns["idInverter"];
      this.columndtmDate = this.Columns["dtmDate"];
      this.columndblUDC = this.Columns["dblUDC"];
      this.columndblIDC = this.Columns["dblIDC"];
      this.columndblUAC = this.Columns["dblUAC"];
      this.columndblIAC = this.Columns["dblIAC"];
      this.columndblPDC = this.Columns["dblPDC"];
      this.columndblPAC = this.Columns["dblPAC"];
      this.columndblTEMP = this.Columns["dblTEMP"];
      this.columndblSOLAR = this.Columns["dblSOLAR"];
      this.columndblPowerDay = this.Columns["dblPowerDay"];
      this.columndblPowerSUM = this.Columns["dblPowerSUM"];
      this.columndblOverflow = this.Columns["dblOverflow"];
      this.columnidStatus = this.Columns["idStatus"];
      this.columnstrUIStatus = this.Columns["strUIStatus"];
      this.columnstrInverterName = this.Columns["strInverterName"];
    }

    private void InitClass()
    {
      this.columnidOnline = new DataColumn("idOnline", typeof (int), (string) null, (MappingType) 1);
      this.Columns.Add(this.columnidOnline);
      this.columnidInverter = new DataColumn("idInverter", typeof (int), (string) null, (MappingType) 1);
      this.Columns.Add(this.columnidInverter);
      this.columndtmDate = new DataColumn("dtmDate", typeof (DateTime), (string) null, (MappingType) 1);
      this.Columns.Add(this.columndtmDate);
      this.columndblUDC = new DataColumn("dblUDC", typeof (double), (string) null, (MappingType) 1);
      this.Columns.Add(this.columndblUDC);
      this.columndblIDC = new DataColumn("dblIDC", typeof (double), (string) null, (MappingType) 1);
      this.Columns.Add(this.columndblIDC);
      this.columndblUAC = new DataColumn("dblUAC", typeof (double), (string) null, (MappingType) 1);
      this.Columns.Add(this.columndblUAC);
      this.columndblIAC = new DataColumn("dblIAC", typeof (double), (string) null, (MappingType) 1);
      this.Columns.Add(this.columndblIAC);
      this.columndblPDC = new DataColumn("dblPDC", typeof (double), (string) null, (MappingType) 1);
      this.Columns.Add(this.columndblPDC);
      this.columndblPAC = new DataColumn("dblPAC", typeof (double), (string) null, (MappingType) 1);
      this.Columns.Add(this.columndblPAC);
      this.columndblTEMP = new DataColumn("dblTEMP", typeof (double), (string) null, (MappingType) 1);
      this.Columns.Add(this.columndblTEMP);
      this.columndblSOLAR = new DataColumn("dblSOLAR", typeof (double), (string) null, (MappingType) 1);
      this.Columns.Add(this.columndblSOLAR);
      this.columndblPowerDay = new DataColumn("dblPowerDay", typeof (double), (string) null, (MappingType) 1);
      this.Columns.Add(this.columndblPowerDay);
      this.columndblPowerSUM = new DataColumn("dblPowerSUM", typeof (double), (string) null, (MappingType) 1);
      this.Columns.Add(this.columndblPowerSUM);
      this.columndblOverflow = new DataColumn("dblOverflow", typeof (double), (string) null, (MappingType) 1);
      this.Columns.Add(this.columndblOverflow);
      this.columnidStatus = new DataColumn("idStatus", typeof (int), (string) null, (MappingType) 1);
      this.Columns.Add(this.columnidStatus);
      this.columnstrUIStatus = new DataColumn("strUIStatus", typeof (string), (string) null, (MappingType) 1);
      this.Columns.Add(this.columnstrUIStatus);
      this.columnstrInverterName = new DataColumn("strInverterName", typeof (string), (string) null, (MappingType) 1);
      this.Columns.Add(this.columnstrInverterName);
      this.Constraints.Add((Constraint) new UniqueConstraint("DatasetOLVKey1", new DataColumn[1]
      {
        this.columnidOnline
      }, true));
      this.columnidOnline.AutoIncrement = true;
      this.columnidOnline.AllowDBNull = false;
      this.columnidOnline.Unique = true;
    }

    public DataSetOLV.tbwOnlineRow NewtbwOnlineRow() => (DataSetOLV.tbwOnlineRow) this.NewRow();

    protected virtual DataRow NewRowFromBuilder(DataRowBuilder builder)
    {
      return (DataRow) new DataSetOLV.tbwOnlineRow(builder);
    }

    protected virtual Type GetRowType() => typeof (DataSetOLV.tbwOnlineRow);

    protected virtual void OnRowChanged(DataRowChangeEventArgs e)
    {
      base.OnRowChanged(e);
      if (this.tbwOnlineRowChanged == null)
        return;
      this.tbwOnlineRowChanged((object) this, new DataSetOLV.tbwOnlineRowChangeEvent((DataSetOLV.tbwOnlineRow) e.Row, e.Action));
    }

    protected virtual void OnRowChanging(DataRowChangeEventArgs e)
    {
      base.OnRowChanging(e);
      if (this.tbwOnlineRowChanging == null)
        return;
      this.tbwOnlineRowChanging((object) this, new DataSetOLV.tbwOnlineRowChangeEvent((DataSetOLV.tbwOnlineRow) e.Row, e.Action));
    }

    protected virtual void OnRowDeleted(DataRowChangeEventArgs e)
    {
      base.OnRowDeleted(e);
      if (this.tbwOnlineRowDeleted == null)
        return;
      this.tbwOnlineRowDeleted((object) this, new DataSetOLV.tbwOnlineRowChangeEvent((DataSetOLV.tbwOnlineRow) e.Row, e.Action));
    }

    protected virtual void OnRowDeleting(DataRowChangeEventArgs e)
    {
      base.OnRowDeleting(e);
      if (this.tbwOnlineRowDeleting == null)
        return;
      this.tbwOnlineRowDeleting((object) this, new DataSetOLV.tbwOnlineRowChangeEvent((DataSetOLV.tbwOnlineRow) e.Row, e.Action));
    }

    public void RemovetbwOnlineRow(DataSetOLV.tbwOnlineRow row) => this.Rows.Remove((DataRow) row);
  }

  [DebuggerStepThrough]
  public class tbwOnlineRow : DataRow
  {
    private DataSetOLV.tbwOnlineDataTable tabletbwOnline;

    internal tbwOnlineRow(DataRowBuilder rb)
      : base(rb)
    {
      this.tabletbwOnline = (DataSetOLV.tbwOnlineDataTable) this.Table;
    }

    public int idOnline
    {
      get => (int) this[this.tabletbwOnline.idOnlineColumn];
      set => this[this.tabletbwOnline.idOnlineColumn] = (object) value;
    }

    public int idInverter
    {
      get
      {
        try
        {
          return (int) this[this.tabletbwOnline.idInverterColumn];
        }
        catch (InvalidCastException ex)
        {
          throw new StrongTypingException("Der Wert kann nicht ermittelt werden, da er DBNull ist.", (Exception) ex);
        }
      }
      set => this[this.tabletbwOnline.idInverterColumn] = (object) value;
    }

    public DateTime dtmDate
    {
      get
      {
        try
        {
          return (DateTime) this[this.tabletbwOnline.dtmDateColumn];
        }
        catch (InvalidCastException ex)
        {
          throw new StrongTypingException("Der Wert kann nicht ermittelt werden, da er DBNull ist.", (Exception) ex);
        }
      }
      set => this[this.tabletbwOnline.dtmDateColumn] = (object) value;
    }

    public double dblUDC
    {
      get
      {
        try
        {
          return (double) this[this.tabletbwOnline.dblUDCColumn];
        }
        catch (InvalidCastException ex)
        {
          throw new StrongTypingException("Der Wert kann nicht ermittelt werden, da er DBNull ist.", (Exception) ex);
        }
      }
      set => this[this.tabletbwOnline.dblUDCColumn] = (object) value;
    }

    public double dblIDC
    {
      get
      {
        try
        {
          return (double) this[this.tabletbwOnline.dblIDCColumn];
        }
        catch (InvalidCastException ex)
        {
          throw new StrongTypingException("Der Wert kann nicht ermittelt werden, da er DBNull ist.", (Exception) ex);
        }
      }
      set => this[this.tabletbwOnline.dblIDCColumn] = (object) value;
    }

    public double dblUAC
    {
      get
      {
        try
        {
          return (double) this[this.tabletbwOnline.dblUACColumn];
        }
        catch (InvalidCastException ex)
        {
          throw new StrongTypingException("Der Wert kann nicht ermittelt werden, da er DBNull ist.", (Exception) ex);
        }
      }
      set => this[this.tabletbwOnline.dblUACColumn] = (object) value;
    }

    public double dblIAC
    {
      get
      {
        try
        {
          return (double) this[this.tabletbwOnline.dblIACColumn];
        }
        catch (InvalidCastException ex)
        {
          throw new StrongTypingException("Der Wert kann nicht ermittelt werden, da er DBNull ist.", (Exception) ex);
        }
      }
      set => this[this.tabletbwOnline.dblIACColumn] = (object) value;
    }

    public double dblPDC
    {
      get
      {
        try
        {
          return (double) this[this.tabletbwOnline.dblPDCColumn];
        }
        catch (InvalidCastException ex)
        {
          throw new StrongTypingException("Der Wert kann nicht ermittelt werden, da er DBNull ist.", (Exception) ex);
        }
      }
      set => this[this.tabletbwOnline.dblPDCColumn] = (object) value;
    }

    public double dblPAC
    {
      get
      {
        try
        {
          return (double) this[this.tabletbwOnline.dblPACColumn];
        }
        catch (InvalidCastException ex)
        {
          throw new StrongTypingException("Der Wert kann nicht ermittelt werden, da er DBNull ist.", (Exception) ex);
        }
      }
      set => this[this.tabletbwOnline.dblPACColumn] = (object) value;
    }

    public double dblTEMP
    {
      get
      {
        try
        {
          return (double) this[this.tabletbwOnline.dblTEMPColumn];
        }
        catch (InvalidCastException ex)
        {
          throw new StrongTypingException("Der Wert kann nicht ermittelt werden, da er DBNull ist.", (Exception) ex);
        }
      }
      set => this[this.tabletbwOnline.dblTEMPColumn] = (object) value;
    }

    public double dblSOLAR
    {
      get
      {
        try
        {
          return (double) this[this.tabletbwOnline.dblSOLARColumn];
        }
        catch (InvalidCastException ex)
        {
          throw new StrongTypingException("Der Wert kann nicht ermittelt werden, da er DBNull ist.", (Exception) ex);
        }
      }
      set => this[this.tabletbwOnline.dblSOLARColumn] = (object) value;
    }

    public double dblPowerDay
    {
      get
      {
        try
        {
          return (double) this[this.tabletbwOnline.dblPowerDayColumn];
        }
        catch (InvalidCastException ex)
        {
          throw new StrongTypingException("Der Wert kann nicht ermittelt werden, da er DBNull ist.", (Exception) ex);
        }
      }
      set => this[this.tabletbwOnline.dblPowerDayColumn] = (object) value;
    }

    public double dblPowerSUM
    {
      get
      {
        try
        {
          return (double) this[this.tabletbwOnline.dblPowerSUMColumn];
        }
        catch (InvalidCastException ex)
        {
          throw new StrongTypingException("Der Wert kann nicht ermittelt werden, da er DBNull ist.", (Exception) ex);
        }
      }
      set => this[this.tabletbwOnline.dblPowerSUMColumn] = (object) value;
    }

    public double dblOverflow
    {
      get
      {
        try
        {
          return (double) this[this.tabletbwOnline.dblOverflowColumn];
        }
        catch (InvalidCastException ex)
        {
          throw new StrongTypingException("Der Wert kann nicht ermittelt werden, da er DBNull ist.", (Exception) ex);
        }
      }
      set => this[this.tabletbwOnline.dblOverflowColumn] = (object) value;
    }

    public int idStatus
    {
      get
      {
        try
        {
          return (int) this[this.tabletbwOnline.idStatusColumn];
        }
        catch (InvalidCastException ex)
        {
          throw new StrongTypingException("Der Wert kann nicht ermittelt werden, da er DBNull ist.", (Exception) ex);
        }
      }
      set => this[this.tabletbwOnline.idStatusColumn] = (object) value;
    }

    public string strUIStatus
    {
      get
      {
        try
        {
          return (string) this[this.tabletbwOnline.strUIStatusColumn];
        }
        catch (InvalidCastException ex)
        {
          throw new StrongTypingException("Der Wert kann nicht ermittelt werden, da er DBNull ist.", (Exception) ex);
        }
      }
      set => this[this.tabletbwOnline.strUIStatusColumn] = (object) value;
    }

    public string strInverterName
    {
      get
      {
        try
        {
          return (string) this[this.tabletbwOnline.strInverterNameColumn];
        }
        catch (InvalidCastException ex)
        {
          throw new StrongTypingException("Der Wert kann nicht ermittelt werden, da er DBNull ist.", (Exception) ex);
        }
      }
      set => this[this.tabletbwOnline.strInverterNameColumn] = (object) value;
    }

    public bool IsidInverterNull() => this.IsNull(this.tabletbwOnline.idInverterColumn);

    public void SetidInverterNull() => this[this.tabletbwOnline.idInverterColumn] = Convert.DBNull;

    public bool IsdtmDateNull() => this.IsNull(this.tabletbwOnline.dtmDateColumn);

    public void SetdtmDateNull() => this[this.tabletbwOnline.dtmDateColumn] = Convert.DBNull;

    public bool IsdblUDCNull() => this.IsNull(this.tabletbwOnline.dblUDCColumn);

    public void SetdblUDCNull() => this[this.tabletbwOnline.dblUDCColumn] = Convert.DBNull;

    public bool IsdblIDCNull() => this.IsNull(this.tabletbwOnline.dblIDCColumn);

    public void SetdblIDCNull() => this[this.tabletbwOnline.dblIDCColumn] = Convert.DBNull;

    public bool IsdblUACNull() => this.IsNull(this.tabletbwOnline.dblUACColumn);

    public void SetdblUACNull() => this[this.tabletbwOnline.dblUACColumn] = Convert.DBNull;

    public bool IsdblIACNull() => this.IsNull(this.tabletbwOnline.dblIACColumn);

    public void SetdblIACNull() => this[this.tabletbwOnline.dblIACColumn] = Convert.DBNull;

    public bool IsdblPDCNull() => this.IsNull(this.tabletbwOnline.dblPDCColumn);

    public void SetdblPDCNull() => this[this.tabletbwOnline.dblPDCColumn] = Convert.DBNull;

    public bool IsdblPACNull() => this.IsNull(this.tabletbwOnline.dblPACColumn);

    public void SetdblPACNull() => this[this.tabletbwOnline.dblPACColumn] = Convert.DBNull;

    public bool IsdblTEMPNull() => this.IsNull(this.tabletbwOnline.dblTEMPColumn);

    public void SetdblTEMPNull() => this[this.tabletbwOnline.dblTEMPColumn] = Convert.DBNull;

    public bool IsdblSOLARNull() => this.IsNull(this.tabletbwOnline.dblSOLARColumn);

    public void SetdblSOLARNull() => this[this.tabletbwOnline.dblSOLARColumn] = Convert.DBNull;

    public bool IsdblPowerDayNull() => this.IsNull(this.tabletbwOnline.dblPowerDayColumn);

    public void SetdblPowerDayNull()
    {
      this[this.tabletbwOnline.dblPowerDayColumn] = Convert.DBNull;
    }

    public bool IsdblPowerSUMNull() => this.IsNull(this.tabletbwOnline.dblPowerSUMColumn);

    public void SetdblPowerSUMNull()
    {
      this[this.tabletbwOnline.dblPowerSUMColumn] = Convert.DBNull;
    }

    public bool IsdblOverflowNull() => this.IsNull(this.tabletbwOnline.dblOverflowColumn);

    public void SetdblOverflowNull()
    {
      this[this.tabletbwOnline.dblOverflowColumn] = Convert.DBNull;
    }

    public bool IsidStatusNull() => this.IsNull(this.tabletbwOnline.idStatusColumn);

    public void SetidStatusNull() => this[this.tabletbwOnline.idStatusColumn] = Convert.DBNull;

    public bool IsstrUIStatusNull() => this.IsNull(this.tabletbwOnline.strUIStatusColumn);

    public void SetstrUIStatusNull()
    {
      this[this.tabletbwOnline.strUIStatusColumn] = Convert.DBNull;
    }

    public bool IsstrInverterNameNull() => this.IsNull(this.tabletbwOnline.strInverterNameColumn);

    public void SetstrInverterNameNull()
    {
      this[this.tabletbwOnline.strInverterNameColumn] = Convert.DBNull;
    }
  }

  [DebuggerStepThrough]
  public class tbwOnlineRowChangeEvent : EventArgs
  {
    private DataSetOLV.tbwOnlineRow eventRow;
    private DataRowAction eventAction;

    public tbwOnlineRowChangeEvent(DataSetOLV.tbwOnlineRow row, DataRowAction action)
    {
      this.eventRow = row;
      this.eventAction = action;
    }

    public DataSetOLV.tbwOnlineRow Row => this.eventRow;

    public DataRowAction Action => this.eventAction;
  }
}
