using System;
using System.Collections.Generic;
using System.Text;
using AgeyevAV;
using AgeyevAV.ExtDB;

namespace Plants
{
  #region ������������ AttrValueSourceType

  /// <summary>
  /// �������� ��� ������ �������� ���������
  /// </summary>
  [Serializable]
  public enum AttrValueSourceType
  {
    /// <summary>
    /// ��� ���������. �������� �������� �������.
    /// ��� ���� ������������ ����������
    /// </summary>
    None = 0,

    /// <summary>
    /// ������������� ������ ��������, �������� ��������������� � ��������� "AttrType"
    /// </summary>
    List = 1,
  }

  #endregion

  #region ������������ ValueType

  /// <summary>
  /// ��� �������� ��������
  /// </summary>
  [Serializable]
  public enum ValueType
  {
    /// <summary>
    /// �����. �� ��������� - 0
    /// </summary>
    Integer = 0,

    /// <summary>
    /// � ��������� ������. �� ��������� - 0.0
    /// </summary>
    Double = 1,

    /// <summary>
    /// ��������. �� ��������� - 0m
    /// </summary>
    Decimal = 2,

    /// <summary>
    /// ����������. �� ��������� - false
    /// </summary>
    Boolean = 3,

    /// <summary>
    /// ���������. �� ��������� - ������ ������
    /// </summary>
    String = 4,

    /// <summary>
    /// ���� (��� �������). �� ��������� - �������� null
    /// </summary>
    Date = 5,

    /// <summary>
    /// ���� � �����. �� ��������� - �������� null
    /// </summary>
    DateTime = 6,

    MinValue = 0,
    MaxValue = 6
  }

  #endregion

  #region ������������ MovementKind

  [Serializable]
  public enum MovementKind
  {
    Add = 0,
    Move = 1,
    Remove = 2,
  }

  #endregion

  #region ������������ ActionKind

  [Serializable]
  public enum ActionKind
  {
    /// <summary>
    /// ������. �������� � ���� ActionName
    /// </summary>
    Other = 0,

    /// <summary>
    /// �������
    /// </summary>
    Planting = 1,

    /// <summary>
    /// ���������
    /// </summary>
    Replanting = 2,

    /// <summary>
    /// ���������
    /// </summary>
    Transshipment = 3,

    /// <summary>
    /// ����������: �������� �������� � �����
    /// </summary>
    TopCutting = 4,

    /// <summary>
    /// ����������
    /// </summary>
    Rooting = 5,

    /// <summary>
    /// ��������� ���. �����������
    /// </summary>
    Treatment = 6,

    /// <summary>
    /// ��������� ������ ������
    /// </summary>
    SoilReplace = 7,

    /// <summary>
    /// �������
    /// </summary>
    Trimming = 8,

    /// <summary>
    /// �������� ������
    /// </summary>
    LooseSoil = 9,

    /// <summary>
    /// ����� ��������
    /// </summary>
    Wash = 10,

    /// <summary>
    /// ���������
    /// </summary>
    AddPlant = 11,

    /// <summary>
    /// �����������
    /// </summary>
    RemovePlant = 12,

    MinValue = 0,
    MaxValue = 12
  }

  #endregion

  #region ������������ PlantMovementState

  /// <summary>
  /// �������� ������������ ���� "MovementState" � ������� "Plants"
  /// </summary>
  public enum PlantMovementState
  {
    /// <summary>
    /// ��� ����������� �������� (�������� ������)
    /// </summary>
    Draft,

    /// <summary>
    /// �������� ��������� - �������� ���-�� �����������
    /// </summary>
    Placed,

    /// <summary>
    /// ������ ����-��
    /// </summary>
    Given,

    /// <summary>
    /// ������
    /// </summary>
    Dead,

    /// <summary>
    /// ��������� � ������� ��������
    /// </summary>
    Merged,
  }

  #endregion

  /// <summary>
  /// ����������� ������� ������� � ������� �����2, �� ��������� � ������� ������
  /// </summary>
  public static class PlantTools
  {
    #region ������ ��� ������������

