using System;
using System.Collections.Generic;
using System.Text;
using FreeLibSet.Data;
using FreeLibSet.Core;

namespace Plants
{
  #region Перечисление AttrValueSourceType

  /// <summary>
  /// Источник для списка значений атрибутов
  /// </summary>
  [Serializable]
  public enum AttrValueSourceType
  {
    /// <summary>
    /// Нет источника. Значение вводится вручную.
    /// Для даты используется календарик
    /// </summary>
    None = 0,

    /// <summary>
    /// Фиксированный список значений, заданный непосредственно в документе "AttrType"
    /// </summary>
    List = 1,
  }

  #endregion

  #region Перечисление ValueType

  /// <summary>
  /// Тип значения атрибута
  /// </summary>
  [Serializable]
  public enum ValueType
  {
    /// <summary>
    /// Целое. По умолчанию - 0
    /// </summary>
    Integer = 0,

    /// <summary>
    /// С плавающей точкой. По умолчанию - 0.0
    /// </summary>
    Double = 1,

    /// <summary>
    /// Денежное. По умолчанию - 0m
    /// </summary>
    Decimal = 2,

    /// <summary>
    /// Логическое. По умолчанию - false
    /// </summary>
    Boolean = 3,

    /// <summary>
    /// Строковое. По умолчанию - пустая строка
    /// </summary>
    String = 4,

    /// <summary>
    /// Дата (без времени). По умолчанию - значение null
    /// </summary>
    Date = 5,

    /// <summary>
    /// Дата и время. По умолчанию - значение null
    /// </summary>
    DateTime = 6,

    MinValue = 0,
    MaxValue = 6
  }

  #endregion

  #region Перечисление MovementKind

  [Serializable]
  public enum MovementKind
  {
    Add = 0,
    Move = 1,
    Remove = 2,
  }

  #endregion

  #region Перечисление ActionKind

  [Serializable]
  public enum ActionKind
  {
    /// <summary>
    /// Другое. Задается в поле ActionName
    /// </summary>
    Other = 0,

    /// <summary>
    /// Посадка
    /// </summary>
    Planting = 1,

    /// <summary>
    /// Пересадка
    /// </summary>
    Replanting = 2,

    /// <summary>
    /// Перевалка
    /// </summary>
    Transshipment = 3,

    /// <summary>
    /// Омоложение: срезание верхушки и сушка
    /// </summary>
    TopCutting = 4,

    /// <summary>
    /// Укоренение
    /// </summary>
    Rooting = 5,

    /// <summary>
    /// Обработка хим. препаратами
    /// </summary>
    Treatment = 6,

    /// <summary>
    /// Частичная замена грунта
    /// </summary>
    SoilReplace = 7,

    /// <summary>
    /// Обрезка
    /// </summary>
    Trimming = 8,

    /// <summary>
    /// Рыхление грунта
    /// </summary>
    LooseSoil = 9,

    /// <summary>
    /// Мытье растения
    /// </summary>
    Wash = 10,

    /// <summary>
    /// Допосадка
    /// </summary>
    AddPlant = 11,

    /// <summary>
    /// Отсаживание
    /// </summary>
    RemovePlant = 12,

    MinValue = 0,
    MaxValue = 12
  }

  #endregion

  #region Перечисление PlantMovementState

  /// <summary>
  /// Значение вычисляемого поля "MovementState" в таблице "Plants"
  /// </summary>
  public enum PlantMovementState
  {
    /// <summary>
    /// Нет перемещений растения (черновая запись)
    /// </summary>
    Draft,

    /// <summary>
    /// Основное состояние - растение где-то расположено
    /// </summary>
    Placed,

    /// <summary>
    /// Отдано кому-то
    /// </summary>
    Given,

    /// <summary>
    /// Сдохло
    /// </summary>
    Dead,

    /// <summary>
    /// Подсажено к другому растению
    /// </summary>
    Merged,
  }

  #endregion

  /// <summary>
  /// Статические функции клиента и сервера АССОО2, не связанные с выводом текста
  /// </summary>
  public static class PlantTools
  {
    #region Строки для перечислений

    #region MovementKind

    public static readonly string[] MovementNames = new string[] { 
      "Приход",
      "Перемещение",
      "Выбытие",
    };

