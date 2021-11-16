using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FreeLibSet.Forms.Docs;
using FreeLibSet.Forms;
using FreeLibSet.IO;
using FreeLibSet.Data.Docs;
using System.IO;
using System.Drawing.Imaging;
using FreeLibSet.Caching;
using System.Diagnostics;
using FreeLibSet.Logging;
using FreeLibSet.UICore;
using FreeLibSet.Core;
using FreeLibSet.Drawing;

namespace Plants
{
  public partial class EditPlant : Form
  {
    #region Конструктор формы

    public EditPlant()
    {
      InitializeComponent();
    }

    #endregion

    #region Табличный просмотр документов

    #region Вычисляемые столбцы

    public static void ImageValueNeeded(object Sender, DBxImageValueNeededEventArgs Args)
    {
      PlantMovementState State = (PlantMovementState)(Args.GetInt("MovementState"));
      Args.ImageKey = PlantTools.GetPlantMovementStateImageKey(State);
      if (State != PlantMovementState.Placed)
        Args.Grayed = true;
      switch (State)
      {
        case PlantMovementState.Draft:
          Args.ColorType = EFPDataGridViewColorType.Special;
          break;
      }
    }

    public static void ContraNameColumnValueNeeded(object sender, EFPGridProducerValueNeededEventArgs args)
    {
      Int32 Id;

      Id = args.GetInt("ToContra");
      if (Id != 0)
      {
        args.Value = ProgramDBUI.TheUI.DocTypes["Contras"].GetTextValue(Id);
        return;
      }

      Id = args.GetInt("ToPlant");
      if (Id != 0)
      {
        args.Value = ProgramDBUI.TheUI.DocTypes["Plants"].GetTextValue(Id);
        return;
      }

      Id = args.GetInt("FromContra");
      if (Id != 0)
      {
        args.Value = ProgramDBUI.TheUI.DocTypes["Contras"].GetTextValue(Id);
        return;
      }

      Id = args.GetInt("FromPlant");
      if (Id != 0)
      {
        args.Value = ProgramDBUI.TheUI.DocTypes["Plants"].GetTextValue(Id);
        return;
      }
    }

    public static void FirstPlannedActionTextColumnValueNeeded(object Sender, EFPGridProducerValueNeededEventArgs Args)
    {
      ActionKind Kind = (ActionKind)(Args.GetInt("FirstPlannedAction.Kind"));
      if (Kind == ActionKind.Other)
        Args.Value = Args.GetString("FirstPlannedAction.ActionName");
      else
        Args.Value = PlantTools.GetActionName(Kind);
    }

    public static void FirstPlannedActionImageColumnValueNeeded(object Sender, EFPGridProducerValueNeededEventArgs Args)
    {
      // TODO: Хорошо бы еще и раскрашивать просроченные действия

      DateTime? Date1 = Args.GetNullableDateTime("FirstPlannedAction.Date1");
      DateTime? Date2 = Args.GetNullableDateTime("FirstPlannedAction.Date2");
      ActionKind Kind = (ActionKind)(Args.GetInt("FirstPlannedAction.Kind"));

      if (Date1.HasValue && Date2.HasValue)
      {
        if (Date2.Value < DateTime.Today)
          Args.Value = EFPApp.MainImages.Images["Warning"];
        else
          Args.Value = EFPApp.MainImages.Images[PlantTools.GetActionImageKey(Kind)];
      }
      else if (Date1.HasValue || Date2.HasValue)
        Args.Value = EFPApp.MainImages.Images["Error"]; // ошибка - не может такого быть
      else
        Args.Value = EFPApp.MainImages.Images["EmptyImage"];
    }

    public static void StateText_Column_ToolTipText_ValueNeeded(object sender, EFPGridProducerValueNeededEventArgs args)
    {
      PlantMovementState state = args.GetEnum<PlantMovementState>("MovementState");
      string s = PlantTools.GetPlantMovementStateName(state);
      switch (state)
      {
        case PlantMovementState.Placed:
          Int32 placeId = args.GetInt("Place");
          s += " - " + ProgramDBUI.TheUI.DocTypes["Places"].GetTextValue(placeId);
          break;
        case PlantMovementState.Given:
          Int32 contraId = args.GetInt("ToContra");
          s += " - " + ProgramDBUI.TheUI.DocTypes["Contras"].GetTextValue(contraId);
          break;
      }

      args.Value = s;
    }

    #endregion

