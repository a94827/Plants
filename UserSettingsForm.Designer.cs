namespace Plants
{
  partial class UserSettingsForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.panel1 = new System.Windows.Forms.Panel();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnOk = new System.Windows.Forms.Button();
      this.TheTabControl = new System.Windows.Forms.TabControl();
      this.tpPlants = new System.Windows.Forms.TabPage();
      this.edNumberDigits = new FreeLibSet.Controls.IntEditBox();
      this.label4 = new System.Windows.Forms.Label();
      this.tpPhoto = new System.Windows.Forms.TabPage();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.rb128x128 = new System.Windows.Forms.RadioButton();
      this.rb64x64 = new System.Windows.Forms.RadioButton();
      this.rb32x32 = new System.Windows.Forms.RadioButton();
      this.rb16x16 = new System.Windows.Forms.RadioButton();
      this.btnBrowsePhotoDir = new System.Windows.Forms.Button();
      this.edPhotoDir = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.tpBackup = new System.Windows.Forms.TabPage();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.btnExploreBackupDir = new System.Windows.Forms.Button();
      this.btnBrowseBackupDir = new System.Windows.Forms.Button();
      this.edBackupDir = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.cbBackupMode = new System.Windows.Forms.ComboBox();
      this.label3 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.panel1.SuspendLayout();
      this.TheTabControl.SuspendLayout();
      this.tpPlants.SuspendLayout();
      this.tpPhoto.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.tpBackup.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btnCancel);
      this.panel1.Controls.Add(this.btnOk);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 222);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(624, 40);
      this.panel1.TabIndex = 1;
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(102, 8);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(88, 24);
      this.btnCancel.TabIndex = 1;
      this.btnCancel.Text = "Отмена";
      this.btnCancel.UseVisualStyleBackColor = true;
      // 
      // btnOk
      // 
      this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnOk.Location = new System.Drawing.Point(8, 8);
      this.btnOk.Name = "btnOk";
      this.btnOk.Size = new System.Drawing.Size(88, 24);
      this.btnOk.TabIndex = 0;
      this.btnOk.Text = "О&К";
      this.btnOk.UseVisualStyleBackColor = true;
      // 
      // TheTabControl
      // 
      this.TheTabControl.Controls.Add(this.tpPlants);
      this.TheTabControl.Controls.Add(this.tpPhoto);
      this.TheTabControl.Controls.Add(this.tpBackup);
      this.TheTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.TheTabControl.Location = new System.Drawing.Point(0, 0);
      this.TheTabControl.Name = "TheTabControl";
      this.TheTabControl.SelectedIndex = 0;
      this.TheTabControl.Size = new System.Drawing.Size(624, 222);
      this.TheTabControl.TabIndex = 0;
      // 
      // tpPlants
      // 
      this.tpPlants.Controls.Add(this.label5);
      this.tpPlants.Controls.Add(this.edNumberDigits);
      this.tpPlants.Controls.Add(this.label4);
      this.tpPlants.Location = new System.Drawing.Point(4, 22);
      this.tpPlants.Name = "tpPlants";
      this.tpPlants.Padding = new System.Windows.Forms.Padding(3);
      this.tpPlants.Size = new System.Drawing.Size(616, 196);
      this.tpPlants.TabIndex = 2;
      this.tpPlants.Text = "Каталог";
      this.tpPlants.UseVisualStyleBackColor = true;
      // 
      // edNumberDigits
      // 
      this.edNumberDigits.Increment = 1;
      this.edNumberDigits.Location = new System.Drawing.Point(278, 26);
      this.edNumberDigits.Name = "edNumberDigits";
      this.edNumberDigits.Size = new System.Drawing.Size(46, 20);
      this.edNumberDigits.TabIndex = 1;
      this.edNumberDigits.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // label4
      // 
      this.label4.Location = new System.Drawing.Point(17, 26);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(255, 19);
      this.label4.TabIndex = 0;
      this.label4.Text = "Количество знаков в номере по каталогу";
      this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // tpPhoto
      // 
      this.tpPhoto.Controls.Add(this.groupBox1);
      this.tpPhoto.Controls.Add(this.btnBrowsePhotoDir);
      this.tpPhoto.Controls.Add(this.edPhotoDir);
      this.tpPhoto.Controls.Add(this.label1);
      this.tpPhoto.Location = new System.Drawing.Point(4, 22);
      this.tpPhoto.Name = "tpPhoto";
      this.tpPhoto.Padding = new System.Windows.Forms.Padding(3);
      this.tpPhoto.Size = new System.Drawing.Size(616, 196);
      this.tpPhoto.TabIndex = 0;
      this.tpPhoto.Text = "Фото";
      this.tpPhoto.UseVisualStyleBackColor = true;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.rb128x128);
      this.groupBox1.Controls.Add(this.rb64x64);
      this.groupBox1.Controls.Add(this.rb32x32);
      this.groupBox1.Controls.Add(this.rb16x16);
      this.groupBox1.Location = new System.Drawing.Point(11, 62);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(200, 118);
      this.groupBox1.TabIndex = 3;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Размеры миниатюр в списке";
      // 
      // rb128x128
      // 
      this.rb128x128.AutoSize = true;
      this.rb128x128.Location = new System.Drawing.Point(13, 91);
      this.rb128x128.Name = "rb128x128";
      this.rb128x128.Size = new System.Drawing.Size(72, 17);
      this.rb128x128.TabIndex = 3;
      this.rb128x128.TabStop = true;
      this.rb128x128.Text = "128 x 128";
      this.rb128x128.UseVisualStyleBackColor = true;
      // 
      // rb64x64
      // 
      this.rb64x64.AutoSize = true;
      this.rb64x64.Location = new System.Drawing.Point(13, 69);
      this.rb64x64.Name = "rb64x64";
      this.rb64x64.Size = new System.Drawing.Size(60, 17);
      this.rb64x64.TabIndex = 2;
      this.rb64x64.TabStop = true;
      this.rb64x64.Text = "64 x 64";
      this.rb64x64.UseVisualStyleBackColor = true;
      // 
      // rb32x32
      // 
      this.rb32x32.AutoSize = true;
      this.rb32x32.Location = new System.Drawing.Point(13, 46);
      this.rb32x32.Name = "rb32x32";
      this.rb32x32.Size = new System.Drawing.Size(60, 17);
      this.rb32x32.TabIndex = 1;
      this.rb32x32.TabStop = true;
      this.rb32x32.Text = "32 x 32";
      this.rb32x32.UseVisualStyleBackColor = true;
      // 
      // rb16x16
      // 
      this.rb16x16.AutoSize = true;
      this.rb16x16.Location = new System.Drawing.Point(13, 23);
      this.rb16x16.Name = "rb16x16";
      this.rb16x16.Size = new System.Drawing.Size(60, 17);
      this.rb16x16.TabIndex = 0;
      this.rb16x16.TabStop = true;
      this.rb16x16.Text = "16 x 16";
      this.rb16x16.UseVisualStyleBackColor = true;
      // 
      // btnBrowsePhotoDir
      // 
      this.btnBrowsePhotoDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnBrowsePhotoDir.Location = new System.Drawing.Point(520, 36);
      this.btnBrowsePhotoDir.Name = "btnBrowsePhotoDir";
      this.btnBrowsePhotoDir.Size = new System.Drawing.Size(88, 24);
      this.btnBrowsePhotoDir.TabIndex = 2;
      this.btnBrowsePhotoDir.Text = "Обзор";
      this.btnBrowsePhotoDir.UseVisualStyleBackColor = true;
      // 
      // edPhotoDir
      // 
      this.edPhotoDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.edPhotoDir.Location = new System.Drawing.Point(11, 36);
      this.edPhotoDir.Name = "edPhotoDir";
      this.edPhotoDir.Size = new System.Drawing.Size(503, 20);
      this.edPhotoDir.TabIndex = 1;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(8, 20);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(136, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Каталог с фотографиями";
      // 
      // tpBackup
      // 
      this.tpBackup.Controls.Add(this.groupBox2);
      this.tpBackup.Location = new System.Drawing.Point(4, 22);
      this.tpBackup.Name = "tpBackup";
      this.tpBackup.Padding = new System.Windows.Forms.Padding(3);
      this.tpBackup.Size = new System.Drawing.Size(616, 196);
      this.tpBackup.TabIndex = 1;
      this.tpBackup.Text = "Резервное копирование";
      this.tpBackup.UseVisualStyleBackColor = true;
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.btnExploreBackupDir);
      this.groupBox2.Controls.Add(this.btnBrowseBackupDir);
      this.groupBox2.Controls.Add(this.edBackupDir);
      this.groupBox2.Controls.Add(this.label2);
      this.groupBox2.Controls.Add(this.cbBackupMode);
      this.groupBox2.Controls.Add(this.label3);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
      this.groupBox2.Location = new System.Drawing.Point(3, 3);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(610, 122);
      this.groupBox2.TabIndex = 0;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Параметры резервного копирования";
      // 
      // btnExploreBackupDir
      // 
      this.btnExploreBackupDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnExploreBackupDir.Location = new System.Drawing.Point(462, 91);
      this.btnExploreBackupDir.Name = "btnExploreBackupDir";
      this.btnExploreBackupDir.Size = new System.Drawing.Size(132, 24);
      this.btnExploreBackupDir.TabIndex = 5;
      this.btnExploreBackupDir.Text = "Проводник";
      this.btnExploreBackupDir.UseVisualStyleBackColor = true;
      // 
      // btnBrowseBackupDir
      // 
      this.btnBrowseBackupDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnBrowseBackupDir.Location = new System.Drawing.Point(506, 63);
      this.btnBrowseBackupDir.Name = "btnBrowseBackupDir";
      this.btnBrowseBackupDir.Size = new System.Drawing.Size(88, 24);
      this.btnBrowseBackupDir.TabIndex = 4;
      this.btnBrowseBackupDir.Text = "Обзор";
      this.btnBrowseBackupDir.UseVisualStyleBackColor = true;
      // 
      // edBackupDir
      // 
      this.edBackupDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.edBackupDir.Location = new System.Drawing.Point(93, 65);
      this.edBackupDir.Name = "edBackupDir";
      this.edBackupDir.Size = new System.Drawing.Size(407, 20);
      this.edBackupDir.TabIndex = 3;
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(6, 65);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(81, 21);
      this.label2.TabIndex = 2;
      this.label2.Text = "Каталог";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // cbBackupMode
      // 
      this.cbBackupMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cbBackupMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbBackupMode.FormattingEnabled = true;
      this.cbBackupMode.Items.AddRange(new object[] {
            "Не делать",
            "При каждом завершении работы программы",
            "Один раз в день"});
      this.cbBackupMode.Location = new System.Drawing.Point(93, 28);
      this.cbBackupMode.Name = "cbBackupMode";
      this.cbBackupMode.Size = new System.Drawing.Size(501, 21);
      this.cbBackupMode.TabIndex = 1;
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(6, 28);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(81, 21);
      this.label3.TabIndex = 0;
      this.label3.Text = "Ре&жим";
      this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // label5
      // 
      this.label5.Location = new System.Drawing.Point(344, 26);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(174, 19);
      this.label5.TabIndex = 2;
      this.label5.Text = "(требуется перезапуск)";
      this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.label5.UseMnemonic = false;
      // 
      // UserSettingsForm
      // 
      this.AcceptButton = this.btnOk;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(624, 262);
      this.Controls.Add(this.TheTabControl);
      this.Controls.Add(this.panel1);
      this.Name = "UserSettingsForm";
      this.Text = "Настройки";
      this.panel1.ResumeLayout(false);
      this.TheTabControl.ResumeLayout(false);
      this.tpPlants.ResumeLayout(false);
      this.tpPhoto.ResumeLayout(false);
      this.tpPhoto.PerformLayout();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.tpBackup.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnOk;
    private System.Windows.Forms.TabControl TheTabControl;
    private System.Windows.Forms.TabPage tpPhoto;
    private System.Windows.Forms.Button btnBrowsePhotoDir;
    private System.Windows.Forms.TextBox edPhotoDir;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.RadioButton rb128x128;
    private System.Windows.Forms.RadioButton rb64x64;
    private System.Windows.Forms.RadioButton rb32x32;
    private System.Windows.Forms.RadioButton rb16x16;
    private System.Windows.Forms.TabPage tpBackup;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Button btnBrowseBackupDir;
    private System.Windows.Forms.TextBox edBackupDir;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox cbBackupMode;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button btnExploreBackupDir;
    private System.Windows.Forms.TabPage tpPlants;
    private FreeLibSet.Controls.IntEditBox edNumberDigits;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
  }
}
