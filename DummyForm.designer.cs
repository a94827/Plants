namespace Plants
{
  partial class DummyForm
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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DummyForm));
      this.MainImageList = new System.Windows.Forms.ImageList(this.components);
      this.SuspendLayout();
      // 
      // MainImageList
      // 
      this.MainImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("MainImageList.ImageStream")));
      this.MainImageList.TransparentColor = System.Drawing.Color.Magenta;
      this.MainImageList.Images.SetKeyName(0, "Place");
      this.MainImageList.Images.SetKeyName(1, "Contra");
      this.MainImageList.Images.SetKeyName(2, "Company");
      this.MainImageList.Images.SetKeyName(3, "Remedy");
      this.MainImageList.Images.SetKeyName(4, "Disease");
      this.MainImageList.Images.SetKeyName(5, "Soil");
      this.MainImageList.Images.SetKeyName(6, "PotKind");
      this.MainImageList.Images.SetKeyName(7, "Attribute");
      this.MainImageList.Images.SetKeyName(8, "AttributeType");
      this.MainImageList.Images.SetKeyName(9, "AttributeTable");
      this.MainImageList.Images.SetKeyName(10, "Plant");
      this.MainImageList.Images.SetKeyName(11, "PlantMovement");
      this.MainImageList.Images.SetKeyName(12, "PlantAction");
      this.MainImageList.Images.SetKeyName(13, "PlantTransshipment");
      this.MainImageList.Images.SetKeyName(14, "PlantFlowering");
      this.MainImageList.Images.SetKeyName(15, "SoilReplace");
      this.MainImageList.Images.SetKeyName(16, "ActionPlanting");
      this.MainImageList.Images.SetKeyName(17, "ActionReplanting");
      this.MainImageList.Images.SetKeyName(18, "ActionTransshipment");
      this.MainImageList.Images.SetKeyName(19, "ActionRooting");
      this.MainImageList.Images.SetKeyName(20, "ActionSoilReplace");
      this.MainImageList.Images.SetKeyName(21, "ActionTopCutting");
      this.MainImageList.Images.SetKeyName(22, "ActionTrimming");
      this.MainImageList.Images.SetKeyName(23, "ActionLooseSoil");
      this.MainImageList.Images.SetKeyName(24, "ActionWash");
      this.MainImageList.Images.SetKeyName(25, "ActionAddPlant");
      this.MainImageList.Images.SetKeyName(26, "ActionRemovePlant");
      this.MainImageList.Images.SetKeyName(27, "Care");
      this.MainImageList.Images.SetKeyName(28, "CareRecord");
      this.MainImageList.Images.SetKeyName(29, "PlantSelReport");
      this.MainImageList.Images.SetKeyName(30, "CareReport");
      this.MainImageList.Images.SetKeyName(31, "ReplantingReport");
      this.MainImageList.Images.SetKeyName(32, "PlanReport");
      this.MainImageList.Images.SetKeyName(33, "FloweringReport");
      // 
      // DummyForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(284, 262);
      this.Name = "DummyForm";
      this.Text = "DummyForm";
      this.ResumeLayout(false);

    }

    #endregion

    public System.Windows.Forms.ImageList MainImageList;

  }
}