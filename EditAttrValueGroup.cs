using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FreeLibSet.Forms;
using FreeLibSet.Forms.Docs;
using FreeLibSet.Data.Docs;
using FreeLibSet.Core;
using FreeLibSet.UICore;

namespace Plants
{
  /// <summary>
  /// Форма группового редактирования значения однотиных атрибутов для нескольких документов
  /// </summary>
  internal partial class EditAttrValueGroup : Form
  {
    #region Конструктор форм

    public EditAttrValueGroup()
    {
      InitializeComponent();
      Icon = EFPApp.MainImageIcon("Edit");
    }


    public void InitForm(DBxMultiDocs mDocs)
    {
      EFPFormProvider efpForm = new EFPFormProvider(this);

      efpDate = new EFPDateTimeBox(efpForm, edDate);
      efpDate.ToolTipText = "Дата, с которой начинает действовать значение атрибута";
      efpDate.CanBeEmptyMode = UIValidateState.Warning;


      //efpAttrType = new EFPAttrTypeComboBox(efpForm, cbAttrType, mDocs.DocType.Name /* Нельзя ограничиваться только действительными атрибутами*/);
      efpAttrType = new EFPDocComboBox(efpForm, cbAttrType, ProgramDBUI.TheUI.DocTypes["AttrTypes"]); // можно
      efpAttrType.CanBeEmpty = false;

      efpAction = new EFPRadioButtons(efpForm, rbEdit);
      efpAction.SelectedIndexEx.ValueChanged += new EventHandler(AttrTypeChanged);

      efpValue = new EFPAttrValueComboBox(efpForm, edValue);
      // 27.03.2017 Ограничение убрано efpValue.MaxLength = Accoo2Tools.AttrValueMaxLength;
      efpValue.ControlLabelText = true;

      efpAttrType.DocIdEx.ValueChanged += new EventHandler(AttrTypeChanged);
    }

    protected override void OnLoad(EventArgs args)
    {
      base.OnLoad(args);

      base.ActiveControl = efpValue.Control;
    }

    #endregion

    #region Поля

    DocTypeUI _DocTypeUI;

    EFPDateTimeBox efpDate;
    EFPDocComboBox efpAttrType;
    EFPRadioButtons efpAction;

    EFPAttrValueComboBox efpValue;

    string _SubDocTypeName { get { return "PlantAttributes"; } }

    SubDocTypeUI _SubDocType { get { return _DocTypeUI.SubDocTypes[_SubDocTypeName]; } }

    #endregion

    #region Обработчики

    private void AttrTypeChanged(object sender, EventArgs args)
    {
      efpValue.AttrTypeId = efpAttrType.DocId;
      efpValue.Enabled = efpAction.SelectedIndex == 0;
    }

    #endregion

    #region Статический метод

    private static int _LastAction = 0;

