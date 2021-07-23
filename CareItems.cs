using System;
using System.Collections.Generic;
using System.Text;
using AgeyevAV;
using AgeyevAV.ExtDB.Docs;
using AgeyevAV.ExtForms;
using AgeyevAV.ExtDB;

namespace Plants
{
  public abstract class CareItem : ObjectWithCode
  {
    #region Конструктор

    public CareItem(string Group, string Code, string Name)
      : base(Code)
    {
#if DEBUG
      if (String.IsNullOrEmpty(Group))
        throw new ArgumentNullException("Group");
      if (String.IsNullOrEmpty(Name))
        throw new ArgumentNullException("Name");
#endif

      _Group = Group;
      _Name = Name;
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

    public abstract object GetItemValue(IDBxDocValues DocValues);

    public abstract void SetItemValue(IDBxDocValues DocValues, object ItemValue);

    public abstract void AddColumns(DBxColumnStructList Columns);

    #endregion

    #region Другие методы

    public abstract string GetTextValue(object ItemValue);

    public abstract bool Edit(ref object ItemValue);

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

    public MemoCareItem(string Group, string Code, string Name)
      : base(Group, Code , Name)
    {
    }

    #endregion

    #region Переопределенные методы

    public override object GetItemValue(IDBxDocValues DocValues)
    {
      string s = DocValues[Code].AsString;
      if (s.Length == 0)
        return null;
      else
        return s;
    }

    public override void SetItemValue(IDBxDocValues DocValues, object ItemValue)
    {
      string s = ItemValue as string;
      DocValues[Code].SetString(s);
    }

    public override void AddColumns(DBxColumnStructList Columns)
    {
      Columns.AddMemo(Code);
    }

    public override string GetTextValue(object ItemValue)
    {
      return DataTools.GetString(ItemValue);
    }

    public override bool Edit(ref object ItemValue)
    {
      MultiLineTextInputDialog dlg = new MultiLineTextInputDialog();
      dlg.Title = Name;
      dlg.CanBeEmpty = true;
      if (ItemValue != null)
        dlg.Value = (string)ItemValue;
      if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
        return false;

      string s = dlg.Value.Trim();
      if (s.Length == 0)
        ItemValue = null;
      else
        ItemValue = s;
      return true;
    }

    #endregion
  }

  public class EnumCareItem : CareItem
  {
    #region Конструктор

    public EnumCareItem(string Group, string Code, string Name, string[] EnumCodes, string[] EnumValues)
      : base(Group, Code , Name)
    {
      if (EnumCodes.Length != EnumValues.Length)
        throw new ArgumentException("Длина массивов не совпадает");

      FEnumCodes = EnumCodes;

      FEnumValues = EnumValues;

      FCodeIndexer = new StringArrayIndexer(EnumCodes, false);
    }

    #endregion

    #region Свойства

    public string[] EnumCodes { get { return FEnumCodes; } }
    private string[] FEnumCodes;

    private StringArrayIndexer FCodeIndexer;

    public string[] EnumValues { get { return FEnumValues; } }
    private string[] FEnumValues;

    #endregion

    #region Переопределенные методы

    public override void AddColumns(DBxColumnStructList Columns)
    {
      int MaxLen = 1;
      for (int i = 0; i < EnumCodes.Length; i++)
        MaxLen = Math.Max(MaxLen, EnumCodes[i].Length);
      Columns.AddString(Code, MaxLen, true);
    }

    public override object GetItemValue(IDBxDocValues DocValues)
    {
      string EnumCode = DocValues[Code].AsString;
      if (EnumCode.Length == 0)
        return null;
      else
        return EnumCode;
    }

    public override void SetItemValue(IDBxDocValues DocValues, object ItemValue)
    {
      DocValues[Code].SetString((string)ItemValue);
    }

    public override string GetTextValue(object ItemValue)
    {
      string EnumCode = ItemValue as string;
      if (String.IsNullOrEmpty(EnumCode))
        return String.Empty;

      int p = FCodeIndexer.IndexOf(EnumCode);
      if (p >= 0)
        return EnumValues[p];
      else
        return "??? " + EnumCode;
    }

    public override bool Edit(ref object ItemValue)
    {
      ListSelectDialog dlg = new ListSelectDialog();
      dlg.Title = Name;
      dlg.Items = new string[EnumCodes.Length + 1];
      dlg.Items[0] = "[ Не задано ]";
      EnumValues.CopyTo(dlg.Items, 1);

      dlg.SelectedIndex = FCodeIndexer.IndexOf(DataTools.GetString(ItemValue)) + 1;
      if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
        return false;

      if (dlg.SelectedIndex < 1)
        ItemValue = null;
      else
        ItemValue = EnumCodes[dlg.SelectedIndex - 1];
      return true;
    }

    #endregion
  }


  public class IntValueCareItem : CareItem
  {
    #region Конструктор