    /// <summary>
    /// Дополнительная инициализация табличного просмотра справочника документов для
    /// добавления команды локального меню
    /// </summary>
    /// <param name="Sender"></param>
    /// <param name="Args"></param>
    public static void InitView(object Sender, InitEFPDBxViewEventArgs Args)
    {
      EFPDataGridView ControlProvider = (EFPDataGridView)(Args.ControlProvider);
      //((EFPDataGridView)(Args.ControlProvider)).Control.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
      ControlProvider.Control.RowHeightInfoNeeded += PlantThumbnailColumn.RowHeightInfoNeeded;

      ControlProvider.Idle += new EventHandler(ControlProvider_Idle);

      #region Фильтры

      NullNotNullGridFilter FiltHasNumber = new NullNotNullGridFilter("Number", typeof(int));
      FiltHasNumber.DisplayName = "Номер по каталогу";
      FiltHasNumber.FilterTextNull = "Нет";
      FiltHasNumber.FilterTextNotNull = "Есть";
      Args.ControlProvider.Filters.Add(FiltHasNumber);

      NullNotNullGridFilter FiltHasPhoto = new NullNotNullGridFilter("Photo", typeof(Int32));
      FiltHasPhoto.DisplayName = "Фото";
      FiltHasPhoto.FilterTextNull = "Нет";
      FiltHasPhoto.FilterTextNotNull = "Есть";
      Args.ControlProvider.Filters.Add(FiltHasPhoto);

      EnumGridFilter FiltState = new EnumGridFilter("MovementState", PlantTools.PlantMovementStateNames);
      FiltState.DisplayName = "Состояние";
      FiltState.ImageKeys = PlantTools.PlantMovementStateImageKeys;
      Args.ControlProvider.Filters.Add(FiltState);

      RefDocGridFilterSet FiltPlace = new RefDocGridFilterSet(ProgramDBUI.TheUI.DocTypes["Places"], "Place");
      FiltPlace.DisplayName = "Текущее место расположения";
      FiltPlace.Nullable = true;
      Args.ControlProvider.Filters.Add(FiltPlace);

      EnumGridFilter FiltLastAction = new EnumGridFilter("LastActionKind", PlantTools.ActionNames);
      FiltLastAction.DisplayName = "Последнее действие";
      FiltLastAction.ImageKeys = PlantTools.ActionImageKeys;
      Args.ControlProvider.Filters.Add(FiltLastAction);

      RefDocGridFilterSet FiltFromContra = new RefDocGridFilterSet(ProgramDBUI.TheUI.DocTypes["Contras"], "FromContra");
      FiltFromContra.DisplayName = "От кого получено";
      FiltFromContra.Nullable = true;
      Args.ControlProvider.Filters.Add(FiltFromContra);

      RefDocGridFilterSet FiltToContra = new RefDocGridFilterSet(ProgramDBUI.TheUI.DocTypes["Contras"], "ToContra");
      FiltToContra.DisplayName = "Кому передано";
      FiltToContra.Nullable = true;
      Args.ControlProvider.Filters.Add(FiltToContra);

      RefDocGridFilterSet FiltSoil = new RefDocGridFilterSet(ProgramDBUI.TheUI.DocTypes["Soils"], "Soil");
      FiltSoil.DisplayName = "Грунт";
      FiltSoil.Nullable = true;
      Args.ControlProvider.Filters.Add(FiltSoil);

      RefDocGridFilterSet FiltPotKind = new RefDocGridFilterSet(ProgramDBUI.TheUI.DocTypes["PotKinds"], "PotKind");
      FiltPotKind.DisplayName = "Горшок";
      FiltPotKind.Nullable = true;
      Args.ControlProvider.Filters.Add(FiltPotKind);

      RefDocGridFilter FiltManufacturer = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["Companies"], "Manufacturer");
      FiltManufacturer.DisplayName = "Изготовитель";
      FiltManufacturer.Nullable = true;
      Args.ControlProvider.Filters.Add(FiltManufacturer);

      RefDocGridFilter FiltCare = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["Care"], "Care");
      FiltCare.DisplayName = "Уход";
      FiltCare.Nullable = true;
      Args.ControlProvider.Filters.Add(FiltCare);

      #endregion

      #region Локальное меню

      EFPCommandItem ci;
      ci = new EFPCommandItem("Edit", "AttrTable");
      ci.MenuText = "Таблица атрибутов";
      ci.ImageKey = "AttributeTable";
      ci.GroupBegin = true;
      ci.GroupEnd = true;
      ci.Tag = Args.ControlProvider;
      ci.Click += new EventHandler(ciRB_AttrTable_Click);
      Args.ControlProvider.CommandItems.Add(ci);

      EFPFileAssociationsCommandItemsHandler ViewHandler = new EFPFileAssociationsCommandItemsHandler(Args.ControlProvider.CommandItems, ".jpg");
      ViewHandler.FileNeeded += new CancelEventHandler(Doc_ViewHandler_FileNeeded);
      ViewHandler.Tag = Args.ControlProvider;

