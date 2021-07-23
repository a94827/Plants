using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AgeyevAV.ExtForms;
using AgeyevAV.ExtForms.Docs;
using AgeyevAV.DependedValues;
using AgeyevAV;
using AgeyevAV.ExtDB;
using AgeyevAV.ExtDB.Docs;

namespace Plants
{
  internal partial class EditPotKind : Form
  {
    #region Конструктор формы

    public EditPotKind()
    {
      InitializeComponent();
    }

    #endregion

    #region Табличный просмотр

    public static void InitView(object Sender, InitEFPDBxViewEventArgs Args)
    {
      #region Фильтры

      RefDocGridFilter FiltManufacturer = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["Companies"], "Manufacturer");
      FiltManufacturer.DisplayName = "Изготовитель";
      FiltManufacturer.Nullable = true;
      Args.ControlProvider.Filters.Add(FiltManufacturer);

      #endregion
    }

    #endregion

    #region Редактор

    #region InitDocEditForm

    DocumentEditor _Editor;

    public static void InitDocEditForm(object Sender, InitDocEditFormEventArgs Args)
    {
      EditPotKind Form = new EditPotKind();

      Form._Editor = Args.Editor;

      Form.AddPage1(Args);
    }

    #endregion

    #region Страница 1 (общие)

    private EFPTextBox efpText, efpColor;

    private EFPNumEditBox efpD, efpH;

    private EFPNumEditBox efpVolume;

    private void AddPage1(InitDocEditFormEventArgs Args)
    {
      DocEditPage Page = Args.AddPage("Общие", MainPanel1);
      Page.ImageKey = "PotKind";

      efpText = new EFPTextBox(Page.BaseProvider, edText);
      efpText.CanBeEmpty = true;
      Args.AddText(efpText, "Text", true);

      efpD = new EFPNumEditBox(Page.BaseProvider, edD);
      Args.AddInt(efpD, "Diameter", false);

      efpH = new EFPNumEditBox(Page.BaseProvider, edH);
      Args.AddInt(efpH, "Height", false);

      efpVolume = new EFPNumEditBox(Page.BaseProvider, edVolume);
      Args.AddSingle(efpVolume, "Volume", false);

      efpColor = new EFPTextBox(Page.BaseProvider, edColor);
      efpColor.CanBeEmpty = true;
      Args.AddText(efpColor, "Color", true);

      EFPDocComboBox efpManufacturer = new EFPDocComboBox(Page.BaseProvider, cbManufacturer, ProgramDBUI.TheUI.DocTypes["Companies"]);
      efpManufacturer.CanBeEmpty = true;
      Args.AddRef(efpManufacturer, "Manufacturer", true);

      EFPDocComboBox efpGroup = new EFPDocComboBox(Page.BaseProvider, cbGroup, ProgramDBUI.TheUI.DocTypes["PotKindGroups"]);
      efpGroup.CanBeEmpty = true;
      Args.AddRef(efpGroup, "GroupId", true);

      #region Комментарий

      EFPTextBox efpComment = new EFPTextBox(Page.BaseProvider, edComment);
      efpComment.CanBeEmpty = true;
      Args.AddText(efpComment, "Comment", true);

      #endregion
    }

    #endregion

    #endregion
  }
}