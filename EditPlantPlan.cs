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
using AgeyevAV;

namespace Plants
{
  internal partial class EditPlantPlan : Form
  {
    #region Конструктор формы

    public EditPlantPlan()
    {
      InitializeComponent();
    }

    #endregion

    #region Табличный просмотр

    public static void ImageValueNeeded(object sender, DBxImageValueNeededEventArgs args)
    {
      ActionKind Kind = (ActionKind)(args.GetInt("Kind"));
      args.ImageKey = PlantTools.GetActionImageKey(Kind);
    }

    public static void ActionTextColumnValueNeeded(object sender, EFPGridProducerValueNeededEventArgs args)
    {
      ActionKind Kind = args.GetEnum<ActionKind>("Kind");
      switch (Kind)
      {
        case ActionKind.Other:
          args.Value = args.GetString("ActionName");
          break;
        case ActionKind.Treatment:
          Int32 RemedyId = args.GetInt("Remedy");
          if (RemedyId == 0)
            args.Value = PlantTools.GetActionName(Kind);
          else
            args.Value = "Обработка препаратом \"" + ProgramDBUI.TheUI.DocProvider.DBCache["Remedies"].GetString(RemedyId, "Name") + "\"";
          break;
        default:
          args.Value = PlantTools.GetActionName(Kind);
          break;
      }
    }

    #endregion

    #region Редактор

    public static void InitEditForm(object Sender, InitSubDocEditFormEventArgs Args)
    {
      EditPlantPlan Form = new EditPlantPlan();
      Form.AddPage1(Args);
    }

    private void AddPage1(InitSubDocEditFormEventArgs Args)
    {
      DocEditPage Page = Args.AddPage("Общие", MainPanel1);
      Page.ImageKey = Args.Editor.SubDocTypeUI.ImageKey;

      cbKind.Items.AddRange(PlantTools.ActionNames);
      EFPListComboBox efpKind = new EFPListComboBox(Page.BaseProvider, cbKind);
      new ListControlImagePainter(cbKind, PlantTools.ActionImageKeys);
      Args.AddInt(efpKind, "Kind", false);

      EFPTextBox efpActionName = new EFPTextBox(Page.BaseProvider, edActionName);
      efpActionName.CanBeEmpty = false;
      DocValueTextBox dvActionName = Args.AddText(efpActionName, "ActionName", false);
      dvActionName.UserEnabledEx = new DepEqual<int>(efpKind.SelectedIndexEx, (int)(ActionKind.Other));
      dvActionName.UserDisabledMode = DocValueUserDisabledMode.KeepOriginalIfGrayed;

      EFPDocComboBox efpRemedy = new EFPDocComboBox(Page.BaseProvider, cbRemedy, ProgramDBUI.TheUI.DocTypes["Remedies"]);
      efpRemedy.CanBeEmpty = false;
      DocValueDocComboBox dvRemedy = Args.AddRef(efpRemedy, "Remedy", false);
      dvRemedy.UserEnabledEx = new DepEqual<int>(efpKind.SelectedIndexEx, (int)(ActionKind.Treatment));
      dvRemedy.UserDisabledMode = DocValueUserDisabledMode.KeepOriginalIfGrayed;

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