using System;
using System.Collections.Generic;
using System.Text;
using FreeLibSet.Data.Docs;
using FreeLibSet.Forms;
using FreeLibSet.Data;
using FreeLibSet.Collections;
using FreeLibSet.Core;

namespace Plants
{
  public abstract class CareItem : ObjectWithCode
  {
    #region Конструктор

    public CareItem(string group, string code, string name)
      : base(code)
    {
#if DEBUG
      if (String.IsNullOrEmpty(group))
        throw new ArgumentNullException("Group");
      if (String.IsNullOrEmpty(name))
        throw new ArgumentNullException("Name");
#endif

      _Group = group;
      _Name = name;
    }

    #endregion

    #region Свойства

    public string Group { get { return _Group; } }
    private string _Group;

    public string Name { get { return _Name; } }
    private string _Name;

    #endregion

    #region Чтение и запись

    // Тип для ItemValue зависит от класса.
    // Значение null означает незаполненное значение

    public abstract object GetItemValue(IDBxDocValues docValues);

    public abstract void SetItemValue(IDBxDocValues docValues, object itemValue);

    public abstract void AddColumns(DBxTableStruct.ColumnCollection columns);

    #endregion

    #region Другие методы

    public abstract string GetTextValue(object itemValue);

    public abstract bool Edit(ref object itemValue);

    #endregion

    #region Статичеcкий список

    /// <summary>
    /// Полный список характеристик ухода за растениями
    /// </summary>
    public static readonly NamedList<CareItem> TheList = CreateCareItems();

