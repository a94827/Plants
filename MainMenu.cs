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
  /// Главное меню программы
  /// </summary>
  internal static class MainMenu
  {
    #region Инициализация

    public static void Init()
    {
      EFPAppToolBarCommandItems SpeedPanelStandard = EFPApp.ToolBars.Add("Standard", "Стандартная");

      EFPCommandItem ci;

      #region Файл

      EFPCommandItem MenuFile = EFPApp.CommandItems.Add(EFPAppStdCommandItems.MenuFile, null);

      ci = EFPApp.CommandItems.Add(EFPAppStdCommandItems.Exit, MenuFile);
      ci.Click += new EventHandler(EFPApp.CommandItems.Exit_Click);
      ci.GroupBegin = true;

      #endregion

      #region Правка

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

      #region Вид

      EFPCommandItem MenuView = EFPApp.CommandItems.Add(EFPAppStdCommandItems.MenuView, null);

      EFPAppCommandItemHelpers Helpers = new EFPAppCommandItemHelpers();

      Helpers.AddViewMenuCommands(MenuView);


      #endregion

      #region Справочники

      EFPCommandItem MenuRB = new EFPCommandItem(null, "Справочники");
      MenuRB.MenuText = "Сп&равочники";
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

      #region Отчеты

      EFPCommandItem MenuReports = new EFPCommandItem(null, "Reports");
      MenuReports.MenuText = "Отчеты";
      EFPApp.CommandItems.Add(MenuReports);

      ci = new EFPCommandItem("Reports", "PlantSelReport");
      ci.Parent = MenuReports;
      ci.MenuText = "Выборка из каталога растений";
      ci.ImageKey = "PlantSelReport";
      ci.Click += new EventHandler(ciPlantSelReport_Click);
      ci.GroupBegin = true;
      EFPApp.CommandItems.Add(ci);
      SpeedPanelStandard.Add(ci);

      ci = new EFPCommandItem("Reports", "CareReport");
      ci.Parent = MenuReports;
      ci.MenuText = "Правила ухода за растениями";
      ci.ImageKey = "CareReport";
      ci.Click += new EventHandler(ciCareReport_Click);
      EFPApp.CommandItems.Add(ci);
      SpeedPanelStandard.Add(ci);

      ci = new EFPCommandItem("Reports", "FloweringReport");
      ci.Parent = MenuReports;
      ci.MenuText = "Цветение растений";
      ci.ImageKey = "FloweringReport";
      ci.Click += new EventHandler(ciFloweringReport_Click);
      EFPApp.CommandItems.Add(ci);
      SpeedPanelStandard.Add(ci);

      ci = new EFPCommandItem("Reports", "ReplantingReport");
      ci.Parent = MenuReports;
      ci.MenuText = "Пересадка";
      ci.ImageKey = "ReplantingReport";
      ci.Click += new EventHandler(ciReplantingReport_Click);
      EFPApp.CommandItems.Add(ci);
      SpeedPanelStandard.Add(ci);

      ci = new EFPCommandItem("Reports", "PlanReport");
      ci.Parent = MenuReports;
      ci.MenuText = "Запланированные действия";
      ci.ImageKey = "PlanReport";
      ci.Click += new EventHandler(ciPlanReport_Click);
      ci.GroupEnd = true;
      EFPApp.CommandItems.Add(ci);
      SpeedPanelStandard.Add(ci);

      #endregion

      #region Сервис

      EFPCommandItem MenuService = new EFPCommandItem(null, "Сервис");
      MenuService.MenuText = "С&ервис";
      EFPApp.CommandItems.Add(MenuService);

      ci = new EFPCommandItem("Сервис", "Настройки");
      ci.Parent = MenuService;
      ci.MenuText = "Настройки";
      ci.ImageKey = "Settings";
      ci.Click += new EventHandler(ciSetting_Click);
      EFPApp.CommandItems.Add(ci);
      SpeedPanelStandard.Add(ci);

      ci = new EFPCommandItem("Сервис", "ПросмотрДействийПользователя");
      ci.Parent = MenuService;
      ci.MenuText = "Просмотр действий пользователя";
      ci.ImageKey = "UserActions";
      ci.Click += new EventHandler(ciUserActions_Click);
      EFPApp.CommandItems.Add(ci);
      //SpeedPanelStandard.Add(ci);

      #endregion

      #region Отладка

      EFPCommandItem MenuDebug = new EFPCommandItem(null, "Отладка");
      MenuDebug.MenuText = "Отладка";
      MenuDebug.ImageKey = "Debug";
      MenuDebug.Parent = MenuService;
      MenuDebug.GroupBegin = true;
      MenuDebug.GroupEnd = true;
      EFPApp.CommandItems.Add(MenuDebug);

      ci = new EFPCommandItem("Отладка", "ПоказыватьИдентификаторы");
      ci.Parent = MenuDebug; 
      ci.MenuText = "Показывать идентификаторы";
      ci.ImageKey = "ПоказыватьИдентификаторы";
      ci.Click += new EventHandler(ShowIds_Click);
      EFPApp.CommandItems.Add(ci);

      #endregion

      #region Окно

      EFPCommandItem MenuWindow = EFPApp.CommandItems.Add(EFPAppStdCommandItems.MenuWindow, null);
      Helpers.AddWindowMenuCommands(MenuWindow);

      #endregion

      #region Справка

      EFPCommandItem MenuHelp = EFPApp.CommandItems.Add(EFPAppStdCommandItems.MenuHelp, null);

      ci = EFPApp.CommandItems.CreateContext(EFPAppStdCommandItems.About);
      ci.Parent = MenuHelp;
      ci.Click += new EventHandler(AboutClick);
      EFPApp.CommandItems.Add(ci);

      #endregion


      // Делаем видимыми или невидимыми нужные команды
      EFPApp.CommandItems.InitMenuVisible();

    }

    #endregion

    #region Методы выполнения команд

    #region Меню "Файл"


    #endregion

    #region Вид

    static void RestoreBars_Click(object Sender, EventArgs Args)
    {
      foreach (EFPAppToolBar ToolBar in EFPApp.AppToolBars)
      {
        ToolBar.Reset();
        if (ToolBar.Name == "Задачи")
          ToolBar.Visible = false;
      }
      EFPApp.AppToolBars.Attach();

      // ClientStatusBar.StatusBar.Visible = true;
    }

    #endregion

    #region Меню "Отчеты"

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

    #region Меню "Сервис"

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
      // Также включаем другие отладочные флаги
      EFPFormProvider.DebugFormProvider = ProgramDBUI.TheUI.DebugShowIds;
      EFPPasteHandler.PasteSpecialDebugMode = ProgramDBUI.TheUI.DebugShowIds;

//#if DEBUG
//      ci.StatusBarText = Accoo2DBUI.TheUI.DebugShowIds ? "Вкл" : "Выкл";
//#endif
    }

    //static void ciClearBuf_Click(object Sender, EventArgs Args)
    //{
    //  Accoo2DBUI.TheUI.DocProvider.ClearCache();
    //  Cache.Clear();
    //  Accoo2DBUI.Cache.Clear("Выполнена команда \"Сброс буферизации данных\"");
    //  Accoo2DBUI.ConfigSections.ClearBuf();
    //  Accoo2DBUI.Docs.Clear(); // 26.11.2015
    //}

    static void ciUserActions_Click(object sender, EventArgs e)
    {
      ProgramDBUI.TheUI.ShowUserActions();
    }

    #endregion

    #region Меню "Отладка"

    static private void ShowIds_Click(object Sender, EventArgs Args)
    {
      EFPCommandItem ci = (EFPCommandItem)Sender;
      ProgramDBUI.TheUI.DebugShowIds = !ProgramDBUI.TheUI.DebugShowIds;
      ci.Checked = ProgramDBUI.TheUI.DebugShowIds;
      // Также включаем другие отладочные флаги
      EFPFormProvider.DebugFormProvider = ProgramDBUI.TheUI.DebugShowIds;
      EFPPasteHandler.PasteSpecialDebugMode = ProgramDBUI.TheUI.DebugShowIds;
    }

    #endregion

    #region Меню "Инфо"

    public static void AboutClick(object Sender, EventArgs Args)
    {
      AboutDialog dlg = new AboutDialog(Assembly.GetExecutingAssembly());
      dlg.ShowDialog();
    }

    #endregion

    #endregion
  }
}
