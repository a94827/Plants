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
    #region �����������

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

    #region ��������

    public string Group { get { return _Group; } }
    private string _Group;

    public string Name { get { return _Name; } }
    private string _Name;

    #endregion

    #region ������ � ������

    // ��� ��� ItemValue ������� �� ������.
    // �������� null �������� ������������� ��������

    public abstract object GetItemValue(IDBxDocValues DocValues);

    public abstract void SetItemValue(IDBxDocValues DocValues, object ItemValue);

    public abstract void AddColumns(DBxColumnStructList Columns);

    #endregion

    #region ������ ������

    public abstract string GetTextValue(object ItemValue);

    public abstract bool Edit(ref object ItemValue);

    #endregion

    #region �������c��� ������

    /// <summary>
    /// ������ ������ ������������� ����� �� ����������
    /// </summary>
    public static readonly NamedList<CareItem> TheList = CreateCareItems();

    private static NamedList<CareItem> CreateCareItems()
    {
      NamedList<CareItem> list = new NamedList<CareItem>();

      IntValueCareItem ivci;
      IntRangeCareItem irci;

      #region ������

      irci = new IntRangeCareItem("������", "Topt", "����������� �����������");
      irci.Minimum = -40;
      irci.Maximum = +60;
      irci.MeasureUnit = "�C";
      list.Add(irci);

      ivci = new IntValueCareItem("������", "Tmin", "����������� �����������");
      ivci.Minimum = -40;
      ivci.Maximum = +60;
      ivci.MeasureUnit = "�C";
      list.Add(ivci);

      ivci = new IntValueCareItem("������", "Tmax", "����������� ������������");
      ivci.Minimum = -40;
      ivci.Maximum = +60;
      ivci.MeasureUnit = "�C";
      list.Add(ivci);

      list.Add(new EnumCareItem("������", "Air", "������",
        new string[] { "NODRAFTS", "AIR", "BALKONY" },
        new string[] { "������ �� ����������", "������������", "�������� �� ������" }));

      list.Add(new MemoCareItem("������", "AirComment", "������ - ����������"));

      #endregion

      #region ����

      list.Add(new EnumCareItem("����", "Sun", "���������",
        new string[] { "MAX", "NOONSHADING", "NOSUN", "DARK" },
        new string[] { "������������ ���������", "��������� �� ���������� �����", "��� ������ �����", "� ������� �������" }));

      list.Add(new EnumCareItem("����", "Rotation", "������� ��������",
        new string[] { "NONE", "MONTH", "HALFMONTH", "WEEK" },
        new string[] { "�� ������������", "1 ��� � �����", "2 ���� � �����", "1 ��� � ������" }));

      ivci = new IntValueCareItem("����", "DaylightUpTo", "�������� ��");
      ivci.Minimum = 0;
      ivci.Maximum = 24;
      ivci.MeasureUnit = "�.";
      list.Add(ivci);

      ivci = new IntValueCareItem("����", "DaylightTime", "����� ��������");
      ivci.Minimum = 0;
      ivci.Maximum = 24;
      ivci.MeasureUnit = "�.";
      list.Add(ivci);

      list.Add(new MemoCareItem("����", "LightComment", "���� - ����������"));

      #endregion

      #region �����

      list.Add(new EnumCareItem("�����", "WateringFrequency", "������� ������",
        new string[] { "NONE", "MONTH", "HALFMONTH", "WEEK", "HALFWEEK", "INADAY", "EVERYDAY" },
        new string[] { "�� ��������", "1 ��� � �����", "2 ���� � �����", "1 ��� � ������", "2 ���� � ������", "����� ����", "������ ����" }));

      list.Add(new EnumCareItem("�����", "WateringTime", "����� ������",
        new string[] { "MORNING", "EVENING", "NOSUN" },
        new string[] { "�����", "�������", "�� �� ������" }));

      list.Add(new EnumCareItem("�����", "WateringQuantity", "������������� ������",
        new string[] { "ABUNDANT", "MODERATE", "SMALL" },
        new string[] { "��������", "���������", "���������" }));

      list.Add(new EnumCareItem("�����", "WateringMethod", "���������� ������",
        new string[] { "PALLET" },
        new string[] { "����� ������" }));

      list.Add(new MemoCareItem("�����", "WateringComment", "����� - ����������"));

      #endregion

      #region ���������

      list.Add(new EnumCareItem("���������", "FertilizerType", "��� ���������",
        new string[] { "NONE", "SUCCULENT", "FLOWERING", "LEAVES", "ROSE" },
        new string[] { "���", "��� �������� � �����������", "��� ��������", "��� ���������", "��� ���" }));

      list.Add(new EnumCareItem("���������", "FertilizerFrequency", "������� ���������",
        new string[] { "NONE", "YEAR", "HALFYEAR", "MONTH3", "MONTH2", "MONTH", "HALFMONTH", "WEEK", "HALFWEEK" },
        new string[] { "�� ��������", "1 ��� � ���", "2 ���� � ���", "1 ��� � 3 ������", "1 ��� � 2 ������", "1 ��� � �����", "2 ���� � �����", "1 ��� � ������", "2 ���� � ������" }));

      list.Add(new EnumCareItem("���������", "FertilizerDoze", "��������� ���������",
        new string[] { "NORMAL", "HALF" },
        new string[] { "���������� ���������", "���������� ���������" }));

      list.Add(new MemoCareItem("���������", "FertilizerComment", "��������� - ����������"));

      #endregion

      #region ���������

      irci = new IntRangeCareItem("���������", "Hopt", "��������� �����������");
      irci.Minimum = 0;
      irci.Maximum = 100;
      irci.MeasureUnit = "%";
      list.Add(irci);

      ivci = new IntValueCareItem("���������", "Hmin", "��������� �����������");
      ivci.Minimum = 0;
      ivci.Maximum = 100;
      ivci.MeasureUnit = "%";
      list.Add(ivci);

      ivci = new IntValueCareItem("���������", "Hmax", "��������� ������������");
      ivci.Minimum = 0;
      ivci.Maximum = 100;
      ivci.MeasureUnit = "%";
      list.Add(ivci);

      list.Add(new EnumCareItem("���������", "Humidification", "����������",
        new string[] { "SPRAYING", "NOSPRAYING" },
        new string[] { "�����������", "�� �����������" }));

      list.Add(new EnumCareItem("���������", "Washing", "����� �������� ��� �����",
        new string[] { "NONE", "REQUIRED", "MONTH", "HALFMONTH", "WEEK" },
        new string[] { "�� ����", "�� �������������", "��� � �����", "2 ���� � �����", "��� � ������" }));

      list.Add(new EnumCareItem("���������", "WetRubbing", "������� �������� �������",
        new string[] { "NONE", "REQUIRED", "MONTH", "HALFMONTH", "WEEK" },
        new string[] { "�� ���������", "�� �������������", "��� � �����", "2 ���� � �����", "��� � ������" }));

      list.Add(new EnumCareItem("���������", "DryRubbing", "����� �������� �������",
        new string[] { "NONE", "REQUIRED" },
        new string[] { "�� ���������", "�� �������������" }));

      list.Add(new MemoCareItem("���������", "HumidityComment", "��������� - ����������"));

      #endregion

      #region �������

      list.Add(new FlagsCareItem("�������", "Pests", "���������", new string[]{
        "�������",
        "���", 
        "�����",
        "��������� ����",
        "��������� ������",
      }));
      list.Add(new MemoCareItem("�������", "DeseasesComment", "������� - ����������"));

      #endregion

      #region ������

      list.Add(new MemoCareItem("������", "OtherComment", "������ ����������"));

      #endregion

      return list;
    }

    #endregion
  }

  public class MemoCareItem : CareItem
  {
    #region �����������

    public MemoCareItem(string Group, string Code, string Name)
      : base(Group, Code , Name)
    {
    }

    #endregion

    #region ���������������� ������

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
    #region �����������

    public EnumCareItem(string Group, string Code, string Name, string[] EnumCodes, string[] EnumValues)
      : base(Group, Code , Name)
    {
      if (EnumCodes.Length != EnumValues.Length)
        throw new ArgumentException("����� �������� �� ���������");

      FEnumCodes = EnumCodes;

      FEnumValues = EnumValues;

      FCodeIndexer = new StringArrayIndexer(EnumCodes, false);
    }

    #endregion

    #region ��������

    public string[] EnumCodes { get { return FEnumCodes; } }
    private string[] FEnumCodes;

    private StringArrayIndexer FCodeIndexer;

    public string[] EnumValues { get { return FEnumValues; } }
    private string[] FEnumValues;

    #endregion

    #region ���������������� ������

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
      dlg.Items[0] = "[ �� ������ ]";
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
    #region �����������

    public IntValueCareItem(string Group, string Code, string Name)
      : base(Group, Code , Name)
    {
      FMeasureUnit = String.Empty;
      FMinimum = Int32.MinValue;
      FMaximum = Int32.MaxValue;
    }

    #endregion

    #region ��������

    public string MeasureUnit { get { return FMeasureUnit; } set { FMeasureUnit = value; } }
    private string FMeasureUnit;

    public int Minimum { get { return FMinimum; } set { FMinimum = value; } }
    private int FMinimum;

    public int Maximum { get { return FMaximum; } set { FMaximum = value; } }
    private int FMaximum;

    #endregion

    #region ���������������� ������

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
        DocValues[Code].SetValue(ItemValue); // � �� SetInt(), �.�. 0 ��� �� null.
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
        case System.Windows.Forms.DialogResult.No: // ���� �� ������
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
    #region ��������

    public struct Range
    {
      #region �����������

      public Range(int Min, int Max)
      {
        FMin = Min;
        FMax = Max;
      }

      #endregion

      #region ��������

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

    #region �����������

    public IntRangeCareItem(string Group, string Code, string Name)
      : base(Group, Code , Name)
    {
      FMeasureUnit = String.Empty;
      FMinimum = Int32.MinValue;
      FMaximum = Int32.MaxValue;
    }

    #endregion

    #region ��������

    public string ColumnName1 { get { return Code + "1"; } }

    public string ColumnName2 { get { return Code + "2"; } }

    public string MeasureUnit { get { return FMeasureUnit; } set { FMeasureUnit = value; } }
    private string FMeasureUnit;

    public int Minimum { get { return FMinimum; } set { FMinimum = value; } }
    private int FMinimum;

    public int Maximum { get { return FMaximum; } set { FMaximum = value; } }
    private int FMaximum;

    #endregion

    #region ���������������� ������

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
        DocValues[ColumnName1].SetValue(r.Min); // � �� SetInt(), �.�. 0 ��� �� null.
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
  // ����������, ���� ����� �����
  public class SingleRangeCareItem : CareItem
  {
  #region ��������

    public struct Range
    {
  #region �����������

      public Range(float Min, float Max)
      {
        FMin = Min;
        FMax = Max;
      }

  #endregion

  #region ��������

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

  #region �����������

    public SingleRangeCareItem(string Code, string Name)
      : base(Code, Name)
    {
      FFormat = String.Empty;
    }

  #endregion

  #region ��������

    public string ColumnName1 { get { return Code + "1"; } }

    public string ColumnName2 { get { return Code + "2"; } }

    public string Format { get { return FFormat; } set { FFormat = value; } }
    private string FFormat;

  #endregion

  #region ���������������� ������

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
        DocValues[ColumnName1].SetValue(r.Min); // � �� SetSingle(), �.�. 0 ��� �� null.
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
  /// ����� ������� (���������).
  /// �������� ��� ���� ������������� ����
  /// </summary>
  public class FlagsCareItem : CareItem
  {
    #region �����������

    /// <summary>
    /// �����������
    /// </summary>
    /// <param name="Group"></param>
    /// <param name="Code"></param>
    /// <param name="Name"></param>
    /// <param name="ItemNames">�������� ��� �������. ������ ������� � ������ ������� ���������� ���������</param>
    public FlagsCareItem(string Group, string Code, string Name, string[] ItemNames)
      : base(Group, Code,Name)
    {
      if (ItemNames.Length > 32)
        throw new ArgumentException("�������� 32 ������", "ItemNames");
      FItemNames = ItemNames;
    }

    #endregion

    #region ��������

    public string[] ItemNames { get { return FItemNames; } }
    private string[] FItemNames;

    #endregion

    #region ���������������� ������

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
    #region �����������

    public CareValueChangedEventArgs(CareValues CareValues, int ItemIndex, object OldValue)
    {
      FCareValues = CareValues;
      FItemIndex = ItemIndex;
      FOldValue = OldValue;
    }

    #endregion

    #region ��������

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
  /// ������ �������� ���� object, � ������� �������� ��������
  /// </summary>
  public class CareValues
  {
    #region �����������

    public CareValues(NamedList<CareItem> Items)
    {
      if (Items == null)
        throw new ArgumentNullException("Items");
      FItems = Items;
      FValues = new object[FItems.Count];
    }

    #endregion

    #region ��������

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

    #region ������ � ������

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