    private static NamedList<CareItem> CreateCareItems()
    {
      NamedList<CareItem> list = new NamedList<CareItem>();

      IntValueCareItem ivci;
      IntRangeCareItem irci;

      #region Воздух

      irci = new IntRangeCareItem("Воздух", "Topt", "Температура оптимальная");
      irci.Minimum = -40;
      irci.Maximum = +60;
      irci.MeasureUnit = "°C";
      list.Add(irci);

      ivci = new IntValueCareItem("Воздух", "Tmin", "Температура минимальная");
      ivci.Minimum = -40;
      ivci.Maximum = +60;
      ivci.MeasureUnit = "°C";
      list.Add(ivci);

      ivci = new IntValueCareItem("Воздух", "Tmax", "Температура максимальная");
      ivci.Minimum = -40;
      ivci.Maximum = +60;
      ivci.MeasureUnit = "°C";
      list.Add(ivci);

      list.Add(new EnumCareItem("Воздух", "Air", "Воздух",
        new string[] { "NODRAFTS", "AIR", "BALKONY" },
        new string[] { "Беречь от сквозняков", "Проветривать", "Выносить на балкон" }));

      list.Add(new MemoCareItem("Воздух", "AirComment", "Воздух - примечания"));

      #endregion

      #region Свет

      list.Add(new EnumCareItem("Свет", "Sun", "Освещение",
        new string[] { "MAX", "NOONSHADING", "NOSUN", "DARK" },
        new string[] { "Максимальное освещение", "Притенять от полуденных лучей", "Без прямых лучей", "В глубине комнаты" }));

      list.Add(new EnumCareItem("Свет", "Rotation", "Поворот растения",
        new string[] { "NONE", "MONTH", "HALFMONTH", "WEEK" },
        new string[] { "Не поворачивать", "1 раз в месяц", "2 раза в месяц", "1 раз в неделю" }));

      ivci = new IntValueCareItem("Свет", "DaylightUpTo", "Досветка до");
      ivci.Minimum = 0;
      ivci.Maximum = 24;
      ivci.MeasureUnit = "ч.";
      list.Add(ivci);

      ivci = new IntValueCareItem("Свет", "DaylightTime", "Время досветки");
      ivci.Minimum = 0;
      ivci.Maximum = 24;
      ivci.MeasureUnit = "ч.";
      list.Add(ivci);

      list.Add(new MemoCareItem("Свет", "LightComment", "Свет - примечания"));

      #endregion

      #region Полив

      list.Add(new EnumCareItem("Полив", "WateringFrequency", "Частота полива",
        new string[] { "NONE", "MONTH", "HALFMONTH", "WEEK", "HALFWEEK", "INADAY", "EVERYDAY" },
        new string[] { "Не поливать", "1 раз в месяц", "2 раза в месяц", "1 раз в неделю", "2 раза в неделю", "Через день", "Каждый день" }));

      list.Add(new EnumCareItem("Полив", "WateringTime", "Время полива",
        new string[] { "MORNING", "EVENING", "NOSUN" },
        new string[] { "Утром", "Вечером", "Не на солнце" }));

      list.Add(new EnumCareItem("Полив", "WateringQuantity", "Интенсивность полива",
        new string[] { "ABUNDANT", "MODERATE", "SMALL" },
        new string[] { "Обильный", "Умеренный", "Небольшой" }));

      list.Add(new EnumCareItem("Полив", "WateringMethod", "Технология полива",
        new string[] { "PALLET" },
        new string[] { "Через поддон" }));

      list.Add(new MemoCareItem("Полив", "WateringComment", "Полив - примечания"));

      #endregion

      #region Удобрение

      list.Add(new EnumCareItem("Удобрение", "FertilizerType", "Вид удобрений",
        new string[] { "NONE", "SUCCULENT", "FLOWERING", "LEAVES", "ROSE" },
        new string[] { "Нет", "Для кактусов и суккулентов", "Для цветущих", "Для листьевых", "Для роз" }));

      list.Add(new EnumCareItem("Удобрение", "FertilizerFrequency", "Частота удобрения",
        new string[] { "NONE", "YEAR", "HALFYEAR", "MONTH3", "MONTH2", "MONTH", "HALFMONTH", "WEEK", "HALFWEEK" },
        new string[] { "Не удобрять", "1 раз в год", "2 раза в год", "1 раз в 3 месяца", "1 раз в 2 месяца", "1 раз в месяц", "2 раза в месяц", "1 раз в неделю", "2 раза в неделю" }));

      list.Add(new EnumCareItem("Удобрение", "FertilizerDoze", "Дозировка удобрений",
        new string[] { "NORMAL", "HALF" },
        new string[] { "Нормальная дозировка", "Половинная дозировка" }));

      list.Add(new MemoCareItem("Удобрение", "FertilizerComment", "Удобрение - примечания"));

      #endregion

      #region Влажность

      irci = new IntRangeCareItem("Влажность", "Hopt", "Влажность оптимальная");
      irci.Minimum = 0;
      irci.Maximum = 100;
      irci.MeasureUnit = "%";
      list.Add(irci);

      ivci = new IntValueCareItem("Влажность", "Hmin", "Влажность минимальная");
      ivci.Minimum = 0;
      ivci.Maximum = 100;
      ivci.MeasureUnit = "%";
      list.Add(ivci);

      ivci = new IntValueCareItem("Влажность", "Hmax", "Влажность максимальная");
      ivci.Minimum = 0;
      ivci.Maximum = 100;
      ivci.MeasureUnit = "%";
      list.Add(ivci);

      list.Add(new EnumCareItem("Влажность", "Humidification", "Увлажнение",
        new string[] { "SPRAYING", "NOSPRAYING" },
        new string[] { "Опрыскивать", "Не опрыскивать" }));

      list.Add(new EnumCareItem("Влажность", "Washing", "Мытье растения под душем",
        new string[] { "NONE", "REQUIRED", "MONTH", "HALFMONTH", "WEEK" },
        new string[] { "Не мыть", "По необходимости", "Раз в месяц", "2 раза в месяц", "Раз в неделю" }));

      list.Add(new EnumCareItem("Влажность", "WetRubbing", "Влажная протирка листьев",
        new string[] { "NONE", "REQUIRED", "MONTH", "HALFMONTH", "WEEK" },
        new string[] { "Не протирать", "По необходимости", "Раз в месяц", "2 раза в месяц", "Раз в неделю" }));

      list.Add(new EnumCareItem("Влажность", "DryRubbing", "Сухая протирка листьев",
        new string[] { "NONE", "REQUIRED" },
        new string[] { "Не протирать", "По необходимости" }));

      list.Add(new MemoCareItem("Влажность", "HumidityComment", "Влажность - примечания"));

      #endregion

      #region Болезни

      list.Add(new FlagsCareItem("Болезни", "Pests", "Вредители", new string[]{
        "Щитовка",
        "Тля", 
        "Трипс",
        "Паутинный клещ",
        "Мучнистый червец",
      }));
      list.Add(new MemoCareItem("Болезни", "DeseasesComment", "Болезни - примечания"));

      #endregion

      #region Прочее

      list.Add(new MemoCareItem("Прочее", "OtherComment", "Прочие примечания"));

      #endregion

      return list;
    }

