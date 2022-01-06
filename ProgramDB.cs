using System;
using System.Collections.Generic;
using System.Text;
using FreeLibSet.IO;
using FreeLibSet.Data.Docs;
using FreeLibSet.Logging;
using FreeLibSet.Data;
using System.IO;
using System.Data;
using FreeLibSet.Core;

namespace Plants
{
  /// <summary>
  /// ����������� � ���� ������
  /// </summary>
  internal class ProgramDB : DisposableObject
  {
    #region ����������� � Dispose

    /// <summary>
    /// ����������� ������ �� ������.
    /// ������������� ����������� � ������ InitDB()
    /// </summary>
    public ProgramDB()
    {
    }

    protected override void Dispose(bool Disposing)
    {
      if (Disposing)
      {
        if (_GlobalDocData != null)
        {
          _GlobalDocData.DisposeDBs();
          _GlobalDocData = null;
        }
      }
      if (_DocTypes != null)
        _DocTypes = null;

      _GlobalDocData = null;

      base.Dispose(Disposing);
    }

    #endregion

    #region InitDB()

    public void InitDB(AbsPath DBDir, ISplash Splash)
    {
      #region ���������� ����� ����������

      string OldPT = Splash.PhaseText;
      Splash.PhaseText = "���������� ����� ����������";

      InitDocTypes();

      Splash.PhaseText = OldPT;

      #endregion

      // ������� ����� FileTools.ForceDirs(DBDir); // ����� �������

      #region ������������� DBConnectionHelper

      InitDBConnectionHelper(DBDir);

      #endregion

      #region �������� / ���������� ��� ������ ����������

      _DBConnectionHelper.Splash = Splash;
      _GlobalDocData = _DBConnectionHelper.CreateRealDocProviderGlobal();
      _DBConnectionHelper.Splash = null; // �� ����� ��������
      if (_DBConnectionHelper.Errors.Count > 0)
      {
        // ������������ ��������� � log-�����
        AbsPath LogFilePath = LogoutTools.GetLogFileName("DBChange", String.Empty);
        using (StreamWriter wrt = new StreamWriter(LogFilePath.Path, false, LogoutTools.LogEncoding))
        {
          wrt.WriteLine("��������� ��������� ��� ������ Plants");
          wrt.WriteLine("�����: " + DateTime.Now.ToString());
          DBx[] AllDBx = _GlobalDocData.GetDBs();
          if (AllDBx.Length > 0) // ������ �����������
          {
            wrt.WriteLine("������ �������: " + AllDBx[0].ServerVersionText);
            wrt.WriteLine("��� ���� ������:");
            for (int i = 0; i < AllDBx.Length; i++)
            {
              wrt.WriteLine((i + 1).ToString() + ". " + AllDBx[i].DisplayName + ". " + AllDBx[i].UnpasswordedConnectionString);
            }
          }
          wrt.Write("----------");
          wrt.WriteLine();
          wrt.Write(_DBConnectionHelper.Errors.AllText);
        }
      }

      using (DBxCon Con = new DBxCon(GlobalDocData.MainDBEntry))
      {
        _DataVersionHandler.InitTableRow(Con);
      }

      #endregion

      #region ����� ������������

      _Source = new DBxRealDocProviderSource(GlobalDocData);

      //UserPermissionCreators upcs = new UserPermissionCreators();
      //UserPermissions ups = new UserPermissions(upcs);
      //_Source.UserPermissions = ups;

      _Source.SetReadOnly();

      #endregion

      _DBDir = DBDir;
    }

    /// <summary>
    /// ������� ���� ������
    /// </summary>
    public static AbsPath DBDir { get { return _DBDir; } }
    private static AbsPath _DBDir;

    #endregion

    #region ���������� ����� ����������

    /// <summary>
    /// ���������� ����� ����������
    /// </summary>
    public DBxDocTypes DocTypes { get { return _DocTypes; } }
    private DBxDocTypes _DocTypes;

