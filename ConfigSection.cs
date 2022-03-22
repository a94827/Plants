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

    public ConfigSectionKey(string sectionName, string category, string configName)
    {
      if (String.IsNullOrEmpty(sectionName))
        throw new ArgumentNullException("SectionName");
      if (sectionName.Length > ConfigSection.MaxSectionNameLength)
        throw new ArgumentException("Слишком длинное название секции", "sectionName");
      _SectionName = sectionName;

      if (String.IsNullOrEmpty(category))
        throw new ArgumentNullException("Category");
      if (category.Length > ConfigSection.MaxCategoryLength)
        throw new ArgumentException("Слишком длинное название конфигурации", "category");
      _Category = category;

      if (configName == null)
        _ConfigName = String.Empty;
      else
      {
        if (configName.Length > ConfigSection.MaxConfigNameLength)
          throw new ArgumentException("Слишком длинное пользовательское название конфигурации", "configName");
        _ConfigName = configName;
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
      ConfigSectionKey key2 = obj as ConfigSectionKey;
      if (key2 == null)
        return false;
      return this == key2;
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

    public ConfigSection(ConfigSections owner, ConfigSectionKey key)
      : base(CfgConverter.Default)
    {
      if (owner == null)
        throw new ArgumentNullException("Owner");
      _Owner = owner;

      if (key == null)
        throw new ArgumentNullException("Key");
      _Key = key;
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

    protected abstract ConfigSection DoGetConfigSection(ConfigSectionKey key);

    protected abstract void DoWriteSect(ConfigSection sect);

    // protected abstract DataTable DoLoadTable(DBxFilter Filter);

    #endregion

    #region Базовое свойство доступа к секциям

    public ConfigSection this[string sectionName, string category]
    {
      get { return this[sectionName, category, String.Empty]; }
    }

    /// <summary>
    /// Основной метод доступа к секциям с заданием всех идентификаторов
    /// </summary>
    /// <param name="sectionName">Название секции</param>
    /// <param name="category">Категория</param>
    /// <param name="configName"></param>
    /// <returns>Секция конфигурации для доступа к значениям в XML-формате</returns>                                
    public ConfigSection this[string sectionName, string category, string configName]
    {
      get
      {
        ConfigSectionKey key = new ConfigSectionKey(sectionName, category, configName);

        return DoGetConfigSection(key);
      }
    }

    /// <summary>
    /// Этот метод реально выполняется при вызове ConfigSection.Save()
    /// </summary>
    internal void WriteSect(ConfigSection sect)
    {
      // Выполняем сравнение
      if (sect.AsXmlText == sect.OrgXmlText)
        return;
      DoWriteSect(sect);
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

    internal ClientConfigSections(ProgramDB db)
    {
      this._DB = db;
    }

    private ProgramDB _DB;

    #endregion

    #region Чтение и запись

    protected override ConfigSection DoGetConfigSection(ConfigSectionKey key)
    {
      string s = GetConfigSectionString(key);
      ConfigSection sect = new ConfigSection(this, key);
      sect.AsXmlText = s;
      return sect;
    }

    public string GetConfigSectionString(ConfigSectionKey key)
    {
      #region Загрузка из БД

      Int32 cfgSectId = GetCfgSectId(key);

      if (cfgSectId == 0)
        return String.Empty;
      else
      {
        using (DBxCon con = new DBxCon(_DB.MainEntry))
        {
          return DataTools.GetString(con.GetValue("UserSettings", cfgSectId, "Data"));
        }
      }

      #endregion
    }

    protected override void DoWriteSect(ConfigSection sect)
    {
      string s = sect.AsXmlText;
      WriteConfigSectionString(sect.Key, s);
    }

    public void WriteConfigSectionString(ConfigSectionKey key, string value)
    {
      if (value == null)
        value = String.Empty;

      using (DBxCon con = new DBxCon(_DB.MainEntry))
      {
        DoRealWrite(con, key, value);
      }

      // Очищаем загруженные таблицы пользователей, по которым были обновления
      // Очистка должна быть после записи всех секций, иначе будет многократная загрузка таблицы в кэш
      Cache.Clear<UserTableCache>(new string[] {  });
    }

    private bool DoRealWrite(DBxCon con, ConfigSectionKey key, string value)
    {
      if (String.IsNullOrEmpty(value))
        value = null;

      Int32 cfgSectId = GetCfgSectId(key);
      if (cfgSectId == 0)
      {
        //                                     0     1         2       3      4
        DBxColumns columns = new DBxColumns("Name,Category,ConfigName,Data,WriteTime");
        object[] values = new object[5];
        values[0] = key.SectionName;
        values[1] = key.Category;
        if (!String.IsNullOrEmpty(key.ConfigName))
          values[2] = key.ConfigName;
        values[3] = value;
        values[4] = DateTime.Now;
        con.AddRecord("UserSettings", columns, values);

        return true;
      }
      else
      {
        con.SetValues("UserSettings", cfgSectId, new DBxColumns("Data,WriteTime"), new object[] { value, DateTime.Now });
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

      public UserTableCache(DataTable table)
      {
        _Table = table;
      }

      #endregion

      #region Свойства

      public DataTable Table { get { return _Table; } }
      private DataTable _Table;

      #endregion
    }

    /// <summary>
    /// Возвращает таблицу секций для выбранного пользователя
    /// Таблица содержит все поля, кроме поля "Данные"
    /// </summary>
    /// <returns></returns>
    public DataTable GetUserTable()
    {
      DataTable table = Cache.GetItem<UserTableCache>(new string[] { "0"}, CachePersistance.MemoryAndTempDir, this).Table;

      // Свойство Table.DefaultView не поддерживает сериализацию
      // Поэтому, его нужно устанавливать здесь, а не в фабрике UserTableCache.
      // Можно было бы переделать десериализацию UserTableCache

      lock (table)
      {
        if (String.IsNullOrEmpty(table.DefaultView.Sort))
          table.DefaultView.Sort = "Name,Category,ConfigName";
      }

      return table;
    }

    #region ICacheFactory<UserTableCache> Members

    public ClientConfigSections.UserTableCache CreateCacheItem(string[] keys)
    {
      using (DBxCon con = new DBxCon(_DB.MainEntry))
      {
        DataTable table = con.FillSelect("UserSettings", new DBxColumns(
          "Id,Name,Category,ConfigName"),
          null, null);

        // Table.DefaultView.Sort = "Название,Категория,Компьютер,Конфигурация";
        return new UserTableCache(table);
      }
    }

    /// <summary>
    /// Возврашает идентификатор секции конфигурации в базе данных.
    /// Если такой секции нет, возвращает 0
    /// Использует буферизацию таблиц для пользователя
    /// </summary>
    private Int32 GetCfgSectId(ConfigSectionKey key)
    {
      DataTable userTable = GetUserTable();

      object[] searchKeys = new object[3];
      searchKeys[0] = key.SectionName;
      searchKeys[1] = key.Category;

      if (String.IsNullOrEmpty(key.ConfigName))
        searchKeys[2] = DBNull.Value;
      else
        searchKeys[2] = key.ConfigName;


      int p = userTable.DefaultView.Find(searchKeys);
      if (p >= 0)
        return DataTools.GetInt(userTable.DefaultView[p].Row, "Id");
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

      public SectDisposer(ConfigSection sect, bool write)
      {
        if (write)
          _Sect = sect;

#if DEBUG
        this._DebugString = sect.ToString() + (write ? " (Write)" : " (Read)");
        this._StackTrace = Environment.StackTrace;
#endif
      }

      protected override void Dispose(bool disposing)
      {
        if (disposing)
        {
          if (_Sect != null)
          {
            try
            {
              _Sect.Save();
            }
            catch (Exception e)
            {
#pragma warning disable 0162
              if (/*ProgramDBUI.TheCon.LogoutCalled*/false)
                // 07.12.2016
                EFPApp.ErrorMessageBox("Нельзя сохранить данные после вызова Logout", "Ошибка записи секции конфигурации");
              else
              {
                e.Data["ConfigSection"] = _Sect.ToString();
                e.Data["StackTrace"] = Environment.StackTrace;
                EFPApp.ShowException(e, "Ошибка записи секции конфигурации");
              }
            }
#pragma warning restore 0162

            _Sect = null;
          }
        }
        base.Dispose(disposing);
      }

      #endregion

      #region Поля

      private ConfigSection _Sect;

      #endregion

      #region Отладка

#if DEBUG

      // 02.06.2017
      // Отладка на случай невозможности записи секции

      private string _DebugString;

      public string StackTrace { get { return _StackTrace; } }
      private string _StackTrace;

      public override string ToString()
      {
        return _DebugString;
      }

      SectDisposer()
      {
        DisposableObject.RegisterLogoutType(typeof(SectDisposer));
      }

#endif

      #endregion
    }

    IDisposable IEFPConfigManager.GetConfig(EFPConfigSectionInfo sectionInfo, EFPConfigMode options, out FreeLibSet.Config.CfgPart cfg)
    {
      //bool ThisComputer = EFPConfigCategories.IsMachineDepended(SectionInfo.Category);

      ConfigSection sect = this[sectionInfo.ConfigSectionName, sectionInfo.Category, sectionInfo.UserSetName];
      cfg = sect;
      if (options == EFPConfigMode.Write)
        return new SectDisposer(sect, true);
      else
        return new SectDisposer(sect, false);
    }

    EFPConfigPersistence IEFPConfigManager.Persistence
    {
      get { return EFPConfigPersistence.Machine; }
    }

    void IEFPConfigManager.Preload(EFPConfigSectionInfo[] configInfos, EFPConfigMode mode)
    { 
    }

    #endregion
  }
}