    public static string GetMovementName(MovementKind kind)
    {
      if ((int)kind >= 0 && (int)kind < MovementNames.Length)
        return MovementNames[(int)kind];
      else
        return "?? " + kind.ToString();
    }

    public static readonly string[] MovementImageKeys = new string[] { 
      "SignAdd",
      "ArrowRight",
      "SignSubstract",
    };

    public static string GetMovementImageKey(MovementKind kind)
    {
      if ((int)kind >= 0 && (int)kind < MovementImageKeys.Length)
        return MovementImageKeys[(int)kind];
      else
        return "Error";
    }

    #endregion

    #region ActionKind

    public static readonly string[] ActionNames = new string[] { 
      "Другое",
      "Посадка или укоренение в грунте",
      "Пересадка",
      "Перевалка",
      "Срезание верхушки и сушка",
      "Укоренение (не в грунте)",
      "Обработка препаратами",
      "Частичная замена грунта",
      "Обрезка",
      "Рыхление грунта",
      "Мытье растения",
      "Допосадка",
      "Отсаживание"
    };

    public static string GetActionName(ActionKind kind)
    {
      if ((int)kind >= 0 && (int)kind < ActionNames.Length)
        return ActionNames[(int)kind];
      else
        return "?? " + kind.ToString();
    }

    public static string GetActionName(ActionKind kind, string otherActionName, string remedyName)
    {
      switch (kind)
      {
        case ActionKind.Other:
          if (!String.IsNullOrEmpty(otherActionName))
            return otherActionName;
          break;
        case ActionKind.Treatment:
          if (!String.IsNullOrEmpty(remedyName))
            return "Обработка препаратом \"" + remedyName + "\"";
          break;
      }
      return GetActionName(kind);
    }

    /// <summary>
    /// Значки для действий с растениями
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

    public static string GetActionImageKey(ActionKind kind)
    {
      if ((int)kind >= 0 && (int)kind < ActionImageKeys.Length)
        return ActionImageKeys[(int)kind];
      else
        return "Error";
    }

    #endregion

    #region Тип значения

    public static readonly string[] ValueTypeNames = new string[] { 
        "Целое число",
        "Число с плавающей точкой",
        "Денежный",
        "Логический",
        "Строковый",
        "Дата",
        "Дата и время",
    };

    /// <summary>
    /// Текстовое представление для типа хранимого значения, например, "Целое число"
    /// </summary>
    /// <param name="ValueType">Тип значения</param>
    /// <returns>Текстовое представление</returns>
    public static string GetValueTypeName(ValueType valueType)
    {
      if ((int)valueType >= 0 && (int)valueType < ValueTypeNames.Length)
        return ValueTypeNames[(int)valueType];
      else
        return "?? " + valueType.ToString();
    }

    /// <summary>
    /// Преобразование перечисления ValueType в тип Net framework
    /// </summary>
    /// <param name="valueType"></param>
    /// <returns></returns>
    public static Type ValueTypeToType(ValueType valueType)
    {
      switch (valueType)
      {
        case ValueType.Integer: return typeof(Int32);
        case ValueType.Double: return typeof(Double);
        case ValueType.Decimal: return typeof(Decimal);
        case ValueType.Boolean: return typeof(Boolean);
        case ValueType.String: return typeof(String);
        case ValueType.Date:
        case ValueType.DateTime: return typeof(DateTime);
        default:
          throw new ArgumentException("Неизвестное значение ValueType=" + valueType.ToString(), "ValueType");
      }
    }

    public static object GetDefaultValue(ValueType valueType)
    {
      switch (valueType)
      {
        case ValueType.Integer: return 0;
        case ValueType.Double: return 0.0;
        case ValueType.Decimal: return 0m;
        case ValueType.Boolean: return false;
        case ValueType.String: return String.Empty;
        case ValueType.Date: return null;
        case ValueType.DateTime: return null;
        default:
          throw new ArgumentException("Неизвестное значение ValueType=" + valueType.ToString());
      }
    }

    /// <summary>
    /// Возвращает true, если значение Value является значением по умолчанию
    /// </summary>
    /// <param name="value"></param>
    /// <param name="valueType"></param>
    /// <returns></returns>
    public static bool IsDefaultValue(object value, ValueType valueType)
    {
      return Compare(value, GetDefaultValue(valueType), valueType) == 0;
    }