    private void InitDocTypes()
    {
      _DocTypes = new DBxDocTypes();

      _DocTypes.UseDeleted = true; // ������������� �������� ����������
      _DocTypes.UseVersions = true; // �� ���������� ������ ����������
      _DocTypes.UseTime = true; // �� ���������� �����
      _DocTypes.UsersTableName = null; // �� ������������

      DBxDocType dt;
      DBxSubDocType sdt;

      #region �������

      #region ������

      dt = new DBxDocType("PlantGroups");
      dt.SingularTitle = "������ ��������";
      dt.PluralTitle = "������ ��������";
      dt.Struct.Columns.AddString("Name", 40, false);
      dt.Struct.Columns.AddReference("ParentId", "PlantGroups", true); // ���������� ������ �����
      dt.TreeParentColumnName = "ParentId";
      dt.DefaultOrder = new DBxOrder("Name");
      _DocTypes.Add(dt);

      #endregion

      #region �������� ��������

      dt = new DBxDocType("Plants");
      dt.PluralTitle = "��������";
      dt.SingularTitle = "��������";
      dt.Struct.Columns.AddInt16("Number");
      dt.Struct.Columns.AddString("LocalName", 120, true);
      dt.Struct.Columns.AddString("LatinName", 120, true);
      dt.Struct.Columns.AddString("Description", 120, true);
      dt.Struct.Columns.AddReference("Manufacturer", "Companies", true); // 30.06.2019
      dt.Struct.Columns.AddReference("Care", "Care", true); // 04.07.2019
      dt.Struct.Columns.AddMemo("Comment");

      dt.Struct.Columns.AddReference("Photo", "PlantPhotos", true); // ����������� ��� �������� - ������ �� �����������

      dt.Struct.Columns.AddReference("GroupId", "PlantGroups", true);
      dt.GroupRefColumnName = "GroupId";

      #region ����������� ����

      dt.Struct.Columns.AddString("Name", 120, false);
      dt.CalculatedColumns.Add("Name");
      dt.Struct.Columns.AddReference("Place", "Places", true);
      dt.CalculatedColumns.Add("Place");

      dt.Struct.Columns.AddReference("FromContra", "Contras", true);
      dt.CalculatedColumns.Add("FromContra");
      dt.Struct.Columns.AddReference("ToContra", "Contras", true);
      dt.CalculatedColumns.Add("ToContra");

      dt.Struct.Columns.AddReference("FromPlant", "Plants", true);
      dt.CalculatedColumns.Add("FromPlant");
      dt.Struct.Columns.AddReference("ToPlant", "Plants", true);
      dt.CalculatedColumns.Add("ToPlant");

      // ���� �������
      dt.Struct.Columns.AddDate("AddDate1", true);
      dt.Struct.Columns.AddDate("AddDate2", true);
      dt.CalculatedColumns.Add("AddDate1");
      dt.CalculatedColumns.Add("AddDate2");

      // ���� �������
      dt.Struct.Columns.AddDate("RemoveDate1", true);
      dt.Struct.Columns.AddDate("RemoveDate2", true);
      dt.CalculatedColumns.Add("RemoveDate1");
      dt.CalculatedColumns.Add("RemoveDate2");

      // ��������� ��������
      dt.Struct.Columns.AddReference("LastPlantAction", "PlantActions", true);
      dt.CalculatedColumns.Add("LastPlantAction");
      // ��������� ���������
      dt.Struct.Columns.AddReference("LastPlantReplanting", "PlantActions", true);
      dt.CalculatedColumns.Add("LastPlantReplanting");
      // ��������� �����������
      dt.Struct.Columns.AddReference("LastPlantDisease", "PlantDiseases", true);
      dt.CalculatedColumns.Add("LastPlantDisease");

      dt.Struct.Columns.AddInt("MovementState", DataTools.GetEnumRange(typeof(PlantMovementState))).Nullable = true;
      dt.CalculatedColumns.Add("MovementState");

      dt.Struct.Columns.AddReference("Soil", "Soils", true);
      dt.CalculatedColumns.Add("Soil");
      dt.Struct.Columns.AddReference("PotKind", "PotKinds", true);
      dt.CalculatedColumns.Add("PotKind");

      dt.Struct.Columns.AddReference("FirstPlannedAction", "PlantPlans", true); // ������ ��������������� ��������
      dt.CalculatedColumns.Add("FirstPlannedAction");

      #endregion

      dt.BeforeWrite += new ServerDocTypeBeforeWriteEventHandler(Plant_BeforeWrite);
      dt.DefaultOrder = new DBxOrder("Name");
      dt.Struct.Indexes.Add("Name");
      _DocTypes.Add(dt);

      #endregion

      #region ����

      sdt = new DBxSubDocType("PlantPhotos");
      sdt.SingularTitle = "���������� ��������";
      sdt.PluralTitle = "���������� ��������";

      sdt.Struct.Columns.AddString("FileName", 80, false);
      sdt.Struct.Columns.AddString("FileMD5", 32, false);
      sdt.BinDataRefs.Add("ThumbnailData");
      //sdt.Struct.Columns.AddBinary("Thumbnail");
      sdt.Struct.Columns.AddDateTime("ShootingTime", true); // 24.11.2019. ����� ���� ������
      sdt.Struct.Columns.AddMemo("Comment");
      sdt.DefaultOrder = new DBxOrder("ShootingTime");
      dt.SubDocs.Add(sdt);

      #endregion

      #region ��������

      sdt = new DBxSubDocType("PlantAttributes");
      sdt.SingularTitle = "�������� ��������";
      sdt.PluralTitle = "��������";
      sdt.Struct.Columns.AddReference("AttrType", "AttrTypes", false);
      sdt.Struct.Columns.AddDate("Date", true);
      sdt.Struct.Columns.AddString("Value", PlantTools.AttrValueShortMaxLength, true); // �������� ����� ���� ������
      sdt.Struct.Columns.AddMemo("LongValue"); // ������������, ���� �� ���������� � ���� "��������"
      sdt.Struct.Columns.AddMemo("Comment"); // 06.02.2022
      sdt.DefaultOrder = new DBxOrder("Date");
      dt.SubDocs.Add(sdt);

      #endregion

      #region ������/�������

      sdt = new DBxSubDocType("PlantMovement");
      sdt.SingularTitle = "�������� ��������";
      sdt.PluralTitle = "�������� ��������";

      sdt.Struct.Columns.AddInt("Kind", DataTools.GetEnumRange(typeof(MovementKind))).Nullable = false;
      sdt.Struct.Columns.AddDate("Date1", false);
      sdt.Struct.Columns.AddDate("Date2", false);
      sdt.Struct.Columns.AddReference("Contra", "Contras", true); // ����� ���� ������ ��� �������� ������� � �������
      sdt.Struct.Columns.AddReference("ForkPlant", "Plants", true); // ����� ���� ������ ��� �������� ������� � �������
      sdt.Struct.Columns.AddReference("Place", "Places", true); // ������ ���� ������ ��� �������� ������� � �����������
      sdt.Struct.Columns.AddReference("Soil", "Soils", true); // ����� ������������ ��� �������� �������
      sdt.Struct.Columns.AddReference("PotKind", "PotKinds", true); // ����� ������������ ��� �������� �������
      sdt.Struct.Columns.AddMemo("Comment");
      sdt.DefaultOrder = new DBxOrder("Date1");
      dt.SubDocs.Add(sdt);

      #endregion

      #region ��������

      sdt = new DBxSubDocType("PlantActions");
      sdt.SingularTitle = "�������� � ���������";
      sdt.PluralTitle = "�������� � ����������";

      sdt.Struct.Columns.AddInt("Kind", DataTools.GetEnumRange(typeof(ActionKind))).Nullable = false;
      sdt.Struct.Columns.AddString("ActionName", 30, true); // ��� ���� "������"
      sdt.Struct.Columns.AddDate("Date1", false);
      sdt.Struct.Columns.AddDate("Date2", false);
      sdt.Struct.Columns.AddReference("Remedy", "Remedies", true);
      sdt.Struct.Columns.AddReference("RemedyUsage", "RemedyUsage", true); // 29.08.2019
      sdt.Struct.Columns.AddReference("Soil", "Soils", true);
      sdt.Struct.Columns.AddReference("PotKind", "PotKinds", true);
      sdt.Struct.Columns.AddMemo("Comment");
      sdt.DefaultOrder = new DBxOrder("Date1");
      dt.SubDocs.Add(sdt);

      #endregion

      #region ��������

      sdt = new DBxSubDocType("PlantFlowering");
      sdt.SingularTitle = "�������� ��������";
      sdt.PluralTitle = "�������� ��������";

      sdt.Struct.Columns.AddDate("Date1", false);
      sdt.Struct.Columns.AddDate("Date2", false);
      sdt.Struct.Columns.AddInt16("FlowerCount");
      sdt.Struct.Columns.AddMemo("Comment");
      sdt.DefaultOrder = new DBxOrder("Date1");
      dt.SubDocs.Add(sdt);

      #endregion

      #region �����������

      sdt = new DBxSubDocType("PlantDiseases");
      sdt.SingularTitle = "����������� ��������";
      sdt.PluralTitle = "����������� ��������";

      sdt.Struct.Columns.AddReference("Disease", "Diseases", true);
      sdt.Struct.Columns.AddDate("Date1", false);
      sdt.Struct.Columns.AddDate("Date2", false);
      sdt.Struct.Columns.AddMemo("Comment");
      sdt.DefaultOrder = new DBxOrder("Date1");
      dt.SubDocs.Add(sdt);

      #endregion

      #region �����

      sdt = new DBxSubDocType("PlantPlans");
      sdt.SingularTitle = "����������� ��������";
      sdt.PluralTitle = "����������� ��������";

      sdt.Struct.Columns.AddInt("Kind", DataTools.GetEnumRange(typeof(ActionKind))).Nullable = false;
      sdt.Struct.Columns.AddString("ActionName", 30, true); // ��� ���� "������"
      sdt.Struct.Columns.AddDate("Date1", false);
      sdt.Struct.Columns.AddDate("Date2", false);
      sdt.Struct.Columns.AddReference("Remedy", "Remedies", true);
      sdt.Struct.Columns.AddMemo("Comment");
      sdt.DefaultOrder = new DBxOrder("Date1");
      dt.SubDocs.Add(sdt);

      #endregion

      #endregion

      #region ���� ���������

      dt = new DBxDocType("AttrTypes");
      dt.SingularTitle = "��� ��������";
      dt.PluralTitle = "���� ���������";
      dt.Struct.Columns.AddString("Name", 30, false);
      dt.Struct.Columns.AddInt("Type", DataTools.GetEnumRange(typeof(ValueType)));
      dt.Struct.Columns.AddString("Format", 20, true);
      dt.Struct.Columns.AddInt("Source", DataTools.GetEnumRange(typeof(AttrValueSourceType)));
      dt.Struct.Columns.AddMemo("ValueList");
      dt.Struct.Columns.AddMemo("Comment");
      dt.DefaultOrder = new DBxOrder("Name");
      dt.BeforeWrite += new ServerDocTypeBeforeWriteEventHandler(AttrType_BeforeWrite);
      _DocTypes.Add(dt);

      #endregion

      #region �����

      #region ������

      dt = new DBxDocType("PlaceGroups");
      dt.SingularTitle = "������ ����";
      dt.PluralTitle = "������ ����";
      dt.Struct.Columns.AddString("Name", 40, false);
      dt.Struct.Columns.AddReference("ParentId", "PlaceGroups", true); // ���������� ������ �����
      dt.TreeParentColumnName = "ParentId";
      dt.DefaultOrder = new DBxOrder("Name");
      _DocTypes.Add(dt);

      #endregion

      #region �������� ��������

      dt = new DBxDocType("Places");
      dt.PluralTitle = "�����";
      dt.SingularTitle = "�����";
      dt.Struct.Columns.AddString("Name", 250, true);
      dt.Struct.Columns.AddMemo("Comment");

      dt.Struct.Columns.AddReference("GroupId", "PlaceGroups", true);
      dt.GroupRefColumnName = "GroupId";

      dt.DefaultOrder = new DBxOrder("Name");
      _DocTypes.Add(dt);

      #endregion

      #endregion

      #region �����������

      #region ������

      dt = new DBxDocType("ContraGroups");
      dt.SingularTitle = "������ ������������";
      dt.PluralTitle = "������ ������������";
      dt.Struct.Columns.AddString("Name", 40, false);
      dt.Struct.Columns.AddReference("ParentId", "ContraGroups", true); // ���������� ������ �����
      dt.TreeParentColumnName = "ParentId";
      dt.DefaultOrder = new DBxOrder("Name");
      _DocTypes.Add(dt);

      #endregion

      #region �������� ��������

      dt = new DBxDocType("Contras");
      dt.PluralTitle = "�����������";
      dt.SingularTitle = "����������";
      dt.Struct.Columns.AddString("Name", 250, true);
      //���� �� ������, ��� ���� dt.Struct.Columns.AddReference("Company", "Companies", true);
      dt.Struct.Columns.AddMemo("Comment");

      dt.Struct.Columns.AddReference("GroupId", "ContraGroups", true);
      dt.GroupRefColumnName = "GroupId";

      dt.DefaultOrder = new DBxOrder("Name");

      _DocTypes.Add(dt);

      #endregion

      #endregion

      #region ���� �����������

      #region ������

      dt = new DBxDocType("DiseaseGroups");
      dt.SingularTitle = "������ ����� �����������";
      dt.PluralTitle = "������ ����� �����������";
      dt.Struct.Columns.AddString("Name", 40, false);
      dt.Struct.Columns.AddReference("ParentId", "DiseaseGroups", true); // ���������� ������ �����
      dt.TreeParentColumnName = "ParentId";
      dt.DefaultOrder = new DBxOrder("Name");
      _DocTypes.Add(dt);

      #endregion

      #region �������� ��������

      dt = new DBxDocType("Diseases");
      dt.PluralTitle = "���� �����������";
      dt.SingularTitle = "��� �����������";
      dt.Struct.Columns.AddString("Name", 250, true);
      dt.Struct.Columns.AddMemo("Comment");

      dt.Struct.Columns.AddReference("GroupId", "DiseaseGroups", true);
      dt.GroupRefColumnName = "GroupId";

      dt.DefaultOrder = new DBxOrder("Name");

      _DocTypes.Add(dt);

      #endregion

      #endregion

      #region ���������

      #region ������

      dt = new DBxDocType("RemedyGroups");
      dt.SingularTitle = "������ ����������";
      dt.PluralTitle = "������ ����������";
      dt.Struct.Columns.AddString("Name", 40, false);
      dt.Struct.Columns.AddReference("ParentId", "RemedyGroups", true); // ���������� ������ �����
      dt.TreeParentColumnName = "ParentId";
      dt.DefaultOrder = new DBxOrder("Name");
      _DocTypes.Add(dt);

      #endregion

      #region �������� ��������

      dt = new DBxDocType("Remedies");
      dt.PluralTitle = "���������";
      dt.SingularTitle = "��������";
      dt.Struct.Columns.AddString("Name", 250, true);
      dt.Struct.Columns.AddReference("Manufacturer", "Companies", true);
      dt.Struct.Columns.AddMemo("Comment");

      dt.Struct.Columns.AddReference("GroupId", "RemedyGroups", true);
      dt.GroupRefColumnName = "GroupId";

      dt.DefaultOrder = new DBxOrder("Name");

      _DocTypes.Add(dt);

      #endregion

      #region ����������

      sdt = new DBxSubDocType("RemedyUsage");
      sdt.SingularTitle = "������� ���������� ���������";
      sdt.PluralTitle = "�������� ���������� ����������";

      sdt.Struct.Columns.AddString("Name", 100, false);

      sdt.DefaultOrder = new DBxOrder("Name");

      dt.SubDocs.Add(sdt);

      #endregion

      #endregion

      #region ������

      #region ������

      dt = new DBxDocType("SoilGroups");
      dt.SingularTitle = "������ �������";
      dt.PluralTitle = "������ �������";
      dt.Struct.Columns.AddString("Name", 40, false);
      dt.Struct.Columns.AddReference("ParentId", "SoilGroups", true); // ���������� ������ �����
      dt.TreeParentColumnName = "ParentId";
      dt.DefaultOrder = new DBxOrder("Name");
      _DocTypes.Add(dt);

      #endregion

      #region �������� ��������

      dt = new DBxDocType("Soils");
      dt.PluralTitle = "������";
      dt.SingularTitle = "�����";
      dt.Struct.Columns.AddString("Name", 250, true);
      dt.Struct.Columns.AddReference("Manufacturer", "Companies", true);
      dt.Struct.Columns.AddSingle("pHmin");
      dt.Struct.Columns.AddSingle("pHmax");
      dt.Struct.Columns.AddMemo("Comment");
      dt.Struct.Columns.AddMemo("Contents");
      dt.Struct.Columns.AddInt("PartCount");
      dt.CalculatedColumns.Add("Contents");
      dt.CalculatedColumns.Add("PartCount");
      dt.BeforeWrite += new ServerDocTypeBeforeWriteEventHandler(Soil_BeforeWrite);

      dt.Struct.Columns.AddReference("GroupId", "SoilGroups", true);
      dt.GroupRefColumnName = "GroupId";

      dt.DefaultOrder = new DBxOrder("Name");

      _DocTypes.Add(dt);

      #endregion

      #region ����������� "��������� ������"

      sdt = new DBxSubDocType("SoilParts");
      sdt.PluralTitle = "���������� ������";
      sdt.SingularTitle = "��������� ������";
      sdt.Struct.Columns.AddReference("Soil", "Soils", false);
      sdt.Struct.Columns.AddInt("Percent", 0, 100);
      sdt.Struct.Columns.AddInt16("Order"); // ��� ����������
      dt.SubDocs.Add(sdt);

      #endregion

      #endregion

      #region ���� ��������� �������

      #region ������

      dt = new DBxDocType("PotKindGroups");
      dt.SingularTitle = "������ ����� �������";
      dt.PluralTitle = "������ ����� �������";
      dt.Struct.Columns.AddString("Name", 40, false);
      dt.Struct.Columns.AddReference("ParentId", "SoilGroups", true); // ���������� ������ �����
      dt.TreeParentColumnName = "ParentId";
      dt.DefaultOrder = new DBxOrder("Name");
      _DocTypes.Add(dt);

      #endregion

      #region �������� ��������

      dt = new DBxDocType("PotKinds");
      dt.PluralTitle = "���� �������";
      dt.SingularTitle = "��� �������";
      dt.Struct.Columns.AddString("Text", 50, true);
      dt.Struct.Columns.AddReference("Manufacturer", "Companies", true);
      dt.Struct.Columns.AddInt16("Height"); // � �����������
      dt.Struct.Columns.AddInt16("Diameter"); // � �����������
      dt.Struct.Columns.AddSingle("Volume"); // � ������
      dt.Struct.Columns.AddString("Color", 20, true);

      dt.Struct.Columns.AddString("Name", 150, true);
      dt.CalculatedColumns.Add("Name");
      dt.BeforeWrite += new ServerDocTypeBeforeWriteEventHandler(PotKind_BeforeWrite);

      dt.Struct.Columns.AddMemo("Comment");
      dt.Struct.Columns.AddReference("GroupId", "PotKindGroups", true);
      dt.GroupRefColumnName = "GroupId";

      dt.DefaultOrder = new DBxOrder("Name");

      _DocTypes.Add(dt);

      #endregion

      #endregion

      #region �����������

      #region ������

      dt = new DBxDocType("CompanyGroups");
      dt.SingularTitle = "������ �����������";
      dt.PluralTitle = "������ �����������";
      dt.Struct.Columns.AddString("Name", 40, false);
      dt.Struct.Columns.AddReference("ParentId", "SoilGroups", true); // ���������� ������ �����
      dt.TreeParentColumnName = "ParentId";
      dt.DefaultOrder = new DBxOrder("Name");
      _DocTypes.Add(dt);

      #endregion

      #region �������� ��������

      dt = new DBxDocType("Companies");
      //dt.PluralTitle = "�����������";
      //dt.SingularTitle = "�����������";
      dt.PluralTitle = "������������";
      dt.SingularTitle = "������������";
      dt.Struct.Columns.AddString("Name", 250, true);
      dt.Struct.Columns.AddMemo("Comment");

      dt.Struct.Columns.AddReference("GroupId", "CompanyGroups", true);
      dt.GroupRefColumnName = "GroupId";

      dt.DefaultOrder = new DBxOrder("Name");

      _DocTypes.Add(dt);

      #endregion

      #endregion

      #region ����

      #region �������� ��������

      dt = new DBxDocType("Care");
      dt.PluralTitle = "���� �� ����������";
      dt.SingularTitle = "���� �� ����������";
      dt.Struct.Columns.AddString("Name", 250, true);
      dt.Struct.Columns.AddMemo("Comment");
      dt.Struct.Columns.AddReference("ParentId", "Care", true); // ��� ������������ �������
      dt.TreeParentColumnName= "ParentId";

      dt.Struct.Columns.AddReference("GroupId", "PlantGroups", true); // �� �� ������, ��� � � �������� ��������
      dt.GroupRefColumnName = "GroupId";

      dt.DefaultOrder = new DBxOrder("Name");
      _DocTypes.Add(dt);

      #endregion

      #region ������ �� �����

      sdt = new DBxSubDocType("CareRecords");
      sdt.SingularTitle = "������ �� ����� �� ����������";
      sdt.PluralTitle = "������ �� ����� �� ����������";

      sdt.Struct.Columns.AddInt("Day1", 0, 365);
      sdt.Struct.Columns.AddInt("Day2", 0, 365);
      sdt.Struct.Columns.AddString("Name", 50, true);

      foreach (CareItem Item in CareItem.TheList)
        Item.AddColumns(sdt.Struct.Columns);

      //sdt.Struct.Columns.AddMemo("Comment");
      sdt.DefaultOrder = new DBxOrder("Day1");
      dt.SubDocs.Add(sdt);

      #endregion

      #endregion

      #region �������������� �������������

      foreach (DBxDocType dt2 in DocTypes)
      {
        // ����-���� ������������ ������������ ������ � �������� ��������

        if (dt2.Struct.Columns.Contains("Comment"))
          dt2.IndividualCacheColumns["Comment"] = false;
      }

      #endregion
    }

