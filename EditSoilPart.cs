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
  internal partial class EditSoilPart : Form
  {
    #region Конструктор формы

    public EditSoilPart()
    {
      InitializeComponent();
    }

    #endregion

    #region Редактор

    public static void InitEditForm(object sender, InitSubDocEditFormEventArgs args)
    {
      EditSoilPart form = new EditSoilPart();
      form.AddPage1(args);
    }

    private void AddPage1(InitSubDocEditFormEventArgs args)
    {
      DocEditPage page = args.AddPage("Общие", MainPanel1);
      page.ImageKey = args.Editor.SubDocTypeUI.ImageKey;

      EFPDocComboBox efpSoil = new EFPDocComboBox(page.BaseProvider, cbSoil, ProgramDBUI.TheUI.DocTypes["Soils"]);
      efpSoil.CanBeEmpty = false;
      args.AddRef(efpSoil, "Soil", false);

      EFPIntEditBox efpPercent = new EFPIntEditBox(page.BaseProvider, edPercent);
      efpPercent.Minimum = 0;
      efpPercent.Maximum = 100;
      args.AddInt(efpPercent, "Percent", false);
    }

    #endregion
  }
}