    /// <summary>
    /// Возврашает true, если значение имеет допустимый тип
    /// </summary>
    /// <param name="value">Значение</param>
    /// <returns>true, если значение имеет допустимый тип</returns>
    public static bool IsValidValue(object value)
    {
      ValueType ValueType;
      return IsValidValue(value, out ValueType);
    }

    public static bool IsValidValue(object value, out ValueType valueType)
    {
      if (value == null)
      {
        valueType = ValueType.Date; // могло бы быть и DateTime
        return true;
      }

      if (value is Int32)
      {
        valueType = ValueType.Integer;
        return true;
      }

      if (value is Double)
      {
        valueType = ValueType.Double;
        return true;
      }

      if (value is Decimal)
      {
        valueType = ValueType.Decimal;
        return true;
      }

      if (value is Boolean)
      {
        valueType = ValueType.Boolean;
        return true;
      }

      if (value is String)
      {
        valueType = ValueType.String;
        return true;
      }

      if (value is DateTime)
      {
        if (((DateTime)value).TimeOfDay.Ticks == 0)
          valueType = ValueType.Date;
        else
          valueType = ValueType.DateTime;
        return true;
      }

      valueType = (ValueType)(-1);
      return false;
    }

    /// <summary>
    /// Функция сравнения значений
    /// </summary>
    public static int Compare(object value1, object value2, ValueType valueType)
    {
      switch (valueType)
      {
        case ValueType.Integer:
          return DataTools.GetInt(value1).CompareTo(DataTools.GetInt(value2));
        case ValueType.Double:
          return DataTools.GetDouble(value1).CompareTo(DataTools.GetDouble(value2));
        case ValueType.Decimal:
          return DataTools.GetDecimal(value1).CompareTo(DataTools.GetDecimal(value2));
        case ValueType.Boolean:
          return DataTools.GetBool(value1).CompareTo(DataTools.GetBool(value2));
        case ValueType.String:
          return DataTools.GetString(value1).CompareTo(DataTools.GetString(value2));
        case ValueType.Date:
        case ValueType.DateTime:
          if (value1 == null)
          {
            if (value2 == null)
              return 0;
            else
              return -1;
          }
          else
          {
            if (value2 == null)
              return +1;
            else
              return ((DateTime)value1).CompareTo((DateTime)value2);
          }
        default:
          throw new ArgumentException("Неизвестный ValueType");
      }
    }


    public static int Compare(object value1, object value2)
    {
      if (value1 == null)
      {
        if (value2 == null)
          return 0;
        else
          value1 = DataTools.GetEmptyValue(value2.GetType());
      }
      else if (value2 == null)
        value2 = DataTools.GetEmptyValue(value1.GetType());

      if (value1 is DateTime)
      {
        if (value2 == null)
          return +1;
        else
          return ((DateTime)value1).CompareTo((DateTime)value2);
      }
      if (value2 is DateTime)
      {
        if (value1 == null)
          return -1;
      }

#if DEBUG
      if (value1 == null || value2 == null)
        throw new BugException("Значений null быть не должно");
#endif

      ValueType vt1, vt2;
      if (!IsValidValue(value1, out vt1))
        throw new ArgumentException("Значение 1 имеет недопустимый тип " + value1.GetType().ToString(), "Value1");
      if (!IsValidValue(value2, out vt2))
        throw new ArgumentException("Значение 2 имеет недопустимый тип " + value2.GetType().ToString(), "Value2");

      if (vt1 == ValueType.DateTime || vt2 == ValueType.DateTime)
        return Compare(value1, value2, ValueType.DateTime);

      if (vt1 == ValueType.Date || vt2 == ValueType.Date)
        return Compare(value1, value2, ValueType.Date);

      if (vt1 == ValueType.String || vt2 == ValueType.String)
        return Compare(value1, value2, ValueType.String);

      if (vt1 == ValueType.Decimal || vt2 == ValueType.Decimal)
        return Compare(value1, value2, ValueType.Decimal);

      if (vt1 == ValueType.Double || vt2 == ValueType.Double)
        return Compare(value1, value2, ValueType.Double);

      if (vt1 == ValueType.Integer || vt2 == ValueType.Integer)
        return Compare(value1, value2, ValueType.Integer);

      if (vt1 == ValueType.Boolean || vt2 == ValueType.Boolean)
        return Compare(value1, value2, ValueType.Boolean);

      throw new BugException("Необрабатываемый тип значений. ValueType1=" + vt1.ToString() + ", ValueType2=" + vt2.ToString());
    }