    #endregion

    #region ����������� ���������� �� ������� �������

    #region AttrTypes

    void AttrType_BeforeWrite(object Sender, ServerDocTypeBeforeWriteEventArgs Args)
    {
      AttrValueSourceType Type = (AttrValueSourceType)(Args.Doc.Values["Source"].AsInteger);
      switch (Type)
      {
        case AttrValueSourceType.List:
          break;
        default:
          Args.Doc.Values["ValueList"].SetNull(); // ������� ������, ���� �� �� ������������
          break;
      }
    }

    #endregion

    #region Plants

    void Plant_BeforeWrite(object Sender, ServerDocTypeBeforeWriteEventArgs Args)
    {
      #region ��������

      if (!Args.Doc.Values["LatinName"].IsNull)
      {
        // 16.09.2019
        // ���� ���� � ������� � ��������� �������� - ������� ���
        string s = Args.Doc.Values["LatinName"].AsString;
        string s2 = Args.Doc.Values["LocalName"].AsString;
        if (s2.Length > 0)
          s += " / " + s2;
        Args.Doc.Values["Name"].SetValue(s);
      }
      else if (!Args.Doc.Values["LocalName"].IsNull)
        Args.Doc.Values["Name"].SetValue(Args.Doc.Values["LocalName"].Value);
      else if (!Args.Doc.Values["Description"].IsNull)
        Args.Doc.Values["Name"].SetValue(Args.Doc.Values["Description"].Value);
      else
        Args.Doc.Values["Name"].SetValue("?");

      #endregion

      DataTable Table;

      #region ����� � �����������

      Args.Doc.Values["Place"].SetNull();
      Args.Doc.Values["FromContra"].SetNull();
      Args.Doc.Values["ToContra"].SetNull();
      Args.Doc.Values["FromContra"].SetNull();
      Args.Doc.Values["ToPlant"].SetNull();
      Args.Doc.Values["FromPlant"].SetNull();
      Args.Doc.Values["MovementState"].SetNull();
      Table = Args.Doc.SubDocs["PlantMovement"].CreateSubDocsData();
      Table.DefaultView.Sort = "Date1";
      bool PlaceFound = false;
      bool RemoveDateFound = false;

      Int32 SoilId = 0;
      Int32 PotKindId = 0;
      DateTime LastAddDate = DateTime.MinValue;

      for (int i = Table.DefaultView.Count - 1; i >= 0; i--)
      {
        DataRow Row = Table.DefaultView[i].Row;
        MovementKind Kind = (MovementKind)(DataTools.GetInt(Row, "Kind"));

        #region �����

        if (!PlaceFound)
        {
          switch (Kind)
          {
            case MovementKind.Add:
            case MovementKind.Move:
              Int32 PlaceId = DataTools.GetInt(Row, "Place");
              Args.Doc.Values["Place"].SetInteger(PlaceId);
              PlaceFound = true;
              break;
          }
        }

        #endregion

        #region ����������

        Int32 ContraId = DataTools.GetInt(Row, "Contra");
        Int32 ForkPlantId = DataTools.GetInt(Row, "ForkPlant");
        switch (Kind)
        {
          case MovementKind.Add:
            if (Args.Doc.Values["FromContra"].IsNull && Args.Doc.Values["FromPlant"].IsNull)
            {
              Args.Doc.Values["FromContra"].SetInteger(ContraId);
              Args.Doc.Values["FromPlant"].SetInteger(ForkPlantId);
            }
            break;

          case MovementKind.Remove:
            Args.Doc.Values["ToContra"].SetInteger(ContraId);
            Args.Doc.Values["ToPlant"].SetInteger(ForkPlantId);
            break;
        }

        #endregion

        #region ������

        if (Kind == MovementKind.Add)
        {
          Args.Doc.Values["AddDate1"].SetValue(DataTools.GetNullableDateTime(Row, "Date1"));
          Args.Doc.Values["AddDate2"].SetValue(DataTools.GetNullableDateTime(Row, "Date2"));

          if (LastAddDate == DateTime.MinValue)
          {
            LastAddDate = DataTools.GetNullableDateTime(Row, "Date1").Value;
            SoilId = DataTools.GetInt(Row, "Soil");
            PotKindId = DataTools.GetInt(Row, "PotKind");
          }
        }
        if (Kind == MovementKind.Remove && (!RemoveDateFound))
        {
          Args.Doc.Values["RemoveDate1"].SetValue(DataTools.GetNullableDateTime(Row, "Date1"));
          Args.Doc.Values["RemoveDate2"].SetValue(DataTools.GetNullableDateTime(Row, "Date2"));
          RemoveDateFound = true;
        }

        #endregion

        #region ������� ���������

        if (Args.Doc.Values["MovementState"].IsNull)
        {
          switch (Kind)
          {
            case MovementKind.Add:
            case MovementKind.Move:
              Args.Doc.Values["MovementState"].SetInteger((int)PlantMovementState.Placed);
              break;
            case MovementKind.Remove:
              if (ContraId == 0)
              {
                if (ForkPlantId==0)
                  Args.Doc.Values["MovementState"].SetInteger((int)PlantMovementState.Dead);
                else
                  Args.Doc.Values["MovementState"].SetInteger((int)PlantMovementState.Merged);
              }
              else
                Args.Doc.Values["MovementState"].SetInteger((int)PlantMovementState.Given);
              break;
          }
        }

        #endregion
      }

      #endregion

      #region ��������

      Table = Args.Doc.SubDocs["PlantActions"].CreateSubDocsData();
      Table.DefaultView.Sort = "Date1";
      bool TransshipmentFound = false;

      Args.Doc.Values["LastPlantAction"].SetNull();
      Args.Doc.Values["LastPlantReplanting"].SetNull();

      for (int i = Table.DefaultView.Count - 1; i >= 0; i--)
      {
        DataRow Row = Table.DefaultView[i].Row;
        ActionKind Kind = (ActionKind)(DataTools.GetInt(Row, "Kind"));
        switch (Kind)
        {
          case ActionKind.Planting:
          case ActionKind.Replanting:
          case ActionKind.Transshipment:
          case ActionKind.SoilReplace:
            if (!TransshipmentFound)
            {
              Args.Doc.Values["LastPlantReplanting"].SetInteger(DataTools.GetInt(Row, "Id"));
              TransshipmentFound = true;
            }
            break;
        }

        if (i == Table.DefaultView.Count - 1)
        {
          Args.Doc.Values["LastPlantAction"].SetInteger(DataTools.GetInt(Row, "Id"));
        }
      }

      for (int i = 0; i < Table.DefaultView.Count; i++)
      {
        DataRow Row = Table.DefaultView[i].Row;
        DateTime Date = DataTools.GetNullableDateTime(Row, "Date1").Value;
        if (Date < LastAddDate)
          continue; // ��������� ��������� ��������
        ActionKind Kind = (ActionKind)(DataTools.GetInt(Row, "Kind"));
        if (PlantTools.IsSoilAppliable(Kind, true))
        {
          Int32 ThisSoilId = DataTools.GetInt(Row, "Soil");
          if (ThisSoilId != 0)
            SoilId = ThisSoilId;
        }
        if (PlantTools.IsPotKindAppliable(Kind, true))
        {
          Int32 ThisPotKindId = DataTools.GetInt(Row, "PotKind");
          if (ThisPotKindId != 0)
            PotKindId = ThisPotKindId;
        }
      }

      #endregion

      #region �����������

        Table = Args.Doc.SubDocs["PlantDiseases"].CreateSubDocsData();
      Table.DefaultView.Sort = "Date1";

      if (Table.DefaultView.Count > 0)
      {
        DataRow Row = Table.DefaultView[Table.DefaultView.Count - 1].Row;
        Args.Doc.Values["LastPlantDisease"].SetInteger(DataTools.GetInt(Row,"Id"));
      }
      else
      {
        Args.Doc.Values["LastPlantDisease"].SetNull();
      }

      #endregion

      Args.Doc.Values["Soil"].SetInteger(SoilId);
      Args.Doc.Values["PotKind"].SetInteger(PotKindId);

      #region ��������������� ��������

      Table = Args.Doc.SubDocs["PlantPlans"].CreateSubDocsData();
      Table.DefaultView.Sort = "Date1";
      if (Table.DefaultView.Count == 0)
        Args.Doc.Values["FirstPlannedAction"].SetNull();
      else
        Args.Doc.Values["FirstPlannedAction"].SetInteger((Int32)(Table.DefaultView[0]["Id"]));

      #endregion
    }