    #endregion
  }

  public class MemoCareItem : CareItem
  {
    #region Конструктор

    public MemoCareItem(string group, string code, string name)
      : base(group, code, name)
    {
    }

    #endregion

    #region Переопределенные методы

    public override object GetItemValue(IDBxDocValues docValues)
    {
      string s = docValues[Code].AsString;
      if (s.Length == 0)
        return null;
      else
        return s;
    }

    public override void SetItemValue(IDBxDocValues docValues, object itemValue)
    {
      string s = itemValue as string;
      docValues[Code].SetString(s);
    }

    public override void AddColumns(DBxTableStruct.ColumnCollection columns)
    {
      columns.AddMemo(Code);
    }

    public override string GetTextValue(object itemValue)
    {
      return DataTools.GetString(itemValue);
    }

    public override bool Edit(ref object itemValue)
    {
      MultiLineTextInputDialog dlg = new MultiLineTextInputDialog();
      dlg.Title = Name;
      dlg.CanBeEmpty = true;
      if (itemValue != null)
        dlg.Text = (string)itemValue;
      if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
        return false;

      string s = dlg.Text.Trim();
      if (s.Length == 0)
        itemValue = null;
      else
        itemValue = s;
      return true;
    }

    #endregion
  }

  public class EnumCareItem : CareItem
  {
    #region Конструктор

    public EnumCareItem(string group, string code, string name, string[] enumCodes, string[] enumValues)
      : base(group, code, name)
    {
      if (enumCodes.Length != enumValues.Length)
        throw new ArgumentException("Длина массивов не совпадает");

      _EnumCodes = enumCodes;

      _EnumValues = enumValues;

      _CodeIndexer = new StringArrayIndexer(enumCodes, false);
    }

    #endregion

    #region Свойства

    public string[] EnumCodes { get { return _EnumCodes; } }
    private string[] _EnumCodes;

    private StringArrayIndexer _CodeIndexer;

    public string[] EnumValues { get { return _EnumValues; } }
    private string[] _EnumValues;

    #endregion

    #region Переопределенные методы

    public override void AddColumns(DBxTableStruct.ColumnCollection columns)
    {
      int maxLen = 1;
      for (int i = 0; i < EnumCodes.Length; i++)
        maxLen = Math.Max(maxLen, EnumCodes[i].Length);
      columns.AddString(Code, maxLen, true);
    }

    public override object GetItemValue(IDBxDocValues docValues)
    {
      string enumCode = docValues[Code].AsString;
      if (enumCode.Length == 0)
        return null;
      else
        return enumCode;
    }

    public override void SetItemValue(IDBxDocValues docValues, object itemValue)
    {
      docValues[Code].SetString((string)itemValue);
    }

    public override string GetTextValue(object itemValue)
    {
      string enumCode = itemValue as string;
      if (String.IsNullOrEmpty(enumCode))
        return String.Empty;

      int p = _CodeIndexer.IndexOf(enumCode);
      if (p >= 0)
        return EnumValues[p];
      else
        return "??? " + enumCode;
    }

