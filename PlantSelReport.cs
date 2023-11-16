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
using FreeLibSet.Config;

namespace Plants
{
  internal partial class PlantSelReportParamForm : EFPReportExtParamsTwoPageForm
  {
    #region Конструктор формы

    public PlantSelReportParamForm()
    {
      InitializeComponent();

      efpPeriod = new EFPDateRangeBox(base.FormProvider, edPeriod);
      efpPeriod.First.CanBeEmpty = false;
      efpPeriod.Last.CanBeEmpty = false;
    }

    #endregion

    #region Поля

    public EFPDateRangeBox efpPeriod;

    #endregion
  }

  internal class PlantSelReportParams : EFPReportExtParams
  {
    #region Конструктор

    public PlantSelReportParams()
    {
      FirstDate = LastDate = DateTime.Today;
      Filters = new PlantReportFilters(String.Empty, String.Empty);
    }

    #endregion

    #region Поля

    public DateTime FirstDate;

    public DateTime LastDate;

    public DateRange Period { get { return new DateRange(FirstDate, LastDate); } }

    public PlantReportFilters Filters;

    #endregion

    #region Переопределенные методы

    protected override void OnInitTitle()
    {
      base.Title = "Выборка растений за " + DateRangeFormatter.Default.ToString(FirstDate, LastDate, true);
      Filters.AddFilterInfo(FilterInfo);
    }

    public override EFPReportExtParamsForm CreateForm()
    {
      return new PlantSelReportParamForm();
    }

    public override SettingsPart UsedParts
    {
      get { return SettingsPart.User | SettingsPart.NoHistory; }
    }

    public override void WriteFormValues(EFPReportExtParamsForm form, SettingsPart part)
    {
      PlantSelReportParamForm form2 = (PlantSelReportParamForm)form;
      form2.efpPeriod.First.Value = FirstDate;
      form2.efpPeriod.Last.Value = LastDate;
      form2.FiltersControlProvider.Filters = Filters;
    }

    public override void ReadFormValues(EFPReportExtParamsForm form, SettingsPart part)
    {
      PlantSelReportParamForm form2 = (PlantSelReportParamForm)form;
      FirstDate = form2.efpPeriod.First.Value;
      LastDate = form2.efpPeriod.Last.Value;
    }

    public override void WriteConfig(FreeLibSet.Config.CfgPart cfg, SettingsPart part)
    {
      switch (part)
      {
        case SettingsPart.User:
          Filters.WriteConfig(cfg);
          break;
        case SettingsPart.NoHistory:
          cfg.SetNullableDate("FirstDate", FirstDate);
          cfg.SetNullableDate("LastDate", LastDate);
          break;
      }
    }

    public override void ReadConfig(FreeLibSet.Config.CfgPart cfg, SettingsPart part)
    {
      switch (part)
      {
        case SettingsPart.User:
          Filters.ReadConfig(cfg);
          break;
        case SettingsPart.NoHistory:
          cfg.GetDate("FirstDate", ref FirstDate);
          cfg.GetDate("LastDate", ref LastDate);
          break;
      }
    }

    #endregion
  }

  internal class PlantSelReport : EFPReport
  {
    #region Конструктор

    public PlantSelReport()
      : base("PlantSelReport")
    {
      MainImageKey = "PlantSelReport";

      _MainPage = new EFPReportDocGridPage(ProgramDBUI.TheUI.DocTypes["Plants"]);
      _MainPage.InitGrid += new EventHandler(MainPage_InitGrid);
      _MainPage.ShowToolBar = true;
      Pages.Add(_MainPage);
    }

    #endregion

    #region Запрос параметров

    protected override EFPReportParams CreateParams()
    {
      return new PlantSelReportParams();
    }

    public PlantSelReportParams Params { get { return (PlantSelReportParams)(base.ReportParams); } }

    #endregion

    #region Построение отчета

    protected override void BuildReport()
    {
      DBxFilter sqlFilter = Params.Filters.GetSqlFilter();
      if (ProgramDBUI.TheUI.DocProvider.DocTypes.UseDeleted)
        sqlFilter &= DBSDocType.DeletedFalseFilter;

      DBxColumnList colNames = new DBxColumnList();
      colNames.Add("Id");
      ProgramDBUI.TheUI.DocTypes["Plants"].GridProducer.GetColumnNames("Plants", "", colNames);

      DataTable table = ProgramDBUI.TheUI.DocProvider.FillSelect("Plants",
        new DBxColumns(colNames),
        sqlFilter,
        DBxOrder.FromDataViewSort("Number,Name,Id"));

      Params.Filters.PerformAuxFiltering(ref table, Params.FirstDate, Params.LastDate);

      _MainPage.DataSource = table.DefaultView;
    }

    #endregion

    #region Страница отчета

    EFPReportDocGridPage _MainPage;

    void MainPage_InitGrid(object sender, EventArgs args)
    {
    }


    #endregion
  }
}
