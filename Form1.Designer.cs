namespace Derivative
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.ExampleBox = new System.Windows.Forms.TextBox();
            this.FunctionLabel = new System.Windows.Forms.Label();
            this.AcceptExampleButton = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.DerivativeButton = new System.Windows.Forms.Button();
            this.DerivativeLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ExampleBox
            // 
            this.ExampleBox.Location = new System.Drawing.Point(218, 110);
            this.ExampleBox.Name = "ExampleBox";
            this.ExampleBox.Size = new System.Drawing.Size(213, 20);
            this.ExampleBox.TabIndex = 0;
            this.ExampleBox.TextChanged += new System.EventHandler(this.ExampleBox_TextChanged);
            // 
            // FunctionLabel
            // 
            this.FunctionLabel.AutoSize = true;
            this.FunctionLabel.Location = new System.Drawing.Point(341, 223);
            this.FunctionLabel.Name = "FunctionLabel";
            this.FunctionLabel.Size = new System.Drawing.Size(48, 13);
            this.FunctionLabel.TabIndex = 1;
            this.FunctionLabel.Text = "Function";
            // 
            // AcceptExampleButton
            // 
            this.AcceptExampleButton.Location = new System.Drawing.Point(448, 164);
            this.AcceptExampleButton.Name = "AcceptExampleButton";
            this.AcceptExampleButton.Size = new System.Drawing.Size(75, 23);
            this.AcceptExampleButton.TabIndex = 2;
            this.AcceptExampleButton.Text = "Accept";
            this.AcceptExampleButton.UseVisualStyleBackColor = true;
            this.AcceptExampleButton.Click += new System.EventHandler(this.AcceptExampleButton_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "X"});
            this.comboBox1.Location = new System.Drawing.Point(512, 109);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(50, 21);
            this.comboBox1.TabIndex = 3;
            // 
            // DerivativeButton
            // 
            this.DerivativeButton.Location = new System.Drawing.Point(575, 163);
            this.DerivativeButton.Name = "DerivativeButton";
            this.DerivativeButton.Size = new System.Drawing.Size(75, 23);
            this.DerivativeButton.TabIndex = 4;
            this.DerivativeButton.Text = "Derivative";
            this.DerivativeButton.UseVisualStyleBackColor = true;
            this.DerivativeButton.Click += new System.EventHandler(this.DerivativeButton_Click);
            // 
            // DerivativeLabel
            // 
            this.DerivativeLabel.AutoSize = true;
            this.DerivativeLabel.Location = new System.Drawing.Point(344, 257);
            this.DerivativeLabel.Name = "DerivativeLabel";
            this.DerivativeLabel.Size = new System.Drawing.Size(55, 13);
            this.DerivativeLabel.TabIndex = 5;
            this.DerivativeLabel.Text = "Derivative";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.DerivativeLabel);
            this.Controls.Add(this.DerivativeButton);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.AcceptExampleButton);
            this.Controls.Add(this.FunctionLabel);
            this.Controls.Add(this.ExampleBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ExampleBox;
        private System.Windows.Forms.Label FunctionLabel;
        private System.Windows.Forms.Button AcceptExampleButton;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button DerivativeButton;
        private System.Windows.Forms.Label DerivativeLabel;
    }
}

