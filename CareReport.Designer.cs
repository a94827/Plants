namespace Plants
{
  partial class CareReportParamForm
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
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.infoLabel1 = new FreeLibSet.Controls.InfoLabel();
      this.edDay = new System.Windows.Forms.MaskedTextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.grpPeriod = new System.Windows.Forms.GroupBox();
      this.edPeriod = new FreeLibSet.Controls.DateRangeBox();
      this.MainTabPage.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.grpPeriod.SuspendLayout();
      this.SuspendLayout();
      // 
      // MainTabPage
      // 
      this.MainTabPage.Controls.Add(this.grpPeriod);
      this.MainTabPage.Controls.Add(this.groupBox2);
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.infoLabel1);
      this.groupBox2.Controls.Add(this.edDay);
      this.groupBox2.Controls.Add(this.label1);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
      this.groupBox2.Location = new System.Drawing.Point(3, 3);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(618, 64);
      this.groupBox2.TabIndex = 0;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Дата для ухода";
      // 
      // infoLabel1
      // 
      this.infoLabel1.Dock = System.Windows.Forms.DockStyle.Right;
      this.infoLabel1.Location = new System.Drawing.Point(254, 16);
      this.infoLabel1.Name = "infoLabel1";
      this.infoLabel1.Size = new System.Drawing.Size(361, 45);
      this.infoLabel1.TabIndex = 2;
      this.infoLabel1.Text = "Если пусто, то будут выведены данные по уходу за весь год";
      // 
      // edDay
      // 
      this.edDay.Location = new System.Drawing.Point(134, 25);
      this.edDay.Name = "edDay";
      this.edDay.Size = new System.Drawing.Size(100, 20);
      this.edDay.TabIndex = 1;
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(12, 22);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(100, 23);
      this.label1.TabIndex = 0;
      this.label1.Text = "День и месяц";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // grpPeriod
      // 
      this.grpPeriod.Controls.Add(this.edPeriod);
      this.grpPeriod.Dock = System.Windows.Forms.DockStyle.Top;
      this.grpPeriod.Location = new System.Drawing.Point(3, 67);
      this.grpPeriod.Name = "grpPeriod";
      this.grpPeriod.Size = new System.Drawing.Size(618, 69);
      this.grpPeriod.TabIndex = 1;
      this.grpPeriod.TabStop = false;
      this.grpPeriod.Text = "Период для фильтров";
      // 
      // edPeriod
      // 
      this.edPeriod.Location = new System.Drawing.Point(15, 19);
      this.edPeriod.Name = "edPeriod";
      this.edPeriod.Size = new System.Drawing.Size(350, 37);
      this.edPeriod.TabIndex = 0;
      // 
      // CareReportParamForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(632, 452);
      this.Name = "CareReportParamForm";
      this.Text = "Создание списка правил ухода за растениями";
      this.MainTabPage.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.grpPeriod.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox grpPeriod;
    private FreeLibSet.Controls.DateRangeBox edPeriod;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Label label1;
    private FreeLibSet.Controls.InfoLabel infoLabel1;
    private System.Windows.Forms.MaskedTextBox edDay;


  }
}