    public override bool Edit(ref object itemValue)
    {
      ListSelectDialog dlg = new ListSelectDialog();
      dlg.Title = Name;
      dlg.Items = new string[EnumCodes.Length + 1];
      dlg.Items[0] = "[ Не задано ]";
      EnumValues.CopyTo(dlg.Items, 1);

      dlg.SelectedIndex = _CodeIndexer.IndexOf(DataTools.GetString(itemValue)) + 1;
      if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
        return false;

      if (dlg.SelectedIndex < 1)
        itemValue = null;
      else
        itemValue = EnumCodes[dlg.SelectedIndex - 1];
      return true;
    }

    #endregion
  }


  public class IntValueCareItem : CareItem
  {
    #region Конструктор

    public IntValueCareItem(string group, string code, string name)
      : base(group, code, name)
    {
      _MeasureUnit = String.Empty;
      _Minimum = Int32.MinValue;
      _Maximum = Int32.MaxValue;
    }

    #endregion

    #region Свойства

    public string MeasureUnit { get { return _MeasureUnit; } set { _MeasureUnit = value; } }
    private string _MeasureUnit;

    public int Minimum { get { return _Minimum; } set { _Minimum = value; } }
    private int _Minimum;

    public int Maximum { get { return _Maximum; } set { _Maximum = value; } }
    private int _Maximum;

    #endregion

    #region Переопределенные методы

    public override void AddColumns(DBxTableStruct.ColumnCollection columns)
    {
      columns.AddInt(Code, Minimum, Maximum);
    }

    public override object GetItemValue(IDBxDocValues docValues)
    {
      if (docValues[Code].IsNull)
        return null;
      else
        return docValues[Code].AsInteger;
    }

    public override void SetItemValue(IDBxDocValues docValues, object itemValue)
    {
      if (itemValue == null)
        docValues[Code].SetNull();
      else
        docValues[Code].SetValue(itemValue); // а не SetInt(), т.к. 0 это не null.
    }

    public override string GetTextValue(object itemValue)
    {
      if (itemValue == null)
        return String.Empty;
      else
      {
        int v = (int)itemValue;
        if (String.IsNullOrEmpty(MeasureUnit))
          return v.ToString();
        else
          return v.ToString() + " " + MeasureUnit;
      }
    }

    public override bool Edit(ref object itemValue)
    {
      IntInputDialog dlg = new IntInputDialog();
      dlg.Title = Name;
      if (!String.IsNullOrEmpty(MeasureUnit))
        dlg.Title += ", " + MeasureUnit;
      dlg.CanBeEmpty = true;
      dlg.NValue = (int?)itemValue;
      //dlg.ShowNoButton = true;
      dlg.Minimum = Minimum;
      dlg.Maximum = Maximum;

      switch (dlg.ShowDialog())
      {
        case System.Windows.Forms.DialogResult.OK:
          itemValue = dlg.NValue;
          return true;
        case System.Windows.Forms.DialogResult.No: // пока не бывает
          itemValue = null;
          return true;
        default:
          return false;
      }
    }

    #endregion
  }

  public class IntRangeCareItem : CareItem
  {
    #region Значение

    public struct Range
    {
      #region Конструктор

      public Range(int min, int max)
      {
        _Min = min;
        _Max = max;
      }

      #endregion

      #region Свойства

      public int Min { get { return _Min; } }
      private int _Min;

      public int Max { get { return _Max; } }
      private int _Max;

      public override string ToString()
      {
        if (_Min == _Max)
          return _Min.ToString();
        else
          return _Min.ToString() + " - " + _Max.ToString();
      }

      #endregion
    }

    #endregion

    #region Конструктор

    public IntRangeCareItem(string group, string code, string name)
      : base(group, code, name)
    {
      _MeasureUnit = String.Empty;
      _Minimum = Int32.MinValue;
      _Maximum = Int32.MaxValue;
    }