    #region MovementKind

    public static readonly string[] MovementNames = new string[] { 
      "������",
      "�����������",
      "�������",
    };

    public static string GetMovementName(MovementKind Kind)
    {
      if ((int)Kind >= 0 && (int)Kind < MovementNames.Length)
        return MovementNames[(int)Kind];
      else
        return "?? " + Kind.ToString();
    }

    public static readonly string[] MovementImageKeys = new string[] { 
      "SignAdd",
      "ArrowRight",
      "SignSubstract",
    };

    public static string GetMovementImageKey(MovementKind Kind)
    {
      if ((int)Kind >= 0 && (int)Kind < MovementImageKeys.Length)
        return MovementImageKeys[(int)Kind];
      else
        return "Error";
    }

    #endregion

    #region ActionKind

    public static readonly string[] ActionNames = new string[] { 
      "������",
      "������� ��� ���������� � ������",
      "���������",
      "���������",
      "�������� �������� � �����",
      "���������� (�� � ������)",
      "��������� �����������",
      "��������� ������ ������",
      "�������",
      "�������� ������",
      "����� ��������",
      "���������",
      "�����������"
    };

    public static string GetActionName(ActionKind Kind)
    {
      if ((int)Kind >= 0 && (int)Kind < ActionNames.Length)
        return ActionNames[(int)Kind];
      else
        return "?? " + Kind.ToString();
    }

    public static string GetActionName(ActionKind Kind, string OtherActionName, string RemedyName)
    {
      switch (Kind)
      {
        case ActionKind.Other:
          if (!String.IsNullOrEmpty(OtherActionName))
            return OtherActionName;
          break;
        case ActionKind.Treatment:
          if (!String.IsNullOrEmpty(RemedyName))
            return "��������� ���������� \"" + RemedyName + "\"";
          break;
      }
      return GetActionName(Kind);
    }

    /// <summary>
    /// ������ ��� �������� � ����������
    /// </summary>
    public static readonly string[] ActionImageKeys = new string[] { 
      "UnknownState", // Other
      "ActionPlanting",
      "ActionReplanting",
      "ActionTransshipment",
      "ActionTopCutting",
      "ActionRooting",
      "Remedy", // Treatment
      "ActionSoilReplace",
      "ActionTrimming",
      "ActionLooseSoil",
      "ActionWash",
      "ActionAddPlant",
      "ActionRemovePlant",
    };

    public static string GetActionImageKey(ActionKind Kind)
    {
      if ((int)Kind >= 0 && (int)Kind < ActionImageKeys.Length)
        return ActionImageKeys[(int)Kind];
      else
        return "Error";
    }

    #endregion

    #region ��� ��������

    public static readonly string[] ValueTypeNames = new string[] { 
        "����� �����",
        "����� � ��������� ������",
        "��������",
        "����������",
        "���������",
        "����",
        "���� � �����",
    };

    /// <summary>
    /// ��������� ������������� ��� ���� ��������� ��������, ��������, "����� �����"
    /// </summary>
    /// <param name="ValueType">��� ��������</param>
    /// <returns>��������� �������������</returns>
    public static string GetValueTypeName(ValueType ValueType)
    {
      if ((int)ValueType >= 0 && (int)ValueType < ValueTypeNames.Length)
        return ValueTypeNames[(int)ValueType];
      else
        return "?? " + ValueType.ToString();
    }

    /// <summary>
    /// �������������� ������������ ValueType � ��� Net framework
    /// </summary>
    /// <param name="ValueType"></param>
    /// <returns></returns>
    public static Type ValueTypeToType(ValueType ValueType)
    {
      switch (ValueType)
      {
        case ValueType.Integer: return typeof(Int32);
        case ValueType.Double: return typeof(Double);
        case ValueType.Decimal: return typeof(Decimal);
        case ValueType.Boolean: return typeof(Boolean);
        case ValueType.String: return typeof(String);
        case ValueType.Date:
        case ValueType.DateTime: return typeof(DateTime);
        default:
          throw new ArgumentException("����������� �������� ValueType=" + ValueType.ToString(), "ValueType");
      }
    }

