using Multi_Camera_MINI_AOI_V3.Utilities;
using Ookii.Dialogs.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoFocusCCD.Forms.Tools
{
    public partial class CropImage : Form
    {
        public CropImage()
        {
            InitializeComponent();
        }
        private DataTable dtData;

        private void CropImage_Load(object sender, EventArgs e)
        {
            dtData = new DataTable();
            dtData.Columns.Add("No");
            dtData.Columns.Add("Name");
            dtData.Columns.Add("Path");

            toolStripStatusLabel1.Text = "";
        }
        private int selectedRow = -1;

        private void ReLoadData()
        {
            string pathFolder = txtPath.Text;
            if (!System.IO.Directory.Exists(pathFolder))
            {
                MessageBox.Show("Folder not found");
                return;
            }

            // file in folder (jpg only)
            string[] files = System.IO.Directory.GetFiles(pathFolder, "*.jpg");

            dtData.Rows.Clear();
            for (int i = 0; i < files.Length; i++)
            {
                string fileName = System.IO.Path.GetFileName(files[i]);
                dtData.Rows.Add(i + 1, fileName, files[i]);
            }
            Extensions.SetDataSourceAndUpdateSelection(dgvData, dtData, visibleColumns: new string[] { "Path" });

            // reset selected row
            if (dgvData.Rows.Count > selectedRow && selectedRow != -1)
            {
                Extensions.SelectedRow(dgvData, selectedRow);
            }
            else
            {
                selectedRow = -1;
                Extensions.SelectedRow(dgvData, selectedRow);
            }

            toolStripStatusLabel1.Text = "Total: " + files.Length;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            //FolderBrowserDialog fbd = new FolderBrowserDialog();
            //if (fbd.ShowDialog() == DialogResult.OK)
            //{
            //    txtPath.Text = fbd.SelectedPath;
            //    selectedRow = -1;
            //    ReLoadData();
            //}

            using(var dialog = new VistaFolderBrowserDialog())
            {
                dialog.SelectedPath = txtPath.Text;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = dialog.SelectedPath;
                    selectedRow = -1;
                    ReLoadData();
                }
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (selectedRow > 0)
            {
                selectedRow--;
                Extensions.SelectedRow(dgvData, selectedRow);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (selectedRow < dgvData.Rows.Count - 1)
            {
                selectedRow++;
                Extensions.SelectedRow(dgvData, selectedRow);

                // scroll to current
                dgvData.FirstDisplayedScrollingRowIndex = selectedRow;   
            }
        }

        private void dgvData_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvData.SelectedRows.Count == 0)
            {
                return;
            }
            selectedRow = dgvData.SelectedRows[0].Index;
            var cellValue = dgvData.SelectedRows[0].Cells["Path"].Value;
            if (cellValue != null)
            {
                string path = cellValue.ToString();
                if (System.IO.File.Exists(path))
                {
                    scrollablePictureBox1.Image?.Dispose();
                    scrollablePictureBox1.Image = new Bitmap(path);
                }
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (scrollablePictureBox1.Image == null)
            {
                MessageBox.Show("No image to save", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (txtNew.Text == "")
            {
                MessageBox.Show("Please input folder name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                btnSave.Enabled = false;
                toolStripStatusLabel1.Text = "Saving...";
                toolStripStatusLabel1.ForeColor = Color.Black;
                await Task.Delay(100);
                using (var bmp = new Bitmap(scrollablePictureBox1.Image))
                {
                    Rectangle rect = scrollablePictureBox1.GetRectOriginal();
                    if (rect.Width == 0 || rect.Height == 0)
                    {
                        MessageBox.Show("No image to save", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string path = System.IO.Path.Combine(txtPath.Text, txtNew.Text);
                    if (!System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }
                    string newFileName = "C_" + Guid.NewGuid().ToString().Substring(0, 10).Trim() + ".jpg";
                    Extensions.CropSaveImage(System.IO.Path.Combine(path, newFileName), bmp, rect);

                    toolStripStatusLabel1.Text = "Save image: " + newFileName;
                    toolStripStatusLabel1.ForeColor = Color.Green;
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = "Error: " + ex.Message;
                toolStripStatusLabel1.ForeColor = Color.Red;
            }
            finally
            {
                btnSave.Enabled = true;
                btnNext.PerformClick();
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            ReLoadData();
        }
    }
}
