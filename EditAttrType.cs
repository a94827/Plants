using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FreeLibSet.Forms;
using FreeLibSet.Forms.Docs;
using FreeLibSet.DependedValues;
using FreeLibSet.Data.Docs;
using FreeLibSet.UICore;
using FreeLibSet.Core;

namespace Plants
{
  internal partial class EditAttrType : Form
  {
    #region Конструктор формы

    public EditAttrType()
    {
      InitializeComponent();
    }

    #endregion

    #region Табличный просмотр

    public static void TypeColumnValueNeeded(object sender, EFPGridProducerValueNeededEventArgs args)
    {
      ValueType vt = args.GetEnum<ValueType>("Type");
      args.Value = PlantTools.GetValueTypeName(vt);
    }

    #endregion

    #region Редактор

    #region InitDocEditForm

    DocumentEditor _Editor;

    public static void InitDocEditForm(object sender, InitDocEditFormEventArgs args)
    {
      EditAttrType form = new EditAttrType();
      form._Editor = args.Editor;

      form.AddPage1(args);
      if (!args.Editor.MultiDocMode)
        form.AddPage2(args);
    }

    #endregion

    #region Страница 1 (общие)

    EFPTextBox efpName;
    EFPListComboBox efpValueType;

    EFPTextBox efpFormat;

    private void AddPage1(InitDocEditFormEventArgs args)
    {
      DocEditPage page = args.AddPage("Общие", MainPanel1);
      page.ImageKey = "AttributeType";

      efpName = new EFPTextBox(page.BaseProvider, edName);
      efpName.CanBeEmpty = false;
      args.AddText(efpName, "Name", false);

      cbValueType.Items.AddRange(PlantTools.ValueTypeNames);
      efpValueType = new EFPListComboBox(page.BaseProvider, cbValueType);
      args.AddInt(efpValueType, "Type", true);


      efpFormat = new EFPTextBox(page.BaseProvider, edFormat);
      efpFormat.CanBeEmpty = true;
      efpFormat.ToolTipText = "Формат стровокого значения. \"0\"-обязательная цифра, \"#\"-необязательная цифра";
      DocValueTextBox dvFormat=args.AddText(efpFormat, "Format", false);
      dvFormat.UserEnabledEx = new DepEqual<int>(efpValueType.SelectedIndexEx, (int)ValueType.String);

      #region Комментарий

      EFPTextBox efpComment = new EFPTextBox(page.BaseProvider, edComment);
      efpComment.CanBeEmpty = true;
      args.AddText(efpComment, "Comment", true);

      #endregion
    }

    #endregion

    #region Страница 2 (список значений)

    /// <summary>
    /// Значения в хранимом формате (используется Accoo2StdConvert).
    /// Единственное строкое поле "Value".
    /// Сортировки нет
    /// </summary>
    private DataTable _StoreValues;

    private EFPDataGridView efpVLTable;
    private EFPTextBox efpVLText;

    private class DocValueVL : IDocEditItem
    {
      #region Конструктор

      public DocValueVL(EditAttrType owner, DBxDocValue value)
      {
        _Owner = owner;
        _Value = value;
        _ChangeInfo = new DepChangeInfoValueItem();
        _ChangeInfo.DisplayName = "Фиксированный список значений";
      }

      private EditAttrType _Owner;
      private DBxDocValue _Value;

      #endregion

      #region IDocEditItem Members

      public void BeforeReadValues()
      {
      }

      public void ReadValues()
      {
        string s = _Value.AsString;
        _Owner._StoreValues.BeginLoadData();
        try
        {
          _Owner._StoreValues.Clear();
          if (!String.IsNullOrEmpty(s))
          {
            string[] a = s.Split(DataTools.CRLFSeparators, // независимо от операционной системы
              StringSplitOptions.None);
            for (int i = 0; i < a.Length; i++)
              _Owner._StoreValues.Rows.Add(a[i]);
          }
        }
        finally
        {
          _Owner._StoreValues.EndLoadData();
        }
        _ChangeInfo.CurrentValue = s;
        _ChangeInfo.OriginalValue = s;
      }

      public void AfterReadValues()
      {
        _Owner.InitVLText();
      }

      public void WriteValues()
      {
        AttrValueSourceType sourceType = (AttrValueSourceType)(_Owner.efpValueSourceType.SelectedIndex);
        if (sourceType == AttrValueSourceType.List && _Owner._StoreValues.DefaultView.Count > 0)
        {
          StringBuilder sb = new StringBuilder();
          for (int i = 0; i < _Owner._StoreValues.DefaultView.Count; i++)
          {
            DataRow row = _Owner._StoreValues.DefaultView[i].Row;
            string s = row[0].ToString().Trim();
            if (s.Length == 0)
              continue;
            if (sb.Length > 0)
              sb.Append("\r\n"); // независимо от операционной системы
            sb.Append(s);
          }
          _Value.SetString(sb.ToString());
        }
        else
          _Value.SetNull();
      }