    public static object GetDefaultValue(ValueType ValueType)
    {
      switch (ValueType)
      {
        case ValueType.Integer: return 0;
        case ValueType.Double: return 0.0;
        case ValueType.Decimal: return 0m;
        case ValueType.Boolean: return false;
        case ValueType.String: return String.Empty;
        case ValueType.Date: return null;
        case ValueType.DateTime: return null;
        default:
          throw new ArgumentException("����������� �������� ValueType=" + ValueType.ToString());
      }
    }

    /// <summary>
    /// ���������� true, ���� �������� Value �������� ��������� �� ���������
    /// </summary>
    /// <param name="Value"></param>
    /// <param name="ValueType"></param>
    /// <returns></returns>
    public static bool IsDefaultValue(object Value, ValueType ValueType)
    {
      return Compare(Value, GetDefaultValue(ValueType), ValueType) == 0;
    }

    /// <summary>
    /// ���������� true, ���� �������� ����� ���������� ���
    /// </summary>
    /// <param name="Value">��������</param>
    /// <returns>true, ���� �������� ����� ���������� ���</returns>
    public static bool IsValidValue(object Value)
    {
      ValueType ValueType;
      return IsValidValue(Value, out ValueType);
    }

    public static bool IsValidValue(object Value, out ValueType ValueType)
    {
      if (Value == null)
      {
        ValueType = ValueType.Date; // ����� �� ���� � DateTime
        return true;
      }

      if (Value is Int32)
      {
        ValueType = ValueType.Integer;
        return true;
      }

      if (Value is Double)
      {
        ValueType = ValueType.Double;
        return true;
      }

      if (Value is Decimal)
      {
        ValueType = ValueType.Decimal;
        return true;
      }

      if (Value is Boolean)
      {
        ValueType = ValueType.Boolean;
        return true;
      }

      if (Value is String)
      {
        ValueType = ValueType.String;
        return true;
      }

      if (Value is DateTime)
      {
        if (((DateTime)Value).TimeOfDay.Ticks == 0)
          ValueType = ValueType.Date;
        else
          ValueType = ValueType.DateTime;
        return true;
      }

      ValueType = (ValueType)(-1);
      return false;
    }

    /// <summary>
    /// ������� ��������� ��������
    /// </summary>
    /// <param name="Value1"></param>
    /// <param name="Value2"></param>
    /// <param name="ValueType"></param>
    /// <returns></returns>
    public static int Compare(object Value1, object Value2, ValueType ValueType)
    {
      switch (ValueType)
      {
        case ValueType.Integer:
          return DataTools.GetInt(Value1).CompareTo(DataTools.GetInt(Value2));
        case ValueType.Double:
          return DataTools.GetDouble(Value1).CompareTo(DataTools.GetDouble(Value2));
        case ValueType.Decimal:
          return DataTools.GetDecimal(Value1).CompareTo(DataTools.GetDecimal(Value2));
        case ValueType.Boolean:
          return DataTools.GetBool(Value1).CompareTo(DataTools.GetBool(Value2));
        case ValueType.String:
          return DataTools.GetString(Value1).CompareTo(DataTools.GetString(Value2));
        case ValueType.Date:
        case ValueType.DateTime:
          if (Value1 == null)
          {
            if (Value2 == null)
              return 0;
            else
              return -1;
          }
          else
          {
            if (Value2 == null)
              return +1;
            else
              return ((DateTime)Value1).CompareTo((DateTime)Value2);
          }
        default:
          throw new ArgumentException("����������� ValueType");
      }
    }


