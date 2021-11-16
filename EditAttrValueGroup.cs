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

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

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

    private void AttrTypeChanged(object Sender, EventArgs Args)
    {
      efpValue.AttrTypeId = efpAttrType.DocId;
      efpValue.Enabled = efpAction.SelectedIndex == 0;
    }

    #endregion

    #region Статический метод

    private static int LastAction = 0;

    public static bool PerformEdit(DocTypeUI DocTypeUI, Int32[] DocIds, Int32 AttrTypeId)
    {
      if (DocIds.Length == 0)
      {
        EFPApp.ErrorMessageBox("Документы не выбраны");
        return false;
      }

      bool Res = false;
      try
      {
        DBxDocSet DocSet = new DBxDocSet(ProgramDBUI.TheUI.DocProvider);
        DBxMultiDocs mDocs = DocSet[DocTypeUI.DocType.Name];
        mDocs.Edit(DocIds);
        // Держим блокировку на все время показа диалога
        Guid LockGuid = DocSet.AddLongLock();
        try
        {
          EditAttrValueGroup Form = new EditAttrValueGroup();
          Form._DocTypeUI = DocTypeUI;
          Form.InitForm(mDocs);

          #region Группа "Выбранные документы"

          Form.lblDocTypeName.ImageList = EFPApp.MainImages;
          Form.lblDocTypeName.ImageAlign = ContentAlignment.MiddleRight;
          if (DocIds.Length == 1)
          {
            Form.lblDocTypeName.Text = DocTypeUI.DocType.SingularTitle;
            Form.lblDocTypeName.Image = DocTypeUI.GetImageValue(DocIds[0]);
            Form.lblDocInfo.Text = DocTypeUI.GetTextValue(DocIds[0]);
          }
          else
          {
            Form.lblDocTypeName.Text = DocTypeUI.DocType.PluralTitle;
            Form.lblDocTypeName.ImageKey = DocTypeUI.ImageKey;
            Form.lblDocInfo.Text = DocIds.Length.ToString() + " документа(ов)";
          }

          #endregion

          Form.efpAttrType.DocId = AttrTypeId;
          Form.efpDate.NValue = EditAttrValueHelper.LastDate;
          Form.efpAction.SelectedIndex = LastAction;

          if (EFPApp.ShowDialog(Form, true) == DialogResult.OK)
          {
            EditAttrValueHelper.LastDate = Form.efpDate.Value;
            LastAction = Form.efpAction.SelectedIndex;

            if (Form.efpAction.SelectedIndex == 0)
            {
              DocSet.ActionInfo = "Установка значений атрибута \"" + DataTools.GetString(Form.efpAttrType.GetColumnValue("Name")) + "\"";
              string sValue = PlantTools.ValueToSaveableString(Form.efpValue.Value, Form.efpValue.ValueType);
              int nAdded, nChanged;
              DoSetValue(mDocs, Form.efpAttrType.DocId, sValue, Form.efpDate.NValue, out nAdded, out nChanged);
              if (nAdded == 0 && nChanged == 0)
                EFPApp.MessageBox("Атрибуты уже имеют требуемое значени. Никаких действий не выполняется",
                  "Установка значений атрибутов", MessageBoxButtons.OK, MessageBoxIcon.Information);
              else
              {
                if (EFPApp.MessageBox("Будет добавлено значений атрибутов: " + nAdded.ToString() + ", изменено: " + nChanged.ToString(),
                  "Установка значений атрибутов", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                  Res = true;
              }
            }
            else
            {
              DocSet.ActionInfo = "Удаление значений атрибута \"" + DataTools.GetString(Form.efpAttrType.GetColumnValue("Name")) + "\"";
              int nDel;
              DoDelValue(mDocs, Form.efpAttrType.DocId, Form.efpDate.Value, out nDel);
              if (nDel == 0)
                EFPApp.MessageBox("Не найдено ни одного значения атрибута для удаления. Никаких действий не выполняется",
                  "Удаление значений атрибутов", MessageBoxButtons.OK, MessageBoxIcon.Information);
              else
              {
                if (EFPApp.MessageBox("Будет удалено значений атрибутов: " + nDel.ToString(),
                  "Удаление значений атрибутов", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                  Res = true;
              }
            }

            if (Res)
              DocSet.ApplyChanges(false);
          }
        }
        finally
        {
          DocSet.RemoveLongLock(LockGuid);
        }
      }
      catch (Exception e)
      {
        EFPApp.ShowException(e, "Ошибка групповой установки значений атрибутов");
      }
      return Res;
    }

    #endregion

    #region Модификация набора

    private static void DoSetValue(DBxMultiDocs mDocs, Int32 AttrTypeId, string sValue, DateTime? Date, out int nAdded, out int nChanged)
    {
      nAdded = 0;
      nChanged = 0;

      DBxMultiSubDocs SubDocs = mDocs.SubDocs["PlantAttributes"];
      DataTable Table = SubDocs.CreateSubDocsData(); // копия таблицы для поиска, менять надо поддокументы
      Table.DefaultView.RowFilter = "AttrType=" + AttrTypeId.ToString();
      Table.DefaultView.Sort = "DocId,Date";

      foreach (DBxSingleDoc Doc in mDocs)
      {
        object[] Keys = new object[2];
        Keys[0] = Doc.DocId;
        if (Date.HasValue)
          Keys[1] = Date.Value;
        else
          Keys[1] = DBNull.Value;

        string sValue1, sValue2;
        EditAttrValueHelper.SplitAttrValue(sValue, out sValue1, out sValue2);

        int p = Table.DefaultView.Find(Keys);
        if (p >= 0)
        {
          // Заменяем поддокумент

          Int32 SubDocId = DataTools.GetInt(Table.DefaultView[p].Row, "Id");
          DBxSubDoc SubDoc = SubDocs.GetSubDocById(SubDocId);
          if (SubDoc.Values["Value"].AsString != sValue1 || SubDoc.Values["LongValue"].AsString != sValue2)
          {
            SubDoc.Values["Value"].SetString(sValue1);
            SubDoc.Values["LongValue"].SetString(sValue2);
            nChanged++;
          }
        }
        else
        {
          // Добавляем поддокумент

          DBxSubDoc SubDoc = Doc.SubDocs[SubDocs.SubDocType.Name].Insert();
          SubDoc.Values["AttrType"].SetInteger(AttrTypeId);
          SubDoc.Values["Date"].SetNullableDateTime(Date);
          SubDoc.Values["Value"].SetString(sValue1);
          SubDoc.Values["LongValue"].SetString(sValue2);
          nAdded++;
        }
      }
    }

    private static void DoDelValue(DBxMultiDocs mDocs, Int32 AttrTypeId, DateTime? Date, out int nDel)
    {
      nDel = 0;
      DBxMultiSubDocs SubDocs = mDocs.SubDocs["PlantAttributes"];
      for (int i = SubDocs.SubDocCount - 1; i >= 0; i--)
      {
        if (SubDocs[i].Values["AttrType"].AsInteger == AttrTypeId &&
          SubDocs[i].Values["Date"].AsNullableDateTime == Date)
        {
          SubDocs[i].Delete();
          nDel++;
        }
      }
    }

    #endregion
  }
}