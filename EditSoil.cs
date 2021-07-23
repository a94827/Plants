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
  internal partial class EditSoil : Form
  {
    #region Конструктор формы

    public EditSoil()
    {
      InitializeComponent();
    }

    #endregion

    #region Табличный просмотр

    public static void InitView(object sender, InitEFPDBxViewEventArgs args)
    {
      #region Фильтры

      RefDocGridFilter FiltManufacturer = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["Companies"], "Manufacturer");
      FiltManufacturer.DisplayName = "Изготовитель";
      FiltManufacturer.Nullable = true;
      args.ControlProvider.Filters.Add(FiltManufacturer);

      #endregion
    }

    public static void pHcolumnValueNeeded(object sender, EFPGridProducerValueNeededEventArgs args)
    {
      float pHmin = args.GetSingle("pHmin");
      float pHmax = args.GetSingle("pHmax");
      if (pHmin == 0f || pHmax == 0f)
        return;
      if (pHmax == pHmin)
        args.Value = pHmin.ToString("0.0");
      else
        args.Value = pHmin.ToString("0.0") + "-" + pHmax.ToString("0.0");
    }

    #endregion

    #region Редактор

    #region InitDocEditForm

    DocumentEditor _Editor;

    public static void InitDocEditForm(object Sender, InitDocEditFormEventArgs Args)
    {
      EditSoil Form = new EditSoil();

      Form._Editor = Args.Editor;

      Form.AddPage1(Args);
      EFPSubDocGridView sdgParts;
      DocEditPage Page2 = Args.AddSubDocsPage("SoilParts", out sdgParts);
      sdgParts.ManualOrderColumn = "Order";
      Page2.Title = "Состав";
    }

    #endregion

    #region Страница 1 (общие)

    private EFPTextBox efpName;
    private EFPNumEditBox efppHmin, efppHmax;

    private void AddPage1(InitDocEditFormEventArgs Args)
    {
      DocEditPage Page = Args.AddPage("Общие", MainPanel1);
      Page.ImageKey = "Remedy";

      efpName = new EFPTextBox(Page.BaseProvider, edName);
      efpName.CanBeEmpty = false;
      Args.AddText(efpName, "Name", false);

      efppHmin = new EFPNumEditBox(Page.BaseProvider, edpHmin);
      efppHmin.Minimum = 0;
      efppHmin.Maximum = 13;
      Args.AddSingle(efppHmin, "pHmin", false);

      efppHmax = new EFPNumEditBox(Page.BaseProvider, edpHmax);
      efppHmax.Minimum = 0;
      efppHmax.Maximum = 13;
      Args.AddSingle(efppHmax, "pHmax", false);
      efppHmin.Validating += new EFPValidatingEventHandler(efppH_Validating);
      efppHmax.Validating += new EFPValidatingEventHandler(efppH_Validating);
      efppHmin.SingleValueEx.ValueChanged += new EventHandler(efppHmax.Validate);
      efppHmax.SingleValueEx.ValueChanged += new EventHandler(efppHmin.Validate);

      EFPDocComboBox efpManufacturer = new EFPDocComboBox(Page.BaseProvider, cbManufacturer, ProgramDBUI.TheUI.DocTypes["Companies"]);
      efpManufacturer.CanBeEmpty = true;
      Args.AddRef(efpManufacturer, "Manufacturer", true);

      EFPDocComboBox efpGroup = new EFPDocComboBox(Page.BaseProvider, cbGroup, ProgramDBUI.TheUI.DocTypes["SoilGroups"]);
      efpGroup.CanBeEmpty = true;
      Args.AddRef(efpGroup, "GroupId", true);

      #region Комментарий

      EFPTextBox efpComment = new EFPTextBox(Page.BaseProvider, edComment);
      efpComment.CanBeEmpty = true;
      Args.AddText(efpComment, "Comment", true);

      #endregion
    }

    void efppH_Validating(object Sender, EFPValidatingEventArgs Args)
    {
      if (Args.ValidateState == EFPValidateState.Error)
        return;

      EFPNumEditBox efp = (EFPNumEditBox)Sender;
      if (efp.SingleValue != 0f && efp.SingleValue < 1f)
      {
        Args.SetError("Допускаются значения pH от 1 до 13. 0 означает отсутствие данных");
        return;
      }

      if (efppHmin.SingleValue == 0f && efppHmax.SingleValue == 0f)
        return;
      if (efppHmin.SingleValue == 0f || efppHmax.SingleValue == 0f)
      {
        Args.SetError("Должно быть задано и минимально и максимальное значение");
        return;
      }

      if (efppHmin.SingleValue > efppHmax.SingleValue)
        Args.SetError("Неправильный диапазон");
    }

    #endregion

    #endregion
  }
}