    public static int Compare(object Value1, object Value2)
    {
      if (Value1 == null)
      {
        if (Value2 == null)
          return 0;
        else
          Value1 = DataTools.GetEmptyValue(Value2.GetType());
      }
      else if (Value2 == null)
        Value2 = DataTools.GetEmptyValue(Value1.GetType());

      if (Value1 is DateTime)
      {
        if (Value2 == null)
          return +1;
        else
          return ((DateTime)Value1).CompareTo((DateTime)Value2);
      }
      if (Value2 is DateTime)
      {
        if (Value1 == null)
          return -1;
      }

#if DEBUG
      if (Value1 == null || Value2 == null)
        throw new BugException("�������� null ���� �� ������");
#endif

      ValueType vt1, vt2;
      if (!IsValidValue(Value1, out vt1))
        throw new ArgumentException("�������� 1 ����� ������������ ��� " + Value1.GetType().ToString(), "Value1");
      if (!IsValidValue(Value2, out vt2))
        throw new ArgumentException("�������� 2 ����� ������������ ��� " + Value2.GetType().ToString(), "Value2");

      if (vt1 == ValueType.DateTime || vt2 == ValueType.DateTime)
        return Compare(Value1, Value2, ValueType.DateTime);

      if (vt1 == ValueType.Date || vt2 == ValueType.Date)
        return Compare(Value1, Value2, ValueType.Date);

      if (vt1 == ValueType.String || vt2 == ValueType.String)
        return Compare(Value1, Value2, ValueType.String);

      if (vt1 == ValueType.Decimal || vt2 == ValueType.Decimal)
        return Compare(Value1, Value2, ValueType.Decimal);

      if (vt1 == ValueType.Double || vt2 == ValueType.Double)
        return Compare(Value1, Value2, ValueType.Double);

      if (vt1 == ValueType.Integer || vt2 == ValueType.Integer)
        return Compare(Value1, Value2, ValueType.Integer);

      if (vt1 == ValueType.Boolean || vt2 == ValueType.Boolean)
        return Compare(Value1, Value2, ValueType.Boolean);

      throw new BugException("���������������� ��� ��������. ValueType1=" + vt1.ToString() + ", ValueType2=" + vt2.ToString());
    }

    /// <summary>
    /// ���������� true, ���� ��� ��������� ���� ������ ����� ��������� ������������ ��������
    /// (Integer, Double � Decimal)
    /// </summary>
    /// <param name="ValueType">��� ��������</param>
    /// <returns>true, ���� ����� ����������� ��������</returns>
    public static bool IsSummableValueType(ValueType ValueType)
    {
      switch (ValueType)
      {
        case ValueType.Integer:
        case ValueType.Double:
        case ValueType.Decimal:
          return true;
        default:
          return false;
      }
    }


    #endregion

    #region ������ �������� ��� ��������

    public static readonly string[] AttrValueSourceTypeNames = new string[] { 
      "����� �������� (������ ����)",
      "������������� ������ ��������",
    };

    public static string GetAttrValueSourceTypeName(AttrValueSourceType Type)
    {
      if ((int)Type >= 0 && (int)Type < AttrValueSourceTypeNames.Length)
        return AttrValueSourceTypeNames[(int)Type];
      else
        return "?? " + Type.ToString();
    }

    #endregion

    #region PlantMovementState

    public static readonly string[] PlantMovementStateNames = new string[] { 
      "��������",
      "���������",
      "������",
      "������",
      "���������",
    };

    public static string GetPlantMovementStateName(PlantMovementState State)
    {
      if ((int)State >= 0 && (int)State < PlantMovementStateNames.Length)
        return PlantMovementStateNames[(int)State];
      else
        return "?? " + State.ToString();
    }

    public static readonly string[] PlantMovementStateImageKeys = new string[] { 
      "UnknownState",
      "Plant",
      "Contra",
      "No",
      "ArrowDownThenRight",
    };

    public static string GetPlantMovementStateImageKey(PlantMovementState State)
    {
      if ((int)State >= 0 && (int)State < PlantMovementStateImageKeys.Length)
        return PlantMovementStateImageKeys[(int)State];
      else
        return "Error";
    }

    #endregion

    #endregion

    #region ��������

    /// <summary>
    /// �������������� �������� ��������� ���� � ������.
    /// ��� ������ �� ��������� ������� ������ ������
    /// </summary>
    /// <param name="Value">��������</param>
    /// <param name="ValueType">��� ������</param>
    /// <returns>��������� �������������</returns>
    public static string ToString(object Value, ValueType ValueType)
    {
      return ToString(Value, ValueType, String.Empty);
    }

