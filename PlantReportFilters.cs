using System;
using System.Collections.Generic;
using System.Text;
using FreeLibSet.Forms.Docs;
using System.Data;
using FreeLibSet.Data;
using FreeLibSet.Data.Docs;
using FreeLibSet.Core;

namespace Plants
{
  /// <summary>
  /// ������� ��� �������.
  /// ��� ��� ��������� �� ����������� �� ������ � ������, ���������� ������ �������� �������, � ������, ������������ �� DBxCommonFilters �� ���������.
  /// </summary>
  internal class PlantReportFilters : DBxClientFilters
  {
    #region �����������

    /// <summary>
    /// ������� �������
    /// </summary>
    /// <param name="ColumnNamePrefix">������� ����� ������� �����, ��������, "DocId."</param>
    public PlantReportFilters(string columnNamePrefix)
    {
      if (columnNamePrefix == null)
        columnNamePrefix = String.Empty;
      _ColumnNamePrefix = columnNamePrefix;

      FiltGroup = new RefGroupDocGridFilter(ProgramDBUI.TheUI, "PlantGroups", columnNamePrefix + "GroupId");
      Add(FiltGroup);

      FiltHasNumber = new NullNotNullGridFilter(columnNamePrefix + "Number", typeof(int));
      FiltHasNumber.Code = "HasNumber";
      FiltHasNumber.DisplayName = "������� ������ �� ��������";
      FiltHasNumber.FilterTextNull = "���";
      FiltHasNumber.FilterTextNotNull = "����";
      Add(FiltHasNumber);

      FiltNumberRange = new IntRangeGridFilter(columnNamePrefix + "Number");
      FiltNumberRange.DisplayName = "�������� ������� �� ��������";
      FiltNumberRange.Minimum = 1;
      FiltNumberRange.Increment = 1;
      Add(FiltNumberRange);

      FiltPlace = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["Places"], "Place");
      FiltPlace.DisplayName = "����� ������������";
      FiltPlace.Nullable = false;
      FiltPlace.UseSqlFilter = false;
      Add(FiltPlace);

      FiltHasAdd = new BoolValueGridFilter("HasAdd");
      FiltHasAdd.DisplayName = "���� ������";
      FiltHasAdd.UseSqlFilter = false;
      FiltHasAdd.FilterTextTrue = "��� ������ �������� � ��������� ������";
      FiltHasAdd.FilterTextFalse = String.Empty;
      Add(FiltHasAdd);

      FiltHasRemove = new BoolValueGridFilter("HasRemove");
      FiltHasRemove.DisplayName = "���� �������";
      FiltHasRemove.UseSqlFilter = false;
      FiltHasRemove.FilterTextTrue = "���� ������� �������� � ��������� ������";
      FiltHasRemove.FilterTextFalse = String.Empty;
      Add(FiltHasRemove);

      FiltFromContra = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["Contras"], "FromContra");
      FiltFromContra.DisplayName = "�� ���� ��������";
      FiltFromContra.Nullable = true;
      FiltFromContra.UseSqlFilter = false;
      Add(FiltFromContra);

