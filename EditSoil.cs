using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FreeLibSet.Forms;
using FreeLibSet.Forms.Docs;
using FreeLibSet.DependedValues;
using FreeLibSet.Data;
using FreeLibSet.Data.Docs;
using FreeLibSet.UICore;

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

      RefDocGridFilter filtManufacturer = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["Companies"], "Manufacturer");
      filtManufacturer.DisplayName = "Изготовитель";
      filtManufacturer.Nullable = true;
      args.ControlProvider.Filters.Add(filtManufacturer);

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

    public static void InitDocEditForm(object sender, InitDocEditFormEventArgs args)
    {
      EditSoil form = new EditSoil();
      form._Editor = args.Editor;
      form.AddPage1(args);

      EFPSubDocGridView sdgParts;
      DocEditPage page2 = args.AddSubDocsPage("SoilParts", out sdgParts);
      sdgParts.ManualOrderColumn = "Order";
      page2.Title = "Состав";
    }

    #endregion

    #region Страница 1 (общие)

    private EFPTextBox efpName;
    private EFPSingleEditBox efppHmin, efppHmax;

    private void AddPage1(InitDocEditFormEventArgs args)
    {
      DocEditPage page = args.AddPage("Общие", MainPanel1);
      page.ImageKey = "Remedy";

      efpName = new EFPTextBox(page.BaseProvider, edName);
      efpName.CanBeEmpty = false;
      args.AddText(efpName, "Name", false);

      efppHmin = new EFPSingleEditBox(page.BaseProvider, edpHmin);
      efppHmin.Minimum = 0;
      efppHmin.Maximum = 13;
      args.AddSingle(efppHmin, "pHmin", false);

      efppHmax = new EFPSingleEditBox(page.BaseProvider, edpHmax);
      efppHmax.Minimum = 0;
      efppHmax.Maximum = 13;
      args.AddSingle(efppHmax, "pHmax", false);
      efppHmin.Validating += new UIValidatingEventHandler(efppH_Validating);
      efppHmax.Validating += new UIValidatingEventHandler(efppH_Validating);
      efppHmin.ValueEx.ValueChanged += new EventHandler(efppHmax.Validate);
      efppHmax.ValueEx.ValueChanged += new EventHandler(efppHmin.Validate);

      EFPDocComboBox efpManufacturer = new EFPDocComboBox(page.BaseProvider, cbManufacturer, ProgramDBUI.TheUI.DocTypes["Companies"]);
      efpManufacturer.CanBeEmpty = true;
      args.AddRef(efpManufacturer, "Manufacturer", true);

      EFPDocComboBox efpGroup = new EFPDocComboBox(page.BaseProvider, cbGroup, ProgramDBUI.TheUI.DocTypes["SoilGroups"]);
      efpGroup.CanBeEmpty = true;
      args.AddRef(efpGroup, "GroupId", true);

      #region Комментарий

      EFPTextBox efpComment = new EFPTextBox(page.BaseProvider, edComment);
      efpComment.CanBeEmpty = true;
      args.AddText(efpComment, "Comment", true);

      #endregion
    }

    void efppH_Validating(object sender, UIValidatingEventArgs args)
    {
      if (args.ValidateState == UIValidateState.Error)
        return;

      EFPSingleEditBox efp = (EFPSingleEditBox)sender;
      if (efp.Value != 0f && efp.Value < 1f)
      {
        args.SetError("Допускаются значения pH от 1 до 13. 0 означает отсутствие данных");
        return;
      }

      if (efppHmin.Value == 0f && efppHmax.Value == 0f)
        return;
      if (efppHmin.Value == 0f || efppHmax.Value == 0f)
      {
        args.SetError("Должно быть задано и минимально и максимальное значение");
        return;
      }

      if (efppHmin.Value > efppHmax.Value)
        args.SetError("Неправильный диапазон");
    }

    #endregion

    #endregion
  }
}