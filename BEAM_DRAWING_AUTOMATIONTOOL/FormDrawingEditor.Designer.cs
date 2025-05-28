namespace BEAM_DRAWING_AUTOMATIONTOOL
{
    partial class FormDrawingEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDrawingEditor));
            this.btncreate = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnclear = new System.Windows.Forms.Button();
            this.chkrdconnmark = new System.Windows.Forms.CheckBox();
            this.chkknockoffdim = new System.Windows.Forms.CheckBox();
            this.chkcutlen = new System.Windows.Forms.CheckBox();
            this.chkwptxteledim = new System.Windows.Forms.CheckBox();
            this.chksecscale = new System.Windows.Forms.CheckBox();
            this.cmbsecscale = new System.Windows.Forms.ComboBox();
            this.chkeledim = new System.Windows.Forms.CheckBox();
            this.cmbclient = new System.Windows.Forms.ComboBox();
            this.lblclient = new System.Windows.Forms.Label();
            this.chkscale = new System.Windows.Forms.CheckBox();
            this.txtscale = new System.Windows.Forms.TextBox();
            this.txtminlen = new System.Windows.Forms.TextBox();
            this.chkminlen = new System.Windows.Forms.CheckBox();
            this.chka1 = new System.Windows.Forms.CheckBox();
            this.chka2 = new System.Windows.Forms.CheckBox();
            this.chka3 = new System.Windows.Forms.CheckBox();
            this.gboxmanualinput = new System.Windows.Forms.GroupBox();
            this.chka0 = new System.Windows.Forms.CheckBox();
            this.chkmanualinput = new System.Windows.Forms.CheckBox();
            this.chkfontsize = new System.Windows.Forms.CheckBox();
            this.gboxfontsize = new System.Windows.Forms.GroupBox();
            this.chk9by64 = new System.Windows.Forms.CheckBox();
            this.chk1by8 = new System.Windows.Forms.CheckBox();
            this.chk3by32 = new System.Windows.Forms.CheckBox();
            this.gboxclientatt = new System.Windows.Forms.GroupBox();
            this.lblsbar2 = new System.Windows.Forms.Label();
            this.lblsbar1 = new System.Windows.Forms.Label();
            this.pbar = new System.Windows.Forms.ProgressBar();
            this.btnpaste = new System.Windows.Forms.Button();
            this.btnselect = new System.Windows.Forms.Button();
            this.dgvlog = new System.Windows.Forms.DataGridView();
            this.drgmark = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.drgname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.drgproftype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.drgsecs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.drglen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.drgattribute = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.drgrmk = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gboxmanualinput.SuspendLayout();
            this.gboxfontsize.SuspendLayout();
            this.gboxclientatt.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvlog)).BeginInit();
            this.SuspendLayout();
            // 
            // btncreate
            // 
            this.btncreate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btncreate.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btncreate.Location = new System.Drawing.Point(184, 665);
            this.btncreate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btncreate.Name = "btncreate";
            this.btncreate.Size = new System.Drawing.Size(87, 37);
            this.btncreate.TabIndex = 0;
            this.btncreate.Text = "Create";
            this.btncreate.UseVisualStyleBackColor = true;
            this.btncreate.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.label3.Location = new System.Drawing.Point(1443, 57);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(467, 30);
            this.label3.TabIndex = 4;
            this.label3.Text = "ESSKAY DESIGN AND STRUCTURES";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.label4.Location = new System.Drawing.Point(1449, 85);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(451, 25);
            this.label4.TabIndex = 5;
            this.label4.Text = "Beam Editor";
            // 
            // btnclear
            // 
            this.btnclear.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnclear.Location = new System.Drawing.Point(271, 665);
            this.btnclear.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnclear.Name = "btnclear";
            this.btnclear.Size = new System.Drawing.Size(27, 37);
            this.btnclear.TabIndex = 8;
            this.btnclear.Text = "Clear";
            this.btnclear.UseVisualStyleBackColor = true;
            this.btnclear.Click += new System.EventHandler(this.button2_Click);
            // 
            // chkrdconnmark
            // 
            this.chkrdconnmark.AutoSize = true;
            this.chkrdconnmark.Location = new System.Drawing.Point(23, 37);
            this.chkrdconnmark.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkrdconnmark.Name = "chkrdconnmark";
            this.chkrdconnmark.Size = new System.Drawing.Size(229, 24);
            this.chkrdconnmark.TabIndex = 10;
            this.chkrdconnmark.Text = "Rd in connecting sidemark";
            this.chkrdconnmark.UseVisualStyleBackColor = true;
            // 
            // chkknockoffdim
            // 
            this.chkknockoffdim.AutoSize = true;
            this.chkknockoffdim.Location = new System.Drawing.Point(23, 65);
            this.chkknockoffdim.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkknockoffdim.Name = "chkknockoffdim";
            this.chkknockoffdim.Size = new System.Drawing.Size(182, 24);
            this.chkknockoffdim.TabIndex = 11;
            this.chkknockoffdim.Text = "Knock off dimension";
            this.chkknockoffdim.UseVisualStyleBackColor = true;
            // 
            // chkcutlen
            // 
            this.chkcutlen.AutoSize = true;
            this.chkcutlen.Location = new System.Drawing.Point(23, 150);
            this.chkcutlen.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkcutlen.Name = "chkcutlen";
            this.chkcutlen.Size = new System.Drawing.Size(107, 24);
            this.chkcutlen.TabIndex = 12;
            this.chkcutlen.Text = "Cut length";
            this.chkcutlen.UseVisualStyleBackColor = true;
            // 
            // chkwptxteledim
            // 
            this.chkwptxteledim.AutoSize = true;
            this.chkwptxteledim.Location = new System.Drawing.Point(23, 122);
            this.chkwptxteledim.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkwptxteledim.Name = "chkwptxteledim";
            this.chkwptxteledim.Size = new System.Drawing.Size(211, 24);
            this.chkwptxteledim.TabIndex = 15;
            this.chkwptxteledim.Text = "WP text in elevation dim";
            this.chkwptxteledim.UseVisualStyleBackColor = true;
            // 
            // chksecscale
            // 
            this.chksecscale.AutoSize = true;
            this.chksecscale.Location = new System.Drawing.Point(23, 178);
            this.chksecscale.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chksecscale.Name = "chksecscale";
            this.chksecscale.Size = new System.Drawing.Size(155, 24);
            this.chksecscale.TabIndex = 16;
            this.chksecscale.Text = "Section scale up";
            this.chksecscale.UseVisualStyleBackColor = true;
            this.chksecscale.CheckedChanged += new System.EventHandler(this.checkBox7_CheckedChanged);
            // 
            // cmbsecscale
            // 
            this.cmbsecscale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbsecscale.Enabled = false;
            this.cmbsecscale.FormattingEnabled = true;
            this.cmbsecscale.Items.AddRange(new object[] {
            "-4",
            "-3",
            "-2",
            "-1",
            "0",
            "1",
            "2",
            "3",
            "4"});
            this.cmbsecscale.Location = new System.Drawing.Point(193, 174);
            this.cmbsecscale.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbsecscale.Name = "cmbsecscale";
            this.cmbsecscale.Size = new System.Drawing.Size(72, 28);
            this.cmbsecscale.TabIndex = 17;
            // 
            // chkeledim
            // 
            this.chkeledim.AutoSize = true;
            this.chkeledim.Location = new System.Drawing.Point(23, 94);
            this.chkeledim.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkeledim.Name = "chkeledim";
            this.chkeledim.Size = new System.Drawing.Size(136, 24);
            this.chkeledim.TabIndex = 18;
            this.chkeledim.Text = "Elevation dim ";
            this.chkeledim.UseVisualStyleBackColor = true;
            // 
            // cmbclient
            // 
            this.cmbclient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbclient.FormattingEnabled = true;
            this.cmbclient.ItemHeight = 16;
            this.cmbclient.Items.AddRange(new object[] {
            "BENHUR",
            "LIPHART",
            "FORD",
            "HAMILTON",
            "HILLSDALE",
            "TRINITY",
            "SME",
            "STEFFY&SON",
            "NONE"});
            this.cmbclient.Location = new System.Drawing.Point(124, 14);
            this.cmbclient.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbclient.Name = "cmbclient";
            this.cmbclient.Size = new System.Drawing.Size(172, 24);
            this.cmbclient.TabIndex = 19;
            this.cmbclient.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // lblclient
            // 
            this.lblclient.AutoSize = true;
            this.lblclient.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblclient.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblclient.Location = new System.Drawing.Point(11, 9);
            this.lblclient.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblclient.Name = "lblclient";
            this.lblclient.Size = new System.Drawing.Size(78, 29);
            this.lblclient.TabIndex = 20;
            this.lblclient.Text = "Client";
            // 
            // chkscale
            // 
            this.chkscale.AutoSize = true;
            this.chkscale.Enabled = false;
            this.chkscale.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkscale.Location = new System.Drawing.Point(23, 153);
            this.chkscale.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkscale.Name = "chkscale";
            this.chkscale.Size = new System.Drawing.Size(71, 24);
            this.chkscale.TabIndex = 21;
            this.chkscale.Text = "Scale";
            this.chkscale.UseVisualStyleBackColor = true;
            this.chkscale.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
            // 
            // txtscale
            // 
            this.txtscale.Enabled = false;
            this.txtscale.Location = new System.Drawing.Point(135, 150);
            this.txtscale.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtscale.Name = "txtscale";
            this.txtscale.Size = new System.Drawing.Size(103, 24);
            this.txtscale.TabIndex = 22;
            this.txtscale.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // txtminlen
            // 
            this.txtminlen.Enabled = false;
            this.txtminlen.Location = new System.Drawing.Point(135, 182);
            this.txtminlen.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtminlen.Name = "txtminlen";
            this.txtminlen.Size = new System.Drawing.Size(103, 24);
            this.txtminlen.TabIndex = 23;
            this.txtminlen.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // chkminlen
            // 
            this.chkminlen.AutoSize = true;
            this.chkminlen.Enabled = false;
            this.chkminlen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkminlen.Location = new System.Drawing.Point(23, 186);
            this.chkminlen.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkminlen.Name = "chkminlen";
            this.chkminlen.Size = new System.Drawing.Size(104, 24);
            this.chkminlen.TabIndex = 24;
            this.chkminlen.Text = "Min length";
            this.chkminlen.UseVisualStyleBackColor = true;
            this.chkminlen.CheckedChanged += new System.EventHandler(this.checkBox5_CheckedChanged);
            // 
            // chka1
            // 
            this.chka1.AutoSize = true;
            this.chka1.Enabled = false;
            this.chka1.Location = new System.Drawing.Point(23, 68);
            this.chka1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chka1.Name = "chka1";
            this.chka1.Size = new System.Drawing.Size(47, 22);
            this.chka1.TabIndex = 25;
            this.chka1.Text = "A1";
            this.chka1.UseVisualStyleBackColor = true;
            // 
            // chka2
            // 
            this.chka2.AutoSize = true;
            this.chka2.Enabled = false;
            this.chka2.Location = new System.Drawing.Point(23, 96);
            this.chka2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chka2.Name = "chka2";
            this.chka2.Size = new System.Drawing.Size(47, 22);
            this.chka2.TabIndex = 26;
            this.chka2.Text = "A2";
            this.chka2.UseVisualStyleBackColor = true;
            // 
            // chka3
            // 
            this.chka3.AutoSize = true;
            this.chka3.Enabled = false;
            this.chka3.Location = new System.Drawing.Point(23, 124);
            this.chka3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chka3.Name = "chka3";
            this.chka3.Size = new System.Drawing.Size(47, 22);
            this.chka3.TabIndex = 27;
            this.chka3.Text = "A3";
            this.chka3.UseVisualStyleBackColor = true;
            // 
            // gboxmanualinput
            // 
            this.gboxmanualinput.Controls.Add(this.chka0);
            this.gboxmanualinput.Controls.Add(this.chka1);
            this.gboxmanualinput.Controls.Add(this.chkminlen);
            this.gboxmanualinput.Controls.Add(this.chka3);
            this.gboxmanualinput.Controls.Add(this.txtminlen);
            this.gboxmanualinput.Controls.Add(this.chka2);
            this.gboxmanualinput.Controls.Add(this.txtscale);
            this.gboxmanualinput.Controls.Add(this.chkscale);
            this.gboxmanualinput.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gboxmanualinput.Location = new System.Drawing.Point(44, 284);
            this.gboxmanualinput.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gboxmanualinput.Name = "gboxmanualinput";
            this.gboxmanualinput.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gboxmanualinput.Size = new System.Drawing.Size(252, 225);
            this.gboxmanualinput.TabIndex = 28;
            this.gboxmanualinput.TabStop = false;
            this.gboxmanualinput.Text = "Manual input";
            this.gboxmanualinput.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // chka0
            // 
            this.chka0.AutoSize = true;
            this.chka0.Enabled = false;
            this.chka0.Location = new System.Drawing.Point(23, 37);
            this.chka0.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chka0.Name = "chka0";
            this.chka0.Size = new System.Drawing.Size(47, 22);
            this.chka0.TabIndex = 28;
            this.chka0.Text = "A0";
            this.chka0.UseVisualStyleBackColor = true;
            // 
            // chkmanualinput
            // 
            this.chkmanualinput.Location = new System.Drawing.Point(11, 271);
            this.chkmanualinput.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkmanualinput.Name = "chkmanualinput";
            this.chkmanualinput.Size = new System.Drawing.Size(36, 33);
            this.chkmanualinput.TabIndex = 29;
            this.chkmanualinput.Text = "-";
            this.chkmanualinput.UseVisualStyleBackColor = true;
            this.chkmanualinput.CheckedChanged += new System.EventHandler(this.checkBox12_CheckedChanged);
            // 
            // chkfontsize
            // 
            this.chkfontsize.Location = new System.Drawing.Point(1488, 437);
            this.chkfontsize.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkfontsize.Name = "chkfontsize";
            this.chkfontsize.Size = new System.Drawing.Size(24, 25);
            this.chkfontsize.TabIndex = 32;
            this.chkfontsize.Text = "-";
            this.chkfontsize.UseVisualStyleBackColor = true;
            this.chkfontsize.CheckedChanged += new System.EventHandler(this.checkBox15_CheckedChanged);
            // 
            // gboxfontsize
            // 
            this.gboxfontsize.Controls.Add(this.chk9by64);
            this.gboxfontsize.Controls.Add(this.chk1by8);
            this.gboxfontsize.Controls.Add(this.chk3by32);
            this.gboxfontsize.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gboxfontsize.Location = new System.Drawing.Point(15, 517);
            this.gboxfontsize.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gboxfontsize.Name = "gboxfontsize";
            this.gboxfontsize.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gboxfontsize.Size = new System.Drawing.Size(232, 52);
            this.gboxfontsize.TabIndex = 33;
            this.gboxfontsize.TabStop = false;
            this.gboxfontsize.Text = "Font size";
            // 
            // chk9by64
            // 
            this.chk9by64.AutoSize = true;
            this.chk9by64.Enabled = false;
            this.chk9by64.Location = new System.Drawing.Point(73, 18);
            this.chk9by64.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chk9by64.Name = "chk9by64";
            this.chk9by64.Size = new System.Drawing.Size(58, 22);
            this.chk9by64.TabIndex = 35;
            this.chk9by64.Text = "9/64";
            this.chk9by64.UseVisualStyleBackColor = true;
            // 
            // chk1by8
            // 
            this.chk1by8.AutoSize = true;
            this.chk1by8.Enabled = false;
            this.chk1by8.Location = new System.Drawing.Point(8, 18);
            this.chk1by8.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chk1by8.Name = "chk1by8";
            this.chk1by8.Size = new System.Drawing.Size(50, 22);
            this.chk1by8.TabIndex = 30;
            this.chk1by8.Text = "1/8";
            this.chk1by8.UseVisualStyleBackColor = true;
            // 
            // chk3by32
            // 
            this.chk3by32.AutoSize = true;
            this.chk3by32.Enabled = false;
            this.chk3by32.Location = new System.Drawing.Point(148, 18);
            this.chk3by32.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chk3by32.Name = "chk3by32";
            this.chk3by32.Size = new System.Drawing.Size(58, 22);
            this.chk3by32.TabIndex = 31;
            this.chk3by32.Text = "3/32";
            this.chk3by32.UseVisualStyleBackColor = true;
            // 
            // gboxclientatt
            // 
            this.gboxclientatt.Controls.Add(this.chkknockoffdim);
            this.gboxclientatt.Controls.Add(this.chkrdconnmark);
            this.gboxclientatt.Controls.Add(this.chkcutlen);
            this.gboxclientatt.Controls.Add(this.chkwptxteledim);
            this.gboxclientatt.Controls.Add(this.chksecscale);
            this.gboxclientatt.Controls.Add(this.cmbsecscale);
            this.gboxclientatt.Controls.Add(this.chkeledim);
            this.gboxclientatt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gboxclientatt.Location = new System.Drawing.Point(16, 46);
            this.gboxclientatt.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gboxclientatt.Name = "gboxclientatt";
            this.gboxclientatt.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gboxclientatt.Size = new System.Drawing.Size(280, 217);
            this.gboxclientatt.TabIndex = 34;
            this.gboxclientatt.TabStop = false;
            this.gboxclientatt.Text = "Client attribute";
            // 
            // lblsbar2
            // 
            this.lblsbar2.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblsbar2.Location = new System.Drawing.Point(1104, 732);
            this.lblsbar2.Name = "lblsbar2";
            this.lblsbar2.Size = new System.Drawing.Size(188, 21);
            this.lblsbar2.TabIndex = 35;
            this.lblsbar2.Text = "Ready.";
            this.lblsbar2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblsbar1
            // 
            this.lblsbar1.Location = new System.Drawing.Point(11, 735);
            this.lblsbar1.Name = "lblsbar1";
            this.lblsbar1.Size = new System.Drawing.Size(1065, 21);
            this.lblsbar1.TabIndex = 37;
            this.lblsbar1.Text = "Ready.";
            this.lblsbar1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pbar
            // 
            this.pbar.Location = new System.Drawing.Point(11, 709);
            this.pbar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pbar.Name = "pbar";
            this.pbar.Size = new System.Drawing.Size(1281, 23);
            this.pbar.TabIndex = 38;
            // 
            // btnpaste
            // 
            this.btnpaste.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnpaste.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnpaste.Location = new System.Drawing.Point(11, 666);
            this.btnpaste.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnpaste.Name = "btnpaste";
            this.btnpaste.Size = new System.Drawing.Size(87, 37);
            this.btnpaste.TabIndex = 39;
            this.btnpaste.Text = "Paste";
            this.btnpaste.UseVisualStyleBackColor = true;
            this.btnpaste.Click += new System.EventHandler(this.btnpaste_Click);
            // 
            // btnselect
            // 
            this.btnselect.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnselect.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnselect.Location = new System.Drawing.Point(97, 666);
            this.btnselect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnselect.Name = "btnselect";
            this.btnselect.Size = new System.Drawing.Size(87, 37);
            this.btnselect.TabIndex = 40;
            this.btnselect.Text = "Select";
            this.btnselect.UseVisualStyleBackColor = true;
            this.btnselect.Click += new System.EventHandler(this.btnselect_Click);
            // 
            // dgvlog
            // 
            this.dgvlog.AllowUserToAddRows = false;
            this.dgvlog.AllowUserToDeleteRows = false;
            this.dgvlog.AllowUserToResizeColumns = false;
            this.dgvlog.AllowUserToResizeRows = false;
            this.dgvlog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvlog.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.drgmark,
            this.drgname,
            this.drgproftype,
            this.drgsecs,
            this.drglen,
            this.drgattribute,
            this.drgrmk});
            this.dgvlog.Location = new System.Drawing.Point(303, 9);
            this.dgvlog.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvlog.Name = "dgvlog";
            this.dgvlog.RowHeadersWidth = 51;
            this.dgvlog.RowTemplate.Height = 24;
            this.dgvlog.Size = new System.Drawing.Size(989, 694);
            this.dgvlog.TabIndex = 36;
            // 
            // drgmark
            // 
            this.drgmark.HeaderText = "Mark";
            this.drgmark.MinimumWidth = 6;
            this.drgmark.Name = "drgmark";
            this.drgmark.ReadOnly = true;
            this.drgmark.Width = 125;
            // 
            // drgname
            // 
            this.drgname.HeaderText = "Name";
            this.drgname.MinimumWidth = 6;
            this.drgname.Name = "drgname";
            this.drgname.Width = 125;
            // 
            // drgproftype
            // 
            this.drgproftype.HeaderText = "Pr.Type";
            this.drgproftype.MinimumWidth = 6;
            this.drgproftype.Name = "drgproftype";
            this.drgproftype.ReadOnly = true;
            this.drgproftype.Width = 125;
            // 
            // drgsecs
            // 
            this.drgsecs.HeaderText = "Sec. #";
            this.drgsecs.MinimumWidth = 6;
            this.drgsecs.Name = "drgsecs";
            this.drgsecs.ReadOnly = true;
            this.drgsecs.Width = 125;
            // 
            // drglen
            // 
            this.drglen.HeaderText = "Length";
            this.drglen.MinimumWidth = 6;
            this.drglen.Name = "drglen";
            this.drglen.ReadOnly = true;
            this.drglen.Width = 125;
            // 
            // drgattribute
            // 
            this.drgattribute.HeaderText = "Attribute";
            this.drgattribute.MinimumWidth = 6;
            this.drgattribute.Name = "drgattribute";
            this.drgattribute.Width = 125;
            // 
            // drgrmk
            // 
            this.drgrmk.HeaderText = "Remark";
            this.drgrmk.MinimumWidth = 6;
            this.drgrmk.Name = "drgrmk";
            this.drgrmk.ReadOnly = true;
            this.drgrmk.Width = 180;
            // 
            // frmdrgeditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1303, 762);
            this.Controls.Add(this.btnselect);
            this.Controls.Add(this.btnpaste);
            this.Controls.Add(this.pbar);
            this.Controls.Add(this.lblsbar1);
            this.Controls.Add(this.dgvlog);
            this.Controls.Add(this.lblsbar2);
            this.Controls.Add(this.gboxclientatt);
            this.Controls.Add(this.gboxfontsize);
            this.Controls.Add(this.chkfontsize);
            this.Controls.Add(this.chkmanualinput);
            this.Controls.Add(this.gboxmanualinput);
            this.Controls.Add(this.lblclient);
            this.Controls.Add(this.cmbclient);
            this.Controls.Add(this.btnclear);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btncreate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.Name = "frmdrgeditor";
            this.Text = "ESSKAY AUTOMATION";
            //this.Load += new System.EventHandler(this.Form1_Load_1);
            this.gboxmanualinput.ResumeLayout(false);
            this.gboxmanualinput.PerformLayout();
            this.gboxfontsize.ResumeLayout(false);
            this.gboxfontsize.PerformLayout();
            this.gboxclientatt.ResumeLayout(false);
            this.gboxclientatt.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvlog)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btncreate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnclear;
        private System.Windows.Forms.CheckBox chkrdconnmark;
        private System.Windows.Forms.CheckBox chkknockoffdim;
        private System.Windows.Forms.CheckBox chkcutlen;
        private System.Windows.Forms.CheckBox chkwptxteledim;
        private System.Windows.Forms.CheckBox chksecscale;
        private System.Windows.Forms.ComboBox cmbsecscale;
        private System.Windows.Forms.CheckBox chkeledim;
        private System.Windows.Forms.ComboBox cmbclient;
        private System.Windows.Forms.Label lblclient;
        private System.Windows.Forms.CheckBox chkscale;
        private System.Windows.Forms.TextBox txtscale;
        private System.Windows.Forms.TextBox txtminlen;
        private System.Windows.Forms.CheckBox chkminlen;
        private System.Windows.Forms.CheckBox chka1;
        private System.Windows.Forms.CheckBox chka2;
        private System.Windows.Forms.CheckBox chka3;
        private System.Windows.Forms.GroupBox gboxmanualinput;
        private System.Windows.Forms.CheckBox chkmanualinput;
        private System.Windows.Forms.CheckBox chkfontsize;
        private System.Windows.Forms.GroupBox gboxfontsize;
        private System.Windows.Forms.GroupBox gboxclientatt;
        private System.Windows.Forms.Label lblsbar2;
        private System.Windows.Forms.CheckBox chka0;
        private System.Windows.Forms.Label lblsbar1;
        private System.Windows.Forms.ProgressBar pbar;
        private System.Windows.Forms.Button btnpaste;
        private System.Windows.Forms.Button btnselect;
        private System.Windows.Forms.DataGridView dgvlog;
        private System.Windows.Forms.DataGridViewTextBoxColumn drgmark;
        private System.Windows.Forms.DataGridViewTextBoxColumn drgname;
        private System.Windows.Forms.DataGridViewTextBoxColumn drgproftype;
        private System.Windows.Forms.DataGridViewTextBoxColumn drgsecs;
        private System.Windows.Forms.DataGridViewTextBoxColumn drglen;
        private System.Windows.Forms.DataGridViewTextBoxColumn drgattribute;
        private System.Windows.Forms.DataGridViewTextBoxColumn drgrmk;
        private System.Windows.Forms.CheckBox chk9by64;
        private System.Windows.Forms.CheckBox chk1by8;
        private System.Windows.Forms.CheckBox chk3by32;
    }
}