    #endregion

    #region Свойства

    public string ColumnName1 { get { return Code + "1"; } }

    public string ColumnName2 { get { return Code + "2"; } }

    public string MeasureUnit { get { return _MeasureUnit; } set { _MeasureUnit = value; } }
    private string _MeasureUnit;

    public int Minimum { get { return _Minimum; } set { _Minimum = value; } }
    private int _Minimum;

    public int Maximum { get { return _Maximum; } set { _Maximum = value; } }
    private int _Maximum;

    #endregion

    #region Переопределенные методы

    public override void AddColumns(DBxTableStruct.ColumnCollection columns)
    {
      columns.AddInt(ColumnName1, Minimum, Maximum);
      columns.AddInt(ColumnName2, Minimum, Maximum);
    }

    public override object GetItemValue(IDBxDocValues docValues)
    {
      if (docValues[ColumnName1].IsNull || docValues[ColumnName2].IsNull)
        return null;
      else
        return new Range(docValues[ColumnName1].AsInteger, docValues[ColumnName2].AsInteger);
    }

    public override void SetItemValue(IDBxDocValues docValues, object itemValue)
    {
      if (itemValue == null)
      {
        docValues[ColumnName1].SetNull();
        docValues[ColumnName2].SetNull();
      }
      else
      {
        Range r = (Range)itemValue;
        docValues[ColumnName1].SetValue(r.Min); // а не SetInt(), т.к. 0 это не null.
        docValues[ColumnName2].SetValue(r.Max);
      }
    }

    public override string GetTextValue(object itemValue)
    {
      if (itemValue == null)
        return String.Empty;
      else
      {
        Range r = (Range)itemValue;
        if (String.IsNullOrEmpty(MeasureUnit))
          return r.ToString();
        else
          return r.ToString() + " " + MeasureUnit;
      }
    }

    public override bool Edit(ref object itemValue)
    {
      IntRangeDialog dlg = new IntRangeDialog();
      dlg.Title = Name;
      if (!String.IsNullOrEmpty(MeasureUnit))
        dlg.Title += ", " + MeasureUnit;
      dlg.CanBeEmpty = true;
      if (itemValue != null)
      {
        Range r = (Range)itemValue;
        dlg.NFirstValue = r.Min;
        if (r.Max != r.Min)
          dlg.NLastValue = r.Max;
        else
          dlg.NLastValue = null;
      }
      else
      {
        dlg.NFirstValue = null;
        dlg.NLastValue = null;
      }
      dlg.Minimum = Minimum;
      dlg.Maximum = Maximum;

      switch (dlg.ShowDialog())
      {
        case System.Windows.Forms.DialogResult.OK:
          if (dlg.NFirstValue.HasValue || dlg.NLastValue.HasValue)
          {
            if (dlg.NFirstValue.HasValue && dlg.NLastValue.HasValue)
              itemValue = new Range(dlg.FirstValue, dlg.LastValue);
            else if (dlg.NFirstValue.HasValue)
              itemValue = new Range(dlg.FirstValue, dlg.FirstValue);
            else
              itemValue = new Range(dlg.LastValue, dlg.LastValue);
          }
          else
            itemValue = null;

          return true;
        case System.Windows.Forms.DialogResult.No:
          itemValue = null;
          return true;
        default:
          return false;
      }
    }

    #endregion
  }

#if XXX
  // Переделать, если будет нужен
  public class SingleRangeCareItem : CareItem
  {
  #region Значение

    public struct Range
    {
  #region Конструктор

      public Range(float Min, float Max)
      {
        FMin = Min;
        FMax = Max;
      }

  #endregion

  #region Свойства

      public float Min { get { return FMin; } }
      private float FMin;

      public float Max { get { return FMax; } }
      private float FMax;

      public override string ToString()
      {
        return ToString(String.Empty);
      }