    /// <summary>
    /// Возвращает true, если для заданного типа данных можно выполнять суммирование значений
    /// (Integer, Double и Decimal)
    /// </summary>
    /// <param name="valueType">Тип значения</param>
    /// <returns>true, если можно суммировать значения</returns>
    public static bool IsSummableValueType(ValueType valueType)
    {
      switch (valueType)
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

    #region Список значений для атрибута

    public static readonly string[] AttrValueSourceTypeNames = new string[] { 
      "Любые значения (ручной ввод)",
      "Фиксированный список значений",
    };

    public static string GetAttrValueSourceTypeName(AttrValueSourceType sourceType)
    {
      if ((int)sourceType >= 0 && (int)sourceType < AttrValueSourceTypeNames.Length)
        return AttrValueSourceTypeNames[(int)sourceType];
      else
        return "?? " + sourceType.ToString();
    }

    #endregion

    #region PlantMovementState

    public static readonly string[] PlantMovementStateNames = new string[] { 
      "Черновик",
      "Размещено",
      "Отдано",
      "Умерло",
      "Подсажено",
    };

    public static string GetPlantMovementStateName(PlantMovementState state)
    {
      if ((int)state >= 0 && (int)state < PlantMovementStateNames.Length)
        return PlantMovementStateNames[(int)state];
      else
        return "?? " + state.ToString();
    }

    public static readonly string[] PlantMovementStateImageKeys = new string[] { 
      "UnknownState",
      "Plant",
      "Contra",
      "No",
      "ArrowDownThenRight",
    };

    public static string GetPlantMovementStateImageKey(PlantMovementState state)
    {
      if ((int)state >= 0 && (int)state < PlantMovementStateImageKeys.Length)
        return PlantMovementStateImageKeys[(int)state];
      else
        return "Error";
    }

    #endregion

    #endregion

    #region Значение

    /// <summary>
    /// Преобразование значение заданного типа в строку.
    /// Эта версия не позволяет указать формат данных
    /// </summary>
    /// <param name="value">Значение</param>
    /// <param name="valueType">Тип данных</param>
    /// <returns>Строковое представление</returns>
    public static string ToString(object value, ValueType valueType)
    {
      return ToString(value, valueType, String.Empty);
    }

    /// <summary>
    /// Преобразование значение заданного типа в строку.
    /// Для чисел используется разделитель дробной части запятая (зависит от настроек ОС).
    /// Для преобразования значений в машинно-независимый формат используйте методы класса Accoo2StdConvert.
    /// </summary>
    /// <param name="value">Значение</param>
    /// <param name="valueType">Тип данных</param>
    /// <param name="format">Формат</param>
    /// <returns>Строковое представление</returns>
    public static string ToString(object value, ValueType valueType, string format)
    {
      if (value == null)
        value = PlantTools.GetDefaultValue(valueType);
      try
      {
        switch (valueType)
        {
          case ValueType.Date:
            if (value == null)
              return String.Empty;
            else if (String.IsNullOrEmpty(format))
              return ((DateTime)value).ToString("dd/MM/yyyy"); // 03.07.2017
            else
              return ((DateTime)value).ToString(format);
          case ValueType.DateTime:
            if (value == null)
              return String.Empty;
            else if (String.IsNullOrEmpty(format))
              return ((DateTime)value).ToString("dd/MM/yyyy HH:mm:ss"); // 14.03.2018
            else
              return ((DateTime)value).ToString(format);
          case ValueType.String:
            return (string)value;
          case ValueType.Boolean:
            return Convert.ToBoolean(value) ? "1" : "0";
          case ValueType.Integer:
            return Convert.ToInt32(value).ToString(format);
          case ValueType.Double:
            return Convert.ToDouble(value).ToString(format);
          case ValueType.Decimal:
            return Convert.ToDecimal(value).ToString(format);
          default:
            return value.ToString();
        }
      }
      catch
      {
        if (value == null)
          return String.Empty;
        else
          return value.ToString();
      }
    }

    /// <summary>
    /// Преобразование текста в заданный тип данных.
    /// Пустая строка преобразуется в значение по умолчанию для заданного типа.
    /// Если преобразование невозможно, генерируется InvalidCastException.
    /// Для чисел используется разделитель дробной части запятая (зависит от настроек ОС).
    /// Для преобразования значений из машинно-независимого формата используйте методы класса Accoo2StdConvert.
    /// </summary>
    /// <param name="text">Преобразуемый текст</param>
    /// <param name="valueType">Требуемый тип данных</param>
    /// <returns>Значение требуемого типа</returns>
    public static object Parse(string text, ValueType valueType)
    {
      object res;
      if (TryParse(text, valueType, out res))
        return res;
      else
        throw new InvalidCastException("Строку \"" + text + "\" нельзя преобразовать к типу \"" + PlantTools.GetValueTypeName(valueType) + "\"");
    }

    /// <summary>
    /// Преобразование текста в заданный тип данных.
    /// Пустая строка преобразуется в значение по умолчанию для заданного типа.
    /// Если преобразование невозможно, возвращается false
    /// Для чисел используется разделитель дробной части запятая (зависит от настроек ОС).
    /// Для преобразования значений из машинно-независимого формата используйте методы класса Accoo2StdConvert.
    /// </summary>
    /// <param name="text">Преобразуемый текст</param>
    /// <param name="valueType">Требуемый тип данных</param>
    /// <param name="value">Сюда записывается значение требуемого типа или null в случае ошибки</param>
    /// <returns>true, если преобразование успешно выполнено</returns>
    public static bool TryParse(string text, ValueType valueType, out object value)
    {
      if (String.IsNullOrEmpty(text))
      {
        value = PlantTools.GetDefaultValue(valueType);
        return true;
      }

      value = null;

      switch (valueType)
      {
        case ValueType.Integer:
          int iv;
          if (int.TryParse(text, out iv))
          {
            value = iv;
            return true;
          }
          else
            return false;

        case ValueType.Double:
          double dv;
          if (double.TryParse(text, out dv))
          {
            value = dv;
            return true;
          }
          else
            return false;

        case ValueType.Decimal:
          decimal mv;
          if (decimal.TryParse(text, out mv))
          {
            value = mv;
            return true;
          }
          else
            return false;

        case ValueType.Boolean:
          if (text == "1")
          {
            value = true;
            return true;
          }
          if (text == "0")
          {
            value = false;
            return true;
          }

          bool bv;
          if (bool.TryParse(text, out bv))
          {
            value = bv;
            return true;
          }
          else
            return false;

        case ValueType.String:
          value = text;
          return true;

        case ValueType.Date:
        case ValueType.DateTime:
          DateTime dtv;
          if (DateTime.TryParse(text, out dtv))
          {
            if (valueType == ValueType.DateTime)
              dtv = dtv.Date;
            value = dtv;
            return true;
          }
          else
            return false;

        default:
          throw new ArgumentException("Неизвестный ValueType", "ValueType");
      }
    }

    #endregion

    #region Атрибуты документов

    /// <summary>
    /// Максимальная длина текстового значения пользовательского атрибута, задаваемого в поле Value.
    /// Можно вводить значения любой длины. Они сохраняются в MEMO-поле LongValue
    /// </summary>
    public const int AttrValueShortMaxLength = 30;

    /// <summary>
    /// Проверяет соответствие типа источника значений для атрибута и типа данных
    /// </summary>
    /// <param name="sourceType">Тип источника данных для значений атрибута</param>
    /// <param name="valueType">Тип данных атрибута</param>
    /// <returns>true, если типы могут использоваться для одного атрибута</returns>
    public static bool IsValidAttrSourceType(AttrValueSourceType sourceType, ValueType valueType)
    {
      if (sourceType == AttrValueSourceType.None)
        return true;

      switch (valueType)
      {
        case ValueType.String:
          return true;
        case ValueType.Integer:
          switch (sourceType)
          {
            case AttrValueSourceType.List:
              return true;
            default:
              return false;
          }
        case ValueType.Boolean:
          return false;
        default:
          return sourceType == AttrValueSourceType.List;
      }
    }

    #region Методы преобразование значения в/из строкового представления

    /// <summary>
    /// Преобразование значения в текстовую строку для хранения в базе данных
    /// </summary>
    /// <param name="value">Значение</param>
    /// <param name="valueType">Тип значения</param>
    /// <returns>Строковое представление</returns>
    public static string ValueToSaveableString(object value, ValueType valueType)
    {
      switch (valueType)
      {
        case ValueType.String:
          return DataTools.GetString(value);
        case ValueType.Integer:
          return StdConvert.ToString(DataTools.GetInt(value));
        case ValueType.Double:
          return StdConvert.ToString(DataTools.GetDouble(value));
        case ValueType.Decimal:
          return StdConvert.ToString(DataTools.GetDecimal(value));
        case ValueType.Boolean:
          return DataTools.GetBool(value) ? "1" : "0";

        case ValueType.Date:
          DateTime? dt1 = DataTools.GetNullableDateTime(value);
          if (dt1.HasValue)
            return StdConvert.ToString(dt1.Value, false);
          else
            return String.Empty;

        case ValueType.DateTime:
          DateTime? dt2 = DataTools.GetNullableDateTime(value);
          if (dt2.HasValue)
            return StdConvert.ToString(dt2.Value, true);
          else
            return String.Empty;

        default:
          throw new ArgumentException("Неизвестный ValueType=" + valueType.ToString(), "ValueType");
      }
    }

    /// <summary>
    /// Преобразование значения атрибута, сохраненного в виде текстовой строки в базе данных,
    /// в значение заданного типа
    /// </summary>
    /// <param name="s">Хранимое строковое значение атрибута</param>
    /// <param name="valueType">Тип данных атрибута</param>
    /// <returns>Значение атрибута, приведенное к требуемому типу</returns>
    public static object ValueFromSaveableString(string s, ValueType valueType)
    {
      if (String.IsNullOrEmpty(s))
        return PlantTools.GetDefaultValue(valueType);

      switch (valueType)
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
          throw new ArgumentException("Неизвестный ValueType=" + valueType.ToString(), "ValueType");
      }
    }

