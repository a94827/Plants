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
using FreeLibSet.Data.Docs;
using FreeLibSet.Controls;
using FreeLibSet.UICore;

namespace Plants
{
  internal partial class EditAttrValue : Form
  {
    #region Конструктор формы

    public EditAttrValue()
    {
      InitializeComponent();
    }

    #endregion

    /*
     * В базе данных атрибуты хранятся как строки.
     * И в поле вводится строка.
     * Но они не совпадают, т.к. требуется преобразование для чисел с плавающей точкой и датой
     * 
     *                               Двойное преобразование
     * Поле в БД               <-->             Внутреннее значение           <-->        Поле ввода
     *       Accoo2DocAttributes.ValueFromSaveableString()   ->      Accoo2Convert.ToString()
     *                                                           или DisplayNamed.ValueToString()
     * 
     *       Accoo2DocAttributes.ValueToSaveableString()     <-      Accoo2Convert.Parse()
     *                                                           или Accoo2DBUI.ValueFromString()
     */

    #region Табличный просмотр

    public static void ValueColumnValueNeeded(object sender, EFPGridProducerValueNeededEventArgs args)
    {
      string s = args.GetString("Value");
      if (s.Length == 0)
        s = args.GetString("LongValue");
      Int32 attrTypeId = args.GetInt("AttrType");
      if (ProgramDBUI.TheUI.DocProvider.IsRealDocId(attrTypeId))
      {
        AttrTypeDoc attrType = new AttrTypeDoc(attrTypeId);
        object v = PlantTools.ValueFromSaveableString(s, attrType.ValueType);
        args.Value = PlantTools.ToString(v, attrType.ValueType);
      }
      else
        args.Value = s;
    }

    public static void ImageValueNeeded(object sender, DBxImageValueNeededEventArgs args)
    {
      Int32 attrTypeId = args.GetInt("AttrType");
      if (attrTypeId == 0)
        return; // не бывает

      AttrTypeDoc attrType = new AttrTypeDoc(attrTypeId);

      string s = args.GetString("Value");
      if (s.Length == 0)
        s = args.GetString("LongValue");

      object v = PlantTools.ValueFromSaveableString(s, attrType.ValueType);

      string errorText;
      if (!attrType.TestValue(v, out errorText))
      {
        args.ColorType = EFPDataGridViewColorType.Warning;
        args.ToolTipText = errorText;
      }
    }

    public static void GetDocSel(object sender, DocTypeDocSelEventArgs args)
    {
      args.AddFromColumn("AttrTypes", "AttrType");
    }

    #endregion

    #region Редактор

    #region InitDocEditForm

    SubDocumentEditor _Editor;

    public static void InitSubDocEditForm(object sender, InitSubDocEditFormEventArgs args)
    {
      EditAttrValue form = new EditAttrValue();
      form._Editor = args.Editor;
      form.AddPage1(args);
    }

    #endregion

    #region Страница 1 (общие)

    EFPDocComboBox efpAttrType;
    EFPAttrValueComboBox efpValue;

    private void AddPage1(InitSubDocEditFormEventArgs args)
    {
      DocEditPage page = args.AddPage("Общие", MainPanel1);
      page.ImageKey = "Атрибут";

      efpAttrType = new EFPDocComboBox(page.BaseProvider, cbAttrType, ProgramDBUI.TheUI.DocTypes["AttrTypes"]);
      efpAttrType.CanBeEmpty = false;
      args.AddRef(efpAttrType, "AttrType", false);

      EFPDateTimeBox efpDate = new EFPDateTimeBox(page.BaseProvider, edDate);
      efpDate.CanBeEmpty = true;
      args.AddDate(efpDate, "Date", false);

      efpValue = new EFPAttrValueComboBox(page.BaseProvider, edValue);
      efpValue.ControlLabelText = true;
      DocValueAnyValueBox dvValue = new DocValueAnyValueBox(args.Values["Value"], args.Values["LongValue"], efpValue, false);
      args.AddDocEditItem(dvValue);

      efpAttrType.DocIdEx.ValueChanged += new EventHandler(efpAttrType_ValueChanged);

      #region Комментарий

      EFPTextBox efpComment = new EFPTextBox(page.BaseProvider, edComment);
      efpComment.CanBeEmpty = true;
      args.AddText(efpComment, "Comment", true);

      #endregion
    }

    void efpAttrType_ValueChanged(object sender, EventArgs args)
    {
      efpValue.AttrTypeId = efpAttrType.DocId;
    }

    #endregion

    #endregion
  }

  /// <summary>
  /// Провайдер управляющего элемента для поля ввода значения произвольного типа, поддерживаемого Ассоо2
  /// Типы значений, которые можно вводить, определяются перечислением ValueType
  /// </summary>
  public class EFPAnyValueBox : EFPControl<UserMaskedComboBox>
  {
    #region Конструктор

    public EFPAnyValueBox(EFPBaseProvider baseProvider, UserMaskedComboBox control)
      : base(baseProvider, control, true)
    {
      _MaxLength = Int16.MaxValue;
      _ValueType = ValueType.String;
      control.TextChanged += new EventHandler(Control_TextChanged);

      control.PopupClick += new EventHandler(Control_PopupClick);
    }

