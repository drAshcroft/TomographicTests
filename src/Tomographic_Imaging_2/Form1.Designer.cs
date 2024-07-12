namespace Tomographic_Imaging_2
{
    partial class Form1
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
            this.zg1 = new ZedGraph.ZedGraphControl();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.bCreatePhantom = new System.Windows.Forms.Button();
            this.nSlices = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.bDoProjections = new System.Windows.Forms.Button();
            this.bBackProjection = new System.Windows.Forms.Button();
            this.bDo3DBackProjection = new System.Windows.Forms.Button();
            this.bDo3dProjections = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.b3DPhantom = new System.Windows.Forms.Button();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.bDo1Projection = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.b3DDisplay = new System.Windows.Forms.Button();
            this.hAngle = new System.Windows.Forms.HScrollBar();
            this.hScrollBar2 = new System.Windows.Forms.HScrollBar();
            this.hScrollBar3 = new System.Windows.Forms.HScrollBar();
            this.button2 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nSlices)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // zg1
            // 
            this.zg1.EditButtons = System.Windows.Forms.MouseButtons.Left;
            this.zg1.Location = new System.Drawing.Point(675, 45);
            this.zg1.Name = "zg1";
            this.zg1.PanModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)));
            this.zg1.ScrollGrace = 0D;
            this.zg1.ScrollMaxX = 0D;
            this.zg1.ScrollMaxY = 0D;
            this.zg1.ScrollMaxY2 = 0D;
            this.zg1.ScrollMinX = 0D;
            this.zg1.ScrollMinY = 0D;
            this.zg1.ScrollMinY2 = 0D;
            this.zg1.Size = new System.Drawing.Size(527, 300);
            this.zg1.TabIndex = 7;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(657, 600);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            // 
            // bCreatePhantom
            // 
            this.bCreatePhantom.Location = new System.Drawing.Point(6, 36);
            this.bCreatePhantom.Name = "bCreatePhantom";
            this.bCreatePhantom.Size = new System.Drawing.Size(123, 40);
            this.bCreatePhantom.TabIndex = 9;
            this.bCreatePhantom.Text = "Create Phantom";
            this.bCreatePhantom.UseVisualStyleBackColor = true;
            this.bCreatePhantom.Click += new System.EventHandler(this.bCreatePhantom_Click);
            // 
            // nSlices
            // 
            this.nSlices.Location = new System.Drawing.Point(6, 104);
            this.nSlices.Name = "nSlices";
            this.nSlices.Size = new System.Drawing.Size(123, 20);
            this.nSlices.TabIndex = 10;
            this.nSlices.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Number Of Slices";
            // 
            // bDoProjections
            // 
            this.bDoProjections.Location = new System.Drawing.Point(6, 130);
            this.bDoProjections.Name = "bDoProjections";
            this.bDoProjections.Size = new System.Drawing.Size(123, 35);
            this.bDoProjections.TabIndex = 12;
            this.bDoProjections.Text = "Do Projections";
            this.bDoProjections.UseVisualStyleBackColor = true;
            this.bDoProjections.Click += new System.EventHandler(this.bDoProjections_Click);
            // 
            // bBackProjection
            // 
            this.bBackProjection.Location = new System.Drawing.Point(6, 185);
            this.bBackProjection.Name = "bBackProjection";
            this.bBackProjection.Size = new System.Drawing.Size(123, 39);
            this.bBackProjection.TabIndex = 13;
            this.bBackProjection.Text = "Do BackProjection";
            this.bBackProjection.UseVisualStyleBackColor = true;
            this.bBackProjection.Click += new System.EventHandler(this.bBackProjection_Click);
            // 
            // bDo3DBackProjection
            // 
            this.bDo3DBackProjection.Location = new System.Drawing.Point(6, 167);
            this.bDo3DBackProjection.Name = "bDo3DBackProjection";
            this.bDo3DBackProjection.Size = new System.Drawing.Size(123, 39);
            this.bDo3DBackProjection.TabIndex = 20;
            this.bDo3DBackProjection.Text = "Do BackProjection";
            this.bDo3DBackProjection.UseVisualStyleBackColor = true;
            this.bDo3DBackProjection.Click += new System.EventHandler(this.bDo3DBackProjection_Click);
            // 
            // bDo3dProjections
            // 
            this.bDo3dProjections.Location = new System.Drawing.Point(6, 126);
            this.bDo3dProjections.Name = "bDo3dProjections";
            this.bDo3dProjections.Size = new System.Drawing.Size(123, 35);
            this.bDo3dProjections.TabIndex = 19;
            this.bDo3dProjections.Text = "Do Projections";
            this.bDo3dProjections.UseVisualStyleBackColor = true;
            this.bDo3dProjections.Click += new System.EventHandler(this.bDo3dProjections_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Number Of Slices";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(6, 100);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(123, 20);
            this.numericUpDown1.TabIndex = 17;
            this.numericUpDown1.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // b3DPhantom
            // 
            this.b3DPhantom.Location = new System.Drawing.Point(6, 32);
            this.b3DPhantom.Name = "b3DPhantom";
            this.b3DPhantom.Size = new System.Drawing.Size(123, 40);
            this.b3DPhantom.TabIndex = 16;
            this.b3DPhantom.Text = "Create 3D Phantom";
            this.b3DPhantom.UseVisualStyleBackColor = true;
            this.b3DPhantom.Click += new System.EventHandler(this.b3DPhantom_Click);
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Enabled = false;
            this.hScrollBar1.Location = new System.Drawing.Point(13, 630);
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(644, 28);
            this.hScrollBar1.TabIndex = 21;
            this.hScrollBar1.Value = 50;
            this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar1_Scroll);
            // 
            // bDo1Projection
            // 
            this.bDo1Projection.Location = new System.Drawing.Point(135, 126);
            this.bDo1Projection.Name = "bDo1Projection";
            this.bDo1Projection.Size = new System.Drawing.Size(123, 35);
            this.bDo1Projection.TabIndex = 22;
            this.bDo1Projection.Text = "Do One Projection";
            this.bDo1Projection.UseVisualStyleBackColor = true;
            this.bDo1Projection.Click += new System.EventHandler(this.bDo1Projection_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(135, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 13);
            this.label3.TabIndex = 23;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(156, 198);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(111, 38);
            this.button1.TabIndex = 24;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // b3DDisplay
            // 
            this.b3DDisplay.Location = new System.Drawing.Point(135, 32);
            this.b3DDisplay.Name = "b3DDisplay";
            this.b3DDisplay.Size = new System.Drawing.Size(123, 40);
            this.b3DDisplay.TabIndex = 25;
            this.b3DDisplay.Text = "Create 3D Phantom 3D Display";
            this.b3DDisplay.UseVisualStyleBackColor = true;
            this.b3DDisplay.Click += new System.EventHandler(this.b3DDisplay_Click);
            // 
            // hAngle
            // 
            this.hAngle.Enabled = false;
            this.hAngle.Location = new System.Drawing.Point(13, 706);
            this.hAngle.Name = "hAngle";
            this.hAngle.Size = new System.Drawing.Size(644, 28);
            this.hAngle.TabIndex = 26;
            this.hAngle.Value = 50;
            this.hAngle.ValueChanged += new System.EventHandler(this.hAngle_ValueChanged);
            // 
            // hScrollBar2
            // 
            this.hScrollBar2.Enabled = false;
            this.hScrollBar2.Location = new System.Drawing.Point(13, 762);
            this.hScrollBar2.Name = "hScrollBar2";
            this.hScrollBar2.Size = new System.Drawing.Size(644, 28);
            this.hScrollBar2.TabIndex = 27;
            this.hScrollBar2.Value = 50;
            this.hScrollBar2.ValueChanged += new System.EventHandler(this.hScrollBar2_ValueChanged);
            // 
            // hScrollBar3
            // 
            this.hScrollBar3.Enabled = false;
            this.hScrollBar3.Location = new System.Drawing.Point(13, 734);
            this.hScrollBar3.Name = "hScrollBar3";
            this.hScrollBar3.Size = new System.Drawing.Size(644, 28);
            this.hScrollBar3.TabIndex = 28;
            this.hScrollBar3.Value = 50;
            this.hScrollBar3.ValueChanged += new System.EventHandler(this.hScrollBar3_ValueChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(6, 236);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(123, 38);
            this.button2.TabIndex = 29;
            this.button2.Text = "Create Mesh";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 693);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 30;
            this.label4.Text = "Angles";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 617);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 13);
            this.label5.TabIndex = 31;
            this.label5.Text = "Slice";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(688, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 13);
            this.label6.TabIndex = 32;
            this.label6.Text = "2D Projections";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.bCreatePhantom);
            this.groupBox1.Controls.Add(this.nSlices);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.bDoProjections);
            this.groupBox1.Controls.Add(this.bBackProjection);
            this.groupBox1.Location = new System.Drawing.Point(675, 363);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(152, 295);
            this.groupBox1.TabIndex = 33;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "2D Phantom";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.b3DPhantom);
            this.groupBox2.Controls.Add(this.numericUpDown1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.bDo3dProjections);
            this.groupBox2.Controls.Add(this.bDo3DBackProjection);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.bDo1Projection);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.b3DDisplay);
            this.groupBox2.Location = new System.Drawing.Point(855, 363);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(307, 316);
            this.groupBox2.TabIndex = 34;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "3D Phantom";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1214, 811);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.hScrollBar3);
            this.Controls.Add(this.hScrollBar2);
            this.Controls.Add(this.hAngle);
            this.Controls.Add(this.hScrollBar1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.zg1);
            this.Name = "Form1";
            this.Text = "Tomographic Trial Desk";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nSlices)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZedGraph.ZedGraphControl zg1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button bCreatePhantom;
        private System.Windows.Forms.NumericUpDown nSlices;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bDoProjections;
        private System.Windows.Forms.Button bBackProjection;
        private System.Windows.Forms.Button bDo3DBackProjection;
        private System.Windows.Forms.Button bDo3dProjections;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button b3DPhantom;
        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.Button bDo1Projection;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button b3DDisplay;
        private System.Windows.Forms.HScrollBar hAngle;
        private System.Windows.Forms.HScrollBar hScrollBar2;
        private System.Windows.Forms.HScrollBar hScrollBar3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}

