using System;
using System.Collections.Generic;
using System.Text;
using AgeyevAV.ExtForms;
using AgeyevAV.IO;
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

      EFPCommandItem MenuFile = EFPApp.CommandItems.Add(EFPAppStdCommandItems.MenuFile, null);

      ci = EFPApp.CommandItems.Add(EFPAppStdCommandItems.Exit, MenuFile);
      ci.Click += new EventHandler(EFPApp.CommandItems.Exit_Click);
      ci.GroupBegin = true;

      #endregion

      #region ������

      EFPCommandItem MenuEdit = EFPApp.CommandItems.Add(EFPAppStdCommandItems.MenuEdit, null);

      ci = EFPApp.CommandItems.Add(EFPAppStdCommandItems.Cut, MenuEdit);
      ci.Enabled = false;
      ci.GroupBegin = true;
      //SpeedPanelStandard.Add(Cut);

      ci = EFPApp.CommandItems.Add(EFPAppStdCommandItems.Copy, MenuEdit);
      ci.Enabled = false;
      //SpeedPanelStandard.Add(Copy);

      ci = EFPApp.CommandItems.Add(EFPAppStdCommandItems.Paste, MenuEdit);
      ci.Enabled = false;
      //SpeedPanelStandard.Add(Paste);

      ci = EFPApp.CommandItems.Add(EFPAppStdCommandItems.PasteSpecial, MenuEdit);
      ci.Enabled = false;
      ci.GroupEnd = true;

      ci = EFPApp.CommandItems.Add(EFPAppStdCommandItems.Find, MenuEdit);
      ci.Enabled = false;
      ci.GroupBegin = true;
      //SpeedPanelStandard.Add(Find);

      ci = EFPApp.CommandItems.Add(EFPAppStdCommandItems.IncSearch, MenuEdit);
      ci.Enabled = false;
      //SpeedPanelStandard.Add(IncSearch);

      ci = EFPApp.CommandItems.Add(EFPAppStdCommandItems.FindNext, MenuEdit);
      ci.Enabled = false;
      //SpeedPanelStandard.Add(FindNext);

      //ci = EFPApp.CommandItems.Add(EFPAppStdCommandItems.Replace, MenuEdit);
      //ci.Enabled = false;
      //SpeedPanelStandard.Add(Replace);

      //ci = EFPApp.CommandItems.Add(EFPAppStdCommandItems.Goto, MenuEdit);
      //ci.Enabled = false;
      //ci.GroupEnd = true;

      ci = EFPApp.CommandItems.Add(EFPAppStdCommandItems.SelectAll, MenuEdit);
      ci.Enabled = false;
      ci.GroupBegin = true;
      ci.GroupEnd = true;

      #endregion

      #region ���

      EFPCommandItem MenuView = EFPApp.CommandItems.Add(EFPAppStdCommandItems.MenuView, null);

      EFPAppCommandItemHelpers Helpers = new EFPAppCommandItemHelpers();

      Helpers.AddViewMenuCommands(MenuView);


      #endregion

      #region �����������

      EFPCommandItem MenuRB = new EFPCommandItem(null, "�����������");
      MenuRB.MenuText = "��&���������";
      EFPApp.CommandItems.Add(MenuRB);

      ci = ProgramDBUI.TheUI.DocTypes["Plants"].CreateMainMenuItem(MenuRB);
      SpeedPanelStandard.Add(ci);

      ci = ProgramDBUI.TheUI.DocTypes["Care"].CreateMainMenuItem(MenuRB);
      SpeedPanelStandard.Add(ci);

      ci = ProgramDBUI.TheUI.DocTypes["Places"].CreateMainMenuItem(MenuRB);
      SpeedPanelStandard.Add(ci);

      ci = ProgramDBUI.TheUI.DocTypes["Contras"].CreateMainMenuItem(MenuRB);
      SpeedPanelStandard.Add(ci);

      ci = ProgramDBUI.TheUI.DocTypes["Companies"].CreateMainMenuItem(MenuRB);
      SpeedPanelStandard.Add(ci);

      ci = ProgramDBUI.TheUI.DocTypes["Diseases"].CreateMainMenuItem(MenuRB);
      SpeedPanelStandard.Add(ci);

      ci = ProgramDBUI.TheUI.DocTypes["Remedies"].CreateMainMenuItem(MenuRB);
      SpeedPanelStandard.Add(ci);

      ci = ProgramDBUI.TheUI.DocTypes["Soils"].CreateMainMenuItem(MenuRB);
      SpeedPanelStandard.Add(ci);

      ci = ProgramDBUI.TheUI.DocTypes["PotKinds"].CreateMainMenuItem(MenuRB);
      SpeedPanelStandard.Add(ci);

      ci = ProgramDBUI.TheUI.DocTypes["AttrTypes"].CreateMainMenuItem(MenuRB);
      SpeedPanelStandard.Add(ci);


      #endregion

      #region ������

      EFPCommandItem MenuReports = new EFPCommandItem(null, "Reports");
      MenuReports.MenuText = "������";
      EFPApp.CommandItems.Add(MenuReports);

      ci = new EFPCommandItem("Reports", "PlantSelReport");
      ci.Parent = MenuReports;
      ci.MenuText = "������� �� �������� ��������";
      ci.ImageKey = "PlantSelReport";
      ci.Click += new EventHandler(ciPlantSelReport_Click);
      ci.GroupBegin = true;
      EFPApp.CommandItems.Add(ci);
      SpeedPanelStandard.Add(ci);

      ci = new EFPCommandItem("Reports", "CareReport");
      ci.Parent = MenuReports;
      ci.MenuText = "������� ����� �� ����������";
      ci.ImageKey = "CareReport";
      ci.Click += new EventHandler(ciCareReport_Click);
      EFPApp.CommandItems.Add(ci);
      SpeedPanelStandard.Add(ci);

      ci = new EFPCommandItem("Reports", "FloweringReport");
      ci.Parent = MenuReports;
      ci.MenuText = "�������� ��������";
      ci.ImageKey = "FloweringReport";
      ci.Click += new EventHandler(ciFloweringReport_Click);
      EFPApp.CommandItems.Add(ci);
      SpeedPanelStandard.Add(ci);

      ci = new EFPCommandItem("Reports", "ReplantingReport");
      ci.Parent = MenuReports;
      ci.MenuText = "���������";
      ci.ImageKey = "ReplantingReport";
      ci.Click += new EventHandler(ciReplantingReport_Click);
      EFPApp.CommandItems.Add(ci);
      SpeedPanelStandard.Add(ci);

      ci = new EFPCommandItem("Reports", "PlanReport");
      ci.Parent = MenuReports;
      ci.MenuText = "��������������� ��������";
      ci.ImageKey = "PlanReport";
      ci.Click += new EventHandler(ciPlanReport_Click);
      ci.GroupEnd = true;
      EFPApp.CommandItems.Add(ci);
      SpeedPanelStandard.Add(ci);

      #endregion

      #region ������

      EFPCommandItem MenuService = new EFPCommandItem(null, "������");
      MenuService.MenuText = "�&�����";
      EFPApp.CommandItems.Add(MenuService);

      ci = new EFPCommandItem("������", "���������");
      ci.Parent = MenuService;
      ci.MenuText = "���������";
      ci.ImageKey = "Settings";
      ci.Click += new EventHandler(ciSetting_Click);
      EFPApp.CommandItems.Add(ci);
      SpeedPanelStandard.Add(ci);

      ci = new EFPCommandItem("������", "����������������������������");
      ci.Parent = MenuService;
      ci.MenuText = "�������� �������� ������������";
      ci.ImageKey = "UserActions";
      ci.Click += new EventHandler(ciUserActions_Click);
      EFPApp.CommandItems.Add(ci);
      //SpeedPanelStandard.Add(ci);

      #endregion

      #region �������

      EFPCommandItem MenuDebug = new EFPCommandItem(null, "�������");
      MenuDebug.MenuText = "�������";
      MenuDebug.ImageKey = "Debug";
      MenuDebug.Parent = MenuService;
      MenuDebug.GroupBegin = true;
      MenuDebug.GroupEnd = true;
      EFPApp.CommandItems.Add(MenuDebug);

      ci = new EFPCommandItem("�������", "������������������������");
      ci.Parent = MenuDebug; 
      ci.MenuText = "���������� ��������������";
      ci.ImageKey = "������������������������";
      ci.Click += new EventHandler(ShowIds_Click);
      EFPApp.CommandItems.Add(ci);

      #endregion

      #region ����

      EFPCommandItem MenuWindow = EFPApp.CommandItems.Add(EFPAppStdCommandItems.MenuWindow, null);
      Helpers.AddWindowMenuCommands(MenuWindow);

      #endregion

      #region �������

      EFPCommandItem MenuHelp = EFPApp.CommandItems.Add(EFPAppStdCommandItems.MenuHelp, null);

      ci = EFPApp.CommandItems.CreateContext(EFPAppStdCommandItems.About);
      ci.Parent = MenuHelp;
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

    static void RestoreBars_Click(object Sender, EventArgs Args)
    {
      foreach (EFPAppToolBar ToolBar in EFPApp.AppToolBars)
      {
        ToolBar.Reset();
        if (ToolBar.Name == "������")
          ToolBar.Visible = false;
      }
      EFPApp.AppToolBars.Attach();

      // ClientStatusBar.StatusBar.Visible = true;
    }

    #endregion

    #region ���� "������"

    static void ciPlantSelReport_Click(object Sender, EventArgs Args)
    {
      new PlantSelReport().Run();
    }

    static void ciFloweringReport_Click(object Sender, EventArgs Args)
    {
      new FloweringReport().Run();
    }

    static void ciReplantingReport_Click(object Sender, EventArgs Args)
    {
      new ReplantingReport().Run();
    }

    static void ciCareReport_Click(object Sender, EventArgs Args)
    {
      new CareReport().Run();
    }

    static void ciPlanReport_Click(object Sender, EventArgs Args)
    {
      new PlanReport().Run();
    }

    #endregion

    #region ���� "������"

    static void ciSetting_Click(object Sender, EventArgs Args)
    {
      UserSettingsForm Form = new UserSettingsForm();
      Form.ValueToForm(ProgramDBUI.Settings);
      if (EFPApp.ShowDialog(Form, true) != DialogResult.OK)
        return;
      Form.ValueFromForm(ProgramDBUI.Settings);
      ProgramDBUI.Settings.WriteConfig();
    }

    static private void ShowIds_Cllick(object Sender, EventArgs Args)
    {
      EFPCommandItem ci = (EFPCommandItem)Sender;
      ProgramDBUI.TheUI.DebugShowIds = !ProgramDBUI.TheUI.DebugShowIds;
      ci.Checked = ProgramDBUI.TheUI.DebugShowIds;
      // ����� �������� ������ ���������� �����
      EFPFormProvider.DebugFormProvider = ProgramDBUI.TheUI.DebugShowIds;
      EFPPasteHandler.PasteSpecialDebugMode = ProgramDBUI.TheUI.DebugShowIds;

//#if DEBUG
//      ci.StatusBarText = Accoo2DBUI.TheUI.DebugShowIds ? "���" : "����";
//#endif
    }

    //static void ciClearBuf_Click(object Sender, EventArgs Args)
    //{
    //  Accoo2DBUI.TheUI.DocProvider.ClearCache();
    //  Cache.Clear();
    //  Accoo2DBUI.Cache.Clear("��������� ������� \"����� ����������� ������\"");
    //  Accoo2DBUI.ConfigSections.ClearBuf();
    //  Accoo2DBUI.Docs.Clear(); // 26.11.2015
    //}

    static void ciUserActions_Click(object sender, EventArgs e)
    {
      ProgramDBUI.TheUI.ShowUserActions();
    }

    #endregion

    #region ���� "�������"

    static private void ShowIds_Click(object Sender, EventArgs Args)
    {
      EFPCommandItem ci = (EFPCommandItem)Sender;
      ProgramDBUI.TheUI.DebugShowIds = !ProgramDBUI.TheUI.DebugShowIds;
      ci.Checked = ProgramDBUI.TheUI.DebugShowIds;
      // ����� �������� ������ ���������� �����
      EFPFormProvider.DebugFormProvider = ProgramDBUI.TheUI.DebugShowIds;
      EFPPasteHandler.PasteSpecialDebugMode = ProgramDBUI.TheUI.DebugShowIds;
    }

    #endregion

    #region ���� "����"

    public static void AboutClick(object Sender, EventArgs Args)
    {
      AboutDialog dlg = new AboutDialog(Assembly.GetExecutingAssembly());
      dlg.ShowDialog();
    }

    #endregion

    #endregion
  }
}
