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
    #region Конструктор формы

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

    #region Поля

    public EFPMonthDayTextBox efpDay;

    public EFPDateRangeBox efpPeriod;

    #endregion
  }

  internal class CareReportParams : EFPReportExtParams
  {
    #region Конструктор

    public CareReportParams()
    {
      Day = new MonthDay(DateTime.Today);
      FirstDate = LastDate = DateTime.Today;
      Filters = new PlantReportFilters(String.Empty, String.Empty);
    }

    #endregion

    #region Поля

    public MonthDay Day;

    public DateTime FirstDate;

    public DateTime LastDate;

    public DateRange Period { get { return new DateRange(FirstDate, LastDate); } }

    public PlantReportFilters Filters;

    #endregion

    #region Переопределенные методы

    protected override void OnInitTitle()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("Уход за растениями");
      if (!Day.IsEmpty)
      {
        sb.Append(" на ");
        sb.Append(Day.ToString());
      }
      base.Title = sb.ToString();
      Filters.AddFilterInfo(FilterInfo);
      if (Filters.PeriodRequired)
        FilterInfo.Add("Период для фильтров", DateRangeFormatter.Default.ToString(Period, true));
    }

    public override EFPReportExtParamsForm CreateForm()
    {
      return new CareReportParamForm();
    }

    public override EFPReportExtParamsPart UsedParts
    {
      get { return EFPReportExtParamsPart.User | EFPReportExtParamsPart.NoHistory; }
    }

    public override void WriteFormValues(EFPReportExtParamsForm form, EFPReportExtParamsPart part)
    {
      CareReportParamForm form2 = (CareReportParamForm)form;
      switch (part)
      {
        case EFPReportExtParamsPart.User:
          form2.FiltersControlProvider.Filters = Filters;
          break;
        case EFPReportExtParamsPart.NoHistory:
          form2.efpDay.Value = Day;
          form2.efpPeriod.First.Value = FirstDate;
          form2.efpPeriod.Last.Value = LastDate;
          break;
      }
    }

    public override void ReadFormValues(EFPReportExtParamsForm form, EFPReportExtParamsPart part)
    {
      CareReportParamForm form2 = (CareReportParamForm)form;
      switch (part)
      {
        case EFPReportExtParamsPart.NoHistory:
          Day = form2.efpDay.Value;
          FirstDate = form2.efpPeriod.First.Value;
          LastDate = form2.efpPeriod.Last.Value;
          break;
      }
    }

    public override void WriteConfig(FreeLibSet.Config.CfgPart cfg, EFPReportExtParamsPart part)
    {
      switch (part)
      {
        case EFPReportExtParamsPart.User:
          Filters.WriteConfig(cfg);
          break;
        case EFPReportExtParamsPart.NoHistory:
          cfg.SetInt("Day", Day.DayOfYear);
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
          Day = new MonthDay(cfg.GetIntDef("Day", Day.DayOfYear));
          cfg.GetDate("FirstDate", ref FirstDate);
          cfg.GetDate("LastDate", ref LastDate);
          break;
      }
    }

    #endregion
  }

  internal class CareReport : EFPReport
  {
    #region Конструктор

    public CareReport()
      : base("CareReport")
    {
      MainImageKey = "CareReport";

      _MainPage = new EFPReportDBxGridPage(ProgramDBUI.TheUI);
      _MainPage.InitGrid += new EventHandler(MainPage_InitGrid);
      _MainPage.ShowToolBar = true;
      Pages.Add(_MainPage);
    }

    #endregion

    #region Запрос параметров

    protected override EFPReportParams CreateParams()
    {
      return new CareReportParams();
    }

    public CareReportParams Params { get { return (CareReportParams)(base.ReportParams); } }

    #endregion

    #region Построение отчета

    private class RecordSortInfo : IComparable<RecordSortInfo>
    {
      #region Поля

      /// <summary>
      /// Идентификатор документа "Care".
      /// В сортировке не участвует
      /// </summary>
      public Int32 CareDocId;

      /// <summary>
      /// Идентификатор поддокумента "CareRecord".
      /// В сортировке не участвует
      /// </summary>
      public Int32 CareRecordId;

      /// <summary>
      /// Период ухода.
      /// Первичное поле для сортировки
      /// </summary>
      public MonthDayRange Range;

      /// <summary>
      /// Название периода.
      /// В сортировке не участвует
      /// </summary>
      public string PeriodName;

      /// <summary>
      /// Индекс документа "Care" в цепочке наследования.
      /// Вторичное поле для сортировки
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
      DBxFilter sqlFilter = Params.Filters.GetSqlFilter();
      if (ProgramDBUI.TheUI.DocProvider.DocTypes.UseDeleted)
        sqlFilter &= DBSDocType.DeletedFalseFilter;

      DBxColumnList colNames = new DBxColumnList();
      colNames.Add("Id,Care");
      ProgramDBUI.TheUI.DocTypes["Plants"].GridProducer.GetColumnNames("Plants", "", colNames);

      DataTable plantTable = ProgramDBUI.TheUI.DocProvider.FillSelect("Plants",
        new DBxColumns(colNames),
        sqlFilter,
        DBxOrder.FromDataViewSort("Number,Name,Id"));

      Params.Filters.PerformAuxFiltering(ref plantTable, Params.FirstDate, Params.LastDate);
      // Порядок строк правильный

      DBxDocSet careSet = new DBxDocSet(ProgramDBUI.TheUI.DocProvider);
      DBxMultiDocs mCareDocs = careSet["Care"];
      Int32[] careIds = DataTools.GetIdsFromColumn(plantTable, "Care");
      if (careIds.Length > 0)
        mCareDocs.View(careIds);

      DataTable resTable = new DataTable();
      resTable.Columns.Add("PlantId", typeof(Int32));
      resTable.Columns.Add("PlantNumber", typeof(int));
      resTable.Columns.Add("PlantName", typeof(string));
      resTable.Columns.Add("CareId", typeof(Int32)); // к которому относится запись
      resTable.Columns.Add("CareRecordId", typeof(Int32)); // Id записи
      resTable.Columns.Add("PeriodText", typeof(string));
      resTable.Columns.Add("PeriodName", typeof(string));
      resTable.Columns.Add("ItemName", typeof(string));
      resTable.Columns.Add("ItemTextValue", typeof(string));
      resTable.Columns.Add("PlantOrder", typeof(int)); // Порядок растения в отчете

      int plantCount = 0;
      foreach (DataRow plantRow in plantTable.Rows)
      {
        // Одна строка добавляется, даже если для растения нет ухода
        DataRow resRow1 = resTable.Rows.Add();
        resRow1["PlantId"] = plantRow["Id"];
        resRow1["PlantName"] = plantRow["Name"];
        resRow1["PlantNumber"] = plantRow["Number"];
        plantCount++;
        resRow1["PlantOrder"] = plantCount;

        DBxSingleDoc[] careDocs = GetCareDocs(mCareDocs, DataTools.GetInt(plantRow, "Care"));
        if (careDocs.Length == 0)
          continue; // не задана ссылка "Care"

        SortedList<RecordSortInfo, CareValues> recList = new SortedList<RecordSortInfo, CareValues>();

        for (int i = 0; i < careDocs.Length; i++)
        {
          foreach (DBxSubDoc subDoc in careDocs[i].SubDocs["CareRecords"])
          {
            MonthDay md1 = new MonthDay(subDoc.Values["Day1"].AsInteger);
            MonthDay md2 = new MonthDay(subDoc.Values["Day2"].AsInteger);
            MonthDayRange thisRange = new MonthDayRange(md1, md2);
            if ((!Params.Day.IsEmpty) && (!thisRange.IsEmpty))
            {
              if (!thisRange.Contains(Params.Day))
                continue;
            }

            CareValues values = new CareValues(CareItem.TheList);
            values.Read(subDoc.Values);

            RecordSortInfo si = new RecordSortInfo();
            si.CareDocId = careDocs[i].DocId;
            si.CareDocHieIndex = i;
            si.CareRecordId = subDoc.SubDocId;
            si.Range = thisRange;
            si.PeriodName = subDoc.Values["Name"].AsString;
            recList.Add(si, values);
          }
        }

        // Теперь можно перебирать отсортированный список

        // Строки отчета должны быть осортированы по порядку характеристик
        for (int j = 0; j < CareItem.TheList.Count; j++)
        {
          if (Params.Day.IsEmpty)
          {
            // Добавляем по одной строке, если есть заданное значение
            foreach (KeyValuePair<RecordSortInfo, CareValues> pair in recList)
            {
              if (pair.Value[j] != null)
                AddReportRow(resTable, pair.Key, pair.Value, j);
            }
          }
          else
          {
            // Находим последнюю действующую запись
            KeyValuePair<RecordSortInfo, CareValues> usedPair = new KeyValuePair<RecordSortInfo, CareValues>(); // неинициализированная структура
            foreach (KeyValuePair<RecordSortInfo, CareValues> pair in recList)
            {
              if (pair.Value[j] != null)
                usedPair = pair;
            }

            if (usedPair.Key != null)
              AddReportRow(resTable, usedPair.Key, usedPair.Value, j);
          }
        }
      }

      _MainPage.DataSource = resTable.DefaultView;
    }

    private static readonly DBxSingleDoc[] _EmptyDocs = new DBxSingleDoc[0];

    /// <summary>
    /// Возвращает массив документов об уходе, которые должны быть просмотрены, с учетом иерархии.
    /// Первым в массиве идет родительский документ, затем дочерний, ... Последним идет документ с идентификатором <paramref name="careId"/>.
    /// Если <paramref name="careId"/>=0, возвращается пустой массив
    /// </summary>
    /// <param name="mCareDocs">Документы "Care"</param>
    /// <param name="careId">Идентификатор записи об уходе, заданной у растения</param>
    /// <returns></returns>
    private static DBxSingleDoc[] GetCareDocs(DBxMultiDocs mCareDocs, Int32 careId)
    {
      if (careId == 0)
        return _EmptyDocs;

      List<DBxSingleDoc> lst = new List<DBxSingleDoc>();
      IdList ids = new IdList();
      while (careId != 0)
      {
        DBxSingleDoc doc;
        if (!mCareDocs.TryGetDocById(careId, out doc))
          doc = mCareDocs.View(careId); // догружаем недостающего родителя

        if (ids.Contains(careId))
        {
          EFPApp.ErrorMessageBox("Цепочка документов об уходе зациклена для DocId=" + careId + ": " + doc.TextValue);
          break;
        }
        ids.Add(careId);
        lst.Insert(0, doc);
        careId = doc.Values["ParentId"].AsInteger;
      }

      return lst.ToArray();
    }

    /// <summary>
    /// Заполнение строки отчета.
    /// Добавляет новую строку или заполняет первую строку для растения
    /// </summary>
    private void AddReportRow(DataTable resTable, RecordSortInfo sortInfo, CareValues values, int itemIndex)
    {
      DataRow lastRow = resTable.Rows[resTable.Rows.Count - 1];
      DataRow resRow;
      if (lastRow.IsNull("CareId"))
        resRow = lastRow; // первая строка для растения
      else
      {
        resRow = resTable.Rows.Add();
        resRow["PlantId"] = lastRow["PlantId"];
        resRow["PlantOrder"] = lastRow["PlantOrder"];
      }

      resRow["CareId"] = sortInfo.CareDocId;
      resRow["CareRecordId"] = sortInfo.CareRecordId;
      resRow["PeriodText"] = DateRangeFormatter.Default.ToString(sortInfo.Range, false);
      resRow["PeriodName"] = sortInfo.PeriodName;
      resRow["ItemName"] = values.Items[itemIndex].Name;
      resRow["ItemTextValue"] = values.Items[itemIndex].GetTextValue(values[itemIndex]);
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
      _MainPage.ControlProvider.Columns.AddTextFill("PlantName", true, "Наименование", 50, 20);
      _MainPage.ControlProvider.Columns.LastAdded.CanIncSearch = true;
      _MainPage.ControlProvider.Columns.AddText("PeriodText", true, "Период записи", DateRangeFormatter.Default.MonthDayRangeShortTextLength, DateRangeFormatter.Default.MonthDayRangeShortTextLength);
      _MainPage.ControlProvider.Columns.LastAdded.TextAlign = HorizontalAlignment.Center;
      //MainPage.ControlProvider.Columns.AddText("PeriodName", true, "Название периода", 20, 10);
      _MainPage.ControlProvider.Columns.AddText("ItemName", true, "Параметр", 20, 10);
      _MainPage.ControlProvider.Columns.AddTextFill("ItemTextValue", true, "Значение", 50, 20);
      _MainPage.ControlProvider.DisableOrdering();

      _MainPage.ControlProvider.GetRowAttributes += new EFPDataGridViewRowAttributesEventHandler(MainPage_GetRowAttributes);

      _MainPage.ControlProvider.ReadOnly = false;
      _MainPage.ControlProvider.CanInsert = false;
      _MainPage.ControlProvider.CanDelete = false;
      _MainPage.ControlProvider.CanView = true;
      _MainPage.ControlProvider.Control.ReadOnly = true;
      _MainPage.ControlProvider.EditData += new EventHandler(MainPage_EditData);
      _MainPage.ControlProvider.GetDocSel += new EFPDBxGridViewDocSelEventHandler(MainPage_GetDocSel);
    }

    void MainPage_GetRowAttributes(object sender, EFPDataGridViewRowAttributesEventArgs args)
    {
      int plantOrder = DataTools.GetInt(args.DataRow, "PlantOrder");
      if ((plantOrder % 2) == 0)
        args.ColorType = EFPDataGridViewColorType.Alter;
      if (DataTools.GetInt(args.DataRow, "CareId") == 0)
        args.Grayed = true;
    }

    void MainPage_EditData(object sender, EventArgs args)
    {
      if (!_MainPage.ControlProvider.CheckSingleRow())
        return;
      switch (_MainPage.ControlProvider.CurrentColumnName)
      {
        case "PlantNumber":
        case "PlantName":
          Int32 plantId = DataTools.GetInt(_MainPage.ControlProvider.CurrentDataRow, "PlantId");
          ProgramDBUI.TheUI.DocTypes["Plants"].PerformEditing(plantId, _MainPage.ControlProvider.State == EFPDataGridViewState.View);
          break;
        case "PeriodText":
        case "PeriodName":
        case "ItemName":
        case "ItemTextValue":
          Int32 careId = DataTools.GetInt(_MainPage.ControlProvider.CurrentDataRow, "CareId");
          if (careId == 0)
            EFPApp.ShowTempMessage("Выбранная строка не ссылается на документ по уходу за растениями");
          else
            ProgramDBUI.TheUI.DocTypes["Care"].PerformEditing(careId, _MainPage.ControlProvider.State == EFPDataGridViewState.View);
          break;
        default:
          EFPApp.ShowTempMessage("Нет выбранного столбца");
          break;
      }
    }

    void MainPage_GetDocSel(object sender, EFPDBxGridViewDocSelEventArgs args)
    {
      args.AddFromColumn("Plants", "PlantId");
      args.AddFromColumn("Care", "CareId");
    }

    #endregion
  }
}