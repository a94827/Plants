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

    public static void InitEditForm(object sender, InitSubDocEditFormEventArgs args)
    {
      EditPlantDisease form = new EditPlantDisease();
      form.AddPage1(args);
    }

    private void AddPage1(InitSubDocEditFormEventArgs args)
    {
      DocEditPage page = args.AddPage("Общие", MainPanel1);
      page.ImageKey = args.Editor.SubDocTypeUI.ImageKey;

      EFPDocComboBox efpDisease = new EFPDocComboBox(page.BaseProvider, cbDisease, ProgramDBUI.TheUI.DocTypes["Diseases"]);
      efpDisease.CanBeEmpty = false;
      args.AddRef(efpDisease, "Disease", false);

      EFPDateOrRangeBox efpDate = new EFPDateOrRangeBox(page.BaseProvider, cbDate);
      efpDate.CanBeEmpty = false;
      args.AddDate(efpDate, "Date1", "Date2", false);

      EFPTextBox efpComment = new EFPTextBox(page.BaseProvider, edComment);
      efpComment.CanBeEmpty = true;
      args.AddText(efpComment, "Comment", true);
    }

    #endregion
  }
}