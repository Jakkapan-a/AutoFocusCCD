using AutoFocusCCD.Components;
using Multi_Camera_MINI_AOI_V3.Utilities;
using NLog;
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
    public partial class Product : Form
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public Product()
        {
            InitializeComponent();
        }

        private void Product_Load(object sender, EventArgs e)
        {
            CreateDt();
            RenderDGV();

            cmbType.SelectedIndex = Properties.Settings.Default.ProductType;

            Logger.Info("Product form loaded.");
            btnNew.PerformClick();
        }

        private DataTable dt;
        private int noDt = 1;
        private int pageSize = 100;
        private int currentPage = 1;
        private int totalPage = 0;
        private int totalData = 0;

        private void CreateDt()
        {
            dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("No", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("Voltage_min", typeof(int));
            dt.Columns.Add("Voltage_max", typeof(int));
            dt.Columns.Add("Current_min", typeof(int));
            dt.Columns.Add("Current_max", typeof(int));
            dt.Columns.Add("Image", typeof(string));
            dt.Columns.Add("CreatedAt", typeof(string));
            dt.Columns.Add("UpdatedAt", typeof(string));
        }
        private SQLite.Product productSeleted = null;
        private void RenderDGV()
        {
            if (dt == null) return;

            totalData = SQLite.Product.Count(txtSearch.Text);
            totalPage = (int)Math.Ceiling((double)totalData / pageSize);
            
            
            var product = SQLite.Product.GetList(txtSearch.Text, pageSize, currentPage);

            if (product == null) return;
            int selectedRow = Extensions.GetSelectedRowIndex(dgvProduct);
            dt.Clear();

            noDt = 1 + (currentPage - 1) * pageSize;

            foreach (var item in product)
            {
                dt.Rows.Add(item.Id, noDt, item.Name, item.Type == 1 ? "PVM(4.6V)" : "NONE(6V)", item.Voltage_min, item.Voltage_max, item.Current_min, item.Current_max, item.ImageFile, item.CreatedAt, item.UpdatedAt);
                noDt++;
            }

            Extensions.SetDataSourceAndUpdateSelection(dgvProduct,dt, visibleColumns: new string[] { "Id", "CreatedAt", "Image" } );
            Extensions.SelectedRow(dgvProduct, selectedRow);
            lblTotalData.Text = $"Total Data : {totalData} | Page : {currentPage}/{totalPage}";
            toolStripStatusMessage.Text = "Data loaded.";
            btnProvice.Enabled = currentPage > 1;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "")
            {
                MessageBox.Show("Name is required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {

                if(btnSave.Text == "Save")
                {
                    btnSave.Text = "Saving...";
                    if (SQLite.Product.IsNameExist(txtName.Text))
                    {
                        throw new Exception("This name is already in use.");
                    }
                    // Save
                    using(var p = new SQLite.Product())
                    {
                        p.Name = txtName.Text;
                        p.Type = cmbType.SelectedIndex;
                        p.Voltage_min = Convert.ToInt32(nmuMinVoltage.Value * 1000);
                        p.Voltage_max = Convert.ToInt32(nmuMaxVoltage.Value * 1000);
                        p.Current_min = Convert.ToInt32(nmuMinCurrent.Value * 1000);
                        p.Current_max = Convert.ToInt32(nmuMaxCurrent.Value * 1000);
                        p.Save();
                    }

                }
                else
                {
                    // Update
                    if(productSeleted == null)
                    {
                        MessageBox.Show("CCD PVM not selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (SQLite.Product.IsNameExist(txtName.Text, productSeleted.Id))
                    {
                        throw new Exception("This name is already in use.");
                    }

                    using (var p = SQLite.Product.Get(productSeleted.Id))
                    {
                        p.Name = txtName.Text;
                        p.Type = cmbType.SelectedIndex;
                        p.Voltage_min = Convert.ToInt32(nmuMinVoltage.Value * 1000);
                        p.Voltage_max = Convert.ToInt32(nmuMaxVoltage.Value * 1000);
                        p.Current_min = Convert.ToInt32(nmuMinCurrent.Value * 1000);
                        p.Current_max = Convert.ToInt32(nmuMaxCurrent.Value * 1000);
                        p.Update();
                    }
                }

            }catch(Exception ex)
            {
                Logger.Error(ex.Message);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                productSeleted = null;
                dgvProduct.ClearSelection();

                RenderDGV();
                btnSave.Text = "Save";
                btnNew.PerformClick();
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            txtName.Text = "";
            // cmbType.SelectedIndex = 0;
            //nmuMinVoltage.Value = 0;
            //nmuMaxVoltage.Value = 8;
            //nmuMinCurrent.Value = 0;
            //nmuMaxCurrent.Value = 3;
            productSeleted = null;

            dgvProduct.ClearSelection();
            btnSave.Text = "Save";
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

            if(dgvProduct.SelectedRows.Count == 0)
            {
                MessageBox.Show("CCD PVM not selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try{

            var id = dgvProduct.SelectedRows[0].Cells["Id"].Value;
            using (var p = SQLite.Product.Get(Convert.ToInt32(id)))
            {
                try
                {
                    string path = System.IO.Path.Combine(Main.path, "images", p.ImageFile);
                    if(System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }

                p.Delete();
            }
            }catch(Exception ex)
            {
                Logger.Error(ex.Message);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                productSeleted = null;
                RenderDGV();

                if(btnSave.Text == "Update")
                {
                    btnNew.PerformClick();
                }
            }
        }

        private void dgvProduct_DoubleClick(object sender, EventArgs e)
        {
            // check if header clicked and selected row
            if(dgvProduct.SelectedRows.Count == 0)
            {
                btnSave.Text = "Save";
                this.productSeleted = null;
                return;
            }

            var id = dgvProduct.SelectedRows[0].Cells["Id"].Value;

            if (id == null)
            {
                toolStripStatusMessage.Text = "CCD PVM not selected.";
                return;
            }

            productSeleted = SQLite.Product.Get(Convert.ToInt32(id));
            if (productSeleted == null)
            {
                toolStripStatusMessage.Text = "CCD PVM not found.";
                return;
            }

            txtName.Text = productSeleted.Name;
            cmbType.SelectedIndex = productSeleted.Type;
            nmuMinVoltage.Value = productSeleted.Voltage_min / 1000;
            nmuMaxVoltage.Value = productSeleted.Voltage_max / 1000;
            nmuMinCurrent.Value = productSeleted.Current_min / 1000;
            nmuMaxCurrent.Value = productSeleted.Current_max / 1000;
            btnSave.Text = "Update";

            toolStripStatusMessage.Text = "Product selected.";
        }

        private void dgvProduct_SelectionChanged(object sender, EventArgs e)
        {
            toolStripStatusMessage.Text = "";
            pictureBox1.Image?.Dispose();
            pictureBox1.Image = null;

            if (dgvProduct.SelectedRows.Count == 0)
            {
                btnDelete.Enabled = false;
                btnImage.Enabled = false;
            }
            else{
                btnDelete.Enabled = true;
                btnImage.Enabled = true;
            }

            if (dgvProduct.SelectedRows.Count == 0)
            {
                return;
            }

            var id = dgvProduct.SelectedRows[0].Cells["Id"].Value;

            if (id == null)
            {
                toolStripStatusMessage.Text = "CCD PVM not selected.";
                return;
            }

            productSeleted = SQLite.Product.Get(Convert.ToInt32(id));

            LoadImage(productSeleted.ImageFile);

            dgvProduct_DoubleClick(sender, e);
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ProductType = cmbType.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(btnNew.Enabled == true)
            {
                btnNew.PerformClick();
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(dgvProduct.SelectedRows.Count == 0)
            {
                MessageBox.Show("CCD PVM not selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.dgvProduct_DoubleClick(sender, e);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(btnDelete.Enabled == true)
            {
                btnDelete.PerformClick();
            }
        }

        private void btnImage_Click(object sender, EventArgs e)
        {
            if(productSeleted == null)
            {
                MessageBox.Show("CCD PVM not selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var boxImage = new BoxImage(productSeleted.Id))
            {
                boxImage.ShowDialog();
            }


            productSeleted = SQLite.Product.Get(productSeleted.Id);

            LoadImage(productSeleted.ImageFile);
        }



        private void LoadImage(string _path)
        {
            pictureBox1.Image?.Dispose();
            pictureBox1.Image = null;

            try
            {
                string path = System.IO.Path.Combine(Main.path, "images", _path);
                if (!string.IsNullOrEmpty(_path) && _path != "" && _path != null)
                {
                    if (System.IO.File.Exists(path))
                    {
                        using (var bmpTemp = new Bitmap(path))
                        {
                            pictureBox1.Image = new Bitmap(bmpTemp);
                        }

                    }
                }
            }catch(Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {

            if (currentPage < totalPage)
            {
                currentPage++;
                RenderDGV();
            }
        }

        private void btnProvice_Click(object sender, EventArgs e)
        {
            if(currentPage > 1)
            {
                currentPage--;
                RenderDGV();
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            this.toolStripStatusMessage.Text = "Searching...";
            this.currentPage = 1;
            
            timerSearch.Stop();
            timerSearch.Start();
        }

        private void timerSearch_Tick(object sender, EventArgs e)
        {
            timerSearch.Stop();
            RenderDGV();
        }

    }
}
