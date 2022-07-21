using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FreeLibSet.Forms.Docs;
using FreeLibSet.Forms;
using FreeLibSet.Data.Docs;
using FreeLibSet.Data;
using FreeLibSet.Formatting;
using FreeLibSet.Core;
using FreeLibSet.UICore;

namespace Plants
{
  internal partial class AttrTableViewForm : Form
  {
    #region Конструктор формы

    public AttrTableViewForm(Int32[] docIds)
    {
      InitializeComponent();

      Text = "Атрибуты";
      Icon = EFPApp.MainImages.Icons["AttributeTable"];

      DataTable table = ProgramDBUI.TheUI.DocTypes["AttrTypes"].GetUnbufferedData(DBSDocType.IdColumns, null, false, new DBxOrder("Name"));
      _AttrTypeDocs = new AttrTypeDoc[table.Rows.Count];
      for (int i = 0; i < table.Rows.Count; i++)
      {
        Int32 id = DataTools.GetInt(table.Rows[i], "Id");
        AttrTypeDoc attr = new AttrTypeDoc(id);
        _AttrTypeDocs[i] = attr;
      }

      this._MainDocIds = docIds;


      _DocTypeUI = ProgramDBUI.TheUI.DocTypes["Plants"];

      #region Создание таблицы

      table = new DataTable();
      table.Columns.Add("Id", typeof(Int32)); // Идентификатор основного документа (учреждения)
      table.Columns.Add("NPop", typeof(int)); // Исходный порядок документов 
      table.Columns.Add("Number", typeof(int));
      table.Columns.Add("Name", typeof(string));

      for (int i = 0; i < _AttrTypeDocs.Length; i++)
        table.Columns.Add("Attr" + _AttrTypeDocs[i].Id.ToString(), PlantTools.ValueTypeToType(_AttrTypeDocs[i].ValueType));

      for (int i = 0; i < docIds.Length; i++)
      {
        DataRow ResRow = table.NewRow();
        ResRow["Id"] = docIds[i];
        ResRow["NPop"] = i + 1;
        table.Rows.Add(ResRow);
      }

      #endregion

      EFPFormProvider efpForm = new EFPFormProvider(this);

      #region Поле даты

      efpDate = new EFPDateTimeBox(efpForm, edDate);
      efpDate.ToolTipText = "Дата, на которую выведены значения атрибутов";
      efpDate.CanBeEmpty = false;
      efpDate.Value = _LastDate;
      efpDate.ValueEx.ValueChanged += new EventHandler(efpDate_ValueChanged);

      #endregion

      #region Табличный просмотр

      #region GridProducer

      EFPGridProducer producer = new EFPGridProducer();
      producer.NewDefaultConfig(false);

      producer.Columns.AddInt("NPop", "№ п/п", 3);
      producer.Columns.LastAdded.CanIncSearch = true;
      producer.DefaultConfig.Columns.Add("NPop");
      producer.Orders.Add("NPop", "В исходном порядке");
      producer.FixedColumns.Add("NPop"); // Иначе не работает сортировка по атрибутам без наличия столбца "№ п/п"

      producer.Columns.AddUserImage("Image", "Id",
        new EFPGridProducerValueNeededEventHandler(Image_ColumnValueNeeded), String.Empty);
      producer.DefaultConfig.Columns.Add("Image");

      // Основные столбцы
      producer.Columns.AddText("Number", "Номер по каталогу", 3, 3);
      producer.Columns.LastAdded.Format = ProgramDBUI.Settings.NumberMask;
      producer.Columns.LastAdded.CanIncSearch = true;
      producer.DefaultConfig.Columns.Add("Number");

      producer.Columns.AddText("Name", "Название", 20, 5);
      producer.Columns.LastAdded.CanIncSearch = true;
      producer.DefaultConfig.Columns.Add("Name");

      producer.Orders.Add("Number", "Номер по каталогу");
      producer.Orders.Add("Name", "Название");

      for (int i = 0; i < _AttrTypeDocs.Length; i++)
      {
        string colName = "Attr" + _AttrTypeDocs[i].Id.ToString();
        ProgramDBUI.AddValueTypeColumn(producer, _AttrTypeDocs[i].ValueType, colName, _AttrTypeDocs[i].Name);
        producer.Columns.LastAdded.DisplayName = _AttrTypeDocs[i].Name;
        if (!String.IsNullOrEmpty(_AttrTypeDocs[i].Comment))
          producer.Columns.LastAdded.DisplayName += " (" + _AttrTypeDocs[i].Comment + ")";
        producer.Columns.LastAdded.HeaderToolTipText = "Тип: " + PlantTools.GetValueTypeName(_AttrTypeDocs[i].ValueType) +
          Environment.NewLine + _AttrTypeDocs[i].Comment;
        producer.DefaultConfig.Columns.Add(colName);

        switch (_AttrTypeDocs[i].ValueType)
        {
          case ValueType.Date:
          case ValueType.DateTime:
            producer.Orders.Add(colName + ",NPop", _AttrTypeDocs[i].Name + " (по возрастанию)", new EFPDataGridViewSortInfo(colName, ListSortDirection.Ascending));
            producer.Orders.Add(colName + " DESC,NPop", _AttrTypeDocs[i].Name + " (по убыванию)", new EFPDataGridViewSortInfo(colName.ToString(), ListSortDirection.Descending));
            break;
          case ValueType.Boolean:
            break;
          default:
            producer.Orders.Add(colName + ",NPop", _AttrTypeDocs[i].Name, new EFPDataGridViewSortInfo(colName, ListSortDirection.Ascending));
            break;
        }
      }

      producer.DefaultConfig.FrozenColumns = 3;

      #region Именные конфигурации

      // Для каждого вида атрибута добавляем именную настройку с единственным столбцом
      for (int i = 0; i < _AttrTypeDocs.Length; i++)
      {
        EFPDataGridViewConfig config = producer.NewNamedConfig(_AttrTypeDocs[i].Name);
        config.ImageKey = "AttributeType";
        string mainColName = "Attr" + _AttrTypeDocs[i].Id.ToString();
        for (int j = 0; j < producer.DefaultConfig.Columns.Count; j++)
        {
          if (producer.DefaultConfig.Columns[j].ColumnName.StartsWith("Attr"))
          {
            if (producer.DefaultConfig.Columns[j].ColumnName != mainColName)
              continue; // оставляем только свой столбец
          }
          config.Columns.Add(producer.DefaultConfig.Columns[j].Clone());
        }
      }

      #endregion

      #endregion

      gh = new EFPDBxGridView(efpForm, TheGrid, ProgramDBUI.TheUI);
      gh.Control.AutoGenerateColumns = false;
      gh.GridProducer = producer;
      gh.ConfigSectionName = "Plants-AttrTable";


      //gh.SetColumnsReadOnly(true);

      gh.DisableOrdering();
      gh.GetCellAttributes += new EFPDataGridViewCellAttributesEventHandler(gh_GetCellAttributes);
      gh.CellFinished += new EFPDataGridViewCellFinishedEventHandler(gh_CellFinished);

      gh.AutoSort = true;

      gh.Control.MultiSelect = true;

      gh.ReadOnly = false;
      gh.Control.ReadOnly = false;
      gh.CanInsert = false;
      gh.CanDelete = false;
      gh.CanView = true;
      gh.CanMultiEdit = true; // даже если документы можно редактировать только по одному
      gh.EditData += new EventHandler(gh_EditData);
      gh.GetDocSel += new EFPDBxGridViewDocSelEventHandler(gh_GetDocSel);

      gh.CommandItems.ClipboardInToolBar = true;
      gh.ToolBarPanel = PanSpb;

      gh.Control.DataSource = table.DefaultView;
      gh.RefreshData += new EventHandler(gh_RefreshData); // 27.03.2017

      #endregion

      #region Редактирование по месту

      efpEnableInPlace = new EFPCheckBox(efpForm, cbEnableInPlace);
      efpStartDate = new EFPDateTimeBox(efpForm, edStartDate);
      efpStartDate.DisplayName = "Дата атрибута";
      efpStartDate.ToolTipText = "Дата начала действия атрибута, которая будет использована при редактировании атрибута \"по месту\"";
      efpStartDate.CanBeEmptyMode = UIValidateState.Warning;
      efpStartDate.EnabledEx = efpEnableInPlace.CheckedEx;
      efpStartDate.NValue = EditAttrValueHelper.LastDate;
      efpStartDate.NValueEx.ValueChanged += new EventHandler(efpStartDate_NValueChanged);
      efpEnableInPlace.CheckedEx.ValueChanged += new EventHandler(efpEnableInPlace_ValueChanged);

      btnSetStartDate.Image = EFPApp.MainImages.Images["ArrowDownThenLeft"];
      btnSetStartDate.ImageAlign = ContentAlignment.MiddleCenter;
      EFPButton efpSetStartDate = new EFPButton(efpForm, btnSetStartDate);
      efpSetStartDate.DisplayName = "Установка даты атрибута";
      efpSetStartDate.ToolTipText = "Копирует дату, на которые выведены атрибуты (вверху) в поле задаваемой даты начала действия атрибута (внизу)";
      efpSetStartDate.EnabledEx = efpStartDate.EnabledEx;
      efpSetStartDate.Click += new EventHandler(efpSetStartDate_Click);

      #endregion

      FillTable(table);

      TheGrid.Select();
    }

