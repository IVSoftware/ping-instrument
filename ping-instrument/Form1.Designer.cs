namespace ping_instrument
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
            this.checkBoxFlag = new System.Windows.Forms.CheckBox();
            this.richTextConsole = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // checkBoxFlag
            // 
            this.checkBoxFlag.AutoSize = true;
            this.checkBoxFlag.Checked = true;
            this.checkBoxFlag.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxFlag.Location = new System.Drawing.Point(12, 12);
            this.checkBoxFlag.Name = "checkBoxFlag";
            this.checkBoxFlag.Size = new System.Drawing.Size(43, 17);
            this.checkBoxFlag.TabIndex = 0;
            this.checkBoxFlag.Text = "flag";
            this.checkBoxFlag.UseVisualStyleBackColor = true;
            this.checkBoxFlag.CheckedChanged += new System.EventHandler(this.checkBoxFlag_CheckedChanged);
            // 
            // richTextConsole
            // 
            this.richTextConsole.Location = new System.Drawing.Point(70, 8);
            this.richTextConsole.Name = "richTextConsole";
            this.richTextConsole.Size = new System.Drawing.Size(406, 245);
            this.richTextConsole.TabIndex = 1;
            this.richTextConsole.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 261);
            this.Controls.Add(this.richTextConsole);
            this.Controls.Add(this.checkBoxFlag);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxFlag;
        private System.Windows.Forms.RichTextBox richTextConsole;
    }
}

