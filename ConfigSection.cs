using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Data;
using FreeLibSet.Config;
using FreeLibSet.Data;
using FreeLibSet.Remoting;
using FreeLibSet.Caching;
using FreeLibSet.Forms;
using FreeLibSet.Core;
namespace Plants
{

  /// <summary>
  /// Ключ для поиска секций конфигурации
  /// </summary>
  [Serializable]
  public class ConfigSectionKey
  {
    #region Конструкторы

    public ConfigSectionKey(string SectionName, string Category, string ConfigName)
    {
      if (String.IsNullOrEmpty(SectionName))
        throw new ArgumentNullException("SectionName");
      if (SectionName.Length > ConfigSection.MaxSectionNameLength)
        throw new ArgumentException("Слишком длинное название секции", "SectionName");
      _SectionName = SectionName;

      if (String.IsNullOrEmpty(Category))
        throw new ArgumentNullException("Category");
      if (Category.Length > ConfigSection.MaxCategoryLength)
        throw new ArgumentException("Слишком длинное название конфигурации", "Category");
      _Category = Category;

      if (ConfigName == null)
        _ConfigName = String.Empty;
      else
      {
        if (ConfigName.Length > ConfigSection.MaxConfigNameLength)
          throw new ArgumentException("Слишком длинное пользовательское название конфигурации", "ConfigName");
        _ConfigName = ConfigName;
      }
    }

    #endregion

    #region Свойства

    public string SectionName { get { return _SectionName; } }
    public string _SectionName;

    public string Category { get { return _Category; } }
    public string _Category;

    public string ConfigName { get { return _ConfigName; } }
    public string _ConfigName;

    #endregion

    #region Переопределенные методы

    public override string ToString()
    {
      string s = "\"" + SectionName + "\"-\"" + Category + "\"";
      if (!String.IsNullOrEmpty(ConfigName))
        s += "( конф. \"" + ConfigName + "\")";

      return s;
    }

    public override int GetHashCode()
    {
      return _SectionName.GetHashCode();
    }

    #endregion

    #region Операции сравнения

    // Оператор сравнения должен быть определен обязательно.
    // Иначе сравнение будет выполняться с помощью ReferenceEquals(), и в списк будет множество одинакровых секций

    public static bool operator ==(ConfigSectionKey a, ConfigSectionKey b)
    {
      bool aIsNull = Object.ReferenceEquals(a, null);
      bool bIsNull = Object.ReferenceEquals(b, null);
      if (aIsNull && bIsNull)
        return true;
      if (aIsNull || bIsNull)
        return false;


      return a.SectionName == b.SectionName &&
        a.Category == b.Category &&
        a.ConfigName == b.ConfigName;
    }

    public static bool operator !=(ConfigSectionKey a, ConfigSectionKey b)
    {
      return !(a == b);
    }

    public override bool Equals(object obj)
    {
      ConfigSectionKey Key2 = obj as ConfigSectionKey;
      if (Key2 == null)
        return false;
      return this == Key2;
    }

    #endregion
  }

  /// <summary>
  /// Секция конфигурация, сохраняемая как единое целое в формате XML
  /// Этот класс не является потокобезопасным
  /// </summary>
  public class ConfigSection : XmlCfgPart
  {
    #region Размеры полей

    /// <summary>
    /// Максимальная длина названия секции
    /// </summary>
    public const int MaxSectionNameLength = 64;

    /// <summary>
    /// Максимальная длина названия категории
    /// </summary>
    public const int MaxCategoryLength = 20;

    /// <summary>
    /// Максимальная длина имени конфигурации, которое может задать пользователь
    /// </summary>
    public const int MaxConfigNameLength = 30;

    #endregion

    #region Конструкторы

    public ConfigSection(ConfigSections Owner, ConfigSectionKey Key)
      : base(CfgConverter.Default)
    {
      if (Owner == null)
        throw new ArgumentNullException("Owner");
      _Owner = Owner;

      if (Key == null)
        throw new ArgumentNullException("Key");
      _Key = Key;
    }

    /// <summary>
    /// Обычно доступ к секции конфигурации получают из ConfigSections
    /// Этот конструктор предназначен для создания временных объектов, которые
    /// нельзя записать
    /// </summary>
    public ConfigSection()
      : base(CfgConverter.Default)
    {
    }

    #endregion

    #region Свойства, задаваемые в конструкторе

    /// <summary>
    /// Объект - владелец секций
    /// </summary>
    public ConfigSections Owner { get { return _Owner; } }
    private ConfigSections _Owner;