    protected override void OnLoad(EventArgs args)
    {
      base.OnLoad(args);

      // Отключаем редактирование по месту после загрузки формы, чтобы команды вставки из буфера обмена
      // успели инициализироваться

      efpEnableInPlace.Checked = false;
      efpEnableInPlace_ValueChanged(null, null);
    }

    #endregion

    #region Поля

    /// <summary>
    /// Документы "Виды атрибутов"
    /// </summary>
    AttrTypeDoc[] _AttrTypeDocs;

    /// <summary>
    /// Идентификаторы документов
    /// </summary>
    Int32[] _MainDocIds;

    DocTypeUI _DocTypeUI;

    EFPDateTimeBox efpDate;

    private static DateTime _LastDate = DateTime.Today;

    EFPDBxGridView gh;

    EFPCheckBox efpEnableInPlace;

    EFPDateTimeBox efpStartDate;

    #endregion

    #region Поле даты

    void efpDate_ValueChanged(object sender, EventArgs args)
    {
      if (efpDate.NValue.HasValue)
      {
        _LastDate = efpDate.NValue.Value;
        FillTable(gh.SourceAsDataTable);
      }
    }

    #endregion

    #region Табличный просмотр

    private void Image_ColumnValueNeeded(object sender, EFPGridProducerValueNeededEventArgs args)
    {
      Int32 docId = args.GetInt("Id");
      args.Value = _DocTypeUI.GetImageValue(docId);
    }

