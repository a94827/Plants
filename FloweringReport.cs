using System;
using System.Collections.Generic;
using System.Text;
using FreeLibSet.Forms;
using FreeLibSet.Forms.Docs;
using System.Data;
using FreeLibSet.Data;
using FreeLibSet.Data.Docs;
using FreeLibSet.Calendar;
using FreeLibSet.Core;

namespace Plants
{
  public class FloweringReportParams : EFPReportParams
  {
    #region Поля

    public DateTime? FirstDate;
    public DateTime? LastDate;

    #endregion

    #region Переопределенные методы

    protected override void OnInitTitle()
    {
      base.Title = "Цветение за " + DateRangeFormatter.Default.ToString(FirstDate, LastDate, false);
    }

    public override void ReadConfig(FreeLibSet.Config.CfgPart Config)
    {
      FirstDate = Config.GetNullableDate("FirstDate");
      LastDate = Config.GetNullableDate("LastDate");
    }

    public override void WriteConfig(FreeLibSet.Config.CfgPart Config)
    {
      Config.SetNullableDate("FirstDate", FirstDate);
      Config.SetNullableDate("LastDate", LastDate);
    }

    #endregion
  }

  public class FloweringReport : EFPReport
  {
    #region Конструктор

    public FloweringReport()
      : base("FloweringReport")
    {
      MainImageKey = "FloweringReport";

      MainPage = new EFPReportVarGridPage();
      MainPage.Title = "Цветение";
      MainPage.ImageKey = MainImageKey;
      Pages.Add(MainPage);
    }

    #endregion

    #region Построение отчета

    protected override EFPReportParams CreateParams()
    {
      return new FloweringReportParams();
    }

    public FloweringReportParams Params { get { return (FloweringReportParams)(base.ReportParams); } }

    protected override bool QueryParams()
    {
      DateRangeDialog dlg = new DateRangeDialog();
      dlg.Title = "Цветение растений";
      dlg.Prompt = "За период";
      dlg.CanBeEmpty = true;
      dlg.NFirstDate = Params.FirstDate;
      dlg.NLastDate = Params.LastDate;
      if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
        return false;
      Params.FirstDate = dlg.NFirstDate;
      Params.LastDate = dlg.NLastDate;
      return true;
    }

    protected override void BuildReport()
    {
      List<DBxFilter> Filters = new List<DBxFilter>();

      PlantTools.AddDateRangeFilter(Filters, Params.FirstDate, Params.LastDate);
      Filters.Add(DBSSubDocType.DeletedFalseFilter);
      Filters.Add(DBSSubDocType.DocIdDeletedFalseFilter);

      DataTable Table1 = ProgramDBUI.TheUI.DocProvider.FillSelect("PlantFlowering",
        new DBxColumns("DocId,Date1,Date2,FlowerCount,DocId.Number"),
        AndFilter.FromList(Filters), null);

      // Идентификаторы растений
      Table1.DefaultView.Sort = "DocId.Number"; // по номеру в каталоге
      IdList PlantIds = IdList.FromColumn(Table1.DefaultView, "DocId");

      DataTable MainTable = new DataTable();
      MainTable.Columns.Add("Date1", typeof(DateTime));
      MainTable.Columns.Add("Date2", typeof(DateTime));
      foreach (Int32 PlantId in PlantIds)
        MainTable.Columns.Add("FlowerCount" + PlantId.ToString(), typeof(int));
      MainTable.Columns.Add("Total", typeof(int));

      DataTools.SetPrimaryKey(MainTable, "Date1,Date2");

      foreach (DataRow SrcRow in Table1.Rows)
      {
        DataRow MainRow = DataTools.FindOrAddPrimaryKeyRow(MainTable, new object[] { SrcRow["Date1"], SrcRow["Date2"] });
        Int32 PlantId = (Int32)(SrcRow["DocId"]);
        int FlowerCount = DataTools.GetInt(SrcRow, "FlowerCount");
        if (FlowerCount == 0)
          FlowerCount = 1;
        DataTools.IncInt(MainRow, "FlowerCount" + PlantId.ToString(), FlowerCount);
      }

      #region Итоги

      foreach (DataRow MainRow in MainTable.Rows)
      {
        int cnt = 0;
        foreach (Int32 PlantId in PlantIds)
          cnt += DataTools.GetInt(MainRow, "FlowerCount" + PlantId.ToString());
        MainRow["Total"] = cnt;
      }

      DataRow TotalRow = MainTable.NewRow();
      TotalRow["Date1"] = DateTime.MaxValue;
      TotalRow["Date2"] = DateTime.MaxValue;
      foreach (Int32 PlantId in PlantIds)
        DataTools.SumInt(TotalRow, "FlowerCount" + PlantId.ToString());
      DataTools.SumInt(TotalRow, "Total");
      MainTable.Rows.Add(TotalRow);

      #endregion

      MainTable.DefaultView.Sort = "Date1,Date2";

      EFPDBxGridView ghMain = new EFPDBxGridView(MainPage.BaseProvider,
        new System.Windows.Forms.DataGridView(), ProgramDBUI.TheUI);

      ghMain.Control.AutoGenerateColumns = false;
      ghMain.Columns.AddText("Date", false, "Дата", 21, 8);
      foreach (Int32 PlantId in PlantIds)
      {
        object[] a = ProgramDBUI.TheUI.DocTypes["Plants"].TableCache.GetValues(PlantId, new DBxColumns("Number,Name"));
        ghMain.Columns.AddInt("FlowerCount" + PlantId.ToString(), true, "№" + DataTools.GetInt(a[0]).ToString(ProgramDBUI.Settings.NumberMask) +
          Environment.NewLine + DataTools.GetString(a[1]), 4);
      }
      ghMain.Columns.AddInt("Total", true, "Всего", 4);
      ghMain.Columns.LastAdded.ColorType = EFPDataGridViewColorType.Total1;

      ghMain.FrozenColumns = 1;
      ghMain.DisableOrdering();
      ghMain.GetRowAttributes += new EFPDataGridViewRowAttributesEventHandler(ghMain_GetRowAttributes);
      ghMain.GetCellAttributes += new EFPDataGridViewCellAttributesEventHandler(ghMain_GetCellAttributes);

      ghMain.Control.MultiSelect = true;

      ghMain.ReadOnly = false;
      ghMain.CanView = true;
      ghMain.Control.ReadOnly = true;
      ghMain.CanInsert = false;
      ghMain.CanDelete = false;
      ghMain.CanMultiEdit = true;
      ghMain.EditData += new EventHandler(ghMain_EditData);
      ghMain.Control.DataSource = MainTable.DefaultView;

      MainPage.ControlProvider = ghMain;
    }