    #endregion

    #endregion

    #region Изображения

    /// <summary>
    /// Размер, в который вписывается миниатюра
    /// </summary>
    public static readonly System.Drawing.Size ThumbnailSize = new System.Drawing.Size(128, 128);

    #endregion

    #region Фильтры

    /// <summary>
    /// Добавляет в список <paramref name="filters"/> фильтр по интервау дат.
    /// Если бы дата была одним полем, можно было бы использовать обычный DateRangeFilter.
    /// В программе даты задаются в виде интервала из двух полей Date1 и Date2, 
    /// требуется более сложный фильтр
    /// </summary>
    /// <param name="filters">Список для добавления фильтров</param>
    /// <param name="firstDate">Начальная дата фильтра</param>
    /// <param name="lastDate">Конечная дата фильтра</param>
    public static void AddDateRangeFilter(List<DBxFilter> filters, DateTime? firstDate, DateTime? lastDate)
    {
      //if (FirstDate.HasValue)
      //  Filters.Add(new ValueFilter("Date1", FirstDate.Value, ValueFilterKind.GreaterOrEqualThan));
      //if (LastDate.HasValue)
      //  Filters.Add(new ValueFilter("Date2", LastDate.Value, ValueFilterKind.LessOrEqualThan));

      // 21.05.2019
      if (firstDate.HasValue)
        filters.Add(new ValueFilter("Date2", firstDate.Value, CompareKind.GreaterOrEqualThan));
      if (lastDate.HasValue)
        filters.Add(new ValueFilter("Date1", lastDate.Value, CompareKind.LessOrEqualThan));
    }

