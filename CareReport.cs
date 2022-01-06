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
using System.Collections;
using FreeLibSet.Calendar;
using FreeLibSet.Core;

namespace Plants
{
  internal partial class CareReportParamForm : EFPReportExtParamsTwoPageForm
  {
    #region ����������� �����

    public CareReportParamForm()
    {
      InitializeComponent();

      efpDay = new EFPMonthDayTextBox(base.FormProvider, edDay);
      efpDay.CanBeEmpty = true;

      efpPeriod = new EFPDateRangeBox(base.FormProvider, edPeriod);
      efpPeriod.First.CanBeEmpty = false;
      efpPeriod.Last.CanBeEmpty = false;
    }

    #endregion

    #region ����

    public EFPMonthDayTextBox efpDay;

    public EFPDateRangeBox efpPeriod;

    #endregion
  }

  internal class CareReportParams : EFPReportExtParams
  {
    #region �����������

    public CareReportParams()
    {
      Day = new MonthDay(DateTime.Today);
      FirstDate = LastDate = DateTime.Today;
      Filters = new PlantReportFilters(String.Empty);
    }

    #endregion

    #region ����

    public MonthDay Day;

    public DateTime FirstDate;

    public DateTime LastDate;

    public DateRange Period { get { return new DateRange(FirstDate, LastDate); } }

    public PlantReportFilters Filters;

    #endregion

    #region ���������������� ������

    protected override void OnInitTitle()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("���� �� ����������");
      if (!Day.IsEmpty)
      {
        sb.Append(" �� ");
        sb.Append(Day.ToString());
      }
      base.Title = sb.ToString();
      Filters.AddFilterInfo(FilterInfo);
      if (Filters.PeriodRequired)
        FilterInfo.Add("������ ��� ��������", DateRangeFormatter.Default.ToString(Period, true));
    }

    public override EFPReportExtParamsForm CreateForm()
    {
      return new CareReportParamForm();
    }

    public override EFPReportExtParamsPart UsedParts
    {
      get { return EFPReportExtParamsPart.User | EFPReportExtParamsPart.NoHistory; }
    }

    public override void WriteFormValues(EFPReportExtParamsForm Form, EFPReportExtParamsPart Part)
    {
      CareReportParamForm Form2 = (CareReportParamForm)Form;
      switch (Part)
      {
        case EFPReportExtParamsPart.User:
          Form2.FiltersControlProvider.Filters = Filters;
          break;
        case EFPReportExtParamsPart.NoHistory:
          Form2.efpDay.Value = Day;
          Form2.efpPeriod.First.Value = FirstDate;
          Form2.efpPeriod.Last.Value = LastDate;
          break;
      }
    }

    public override void ReadFormValues(EFPReportExtParamsForm Form, EFPReportExtParamsPart Part)
    {
      CareReportParamForm Form2 = (CareReportParamForm)Form;
      switch (Part)
      {
        case EFPReportExtParamsPart.NoHistory:
          Day = Form2.efpDay.Value;
          FirstDate = Form2.efpPeriod.First.Value;
          LastDate = Form2.efpPeriod.Last.Value;
          break;
      }
    }

    public override void WriteConfig(FreeLibSet.Config.CfgPart Config, EFPReportExtParamsPart Part)
    {
      switch (Part)
      {
        case EFPReportExtParamsPart.User:
          Filters.WriteConfig(Config);
          break;
        case EFPReportExtParamsPart.NoHistory:
          Config.SetInt("Day", Day.DayOfYear);
          Config.SetNullableDate("FirstDate", FirstDate);
          Config.SetNullableDate("LastDate", LastDate);
          break;
      }
    }

    public override void ReadConfig(FreeLibSet.Config.CfgPart Config, EFPReportExtParamsPart Part)
    {
      switch (Part)
      {
        case EFPReportExtParamsPart.User:
          Filters.ReadConfig(Config);
          break;
        case EFPReportExtParamsPart.NoHistory:
          Day = new MonthDay(Config.GetIntDef("Day", Day.DayOfYear));
          Config.GetDate("FirstDate", ref FirstDate);
          Config.GetDate("LastDate", ref LastDate);
          break;
      }
    }

