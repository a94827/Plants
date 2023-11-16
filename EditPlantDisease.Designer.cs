namespace Plants
{
  partial class EditPlantDisease
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
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.edComment = new System.Windows.Forms.TextBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.cbDate = new FreeLibSet.Controls.UserMaskedComboBox();
      this.label4 = new System.Windows.Forms.Label();
      this.cbDisease = new FreeLibSet.Controls.UserSelComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.btnDate99991231 = new System.Windows.Forms.Button();
      this.MainPanel1.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // MainPanel1
      // 
      this.MainPanel1.Controls.Add(this.groupBox3);
      this.MainPanel1.Controls.Add(this.groupBox1);
      this.MainPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.MainPanel1.Location = new System.Drawing.Point(0, 0);
      this.MainPanel1.Name = "MainPanel1";
      this.MainPanel1.Size = new System.Drawing.Size(423, 254);
      this.MainPanel1.TabIndex = 0;
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.edComment);
      this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox3.Location = new System.Drawing.Point(0, 87);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(423, 167);
      this.groupBox3.TabIndex = 1;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Комментари&й";
      // 
      // edComment
      // 
      this.edComment.Dock = System.Windows.Forms.DockStyle.Fill;
      this.edComment.Location = new System.Drawing.Point(3, 16);
      this.edComment.Multiline = true;
      this.edComment.Name = "edComment";
      this.edComment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.edComment.Size = new System.Drawing.Size(417, 148);
      this.edComment.TabIndex = 0;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.btnDate99991231);
      this.groupBox1.Controls.Add(this.cbDate);
      this.groupBox1.Controls.Add(this.label4);
      this.groupBox1.Controls.Add(this.cbDisease);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
      this.groupBox1.Location = new System.Drawing.Point(0, 0);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(423, 87);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Приход / перемещение / выбытие";
      // 
      // cbDate
      // 
      this.cbDate.ClearButtonEnabled = false;
      this.cbDate.Culture = new System.Globalization.CultureInfo("ru-RU");
      this.cbDate.Location = new System.Drawing.Point(152, 53);
      this.cbDate.Name = "cbDate";
      this.cbDate.Size = new System.Drawing.Size(162, 20);
      this.cbDate.TabIndex = 3;
      // 
      // label4
      // 
      this.label4.Location = new System.Drawing.Point(6, 53);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(140, 20);
      this.label4.TabIndex = 2;
      this.label4.Text = "&Дата";
      this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // cbDisease
      // 
      this.cbDisease.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cbDisease.Location = new System.Drawing.Point(152, 27);
      this.cbDisease.Name = "cbDisease";
      this.cbDisease.Size = new System.Drawing.Size(259, 20);
      this.cbDisease.TabIndex = 1;
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(6, 27);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(140, 20);
      this.label2.TabIndex = 0;
      this.label2.Text = "&Заболевание";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // btnDate99991231
      // 
      this.btnDate99991231.Location = new System.Drawing.Point(320, 53);
      this.btnDate99991231.Name = "btnDate99991231";
      this.btnDate99991231.Size = new System.Drawing.Size(32, 24);
      this.btnDate99991231.TabIndex = 5;
      this.btnDate99991231.UseVisualStyleBackColor = true;
      // 
      // EditPlantDisease
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(423, 254);
      this.Controls.Add(this.MainPanel1);
      this.Name = "EditPlantDisease";
      this.Text = "EditPlantMovement";
      this.MainPanel1.ResumeLayout(false);
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.groupBox1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel MainPanel1;
    private System.Windows.Forms.GroupBox groupBox1;
    private FreeLibSet.Controls.UserSelComboBox cbDisease;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.TextBox edComment;
    private FreeLibSet.Controls.UserMaskedComboBox cbDate;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Button btnDate99991231;
  }
}
