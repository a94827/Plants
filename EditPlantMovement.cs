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

    public static void ImageValueNeeded(object Sender, DBxImageValueNeededEventArgs Args)
    {
      MovementKind Kind = (MovementKind)(Args.GetInt("Kind"));
      Args.ImageKey = PlantTools.GetMovementImageKey(Kind);
    }

    #endregion

    #region Редактор

    public static void InitEditForm(object Sender, InitSubDocEditFormEventArgs Args)
    {
      EditPlantMovement Form = new EditPlantMovement();
      Form.AddPage1(Args);
    }

    private EFPDocComboBox efpContra, efpForkPlant;

    private void AddPage1(InitSubDocEditFormEventArgs Args)
    {
      DocEditPage Page = Args.AddPage("Общие", MainPanel1);
      Page.ImageKey = Args.Editor.SubDocTypeUI.ImageKey;

      cbKind.Items.AddRange(PlantTools.MovementNames);
      EFPListComboBox efpKind = new EFPListComboBox(Page.BaseProvider, cbKind);
      new ListControlImagePainter(cbKind, PlantTools.MovementImageKeys);
      Args.AddInt(efpKind, "Kind", false);

      EFPDocComboBox efpPlace = new EFPDocComboBox(Page.BaseProvider, cbPlace, ProgramDBUI.TheUI.DocTypes["Places"]);
      efpPlace.CanBeEmpty = false;
      DocValueDocComboBox dvPlace = Args.AddRef(efpPlace, "Place", false);
      dvPlace.UserEnabledEx = new DepInArray<int>(efpKind.SelectedIndexEx, new int[]{
        (int)MovementKind.Add, (int)MovementKind.Move});
      dvPlace.UserDisabledMode = DocValueUserDisabledMode.KeepOriginalIfGrayed;

      efpContra = new EFPDocComboBox(Page.BaseProvider, cbContra, ProgramDBUI.TheUI.DocTypes["Contras"]);
      efpContra.CanBeEmpty = true;
      DocValueDocComboBox dvContra = Args.AddRef(efpContra, "Contra", false);
      dvContra.UserEnabledEx = new DepInArray<int>(efpKind.SelectedIndexEx, new int[]{
        (int)MovementKind.Add, (int)MovementKind.Remove});
      dvContra.UserDisabledMode = DocValueUserDisabledMode.KeepOriginalIfGrayed;

      efpForkPlant = new EFPDocComboBox(Page.BaseProvider, cbForkPlant, ProgramDBUI.TheUI.DocTypes["Plants"]);
      efpForkPlant.CanBeEmpty = true;
      DocValueDocComboBox dvForkPlant = Args.AddRef(efpForkPlant, "ForkPlant", false);
      dvForkPlant.UserEnabledEx = dvContra.UserEnabledEx;
      dvForkPlant.UserDisabledMode = DocValueUserDisabledMode.KeepOriginalIfGrayed;

      efpContra.Validating += new EFPValidatingEventHandler(efpContraAndForkPlant_Validating);
      efpForkPlant.Validating += new EFPValidatingEventHandler(efpContraAndForkPlant_Validating);
      efpContra.DocIdEx.ValueChanged+=new EventHandler(efpForkPlant.Validate);
      efpForkPlant.DocIdEx.ValueChanged += new EventHandler(efpContra.Validate);

      EFPDocComboBox efpSoil = new EFPDocComboBox(Page.BaseProvider, cbSoil, ProgramDBUI.TheUI.DocTypes["Soils"]);
      efpSoil.CanBeEmpty = true;
      DocValueDocComboBox dvSoil = Args.AddRef(efpSoil, "Soil", false);
      dvSoil.UserEnabledEx = new DepEqual<int>(efpKind.SelectedIndexEx, (int)MovementKind.Add);
      dvSoil.UserDisabledMode = DocValueUserDisabledMode.KeepOriginalIfGrayed;

      EFPDocComboBox efpPotKind = new EFPDocComboBox(Page.BaseProvider, cbPotKind, ProgramDBUI.TheUI.DocTypes["PotKinds"]);
      efpPotKind.CanBeEmpty = true;
      DocValueDocComboBox dvPotKind = Args.AddRef(efpPotKind, "PotKind", false);
      dvPotKind.UserEnabledEx = new DepEqual<int>(efpKind.SelectedIndexEx, (int)MovementKind.Add);
      dvPotKind.UserDisabledMode = DocValueUserDisabledMode.KeepOriginalIfGrayed;

      EFPDateOrRangeBox efpDate = new EFPDateOrRangeBox(Page.BaseProvider, cbDate);
      efpDate.CanBeEmpty = false;
      Args.AddDate(efpDate, "Date1", "Date2", false);

      EFPTextBox efpComment = new EFPTextBox(Page.BaseProvider, edComment);
      efpComment.CanBeEmpty = true;
      Args.AddText(efpComment, "Comment", true);
    }

    void efpContraAndForkPlant_Validating(object Sender, EFPValidatingEventArgs Args)
    {
      if (Args.ValidateState == EFPValidateState.Error)
        return;
      if (efpContra.DocId != 0 && efpForkPlant.DocId != 0)
        Args.SetError("Нельзя одновременно заполнять поля \"От кого / кому\" и \"Отсажено / подсажено\"");
    }

    #endregion
  }
}