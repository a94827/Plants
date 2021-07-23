using System;
using System.Collections.Generic;
using System.Text;
using AgeyevAV.ExtDB;
using System.Data;
using System.Runtime.Serialization;
using AgeyevAV;

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

    public DBxDataVersionHandler(Guid AppGuid, int CurrentVersion, int MinVersion)
    {
      if (AppGuid == Guid.Empty)
        throw new ArgumentException("Не задан AppGuid");
      if (CurrentVersion < 1)
        throw new ArgumentException("Версия должна быть не меньше 1", "CurrentVersion");
      if (MinVersion < 1)
        throw new ArgumentException("Минимальная версия должна быть не меньше 1", "MinVersion");
      if (MinVersion > CurrentVersion)
        throw new ArgumentException("Минимальная версия не может быть больше основной версии", "MinVersion");

      _AppGuid = AppGuid;
      _CurrentVersion = CurrentVersion;
      _MinVersion = CurrentVersion;
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

    public DBxTableStruct AddTableStruct(DBxStruct DBStruct)
    {
      DBxTableStruct ts = new DBxTableStruct("DataVersion");
      ts.Columns.AddId(); // не используется
      ts.Columns.AddString("AppGUID", 36, false);
      ts.Columns.AddString("DataGUID", 36, false);
      ts.Columns.AddInt("CurrentVersion", 1, Int32.MaxValue);
      ts.Columns.AddInt("MinVersion", 1, Int32.MaxValue);
      DBStruct.Tables.Add(ts);
      return ts;
    }

    #endregion

    #region Инициализация

    public Guid DataGuid { get { return FDataGuid; } }
    private Guid FDataGuid;

    public int PrevVersion { get { return FPrevVersion; } }
    private int FPrevVersion;

    public void InitTableRow(DBxCon Con)
    {
      DataTable Table = Con.FillSelect("DataVersion");
      if (Table.Rows.Count == 0)
      {
        // Первый запуск

        FDataGuid = Guid.NewGuid();
        Con.AddRecord("DataVersion", new DBxColumns("AppGUID,DataGUID,CurrentVersion,MinVersion"),
          new object[] { AppGuid.ToString(), FDataGuid.ToString(), CurrentVersion, MinVersion });
      }
      else if (Table.Rows.Count > 1)
        throw new DBxDataVersionHandlerException("Таблица DataVersion содержит недопустимое число строк: " + Table.Rows.Count.ToString());
      else
      {
        // Проверяем версию

        DataRow Row = Table.Rows[0];
        Guid OldAppGuid = new Guid(DataTools.GetString(Row, "AppGUID"));
        if (OldAppGuid != AppGuid)
          throw new DBxDataVersionHandlerException("База данных предназначена для работы с другой программой");

        FDataGuid = new Guid(DataTools.GetString(Row, "DataGUID"));
        FPrevVersion = DataTools.GetInt(Row, "CurrentVersion");

        int OldMinVersion = DataTools.GetInt(Row, "MinVersion");
        if (CurrentVersion < OldMinVersion)
          throw new DBxDataVersionHandlerException("База данных была обновлена в более новой версии программы. Откат до текущей версии невозможен");

        if (CurrentVersion != FPrevVersion)
        {
          Int32 DummyId = DataTools.GetInt(Row, "Id");
          Con.SetValues("DataVersion", DummyId, new DBxColumns("CurrentVesrion,MinVersion"),
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

    public DBxDataVersionHandlerException(string Message)
      : base(Message)
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