    public IntValueCareItem(string Group, string Code, string Name)
      : base(Group, Code , Name)
    {
      FMeasureUnit = String.Empty;
      FMinimum = Int32.MinValue;
      FMaximum = Int32.MaxValue;
    }

    #endregion

    #region Свойства

    public string MeasureUnit { get { return FMeasureUnit; } set { FMeasureUnit = value; } }
    private string FMeasureUnit;

    public int Minimum { get { return FMinimum; } set { FMinimum = value; } }
    private int FMinimum;

    public int Maximum { get { return FMaximum; } set { FMaximum = value; } }
    private int FMaximum;

    #endregion

    #region Переопределенные методы

    public override void AddColumns(DBxColumnStructList Columns)
    {
      Columns.AddInt(Code, Minimum, Maximum);
    }

    public override object GetItemValue(IDBxDocValues DocValues)
    {
      if (DocValues[Code].IsNull)
        return null;
      else
        return DocValues[Code].AsInteger;
    }

    public override void SetItemValue(IDBxDocValues DocValues, object ItemValue)
    {
      if (ItemValue == null)
        DocValues[Code].SetNull();
      else
        DocValues[Code].SetValue(ItemValue); // а не SetInt(), т.к. 0 это не null.
    }

    public override string GetTextValue(object ItemValue)
    {
      if (ItemValue == null)
        return String.Empty;
      else
      {
        int v = (int)ItemValue;
        if (String.IsNullOrEmpty(MeasureUnit))
          return v.ToString();
        else
          return v.ToString() + " " + MeasureUnit;
      }
    }

    public override bool Edit(ref object ItemValue)
    {
      IntInputDialog dlg = new IntInputDialog();
      dlg.Title = Name;
      if (!String.IsNullOrEmpty(MeasureUnit))
        dlg.Title += ", " + MeasureUnit;
      dlg.CanBeEmpty = true;
      dlg.NullableValue = (int?)ItemValue;
      //dlg.ShowNoButton = true;
      dlg.MinValue = Minimum;
      dlg.MaxValue = Maximum;

      switch (dlg.ShowDialog())
      {
        case System.Windows.Forms.DialogResult.OK:
          ItemValue = dlg.NullableValue;
          return true;
        case System.Windows.Forms.DialogResult.No: // пока не бывает
          ItemValue = null;
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

      public Range(int Min, int Max)
      {
        FMin = Min;
        FMax = Max;
      }

      #endregion

      #region Свойства

      public int Min { get { return FMin; } }
      private int FMin;

      public int Max { get { return FMax; } }
      private int FMax;

      public override string ToString()
      {
        if (FMin == FMax)
          return FMin.ToString();
        else
          return FMin.ToString() + " - " + FMax.ToString();
      }

      #endregion
    }

    #endregion

    #region Конструктор

    public IntRangeCareItem(string Group, string Code, string Name)
      : base(Group, Code , Name)
    {
      FMeasureUnit = String.Empty;
      FMinimum = Int32.MinValue;
      FMaximum = Int32.MaxValue;
    }

    #endregion

    #region Свойства

    public string ColumnName1 { get { return Code + "1"; } }

    public string ColumnName2 { get { return Code + "2"; } }

    public string MeasureUnit { get { return FMeasureUnit; } set { FMeasureUnit = value; } }
    private string FMeasureUnit;

    public int Minimum { get { return FMinimum; } set { FMinimum = value; } }
    private int FMinimum;

    public int Maximum { get { return FMaximum; } set { FMaximum = value; } }
    private int FMaximum;

    #endregion

    #region Переопределенные методы

    public override void AddColumns(DBxColumnStructList Columns)
    {
      Columns.AddInt(ColumnName1, Minimum, Maximum);
      Columns.AddInt(ColumnName2, Minimum, Maximum);
    }

    public override object GetItemValue(IDBxDocValues DocValues)
    {
      if (DocValues[ColumnName1].IsNull || DocValues[ColumnName2].IsNull)
        return null;
      else
        return new Range(DocValues[ColumnName1].AsInteger, DocValues[ColumnName2].AsInteger);
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
        DocValues[ColumnName1].SetValue(r.Min); // а не SetInt(), т.к. 0 это не null.
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
        if (String.IsNullOrEmpty(MeasureUnit))
          return r.ToString();
        else
          return r.ToString() + " " + MeasureUnit;
      }
    }

