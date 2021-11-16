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

    public AttrTableViewForm(Int32[] DocIds)
    {
      InitializeComponent();

      Text = "Атрибуты";
      Icon = EFPApp.MainImageIcon("AttributeTable");

      DataTable Table = ProgramDBUI.TheUI.DocTypes["AttrTypes"].GetUnbufferedData(DBSDocType.IdColumns, null, false, new DBxOrder("Name"));
      _AttrTypeDocs = new AttrTypeDoc[Table.Rows.Count];
      for (int i = 0; i < Table.Rows.Count; i++)
      {
        Int32 Id = DataTools.GetInt(Table.Rows[i], "Id");
        AttrTypeDoc attr = new AttrTypeDoc(Id);
        _AttrTypeDocs[i] = attr;
      }

      this._MainDocIds = DocIds;


      _DocTypeUI = ProgramDBUI.TheUI.DocTypes["Plants"];

      #region Создание таблицы

      Table = new DataTable();
      Table.Columns.Add("Id", typeof(Int32)); // Идентификатор основного документа (учреждения)
      Table.Columns.Add("NPop", typeof(int)); // Исходный порядок документов 
      Table.Columns.Add("Number", typeof(int));
      Table.Columns.Add("Name", typeof(string));

      for (int i = 0; i < _AttrTypeDocs.Length; i++)
        Table.Columns.Add("Attr" + _AttrTypeDocs[i].Id.ToString(), PlantTools.ValueTypeToType(_AttrTypeDocs[i].ValueType));

      for (int i = 0; i < DocIds.Length; i++)
      {
        DataRow ResRow = Table.NewRow();
        ResRow["Id"] = DocIds[i];
        ResRow["NPop"] = i + 1;
        Table.Rows.Add(ResRow);
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

      EFPGridProducer Producer = new EFPGridProducer();
      Producer.NewDefaultConfig(false);

      Producer.Columns.AddInt("NPop", "№ п/п", 3);
      Producer.Columns.LastAdded.CanIncSearch = true;
      Producer.DefaultConfig.Columns.Add("NPop");
      Producer.Orders.Add("NPop", "В исходном порядке");
      Producer.FixedColumns.Add("NPop"); // Иначе не работает сортировка по атрибутам без наличия столбца "№ п/п"

      Producer.Columns.AddUserImage("Image", "Id",
        new EFPGridProducerValueNeededEventHandler(Image_ColumnValueNeeded), String.Empty);
      Producer.DefaultConfig.Columns.Add("Image");

      // Основные столбцы
      Producer.Columns.AddText("Number", "Номер по каталогу", 3, 3);
      Producer.Columns.LastAdded.Format = ProgramDBUI.Settings.NumberMask;
      Producer.Columns.LastAdded.CanIncSearch = true;
      Producer.DefaultConfig.Columns.Add("Number");

      Producer.Columns.AddText("Name", "Название", 20, 5);
      Producer.Columns.LastAdded.CanIncSearch = true;
      Producer.DefaultConfig.Columns.Add("Name");

      Producer.Orders.Add("Number", "Номер по каталогу");
      Producer.Orders.Add("Name", "Название");

      for (int i = 0; i < _AttrTypeDocs.Length; i++)
      {
        string ColName = "Attr" + _AttrTypeDocs[i].Id.ToString();
        ProgramDBUI.AddValueTypeColumn(Producer, _AttrTypeDocs[i].ValueType, ColName, _AttrTypeDocs[i].Name);
        Producer.Columns.LastAdded.DisplayName = _AttrTypeDocs[i].Name;
        if (!String.IsNullOrEmpty(_AttrTypeDocs[i].Comment))
          Producer.Columns.LastAdded.DisplayName += " (" + _AttrTypeDocs[i].Comment + ")";
        Producer.Columns.LastAdded.HeaderToolTipText = "Тип: " + PlantTools.GetValueTypeName(_AttrTypeDocs[i].ValueType) +
          Environment.NewLine + _AttrTypeDocs[i].Comment;
        Producer.DefaultConfig.Columns.Add(ColName);

        switch (_AttrTypeDocs[i].ValueType)
        {
          case ValueType.Date:
          case ValueType.DateTime:
            Producer.Orders.Add(ColName + ",NPop", _AttrTypeDocs[i].Name + " (по возрастанию)", new EFPDataGridViewSortInfo(ColName, ListSortDirection.Ascending));
            Producer.Orders.Add(ColName + " DESC,NPop", _AttrTypeDocs[i].Name + " (по убыванию)", new EFPDataGridViewSortInfo(ColName.ToString(), ListSortDirection.Descending));
            break;
          case ValueType.Boolean:
            break;
          default:
            Producer.Orders.Add(ColName + ",NPop", _AttrTypeDocs[i].Name, new EFPDataGridViewSortInfo(ColName, ListSortDirection.Ascending));
            break;
        }
      }

      Producer.DefaultConfig.FrozenColumns = 3;

      #region Именные конфигурации

      // Для каждого вида атрибута добавляем именную настройку с единственным столбцом
      for (int i = 0; i < _AttrTypeDocs.Length; i++)
      {
        EFPDataGridViewConfig Cfg = Producer.NewNamedConfig(_AttrTypeDocs[i].Name);
        Cfg.ImageKey = "AttributeType";
        string MainColName = "Attr" + _AttrTypeDocs[i].Id.ToString();
        for (int j = 0; j < Producer.DefaultConfig.Columns.Count; j++)
        {
          if (Producer.DefaultConfig.Columns[j].ColumnName.StartsWith("Attr"))
          {
            if (Producer.DefaultConfig.Columns[j].ColumnName != MainColName)
              continue; // оставляем только свой столбец
          }
          Cfg.Columns.Add(Producer.DefaultConfig.Columns[j].Clone());
        }
      }

      #endregion

      #endregion

      gh = new EFPDBxGridView(efpForm, TheGrid, ProgramDBUI.TheUI);
      gh.Control.AutoGenerateColumns = false;
      gh.GridProducer = Producer;
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

      gh.Control.DataSource = Table.DefaultView;
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

      FillTable(Table);

      TheGrid.Select();
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

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

    void efpDate_ValueChanged(object Sender, EventArgs Args)
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
      Int32 DocId = args.GetInt("Id");
      args.Value = _DocTypeUI.GetImageValue(DocId);
    }

    void gh_GetCellAttributes(object sender, EFPDataGridViewCellAttributesEventArgs args)
    {
      if (args.DataRow == null)
        return;
      if (args.Reason == EFPDataGridViewAttributesReason.ToolTip)
      {
        if (args.ColumnName == "Image")
        {
          Int32 DocId = DataTools.GetInt(args.DataRow, "Id");
          args.ToolTipText = _DocTypeUI.GetToolTipText(DocId);
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
        Int32 AttrTypeId = Int32.Parse(txt);
        AttrTypeDoc AttrDoc = new AttrTypeDoc(AttrTypeId);
        object Value = args.DataRow[args.ColumnName];
        string ErrorText;
        if (!AttrDoc.TestValue(Value, out ErrorText))
        {
          args.ColorType = EFPDataGridViewColorType.Warning;
          args.ToolTipText += Environment.NewLine + ErrorText;
        }
      }
    }

    void gh_EditData(object Sender, EventArgs Args)
    {
      Int32[] SelDocIds = gh.SelectedIds;
      if (SelDocIds.Length == 0)
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
          Int32 AttrTypeId = Int32.Parse(txt);
          DoEditAttrs(AttrTypeId, SelDocIds);
          return;
        }
      }

      // Обычное редактирование

      if (SelDocIds.Length > 0 && (!_DocTypeUI.CanMultiEdit))
      {
        EFPApp.ShowTempMessage("Групповое редактирование документов \"" + _DocTypeUI.DocType.PluralTitle + "\" не допускается");
        return;
      }

      if (_DocTypeUI.PerformEditing(SelDocIds, gh.State, true, null))
        FillRows(gh.SelectedDataRows);
    }

    private void DoEditAttrs(Int32 AttrTypeId, Int32[] SelDocIds)
    {
      if (!EditAttrValueGroup.PerformEdit(_DocTypeUI, SelDocIds, AttrTypeId))
        return;

      DataRow[] Rows = gh.SelectedDataRows;
      FillRows(Rows);
    }

    void gh_GetDocSel(object Sender, EFPDBxGridViewDocSelEventArgs Args)
    {
      Args.AddFromColumn(_DocTypeUI.DocType.Name, "Id");
    }

    void gh_RefreshData(object Sender, EventArgs Args)
    {
      FillTable(gh.SourceAsDataTable);
    }


    #endregion

    #region Заполнение таблицы

    private void FillTable(DataTable Table)
    {
      DataRow[] Rows = new DataRow[Table.Rows.Count];
      Table.Rows.CopyTo(Rows, 0);
      FillRows(Rows);
    }

    private void FillRows(DataRow[] Rows)
    {
      Int32[] DocIds = DataTools.GetIds(Rows);
      if (DocIds.Length == 0)
        return;

      DBxDocSet DocSet = new DBxDocSet(ProgramDBUI.TheUI.DocProvider);
      DBxMultiDocs mDocs = DocSet[_DocTypeUI.DocType.Name];
      mDocs.View(DocIds);

      foreach (DataRow DocRow in Rows)
      {
        Int32 DocId = DataTools.GetInt(DocRow, "Id");
        DBxSingleDoc Doc = mDocs.GetDocById(DocId);
        FillOneRow(DocRow, Doc);
      }
    }

    private void FillOneRow(DataRow DocRow, DBxSingleDoc Doc)
    {
      DocRow["Number"] = Doc.Values["Number"].AsInteger;
      DocRow["Name"] = Doc.Values["Name"].AsString;


      DataTable SubTable = Doc.SubDocs["PlantAttributes"].CreateSubDocsData();
      SubTable.DefaultView.Sort = "Date";
      for (int i = 0; i < _AttrTypeDocs.Length; i++)
      {
        DocRow["Attr" + _AttrTypeDocs[i].Id.ToString()] = DBNull.Value;

        SubTable.DefaultView.RowFilter = "AttrType=" + _AttrTypeDocs[i].Id;
        for (int j = SubTable.DefaultView.Count - 1; j >= 0; j--)
        {
          DataRow ValueRow = SubTable.DefaultView[j].Row;
          DateTime? dt = DataTools.GetNullableDateTime(ValueRow, "Date");
          if (dt.HasValue)
          {
            if (dt.Value > efpDate.Value)
              continue;
          }

          string s = DataTools.GetString(ValueRow, "LongValue");
          if (String.IsNullOrEmpty(s))
            s = DataTools.GetString(ValueRow, "Value");
          object v = PlantTools.ValueFromSaveableString(s, _AttrTypeDocs[i].ValueType);
          if (v != null)
            DocRow["Attr" + _AttrTypeDocs[i].Id.ToString()] = v;
          break;
        }
      }
    }

    /// <summary>
    /// Обновление одной строк после редактирование
    /// </summary>
    /// <param name="DocRow"></param>
    private void FillOneRow(DataRow DocRow)
    {
      Int32 DocId = DataTools.GetInt(DocRow, "Id");
      DBxDocSet DocSet = new DBxDocSet(ProgramDBUI.TheUI.DocProvider);
      DBxMultiDocs mDocs = DocSet[_DocTypeUI.DocType.Name];
      mDocs.View(DocId);
      FillOneRow(DocRow, mDocs[0]);
    }

    #endregion

    #region Редактирование по месту

    void efpEnableInPlace_ValueChanged(object Sender, EventArgs Args)
    {
      gh.Control.ReadOnly = !efpEnableInPlace.Checked;
    }

    void efpStartDate_NValueChanged(object Sender, EventArgs Args)
    {
      EditAttrValueHelper.LastDate = efpStartDate.NValue;
    }

    void gh_CellFinished(object Sender, EFPDataGridViewCellFinishedEventArgs Args)
    {
      if (!efpEnableInPlace.Checked)
        throw new InvalidOperationException("Редактирование по месту запрещено");

      if (!Args.ColumnName.StartsWith("Attr"))
        throw new InvalidOperationException("Столбец не относится к атрибутам");

      string txt = gh.CurrentColumnName.Substring(4);
      Int32 AttrTypeId = Int32.Parse(txt);

      Int32 DocId = DataTools.GetInt(Args.DataRow, "Id");
      if (Args.Cell.Value == null || (Args.Cell.Value is DBNull))
        EditAttrValueHelper.ClearValue(_DocTypeUI.DocType.Name, DocId, AttrTypeId, efpStartDate.Value);
      else
        EditAttrValueHelper.SetValue(DocId, AttrTypeId, efpStartDate.Value, Args.Cell.Value);

      FillOneRow(Args.DataRow); // Перечитываем значение
    }

    void efpSetStartDate_Click(object Sender, EventArgs Args)
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

    public AttrTypeDoc(Int32 Id)
    {
      object[] a = ProgramDBUI.TheUI.DocTypes["AttrTypes"].TableCache.GetValues(Id,
        new DBxColumns("Name,Type,Comment,Format,Source"));
      this._Id = Id;
      this._Name = DataTools.GetString(a[0]);
      this._ValueType = (ValueType)(DataTools.GetInt(a[1]));
      this._Comment = DataTools.GetString(a[2]);
      if (ValueType == ValueType.String)
        this._Format = DataTools.GetString(a[3]);
      else
        this._Format = String.Empty;

      this.FSourceType = (AttrValueSourceType)(DataTools.GetInt(a[4]));
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
    public AttrValueSourceType SourceType { get { return FSourceType; } }
    private AttrValueSourceType FSourceType;


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
        if (FStoreValueList == null)
          FStoreValueList = GetStoreValueList();
        return FStoreValueList;
      }
    }
    private string[] FStoreValueList;

    private static readonly string[] BoolStoreValueList = new string[] { "0", "1" };

    private string[] GetStoreValueList()
    {
      if (ValueType == ValueType.Boolean)
        return BoolStoreValueList;

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
        if (FValueList == null)
          FValueList = GetValueList();
        return FValueList;
      }
    }
    private object[] FValueList;

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
    /// <param name="Value">Проверяемое значение</param>
    /// <param name="ErrorText">Сюда помещается сообщение об ошибке или null, если ошибки нет</param>
    /// <returns>true, если значение является корректным</returns>
    public bool TestValue(object Value, out string ErrorText)
    {
      ErrorText = null;
      if (Value == null)
        return true;

      try
      {
        if (PlantTools.IsDefaultValue(Value, this.ValueType))
          return true;

        if (this.Mask != null)
        {
          if (!this.Mask.Test(Value.ToString(), out ErrorText))
            return false;
        }

        if (this.ValueList.Length > 0)
        {

          if (Array.IndexOf<object>(this.ValueList, Value) < 0)
          {
            ErrorText = "Значения \"" + PlantTools.ToString(Value, this.ValueType) + "\" нет в списке допустимых для атрибута \"" + this.Name + "\"";
            return false;
          }
        }

        return true;
      }
      catch (Exception e)
      {
        ErrorText = "Исключение при проверке значения атрибута \"" + this.Name + "\" " + e.Message;
        return false;
      }
    }

    /// <summary>
    /// Проверка корректности значения атрибута.
    /// Null и значение по умолчанию (0) считаются корректными значениями.
    /// </summary>
    /// <param name="Value">Проверяемое значение</param>
    /// <returns>true, если значение является корректным</returns>
    public bool TestValue(object Value)
    {
      string ErrorText;
      return TestValue(Value, out ErrorText);
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
    /// <param name="AttrType"></param>
    /// <param name="Args"></param>
    public static void ValidateAttributeText(AttrTypeDoc AttrType, EFPSelRCValidatingEventArgs Args)
    {
      if (String.IsNullOrEmpty(Args.SourceData))
      {
        Args.ResultValue = null;
        return;
      }

      object v;
      if (PlantTools.TryParse(Args.SourceData, AttrType.ValueType, out v))
        Args.ResultValue = v;
      else
        Args.SetError("Нельзя преобразовать в \"" + PlantTools.GetValueTypeName(AttrType.ValueType) + "\"");
    }

    public static void AddAtrSubDoc(DBxSingleDoc Doc, AttrTypeDoc AttrType, object Value, DateTime? Date)
    {
      if (PlantTools.IsDefaultValue(Value, AttrType.ValueType))
        // Значение по умолчанию добавлять не будем
        return;

      DBxSubDoc SubDoc = Doc.SubDocs["PlantAttributes"].Insert();
      SubDoc.Values["AttrType"].SetInteger(AttrType.Id);
      SubDoc.Values["Date"].SetNullableDateTime(Date);
      string s = PlantTools.ValueToSaveableString(Value, AttrType.ValueType);
      if (s.Length <= PlantTools.AttrValueShortMaxLength)
        SubDoc.Values["Value"].SetString(s);
      else
        SubDoc.Values["LongValue"].SetString(s);
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
    /// <param name="DocTypeName"></param>
    /// <param name="DocId"></param>
    /// <param name="AttrTypeId"></param>
    /// <param name="Date"></param>
    public static void ClearValue(string DocTypeName, Int32 DocId, Int32 AttrTypeId, DateTime? Date)
    {
      AttrTypeDoc AttrType = new AttrTypeDoc(AttrTypeId);
      DBxDocSet DocSet = new DBxDocSet(ProgramDBUI.TheUI.DocProvider);
      DBxSingleDoc Doc = DocSet[DocTypeName].Edit(DocId);
      DBxSubDoc SubDoc;
      if (FindSubDoc(Doc, AttrTypeId, Date, out SubDoc))
      {
        DocSet.ActionInfo = "Удаление атрибута \"" + AttrType.Name + "\"";
        SubDoc.Delete();
        DocSet.ApplyChanges(false);
      }
    }

    public static void SetValue(Int32 DocId, Int32 AttrTypeId, DateTime? Date, object Value)
    {
      AttrTypeDoc AttrType = new AttrTypeDoc(AttrTypeId);
      string NewSValue = PlantTools.ValueToSaveableString(Value, AttrType.ValueType);
      string NewSValue1, NewSValue2;
      SplitAttrValue(NewSValue, out NewSValue1, out NewSValue2);


      DBxDocSet DocSet = new DBxDocSet(ProgramDBUI.TheUI.DocProvider);
      DBxSingleDoc Doc = DocSet["Plants"].Edit(DocId);

      DBxSubDoc SubDoc;
      if (FindSubDoc(Doc, AttrTypeId, Date, out SubDoc))
      {
        string OldSValue1 = SubDoc.Values["Value"].AsString;
        string OldSValue2 = SubDoc.Values["LongValue"].AsString;
        if (NewSValue1 == OldSValue1 && NewSValue2 == OldSValue2)
          return; // Ничего менять не надо

        DocSet.ActionInfo = "Изменение атрибута \"" + AttrType.Name + "\"";
        SubDoc.Values["Value"].SetString(NewSValue1);
        SubDoc.Values["LongValue"].SetString(NewSValue2);
      }
      else
      {
        // Нет такого атрибута
        // Добавление пустого значения не эквивалентно отсутствию атрибута вообще, если задана дата атрибута
        if ((!Date.HasValue) && PlantTools.IsDefaultValue(Value, AttrType.ValueType))
          return;

        DocSet.ActionInfo = "Добавление атрибута \"" + AttrType.Name + "\"";

        SubDoc = Doc.SubDocs["PlantAttributes"].Insert();
        SubDoc.Values["AttrType"].SetInteger(AttrType.Id);
        SubDoc.Values["Date"].SetNullableDateTime(Date);
        SubDoc.Values["Value"].SetString(NewSValue1);
        SubDoc.Values["LongValue"].SetString(NewSValue2);
      }

      DocSet.ApplyChanges(false);
    }

    private static bool FindSubDoc(DBxSingleDoc Doc, Int32 AttrTypeId, DateTime? Date, out DBxSubDoc SubDoc)
    {
      DBxSingleSubDocs SubDocs = Doc.SubDocs["PlantAttributes"];
      foreach (DBxSubDoc ThisSubDoc in SubDocs)
      {
        if (ThisSubDoc.Values["AttrType"].AsInteger == AttrTypeId &&
          ThisSubDoc.Values["Date"].AsNullableDateTime == Date)
        {
          SubDoc = ThisSubDoc;
          return true;
        }
      }

      // Не нашли
      SubDoc = new DBxSubDoc();
      return false;
    }

    #endregion
  }

}