    public static bool PerformEdit(DocTypeUI docTypeUI, Int32[] docIds, Int32 attrTypeId)
    {
      if (docIds.Length == 0)
      {
        EFPApp.ErrorMessageBox("Документы не выбраны");
        return false;
      }

      bool res = false;
      try
      {
        DBxDocSet docSet = new DBxDocSet(ProgramDBUI.TheUI.DocProvider);
        DBxMultiDocs mDocs = docSet[docTypeUI.DocType.Name];
        mDocs.Edit(docIds);
        // Держим блокировку на все время показа диалога
        Guid lockGuid = docSet.AddLongLock();
        try
        {
          EditAttrValueGroup form = new EditAttrValueGroup();
          form._DocTypeUI = docTypeUI;
          form.InitForm(mDocs);

          #region Группа "Выбранные документы"

          form.lblDocTypeName.ImageList = EFPApp.MainImages;
          form.lblDocTypeName.ImageAlign = ContentAlignment.MiddleRight;
          if (docIds.Length == 1)
          {
            form.lblDocTypeName.Text = docTypeUI.DocType.SingularTitle;
            form.lblDocTypeName.Image = docTypeUI.GetImageValue(docIds[0]);
            form.lblDocInfo.Text = docTypeUI.GetTextValue(docIds[0]);
          }
          else
          {
            form.lblDocTypeName.Text = docTypeUI.DocType.PluralTitle;
            form.lblDocTypeName.ImageKey = docTypeUI.ImageKey;
            form.lblDocInfo.Text = docIds.Length.ToString() + " документа(ов)";
          }

          #endregion

          form.efpAttrType.DocId = attrTypeId;
          form.efpDate.NValue = EditAttrValueHelper.LastDate;
          form.efpAction.SelectedIndex = _LastAction;

          if (EFPApp.ShowDialog(form, true) == DialogResult.OK)
          {
            EditAttrValueHelper.LastDate = form.efpDate.Value;
            _LastAction = form.efpAction.SelectedIndex;

            if (form.efpAction.SelectedIndex == 0)
            {
              docSet.ActionInfo = "Установка значений атрибута \"" + DataTools.GetString(form.efpAttrType.GetColumnValue("Name")) + "\"";
              string sValue = PlantTools.ValueToSaveableString(form.efpValue.Value, form.efpValue.ValueType);
              int nAdded, nChanged;
              DoSetValue(mDocs, form.efpAttrType.DocId, sValue, form.efpDate.NValue, out nAdded, out nChanged);
              if (nAdded == 0 && nChanged == 0)
                EFPApp.MessageBox("Атрибуты уже имеют требуемое значени. Никаких действий не выполняется",
                  "Установка значений атрибутов", MessageBoxButtons.OK, MessageBoxIcon.Information);
              else
              {
                if (EFPApp.MessageBox("Будет добавлено значений атрибутов: " + nAdded.ToString() + ", изменено: " + nChanged.ToString(),
                  "Установка значений атрибутов", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                  res = true;
              }
            }
            else
            {
              docSet.ActionInfo = "Удаление значений атрибута \"" + DataTools.GetString(form.efpAttrType.GetColumnValue("Name")) + "\"";
              int nDel;
              DoDelValue(mDocs, form.efpAttrType.DocId, form.efpDate.Value, out nDel);
              if (nDel == 0)
                EFPApp.MessageBox("Не найдено ни одного значения атрибута для удаления. Никаких действий не выполняется",
                  "Удаление значений атрибутов", MessageBoxButtons.OK, MessageBoxIcon.Information);
              else
              {
                if (EFPApp.MessageBox("Будет удалено значений атрибутов: " + nDel.ToString(),
                  "Удаление значений атрибутов", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                  res = true;
              }
            }

            if (res)
              docSet.ApplyChanges(false);
          }
        }
        finally
        {
          docSet.RemoveLongLock(lockGuid);
        }
      }
      catch (Exception e)
      {
        EFPApp.ShowException(e, "Ошибка групповой установки значений атрибутов");
      }
      return res;
    }

    #endregion

    #region Модификация набора

    private static void DoSetValue(DBxMultiDocs mDocs, Int32 attrTypeId, string sValue, DateTime? date, out int nAdded, out int nChanged)
    {
      nAdded = 0;
      nChanged = 0;

      DBxMultiSubDocs subDocs = mDocs.SubDocs["PlantAttributes"];
      DataTable table = subDocs.CreateSubDocsData(); // копия таблицы для поиска, менять надо поддокументы
      table.DefaultView.RowFilter = "AttrType=" + attrTypeId.ToString();
      table.DefaultView.Sort = "DocId,Date";

      foreach (DBxSingleDoc doc in mDocs)
      {
        object[] keys = new object[2];
        keys[0] = doc.DocId;
        if (date.HasValue)
          keys[1] = date.Value;
        else
          keys[1] = DBNull.Value;

        string sValue1, sValue2;
        EditAttrValueHelper.SplitAttrValue(sValue, out sValue1, out sValue2);

        int p = table.DefaultView.Find(keys);
        if (p >= 0)
        {
          // Заменяем поддокумент

          Int32 subDocId = DataTools.GetInt(table.DefaultView[p].Row, "Id");
          DBxSubDoc subDoc = subDocs.GetSubDocById(subDocId);
          if (subDoc.Values["Value"].AsString != sValue1 || subDoc.Values["LongValue"].AsString != sValue2)
          {
            subDoc.Values["Value"].SetString(sValue1);
            subDoc.Values["LongValue"].SetString(sValue2);
            nChanged++;
          }
        }
        else
        {
          // Добавляем поддокумент

          DBxSubDoc subDoc = doc.SubDocs[subDocs.SubDocType.Name].Insert();
          subDoc.Values["AttrType"].SetInteger(attrTypeId);
          subDoc.Values["Date"].SetNullableDateTime(date);
          subDoc.Values["Value"].SetString(sValue1);
          subDoc.Values["LongValue"].SetString(sValue2);
          nAdded++;
        }
      }
    }

    private static void DoDelValue(DBxMultiDocs mDocs, Int32 attrTypeId, DateTime? date, out int nDel)
    {
      nDel = 0;
      DBxMultiSubDocs subDocs = mDocs.SubDocs["PlantAttributes"];
      for (int i = subDocs.SubDocCount - 1; i >= 0; i--)
      {
        if (subDocs[i].Values["AttrType"].AsInteger == attrTypeId &&
          subDocs[i].Values["Date"].AsNullableDateTime == date)
        {
          subDocs[i].Delete();
          nDel++;
        }
      }
    }

    #endregion
  }
}