    #endregion
  }

  internal class CareReport : EFPReport
  {
    #region �����������

    public CareReport()
      : base("CareReport")
    {
      MainImageKey = "CareReport";

      MainPage = new EFPReportDBxGridPage(ProgramDBUI.TheUI);
      MainPage.InitGrid += new EventHandler(MainPage_InitGrid);
      MainPage.ShowToolBar = true;
      Pages.Add(MainPage);
    }

    #endregion

    #region ������ ����������

    protected override EFPReportParams CreateParams()
    {
      return new CareReportParams();
    }

    public CareReportParams Params { get { return (CareReportParams)(base.ReportParams); } }

    #endregion

    #region ���������� ������

    private class RecordSortInfo : IComparable<RecordSortInfo>
    {
      #region ����

      /// <summary>
      /// ������������� ��������� "Care".
      /// � ���������� �� ���������
      /// </summary>
      public Int32 CareDocId;

      /// <summary>
      /// ������������� ������������ "CareRecord".
      /// � ���������� �� ���������
      /// </summary>
      public Int32 CareRecordId;

      /// <summary>
      /// ������ �����.
      /// ��������� ���� ��� ����������
      /// </summary>
      public MonthDayRange Range;

      /// <summary>
      /// �������� �������.
      /// � ���������� �� ���������
      /// </summary>
      public string PeriodName;

      /// <summary>
      /// ������ ��������� "Care" � ������� ������������.
      /// ��������� ���� ��� ����������
      /// </summary>
      public int CareDocHieIndex;

      #endregion

      #region IComparable<RecordSortInfo> Members

      public int CompareTo(RecordSortInfo other)
      {
        int d1 = this.Range.First.DayOfYear;
        int d2 = other.Range.First.DayOfYear;
        if (d1 != d2)
          return d1 - d2;

        return this.CareDocHieIndex - other.CareDocHieIndex;
      }

      #endregion
    }

    protected override void BuildReport()
    {
      DBxFilter SqlFilter = Params.Filters.GetSqlFilter();
      if (ProgramDBUI.TheUI.DocProvider.DocTypes.UseDeleted)
        SqlFilter &= DBSDocType.DeletedFalseFilter;

      DBxColumnList ColNames = new DBxColumnList();
      ColNames.Add("Id,Care");
      ProgramDBUI.TheUI.DocTypes["Plants"].GridProducer.GetColumnNames("Plants", "", ColNames);

      DataTable PlantTable = ProgramDBUI.TheUI.DocProvider.FillSelect("Plants",
        new DBxColumns(ColNames),
        SqlFilter,
        DBxOrder.FromDataViewSort("Number,Name,Id"));

      Params.Filters.PerformAuxFiltering(ref PlantTable, Params.FirstDate, Params.LastDate);
      // ������� ����� ����������

      DBxDocSet CareSet = new DBxDocSet(ProgramDBUI.TheUI.DocProvider);
      DBxMultiDocs mCareDocs = CareSet["Care"];
      Int32[] CareIds = DataTools.GetIdsFromColumn(PlantTable, "Care");
      if (CareIds.Length > 0)
        mCareDocs.View(CareIds);


      DataTable ResTable = new DataTable();
      ResTable.Columns.Add("PlantId", typeof(Int32));
      ResTable.Columns.Add("PlantNumber", typeof(int));
      ResTable.Columns.Add("PlantName", typeof(string));
      ResTable.Columns.Add("CareId", typeof(Int32)); // � �������� ��������� ������
      ResTable.Columns.Add("CareRecordId", typeof(Int32)); // Id ������
      ResTable.Columns.Add("PeriodText", typeof(string));
      ResTable.Columns.Add("PeriodName", typeof(string));
      ResTable.Columns.Add("ItemName", typeof(string));
      ResTable.Columns.Add("ItemTextValue", typeof(string));
      ResTable.Columns.Add("PlantOrder", typeof(int)); // ������� �������� � ������

      int PlantCount = 0;
      foreach (DataRow PlantRow in PlantTable.Rows)
      {
        // ���� ������ �����������, ���� ���� ��� �������� ��� �����
        DataRow ResRow1 = ResTable.Rows.Add();
        ResRow1["PlantId"] = PlantRow["Id"];
        ResRow1["PlantName"] = PlantRow["Name"];
        ResRow1["PlantNumber"] = PlantRow["Number"];
        PlantCount++;
        ResRow1["PlantOrder"] = PlantCount;

        DBxSingleDoc[] CareDocs = GetCareDocs(mCareDocs, DataTools.GetInt(PlantRow, "Care"));
        if (CareDocs.Length == 0)
          continue; // �� ������ ������ "Care"

        SortedList<RecordSortInfo, CareValues> RecList = new SortedList<RecordSortInfo, CareValues>();

        for (int i = 0; i < CareDocs.Length; i++)
        {
          foreach (DBxSubDoc SubDoc in CareDocs[i].SubDocs["CareRecords"])
          {
            MonthDay md1 = new MonthDay(SubDoc.Values["Day1"].AsInteger);
            MonthDay md2 = new MonthDay(SubDoc.Values["Day2"].AsInteger);
            MonthDayRange ThisRange = new MonthDayRange(md1, md2);
            if ((!Params.Day.IsEmpty) && (!ThisRange.IsEmpty))
            {
              if (!ThisRange.Contains(Params.Day))
                continue;
            }

            CareValues Values = new CareValues(CareItem.TheList);
            Values.Read(SubDoc.Values);

            RecordSortInfo si = new RecordSortInfo();
            si.CareDocId = CareDocs[i].DocId;
            si.CareDocHieIndex = i;
            si.CareRecordId = SubDoc.SubDocId;
            si.Range = ThisRange;
            si.PeriodName = SubDoc.Values["Name"].AsString;
            RecList.Add(si, Values);
          }
        }

        // ������ ����� ���������� ��������������� ������

        // ������ ������ ������ ���� ������������ �� ������� �������������
        for (int j = 0; j < CareItem.TheList.Count; j++)
        {
          if (Params.Day.IsEmpty)
          {
            // ��������� �� ����� ������, ���� ���� �������� ��������
            foreach (KeyValuePair<RecordSortInfo, CareValues> Pair in RecList)
            {
              if (Pair.Value[j] != null)
                AddReportRow(ResTable, Pair.Key, Pair.Value, j);
            }
          }
          else
          {
            // ������� ��������� ����������� ������
            KeyValuePair<RecordSortInfo, CareValues> UsedPair = new KeyValuePair<RecordSortInfo, CareValues>(); // �������������������� ���������
            foreach (KeyValuePair<RecordSortInfo, CareValues> Pair in RecList)
            {
              if (Pair.Value[j] != null)
                UsedPair = Pair;
            }

            if (UsedPair.Key != null)
              AddReportRow(ResTable, UsedPair.Key, UsedPair.Value, j);
          }
        }
      }

      MainPage.DataSource = ResTable.DefaultView;
    }

    private static readonly DBxSingleDoc[] EmptyDocs = new DBxSingleDoc[0];

    /// <summary>
    /// ���������� ������ ���������� �� �����, ������� ������ ���� �����������, � ������ ��������.
    /// ������ � ������� ���� ������������ ��������, ����� ��������, ... ��������� ���� �������� � ��������������� <paramref name="CareId"/>.
    /// ���� <paramref name="CareId"/>=0, ������������ ������ ������
    /// </summary>
    /// <param name="mCareDocs">��������� "Care"</param>
    /// <param name="CareId">������������� ������ �� �����, �������� � ��������</param>
    /// <returns></returns>
    private static DBxSingleDoc[] GetCareDocs(DBxMultiDocs mCareDocs, Int32 CareId)
    {
      if (CareId == 0)
        return EmptyDocs;

      List<DBxSingleDoc> lst = new List<DBxSingleDoc>();
      IdList Ids = new IdList();
      while (CareId != 0)
      {
        DBxSingleDoc Doc;
        if (!mCareDocs.TryGetDocById(CareId, out Doc))
          Doc = mCareDocs.View(CareId); // ��������� ������������ ��������

        if (Ids.Contains(CareId))
        {
          EFPApp.ErrorMessageBox("������� ���������� �� ����� ��������� ��� DocId=" + CareId + ": " + Doc.TextValue);
          break;
        }
        Ids.Add(CareId);
        lst.Insert(0, Doc);
        CareId = Doc.Values["ParentId"].AsInteger;
      }

      return lst.ToArray();
    }

    /// <summary>
    /// ���������� ������ ������.
    /// ��������� ����� ������ ��� ��������� ������ ������ ��� ��������
    /// </summary>
    /// <param name="ResTable">�������</param>
    /// <param name="Pair"></param>
    private void AddReportRow(DataTable ResTable, RecordSortInfo Info, CareValues Values, int ItemIndex)
    {
      DataRow LastRow = ResTable.Rows[ResTable.Rows.Count - 1];
      DataRow ResRow;
      if (LastRow.IsNull("CareId"))
        ResRow = LastRow; // ������ ������ ��� ��������
      else
      {
        ResRow = ResTable.Rows.Add();
        ResRow["PlantId"] = LastRow["PlantId"];
        ResRow["PlantOrder"] = LastRow["PlantOrder"];
      }

      ResRow["CareId"] = Info.CareDocId;
      ResRow["CareRecordId"] = Info.CareRecordId;
      ResRow["PeriodText"] = DateRangeFormatter.Default.ToString(Info.Range, false);
      ResRow["PeriodName"] = Info.PeriodName;
      ResRow["ItemName"] = Values.Items[ItemIndex].Name;
      ResRow["ItemTextValue"] = Values.Items[ItemIndex].GetTextValue(Values[ItemIndex]);
    }

    #endregion

    #region �������� ������

    EFPReportDBxGridPage MainPage;

    void MainPage_InitGrid(object Sender, EventArgs Args)
    {
      MainPage.ControlProvider.Control.AutoGenerateColumns = false;
      MainPage.ControlProvider.Columns.AddInt("PlantNumber", true, "� �� ��������", 3);
      MainPage.ControlProvider.Columns.LastAdded.GridColumn.DefaultCellStyle.Format = ProgramDBUI.Settings.NumberMask;
      MainPage.ControlProvider.Columns.LastAdded.CanIncSearch = true;
      MainPage.ControlProvider.Columns.AddTextFill("PlantName", true, "������������", 50, 20);
      MainPage.ControlProvider.Columns.LastAdded.CanIncSearch = true;
      MainPage.ControlProvider.Columns.AddText("PeriodText", true, "������ ������", DateRangeFormatter.Default.MonthDayRangeShortTextLength, DateRangeFormatter.Default.MonthDayRangeShortTextLength);
      MainPage.ControlProvider.Columns.LastAdded.TextAlign = HorizontalAlignment.Center;
      //MainPage.ControlProvider.Columns.AddText("PeriodName", true, "�������� �������", 20, 10);
      MainPage.ControlProvider.Columns.AddText("ItemName", true, "��������", 20, 10);
      MainPage.ControlProvider.Columns.AddTextFill("ItemTextValue", true, "��������", 50, 20);
      MainPage.ControlProvider.DisableOrdering();

      MainPage.ControlProvider.GetRowAttributes += new EFPDataGridViewRowAttributesEventHandler(MainPage_GetRowAttributes);

      MainPage.ControlProvider.ReadOnly = false;
      MainPage.ControlProvider.CanInsert = false;
      MainPage.ControlProvider.CanDelete = false;
      MainPage.ControlProvider.CanView = true;
      MainPage.ControlProvider.Control.ReadOnly = true;
      MainPage.ControlProvider.EditData += new EventHandler(MainPage_EditData);
      MainPage.ControlProvider.GetDocSel += new EFPDBxGridViewDocSelEventHandler(MainPage_GetDocSel);
    }

    void MainPage_GetRowAttributes(object Sender, EFPDataGridViewRowAttributesEventArgs Args)
    {
      int PlantOrder = DataTools.GetInt(Args.DataRow, "PlantOrder");
      if ((PlantOrder % 2) == 0)
        Args.ColorType = EFPDataGridViewColorType.Alter;
      if (DataTools.GetInt(Args.DataRow, "CareId") == 0)
        Args.Grayed = true;
    }

    void MainPage_EditData(object Sender, EventArgs Args)
    {
      if (!MainPage.ControlProvider.CheckSingleRow())
        return;
      switch (MainPage.ControlProvider.CurrentColumnName)
      {
        case "PlantNumber":
        case "PlantName":
          Int32 PlantId = DataTools.GetInt(MainPage.ControlProvider.CurrentDataRow, "PlantId");
          ProgramDBUI.TheUI.DocTypes["Plants"].PerformEditing(PlantId, MainPage.ControlProvider.State == EFPDataGridViewState.View);
          break;
        case "PeriodText":
        case "PeriodName":
        case "ItemName":
        case "ItemTextValue":
          Int32 CareId = DataTools.GetInt(MainPage.ControlProvider.CurrentDataRow, "CareId");
          if (CareId == 0)
            EFPApp.ShowTempMessage("��������� ������ �� ��������� �� �������� �� ����� �� ����������");
          else
            ProgramDBUI.TheUI.DocTypes["Care"].PerformEditing(CareId, MainPage.ControlProvider.State == EFPDataGridViewState.View);
          break;
        default:
          EFPApp.ShowTempMessage("��� ���������� �������");
          break;
      }
    }

    void MainPage_GetDocSel(object Sender, EFPDBxGridViewDocSelEventArgs Args)
    {
      Args.AddFromColumn("Plants", "PlantId");
      Args.AddFromColumn("Care", "CareId");
    }


    #endregion
  }
}