    void gh_GetCellAttributes(object sender, EFPDataGridViewCellAttributesEventArgs args)
    {
      if (args.DataRow == null)
        return;
      if (args.Reason == EFPDataGridViewAttributesReason.ToolTip)
      {
        if (args.ColumnName == "Image")
        {
          Int32 docId = DataTools.GetInt(args.DataRow, "Id");
          args.ToolTipText = _DocTypeUI.GetToolTipText(docId);
        }
      }
      else if (args.Reason == EFPDataGridViewAttributesReason.ReadOnly)
      {
        args.ReadOnly = !args.ColumnName.StartsWith("Attr");

        if (args.ReadOnly)
          args.ReadOnlyMessage = "Редактировать можно только значения атрибутов";
      }

      if (args.ColumnName.StartsWith("Attr"))
      {
        string txt = args.ColumnName.Substring(4);
        Int32 attrTypeId = Int32.Parse(txt);
        AttrTypeDoc attrDoc = new AttrTypeDoc(attrTypeId);
        object value = args.DataRow[args.ColumnName];
        string errorText;
        if (!attrDoc.TestValue(value, out errorText))
        {
          args.ColorType = EFPDataGridViewColorType.Warning;
          args.ToolTipText += Environment.NewLine + errorText;
        }
      }
    }

