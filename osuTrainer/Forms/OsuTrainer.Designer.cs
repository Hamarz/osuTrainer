namespace osuTrainer.Forms
{
    partial class OsuTrainer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OsuTrainer));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BpmLbl = new System.Windows.Forms.Label();
            this.DrainingTimeLbl = new System.Windows.Forms.Label();
            this.TotalTimeLbl = new System.Windows.Forms.Label();
            this.CreatorLbl = new System.Windows.Forms.Label();
            this.TitleLbl = new System.Windows.Forms.Label();
            this.ArtistLbl = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ChangerUserButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.MinPPTB = new System.Windows.Forms.TrackBar();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.MinPPLabel = new System.Windows.Forms.Label();
            this.UpdateButton = new System.Windows.Forms.Button();
            this.DoubletimeCB = new System.Windows.Forms.CheckBox();
            this.HardrockCB = new System.Windows.Forms.CheckBox();
            this.HiddenCB = new System.Windows.Forms.CheckBox();
            this.FlashlightCB = new System.Windows.Forms.CheckBox();
            this.SearchtimeTB = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.progressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.PlayersCheckedLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.ScoresAddedLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.ExclusiveCB = new System.Windows.Forms.CheckBox();
            this.GameModeCB = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MinPPTB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SearchtimeTB)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.EnableHeadersVisualStyles = false;
            this.dataGridView1.Location = new System.Drawing.Point(6, 19);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(626, 266);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            this.dataGridView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BpmLbl);
            this.groupBox1.Controls.Add(this.DrainingTimeLbl);
            this.groupBox1.Controls.Add(this.TotalTimeLbl);
            this.groupBox1.Controls.Add(this.CreatorLbl);
            this.groupBox1.Controls.Add(this.TitleLbl);
            this.groupBox1.Controls.Add(this.ArtistLbl);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(6, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(379, 94);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Beatmap Info";
            // 
            // BpmLbl
            // 
            this.BpmLbl.AutoEllipsis = true;
            this.BpmLbl.Location = new System.Drawing.Point(321, 72);
            this.BpmLbl.Name = "BpmLbl";
            this.BpmLbl.Size = new System.Drawing.Size(46, 13);
            this.BpmLbl.TabIndex = 15;
            // 
            // DrainingTimeLbl
            // 
            this.DrainingTimeLbl.AutoEllipsis = true;
            this.DrainingTimeLbl.Location = new System.Drawing.Point(321, 47);
            this.DrainingTimeLbl.Name = "DrainingTimeLbl";
            this.DrainingTimeLbl.Size = new System.Drawing.Size(46, 13);
            this.DrainingTimeLbl.TabIndex = 14;
            // 
            // TotalTimeLbl
            // 
            this.TotalTimeLbl.AutoEllipsis = true;
            this.TotalTimeLbl.Location = new System.Drawing.Point(321, 22);
            this.TotalTimeLbl.Name = "TotalTimeLbl";
            this.TotalTimeLbl.Size = new System.Drawing.Size(46, 13);
            this.TotalTimeLbl.TabIndex = 13;
            // 
            // CreatorLbl
            // 
            this.CreatorLbl.AutoEllipsis = true;
            this.CreatorLbl.Location = new System.Drawing.Point(55, 72);
            this.CreatorLbl.Name = "CreatorLbl";
            this.CreatorLbl.Size = new System.Drawing.Size(179, 13);
            this.CreatorLbl.TabIndex = 11;
            // 
            // TitleLbl
            // 
            this.TitleLbl.AutoEllipsis = true;
            this.TitleLbl.Location = new System.Drawing.Point(55, 47);
            this.TitleLbl.Name = "TitleLbl";
            this.TitleLbl.Size = new System.Drawing.Size(179, 13);
            this.TitleLbl.TabIndex = 10;
            // 
            // ArtistLbl
            // 
            this.ArtistLbl.AutoEllipsis = true;
            this.ArtistLbl.Location = new System.Drawing.Point(55, 22);
            this.ArtistLbl.Name = "ArtistLbl";
            this.ArtistLbl.Size = new System.Drawing.Size(179, 13);
            this.ArtistLbl.TabIndex = 9;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(282, 72);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(33, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "BPM:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(240, 47);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Draining Time:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(255, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Total Time:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Artist:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Title:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Creator:";
            // 
            // ChangerUserButton
            // 
            this.ChangerUserButton.Location = new System.Drawing.Point(517, 9);
            this.ChangerUserButton.Name = "ChangerUserButton";
            this.ChangerUserButton.Size = new System.Drawing.Size(127, 51);
            this.ChangerUserButton.TabIndex = 3;
            this.ChangerUserButton.Text = "Change User";
            this.ChangerUserButton.UseVisualStyleBackColor = true;
            this.ChangerUserButton.Click += new System.EventHandler(this.ChangeUserButton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(391, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(120, 90);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.dataGridView1);
            this.groupBox2.Location = new System.Drawing.Point(6, 151);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(638, 291);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Recommended Scores";
            // 
            // MinPPTB
            // 
            this.MinPPTB.AutoSize = false;
            this.MinPPTB.Location = new System.Drawing.Point(51, 110);
            this.MinPPTB.Name = "MinPPTB";
            this.MinPPTB.Size = new System.Drawing.Size(149, 35);
            this.MinPPTB.TabIndex = 7;
            this.MinPPTB.Scroll += new System.EventHandler(this.MinPPTB_Scroll);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 100;
            this.toolTip1.ReshowDelay = 50;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Min. PP:";
            // 
            // MinPPLabel
            // 
            this.MinPPLabel.AutoSize = true;
            this.MinPPLabel.Location = new System.Drawing.Point(12, 129);
            this.MinPPLabel.Name = "MinPPLabel";
            this.MinPPLabel.Size = new System.Drawing.Size(35, 13);
            this.MinPPLabel.TabIndex = 9;
            this.MinPPLabel.Text = "label2";
            // 
            // UpdateButton
            // 
            this.UpdateButton.Location = new System.Drawing.Point(202, 110);
            this.UpdateButton.Name = "UpdateButton";
            this.UpdateButton.Size = new System.Drawing.Size(89, 39);
            this.UpdateButton.TabIndex = 10;
            this.UpdateButton.Text = "Update";
            this.UpdateButton.UseVisualStyleBackColor = true;
            this.UpdateButton.Click += new System.EventHandler(this.UpdateButton_Click);
            // 
            // DoubletimeCB
            // 
            this.DoubletimeCB.AutoSize = true;
            this.DoubletimeCB.Location = new System.Drawing.Point(303, 113);
            this.DoubletimeCB.Name = "DoubletimeCB";
            this.DoubletimeCB.Size = new System.Drawing.Size(79, 17);
            this.DoubletimeCB.TabIndex = 11;
            this.DoubletimeCB.Text = "Doubletime";
            this.DoubletimeCB.UseVisualStyleBackColor = true;
            this.DoubletimeCB.CheckedChanged += new System.EventHandler(this.DoubletimeCB_CheckedChanged);
            // 
            // HardrockCB
            // 
            this.HardrockCB.AutoSize = true;
            this.HardrockCB.Location = new System.Drawing.Point(303, 133);
            this.HardrockCB.Name = "HardrockCB";
            this.HardrockCB.Size = new System.Drawing.Size(70, 17);
            this.HardrockCB.TabIndex = 12;
            this.HardrockCB.Text = "Hardrock";
            this.HardrockCB.UseVisualStyleBackColor = true;
            this.HardrockCB.CheckedChanged += new System.EventHandler(this.HardrockCB_CheckedChanged);
            // 
            // HiddenCB
            // 
            this.HiddenCB.AutoSize = true;
            this.HiddenCB.Location = new System.Drawing.Point(378, 113);
            this.HiddenCB.Name = "HiddenCB";
            this.HiddenCB.Size = new System.Drawing.Size(60, 17);
            this.HiddenCB.TabIndex = 13;
            this.HiddenCB.Text = "Hidden";
            this.HiddenCB.UseVisualStyleBackColor = true;
            this.HiddenCB.CheckedChanged += new System.EventHandler(this.HiddenCB_CheckedChanged);
            // 
            // FlashlightCB
            // 
            this.FlashlightCB.AutoSize = true;
            this.FlashlightCB.Location = new System.Drawing.Point(378, 133);
            this.FlashlightCB.Name = "FlashlightCB";
            this.FlashlightCB.Size = new System.Drawing.Size(70, 17);
            this.FlashlightCB.TabIndex = 14;
            this.FlashlightCB.Text = "Flashlight";
            this.FlashlightCB.UseVisualStyleBackColor = true;
            this.FlashlightCB.CheckedChanged += new System.EventHandler(this.FlashlightCB_CheckedChanged);
            // 
            // SearchtimeTB
            // 
            this.SearchtimeTB.AutoSize = false;
            this.SearchtimeTB.Location = new System.Drawing.Point(517, 81);
            this.SearchtimeTB.Maximum = 7;
            this.SearchtimeTB.Minimum = 2;
            this.SearchtimeTB.Name = "SearchtimeTB";
            this.SearchtimeTB.Size = new System.Drawing.Size(126, 28);
            this.SearchtimeTB.TabIndex = 15;
            this.SearchtimeTB.Value = 2;
            this.SearchtimeTB.Scroll += new System.EventHandler(this.SearchtimeTB_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(532, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Search duration (s)";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar1,
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel3,
            this.PlayersCheckedLbl,
            this.toolStripStatusLabel2,
            this.ScoresAddedLbl});
            this.statusStrip1.Location = new System.Drawing.Point(0, 445);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(650, 22);
            this.statusStrip1.TabIndex = 17;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // progressBar1
            // 
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(100, 16);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(333, 17);
            this.toolStripStatusLabel1.Spring = true;
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(94, 17);
            this.toolStripStatusLabel3.Text = "Players checked:";
            // 
            // PlayersCheckedLbl
            // 
            this.PlayersCheckedLbl.Name = "PlayersCheckedLbl";
            this.PlayersCheckedLbl.Size = new System.Drawing.Size(13, 17);
            this.PlayersCheckedLbl.Text = "0";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(80, 17);
            this.toolStripStatusLabel2.Text = "Scores added:";
            // 
            // ScoresAddedLbl
            // 
            this.ScoresAddedLbl.Name = "ScoresAddedLbl";
            this.ScoresAddedLbl.Size = new System.Drawing.Size(13, 17);
            this.ScoresAddedLbl.Text = "0";
            // 
            // ExclusiveCB
            // 
            this.ExclusiveCB.AutoSize = true;
            this.ExclusiveCB.Location = new System.Drawing.Point(444, 133);
            this.ExclusiveCB.Name = "ExclusiveCB";
            this.ExclusiveCB.Size = new System.Drawing.Size(71, 17);
            this.ExclusiveCB.TabIndex = 19;
            this.ExclusiveCB.Text = "Exclusive";
            this.ExclusiveCB.UseVisualStyleBackColor = true;
            // 
            // GameModeCB
            // 
            this.GameModeCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GameModeCB.FormattingEnabled = true;
            this.GameModeCB.Location = new System.Drawing.Point(521, 128);
            this.GameModeCB.Name = "GameModeCB";
            this.GameModeCB.Size = new System.Drawing.Size(117, 21);
            this.GameModeCB.TabIndex = 20;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(520, 113);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 13);
            this.label6.TabIndex = 21;
            this.label6.Text = "Game Mode";
            // 
            // OsuTrainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 467);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.GameModeCB);
            this.Controls.Add(this.ExclusiveCB);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SearchtimeTB);
            this.Controls.Add(this.FlashlightCB);
            this.Controls.Add(this.HiddenCB);
            this.Controls.Add(this.HardrockCB);
            this.Controls.Add(this.DoubletimeCB);
            this.Controls.Add(this.UpdateButton);
            this.Controls.Add(this.MinPPLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MinPPTB);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.ChangerUserButton);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "OsuTrainer";
            this.Load += new System.EventHandler(this.OsuTrainer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MinPPTB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SearchtimeTB)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button ChangerUserButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label ArtistLbl;
        private System.Windows.Forms.Label CreatorLbl;
        private System.Windows.Forms.Label TitleLbl;
        private System.Windows.Forms.Label BpmLbl;
        private System.Windows.Forms.Label DrainingTimeLbl;
        private System.Windows.Forms.Label TotalTimeLbl;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TrackBar MinPPTB;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label MinPPLabel;
        private System.Windows.Forms.Button UpdateButton;
        private System.Windows.Forms.CheckBox DoubletimeCB;
        private System.Windows.Forms.CheckBox HardrockCB;
        private System.Windows.Forms.CheckBox HiddenCB;
        private System.Windows.Forms.CheckBox FlashlightCB;
        private System.Windows.Forms.TrackBar SearchtimeTB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel PlayersCheckedLbl;
        private System.Windows.Forms.ToolStripStatusLabel ScoresAddedLbl;
        private System.Windows.Forms.ToolStripProgressBar progressBar1;
        private System.Windows.Forms.CheckBox ExclusiveCB;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ComboBox GameModeCB;
        private System.Windows.Forms.Label label6;
    }
}