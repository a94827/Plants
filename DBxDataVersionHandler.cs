using System;
using System.Collections.Generic;
using System.Text;
using FreeLibSet.Data;
using System.Data;
using System.Runtime.Serialization;
using FreeLibSet.Core;

// ��� �������� � ExtDBDocs.dll

namespace Plants
{
  /// <summary>
  /// ��������� ������� "DataVersion" � ���� ������ ����������.
  /// ������������� ������������� ���� ������ � ������ ����������.
  /// ������ ������ ������, ������������� downgrade ��������� �� ������������� ������.
  /// 
  /// </summary>
  public class DBxDataVersionHandler
  {
    #region �����������

    public DBxDataVersionHandler(Guid appGuid, int currentVersion, int minVersion)
    {
      if (appGuid == Guid.Empty)
        throw new ArgumentException("�� ����� AppGuid", "appGuid");
      if (currentVersion < 1)
        throw new ArgumentException("������ ������ ���� �� ������ 1", "currentVersion");
      if (minVersion < 1)
        throw new ArgumentException("����������� ������ ������ ���� �� ������ 1", "minVersion");
      if (minVersion > currentVersion)
        throw new ArgumentException("����������� ������ �� ����� ���� ������ �������� ������", "minVersion");

      _AppGuid = appGuid;
      _CurrentVersion = currentVersion;
      _MinVersion = currentVersion;
    }

    #endregion

    #region ��������

    public Guid AppGuid { get { return _AppGuid; } }
    private Guid _AppGuid;

    public int CurrentVersion { get { return _CurrentVersion; } }
    private int _CurrentVersion;

    public int MinVersion { get { return _MinVersion; } }
    private int _MinVersion;

    #endregion

    #region ���������� ��������� �������

    public DBxTableStruct AddTableStruct(DBxStruct dbStruct)
    {
      DBxTableStruct ts = new DBxTableStruct("DataVersion");
      ts.Columns.AddId(); // �� ������������
      ts.Columns.AddString("AppGUID", 36, false);
      ts.Columns.AddString("DataGUID", 36, false);
      ts.Columns.AddInt("CurrentVersion", 1, Int32.MaxValue);
      ts.Columns.AddInt("MinVersion", 1, Int32.MaxValue);
      dbStruct.Tables.Add(ts);
      return ts;
    }

    #endregion

    #region �������������

    public Guid DataGuid { get { return _DataGuid; } }
    private Guid _DataGuid;

    public int PrevVersion { get { return _PrevVersion; } }
    private int _PrevVersion;

    public void InitTableRow(DBxCon con)
    {
      DataTable table = con.FillSelect("DataVersion");
      if (table.Rows.Count == 0)
      {
        // ������ ������

        _DataGuid = Guid.NewGuid();
        con.AddRecord("DataVersion", new DBxColumns("AppGUID,DataGUID,CurrentVersion,MinVersion"),
          new object[] { AppGuid.ToString(), _DataGuid.ToString(), CurrentVersion, MinVersion });
      }
      else if (table.Rows.Count > 1)
        throw new DBxDataVersionHandlerException("������� DataVersion �������� ������������ ����� �����: " + table.Rows.Count.ToString());
      else
      {
        // ��������� ������

        DataRow row = table.Rows[0];
        Guid oldAppGuid = new Guid(DataTools.GetString(row, "AppGUID"));
        if (oldAppGuid != AppGuid)
          throw new DBxDataVersionHandlerException("���� ������ ������������� ��� ������ � ������ ����������");

        _DataGuid = new Guid(DataTools.GetString(row, "DataGUID"));
        _PrevVersion = DataTools.GetInt(row, "CurrentVersion");

        int oldMinVersion = DataTools.GetInt(row, "MinVersion");
        if (CurrentVersion < oldMinVersion)
          throw new DBxDataVersionHandlerException("���� ������ ���� ��������� � ����� ����� ������ ���������. ����� �� ������� ������ ����������");

        if (CurrentVersion != _PrevVersion)
        {
          Int32 dummyId = DataTools.GetInt(row, "Id");
          con.SetValues("DataVersion", dummyId, new DBxColumns("CurrentVesrion,MinVersion"),
            new object[] { CurrentVersion, MinVersion });
        }
      }
    }

    #endregion
  }

  [Serializable]
  public class DBxDataVersionHandlerException : ApplicationException
  {
    #region �����������

    public DBxDataVersionHandlerException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// ��� ������ ������������ ����� ��� ���������� ��������������
    /// </summary>
    protected DBxDataVersionHandlerException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    #endregion
  }
}
