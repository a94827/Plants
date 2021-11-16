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

    DocumentEditor Editor;

    public static void InitDocEditForm(object Sender, InitDocEditFormEventArgs Args)
    {
      EditAttrType Form = new EditAttrType();

      Form.Editor = Args.Editor;

      Form.AddPage1(Args);
      if (!Args.Editor.MultiDocMode)
        Form.AddPage2(Args);
    }

    #endregion

    #region Страница 1 (общие)

    EFPTextBox efpName;
    EFPListComboBox efpValueType;

    EFPTextBox efpFormat;

    private void AddPage1(InitDocEditFormEventArgs Args)
    {
      DocEditPage Page = Args.AddPage("Общие", MainPanel1);
      Page.ImageKey = "AttributeType";

      efpName = new EFPTextBox(Page.BaseProvider, edName);
      efpName.CanBeEmpty = false;
      Args.AddText(efpName, "Name", false);

      cbValueType.Items.AddRange(PlantTools.ValueTypeNames);
      efpValueType = new EFPListComboBox(Page.BaseProvider, cbValueType);
      Args.AddInt(efpValueType, "Type", true);


      efpFormat = new EFPTextBox(Page.BaseProvider, edFormat);
      efpFormat.CanBeEmpty = true;
      efpFormat.ToolTipText = "Формат стровокого значения. \"0\"-обязательная цифра, \"#\"-необязательная цифра";
      DocValueTextBox dvFormat=Args.AddText(efpFormat, "Format", false);
      dvFormat.UserEnabledEx = new DepEqual<int>(efpValueType.SelectedIndexEx, (int)ValueType.String);

      #region Комментарий

      EFPTextBox efpComment = new EFPTextBox(Page.BaseProvider, edComment);
      efpComment.CanBeEmpty = true;
      Args.AddText(efpComment, "Comment", true);

      #endregion
    }

    #endregion

    #region Страница 2 (список значений)

    /// <summary>
    /// Значения в хранимом формате (используется Accoo2StdConvert).
    /// Единственное строкое поле "Value".
    /// Сортировки нет
    /// </summary>
    private DataTable StoreValues;


    private EFPDataGridView efpVLTable;
    private EFPTextBox efpVLText;

    private class DocValueVL : IDocEditItem
    {
      #region Конструктор

      public DocValueVL(EditAttrType Owner, DBxDocValue Value)
      {
        FOwner = Owner;
        FValue = Value;
        FChangeInfo = new DepChangeInfoValueItem();
        FChangeInfo.DisplayName = "Фиксированный список значений";
      }

      private EditAttrType FOwner;
      private DBxDocValue FValue;

      #endregion

      #region IDocEditItem Members

      public void BeforeReadValues()
      {
      }

      public void ReadValues()
      {
        string s = FValue.AsString;
        FOwner.StoreValues.BeginLoadData();
        try
        {
          FOwner.StoreValues.Clear();
          if (!String.IsNullOrEmpty(s))
          {
            string[] a = s.Split(DataTools.CRLFSeparators, // независимо от операционной системы
              StringSplitOptions.None);
            for (int i = 0; i < a.Length; i++)
              FOwner.StoreValues.Rows.Add(a[i]);
          }
        }
        finally
        {
          FOwner.StoreValues.EndLoadData();
        }
        FChangeInfo.CurrentValue = s;
        FChangeInfo.OriginalValue = s;
      }

      public void AfterReadValues()
      {
        FOwner.InitVLText();
      }

      public void WriteValues()
      {
        AttrValueSourceType Type = (AttrValueSourceType)(FOwner.efpValueSourceType.SelectedIndex);
        if (Type == AttrValueSourceType.List && FOwner.StoreValues.DefaultView.Count > 0)
        {
          StringBuilder sb = new StringBuilder();
          for (int i = 0; i < FOwner.StoreValues.DefaultView.Count; i++)
          {
            DataRow Row = FOwner.StoreValues.DefaultView[i].Row;
            string s = Row[0].ToString().Trim();
            if (s.Length == 0)
              continue;
            if (sb.Length > 0)
              sb.Append("\r\n"); // независимо от операционной системы
            sb.Append(s);
          }
          FValue.SetString(sb.ToString());
        }
        else
          FValue.SetNull();
      }

      DepChangeInfo IDocEditItem.ChangeInfo { get { return FChangeInfo; } }
      public DepChangeInfoValueItem ChangeInfo { get { return FChangeInfo; } }
      private DepChangeInfoValueItem FChangeInfo;

      #endregion
    }

    private EFPListComboBox efpValueSourceType;

    private static int LastTCVLViewSelectedIndex = 0;

    private void AddPage2(InitDocEditFormEventArgs Args)
    {
      DocEditPage Page = Args.AddPage("Значения", MainPanel2);
      Page.ImageKey = "Table";

      cbValueSourceType.Items.AddRange(PlantTools.AttrValueSourceTypeNames);
      efpValueSourceType = new EFPListComboBox(Page.BaseProvider, cbValueSourceType);
      Args.AddInt(efpValueSourceType, "Source", false);
      efpValueSourceType.Validating += new UIValidatingEventHandler(efpValueSourceType_Validating);


      tcVLView.ImageList = EFPApp.MainImages;
      tpVLTable.ImageKey = "Table";
      tpVLText.ImageKey = "Font"; // !!!

      StoreValues = new DataTable(); // Можно было бы использовать List<String>, но EFPDataGridView что-то глючит.
      StoreValues.Columns.Add("Value", typeof(string));

      efpVLTable = new EFPDataGridView(Page.BaseProvider, grVLTable);
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

      efpVLText = new EFPTextBox(Page.BaseProvider, edVLText);
      if (Args.Editor.IsReadOnly)
        efpVLText.ReadOnly = true;
      else
        edVLText.Leave += new EventHandler(edVLText_Leave);

      efpValueType.SelectedIndexEx.ValueChanged += new EventHandler(efpValueType_ValueChanged2);
      efpValueType_ValueChanged2(null, null);

      efpValueSourceType.SelectedIndexEx.ValueChanged += new EventHandler(efpValueSourceType_ValueChanged);

      tcVLView.SelectedIndex = LastTCVLViewSelectedIndex;
      tcVLView.SelectedIndexChanged += new EventHandler(tcVLView_SelectedIndexChanged);

      Args.AddDocEditItem(new DocValueVL(this, Args.Values["ValueList"]));

      Args.Editor.BeforeWrite += new DocEditCancelEventHandler(Editor_BeforeWrite2);
    }

    void tcVLView_SelectedIndexChanged(object Sender, EventArgs Args)
    {
      LastTCVLViewSelectedIndex = tcVLView.SelectedIndex;
    }

    void efpValueSourceType_ValueChanged(object Sender, EventArgs Args)
    {
      InitVLTableDataSource();
    }

    void efpValueSourceType_Validating(object Sender, UIValidatingEventArgs Args)
    {
      AttrValueSourceType SourceType = (AttrValueSourceType)(efpValueSourceType.SelectedIndex);
      ValueType ValueType = (ValueType)(efpValueType.SelectedIndex);
      if (!PlantTools.IsValidAttrSourceType(SourceType, ValueType))
        Args.SetError("Тип источника значений \"" + PlantTools.GetAttrValueSourceTypeName(SourceType) +
          "\" не может использоваться для типа данных \"" + PlantTools.GetValueTypeName(ValueType) + "\"");
    }

    void efpValueRB_ValueChanged(object Sender, EventArgs Args)
    {
      // При выборе другого справочника перестраиваем "демонстрационный" список ээлементов
      InitVLTableDataSource();
    }

    private void InitVLTableDataSource()
    {
      AttrValueSourceType Type = (AttrValueSourceType)(efpValueSourceType.SelectedIndex);
      switch (Type)
      {
        case AttrValueSourceType.List:
          efpVLTable.Control.DataSource = StoreValues.DefaultView;
          break;
        default:
          efpVLTable.Control.DataSource = null;
          break;
      }

      if (efpValueSourceType.Enabled)
      {
        grVLTable.ReadOnly = (Type != AttrValueSourceType.List);
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

    private void efpValueType_ValueChanged2(object Sender, EventArgs Args)
    {
      efpValueSourceType.Validate(); // проверяем допустимость типа источника

      if (efpValueType.SelectedIndex < 0)
        return;
      ValueType vt = (ValueType)(efpValueType.SelectedIndex);
      grVLTable.Columns[0].ValueType = PlantTools.ValueTypeToType(vt);
      grVLTable.Columns[0].HeaderText = PlantTools.GetValueTypeName(vt);
    }

    void efpVLTable_GetCellAttributes(object Sender, EFPDataGridViewCellAttributesEventArgs Args)
    {
      if (Args.DataRow == null)
        return;
      if (Args.ColumnIndex != 0)
        return;

      ValueType vt = (ValueType)(efpValueType.SelectedIndex);
      if (vt == ValueType.String)
        Args.Value = Args.DataRow[0].ToString();
      else
      {
        object v;
        if (PlantsStdConvert.TryParse(Args.DataRow[0].ToString(), vt, out v))
          Args.Value = v;
        else
        {
          Args.Value = Args.DataRow[0].ToString();
          Args.ColorType = EFPDataGridViewColorType.Error;
        }
      }
    }

    void efpVLTable_CellValuePushed(object Sender, DataGridViewCellValueEventArgs Args)
    {
      try
      {
        ValueType vt = (ValueType)(efpValueType.SelectedIndex);
        object v = Args.Value;
        string s = PlantsStdConvert.ToString(v, vt);
        DataRow Row = efpVLTable.GetDataRow(Args.RowIndex);
        if (Row == null)
          return;
        Row[0] = s;
      }
      catch (Exception e)
      {
        EFPApp.ShowException(e, "Ошибка записи значения атрибута в список");
      }
    }

    void grVLTable_Leave(object Sender, EventArgs Args)
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

    void edVLText_Leave(object Sender, EventArgs Args)
    {
      try
      {
        if (efpVLText.Editable)
        {
          ValueType vt = (ValueType)(efpValueType.SelectedIndex);
          string[] a = edVLText.Lines;
          StoreValues.BeginLoadData();
          try
          {
            StoreValues.Rows.Clear();
            for (int i = 0; i < a.Length; i++)
            {
              string s = a[i].Trim();
              if (String.IsNullOrEmpty(s))
                continue; // пустые строки текста пропускаем
              object x;
              if (PlantTools.TryParse(s, vt, out x))
                s = PlantsStdConvert.ToString(x, vt);
              StoreValues.Rows.Add(s);
            }
          }
          finally
          {
            StoreValues.EndLoadData();
          }
        }
      }
      catch (Exception e)
      {
        EFPApp.ShowException(e, "Ошибка преобразования текста в список значений атрибута");
      }
    }

    void Editor_BeforeWrite2(object Sender, DocEditCancelEventArgs Args)
    {
      if (edVLText.Focused)
        edVLText_Leave(null, null);
    }

    #endregion

    #endregion
  }
}