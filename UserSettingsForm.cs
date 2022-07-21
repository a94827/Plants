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
  /// ������ "���������"
  /// </summary>
  internal partial class UserSettingsForm : Form
  {
    #region ����������� �����

    public UserSettingsForm()
    {
      InitializeComponent();
      Icon = EFPApp.MainImageIcon("Settings");

      EFPFormProvider efpForm = new EFPFormProvider(this);

      #region �������

      efpNumberDigits = new EFPIntEditBox(efpForm, edNumberDigits);
      efpNumberDigits.Minimum = 1;
      efpNumberDigits.Maximum = UserSettings.MaxNumberDigits;

      #endregion

      #region ����

      efpPhotoDir = new EFPTextBox(efpForm, edPhotoDir);
      efpPhotoDir.CanBeEmpty = true;

      EFPFolderBrowserButton efpBrowse = new EFPFolderBrowserButton(efpPhotoDir, btnBrowsePhotoDir);
      efpBrowse.Description = "�������, ��� ����������� ����. � ���� ������ �������� ������ ���������." +
        "���� ������� �� �����, ������ ��������� � ������� ������";
      efpBrowse.ShowNewFolderButton = false;

      efpThumbnailSize = new EFPRadioButtons(efpForm, rb16x16);

      #endregion

      #region ��������� �����������

      efpBackupMode = new EFPListComboBox(efpForm, cbBackupMode);

      efpBackupDir = new EFPTextBox(efpForm, edBackupDir);
      efpBackupDir.CanBeEmpty = true;
      efpBackupDir.ToolTipText = "����� ��� ���������� ��������� �����." + Environment.NewLine + "���� �� ������, ����� ����������� � ����� ��������� (" + FileTools.ApplicationBaseDir.Path + ")" + Environment.NewLine +
        "����� ������ ���������� ���� � ������� ������ \"�����\" ��� ������������� ���� �� ����� ���������";
      efpBackupDir.EnabledEx = new DepNot(new DepEqual<int>(efpBackupMode.SelectedIndexEx, (int)UserSettings.BackupModes.None));

      EFPFolderBrowserButton efpBrowseBackupDir = new EFPFolderBrowserButton(efpBackupDir, btnBrowseBackupDir);
      efpBrowseBackupDir.ShowNewFolderButton = true;
      efpBrowseBackupDir.Description = "����� ��� ���������� ��������� �����";

      EFPWindowsExplorerButton efpExploreBackupDir = new EFPWindowsExplorerButton(efpBackupDir, btnExploreBackupDir);

      #endregion
    }

    #endregion

    #region ����

    #region �������

    EFPIntEditBox efpNumberDigits;

    #endregion

    #region ����

    EFPTextBox efpPhotoDir;
    EFPRadioButtons efpThumbnailSize;

    #endregion

    #region ��������� �����������

    EFPListComboBox efpBackupMode;
    EFPTextBox efpBackupDir;

    #endregion

    #endregion

    #region ������ � ������ �����

    public void ValueToForm(UserSettings settings)
    {
      efpNumberDigits.Value = settings.NumberDigits;

      efpPhotoDir.Text = settings.PhotoDir.SlashedPath;
      efpThumbnailSize.SelectedIndex = (int)(settings.ThumbnailSizeCode);

      efpBackupMode.SelectedIndex = (int)(settings.BackupMode);
      efpBackupDir.Text = settings.BackupDir;
    }

    public void ValueFromForm(UserSettings settings)
    {
      settings.NumberDigits = efpNumberDigits.Value;

      settings.PhotoDir = new AbsPath(efpPhotoDir.Text);
      settings.ThumbnailSizeCode = (ThumbnailSizeCode)(efpThumbnailSize.SelectedIndex);

      settings.BackupMode = (UserSettings.BackupModes)(efpBackupMode.SelectedIndex);
      settings.BackupDir = efpBackupDir.Text;
    }

    #endregion

    #region ��������� �������

    /// <summary>
    /// ���������� ��������� ������� ����� �������� � �������� ������ ������
    /// </summary>
    private static int _LastSelectedPageIndex = 0;

    protected override void OnLoad(EventArgs args)
    {
      base.OnLoad(args);
      TheTabControl.SelectedIndex = _LastSelectedPageIndex;
    }

    protected override void OnClosing(CancelEventArgs args)
    {
      _LastSelectedPageIndex = TheTabControl.SelectedIndex;
      base.OnClosing(args);
    }

    #endregion
  }

  public enum ThumbnailSizeCode { Small, Normal, Large, Max }

  /// <summary>
  /// ��������� ������������
  /// </summary>
  public class UserSettings
  {
    #region �����������

    public UserSettings()
    {
      _NumberDigits = 3;
      _NumberMask = "000";

      ThumbnailSizeCode = ThumbnailSizeCode.Normal;

      BackupMode = BackupModes.AfterEveryRun;
      BackupDir = "." + System.IO.Path.DirectorySeparatorChar + "Backup" + System.IO.Path.DirectorySeparatorChar;
    }

    #endregion

    #region ����

    #region ����� �� ��������

    /// <summary>
    /// ���������� ������ � ������ �� ��������.
    /// �� ��������� - 3
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
    /// ������������ �������� ��� NumberDigits
    /// </summary>
    public const int MaxNumberDigits = 5;

    /// <summary>
    /// ����� ��� �������������� ������ �� ��������. �� ��������� - "000"
    /// </summary>
    public string NumberMask { get { return _NumberMask; } }
    private string _NumberMask;

    #endregion

    #region ����

    /// <summary>
    /// ������� � ������������.
    /// ���� �� ������, �� ������ ���������/������� ����
    /// </summary>
    public AbsPath PhotoDir;

    /// <summary>
    /// ��������� ������ �������� (������������)
    /// </summary>
    public ThumbnailSizeCode ThumbnailSizeCode;

    /// <summary>
    /// ��������� ������ �������� (� ��������)
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

    #region ��������� �����������

    public enum BackupModes { None, AfterEveryRun, EveryDay }

    public BackupModes BackupMode;

    /// <summary>
    /// ������� ��� ���������� �����������.
    /// �� ���������� AbsPath, �.�. ����� ���� ������������� ����
    /// </summary>
    public string BackupDir;

    #endregion

    #endregion

    #region ������ / ������

    /// <summary>
    /// ���� � ����� "LocalConfig.xml"
    /// </summary>
    public static AbsPath LocalConfigFilePath
    {
      get { return new AbsPath(ProgramDB.DBDir.ParentDir, "LocalConfig.xml"); }
    }

    public void WriteConfig()
    {
      XmlCfgFile cfg = new XmlCfgFile(LocalConfigFilePath);

      cfg.SetInt("NumberDigits", NumberDigits);

      cfg.SetString("PhotoDir", PhotoDir.Path);
      cfg.SetEnum<ThumbnailSizeCode>("ThumbnailSize", ThumbnailSizeCode);

      cfg.SetEnum<BackupModes>("BackupMode", BackupMode);
      cfg.SetString("BackupDir", BackupDir);

      cfg.Save();
    }

    public void ReadConfig()
    {

      XmlCfgFile cfg = new XmlCfgFile(LocalConfigFilePath);

      int x = cfg.GetInt("NumberDigits");
      if (x >= 1 && x <= MaxNumberDigits)
        NumberDigits = x;


      PhotoDir = new AbsPath(cfg.GetString("PhotoDir"));
      cfg.GetEnum<ThumbnailSizeCode>("ThumbnailSize", ref ThumbnailSizeCode);

      cfg.GetEnum<BackupModes>("BackupMode", ref BackupMode);
      BackupDir = cfg.GetString("BackupDir");
    }

    #endregion
  }

}