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
  internal partial class EditPlantAction : Form
  {
    #region Конструктор формы

    public EditPlantAction()
    {
      InitializeComponent();
    }

    #endregion

    #region Табличный просмотр

    public static void ImageValueNeeded(object sender, DBxImageValueNeededEventArgs args)
    {
      ActionKind Kind = args.GetEnum<ActionKind>("Kind");
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
      EditPlantAction Form = new EditPlantAction();
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

      EFPSubDocComboBox efpRemedyUsage = new EFPSubDocComboBox(efpRemedy, cbRemedyUsage, "RemedyUsage");
      efpRemedyUsage.CanBeEmpty = true;
      DocValueSubDocComboBox dvRemedyUsage = Args.AddRef(efpRemedyUsage, "RemedyUsage", false);
      dvRemedyUsage.UserEnabledEx = dvRemedy.UserEnabledEx;
      dvRemedyUsage.UserDisabledMode = DocValueUserDisabledMode.KeepOriginalIfGrayed;

      EFPDocComboBox efpSoil = new EFPDocComboBox(Page.BaseProvider, cbSoil, ProgramDBUI.TheUI.DocTypes["Soils"]);
      efpSoil.CanBeEmpty = true;
      DocValueDocComboBox dvSoil = Args.AddRef(efpSoil, "Soil", false);
      dvSoil.UserEnabledEx = new DepInArray<int>(efpKind.SelectedIndexEx, PlantTools.GetSoilAppliableIntArray());
      dvSoil.UserDisabledMode = DocValueUserDisabledMode.KeepOriginalIfGrayed;

      EFPDocComboBox efpPotKind = new EFPDocComboBox(Page.BaseProvider, cbPotKind, ProgramDBUI.TheUI.DocTypes["PotKinds"]);
      efpPotKind.CanBeEmpty = true;
      DocValueDocComboBox dvPotKind = Args.AddRef(efpPotKind, "PotKind", false);
      dvPotKind.UserEnabledEx = new DepInArray<int>(efpKind.SelectedIndexEx, PlantTools.GetPotKindAppliableIntArray());
      dvPotKind.UserDisabledMode = DocValueUserDisabledMode.KeepOriginalIfGrayed;

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