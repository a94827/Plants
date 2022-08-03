using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FreeLibSet.Forms;
using FreeLibSet.Data;
using FreeLibSet.Data.Docs;
using FreeLibSet.Forms.Docs;
using FreeLibSet.Calendar;
using FreeLibSet.Core;

namespace Plants
{
  internal partial class PlanReportParamForm : EFPReportExtParamsTwoPageForm
  {
    #region Конструктор формы

    public PlanReportParamForm()
    {
      InitializeComponent();

      efpPeriod = new EFPDateRangeBox(base.FormProvider, edPeriod);
      efpPeriod.First.CanBeEmpty = true;
      efpPeriod.Last.CanBeEmpty = true;
    }

    #endregion

    #region Поля

    public EFPDateRangeBox efpPeriod;

    #endregion
  }

  internal class PlanReportParams : EFPReportExtParams
  {
    #region Конструктор

    public PlanReportParams()
    {
      Filters = new PlantReportFilters("DocId.", String.Empty);
    }

    #endregion

    #region Поля

    public DateTime? FirstDate;

    public DateTime? LastDate;

    public PlantReportFilters Filters;

    #endregion

    #region Переопределенные методы

    protected override void OnInitTitle()
    {
      base.Title = "Планируемые действия - " + DateRangeFormatter.Default.ToString(FirstDate, LastDate, true);
      Filters.AddFilterInfo(FilterInfo);
    }

    public override EFPReportExtParamsForm CreateForm()
    {
      return new PlanReportParamForm();
    }

    public override EFPReportExtParamsPart UsedParts
    {
      get { return EFPReportExtParamsPart.User | EFPReportExtParamsPart.NoHistory; }
    }

    public override void WriteFormValues(EFPReportExtParamsForm form, EFPReportExtParamsPart part)
    {
      PlanReportParamForm form2 = (PlanReportParamForm)form;
      form2.efpPeriod.First.NValue = FirstDate;
      form2.efpPeriod.Last.NValue = LastDate;
      form2.FiltersControlProvider.Filters = Filters;
    }

    public override void ReadFormValues(EFPReportExtParamsForm form, EFPReportExtParamsPart part)
    {
      PlanReportParamForm form2 = (PlanReportParamForm)form;
      FirstDate = form2.efpPeriod.First.NValue;
      LastDate = form2.efpPeriod.Last.NValue;
    }

    public override void WriteConfig(FreeLibSet.Config.CfgPart cfg, EFPReportExtParamsPart part)
    {
      switch (part)
      {
        case EFPReportExtParamsPart.User:
          Filters.WriteConfig(cfg);
          break;
        case EFPReportExtParamsPart.NoHistory:
          cfg.SetNullableDate("FirstDate", FirstDate);
          cfg.SetNullableDate("LastDate", LastDate);
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
        case EFPReportExtParamsPart.NoHistory:
          FirstDate = cfg.GetNullableDate("FirstDate");
          LastDate = cfg.GetNullableDate("LastDate");
          break;
      }
    }

    #endregion
  }

  internal class PlanReport : EFPReport
  {
    #region Конструктор

    public PlanReport()
      : base("PlanReport")
    {
      MainImageKey = "PlanReport";

      _MainPage = new EFPReportDBxGridPage(ProgramDBUI.TheUI);
      _MainPage.InitGrid += new EventHandler(MainPage_InitGrid);
      _MainPage.ShowToolBar = true;
      Pages.Add(_MainPage);
    }

    #endregion

    #region Запрос параметров

    protected override EFPReportParams CreateParams()
    {
      return new PlanReportParams();
    }

    public PlanReportParams Params { get { return (PlanReportParams)(base.ReportParams); } }

    #endregion

    #region Построение отчета

    protected override void BuildReport()
    {
      DataTable table = CreateTable(Params.FirstDate, Params.LastDate, Params.Filters);
      _MainPage.DataSource = table.DefaultView;
    }

    public static DataTable CreateTable(DateTime? firstDate, DateTime? lastDate, PlantReportFilters reportFilters)
    {
      List<DBxFilter> filters = new List<DBxFilter>();
      if (firstDate.HasValue)
        filters.Add(new ValueFilter("Date2", firstDate.Value, CompareKind.GreaterOrEqualThan));
      if (lastDate.HasValue)
        filters.Add(new ValueFilter("Date1", lastDate.Value, CompareKind.LessOrEqualThan));
      if (ProgramDBUI.TheUI.DocProvider.DocTypes.UseDeleted)
      {
        filters.Add(DBSSubDocType.DeletedFalseFilter);
        filters.Add(DBSSubDocType.DocIdDeletedFalseFilter);
      }
      if (reportFilters != null)
      {
        DBxFilter filters2 = reportFilters.GetSqlFilter();
        if (filters2 != null)
          filters.Add(filters2);
      }

      DataTable table = ProgramDBUI.TheUI.DocProvider.FillSelect("PlantPlans",
        new DBxColumns("Id,Kind,ActionName,Date1,Date2,Remedy.Name,Comment,DocId,DocId.Name,DocId.Number"),
        AndFilter.FromList(filters),
        DBxOrder.FromDataViewSort("DocId.Number,DocId.Name,DocId,Date1,Id"));
      if (reportFilters != null)
        reportFilters.PerformAuxFiltering(ref table, firstDate, lastDate);

      return table;

    }