    #endregion

    #region Грунты и горшки для действий

    /// <summary>
    /// Возвращает true, если для действия может быть задан грунт
    /// </summary>
    /// <param name="kind">Действие</param>
    /// <param name="forFinal">True, если запрашивается финальное состояние.
    /// В этом режиме частичная замена грунта не учитывается SoilReplace</param>
    /// <returns>true, если грунт применим к действию</returns>
    public static bool IsSoilAppliable(ActionKind kind, bool forFinal)
    {
      switch (kind)
      {
        case ActionKind.Planting:
        case ActionKind.Replanting:
        case ActionKind.Transshipment:
        case ActionKind.Other:
          return true;
        case ActionKind.SoilReplace:
          return !forFinal;
        default:
          return false;
      }
    }

    /// <summary>
    /// Возвращает true, если для действия может быть задан горшок
    /// </summary>
    /// <param name="kind">Действие</param>
    /// <param name="forFinal">True, если запрашивается финальное состояние.
    /// Не учитывается</param>
    /// <returns>true, если грунт применим к действию</returns>
    public static bool IsPotKindAppliable(ActionKind kind, bool forFinal)
    {
      switch (kind)
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
    /// Получение массива флажков для вызова IsSoilAppliable()
    /// </summary>
    public static int[] GetSoilAppliableIntArray()
    {
      List<int> lst = new List<int>();
      for (ActionKind kind = ActionKind.MinValue; kind <= ActionKind.MaxValue; kind++)
      {
        if (IsSoilAppliable(kind, false))
          lst.Add((int)kind);
      }
      return lst.ToArray();
    }

    /// <summary>
    /// Получение массива флажков для вызова IsPotKindAppliable()
    /// </summary>
    public static int[] GetPotKindAppliableIntArray()
    {
      List<int> lst = new List<int>();
      for (ActionKind kind = ActionKind.MinValue; kind <= ActionKind.MaxValue; kind++)
      {
        if (IsPotKindAppliable(kind, false))
          lst.Add((int)kind);
      }
      return lst.ToArray();
    }

    #endregion
  }


