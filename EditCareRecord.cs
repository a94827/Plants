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

    public static void InitEditForm(object Sender, InitSubDocEditFormEventArgs Args)
    {
      EditCareRecord Form = new EditCareRecord();
      Form.AddPage1(Args);
    }

    EFPMonthDayTextBox efpDay1, efpDay2;

    private void AddPage1(InitSubDocEditFormEventArgs Args)
    {
      DocEditPage Page = Args.AddPage("Общие", MainPanel1);
      Page.ImageKey = Args.Editor.SubDocTypeUI.ImageKey;

      efpDay1 = new EFPMonthDayTextBox(Page.BaseProvider, edDay1);
      efpDay1.DisplayName = "Начало периода";
      efpDay1.CanBeEmpty = true;
      Args.AddInt(efpDay1, "Day1", false);

      efpDay2 = new EFPMonthDayTextBox(Page.BaseProvider, edDay2);
      efpDay2.DisplayName = "Конец периода";
      efpDay2.CanBeEmpty = true;
      Args.AddInt(efpDay2, "Day2", false);

      EFPTextBox efpName = new EFPTextBox(Page.BaseProvider, edName);
      Args.AddText(efpName, "Name", false);

      efpDay1.Validating += new UIValidatingEventHandler(efpDay_Validating);
      efpDay2.Validating += new UIValidatingEventHandler(efpDay_Validating);
      efpDay1.ValueEx.ValueChanged += new EventHandler(efpDay2.Validate);
      efpDay2.ValueEx.ValueChanged += new EventHandler(efpDay1.Validate);

      efpName.CanBeEmpty = true;
      efpName.Validators.AddError(efpName.IsNotEmptyEx, 
        "Значение должно быть задано",
        efpDay1.IsNotEmptyEx);

      EFPCareGridView ghItems=new EFPCareGridView(Page.BaseProvider, grItems);
      CareDocEditItem deiItems = new CareDocEditItem(Args.Values, ghItems);
      Args.AddDocEditItem(deiItems);
    }

    void efpDay_Validating(object Sender, UIValidatingEventArgs Args)
    {
      if (Args.ValidateState == UIValidateState.Error)
        return;

      if (efpDay1.Value.IsEmpty != efpDay2.Value.IsEmpty)
        Args.SetError("Начало и конец периода должны быть заполнены одновременно");
    }


    #endregion

    #region Переходник для таблицы значений

    public class CareDocEditItem : IDocEditItem
    {
      #region Конструктор

      public CareDocEditItem(IDBxDocValues DocValues, EFPCareGridView ControlProvider)
      {
        FDocValues = DocValues;
        FControlProvider = ControlProvider;

        FChangeInfo=new DepChangeInfoList();
        FChangeInfoItems = new DepChangeInfoItem[ControlProvider.UsedItems.Count];
        for (int i = 0; i < ControlProvider.UsedItems.Count; i++)
        {
          FChangeInfoItems[i] = new DepChangeInfoItem();
          FChangeInfoItems[i].DisplayName = ControlProvider.UsedItems[i].Name;
          FChangeInfo.Add(FChangeInfoItems[i]);
        }
        FControlProvider.Values.Changed += new CareValueChangedEventHandler(Values_Changed);
      }

      #endregion

      #region Свойства

      public IDBxDocValues DocValues { get { return FDocValues; } }
      private IDBxDocValues FDocValues;

      public EFPCareGridView ControlProvider { get { return FControlProvider; } }
      private EFPCareGridView FControlProvider;

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

      public DepChangeInfo ChangeInfo { get { return FChangeInfo; } }
      private DepChangeInfoList FChangeInfo;

      private DepChangeInfoItem[] FChangeInfoItems;

      private void ClearChangeInfo()
      {
        for (int i = 0; i < FChangeInfoItems.Length; i++)
          FChangeInfoItems[i].Changed = false;
      }

      void Values_Changed(object Sender, CareValueChangedEventArgs Args)
      {
        FChangeInfoItems[Args.ItemIndex].Changed = !Object.Equals(OrgValues[Args.ItemIndex], Args.NewValue);
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

    public EFPCareGridView(EFPBaseProvider BaseProvider, DataGridView Control)
      : base(BaseProvider, Control)
    {
      Init();
    }

    public EFPCareGridView(EFPControlWithToolBar<DataGridView> ControlWithToolBar)
      : base(ControlWithToolBar)
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

    protected override void OnGetRowAttributes(EFPDataGridViewRowAttributesEventArgs Args)
    {
      if ((GroupIndexes[Args.RowIndex] % 2) == 0)
        Args.ColorType = EFPDataGridViewColorType.Alter;
      base.OnGetRowAttributes(Args);
    }

    protected override void OnGetCellAttributes(EFPDataGridViewCellAttributesEventArgs Args)
    {
      if (Args.ColumnIndex == 0)
      {
        if (Values[Args.RowIndex] == null)
          Args.Value = EFPApp.MainImages.Images["EmptyImage"];
        else
          Args.Value = EFPApp.MainImages.Images["Item"];
      }
      base.OnGetCellAttributes(Args);
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
      get { return FValues.Items; }
      set
      {
        if (value == null)
          throw new ArgumentNullException();

        FValues = new CareValues(value);
        Control.RowCount = UsedItems.Count;
        for (int i = 0; i < UsedItems.Count; i++)
        {
          Control[1, i].Value = UsedItems[i].Name;
          Control[2, i].Value = String.Empty;
        }
        FValues.Changed += new CareValueChangedEventHandler(FValues_Changed);

        GroupIndexes = new int[UsedItems.Count];
        for (int i = 0; i < UsedItems.Count; i++)
        {
          if (i == 0)
            GroupIndexes[i] = 1;
          else
          {
            int GI = GroupIndexes[i - 1];
            if (UsedItems[i].Group != UsedItems[i - 1].Group)
              GI++;
            GroupIndexes[i] = GI;
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
    private int[] GroupIndexes;

    #endregion

    #region Список значений

    public CareValues Values { get { return FValues; } }
    private CareValues FValues;

    void FValues_Changed(object Sender, CareValueChangedEventArgs Args)
    {
      if (Args.NewValue == null)
        Control[2, Args.ItemIndex].Value = String.Empty;
      else
        Control[2, Args.ItemIndex].Value = DataTools.GetString(Args.Item.GetTextValue(Args.NewValue)).Replace(Environment.NewLine, " ");
      Control.InvalidateCell(0, Args.ItemIndex);
    }

    #endregion

    #region Редактирование

    protected override bool OnEditData(EventArgs Args)
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