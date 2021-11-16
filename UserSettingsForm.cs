using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FreeLibSet.Forms;
using FreeLibSet.IO;
using FreeLibSet.Config;
using FreeLibSet.DependedValues;

namespace Plants
{
  /// <summary>
  /// Диалог "Настройки"
  /// </summary>
  internal partial class UserSettingsForm : Form
  {
    #region Конструктор формы

    public UserSettingsForm()
    {
      InitializeComponent();
      Icon = EFPApp.MainImageIcon("Settings");

      EFPFormProvider efpForm = new EFPFormProvider(this);

      #region Каталог

      efpNumberDigits = new EFPIntEditBox(efpForm, edNumberDigits);
      efpNumberDigits.Minimum = 1;
      efpNumberDigits.Maximum = UserSettings.MaxNumberDigits;

      #endregion

      #region Фото

      efpPhotoDir = new EFPTextBox(efpForm, edPhotoDir);
      efpPhotoDir.CanBeEmpty = true;

      EFPFolderBrowserButton efpBrowse = new EFPFolderBrowserButton(efpPhotoDir, btnBrowsePhotoDir);
      efpBrowse.Description = "Каталог, где расположены фото. В базе данных хранятся только миниатюры." +
        "Если каталог не задан, нельзя добавлять и удалять снимки";
      efpBrowse.ShowNewFolderButton = false;

      efpThumbnailSize = new EFPRadioButtons(efpForm, rb16x16);

      #endregion

      #region Резервное копирование

      efpBackupMode = new EFPListComboBox(efpForm, cbBackupMode);

      efpBackupDir = new EFPTextBox(efpForm, edBackupDir);
      efpBackupDir.CanBeEmpty = true;
      efpBackupDir.ToolTipText = "Папка для размещения резервных копий." + Environment.NewLine + "Если не задана, копии сохраняются в папке программы (" + FileTools.ApplicationBaseDir.Path + ")" + Environment.NewLine +
        "Можно задать абсолютный путь с помощью кнопки \"Обзор\" или относительный путь от папки программы";
      efpBackupDir.EnabledEx = new DepNot(new DepEqual<int>(efpBackupMode.SelectedIndexEx, (int)UserSettings.BackupModes.None));

      EFPFolderBrowserButton efpBrowseBackupDir = new EFPFolderBrowserButton(efpBackupDir, btnBrowseBackupDir);
      efpBrowseBackupDir.ShowNewFolderButton = true;
      efpBrowseBackupDir.Description = "Папка для размещения резервных копий";

      EFPWindowsExplorerButton efpExploreBackupDir = new EFPWindowsExplorerButton(efpBackupDir, btnExploreBackupDir);

      #endregion
    }

    #endregion

    #region Поля

    #region Каталог

    EFPIntEditBox efpNumberDigits;

    #endregion

    #region Фото

    EFPTextBox efpPhotoDir;
    EFPRadioButtons efpThumbnailSize;

    #endregion

    #region Резервное копирование

    EFPListComboBox efpBackupMode;
    EFPTextBox efpBackupDir;

    #endregion

    #endregion

    #region Чтение и запись полей

    public void ValueToForm(UserSettings Settings)
    {
      efpNumberDigits.Value = Settings.NumberDigits;

      efpPhotoDir.Text = Settings.PhotoDir.SlashedPath;
      efpThumbnailSize.SelectedIndex = (int)(Settings.ThumbnailSizeCode);

      efpBackupMode.SelectedIndex = (int)(Settings.BackupMode);
      efpBackupDir.Text = Settings.BackupDir;
    }

    public void ValueFromForm(UserSettings Settings)
    {
      Settings.NumberDigits = efpNumberDigits.Value;

      Settings.PhotoDir = new AbsPath(efpPhotoDir.Text);
      Settings.ThumbnailSizeCode = (ThumbnailSizeCode)(efpThumbnailSize.SelectedIndex);

      Settings.BackupMode = (UserSettings.BackupModes)(efpBackupMode.SelectedIndex);
      Settings.BackupDir = efpBackupDir.Text;
    }

    #endregion

    #region Выбранная вкладка

