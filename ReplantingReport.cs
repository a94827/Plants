using System;
using System.Collections.Generic;
using System.Text;
using FreeLibSet.Forms;
using FreeLibSet.Forms.Docs;
using System.Windows.Forms;
using System.Data;
using FreeLibSet.Data;
using FreeLibSet.Data.Docs;
using System.ComponentModel;
using FreeLibSet.Core;

namespace Plants
{
  internal class ReplantingReportParamsForm : EFPReportExtParamsForm
  {
    #region Конструктор

    public ReplantingReportParamsForm()
    {
      Text = "Пересадка";

      EFPControlWithToolBar<DataGridView> cwt = new EFPControlWithToolBar<DataGridView>(FormProvider, base.MainPanel);
      _FiltersControlProvider = new EFPGridFilterEditorGridView(cwt);

    }

    #endregion

    #region Свойства

    /// <summary>
    /// Провайдер табличного просмотра для таблицы редактирования фильтров.
    /// </summary>
    public EFPGridFilterEditorGridView FiltersControlProvider { get { return _FiltersControlProvider; } }
    private EFPGridFilterEditorGridView _FiltersControlProvider;

    #endregion
  }

  /// <summary>
  /// Фильтры для отчета.
  /// </summary>
  internal class ReplantingReportFilters : DBxClientFilters
  {
    #region Конструктор

    /// <summary>
    /// Создает фильтры
    /// </summary>
    public ReplantingReportFilters()
    {
      FiltGroup = new RefGroupDocGridFilter(ProgramDBUI.TheUI, "PlantGroups", "GroupId");
      Add(FiltGroup);

      FiltPlace = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["Places"], "Place");
      FiltPlace.DisplayName = "Место расположения";
      FiltPlace.Nullable = false;
      FiltPlace.UseSqlFilter = true; // здесь используем вычисляемое поле
      Add(FiltPlace);

      SetReadOnly();
    }

    #endregion

    #region Поля фильтров

    public RefGroupDocGridFilter FiltGroup;

    public RefDocGridFilter FiltPlace;