    void gh_EditData(object Sender, EventArgs Args)
    {
      Int32[] selDocIds = gh.SelectedIds;
      if (selDocIds.Length == 0)
      {
        EFPApp.ShowTempMessage("Нет выбранных документов");
        return;
      }

      if (gh.State == EFPDataGridViewState.Edit)
      {
        // Редактирование атрибутов
        if (gh.CurrentColumnName.StartsWith("Attr"))
        {
          string txt = gh.CurrentColumnName.Substring(4);
          Int32 attrTypeId = Int32.Parse(txt);
          DoEditAttrs(attrTypeId, selDocIds);
          return;
        }
      }

      // Обычное редактирование

      if (selDocIds.Length > 0 && (!_DocTypeUI.CanMultiEdit))
      {
        EFPApp.ShowTempMessage("Групповое редактирование документов \"" + _DocTypeUI.DocType.PluralTitle + "\" не допускается");
        return;
      }

      if (_DocTypeUI.PerformEditing(selDocIds, gh.State, true, null))
        FillRows(gh.SelectedDataRows);
    }

    private void DoEditAttrs(Int32 attrTypeId, Int32[] selDocIds)
    {
      if (!EditAttrValueGroup.PerformEdit(_DocTypeUI, selDocIds, attrTypeId))
        return;

      DataRow[] rows = gh.SelectedDataRows;
      FillRows(rows);
    }

    void gh_GetDocSel(object sender, EFPDBxGridViewDocSelEventArgs args)
    {
      args.AddFromColumn(_DocTypeUI.DocType.Name, "Id");
    }

    void gh_RefreshData(object sender, EventArgs args)
    {
      FillTable(gh.SourceAsDataTable);
    }

    #endregion

    #region Заполнение таблицы

    private void FillTable(DataTable table)
    {
      DataRow[] rows = new DataRow[table.Rows.Count];
      table.Rows.CopyTo(rows, 0);
      FillRows(rows);
    }

    private void FillRows(DataRow[] rows)
    {
      Int32[] docIds = DataTools.GetIds(rows);
      if (docIds.Length == 0)
        return;

      DBxDocSet docSet = new DBxDocSet(ProgramDBUI.TheUI.DocProvider);
      DBxMultiDocs mDocs = docSet[_DocTypeUI.DocType.Name];
      mDocs.View(docIds);

      foreach (DataRow docRow in rows)
      {
        Int32 docId = DataTools.GetInt(docRow, "Id");
        DBxSingleDoc doc = mDocs.GetDocById(docId);
        FillOneRow(docRow, doc);
      }
    }

    private void FillOneRow(DataRow docRow, DBxSingleDoc doc)
    {
      docRow["Number"] = doc.Values["Number"].AsInteger;
      docRow["Name"] = doc.Values["Name"].AsString;

      DataTable subTable = doc.SubDocs["PlantAttributes"].CreateSubDocsData();
      subTable.DefaultView.Sort = "Date";
      for (int i = 0; i < _AttrTypeDocs.Length; i++)
      {
        docRow["Attr" + _AttrTypeDocs[i].Id.ToString()] = DBNull.Value;

        subTable.DefaultView.RowFilter = "AttrType=" + _AttrTypeDocs[i].Id;
        for (int j = subTable.DefaultView.Count - 1; j >= 0; j--)
        {
          DataRow valueRow = subTable.DefaultView[j].Row;
          DateTime? dt = DataTools.GetNullableDateTime(valueRow, "Date");
          if (dt.HasValue)
          {
            if (dt.Value > efpDate.Value)
              continue;
          }

          string s = DataTools.GetString(valueRow, "LongValue");
          if (String.IsNullOrEmpty(s))
            s = DataTools.GetString(valueRow, "Value");
          object v = PlantTools.ValueFromSaveableString(s, _AttrTypeDocs[i].ValueType);
          if (v != null)
            docRow["Attr" + _AttrTypeDocs[i].Id.ToString()] = v;
          break;
        }
      }
    }

    /// <summary>
    /// Обновление одной строк после редактирование
    /// </summary>
    /// <param name="docRow"></param>
    private void FillOneRow(DataRow docRow)
    {
      Int32 docId = DataTools.GetInt(docRow, "Id");
      DBxDocSet docSet = new DBxDocSet(ProgramDBUI.TheUI.DocProvider);
      DBxMultiDocs mDocs = docSet[_DocTypeUI.DocType.Name];
      mDocs.View(docId);
      FillOneRow(docRow, mDocs[0]);
    }

    #endregion

    #region Редактирование по месту

    void efpEnableInPlace_ValueChanged(object sender, EventArgs args)
    {
      gh.Control.ReadOnly = !efpEnableInPlace.Checked;
    }