      DepChangeInfo IDocEditItem.ChangeInfo { get { return _ChangeInfo; } }
      public DepChangeInfoValueItem ChangeInfo { get { return _ChangeInfo; } }
      private DepChangeInfoValueItem _ChangeInfo;

      #endregion
    }

    private EFPListComboBox efpValueSourceType;

    private static int _LastTCVLViewSelectedIndex = 0;

    private void AddPage2(InitDocEditFormEventArgs args)
    {
      DocEditPage page = args.AddPage("Значения", MainPanel2);
      page.ImageKey = "Table";

      cbValueSourceType.Items.AddRange(PlantTools.AttrValueSourceTypeNames);
      efpValueSourceType = new EFPListComboBox(page.BaseProvider, cbValueSourceType);
      args.AddInt(efpValueSourceType, "Source", false);
      efpValueSourceType.Validating += new UIValidatingEventHandler(efpValueSourceType_Validating);


      tcVLView.ImageList = EFPApp.MainImages.ImageList;
      tpVLTable.ImageKey = "Table";
      tpVLText.ImageKey = "Font"; // !!!

      _StoreValues = new DataTable(); // Можно было бы использовать List<String>, но EFPDataGridView что-то глючит.
      _StoreValues.Columns.Add("Value", typeof(string));

      efpVLTable = new EFPDataGridView(page.BaseProvider, grVLTable);
      grVLTable.AutoGenerateColumns = false;
      efpVLTable.ReadOnly = true;
      efpVLTable.CanView = false;
      efpVLTable.Columns.AddTextFill("Value", false, String.Empty, 100, 10);
      efpVLTable.DisableOrdering();
      efpVLTable.GetCellAttributes += new EFPDataGridViewCellAttributesEventHandler(efpVLTable_GetCellAttributes);
      efpVLTable.Control.CellValuePushed += new DataGridViewCellValueEventHandler(efpVLTable_CellValuePushed);
      grVLTable.VirtualMode = true;
      efpVLTable.UseRowImages = false;
      grVLTable.Leave += new EventHandler(grVLTable_Leave);

      efpVLText = new EFPTextBox(page.BaseProvider, edVLText);
      if (args.Editor.IsReadOnly)
        efpVLText.ReadOnly = true;
      else
        edVLText.Leave += new EventHandler(edVLText_Leave);

      efpValueType.SelectedIndexEx.ValueChanged += new EventHandler(efpValueType_ValueChanged2);
      efpValueType_ValueChanged2(null, null);

      efpValueSourceType.SelectedIndexEx.ValueChanged += new EventHandler(efpValueSourceType_ValueChanged);

      tcVLView.SelectedIndex = _LastTCVLViewSelectedIndex;
      tcVLView.SelectedIndexChanged += new EventHandler(tcVLView_SelectedIndexChanged);

      args.AddDocEditItem(new DocValueVL(this, args.Values["ValueList"]));

      args.Editor.BeforeWrite += new DocEditCancelEventHandler(Editor_BeforeWrite2);
    }

    void tcVLView_SelectedIndexChanged(object sender, EventArgs args)
    {
      _LastTCVLViewSelectedIndex = tcVLView.SelectedIndex;
    }

    void efpValueSourceType_ValueChanged(object sender, EventArgs args)
    {
      InitVLTableDataSource();
    }

    void efpValueSourceType_Validating(object sender, UIValidatingEventArgs args)
    {
      AttrValueSourceType sourceType = (AttrValueSourceType)(efpValueSourceType.SelectedIndex);
      ValueType valueType = (ValueType)(efpValueType.SelectedIndex);
      if (!PlantTools.IsValidAttrSourceType(sourceType, valueType))
        args.SetError("Тип источника значений \"" + PlantTools.GetAttrValueSourceTypeName(sourceType) +
          "\" не может использоваться для типа данных \"" + PlantTools.GetValueTypeName(valueType) + "\"");
    }

    void efpValueRB_ValueChanged(object sender, EventArgs args)
    {
      // При выборе другого справочника перестраиваем "демонстрационный" список ээлементов
      InitVLTableDataSource();
    }