    #endregion

    #region Soil

    void Soil_BeforeWrite(object Sender, ServerDocTypeBeforeWriteEventArgs Args)
    {
      if (Args.Doc.SubDocs["SoilParts"].SubDocCount == 0)
      {
        Args.Doc.Values["Contents"].SetNull();
        Args.Doc.Values["PartCount"].SetNull();
      }
      else
      {
        DataTable Table = Args.Doc.SubDocs["SoilParts"].CreateSubDocsData();
        Table.DefaultView.Sort = "Order";
        string[] a = new string[Table.DefaultView.Count];
        for (int i = 0; i < a.Length; i++)
        {
          DataRow Row = Table.DefaultView[i].Row;
          Int32 SoilId = DataTools.GetInt(Row, "Soil");
          a[i] = Args.DBCache["Soils"].GetString(SoilId, "Name");
          int prc = DataTools.GetInt(Row, "Percent");
          if (prc > 0)
            a[i] += " " + prc.ToString() + "%";
        }
        Args.Doc.Values["Contents"].SetString(String.Join(", ", a));
        Args.Doc.Values["PartCount"].SetInteger(a.Length);
      }
    }

    #endregion

    #region PotKinds

    void PotKind_BeforeWrite(object Sender, ServerDocTypeBeforeWriteEventArgs Args)
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(Args.Doc.Values["Text"].AsString);
      float v = Args.Doc.Values["Volume"].AsSingle;
      if (v > 0)
      {
        if (sb.Length > 0)
          sb.Append(", ");
        sb.Append(v);
        sb.Append("�");
      }
      else
      {
        int d = Args.Doc.Values["Diameter"].AsInteger;
        if (d > 0)
        {
          if (sb.Length > 0)
            sb.Append(", ");
          sb.Append("d=");
          sb.Append(d);
          sb.Append("��");
        }

        int h = Args.Doc.Values["Height"].AsInteger;
        if (h > 0)
        {
          if (sb.Length > 0)
            sb.Append(", ");
          sb.Append("h=");
          sb.Append(h);
          sb.Append("��");
        }
      }