    /// <summary>
    /// �������������� �������� ��������� ���� � ������.
    /// ��� ����� ������������ ����������� ������� ����� ������� (������� �� �������� ��).
    /// ��� �������������� �������� � �������-����������� ������ ����������� ������ ������ Accoo2StdConvert.
    /// </summary>
    /// <param name="Value">��������</param>
    /// <param name="ValueType">��� ������</param>
    /// <param name="Format">������</param>
    /// <returns>��������� �������������</returns>
    public static string ToString(object Value, ValueType ValueType, string Format)
    {
      if (Value == null)
        Value = PlantTools.GetDefaultValue(ValueType);
      try
      {
        switch (ValueType)
        {
          case ValueType.Date:
            if (Value == null)
              return String.Empty;
            else if (String.IsNullOrEmpty(Format))
              return ((DateTime)Value).ToString("dd/MM/yyyy"); // 03.07.2017
            else
              return ((DateTime)Value).ToString(Format);
          case ValueType.DateTime:
            if (Value == null)
              return String.Empty;
            else if (String.IsNullOrEmpty(Format))
              return ((DateTime)Value).ToString("dd/MM/yyyy HH:mm:ss"); // 14.03.2018
            else
              return ((DateTime)Value).ToString(Format);
          case ValueType.String:
            return (string)Value;
          case ValueType.Boolean:
            return Convert.ToBoolean(Value) ? "1" : "0";
          case ValueType.Integer:
            return Convert.ToInt32(Value).ToString(Format);
          case ValueType.Double:
            return Convert.ToDouble(Value).ToString(Format);
          case ValueType.Decimal:
            return Convert.ToDecimal(Value).ToString(Format);
          default:
            return Value.ToString();
        }
      }
      catch
      {
        if (Value == null)
          return String.Empty;
        else
          return Value.ToString();
      }
    }

    /// <summary>
    /// �������������� ������ � �������� ��� ������.
    /// ������ ������ ������������� � �������� �� ��������� ��� ��������� ����.
    /// ���� �������������� ����������, ������������ InvalidCastException.
    /// ��� ����� ������������ ����������� ������� ����� ������� (������� �� �������� ��).
    /// ��� �������������� �������� �� �������-������������ ������� ����������� ������ ������ Accoo2StdConvert.
    /// </summary>
    /// <param name="Text">������������� �����</param>
    /// <param name="ValueType">��������� ��� ������</param>
    /// <returns>�������� ���������� ����</returns>
    public static object Parse(string Text, ValueType ValueType)
    {
      object Res;
      if (TryParse(Text, ValueType, out Res))
        return Res;
      else
        throw new InvalidCastException("������ \"" + Text + "\" ������ ������������� � ���� \"" + PlantTools.GetValueTypeName(ValueType) + "\"");
    }

