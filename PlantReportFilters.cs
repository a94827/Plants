using System;
using System.Collections.Generic;
using System.Text;
using FreeLibSet.Forms.Docs;
using System.Data;
using FreeLibSet.Data;
using FreeLibSet.Data.Docs;
using FreeLibSet.Core;
using FreeLibSet.Collections;

namespace Plants
{
  /// <summary>
  /// Фильтры для отчетов.
  /// Так как программа не разделяется на клиент и сервер, достаточно только фильтров клиента, а класса, производного от DBxCommonFilters не требуется.
  /// </summary>
  internal class PlantReportFilters : DBxClientFilters
  {
    #region Конструктор

    /// <summary>
    /// Создает фильтры
    /// </summary>
    /// <param name="columnNamePrefix">Префикс перед именами полей, например, "DocId."</param>
    /// <param name="usedFilters">Список кодов фильтров, разделенных запятыми, которые требуется использовать.
    /// Если пустая строка, то используются все возможные фильтры</param>
    public PlantReportFilters(string columnNamePrefix, string usedFilters)
    {
      if (columnNamePrefix == null)
        columnNamePrefix = String.Empty;
      _ColumnNamePrefix = columnNamePrefix;

      if (String.IsNullOrEmpty(usedFilters))
        usedFilters = "Group,HasNumber,NumberRange,Place,HasAdd,HasRemove,FromContra,ToContra,ActionKind,Remedy,Soil,PotKind,Disease";
      StringArrayIndexer ufidx = new StringArrayIndexer(usedFilters.Split(','));

      if (ufidx.Contains("Group"))
      {
        FiltGroup = new RefGroupDocGridFilter(ProgramDBUI.TheUI, "PlantGroups", columnNamePrefix + "GroupId");
        FiltGroup.Code = "Group";
        Add(FiltGroup);
      }

      if (ufidx.Contains("HasNumber"))
      {
        FiltHasNumber = new NullNotNullGridFilter(columnNamePrefix + "Number", typeof(int));
        FiltHasNumber.Code = "HasNumber";
        FiltHasNumber.DisplayName = "Наличие номера по каталогу";
        FiltHasNumber.FilterTextNull = "Нет";
        FiltHasNumber.FilterTextNotNull = "Есть";
        Add(FiltHasNumber);
      }

      if (ufidx.Contains("NumberRange"))
      {
        FiltNumberRange = new IntRangeGridFilter(columnNamePrefix + "Number");
        FiltNumberRange.Code = "NumberRange";
        FiltNumberRange.DisplayName = "Диапазон номеров по каталогу";
        FiltNumberRange.Minimum = 1;
        FiltNumberRange.Increment = 1;
        Add(FiltNumberRange);
      }

      if (ufidx.Contains("Place"))
      {
        FiltPlace = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["Places"], "Place");
        FiltPlace.DisplayName = "Место расположения";
        FiltPlace.Nullable = false;
        FiltPlace.UseSqlFilter = false;
        Add(FiltPlace);
      }

      if (ufidx.Contains("HasAdd"))
      {
        FiltHasAdd = new BoolValueGridFilter("HasAdd");
        FiltHasAdd.DisplayName = "Есть приход";
        FiltHasAdd.UseSqlFilter = false;
        FiltHasAdd.FilterTextTrue = "Был приход растения в выбранный период";
        FiltHasAdd.FilterTextFalse = String.Empty;
        Add(FiltHasAdd);
      }

      if (ufidx.Contains("HasRemove"))
      {
        FiltHasRemove = new BoolValueGridFilter("HasRemove");
        FiltHasRemove.DisplayName = "Есть выбытие";
        FiltHasRemove.UseSqlFilter = false;
        FiltHasRemove.FilterTextTrue = "Было выбытие растения в выбранный период";
        FiltHasRemove.FilterTextFalse = String.Empty;
        Add(FiltHasRemove);
      }

      if (ufidx.Contains("FromContra"))
      {
        FiltFromContra = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["Contras"], "FromContra");
        FiltFromContra.DisplayName = "От кого получено";
        FiltFromContra.Nullable = true;
        FiltFromContra.UseSqlFilter = false;
        Add(FiltFromContra);
      }