    /// <summary>
    /// Название секции
    /// </summary>
    public ConfigSectionKey Key { get { return _Key; } }
    private ConfigSectionKey _Key;

    /// <summary>
    /// Получение читаемого названия секции конфигурации, например, для текста заставки
    /// </summary>
    /// 
    public override string ToString()
    {
      if (Owner == null)
        return "Временный объект";
      else
        return Key.ToString();
    }


    #endregion

    #region Преобразование в XML

    /// <summary>
    /// Преобразование всего XML-документа в строку или из строки
    /// </summary>
    public string AsXmlText
    {
      get
      {
        if (IsEmpty)
          return String.Empty;
        StringWriter wr1 = new StringWriter();
        XmlTextWriter wr2 = new XmlTextWriter(wr1);
        Document.WriteTo(wr2);
        return wr1.ToString();
      }
      set
      {
        if (String.IsNullOrEmpty(value))
        {
          Clear();
          return;
        }
        StringReader rd1 = new StringReader(value);
        XmlTextReader rd2 = new XmlTextReader(rd1);
        Document.Load(rd2);
        XmlNode node2 = Document.SelectSingleNode("Config");
        if (node2 != null)
          RootNode = node2;
      }
    }

    /// <summary>
    /// Сюда копируем текст XML после считывания секции, чтобы перед записью
    /// можно было сравнить
    /// </summary>
    public string OrgXmlText;

    /// <summary>
    /// Требует обязательной записи секции на сервере, независимо от наличия изменений
    /// Вызывается при обнаружении ошибки чтения секции
    /// </summary>
    public void SetMustBeWriitenFlag()
    {
      OrgXmlText = "?";
    }


    #endregion

    #region Запись

    /// <summary>
    /// Сохранить изменения в секции
    /// </summary>
    public void Save()
    {
      if (Owner == null)
        throw new InvalidOperationException("Эта секции конфигурации не может быть записана, т.к. она получена не из ConfigSections");

      Owner.WriteSect(this);
      OrgXmlText = AsXmlText;
    }

    #endregion
  }

  /// <summary>
  /// Абстрактный базовый класс для ClientConfigSections
  /// </summary>
  public abstract class ConfigSections
  {
    #region Конструктор

    public ConfigSections()
    {
    }

    #endregion

    #region Абстрактные методы для чтения / записи

    protected abstract ConfigSection DoGetConfigSection(ConfigSectionKey Key);

    protected abstract void DoWriteSect(ConfigSection Sect);

    // protected abstract DataTable DoLoadTable(DBxFilter Filter);

    #endregion

    #region Базовое свойство доступа к секциям

    public ConfigSection this[string SectionName, string Category]
    {
      get { return this[SectionName, Category, String.Empty]; }
    }

    /// <summary>
    /// Основной метод доступа к секциям с заданием всех идентификаторов
    /// </summary>
    /// <param name="SectionName">Название секции</param>
    /// <param name="Category">Категория</param>
    /// <param name="UserId">Идентификатор пользователя или 0, если секция общая для всех пользователей</param>
    /// <param name="ComputerName">Имя компьютера, возврашщаемое ConfigSections.ComputerName или null, если секция не зависит от компьютера если секция не предусматривает нескольких конфигураций</param>
    /// <returns>Секция конфигурации для доступа к значениям в XML-формате</returns>                                
    public ConfigSection this[string SectionName, string Category, string ConfigName]
    {
      get
      {
        ConfigSectionKey Key = new ConfigSectionKey(SectionName, Category, ConfigName);

        return DoGetConfigSection(Key);
      }
    }

    /// <summary>
    /// Этот метод реально выполняется при вызове ConfigSection.Save()
    /// </summary>
    /// <param name="Sect"></param>
    internal void WriteSect(ConfigSection Sect)
    {
      // Выполняем сравнение
      if (Sect.AsXmlText == Sect.OrgXmlText)
        return;
      DoWriteSect(Sect);
    }

    #endregion

    #region Поиск секций

#if XXX
    /// <summary>
    /// Найти секции конфигураций для всех пользователей с заданным именем и категорией
    /// </summary>
    /// <param name="SectionName"></param>
    /// <param name="Category"></param>
    /// <returns></returns>
    public ConfigSection[] Find(string SectionName, string Category)
    {
      DBxFilter[] Filters = new DBxFilter[2];
      Filters[0] = new ValueFilter("Название", SectionName);
      Filters[1] = new ValueFilter("Категория", Category);
      return Find(new AndFilter(Filters));
    }