    /// <summary>
    /// �������������� ������ � �������� ��� ������.
    /// ������ ������ ������������� � �������� �� ��������� ��� ��������� ����.
    /// ���� �������������� ����������, ������������ false
    /// ��� ����� ������������ ����������� ������� ����� ������� (������� �� �������� ��).
    /// ��� �������������� �������� �� �������-������������ ������� ����������� ������ ������ Accoo2StdConvert.
    /// </summary>
    /// <param name="Text">������������� �����</param>
    /// <param name="ValueType">��������� ��� ������</param>
    /// <param name="Res">���� ������������ �������� ���������� ���� ��� null � ������ ������</param>
    /// <returns>true, ���� �������������� ������� ���������</returns>
    public static bool TryParse(string Text, ValueType ValueType, out object Res)
    {
      if (String.IsNullOrEmpty(Text))
      {
        Res = PlantTools.GetDefaultValue(ValueType);
        return true;
      }

      Res = null;

      switch (ValueType)
      {
        case ValueType.Integer:
          int iv;
          if (int.TryParse(Text, out iv))
          {
            Res = iv;
            return true;
          }
          else
            return false;

        case ValueType.Double:
          double dv;
          if (double.TryParse(Text, out dv))
          {
            Res = dv;
            return true;
          }
          else
            return false;

        case ValueType.Decimal:
          decimal mv;
          if (decimal.TryParse(Text, out mv))
          {
            Res = mv;
            return true;
          }
          else
            return false;

        case ValueType.Boolean:
          if (Text == "1")
          {
            Res = true;
            return true;
          }
          if (Text == "0")
          {
            Res = false;
            return true;
          }

          bool bv;
          if (bool.TryParse(Text, out bv))
          {
            Res = bv;
            return true;
          }
          else
            return false;

        case ValueType.String:
          Res = Text;
          return true;

        case ValueType.Date:
        case ValueType.DateTime:
          DateTime dtv;
          if (DateTime.TryParse(Text, out dtv))
          {
            if (ValueType == ValueType.DateTime)
              dtv = dtv.Date;
            Res = dtv;
            return true;
          }
          else
            return false;

        default:
          throw new ArgumentException("����������� ValueType", "ValueType");
      }
    }

    #endregion

    #region �������� ����������

    /// <summary>
    /// ������������ ����� ���������� �������� ����������������� ��������, ����������� � ���� Value.
    /// ����� ������� �������� ����� �����. ��� ����������� � MEMO-���� LongValue
    /// </summary>
    public const int AttrValueShortMaxLength = 30;

    /// <summary>
    /// ��������� ������������ ���� ��������� �������� ��� �������� � ���� ������
    /// </summary>
    /// <param name="SourceType">��� ��������� ������ ��� �������� ��������</param>
    /// <param name="ValueType">��� ������ ��������</param>
    /// <returns>true, ���� ���� ����� �������������� ��� ������ ��������</returns>
    public static bool IsValidAttrSourceType(AttrValueSourceType SourceType, ValueType ValueType)
    {
      if (SourceType == AttrValueSourceType.None)
        return true;

      switch (ValueType)
      {
        case ValueType.String:
          return true;
        case ValueType.Integer:
          switch (SourceType)
          {
            case AttrValueSourceType.List:
              return true;
            default:
              return false;
          }
        case ValueType.Boolean:
          return false;
        default:
          return SourceType == AttrValueSourceType.List;
      }
    }

    #region ������ �������������� �������� �/�� ���������� �������������

    /// <summary>
    /// �������������� �������� � ��������� ������ ��� �������� � ���� ������
    /// </summary>
    /// <param name="Value">��������</param>
    /// <param name="ValueType">��� ��������</param>
    /// <returns>��������� �������������</returns>
    public static string ValueToSaveableString(object Value, ValueType ValueType)
    {
      switch (ValueType)
      {
        case ValueType.String:
          return DataTools.GetString(Value);
        case ValueType.Integer:
          return StdConvert.ToString(DataTools.GetInt(Value));
        case ValueType.Double:
          return StdConvert.ToString(DataTools.GetDouble(Value));
        case ValueType.Decimal:
          return StdConvert.ToString(DataTools.GetDecimal(Value));
        case ValueType.Boolean:
          return DataTools.GetBool(Value) ? "1" : "0";

        case ValueType.Date:
          DateTime? dt1 = DataTools.GetNullableDateTime(Value);
          if (dt1.HasValue)
            return StdConvert.ToString(dt1.Value, false);
          else
            return String.Empty;

        case ValueType.DateTime:
          DateTime? dt2 = DataTools.GetNullableDateTime(Value);
          if (dt2.HasValue)
            return StdConvert.ToString(dt2.Value, true);
          else
            return String.Empty;

        default:
          throw new ArgumentException("����������� ValueType=" + ValueType.ToString(), "ValueType");
      }
    }