    #endregion

    #region Свойство ValueType

    /// <summary>
    /// Тип вводимого значения.
    /// По умолчанию настроено на ввод значений строкового значения
    /// </summary>
    public ValueType ValueType
    {
      get { return _ValueType; }
      set
      {
        if (value == _ValueType)
          return;

        _ValueType = value;

        switch (value)
        {
          case ValueType.Date:
            Control.Mask = "00\\.00\\.0000";
            break;
          case ValueType.DateTime:
            Control.Mask = "00\\.00\\.0000 00\\:00\\:00";
            break;
          case ValueType.Boolean:
            Control.Mask = "0";
            break;
          default:
            Control.Mask = String.Empty;
            break;
        }

        if (ControlLabelText)
          InitLabelText();

        Validate();
      }
    }
    private ValueType _ValueType;

    #endregion

    #region Свойство Value

    /// <summary>
    /// Текущее значение
    /// Тип возвращаемого значения соответствует текущему значению свойства ValueType
    /// </summary>
    public object Value
    {
      get { return _Value; }
      set
      {
        if (_InsideValueChanged)
          return;
        Control.Text = PlantTools.ToString(value, ValueType);
      }
    }
    /// <summary>
    /// Текущее значение извлекаем в OnValidate()
    /// </summary>
    private object _Value;

    public DepValue<object> ValueEx
    {
      get
      {
        InitValueEx();
        return _ValueEx;
      }
      set
      {
        InitValueEx();
        _ValueEx.Source = value;
      }
    }
    private DepInput<object> _ValueEx;

    private void InitValueEx()
    {
      if (_ValueEx == null)
      {
        _ValueEx = new DepInput<object>(Value, ValueEx_ValueChanged);
        _ValueEx.OwnerInfo = new DepOwnerInfo(this, "ValueEx");
      }
    }

    private bool _InsideValueChanged;

    void Control_TextChanged(object sender, EventArgs args)
    {
      if (_InsideValueChanged)
        return;
      _InsideValueChanged = true;
      try
      {
        Validate();
        if (_ValueEx != null)
          _ValueEx.Value = _Value;
      }
      finally
      {
        _InsideValueChanged = false;
      }
    }

    void ValueEx_ValueChanged(object sender, EventArgs args)
    {
      Value = _ValueEx.Value;
    }

    #endregion

    #region Свойство MaxLength

    /// <summary>
    /// Максимальная длина текста.
    /// Свойство действует только когда ValueType=String
    /// По умолчанию - 32767 символов
    /// </summary>
    public int MaxLength
    {
      get { return _MaxLength; }
      set
      {
        if (value < 1 || value > Int16.MaxValue)
          throw new ArgumentOutOfRangeException();
        _MaxLength = value;
        if (ValueType == ValueType.String)
          Validate();
      }
    }
    private int _MaxLength;

    #endregion

    #region Проверка

    protected override void OnValidate()
    {
      _Value = null;
      base.OnValidate();
      if (ValidateState == UIValidateState.Error)
        return;

      if (!PlantTools.TryParse(Control.Text, ValueType, out _Value))
        SetError("Введенный текст нельзя преобразовать к типу " + PlantTools.GetValueTypeName(ValueType));
      switch (ValueType)
      {
        case ValueType.String:
          if (Control.Text.Length > MaxLength)
            SetError("Максимальная длина текста равна " + MaxLength.ToString() + ". Введено символов: " + Control.Text.Length.ToString());
          break;
      }
    }

    #endregion

    #region Свойство ControlLabelText

    public bool ControlLabelText
    {
      get { return _ControlLabelText; }
      set
      {
        if (value)
          InitLabelText();
        _ControlLabelText = value;
      }
    }
    private bool _ControlLabelText;

    private void InitLabelText()
    {
      if (Label == null)
        throw new NullReferenceException("Свойство Label не установлено");

      switch (ValueType)
      {
        case ValueType.String:
          Label.Text = "Строка";
          break;
        default:
          Label.Text = PlantTools.GetValueTypeName(ValueType);
          break;
      }
    }

    #endregion

    #region Выпадающий список

    protected virtual void Control_PopupClick(object sender, EventArgs args)
    {
      //if (ValueType==ValueType.Boolean)

      EFPApp.ShowTempMessage("Выбор значения не реализован");
    }


    #endregion
  }

  /// <summary>
  /// Переходник для EFPAccoo2ValueBox на текстовое поле
  /// </summary>
  public class DocValueAnyValueBox : TwoDocValueControl<string, string, EFPAnyValueBox>
  {
    #region Конструктор

    public DocValueAnyValueBox(DBxDocValue shortDocValue, DBxDocValue longDocValue, EFPAnyValueBox controlProvider, bool canMultiEdit)
      : base(shortDocValue, longDocValue, controlProvider, true, canMultiEdit)
    {
      DepExpr1<string, object> f1 = new DepExpr1<string, object>(controlProvider.ValueEx, CalcF1);
      DepExpr1<string, object> f2 = new DepExpr1<string, object>(controlProvider.ValueEx, CalcF2);
      base.SetCurrentValueEx(f1, f2);
      controlProvider.ValueEx.ValueChanged += ControlChanged;
      DepAnd.AttachInput(controlProvider.EnabledEx, EnabledEx);
    }

