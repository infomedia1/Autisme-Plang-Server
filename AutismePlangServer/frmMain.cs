using System;
using System.Windows.Forms;

namespace AutismePlangServer
{
    public partial class frmMain : Form
    {
        public static frmMain Self;

        public frmMain()
        {
            InitializeComponent();
            Self = this;

            WebSocketServer.ScreensClients.Add("knkzb", 1);
            WebSocketServer.ScreensClients.Add("gecbarxm", 2);
            WebSocketServer.ScreensClients.Add("szdddmpdst", 3);
            WebSocketServer.StartUp();

            //Not best but easy
            dataGridView1.Rows.Add("1", "knkzb");
            dataGridView1.Rows.Add("2", "gecbarxm");
            dataGridView1.Rows.Add("3", "szdddmpdst");
        }

        private async void BtnreadPlang_Click(object sender, EventArgs e)
        {
            //Readplang
            await WebSocketServer.ReadPlangFromServer();
        }

        private void BtnSendPlang_Click(object sender, EventArgs e)
        {
            WebSocketServer.SendPlang();
        }

        private void TbUnzuel_TextChanged(object sender, EventArgs e)
        {
            WebSocketServer.NumberofScreens = Int32.Parse(tbUnzuel.Text);
        }

        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //dataGridView1
        }

        private void LbConnectedClients_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            if (lbConnectedClients.SelectedIndex > -1)
            {
                contextMenuStrip1.Show(Cursor.Position);
                contextMenuStrip1.Visible = true;
            }
        }

        private void AddToListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add("", lbConnectedClients.Items[lbConnectedClients.SelectedIndex].ToString());
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            WebSocketServer.ScreensClients.Clear();
            for (int i = 0; i <= dataGridView1.RowCount - 1; i++)
            {
                WebSocketServer.ScreensClients.Add(dataGridView1.Rows[i].Cells[1].Value.ToString(), Int32.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString()));
            }
        }

        private void TCheckForModTimer_Tick(object sender, EventArgs e)
        {
            //check if there is a Modification
            WebSocketServer.CheckForExternalUpdate();
        }

        private void BtnTestUpdate_Click(object sender, EventArgs e)
        {
            //
            //WebSocketServer.ToggleScreens();
        }
    }
}