      public string ToString(string Format)
      {
        if (FMin == FMax)
          return FMin.ToString(Format);
        else
          return FMin.ToString(Format) + " - " + FMax.ToString(Format);
      }

  #endregion
    }

  #endregion

  #region Конструктор

    public SingleRangeCareItem(string Code, string Name)
      : base(Code, Name)
    {
      FFormat = String.Empty;
    }

  #endregion

  #region Свойства

    public string ColumnName1 { get { return Code + "1"; } }

    public string ColumnName2 { get { return Code + "2"; } }

    public string Format { get { return FFormat; } set { FFormat = value; } }
    private string FFormat;

  #endregion

  #region Переопределенные методы

    public override void AddColumns(DBxColumnStructList Columns)
    {
      Columns.AddSingle(ColumnName1);
      Columns.AddSingle(ColumnName2);
    }

    public override object GetItemValue(IDBxDocValues DocValues)
    {
      if (DocValues[ColumnName1].IsNull || DocValues[ColumnName2].IsNull)
        return null;
      else
        return new Range(DocValues[ColumnName1].AsSingle, DocValues[ColumnName2].AsSingle);
    }

    public override void SetItemValue(IDBxDocValues DocValues, object ItemValue)
    {
      if (ItemValue == null)
      {
        DocValues[ColumnName1].SetNull();
        DocValues[ColumnName2].SetNull();
      }
      else
      {
        Range r = (Range)ItemValue;
        DocValues[ColumnName1].SetValue(r.Min); // а не SetSingle(), т.к. 0 это не null.
        DocValues[ColumnName2].SetValue(r.Max);
      }
    }

    public override string GetTextValue(object ItemValue)
    {
      if (ItemValue == null)
        return String.Empty;
      else
      {
        Range r = (Range)ItemValue;
        return r.ToString(Format);
      }
    }

    public override bool Edit(ref object ItemValue)
    {
      SingleRangeDialog dlg = new SingleRangeDialog();
      dlg.Title = Name;
      dlg.CanBeEmpty = true;
      if (ItemValue != null)
      {
        Range r = (Range)ItemValue;
        dlg.NullableFirstValue = r.Min;
        if (r.Max != r.Min)
          dlg.NullableLastValue = r.Max;
      }
      dlg.Format = Format;
      dlg.ShowNoButton = true;

      switch (dlg.ShowDialog())
      {
        case System.Windows.Forms.DialogResult.OK:
          if (dlg.NullableFirstValue.HasValue || dlg.NullableFirstValue.HasValue)
          {
            if (dlg.NullableFirstValue.HasValue && dlg.NullableFirstValue.HasValue)
              ItemValue = new Range(dlg.FirstValue, dlg.LastValue);
            else if (dlg.NullableFirstValue.HasValue)
              ItemValue = new Range(dlg.FirstValue, dlg.FirstValue);
            else
              ItemValue = new Range(dlg.LastValue, dlg.LastValue);
          }
          else
            ItemValue = null;

          return true;
        case System.Windows.Forms.DialogResult.No:
          ItemValue = null;
          return true;
        default:
          return false;
      }
    }

  #endregion
  }
#endif

  /// <summary>
  /// Набор флажков (вредители).
  /// Хранится как одно целочисленное поле
  /// </summary>
  public class FlagsCareItem : CareItem
  {
    #region Конструктор

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="group"></param>
    /// <param name="code"></param>
    /// <param name="name"></param>
    /// <param name="itemNames">Названия для флажков. Нельзя убирать и менять порядок следования элементов</param>
    public FlagsCareItem(string group, string code, string name, string[] itemNames)
      : base(group, code, name)
    {
      if (itemNames.Length > 32)
        throw new ArgumentException("Максимум 32 флажка", "ItemNames");
      _ItemNames = itemNames;
    }

    #endregion

    #region Свойства

    public string[] ItemNames { get { return _ItemNames; } }
    private string[] _ItemNames;