    void efpStartDate_NValueChanged(object sender, EventArgs args)
    {
      EditAttrValueHelper.LastDate = efpStartDate.NValue;
    }

    void gh_CellFinished(object sender, EFPDataGridViewCellFinishedEventArgs args)
    {
      if (!efpEnableInPlace.Checked)
        throw new InvalidOperationException("Редактирование по месту запрещено");

      if (!args.ColumnName.StartsWith("Attr"))
        throw new InvalidOperationException("Столбец не относится к атрибутам");

      string txt = gh.CurrentColumnName.Substring(4);
      Int32 attrTypeId = Int32.Parse(txt);

      Int32 docId = DataTools.GetInt(args.DataRow, "Id");
      if (args.Cell.Value == null || (args.Cell.Value is DBNull))
        EditAttrValueHelper.ClearValue(_DocTypeUI.DocType.Name, docId, attrTypeId, efpStartDate.Value);
      else
        EditAttrValueHelper.SetValue(docId, attrTypeId, efpStartDate.Value, args.Cell.Value);

      FillOneRow(args.DataRow); // Перечитываем значение
    }

    void efpSetStartDate_Click(object sender, EventArgs args)
    {
      efpStartDate.Value = efpDate.Value;
    }

    #endregion
  }

  /// <summary>
  /// Документ "Вид атрибута" загруженный в память
  /// </summary>
  public sealed class AttrTypeDoc
  {
    #region Конструктор

    public AttrTypeDoc(Int32 id)
    {
      object[] a = ProgramDBUI.TheUI.DocTypes["AttrTypes"].TableCache.GetValues(id,
        new DBxColumns("Name,Type,Comment,Format,Source"));
      this._Id = id;
      this._Name = DataTools.GetString(a[0]);
      this._ValueType = (ValueType)(DataTools.GetInt(a[1]));
      this._Comment = DataTools.GetString(a[2]);
      if (ValueType == ValueType.String)
        this._Format = DataTools.GetString(a[3]);
      else
        this._Format = String.Empty;

      this._SourceType = (AttrValueSourceType)(DataTools.GetInt(a[4]));
    }

    #endregion

    #region Основные свойства

    public Int32 Id { get { return _Id; } }
    private Int32 _Id;

    public string Name { get { return _Name; } }
    private string _Name;

    public ValueType ValueType { get { return _ValueType; } }
    private ValueType _ValueType;

    public string Comment { get { return _Comment; } }
    private string _Comment;

    public override string ToString()
    {
      return "Id=" + Id.ToString() + ", Name=" + Name;
    }

    #endregion

    #region Источник значений

    /// <summary>
    /// Тип источника значений
    /// </summary>
    public AttrValueSourceType SourceType { get { return _SourceType; } }
    private AttrValueSourceType _SourceType;


    /// <summary>
    /// Список значений в "хранимом" формате.
    /// Могут быть значения, которые не преобразуются в ValueType.
    /// Если источник данных SourceType указывает на справочник "Код-Значение", то список 
    /// содержит значения.
    /// Для логического типа содержит список "0", "1"
    /// </summary>
    public string[] StoreValueList
    {
      get
      {
        if (_StoreValueList == null)
          _StoreValueList = GetStoreValueList();
        return _StoreValueList;
      }
    }
    private string[] _StoreValueList;

    private static readonly string[] _BoolStoreValueList = new string[] { "0", "1" };

    private string[] GetStoreValueList()
    {
      if (ValueType == ValueType.Boolean)
        return _BoolStoreValueList;

      switch (SourceType)
      {
        case AttrValueSourceType.List:
          string s = ProgramDBUI.TheUI.DocTypes["AttrTypes"].TableCache.GetString(Id, "ValueList");
          return s.Split(DataTools.CRLFSeparators, // независимо от ОС 
            StringSplitOptions.RemoveEmptyEntries);
        default:
          return DataTools.EmptyStrings;
      }
    }

    /// <summary>
    /// Список для выбора значений.
    /// Все значения имеют тип, соответствуюший ValueType. Если в списке StoreValueList есть
    /// непреобразуеиые значения, они не включаются в этот список.
    /// </summary>
    public object[] ValueList
    {
      get
      {
        if (_ValueList == null)
          _ValueList = GetValueList();
        return _ValueList;
      }
    }
    private object[] _ValueList;

