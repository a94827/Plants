namespace Plants
{
  partial class AttrTableViewForm
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
      this.Panel3 = new System.Windows.Forms.Panel();
      this.PanSpb = new System.Windows.Forms.Panel();
      this.panel2 = new System.Windows.Forms.Panel();
      this.edDate = new FreeLibSet.Controls.DateTimeBox();
      this.label1 = new System.Windows.Forms.Label();
      this.panel4 = new System.Windows.Forms.Panel();
      this.edStartDate = new FreeLibSet.Controls.DateTimeBox();
      this.cbEnableInPlace = new System.Windows.Forms.CheckBox();
      this.TheGrid = new System.Windows.Forms.DataGridView();
      this.btnSetStartDate = new System.Windows.Forms.Button();
      this.panel1.SuspendLayout();
      this.Panel3.SuspendLayout();
      this.panel2.SuspendLayout();
      this.panel4.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.TheGrid)).BeginInit();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.Panel3);
      this.panel1.Controls.Add(this.panel2);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(624, 36);
      this.panel1.TabIndex = 0;
      // 
      // Panel3
      // 
      this.Panel3.Controls.Add(this.PanSpb);
      this.Panel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.Panel3.Location = new System.Drawing.Point(182, 0);
      this.Panel3.Name = "Panel3";
      this.Panel3.Size = new System.Drawing.Size(442, 36);
      this.Panel3.TabIndex = 1;
      // 
      // PanSpb
      // 
      this.PanSpb.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.PanSpb.Location = new System.Drawing.Point(0, 8);
      this.PanSpb.Name = "PanSpb";
      this.PanSpb.Size = new System.Drawing.Size(442, 28);
      this.PanSpb.TabIndex = 0;
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.edDate);
      this.panel2.Controls.Add(this.label1);
      this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
      this.panel2.Location = new System.Drawing.Point(0, 0);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(182, 36);
      this.panel2.TabIndex = 0;
      // 
      // edDate
      // 
      this.edDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.edDate.Location = new System.Drawing.Point(59, 9);
      this.edDate.Name = "edDate";
      this.edDate.Size = new System.Drawing.Size(117, 20);
      this.edDate.TabIndex = 1;
      // 
      // label1
      // 
      this.label1.Dock = System.Windows.Forms.DockStyle.Left;
      this.label1.Location = new System.Drawing.Point(0, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(53, 36);
      this.label1.TabIndex = 0;
      this.label1.Text = "&Дата";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // panel4
      // 
      this.panel4.Controls.Add(this.btnSetStartDate);
      this.panel4.Controls.Add(this.edStartDate);
      this.panel4.Controls.Add(this.cbEnableInPlace);
      this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel4.Location = new System.Drawing.Point(0, 400);
      this.panel4.Name = "panel4";
      this.panel4.Size = new System.Drawing.Size(624, 42);
      this.panel4.TabIndex = 2;
      // 
      // edStartDate
      // 
      this.edStartDate.ClearButton = true;
      this.edStartDate.Location = new System.Drawing.Point(293, 15);
      this.edStartDate.Name = "edStartDate";
      this.edStartDate.Size = new System.Drawing.Size(130, 20);
      this.edStartDate.TabIndex = 2;
      // 
      // cbEnableInPlace
      // 
      this.cbEnableInPlace.AutoSize = true;
      this.cbEnableInPlace.Location = new System.Drawing.Point(12, 17);
      this.cbEnableInPlace.Name = "cbEnableInPlace";
      this.cbEnableInPlace.Size = new System.Drawing.Size(256, 17);
      this.cbEnableInPlace.TabIndex = 0;
      this.cbEnableInPlace.Text = "Разрешить редактирование прямо в таблице";
      this.cbEnableInPlace.UseVisualStyleBackColor = true;
      // 
      // TheGrid
      // 
      this.TheGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.TheGrid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.TheGrid.Location = new System.Drawing.Point(0, 36);
      this.TheGrid.Name = "TheGrid";
      this.TheGrid.Size = new System.Drawing.Size(624, 364);
      this.TheGrid.TabIndex = 1;
      // 
      // btnSetStartDate
      // 
      this.btnSetStartDate.Location = new System.Drawing.Point(429, 12);
      this.btnSetStartDate.Name = "btnSetStartDate";
      this.btnSetStartDate.Size = new System.Drawing.Size(32, 24);
      this.btnSetStartDate.TabIndex = 3;
      this.btnSetStartDate.UseVisualStyleBackColor = true;
      // 
      // AttrTableViewForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(624, 442);
      this.Controls.Add(this.TheGrid);
      this.Controls.Add(this.panel4);
      this.Controls.Add(this.panel1);
      this.Name = "AttrTableViewForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
      this.panel1.ResumeLayout(false);
      this.Panel3.ResumeLayout(false);
      this.panel2.ResumeLayout(false);
      this.panel4.ResumeLayout(false);
      this.panel4.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.TheGrid)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Panel Panel3;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Label label1;
    private FreeLibSet.Controls.DateTimeBox edDate;
    private System.Windows.Forms.Panel PanSpb;
    private System.Windows.Forms.Panel panel4;
    private FreeLibSet.Controls.DateTimeBox edStartDate;
    private System.Windows.Forms.CheckBox cbEnableInPlace;
    private System.Windows.Forms.DataGridView TheGrid;
    private System.Windows.Forms.Button btnSetStartDate;
  }
}
