using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FreeLibSet.Forms;
using FreeLibSet.Forms.Docs;
using FreeLibSet.DependedValues;
using FreeLibSet.Data;
using FreeLibSet.Data.Docs;

namespace Plants
{
  internal partial class EditPotKind : Form
  {
    #region Конструктор формы

    public EditPotKind()
    {
      InitializeComponent();
    }

    #endregion

    #region Табличный просмотр

    public static void InitView(object sender, InitEFPDBxViewEventArgs args)
    {
      #region Фильтры

      RefDocGridFilter filtManufacturer = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["Companies"], "Manufacturer");
      filtManufacturer.DisplayName = "Изготовитель";
      filtManufacturer.Nullable = true;
      args.ControlProvider.Filters.Add(filtManufacturer);

      #endregion
    }

    #endregion

    #region Редактор

    #region InitDocEditForm

    DocumentEditor _Editor;

    public static void InitDocEditForm(object sender, InitDocEditFormEventArgs args)
    {
      EditPotKind form = new EditPotKind();
      form._Editor = args.Editor;
      form.AddPage1(args);
    }

    #endregion

    #region Страница 1 (общие)

    private EFPTextBox efpText, efpColor;

    private EFPIntEditBox efpD, efpH;

    private EFPSingleEditBox efpVolume;

    private void AddPage1(InitDocEditFormEventArgs args)
    {
      DocEditPage page = args.AddPage("Общие", MainPanel1);
      page.ImageKey = "PotKind";

      efpText = new EFPTextBox(page.BaseProvider, edText);
      efpText.CanBeEmpty = true;
      args.AddText(efpText, "Text", true);

      efpD = new EFPIntEditBox(page.BaseProvider, edD);
      args.AddInt(efpD, "Diameter", false);

      efpH = new EFPIntEditBox(page.BaseProvider, edH);
      args.AddInt(efpH, "Height", false);

      efpVolume = new EFPSingleEditBox(page.BaseProvider, edVolume);
      args.AddSingle(efpVolume, "Volume", false);

      efpColor = new EFPTextBox(page.BaseProvider, edColor);
      efpColor.CanBeEmpty = true;
      args.AddText(efpColor, "Color", true);

      EFPDocComboBox efpManufacturer = new EFPDocComboBox(page.BaseProvider, cbManufacturer, ProgramDBUI.TheUI.DocTypes["Companies"]);
      efpManufacturer.CanBeEmpty = true;
      args.AddRef(efpManufacturer, "Manufacturer", true);

      EFPDocComboBox efpGroup = new EFPDocComboBox(page.BaseProvider, cbGroup, ProgramDBUI.TheUI.DocTypes["PotKindGroups"]);
      efpGroup.CanBeEmpty = true;
      args.AddRef(efpGroup, "GroupId", true);

      #region Комментарий

      EFPTextBox efpComment = new EFPTextBox(page.BaseProvider, edComment);
      efpComment.CanBeEmpty = true;
      args.AddText(efpComment, "Comment", true);

      #endregion
    }

    #endregion

    #endregion
  }
}