    private object[] GetValueList()
    {
      string[] a = StoreValueList;
      if (a.Length == 0)
        return DataTools.EmptyObjects;

      List<object> lst = new List<object>();
      for (int i = 0; i < a.Length; i++)
      {
        string s = a[i].Trim();
        if (s.Length == 0)
          continue;
        object x;
        if (PlantTools.TryParse(s, ValueType, out x))
          lst.Add(x);
      }
      return lst.ToArray();
    }

    #endregion

    #region Формат значения

    /// <summary>
    /// Формат значения (используется только для строковых атрибутов)
    /// </summary>
    public string Format { get { return _Format; } }
    private string _Format;


    /// <summary>
    /// Маска для значения.
    /// Возвращает null, если свойство Format содержит пустую строку или тип данных не является String
    /// </summary>
    public IMaskProvider Mask
    {
      get
      {
        if (!_MaskDefined)
        {
          if (String.IsNullOrEmpty(Format))
            _Mask = null;
          else
            _Mask = new StdMaskProvider(Format);
          _MaskDefined = true;
        }
        return _Mask;
      }
    }
    private IMaskProvider _Mask;
    private bool _MaskDefined;

    #endregion

    #region Проверка значения

    /// <summary>
    /// Проверка корректности значения атрибута.
    /// Null и значение по умолчанию (0) считаются корректными значениями.
    /// </summary>
    /// <param name="value">Проверяемое значение</param>
    /// <param name="errorText">Сюда помещается сообщение об ошибке или null, если ошибки нет</param>
    /// <returns>true, если значение является корректным</returns>
    public bool TestValue(object value, out string errorText)
    {
      errorText = null;
      if (value == null)
        return true;

      try
      {
        if (PlantTools.IsDefaultValue(value, this.ValueType))
          return true;

        if (this.Mask != null)
        {
          if (!this.Mask.Test(value.ToString(), out errorText))
            return false;
        }

        if (this.ValueList.Length > 0)
        {

          if (Array.IndexOf<object>(this.ValueList, value) < 0)
          {
            errorText = "Значения \"" + PlantTools.ToString(value, this.ValueType) + "\" нет в списке допустимых для атрибута \"" + this.Name + "\"";
            return false;
          }
        }

        return true;
      }
      catch (Exception e)
      {
        errorText = "Исключение при проверке значения атрибута \"" + this.Name + "\" " + e.Message;
        return false;
      }
    }

    /// <summary>
    /// Проверка корректности значения атрибута.
    /// Null и значение по умолчанию (0) считаются корректными значениями.
    /// </summary>
    /// <param name="value">Проверяемое значение</param>
    /// <returns>true, если значение является корректным</returns>
    public bool TestValue(object value)
    {
      string errorText;
      return TestValue(value, out errorText);
    }

    #endregion
  }

  /// <summary>
  /// Статические методы и поля, используемые при редактировании и вставке значений атрибутов
  /// </summary>
  public static class EditAttrValueHelper
  {
    /// <summary>
    /// Дата начала действия атрибута, сохраняемая между вызовами
    /// </summary>
    public static DateTime? LastDate = null;

    #region Вставка из буфера обмена

    /// <summary>
    /// Проверка текста атрибута и преобразование его в значение
    /// </summary>
    public static void ValidateAttributeText(AttrTypeDoc attrType, UISelRCValidatingEventArgs args)
    {
      if (String.IsNullOrEmpty(args.SourceText))
      {
        args.ResultValue = null;
        return;
      }

      object v;
      if (PlantTools.TryParse(args.SourceText, attrType.ValueType, out v))
        args.ResultValue = v;
      else
        args.SetError("Нельзя преобразовать в \"" + PlantTools.GetValueTypeName(attrType.ValueType) + "\"");
    }

    public static void AddAtrSubDoc(DBxSingleDoc doc, AttrTypeDoc attrType, object value, DateTime? date)
    {
      if (PlantTools.IsDefaultValue(value, attrType.ValueType))
        // Значение по умолчанию добавлять не будем
        return;

      DBxSubDoc subDoc = doc.SubDocs["PlantAttributes"].Insert();
      subDoc.Values["AttrType"].SetInteger(attrType.Id);
      subDoc.Values["Date"].SetNullableDateTime(date);
      string s = PlantTools.ValueToSaveableString(value, attrType.ValueType);
      if (s.Length <= PlantTools.AttrValueShortMaxLength)
        subDoc.Values["Value"].SetString(s);
      else
        subDoc.Values["LongValue"].SetString(s);
    }