    /// <summary>
    /// Запоминаем выбранную вкладку между вызовами в пределах сеанса работы
    /// </summary>
    private static int _LastSelectedPageIndex = 0;

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      TheTabControl.SelectedIndex = _LastSelectedPageIndex;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      _LastSelectedPageIndex = TheTabControl.SelectedIndex;
      base.OnClosing(e);
    }

    #endregion
  }

  public enum ThumbnailSizeCode { Small, Normal, Large, Max }

  /// <summary>
  /// Настройки пользователя
  /// </summary>
  public class UserSettings
  {
    #region Конструктор

    public UserSettings()
    {
      _NumberDigits = 3;
      _NumberMask = "000";

      ThumbnailSizeCode = ThumbnailSizeCode.Normal;

      BackupMode = BackupModes.AfterEveryRun;
      BackupDir = "." + System.IO.Path.DirectorySeparatorChar + "Backup" + System.IO.Path.DirectorySeparatorChar;
    }

    #endregion

    #region Поля

    #region Номер по каталогу

    /// <summary>
    /// Количество знаков в номере по каталогу.
    /// По умолчанию - 3
    /// </summary>
    public int NumberDigits
    {
      get { return _NumberDigits; }
      set
      {
        if (value < 1 || value > MaxNumberDigits)
          throw new ArgumentOutOfRangeException();
        _NumberDigits = value;
        _NumberMask = new string('0', value);
      }
    }
    private int _NumberDigits;

    /// <summary>
    /// Мкасимальное значение для NumberDigits
    /// </summary>
    public const int MaxNumberDigits = 5;

    /// <summary>
    /// Маска для форматирования номера по каталогу. По умолчанию - "000"
    /// </summary>
    public string NumberMask { get { return _NumberMask; } }
    private string _NumberMask;

    #endregion

    #region Фото

    /// <summary>
    /// Каталог с фотографиями.
    /// Если не задано, то нельзя добавлять/удалять фото
    /// </summary>
    public AbsPath PhotoDir;

    /// <summary>
    /// Выбранный размер миниатюр (перечисление)
    /// </summary>
    public ThumbnailSizeCode ThumbnailSizeCode;

    /// <summary>
    /// Выбранный размер миниатюр (в пикселях)
    /// </summary>
    public Size ThumbnailSize
    {
      get
      {
        switch (ThumbnailSizeCode)
        {
          case ThumbnailSizeCode.Small: return new Size(16, 16);
          case ThumbnailSizeCode.Normal: return new Size(32, 32);
          case ThumbnailSizeCode.Large: return new Size(64, 64);
          case ThumbnailSizeCode.Max: return new Size(128, 128);
          default: return new Size(128, 128);
        }
      }
    }

    #endregion

    #region Резервное копирование

    public enum BackupModes { None, AfterEveryRun, EveryDay }

    public BackupModes BackupMode;

    /// <summary>
    /// Каталог для резервного копирования.
    /// Не используем AbsPath, т.к. может быть относительный путь
    /// </summary>
    public string BackupDir;

    #endregion

    #endregion

    #region Чтение / запись

    /// <summary>
    /// Путь к файлу "LocalConfig.xml"
    /// </summary>
    public static AbsPath LocalConfigFilePath
    {
      get { return new AbsPath(ProgramDB.DBDir.ParentDir, "LocalConfig.xml"); }
    }

    public void WriteConfig()
    {
      XmlCfgFile File = new XmlCfgFile(LocalConfigFilePath);

      File.SetInt("NumberDigits", NumberDigits);

      File.SetString("PhotoDir", PhotoDir.Path);
      File.SetEnum<ThumbnailSizeCode>("ThumbnailSize", ThumbnailSizeCode);

      File.SetEnum<BackupModes>("BackupMode", BackupMode);
      File.SetString("BackupDir", BackupDir);

      File.Save();
    }

    public void ReadConfig()
    {

      XmlCfgFile File = new XmlCfgFile(LocalConfigFilePath);

      int x = File.GetInt("NumberDigits");
      if (x >= 1 && x <= MaxNumberDigits)
        NumberDigits = x;


      PhotoDir = new AbsPath(File.GetString("PhotoDir"));
      File.GetEnum<ThumbnailSizeCode>("ThumbnailSize", ref ThumbnailSizeCode);

      File.GetEnum<BackupModes>("BackupMode", ref BackupMode);
      BackupDir = File.GetString("BackupDir");
    }

    #endregion
  }

}