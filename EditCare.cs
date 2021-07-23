using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AgeyevAV.ExtForms.Docs;
using AgeyevAV.ExtForms;

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

    public static void InitDocEditForm(object Sender, InitDocEditFormEventArgs Args)
    {
      EditCare Form = new EditCare();

      Form._Editor = Args.Editor;
      Form.AddPage1(Args);
      if (!Args.Editor.MultiDocMode)
        Args.AddSubDocsPage("CareRecords");
    }

    #endregion

    #region Страница 1 (Общие)

    private EFPTextBox efpName;

    private void AddPage1(InitDocEditFormEventArgs Args)
    {
      DocEditPage Page = Args.AddPage("Общие", MainPanel1);
      Page.ImageKey = "Care";

      //Page.HelpContext = "BuxBase.chm::CompanyEdit.htm#Общие";

      efpName = new EFPTextBox(Page.BaseProvider, edName);
      efpName.CanBeEmpty = false;
      Args.AddText(efpName, "Name", false);

      EFPDocComboBox efpParent = new EFPDocComboBox(Page.BaseProvider, cbParent, ProgramDBUI.TheUI.DocTypes["Care"]);
      efpParent.CanBeEmpty = true;
      Args.AddRef(efpParent, "ParentId", true);

      EFPDocComboBox efpGroup = new EFPDocComboBox(Page.BaseProvider, cbGroup, ProgramDBUI.TheUI.DocTypes["PlantGroups"]);
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