    #endregion

    #region Автономная установка значений

    public static void SplitAttrValue(string sValue, out string sValue1, out string sValue2)
    {
      if (String.IsNullOrEmpty(sValue))
      {
        sValue1 = String.Empty;
        sValue2 = String.Empty;
      }
      else if (sValue.Length <= PlantTools.AttrValueShortMaxLength)
      {
        sValue1 = sValue;
        sValue2 = String.Empty;
      }
      else
      {
        sValue1 = String.Empty;
        sValue2 = sValue;
      }
    }


    /// <summary>
    /// Удаляет для документа значение атрибута, если оно существует
    /// </summary>
    public static void ClearValue(string docTypeName, Int32 docId, Int32 attrTypeId, DateTime? date)
    {
      AttrTypeDoc attrType = new AttrTypeDoc(attrTypeId);
      DBxDocSet docSet = new DBxDocSet(ProgramDBUI.TheUI.DocProvider);
      DBxSingleDoc doc = docSet[docTypeName].Edit(docId);
      DBxSubDoc subDoc;
      if (FindSubDoc(doc, attrTypeId, date, out subDoc))
      {
        docSet.ActionInfo = "Удаление атрибута \"" + attrType.Name + "\"";
        subDoc.Delete();
        docSet.ApplyChanges(false);
      }
    }

    public static void SetValue(Int32 docId, Int32 attrTypeId, DateTime? date, object value)
    {
      AttrTypeDoc attrType = new AttrTypeDoc(attrTypeId);
      string newSValue = PlantTools.ValueToSaveableString(value, attrType.ValueType);
      string newSValue1, newSValue2;
      SplitAttrValue(newSValue, out newSValue1, out newSValue2);


      DBxDocSet docSet = new DBxDocSet(ProgramDBUI.TheUI.DocProvider);
      DBxSingleDoc doc = docSet["Plants"].Edit(docId);

      DBxSubDoc subDoc;
      if (FindSubDoc(doc, attrTypeId, date, out subDoc))
      {
        string oldSValue1 = subDoc.Values["Value"].AsString;
        string oldSValue2 = subDoc.Values["LongValue"].AsString;
        if (newSValue1 == oldSValue1 && newSValue2 == oldSValue2)
          return; // Ничего менять не надо

        docSet.ActionInfo = "Изменение атрибута \"" + attrType.Name + "\"";
        subDoc.Values["Value"].SetString(newSValue1);
        subDoc.Values["LongValue"].SetString(newSValue2);
      }
      else
      {
        // Нет такого атрибута
        // Добавление пустого значения не эквивалентно отсутствию атрибута вообще, если задана дата атрибута
        if ((!date.HasValue) && PlantTools.IsDefaultValue(value, attrType.ValueType))
          return;

        docSet.ActionInfo = "Добавление атрибута \"" + attrType.Name + "\"";

        subDoc = doc.SubDocs["PlantAttributes"].Insert();
        subDoc.Values["AttrType"].SetInteger(attrType.Id);
        subDoc.Values["Date"].SetNullableDateTime(date);
        subDoc.Values["Value"].SetString(newSValue1);
        subDoc.Values["LongValue"].SetString(newSValue2);
      }

      docSet.ApplyChanges(false);
    }

    private static bool FindSubDoc(DBxSingleDoc doc, Int32 attrTypeId, DateTime? date, out DBxSubDoc subDoc)
    {
      DBxSingleSubDocs subDocs = doc.SubDocs["PlantAttributes"];
      foreach (DBxSubDoc thisSubDoc in subDocs)
      {
        if (thisSubDoc.Values["AttrType"].AsInteger == attrTypeId &&
          thisSubDoc.Values["Date"].AsNullableDateTime == date)
        {
          subDoc = thisSubDoc;
          return true;
        }
      }

      // Не нашли
      subDoc = new DBxSubDoc();
      return false;
    }

    #endregion
  }
}