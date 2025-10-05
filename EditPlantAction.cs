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
using FreeLibSet.Forms.Data;

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
          Int32 remedyId = args.GetInt32("Remedy");
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
      ExtEditPage page = args.AddPage("Общие", MainPanel1);
      page.ImageKey = args.Editor.SubDocTypeUI.ImageKey;

      // Редактируются разные действия?
      bool isDiffKind = args.Values["Kind"].Grayed;

      cbKind.Items.AddRange(PlantTools.ActionNames);
      EFPListComboBox efpKind = new EFPListComboBox(page.BaseProvider, cbKind);
      new ListControlImagePainter(cbKind, PlantTools.ActionImageKeys);
      args.AddInt32(efpKind, "Kind", !isDiffKind);

      EFPTextBox efpActionName = new EFPTextBox(page.BaseProvider, edActionName);
      efpActionName.CanBeEmpty = false;
      ExtValueTextBox dvActionName = args.AddText(efpActionName, "ActionName", !isDiffKind);
      dvActionName.UserEnabledEx = new DepEqual<int>(efpKind.SelectedIndexEx, (int)(ActionKind.Other));
      dvActionName.UserDisabledMode = ExtValueUserDisabledMode.KeepOriginalIfGrayed;

      EFPDocComboBox efpRemedy = new EFPDocComboBox(page.BaseProvider, cbRemedy, ProgramDBUI.TheUI.DocTypes["Remedies"]);
      efpRemedy.CanBeEmpty = false;
      ExtValueDocComboBox dvRemedy = args.AddRef(efpRemedy, "Remedy", !isDiffKind);
      dvRemedy.UserEnabledEx = new DepEqual<int>(efpKind.SelectedIndexEx, (int)(ActionKind.Treatment));
      dvRemedy.UserDisabledMode = ExtValueUserDisabledMode.KeepOriginalIfGrayed;

      EFPSubDocComboBox efpRemedyUsage = new EFPSubDocComboBox(efpRemedy, cbRemedyUsage, "RemedyUsage");
      efpRemedyUsage.CanBeEmpty = true;
      ExtValueSubDocComboBox dvRemedyUsage = args.AddRef(efpRemedyUsage, "RemedyUsage", !isDiffKind);
      dvRemedyUsage.UserEnabledEx = dvRemedy.UserEnabledEx;
      dvRemedyUsage.UserDisabledMode = ExtValueUserDisabledMode.KeepOriginalIfGrayed;

      EFPDocComboBox efpSoil = new EFPDocComboBox(page.BaseProvider, cbSoil, ProgramDBUI.TheUI.DocTypes["Soils"]);
      efpSoil.CanBeEmpty = true;
      ExtValueDocComboBox dvSoil = args.AddRef(efpSoil, "Soil", !isDiffKind);
      dvSoil.UserEnabledEx = new DepInArray<int>(efpKind.SelectedIndexEx, PlantTools.GetSoilAppliableIntArray());
      dvSoil.UserDisabledMode = ExtValueUserDisabledMode.KeepOriginalIfGrayed;

      EFPDocComboBox efpPotKind = new EFPDocComboBox(page.BaseProvider, cbPotKind, ProgramDBUI.TheUI.DocTypes["PotKinds"]);
      efpPotKind.CanBeEmpty = true;
      ExtValueDocComboBox dvPotKind = args.AddRef(efpPotKind, "PotKind", !isDiffKind);
      dvPotKind.UserEnabledEx = new DepInArray<int>(efpKind.SelectedIndexEx, PlantTools.GetPotKindAppliableIntArray());
      dvPotKind.UserDisabledMode = ExtValueUserDisabledMode.KeepOriginalIfGrayed;

      EFPDateOrRangeBox efpDate = new EFPDateOrRangeBox(page.BaseProvider, cbDate);
      efpDate.CanBeEmpty = false;
      args.AddDate(efpDate, "Date1", "Date2", true);

      EFPTextBox efpComment = new EFPTextBox(page.BaseProvider, edComment);
      efpComment.CanBeEmpty = true;
      args.AddText(efpComment, "Comment", true);
    }

    #endregion
  }
}
