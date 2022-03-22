using System;
using System.Collections.Generic;
using System.Text;
using FreeLibSet.Forms;
using FreeLibSet.IO;
using System.Windows.Forms;
using System.Reflection;

namespace Plants
{
  /// <summary>
  /// ������� ���� ���������
  /// </summary>
  internal static class MainMenu
  {
    #region �������������

    public static void Init()
    {
      EFPAppToolBarCommandItems SpeedPanelStandard = EFPApp.ToolBars.Add("Standard", "�����������");

      EFPCommandItem ci;

      #region ����

      EFPCommandItem menuFile = EFPApp.CommandItems.Add(EFPAppStdCommandItems.MenuFile, null);

      ci = EFPApp.CommandItems.Add(EFPAppStdCommandItems.Exit, menuFile);
      ci.Click += new EventHandler(EFPApp.CommandItems.Exit_Click);
      ci.GroupBegin = true;

      #endregion

      #region ������

      EFPCommandItem menuEdit = EFPApp.CommandItems.Add(EFPAppStdCommandItems.MenuEdit, null);

      ci = EFPApp.CommandItems.Add(EFPAppStdCommandItems.Cut, menuEdit);
      ci.Enabled = false;
      ci.GroupBegin = true;
      //SpeedPanelStandard.Add(Cut);

      ci = EFPApp.CommandItems.Add(EFPAppStdCommandItems.Copy, menuEdit);
      ci.Enabled = false;
      //SpeedPanelStandard.Add(Copy);

      ci = EFPApp.CommandItems.Add(EFPAppStdCommandItems.Paste, menuEdit);
      ci.Enabled = false;
      //SpeedPanelStandard.Add(Paste);

      ci = EFPApp.CommandItems.Add(EFPAppStdCommandItems.PasteSpecial, menuEdit);
      ci.Enabled = false;
      ci.GroupEnd = true;

      ci = EFPApp.CommandItems.Add(EFPAppStdCommandItems.Find, menuEdit);
      ci.Enabled = false;
      ci.GroupBegin = true;
      //SpeedPanelStandard.Add(Find);

      ci = EFPApp.CommandItems.Add(EFPAppStdCommandItems.IncSearch, menuEdit);
      ci.Enabled = false;
      //SpeedPanelStandard.Add(IncSearch);

      ci = EFPApp.CommandItems.Add(EFPAppStdCommandItems.FindNext, menuEdit);
      ci.Enabled = false;
      //SpeedPanelStandard.Add(FindNext);

      //ci = EFPApp.CommandItems.Add(EFPAppStdCommandItems.Replace, MenuEdit);
      //ci.Enabled = false;
      //SpeedPanelStandard.Add(Replace);

      //ci = EFPApp.CommandItems.Add(EFPAppStdCommandItems.Goto, MenuEdit);
      //ci.Enabled = false;
      //ci.GroupEnd = true;

      ci = EFPApp.CommandItems.Add(EFPAppStdCommandItems.SelectAll, menuEdit);
      ci.Enabled = false;
      ci.GroupBegin = true;
      ci.GroupEnd = true;

      #endregion

      #region ���

      EFPCommandItem menuView = EFPApp.CommandItems.Add(EFPAppStdCommandItems.MenuView, null);

      EFPAppCommandItemHelpers helpers = new EFPAppCommandItemHelpers();

      helpers.AddViewMenuCommands(menuView);


      #endregion

      #region �����������

      EFPCommandItem menuRB = new EFPCommandItem(null, "�����������");
      menuRB.MenuText = "��&���������";
      EFPApp.CommandItems.Add(menuRB);

      ci = ProgramDBUI.TheUI.DocTypes["Plants"].CreateMainMenuItem(menuRB);
      SpeedPanelStandard.Add(ci);

      ci = ProgramDBUI.TheUI.DocTypes["Care"].CreateMainMenuItem(menuRB);
      SpeedPanelStandard.Add(ci);

      ci = ProgramDBUI.TheUI.DocTypes["Places"].CreateMainMenuItem(menuRB);
      SpeedPanelStandard.Add(ci);

      ci = ProgramDBUI.TheUI.DocTypes["Contras"].CreateMainMenuItem(menuRB);
      SpeedPanelStandard.Add(ci);

      ci = ProgramDBUI.TheUI.DocTypes["Companies"].CreateMainMenuItem(menuRB);
      SpeedPanelStandard.Add(ci);

      ci = ProgramDBUI.TheUI.DocTypes["Diseases"].CreateMainMenuItem(menuRB);
      SpeedPanelStandard.Add(ci);

      ci = ProgramDBUI.TheUI.DocTypes["Remedies"].CreateMainMenuItem(menuRB);
      SpeedPanelStandard.Add(ci);

      ci = ProgramDBUI.TheUI.DocTypes["Soils"].CreateMainMenuItem(menuRB);
      SpeedPanelStandard.Add(ci);

      ci = ProgramDBUI.TheUI.DocTypes["PotKinds"].CreateMainMenuItem(menuRB);
      SpeedPanelStandard.Add(ci);

      ci = ProgramDBUI.TheUI.DocTypes["AttrTypes"].CreateMainMenuItem(menuRB);
      SpeedPanelStandard.Add(ci);


      #endregion

      #region ������

      EFPCommandItem menuReports = new EFPCommandItem(null, "Reports");
      menuReports.MenuText = "������";
      EFPApp.CommandItems.Add(menuReports);

      ci = new EFPCommandItem("Reports", "PlantSelReport");
      ci.Parent = menuReports;
      ci.MenuText = "������� �� �������� ��������";
      ci.ImageKey = "PlantSelReport";
      ci.Click += new EventHandler(ciPlantSelReport_Click);
      ci.GroupBegin = true;
      EFPApp.CommandItems.Add(ci);
      SpeedPanelStandard.Add(ci);

      ci = new EFPCommandItem("Reports", "CareReport");
      ci.Parent = menuReports;
      ci.MenuText = "������� ����� �� ����������";
      ci.ImageKey = "CareReport";
      ci.Click += new EventHandler(ciCareReport_Click);
      EFPApp.CommandItems.Add(ci);
      SpeedPanelStandard.Add(ci);

      ci = new EFPCommandItem("Reports", "FloweringReport");
      ci.Parent = menuReports;
      ci.MenuText = "�������� ��������";
      ci.ImageKey = "FloweringReport";
      ci.Click += new EventHandler(ciFloweringReport_Click);
      EFPApp.CommandItems.Add(ci);
      SpeedPanelStandard.Add(ci);

      ci = new EFPCommandItem("Reports", "ReplantingReport");
      ci.Parent = menuReports;
      ci.MenuText = "���������";
      ci.ImageKey = "ReplantingReport";
      ci.Click += new EventHandler(ciReplantingReport_Click);
      EFPApp.CommandItems.Add(ci);
      SpeedPanelStandard.Add(ci);

      ci = new EFPCommandItem("Reports", "PlanReport");
      ci.Parent = menuReports;
      ci.MenuText = "��������������� ��������";
      ci.ImageKey = "PlanReport";
      ci.Click += new EventHandler(ciPlanReport_Click);
      ci.GroupEnd = true;
      EFPApp.CommandItems.Add(ci);
      SpeedPanelStandard.Add(ci);

      #endregion

      #region ������

      EFPCommandItem menuService = new EFPCommandItem(null, "������");
      menuService.MenuText = "�&�����";
      EFPApp.CommandItems.Add(menuService);

      ci = new EFPCommandItem("������", "���������");
      ci.Parent = menuService;
      ci.MenuText = "���������";
      ci.ImageKey = "Settings";
      ci.Click += new EventHandler(ciSetting_Click);
      EFPApp.CommandItems.Add(ci);
      SpeedPanelStandard.Add(ci);

      ci = new EFPCommandItem("������", "����������������������������");
      ci.Parent = menuService;
      ci.MenuText = "�������� �������� ������������";
      ci.ImageKey = "UserActions";
      ci.Click += new EventHandler(ciUserActions_Click);
      EFPApp.CommandItems.Add(ci);
      //SpeedPanelStandard.Add(ci);

      #endregion

      #region �������

      EFPCommandItem menuDebug = new EFPCommandItem(null, "�������");
      menuDebug.MenuText = "�������";
      menuDebug.ImageKey = "Debug";
      menuDebug.Parent = menuService;
      menuDebug.GroupBegin = true;
      menuDebug.GroupEnd = true;
      EFPApp.CommandItems.Add(menuDebug);

      ci = new EFPCommandItem("�������", "������������������������");
      ci.Parent = menuDebug;
      ci.MenuText = "���������� ��������������";
      ci.ImageKey = "������������������������";
      ci.Click += new EventHandler(ShowIds_Click);
      EFPApp.CommandItems.Add(ci);

      #endregion

      #region ����

      EFPCommandItem menuWindow = EFPApp.CommandItems.Add(EFPAppStdCommandItems.MenuWindow, null);
      helpers.AddWindowMenuCommands(menuWindow);

      #endregion

      #region �������

      EFPCommandItem menuHelp = EFPApp.CommandItems.Add(EFPAppStdCommandItems.MenuHelp, null);

      ci = EFPApp.CommandItems.CreateContext(EFPAppStdCommandItems.About);
      ci.Parent = menuHelp;
      ci.Click += new EventHandler(AboutClick);
      EFPApp.CommandItems.Add(ci);

      #endregion


      // ������ �������� ��� ���������� ������ �������
      EFPApp.CommandItems.InitMenuVisible();

    }

