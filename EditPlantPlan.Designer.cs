namespace Plants
{
  partial class EditPlantPlan
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
      this.cbRemedy = new AgeyevAV.ExtForms.UserSelComboBox();
      this.label3 = new System.Windows.Forms.Label();
      this.edActionName = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.cbDate = new AgeyevAV.ExtForms.UserMaskedComboBox();
      this.label4 = new System.Windows.Forms.Label();
      this.cbKind = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
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
      this.MainPanel1.Size = new System.Drawing.Size(497, 257);
      this.MainPanel1.TabIndex = 0;
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.edComment);
      this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox3.Location = new System.Drawing.Point(0, 152);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(497, 105);
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
      this.edComment.Size = new System.Drawing.Size(491, 86);
      this.edComment.TabIndex = 0;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.cbRemedy);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Controls.Add(this.edActionName);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.cbDate);
      this.groupBox1.Controls.Add(this.label4);
      this.groupBox1.Controls.Add(this.cbKind);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
      this.groupBox1.Location = new System.Drawing.Point(0, 0);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(497, 152);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Планимруемое действие";
      // 
      // cbRemedy
      // 
      this.cbRemedy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cbRemedy.Location = new System.Drawing.Point(152, 88);
      this.cbRemedy.Name = "cbRemedy";
      this.cbRemedy.Size = new System.Drawing.Size(333, 20);
      this.cbRemedy.TabIndex = 5;
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(6, 88);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(140, 21);
      this.label3.TabIndex = 4;
      this.label3.Text = "&Препарат";
      this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // edActionName
      // 
      this.edActionName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.edActionName.Location = new System.Drawing.Point(152, 57);
      this.edActionName.Name = "edActionName";
      this.edActionName.Size = new System.Drawing.Size(333, 20);
      this.edActionName.TabIndex = 3;
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(6, 57);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(140, 21);
      this.label2.TabIndex = 2;
      this.label2.Text = "Д&ругое";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // cbDate
      // 
      this.cbDate.ClearButtonEnabled = false;
      this.cbDate.Culture = new System.Globalization.CultureInfo("ru-RU");
      this.cbDate.Location = new System.Drawing.Point(153, 117);
      this.cbDate.Name = "cbDate";
      this.cbDate.Size = new System.Drawing.Size(162, 20);
      this.cbDate.TabIndex = 7;
      // 
      // label4
      // 
      this.label4.Location = new System.Drawing.Point(7, 117);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(140, 20);
      this.label4.TabIndex = 6;
      this.label4.Text = "&Дата";
      this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // cbKind
      // 
      this.cbKind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cbKind.FormattingEnabled = true;
      this.cbKind.Location = new System.Drawing.Point(152, 26);
      this.cbKind.Name = "cbKind";
      this.cbKind.Size = new System.Drawing.Size(333, 21);
      this.cbKind.TabIndex = 1;
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(6, 26);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(140, 21);
      this.label1.TabIndex = 0;
      this.label1.Text = "Д&ействие";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // EditPlantPlan
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(497, 257);
      this.Controls.Add(this.MainPanel1);
      this.Name = "EditPlantPlan";
      this.MainPanel1.ResumeLayout(false);
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel MainPanel1;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.ComboBox cbKind;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.TextBox edComment;
    private AgeyevAV.ExtForms.UserMaskedComboBox cbDate;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox edActionName;
    private System.Windows.Forms.Label label2;
    private AgeyevAV.ExtForms.UserSelComboBox cbRemedy;
    private System.Windows.Forms.Label label3;
  }
}