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

    public static void InitView(object Sender, InitEFPDBxViewEventArgs Args)
    {
      #region Фильтры

      RefDocGridFilter FiltManufacturer = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["Companies"], "Manufacturer");
      FiltManufacturer.DisplayName = "Изготовитель";
      FiltManufacturer.Nullable = true;
      Args.ControlProvider.Filters.Add(FiltManufacturer);

      #endregion
    }

    #endregion

    #region Редактор

    #region InitDocEditForm

    DocumentEditor _Editor;

    public static void InitDocEditForm(object Sender, InitDocEditFormEventArgs Args)
    {
      EditRemedy Form = new EditRemedy();

      Form._Editor = Args.Editor;

      Form.AddPage1(Args);

      Args.AddSubDocsPage("RemedyUsage");
    }

    #endregion

    #region Страница 1 (общие)

    private EFPTextBox efpName;

    private void AddPage1(InitDocEditFormEventArgs Args)
    {
      DocEditPage Page = Args.AddPage("Общие", MainPanel1);
      Page.ImageKey = "Remedy";

      efpName = new EFPTextBox(Page.BaseProvider, edName);
      efpName.CanBeEmpty = false;
      Args.AddText(efpName, "Name", false);

      EFPDocComboBox efpManufacturer = new EFPDocComboBox(Page.BaseProvider, cbManufacturer, ProgramDBUI.TheUI.DocTypes["Companies"]);
      efpManufacturer.CanBeEmpty = true;
      Args.AddRef(efpManufacturer, "Manufacturer", true);

      EFPDocComboBox efpGroup = new EFPDocComboBox(Page.BaseProvider, cbGroup, ProgramDBUI.TheUI.DocTypes["RemedyGroups"]);
      efpGroup.CanBeEmpty = true;
      Args.AddRef(efpGroup, "GroupId", true);

      #region Комментарий

      EFPTextBox efpComment = new EFPTextBox(Page.BaseProvider, edComment);
      efpComment.CanBeEmpty = true;
      Args.AddText(efpComment, "Comment", true);

      #endregion
    }

    #endregion

    #endregion
  }
}