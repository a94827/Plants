using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FreeLibSet.Forms.Docs;
using FreeLibSet.Forms;
using FreeLibSet.DependedValues;

namespace Plants
{
  internal partial class EditPlantDisease : Form
  {
    #region Конструктор формы

    public EditPlantDisease()
    {
      InitializeComponent();
    }

    #endregion

    #region Редактор

    public static void InitEditForm(object Sender, InitSubDocEditFormEventArgs Args)
    {
      EditPlantDisease Form = new EditPlantDisease();
      Form.AddPage1(Args);
    }

    private void AddPage1(InitSubDocEditFormEventArgs Args)
    {
      DocEditPage Page = Args.AddPage("Общие", MainPanel1);
      Page.ImageKey = Args.Editor.SubDocTypeUI.ImageKey;

      EFPDocComboBox efpDisease = new EFPDocComboBox(Page.BaseProvider, cbDisease, ProgramDBUI.TheUI.DocTypes["Diseases"]);
      efpDisease.CanBeEmpty = false;
      Args.AddRef(efpDisease, "Disease", false);

      EFPDateOrRangeBox efpDate = new EFPDateOrRangeBox(Page.BaseProvider, cbDate);
      efpDate.CanBeEmpty = false;
      Args.AddDate(efpDate, "Date1", "Date2", false);

      EFPTextBox efpComment = new EFPTextBox(Page.BaseProvider, edComment);
      efpComment.CanBeEmpty = true;
      Args.AddText(efpComment, "Comment", true);
    }

    #endregion
  }
}