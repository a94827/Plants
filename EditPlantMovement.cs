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
using FreeLibSet.UICore;

namespace Plants
{
  internal partial class EditPlantMovement : Form
  {
    #region Конструктор формы

    public EditPlantMovement()
    {
      InitializeComponent();
    }

    #endregion

    #region Табличный просмотр

    public static void ImageValueNeeded(object sender, DBxImageValueNeededEventArgs args)
    {
      MovementKind kind = (MovementKind)(args.GetInt("Kind"));
      args.ImageKey = PlantTools.GetMovementImageKey(kind);
    }

    #endregion

    #region Редактор

    public static void InitEditForm(object sender, InitSubDocEditFormEventArgs args)
    {
      EditPlantMovement form = new EditPlantMovement();
      form.AddPage1(args);
    }

    private EFPDocComboBox efpContra, efpForkPlant;

    private void AddPage1(InitSubDocEditFormEventArgs args)
    {
      DocEditPage page = args.AddPage("Общие", MainPanel1);
      page.ImageKey = args.Editor.SubDocTypeUI.ImageKey;

      cbKind.Items.AddRange(PlantTools.MovementNames);
      EFPListComboBox efpKind = new EFPListComboBox(page.BaseProvider, cbKind);
      new ListControlImagePainter(cbKind, PlantTools.MovementImageKeys);
      args.AddInt(efpKind, "Kind", false);

      EFPDocComboBox efpPlace = new EFPDocComboBox(page.BaseProvider, cbPlace, ProgramDBUI.TheUI.DocTypes["Places"]);
      efpPlace.CanBeEmpty = false;
      DocValueDocComboBox dvPlace = args.AddRef(efpPlace, "Place", false);
      dvPlace.UserEnabledEx = new DepInArray<int>(efpKind.SelectedIndexEx, new int[]{
        (int)MovementKind.Add, (int)MovementKind.Move});
      dvPlace.UserDisabledMode = DocValueUserDisabledMode.KeepOriginalIfGrayed;

      efpContra = new EFPDocComboBox(page.BaseProvider, cbContra, ProgramDBUI.TheUI.DocTypes["Contras"]);
      efpContra.CanBeEmpty = true;
      DocValueDocComboBox dvContra = args.AddRef(efpContra, "Contra", false);
      dvContra.UserEnabledEx = new DepInArray<int>(efpKind.SelectedIndexEx, new int[]{
        (int)MovementKind.Add, (int)MovementKind.Remove});
      dvContra.UserDisabledMode = DocValueUserDisabledMode.KeepOriginalIfGrayed;

      efpForkPlant = new EFPDocComboBox(page.BaseProvider, cbForkPlant, ProgramDBUI.TheUI.DocTypes["Plants"]);
      efpForkPlant.CanBeEmpty = true;
      DocValueDocComboBox dvForkPlant = args.AddRef(efpForkPlant, "ForkPlant", false);
      dvForkPlant.UserEnabledEx = dvContra.UserEnabledEx;
      dvForkPlant.UserDisabledMode = DocValueUserDisabledMode.KeepOriginalIfGrayed;

      efpContra.Validating += new UIValidatingEventHandler(efpContraAndForkPlant_Validating);
      efpForkPlant.Validating += new UIValidatingEventHandler(efpContraAndForkPlant_Validating);
      efpContra.DocIdEx.ValueChanged += new EventHandler(efpForkPlant.Validate);
      efpForkPlant.DocIdEx.ValueChanged += new EventHandler(efpContra.Validate);

      EFPDocComboBox efpSoil = new EFPDocComboBox(page.BaseProvider, cbSoil, ProgramDBUI.TheUI.DocTypes["Soils"]);
      efpSoil.CanBeEmpty = true;
      DocValueDocComboBox dvSoil = args.AddRef(efpSoil, "Soil", false);
      dvSoil.UserEnabledEx = new DepEqual<int>(efpKind.SelectedIndexEx, (int)MovementKind.Add);
      dvSoil.UserDisabledMode = DocValueUserDisabledMode.KeepOriginalIfGrayed;

      EFPDocComboBox efpPotKind = new EFPDocComboBox(page.BaseProvider, cbPotKind, ProgramDBUI.TheUI.DocTypes["PotKinds"]);
      efpPotKind.CanBeEmpty = true;
      DocValueDocComboBox dvPotKind = args.AddRef(efpPotKind, "PotKind", false);
      dvPotKind.UserEnabledEx = new DepEqual<int>(efpKind.SelectedIndexEx, (int)MovementKind.Add);
      dvPotKind.UserDisabledMode = DocValueUserDisabledMode.KeepOriginalIfGrayed;

      EFPDateOrRangeBox efpDate = new EFPDateOrRangeBox(page.BaseProvider, cbDate);
      efpDate.CanBeEmpty = false;
      args.AddDate(efpDate, "Date1", "Date2", false);

      EFPTextBox efpComment = new EFPTextBox(page.BaseProvider, edComment);
      efpComment.CanBeEmpty = true;
      args.AddText(efpComment, "Comment", true);
    }

    void efpContraAndForkPlant_Validating(object sender, UIValidatingEventArgs args)
    {
      if (args.ValidateState == UIValidateState.Error)
        return;
      if (efpContra.DocId != 0 && efpForkPlant.DocId != 0)
        args.SetError("Нельзя одновременно заполнять поля \"От кого / кому\" и \"Отсажено / подсажено\"");
    }

    #endregion
  }
}