    /// <summary>
    /// �������������� �������� ��������, ������������ � ���� ��������� ������ � ���� ������,
    /// � �������� ��������� ����
    /// </summary>
    /// <param name="s">�������� ��������� �������� ��������</param>
    /// <param name="ValueType">��� ������ ��������</param>
    /// <returns>�������� ��������, ����������� � ���������� ����</returns>
    public static object ValueFromSaveableString(string s, ValueType ValueType)
    {
      if (String.IsNullOrEmpty(s))
        return PlantTools.GetDefaultValue(ValueType);

      switch (ValueType)
      {
        case ValueType.String:
          return s;
        case ValueType.Integer:
          return StdConvert.ToInt32(s);
        case ValueType.Double:
          return StdConvert.ToDouble(s);
        case ValueType.Decimal:
          return StdConvert.ToDecimal(s);
        case ValueType.Boolean:
          return DataTools.GetBool(s);

        case ValueType.Date:
          return StdConvert.ToDateTime(s, false);
        case ValueType.DateTime:
          return StdConvert.ToDateTime(s, true);

        default:
          throw new ArgumentException("����������� ValueType=" + ValueType.ToString(), "ValueType");
      }
    }

    #endregion

    #endregion

    #region �����������

    /// <summary>
    /// ������, � ������� ����������� ���������
    /// </summary>
    public static readonly System.Drawing.Size ThumbnailSize = new System.Drawing.Size(128, 128);

    #endregion

    #region �������

    /// <summary>
    /// ��������� � ������ <paramref name="Filters"/> ������ �� �������� ���.
    /// ���� �� ���� ���� ����� �����, ����� ���� �� ������������ ������� DateRangeFilter.
    /// � ��������� ���� �������� � ���� ��������� �� ���� ����� Date1 � Date2, 
    /// ��������� ����� ������� ������
    /// </summary>
    /// <param name="Filters">������ ��� ���������� ��������</param>
    /// <param name="FirstDate">��������� ���� �������</param>
    /// <param name="LastDate">�������� ���� �������</param>
    public static void AddDateRangeFilter(List<DBxFilter> Filters, DateTime? FirstDate, DateTime? LastDate)
    {
      //if (FirstDate.HasValue)
      //  Filters.Add(new ValueFilter("Date1", FirstDate.Value, ValueFilterKind.GreaterOrEqualThan));
      //if (LastDate.HasValue)
      //  Filters.Add(new ValueFilter("Date2", LastDate.Value, ValueFilterKind.LessOrEqualThan));

      // 21.05.2019
      if (FirstDate.HasValue)
        Filters.Add(new ValueFilter("Date2", FirstDate.Value, CompareKind.GreaterOrEqualThan));
      if (LastDate.HasValue)
        Filters.Add(new ValueFilter("Date1", LastDate.Value, CompareKind.LessOrEqualThan));
    }

    #endregion

    #region ������ � ������ ��� ��������

    /// <summary>
    /// ���������� true, ���� ��� �������� ����� ���� ����� �����
    /// </summary>
    /// <param name="Kind">��������</param>
    /// <param name="ForFinal">True, ���� ������������� ��������� ���������.
    /// � ���� ������ ��������� ������ ������ �� ����������� SoilReplace</param>
    /// <returns>true, ���� ����� �������� � ��������</returns>
    public static bool IsSoilAppliable(ActionKind Kind, bool ForFinal)
    {
      switch (Kind)
      {
        case ActionKind.Planting:
        case ActionKind.Replanting:
        case ActionKind.Transshipment:
        case ActionKind.Other:
          return true;
        case ActionKind.SoilReplace:
          return !ForFinal;
        default:
          return false;
      }
    }

    /// <summary>
    /// ���������� true, ���� ��� �������� ����� ���� ����� ������
    /// </summary>
    /// <param name="Kind">��������</param>
    /// <param name="ForFinal">True, ���� ������������� ��������� ���������.
    /// �� �����������</param>
    /// <returns>true, ���� ����� �������� � ��������</returns>
    public static bool IsPotKindAppliable(ActionKind Kind, bool ForFinal)
    {
      switch (Kind)
      {
        case ActionKind.Planting:
        case ActionKind.Replanting:
        case ActionKind.Transshipment:
        case ActionKind.Other:
          return true;
        default:
          return false;
      }
    }

