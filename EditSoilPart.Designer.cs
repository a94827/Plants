namespace Plants
{
  partial class EditSoilPart
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
      this.MainPanel1 = new System.Windows.Forms.Panel();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.cbSoil = new FreeLibSet.Controls.UserSelComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.edPercent = new FreeLibSet.Controls.IntEditBox();
      this.MainPanel1.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // MainPanel1
      // 
      this.MainPanel1.Controls.Add(this.groupBox1);
      this.MainPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.MainPanel1.Location = new System.Drawing.Point(0, 0);
      this.MainPanel1.Name = "MainPanel1";
      this.MainPanel1.Size = new System.Drawing.Size(423, 97);
      this.MainPanel1.TabIndex = 0;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.edPercent);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.cbSoil);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
      this.groupBox1.Location = new System.Drawing.Point(0, 0);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(423, 88);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Компонент грунта";
      // 
      // cbSoil
      // 
      this.cbSoil.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cbSoil.Location = new System.Drawing.Point(152, 27);
      this.cbSoil.Name = "cbSoil";
      this.cbSoil.Size = new System.Drawing.Size(259, 20);
      this.cbSoil.TabIndex = 1;
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(6, 27);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(140, 20);
      this.label2.TabIndex = 0;
      this.label2.Text = "&Смешиваемый грунт";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(7, 53);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(140, 20);
      this.label1.TabIndex = 2;
      this.label1.Text = "&Процент";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // edPercent
      // 
      this.edPercent.Location = new System.Drawing.Point(153, 53);
      this.edPercent.Name = "edPercent";
      this.edPercent.Size = new System.Drawing.Size(100, 20);
      this.edPercent.TabIndex = 3;
      // 
      // EditSoilPart
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(423, 97);
      this.Controls.Add(this.MainPanel1);
      this.Name = "EditSoilPart";
      this.MainPanel1.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel MainPanel1;
    private System.Windows.Forms.GroupBox groupBox1;
    private FreeLibSet.Controls.UserSelComboBox cbSoil;
    private System.Windows.Forms.Label label2;
    private FreeLibSet.Controls.IntEditBox edPercent;
    private System.Windows.Forms.Label label1;
  }
}