      if (ufidx.Contains("ToContra"))
      {
        FiltToContra = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["Contras"], "ToContra");
        FiltToContra.DisplayName = "Кому передано";
        FiltToContra.Nullable = true;
        FiltToContra.UseSqlFilter = false;
        Add(FiltToContra);
      }

      if (ufidx.Contains("ActionKind"))
      {
        FiltAction = new EnumGridFilter("Kind", PlantTools.ActionNames);
        FiltAction.Code = "ActionKind";
        FiltAction.DisplayName = "Действия";
        FiltAction.ImageKeys = PlantTools.ActionImageKeys;
        FiltAction.UseSqlFilter = false;
        Add(FiltAction);
      }

      if (ufidx.Contains("Remedy"))
      {
        FiltRemedy = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["Remedies"], "Remedy");
        FiltRemedy.DisplayName = "Обработка препаратом";
        FiltRemedy.Nullable = false;
        FiltRemedy.UseSqlFilter = false;
        Add(FiltRemedy);
      }

      if (ufidx.Contains("Soil"))
      {
        FiltSoil = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["Soils"], "Soil");
        FiltSoil.DisplayName = "Грунт";
        FiltSoil.Nullable = true;
        FiltSoil.UseSqlFilter = false;
        Add(FiltSoil);
      }

      if (ufidx.Contains("PotKind"))
      {
        FiltPotKind = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["PotKinds"], "PotKind");
        FiltPotKind.DisplayName = "Горшок";
        FiltPotKind.Nullable = true;
        FiltPotKind.UseSqlFilter = false;
        Add(FiltPotKind);
      }

      if (ufidx.Contains("Disease"))
      {
        FiltDisease = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["Diseases"], "Disease");
        FiltDisease.DisplayName = "Заболевания";
        FiltDisease.Nullable = true;
        FiltDisease.UseSqlFilter = false;
        Add(FiltDisease);
      }
      /*
      EnumGridFilter FiltState = new EnumGridFilter("MovementState", PlantTools.PlantMovementStateNames);
      FiltState.DisplayName = "Состояние";
      FiltState.ImageKeys = PlantTools.PlantMovementStateImageKeys;
      Args.ControlProvider.Filters.Add(FiltState);

       * */

      SetReadOnly();
    }

    #endregion

    #region Поля фильтров

    RefGroupDocGridFilter FiltGroup;

    /// <summary>
    /// Фильтр на наличие или отсутствие номера
    /// </summary>
    NullNotNullGridFilter FiltHasNumber;

    IntRangeGridFilter FiltNumberRange;

    RefDocGridFilter FiltPlace;

    BoolValueGridFilter FiltHasAdd, FiltHasRemove;

    RefDocGridFilter FiltFromContra, FiltToContra;

    EnumGridFilter FiltAction;

    RefDocGridFilter FiltSoil, FiltPotKind;

    RefDocGridFilter FiltDisease, FiltRemedy;

    private static bool IsSet(DBxCommonFilter filter)
    {
      if (filter == null)
        return false;
      else
        return !filter.IsEmpty;
    }

    #endregion

    #region Взаимная блокировка фильтров

    protected override void OnChanged(DBxCommonFilter filter)
    {
      base.OnChanged(filter);
      if (filter.IsEmpty)
        return;
      switch (filter.Code)
      {
        case "HasNumber": ClearFilter("Number"); break;
        case "Number": ClearFilter("HasNumber"); break;
      }
    }

    #endregion

    #region Ручная фильтрация строк таблицы

    #region Основной метод

    private string _ColumnNamePrefix;

    private DataTable _TableMovement;

    private DataTable _TableActions;

    private DataTable _TempTableSoilAndPotKind;

    private DataTable _TableDiseases;

    /// <summary>
    /// Выполняет дополнительную фильтрацию таблицы каталога растений, которую нельзя было выполнить с помощью фильтра WHERE в SQL-запросе.
    /// Диапазон дат используется для фильтрации таблиц поддокументов. Растение считается проходящим условие фильтра,
    /// если это условие выполнялось когда-либо в течение диапазона дат.
    /// </summary>
    /// <param name="table">Таблица документов "Растения" или одного из поддокументов</param>
    /// <param name="firstDate">Начальная дата диапазона</param>
    /// <param name="lastDate">Конечная дата диапазона</param>
    public void PerformAuxFiltering(ref DataTable table, DateTime? firstDate, DateTime? lastDate)
    {
      string idColumnName = "Id";
      if (_ColumnNamePrefix.Length > 0)
        idColumnName = _ColumnNamePrefix.Substring(0, _ColumnNamePrefix.Length - 1); // без точки
      int pId = table.Columns.IndexOf(idColumnName);
      if (pId < 0)
        throw new BugException("Таблица должна содержать поле \"" + idColumnName + "\"");

      if (this.IsNonSqlEmpty)
        return; // Никаких ручных фильтров нет

      bool useDeleted = ProgramDBUI.TheUI.DocProvider.DocTypes.UseDeleted;

      #region Создание TableMovement

      if (IsSet(FiltPlace) || IsSet(FiltHasAdd) || IsSet(FiltHasRemove) || IsSet(FiltFromContra) || IsSet(FiltToContra)
        || IsSet(FiltSoil) || IsSet(FiltPotKind))
      {
        List<DBxFilter> filters = new List<DBxFilter>();
        DBxColumnList columns = new DBxColumnList();
        columns.Add("Id,DocId,Kind,Date1,Date2");
        if (IsSet(FiltPlace) ||
          IsSet(FiltHasAdd) || IsSet(FiltHasRemove) /* 16.10.2019 */)
          columns.Add("Place");
        if (IsSet(FiltFromContra) || IsSet(FiltToContra))
          columns.Add("Contra");
        if (IsSet(FiltSoil))
          columns.Add("Soil");
        if (IsSet(FiltPotKind))
          columns.Add("PotKind");

        filters.Add(new DateRangeCrossFilter("Date1", "Date2", null, lastDate)); // учитываются все даты с начала времен
        if (useDeleted)
        {
          filters.Add(DBSSubDocType.DeletedFalseFilter);
          filters.Add(DBSSubDocType.DocIdDeletedFalseFilter);
        }
        _TableMovement = ProgramDBUI.TheUI.DocProvider.FillSelect("PlantMovement", new DBxColumns(columns),
          AndFilter.FromList(filters), null);
        _TableMovement.DefaultView.Sort = "Date1";
      }

      #endregion

      #region Создание TableActions

      if (IsSet(FiltAction) || IsSet(FiltRemedy) || IsSet(FiltSoil) || IsSet(FiltPotKind))
      {
        List<DBxFilter> filters = new List<DBxFilter>();
        DBxColumnList columns = new DBxColumnList();
        columns.Add("Id,DocId,Date1,Date2,Kind");

        if (IsSet(FiltRemedy) || IsSet(FiltSoil) || IsSet(FiltPotKind))
        {
          filters.Add(new DateRangeCrossFilter("Date1", "Date2", null, lastDate));// учитываются все даты с начала времен
          //Filters.Add(FiltAction.GetSqlFilter()); // придется фильтровать руками, т.к. грунт и горшок могут относиться к другим действиям
          if (IsSet(FiltSoil))
            columns.Add("Soil");
          if (IsSet(FiltPotKind))
            columns.Add("PotKind");
          if (IsSet(FiltRemedy))
            columns.Add("Remedy");
        }
        else
        {
          // Если есть только фильтр по виду действия
          filters.Add(new DateRangeCrossFilter("Date1", "Date2", firstDate, lastDate));// учитываются только даты в диапазоне
          filters.Add(FiltAction.GetSqlFilter());
        }

        if (useDeleted)
        {
          filters.Add(DBSSubDocType.DeletedFalseFilter);
          filters.Add(DBSSubDocType.DocIdDeletedFalseFilter);
        }


        _TableActions = ProgramDBUI.TheUI.DocProvider.FillSelect("PlantActions", new DBxColumns(columns),
          AndFilter.FromList(filters), null);
        _TableActions.DefaultView.Sort = "Date1";
      }

      #endregion

      #region Создание TableDisease

      if (IsSet(FiltDisease))
      {
        List<DBxFilter> filters = new List<DBxFilter>();
        DBxColumnList columns = new DBxColumnList();
        columns.Add("Id,DocId,Date1,Date2,Disease");

        filters.Add(new DateRangeCrossFilter("Date1", "Date2", firstDate, lastDate));// учитываются только даты в диапазоне
        filters.Add(FiltDisease.GetSqlFilter());

        if (useDeleted)
        {
          filters.Add(DBSSubDocType.DeletedFalseFilter);
          filters.Add(DBSSubDocType.DocIdDeletedFalseFilter);
        }


        _TableDiseases = ProgramDBUI.TheUI.DocProvider.FillSelect("PlantDiseases", new DBxColumns(columns),
          AndFilter.FromList(filters), null);
        _TableDiseases.DefaultView.Sort = "Date1";
      }

      #endregion

      #region Создание временной таблицы

      // Таблица DataTable это тяжеловесный объект, который лучше создать один раз, а не для каждого растения
      if (IsSet(FiltSoil) || IsSet(FiltPotKind))
      {
        _TempTableSoilAndPotKind = new DataTable();
        _TempTableSoilAndPotKind.Columns.Add("Date1", typeof(DateTime));
        _TempTableSoilAndPotKind.Columns.Add("Date2", typeof(DateTime));
        _TempTableSoilAndPotKind.Columns.Add("Soil", typeof(Int32));
        _TempTableSoilAndPotKind.Columns.Add("PotKind", typeof(Int32));
        _TempTableSoilAndPotKind.DefaultView.Sort = "Date1";
      }

      #endregion

      #region Фильтрация записей

      bool delFlag = false;
      for (int i = table.Rows.Count - 1; i >= 0; i--)
      {
        Int32 id = DataTools.GetInt(table.Rows[i][pId]);
        if (!TestPlantFilter(id, firstDate, lastDate))
        {
          table.Rows[i].Delete();
          delFlag = true;
        }
      }
      if (delFlag)
        table.AcceptChanges();

      #endregion
    }

    #endregion

    #region Проверка одного растения

    private bool TestPlantFilter(Int32 docId, DateTime? firstDate, DateTime? lastDate)
    {
      if (_TableMovement != null)
      {
        _TableMovement.DefaultView.RowFilter = new ValueFilter("DocId", docId).ToString();
        if (IsSet(FiltPlace))
        {
          if (!TestPlaceFilter(_TableMovement.DefaultView, firstDate ?? DateTime.MinValue))
            return false;
        }
        if (IsSet(FiltHasAdd))
        {
          if (!TestHasAddFilter(_TableMovement.DefaultView, firstDate ?? DateTime.MinValue))
            return false;
        }
        if (IsSet(FiltHasRemove))
        {
          if (!TestHasRemoveFilter(_TableMovement.DefaultView, firstDate ?? DateTime.MinValue))
            return false;
        }
        if (IsSet(FiltFromContra))
        {
          if (!TestContraFilter(_TableMovement.DefaultView, firstDate ?? DateTime.MinValue, true))
            return false;
        }
        if (IsSet(FiltToContra))
        {
          if (!TestContraFilter(_TableMovement.DefaultView, firstDate ?? DateTime.MinValue, false))
            return false;
        }
      }

      if (_TableActions != null)
      {
        _TableActions.DefaultView.RowFilter = new ValueFilter("DocId", docId).ToString();
        if (IsSet(FiltAction))
        {
          if (!TestActionFilter(_TableActions.DefaultView, firstDate ?? DateTime.MinValue))
            return false;
        }

        if (IsSet(FiltRemedy))
        {
          if (!TestRemedyFilter(_TableActions.DefaultView, firstDate ?? DateTime.MinValue))
            return false;
        }
      }

      if (IsSet(FiltSoil))
      {
        if (!TestSoilOrPotKind(_TableMovement.DefaultView, _TableActions.DefaultView, FiltSoil, firstDate ?? DateTime.MinValue))
          return false;
      }

      if (IsSet(FiltPotKind))
      {
        if (!TestSoilOrPotKind(_TableMovement.DefaultView, _TableActions.DefaultView, FiltPotKind, firstDate ?? DateTime.MinValue))
          return false;
      }

      if (_TableDiseases != null)
      {
        _TableDiseases.DefaultView.RowFilter = new ValueFilter("DocId", docId).ToString();
        if (IsSet(FiltDisease))
        {
          if (!TestDiseaseFilter(_TableDiseases.DefaultView, firstDate ?? DateTime.MinValue))
            return false;
        }
      }

      return true;
    }

    private bool TestPlaceFilter(DataView dv, DateTime firstDate)
    {
      Int32 lastPlaceId = 0;
      foreach (DataRowView drv in dv)
      {
        MovementKind kind = (MovementKind)DataTools.GetInt(drv.Row, "Kind");
        Int32 placeId = DataTools.GetInt(drv.Row, "Place");
        DateTime dt1 = DataTools.GetDateTime(drv.Row, "Date1");

        if (dt1 > firstDate && lastPlaceId != 0 && FiltPlace.TestValue(lastPlaceId))
          return true;

        if (kind == MovementKind.Add || kind == MovementKind.Move)
          lastPlaceId = placeId;
        else
          lastPlaceId = 0;
      }
      if (lastPlaceId != 0 && FiltPlace.TestValue(lastPlaceId))
        return true;

      return false;
    }

    private bool TestHasAddFilter(DataView dv, DateTime firstDate)
    {
      bool lastInPlace = false;
      foreach (DataRowView drv in dv)
      {
        MovementKind kind = (MovementKind)DataTools.GetInt(drv.Row, "Kind");
        DateTime dt1 = DataTools.GetDateTime(drv.Row, "Date1");
        Int32 placeId = DataTools.GetInt(drv.Row, "Place");
        bool inPlace = true;
        if (IsSet(FiltPlace))
          inPlace = FiltPlace.TestValue(placeId);

        switch (kind)
        {
          case MovementKind.Add:
            if (dt1 >= firstDate && inPlace)
              return true;
            lastInPlace = inPlace;
            break;
          case MovementKind.Move:
            if (dt1 >= firstDate && inPlace && (!lastInPlace))
              return true;
            lastInPlace = inPlace;
            break;
          case MovementKind.Remove:
            lastInPlace = false;
            break;
        }
      }

      return false;
    }

    private bool TestHasRemoveFilter(DataView dv, DateTime firstDate)
    {
      bool lastInPlace = false;
      foreach (DataRowView drv in dv)
      {
        MovementKind kind = (MovementKind)DataTools.GetInt(drv.Row, "Kind");
        DateTime dt1 = DataTools.GetDateTime(drv.Row, "Date1");
        Int32 placeId = DataTools.GetInt(drv.Row, "Place");
        bool inPlace = true;
        if (IsSet(FiltPlace))
          inPlace = FiltPlace.TestValue(placeId);

        switch (kind)
        {
          case MovementKind.Add:
            lastInPlace = inPlace;
            break;
          case MovementKind.Move:
            if (dt1 >= firstDate && (!inPlace) && lastInPlace)
              return true;
            lastInPlace = inPlace;
            break;
          case MovementKind.Remove:
            if (dt1 >= firstDate && lastInPlace)
              return true;
            lastInPlace = false;
            break;
        }
      }

      return false;
    }

    private bool TestContraFilter(DataView dv, DateTime firstDate, bool isFromContra)
    {
      // Проверяем только операции прихода или выбытия в периоде

      foreach (DataRowView drv in dv)
      {
        DateTime dt1 = DataTools.GetDateTime(drv.Row, "Date1");
        if (dt1 < firstDate)
          continue;
        MovementKind kind = (MovementKind)DataTools.GetInt(drv.Row, "Kind");
        Int32 contraId = DataTools.GetInt(drv.Row, "Contra");

        switch (kind)
        {
          case MovementKind.Add:
            if (isFromContra)
            {
              if (FiltFromContra.TestValue(contraId))
                return true;
            }
            break;
          case MovementKind.Remove:
            if (!isFromContra)
            {
              if (FiltToContra.TestValue(contraId))
                return true;
            }
            break;
        }
      }

      return false;
    }

    private bool TestActionFilter(DataView dv, DateTime firstDate)
    {
      foreach (DataRowView drv in dv)
      {
        DateTime dt1 = DataTools.GetDateTime(drv.Row, "Date1");
        if (dt1 < firstDate)
          continue;
        ActionKind kind = (ActionKind)DataTools.GetInt(drv.Row, "Kind");
        if (FiltAction.TestValue((int)kind))
          return true;
      }
      return false;
    }

    private bool TestRemedyFilter(DataView dv, DateTime firstDate)
    {
      foreach (DataRowView drv in dv)
      {
        DateTime dt1 = DataTools.GetDateTime(drv.Row, "Date1");
        if (dt1 < firstDate)
          continue;
        ActionKind kind = (ActionKind)DataTools.GetInt(drv.Row, "Kind");
        Int32 remedyId = DataTools.GetInt(drv.Row, "Remedy");
        if (kind == ActionKind.Treatment)
        {
          if (FiltRemedy.TestValue(remedyId))
            return true;
        }
      }
      return false;
    }

    private bool TestDiseaseFilter(DataView dv, DateTime firstDate)
    {
      foreach (DataRowView drv in dv)
      {
        DateTime dt1 = DataTools.GetDateTime(drv.Row, "Date1");
        if (dt1 < firstDate)
          continue;
        Int32 diseaseId = DataTools.GetInt(drv.Row, "Disease");
        if (FiltDisease.TestValue(diseaseId))
          return true;
      }
      return false;
    }

    private bool TestSoilOrPotKind(DataView dvMovement, DataView dvActions, RefDocGridFilter gridFilter, DateTime firstDate)
    {
      #region Требуется таблица для объединения значений

      _TempTableSoilAndPotKind.Rows.Clear();
      foreach (DataRowView drv in dvMovement)
      {
        MovementKind kind = (MovementKind)DataTools.GetInt(drv.Row, "Kind");
        if (kind == MovementKind.Add)
        {
          DataRow tempRow = _TempTableSoilAndPotKind.Rows.Add(drv.Row["Date1"], drv.Row["Date2"]);
          tempRow[gridFilter.ColumnName] = drv.Row[gridFilter.ColumnName];
        }
      }
      foreach (DataRowView drv in dvActions)
      {
        ActionKind kind = (ActionKind)DataTools.GetInt(drv.Row, "Kind");
        switch (gridFilter.ColumnName)
        {
          case "Soil":
            if (!PlantTools.IsSoilAppliable(kind, true))
              continue;
            break;
          case "PotKind":
            if (!PlantTools.IsPotKindAppliable(kind, true))
              continue;
            break;
          default:
            throw new ArgumentException("Неизвестный фильтр " + gridFilter.ColumnName, "Filt");
        }
        object id = drv.Row[gridFilter.ColumnName];
        if (id != null)
        {
          DataRow tempRow = _TempTableSoilAndPotKind.Rows.Add(drv.Row["Date1"], drv.Row["Date2"]);
          tempRow[gridFilter.ColumnName] = id;
        }
      }

      #endregion

      #region Теперь перебираем объединенные записи

      Int32 prevId = 0;
      bool needsTestPrev = false;

      foreach (DataRowView drv in _TempTableSoilAndPotKind.DefaultView)
      {
        Int32 id = DataTools.GetInt(drv.Row, gridFilter.ColumnName);
        DateTime dt1 = DataTools.GetDateTime(drv.Row, "Date1");
        if (dt1 >= firstDate)
        {
          if (gridFilter.TestValue(id))
            return true;
        }
        else
        {
          prevId = id;
          needsTestPrev = true;
        }
      }

      #endregion

      #region Проверка первоначального значения

      if (needsTestPrev)
        return gridFilter.TestValue(prevId);
      else
        return false;

      #endregion
    }

    #endregion

    /// <summary>
    /// Возвращает true, если для выбранных фильтров требуется период времени, за который просматривать данные.
    /// Возвращает false, если установлены только фильтры, не зависящие от времени, или нет установленных фильтров.
    /// </summary>
    public bool PeriodRequired
    {
      get
      {
        return !IsNonSqlEmpty;
      }
    }

    #endregion
  }
}
