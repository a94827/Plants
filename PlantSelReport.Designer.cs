namespace Plants
{
  partial class PlantSelReportParamForm
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
      this.grpPeriod = new System.Windows.Forms.GroupBox();
      this.edPeriod = new FreeLibSet.Controls.DateRangeBox();
      this.MainTabPage.SuspendLayout();
      this.grpPeriod.SuspendLayout();
      this.SuspendLayout();
      // 
      // MainTabPage
      // 
      this.MainTabPage.Controls.Add(this.grpPeriod);
      // 
      // grpPeriod
      // 
      this.grpPeriod.Controls.Add(this.edPeriod);
      this.grpPeriod.Dock = System.Windows.Forms.DockStyle.Top;
      this.grpPeriod.Location = new System.Drawing.Point(3, 3);
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
      // PlantSelReportParamForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(632, 452);
      this.Name = "PlantSelReportParamForm";
      this.Text = "Создание выборки из каталога растений";
      this.MainTabPage.ResumeLayout(false);
      this.grpPeriod.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox grpPeriod;
    private FreeLibSet.Controls.DateRangeBox edPeriod;

  }
}