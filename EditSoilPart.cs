using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AgeyevAV.ExtForms.Docs;
using AgeyevAV.ExtForms;
using AgeyevAV.DependedValues;

namespace Plants
{
  internal partial class EditSoilPart : Form
  {
    #region Конструктор формы

    public EditSoilPart()
    {
      InitializeComponent();
    }

    #endregion

    #region Редактор

    public static void InitEditForm(object Sender, InitSubDocEditFormEventArgs Args)
    {
      EditSoilPart Form = new EditSoilPart();
      Form.AddPage1(Args);
    }

    private void AddPage1(InitSubDocEditFormEventArgs Args)
    {
      DocEditPage Page = Args.AddPage("Общие", MainPanel1);
      Page.ImageKey = Args.Editor.SubDocTypeUI.ImageKey;

      EFPDocComboBox efpSoil = new EFPDocComboBox(Page.BaseProvider, cbSoil, ProgramDBUI.TheUI.DocTypes["Soils"]);
      efpSoil.CanBeEmpty = false;
      Args.AddRef(efpSoil, "Soil", false);

      EFPNumEditBox efpPercent = new EFPNumEditBox(Page.BaseProvider, edPercent);
      efpPercent.Minimum = 0;
      efpPercent.Maximum = 100;
      Args.AddInt(efpPercent, "Percent", false);
    }

    #endregion
  }
}