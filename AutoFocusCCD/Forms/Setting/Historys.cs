using AutoFocusCCD.Config;
using Multi_Camera_MINI_AOI_V3.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoFocusCCD.Forms.Setting
{
    public partial class Historys : Form
    {
        public Historys()
        {
            InitializeComponent();
        }

        private DataTable dtHistory;
        private int noDtHistory = 1;
        private int pageSizeHistory = 50;
        private int currentPageHistory = 1;
        private int totalPagesHistory = 1;
        private int totalDataHistory = 0;

        private void Historys_Load(object sender, EventArgs e)
        {
            dtHistory = new DataTable();
            dtHistory.Columns.Add("Id");
            dtHistory.Columns.Add("No");
            dtHistory.Columns.Add("Emp");
            dtHistory.Columns.Add("QrCode");
            dtHistory.Columns.Add("Model");
            dtHistory.Columns.Add("Folder");
            dtHistory.Columns.Add("Result");
            dtHistory.Columns.Add("CreatedAt");
            dtHistory.Columns.Add("UpdatedAt");

            RenderDGVHistory();
            dateTimePicker.Enabled = false;
        }

        private void RenderDGVHistory()
        {
            string date = cbDate.Checked ? dateTimePicker.Value.ToString("yyyy-MM-dd") : "";
            totalDataHistory = SQLite.History.Count(this.txtEmp.Text, this.txtQrCode.Text, date, this.txtResult.Text);
            totalPagesHistory = (int)Math.Ceiling((double)totalDataHistory / pageSizeHistory);

            dtHistory.Rows.Clear();

            var histories = SQLite.History.GetList(this.txtEmp.Text, this.txtQrCode.Text, date, this.txtResult.Text, currentPageHistory, pageSizeHistory);

            if (histories == null || histories.Count == 0) return;

            int slectedRow = Extensions.GetSelectedRowIndex(dgvHistory);
            noDtHistory = (currentPageHistory - 1) * pageSizeHistory + 1;
            foreach (var history in histories)
            {
                dtHistory.Rows.Add(history.Id, noDtHistory, history.employee, history.qr_code, history.product_name, history.path_folder, history.result, history.CreatedAt, history.UpdatedAt);
                noDtHistory++;
            }

            Extensions.SetDataSourceAndUpdateSelection(dgvHistory, dtHistory, new string[] { "Id" , "UpdatedAt" });
            Extensions.SelectedRow(dgvHistory, slectedRow);

            toolStripStatusLabel1.Text = $"Total: {totalDataHistory} records, Page: {currentPageHistory}/{totalPagesHistory}";

            btnPrevious.Enabled = currentPageHistory > 1;
            btnNext.Enabled = currentPageHistory < totalPagesHistory;
            
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            currentPageHistory--;
            RenderDGVHistory();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            currentPageHistory++;
            RenderDGVHistory();
        }

        private void txtEmp_TextChanged(object sender, EventArgs e)
        {
            timerSearch.Stop();
            timerSearch.Start();
        }

        private void timerSearch_Tick(object sender, EventArgs e)
        {
            RenderDGVHistory();
        }

        private void cbDate_CheckedChanged(object sender, EventArgs e)
        {
            if(cbDate.Checked)
            {
                dateTimePicker.Enabled = true;
                timerSearch.Stop();
                timerSearch.Start();
            }
            else
            {
                dateTimePicker.Enabled = false;
            }
        }

        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            timerSearch.Stop();
            timerSearch.Start();
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvHistory.SelectedRows.Count > 0)
            {
                string path = dgvHistory.SelectedRows[0].Cells["Folder"].Value.ToString();
                try
                {
                    // Open windows explorer
                    System.Diagnostics.Process.Start("explorer.exe", path);
                }
                catch (Exception ex)
                {
                    Main.Logger.Error("Error opening path: " + ex.Message);
                }
            }
        }
    }
}
