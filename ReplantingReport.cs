using System;
using System.Collections.Generic;
using System.Text;
using AgeyevAV.ExtForms;
using AgeyevAV.ExtForms.Docs;
using System.Windows.Forms;
using AgeyevAV;
using System.Data;
using AgeyevAV.ExtDB;
using AgeyevAV.ExtDB.Docs;
using System.ComponentModel;

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

    RefGroupDocGridFilter FiltGroup;

    RefDocGridFilter FiltPlace;

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

    public override void WriteFormValues(EFPReportExtParamsForm Form, EFPReportExtParamsPart Part)
    {
      ReplantingReportParamsForm Form2 = (ReplantingReportParamsForm)Form;
      Form2.FiltersControlProvider.Filters = Filters;
    }

    public override void ReadFormValues(EFPReportExtParamsForm Form, EFPReportExtParamsPart Part)
    {
      ReplantingReportParamsForm Form2 = (ReplantingReportParamsForm)Form;
    }

    public override void WriteConfig(AgeyevAV.Config.CfgPart Config, EFPReportExtParamsPart Part)
    {
      switch (Part)
      {
        case EFPReportExtParamsPart.User:
          Filters.WriteConfig(Config);
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

      MainPage = new EFPReportDBxGridPage(ProgramDBUI.TheUI);
      MainPage.InitGrid += new EventHandler(MainPage_InitGrid);
      MainPage.ShowToolBar = true;
      Pages.Add(MainPage);
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

    const ActionKind ActionNone = (ActionKind) (-2);
    const ActionKind ActionAdd = (ActionKind)(-1); // операция "Приход"

    protected override void BuildReport()
    {
      DataTable ResTable = new DataTable();
      ResTable.Columns.Add("PlantId", typeof(Int32));
      ResTable.Columns.Add("PlantNumber", typeof(int));
      ResTable.Columns.Add("PlantName", typeof(string));

      ResTable.Columns.Add("ActionKind", typeof(int));
      ResTable.Columns.Add("ActionDate", typeof(DateTime));
      ResTable.Columns.Add("SoilText", typeof(string));
      ResTable.Columns.Add("PotKindText", typeof(string));
      ResTable.Columns.Add("ActionComment", typeof(string));

      DataTools.SetPrimaryKey(ResTable, "PlantId");

      #region Таблица растений

      DBxFilter SqlFilter = Params.Filters.GetSqlFilter();
      SqlFilter&=new ValueFilter("MovementState", (int)(PlantMovementState.Placed)); // только существующие
      if (ProgramDBUI.TheUI.DocProvider.DocTypes.UseDeleted)
        SqlFilter &= DBSDocType.DeletedFalseFilter;

      DataTable PlantTable = ProgramDBUI.TheUI.DocProvider.FillSelect("Plants",
        new DBxColumns("Id,Number,Name"),
        SqlFilter);

      foreach (DataRow row in PlantTable.Rows)
        ResTable.Rows.Add(row["Id"], row["Number"], row["Name"], (int)ActionNone);

      #endregion

      #region Таблица действий

      SqlFilter = new ValuesFilter("Kind", new int[] { 
        (int)ActionKind.Planting, 
        (int)ActionKind.Replanting,
        (int)ActionKind.Transshipment,
        (int)ActionKind.SoilReplace});
      if (ProgramDBUI.TheUI.DocProvider.DocTypes.UseDeleted)
      {
        SqlFilter &= DBSSubDocType.DeletedFalseFilter;
        SqlFilter &= DBSSubDocType.DocIdDeletedFalseFilter;
      }

      DataTable ActionTable = ProgramDBUI.TheUI.DocProvider.FillSelect("PlantActions",
        new DBxColumns("DocId,Kind,Date2,Soil,PotKind,Comment"),
        SqlFilter,
        new DBxOrder("Date2", ListSortDirection.Descending));

      foreach (DataRow row in ActionTable.Rows)
      {
        Int32 PlantId = DataTools.GetInt(row, "DocId");
        DataRow ResRow = ResTable.Rows.Find(PlantId);
        if (ResRow == null)
          continue; // растение не прошло фильтрацию

        if (!ResRow.IsNull("ActionDate"))
          continue; // уже была более поздняя операция

        ResRow["ActionKind"] = row["Kind"];
        ResRow["ActionDate"] = row["Date2"];
        Int32 SoilId = DataTools.GetInt(row, "Soil");
        if (SoilId != 0)
          ResRow["SoilText"] = ProgramDBUI.TheUI.DocTypes["Soils"].GetTextValue(SoilId);
        Int32 PotKindId = DataTools.GetInt(row, "PotKind");
        if (PotKindId != 0)
          ResRow["PotKindText"] = ProgramDBUI.TheUI.DocTypes["PotKinds"].GetTextValue(PotKindId);
        ResRow["ActionComment"] = row["Comment"];
      }

      #endregion

      #region Таблица движения

      SqlFilter = new ValueFilter("Kind", (int)MovementKind.Add);
      if (ProgramDBUI.TheUI.DocProvider.DocTypes.UseDeleted)
      {
        SqlFilter &= DBSSubDocType.DeletedFalseFilter;
        SqlFilter &= DBSSubDocType.DocIdDeletedFalseFilter;
      }

      DataTable MovementTable = ProgramDBUI.TheUI.DocProvider.FillSelect("PlantMovement",
        new DBxColumns("DocId,Date2,Soil,PotKind,Comment"),
        SqlFilter,
        new DBxOrder("Date2", ListSortDirection.Descending));

      foreach (DataRow row in MovementTable.Rows)
      {
        Int32 PlantId = DataTools.GetInt(row, "DocId");
        DataRow ResRow = ResTable.Rows.Find(PlantId);
        if (ResRow == null)
          continue; // растение не прошло фильтрацию

        if (!ResRow.IsNull("ActionDate"))
          continue; // уже была более какая-либо операция

        ResRow["ActionKind"] = ActionAdd;
        ResRow["ActionDate"] = row["Date2"];
        Int32 SoilId = DataTools.GetInt(row, "Soil");
        if (SoilId != 0)
          ResRow["SoilText"] = ProgramDBUI.TheUI.DocTypes["Soils"].GetTextValue(SoilId);
        Int32 PotKindId = DataTools.GetInt(row, "PotKind");
        if (PotKindId != 0)
          ResRow["PotKindText"] = ProgramDBUI.TheUI.DocTypes["PotKinds"].GetTextValue(PotKindId);
        ResRow["ActionComment"] = row["Comment"];
      }

      #endregion

      MainPage.DataSource = ResTable.DefaultView;
    }

    #endregion

    #region Страница отчета

    EFPReportDBxGridPage MainPage;

    void MainPage_InitGrid(object Sender, EventArgs Args)
    {
      MainPage.ControlProvider.Control.AutoGenerateColumns = false;
      MainPage.ControlProvider.Columns.AddInt("PlantNumber", true, "№ по каталогу", 3);
      MainPage.ControlProvider.Columns.LastAdded.GridColumn.DefaultCellStyle.Format = ProgramDBUI.Settings.NumberMask;
      MainPage.ControlProvider.Columns.LastAdded.CanIncSearch = true;
      MainPage.ControlProvider.Columns.AddText("PlantName", true, "Наименование", 40, 20);
      MainPage.ControlProvider.Columns.LastAdded.CanIncSearch = true;
      MainPage.ControlProvider.Columns.AddImage("ActionImage");
      MainPage.ControlProvider.Columns.AddDate("ActionDate", true, "Дата операции");
      MainPage.ControlProvider.Columns.AddText("ActionName", false, "Действие", 20, 10);
      MainPage.ControlProvider.Columns.AddText("SoilText", true, "Грунт", 20, 10);
      MainPage.ControlProvider.Columns.AddText("PotKindText", true, "Горшок", 20, 10);
      MainPage.ControlProvider.Columns.AddText("ActionComment", true, "Комментарий к операции", 40, 10);
      MainPage.ControlProvider.DisableOrdering();

      MainPage.ControlProvider.Orders.Add("PlantNumber,PlantName", "По номеру");
      MainPage.ControlProvider.Orders.Add("PlantName,PlantNumber", "По названию");
      MainPage.ControlProvider.Orders.Add("ActionDate,PlantNumber,PlantName", "По дате пересадки (по возрастанию",new EFPDataGridViewSortInfo("ActionDate", ListSortDirection.Ascending));
      MainPage.ControlProvider.Orders.Add("ActionDate DESC,PlantNumber,PlantName", "По дате пересадки (по убыванию", new EFPDataGridViewSortInfo("ActionDate", ListSortDirection.Descending));
      MainPage.ControlProvider.AutoSort = true;
      MainPage.ControlProvider.CurrentOrderIndex = 2;

      MainPage.ControlProvider.GetCellAttributes += new EFPDataGridViewCellAttributesEventHandler(MainPage_GetCellAttributes);

      MainPage.ControlProvider.ReadOnly = false;
      MainPage.ControlProvider.CanInsert = false;
      MainPage.ControlProvider.CanDelete = false;
      MainPage.ControlProvider.CanView = true;
      MainPage.ControlProvider.Control.ReadOnly = true;
      MainPage.ControlProvider.EditData += new EventHandler(MainPage_EditData);
      MainPage.ControlProvider.GetDocSel += new EFPDBxGridViewDocSelEventHandler(MainPage_GetDocSel);
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

    void MainPage_EditData(object Sender, EventArgs Args)
    {
      if (!MainPage.ControlProvider.CheckSingleRow())
        return;
      Int32 PlantId = DataTools.GetInt(MainPage.ControlProvider.CurrentDataRow, "PlantId");
      ProgramDBUI.TheUI.DocTypes["Plants"].PerformEditing(PlantId, MainPage.ControlProvider.State == EFPDataGridViewState.View);
    }

    void MainPage_GetDocSel(object Sender, EFPDBxGridViewDocSelEventArgs Args)
    {
      Args.AddFromColumn("Plants", "PlantId");
    }

    #endregion
  }
}
