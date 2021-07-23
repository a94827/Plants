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
  internal partial class PlantSelReportParamForm : EFPReportExtParamsTwoPageForm
  {
    #region Конструктор формы

    public PlantSelReportParamForm()
    {
      InitializeComponent();

      efpPeriod = new EFPDateRangeBox(base.FormProvider, edPeriod);
      efpPeriod.FirstDate.CanBeEmpty = false;
      efpPeriod.LastDate.CanBeEmpty = false;
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
      Filters = new PlantReportFilters(String.Empty);
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

    public override EFPReportExtParamsPart UsedParts
    {
      get { return EFPReportExtParamsPart.User | EFPReportExtParamsPart.NoHistory; }
    }

    public override void WriteFormValues(EFPReportExtParamsForm Form, EFPReportExtParamsPart Part)
    {
      PlantSelReportParamForm Form2 = (PlantSelReportParamForm)Form;
      Form2.efpPeriod.FirstDate.Value = FirstDate;
      Form2.efpPeriod.LastDate.Value = LastDate;
      Form2.FiltersControlProvider.Filters = Filters;
    }

    public override void ReadFormValues(EFPReportExtParamsForm Form, EFPReportExtParamsPart Part)
    {
      PlantSelReportParamForm Form2 = (PlantSelReportParamForm)Form;
      FirstDate = Form2.efpPeriod.FirstDate.Value.Value;
      LastDate = Form2.efpPeriod.LastDate.Value.Value;
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
          Config.GetDate("FirstDate", ref FirstDate);
          Config.GetDate("LastDate", ref LastDate);
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

      MainPage = new EFPReportDocGridPage(ProgramDBUI.TheUI.DocTypes["Plants"]);
      MainPage.InitGrid += new EventHandler(MainPage_InitGrid);
      MainPage.ShowToolBar = true;
      Pages.Add(MainPage);
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
      DBxFilter SqlFilter = Params.Filters.GetSqlFilter();
      if (ProgramDBUI.TheUI.DocProvider.DocTypes.UseDeleted)
        SqlFilter &= DBSDocType.DeletedFalseFilter;

      DBxColumnList ColNames = new DBxColumnList();
      ColNames.Add("Id");
      ProgramDBUI.TheUI.DocTypes["Plants"].GridProducer.GetColumnNames("Plants", "", ColNames);

      DataTable Table = ProgramDBUI.TheUI.DocProvider.FillSelect("Plants",
        new DBxColumns(ColNames),
        SqlFilter,
        DBxOrder.FromDataViewSort("Number,Name,Id"));

      Params.Filters.PerformAuxFiltering(ref Table, Params.FirstDate, Params.LastDate);

      MainPage.DataSource = Table.DefaultView;
    }

    #endregion

    #region Страница отчета

    EFPReportDocGridPage MainPage;

    void MainPage_InitGrid(object Sender, EventArgs Args)
    {
    }


    #endregion
  }
}