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
  internal partial class EditRemedy : Form
  {
    #region Конструктор формы

    public EditRemedy()
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
      EditRemedy form = new EditRemedy();
      form._Editor = args.Editor;
      form.AddPage1(args);

      args.AddSubDocsPage("RemedyUsage");
    }

    #endregion

    #region Страница 1 (общие)

    private EFPTextBox efpName;

    private void AddPage1(InitDocEditFormEventArgs args)
    {
      DocEditPage page = args.AddPage("Общие", MainPanel1);
      page.ImageKey = "Remedy";

      efpName = new EFPTextBox(page.BaseProvider, edName);
      efpName.CanBeEmpty = false;
      args.AddText(efpName, "Name", false);

      EFPDocComboBox efpManufacturer = new EFPDocComboBox(page.BaseProvider, cbManufacturer, ProgramDBUI.TheUI.DocTypes["Companies"]);
      efpManufacturer.CanBeEmpty = true;
      args.AddRef(efpManufacturer, "Manufacturer", true);

      EFPDocComboBox efpGroup = new EFPDocComboBox(page.BaseProvider, cbGroup, ProgramDBUI.TheUI.DocTypes["RemedyGroups"]);
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
