namespace AutoComplete
{
    partial class frm_autoComplete
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
            this.txtInput = new System.Windows.Forms.TextBox();
            this.ltbSuggestions = new System.Windows.Forms.ListBox();
            this.btnClearIndexes = new System.Windows.Forms.Button();
            this.btnTxtProcessing = new System.Windows.Forms.Button();
            this.btnShowFullText = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnShowAllSuggestions = new System.Windows.Forms.Button();
            this.btnStopIndexing = new System.Windows.Forms.Button();
            this.lblProgress = new System.Windows.Forms.Label();
            this.btnSaveIndexesToDisk = new System.Windows.Forms.Button();
            this.lbl_typed = new System.Windows.Forms.Label();
            this.lbl_total = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtInput
            // 
            this.txtInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInput.Location = new System.Drawing.Point(54, 39);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(407, 31);
            this.txtInput.TabIndex = 0;
            this.txtInput.TextChanged += new System.EventHandler(this.txtInput_TextChanged);
            this.txtInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtInput_KeyPress);
            this.txtInput.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtInput_KeyUp);
            // 
            // ltbSuggestions
            // 
            this.ltbSuggestions.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ltbSuggestions.FormattingEnabled = true;
            this.ltbSuggestions.ItemHeight = 25;
            this.ltbSuggestions.Location = new System.Drawing.Point(74, 171);
            this.ltbSuggestions.Name = "ltbSuggestions";
            this.ltbSuggestions.Size = new System.Drawing.Size(365, 254);
            this.ltbSuggestions.TabIndex = 1;
            // 
            // btnClearIndexes
            // 
            this.btnClearIndexes.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnClearIndexes.Location = new System.Drawing.Point(26, 89);
            this.btnClearIndexes.Name = "btnClearIndexes";
            this.btnClearIndexes.Size = new System.Drawing.Size(110, 23);
            this.btnClearIndexes.TabIndex = 2;
            this.btnClearIndexes.Text = "Limpar Indexes";
            this.btnClearIndexes.UseVisualStyleBackColor = true;
            this.btnClearIndexes.Click += new System.EventHandler(this.btnClearIndexes_Click);
            // 
            // btnTxtProcessing
            // 
            this.btnTxtProcessing.Location = new System.Drawing.Point(179, 89);
            this.btnTxtProcessing.Name = "btnTxtProcessing";
            this.btnTxtProcessing.Size = new System.Drawing.Size(110, 23);
            this.btnTxtProcessing.TabIndex = 3;
            this.btnTxtProcessing.Text = "Processar Txt";
            this.btnTxtProcessing.UseVisualStyleBackColor = true;
            this.btnTxtProcessing.Click += new System.EventHandler(this.btnTxtProcessing_Click);
            // 
            // btnShowFullText
            // 
            this.btnShowFullText.Location = new System.Drawing.Point(26, 129);
            this.btnShowFullText.Name = "btnShowFullText";
            this.btnShowFullText.Size = new System.Drawing.Size(168, 23);
            this.btnShowFullText.TabIndex = 4;
            this.btnShowFullText.Text = "Mostrar Texto Completo";
            this.btnShowFullText.UseVisualStyleBackColor = true;
            this.btnShowFullText.Click += new System.EventHandler(this.btnShowFullText_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(51, 446);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(238, 18);
            this.label1.TabIndex = 6;
            this.label1.Text = "Indexando texto, por favor aguarde!";
            // 
            // btnShowAllSuggestions
            // 
            this.btnShowAllSuggestions.Location = new System.Drawing.Point(325, 89);
            this.btnShowAllSuggestions.Name = "btnShowAllSuggestions";
            this.btnShowAllSuggestions.Size = new System.Drawing.Size(157, 23);
            this.btnShowAllSuggestions.TabIndex = 8;
            this.btnShowAllSuggestions.Text = "Mostrar banco de palavras";
            this.btnShowAllSuggestions.UseVisualStyleBackColor = true;
            this.btnShowAllSuggestions.Click += new System.EventHandler(this.btnShowAllSuggestions_Click);
            // 
            // btnStopIndexing
            // 
            this.btnStopIndexing.Location = new System.Drawing.Point(218, 129);
            this.btnStopIndexing.Name = "btnStopIndexing";
            this.btnStopIndexing.Size = new System.Drawing.Size(110, 23);
            this.btnStopIndexing.TabIndex = 9;
            this.btnStopIndexing.Text = "Parar de Indexar";
            this.btnStopIndexing.UseVisualStyleBackColor = true;
            this.btnStopIndexing.Click += new System.EventHandler(this.btnStopIndexing_Click);
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(51, 502);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(0, 13);
            this.lblProgress.TabIndex = 10;
            // 
            // btnSaveIndexesToDisk
            // 
            this.btnSaveIndexesToDisk.Location = new System.Drawing.Point(355, 130);
            this.btnSaveIndexesToDisk.Name = "btnSaveIndexesToDisk";
            this.btnSaveIndexesToDisk.Size = new System.Drawing.Size(127, 22);
            this.btnSaveIndexesToDisk.TabIndex = 11;
            this.btnSaveIndexesToDisk.Text = "Salvar em Disco";
            this.btnSaveIndexesToDisk.UseVisualStyleBackColor = true;
            this.btnSaveIndexesToDisk.Click += new System.EventHandler(this.btnSaveIndexesToDisk_Click);
            // 
            // lbl_typed
            // 
            this.lbl_typed.AutoSize = true;
            this.lbl_typed.Location = new System.Drawing.Point(54, 20);
            this.lbl_typed.Name = "lbl_typed";
            this.lbl_typed.Size = new System.Drawing.Size(35, 13);
            this.lbl_typed.TabIndex = 12;
            this.lbl_typed.Text = "label2";
            // 
            // lbl_total
            // 
            this.lbl_total.AutoSize = true;
            this.lbl_total.Location = new System.Drawing.Point(230, 20);
            this.lbl_total.Name = "lbl_total";
            this.lbl_total.Size = new System.Drawing.Size(35, 13);
            this.lbl_total.TabIndex = 13;
            this.lbl_total.Text = "label3";
            this.lbl_total.Click += new System.EventHandler(this.label3_Click);
            // 
            // frm_autoComplete
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 543);
            this.Controls.Add(this.lbl_total);
            this.Controls.Add(this.lbl_typed);
            this.Controls.Add(this.btnSaveIndexesToDisk);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.btnStopIndexing);
            this.Controls.Add(this.btnShowAllSuggestions);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnShowFullText);
            this.Controls.Add(this.btnTxtProcessing);
            this.Controls.Add(this.btnClearIndexes);
            this.Controls.Add(this.ltbSuggestions);
            this.Controls.Add(this.txtInput);
            this.Name = "frm_autoComplete";
            this.Text = "AutoComplete";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frm_autoComplete_FormClosed);
            this.Load += new System.EventHandler(this.frmAutoComplete_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.ListBox ltbSuggestions;
        private System.Windows.Forms.Button btnClearIndexes;
        private System.Windows.Forms.Button btnTxtProcessing;
        private System.Windows.Forms.Button btnShowFullText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnShowAllSuggestions;
        private System.Windows.Forms.Button btnStopIndexing;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Button btnSaveIndexesToDisk;
        private System.Windows.Forms.Label lbl_typed;
        private System.Windows.Forms.Label lbl_total;
    }
}