    #endregion
  }

  internal class ReplantingReportParams : EFPReportExtParams
  {
    #region Конструктор

    public ReplantingReportParams()
    {
      Filters = new ReplantingReportFilters();
    }

    #endregion

    #region Поля

    public ReplantingReportFilters Filters;

    #endregion

    #region Переопределенные методы

    protected override void OnInitTitle()
    {
      base.Title = "Пересадка растений";
      Filters.AddFilterInfo(FilterInfo);
    }

    public override EFPReportExtParamsForm CreateForm()
    {
      return new ReplantingReportParamsForm();
    }

    public override EFPReportExtParamsPart UsedParts
    {
      get { return EFPReportExtParamsPart.User; }
    }

    public override void WriteFormValues(EFPReportExtParamsForm form, EFPReportExtParamsPart part)
    {
      ReplantingReportParamsForm form2 = (ReplantingReportParamsForm)form;
      form2.FiltersControlProvider.Filters = Filters;
    }

    public override void ReadFormValues(EFPReportExtParamsForm form, EFPReportExtParamsPart part)
    {
      //ReplantingReportParamsForm form2 = (ReplantingReportParamsForm)form;
    }

    public override void WriteConfig(FreeLibSet.Config.CfgPart cfg, EFPReportExtParamsPart part)
    {
      switch (part)
      {
        case EFPReportExtParamsPart.User:
          Filters.WriteConfig(cfg);
          break;
      }
    }

    public override void ReadConfig(FreeLibSet.Config.CfgPart cfg, EFPReportExtParamsPart part)
    {
      switch (part)
      {
        case EFPReportExtParamsPart.User:
          Filters.ReadConfig(cfg);
          break;
      }
    }

    #endregion
  }

  internal class ReplantingReport : EFPReport
  {
    #region Конструктор

    public ReplantingReport()
      : base("ReplantingReport")
    {
      MainImageKey = "ReplantingReport";

      _MainPage = new EFPReportDBxGridPage(ProgramDBUI.TheUI);
      _MainPage.InitGrid += new EventHandler(MainPage_InitGrid);
      _MainPage.ShowToolBar = true;
      Pages.Add(_MainPage);
    }

    #endregion

    #region Запрос параметров

    protected override EFPReportParams CreateParams()
    {
      return new ReplantingReportParams();
    }

    public ReplantingReportParams Params { get { return (ReplantingReportParams)(base.ReportParams); } }

    #endregion

    #region Построение отчета

    const ActionKind ActionNone = (ActionKind)(-2);
    const ActionKind ActionAdd = (ActionKind)(-1); // операция "Приход"

    protected override void BuildReport()
    {
      DataTable resTable = new DataTable();
      resTable.Columns.Add("PlantId", typeof(Int32));
      resTable.Columns.Add("PlantNumber", typeof(int));
      resTable.Columns.Add("PlantName", typeof(string));

      resTable.Columns.Add("ActionKind", typeof(int));
      resTable.Columns.Add("ActionDate", typeof(DateTime));
      resTable.Columns.Add("SoilText", typeof(string));
      resTable.Columns.Add("PotKindText", typeof(string));
      resTable.Columns.Add("ActionComment", typeof(string));

      DataTools.SetPrimaryKey(resTable, "PlantId");

      #region Таблица растений

      DBxFilter sqlFilter = Params.Filters.GetSqlFilter();
      sqlFilter &= new ValueFilter("MovementState", (int)(PlantMovementState.Placed)); // только существующие
      if (ProgramDBUI.TheUI.DocProvider.DocTypes.UseDeleted)
        sqlFilter &= DBSDocType.DeletedFalseFilter;

      DataTable plantTable = ProgramDBUI.TheUI.DocProvider.FillSelect("Plants",
        new DBxColumns("Id,Number,Name"),
        sqlFilter);

      foreach (DataRow row in plantTable.Rows)
        resTable.Rows.Add(row["Id"], row["Number"], row["Name"], (int)ActionNone);

      #endregion

      #region Таблица действий

      sqlFilter = new ValuesFilter("Kind", new int[] { 
        (int)ActionKind.Planting, 
        (int)ActionKind.Replanting,
        (int)ActionKind.Transshipment,
        (int)ActionKind.SoilReplace});
      if (ProgramDBUI.TheUI.DocProvider.DocTypes.UseDeleted)
      {
        sqlFilter &= DBSSubDocType.DeletedFalseFilter;
        sqlFilter &= DBSSubDocType.DocIdDeletedFalseFilter;
      }

      DataTable actionTable = ProgramDBUI.TheUI.DocProvider.FillSelect("PlantActions",
        new DBxColumns("DocId,Kind,Date2,Soil,PotKind,Comment"),
        sqlFilter,
        new DBxOrder("Date2", ListSortDirection.Descending));

      foreach (DataRow row in actionTable.Rows)
      {
        Int32 plantId = DataTools.GetInt(row, "DocId");
        DataRow resRow = resTable.Rows.Find(plantId);
        if (resRow == null)
          continue; // растение не прошло фильтрацию

        if (!resRow.IsNull("ActionDate"))
          continue; // уже была более поздняя операция

        resRow["ActionKind"] = row["Kind"];
        resRow["ActionDate"] = row["Date2"];
        Int32 SoilId = DataTools.GetInt(row, "Soil");
        if (SoilId != 0)
          resRow["SoilText"] = ProgramDBUI.TheUI.DocTypes["Soils"].GetTextValue(SoilId);
        Int32 PotKindId = DataTools.GetInt(row, "PotKind");
        if (PotKindId != 0)
          resRow["PotKindText"] = ProgramDBUI.TheUI.DocTypes["PotKinds"].GetTextValue(PotKindId);
        resRow["ActionComment"] = row["Comment"];
      }

      #endregion

      #region Таблица движения

      sqlFilter = new ValueFilter("Kind", (int)MovementKind.Add);
      if (ProgramDBUI.TheUI.DocProvider.DocTypes.UseDeleted)
      {
        sqlFilter &= DBSSubDocType.DeletedFalseFilter;
        sqlFilter &= DBSSubDocType.DocIdDeletedFalseFilter;
      }

      DataTable movementTable = ProgramDBUI.TheUI.DocProvider.FillSelect("PlantMovement",
        new DBxColumns("DocId,Date2,Soil,PotKind,Comment"),
        sqlFilter,
        new DBxOrder("Date2", ListSortDirection.Descending));

      foreach (DataRow row in movementTable.Rows)
      {
        Int32 plantId = DataTools.GetInt(row, "DocId");
        DataRow resRow = resTable.Rows.Find(plantId);
        if (resRow == null)
          continue; // растение не прошло фильтрацию

        if (!resRow.IsNull("ActionDate"))
          continue; // уже была более какая-либо операция

        resRow["ActionKind"] = ActionAdd;
        resRow["ActionDate"] = row["Date2"];
        Int32 soilId = DataTools.GetInt(row, "Soil");
        if (soilId != 0)
          resRow["SoilText"] = ProgramDBUI.TheUI.DocTypes["Soils"].GetTextValue(soilId);
        Int32 potKindId = DataTools.GetInt(row, "PotKind");
        if (potKindId != 0)
          resRow["PotKindText"] = ProgramDBUI.TheUI.DocTypes["PotKinds"].GetTextValue(potKindId);
        resRow["ActionComment"] = row["Comment"];
      }

      #endregion

      _MainPage.DataSource = resTable.DefaultView;
    }

    #endregion

    #region Страница отчета

    EFPReportDBxGridPage _MainPage;

    void MainPage_InitGrid(object sender, EventArgs args)
    {
      _MainPage.ControlProvider.Control.AutoGenerateColumns = false;
      _MainPage.ControlProvider.Columns.AddInt("PlantNumber", true, "№ по каталогу", 3);
      _MainPage.ControlProvider.Columns.LastAdded.GridColumn.DefaultCellStyle.Format = ProgramDBUI.Settings.NumberMask;
      _MainPage.ControlProvider.Columns.LastAdded.CanIncSearch = true;
      _MainPage.ControlProvider.Columns.AddText("PlantName", true, "Наименование", 40, 20);
      _MainPage.ControlProvider.Columns.LastAdded.CanIncSearch = true;
      _MainPage.ControlProvider.Columns.AddImage("ActionImage");
      _MainPage.ControlProvider.Columns.AddDate("ActionDate", true, "Дата операции");
      _MainPage.ControlProvider.Columns.AddText("ActionName", false, "Действие", 20, 10);
      _MainPage.ControlProvider.Columns.AddText("SoilText", true, "Грунт", 20, 10);
      _MainPage.ControlProvider.Columns.AddText("PotKindText", true, "Горшок", 20, 10);
      _MainPage.ControlProvider.Columns.AddText("ActionComment", true, "Комментарий к операции", 40, 10);
      _MainPage.ControlProvider.DisableOrdering();

      _MainPage.ControlProvider.Orders.Add("PlantNumber,PlantName", "По номеру");
      _MainPage.ControlProvider.Orders.Add("PlantName,PlantNumber", "По названию");
      _MainPage.ControlProvider.Orders.Add("ActionDate,PlantNumber,PlantName", "По дате пересадки (по возрастанию", new EFPDataGridViewSortInfo("ActionDate", ListSortDirection.Ascending));
      _MainPage.ControlProvider.Orders.Add("ActionDate DESC,PlantNumber,PlantName", "По дате пересадки (по убыванию", new EFPDataGridViewSortInfo("ActionDate", ListSortDirection.Descending));
      _MainPage.ControlProvider.AutoSort = true;
      _MainPage.ControlProvider.CurrentOrderIndex = 2;

      _MainPage.ControlProvider.GetCellAttributes += new EFPDataGridViewCellAttributesEventHandler(MainPage_GetCellAttributes);

      _MainPage.ControlProvider.ReadOnly = false;
      _MainPage.ControlProvider.CanInsert = false;
      _MainPage.ControlProvider.CanDelete = false;
      _MainPage.ControlProvider.CanView = true;
      _MainPage.ControlProvider.Control.ReadOnly = true;
      _MainPage.ControlProvider.EditData += new EventHandler(MainPage_EditData);
      _MainPage.ControlProvider.GetDocSel += new EFPDBxGridViewDocSelEventHandler(MainPage_GetDocSel);
    }

    private static void MainPage_GetCellAttributes(object sender, EFPDataGridViewCellAttributesEventArgs args)
    {
      ActionKind kind;
      switch (args.ColumnName)
      {
        case "ActionImage":
          kind = (ActionKind)DataTools.GetInt(args.DataRow, "ActionKind");
          if (kind == ActionNone)
            args.Value = EFPApp.MainImages.Images["EmptyImage"];
          else if (kind == ActionAdd)
            args.Value = EFPApp.MainImages.Images["Insert"];
          else
            args.Value = EFPApp.MainImages.Images[PlantTools.GetActionImageKey(kind)];
          break;
        case "ActionName":
          kind = (ActionKind)DataTools.GetInt(args.DataRow, "ActionKind");
          if (kind == ActionAdd)
            args.Value = "Поступление";
          else if (kind != ActionNone)
            args.Value = PlantTools.GetActionName(kind);
          break;
      }
    }

    void MainPage_EditData(object sender, EventArgs args)
    {
      if (!_MainPage.ControlProvider.CheckSingleRow())
        return;
      Int32 plantId = DataTools.GetInt(_MainPage.ControlProvider.CurrentDataRow, "PlantId");
      ProgramDBUI.TheUI.DocTypes["Plants"].PerformEditing(plantId, _MainPage.ControlProvider.State == EFPDataGridViewState.View);
    }

    void MainPage_GetDocSel(object sender, EFPDBxGridViewDocSelEventArgs args)
    {
      args.AddFromColumn("Plants", "PlantId");
    }

    #endregion
  }
}
