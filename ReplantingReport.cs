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
    #region �����������

    public ReplantingReportParamsForm()
    {
      Text = "���������";

      EFPControlWithToolBar<DataGridView> cwt = new EFPControlWithToolBar<DataGridView>(FormProvider, base.MainPanel);
      _FiltersControlProvider = new EFPGridFilterEditorGridView(cwt);

    }

    #endregion

    #region ��������

    /// <summary>
    /// ��������� ���������� ��������� ��� ������� �������������� ��������.
    /// </summary>
    public EFPGridFilterEditorGridView FiltersControlProvider { get { return _FiltersControlProvider; } }
    private EFPGridFilterEditorGridView _FiltersControlProvider;

    #endregion
  }

  /// <summary>
  /// ������� ��� ������.
  /// </summary>
  internal class ReplantingReportFilters : DBxClientFilters
  {
    #region �����������

    /// <summary>
    /// ������� �������
    /// </summary>
    public ReplantingReportFilters()
    {
      FiltGroup = new RefGroupDocGridFilter(ProgramDBUI.TheUI, "PlantGroups", "GroupId");
      Add(FiltGroup);

      FiltPlace = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["Places"], "Place");
      FiltPlace.DisplayName = "����� ������������";
      FiltPlace.Nullable = false;
      FiltPlace.UseSqlFilter = true; // ����� ���������� ����������� ����
      Add(FiltPlace);

      SetReadOnly();
    }

    #endregion

    #region ���� ��������

    RefGroupDocGridFilter FiltGroup;

    RefDocGridFilter FiltPlace;

    #endregion
  }

  internal class ReplantingReportParams : EFPReportExtParams
  {
    #region �����������

    public ReplantingReportParams()
    {
      Filters = new ReplantingReportFilters();
    }

    #endregion

    #region ����

    public ReplantingReportFilters Filters;

    #endregion

    #region ���������������� ������

    protected override void OnInitTitle()
    {
      base.Title = "��������� ��������";
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
    #region �����������

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

    #region ������ ����������

    protected override EFPReportParams CreateParams()
    {
      return new ReplantingReportParams();
    }

    public ReplantingReportParams Params { get { return (ReplantingReportParams)(base.ReportParams); } }

    #endregion

    #region ���������� ������

    const ActionKind ActionNone = (ActionKind) (-2);
    const ActionKind ActionAdd = (ActionKind)(-1); // �������� "������"

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

      #region ������� ��������

      DBxFilter SqlFilter = Params.Filters.GetSqlFilter();
      SqlFilter&=new ValueFilter("MovementState", (int)(PlantMovementState.Placed)); // ������ ������������
      if (ProgramDBUI.TheUI.DocProvider.DocTypes.UseDeleted)
        SqlFilter &= DBSDocType.DeletedFalseFilter;

      DataTable PlantTable = ProgramDBUI.TheUI.DocProvider.FillSelect("Plants",
        new DBxColumns("Id,Number,Name"),
        SqlFilter);

      foreach (DataRow row in PlantTable.Rows)
        ResTable.Rows.Add(row["Id"], row["Number"], row["Name"], (int)ActionNone);

      #endregion

      #region ������� ��������

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
          continue; // �������� �� ������ ����������

        if (!ResRow.IsNull("ActionDate"))
          continue; // ��� ���� ����� ������� ��������

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

      #region ������� ��������

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
          continue; // �������� �� ������ ����������

        if (!ResRow.IsNull("ActionDate"))
          continue; // ��� ���� ����� �����-���� ��������

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

    #region �������� ������

    EFPReportDBxGridPage MainPage;

    void MainPage_InitGrid(object Sender, EventArgs Args)
    {
      MainPage.ControlProvider.Control.AutoGenerateColumns = false;
      MainPage.ControlProvider.Columns.AddInt("PlantNumber", true, "� �� ��������", 3);
      MainPage.ControlProvider.Columns.LastAdded.GridColumn.DefaultCellStyle.Format = ProgramDBUI.Settings.NumberMask;
      MainPage.ControlProvider.Columns.LastAdded.CanIncSearch = true;
      MainPage.ControlProvider.Columns.AddText("PlantName", true, "������������", 40, 20);
      MainPage.ControlProvider.Columns.LastAdded.CanIncSearch = true;
      MainPage.ControlProvider.Columns.AddImage("ActionImage");
      MainPage.ControlProvider.Columns.AddDate("ActionDate", true, "���� ��������");
      MainPage.ControlProvider.Columns.AddText("ActionName", false, "��������", 20, 10);
      MainPage.ControlProvider.Columns.AddText("SoilText", true, "�����", 20, 10);
      MainPage.ControlProvider.Columns.AddText("PotKindText", true, "������", 20, 10);
      MainPage.ControlProvider.Columns.AddText("ActionComment", true, "����������� � ��������", 40, 10);
      MainPage.ControlProvider.DisableOrdering();

      MainPage.ControlProvider.Orders.Add("PlantNumber,PlantName", "�� ������");
      MainPage.ControlProvider.Orders.Add("PlantName,PlantNumber", "�� ��������");
      MainPage.ControlProvider.Orders.Add("ActionDate,PlantNumber,PlantName", "�� ���� ��������� (�� �����������",new EFPDataGridViewSortInfo("ActionDate", ListSortDirection.Ascending));
      MainPage.ControlProvider.Orders.Add("ActionDate DESC,PlantNumber,PlantName", "�� ���� ��������� (�� ��������", new EFPDataGridViewSortInfo("ActionDate", ListSortDirection.Descending));
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
            args.Value = "�����������";
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