      FiltToContra = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["Contras"], "ToContra");
      FiltToContra.DisplayName = "���� ��������";
      FiltToContra.Nullable = true;
      FiltToContra.UseSqlFilter = false;
      Add(FiltToContra);

      FiltAction = new EnumGridFilter("Kind", PlantTools.ActionNames);
      FiltAction.Code = "ActionKind";
      FiltAction.DisplayName = "��������";
      FiltAction.ImageKeys = PlantTools.ActionImageKeys;
      FiltAction.UseSqlFilter = false;
      Add(FiltAction);

      FiltRemedy = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["Remedies"], "Remedy");
      FiltRemedy.DisplayName = "��������� ����������";
      FiltRemedy.Nullable = false;
      FiltRemedy.UseSqlFilter = false;
      Add(FiltRemedy);

      FiltSoil = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["Soils"], "Soil");
      FiltSoil.DisplayName = "�����";
      FiltSoil.Nullable = true;
      FiltSoil.UseSqlFilter = false;
      Add(FiltSoil);

      FiltPotKind = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["PotKinds"], "PotKind");
      FiltPotKind.DisplayName = "������";
      FiltPotKind.Nullable = true;
      FiltPotKind.UseSqlFilter = false;
      Add(FiltPotKind);

      FiltDisease = new RefDocGridFilter(ProgramDBUI.TheUI.DocTypes["Diseases"], "Disease");
      FiltDisease.DisplayName = "�����������";
      FiltDisease.Nullable = true;
      FiltDisease.UseSqlFilter = false;
      Add(FiltDisease);

      /*
      EnumGridFilter FiltState = new EnumGridFilter("MovementState", PlantTools.PlantMovementStateNames);
      FiltState.DisplayName = "���������";
      FiltState.ImageKeys = PlantTools.PlantMovementStateImageKeys;
      Args.ControlProvider.Filters.Add(FiltState);

       * */

      SetReadOnly();
    }

    #endregion

    #region ���� ��������

    RefGroupDocGridFilter FiltGroup;

    /// <summary>
    /// ������ �� ������� ��� ���������� ������
    /// </summary>
    NullNotNullGridFilter FiltHasNumber;

    IntRangeGridFilter FiltNumberRange;

    RefDocGridFilter FiltPlace;

    BoolValueGridFilter FiltHasAdd, FiltHasRemove;

    RefDocGridFilter FiltFromContra, FiltToContra;

    EnumGridFilter FiltAction;

    RefDocGridFilter FiltSoil, FiltPotKind;

    RefDocGridFilter FiltDisease, FiltRemedy;

    #endregion

    #region �������� ���������� ��������

    protected override void OnChanged(DBxCommonFilter filter)
    {
      base.OnChanged(filter);
      if (filter.IsEmpty)
        return;
      switch (filter.Code)
      {
        case "HasNumber": this["Number"].Clear(); break;
        case "Number": this["HasNumber"].Clear(); break;
      }
    }

    #endregion

    #region ������ ���������� ����� �������

    #region �������� �����

    private string _ColumnNamePrefix;

    private DataTable _TableMovement;

    private DataTable _TableActions;

    private DataTable _TempTableSoilAndPotKind;

    private DataTable _TableDiseases;


    public void PerformAuxFiltering(ref DataTable table, DateTime? firstDate, DateTime? lastDate)
    {
      string idColumnName = "Id";
      if (_ColumnNamePrefix.Length > 0)
        idColumnName = _ColumnNamePrefix.Substring(0, _ColumnNamePrefix.Length - 1); // ��� �����
      int pId = table.Columns.IndexOf(idColumnName);
      if (pId < 0)
        throw new BugException("������� ������ ��������� ���� \"" + idColumnName + "\"");

      if (this.IsNonSqlEmpty)
        return; // ������� ������ �������� ���

      bool useDeleted = ProgramDBUI.TheUI.DocProvider.DocTypes.UseDeleted;

      #region �������� TableMovement

      if (!(FiltPlace.IsEmpty && FiltHasAdd.IsEmpty && FiltHasRemove.IsEmpty && FiltFromContra.IsEmpty && FiltToContra.IsEmpty
        && FiltSoil.IsEmpty && FiltPotKind.IsEmpty))
      {
        List<DBxFilter> filters = new List<DBxFilter>();
        DBxColumnList columns = new DBxColumnList();
        columns.Add("Id,DocId,Kind,Date1,Date2");
        if (!(FiltPlace.IsEmpty &&
          FiltHasAdd.IsEmpty && FiltHasRemove.IsEmpty /* 16.10.2019 */))
          columns.Add("Place");
        if (!(FiltFromContra.IsEmpty && FiltToContra.IsEmpty))
          columns.Add("Contra");
        if (!FiltSoil.IsEmpty)
          columns.Add("Soil");
        if (!FiltPotKind.IsEmpty)
          columns.Add("PotKind");

        filters.Add(new DateRangeCrossFilter("Date1", "Date2", null, lastDate)); // ����������� ��� ���� � ������ ������
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

      #region �������� TableActions

      if (!(FiltAction.IsEmpty && FiltRemedy.IsEmpty && FiltSoil.IsEmpty && FiltPotKind.IsEmpty))
      {
        List<DBxFilter> filters = new List<DBxFilter>();
        DBxColumnList columns = new DBxColumnList();
        columns.Add("Id,DocId,Date1,Date2,Kind");

        if (FiltRemedy.IsEmpty && FiltSoil.IsEmpty && FiltPotKind.IsEmpty)
        {
          // ���� ���� ������ ������ �� ���� ��������
          filters.Add(new DateRangeCrossFilter("Date1", "Date2", firstDate, lastDate));// ����������� ������ ���� � ���������
          filters.Add(FiltAction.GetSqlFilter());
        }
        else
        {
          filters.Add(new DateRangeCrossFilter("Date1", "Date2", null, lastDate));// ����������� ��� ���� � ������ ������
          //Filters.Add(FiltAction.GetSqlFilter()); // �������� ����������� ������, �.�. ����� � ������ ����� ���������� � ������ ���������
          if (!FiltSoil.IsEmpty)
            columns.Add("Soil");
          if (!FiltPotKind.IsEmpty)
            columns.Add("PotKind");
          if (!FiltRemedy.IsEmpty)
            columns.Add("Remedy");
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

      #region �������� TableDisease

      if (!(FiltDisease.IsEmpty))
      {
        List<DBxFilter> filters = new List<DBxFilter>();
        DBxColumnList columns = new DBxColumnList();
        columns.Add("Id,DocId,Date1,Date2,Disease");

        filters.Add(new DateRangeCrossFilter("Date1", "Date2", firstDate, lastDate));// ����������� ������ ���� � ���������
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

      #region �������� ��������� �������

      // ������� DataTable ��� ������������ ������, ������� ����� ������� ���� ���, � �� ��� ������� ��������
      if (!(FiltSoil.IsEmpty && FiltPotKind.IsEmpty))
      {
        _TempTableSoilAndPotKind = new DataTable();
        _TempTableSoilAndPotKind.Columns.Add("Date1", typeof(DateTime));
        _TempTableSoilAndPotKind.Columns.Add("Date2", typeof(DateTime));
        _TempTableSoilAndPotKind.Columns.Add("Soil", typeof(Int32));
        _TempTableSoilAndPotKind.Columns.Add("PotKind", typeof(Int32));
        _TempTableSoilAndPotKind.DefaultView.Sort = "Date1";
      }

      #endregion

      #region ���������� �������

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

    #region �������� ������ ��������

    private bool TestPlantFilter(Int32 docId, DateTime? firstDate, DateTime? lastDate)
    {
      if (_TableMovement != null)
      {
        _TableMovement.DefaultView.RowFilter = new ValueFilter("DocId", docId).ToString();
        if (!FiltPlace.IsEmpty)
        {
          if (!TestPlaceFilter(_TableMovement.DefaultView, firstDate ?? DateTime.MinValue))
            return false;
        }
        if (!FiltHasAdd.IsEmpty)
        {
          if (!TestHasAddFilter(_TableMovement.DefaultView, firstDate ?? DateTime.MinValue))
            return false;
        }
        if (!FiltHasRemove.IsEmpty)
        {
          if (!TestHasRemoveFilter(_TableMovement.DefaultView, firstDate ?? DateTime.MinValue))
            return false;
        }
        if (!FiltFromContra.IsEmpty)
        {
          if (!TestContraFilter(_TableMovement.DefaultView, firstDate ?? DateTime.MinValue, true))
            return false;
        }
        if (!FiltToContra.IsEmpty)
        {
          if (!TestContraFilter(_TableMovement.DefaultView, firstDate ?? DateTime.MinValue, false))
            return false;
        }
      }

      if (_TableActions != null)
      {
        _TableActions.DefaultView.RowFilter = new ValueFilter("DocId", docId).ToString();
        if (!FiltAction.IsEmpty)
        {
          if (!TestActionFilter(_TableActions.DefaultView, firstDate ?? DateTime.MinValue))
            return false;
        }

        if (!FiltRemedy.IsEmpty)
        {
          if (!TestRemedyFilter(_TableActions.DefaultView, firstDate ?? DateTime.MinValue))
            return false;
        }
      }


      if (!FiltSoil.IsEmpty)
      {
        if (!TestSoilOrPotKind(_TableMovement.DefaultView, _TableActions.DefaultView, FiltSoil, firstDate ?? DateTime.MinValue))
          return false;
      }

      if (!FiltPotKind.IsEmpty)
      {
        if (!TestSoilOrPotKind(_TableMovement.DefaultView, _TableActions.DefaultView, FiltPotKind, firstDate ?? DateTime.MinValue))
          return false;
      }

      if (_TableDiseases != null)
      {
        _TableDiseases.DefaultView.RowFilter = new ValueFilter("DocId", docId).ToString();
        if (!FiltDisease.IsEmpty)
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
        if (!FiltPlace.IsEmpty)
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
        if (!FiltPlace.IsEmpty)
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
      // ��������� ������ �������� ������� ��� ������� � �������

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
      #region ��������� ������� ��� ����������� ��������

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
            throw new ArgumentException("����������� ������ " + gridFilter.ColumnName, "Filt");
        }
        object id = drv.Row[gridFilter.ColumnName];
        if (id != null)
        {
          DataRow tempRow = _TempTableSoilAndPotKind.Rows.Add(drv.Row["Date1"], drv.Row["Date2"]);
          tempRow[gridFilter.ColumnName] = id;
        }
      }

      #endregion

      #region ������ ���������� ������������ ������

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

      #region �������� ��������������� ��������

      if (needsTestPrev)
        return gridFilter.TestValue(prevId);
      else
        return false;

      #endregion
    }

    #endregion

    /// <summary>
    /// ���������� true, ���� ��� ��������� �������� ��������� ������ �������, �� ������� ������������� ������.
    /// ���������� false, ���� ����������� ������ �������, �� ��������� �� �������, ��� ��� ������������� ��������.
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