    #endregion

    #region ������ ���������� ������

    #region ���� "����"


    #endregion

    #region ���

    static void RestoreBars_Click(object sender, EventArgs args)
    {
      foreach (EFPAppToolBar toolBar in EFPApp.AppToolBars)
      {
        toolBar.Reset();
        if (toolBar.Name == "������")
          toolBar.Visible = false;
      }
      EFPApp.AppToolBars.Attach();

      // ClientStatusBar.StatusBar.Visible = true;
    }

    #endregion

    #region ���� "������"

    static void ciPlantSelReport_Click(object sender, EventArgs args)
    {
      new PlantSelReport().Run();
    }

    static void ciFloweringReport_Click(object sender, EventArgs args)
    {
      new FloweringReport().Run();
    }

    static void ciReplantingReport_Click(object sender, EventArgs args)
    {
      new ReplantingReport().Run();
    }

    static void ciCareReport_Click(object sender, EventArgs args)
    {
      new CareReport().Run();
    }

    static void ciPlanReport_Click(object sender, EventArgs args)
    {
      new PlanReport().Run();
    }

    #endregion

    #region ���� "������"

    static void ciSetting_Click(object sender, EventArgs args)
    {
      UserSettingsForm form = new UserSettingsForm();
      form.ValueToForm(ProgramDBUI.Settings);
      if (EFPApp.ShowDialog(form, true) != DialogResult.OK)
        return;
      form.ValueFromForm(ProgramDBUI.Settings);
      ProgramDBUI.Settings.WriteConfig();
    }

