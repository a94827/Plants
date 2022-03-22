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

    public override void ReadConfig(FreeLibSet.Config.CfgPart cfg)
    {
      FirstDate = cfg.GetNullableDate("FirstDate");
      LastDate = cfg.GetNullableDate("LastDate");
    }

    public override void WriteConfig(FreeLibSet.Config.CfgPart cfg)
    {
      cfg.SetNullableDate("FirstDate", FirstDate);
      cfg.SetNullableDate("LastDate", LastDate);
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

      _MainPage = new EFPReportVarGridPage();
      _MainPage.Title = "Цветение";
      _MainPage.ImageKey = MainImageKey;
      Pages.Add(_MainPage);
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
      List<DBxFilter> filters = new List<DBxFilter>();

      PlantTools.AddDateRangeFilter(filters, Params.FirstDate, Params.LastDate);
      filters.Add(DBSSubDocType.DeletedFalseFilter);
      filters.Add(DBSSubDocType.DocIdDeletedFalseFilter);

      DataTable table1 = ProgramDBUI.TheUI.DocProvider.FillSelect("PlantFlowering",
        new DBxColumns("DocId,Date1,Date2,FlowerCount,DocId.Number"),
        AndFilter.FromList(filters), null);

      // Идентификаторы растений
      table1.DefaultView.Sort = "DocId.Number"; // по номеру в каталоге
      IdList plantIds = IdList.FromColumn(table1.DefaultView, "DocId");

      DataTable mainTable = new DataTable();
      mainTable.Columns.Add("Date1", typeof(DateTime));
      mainTable.Columns.Add("Date2", typeof(DateTime));
      foreach (Int32 PlantId in plantIds)
        mainTable.Columns.Add("FlowerCount" + PlantId.ToString(), typeof(int));
      mainTable.Columns.Add("Total", typeof(int));

      DataTools.SetPrimaryKey(mainTable, "Date1,Date2");

      foreach (DataRow srcRow in table1.Rows)
      {
        DataRow mainRow = DataTools.FindOrAddPrimaryKeyRow(mainTable, new object[] { srcRow["Date1"], srcRow["Date2"] });
        Int32 plantId = (Int32)(srcRow["DocId"]);
        int flowerCount = DataTools.GetInt(srcRow, "FlowerCount");
        if (flowerCount == 0)
          flowerCount = 1;
        DataTools.IncInt(mainRow, "FlowerCount" + plantId.ToString(), flowerCount);
      }

      #region Итоги

      foreach (DataRow mainRow in mainTable.Rows)
      {
        int cnt = 0;
        foreach (Int32 PlantId in plantIds)
          cnt += DataTools.GetInt(mainRow, "FlowerCount" + PlantId.ToString());
        mainRow["Total"] = cnt;
      }

      DataRow totalRow = mainTable.NewRow();
      totalRow["Date1"] = DateTime.MaxValue;
      totalRow["Date2"] = DateTime.MaxValue;
      foreach (Int32 PlantId in plantIds)
        DataTools.SumInt(totalRow, "FlowerCount" + PlantId.ToString());
      DataTools.SumInt(totalRow, "Total");
      mainTable.Rows.Add(totalRow);

      #endregion

      mainTable.DefaultView.Sort = "Date1,Date2";

      EFPDBxGridView ghMain = new EFPDBxGridView(_MainPage.BaseProvider,
        new System.Windows.Forms.DataGridView(), ProgramDBUI.TheUI);

      ghMain.Control.AutoGenerateColumns = false;
      ghMain.Columns.AddText("Date", false, "Дата", 21, 8);
      foreach (Int32 plantId in plantIds)
      {
        object[] a = ProgramDBUI.TheUI.DocTypes["Plants"].TableCache.GetValues(plantId, new DBxColumns("Number,Name"));
        ghMain.Columns.AddInt("FlowerCount" + plantId.ToString(), true, "№" + DataTools.GetInt(a[0]).ToString(ProgramDBUI.Settings.NumberMask) +
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
      ghMain.Control.DataSource = mainTable.DefaultView;

      _MainPage.ControlProvider = ghMain;
    }

    #endregion

    #region Страница отчета

    EFPReportVarGridPage _MainPage;

    void ghMain_GetRowAttributes(object sender, EFPDataGridViewRowAttributesEventArgs args)
    {
      DateTime date1 = DataTools.GetNullableDateTime(args.DataRow, "Date1").Value;
      if (date1 == DateTime.MaxValue)
        args.ColorType = EFPDataGridViewColorType.TotalRow;
    }

    void ghMain_GetCellAttributes(object sender, EFPDataGridViewCellAttributesEventArgs args)
    {
      if (args.ColumnName == "Date")
      {
        DateTime Date1 = DataTools.GetNullableDateTime(args.DataRow, "Date1").Value;
        DateTime Date2 = DataTools.GetNullableDateTime(args.DataRow, "Date2").Value;
        if (Date1 == DateTime.MaxValue)
          args.Value = "Итого";
        else
          args.Value = DateRangeFormatter.Default.ToString(Date1, Date2, false);
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

    void ghMain_EditData(object sender, EventArgs args)
    {
      EFPDataGridViewColumn[] cols = _MainPage.ControlProvider.SelectedColumns;
      IdList plantIds = new IdList();
      for (int i = 0; i < cols.Length; i++)
      {
        if (cols[i].Name.StartsWith("FlowerCount"))
        {
          string sId = cols[i].Name.Substring("FlowerCount".Length);
          Int32 id = Int32.Parse(sId);
          plantIds.Add(id);
        }
      }

      if (plantIds.Count == 0)
      {
        EFPApp.ShowTempMessage("Растения не выбраны");
        return;
      }

      ProgramDBUI.TheUI.DocTypes["Plants"].PerformEditing(plantIds.ToArray(), _MainPage.ControlProvider.State, false);
    }

    #endregion
  }
}
