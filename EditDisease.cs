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
  internal partial class EditDisease : Form
  {
    #region Конструктор формы

    public EditDisease()
    {
      InitializeComponent();
    }

    #endregion

    #region Редактор

    #region InitDocEditForm

    DocumentEditor _Editor;

    public static void InitDocEditForm(object Sender, InitDocEditFormEventArgs Args)
    {
      EditDisease Form = new EditDisease();

      Form._Editor = Args.Editor;

      Form.AddPage1(Args);
    }

    #endregion

    #region Страница 1 (общие)

    private EFPTextBox efpName;

    private void AddPage1(InitDocEditFormEventArgs Args)
    {
      DocEditPage Page = Args.AddPage("Общие", MainPanel1);
      Page.ImageKey = "Disease";

      efpName = new EFPTextBox(Page.BaseProvider, edName);
      efpName.CanBeEmpty = false;
      Args.AddText(efpName, "Name", false);

      EFPDocComboBox efpGroup = new EFPDocComboBox(Page.BaseProvider, cbGroup, ProgramDBUI.TheUI.DocTypes["DiseaseGroups"]);
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