    //static void ciClearBuf_Click(object Sender, EventArgs Args)
    //{
    //  Accoo2DBUI.TheUI.DocProvider.ClearCache();
    //  Cache.Clear();
    //  Accoo2DBUI.Cache.Clear("��������� ������� \"����� ����������� ������\"");
    //  Accoo2DBUI.ConfigSections.ClearBuf();
    //  Accoo2DBUI.Docs.Clear(); // 26.11.2015
    //}

    static void ciUserActions_Click(object sender, EventArgs args)
    {
      ProgramDBUI.TheUI.ShowUserActions();
    }

    #endregion

    #region ���� "�������"

    static private void ShowIds_Click(object sender, EventArgs args)
    {
      EFPCommandItem ci = (EFPCommandItem)sender;
      ProgramDBUI.TheUI.DebugShowIds = !ProgramDBUI.TheUI.DebugShowIds;
      ci.Checked = ProgramDBUI.TheUI.DebugShowIds;
      // ����� �������� ������ ���������� �����
      EFPFormProvider.DebugFormProvider = ProgramDBUI.TheUI.DebugShowIds;
      EFPPasteHandler.PasteSpecialDebugMode = ProgramDBUI.TheUI.DebugShowIds;
    }

    #endregion

    #region ���� "����"

    public static void AboutClick(object sender, EventArgs args)
    {
      AboutDialog dlg = new AboutDialog(Assembly.GetExecutingAssembly());
      dlg.ShowDialog();
    }

    #endregion

    #endregion
  }
}
