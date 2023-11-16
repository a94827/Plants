using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FreeLibSet.Forms.Docs;
using FreeLibSet.Forms;
using FreeLibSet.DependedValues;
using FreeLibSet.Data.Docs;
using FreeLibSet.Collections;
using FreeLibSet.UICore;
using FreeLibSet.Core;

namespace Plants
{
  internal partial class EditCareRecord : Form
  {
    #region Конструктор формы

    public EditCareRecord()
    {
      InitializeComponent();
    }

    #endregion

    #region Редактор

    public static void InitEditForm(object sender, InitSubDocEditFormEventArgs args)
    {
      EditCareRecord form = new EditCareRecord();
      form.AddPage1(args);
    }

    EFPMonthDayTextBox efpDay1, efpDay2;

    private void AddPage1(InitSubDocEditFormEventArgs args)
    {
      DocEditPage page = args.AddPage("Общие", MainPanel1);
      page.ImageKey = args.Editor.SubDocTypeUI.ImageKey;

      efpDay1 = new EFPMonthDayTextBox(page.BaseProvider, edDay1);
      efpDay1.DisplayName = "Начало периода";
      efpDay1.CanBeEmpty = true;
      args.AddInt(efpDay1, "Day1", false);

      efpDay2 = new EFPMonthDayTextBox(page.BaseProvider, edDay2);
      efpDay2.DisplayName = "Конец периода";
      efpDay2.CanBeEmpty = true;
      args.AddInt(efpDay2, "Day2", false);

      EFPTextBox efpName = new EFPTextBox(page.BaseProvider, edName);
      args.AddText(efpName, "Name", false);

      efpDay1.Validating += new UIValidatingEventHandler(efpDay_Validating);
      efpDay2.Validating += new UIValidatingEventHandler(efpDay_Validating);
      efpDay1.ValueEx.ValueChanged += new EventHandler(efpDay2.Validate);
      efpDay2.ValueEx.ValueChanged += new EventHandler(efpDay1.Validate);

      efpName.CanBeEmpty = true;
      efpName.Validators.AddError(efpName.IsNotEmptyEx,
        "Значение должно быть задано",
        efpDay1.IsNotEmptyEx);

      EFPCareGridView ghItems = new EFPCareGridView(page.BaseProvider, grItems);
      CareDocEditItem deiItems = new CareDocEditItem(args.Values, ghItems);
      args.AddDocEditItem(deiItems);
    }

    void efpDay_Validating(object sender, UIValidatingEventArgs args)
    {
      if (args.ValidateState == UIValidateState.Error)
        return;

      if (efpDay1.Value.IsEmpty != efpDay2.Value.IsEmpty)
        args.SetError("Начало и конец периода должны быть заполнены одновременно");
    }


    #endregion

    #region Переходник для таблицы значений

    public class CareDocEditItem : IDocEditItem
    {
      #region Конструктор

      public CareDocEditItem(IDBxDocValues docValues, EFPCareGridView controlProvider)
      {
        _DocValues = docValues;
        _ControlProvider = controlProvider;

        _ChangeInfo = new DepChangeInfoList();
        _ChangeInfoItems = new DepChangeInfoItem[controlProvider.UsedItems.Count];
        for (int i = 0; i < controlProvider.UsedItems.Count; i++)
        {
          _ChangeInfoItems[i] = new DepChangeInfoItem();
          _ChangeInfoItems[i].DisplayName = controlProvider.UsedItems[i].Name;
          _ChangeInfo.Add(_ChangeInfoItems[i]);
        }
        _ControlProvider.Values.Changed += new CareValueChangedEventHandler(Values_Changed);
      }

      #endregion

      #region Свойства

      public IDBxDocValues DocValues { get { return _DocValues; } }
      private IDBxDocValues _DocValues;

      public EFPCareGridView ControlProvider { get { return _ControlProvider; } }
      private EFPCareGridView _ControlProvider;

      private CareValues OrgValues;

      #endregion

      #region IDocEditItem Members

      public void BeforeReadValues()
      {
      }

      public void ReadValues()
      {
        OrgValues = new CareValues(ControlProvider.UsedItems);
        OrgValues.Read(DocValues);

        ControlProvider.Values.Read(DocValues);
        ClearChangeInfo();
      }

      public void AfterReadValues()
      {
      }

      public void WriteValues() // TODO: проверить реализацию!
      {
        ControlProvider.Values.Write(DocValues);
        OrgValues.Read(DocValues);
        ClearChangeInfo();
      }

      public DepChangeInfo ChangeInfo { get { return _ChangeInfo; } }
      private DepChangeInfoList _ChangeInfo;

      private DepChangeInfoItem[] _ChangeInfoItems;

