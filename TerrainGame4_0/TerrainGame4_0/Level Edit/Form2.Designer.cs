namespace TerrainTutorial
{
    partial class Form2
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
            this.label1 = new System.Windows.Forms.Label();
            this.label_box = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.delay_box = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.levelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.events_list = new System.Windows.Forms.ListBox();
            this.conditionBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.ConditionBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.delay_change = new System.Windows.Forms.Button();
            this.label_change = new System.Windows.Forms.Button();
            this.gameEventTypeBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.event_add = new System.Windows.Forms.Button();
            this.event_remove = new System.Windows.Forms.Button();
            this.event_type = new System.Windows.Forms.TextBox();
            this.type_change = new System.Windows.Forms.Button();
            this.target_change = new System.Windows.Forms.Button();
            this.event_target = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.conditionBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.eventFlagBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gameEventBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.listBox1 = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.levelBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.conditionBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gameEventTypeBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.conditionBindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.eventFlagBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gameEventBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // label_box
            // 
            this.label_box.Location = new System.Drawing.Point(15, 29);
            this.label_box.Name = "label_box";
            this.label_box.ReadOnly = true;
            this.label_box.Size = new System.Drawing.Size(100, 20);
            this.label_box.TabIndex = 1;
            this.label_box.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Label:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Active Corner:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Delay: (milseconds)";
            // 
            // delay_box
            // 
            this.delay_box.Location = new System.Drawing.Point(15, 112);
            this.delay_box.Name = "delay_box";
            this.delay_box.ReadOnly = true;
            this.delay_box.Size = new System.Drawing.Size(100, 20);
            this.delay_box.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 141);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Conditions:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 223);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Events:";
            // 
            // events_list
            // 
            this.events_list.FormattingEnabled = true;
            this.events_list.Items.AddRange(new object[] {
            "kreep",
            "long"});
            this.events_list.Location = new System.Drawing.Point(15, 239);
            this.events_list.Name = "events_list";
            this.events_list.Size = new System.Drawing.Size(118, 56);
            this.events_list.TabIndex = 9;
            this.events_list.SelectedIndexChanged += new System.EventHandler(this.events_list_SelectedIndexChanged);
            // 
            // ConditionBox
            // 
            this.ConditionBox.Location = new System.Drawing.Point(15, 154);
            this.ConditionBox.Multiline = true;
            this.ConditionBox.Name = "ConditionBox";
            this.ConditionBox.ReadOnly = true;
            this.ConditionBox.Size = new System.Drawing.Size(118, 48);
            this.ConditionBox.TabIndex = 10;
            this.ConditionBox.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(139, 154);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(34, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "+";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(139, 183);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(34, 23);
            this.button2.TabIndex = 12;
            this.button2.Text = "-";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // delay_change
            // 
            this.delay_change.Location = new System.Drawing.Point(139, 109);
            this.delay_change.Name = "delay_change";
            this.delay_change.Size = new System.Drawing.Size(34, 23);
            this.delay_change.TabIndex = 13;
            this.delay_change.Text = "+";
            this.delay_change.UseVisualStyleBackColor = true;
            this.delay_change.Click += new System.EventHandler(this.delay_change_Click);
            // 
            // label_change
            // 
            this.label_change.Location = new System.Drawing.Point(139, 26);
            this.label_change.Name = "label_change";
            this.label_change.Size = new System.Drawing.Size(34, 23);
            this.label_change.TabIndex = 14;
            this.label_change.Text = "+";
            this.label_change.UseVisualStyleBackColor = true;
            this.label_change.Click += new System.EventHandler(this.button4_Click);
            // 
            // gameEventTypeBindingSource
            // 
            this.gameEventTypeBindingSource.DataSource = typeof(TerrainTutorial.GameEvent.GameEventType);
            // 
            // event_add
            // 
            this.event_add.Location = new System.Drawing.Point(139, 239);
            this.event_add.Name = "event_add";
            this.event_add.Size = new System.Drawing.Size(34, 23);
            this.event_add.TabIndex = 15;
            this.event_add.Text = "+";
            this.event_add.UseVisualStyleBackColor = true;
            this.event_add.Click += new System.EventHandler(this.event_add_Click);
            // 
            // event_remove
            // 
            this.event_remove.Location = new System.Drawing.Point(139, 268);
            this.event_remove.Name = "event_remove";
            this.event_remove.Size = new System.Drawing.Size(34, 23);
            this.event_remove.TabIndex = 16;
            this.event_remove.Text = "-";
            this.event_remove.UseVisualStyleBackColor = true;
            this.event_remove.Click += new System.EventHandler(this.event_remove_Click);
            // 
            // event_type
            // 
            this.event_type.Location = new System.Drawing.Point(15, 354);
            this.event_type.Name = "event_type";
            this.event_type.ReadOnly = true;
            this.event_type.Size = new System.Drawing.Size(100, 20);
            this.event_type.TabIndex = 17;
            // 
            // type_change
            // 
            this.type_change.Location = new System.Drawing.Point(139, 354);
            this.type_change.Name = "type_change";
            this.type_change.Size = new System.Drawing.Size(34, 23);
            this.type_change.TabIndex = 18;
            this.type_change.Text = "+";
            this.type_change.UseVisualStyleBackColor = true;
            this.type_change.Click += new System.EventHandler(this.type_change_Click);
            // 
            // target_change
            // 
            this.target_change.Location = new System.Drawing.Point(139, 397);
            this.target_change.Name = "target_change";
            this.target_change.Size = new System.Drawing.Size(34, 23);
            this.target_change.TabIndex = 20;
            this.target_change.Text = "+";
            this.target_change.UseVisualStyleBackColor = true;
            this.target_change.Click += new System.EventHandler(this.target_change_Click);
            // 
            // event_target
            // 
            this.event_target.Location = new System.Drawing.Point(15, 397);
            this.event_target.Name = "event_target";
            this.event_target.ReadOnly = true;
            this.event_target.Size = new System.Drawing.Size(100, 20);
            this.event_target.TabIndex = 19;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 338);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "Type:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 381);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "Target:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 208);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(121, 13);
            this.label9.TabIndex = 23;
            this.label9.Text = "--------------------------------------";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(15, 316);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(121, 13);
            this.label10.TabIndex = 24;
            this.label10.Text = "--------------------------------------";
            // 
            // conditionBindingSource1
            // 
            this.conditionBindingSource1.DataSource = typeof(TerrainTutorial.Condition);
            // 
            // eventFlagBindingSource
            // 
            this.eventFlagBindingSource.DataSource = typeof(TerrainTutorial.EventFlag);
            // 
            // gameEventBindingSource
            // 
            this.gameEventBindingSource.DataSource = typeof(TerrainTutorial.GameEvent);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Items.AddRange(new object[] {
            "kreep",
            "long"});
            this.listBox1.Location = new System.Drawing.Point(15, 423);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(118, 56);
            this.listBox1.TabIndex = 25;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(186, 502);
            this.ControlBox = false;
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.target_change);
            this.Controls.Add(this.event_target);
            this.Controls.Add(this.type_change);
            this.Controls.Add(this.event_type);
            this.Controls.Add(this.event_remove);
            this.Controls.Add(this.event_add);
            this.Controls.Add(this.label_change);
            this.Controls.Add(this.delay_change);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ConditionBox);
            this.Controls.Add(this.events_list);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.delay_box);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label_box);
            this.Controls.Add(this.label1);
            this.Location = new System.Drawing.Point(200, 40);
            this.Name = "Form2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            ((System.ComponentModel.ISupportInitialize)(this.levelBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.conditionBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gameEventTypeBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.conditionBindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.eventFlagBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gameEventBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox label_box;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.BindingSource conditionBindingSource;
        private System.Windows.Forms.BindingSource levelBindingSource;
        private System.Windows.Forms.BindingSource conditionBindingSource1;
        public System.Windows.Forms.TextBox ConditionBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button delay_change;
        public System.Windows.Forms.TextBox delay_box;
        private System.Windows.Forms.Button label_change;
        private System.Windows.Forms.BindingSource gameEventTypeBindingSource;
        private System.Windows.Forms.BindingSource eventFlagBindingSource;
        private System.Windows.Forms.BindingSource gameEventBindingSource;
        public System.Windows.Forms.ListBox events_list;
        private System.Windows.Forms.Button event_add;
        private System.Windows.Forms.Button event_remove;
        public System.Windows.Forms.TextBox event_type;
        private System.Windows.Forms.Button type_change;
        private System.Windows.Forms.Button target_change;
        public System.Windows.Forms.TextBox event_target;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        public System.Windows.Forms.ListBox listBox1;

    }
}