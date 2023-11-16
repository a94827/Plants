namespace Plants
{
  partial class EditAttrValueGroup
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
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.lblDocInfo = new System.Windows.Forms.Label();
      this.lblDocTypeName = new System.Windows.Forms.Label();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.cbAttrType = new FreeLibSet.Controls.UserSelComboBox();
      this.label3 = new System.Windows.Forms.Label();
      this.edDate = new FreeLibSet.Controls.DateTimeBox();
      this.label1 = new System.Windows.Forms.Label();
      this.groupBox4 = new System.Windows.Forms.GroupBox();
      this.rbDelete = new System.Windows.Forms.RadioButton();
      this.rbEdit = new System.Windows.Forms.RadioButton();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.edValue = new FreeLibSet.Controls.UserMaskedComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.panel1 = new System.Windows.Forms.Panel();
      this.btnOk = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.groupBox3.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.groupBox4.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.lblDocInfo);
      this.groupBox3.Controls.Add(this.lblDocTypeName);
      this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
      this.groupBox3.Location = new System.Drawing.Point(0, 0);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(537, 75);
      this.groupBox3.TabIndex = 0;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Выбранные документы";
      // 
      // lblDocInfo
      // 
      this.lblDocInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.lblDocInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.lblDocInfo.Location = new System.Drawing.Point(12, 39);
      this.lblDocInfo.Name = "lblDocInfo";
      this.lblDocInfo.Size = new System.Drawing.Size(513, 23);
      this.lblDocInfo.TabIndex = 1;
      this.lblDocInfo.Text = "???";
      this.lblDocInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // lblDocTypeName
      // 
      this.lblDocTypeName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.lblDocTypeName.Location = new System.Drawing.Point(10, 16);
      this.lblDocTypeName.Name = "lblDocTypeName";
      this.lblDocTypeName.Size = new System.Drawing.Size(515, 23);
      this.lblDocTypeName.TabIndex = 0;
      this.lblDocTypeName.Text = "???";
      this.lblDocTypeName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.cbAttrType);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Controls.Add(this.edDate);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
      this.groupBox1.Location = new System.Drawing.Point(0, 75);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(537, 85);
      this.groupBox1.TabIndex = 1;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Атрибут";
      // 
      // cbAttrType
      // 
      this.cbAttrType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cbAttrType.Location = new System.Drawing.Point(145, 21);
      this.cbAttrType.Name = "cbAttrType";
      this.cbAttrType.Size = new System.Drawing.Size(380, 20);
      this.cbAttrType.TabIndex = 1;
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(10, 21);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(133, 21);
      this.label3.TabIndex = 0;
      this.label3.Text = "&Атрибут";
      this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // edDate
      // 
      this.edDate.Location = new System.Drawing.Point(145, 52);
      this.edDate.Margin = new System.Windows.Forms.Padding(0);
      this.edDate.Name = "edDate";
      this.edDate.Size = new System.Drawing.Size(127, 20);
      this.edDate.TabIndex = 3;
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(9, 52);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(133, 20);
      this.label1.TabIndex = 2;
      this.label1.Text = "Начало &действия";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // groupBox4
      // 
      this.groupBox4.Controls.Add(this.rbDelete);
      this.groupBox4.Controls.Add(this.rbEdit);
      this.groupBox4.Dock = System.Windows.Forms.DockStyle.Top;
      this.groupBox4.Location = new System.Drawing.Point(0, 160);
      this.groupBox4.Name = "groupBox4";
      this.groupBox4.Size = new System.Drawing.Size(537, 46);
      this.groupBox4.TabIndex = 2;
      this.groupBox4.TabStop = false;
      this.groupBox4.Text = "Действие";
      // 
      // rbDelete
      // 
      this.rbDelete.AutoSize = true;
      this.rbDelete.Location = new System.Drawing.Point(223, 19);
      this.rbDelete.Name = "rbDelete";
      this.rbDelete.Size = new System.Drawing.Size(118, 17);
      this.rbDelete.TabIndex = 1;
      this.rbDelete.TabStop = true;
      this.rbDelete.Text = "Удалить значение";
      this.rbDelete.UseVisualStyleBackColor = true;
      // 
      // rbEdit
      // 
      this.rbEdit.AutoSize = true;
      this.rbEdit.Location = new System.Drawing.Point(12, 19);
      this.rbEdit.Name = "rbEdit";
      this.rbEdit.Size = new System.Drawing.Size(185, 17);
      this.rbEdit.TabIndex = 0;
      this.rbEdit.TabStop = true;
      this.rbEdit.Text = "Добавить / изменить значение";
      this.rbEdit.UseVisualStyleBackColor = true;
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.edValue);
      this.groupBox2.Controls.Add(this.label2);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
      this.groupBox2.Location = new System.Drawing.Point(0, 206);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(537, 68);
      this.groupBox2.TabIndex = 3;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Значение атрибута";
      // 
      // edValue
      // 
      this.edValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.edValue.ClearButtonEnabled = false;
      this.edValue.Location = new System.Drawing.Point(143, 32);
      this.edValue.Name = "edValue";
      this.edValue.Size = new System.Drawing.Size(388, 20);
      this.edValue.TabIndex = 1;
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(10, 33);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(131, 19);
      this.label2.TabIndex = 0;
      this.label2.Text = "&Значение";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btnCancel);
      this.panel1.Controls.Add(this.btnOk);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 277);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(537, 40);
      this.panel1.TabIndex = 4;
      // 
      // btnOk
      // 
      this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnOk.Location = new System.Drawing.Point(8, 8);
      this.btnOk.Name = "btnOk";
      this.btnOk.Size = new System.Drawing.Size(88, 24);
      this.btnOk.TabIndex = 0;
      this.btnOk.Text = "О&К";
      this.btnOk.UseVisualStyleBackColor = true;
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(102, 8);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(88, 24);
      this.btnCancel.TabIndex = 1;
      this.btnCancel.Text = "Отмена";
      this.btnCancel.UseVisualStyleBackColor = true;
      // 
      // EditAttrValueGroup
      // 
      this.AcceptButton = this.btnOk;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(537, 317);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox4);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.groupBox3);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "EditAttrValueGroup";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Групповая установка значений атрибута";
      this.groupBox3.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      this.groupBox4.ResumeLayout(false);
      this.groupBox4.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.Label lblDocInfo;
    private System.Windows.Forms.Label lblDocTypeName;
    private System.Windows.Forms.GroupBox groupBox1;
    private FreeLibSet.Controls.UserSelComboBox cbAttrType;
    private System.Windows.Forms.Label label3;
    private FreeLibSet.Controls.DateTimeBox edDate;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.GroupBox groupBox4;
    private System.Windows.Forms.RadioButton rbDelete;
    private System.Windows.Forms.RadioButton rbEdit;
    private System.Windows.Forms.GroupBox groupBox2;
    private FreeLibSet.Controls.UserMaskedComboBox edValue;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnOk;

  }
}
