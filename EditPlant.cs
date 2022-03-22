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

    public static void ImageValueNeeded(object sender, DBxImageValueNeededEventArgs args)
    {
      PlantMovementState movementState = (PlantMovementState)(args.GetInt("MovementState"));
      args.ImageKey = PlantTools.GetPlantMovementStateImageKey(movementState);
      if (movementState != PlantMovementState.Placed)
        args.Grayed = true;
      switch (movementState)
      {
        case PlantMovementState.Draft:
          args.ColorType = EFPDataGridViewColorType.Special;
          break;
      }
    }

    public static void ContraNameColumnValueNeeded(object sender, EFPGridProducerValueNeededEventArgs args)
    {
      Int32 id;

      id = args.GetInt("ToContra");
      if (id != 0)
      {
        args.Value = ProgramDBUI.TheUI.DocTypes["Contras"].GetTextValue(id);
        return;
      }

      id = args.GetInt("ToPlant");
      if (id != 0)
      {
        args.Value = ProgramDBUI.TheUI.DocTypes["Plants"].GetTextValue(id);
        return;
      }

      id = args.GetInt("FromContra");
      if (id != 0)
      {
        args.Value = ProgramDBUI.TheUI.DocTypes["Contras"].GetTextValue(id);
        return;
      }

      id = args.GetInt("FromPlant");
      if (id != 0)
      {
        args.Value = ProgramDBUI.TheUI.DocTypes["Plants"].GetTextValue(id);
        return;
      }
    }

    public static void FirstPlannedActionTextColumnValueNeeded(object sender, EFPGridProducerValueNeededEventArgs args)
    {
      ActionKind kind = (ActionKind)(args.GetInt("FirstPlannedAction.Kind"));
      if (kind == ActionKind.Other)
        args.Value = args.GetString("FirstPlannedAction.ActionName");
      else
        args.Value = PlantTools.GetActionName(kind);
    }

    public static void FirstPlannedActionImageColumnValueNeeded(object sender, EFPGridProducerValueNeededEventArgs args)
    {
      // TODO: Хорошо бы еще и раскрашивать просроченные действия

      DateTime? date1 = args.GetNullableDateTime("FirstPlannedAction.Date1");
      DateTime? date2 = args.GetNullableDateTime("FirstPlannedAction.Date2");
      ActionKind kind = (ActionKind)(args.GetInt("FirstPlannedAction.Kind"));

      if (date1.HasValue && date2.HasValue)
      {
        if (date2.Value < DateTime.Today)
          args.Value = EFPApp.MainImages.Images["Warning"];
        else
          args.Value = EFPApp.MainImages.Images[PlantTools.GetActionImageKey(kind)];
      }
      else if (date1.HasValue || date2.HasValue)
        args.Value = EFPApp.MainImages.Images["Error"]; // ошибка - не может такого быть
      else
        args.Value = EFPApp.MainImages.Images["EmptyImage"];
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
    public static void InitView(object sender, InitEFPDBxViewEventArgs args)
    {
      EFPDataGridView controlProvider = (EFPDataGridView)(args.ControlProvider);
      //((EFPDataGridView)(Args.ControlProvider)).Control.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
      controlProvider.Control.RowHeightInfoNeeded += PlantThumbnailColumn.RowHeightInfoNeeded;

      controlProvider.Idle += new EventHandler(ControlProvider_Idle);

      #region Фильтры

      NullNotNullGridFilter filtHasNumber = new NullNotNullGridFilter("Number", typeof(int));
      filtHasNumber.DisplayName = "Номер по каталогу";
      filtHasNumber.FilterTextNull = "Нет";
      filtHasNumber.FilterTextNotNull = "Есть";
      args.ControlProvider.Filters.Add(filtHasNumber);

      NullNotNullGridFilter filtHasPhoto = new NullNotNullGridFilter("Photo", typeof(Int32));
      filtHasPhoto.DisplayName = "Фото";
      filtHasPhoto.FilterTextNull = "Нет";
      filtHasPhoto.FilterTextNotNull = "Есть";
      args.ControlProvider.Filters.Add(filtHasPhoto);

      EnumGridFilter filtState = new EnumGridFilter("MovementState", PlantTools.PlantMovementStateNames);
      filtState.DisplayName = "Состояние";
      filtState.ImageKeys = PlantTools.PlantMovementStateImageKeys;
      args.ControlProvider.Filters.Add(filtState);

      RefDocGridFilterSet filtPlace = new RefDocGridFilterSet(ProgramDBUI.TheUI.DocTypes["Places"], "Place");
      filtPlace.DisplayName = "Текущее место расположения";
      filtPlace.Nullable = true;
      args.ControlProvider.Filters.Add(filtPlace);

      EnumGridFilter filtLastAction = new EnumGridFilter("LastActionKind", PlantTools.ActionNames);
      filtLastAction.DisplayName = "Последнее действие";
      filtLastAction.ImageKeys = PlantTools.ActionImageKeys;
      args.ControlProvider.Filters.Add(filtLastAction);

      RefDocGridFilterSet filtFromContra = new RefDocGridFilterSet(ProgramDBUI.TheUI.DocTypes["Contras"], "FromContra");
      filtFromContra.DisplayName = "От кого получено";
      filtFromContra.Nullable = true;
      args.ControlProvider.Filters.Add(filtFromContra);

      RefDocGridFilterSet filtToContra = new RefDocGridFilterSet(ProgramDBUI.TheUI.DocTypes["Contras"], "ToContra");
      filtToContra.DisplayName = "Кому передано";
      filtToContra.Nullable = true;
      args.ControlProvider.Filters.Add(filtToContra);

      RefDocGridFilterSet filtSoil = new RefDocGridFilterSet(ProgramDBUI.TheUI.DocTypes["Soils"], "Soil");
      filtSoil.DisplayName = "Грунт";
      filtSoil.Nullable = true;
      args.ControlProvider.Filters.Add(filtSoil);

      RefDocGridFilterSet filtPotKind = new RefDocGridFilterSet(ProgramDBUI.TheUI.DocTypes["PotKinds"], "PotKind");
      filtPotKind.DisplayName = "Горшок";
      filtPotKind.Nullable = true;
      args.ControlProvider.Filters.Add(filtPotKind);

      RefDocGridFilter filtManufacturer = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["Companies"], "Manufacturer");
      filtManufacturer.DisplayName = "Изготовитель";
      filtManufacturer.Nullable = true;
      args.ControlProvider.Filters.Add(filtManufacturer);

      RefDocGridFilter filtCare = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["Care"], "Care");
      filtCare.DisplayName = "Уход";
      filtCare.Nullable = true;
      args.ControlProvider.Filters.Add(filtCare);

      #endregion

      #region Локальное меню

      EFPCommandItem ci;
      ci = new EFPCommandItem("Edit", "AttrTable");
      ci.MenuText = "Таблица атрибутов";
      ci.ImageKey = "AttributeTable";
      ci.GroupBegin = true;
      ci.GroupEnd = true;
      ci.Tag = args.ControlProvider;
      ci.Click += new EventHandler(ciRB_AttrTable_Click);
      args.ControlProvider.CommandItems.Add(ci);

      EFPFileAssociationsCommandItemsHandler viewHandler = new EFPFileAssociationsCommandItemsHandler(args.ControlProvider.CommandItems, ".jpg");
      viewHandler.FileNeeded += new CancelEventHandler(Doc_ViewHandler_FileNeeded);
      viewHandler.Tag = args.ControlProvider;

      #endregion
    }

    static void ControlProvider_Idle(object sender, EventArgs args)
    {
      try
      {
        EFPDataGridView controlProvider = (EFPDataGridView)sender;

        EFPDataGridViewColumn col = controlProvider.Columns["Thumbnail"];
        if (col != null)
        {
          int w = PlantThumbnailColumn.DoGetWidth();
          col.GridColumn.MinimumWidth = w;
          col.GridColumn.Width = w;
        }
      }
      catch (Exception e)
      {
        LogoutTools.LogoutException(e, "Ошибка Plants EFPDataGridView.Idle");
      }
    }


    #region Команды локального меню

    static void ciRB_AttrTable_Click(object sender, EventArgs args)
    {
      EFPCommandItem ci = (EFPCommandItem)sender;
      IEFPDocView controlProvider = (IEFPDocView)(ci.Tag);
      Int32[] docIds = controlProvider.SelectedIds;

      AttrTableViewForm form = new AttrTableViewForm(docIds);
      EFPApp.ShowFormOrDialog(form);

    }

    static void Doc_ViewHandler_FileNeeded(object sender, CancelEventArgs args)
    {
      EFPFileAssociationsCommandItemsHandler ViewHandler = (EFPFileAssociationsCommandItemsHandler)sender;
      EFPDocGridView controlProvider = (EFPDocGridView)(ViewHandler.Tag);

      ViewHandler.FilePath = AbsPath.Empty;

      if (ProgramDBUI.Settings.PhotoDir.IsEmpty)
      {
        EFPApp.ErrorMessageBox("Каталог со снимками не указан");
        args.Cancel = true;
        return;
      }

      if (!controlProvider.CheckSingleRow())
      {
        args.Cancel = true;
        return;
      }


      string fileName = controlProvider.DocTypeUI.TableCache.GetString(controlProvider.CurrentId, "Photo.FileName");
      if (String.IsNullOrEmpty(fileName))
      {
        EFPApp.ErrorMessageBox("Для выбранного растения не задано изображение");
        args.Cancel = true;
        return;
      }

      AbsPath path = new AbsPath(ProgramDBUI.Settings.PhotoDir, fileName);
      if (!EFPApp.FileExists(path))
      {
        EFPApp.ErrorMessageBox("Файл не найден: " + path.ToString());
        args.Cancel = true;
        return;
      }
      ViewHandler.FilePath = path;
    }

    #endregion

    #endregion

    #region Редактор документа

    public static void BeforeEditDoc(object sender, BeforeDocEditEventArgs args)
    {
      switch (args.Editor.State)
      {
        case EFPDataGridViewState.Edit:
        case EFPDataGridViewState.View:
          if (args.CurrentColumnName == "Thumbnail" && (!args.Editor.MultiDocMode))
          {
            args.Cancel = true;
            Int32 subDocId = args.Editor.MainValues["Photo"].AsInteger;
            if (subDocId == 0)
            {
              EFPApp.ShowTempMessage("Нет фото растения");
              return;
            }
            DBxSubDoc sd = args.Editor.Documents[0][0].SubDocs["PlantPhotos"].GetSubDocById(subDocId);
            string fileName = sd.Values["FileName"].AsString;
            ViewFile(fileName);
            return;
          }
          break;
      }
    }

    public static void InitDocEditForm(object sender, InitDocEditFormEventArgs args)
    {
      EditPlant form = new EditPlant();
      form.AddPage1(args);
      if (!args.Editor.MultiDocMode)
      {
        form.AddPage2(args);
        args.AddSubDocsPage("PlantAttributes");
        args.AddSubDocsPage("PlantMovement").Title = "Движение";
        args.AddSubDocsPage("PlantActions").Title = "Действия";
        args.AddSubDocsPage("PlantFlowering").Title = "Цветение";
        args.AddSubDocsPage("PlantDiseases").Title = "Заболевания";
        args.AddSubDocsPage("PlantPlans").Title = "План";
      }
    }

    #region Общие

    EFPTextBox efpLocalName, efpLatinName, efpDescrName;

    EFPIntEditBox efpNumber;

    private void AddPage1(InitDocEditFormEventArgs args)
    {
      DocEditPage page = args.AddPage("Общие", MainPanel1);
      page.ImageKey = "Properties";

      efpLocalName = new EFPTextBox(page.BaseProvider, edLocalName);
      efpLocalName.CanBeEmpty = true;
      args.AddText(efpLocalName, "LocalName", true);

      efpLatinName = new EFPTextBox(page.BaseProvider, edLatinName);
      efpLatinName.CanBeEmpty = true;
      args.AddText(efpLatinName, "LatinName", true);

      efpDescrName = new EFPTextBox(page.BaseProvider, edDescrName);
      efpDescrName.CanBeEmpty = true;
      args.AddText(efpDescrName, "Description", true);

      if (!args.Editor.MultiDocMode)
      {
        efpLocalName.Validating += new UIValidatingEventHandler(efpAnyName_Validating);
        efpLocalName.TextEx.ValueChanged += new EventHandler(efpAnyName_ValueChanged);
        efpLatinName.Validating += new UIValidatingEventHandler(efpAnyName_Validating);
        efpLatinName.TextEx.ValueChanged += new EventHandler(efpAnyName_ValueChanged);
        efpDescrName.Validating += new UIValidatingEventHandler(efpAnyName_Validating);
        efpDescrName.TextEx.ValueChanged += new EventHandler(efpAnyName_ValueChanged);
      }

      EFPDocComboBox efpManufacturer = new EFPDocComboBox(page.BaseProvider, cbManufacturer, ProgramDBUI.TheUI.DocTypes["Companies"]);
      efpManufacturer.CanBeEmpty = true;
      args.AddRef(efpManufacturer, "Manufacturer", true);

      EFPDocComboBox efpCare = new EFPDocComboBox(page.BaseProvider, cbCare, ProgramDBUI.TheUI.DocTypes["Care"]);
      efpCare.CanBeEmpty = true;
      args.AddRef(efpCare, "Care", true);

      efpNumber = new EFPIntEditBox(page.BaseProvider, edNumber);
      efpNumber.Validating += new UIValidatingEventHandler(efpNumber_Validating);
      efpNumber.Minimum = 0;
      efpNumber.Maximum = Int16.MaxValue;
      args.AddInt(efpNumber, "Number", false);

      EFPDocComboBox efpGroup = new EFPDocComboBox(page.BaseProvider, cbGroup, ProgramDBUI.TheUI.DocTypes["PlantGroups"]);
      efpGroup.CanBeEmpty = true;
      args.AddRef(efpGroup, "GroupId", true);

      EFPTextBox efpComment = new EFPTextBox(page.BaseProvider, edComment);
      efpComment.CanBeEmpty = true;
      args.AddText(efpComment, "Comment", true);
    }

    void efpAnyName_ValueChanged(object sender, EventArgs args)
    {
      efpLocalName.Validate();
      efpLatinName.Validate();
      efpDescrName.Validate();
    }

    void efpAnyName_Validating(object sender, UIValidatingEventArgs args)
    {
      if (args.ValidateState == UIValidateState.Error)
        return;
      if (efpLocalName.Text.Length + efpLatinName.Text.Length + efpDescrName.Text.Length == 0)
        args.SetError("Какое-либо из названий должно быт заполнено");
    }

    void efpNumber_Validating(object sender, UIValidatingEventArgs args)
    {
      if (args.ValidateState != UIValidateState.Ok)
        return;
      if (efpNumber.Value == 0)
        args.SetWarning("Номер по каталогу не задан");
    }

    #endregion

    #region Фото

    /// <summary>
    /// Идентификатор поддокумента фото, которое будет использоваться в каталог
    /// </summary>
    private Int32 _MainPhotoSubDocId;

    private void AddPage2(InitDocEditFormEventArgs args)
    {
      args.Editor.AfterReadValues += new DocEditEventHandler(Editor_AfterReadValues);
      args.Editor.BeforeWrite += new DocEditCancelEventHandler(Editor_BeforeWrite);
      args.Editor.AfterWrite += new DocEditEventHandler(Editor_AfterWrite);

      DocEditPage page = args.AddPage("Фото", MainPanel2);
      page.ImageKey = "Picture";

      EFPControlWithToolBar<DataGridView> cwt = new EFPControlWithToolBar<DataGridView>(page.BaseProvider, MainPanel2);
      EFPSubDocGridView sdgPhotos = new EFPSubDocGridView(cwt, args.Editor, args.MultiDocs.SubDocs["PlantPhotos"]);
      sdgPhotos.GridProducerPostInit += new EventHandler(sdgPhotos_GridProducerPostInit);

      if (ProgramDBUI.Settings.PhotoDir.IsEmpty)
        sdgPhotos.CanView = false; // нельзя просматривать снимки

      EFPCommandItem ci;
      ci = new EFPCommandItem("Edit", "SelectDefault");
      ci.MenuText = "Выбрать изображение для каталога";
      ci.ImageKey = "Ok";
      ci.ShortCut = Keys.F4;
      ci.Click += new EventHandler(ciSelectDefault_Click);
      ci.Enabled = !args.Editor.IsReadOnly;
      ci.Tag = sdgPhotos;
      ci.GroupBegin = true;
      ci.GroupEnd = true;
      sdgPhotos.CommandItems.Add(ci);
    }

    void sdgPhotos_GridProducerPostInit(object sender, EventArgs args)
    {
      EFPSubDocGridView sdgPhotos = (EFPSubDocGridView)sender;
      //sdgPhotos.GetRowAttributes += new EFPDataGridViewRowAttributesEventHandler(sdgPhotos_GetRowAttributes);
      sdgPhotos.GetCellAttributes += new EFPDataGridViewCellAttributesEventHandler(sdgPhotos_GetCellAttributes);

    }

    void Editor_AfterReadValues(object sender, DocEditEventArgs args)
    {
      _MainPhotoSubDocId = args.Editor.Documents[0].Values["Photo"].AsInteger;
    }

    void Editor_BeforeWrite(object sender, DocEditCancelEventArgs args)
    {
      DataTable table = args.Editor.Documents[0].SubDocs["PlantPhotos"].CreateSubDocsData();
      if (table.Rows.Count == 0)
        args.Editor.Documents[0].Values["Photo"].SetNull();
      else
      {
        table.DefaultView.Sort = "ShootingTime";
        foreach (DataRowView drv in table.DefaultView)
        {
          if (DataTools.GetInt(drv.Row, "Id") == _MainPhotoSubDocId)
          {
            args.Editor.Documents[0].Values["Photo"].SetInteger(_MainPhotoSubDocId);
            return;
          }
        }
        // Берем первое изображение
        _MainPhotoSubDocId = DataTools.GetInt(table.DefaultView[0].Row, "Id");
        args.Editor.Documents[0].Values["Photo"].SetInteger(_MainPhotoSubDocId);
      }
    }

    void Editor_AfterWrite(object sender, DocEditEventArgs args)
    {
      // Нужно после нажатия кнопки "Запись", если основным было сделано новое фото
      _MainPhotoSubDocId = args.Editor.Documents[0].Values["Photo"].AsInteger;
    }

    void ciSelectDefault_Click(object sender, EventArgs args)
    {
      EFPCommandItem ci = (EFPCommandItem)sender;
      EFPSubDocGridView sdgPhotos = (EFPSubDocGridView)(ci.Tag);
      if (!sdgPhotos.CheckSingleRow())
        return;

      _MainPhotoSubDocId = sdgPhotos.CurrentId;
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
    void sdgPhotos_GetCellAttributes(object sender, EFPDataGridViewCellAttributesEventArgs args)
    {
      if (args.DataRow == null)
        return;
      if (args.ColumnIndex == 0) // значок
      {
        Int32 id = DataTools.GetInt(args.DataRow, "Id");
        if (id == _MainPhotoSubDocId)
          args.Value = EFPApp.MainImages.Images["Ok"];
      }
    }

    #endregion

    #endregion

    #region Табличный просмотр поддокументов "Фото"

    public static void SubDocPhoto_InitView(object sender, InitEFPDBxViewEventArgs args)
    {
      if (ProgramDBUI.Settings.PhotoDir.IsEmpty)
        args.ControlProvider.ReadOnly = true;
      // ((EFPDataGridView)(Args.ControlProvider)).Control.RowHeightInfoNeeded += new DataGridViewRowHeightInfoNeededEventHandler(Control_RowHeightInfoNeeded);
      ((EFPDataGridView)(args.ControlProvider)).Control.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

      EFPFileAssociationsCommandItemsHandler viewHandler = new EFPFileAssociationsCommandItemsHandler(args.ControlProvider.CommandItems, ".jpg");
      viewHandler.FileNeeded += new CancelEventHandler(SubDocPhoto_ViewHandler_FileNeeded);
      viewHandler.Tag = args.ControlProvider;
    }

    //static void Control_RowHeightInfoNeeded(object Sender, DataGridViewRowHeightInfoNeededEventArgs Args)
    //{
    //  Args.Height = Math.Max(Args.Height, ProgramDBUI.Setting.ThumbnailSize.Height + 4);
    //}

    #endregion

    #region Редактор документа "Фото"

    public static void SubDocPhoto_BeforeEdit(object sender, BeforeSubDocEditEventArgs args)
    {
      args.ShowEditor = false;
      switch (args.Editor.State)
      {
        case EFPDataGridViewState.Insert:
          try
          {
            args.Cancel = true;
            if (InsertFiles(args.MainEditor.Documents[0][0].SubDocs["PlantPhotos"]))
              args.MainEditor.SubDocsChangeInfo.Changed = true;
          }
          catch (Exception e)
          {
            EFPApp.ShowException(e, "Ошибка добавления фото");
            args.Cancel = true;
          }
          break;
        case EFPDataGridViewState.Edit:
          // TODO: Определять, что текущий столбец - "Comment", иначе показывать изображение
          SubDocPhoto_EditComment(args);
          break;

        case EFPDataGridViewState.View:
          DBxSubDoc subDoc = args.Editor.SubDocs[0];
          ViewFile(subDoc.Values["FileName"].AsString);
          args.Cancel = true;
          break;
      }
    }

    private static void SubDocPhoto_EditComment(BeforeSubDocEditEventArgs args)
    {
      DBxSubDoc subDoc = args.Editor.SubDocs[0];

      MultiLineTextInputDialog dlg = new MultiLineTextInputDialog();
      dlg.Title = "Комментарий";
      dlg.Prompt = "Комментарий к снимку " + subDoc.Values["FileName"].AsString;
      dlg.Text = subDoc.Values["Comment"].AsString;
      dlg.CanBeEmpty = true;
      if (dlg.ShowDialog() != DialogResult.OK)
        return;

      subDoc.Values["Comment"].SetString(dlg.Text);

      args.MainEditor.SubDocsChangeInfo.Changed = true;
    }

    private static bool InsertFiles(DBxSingleSubDocs subDocs)
    {
      OpenFileDialog dlg = new OpenFileDialog();
      dlg.InitialDirectory = ProgramDBUI.Settings.PhotoDir.Path;
      dlg.Filter = "Фотографии JPEG|*.jpg;*.jpeg";
      dlg.Multiselect = true; // 22.09.2019
      if (dlg.ShowDialog() != DialogResult.OK)
        return false;
      for (int i = 0; i < dlg.FileNames.Length; i++)
      {
        AbsPath path = new AbsPath(dlg.FileNames[i]);
        if (path.ParentDir != ProgramDBUI.Settings.PhotoDir)
        {
          EFPApp.ErrorMessageBox("Файл должен располагаться в каталоге \"" + ProgramDBUI.Settings.PhotoDir.Path + "\"");
          return false;
        }

        DBxSubDoc subDoc = subDocs.Insert();

        subDoc.Values["FileName"].SetString(path.FileName);
        using (FileStream fs = new FileStream(path.Path, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          string md5 = FileTools.MD5Sum(fs);
          subDoc.Values["FileMD5"].SetString(md5);
          fs.Position = 0;
          using (Image img = Image.FromStream(fs))
          {
            using (Bitmap thumbnail = WinFormsTools.CreateThumbnailImage(img, PlantTools.ThumbnailSize))
            {
              // 22.09.2019
              RotateThumbnail(thumbnail, img);

              using (MemoryStream ms = new MemoryStream())
              {
                thumbnail.Save(ms, ImageFormat.Bmp);
                subDoc.Values["ThumbnailData"].SetBinData(ms.GetBuffer());
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

                  subDoc.Values["ShootingTime"].SetNullableDateTime(dt);
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
    public static void RotateThumbnail(Bitmap thumbnail, Image img)
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
        case 2: thumbnail.RotateFlip(RotateFlipType.RotateNoneFlipX); break;
        case 3: thumbnail.RotateFlip(RotateFlipType.RotateNoneFlipXY); break;
        case 4: thumbnail.RotateFlip(RotateFlipType.RotateNoneFlipY); break;
        case 5: thumbnail.RotateFlip(RotateFlipType.Rotate90FlipX); break;
        case 6: thumbnail.RotateFlip(RotateFlipType.Rotate90FlipNone); break;
        case 7: thumbnail.RotateFlip(RotateFlipType.Rotate90FlipY); break;
        case 8: thumbnail.RotateFlip(RotateFlipType.Rotate90FlipXY); break;
      }
    }


    #endregion

    #region Просмотр фото

    /// <summary>
    /// Открывает на просмотр файл фото
    /// </summary>
    /// <param name="fileName">Имя файла с расширением но без пути</param>
    public static void ViewFile(string fileName)
    {
      if (ProgramDBUI.Settings.PhotoDir.IsEmpty)
      {
        EFPApp.ErrorMessageBox("Каталог со снимками не указан");
        return;
      }

      AbsPath path = new AbsPath(ProgramDBUI.Settings.PhotoDir, fileName);
      if (!EFPApp.FileExists(path))
      {
        EFPApp.ErrorMessageBox("Файл не найден: " + path.ToString());
        return;
      }

      ProcessStartInfo psi = new ProcessStartInfo();
      psi.FileName = path.Path;
      psi.UseShellExecute = true;
      Process.Start(psi);
    }

    static void SubDocPhoto_ViewHandler_FileNeeded(object sender, CancelEventArgs args)
    {
      EFPFileAssociationsCommandItemsHandler viewHandler = (EFPFileAssociationsCommandItemsHandler)sender;
      EFPDataGridView controlProvider = (EFPDataGridView)(viewHandler.Tag);

      viewHandler.FilePath = AbsPath.Empty;

      if (ProgramDBUI.Settings.PhotoDir.IsEmpty)
      {
        EFPApp.ErrorMessageBox("Каталог со снимками не указан");
        args.Cancel = true;
        return;
      }

      if (!controlProvider.CheckSingleRow())
      {
        args.Cancel = true;
        return;
      }

      string fileName = DataTools.GetString(controlProvider.CurrentDataRow, "FileName");
      AbsPath path = new AbsPath(ProgramDBUI.Settings.PhotoDir, fileName);
      if (!EFPApp.FileExists(path))
      {
        EFPApp.ErrorMessageBox("Файл не найден: " + path.ToString());
        args.Cancel = true;
        return;
      }
      viewHandler.FilePath = path;
    }

    #endregion
  }


  /// <summary>
  /// Столбец изображения "Thumbnail" для поля "Photo" (в документе) или поля "Id" (в поддокументе)
  /// </summary>
  public class PlantThumbnailColumn : EFPGridProducerImageColumn, ICacheFactory<PlantThumbnailColumn.ImageInfo>
  {
    #region Конструктор

    public PlantThumbnailColumn(bool isSubDoc)
      : base("Thumbnail", new string[1] { isSubDoc ? "Id" : "Photo" })
    {
      _IsSubDoc = isSubDoc;
      base.HeaderText = "Фото";
      base.Resizable = false;
    }

    private bool _IsSubDoc;

    DBxMultiSubDocs _SubDocs;

    #endregion

    #region Переопределенные методы

    public override void ApplyConfig(DataGridViewColumn gridColumn, EFPDataGridViewConfigColumn config, EFPDataGridView controlProvider)
    {
      base.ApplyConfig(gridColumn, config, controlProvider);
      if (_IsSubDoc)
      {
        EFPSubDocGridView sdg = (EFPSubDocGridView)controlProvider;
        _SubDocs = sdg.MainEditor.Documents["Plants"].SubDocs["PlantPhotos"];
      }

      DataGridViewImageColumn column2 = (DataGridViewImageColumn)gridColumn;
      column2.ImageLayout = DataGridViewImageCellLayout.Normal;
      //Column2.Resizable = DataGridViewTriState.False;
    }

    public override int GetWidth(IEFPGridControlMeasures measures)
    {
      return DoGetWidth();
    }

    public static int DoGetWidth()
    {
      return ProgramDBUI.Settings.ThumbnailSize.Width + 4;
    }


    protected override void OnValueNeeded(EFPGridProducerValueNeededEventArgs args)
    {
      Int32 id = args.GetInt(0);
      if (id == 0)
      {
        args.Value = EFPApp.MainImages.Images["EmptyImage"];
        args.ToolTipText = "Нет изображения";
        return;
      }
      else if (id < 0)
      {
        if (_SubDocs != null)
        {
          int p = _SubDocs.IndexOfSubDocId(id);
          if (p >= 0)
          {
            DBxSubDoc subDoc = _SubDocs[p];
            byte[] b = subDoc.Values["ThumbnailData"].GetBinData();
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
          ImageInfo ii = Cache.GetItem<ImageInfo>(new string[] { id.ToString(), ProgramDBUI.Settings.ThumbnailSizeCode.ToString() },
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
        Size maxSize = ProgramDBUI.Settings.ThumbnailSize;
        Size newSize;
        if (ImagingTools.IsImageShrinkNeeded(img, maxSize, out newSize))
          img = new Bitmap(img, newSize);
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

    ImageInfo ICacheFactory<ImageInfo>.CreateCacheItem(string[] keys)
    {
      Int32 Id = Int32.Parse(keys[0]);

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

    public static void RowHeightInfoNeeded(object sender, DataGridViewRowHeightInfoNeededEventArgs args)
    {
      try
      {
        DataGridView Control = (DataGridView)sender;
        if (Control.Columns.Contains("Thumbnail"))
          args.Height = Math.Max(Control.RowTemplate.Height, ProgramDBUI.Settings.ThumbnailSize.Height + 4);
        else
          args.Height = Control.RowTemplate.Height;
      }
      catch { }
    }

    #endregion
  }
}