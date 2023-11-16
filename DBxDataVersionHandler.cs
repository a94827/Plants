using System;
using System.Collections.Generic;
using System.Text;
using FreeLibSet.Data;
using System.Data;
using System.Runtime.Serialization;
using FreeLibSet.Core;

// Для переноса в ExtDBDocs.dll

namespace Plants
{
  /// <summary>
  /// Обработка таблицы "DataVersion" в базе данных документов.
  /// Предотвращает присоединение базы данных к чужому приложению.
  /// Хранит версию данных, предотвращает downgrade программы до несовместимой версии.
  /// 
  /// </summary>
  public class DBxDataVersionHandler
  {
    #region Конструктор

    public DBxDataVersionHandler(Guid appGuid, int currentVersion, int minVersion)
    {
      if (appGuid == Guid.Empty)
        throw new ArgumentException("Не задан AppGuid", "appGuid");
      if (currentVersion < 1)
        throw new ArgumentException("Версия должна быть не меньше 1", "currentVersion");
      if (minVersion < 1)
        throw new ArgumentException("Минимальная версия должна быть не меньше 1", "minVersion");
      if (minVersion > currentVersion)
        throw new ArgumentException("Минимальная версия не может быть больше основной версии", "minVersion");

      _AppGuid = appGuid;
      _CurrentVersion = currentVersion;
      _MinVersion = currentVersion;
    }

    #endregion

    #region Свойства

    public Guid AppGuid { get { return _AppGuid; } }
    private Guid _AppGuid;

    public int CurrentVersion { get { return _CurrentVersion; } }
    private int _CurrentVersion;

    public int MinVersion { get { return _MinVersion; } }
    private int _MinVersion;

    #endregion

    #region Объявление структуры таблицы

    public DBxTableStruct AddTableStruct(DBxStruct dbStruct)
    {
      DBxTableStruct ts = new DBxTableStruct("DataVersion");
      ts.Columns.AddId(); // не используется
      ts.Columns.AddString("AppGUID", 36, false);
      ts.Columns.AddString("DataGUID", 36, false);
      ts.Columns.AddInt("CurrentVersion", 1, Int32.MaxValue);
      ts.Columns.AddInt("MinVersion", 1, Int32.MaxValue);
      dbStruct.Tables.Add(ts);
      return ts;
    }

    #endregion

    #region Инициализация

    public Guid DataGuid { get { return _DataGuid; } }
    private Guid _DataGuid;

    public int PrevVersion { get { return _PrevVersion; } }
    private int _PrevVersion;

    public void InitTableRow(DBxCon con)
    {
      DataTable table = con.FillSelect("DataVersion");
      if (table.Rows.Count == 0)
      {
        // Первый запуск

        _DataGuid = Guid.NewGuid();
        con.AddRecord("DataVersion", new DBxColumns("AppGUID,DataGUID,CurrentVersion,MinVersion"),
          new object[] { AppGuid.ToString(), _DataGuid.ToString(), CurrentVersion, MinVersion });
      }
      else if (table.Rows.Count > 1)
        throw new DBxDataVersionHandlerException("Таблица DataVersion содержит недопустимое число строк: " + table.Rows.Count.ToString());
      else
      {
        // Проверяем версию

        DataRow row = table.Rows[0];
        Guid oldAppGuid = new Guid(DataTools.GetString(row, "AppGUID"));
        if (oldAppGuid != AppGuid)
          throw new DBxDataVersionHandlerException("База данных предназначена для работы с другой программой");

        _DataGuid = new Guid(DataTools.GetString(row, "DataGUID"));
        _PrevVersion = DataTools.GetInt(row, "CurrentVersion");

        int oldMinVersion = DataTools.GetInt(row, "MinVersion");
        if (CurrentVersion < oldMinVersion)
          throw new DBxDataVersionHandlerException("База данных была обновлена в более новой версии программы. Откат до текущей версии невозможен");

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
    #region Конструктор

    public DBxDataVersionHandlerException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// Эта версия конструктора нужна для правильной десериализации
    /// </summary>
    protected DBxDataVersionHandlerException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    #endregion
  }
}
