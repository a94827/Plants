using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AgeyevAV.ExtForms.Docs;
using AgeyevAV.ExtForms;
using AgeyevAV;

namespace Plants
{
  /// <summary>
  /// Уход за растениями
  /// </summary>
  internal partial class EditPlantCare : Form
  {
    #region Конструктор формы

    public EditPlantCare()
    {
      InitializeComponent();
    }

    #endregion

    #region Табличный просмотр

    public static void StartDayTextColumnValueNeeded(object Sender, GridProducerUserColumnValueNeededEventArgs Args)
    {
      int StartDay = Args.GetInt("StartDay");
      if (StartDay == 0)
        Args.Value = "Весь год";
      else
      {
        MonthDay md = MonthDay.FromIntValue(StartDay);
        Args.Value = "С " + md.ToString();
      }
    }

    #endregion

    #region Редактор

    public static void InitEditForm(object Sender, InitSubDocEditFormEventArgs Args)
    {
      EditPlantCare Form = new EditPlantCare();
      Form.AddPage1(Args);
      Form.AddPage2(Args);
    }

    private void AddPage1(InitSubDocEditFormEventArgs Args)
    {
      DocEditPage Page = Args.AddPage("Общие", MainPanel1);
      Page.ImageKey = "Properties";

      EFPCheckBox efpSetStartDay = new EFPCheckBox(Page.BaseProvider, cbSetStartDay);

      EFPMonthDayBox efpStartDay = new EFPMonthDayBox(Page.BaseProvider, edStartDay);
      Args.AddInt(efpSetStartDay, efpStartDay, "StartDay", false);

      EFPTextBox efpComment = new EFPTextBox(Page.BaseProvider, edComment);
      efpComment.CanBeEmpty = true;
      Args.AddText(efpComment, "Comment", true);
    }


    EFPNumEditBox efpTMin, efpTMax;

    private void AddPage2(InitSubDocEditFormEventArgs Args)
    {
      DocEditPage Page = Args.AddPage("Температура", MainPanel2);
      Page.ImageKey = "Temperature";

      efpTMin = new EFPNumEditBox(Page.BaseProvider, edTMin);
      efpTMax = new EFPNumEditBox(Page.BaseProvider, edTMax);

      Args.AddInt(efpTMin, "TMin", true).ZeroAsNull = false;
      Args.AddInt(efpTMin, "TMax", true).ZeroAsNull = false;
    }

    #endregion

  }
}