      string clr = Args.Doc.Values["Color"].AsString;
      if (clr.Length > 0)
      {
        if (sb.Length > 0)
          sb.Append(", ");
        sb.Append(clr);
      }

      Args.Doc.Values["Name"].SetString(sb.ToString());
    }

    #endregion

    #endregion

    #region DBConnectionHelper

    /// <summary>
    /// ��������� ��� ������
    /// </summary>
    private DBxDocDBConnectionHelper _DBConnectionHelper;

    private DBxDataVersionHandler _DataVersionHandler;

    /// <summary>
    /// ������������� ������� DBxDocDBConnectionHelper
    /// </summary>
    /// <param name="DBDir"></param>
    /// <returns></returns>
    public void InitDBConnectionHelper(AbsPath DBDir)
    {
      _DBConnectionHelper = new DBxDocDBConnectionHelper();
      _DBConnectionHelper.DBDir = DBDir;

      _DBConnectionHelper.ProviderName = "SQLite";
      AbsPath FileName = new AbsPath(DBDir, "db.db");
      _DBConnectionHelper.ConnectionString = "Data Source=" + FileName.Path;

      _DBConnectionHelper.CommandTimeout = 0; // ����������� ����� ���������� ������
      _DBConnectionHelper.DocTypes = _DocTypes;

      #region ��������� ������������� (������ ������������)

      DBxTableStruct ts = new DBxTableStruct("UserSettings");
      ts.Columns.AddId();
      ts.Columns.AddString("Name", ConfigSection.MaxSectionNameLength, false);
      ts.Columns.AddString("Category", ConfigSection.MaxCategoryLength, false);
      ts.Columns.AddString("ConfigName", ConfigSection.MaxConfigNameLength, true);
      ts.Columns.AddXmlConfig("Data");
      ts.Columns.AddDateTime("WriteTime", true); // ���� ��������� ������
      //ts.Indices.Add("������������");
      ts.Indexes.Add("Name,Category,ConfigName");

      _DBConnectionHelper.MainDBStruct.Tables.Add(ts);

      #endregion

      _DataVersionHandler = new DBxDataVersionHandler(new Guid("26a8bf65-f637-4921-a224-aa6705c45f89"), 1, 1);
      _DataVersionHandler.AddTableStruct(_DBConnectionHelper.MainDBStruct);
    }