      #endregion
    }

    static void ControlProvider_Idle(object Sender, EventArgs Args)
    {
      try
      {
        EFPDataGridView Control = (EFPDataGridView)Sender;

        EFPDataGridViewColumn Col = Control.Columns["Thumbnail"];
        if (Col != null)
        {
          int w = PlantThumbnailColumn.DoGetWidth();
          Col.GridColumn.MinimumWidth = w;
          Col.GridColumn.Width = w;
        }
      }
      catch (Exception e)
      {
        LogoutTools.LogoutException(e, "Ошибка Plants EFPDataGridView.Idle");
      }
    }


    #region Команды локального меню

    static void ciRB_AttrTable_Click(object Sender, EventArgs Args)
    {
      EFPCommandItem ci = (EFPCommandItem)Sender;
      IEFPDocView ControlProvider = (IEFPDocView)(ci.Tag);
      Int32[] DocIds = ControlProvider.SelectedIds;

      AttrTableViewForm Form = new AttrTableViewForm(DocIds);
      EFPApp.ShowFormOrDialog(Form);

    }

    static void Doc_ViewHandler_FileNeeded(object Sender, CancelEventArgs Args)
    {
      EFPFileAssociationsCommandItemsHandler ViewHandler = (EFPFileAssociationsCommandItemsHandler)Sender;
      EFPDocGridView ControlProvider = (EFPDocGridView)(ViewHandler.Tag);

      ViewHandler.FilePath = AbsPath.Empty;

      if (ProgramDBUI.Settings.PhotoDir.IsEmpty)
      {
        EFPApp.ErrorMessageBox("Каталог со снимками не указан");
        Args.Cancel = true;
        return;
      }

      if (!ControlProvider.CheckSingleRow())
      {
        Args.Cancel = true;
        return;
      }


      string FileName = ControlProvider.DocTypeUI.TableCache.GetString(ControlProvider.CurrentId, "Photo.FileName");
      if (String.IsNullOrEmpty(FileName))
      {
        EFPApp.ErrorMessageBox("Для выбранного растения не задано изображение");
        Args.Cancel = true;
        return;
      }

      AbsPath Path = new AbsPath(ProgramDBUI.Settings.PhotoDir, FileName);
      if (!EFPApp.FileExists(Path))
      {
        EFPApp.ErrorMessageBox("Файл не найден: " + Path.ToString());
        Args.Cancel = true;
        return;
      }
      ViewHandler.FilePath = Path;
    }


    #endregion

    #endregion

    #region Редактор документа

    public static void BeforeEditDoc(object Sender, BeforeDocEditEventArgs Args)
    {
      switch (Args.Editor.State)
      {
        case EFPDataGridViewState.Edit:
        case EFPDataGridViewState.View:
          if (Args.CurrentColumnName == "Thumbnail" && (!Args.Editor.MultiDocMode))
          {
            Args.Cancel = true;
            Int32 SubDocId = Args.Editor.MainValues["Photo"].AsInteger;
            if (SubDocId == 0)
            {
              EFPApp.ShowTempMessage("Нет фото растения");
              return;
            }
            DBxSubDoc sd = Args.Editor.Documents[0][0].SubDocs["PlantPhotos"].GetSubDocById(SubDocId);
            string FileName = sd.Values["FileName"].AsString;
            ViewFile(FileName);
            return;
          }
          break;
      }
    }

    public static void InitDocEditForm(object Sender, InitDocEditFormEventArgs Args)
    {
      EditPlant Form = new EditPlant();
      Form.AddPage1(Args);
      if (!Args.Editor.MultiDocMode)
      {
        Form.AddPage2(Args);
        Args.AddSubDocsPage("PlantAttributes");
        Args.AddSubDocsPage("PlantMovement").Title = "Движение";
        Args.AddSubDocsPage("PlantActions").Title = "Действия";
        Args.AddSubDocsPage("PlantFlowering").Title = "Цветение";
        Args.AddSubDocsPage("PlantDiseases").Title = "Заболевания";
        Args.AddSubDocsPage("PlantPlans").Title = "План";
      }
    }

    #region Общие

    EFPTextBox efpLocalName, efpLatinName, efpDescrName;

    EFPIntEditBox efpNumber;

    private void AddPage1(InitDocEditFormEventArgs Args)
    {
      DocEditPage Page = Args.AddPage("Общие", MainPanel1);
      Page.ImageKey = "Properties";

      efpLocalName = new EFPTextBox(Page.BaseProvider, edLocalName);
      efpLocalName.CanBeEmpty = true;
      Args.AddText(efpLocalName, "LocalName", true);

      efpLatinName = new EFPTextBox(Page.BaseProvider, edLatinName);
      efpLatinName.CanBeEmpty = true;
      Args.AddText(efpLatinName, "LatinName", true);

      efpDescrName = new EFPTextBox(Page.BaseProvider, edDescrName);
      efpDescrName.CanBeEmpty = true;
      Args.AddText(efpDescrName, "Description", true);

      if (!Args.Editor.MultiDocMode)
      {
        efpLocalName.Validating += new UIValidatingEventHandler(efpAnyName_Validating);
        efpLocalName.TextEx.ValueChanged += new EventHandler(efpAnyName_ValueChanged);
        efpLatinName.Validating += new UIValidatingEventHandler(efpAnyName_Validating);
        efpLatinName.TextEx.ValueChanged += new EventHandler(efpAnyName_ValueChanged);
        efpDescrName.Validating += new UIValidatingEventHandler(efpAnyName_Validating);
        efpDescrName.TextEx.ValueChanged += new EventHandler(efpAnyName_ValueChanged);
      }

      EFPDocComboBox efpManufacturer = new EFPDocComboBox(Page.BaseProvider, cbManufacturer, ProgramDBUI.TheUI.DocTypes["Companies"]);
      efpManufacturer.CanBeEmpty = true;
      Args.AddRef(efpManufacturer, "Manufacturer", true);

      EFPDocComboBox efpCare = new EFPDocComboBox(Page.BaseProvider, cbCare, ProgramDBUI.TheUI.DocTypes["Care"]);
      efpCare.CanBeEmpty = true;
      Args.AddRef(efpCare, "Care", true);

      efpNumber = new EFPIntEditBox(Page.BaseProvider, edNumber);
      efpNumber.Validating += new UIValidatingEventHandler(efpNumber_Validating);
      efpNumber.Minimum = 0;
      efpNumber.Maximum = Int16.MaxValue;
      Args.AddInt(efpNumber, "Number", false);

      EFPDocComboBox efpGroup = new EFPDocComboBox(Page.BaseProvider, cbGroup, ProgramDBUI.TheUI.DocTypes["PlantGroups"]);
      efpGroup.CanBeEmpty = true;
      Args.AddRef(efpGroup, "GroupId", true);

      EFPTextBox efpComment = new EFPTextBox(Page.BaseProvider, edComment);
      efpComment.CanBeEmpty = true;
      Args.AddText(efpComment, "Comment", true);
    }

    void efpAnyName_ValueChanged(object Sender, EventArgs Args)
    {
      efpLocalName.Validate();
      efpLatinName.Validate();
      efpDescrName.Validate();
    }

    void efpAnyName_Validating(object Sender, UIValidatingEventArgs Args)
    {
      if (Args.ValidateState == UIValidateState.Error)
        return;
      if (efpLocalName.Text.Length + efpLatinName.Text.Length + efpDescrName.Text.Length == 0)
        Args.SetError("Какое-либо из названий должно быт заполнено");
    }

    void efpNumber_Validating(object Sender, UIValidatingEventArgs Args)
    {
      if (Args.ValidateState != UIValidateState.Ok)
        return;
      if (efpNumber.Value == 0)
        Args.SetWarning("Номер по каталогу не задан");
    }

    #endregion

    #region Фото

    /// <summary>
    /// Идентификатор поддокумента фото, которое будет использоваться в каталог
    /// </summary>
    private Int32 MainPhotoSubDocId;

    private void AddPage2(InitDocEditFormEventArgs Args)
    {
      Args.Editor.AfterReadValues += new DocEditEventHandler(Editor_AfterReadValues);
      Args.Editor.BeforeWrite += new DocEditCancelEventHandler(Editor_BeforeWrite);
      Args.Editor.AfterWrite += new DocEditEventHandler(Editor_AfterWrite);

      DocEditPage Page = Args.AddPage("Фото", MainPanel2);
      Page.ImageKey = "Picture";

      EFPControlWithToolBar<DataGridView> cwt = new EFPControlWithToolBar<DataGridView>(Page.BaseProvider, MainPanel2);
      EFPSubDocGridView sdgPhotos = new EFPSubDocGridView(cwt, Args.Editor, Args.MultiDocs.SubDocs["PlantPhotos"]);
      sdgPhotos.GridProducerPostInit += new EventHandler(sdgPhotos_GridProducerPostInit);

      if (ProgramDBUI.Settings.PhotoDir.IsEmpty)
        sdgPhotos.CanView = false; // нельзя просматривать снимки

      EFPCommandItem ci;
      ci = new EFPCommandItem("Edit", "SelectDefault");
      ci.MenuText = "Выбрать изображение для каталога";
      ci.ImageKey = "Ok";
      ci.ShortCut = Keys.F4;
      ci.Click += new EventHandler(ciSelectDefault_Click);
      ci.Enabled = !Args.Editor.IsReadOnly;
      ci.Tag = sdgPhotos;
      ci.GroupBegin = true;
      ci.GroupEnd = true;
      sdgPhotos.CommandItems.Add(ci);
    }

    void sdgPhotos_GridProducerPostInit(object Sender, EventArgs Args)
    {
      EFPSubDocGridView sdgPhotos = (EFPSubDocGridView)Sender;
      //sdgPhotos.GetRowAttributes += new EFPDataGridViewRowAttributesEventHandler(sdgPhotos_GetRowAttributes);
      sdgPhotos.GetCellAttributes += new EFPDataGridViewCellAttributesEventHandler(sdgPhotos_GetCellAttributes);

    }

    void Editor_AfterReadValues(object Sender, DocEditEventArgs Args)
    {
      MainPhotoSubDocId = Args.Editor.Documents[0].Values["Photo"].AsInteger;
    }

    void Editor_BeforeWrite(object Sender, DocEditCancelEventArgs Args)
    {
      DataTable Table = Args.Editor.Documents[0].SubDocs["PlantPhotos"].CreateSubDocsData();
      if (Table.Rows.Count == 0)
        Args.Editor.Documents[0].Values["Photo"].SetNull();
      else
      {
        Table.DefaultView.Sort = "ShootingTime";
        foreach (DataRowView drv in Table.DefaultView)
        {
          if (DataTools.GetInt(drv.Row, "Id") == MainPhotoSubDocId)
          {
            Args.Editor.Documents[0].Values["Photo"].SetInteger(MainPhotoSubDocId);
            return;
          }
        }
        // Берем первое изображение
        MainPhotoSubDocId = DataTools.GetInt(Table.DefaultView[0].Row, "Id");
        Args.Editor.Documents[0].Values["Photo"].SetInteger(MainPhotoSubDocId);
      }
    }

    void Editor_AfterWrite(object Sender, DocEditEventArgs Args)
    {
      // Нужно после нажатия кнопки "Запись", если основным было сделано новое фото
      MainPhotoSubDocId = Args.Editor.Documents[0].Values["Photo"].AsInteger;
    }

    void ciSelectDefault_Click(object Sender, EventArgs Args)
    {
      EFPCommandItem ci = (EFPCommandItem)Sender;
      EFPSubDocGridView sdgPhotos = (EFPSubDocGridView)(ci.Tag);
      if (!sdgPhotos.CheckSingleRow())
        return;

      MainPhotoSubDocId = sdgPhotos.CurrentId;
      sdgPhotos.Control.InvalidateColumn(0); // требуется перерисовка значков всех строк
      sdgPhotos.MainEditor.SubDocsChangeInfo.Changed = true;
    }

    //void sdgPhotos_GetRowAttributes(object Sender, EFPDataGridViewRowAttributesEventArgs Args)
    //{
    //  if (Args.DataRow == null)
    //    return;
    //  Int32 Id = DataTools.GetInt(Args.DataRow, "Id");
    //  if (Id == MainPhotoSubDocId)
    //    Args.ColorType = EFPDataGridViewColorType.Total1; 
    //}

    // Не работает, т.к. после этого вызывается обработчик SubDocTypeUI.ControlProvider_GetCellAttributes
    void sdgPhotos_GetCellAttributes(object Sender, EFPDataGridViewCellAttributesEventArgs Args)
    {
      if (Args.DataRow == null)
        return;
      if (Args.ColumnIndex == 0) // значок
      {
        Int32 Id = DataTools.GetInt(Args.DataRow, "Id");
        if (Id == MainPhotoSubDocId)
          Args.Value = EFPApp.MainImages.Images["Ok"];
      }
    }

    #endregion

    #endregion

    #region Табличный просмотр поддокументов "Фото"

    public static void SubDocPhoto_InitView(object Sender, InitEFPDBxViewEventArgs Args)
    {
      if (ProgramDBUI.Settings.PhotoDir.IsEmpty)
        Args.ControlProvider.ReadOnly = true;
      // ((EFPDataGridView)(Args.ControlProvider)).Control.RowHeightInfoNeeded += new DataGridViewRowHeightInfoNeededEventHandler(Control_RowHeightInfoNeeded);
      ((EFPDataGridView)(Args.ControlProvider)).Control.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

      EFPFileAssociationsCommandItemsHandler ViewHandler = new EFPFileAssociationsCommandItemsHandler(Args.ControlProvider.CommandItems, ".jpg");
      ViewHandler.FileNeeded += new CancelEventHandler(SubDocPhoto_ViewHandler_FileNeeded);
      ViewHandler.Tag = Args.ControlProvider;
    }

    //static void Control_RowHeightInfoNeeded(object Sender, DataGridViewRowHeightInfoNeededEventArgs Args)
    //{
    //  Args.Height = Math.Max(Args.Height, ProgramDBUI.Setting.ThumbnailSize.Height + 4);
    //}

    #endregion

    #region Редактор документа "Фото"

    public static void SubDocPhoto_BeforeEdit(object Sender, BeforeSubDocEditEventArgs Args)
    {
      Args.ShowEditor = false;
      switch (Args.Editor.State)
      {
        case EFPDataGridViewState.Insert:
          try
          {
            Args.Cancel = true;
            if (InsertFiles(Args.MainEditor.Documents[0][0].SubDocs["PlantPhotos"]))
              Args.MainEditor.SubDocsChangeInfo.Changed = true;
          }
          catch (Exception e)
          {
            EFPApp.ShowException(e, "Ошибка добавления фото");
            Args.Cancel = true;
          }
          break;
        case EFPDataGridViewState.Edit:
          // TODO: Определять, что текущий столбец - "Comment", иначе показывать изображение
          SubDocPhoto_EditComment(Args);
          break;

        case EFPDataGridViewState.View:
          DBxSubDoc SubDoc = Args.Editor.SubDocs[0];
          ViewFile(SubDoc.Values["FileName"].AsString);
          Args.Cancel = true;
          break;
      }
    }

    private static void SubDocPhoto_EditComment(BeforeSubDocEditEventArgs Args)
    {
      DBxSubDoc SubDoc = Args.Editor.SubDocs[0];

      MultiLineTextInputDialog dlg = new MultiLineTextInputDialog();
      dlg.Title = "Комментарий";
      dlg.Prompt = "Комментарий к снимку " + SubDoc.Values["FileName"].AsString;
      dlg.Text = SubDoc.Values["Comment"].AsString;
      dlg.CanBeEmpty = true;
      if (dlg.ShowDialog() != DialogResult.OK)
        return;

      SubDoc.Values["Comment"].SetString(dlg.Text);

      Args.MainEditor.SubDocsChangeInfo.Changed = true;
    }

    private static bool InsertFiles(DBxSingleSubDocs SubDocs)
    {
      OpenFileDialog dlg = new OpenFileDialog();
      dlg.InitialDirectory = ProgramDBUI.Settings.PhotoDir.Path;
      dlg.Filter = "Фотографии JPEG|*.jpg;*.jpeg";
      dlg.Multiselect = true; // 22.09.2019
      if (dlg.ShowDialog() != DialogResult.OK)
        return false;
      for (int i = 0; i < dlg.FileNames.Length; i++)
      {
        AbsPath Path = new AbsPath(dlg.FileNames[i]);
        if (Path.ParentDir != ProgramDBUI.Settings.PhotoDir)
        {
          EFPApp.ErrorMessageBox("Файл должен располагаться в каталоге \"" + ProgramDBUI.Settings.PhotoDir.Path + "\"");
          return false;
        }

        DBxSubDoc SubDoc = SubDocs.Insert();

        SubDoc.Values["FileName"].SetString(Path.FileName);
        using (FileStream fs = new FileStream(Path.Path, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          string md5 = FileTools.MD5Sum(fs);
          SubDoc.Values["FileMD5"].SetString(md5);
          fs.Position = 0;
          using (Image img = Image.FromStream(fs))
          {
            using (Bitmap Thumbnail = WinFormsTools.CreateThumbnailImage(img, PlantTools.ThumbnailSize))
            {
              // 22.09.2019
              RotateThumbnail(Thumbnail, img);

              using (MemoryStream ms = new MemoryStream())
              {
                Thumbnail.Save(ms, ImageFormat.Bmp);
                SubDoc.Values["ThumbnailData"].SetBinData(ms.GetBuffer());
              }
            }

            // См. документацию GDI+
            // https://docs.microsoft.com/ru-ru/windows/desktop/gdiplus/-gdiplus-constant-property-item-descriptions
            const int PropertyTagDateTime = 0x0132;

            PropertyItem pi = GetPropertyItem(img, PropertyTagDateTime);
            if (pi != null)
            {
              string s = Encoding.ASCII.GetString(pi.Value);
              //                                 01234567890123456789
              // 20-символьная строка в формате "ГГГГ:ММ:ДД ЧЧ:ММ:СС"+'\0'
              // Разделители - двоеточие
              if (s.Length == 20)
              {
                s = s.Substring(0, 4) + s.Substring(5, 2) + s.Substring(8, 2) +
                  s.Substring(11, 2) + s.Substring(14, 2) + s.Substring(17, 2);
                DateTime dt;
                if (DateTime.TryParseExact(s, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture,
                  System.Globalization.DateTimeStyles.None, out dt))

                  SubDoc.Values["ShootingTime"].SetNullableDateTime(dt);
              }
            }
          }
        }
      }

      return true;
    }

    /// <summary>
    /// Получает значение свойства из jpeg-файла
    /// Исключения перехватываются и возвращается null
    /// </summary>
    /// <param name="image">Загруженный файл</param>
    /// <param name="propId">Идентификатор свойства</param>
    /// <returns>PropertyItem</returns>
    [DebuggerStepThrough]
    public static PropertyItem GetPropertyItem(Image image, int propId)
    {
      try
      {
        return image.GetPropertyItem(propId);
      }
      catch
      {
        return null;
      }
    }

    /// <summary>
    /// Вращение изображения.
    /// Можно было бы вращать исходное изображение, а потом сделать Thumbnail, но это будет медленнее,
    /// так как исходное изображение может быть большим.
    /// Выгоднее вертеть миниатюру.
    /// Нельзя хранить неповернутые миниатюры, так как у них не сохраняются атрибуты исходного файла (EXIF).
    /// </summary>
    /// <param name="Thumbnail">Миниатюра</param>
    /// <param name="img">Исходное изображение для извлечения свойств</param>
    public static void RotateThumbnail(Bitmap Thumbnail, Image img)
    {
      // См. документацию GDI+
      // https://docs.microsoft.com/ru-ru/windows/desktop/gdiplus/-gdiplus-constant-property-item-descriptions
      const int PropertyTagOrientation = 0x0112;

      PropertyItem pi = GetPropertyItem(img, PropertyTagOrientation);
      if (pi == null)
        return;

      int v = pi.Value[0];

      // Взято из:
      // https://stackoverrun.com/ru/q/11817698
      switch (v)
      {
        case 2: Thumbnail.RotateFlip(RotateFlipType.RotateNoneFlipX); break;
        case 3: Thumbnail.RotateFlip(RotateFlipType.RotateNoneFlipXY); break;
        case 4: Thumbnail.RotateFlip(RotateFlipType.RotateNoneFlipY); break;
        case 5: Thumbnail.RotateFlip(RotateFlipType.Rotate90FlipX); break;
        case 6: Thumbnail.RotateFlip(RotateFlipType.Rotate90FlipNone); break;
        case 7: Thumbnail.RotateFlip(RotateFlipType.Rotate90FlipY); break;
        case 8: Thumbnail.RotateFlip(RotateFlipType.Rotate90FlipXY); break;
      }
    }


    #endregion

    #region Просмотр фото

    /// <summary>
    /// Открывает на просмотр файл фото
    /// </summary>
    /// <param name="FileName">Имя файла с расширением но без пути</param>
    public static void ViewFile(string FileName)
    {
      if (ProgramDBUI.Settings.PhotoDir.IsEmpty)
      {
        EFPApp.ErrorMessageBox("Каталог со снимками не указан");
        return;
      }

      AbsPath Path = new AbsPath(ProgramDBUI.Settings.PhotoDir, FileName);
      if (!EFPApp.FileExists(Path))
      {
        EFPApp.ErrorMessageBox("Файл не найден: " + Path.ToString());
        return;
      }

      ProcessStartInfo psi = new ProcessStartInfo();
      psi.FileName = Path.Path;
      psi.UseShellExecute = true;
      Process.Start(psi);
    }

    static void SubDocPhoto_ViewHandler_FileNeeded(object Sender, CancelEventArgs Args)
    {
      EFPFileAssociationsCommandItemsHandler ViewHandler = (EFPFileAssociationsCommandItemsHandler)Sender;
      EFPDataGridView ControlProvider = (EFPDataGridView)(ViewHandler.Tag);

      ViewHandler.FilePath = AbsPath.Empty;

      if (ProgramDBUI.Settings.PhotoDir.IsEmpty)
      {
        EFPApp.ErrorMessageBox("Каталог со снимками не указан");
        Args.Cancel = true;
        return;
      }

      if (!ControlProvider.CheckSingleRow())
      {
        Args.Cancel = true;
        return;
      }

      string FileName = DataTools.GetString(ControlProvider.CurrentDataRow, "FileName");
      AbsPath Path = new AbsPath(ProgramDBUI.Settings.PhotoDir, FileName);
      if (!EFPApp.FileExists(Path))
      {
        EFPApp.ErrorMessageBox("Файл не найден: " + Path.ToString());
        Args.Cancel = true;
        return;
      }
      ViewHandler.FilePath = Path;
    }

    #endregion
  }


  /// <summary>
  /// Столбец изображения "Thumbnail" для поля "Photo" (в документе) или поля "Id" (в поддокументе)
  /// </summary>
  public class PlantThumbnailColumn : EFPGridProducerImageColumn, ICacheFactory<PlantThumbnailColumn.ImageInfo>
  {
    #region Конструктор

    public PlantThumbnailColumn(bool IsSubDoc)
      : base("Thumbnail", new string[1] { IsSubDoc ? "Id" : "Photo" })
    {
      _IsSubDoc = IsSubDoc;
      base.HeaderText = "Фото";
      base.Resizable = false;
    }

    private bool _IsSubDoc;

    DBxMultiSubDocs _SubDocs;

    #endregion

    #region Переопределенные методы

    public override void ApplyConfig(DataGridViewColumn Column, EFPDataGridViewConfigColumn Config, EFPDataGridView ControlProvider)
    {
      base.ApplyConfig(Column, Config, ControlProvider);
      if (_IsSubDoc)
      {
        EFPSubDocGridView sdg = (EFPSubDocGridView)ControlProvider;
        _SubDocs = sdg.MainEditor.Documents["Plants"].SubDocs["PlantPhotos"];
      }

      DataGridViewImageColumn Column2 = (DataGridViewImageColumn)Column;
      Column2.ImageLayout = DataGridViewImageCellLayout.Normal;
      //Column2.Resizable = DataGridViewTriState.False;
    }

    public override int GetWidth(IEFPGridControlMeasures Measures)
    {
      return DoGetWidth();
    }

    public static int DoGetWidth()
    {
      return ProgramDBUI.Settings.ThumbnailSize.Width + 4;
    }


    protected override void OnValueNeeded(EFPGridProducerValueNeededEventArgs args)
    {
      Int32 Id = args.GetInt(0);
      if (Id == 0)
      {
        args.Value = EFPApp.MainImages.Images["EmptyImage"];
        args.ToolTipText = "Нет изображения";
        return;
      }
      else if (Id < 0)
      {
        if (_SubDocs != null)
        {
          int p = _SubDocs.IndexOfSubDocId(Id);
          if (p >= 0)
          {
            DBxSubDoc SubDoc = _SubDocs[p];
            byte[] b = SubDoc.Values["ThumbnailData"].GetBinData();
            if (b != null)
              args.Value = GetImage(b);
            return;
          }
        }
        args.Value = EFPApp.MainImages.Images["UnknownState"];
        return;
      }
      else
      {
        try
        {
          ImageInfo ii = Cache.GetItem<ImageInfo>(new string[] { Id.ToString(), ProgramDBUI.Settings.ThumbnailSizeCode.ToString() },
            CachePersistance.MemoryOnly, this);
          if (args.Reason == EFPGridProducerValueReason.Value)
            args.Value = ii.Image;
          else
          {
            args.ToolTipText = "Имя файла: " + ii.FileName;
            if (ii.ShootingTime.HasValue)
              args.ToolTipText += ", дата снимка: " + ii.ShootingTime.Value.ToString("g");
          }
        }
        catch (Exception e) // 24.11.2019
        {
          args.Value = EFPApp.MainImages.Images["Error"];
          args.ToolTipText = "Ошибка. " + e.Message;
        }
      }
    }

    private static Image GetImage(byte[] b)
    {
      if (b == null)
        return EFPApp.MainImages.Images["EmptyImage"]; // 24.11.2019
      using (MemoryStream ms = new MemoryStream(b))
      {
        Bitmap img = Image.FromStream(ms) as Bitmap;
        Size MaxSize = ProgramDBUI.Settings.ThumbnailSize;
        Size NewSize;
        if (ImagingTools.IsImageShrinkNeeded(img, MaxSize, out NewSize))
          img = new Bitmap(img, NewSize);
        return img;
      }
    }

    #endregion

    #region Класс ImageInfo

    private class ImageInfo
    {
      #region Поля

      public Image Image;

      public string FileName;

      public DateTime? ShootingTime;

      #endregion
    }

    #endregion

    #region ICacheFactory<Image> Members

    ImageInfo ICacheFactory<ImageInfo>.CreateCacheItem(string[] Keys)
    {
      Int32 Id = Int32.Parse(Keys[0]);

      // Размер можно не проверять

      byte[] b = ProgramDBUI.TheUI.DocProvider.GetBinData("PlantPhotos", Id, "ThumbnailData");
      ImageInfo ii = new ImageInfo();
      ii.Image = GetImage(b);
      ii.FileName = ProgramDBUI.TheUI.DocTypes["Plants"].SubDocTypes["PlantPhotos"].TableCache.GetString(Id, "FileName");
      ii.ShootingTime = ProgramDBUI.TheUI.DocTypes["Plants"].SubDocTypes["PlantPhotos"].TableCache.GetNullableDateTime(Id, "ShootingTime");
      return ii;
    }

    #endregion

    #region Высота строки

    public static void RowHeightInfoNeeded(object Sender, DataGridViewRowHeightInfoNeededEventArgs Args)
    {
      try
      {
        DataGridView Control = (DataGridView)Sender;
        if (Control.Columns.Contains("Thumbnail"))
          Args.Height = Math.Max(Control.RowTemplate.Height, ProgramDBUI.Settings.ThumbnailSize.Height + 4);
        else
          Args.Height = Control.RowTemplate.Height;
      }
      catch { }
    }

    #endregion
  }
}