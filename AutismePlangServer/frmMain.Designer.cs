namespace AutismePlangServer
{
    public partial class frmMain
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

        delegate void CleanupItemsCallback();
        public void CleanupItems()
        {
            if (this.lbConnectedClients.InvokeRequired)
            {
                CleanupItemsCallback d = new CleanupItemsCallback(CleanupItems);
                this.Invoke(d);
            } else { lbConnectedClients.Items.Clear(); }
        }

        delegate void AddItemToListCallback(string item);
        public void AddItemToList(string item)
        {
            if (this.lbConnectedClients.InvokeRequired)
            {
                AddItemToListCallback d = new AddItemToListCallback(AddItemToList);
                this.Invoke(d, new object[] { item });
            } else { lbConnectedClients.Items.Add(item); }
        }

        delegate void UpdateCountDisplayCallback(int item);
        public void UpdateCountDisplay(int item)
        {
            if (this.tbUnzuel.InvokeRequired)
            {
                UpdateCountDisplayCallback d = new UpdateCountDisplayCallback(UpdateCountDisplay);
                this.Invoke(d, new object[] { item });
            }
            else { tbUnzuel.Text=(item.ToString()); }
        }


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lbConnectedClients = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolbar = new System.Windows.Forms.ToolStrip();
            this.ServerIP = new System.Windows.Forms.ToolStripLabel();
            this.btnReadPlang = new System.Windows.Forms.Button();
            this.btnSendPlang = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.numm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.tbUnzuel = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tCheckForModTimer = new System.Windows.Forms.Timer(this.components);
            this.btnTestUpdate = new System.Windows.Forms.Button();
            this.toolbar.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbConnectedClients
            // 
            this.lbConnectedClients.FormattingEnabled = true;
            this.lbConnectedClients.Location = new System.Drawing.Point(12, 25);
            this.lbConnectedClients.Name = "lbConnectedClients";
            this.lbConnectedClients.Size = new System.Drawing.Size(120, 173);
            this.lbConnectedClients.TabIndex = 0;
            this.lbConnectedClients.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LbConnectedClients_MouseDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Connected Clients:";
            // 
            // toolbar
            // 
            this.toolbar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ServerIP});
            this.toolbar.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolbar.Location = new System.Drawing.Point(0, 543);
            this.toolbar.Name = "toolbar";
            this.toolbar.Size = new System.Drawing.Size(1008, 18);
            this.toolbar.TabIndex = 2;
            // 
            // ServerIP
            // 
            this.ServerIP.Name = "ServerIP";
            this.ServerIP.Size = new System.Drawing.Size(87, 15);
            this.ServerIP.Text = "PLACEHOLDER";
            // 
            // btnReadPlang
            // 
            this.btnReadPlang.Location = new System.Drawing.Point(12, 470);
            this.btnReadPlang.Name = "btnReadPlang";
            this.btnReadPlang.Size = new System.Drawing.Size(120, 70);
            this.btnReadPlang.TabIndex = 3;
            this.btnReadPlang.Text = "Read Plang";
            this.btnReadPlang.UseVisualStyleBackColor = true;
            this.btnReadPlang.Click += new System.EventHandler(this.BtnreadPlang_Click);
            // 
            // btnSendPlang
            // 
            this.btnSendPlang.Location = new System.Drawing.Point(138, 470);
            this.btnSendPlang.Name = "btnSendPlang";
            this.btnSendPlang.Size = new System.Drawing.Size(120, 70);
            this.btnSendPlang.TabIndex = 4;
            this.btnSendPlang.Text = "Send Plang";
            this.btnSendPlang.UseVisualStyleBackColor = true;
            this.btnSendPlang.Click += new System.EventHandler(this.BtnSendPlang_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Location = new System.Drawing.Point(138, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(858, 439);
            this.panel1.TabIndex = 5;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.numm,
            this.ID});
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridView1.Size = new System.Drawing.Size(193, 170);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellDoubleClick);
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellValueChanged);
            // 
            // numm
            // 
            this.numm.Frozen = true;
            this.numm.HeaderText = "Nummer";
            this.numm.Name = "numm";
            this.numm.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.numm.Width = 50;
            // 
            // ID
            // 
            this.ID.HeaderText = "Display ID";
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 201);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Unzuel Display Acceuil:";
            // 
            // tbUnzuel
            // 
            this.tbUnzuel.Location = new System.Drawing.Point(12, 217);
            this.tbUnzuel.Name = "tbUnzuel";
            this.tbUnzuel.Size = new System.Drawing.Size(31, 20);
            this.tbUnzuel.TabIndex = 7;
            this.tbUnzuel.TextChanged += new System.EventHandler(this.TbUnzuel_TextChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToListToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(132, 26);
            // 
            // addToListToolStripMenuItem
            // 
            this.addToListToolStripMenuItem.Name = "addToListToolStripMenuItem";
            this.addToListToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.addToListToolStripMenuItem.Text = "Add to List";
            this.addToListToolStripMenuItem.Click += new System.EventHandler(this.AddToListToolStripMenuItem_Click);
            // 
            // tCheckForModTimer
            // 
            this.tCheckForModTimer.Enabled = true;
            this.tCheckForModTimer.Interval = 60000;
            this.tCheckForModTimer.Tick += new System.EventHandler(this.TCheckForModTimer_Tick);
            // 
            // btnTestUpdate
            // 
            this.btnTestUpdate.Location = new System.Drawing.Point(264, 470);
            this.btnTestUpdate.Name = "btnTestUpdate";
            this.btnTestUpdate.Size = new System.Drawing.Size(130, 70);
            this.btnTestUpdate.TabIndex = 8;
            this.btnTestUpdate.Text = "Test SoapCall";
            this.btnTestUpdate.UseVisualStyleBackColor = true;
            this.btnTestUpdate.Click += new System.EventHandler(this.BtnTestUpdate_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 561);
            this.Controls.Add(this.btnTestUpdate);
            this.Controls.Add(this.tbUnzuel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnSendPlang);
            this.Controls.Add(this.btnReadPlang);
            this.Controls.Add(this.toolbar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbConnectedClients);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Autisme Plang Server Manager";
            this.toolbar.ResumeLayout(false);
            this.toolbar.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ListBox lbConnectedClients;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ToolStrip toolbar;
        public System.Windows.Forms.ToolStripLabel ServerIP;
        private System.Windows.Forms.Button btnReadPlang;
        private System.Windows.Forms.Button btnSendPlang;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbUnzuel;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addToListToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn numm;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.Timer tCheckForModTimer;
        private System.Windows.Forms.Button btnTestUpdate;
    }
}