    #endregion

    #region ����� �����������

    /// <summary>
    /// �������� ������ ������� ����������
    /// </summary>
    public DBxRealDocProviderGlobal GlobalDocData { get { return _GlobalDocData; } }
    private DBxRealDocProviderGlobal _GlobalDocData;

    public DBxRealDocProviderSource Source { get { return _Source; } }
    private DBxRealDocProviderSource _Source;

    /// <summary>
    /// �������� ����������� � ���� ������ � ������ ������� ���� (��� ��������, ����������� ����������)
    /// </summary>
    public DBxEntry MainEntry { get { return _GlobalDocData.MainDBEntry; } }

    #endregion

    public DBxDocProvider CreateDocProvider()
    {
      return new DBxRealDocProvider(Source, 0, false);
    }

    #region ��������� �����������

    public static AbsPath BackupDir
    {
      get
      {
        if (ProgramDBUI.Settings == null)
          return AbsPath.Empty;
        else
          return FileTools.ApplicationBaseDir + new RelPath(ProgramDBUI.Settings.BackupDir);
      }
    }

    internal void CreateBackup(ISplash spl)
    {
      FileTools.ForceDirs(BackupDir);
      FileCompressor fc = new FileCompressor();
      fc.ArchiveFileName = new AbsPath(BackupDir, DateTime.Now.ToString(@"yyyyMMdd\-HHmmss", StdConvert.DateTimeFormat) + ".7z");
      fc.FileDirectory = _DBConnectionHelper.DBDir;
      fc.FileTemplates.Add("db.db");
      fc.FileTemplates.Add("undo.db");
      fc.Compress();
    }

    internal void RemoveOldBackups(ISplash spl)
    {
      spl.PhaseText = "����� ������ �����";

      string[] aFiles = System.IO.Directory.GetFiles(BackupDir.Path, "????????-??????.7z", SearchOption.TopDirectoryOnly);
      for (int i = 0; i < aFiles.Length; i++)
      {
        DateTime dt = System.IO.File.GetLastWriteTime(aFiles[i]);
        TimeSpan ts = DateTime.Today - dt;
        if (ts.TotalDays > 7)
        {
          spl.PhaseText = "�������� " + aFiles[i];
          try
          {
            System.IO.File.Delete(aFiles[i]);
          }
          catch { }
        }
      }
    }

    #endregion
  }
}
