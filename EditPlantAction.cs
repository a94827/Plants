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
      ActionKind kind = args.GetEnum<ActionKind>("Kind");
      args.ImageKey = PlantTools.GetActionImageKey(kind);
    }

    public static void ActionTextColumnValueNeeded(object sender, EFPGridProducerValueNeededEventArgs args)
    {
      ActionKind kind = args.GetEnum<ActionKind>("Kind");
      switch (kind)
      {
        case ActionKind.Other:
          args.Value = args.GetString("ActionName");
          break;
        case ActionKind.Treatment:
          Int32 remedyId = args.GetInt("Remedy");
          if (remedyId == 0)
            args.Value = PlantTools.GetActionName(kind);
          else
            args.Value = "Обработка препаратом \"" + ProgramDBUI.TheUI.DocProvider.DBCache["Remedies"].GetString(remedyId, "Name") + "\"";
          break;
        default:
          args.Value = PlantTools.GetActionName(kind);
          break;
      }
    }

    #endregion

    #region Редактор

    public static void InitEditForm(object sender, InitSubDocEditFormEventArgs args)
    {
      EditPlantAction form = new EditPlantAction();
      form.AddPage1(args);
    }

    private void AddPage1(InitSubDocEditFormEventArgs args)
    {
      DocEditPage page = args.AddPage("Общие", MainPanel1);
      page.ImageKey = args.Editor.SubDocTypeUI.ImageKey;

      cbKind.Items.AddRange(PlantTools.ActionNames);
      EFPListComboBox efpKind = new EFPListComboBox(page.BaseProvider, cbKind);
      new ListControlImagePainter(cbKind, PlantTools.ActionImageKeys);
      args.AddInt(efpKind, "Kind", false);

      EFPTextBox efpActionName = new EFPTextBox(page.BaseProvider, edActionName);
      efpActionName.CanBeEmpty = false;
      DocValueTextBox dvActionName = args.AddText(efpActionName, "ActionName", false);
      dvActionName.UserEnabledEx = new DepEqual<int>(efpKind.SelectedIndexEx, (int)(ActionKind.Other));
      dvActionName.UserDisabledMode = DocValueUserDisabledMode.KeepOriginalIfGrayed;

      EFPDocComboBox efpRemedy = new EFPDocComboBox(page.BaseProvider, cbRemedy, ProgramDBUI.TheUI.DocTypes["Remedies"]);
      efpRemedy.CanBeEmpty = false;
      DocValueDocComboBox dvRemedy = args.AddRef(efpRemedy, "Remedy", false);
      dvRemedy.UserEnabledEx = new DepEqual<int>(efpKind.SelectedIndexEx, (int)(ActionKind.Treatment));
      dvRemedy.UserDisabledMode = DocValueUserDisabledMode.KeepOriginalIfGrayed;

      EFPSubDocComboBox efpRemedyUsage = new EFPSubDocComboBox(efpRemedy, cbRemedyUsage, "RemedyUsage");
      efpRemedyUsage.CanBeEmpty = true;
      DocValueSubDocComboBox dvRemedyUsage = args.AddRef(efpRemedyUsage, "RemedyUsage", false);
      dvRemedyUsage.UserEnabledEx = dvRemedy.UserEnabledEx;
      dvRemedyUsage.UserDisabledMode = DocValueUserDisabledMode.KeepOriginalIfGrayed;

      EFPDocComboBox efpSoil = new EFPDocComboBox(page.BaseProvider, cbSoil, ProgramDBUI.TheUI.DocTypes["Soils"]);
      efpSoil.CanBeEmpty = true;
      DocValueDocComboBox dvSoil = args.AddRef(efpSoil, "Soil", false);
      dvSoil.UserEnabledEx = new DepInArray<int>(efpKind.SelectedIndexEx, PlantTools.GetSoilAppliableIntArray());
      dvSoil.UserDisabledMode = DocValueUserDisabledMode.KeepOriginalIfGrayed;

      EFPDocComboBox efpPotKind = new EFPDocComboBox(page.BaseProvider, cbPotKind, ProgramDBUI.TheUI.DocTypes["PotKinds"]);
      efpPotKind.CanBeEmpty = true;
      DocValueDocComboBox dvPotKind = args.AddRef(efpPotKind, "PotKind", false);
      dvPotKind.UserEnabledEx = new DepInArray<int>(efpKind.SelectedIndexEx, PlantTools.GetPotKindAppliableIntArray());
      dvPotKind.UserDisabledMode = DocValueUserDisabledMode.KeepOriginalIfGrayed;

      EFPDateOrRangeBox efpDate = new EFPDateOrRangeBox(page.BaseProvider, cbDate);
      efpDate.CanBeEmpty = false;
      args.AddDate(efpDate, "Date1", "Date2", false);

      EFPTextBox efpComment = new EFPTextBox(page.BaseProvider, edComment);
      efpComment.CanBeEmpty = true;
      args.AddText(efpComment, "Comment", true);
    }

    #endregion
  }
}