    #endregion

    #region Страница отчета

    EFPReportDBxGridPage _MainPage;

    void MainPage_InitGrid(object sender, EventArgs args)
    {
      _MainPage.ControlProvider.Control.AutoGenerateColumns = false;
      _MainPage.ControlProvider.Columns.AddImage("ActionImage");
      _MainPage.ControlProvider.Columns.AddText("Period", false, "Период", 15, 10);
      _MainPage.ControlProvider.Columns.AddText("ActionText", false, "Действие", 30, 10);

      _MainPage.ControlProvider.Columns.AddText("DocId.Number", true, "№ по каталогу", 3, 3);
      //MainPage.ControlProvider.Columns.LastAdded.Format = PlantTools.NumberMask ;
      _MainPage.ControlProvider.Columns.LastAdded.CanIncSearch = true;
      _MainPage.ControlProvider.Columns.LastAdded.TextAlign = HorizontalAlignment.Center;

      _MainPage.ControlProvider.Columns.AddTextFill("DocId.Name", true, "Название или описание", 100, 15);
      _MainPage.ControlProvider.Columns.LastAdded.CanIncSearch = true;
      _MainPage.ControlProvider.DisableOrdering();

      _MainPage.ControlProvider.GetCellAttributes += new EFPDataGridViewCellAttributesEventHandler(MainPage_GetCellAttributes);

      _MainPage.ControlProvider.ReadOnly = false;
      _MainPage.ControlProvider.Control.ReadOnly = true;
      _MainPage.ControlProvider.CanInsert = false;
      _MainPage.ControlProvider.CanDelete = false;
      _MainPage.ControlProvider.EditData += new EventHandler(MainPage_EditData);

      _MainPage.ControlProvider.GetDocSel += new EFPDBxGridViewDocSelEventHandler(MainPage_GetDocSel);
    }

    void MainPage_GetCellAttributes(object sender, EFPDataGridViewCellAttributesEventArgs args)
    {
      ActionKind kind;
      switch (args.ColumnName)
      {
        case "Period":
          DateTime dt1 = DataTools.GetDateTime(args.DataRow, "Date1");
          DateTime dt2 = DataTools.GetDateTime(args.DataRow, "Date2");
          args.Value = DateRangeFormatter.Default.ToString(dt1, dt2, false);
          break;
        case "ActionImage":
          kind = (ActionKind)DataTools.GetInt(args.DataRow, "Kind");
          args.Value = EFPApp.MainImages.Images[PlantTools.GetActionImageKey(kind)];
          break;
        case "ActionText":
          kind = (ActionKind)DataTools.GetInt(args.DataRow, "Kind");
          args.Value = PlantTools.GetActionName(kind,
            DataTools.GetString(args.DataRow, "ActionName"),
            DataTools.GetString(args.DataRow, "Remedy.Name"));
          break;
      }
    }

    void MainPage_EditData(object sender, EventArgs args)
    {
      Int32[] docIds = DataTools.GetIdsFromColumn(_MainPage.ControlProvider.SelectedDataRows, "DocId");
      if (docIds.Length == 0)
        EFPApp.ShowTempMessage("Нет выбранных растений");
      ProgramDBUI.TheUI.DocTypes["Plants"].PerformEditing(docIds, _MainPage.ControlProvider.State, false);
    }

    void MainPage_GetDocSel(object sender, EFPDBxGridViewDocSelEventArgs args)
    {
      args.AddFromColumn("Plants", "DocId");
    }

    #endregion

    #region Показ при запуске программы

    public static void ShowOnStart()
    {
      if (CreateTable(null, DateTime.Today, null).Rows.Count == 0)
        return;

      PlanReportParams reportParams = new PlanReportParams();
      reportParams.FirstDate = null;
      reportParams.LastDate = DateTime.Today;
      PlanReport report = new PlanReport();
      report.ReportParams = reportParams;
      report.Run();
    }

    #endregion
  }
}