    private ConfigSection[] Find(DBxFilter Filter)
    {
      DataTable Table = DoLoadTable(Filter);
      ConfigSection[] a = new ConfigSection[Table.Rows.Count];
      for (int i = 0; i < a.Length; i++)
      {
        DataRow Row = Table.Rows[i];
        a[i] = this[DataTools.GetString(Row, "Название"),
          DataTools.GetString(Row, "Категория"),
          DataTools.GetInt(Row, "Пользователь"),
          DataTools.GetString(Row, "Компьютер"),
          DataTools.GetString(Row, "Конфигурация")];
      }

      return a;
    }
#endif

    #endregion
  }

  /// <summary>
  /// Чтение / запись секции конфигурации в БД вынесли в отдельный класс
  /// </summary>
  public class ClientConfigSections : ConfigSections, 
    ICacheFactory<ClientConfigSections.UserTableCache>,
    IEFPConfigManager
  {
    #region Конструктор

    internal ClientConfigSections(ProgramDB DB)
    {
      this._DB = DB;
    }

    private ProgramDB _DB;

    #endregion

    #region Чтение и запись

    protected override ConfigSection DoGetConfigSection(ConfigSectionKey Key)
    {
      string s = GetConfigSectionString(Key);
      ConfigSection Sect = new ConfigSection(this, Key);
      Sect.AsXmlText = s;
      return Sect;
    }

    public string GetConfigSectionString(ConfigSectionKey Key)
    {
      #region Загрузка из БД

      Int32 CfgSectId = GetCfgSectId(Key);

      if (CfgSectId == 0)
        return String.Empty;
      else
      {
        using (DBxCon Con = new DBxCon(_DB.MainEntry))
        {
          return DataTools.GetString(Con.GetValue("UserSettings", CfgSectId, "Data"));
        }
      }

      #endregion
    }

    protected override void DoWriteSect(ConfigSection Sect)
    {
      string s = Sect.AsXmlText;
      WriteConfigSectionString(Sect.Key, s);
    }

    public void WriteConfigSectionString(ConfigSectionKey Key, string Value)
    {
      if (Value == null)
        Value = String.Empty;

      using (DBxCon Con = new DBxCon(_DB.MainEntry))
      {
        DoRealWrite(Con, Key, Value);
      }

      // Очищаем загруженные таблицы пользователей, по которым были обновления
      // Очистка должна быть после записи всех секций, иначе будет многократная загрузка таблицы в кэш
      Cache.Clear<UserTableCache>(new string[] {  });
    }

    private bool DoRealWrite(DBxCon Con, ConfigSectionKey Key, string Value)
    {
      if (String.IsNullOrEmpty(Value))
        Value = null;

      Int32 CfgSectId = GetCfgSectId(Key);
      if (CfgSectId == 0)
      {
        //                                     0     1         2       3      4
        DBxColumns Columns = new DBxColumns("Name,Category,ConfigName,Data,WriteTime");
        object[] Values = new object[5];
        Values[0] = Key.SectionName;
        Values[1] = Key.Category;
        if (!String.IsNullOrEmpty(Key.ConfigName))
          Values[2] = Key.ConfigName;
        Values[3] = Value;
        Values[4] = DateTime.Now;
        Con.AddRecord("UserSettings", Columns, Values);

        return true;
      }
      else
      {
        Con.SetValues("UserSettings", CfgSectId, new DBxColumns("Data,WriteTime"), new object[] { Value, DateTime.Now });
        return false;
      }
    }

    #endregion

    #region Буферизация списка секций

    /*
     * Крайне не выгодно при каждом запросе выполнять FindRecord() для поиска существующей секции
     * Используем систему кэширования, которая хранит объект DataTable для каждого пользователя
     */

    /// <summary>
    /// Нельзя использовать готовый класс DataTable для кэширования в системе Cache
    /// </summary>
    [Serializable]
    public class UserTableCache
    {
      #region Конструктор

      public UserTableCache(DataTable Table)
      {
        FTable = Table;
      }

      #endregion

      #region Свойства

      public DataTable Table { get { return FTable; } }
      private DataTable FTable;

      #endregion
    }

    /// <summary>
    /// Возвращает таблицу секций для выбранного пользователя
    /// Таблица содержит все поля, кроме поля "Данные"
    /// </summary>
    /// <param name="UserId"></param>
    /// <returns></returns>
    public DataTable GetUserTable()
    {
      DataTable Table = Cache.GetItem<UserTableCache>(new string[] { "0"}, CachePersistance.MemoryAndTempDir, this).Table;

      // Свойство Table.DefaultView не поддерживает сериализацию
      // Поэтому, его нужно устанавливать здесь, а не в фабрике UserTableCache.
      // Можно было бы переделать десериализацию UserTableCache

      lock (Table)
      {
        if (String.IsNullOrEmpty(Table.DefaultView.Sort))
          Table.DefaultView.Sort = "Name,Category,ConfigName";
      }

      return Table;
    }

    #region ICacheFactory<UserTableCache> Members

    public ClientConfigSections.UserTableCache CreateCacheItem(string[] Keys)
    {
      using (DBxCon Con = new DBxCon(_DB.MainEntry))
      {
        DataTable Table = Con.FillSelect("UserSettings", new DBxColumns(
          "Id,Name,Category,ConfigName"),
          null, null);

        // Table.DefaultView.Sort = "Название,Категория,Компьютер,Конфигурация";
        return new UserTableCache(Table);
      }
    }

    /// <summary>
    /// Возврашает идентификатор секции конфигурации в базе данных.
    /// Если такой секции нет, возвращает 0
    /// Использует буферизацию таблиц для пользователя
    /// </summary>
    private Int32 GetCfgSectId(ConfigSectionKey Key)
    {
      DataTable UserTable = GetUserTable();

      object[] Keys = new object[3];
      Keys[0] = Key.SectionName;
      Keys[1] = Key.Category;

      if (String.IsNullOrEmpty(Key.ConfigName))
        Keys[2] = DBNull.Value;
      else
        Keys[2] = Key.ConfigName;


      int p = UserTable.DefaultView.Find(Keys);
      if (p >= 0)
        return DataTools.GetInt(UserTable.DefaultView[p].Row, "Id");
      else
        return 0;
    }

    public void ClearCache()
    {
      Cache.Clear<UserTableCache>(null);
    }

    #endregion

    #endregion

    #region IEFPConfigManager Members

    private class SectDisposer : DisposableObject
    {
      #region Конструктор и Dispose

      public SectDisposer(ConfigSection Sect, bool Write)
      {
        if (Write)
          FSect = Sect;

#if DEBUG
        this.DebugString = Sect.ToString() + (Write ? " (Write)" : " (Read)");
        this.FStackTrace = Environment.StackTrace;
#endif
      }

      protected override void Dispose(bool Disposing)
      {
        if (Disposing)
        {
          if (FSect != null)
          {
            try
            {
              FSect.Save();
            }
            catch (Exception e)
            {
#pragma warning disable 0162
              if (/*ProgramDBUI.TheCon.LogoutCalled*/false)
                // 07.12.2016
                EFPApp.ErrorMessageBox("Нельзя сохранить данные после вызова Logout", "Ошибка записи секции конфигурации");
              else
              {
                e.Data["ConfigSection"] = FSect.ToString();
                e.Data["StackTrace"] = Environment.StackTrace;
                EFPApp.ShowException(e, "Ошибка записи секции конфигурации");
              }
            }
#pragma warning restore 0162

            FSect = null;
          }
        }
        base.Dispose(Disposing);
      }

      #endregion

      #region Поля

      private ConfigSection FSect;

      #endregion

      #region Отладка

#if DEBUG

      // 02.06.2017
      // Отладка на случай невозможности записи секции

      private string DebugString;

      public string StackTrace { get { return FStackTrace; } }
      private string FStackTrace;

      public override string ToString()
      {
        return DebugString;
      }

      SectDisposer()
      {
        DisposableObject.RegisterLogoutType(typeof(SectDisposer));
      }

#endif

      #endregion
    }

    IDisposable IEFPConfigManager.GetConfig(EFPConfigSectionInfo SectionInfo, EFPConfigMode Options, out FreeLibSet.Config.CfgPart Cfg)
    {
      //bool ThisComputer = EFPConfigCategories.IsMachineDepended(SectionInfo.Category);

      ConfigSection Sect = this[SectionInfo.ConfigSectionName, SectionInfo.Category, SectionInfo.UserSetName];
      Cfg = Sect;
      if (Options == EFPConfigMode.Write)
        return new SectDisposer(Sect, true);
      else
        return new SectDisposer(Sect, false);
    }

    EFPConfigPersistence IEFPConfigManager.Persistence
    {
      get { return EFPConfigPersistence.Machine; }
    }

    void IEFPConfigManager.Preload(EFPConfigSectionInfo[] ConfigInfos, EFPConfigMode Mode)
    { 
    }

    #endregion
  }
}
