namespace KooliProjekt.WindowsForms
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dataGridView1 = new DataGridView();
            idTextBox = new TextBox();
            numberPlateTextBox = new TextBox();
            typeIdTextBox = new TextBox();
            kmRateTextBox = new TextBox();
            timeRateTextBox = new TextBox();
            isAvailableCheckBox = new CheckBox();
            addButton = new Button();
            saveButton = new Button();
            deleteButton = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(12, 12);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.Size = new Size(860, 260);
            dataGridView1.TabIndex = 0;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            // 
            // idTextBox
            // 
            idTextBox.Location = new Point(80, 292);
            idTextBox.Name = "idTextBox";
            idTextBox.ReadOnly = true;
            idTextBox.Size = new Size(100, 27);
            idTextBox.TabIndex = 1;
            // 
            // numberPlateTextBox
            // 
            numberPlateTextBox.Location = new Point(280, 292);
            numberPlateTextBox.Name = "numberPlateTextBox";
            numberPlateTextBox.Size = new Size(160, 27);
            numberPlateTextBox.TabIndex = 2;
            // 
            // typeIdTextBox
            // 
            typeIdTextBox.Location = new Point(540, 292);
            typeIdTextBox.Name = "typeIdTextBox";
            typeIdTextBox.Size = new Size(100, 27);
            typeIdTextBox.TabIndex = 3;
            // 
            // kmRateTextBox
            // 
            kmRateTextBox.Location = new Point(80, 342);
            kmRateTextBox.Name = "kmRateTextBox";
            kmRateTextBox.Size = new Size(120, 27);
            kmRateTextBox.TabIndex = 4;
            // 
            // timeRateTextBox
            // 
            timeRateTextBox.Location = new Point(280, 342);
            timeRateTextBox.Name = "timeRateTextBox";
            timeRateTextBox.Size = new Size(120, 27);
            timeRateTextBox.TabIndex = 5;
            // 
            // isAvailableCheckBox
            // 
            isAvailableCheckBox.AutoSize = true;
            isAvailableCheckBox.Location = new Point(460, 344);
            isAvailableCheckBox.Name = "isAvailableCheckBox";
            isAvailableCheckBox.Size = new Size(111, 24);
            isAvailableCheckBox.TabIndex = 6;
            isAvailableCheckBox.Text = "Is available";
            isAvailableCheckBox.UseVisualStyleBackColor = true;
            // 
            // Add button
            // 
            addButton.Location = new Point(12, 410);
            addButton.Name = "addButton";
            addButton.Size = new Size(110, 35);
            addButton.TabIndex = 7;
            addButton.Text = "Add new";
            addButton.UseVisualStyleBackColor = true;
            addButton.Click += addButton_Click;
            // 
            // Save button
            // 
            saveButton.Location = new Point(140, 410);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(110, 35);
            saveButton.TabIndex = 8;
            saveButton.Text = "Save";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            // 
            // Delete button
            // 
            deleteButton.Location = new Point(268, 410);
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new Size(110, 35);
            deleteButton.TabIndex = 9;
            deleteButton.Text = "Delete";
            deleteButton.UseVisualStyleBackColor = true;
            deleteButton.Click += deleteButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(884, 470);
            Controls.Add(new Label { AutoSize = true, Location = new Point(12, 295), Text = "Id" });
            Controls.Add(new Label { AutoSize = true, Location = new Point(210, 295), Text = "Number plate" });
            Controls.Add(new Label { AutoSize = true, Location = new Point(470, 295), Text = "Type ID" });
            Controls.Add(new Label { AutoSize = true, Location = new Point(12, 345), Text = "Km rate" });
            Controls.Add(new Label { AutoSize = true, Location = new Point(210, 345), Text = "Time rate" });
            Controls.Add(idTextBox);
            Controls.Add(numberPlateTextBox);
            Controls.Add(typeIdTextBox);
            Controls.Add(kmRateTextBox);
            Controls.Add(timeRateTextBox);
            Controls.Add(isAvailableCheckBox);
            Controls.Add(addButton);
            Controls.Add(saveButton);
            Controls.Add(deleteButton);
            Controls.Add(dataGridView1);
            Name = "Form1";
            Text = "Cars";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridView1;
        private TextBox idTextBox;
        private TextBox numberPlateTextBox;
        private TextBox typeIdTextBox;
        private TextBox kmRateTextBox;
        private TextBox timeRateTextBox;
        private CheckBox isAvailableCheckBox;
        private Button addButton;
        private Button saveButton;
        private Button deleteButton;
    }
}
