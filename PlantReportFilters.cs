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
    public PlantReportFilters(string ColumnNamePrefix)
    {
      if (ColumnNamePrefix == null)
        ColumnNamePrefix = String.Empty;
      _ColumnNamePrefix = ColumnNamePrefix;

      FiltGroup = new RefGroupDocGridFilter(ProgramDBUI.TheUI, "PlantGroups", ColumnNamePrefix + "GroupId");
      Add(FiltGroup);

      FiltHasNumber = new NullNotNullGridFilter(ColumnNamePrefix + "Number", typeof(int));
      FiltHasNumber.Code = "HasNumber";
      FiltHasNumber.DisplayName = "������� ������ �� ��������";
      FiltHasNumber.FilterTextNull = "���";
      FiltHasNumber.FilterTextNotNull = "����";
      Add(FiltHasNumber);

      FiltNumberRange = new IntRangeGridFilter(ColumnNamePrefix + "Number");
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

    protected override void OnChanged(DBxCommonFilter Filter)
    {
      base.OnChanged(Filter);
      if (Filter.IsEmpty)
        return;
      switch (Filter.Code)
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


    public void PerformAuxFiltering(ref DataTable Table, DateTime? FirstDate, DateTime? LastDate)
    {
      string IdColumnName = "Id";
      if (_ColumnNamePrefix.Length > 0)
        IdColumnName = _ColumnNamePrefix.Substring(0, _ColumnNamePrefix.Length - 1); // ��� �����
      int pId = Table.Columns.IndexOf(IdColumnName);
      if (pId < 0)
        throw new BugException("������� ������ ��������� ���� \"" + IdColumnName + "\"");

      if (this.IsNonSqlEmpty)
        return; // ������� ������ �������� ���


      bool UseDeleted = ProgramDBUI.TheUI.DocProvider.DocTypes.UseDeleted;

      #region �������� TableMovement

      if (!(FiltPlace.IsEmpty && FiltHasAdd.IsEmpty && FiltHasRemove.IsEmpty && FiltFromContra.IsEmpty && FiltToContra.IsEmpty
        && FiltSoil.IsEmpty && FiltPotKind.IsEmpty))
      {
        List<DBxFilter> Filters = new List<DBxFilter>();
        DBxColumnList Columns = new DBxColumnList();
        Columns.Add("Id,DocId,Kind,Date1,Date2");
        if (!(FiltPlace.IsEmpty &&
          FiltHasAdd.IsEmpty && FiltHasRemove.IsEmpty /* 16.10.2019 */))
          Columns.Add("Place");
        if (!(FiltFromContra.IsEmpty && FiltToContra.IsEmpty))
          Columns.Add("Contra");
        if (!FiltSoil.IsEmpty)
          Columns.Add("Soil");
        if (!FiltPotKind.IsEmpty)
          Columns.Add("PotKind");

        Filters.Add(new DateRangeCrossFilter("Date1", "Date2", null, LastDate)); // ����������� ��� ���� � ������ ������
        if (UseDeleted)
        {
          Filters.Add(DBSSubDocType.DeletedFalseFilter);
          Filters.Add(DBSSubDocType.DocIdDeletedFalseFilter);
        }
        _TableMovement = ProgramDBUI.TheUI.DocProvider.FillSelect("PlantMovement", new DBxColumns(Columns),
          AndFilter.FromList(Filters), null);
        _TableMovement.DefaultView.Sort = "Date1";
      }

      #endregion

      #region �������� TableActions

      if (!(FiltAction.IsEmpty && FiltRemedy.IsEmpty && FiltSoil.IsEmpty && FiltPotKind.IsEmpty))
      {
        List<DBxFilter> Filters = new List<DBxFilter>();
        DBxColumnList Columns = new DBxColumnList();
        Columns.Add("Id,DocId,Date1,Date2,Kind");

        if (FiltRemedy.IsEmpty && FiltSoil.IsEmpty && FiltPotKind.IsEmpty)
        {
          // ���� ���� ������ ������ �� ���� ��������
          Filters.Add(new DateRangeCrossFilter("Date1", "Date2", FirstDate, LastDate));// ����������� ������ ���� � ���������
          Filters.Add(FiltAction.GetSqlFilter()); 
        }
        else
        {
          Filters.Add(new DateRangeCrossFilter("Date1", "Date2", null, LastDate));// ����������� ��� ���� � ������ ������
          //Filters.Add(FiltAction.GetSqlFilter()); // �������� ����������� ������, �.�. ����� � ������ ����� ���������� � ������ ���������
          if (!FiltSoil.IsEmpty)
            Columns.Add("Soil");
          if (!FiltPotKind.IsEmpty)
            Columns.Add("PotKind");
          if (!FiltRemedy.IsEmpty)
            Columns.Add("Remedy");
        }

        if (UseDeleted)
        {
          Filters.Add(DBSSubDocType.DeletedFalseFilter);
          Filters.Add(DBSSubDocType.DocIdDeletedFalseFilter);
        }


        _TableActions = ProgramDBUI.TheUI.DocProvider.FillSelect("PlantActions", new DBxColumns(Columns),
          AndFilter.FromList(Filters), null);
        _TableActions.DefaultView.Sort = "Date1";
      }

      #endregion

      #region �������� TableDisease

      if (!(FiltDisease.IsEmpty))
      {
        List<DBxFilter> Filters = new List<DBxFilter>();
        DBxColumnList Columns = new DBxColumnList();
        Columns.Add("Id,DocId,Date1,Date2,Disease");

        Filters.Add(new DateRangeCrossFilter("Date1", "Date2", FirstDate, LastDate));// ����������� ������ ���� � ���������
        Filters.Add(FiltDisease.GetSqlFilter());

        if (UseDeleted)
        {
          Filters.Add(DBSSubDocType.DeletedFalseFilter);
          Filters.Add(DBSSubDocType.DocIdDeletedFalseFilter);
        }


        _TableDiseases = ProgramDBUI.TheUI.DocProvider.FillSelect("PlantDiseases", new DBxColumns(Columns),
          AndFilter.FromList(Filters), null);
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

      bool DelFlag = false;
      for (int i = Table.Rows.Count - 1; i >= 0; i--)
      {
        Int32 Id = DataTools.GetInt(Table.Rows[i][pId]);
        if (!TestPlantFilter(Id, FirstDate, LastDate))
        {
          Table.Rows[i].Delete();
          DelFlag = true;
        }
      }
      if (DelFlag)
        Table.AcceptChanges();

      #endregion
    }

    #endregion

    #region �������� ������ ��������

    private bool TestPlantFilter(Int32 DocId, DateTime? FirstDate, DateTime? LastDate)
    {
      if (_TableMovement != null)
      {
        _TableMovement.DefaultView.RowFilter = new ValueFilter("DocId", DocId).ToString();
        if (!FiltPlace.IsEmpty)
        {
          if (!TestPlaceFilter(_TableMovement.DefaultView, FirstDate ?? DateTime.MinValue))
            return false;
        }
        if (!FiltHasAdd.IsEmpty)
        {
          if (!TestHasAddFilter(_TableMovement.DefaultView, FirstDate ?? DateTime.MinValue))
            return false;
        }
        if (!FiltHasRemove.IsEmpty)
        {
          if (!TestHasRemoveFilter(_TableMovement.DefaultView, FirstDate ?? DateTime.MinValue))
            return false;
        }
        if (!FiltFromContra.IsEmpty)
        {
          if (!TestContraFilter(_TableMovement.DefaultView, FirstDate ?? DateTime.MinValue, true))
            return false;
        }
        if (!FiltToContra.IsEmpty)
        {
          if (!TestContraFilter(_TableMovement.DefaultView, FirstDate ?? DateTime.MinValue, false))
            return false;
        }
      }

      if (_TableActions != null)
      {
        _TableActions.DefaultView.RowFilter = new ValueFilter("DocId", DocId).ToString();
        if (!FiltAction.IsEmpty)
        {
          if (!TestActionFilter(_TableActions.DefaultView, FirstDate ?? DateTime.MinValue))
            return false;
        }

        if (!FiltRemedy.IsEmpty)
        {
          if (!TestRemedyFilter(_TableActions.DefaultView, FirstDate ?? DateTime.MinValue))
            return false;
        }
      }


      if (!FiltSoil.IsEmpty)
      {
        if (!TestSoilOrPotKind(_TableMovement.DefaultView, _TableActions.DefaultView, FiltSoil, FirstDate ?? DateTime.MinValue))
          return false;
      }

      if (!FiltPotKind.IsEmpty)
      {
        if (!TestSoilOrPotKind(_TableMovement.DefaultView, _TableActions.DefaultView, FiltPotKind, FirstDate ?? DateTime.MinValue))
          return false;
      }

      if (_TableDiseases != null)
      {
        _TableDiseases.DefaultView.RowFilter = new ValueFilter("DocId", DocId).ToString();
        if (!FiltDisease.IsEmpty)
        {
          if (!TestDiseaseFilter(_TableDiseases.DefaultView, FirstDate ?? DateTime.MinValue))
            return false;
        }
      }

      return true;
    }

    private bool TestPlaceFilter(DataView dv, DateTime FirstDate)
    {
      Int32 LastPlaceId = 0;
      foreach (DataRowView drv in dv)
      {
        MovementKind Kind = (MovementKind)DataTools.GetInt(drv.Row, "Kind");
        Int32 PlaceId = DataTools.GetInt(drv.Row, "Place");
        DateTime dt1 = DataTools.GetDateTime(drv.Row, "Date1");

        if (dt1 > FirstDate && LastPlaceId != 0 && FiltPlace.TestValue(LastPlaceId))
          return true;

        if (Kind == MovementKind.Add || Kind == MovementKind.Move)
          LastPlaceId = PlaceId;
        else
          LastPlaceId = 0;
      }
      if (LastPlaceId != 0 && FiltPlace.TestValue(LastPlaceId))
        return true;

      return false;
    }

    private bool TestHasAddFilter(DataView dv, DateTime FirstDate)
    {
      bool LastInPlace = false;
      foreach (DataRowView drv in dv)
      {
        MovementKind Kind = (MovementKind)DataTools.GetInt(drv.Row, "Kind");
        DateTime dt1 = DataTools.GetDateTime(drv.Row, "Date1");
        Int32 PlaceId = DataTools.GetInt(drv.Row, "Place");
        bool InPlace = true;
        if (!FiltPlace.IsEmpty)
          InPlace = FiltPlace.TestValue(PlaceId);

        switch (Kind)
        {
          case MovementKind.Add:
            if (dt1 >= FirstDate && InPlace)
              return true;
            LastInPlace = InPlace;
            break;
          case MovementKind.Move:
            if (dt1 >= FirstDate && InPlace && (!LastInPlace))
              return true;
            LastInPlace = InPlace;
            break;
          case MovementKind.Remove:
            LastInPlace = false;
            break;
        }
      }

      return false;
    }

    private bool TestHasRemoveFilter(DataView dv, DateTime FirstDate)
    {
      bool LastInPlace = false;
      foreach (DataRowView drv in dv)
      {
        MovementKind Kind = (MovementKind)DataTools.GetInt(drv.Row, "Kind");
        DateTime dt1 = DataTools.GetDateTime(drv.Row, "Date1");
        Int32 PlaceId = DataTools.GetInt(drv.Row, "Place");
        bool InPlace = true;
        if (!FiltPlace.IsEmpty)
          InPlace = FiltPlace.TestValue(PlaceId);

        switch (Kind)
        {
          case MovementKind.Add:
            LastInPlace = InPlace;
            break;
          case MovementKind.Move:
            if (dt1 >= FirstDate && (!InPlace) && LastInPlace)
              return true;
            LastInPlace = InPlace;
            break;
          case MovementKind.Remove:
            if (dt1 >= FirstDate && LastInPlace)
              return true;
            LastInPlace = false;
            break;
        }
      }

      return false;
    }

    private bool TestContraFilter(DataView dv, DateTime FirstDate, bool IsFromContra)
    {
      // ��������� ������ �������� ������� ��� ������� � �������

      foreach (DataRowView drv in dv)
      {
        DateTime dt1 = DataTools.GetDateTime(drv.Row, "Date1");
        if (dt1 < FirstDate)
          continue;
        MovementKind Kind = (MovementKind)DataTools.GetInt(drv.Row, "Kind");
        Int32 ContraId = DataTools.GetInt(drv.Row, "Contra");

        switch (Kind)
        {
          case MovementKind.Add:
            if (IsFromContra)
            {
              if (FiltFromContra.TestValue(ContraId))
                return true;
            }
            break;
          case MovementKind.Remove:
            if (!IsFromContra)
            {
              if (FiltToContra.TestValue(ContraId))
                return true;
            }
            break;
        }
      }

      return false;
    }

    private bool TestActionFilter(DataView dv, DateTime FirstDate)
    {
      foreach (DataRowView drv in dv)
      {
        DateTime dt1 = DataTools.GetDateTime(drv.Row, "Date1");
        if (dt1 < FirstDate)
          continue;
        ActionKind Kind = (ActionKind)DataTools.GetInt(drv.Row, "Kind");
        if (FiltAction.TestValue((int)Kind))
          return true;
      }
      return false;
    }

    private bool TestRemedyFilter(DataView dv, DateTime FirstDate)
    {
      foreach (DataRowView drv in dv)
      {
        DateTime dt1 = DataTools.GetDateTime(drv.Row, "Date1");
        if (dt1 < FirstDate)
          continue;
        ActionKind Kind = (ActionKind)DataTools.GetInt(drv.Row, "Kind");
        Int32 RemedyId = DataTools.GetInt(drv.Row, "Remedy");
        if (Kind == ActionKind.Treatment)
        {
          if (FiltRemedy.TestValue(RemedyId))
            return true;
        }
      }
      return false;
    }

    private bool TestDiseaseFilter(DataView dv, DateTime FirstDate)
    {
      foreach (DataRowView drv in dv)
      {
        DateTime dt1 = DataTools.GetDateTime(drv.Row, "Date1");
        if (dt1 < FirstDate)
          continue;
        Int32 DiseaseId = DataTools.GetInt(drv.Row, "Disease");
        if (FiltDisease.TestValue(DiseaseId))
          return true;
      }
      return false;
    }

    private bool TestSoilOrPotKind(DataView dvMovement, DataView dvActions, RefDocGridFilter Filt, DateTime FirstDate)
    {
      #region ��������� ������� ��� ����������� ��������

      _TempTableSoilAndPotKind.Rows.Clear();
      foreach (DataRowView drv in dvMovement)
      {
        MovementKind Kind = (MovementKind)DataTools.GetInt(drv.Row, "Kind");
        if (Kind == MovementKind.Add)
        {
          DataRow TempRow = _TempTableSoilAndPotKind.Rows.Add(drv.Row["Date1"], drv.Row["Date2"]);
          TempRow[Filt.ColumnName] = drv.Row[Filt.ColumnName];
        }
      }
      foreach (DataRowView drv in dvActions)
      {
        ActionKind Kind = (ActionKind)DataTools.GetInt(drv.Row, "Kind");
        switch (Filt.ColumnName)
        {
          case "Soil":
            if (!PlantTools.IsSoilAppliable(Kind, true))
              continue;
            break;
          case "PotKind":
            if (!PlantTools.IsPotKindAppliable(Kind, true))
              continue;
            break;
          default:
            throw new ArgumentException("����������� ������ " + Filt.ColumnName, "Filt");
        }
        object Id = drv.Row[Filt.ColumnName];
        if (Id != null)
        {
          DataRow TempRow = _TempTableSoilAndPotKind.Rows.Add(drv.Row["Date1"], drv.Row["Date2"]);
          TempRow[Filt.ColumnName] = Id;
        }
      }

      #endregion

      #region ������ ���������� ������������ ������

      Int32 PrevId = 0;
      bool NeedsTestPrev = false;

      foreach (DataRowView drv in _TempTableSoilAndPotKind.DefaultView)
      {
        Int32 Id = DataTools.GetInt(drv.Row, Filt.ColumnName);
        DateTime dt1 = DataTools.GetDateTime(drv.Row, "Date1");
        if (dt1 >= FirstDate)
        {
          if (Filt.TestValue(Id))
            return true;
        }
        else
        {
          PrevId = Id;
          NeedsTestPrev = true;
        }
      }

      #endregion

      #region �������� ��������������� ��������

      if (NeedsTestPrev)
        return Filt.TestValue(PrevId);
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