    private string CalcF1(object value)
    {
      string s = PlantTools.ValueToSaveableString(ControlProvider.Value, ControlProvider.ValueType);
      if (String.IsNullOrEmpty(s))
        return String.Empty;
      else if (s.Length <= PlantTools.AttrValueShortMaxLength)
        return s;
      else
        return String.Empty;
    }

    private string CalcF2(object value)
    {
      string s = PlantTools.ValueToSaveableString(ControlProvider.Value, ControlProvider.ValueType);
      if (String.IsNullOrEmpty(s))
        return String.Empty;
      else if (s.Length <= PlantTools.AttrValueShortMaxLength)
        return String.Empty;
      else
        return s;
    }

    #endregion

    #region Свойства

    /// <summary>
    /// Формат поля для хранения даты
    /// </summary>
    //public DocValueDateMode Mode;

    #endregion

    #region Переопределенные методы

    protected override void ValueToControl()
    {
      string s1 = DocValue1.AsString; // короткое значение
      string s2 = DocValue2.AsString; // длинное значение
      if (String.IsNullOrEmpty(s2))
        ControlProvider.Value = PlantTools.ValueFromSaveableString(s1, ControlProvider.ValueType);
      else
        ControlProvider.Value = PlantTools.ValueFromSaveableString(s2, ControlProvider.ValueType);
    }

    protected override void ValueFromControl()
    {
      string s = PlantTools.ValueToSaveableString(ControlProvider.Value, ControlProvider.ValueType);
      if (String.IsNullOrEmpty(s))
      {
        DocValue1.SetNull();
        DocValue2.SetNull();
      }
      else if (s.Length <= PlantTools.AttrValueShortMaxLength)
      {
        DocValue1.SetString(s);
        DocValue2.SetNull();
      }
      else
      {
        DocValue1.SetNull();
        DocValue2.SetString(s);
      }
    }

    #endregion
  }


  public class EFPAttrValueComboBox : EFPAnyValueBox
  {
    #region Конструктор

    public EFPAttrValueComboBox(EFPBaseProvider baseProvider, UserMaskedComboBox control)
      : base(baseProvider, control)
    {
    }

    #endregion

    #region AttrTypeId

    /// <summary>
    /// Идентификатор вида атрибута
    /// </summary>
    public Int32 AttrTypeId
    {
      get { return _AttrTypeId; }
      set
      {
        if (value == _AttrTypeId)
          return;
        _AttrTypeId = value;

        if (AttrType.Mask == null)
          base.Control.Mask = String.Empty;
        else
          base.Control.Mask = AttrType.Mask.EditMask;

        if (value == 0)
          base.ValueType = ValueType.String;
        else
          base.ValueType = AttrType.ValueType;

        if (_AttrTypeIdEx != null)
          _AttrTypeIdEx.Value = value;

        Validate();
      }
    }
    private Int32 _AttrTypeId;

    /// <summary>
    /// Управляемое свойство для AttrTypeId
    /// </summary>
    public DepValue<Int32> AttrTypeIdEx
    {
      get
      {
        InitAttrTypeIdEx();
        return _AttrTypeIdEx;
      }
      set
      {
        InitAttrTypeIdEx();
        _AttrTypeIdEx.Source = value;
      }
    }
    private DepInput<Int32> _AttrTypeIdEx;

    private void InitAttrTypeIdEx()
    {
      if (_AttrTypeIdEx == null)
      {
        _AttrTypeIdEx = new DepInput<Int32>(AttrTypeId, AttrTypeIdEx_ValueChanged);
        _AttrTypeIdEx.OwnerInfo = new DepOwnerInfo(this, "AttrTypeIdEx");
      }
    }

    void AttrTypeIdEx_ValueChanged(object sender, EventArgs args)
    {
      AttrTypeId = _AttrTypeIdEx.Value;
    }

    /// <summary>
    /// Объект документа "Вид атрибута" или null, если AttrTypeId=0
    /// </summary>
    public AttrTypeDoc AttrType
    {
      get
      {
        if (_AttrTypeId == 0)
          return null;
        else
          return new AttrTypeDoc(_AttrTypeId);
      }
    }

    #endregion

    #region Выпадающий список

    protected override void Control_PopupClick(object sender, EventArgs args)
    {
      if (AttrTypeId == 0)
      {
        EFPApp.ShowTempMessage("Не задан вид атрибута");
        return;
      }

      object x = this.Value;
      if (ProgramDBUI.SelectAttrValue(ref x, this.DisplayName, AttrType))
        this.Value = x;
    }

    #endregion

    #region Проверка

    protected override void OnValidate()
    {
      base.OnValidate();
      if (ValidateState != UIValidateState.Ok)
        return;
      if (AttrType == null)
        return;

      string errorText;
      if (!AttrType.TestValue(Value, out errorText))
        SetWarning(errorText);
    }

    #endregion
  }
}