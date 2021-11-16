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
  internal partial class EditPlantFlowering : Form
  {
    #region Конструктор формы

    public EditPlantFlowering()
    {
      InitializeComponent();
    }

    #endregion

    #region Редактор

    public static void InitEditForm(object Sender, InitSubDocEditFormEventArgs Args)
    {
      EditPlantFlowering Form = new EditPlantFlowering();
      Form.AddPage1(Args);
    }

    private void AddPage1(InitSubDocEditFormEventArgs Args)
    {
      DocEditPage Page = Args.AddPage("Общие", MainPanel1);
      Page.ImageKey = Args.Editor.SubDocTypeUI.ImageKey;

      EFPDateOrRangeBox efpDate = new EFPDateOrRangeBox(Page.BaseProvider, cbDate);
      efpDate.CanBeEmpty = false;
      Args.AddDate(efpDate, "Date1", "Date2", false);

      EFPIntEditBox efpCount = new EFPIntEditBox(Page.BaseProvider, edCount);
      efpCount.Minimum = 0; // если много цветков, которые неохота считать
      efpCount.Maximum = 100;
      Args.AddInt(efpCount, "FlowerCount", true);

      EFPTextBox efpComment = new EFPTextBox(Page.BaseProvider, edComment);
      efpComment.CanBeEmpty = true;
      Args.AddText(efpComment, "Comment", true);
    }

    #endregion
  }
}