  /// <summary>
  /// Преобразование значения, хранящегося в ячейке в / из хранимого представления.
  /// Не зависит от настроек операционной системы.
  /// </summary>
  public static class PlantsStdConvert
  {
    #region Функции преобразования

    public static string ToString(object value, ValueType valueType)
    {
      switch (valueType)
      {
        case ValueType.Integer:
          return StdConvert.ToString(DataTools.GetInt(value));
        case ValueType.Double:
          return StdConvert.ToString(DataTools.GetDouble(value));
        case ValueType.Decimal:
          return StdConvert.ToString(DataTools.GetDecimal(value));
        case ValueType.Boolean:
          return DataTools.GetBool(value) ? "1" : "0";
        case ValueType.String:
          return DataTools.GetString(value);
        case ValueType.Date:
        case ValueType.DateTime:
          DateTime? dt = DataTools.GetNullableDateTime(value);
          if (dt.HasValue)
            return StdConvert.ToString(dt.Value, valueType == ValueType.DateTime);
          else
            return string.Empty;
        default:
          throw new ArgumentException("Неправильный ValueType=" + valueType.ToString(), "ValueType");
      }
    }

    public static bool TryParse(string text, ValueType valueType, out object value)
    {
      if (String.IsNullOrEmpty(text))
      {
        value = PlantTools.GetDefaultValue(valueType);
        return true;
      }

      switch (valueType)
      {
        case ValueType.Integer:
          int vi;
          if (StdConvert.TryParse(text, out vi))
          {
            value = vi;
            return true;
          }
          else
          {
            decimal vdc2;
            if (StdConvert.TryParse(text, out vdc2))
            {
              vi = (int)Math.Round(vdc2, 0, MidpointRounding.AwayFromZero);
              value = vi;
              return true;
            }

            value = 0;
            return false;
          }

        case ValueType.Double:
          double vd;
          if (StdConvert.TryParse(text, out vd))
          {
            value = vd;
            return true;
          }
          else
          {
            value = 0.0;
            return false;
          }

        case ValueType.Decimal:
          decimal vdс;
          if (StdConvert.TryParse(text, out vdс))
          {
            value = vdс;
            return true;
          }
          else
          {
            value = 0m;
            return false;
          }

        case ValueType.Boolean:
          int vb;
          if (StdConvert.TryParse(text, out vb))
          {
            value = vb != 0;
            return true;
          }
          else
          {
            value = false;
            return false;
          }

        case ValueType.String:
          value = text;
          return true;

        case ValueType.Date:
        case ValueType.DateTime:
          DateTime dt;
          if (StdConvert.TryParse(text, out dt, valueType == ValueType.DateTime))
          {
            value = dt;
            return true;
          }
          else
          {
            value = null;
            return false;
          }
        default:
          throw new ArgumentException("Неправильный ValueType=" + valueType.ToString(), "ValueType");
      }
    }

    #endregion
  }
}