    /// <summary>
    /// ��������� ������� ������� ��� ������ IsSoilAppliable()
    /// </summary>
    public static int[] GetSoilAppliableIntArray()
    {
      List<int> lst = new List<int>();
      for (ActionKind Kind = ActionKind.MinValue; Kind <= ActionKind.MaxValue; Kind++)
      {
        if (IsSoilAppliable(Kind, false))
          lst.Add((int)Kind);
      }
      return lst.ToArray();
    }

    /// <summary>
    /// ��������� ������� ������� ��� ������ IsPotKindAppliable()
    /// </summary>
    public static int[] GetPotKindAppliableIntArray()
    {
      List<int> lst = new List<int>();
      for (ActionKind Kind = ActionKind.MinValue; Kind <= ActionKind.MaxValue; Kind++)
      {
        if (IsPotKindAppliable(Kind, false))
          lst.Add((int)Kind);
      }
      return lst.ToArray();
    }

    #endregion
  }


  /// <summary>
  /// �������������� ��������, ����������� � ������ � / �� ��������� �������������.
  /// �� ������� �� �������� ������������ �������.
  /// </summary>
  public static class PlantsStdConvert
  {
    #region ������� ��������������

    public static string ToString(object Value, ValueType ValueType)
    {
      switch (ValueType)
      {
        case ValueType.Integer:
          return StdConvert.ToString(DataTools.GetInt(Value));
        case ValueType.Double:
          return StdConvert.ToString(DataTools.GetDouble(Value));
        case ValueType.Decimal:
          return StdConvert.ToString(DataTools.GetDecimal(Value));
        case ValueType.Boolean:
          return DataTools.GetBool(Value) ? "1" : "0";
        case ValueType.String:
          return DataTools.GetString(Value);
        case ValueType.Date:
        case ValueType.DateTime:
          DateTime? dt = DataTools.GetNullableDateTime(Value);
          if (dt.HasValue)
            return StdConvert.ToString(dt.Value, ValueType == ValueType.DateTime);
          else
            return string.Empty;
        default:
          throw new ArgumentException("������������ ValueType=" + ValueType.ToString(), "ValueType");
      }
    }

    public static bool TryParse(string Text, ValueType ValueType, out object Value)
    {
      if (String.IsNullOrEmpty(Text))
      {
        Value = PlantTools.GetDefaultValue(ValueType);
        return true;
      }

      switch (ValueType)
      {
        case ValueType.Integer:
          int vi;
          if (StdConvert.TryParse(Text, out vi))
          {
            Value = vi;
            return true;
          }
          else
          {
            decimal vdc2;
            if (StdConvert.TryParse(Text, out vdc2))
            {
              vi = (int)Math.Round(vdc2, 0, MidpointRounding.AwayFromZero);
              Value = vi;
              return true;
            }

            Value = 0;
            return false;
          }

        case ValueType.Double:
          double vd;
          if (StdConvert.TryParse(Text, out vd))
          {
            Value = vd;
            return true;
          }
          else
          {
            Value = 0.0;
            return false;
          }

        case ValueType.Decimal:
          decimal vd�;
          if (StdConvert.TryParse(Text, out vd�))
          {
            Value = vd�;
            return true;
          }
          else
          {
            Value = 0m;
            return false;
          }

        case ValueType.Boolean:
          int vb;
          if (StdConvert.TryParse(Text, out vb))
          {
            Value = vb != 0;
            return true;
          }
          else
          {
            Value = false;
            return false;
          }

        case ValueType.String:
          Value = Text;
          return true;

        case ValueType.Date:
        case ValueType.DateTime:
          DateTime dt;
          if (StdConvert.TryParse(Text, out dt, ValueType == ValueType.DateTime))
          {
            Value = dt;
            return true;
          }
          else
          {
            Value = null;
            return false;
          }
        default:
          throw new ArgumentException("������������ ValueType=" + ValueType.ToString(), "ValueType");
      }
    }

    #endregion
  }
}
