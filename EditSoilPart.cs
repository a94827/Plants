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
    #region ����������� �����

    public EditSoilPart()
    {
      InitializeComponent();
    }

    #endregion

    #region ��������

    public static void InitEditForm(object Sender, InitSubDocEditFormEventArgs Args)
    {
      EditSoilPart Form = new EditSoilPart();
      Form.AddPage1(Args);
    }

    private void AddPage1(InitSubDocEditFormEventArgs Args)
    {
      DocEditPage Page = Args.AddPage("�����", MainPanel1);
      Page.ImageKey = Args.Editor.SubDocTypeUI.ImageKey;

      EFPDocComboBox efpSoil = new EFPDocComboBox(Page.BaseProvider, cbSoil, ProgramDBUI.TheUI.DocTypes["Soils"]);
      efpSoil.CanBeEmpty = false;
      Args.AddRef(efpSoil, "Soil", false);

      EFPIntEditBox efpPercent = new EFPIntEditBox(Page.BaseProvider, edPercent);
      efpPercent.Minimum = 0;
      efpPercent.Maximum = 100;
      Args.AddInt(efpPercent, "Percent", false);
    }

    #endregion
  }
}