using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AgeyevAV.ExtForms;
using AgeyevAV;
using AgeyevAV.ExtDB;
using AgeyevAV.ExtDB.Docs;
using AgeyevAV.ExtForms.Docs;

namespace Plants
{
  internal partial class PlanReportParamForm : EFPReportExtParamsTwoPageForm
  {
    #region Конструктор формы

    public PlanReportParamForm()
    {
      InitializeComponent();

      efpPeriod = new EFPDateRangeBox(base.FormProvider, edPeriod);
      efpPeriod.FirstDate.CanBeEmpty = true;
      efpPeriod.LastDate.CanBeEmpty = true;
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
      Filters = new PlantReportFilters("DocId.");
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

    public override void WriteFormValues(EFPReportExtParamsForm Form, EFPReportExtParamsPart Part)
    {
      PlanReportParamForm Form2 = (PlanReportParamForm)Form;
      Form2.efpPeriod.FirstDate.Value = FirstDate;
      Form2.efpPeriod.LastDate.Value = LastDate;
      Form2.FiltersControlProvider.Filters = Filters;
    }

    public override void ReadFormValues(EFPReportExtParamsForm Form, EFPReportExtParamsPart Part)
    {
      PlanReportParamForm Form2 = (PlanReportParamForm)Form;
      FirstDate = Form2.efpPeriod.FirstDate.Value;
      LastDate = Form2.efpPeriod.LastDate.Value;
    }

    public override void WriteConfig(AgeyevAV.Config.CfgPart Config, EFPReportExtParamsPart Part)
    {
      switch (Part)
      {
        case EFPReportExtParamsPart.User:
          Filters.WriteConfig(Config);
          break;
        case EFPReportExtParamsPart.NoHistory:
          Config.SetNullableDate("FirstDate", FirstDate);
          Config.SetNullableDate("LastDate", LastDate);
          break;
      }
    }

    public override void ReadConfig(AgeyevAV.Config.CfgPart Config, EFPReportExtParamsPart Part)
    {
      switch (Part)
      {
        case EFPReportExtParamsPart.User:
          Filters.ReadConfig(Config);
          break;
        case EFPReportExtParamsPart.NoHistory:
          FirstDate = Config.GetNullableDate("FirstDate");
          LastDate = Config.GetNullableDate("LastDate");
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

      MainPage = new EFPReportDBxGridPage(ProgramDBUI.TheUI);
      MainPage.InitGrid += new EventHandler(MainPage_InitGrid);
      MainPage.ShowToolBar = true;
      Pages.Add(MainPage);
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
      DataTable Table = CreateTable(Params.FirstDate, Params.LastDate, Params.Filters);
      MainPage.DataSource = Table.DefaultView;
    }

    public static DataTable CreateTable(DateTime? FirstDate, DateTime? LastDate, PlantReportFilters ReportFilters)
    {
      List<DBxFilter> Filters = new List<DBxFilter>();
      if (FirstDate.HasValue)
        Filters.Add(new ValueFilter("Date2", FirstDate.Value, CompareKind.GreaterOrEqualThan));
      if (LastDate.HasValue)
        Filters.Add(new ValueFilter("Date1", LastDate.Value, CompareKind.LessOrEqualThan));
      if (ProgramDBUI.TheUI.DocProvider.DocTypes.UseDeleted)
      {
        Filters.Add(DBSSubDocType.DeletedFalseFilter);
        Filters.Add(DBSSubDocType.DocIdDeletedFalseFilter);
      }
      if (ReportFilters != null)
      {
        DBxFilter Filters2 = ReportFilters.GetSqlFilter();
        if (Filters2 != null)
          Filters.Add(Filters2);
      }

      DataTable Table = ProgramDBUI.TheUI.DocProvider.FillSelect("PlantPlans",
        new DBxColumns("Id,Kind,ActionName,Date1,Date2,Remedy.Name,Comment,DocId,DocId.Name,DocId.Number"),
        AndFilter.FromList(Filters),
        DBxOrder.FromDataViewSort("DocId.Number,DocId.Name,DocId,Date1,Id"));
      if (ReportFilters != null)
        ReportFilters.PerformAuxFiltering(ref Table, FirstDate, LastDate);

      return Table;

    }

    #endregion

    #region Страница отчета

    EFPReportDBxGridPage MainPage;

    void MainPage_InitGrid(object Sender, EventArgs Args)
    {
      MainPage.ControlProvider.Control.AutoGenerateColumns = false;
      MainPage.ControlProvider.Columns.AddImage("ActionImage");
      MainPage.ControlProvider.Columns.AddText("Period", false, "Период", 15, 10);
      MainPage.ControlProvider.Columns.AddText("ActionText", false, "Действие", 30, 10);

      MainPage.ControlProvider.Columns.AddText("DocId.Number", true, "№ по каталогу", 3, 3);
      //MainPage.ControlProvider.Columns.LastAdded.Format = PlantTools.NumberMask ;
      MainPage.ControlProvider.Columns.LastAdded.CanIncSearch = true;
      MainPage.ControlProvider.Columns.LastAdded.TextAlign = HorizontalAlignment.Center;

      MainPage.ControlProvider.Columns.AddTextFill("DocId.Name", true, "Название или описание", 100, 15);
      MainPage.ControlProvider.Columns.LastAdded.CanIncSearch = true;
      MainPage.ControlProvider.DisableOrdering();

      MainPage.ControlProvider.GetCellAttributes += new EFPDataGridViewCellAttributesEventHandler(MainPage_GetCellAttributes);

      MainPage.ControlProvider.ReadOnly = false;
      MainPage.ControlProvider.Control.ReadOnly = true;
      MainPage.ControlProvider.CanInsert = false;
      MainPage.ControlProvider.CanDelete = false;
      MainPage.ControlProvider.EditData += new EventHandler(MainPage_EditData);

      MainPage.ControlProvider.GetDocSel += new EFPDBxGridViewDocSelEventHandler(MainPage_GetDocSel);
    }

    void MainPage_GetCellAttributes(object Sender, EFPDataGridViewCellAttributesEventArgs Args)
    {
      ActionKind Kind;
      switch (Args.ColumnName)
      {
        case "Period":
          DateTime dt1 = DataTools.GetDateTime(Args.DataRow, "Date1");
          DateTime dt2 = DataTools.GetDateTime(Args.DataRow, "Date2");
          Args.Value = DateRangeFormatter.Default.ToString(dt1, dt2, false);
          break;
        case "ActionImage":
          Kind = (ActionKind)DataTools.GetInt(Args.DataRow, "Kind");
          Args.Value = EFPApp.MainImages.Images[PlantTools.GetActionImageKey(Kind)];
          break;
        case "ActionText":
          Kind = (ActionKind)DataTools.GetInt(Args.DataRow, "Kind");
          Args.Value = PlantTools.GetActionName(Kind,
            DataTools.GetString(Args.DataRow, "ActionName"),
            DataTools.GetString(Args.DataRow, "Remedy.Name"));
          break;
      }
    }

    void MainPage_EditData(object Sender, EventArgs Args)
    {
      Int32[] DocIds = DataTools.GetIdsFromField(MainPage.ControlProvider.SelectedDataRows, "DocId");
      if (DocIds.Length == 0)
        EFPApp.ShowTempMessage("Нет выбранных растений");
      ProgramDBUI.TheUI.DocTypes["Plants"].PerformEditing(DocIds, MainPage.ControlProvider.State, false);
    }

    void MainPage_GetDocSel(object Sender, EFPDBxGridViewDocSelEventArgs Args)
    {
      Args.AddFromColumn("Plants", "DocId");
    }

    #endregion

    #region Показ при запуске программы

    public static void ShowOnStart()
    {
      if (CreateTable(null, DateTime.Today, null).Rows.Count == 0)
        return;

      PlanReportParams Params = new PlanReportParams();
      Params.FirstDate = null;
      Params.LastDate = DateTime.Today;
      PlanReport Report = new PlanReport();
      Report.ReportParams = Params;
      Report.Run();
    }

    #endregion
  }
}