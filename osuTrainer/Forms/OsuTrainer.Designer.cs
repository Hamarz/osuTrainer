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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
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
            this.BeatmapIdLbl = new System.Windows.Forms.Label();
            this.ChangerUserButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.MinppTextbox = new System.Windows.Forms.TextBox();
            this.UpdateButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
            this.dataGridView1.Location = new System.Drawing.Point(12, 159);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(721, 217);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            this.dataGridView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseClick);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 143);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Recommended Scores:";
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
            this.groupBox1.Controls.Add(this.BeatmapIdLbl);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(379, 124);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Beatmap Info";
            // 
            // BpmLbl
            // 
            this.BpmLbl.AutoEllipsis = true;
            this.BpmLbl.Location = new System.Drawing.Point(321, 78);
            this.BpmLbl.Name = "BpmLbl";
            this.BpmLbl.Size = new System.Drawing.Size(46, 13);
            this.BpmLbl.TabIndex = 15;
            // 
            // DrainingTimeLbl
            // 
            this.DrainingTimeLbl.AutoEllipsis = true;
            this.DrainingTimeLbl.Location = new System.Drawing.Point(321, 53);
            this.DrainingTimeLbl.Name = "DrainingTimeLbl";
            this.DrainingTimeLbl.Size = new System.Drawing.Size(46, 13);
            this.DrainingTimeLbl.TabIndex = 14;
            // 
            // TotalTimeLbl
            // 
            this.TotalTimeLbl.AutoEllipsis = true;
            this.TotalTimeLbl.Location = new System.Drawing.Point(321, 28);
            this.TotalTimeLbl.Name = "TotalTimeLbl";
            this.TotalTimeLbl.Size = new System.Drawing.Size(46, 13);
            this.TotalTimeLbl.TabIndex = 13;
            // 
            // CreatorLbl
            // 
            this.CreatorLbl.AutoEllipsis = true;
            this.CreatorLbl.Location = new System.Drawing.Point(55, 78);
            this.CreatorLbl.Name = "CreatorLbl";
            this.CreatorLbl.Size = new System.Drawing.Size(179, 13);
            this.CreatorLbl.TabIndex = 11;
            // 
            // TitleLbl
            // 
            this.TitleLbl.AutoEllipsis = true;
            this.TitleLbl.Location = new System.Drawing.Point(55, 53);
            this.TitleLbl.Name = "TitleLbl";
            this.TitleLbl.Size = new System.Drawing.Size(179, 13);
            this.TitleLbl.TabIndex = 10;
            // 
            // ArtistLbl
            // 
            this.ArtistLbl.AutoEllipsis = true;
            this.ArtistLbl.Location = new System.Drawing.Point(55, 28);
            this.ArtistLbl.Name = "ArtistLbl";
            this.ArtistLbl.Size = new System.Drawing.Size(179, 13);
            this.ArtistLbl.TabIndex = 9;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(282, 78);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(33, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "BPM:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(240, 53);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Draining Time:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(255, 28);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Total Time:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Artist:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Title:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Creator:";
            // 
            // BeatmapIdLbl
            // 
            this.BeatmapIdLbl.AutoSize = true;
            this.BeatmapIdLbl.Location = new System.Drawing.Point(321, 103);
            this.BeatmapIdLbl.Name = "BeatmapIdLbl";
            this.BeatmapIdLbl.Size = new System.Drawing.Size(34, 13);
            this.BeatmapIdLbl.TabIndex = 1;
            this.BeatmapIdLbl.Text = "-1234";
            this.BeatmapIdLbl.Visible = false;
            // 
            // ChangerUserButton
            // 
            this.ChangerUserButton.Location = new System.Drawing.Point(572, 16);
            this.ChangerUserButton.Name = "ChangerUserButton";
            this.ChangerUserButton.Size = new System.Drawing.Size(161, 33);
            this.ChangerUserButton.TabIndex = 3;
            this.ChangerUserButton.Text = "Change User";
            this.ChangerUserButton.UseVisualStyleBackColor = true;
            this.ChangerUserButton.Click += new System.EventHandler(this.ChangeUserButton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(397, 16);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(160, 120);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // MinppTextbox
            // 
            this.MinppTextbox.Location = new System.Drawing.Point(572, 79);
            this.MinppTextbox.Name = "MinppTextbox";
            this.MinppTextbox.Size = new System.Drawing.Size(100, 20);
            this.MinppTextbox.TabIndex = 5;
            this.MinppTextbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MinppTextbox_KeyPress);
            // 
            // UpdateButton
            // 
            this.UpdateButton.Location = new System.Drawing.Point(572, 105);
            this.UpdateButton.Name = "UpdateButton";
            this.UpdateButton.Size = new System.Drawing.Size(161, 33);
            this.UpdateButton.TabIndex = 6;
            this.UpdateButton.Text = "Update";
            this.UpdateButton.UseVisualStyleBackColor = true;
            this.UpdateButton.Click += new System.EventHandler(this.UpdateButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(678, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Min. PP";
            // 
            // OsuTrainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(745, 388);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.UpdateButton);
            this.Controls.Add(this.MinppTextbox);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.ChangerUserButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView1);
            this.MaximizeBox = false;
            this.Name = "OsuTrainer";
            this.Text = "osu! Trainer";
            this.Load += new System.EventHandler(this.OsuTrainer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button ChangerUserButton;
        private System.Windows.Forms.Label BeatmapIdLbl;
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
        private System.Windows.Forms.TextBox MinppTextbox;
        private System.Windows.Forms.Button UpdateButton;
        private System.Windows.Forms.Label label2;
    }
}