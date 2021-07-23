using System;
using System.Collections.Generic;
using System.Text;
using AgeyevAV.ExtForms.Docs;
using AgeyevAV.ExtDB.Docs;
using AgeyevAV.ExtDB;
using System.Windows.Forms;
using AgeyevAV.ExtForms;
using AgeyevAV;
using System.ComponentModel;

namespace Plants
{
  internal class ProgramDBUI : DBUI
  {
    #region �����������

    public ProgramDBUI(DBxDocProviderProxy sourceDocProviderProxy)
      : base(sourceDocProviderProxy)
    {
      DocTypeUI dt;
      SubDocTypeUI sdt;

      #region ������� ��������

      #region �������� ��������

      dt = base.DocTypes["Plants"];

      dt.GridProducer.Columns.AddText("Number", "� �� ��������", 3, 3);
      dt.GridProducer.Columns.LastAdded.Format = ProgramDBUI.Settings.NumberMask;
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;
      dt.GridProducer.Columns.LastAdded.TextAlign = HorizontalAlignment.Center;

      dt.GridProducer.Columns.AddText("Name", "�������� ��� ��������", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;
      dt.GridProducer.Columns.AddText("LocalName", "������� ��������", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;
      dt.GridProducer.Columns.AddText("LatinName", "��������� ��������", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;
      dt.GridProducer.Columns.AddText("Description", "��������", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      dt.GridProducer.Columns.Add(new PlantThumbnailColumn(false));

      dt.GridProducer.Columns.AddText("Place.Name", "�����", 15, 10);
      dt.GridProducer.Columns.AddDateRange("AddDate", "AddDate1", "AddDate2", "���� �����������", false, 10, 10);
      dt.GridProducer.Columns.LastAdded.TextAlign = HorizontalAlignment.Center;
      dt.GridProducer.Columns.LastAdded.EmptyValue = String.Empty; // � �� "��� ����"
      dt.GridProducer.Columns.AddText("FromContra.Name", "�� ���� ��������", 15, 10);
      dt.GridProducer.Columns.AddRefDocText("FromPlant", DocTypes["Plants"], "�������� �� ��������", 15, 10);

      dt.GridProducer.Columns.AddUserText("StateText", "MovementState,Place,ToContra", new EFPGridProducerValueNeededEventHandler(EditPlant.StateText_Column_ToolTipText_ValueNeeded), "���������", 30, 5);

      dt.GridProducer.Columns.AddRefDocTextAndImage("LastPlantAction", this.DocTypes["Plants"].SubDocTypes["PlantActions"], "��������� ��������", 30, 10);
      dt.GridProducer.Columns.AddDateRange("LastPlantActionDate", "LastPlantAction.Date1", "LastPlantAction.Date2", "���� ���������� ��������", false, 10, 10);
      dt.GridProducer.Columns.LastAdded.TextAlign = HorizontalAlignment.Center;
      dt.GridProducer.Columns.LastAdded.EmptyValue = String.Empty; // � �� "��� ����"

      dt.GridProducer.Columns.AddRefDocTextAndImage("LastPlantReplanting", this.DocTypes["Plants"].SubDocTypes["PlantActions"], "���������", 30, 10);
      dt.GridProducer.Columns.AddDateRange("LastPlantReplantingDate", "LastPlantReplanting.Date1", "LastPlantReplanting.Date2", "���� ���������", false, 10, 10);
      dt.GridProducer.Columns.LastAdded.TextAlign = HorizontalAlignment.Center;
      dt.GridProducer.Columns.LastAdded.EmptyValue = String.Empty; // � �� "��� ����"

      dt.GridProducer.Columns.AddRefDocText("LastPlantDisease", this.DocTypes["Plants"].SubDocTypes["PlantDiseases"], "�����������", 30, 10);

      dt.GridProducer.Columns.AddDateRange("RemoveDate", "RemoveDate1", "RemoveDate2", "���� �������", false, 10, 10);
      dt.GridProducer.Columns.LastAdded.TextAlign = HorizontalAlignment.Center;
      dt.GridProducer.Columns.LastAdded.EmptyValue = String.Empty; // � �� "��� ����"
      dt.GridProducer.Columns.AddText("ToContra.Name", "���� ��������", 15, 10);
      dt.GridProducer.Columns.AddRefDocText("ToPlant", DocTypes["Plants"], "��������� � ��������", 15, 10);

      dt.GridProducer.Columns.AddUserText("ContraName", "FromContra,ToContra,FromPlant,ToPlant",
        new EFPGridProducerValueNeededEventHandler(EditPlant.ContraNameColumnValueNeeded),
        "�� ���� / ����", 15, 10);

      dt.GridProducer.Columns.AddText("Manufacturer.Name", "������������ (��������)", 15, 10);
      dt.GridProducer.Columns.AddText("Care.Name", "����", 15, 10);


      dt.GridProducer.Columns.AddUserImage("FirstPlannedActionImage", "FirstPlannedAction.Date1,FirstPlannedAction.Date2,FirstPlannedAction.Kind",
        new EFPGridProducerValueNeededEventHandler(EditPlant.FirstPlannedActionImageColumnValueNeeded), String.Empty).DisplayName = "��������������� �������� (������)";
      dt.GridProducer.Columns.AddUserText("FirstPlannedActionText", "FirstPlannedAction.Kind,FirstPlannedAction.ActionName",
        new EFPGridProducerValueNeededEventHandler(EditPlant.FirstPlannedActionTextColumnValueNeeded), "��������������� ��������", 15, 10);
      dt.GridProducer.Columns.AddDateRange("FirstPlannedActionDate", "FirstPlannedAction.Date1", "FirstPlannedAction.Date2", "���� ���������������� ��������", false, 15, 10);

      dt.GridProducer.Columns.AddText("GroupId.Name", "������", 15, 5);
      dt.GridProducer.Columns.AddText("Comment", "�����������", 30, 10);

      dt.GridProducer.ToolTips.AddText("LocalName", "������� ��������");
      dt.GridProducer.ToolTips.AddText("LatinName", "��������� ��������");
      dt.GridProducer.ToolTips.AddText("Description", "��������");
      dt.GridProducer.ToolTips.AddUserItem("StateText", "MovementState,Place,ToContra", new EFPGridProducerValueNeededEventHandler(EditPlant.StateText_Column_ToolTipText_ValueNeeded), "���������");
      dt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "����������� (���� �����)";

      dt.GridProducer.Orders.Add("Number", "����� �� ��������");
      dt.GridProducer.Orders.Add("Name", "�������� ��� ��������");
      dt.GridProducer.Orders.Add("LocalName", "������� ��������");
      dt.GridProducer.Orders.Add("LatinName", "��������� ��������");
      dt.GridProducer.Orders.Add("Description", "��������");
      dt.GridProducer.Orders.Add("AddDate1", "���� ������� (�� �����������)", new EFPDataGridViewSortInfo("AddDate", ListSortDirection.Ascending));
      dt.GridProducer.Orders.Add("AddDate1 DESC", "���� ������� (�� ��������)", new EFPDataGridViewSortInfo("AddDate", ListSortDirection.Descending));
      dt.GridProducer.Orders.Add("RemoveDate1", "���� ������� (�� �����������)", new EFPDataGridViewSortInfo("RemoveDate", ListSortDirection.Ascending));
      dt.GridProducer.Orders.Add("RemoveDate1 DESC", "���� ������� (�� ��������)", new EFPDataGridViewSortInfo("RemoveDate", ListSortDirection.Descending));
      dt.GridProducer.Orders.Add("LastPlantAction.Date1", "���� ���������� �������� (�� �����������)", new EFPDataGridViewSortInfo("LastPlantActionDate", ListSortDirection.Ascending));
      dt.GridProducer.Orders.Add("LastPlantAction.Date1 DESC", "���� ���������� �������� (�� ��������)", new EFPDataGridViewSortInfo("LastPlantActionDate", ListSortDirection.Descending));
      dt.GridProducer.Orders.Add("LastPlantReplanting.Date1", "���� ��������� (�� �����������)", new EFPDataGridViewSortInfo("LastPlantReplantingDate", ListSortDirection.Ascending));
      dt.GridProducer.Orders.Add("LastPlantReplanting.Date1 DESC", "���� ��������� (�� ��������)", new EFPDataGridViewSortInfo("LastPlantReplantingDate", ListSortDirection.Descending));
      dt.GridProducer.Orders.Add("LastDiseaseDate1", "���� ����������� (�� �����������)", new EFPDataGridViewSortInfo("LastDiseaseDate", ListSortDirection.Ascending));
      dt.GridProducer.Orders.Add("LastDiseaseDate1 DESC", "���� ����������� (�� ��������)", new EFPDataGridViewSortInfo("LastDiseaseDate", ListSortDirection.Descending));
      dt.GridProducer.Columns.AddText("Soil.Name", "�����", 20, 10);
      dt.GridProducer.Columns.AddText("PotKind.Name", "������", 20, 10);


      dt.GridProducer.NewDefaultConfig(false);
      dt.GridProducer.DefaultConfig.Columns.Add("Number");
      dt.GridProducer.DefaultConfig.Columns.AddFill("Name", 100);
      dt.GridProducer.DefaultConfig.Columns.Add("Thumbnail");
      dt.GridProducer.DefaultConfig.ToolTips.Add("StateText");
      dt.GridProducer.DefaultConfig.ToolTips.Add("Comment");

      dt.AddImageHandler("Plant", new DBxColumns("MovementState"), new DBxImageValueNeededEventHandler(EditPlant.ImageValueNeeded));
      dt.InitView += new InitEFPDBxViewEventHandler(EditPlant.InitView);

      dt.BeforeEdit += new BeforeDocEditEventHandler(EditPlant.BeforeEditDoc);
      dt.InitEditForm += new InitDocEditFormEventHandler(EditPlant.InitDocEditForm);
      dt.CanInsertCopy = true;
      dt.CanMultiEdit = true;
      dt.DataBuffering = true;

      //dt.Writing += EditOrg.Writing;

      dt.Columns["Number"].NewMode = ColumnNewMode.AlwaysDefaultValue;

      #endregion

      #region ����

      sdt = dt.SubDocTypes["PlantPhotos"];
      sdt.GridProducer.Columns.Add(new PlantThumbnailColumn(true));
      sdt.GridProducer.Columns.AddText("FileName", "��� �����", 20, 7);
      sdt.GridProducer.Columns.AddDate("ShootingTime", "���� ������");
      sdt.GridProducer.Columns.AddText("Comment", "�����������", 20, 7);
      sdt.InitView += new InitEFPDBxViewEventHandler(EditPlant.SubDocPhoto_InitView);

      sdt.GridProducer.DefaultConfig = new EFPDataGridViewConfig();
      sdt.GridProducer.DefaultConfig.Columns.Add("Thumbnail");
      sdt.GridProducer.DefaultConfig.Columns.Add("FileName");
      sdt.GridProducer.DefaultConfig.Columns.Add("ShootingTime");
      sdt.GridProducer.DefaultConfig.Columns.AddFill("Comment");

      sdt.BeforeEdit += new BeforeSubDocEditEventHandler(EditPlant.SubDocPhoto_BeforeEdit);

      #endregion

      #region ��������

      sdt = dt.SubDocTypes["PlantAttributes"];
      sdt.AddImageHandler("Attribute", "AttrType,Value,LongValue", new DBxImageValueNeededEventHandler(EditAttrValue.ImageValueNeeded));
      sdt.GridProducer.Columns.AddText("AttrType.Name", "�������", 15, 5);
      sdt.GridProducer.Columns.LastAdded.CanIncSearch = true;
      sdt.GridProducer.Columns.AddDate("Date", "������ ��������");
      sdt.GridProducer.Columns.AddUserText("ValueText", "Value,LongValue,AttrType.Type", EditAttrValue.ValueColumnValueNeeded, "��������", 20, 10);

      sdt.GridProducer.NewDefaultConfig(false);
      sdt.GridProducer.DefaultConfig.Columns.Add("AttrType.Name");
      sdt.GridProducer.DefaultConfig.Columns.Add("Date");
      sdt.GridProducer.DefaultConfig.Columns.AddFill("ValueText");

      sdt.InitEditForm += new InitSubDocEditFormEventHandler(EditAttrValue.InitSubDocEditForm);
      sdt.CanMultiEdit = false;
      sdt.CanInsertCopy = true;

      sdt.Columns["AttrType"].NewMode = ColumnNewMode.Saved;
      sdt.Columns["Date"].NewMode = ColumnNewMode.Default;

      sdt.GetDocSel += new DocTypeDocSelEventHandler(EditAttrValue.GetDocSel);

      #endregion

      #region �����������

      sdt = dt.SubDocTypes["PlantMovement"];
      sdt.GridProducer.Columns.AddEnumText("Kind", PlantTools.MovementNames, "��������", 10, 5);
      sdt.GridProducer.Columns.AddDateRange("Date", "Date1", "Date2", "����", false, 15, 10);
      sdt.GridProducer.Columns.AddText("Place.Name", "�����", 20, 5);
      sdt.GridProducer.Columns.AddText("Contra.Name", "�� ���� / ����", 20, 5);
      sdt.GridProducer.Columns.AddText("ForkPlant.Name", "�������� / ���������", 20, 10);
      sdt.GridProducer.Columns.AddText("Soil.Name", "�����", 20, 10);
      sdt.GridProducer.Columns.AddText("PotKind.Name", "������", 20, 10);
      sdt.GridProducer.Columns.AddText("Comment", "�����������", 30, 10);

      sdt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "����������� (���� �����)";

      sdt.GridProducer.DefaultConfig = new EFPDataGridViewConfig();
      sdt.GridProducer.DefaultConfig.Columns.Add("Date");
      sdt.GridProducer.DefaultConfig.Columns.AddFill("KindText_Text", 40);
      sdt.GridProducer.DefaultConfig.Columns.AddFill("Place.Name", 30);
      sdt.GridProducer.DefaultConfig.Columns.AddFill("Contra.Name", 30);
      sdt.GridProducer.DefaultConfig.ToolTips.Add("Comment");

      sdt.AddImageHandler("PlantMovement", new DBxColumns("Kind"),
        new DBxImageValueNeededEventHandler(EditPlantMovement.ImageValueNeeded));

      sdt.CanInsertCopy = true;
      sdt.InitEditForm += new InitSubDocEditFormEventHandler(EditPlantMovement.InitEditForm);
      sdt.Columns["Date1"].NewMode = ColumnNewMode.Saved;
      sdt.Columns["Date2"].NewMode = ColumnNewMode.Saved;
      sdt.Columns["Date1"].DefaultValueNeeded += new EventHandler(DateColumn_DefaultValueNeeded);
      sdt.Columns["Date2"].DefaultValueNeeded += new EventHandler(DateColumn_DefaultValueNeeded);
      sdt.Columns["Place"].NewMode = ColumnNewMode.Saved;
      sdt.Columns["Contra"].NewMode = ColumnNewMode.Saved;
      sdt.Columns["Soil"].NewMode = ColumnNewMode.Saved;
      sdt.Columns["PotKind"].NewMode = ColumnNewMode.Saved;

      #endregion

      #region ��������

      sdt = dt.SubDocTypes["PlantActions"];
      sdt.GridProducer.Columns.AddUserText("ActionText", "Kind,ActionName,Remedy", /* ������ ������������ Remedy.Name */
        new EFPGridProducerValueNeededEventHandler(EditPlantAction.ActionTextColumnValueNeeded),
        "��������", 30, 10);
      sdt.GridProducer.Columns.AddDateRange("Date", "Date1", "Date2", "����", false, 15, 10);
      sdt.GridProducer.Columns.AddText("Soil.Name", "�����", 20, 10);
      sdt.GridProducer.Columns.AddText("PotKind.Name", "������", 20, 10);
      sdt.GridProducer.Columns.AddText("Comment", "�����������", 30, 10);

      sdt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "����������� (���� �����)";

      sdt.GridProducer.DefaultConfig = new EFPDataGridViewConfig();
      sdt.GridProducer.DefaultConfig.Columns.AddFill("ActionText");
      sdt.GridProducer.DefaultConfig.Columns.Add("Date");
      sdt.GridProducer.DefaultConfig.ToolTips.Add("Comment");

      //sdt.ImageKey = "PlantAction";
      sdt.AddImageHandler("PlantAction", new DBxColumns("Kind"),
        new DBxImageValueNeededEventHandler(EditPlantAction.ImageValueNeeded));

      sdt.CanInsertCopy = true;
      sdt.InitEditForm += new InitSubDocEditFormEventHandler(EditPlantAction.InitEditForm);
      sdt.Columns["Date1"].NewMode = ColumnNewMode.Saved;
      sdt.Columns["Date2"].NewMode = ColumnNewMode.Saved;
      sdt.Columns["Date1"].DefaultValueNeeded += new EventHandler(DateColumn_DefaultValueNeeded);
      sdt.Columns["Date2"].DefaultValueNeeded += new EventHandler(DateColumn_DefaultValueNeeded);
      sdt.Columns["Kind"].NewMode = ColumnNewMode.Saved;
      sdt.Columns["ActionName"].NewMode = ColumnNewMode.Saved;
      sdt.Columns["Soil"].NewMode = ColumnNewMode.Saved;
      sdt.Columns["PotKind"].NewMode = ColumnNewMode.Saved;

      #endregion

      #region ��������

      sdt = dt.SubDocTypes["PlantFlowering"];
      sdt.GridProducer.Columns.AddDateRange("Date", "Date1", "Date2", "����", false, 15, 10);
      sdt.GridProducer.Columns.AddInt("FlowerCount", "���������� �������", 3);
      sdt.GridProducer.Columns.AddText("Comment", "�����������", 30, 10);

      sdt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "����������� (���� �����)";

      sdt.GridProducer.DefaultConfig = new EFPDataGridViewConfig();
      sdt.GridProducer.DefaultConfig.Columns.Add("Date");
      sdt.GridProducer.DefaultConfig.Columns.Add("FlowerCount");
      sdt.GridProducer.DefaultConfig.ToolTips.Add("Comment");

      sdt.ImageKey = "PlantFlowering";
      sdt.CanInsertCopy = true;
      sdt.CanMultiEdit = true;
      sdt.InitEditForm += new InitSubDocEditFormEventHandler(EditPlantFlowering.InitEditForm);
      sdt.Columns["Date1"].NewMode = ColumnNewMode.Saved;
      sdt.Columns["Date2"].NewMode = ColumnNewMode.Saved;
      sdt.Columns["Date1"].DefaultValueNeeded += new EventHandler(DateColumn_DefaultValueNeeded);
      sdt.Columns["Date2"].DefaultValueNeeded += new EventHandler(DateColumn_DefaultValueNeeded);

      #endregion

      #region �����������

      sdt = dt.SubDocTypes["PlantDiseases"];
      sdt.GridProducer.Columns.AddText("Disease.Name", "�����������", 30, 5);
      sdt.GridProducer.Columns.AddDateRange("Date", "Date1", "Date2", "����", false, 15, 10);
      sdt.GridProducer.Columns.AddText("Comment", "�����������", 30, 10);

      sdt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "����������� (���� �����)";

      sdt.GridProducer.DefaultConfig = new EFPDataGridViewConfig();
      sdt.GridProducer.DefaultConfig.Columns.AddFill("Disease.Name");
      sdt.GridProducer.DefaultConfig.Columns.Add("Date");
      sdt.GridProducer.DefaultConfig.ToolTips.Add("Comment");

      sdt.ImageKey = "Disease";

      sdt.CanInsertCopy = true;
      sdt.InitEditForm += new InitSubDocEditFormEventHandler(EditPlantDisease.InitEditForm);
      sdt.Columns["Date1"].NewMode = ColumnNewMode.Saved;
      sdt.Columns["Date2"].NewMode = ColumnNewMode.Saved;
      sdt.Columns["Date1"].DefaultValueNeeded += new EventHandler(DateColumn_DefaultValueNeeded);
      sdt.Columns["Disease"].NewMode = ColumnNewMode.Saved;

      #endregion

      #region �����

      sdt = dt.SubDocTypes["PlantPlans"];
      sdt.GridProducer.Columns.AddUserText("ActionText", "Kind,ActionName,Remedy",
        new EFPGridProducerValueNeededEventHandler(EditPlantPlan.ActionTextColumnValueNeeded),
        "��������", 30, 10);
      sdt.GridProducer.Columns.AddDateRange("Date", "Date1", "Date2", "����", false, 15, 10);
      sdt.GridProducer.Columns.AddText("Comment", "�����������", 30, 10);

      sdt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "����������� (���� �����)";

      sdt.GridProducer.DefaultConfig = new EFPDataGridViewConfig();
      sdt.GridProducer.DefaultConfig.Columns.AddFill("ActionText");
      sdt.GridProducer.DefaultConfig.Columns.Add("Date");
      sdt.GridProducer.DefaultConfig.ToolTips.Add("Comment");

      sdt.AddImageHandler("Calendar", new DBxColumns("Kind"),
        new DBxImageValueNeededEventHandler(EditPlantPlan.ImageValueNeeded));

      sdt.CanInsertCopy = true;
      sdt.InitEditForm += new InitSubDocEditFormEventHandler(EditPlantPlan.InitEditForm);
      sdt.Columns["Date1"].NewMode = ColumnNewMode.Saved;
      sdt.Columns["Date2"].NewMode = ColumnNewMode.Saved;
      // ����� ���� �������� �� �� ����������� ����
      //sdt.Columns["Date1"].DefaultValueNeeded += new EventHandler(DateColumn_DefaultValueNeeded);
      //sdt.Columns["Date2"].DefaultValueNeeded += new EventHandler(DateColumn_DefaultValueNeeded);
      sdt.Columns["Kind"].NewMode = ColumnNewMode.Saved;
      sdt.Columns["ActionName"].NewMode = ColumnNewMode.Saved;

      #endregion

      #endregion

      #region �����

      dt = base.DocTypes["Places"];

      dt.GridProducer.Columns.AddText("Name", "��������", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      dt.GridProducer.Columns.AddText("GroupId.Name", "������", 15, 5);
      dt.GridProducer.Columns.AddText("Comment", "�����������", 30, 10);

      dt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "����������� (���� �����)";

      // dt.GridProducer.Orders.Add("Name", "��������");

      dt.GridProducer.NewDefaultConfig(false);
      dt.GridProducer.DefaultConfig.Columns.AddFill("Name", 100);
      dt.GridProducer.DefaultConfig.ToolTips.Add("Comment");

      dt.ImageKey = "Place";

      dt.InitEditForm += new InitDocEditFormEventHandler(EditPlace.InitDocEditForm);
      dt.CanInsertCopy = true;
      dt.CanMultiEdit = true; // ����� ������ ������ � ���������� ���������� �����
      dt.DataBuffering = true;
      dt.Columns["Name"].NewMode = ColumnNewMode.AlwaysDefaultValue;

      #endregion

      #region �����������

      dt = base.DocTypes["Contras"];

      dt.GridProducer.Columns.AddText("Name", "��������", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      dt.GridProducer.Columns.AddText("GroupId.Name", "������", 15, 5);
      dt.GridProducer.Columns.AddText("Comment", "�����������", 30, 10);

      dt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "����������� (���� �����)";

      dt.GridProducer.NewDefaultConfig(false);
      dt.GridProducer.DefaultConfig.Columns.AddFill("Name", 100);
      dt.GridProducer.DefaultConfig.ToolTips.Add("Comment");

      dt.ImageKey = "Contra";

      dt.InitEditForm += new InitDocEditFormEventHandler(EditContra.InitDocEditForm);
      dt.CanInsertCopy = true;
      dt.CanMultiEdit = true; // ����� ������ ������ � ���������� ���������� �����
      dt.DataBuffering = true;
      dt.Columns["Name"].NewMode = ColumnNewMode.AlwaysDefaultValue;

      #endregion

      #region �����������

      dt = base.DocTypes["Companies"];

      dt.GridProducer.Columns.AddText("Name", "��������", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      dt.GridProducer.Columns.AddText("GroupId.Name", "������", 15, 5);
      dt.GridProducer.Columns.AddText("Comment", "�����������", 30, 10);

      dt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "����������� (���� �����)";

      dt.GridProducer.NewDefaultConfig(false);
      dt.GridProducer.DefaultConfig.Columns.AddFill("Name", 100);
      dt.GridProducer.DefaultConfig.ToolTips.Add("Comment");

      dt.ImageKey = "Company";

      dt.InitEditForm += new InitDocEditFormEventHandler(EditCompany.InitDocEditForm);
      dt.CanInsertCopy = true;
      dt.CanMultiEdit = true; // ����� ������ ������ � ���������� ���������� �����
      dt.DataBuffering = true;
      dt.Columns["Name"].NewMode = ColumnNewMode.AlwaysDefaultValue;

      #endregion

      #region ���� �����������

      dt = base.DocTypes["Diseases"];

      dt.GridProducer.Columns.AddText("Name", "��������", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      dt.GridProducer.Columns.AddText("GroupId.Name", "������", 15, 5);

      dt.GridProducer.Columns.AddText("Comment", "�����������", 30, 10);

      dt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "����������� (���� �����)";

      dt.GridProducer.NewDefaultConfig(false);
      dt.GridProducer.DefaultConfig.Columns.AddFill("Name", 100);
      dt.GridProducer.DefaultConfig.ToolTips.Add("Comment");

      dt.ImageKey = "Disease";

      dt.InitEditForm += new InitDocEditFormEventHandler(EditDisease.InitDocEditForm);
      dt.CanInsertCopy = true;
      dt.CanMultiEdit = true; // ����� ������ ������ � ���������� ���������� �����
      dt.DataBuffering = true;
      dt.Columns["Name"].NewMode = ColumnNewMode.AlwaysDefaultValue;

      #endregion

      #region ���������

      #region �������� ��������

      dt = base.DocTypes["Remedies"];

      dt.GridProducer.Columns.AddText("Name", "��������", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      dt.GridProducer.Columns.AddText("Manufacturer.Name", "������������", 20, 5);

      dt.GridProducer.Columns.AddText("GroupId.Name", "������", 15, 5);
      dt.GridProducer.Columns.AddText("Comment", "�����������", 30, 10);

      dt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "����������� (���� �����)";

      dt.GridProducer.NewDefaultConfig(false);
      dt.GridProducer.DefaultConfig.Columns.AddFill("Name", 100);
      dt.GridProducer.DefaultConfig.ToolTips.Add("Comment");

      dt.ImageKey = "Remedy";
      dt.InitView += new InitEFPDBxViewEventHandler(EditRemedy.InitView);

      dt.InitEditForm += new InitDocEditFormEventHandler(EditRemedy.InitDocEditForm);
      dt.CanInsertCopy = true;
      dt.CanMultiEdit = true; // ����� ������ ������ � ���������� ���������� �����
      dt.DataBuffering = true;
      dt.Columns["Name"].NewMode = ColumnNewMode.AlwaysDefaultValue;
      dt.Columns["Manufacturer"].NewMode = ColumnNewMode.Saved;

      #endregion

      #region ������� ���������� ���������

      sdt = dt.SubDocTypes["RemedyUsage"];
      sdt.GridProducer.Columns.AddText("Name", "��������", 40, 15);
      sdt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      sdt.GridProducer.DefaultConfig = new EFPDataGridViewConfig();
      sdt.GridProducer.DefaultConfig.Columns.AddFill("Name");

      sdt.ImageKey = "Item";
      sdt.InitEditForm += new InitSubDocEditFormEventHandler(EditRemedyUsage.InitEditForm);
      sdt.CanInsertCopy = false;
      sdt.CanMultiEdit = false;

      #endregion

      #endregion

      #region ������

      #region �������� ��������

      dt = base.DocTypes["Soils"];

      dt.GridProducer.Columns.AddText("Name", "��������", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      dt.GridProducer.Columns.AddText("Manufacturer.Name", "������������", 20, 5);

      dt.GridProducer.Columns.AddInt("PartCount", "���������� �����������", 2);
      dt.GridProducer.Columns.AddText("Contents", "������", 50, 10);
      dt.GridProducer.Columns.AddUserText("pHtext", "pHmin,pHmax",
        new EFPGridProducerValueNeededEventHandler(EditSoil.pHcolumnValueNeeded),
        "pH", 7, 7);
      dt.GridProducer.Columns.LastAdded.TextAlign = HorizontalAlignment.Center;

      dt.GridProducer.Columns.AddText("GroupId.Name", "������", 15, 5);
      dt.GridProducer.Columns.AddText("Comment", "�����������", 30, 10);

      dt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "����������� (���� �����)";

      dt.GridProducer.NewDefaultConfig(false);
      dt.GridProducer.DefaultConfig.Columns.AddFill("Name", 100);
      dt.GridProducer.DefaultConfig.Columns.Add("PartCount");
      dt.GridProducer.DefaultConfig.ToolTips.Add("Comment");

      dt.ImageKey = "Soil";
      dt.InitView += new InitEFPDBxViewEventHandler(EditSoil.InitView);

      dt.InitEditForm += new InitDocEditFormEventHandler(EditSoil.InitDocEditForm);
      dt.CanInsertCopy = true;
      dt.CanMultiEdit = true; // ����� ������ ������ � ���������� ���������� �����
      dt.DataBuffering = true;
      dt.Columns["Name"].NewMode = ColumnNewMode.AlwaysDefaultValue;
      dt.Columns["Manufacturer"].NewMode = ColumnNewMode.Saved;

      #endregion

      #region ����������� "��������� ������"

      sdt = dt.SubDocTypes["SoilParts"];
      sdt.GridProducer.Columns.AddText("Soil.Name", "���������", 20, 10);
      sdt.GridProducer.Columns.LastAdded.CanIncSearch = true;
      sdt.GridProducer.Columns.AddInt("Percent", "%", 3);

      sdt.GridProducer.NewDefaultConfig(false);
      sdt.GridProducer.DefaultConfig.Columns.AddFill("Soil.Name", 100);
      sdt.GridProducer.DefaultConfig.Columns.Add("Percent");

      sdt.InitEditForm += new InitSubDocEditFormEventHandler(EditSoilPart.InitEditForm);
      sdt.CanInsertCopy = false;
      sdt.CanMultiEdit = false;
      dt.Columns["Soil"].NewMode = ColumnNewMode.AlwaysDefaultValue;

      #endregion

      #endregion

      #region ������

      dt = base.DocTypes["PotKinds"];

      dt.GridProducer.Columns.AddText("Name", "��������", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      dt.GridProducer.Columns.AddText("Text", "��������� �����", 20, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      dt.GridProducer.Columns.AddInt("Diameter", "d, ��", 3);
      dt.GridProducer.Columns.LastAdded.SizeGroup = "SizeMM";
      dt.GridProducer.Columns.AddInt("Height", "H, ��", 3);
      dt.GridProducer.Columns.LastAdded.SizeGroup = "SizeMM";

      dt.GridProducer.Columns.AddFixedPoint("Volume", "�����, �", 5, 2, "VolumeL");
      dt.GridProducer.Columns.AddText("Color", "����", 10, 5);

      dt.GridProducer.Columns.AddText("Manufacturer.Name", "������������", 20, 5);

      dt.GridProducer.Columns.AddText("GroupId.Name", "������", 15, 5);
      dt.GridProducer.Columns.AddText("Comment", "�����������", 30, 10);

      dt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "����������� (���� �����)";

      dt.GridProducer.NewDefaultConfig(false);
      dt.GridProducer.DefaultConfig.Columns.AddFill("Name", 100);
      dt.GridProducer.DefaultConfig.ToolTips.Add("Comment");

      dt.ImageKey = "PotKind";
      dt.InitView += new InitEFPDBxViewEventHandler(EditPotKind.InitView);

      dt.InitEditForm += new InitDocEditFormEventHandler(EditPotKind.InitDocEditForm);
      dt.CanInsertCopy = true;
      dt.CanMultiEdit = true;
      dt.DataBuffering = true;
      dt.Columns["Name"].NewMode = ColumnNewMode.Default;
      dt.Columns["Manufacturer"].NewMode = ColumnNewMode.Saved;


      #endregion

      #region ���� ���������

      dt = base.DocTypes["AttrTypes"];
      dt.ImageKey = "AttributeType";
      dt.GridProducer.Columns.AddText("Name", "��� ��������", 20, 5);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      dt.GridProducer.Columns.AddUserText("TypeText", "Type",
        new EFPGridProducerValueNeededEventHandler(EditAttrType.TypeColumnValueNeeded),
        "���", 15, 2);

      dt.GridProducer.Columns.AddText("Comment", "�����������", 30, 10);

      dt.GridProducer.ToolTips.AddText("Comment", "").DisplayName = "����������� (���� �����)";

      dt.GridProducer.NewDefaultConfig(false);
      dt.GridProducer.DefaultConfig.Columns.AddFill("Name");
      dt.GridProducer.DefaultConfig.Columns.Add("TypeText");
      dt.GridProducer.DefaultConfig.ToolTips.Add("Comment");

      dt.Columns["Name"].NewMode = ColumnNewMode.AlwaysDefaultValue;

      dt.InitEditForm += new InitDocEditFormEventHandler(EditAttrType.InitDocEditForm);
      dt.CanInsertCopy = false;
      dt.CanMultiEdit = false;
      dt.DataBuffering = true;

      #endregion

      #region ����

      #region �������� ��������

      dt = base.DocTypes["Care"];

      dt.GridProducer.Columns.AddText("Name", "��������", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      dt.GridProducer.Columns.AddText("Parent.Name", "��������", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      dt.GridProducer.Columns.AddText("GroupId.Name", "������", 15, 5);
      dt.GridProducer.Columns.AddText("Comment", "�����������", 30, 10);

      dt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "����������� (���� �����)";

      dt.GridProducer.Orders.Add("Name", "��������");


      dt.GridProducer.NewDefaultConfig(false);
      dt.GridProducer.DefaultConfig.Columns.AddFill("Name", 100);
      dt.GridProducer.DefaultConfig.ToolTips.Add("Comment");

      dt.ImageKey = "Care";
      //dt.InitView += new InitEFPDBxViewEventHandler(EditCare.InitView);

      dt.InitEditForm += new InitDocEditFormEventHandler(EditCare.InitDocEditForm);
      dt.CanInsertCopy = true;
      dt.CanMultiEdit = true; // ������ ������� "�����"
      dt.DataBuffering = false;

      //dt.Writing += EditOrg.Writing;

      #endregion

      #region ������ �� �����

      sdt = dt.SubDocTypes["CareRecords"];
      sdt.GridProducer.Columns.AddMonthDayRange("PeriodText", "Day1", "Day2",
        "������", false).EmptyValue = "���";
      sdt.GridProducer.Columns.LastAdded.TextAlign = HorizontalAlignment.Center;
      sdt.GridProducer.Columns.AddText("Name", "�������� �������", 20, 10);
      //sdt.GridProducer.Columns.AddText("Comment", "�����������", 30, 10);

      //sdt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "����������� (���� �����)";

      sdt.GridProducer.DefaultConfig = new EFPDataGridViewConfig();
      sdt.GridProducer.DefaultConfig.Columns.Add("PeriodText");
      sdt.GridProducer.DefaultConfig.Columns.AddFill("Name", 100);
      //sdt.GridProducer.DefaultConfig.ToolTips.Add("Comment");

      sdt.ImageKey = "CareRecord";

      sdt.CanInsertCopy = true;
      sdt.InitEditForm += new InitSubDocEditFormEventHandler(EditCareRecord.InitEditForm);

      #endregion

      #endregion

      InitDocTextValues.Init(base.TextHandlers);
      base.EndInit();
    }

    private static void DateColumn_DefaultValueNeeded(object Sender, EventArgs Args)
    {
      ColumnUI ColUI = (ColumnUI)Sender;
      ColUI.DefaultValue = DateTime.Today;
    }

    #endregion

    #region ������ ������ �������� �������� �� ������

    /// <summary>
    /// ������ ������ �������� ��������
    /// </summary>
    /// <param name="Value">����-�����: ���������� ��������</param>
    /// <param name="Title">��������� ����� �������</param>
    /// <param name="AttrType">�������� "��� ��������"</param>
    /// <returns>true, ���� �������� ���� �������</returns>
    public static bool SelectAttrValue(ref object Value, string Title, AttrTypeDoc AttrType)
    {
      switch (AttrType.SourceType)
      {
        case AttrValueSourceType.List:
          return SelectAttrValueList(ref Value, Title, AttrType);
        default:
          if (AttrType.ValueType == ValueType.Boolean)
            return SelectAttrValueBoolean(ref Value, Title, AttrType);
          EFPApp.ShowTempMessage("��� �������� \"" + AttrType.Name + "\" ������������ ������ ������ ���� ��������");
          return false;
      }
    }

    private static bool SelectAttrValueList(ref object Value, string Title, AttrTypeDoc AttrType)
    {
      if (AttrType.ValueList.Length == 0)
      {
        EFPApp.ShowTempMessage("� ���� �������� \"" + AttrType.Name + "\" ��� ������ �������� ��� ������");
        return false;
      }

      ListSelectDialog dlg = new ListSelectDialog();
      dlg.Title = Title;
      dlg.ListTitle = "�������� �������� \"" + AttrType.Name + "\"";
      string[] a = new string[AttrType.ValueList.Length];
      for (int i = 0; i < AttrType.ValueList.Length; i++)
        a[i] = PlantTools.ToString(AttrType.ValueList[i], AttrType.ValueType);
      dlg.Items = a;
      dlg.SelectedIndex = Array.IndexOf(AttrType.ValueList, Value);
      if (dlg.ShowDialog() != DialogResult.OK)
        return false;

      Value = AttrType.ValueList[dlg.SelectedIndex];
      return true;
    }

    private static bool SelectAttrValueBoolean(ref object Value, string Title, AttrTypeDoc AttrType)
    {
      RadioSelectDialog dlg = new RadioSelectDialog();
      dlg.Title = Title;
      //dlg.ImageKey = "�������";
      dlg.GroupTitle = "�������� �������� \"" + AttrType.Name + "\"";
      dlg.Items = new string[] { "&1 - ��", "&0 - ���" };
      dlg.SelectedIndex = DataTools.GetBool(Value) ? 0 : 1;
      if (dlg.ShowDialog() != DialogResult.OK)
        return false;

      Value = (dlg.SelectedIndex == 0);
      return true;
    }

    #endregion

    #region ������ ������

    /// <summary>
    /// �������� � GridProducer ������� �������, ��������� � ����� ������, ��� ���������� ���� ������
    /// </summary>
    /// <param name="Producer"></param>
    /// <param name="ValueType"></param>
    /// <param name="ColumnName"></param>
    public static void AddValueTypeColumn(EFPGridProducer Producer, ValueType ValueType, string ColumnName, string HeaderText)
    {
      switch (ValueType)
      {
        case ValueType.String:
          Producer.Columns.AddText(ColumnName, HeaderText, 10, 5);
          break;
        case ValueType.Integer:
          Producer.Columns.AddInt(ColumnName, HeaderText, 5);
          Producer.Columns.LastAdded.TextAlign = HorizontalAlignment.Right;
          break;
        case ValueType.Double:
          Producer.Columns.AddText(ColumnName, HeaderText, 10, 5);
          Producer.Columns.LastAdded.SizeGroup = "Double";
          Producer.Columns.LastAdded.TextAlign = HorizontalAlignment.Right;
          break;
        case ValueType.Decimal:
          Producer.Columns.AddText(ColumnName, HeaderText, 10, 5);
          Producer.Columns.LastAdded.SizeGroup = "Decimal";
          Producer.Columns.LastAdded.TextAlign = HorizontalAlignment.Right;
          break;
        case ValueType.Boolean:
          Producer.Columns.AddBool(ColumnName, HeaderText);
          break;
        case ValueType.Date:
          Producer.Columns.AddDate(ColumnName, HeaderText);
          break;
        case ValueType.DateTime:
          Producer.Columns.AddDateTime(ColumnName, HeaderText);
          break;
        default:
          throw new ArgumentException("����������� ��� ������: " + ValueType.ToString() + " ��� ������� \"" + ColumnName + "\"", "ValueType");
      }
    }



    /// <summary>
    /// ���������� �������������� ������������ �� ��������� ��� �������� ��������� ����
    /// </summary>
    /// <param name="ValueType">��� ��������</param>
    /// <returns>������������</returns>
    public static HorizontalAlignment GetAlignment(ValueType ValueType)
    {
      switch (ValueType)
      {
        case ValueType.String:
          return HorizontalAlignment.Left;
        case ValueType.Date:
        case ValueType.DateTime:
        case ValueType.Boolean:
          return HorizontalAlignment.Center;
        default:
          return HorizontalAlignment.Right;
      }
    }

    #endregion

    #region ����������� ��������

    public static ProgramDBUI TheUI;

    public static ClientConfigSections ConfigSections;

    //public static CoProDevDocuments Docs;

    //public static ProgramCache Cache;

    /// <summary>
    /// ���������
    /// </summary>
    public static UserSettings Settings;

    #endregion
  }
}
