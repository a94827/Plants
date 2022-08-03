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
using FreeLibSet.Calendar;

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

    public static void InitEditForm(object sender, InitSubDocEditFormEventArgs args)
    {
      EditPlantFlowering form = new EditPlantFlowering();
      form.AddPage1(args);
    }

    EFPDateOrRangeBox efpDate;

    private void AddPage1(InitSubDocEditFormEventArgs args)
    {
      DocEditPage page = args.AddPage("Общие", MainPanel1);
      page.ImageKey = args.Editor.SubDocTypeUI.ImageKey;

      efpDate = new EFPDateOrRangeBox(page.BaseProvider, cbDate);
      efpDate.CanBeEmpty = false;
      args.AddDate(efpDate, "Date1", "Date2", false);

      btnDate99991231.Image = EFPApp.MainImages.Images["Date99991231"];
      EFPButton efpDate99991231 = new EFPButton(page.BaseProvider, btnDate99991231);
      efpDate99991231.DisplayName = "Бесконечное время";
      efpDate99991231.ToolTipText = "Устанавливает конечную дату диапазона " + DateRangeFormatter.Default.ToString(DateRange.Whole.LastDate, false);
      //efpDate99991231.EnabledEx = new DepAnd(efpDate.EditableEx, new DepNot(new DepEqual<DateTime>(efpDate.LastDateEx, DateRange.Whole.LastDate)));
      efpDate99991231.EnabledEx = new DepExpr3<bool, bool, bool, DateTime>(efpDate.EditableEx, efpDate.IsNotEmptyEx, efpDate.LastDateEx, CalcDate99991231Enabled);
      efpDate99991231.Click += new EventHandler(efpDate99991231_Click);

      EFPIntEditBox efpCount = new EFPIntEditBox(page.BaseProvider, edCount);
      efpCount.Minimum = 0; // если много цветков, которые неохота считать
      efpCount.Maximum = 100;
      args.AddInt(efpCount, "FlowerCount", true);

      EFPTextBox efpComment = new EFPTextBox(page.BaseProvider, edComment);
      efpComment.CanBeEmpty = true;
      args.AddText(efpComment, "Comment", true);
    }

    static bool CalcDate99991231Enabled(bool isEditable, bool isNotEmpty, DateTime lastDate)
    {
      if (!isEditable)
        return false;
      if (!isNotEmpty)
        return true;
      return lastDate != DateRange.Whole.LastDate;
    }

    void efpDate99991231_Click(object sender, EventArgs args)
    {
      DateTime dt1;
      if (efpDate.DateRange.IsEmpty)
        dt1 = DateTime.Today;
      else
        dt1 = efpDate.DateRange.FirstDate;

      efpDate.DateRange = new DateRange(dt1, DateRange.Whole.LastDate);
    }

    #endregion
  }
}