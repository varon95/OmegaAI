namespace WindowsFormsApp5
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.startTheGame = new System.Windows.Forms.Button();
            this.radioButtonWhite = new System.Windows.Forms.RadioButton();
            this.radioButtonBlack = new System.Windows.Forms.RadioButton();
            this.nNumber = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.whiteScore = new System.Windows.Forms.Label();
            this.blackScore = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.twoPlayers = new System.Windows.Forms.RadioButton();
            this.blackAI = new System.Windows.Forms.RadioButton();
            this.whiteAI = new System.Windows.Forms.RadioButton();
            this.undoButton = new System.Windows.Forms.Button();
            this.stepsLabel = new System.Windows.Forms.Label();
            this.stepsLeftLabel = new System.Windows.Forms.Label();
            this.loadButton = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AllowDrop = true;
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.AutoSize = true;
            this.panel1.Location = new System.Drawing.Point(12, 68);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(773, 454);
            this.panel1.TabIndex = 1;
            // 
            // startTheGame
            // 
            this.startTheGame.Location = new System.Drawing.Point(392, 3);
            this.startTheGame.Name = "startTheGame";
            this.startTheGame.Size = new System.Drawing.Size(58, 27);
            this.startTheGame.TabIndex = 2;
            this.startTheGame.Text = "Go!";
            this.startTheGame.UseVisualStyleBackColor = true;
            this.startTheGame.Click += new System.EventHandler(this.startTheGame_Click);
            // 
            // radioButtonWhite
            // 
            this.radioButtonWhite.AutoSize = true;
            this.radioButtonWhite.Enabled = false;
            this.radioButtonWhite.Location = new System.Drawing.Point(456, 9);
            this.radioButtonWhite.Name = "radioButtonWhite";
            this.radioButtonWhite.Size = new System.Drawing.Size(81, 17);
            this.radioButtonWhite.TabIndex = 3;
            this.radioButtonWhite.TabStop = true;
            this.radioButtonWhite.Text = "White\'s turn";
            this.radioButtonWhite.UseVisualStyleBackColor = true;
            // 
            // radioButtonBlack
            // 
            this.radioButtonBlack.AutoSize = true;
            this.radioButtonBlack.Enabled = false;
            this.radioButtonBlack.Location = new System.Drawing.Point(457, 40);
            this.radioButtonBlack.Name = "radioButtonBlack";
            this.radioButtonBlack.Size = new System.Drawing.Size(80, 17);
            this.radioButtonBlack.TabIndex = 4;
            this.radioButtonBlack.TabStop = true;
            this.radioButtonBlack.Text = "Black\'s turn";
            this.radioButtonBlack.UseVisualStyleBackColor = true;
            // 
            // nNumber
            // 
            this.nNumber.Location = new System.Drawing.Point(253, 20);
            this.nNumber.Name = "nNumber";
            this.nNumber.Size = new System.Drawing.Size(41, 20);
            this.nNumber.TabIndex = 5;
            this.nNumber.Text = "5";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(14, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(232, 30);
            this.label1.TabIndex = 6;
            this.label1.Text = "To start, indicate the number of hexagons\r\n in one side, and press \"Go!\"";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(563, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "white\'s score:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(563, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "black\'s score:";
            // 
            // whiteScore
            // 
            this.whiteScore.AutoSize = true;
            this.whiteScore.Location = new System.Drawing.Point(641, 11);
            this.whiteScore.Name = "whiteScore";
            this.whiteScore.Size = new System.Drawing.Size(10, 13);
            this.whiteScore.TabIndex = 9;
            this.whiteScore.Text = "-";
            // 
            // blackScore
            // 
            this.blackScore.AutoSize = true;
            this.blackScore.Location = new System.Drawing.Point(641, 36);
            this.blackScore.Name = "blackScore";
            this.blackScore.Size = new System.Drawing.Size(10, 13);
            this.blackScore.TabIndex = 10;
            this.blackScore.Text = "-";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.twoPlayers);
            this.panel2.Controls.Add(this.blackAI);
            this.panel2.Controls.Add(this.whiteAI);
            this.panel2.Location = new System.Drawing.Point(300, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(86, 63);
            this.panel2.TabIndex = 11;
            // 
            // twoPlayers
            // 
            this.twoPlayers.AutoSize = true;
            this.twoPlayers.Checked = true;
            this.twoPlayers.Location = new System.Drawing.Point(13, 44);
            this.twoPlayers.Name = "twoPlayers";
            this.twoPlayers.Size = new System.Drawing.Size(67, 17);
            this.twoPlayers.TabIndex = 16;
            this.twoPlayers.TabStop = true;
            this.twoPlayers.Text = "2 players";
            this.twoPlayers.UseVisualStyleBackColor = true;
            // 
            // blackAI
            // 
            this.blackAI.AutoSize = true;
            this.blackAI.Location = new System.Drawing.Point(13, 21);
            this.blackAI.Name = "blackAI";
            this.blackAI.Size = new System.Drawing.Size(64, 17);
            this.blackAI.TabIndex = 15;
            this.blackAI.Text = "black AI";
            this.blackAI.UseVisualStyleBackColor = true;
            // 
            // whiteAI
            // 
            this.whiteAI.AutoSize = true;
            this.whiteAI.Location = new System.Drawing.Point(13, -2);
            this.whiteAI.Name = "whiteAI";
            this.whiteAI.Size = new System.Drawing.Size(63, 17);
            this.whiteAI.TabIndex = 14;
            this.whiteAI.Text = "white AI";
            this.whiteAI.UseVisualStyleBackColor = true;
            // 
            // undoButton
            // 
            this.undoButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.undoButton.Location = new System.Drawing.Point(701, 3);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(82, 31);
            this.undoButton.TabIndex = 12;
            this.undoButton.Text = "Undo";
            this.undoButton.UseVisualStyleBackColor = false;
            this.undoButton.Click += new System.EventHandler(this.undoButton_Click);
            // 
            // stepsLabel
            // 
            this.stepsLabel.AutoSize = true;
            this.stepsLabel.Location = new System.Drawing.Point(698, 42);
            this.stepsLabel.Name = "stepsLabel";
            this.stepsLabel.Size = new System.Drawing.Size(34, 13);
            this.stepsLabel.TabIndex = 13;
            this.stepsLabel.Text = "Steps";
            // 
            // stepsLeftLabel
            // 
            this.stepsLeftLabel.AutoSize = true;
            this.stepsLeftLabel.Location = new System.Drawing.Point(748, 42);
            this.stepsLeftLabel.Name = "stepsLeftLabel";
            this.stepsLeftLabel.Size = new System.Drawing.Size(45, 13);
            this.stepsLeftLabel.TabIndex = 14;
            this.stepsLeftLabel.Text = "Stps left";
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(393, 36);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(56, 28);
            this.loadButton.TabIndex = 15;
            this.loadButton.Text = "Load";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(796, 534);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.stepsLeftLabel);
            this.Controls.Add(this.stepsLabel);
            this.Controls.Add(this.undoButton);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.blackScore);
            this.Controls.Add(this.whiteScore);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nNumber);
            this.Controls.Add(this.radioButtonBlack);
            this.Controls.Add(this.radioButtonWhite);
            this.Controls.Add(this.startTheGame);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "OMEGA";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button startTheGame;
        private System.Windows.Forms.RadioButton radioButtonWhite;
        private System.Windows.Forms.RadioButton radioButtonBlack;
        private System.Windows.Forms.TextBox nNumber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label whiteScore;
        private System.Windows.Forms.Label blackScore;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton twoPlayers;
        private System.Windows.Forms.RadioButton blackAI;
        private System.Windows.Forms.RadioButton whiteAI;
        private System.Windows.Forms.Button undoButton;
        private System.Windows.Forms.Label stepsLabel;
        private System.Windows.Forms.Label stepsLeftLabel;
        private System.Windows.Forms.Button loadButton;
    }
}