    private void InitVLTableDataSource()
    {
      AttrValueSourceType sourceType = (AttrValueSourceType)(efpValueSourceType.SelectedIndex);
      switch (sourceType)
      {
        case AttrValueSourceType.List:
          efpVLTable.Control.DataSource = _StoreValues.DefaultView;
          break;
        default:
          efpVLTable.Control.DataSource = null;
          break;
      }

      if (efpValueSourceType.Enabled)
      {
        grVLTable.ReadOnly = (sourceType != AttrValueSourceType.List);
        grVLTable.AllowUserToAddRows = !grVLTable.ReadOnly;
        grVLTable.AllowUserToDeleteRows = !grVLTable.ReadOnly;
        efpVLTable.Columns[0].Grayed = grVLTable.ReadOnly;
      }

      // Инициализация текста
      InitVLText();
      efpVLText.ReadOnly = grVLTable.ReadOnly;
    }

    private void InitVLText()
    {
      ValueType vt = (ValueType)(efpValueType.SelectedIndex);
      StringBuilder sb = new StringBuilder();
      if (efpVLTable.Control.DataSource != null)
      {
        foreach (DataRowView drv in efpVLTable.SourceAsDataView)
        {
          if (sb.Length > 0)
            sb.Append(Environment.NewLine);
          string s = drv.Row[0].ToString();
          if (vt == ValueType.String)
            sb.Append(s);
          else
          {
            object x;
            if (PlantsStdConvert.TryParse(s, vt, out x))
              sb.Append(PlantTools.ToString(x, vt));
            else
              sb.Append(s);
          }
        }
      }

      edVLText.Text = sb.ToString();
    }

    private void efpValueType_ValueChanged2(object sender, EventArgs args)
    {
      efpValueSourceType.Validate(); // проверяем допустимость типа источника

      if (efpValueType.SelectedIndex < 0)
        return;
      ValueType vt = (ValueType)(efpValueType.SelectedIndex);
      grVLTable.Columns[0].ValueType = PlantTools.ValueTypeToType(vt);
      grVLTable.Columns[0].HeaderText = PlantTools.GetValueTypeName(vt);
    }

    void efpVLTable_GetCellAttributes(object sender, EFPDataGridViewCellAttributesEventArgs args)
    {
      if (args.DataRow == null)
        return;
      if (args.ColumnIndex != 0)
        return;

      ValueType vt = (ValueType)(efpValueType.SelectedIndex);
      if (vt == ValueType.String)
        args.Value = args.DataRow[0].ToString();
      else
      {
        object v;
        if (PlantsStdConvert.TryParse(args.DataRow[0].ToString(), vt, out v))
          args.Value = v;
        else
        {
          args.Value = args.DataRow[0].ToString();
          args.ColorType = EFPDataGridViewColorType.Error;
        }
      }
    }

    void efpVLTable_CellValuePushed(object sender, DataGridViewCellValueEventArgs args)
    {
      try
      {
        ValueType vt = (ValueType)(efpValueType.SelectedIndex);
        object v = args.Value;
        string s = PlantsStdConvert.ToString(v, vt);
        DataRow row = efpVLTable.GetDataRow(args.RowIndex);
        if (row == null)
          return;
        row[0] = s;
      }
      catch (Exception e)
      {
        EFPApp.ShowException(e, "Ошибка записи значения атрибута в список");
      }
    }

    void grVLTable_Leave(object sender, EventArgs args)
    {
      try
      {
        InitVLText();
      }
      catch(Exception e)
      {
        EFPApp.ShowException(e, "Ошибка InitVLText()");
      }
    }

    void edVLText_Leave(object sender, EventArgs args)
    {
      try
      {
        if (efpVLText.Editable)
        {
          ValueType vt = (ValueType)(efpValueType.SelectedIndex);
          string[] a = edVLText.Lines;
          _StoreValues.BeginLoadData();
          try
          {
            _StoreValues.Rows.Clear();
            for (int i = 0; i < a.Length; i++)
            {
              string s = a[i].Trim();
              if (String.IsNullOrEmpty(s))
                continue; // пустые строки текста пропускаем
              object x;
              if (PlantTools.TryParse(s, vt, out x))
                s = PlantsStdConvert.ToString(x, vt);
              _StoreValues.Rows.Add(s);
            }
          }
          finally
          {
            _StoreValues.EndLoadData();
          }
        }
      }
      catch (Exception e)
      {
        EFPApp.ShowException(e, "Ошибка преобразования текста в список значений атрибута");
      }
    }

    void Editor_BeforeWrite2(object sender, DocEditCancelEventArgs args)
    {
      if (edVLText.Focused)
        edVLText_Leave(null, null);
    }

    #endregion

    #endregion
  }
}