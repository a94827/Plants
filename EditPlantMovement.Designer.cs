namespace Plants
{
  partial class EditPlantMovement
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
      this.cbPotKind = new FreeLibSet.Controls.UserSelComboBox();
      this.label6 = new System.Windows.Forms.Label();
      this.cbSoil = new FreeLibSet.Controls.UserSelComboBox();
      this.label5 = new System.Windows.Forms.Label();
      this.cbDate = new FreeLibSet.Controls.UserMaskedComboBox();
      this.label4 = new System.Windows.Forms.Label();
      this.cbContra = new FreeLibSet.Controls.UserSelComboBox();
      this.label3 = new System.Windows.Forms.Label();
      this.cbPlace = new FreeLibSet.Controls.UserSelComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.cbKind = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.cbForkPlant = new FreeLibSet.Controls.UserSelComboBox();
      this.label7 = new System.Windows.Forms.Label();
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
      this.MainPanel1.Size = new System.Drawing.Size(502, 325);
      this.MainPanel1.TabIndex = 0;
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.edComment);
      this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox3.Location = new System.Drawing.Point(0, 210);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(502, 115);
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
      this.edComment.Size = new System.Drawing.Size(496, 96);
      this.edComment.TabIndex = 0;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.cbForkPlant);
      this.groupBox1.Controls.Add(this.label7);
      this.groupBox1.Controls.Add(this.cbPotKind);
      this.groupBox1.Controls.Add(this.label6);
      this.groupBox1.Controls.Add(this.cbSoil);
      this.groupBox1.Controls.Add(this.label5);
      this.groupBox1.Controls.Add(this.cbDate);
      this.groupBox1.Controls.Add(this.label4);
      this.groupBox1.Controls.Add(this.cbContra);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Controls.Add(this.cbPlace);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.cbKind);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
      this.groupBox1.Location = new System.Drawing.Point(0, 0);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(502, 210);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Приход / перемещение / выбытие";
      // 
      // cbPotKind
      // 
      this.cbPotKind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cbPotKind.Location = new System.Drawing.Point(152, 183);
      this.cbPotKind.Name = "cbPotKind";
      this.cbPotKind.Size = new System.Drawing.Size(338, 20);
      this.cbPotKind.TabIndex = 13;
      // 
      // label6
      // 
      this.label6.Location = new System.Drawing.Point(6, 183);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(140, 21);
      this.label6.TabIndex = 12;
      this.label6.Text = "Гор&шок";
      this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // cbSoil
      // 
      this.cbSoil.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cbSoil.Location = new System.Drawing.Point(152, 157);
      this.cbSoil.Name = "cbSoil";
      this.cbSoil.Size = new System.Drawing.Size(338, 20);
      this.cbSoil.TabIndex = 11;
      // 
      // label5
      // 
      this.label5.Location = new System.Drawing.Point(6, 157);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(140, 21);
      this.label5.TabIndex = 10;
      this.label5.Text = "&Грунт";
      this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
      // cbContra
      // 
      this.cbContra.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cbContra.Location = new System.Drawing.Point(152, 105);
      this.cbContra.Name = "cbContra";
      this.cbContra.Size = new System.Drawing.Size(338, 20);
      this.cbContra.TabIndex = 7;
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(6, 105);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(140, 20);
      this.label3.TabIndex = 6;
      this.label3.Text = "О&т кого / кому";
      this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // cbPlace
      // 
      this.cbPlace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cbPlace.Location = new System.Drawing.Point(152, 79);
      this.cbPlace.Name = "cbPlace";
      this.cbPlace.Size = new System.Drawing.Size(338, 20);
      this.cbPlace.TabIndex = 5;
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(6, 79);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(140, 20);
      this.label2.TabIndex = 4;
      this.label2.Text = "&Место";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // cbKind
      // 
      this.cbKind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cbKind.FormattingEnabled = true;
      this.cbKind.Location = new System.Drawing.Point(152, 26);
      this.cbKind.Name = "cbKind";
      this.cbKind.Size = new System.Drawing.Size(338, 21);
      this.cbKind.TabIndex = 1;
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(6, 26);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(140, 21);
      this.label1.TabIndex = 0;
      this.label1.Text = "&Операция";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // cbForkPlant
      // 
      this.cbForkPlant.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cbForkPlant.Location = new System.Drawing.Point(152, 131);
      this.cbForkPlant.Name = "cbForkPlant";
      this.cbForkPlant.Size = new System.Drawing.Size(338, 20);
      this.cbForkPlant.TabIndex = 9;
      // 
      // label7
      // 
      this.label7.Location = new System.Drawing.Point(6, 131);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(140, 20);
      this.label7.TabIndex = 8;
      this.label7.Text = "Отса&жено / подсажено";
      this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // EditPlantMovement
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(502, 325);
      this.Controls.Add(this.MainPanel1);
      this.Name = "EditPlantMovement";
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
    private FreeLibSet.Controls.UserSelComboBox cbPlace;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox cbKind;
    private System.Windows.Forms.Label label1;
    private FreeLibSet.Controls.UserSelComboBox cbContra;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.TextBox edComment;
    private FreeLibSet.Controls.UserMaskedComboBox cbDate;
    private System.Windows.Forms.Label label4;
    private FreeLibSet.Controls.UserSelComboBox cbPotKind;
    private System.Windows.Forms.Label label6;
    private FreeLibSet.Controls.UserSelComboBox cbSoil;
    private System.Windows.Forms.Label label5;
    private FreeLibSet.Controls.UserSelComboBox cbForkPlant;
    private System.Windows.Forms.Label label7;
  }
}