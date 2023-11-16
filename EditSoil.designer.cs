namespace Plants
{
  partial class EditSoil
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
      this.groupBox5 = new System.Windows.Forms.GroupBox();
      this.edComment = new System.Windows.Forms.TextBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.cbManufacturer = new FreeLibSet.Controls.UserSelComboBox();
      this.label7 = new System.Windows.Forms.Label();
      this.cbGroup = new FreeLibSet.Controls.UserSelComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.edName = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.edpHmin = new FreeLibSet.Controls.SingleEditBox();
      this.edpHmax = new FreeLibSet.Controls.SingleEditBox();
      this.label4 = new System.Windows.Forms.Label();
      this.tabControl1.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.MainPanel1.SuspendLayout();
      this.groupBox5.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.tabPage1);
      this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl1.Location = new System.Drawing.Point(0, 0);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(582, 258);
      this.tabControl1.TabIndex = 0;
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.MainPanel1);
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(574, 232);
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
      this.MainPanel1.Size = new System.Drawing.Size(568, 226);
      this.MainPanel1.TabIndex = 0;
      // 
      // groupBox5
      // 
      this.groupBox5.Controls.Add(this.edComment);
      this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox5.Location = new System.Drawing.Point(0, 139);
      this.groupBox5.Name = "groupBox5";
      this.groupBox5.Size = new System.Drawing.Size(568, 87);
      this.groupBox5.TabIndex = 1;
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
      this.edComment.Size = new System.Drawing.Size(556, 62);
      this.edComment.TabIndex = 0;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.edpHmax);
      this.groupBox1.Controls.Add(this.label4);
      this.groupBox1.Controls.Add(this.edpHmin);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Controls.Add(this.cbManufacturer);
      this.groupBox1.Controls.Add(this.label7);
      this.groupBox1.Controls.Add(this.cbGroup);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.edName);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
      this.groupBox1.Location = new System.Drawing.Point(0, 0);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(568, 139);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Грунт";
      // 
      // cbManufacturer
      // 
      this.cbManufacturer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cbManufacturer.Location = new System.Drawing.Point(180, 47);
      this.cbManufacturer.Name = "cbManufacturer";
      this.cbManufacturer.Size = new System.Drawing.Size(372, 20);
      this.cbManufacturer.TabIndex = 3;
      // 
      // label7
      // 
      this.label7.Location = new System.Drawing.Point(6, 47);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(158, 20);
      this.label7.TabIndex = 2;
      this.label7.Text = "&Изготовитель";
      this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // cbGroup
      // 
      this.cbGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cbGroup.Location = new System.Drawing.Point(180, 106);
      this.cbGroup.Name = "cbGroup";
      this.cbGroup.Size = new System.Drawing.Size(372, 20);
      this.cbGroup.TabIndex = 9;
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(6, 106);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(158, 20);
      this.label2.TabIndex = 8;
      this.label2.Text = "&Группа";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // edName
      // 
      this.edName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.edName.Location = new System.Drawing.Point(180, 19);
      this.edName.Name = "edName";
      this.edName.Size = new System.Drawing.Size(372, 20);
      this.edName.TabIndex = 1;
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(6, 19);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(158, 20);
      this.label1.TabIndex = 0;
      this.label1.Text = "&Название";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(6, 76);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(158, 20);
      this.label3.TabIndex = 4;
      this.label3.Text = "Кислотность pH от";
      this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // edpHmin
      // 
      this.edpHmin.DecimalPlaces = 1;
      this.edpHmin.Location = new System.Drawing.Point(180, 76);
      this.edpHmin.Name = "edpHmin";
      this.edpHmin.Size = new System.Drawing.Size(81, 20);
      this.edpHmin.TabIndex = 5;
      // 
      // edpHmax
      // 
      this.edpHmax.DecimalPlaces = 1;
      this.edpHmax.Location = new System.Drawing.Point(322, 76);
      this.edpHmax.Name = "edpHmax";
      this.edpHmax.Size = new System.Drawing.Size(81, 20);
      this.edpHmax.TabIndex = 7;
      // 
      // label4
      // 
      this.label4.Location = new System.Drawing.Point(272, 76);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(44, 20);
      this.label4.TabIndex = 6;
      this.label4.Text = "до";
      this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // EditSoil
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(582, 258);
      this.Controls.Add(this.tabControl1);
      this.Name = "EditSoil";
      this.tabControl1.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.MainPanel1.ResumeLayout(false);
      this.groupBox5.ResumeLayout(false);
      this.groupBox5.PerformLayout();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.Panel MainPanel1;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TextBox edName;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.GroupBox groupBox5;
    private System.Windows.Forms.TextBox edComment;
    private FreeLibSet.Controls.UserSelComboBox cbGroup;
    private System.Windows.Forms.Label label2;
    private FreeLibSet.Controls.UserSelComboBox cbManufacturer;
    private System.Windows.Forms.Label label7;
    private FreeLibSet.Controls.SingleEditBox edpHmax;
    private System.Windows.Forms.Label label4;
    private FreeLibSet.Controls.SingleEditBox edpHmin;
    private System.Windows.Forms.Label label3;
  }
}
