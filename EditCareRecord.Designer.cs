namespace Plants
{
  partial class EditCareRecord
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
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.grItems = new System.Windows.Forms.DataGridView();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.edDay2 = new System.Windows.Forms.MaskedTextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.edDay1 = new System.Windows.Forms.MaskedTextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.edName = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.MainPanel1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.grItems)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // MainPanel1
      // 
      this.MainPanel1.Controls.Add(this.groupBox2);
      this.MainPanel1.Controls.Add(this.groupBox1);
      this.MainPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.MainPanel1.Location = new System.Drawing.Point(0, 0);
      this.MainPanel1.Name = "MainPanel1";
      this.MainPanel1.Size = new System.Drawing.Size(592, 366);
      this.MainPanel1.TabIndex = 0;
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.grItems);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox2.Location = new System.Drawing.Point(0, 56);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(592, 310);
      this.groupBox2.TabIndex = 1;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Параметры ухода";
      // 
      // grItems
      // 
      this.grItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.grItems.Dock = System.Windows.Forms.DockStyle.Fill;
      this.grItems.Location = new System.Drawing.Point(3, 16);
      this.grItems.Name = "grItems";
      this.grItems.Size = new System.Drawing.Size(586, 291);
      this.grItems.TabIndex = 0;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.edDay2);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.edDay1);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.edName);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
      this.groupBox1.Location = new System.Drawing.Point(0, 0);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(592, 56);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Запись";
      // 
      // edDay2
      // 
      this.edDay2.Location = new System.Drawing.Point(170, 21);
      this.edDay2.Name = "edDay2";
      this.edDay2.Size = new System.Drawing.Size(69, 20);
      this.edDay2.TabIndex = 3;
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(126, 21);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(38, 18);
      this.label2.TabIndex = 2;
      this.label2.Text = "&По";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // edDay1
      // 
      this.edDay1.Location = new System.Drawing.Point(50, 21);
      this.edDay1.Name = "edDay1";
      this.edDay1.Size = new System.Drawing.Size(69, 20);
      this.edDay1.TabIndex = 1;
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(6, 21);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(38, 18);
      this.label1.TabIndex = 0;
      this.label1.Text = "&С";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // edName
      // 
      this.edName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.edName.Location = new System.Drawing.Point(331, 19);
      this.edName.Name = "edName";
      this.edName.Size = new System.Drawing.Size(249, 20);
      this.edName.TabIndex = 5;
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(245, 20);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(80, 20);
      this.label3.TabIndex = 4;
      this.label3.Text = "&Название";
      this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // EditCareRecord
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(592, 366);
      this.Controls.Add(this.MainPanel1);
      this.Name = "EditCareRecord";
      this.Text = "EditPlantMovement";
      this.MainPanel1.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.grItems)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel MainPanel1;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TextBox edName;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.MaskedTextBox edDay2;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.MaskedTextBox edDay1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.DataGridView grItems;
  }
}