    #endregion

    #region Страница отчета

    EFPReportVarGridPage MainPage;

    void ghMain_GetRowAttributes(object Sender, EFPDataGridViewRowAttributesEventArgs Args)
    {
      DateTime Date1 = DataTools.GetNullableDateTime(Args.DataRow, "Date1").Value;
      if (Date1 == DateTime.MaxValue)
        Args.ColorType = EFPDataGridViewColorType.TotalRow;
    }

    void ghMain_GetCellAttributes(object Sender, EFPDataGridViewCellAttributesEventArgs Args)
    {
      if (Args.ColumnName == "Date")
      {
        DateTime Date1 = DataTools.GetNullableDateTime(Args.DataRow, "Date1").Value;
        DateTime Date2 = DataTools.GetNullableDateTime(Args.DataRow, "Date2").Value;
        if (Date1 == DateTime.MaxValue)
          Args.Value = "Итого";
        else
          Args.Value = DateRangeFormatter.Default.ToString(Date1, Date2, false);
      }

      ////return;
      //switch ((Args.ColumnIndex + Args.RowIndex) % 4)
      //{ 
      //  case 1:
      //    Args.DiagonalUpBorder = EFPDataGridViewBorderStyle.Thin;
      //    break;
      //  case 2:
      //    Args.DiagonalDownBorder = EFPDataGridViewBorderStyle.Thin;
      //    break;
      //  case 3:
      //    Args.DiagonalUpBorder = EFPDataGridViewBorderStyle.Thick;
      //    Args.DiagonalDownBorder = EFPDataGridViewBorderStyle.Thick;
      //    break;
      //}
    }

    void ghMain_EditData(object Sender, EventArgs Args)
    {
      EFPDataGridViewColumn[] Cols = MainPage.ControlProvider.SelectedColumns;
      IdList PlanIds = new IdList();
      for (int i = 0; i < Cols.Length; i++)
      {
        if (Cols[i].Name.StartsWith("FlowerCount"))
        {
          string sId = Cols[i].Name.Substring("FlowerCount".Length);
          Int32 Id = Int32.Parse(sId);
          PlanIds.Add(Id);
        }
      }

      if (PlanIds.Count == 0)
      {
        EFPApp.ShowTempMessage("Растения не выбраны");
        return;
      }

      ProgramDBUI.TheUI.DocTypes["Plants"].PerformEditing(PlanIds.ToArray(), MainPage.ControlProvider.State, false);
    }

    #endregion
  }
}
