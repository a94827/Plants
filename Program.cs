using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FreeLibSet.Logging;
using FreeLibSet.IO;
using FreeLibSet.Forms;
using FreeLibSet.Forms.Docs;
using FreeLibSet.Core;
using FreeLibSet.Calendar;

namespace Plants
{
  static class Program
  {
    #region Main()

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      EFPApp.InitApp();
      try
      {
        using (ProgramDB DB = new ProgramDB())
        {
          using (Splash spl = new Splash(new string[] { 
          "Инициализация базы данных", 
          "Инициализация главного окна",
          "Проверка запланированных действий"}))
          {
            #region Инициализация каталогов

            LogoutTools.LogBaseDirectory = FileTools.ApplicationBaseDir + "Log";
            if (!CreateDir(LogoutTools.LogBaseDirectory))
              return;

            TempDirectory.RootDir = FileTools.ApplicationBaseDir + "Temp";
            if (!CreateDir(TempDirectory.RootDir))
              return;

            AbsPath DBDir = FileTools.ApplicationBaseDir + "DB";
            if (!CreateDir(DBDir))
              return;

            #endregion

            DB.InitDB(DBDir, spl);

            ProgramDBUI.Settings = new UserSettings();
            ProgramDBUI.Settings.ReadConfig();

            spl.Complete();

            DateRangeFormatter.Default = new MyDateRangeFormatter();

            #region Главное окно

            // Картинки
            DBUI.InitImages();
            DummyForm frm = new DummyForm();
            EFPApp.AddMainImages(frm.MainImageList);
            EFPFormProvider.UseErrorProvider = false;

            ProgramDBUI.TheUI = new ProgramDBUI(DB.CreateDocProvider().CreateProxy());
            //ProgramDBUI.TheUI.DebugShowIds = true; // показывать идентификаторы для отладки
            ProgramDBUI.ConfigSections = new ClientConfigSections(DB);
            EFPApp.ConfigManager = ProgramDBUI.ConfigSections; // должно быть до показа форм
            //ProgramDBUI.Docs = new CoProDevDocuments(ProgramDBUI.TheUI.TextHandlers);
            //ProgramDBUI.Cache = new ProgramCache();
            MainMenu.Init();
            EFPApp.Interface = new EFPAppInterfaceSDI();
            EFPApp.MainWindowTitle = "Каталог растений";
            EFPApp.LoadMainWindowLayout();
            EFPApp.FormCreators.Add(ProgramDBUI.TheUI);
            EFPApp.LoadComposition();
            if (EFPApp.Interface.ChildFormCount == 0)
              EFPApp.Interface.ShowChildForm(new DocTableViewForm(ProgramDBUI.TheUI.DocTypes["Plants"], DocTableViewMode.Browse));
            EFPApp.BeforeClosing += new System.ComponentModel.CancelEventHandler(EFPApp_BeforeClosing);

            #endregion

            spl.Complete();

            PlanReport.ShowOnStart();
            spl.Complete();
          }

          Application.Run();

          if (ProgramDBUI.Settings.BackupMode != UserSettings.BackupModes.None)
          {
            using (Splash spl = new Splash("Создание резервной копии"))
            {
              switch (ProgramDBUI.Settings.BackupMode)
              {
                case UserSettings.BackupModes.AfterEveryRun:
                  DB.CreateBackup(spl);
                  break;
                case UserSettings.BackupModes.EveryDay:
                  if (!HasDailyBackup())
                    DB.CreateBackup(spl);
                  break;
              }
              DB.RemoveOldBackups(spl);
            }
          }

          ProgramDBUI.ConfigSections = null;
        }
      }
      catch (Exception e)
      {
        EFPApp.ShowException(e, "Ошибка запуска программы");
      }
    }

    static void EFPApp_BeforeClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      EFPApp.SaveComposition();
      EFPApp.SaveMainWindowLayout();
    }

    private static bool CreateDir(AbsPath Dir)
    {
      try
      {
        FileTools.ForceDirs(Dir);
        return true;
      }
      catch (System.IO.IOException e)
      {
        EFPApp.ErrorMessageBox("Невозможно создать каталог \"" + Dir.Path + "\". " + e.Message);
        return false;
      }
    }

    /// <summary>
    /// Возвращает true, если сегодня уже была создана резервная копия
    /// </summary>
    /// <returns></returns>
    private static bool HasDailyBackup()
    {
      string s1 = DateTime.Today.ToString(@"yyyyMMdd", StdConvert.DateTimeFormat);
      string[] aFiles = System.IO.Directory.GetFiles(ProgramDB.BackupDir.Path, s1 + "-??????.7z", System.IO.SearchOption.TopDirectoryOnly);
      return aFiles.Length > 0;
    }

    #endregion

    private class MyDateRangeFormatter : FreeLibSet.Russian.RusDateRangeFormatter
    {
      public override string ToString(DateTime? FirstDate, DateTime? LastDate, bool Long)
      {
        if (FirstDate.HasValue && LastDate.HasValue)
        {
          if (LastDate.Value.Year < 1900)
            return "?";
        }
        return base.ToString(FirstDate, LastDate, Long);
      }
    }
  }
}