    #endregion

    #region Переопределенные методы

    public override object GetItemValue(IDBxDocValues docValues)
    {
      int v = docValues[Code].AsInteger;
      if (v == 0)
        return null;
      else
        return v;
    }

    public override void SetItemValue(IDBxDocValues docValues, object itemValue)
    {
      docValues[Code].SetValue(itemValue);
    }

    public override void AddColumns(DBxTableStruct.ColumnCollection columns)
    {
      columns.AddInt(Code);
    }

    public override string GetTextValue(object itemValue)
    {
      int v = DataTools.GetInt(itemValue);
      if (v == 0)
        return String.Empty;

      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < ItemNames.Length; i++)
      {
        int mask = 1 << i;
        if ((v & mask) != 0)
        {
          if (sb.Length > 0)
            sb.Append(", ");
          sb.Append(ItemNames[i]);
        }
      }
      return sb.ToString();
    }

    public override bool Edit(ref object itemValue)
    {
      int v = DataTools.GetInt(itemValue);

      ListSelectDialog dlg = new ListSelectDialog();
      dlg.Title = Name;
      dlg.MultiSelect = true;
      dlg.CanBeEmpty = true;
      dlg.Items = ItemNames;
      for (int i = 0; i < ItemNames.Length; i++)
      {
        int mask = 1 << i;
        dlg.Selections[i] = ((v & mask) != 0);
      }

      if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
        return false;

      v = 0;
      for (int i = 0; i < ItemNames.Length; i++)
      {
        int mask = 1 << i;
        if (dlg.Selections[i])
          v |= mask;
      }
      if (v == 0)
        itemValue = null;
      else
        itemValue = v;

      return true;
    }

    #endregion
  }

  public class CareValueChangedEventArgs : EventArgs
  {
    #region Конструктор

    public CareValueChangedEventArgs(CareValues careValues, int itemIndex, object oldValue)
    {
      _CareValues = careValues;
      _ItemIndex = itemIndex;
      _OldValue = oldValue;
    }

    #endregion

    #region Свойства

    //    public CareValues CareValues { get { return FCareValues; } }
    private CareValues _CareValues;

    public CareItem Item { get { return _CareValues.Items[_ItemIndex]; } }

    public int ItemIndex { get { return _ItemIndex; } }
    private int _ItemIndex;

    public object NewValue { get { return _CareValues[_ItemIndex]; } }

    public object OldValue { get { return _OldValue; } }
    private object _OldValue;

    #endregion
  }

  public delegate void CareValueChangedEventHandler(object sender, CareValueChangedEventArgs args);

  /// <summary>
  /// Массив значений типа object, в которых хранятся значения
  /// </summary>
  public class CareValues
  {
    #region Конструктор

    public CareValues(NamedList<CareItem> items)
    {
      if (items == null)
        throw new ArgumentNullException("Items");
      _Items = items;
      _Values = new object[_Items.Count];
    }

    #endregion

    #region Свойства

    public NamedList<CareItem> Items { get { return _Items; } }
    private NamedList<CareItem> _Items;

    public object this[int index]
    {
      get { return _Values[index]; }
      set
      {
        object oldValue = _Values[index];
        if (object.Equals(oldValue, value))
          return;
        _Values[index] = value;

        if (Changed != null)
        {
          CareValueChangedEventArgs args = new CareValueChangedEventArgs(this, index, oldValue);
          Changed(this, args);
        }
      }
    }
    private object[] _Values;

    public event CareValueChangedEventHandler Changed;

    #endregion

    #region Чтение и запись

    public void Read(IDBxDocValues docValues)
    {
      for (int i = 0; i < Items.Count; i++)
        this[i] = Items[i].GetItemValue(docValues);
    }

    public void Write(IDBxDocValues docValues)
    {
      for (int i = 0; i < Items.Count; i++)
        Items[i].SetItemValue(docValues, this[i]);
    }

    #endregion
  }
}
