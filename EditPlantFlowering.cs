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
    #region ����������� �����

    public EditPlantFlowering()
    {
      InitializeComponent();
    }

    #endregion

    #region ��������

    public static void InitEditForm(object sender, InitSubDocEditFormEventArgs args)
    {
      EditPlantFlowering form = new EditPlantFlowering();
      form.AddPage1(args);
    }

    private void AddPage1(InitSubDocEditFormEventArgs args)
    {
      DocEditPage page = args.AddPage("�����", MainPanel1);
      page.ImageKey = args.Editor.SubDocTypeUI.ImageKey;

      EFPDateOrRangeBox efpDate = new EFPDateOrRangeBox(page.BaseProvider, cbDate);
      efpDate.CanBeEmpty = false;
      args.AddDate(efpDate, "Date1", "Date2", false);

      EFPIntEditBox efpCount = new EFPIntEditBox(page.BaseProvider, edCount);
      efpCount.Minimum = 0; // ���� ����� �������, ������� ������� �������
      efpCount.Maximum = 100;
      args.AddInt(efpCount, "FlowerCount", true);

      EFPTextBox efpComment = new EFPTextBox(page.BaseProvider, edComment);
      efpComment.CanBeEmpty = true;
      args.AddText(efpComment, "Comment", true);
    }

    #endregion
  }
}