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
    #region Конструктор

    public ProgramDBUI(DBxDocProviderProxy sourceDocProviderProxy)
      : base(sourceDocProviderProxy)
    {
      DocTypeUI dt;
      SubDocTypeUI sdt;

      #region Каталог растений

      #region Основной документ

      dt = base.DocTypes["Plants"];

      dt.GridProducer.Columns.AddText("Number", "№ по каталогу", 3, 3);
      dt.GridProducer.Columns.LastAdded.Format = ProgramDBUI.Settings.NumberMask;
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;
      dt.GridProducer.Columns.LastAdded.TextAlign = HorizontalAlignment.Center;

      dt.GridProducer.Columns.AddText("Name", "Название или описание", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;
      dt.GridProducer.Columns.AddText("LocalName", "Русское название", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;
      dt.GridProducer.Columns.AddText("LatinName", "Латинское название", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;
      dt.GridProducer.Columns.AddText("Description", "Описание", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      dt.GridProducer.Columns.Add(new PlantThumbnailColumn(false));

      dt.GridProducer.Columns.AddText("Place.Name", "Место", 15, 10);
      dt.GridProducer.Columns.AddDateRange("AddDate", "AddDate1", "AddDate2", "Дата поступления", false, 10, 10);
      dt.GridProducer.Columns.LastAdded.TextAlign = HorizontalAlignment.Center;
      dt.GridProducer.Columns.LastAdded.EmptyValue = String.Empty; // а не "все даты"
      dt.GridProducer.Columns.AddText("FromContra.Name", "От кого получено", 15, 10);
      dt.GridProducer.Columns.AddRefDocText("FromPlant", DocTypes["Plants"], "Отсажено от растения", 15, 10);

      dt.GridProducer.Columns.AddUserText("StateText", "MovementState,Place,ToContra", new EFPGridProducerValueNeededEventHandler(EditPlant.StateText_Column_ToolTipText_ValueNeeded), "Состояние", 30, 5);

      dt.GridProducer.Columns.AddRefDocTextAndImage("LastPlantAction", this.DocTypes["Plants"].SubDocTypes["PlantActions"], "Последнее действие", 30, 10);
      dt.GridProducer.Columns.AddDateRange("LastPlantActionDate", "LastPlantAction.Date1", "LastPlantAction.Date2", "Дата последнего действия", false, 10, 10);
      dt.GridProducer.Columns.LastAdded.TextAlign = HorizontalAlignment.Center;
      dt.GridProducer.Columns.LastAdded.EmptyValue = String.Empty; // а не "все даты"

      dt.GridProducer.Columns.AddRefDocTextAndImage("LastPlantReplanting", this.DocTypes["Plants"].SubDocTypes["PlantActions"], "Пересадка", 30, 10);
      dt.GridProducer.Columns.AddDateRange("LastPlantReplantingDate", "LastPlantReplanting.Date1", "LastPlantReplanting.Date2", "Дата пересадки", false, 10, 10);
      dt.GridProducer.Columns.LastAdded.TextAlign = HorizontalAlignment.Center;
      dt.GridProducer.Columns.LastAdded.EmptyValue = String.Empty; // а не "все даты"

      dt.GridProducer.Columns.AddRefDocText("LastPlantDisease", this.DocTypes["Plants"].SubDocTypes["PlantDiseases"], "Заболевание", 30, 10);

      dt.GridProducer.Columns.AddDateRange("RemoveDate", "RemoveDate1", "RemoveDate2", "Дата выбытия", false, 10, 10);
      dt.GridProducer.Columns.LastAdded.TextAlign = HorizontalAlignment.Center;
      dt.GridProducer.Columns.LastAdded.EmptyValue = String.Empty; // а не "все даты"
      dt.GridProducer.Columns.AddText("ToContra.Name", "Кому передано", 15, 10);
      dt.GridProducer.Columns.AddRefDocText("ToPlant", DocTypes["Plants"], "Подсажено к растению", 15, 10);

      dt.GridProducer.Columns.AddUserText("ContraName", "FromContra,ToContra,FromPlant,ToPlant",
        new EFPGridProducerValueNeededEventHandler(EditPlant.ContraNameColumnValueNeeded),
        "От кого / кому", 15, 10);

      dt.GridProducer.Columns.AddText("Manufacturer.Name", "Изготовитель (питомник)", 15, 10);
      dt.GridProducer.Columns.AddText("Care.Name", "Уход", 15, 10);


      dt.GridProducer.Columns.AddUserImage("FirstPlannedActionImage", "FirstPlannedAction.Date1,FirstPlannedAction.Date2,FirstPlannedAction.Kind",
        new EFPGridProducerValueNeededEventHandler(EditPlant.FirstPlannedActionImageColumnValueNeeded), String.Empty).DisplayName = "Запланированное действие (значок)";
      dt.GridProducer.Columns.AddUserText("FirstPlannedActionText", "FirstPlannedAction.Kind,FirstPlannedAction.ActionName",
        new EFPGridProducerValueNeededEventHandler(EditPlant.FirstPlannedActionTextColumnValueNeeded), "Запланированное действие", 15, 10);
      dt.GridProducer.Columns.AddDateRange("FirstPlannedActionDate", "FirstPlannedAction.Date1", "FirstPlannedAction.Date2", "Дата запланированного действия", false, 15, 10);

      dt.GridProducer.Columns.AddText("GroupId.Name", "Группа", 15, 5);
      dt.GridProducer.Columns.AddText("Comment", "Комментарий", 30, 10);

      dt.GridProducer.ToolTips.AddText("LocalName", "Русское название");
      dt.GridProducer.ToolTips.AddText("LatinName", "Латинское название");
      dt.GridProducer.ToolTips.AddText("Description", "Описание");
      dt.GridProducer.ToolTips.AddUserItem("StateText", "MovementState,Place,ToContra", new EFPGridProducerValueNeededEventHandler(EditPlant.StateText_Column_ToolTipText_ValueNeeded), "Состояние");
      dt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "Комментарий (если задан)";

      dt.GridProducer.Orders.Add("Number", "Номер по каталогу");
      dt.GridProducer.Orders.Add("Name", "Название или описание");
      dt.GridProducer.Orders.Add("LocalName", "Русское название");
      dt.GridProducer.Orders.Add("LatinName", "Латинское название");
      dt.GridProducer.Orders.Add("Description", "Описание");
      dt.GridProducer.Orders.Add("AddDate1", "Дата прихода (по возрастанию)", new EFPDataGridViewSortInfo("AddDate", ListSortDirection.Ascending));
      dt.GridProducer.Orders.Add("AddDate1 DESC", "Дата прихода (по убыванию)", new EFPDataGridViewSortInfo("AddDate", ListSortDirection.Descending));
      dt.GridProducer.Orders.Add("RemoveDate1", "Дата выбытия (по возрастанию)", new EFPDataGridViewSortInfo("RemoveDate", ListSortDirection.Ascending));
      dt.GridProducer.Orders.Add("RemoveDate1 DESC", "Дата выбытия (по убыванию)", new EFPDataGridViewSortInfo("RemoveDate", ListSortDirection.Descending));
      dt.GridProducer.Orders.Add("LastPlantAction.Date1", "Дата последнего действия (по возрастанию)", new EFPDataGridViewSortInfo("LastPlantActionDate", ListSortDirection.Ascending));
      dt.GridProducer.Orders.Add("LastPlantAction.Date1 DESC", "Дата последнего действия (по убыванию)", new EFPDataGridViewSortInfo("LastPlantActionDate", ListSortDirection.Descending));
      dt.GridProducer.Orders.Add("LastPlantReplanting.Date1", "Дата пересадки (по возрастанию)", new EFPDataGridViewSortInfo("LastPlantReplantingDate", ListSortDirection.Ascending));
      dt.GridProducer.Orders.Add("LastPlantReplanting.Date1 DESC", "Дата пересадки (по убыванию)", new EFPDataGridViewSortInfo("LastPlantReplantingDate", ListSortDirection.Descending));
      dt.GridProducer.Orders.Add("LastDiseaseDate1", "Дата заболевания (по возрастанию)", new EFPDataGridViewSortInfo("LastDiseaseDate", ListSortDirection.Ascending));
      dt.GridProducer.Orders.Add("LastDiseaseDate1 DESC", "Дата заболевания (по убыванию)", new EFPDataGridViewSortInfo("LastDiseaseDate", ListSortDirection.Descending));
      dt.GridProducer.Columns.AddText("Soil.Name", "Грунт", 20, 10);
      dt.GridProducer.Columns.AddText("PotKind.Name", "Горшок", 20, 10);


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

      #region Фото

      sdt = dt.SubDocTypes["PlantPhotos"];
      sdt.GridProducer.Columns.Add(new PlantThumbnailColumn(true));
      sdt.GridProducer.Columns.AddText("FileName", "Имя файла", 20, 7);
      sdt.GridProducer.Columns.AddDate("ShootingTime", "Дата съемки");
      sdt.GridProducer.Columns.AddText("Comment", "Комментарий", 20, 7);
      sdt.InitView += new InitEFPDBxViewEventHandler(EditPlant.SubDocPhoto_InitView);

      sdt.GridProducer.DefaultConfig = new EFPDataGridViewConfig();
      sdt.GridProducer.DefaultConfig.Columns.Add("Thumbnail");
      sdt.GridProducer.DefaultConfig.Columns.Add("FileName");
      sdt.GridProducer.DefaultConfig.Columns.Add("ShootingTime");
      sdt.GridProducer.DefaultConfig.Columns.AddFill("Comment");

      sdt.BeforeEdit += new BeforeSubDocEditEventHandler(EditPlant.SubDocPhoto_BeforeEdit);

      #endregion

      #region Атрибуты

      sdt = dt.SubDocTypes["PlantAttributes"];
      sdt.AddImageHandler("Attribute", "AttrType,Value,LongValue", new DBxImageValueNeededEventHandler(EditAttrValue.ImageValueNeeded));
      sdt.GridProducer.Columns.AddText("AttrType.Name", "Атрибут", 15, 5);
      sdt.GridProducer.Columns.LastAdded.CanIncSearch = true;
      sdt.GridProducer.Columns.AddDate("Date", "Начало действия");
      sdt.GridProducer.Columns.AddUserText("ValueText", "Value,LongValue,AttrType.Type", EditAttrValue.ValueColumnValueNeeded, "Значение", 20, 10);

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

      #region Перемещение

      sdt = dt.SubDocTypes["PlantMovement"];
      sdt.GridProducer.Columns.AddEnumText("Kind", PlantTools.MovementNames, "Действие", 10, 5);
      sdt.GridProducer.Columns.AddDateRange("Date", "Date1", "Date2", "Дата", false, 15, 10);
      sdt.GridProducer.Columns.AddText("Place.Name", "Место", 20, 5);
      sdt.GridProducer.Columns.AddText("Contra.Name", "От кого / кому", 20, 5);
      sdt.GridProducer.Columns.AddText("ForkPlant.Name", "Отсажено / подсажено", 20, 10);
      sdt.GridProducer.Columns.AddText("Soil.Name", "Грунт", 20, 10);
      sdt.GridProducer.Columns.AddText("PotKind.Name", "Горшок", 20, 10);
      sdt.GridProducer.Columns.AddText("Comment", "Комментарий", 30, 10);

      sdt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "Комментарий (если задан)";

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

      #region Действия

      sdt = dt.SubDocTypes["PlantActions"];
      sdt.GridProducer.Columns.AddUserText("ActionText", "Kind,ActionName,Remedy", /* нельзя использовать Remedy.Name */
        new EFPGridProducerValueNeededEventHandler(EditPlantAction.ActionTextColumnValueNeeded),
        "Действие", 30, 10);
      sdt.GridProducer.Columns.AddDateRange("Date", "Date1", "Date2", "Дата", false, 15, 10);
      sdt.GridProducer.Columns.AddText("Soil.Name", "Грунт", 20, 10);
      sdt.GridProducer.Columns.AddText("PotKind.Name", "Горшок", 20, 10);
      sdt.GridProducer.Columns.AddText("Comment", "Комментарий", 30, 10);

      sdt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "Комментарий (если задан)";

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

      #region Цветение

      sdt = dt.SubDocTypes["PlantFlowering"];
      sdt.GridProducer.Columns.AddDateRange("Date", "Date1", "Date2", "Дата", false, 15, 10);
      sdt.GridProducer.Columns.AddInt("FlowerCount", "Количество цветков", 3);
      sdt.GridProducer.Columns.AddText("Comment", "Комментарий", 30, 10);

      sdt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "Комментарий (если задан)";

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

      #region Заболевания

      sdt = dt.SubDocTypes["PlantDiseases"];
      sdt.GridProducer.Columns.AddText("Disease.Name", "Заболевание", 30, 5);
      sdt.GridProducer.Columns.AddDateRange("Date", "Date1", "Date2", "Дата", false, 15, 10);
      sdt.GridProducer.Columns.AddText("Comment", "Комментарий", 30, 10);

      sdt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "Комментарий (если задан)";

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

      #region Планы

      sdt = dt.SubDocTypes["PlantPlans"];
      sdt.GridProducer.Columns.AddUserText("ActionText", "Kind,ActionName,Remedy",
        new EFPGridProducerValueNeededEventHandler(EditPlantPlan.ActionTextColumnValueNeeded),
        "Действие", 30, 10);
      sdt.GridProducer.Columns.AddDateRange("Date", "Date1", "Date2", "Дата", false, 15, 10);
      sdt.GridProducer.Columns.AddText("Comment", "Комментарий", 30, 10);

      sdt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "Комментарий (если задан)";

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
      // Планы явно делаются не на сегодняшний день
      //sdt.Columns["Date1"].DefaultValueNeeded += new EventHandler(DateColumn_DefaultValueNeeded);
      //sdt.Columns["Date2"].DefaultValueNeeded += new EventHandler(DateColumn_DefaultValueNeeded);
      sdt.Columns["Kind"].NewMode = ColumnNewMode.Saved;
      sdt.Columns["ActionName"].NewMode = ColumnNewMode.Saved;

      #endregion

      #endregion

      #region Места

      dt = base.DocTypes["Places"];

      dt.GridProducer.Columns.AddText("Name", "Название", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      dt.GridProducer.Columns.AddText("GroupId.Name", "Группа", 15, 5);
      dt.GridProducer.Columns.AddText("Comment", "Комментарий", 30, 10);

      dt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "Комментарий (если задан)";

      // dt.GridProducer.Orders.Add("Name", "Название");

      dt.GridProducer.NewDefaultConfig(false);
      dt.GridProducer.DefaultConfig.Columns.AddFill("Name", 100);
      dt.GridProducer.DefaultConfig.ToolTips.Add("Comment");

      dt.ImageKey = "Place";

      dt.InitEditForm += new InitDocEditFormEventHandler(EditPlace.InitDocEditForm);
      dt.CanInsertCopy = true;
      dt.CanMultiEdit = true; // можно менять группу у нескольких документов сразу
      dt.DataBuffering = true;
      dt.Columns["Name"].NewMode = ColumnNewMode.AlwaysDefaultValue;

      #endregion

      #region Контрагенты

      dt = base.DocTypes["Contras"];

      dt.GridProducer.Columns.AddText("Name", "Название", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      dt.GridProducer.Columns.AddText("GroupId.Name", "Группа", 15, 5);
      dt.GridProducer.Columns.AddText("Comment", "Комментарий", 30, 10);

      dt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "Комментарий (если задан)";

      dt.GridProducer.NewDefaultConfig(false);
      dt.GridProducer.DefaultConfig.Columns.AddFill("Name", 100);
      dt.GridProducer.DefaultConfig.ToolTips.Add("Comment");

      dt.ImageKey = "Contra";

      dt.InitEditForm += new InitDocEditFormEventHandler(EditContra.InitDocEditForm);
      dt.CanInsertCopy = true;
      dt.CanMultiEdit = true; // можно менять группу у нескольких документов сразу
      dt.DataBuffering = true;
      dt.Columns["Name"].NewMode = ColumnNewMode.AlwaysDefaultValue;

      #endregion

      #region Организации

      dt = base.DocTypes["Companies"];

      dt.GridProducer.Columns.AddText("Name", "Название", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      dt.GridProducer.Columns.AddText("GroupId.Name", "Группа", 15, 5);
      dt.GridProducer.Columns.AddText("Comment", "Комментарий", 30, 10);

      dt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "Комментарий (если задан)";

      dt.GridProducer.NewDefaultConfig(false);
      dt.GridProducer.DefaultConfig.Columns.AddFill("Name", 100);
      dt.GridProducer.DefaultConfig.ToolTips.Add("Comment");

      dt.ImageKey = "Company";

      dt.InitEditForm += new InitDocEditFormEventHandler(EditCompany.InitDocEditForm);
      dt.CanInsertCopy = true;
      dt.CanMultiEdit = true; // можно менять группу у нескольких документов сразу
      dt.DataBuffering = true;
      dt.Columns["Name"].NewMode = ColumnNewMode.AlwaysDefaultValue;

      #endregion

      #region Виды заболеваний

      dt = base.DocTypes["Diseases"];

      dt.GridProducer.Columns.AddText("Name", "Название", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      dt.GridProducer.Columns.AddText("GroupId.Name", "Группа", 15, 5);

      dt.GridProducer.Columns.AddText("Comment", "Комментарий", 30, 10);

      dt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "Комментарий (если задан)";

      dt.GridProducer.NewDefaultConfig(false);
      dt.GridProducer.DefaultConfig.Columns.AddFill("Name", 100);
      dt.GridProducer.DefaultConfig.ToolTips.Add("Comment");

      dt.ImageKey = "Disease";

      dt.InitEditForm += new InitDocEditFormEventHandler(EditDisease.InitDocEditForm);
      dt.CanInsertCopy = true;
      dt.CanMultiEdit = true; // можно менять группу у нескольких документов сразу
      dt.DataBuffering = true;
      dt.Columns["Name"].NewMode = ColumnNewMode.AlwaysDefaultValue;

      #endregion

      #region Препараты

      #region Основной документ

      dt = base.DocTypes["Remedies"];

      dt.GridProducer.Columns.AddText("Name", "Название", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      dt.GridProducer.Columns.AddText("Manufacturer.Name", "Изготовитель", 20, 5);

      dt.GridProducer.Columns.AddText("GroupId.Name", "Группа", 15, 5);
      dt.GridProducer.Columns.AddText("Comment", "Комментарий", 30, 10);

      dt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "Комментарий (если задан)";

      dt.GridProducer.NewDefaultConfig(false);
      dt.GridProducer.DefaultConfig.Columns.AddFill("Name", 100);
      dt.GridProducer.DefaultConfig.ToolTips.Add("Comment");

      dt.ImageKey = "Remedy";
      dt.InitView += new InitEFPDBxViewEventHandler(EditRemedy.InitView);

      dt.InitEditForm += new InitDocEditFormEventHandler(EditRemedy.InitDocEditForm);
      dt.CanInsertCopy = true;
      dt.CanMultiEdit = true; // можно менять группу у нескольких документов сразу
      dt.DataBuffering = true;
      dt.Columns["Name"].NewMode = ColumnNewMode.AlwaysDefaultValue;
      dt.Columns["Manufacturer"].NewMode = ColumnNewMode.Saved;

      #endregion

      #region Вариант применения препарата

      sdt = dt.SubDocTypes["RemedyUsage"];
      sdt.GridProducer.Columns.AddText("Name", "Название", 40, 15);
      sdt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      sdt.GridProducer.DefaultConfig = new EFPDataGridViewConfig();
      sdt.GridProducer.DefaultConfig.Columns.AddFill("Name");

      sdt.ImageKey = "Item";
      sdt.InitEditForm += new InitSubDocEditFormEventHandler(EditRemedyUsage.InitEditForm);
      sdt.CanInsertCopy = false;
      sdt.CanMultiEdit = false;

      #endregion

      #endregion

      #region Грунты

      #region Основной документ

      dt = base.DocTypes["Soils"];

      dt.GridProducer.Columns.AddText("Name", "Название", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      dt.GridProducer.Columns.AddText("Manufacturer.Name", "Изготовитель", 20, 5);

      dt.GridProducer.Columns.AddInt("PartCount", "Количество компонентов", 2);
      dt.GridProducer.Columns.AddText("Contents", "Состав", 50, 10);
      dt.GridProducer.Columns.AddUserText("pHtext", "pHmin,pHmax",
        new EFPGridProducerValueNeededEventHandler(EditSoil.pHcolumnValueNeeded),
        "pH", 7, 7);
      dt.GridProducer.Columns.LastAdded.TextAlign = HorizontalAlignment.Center;

      dt.GridProducer.Columns.AddText("GroupId.Name", "Группа", 15, 5);
      dt.GridProducer.Columns.AddText("Comment", "Комментарий", 30, 10);

      dt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "Комментарий (если задан)";

      dt.GridProducer.NewDefaultConfig(false);
      dt.GridProducer.DefaultConfig.Columns.AddFill("Name", 100);
      dt.GridProducer.DefaultConfig.Columns.Add("PartCount");
      dt.GridProducer.DefaultConfig.ToolTips.Add("Comment");

      dt.ImageKey = "Soil";
      dt.InitView += new InitEFPDBxViewEventHandler(EditSoil.InitView);

      dt.InitEditForm += new InitDocEditFormEventHandler(EditSoil.InitDocEditForm);
      dt.CanInsertCopy = true;
      dt.CanMultiEdit = true; // можно менять группу у нескольких документов сразу
      dt.DataBuffering = true;
      dt.Columns["Name"].NewMode = ColumnNewMode.AlwaysDefaultValue;
      dt.Columns["Manufacturer"].NewMode = ColumnNewMode.Saved;

      #endregion

      #region Поддокумент "Компонент грунта"

      sdt = dt.SubDocTypes["SoilParts"];
      sdt.GridProducer.Columns.AddText("Soil.Name", "Компонент", 20, 10);
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

      #region Горшки

      dt = base.DocTypes["PotKinds"];

      dt.GridProducer.Columns.AddText("Name", "Описание", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      dt.GridProducer.Columns.AddText("Text", "Текстовая часть", 20, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      dt.GridProducer.Columns.AddInt("Diameter", "d, мм", 3);
      dt.GridProducer.Columns.LastAdded.SizeGroup = "SizeMM";
      dt.GridProducer.Columns.AddInt("Height", "H, мм", 3);
      dt.GridProducer.Columns.LastAdded.SizeGroup = "SizeMM";

      dt.GridProducer.Columns.AddFixedPoint("Volume", "Объем, л", 5, 2, "VolumeL");
      dt.GridProducer.Columns.AddText("Color", "Цвет", 10, 5);

      dt.GridProducer.Columns.AddText("Manufacturer.Name", "Изготовитель", 20, 5);

      dt.GridProducer.Columns.AddText("GroupId.Name", "Группа", 15, 5);
      dt.GridProducer.Columns.AddText("Comment", "Комментарий", 30, 10);

      dt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "Комментарий (если задан)";

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

      #region Виды атрибутов

      dt = base.DocTypes["AttrTypes"];
      dt.ImageKey = "AttributeType";
      dt.GridProducer.Columns.AddText("Name", "Имя атрибута", 20, 5);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      dt.GridProducer.Columns.AddUserText("TypeText", "Type",
        new EFPGridProducerValueNeededEventHandler(EditAttrType.TypeColumnValueNeeded),
        "Тип", 15, 2);

      dt.GridProducer.Columns.AddText("Comment", "Комментарий", 30, 10);

      dt.GridProducer.ToolTips.AddText("Comment", "").DisplayName = "Комментарий (если задан)";

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

      #region Уход

      #region Основной документ

      dt = base.DocTypes["Care"];

      dt.GridProducer.Columns.AddText("Name", "Название", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      dt.GridProducer.Columns.AddText("Parent.Name", "Родитель", 40, 15);
      dt.GridProducer.Columns.LastAdded.CanIncSearch = true;

      dt.GridProducer.Columns.AddText("GroupId.Name", "Группа", 15, 5);
      dt.GridProducer.Columns.AddText("Comment", "Комментарий", 30, 10);

      dt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "Комментарий (если задан)";

      dt.GridProducer.Orders.Add("Name", "Название");


      dt.GridProducer.NewDefaultConfig(false);
      dt.GridProducer.DefaultConfig.Columns.AddFill("Name", 100);
      dt.GridProducer.DefaultConfig.ToolTips.Add("Comment");

      dt.ImageKey = "Care";
      //dt.InitView += new InitEFPDBxViewEventHandler(EditCare.InitView);

      dt.InitEditForm += new InitDocEditFormEventHandler(EditCare.InitDocEditForm);
      dt.CanInsertCopy = true;
      dt.CanMultiEdit = true; // только вкладка "Общие"
      dt.DataBuffering = false;

      //dt.Writing += EditOrg.Writing;

      #endregion

      #region Запись об уходе

      sdt = dt.SubDocTypes["CareRecords"];
      sdt.GridProducer.Columns.AddMonthDayRange("PeriodText", "Day1", "Day2",
        "Период", false).EmptyValue = "Год";
      sdt.GridProducer.Columns.LastAdded.TextAlign = HorizontalAlignment.Center;
      sdt.GridProducer.Columns.AddText("Name", "Название периода", 20, 10);
      //sdt.GridProducer.Columns.AddText("Comment", "Комментарий", 30, 10);

      //sdt.GridProducer.ToolTips.AddText("Comment", String.Empty).DisplayName = "Комментарий (если задан)";

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

    #region Диалог выбора значения атрибута из списка

    /// <summary>
    /// Диалог выбора значения атрибута
    /// </summary>
    /// <param name="Value">Вход-выход: выбираемое значение</param>
    /// <param name="Title">Заголовок блока диалога</param>
    /// <param name="AttrType">Документ "Вид атрибута"</param>
    /// <returns>true, если значение было выбрано</returns>
    public static bool SelectAttrValue(ref object Value, string Title, AttrTypeDoc AttrType)
    {
      switch (AttrType.SourceType)
      {
        case AttrValueSourceType.List:
          return SelectAttrValueList(ref Value, Title, AttrType);
        default:
          if (AttrType.ValueType == ValueType.Boolean)
            return SelectAttrValueBoolean(ref Value, Title, AttrType);
          EFPApp.ShowTempMessage("Для атрибута \"" + AttrType.Name + "\" предусмотрен только ручной ввод значений");
          return false;
      }
    }

    private static bool SelectAttrValueList(ref object Value, string Title, AttrTypeDoc AttrType)
    {
      if (AttrType.ValueList.Length == 0)
      {
        EFPApp.ShowTempMessage("У вида атрибута \"" + AttrType.Name + "\" нет списка значений для выбора");
        return false;
      }

      ListSelectDialog dlg = new ListSelectDialog();
      dlg.Title = Title;
      dlg.ListTitle = "Значение атрибута \"" + AttrType.Name + "\"";
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
      //dlg.ImageKey = "Атрибут";
      dlg.GroupTitle = "Значение атрибута \"" + AttrType.Name + "\"";
      dlg.Items = new string[] { "&1 - Да", "&0 - Нет" };
      dlg.SelectedIndex = DataTools.GetBool(Value) ? 0 : 1;
      if (dlg.ShowDialog() != DialogResult.OK)
        return false;

      Value = (dlg.SelectedIndex == 0);
      return true;
    }

    #endregion

    #region Прочие методы

    /// <summary>
    /// Добавить в GridProducer обычный столбец, связанный с полем данных, для указанного типа данных
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
          throw new ArgumentException("Неизвестный тип данных: " + ValueType.ToString() + " для столбца \"" + ColumnName + "\"", "ValueType");
      }
    }



    /// <summary>
    /// Возвращает горизонтальное выравнивание по умолчанию для значения заданного типа
    /// </summary>
    /// <param name="ValueType">Тип значения</param>
    /// <returns>Выравнивание</returns>
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

    #region Статические свойства

    public static ProgramDBUI TheUI;

    public static ClientConfigSections ConfigSections;

    //public static CoProDevDocuments Docs;

    //public static ProgramCache Cache;

    /// <summary>
    /// Настройки
    /// </summary>
    public static UserSettings Settings;

    #endregion
  }
}