      private void ClearChangeInfo()
      {
        for (int i = 0; i < _ChangeInfoItems.Length; i++)
          _ChangeInfoItems[i].Changed = false;
      }

      void Values_Changed(object sender, CareValueChangedEventArgs args)
      {
        _ChangeInfoItems[args.ItemIndex].Changed = !Object.Equals(OrgValues[args.ItemIndex], args.NewValue);
      }

      #endregion
    }

    #endregion
  }

  /// <summary>
  /// Провайдер табличного просмотра для просмотра / редактирования списка характеристик
  /// </summary>
  public class EFPCareGridView : EFPDataGridView
  {
    #region Конструкторы

    public EFPCareGridView(EFPBaseProvider baseProvider, DataGridView control)
      : base(baseProvider, control)
    {
      Init();
    }

    public EFPCareGridView(EFPControlWithToolBar<DataGridView> controlWithToolBar)
      : base(controlWithToolBar)
    {
      Init();
    }

    private void Init()
    {
      Control.AutoGenerateColumns = false;
      Columns.AddImage();
      Columns.AddText("Name", false, "Параметр", 25, 10);
      Columns.LastAdded.CanIncSearch = true;

      Columns.AddTextFill("Value", false, "Значение", 100, 10);
      DisableOrdering();

      Control.ReadOnly = true;
      CanInsert = false;
      CanView = false;
      Control.MultiSelect = true;
      CanMultiEdit = false;

      UsedItems = CareItem.TheList;
    }

    protected override void OnGetRowAttributes(EFPDataGridViewRowAttributesEventArgs args)
    {
      if ((_GroupIndexes[args.RowIndex] % 2) == 0)
        args.ColorType = EFPDataGridViewColorType.Alter;
      base.OnGetRowAttributes(args);
    }

    protected override void OnGetCellAttributes(EFPDataGridViewCellAttributesEventArgs args)
    {
      if (args.ColumnIndex == 0)
      {
        if (Values[args.RowIndex] == null)
          args.Value = EFPApp.MainImages.Images["EmptyImage"];
        else
          args.Value = EFPApp.MainImages.Images["Item"];
      }
      base.OnGetCellAttributes(args);
    }

    #endregion

    #region Список

    /// <summary>
    /// Используемый список характеристик.
    /// Определяет количество строк в просмотре.
    /// Инициализируется равным CareItem.TheList.
    /// При необходимости, следует создать собственный список и задать ссылку на него.
    /// </summary>
    public NamedList<CareItem> UsedItems
    {
      get { return _Values.Items; }
      set
      {
        if (value == null)
          throw new ArgumentNullException();

        _Values = new CareValues(value);
        Control.RowCount = UsedItems.Count;
        for (int i = 0; i < UsedItems.Count; i++)
        {
          Control[1, i].Value = UsedItems[i].Name;
          Control[2, i].Value = String.Empty;
        }
        _Values.Changed += new CareValueChangedEventHandler(Values_Changed);

        _GroupIndexes = new int[UsedItems.Count];
        for (int i = 0; i < UsedItems.Count; i++)
        {
          if (i == 0)
            _GroupIndexes[i] = 1;
          else
          {
            int gi = _GroupIndexes[i - 1];
            if (UsedItems[i].Group != UsedItems[i - 1].Group)
              gi++;
            _GroupIndexes[i] = gi;
          }
        }
      }
    }

    /// <summary>
    /// Устанавливается вместе со свойством UsedItems.
    /// Длина массива соответствует числу строк в просмотре.
    /// Содержит номера групп (1,2,...).
    /// Используется для полосатой раскраски
    /// </summary>
    private int[] _GroupIndexes;

    #endregion

    #region Список значений

    public CareValues Values { get { return _Values; } }
    private CareValues _Values;

    void Values_Changed(object sender, CareValueChangedEventArgs args)
    {
      if (args.NewValue == null)
        Control[2, args.ItemIndex].Value = String.Empty;
      else
        Control[2, args.ItemIndex].Value = DataTools.GetString(args.Item.GetTextValue(args.NewValue)).Replace(Environment.NewLine, " ");
      Control.InvalidateCell(0, args.ItemIndex);
    }

    #endregion

    #region Редактирование

    protected override bool OnEditData(EventArgs args)
    {
      switch (State)
      {
        case EFPDataGridViewState.Edit:
          if (!CheckSingleRow())
            return true;
          object v = Values[CurrentRowIndex];
          if (!UsedItems[CurrentRowIndex].Edit(ref v))
            return true;
          Values[CurrentRowIndex] = v;
          break;
        case EFPDataGridViewState.Delete:
          int[] ris = SelectedRowIndices;
          for (int i = 0; i < ris.Length; i++)
            Values[ris[i]] = null;
          break;
      }
      return true;
    }

    #endregion
  }
}
