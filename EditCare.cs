using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FreeLibSet.Forms.Docs;
using FreeLibSet.Forms;

namespace Plants
{
  internal partial class EditCare : Form
  {
    #region Конструктор формы

    public EditCare()
    {
      InitializeComponent();
    }

    #endregion

    #region Редактор

    #region InitDocEditForm

    DocumentEditor _Editor;

    public static void InitDocEditForm(object sender, InitDocEditFormEventArgs args)
    {
      EditCare form = new EditCare();
      form._Editor = args.Editor;
      form.AddPage1(args);
      if (!args.Editor.MultiDocMode)
        args.AddSubDocsPage("CareRecords");
    }

    #endregion

    #region Страница 1 (Общие)

    private EFPTextBox efpName;

    private void AddPage1(InitDocEditFormEventArgs args)
    {
      DocEditPage page = args.AddPage("Общие", MainPanel1);
      page.ImageKey = "Care";

      //Page.HelpContext = "BuxBase.chm::CompanyEdit.htm#Общие";

      efpName = new EFPTextBox(page.BaseProvider, edName);
      efpName.CanBeEmpty = false;
      args.AddText(efpName, "Name", false);

      EFPDocComboBox efpParent = new EFPDocComboBox(page.BaseProvider, cbParent, ProgramDBUI.TheUI.DocTypes["Care"]);
      efpParent.CanBeEmpty = true;
      args.AddRef(efpParent, "ParentId", true);

      EFPDocComboBox efpGroup = new EFPDocComboBox(page.BaseProvider, cbGroup, ProgramDBUI.TheUI.DocTypes["PlantGroups"]);
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