    public override bool Edit(ref object ItemValue)
    {
      IntRangeDialog dlg = new IntRangeDialog();
      dlg.Title = Name;
      if (!String.IsNullOrEmpty(MeasureUnit))
        dlg.Title += ", " + MeasureUnit;
      dlg.CanBeEmpty = true;
      if (ItemValue != null)
      {
        Range r = (Range)ItemValue;
        dlg.NullableFirstValue = r.Min;
        if (r.Max != r.Min)
          dlg.NullableLastValue = r.Max;
        else
          dlg.NullableLastValue = null;
      }
      else
      {
        dlg.NullableFirstValue = null;
        dlg.NullableLastValue = null;
      }
      dlg.ShowNoButton = true;
      dlg.MinValue = Minimum;
      dlg.MaxValue = Maximum;

      switch (dlg.ShowDialog())
      {
        case System.Windows.Forms.DialogResult.OK:
          if (dlg.NullableFirstValue.HasValue || dlg.NullableLastValue.HasValue)
          {
            if (dlg.NullableFirstValue.HasValue && dlg.NullableLastValue.HasValue)
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
    /// <param name="Group"></param>
    /// <param name="Code"></param>
    /// <param name="Name"></param>
    /// <param name="ItemNames">Названия для флажков. Нельзя убирать и менять порядок следования элементов</param>
    public FlagsCareItem(string Group, string Code, string Name, string[] ItemNames)
      : base(Group, Code,Name)
    {
      if (ItemNames.Length > 32)
        throw new ArgumentException("Максимум 32 флажка", "ItemNames");
      FItemNames = ItemNames;
    }

    #endregion

    #region Свойства

    public string[] ItemNames { get { return FItemNames; } }
    private string[] FItemNames;

    #endregion

    #region Переопределенные методы

    public override object GetItemValue(IDBxDocValues DocValues)
    {
      int v = DocValues[Code].AsInteger;
      if (v == 0)
        return null;
      else
        return v;
    }

    public override void SetItemValue(IDBxDocValues DocValues, object ItemValue)
    {
      DocValues[Code].SetValue(ItemValue);
    }

    public override void AddColumns(DBxColumnStructList Columns)
    {
      Columns.AddInt(Code);
    }

    public override string GetTextValue(object ItemValue)
    {
      int v = DataTools.GetInt(ItemValue);
      if (v == 0)
        return String.Empty;

      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < ItemNames.Length; i++)
      {
        int Mask = 1 << i;
        if ((v & Mask) != 0)
        {
          if (sb.Length > 0)
            sb.Append(", ");
          sb.Append(ItemNames[i]);
        }
      }
      return sb.ToString();
    }

    public override bool Edit(ref object ItemValue)
    {
      int v = DataTools.GetInt(ItemValue);

      ListSelectDialog dlg = new ListSelectDialog();
      dlg.Title = Name;
      dlg.MultiSelect = true;
      dlg.CanBeEmpty = true;
      dlg.Items = ItemNames;
      for (int i = 0; i < ItemNames.Length; i++)
      {
        int Mask = 1 << i;
        dlg.Selections[i] = ((v & Mask) != 0);
      }

      if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
        return false;

      v = 0;
      for (int i = 0; i < ItemNames.Length; i++)
      {
        int Mask = 1 << i;
        if (dlg.Selections[i])
          v |= Mask;
      }
      if (v == 0)
        ItemValue = null;
      else
        ItemValue = v;

      return true;
    }

    #endregion
  }

  public class CareValueChangedEventArgs : EventArgs
  {
    #region Конструктор

    public CareValueChangedEventArgs(CareValues CareValues, int ItemIndex, object OldValue)
    {
      FCareValues = CareValues;
      FItemIndex = ItemIndex;
      FOldValue = OldValue;
    }

    #endregion

    #region Свойства

    //    public CareValues CareValues { get { return FCareValues; } }
    private CareValues FCareValues;

    public CareItem Item { get { return FCareValues.Items[FItemIndex]; } }

    public int ItemIndex { get { return FItemIndex; } }
    private int FItemIndex;

    public object NewValue { get { return FCareValues[FItemIndex]; } }

    public object OldValue { get { return FOldValue; } }
    private object FOldValue;

    #endregion
  }

  public delegate void CareValueChangedEventHandler(object Sender, CareValueChangedEventArgs Args);

  /// <summary>
  /// Массив значений типа object, в которых хранятся значения
  /// </summary>
  public class CareValues
  {
    #region Конструктор

    public CareValues(NamedList<CareItem> Items)
    {
      if (Items == null)
        throw new ArgumentNullException("Items");
      FItems = Items;
      FValues = new object[FItems.Count];
    }

    #endregion

    #region Свойства

    public NamedList<CareItem> Items { get { return FItems; } }
    private NamedList<CareItem> FItems;

    public object this[int Index]
    {
      get { return FValues[Index]; }
      set
      {
        object OldValue = FValues[Index];
        if (object.Equals(OldValue, value))
          return;
        FValues[Index] = value;

        if (Changed != null)
        {
          CareValueChangedEventArgs Args = new CareValueChangedEventArgs(this, Index, OldValue);
          Changed(this, Args);
        }
      }
    }
    private object[] FValues;

    public event CareValueChangedEventHandler Changed;

    #endregion

    #region Чтение и запись

    public void Read(IDBxDocValues DocValues)
    {
      for (int i = 0; i < Items.Count; i++)
        this[i] = Items[i].GetItemValue(DocValues);
    }

    public void Write(IDBxDocValues DocValues)
    {
      for (int i = 0; i < Items.Count; i++)
        Items[i].SetItemValue(DocValues, this[i]);
    }

    #endregion
  }
}
