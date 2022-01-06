namespace Plants
{
  partial class EditAttrValue
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
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.MainPanel1 = new System.Windows.Forms.Panel();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.edValue = new FreeLibSet.Controls.UserMaskedComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.edDate = new FreeLibSet.Controls.DateTimeBox();
      this.label1 = new System.Windows.Forms.Label();
      this.cbAttrType = new FreeLibSet.Controls.UserSelComboBox();
      this.label3 = new System.Windows.Forms.Label();
      this.groupBox5 = new System.Windows.Forms.GroupBox();
      this.edComment = new System.Windows.Forms.TextBox();
      this.tabControl1.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.MainPanel1.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.groupBox5.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.tabPage1);
      this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl1.Location = new System.Drawing.Point(0, 0);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(582, 273);
      this.tabControl1.TabIndex = 0;
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.MainPanel1);
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(574, 247);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "Общие";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // MainPanel1
      // 
      this.MainPanel1.Controls.Add(this.groupBox5);
      this.MainPanel1.Controls.Add(this.groupBox1);
      this.MainPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.MainPanel1.Location = new System.Drawing.Point(3, 3);
      this.MainPanel1.Name = "MainPanel1";
      this.MainPanel1.Size = new System.Drawing.Size(568, 241);
      this.MainPanel1.TabIndex = 0;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.edValue);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.edDate);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.cbAttrType);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
      this.groupBox1.Location = new System.Drawing.Point(0, 0);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(568, 133);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Значение атрибута";
      // 
      // edValue
      // 
      this.edValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.edValue.ClearButtonEnabled = false;
      this.edValue.Culture = new System.Globalization.CultureInfo("ru-RU");
      this.edValue.Location = new System.Drawing.Point(143, 99);
      this.edValue.Name = "edValue";
      this.edValue.Size = new System.Drawing.Size(419, 20);
      this.edValue.TabIndex = 5;
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(5, 100);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(131, 19);
      this.label2.TabIndex = 4;
      this.label2.Text = "&Значение";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // edDate
      // 
      this.edDate.Location = new System.Drawing.Point(143, 63);
      this.edDate.Name = "edDate";
      this.edDate.Size = new System.Drawing.Size(120, 20);
      this.edDate.TabIndex = 3;
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(6, 64);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(131, 19);
      this.label1.TabIndex = 2;
      this.label1.Text = "&Начало действия";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // cbAttrType
      // 
      this.cbAttrType.Location = new System.Drawing.Point(143, 28);
      this.cbAttrType.Name = "cbAttrType";
      this.cbAttrType.Size = new System.Drawing.Size(419, 20);
      this.cbAttrType.TabIndex = 1;
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(6, 28);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(131, 19);
      this.label3.TabIndex = 0;
      this.label3.Text = "&Атрибут";
      this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // groupBox5
      // 
      this.groupBox5.Controls.Add(this.edComment);
      this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox5.Location = new System.Drawing.Point(0, 133);
      this.groupBox5.Name = "groupBox5";
      this.groupBox5.Size = new System.Drawing.Size(568, 108);
      this.groupBox5.TabIndex = 2;
      this.groupBox5.TabStop = false;
      this.groupBox5.Text = "Комментари&й";
      // 
      // edComment
      // 
      this.edComment.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.edComment.Location = new System.Drawing.Point(6, 19);
      this.edComment.Multiline = true;
      this.edComment.Name = "edComment";
      this.edComment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.edComment.Size = new System.Drawing.Size(556, 83);
      this.edComment.TabIndex = 0;
      // 
      // EditAttrValue
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(582, 273);
      this.Controls.Add(this.tabControl1);
      this.Name = "EditAttrValue";
      this.tabControl1.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.MainPanel1.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      this.groupBox5.ResumeLayout(false);
      this.groupBox5.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.Panel MainPanel1;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label3;
    private FreeLibSet.Controls.UserSelComboBox cbAttrType;
    private System.Windows.Forms.Label label1;
    private FreeLibSet.Controls.DateTimeBox edDate;
    private FreeLibSet.Controls.UserMaskedComboBox edValue;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.GroupBox groupBox5;
    private System.Windows.Forms.TextBox edComment;
  }
}