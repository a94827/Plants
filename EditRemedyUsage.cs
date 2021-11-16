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
  internal partial class EditRemedyUsage : Form
  {
    #region Конструктор формы

    public EditRemedyUsage()
    {
      InitializeComponent();
    }

    #endregion

    #region Редактор

    public static void InitEditForm(object Sender, InitSubDocEditFormEventArgs Args)
    {
      EditRemedyUsage Form = new EditRemedyUsage();
      Form.AddPage1(Args);
    }

    private void AddPage1(InitSubDocEditFormEventArgs Args)
    {
      DocEditPage Page = Args.AddPage("Общие", MainPanel1);
      Page.ImageKey = Args.Editor.SubDocTypeUI.ImageKey;

      EFPTextBox efpName = new EFPTextBox(Page.BaseProvider, edName);
      efpName.CanBeEmpty = false;
      Args.AddText(efpName, "Name", false);
    }

    #endregion
  }
}