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
    public partial class Product : Form
    {
        public Product()
        {
            InitializeComponent();
        }

        private void Product_Load(object sender, EventArgs e)
        {
            CreateDt();
            RenderDGV();

            cmbType.SelectedIndex = Properties.Settings.Default.ProductType;
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
                dt.Rows.Add(item.Id, noDt, item.Name, item.Type == 1 ? "PVM" : "NONE", item.Voltage_min, item.Voltage_max, item.Current_min, item.Current_max, item.ImageFile, item.CreatedAt, item.UpdatedAt);
                noDt++;
            }

            Extensions.SetDataSourceAndUpdateSelection(dgvProduct,dt, visibleColumns: new string[] { "Id", "CreatedAt", "Image" } );
            Extensions.SelectedRow(dgvProduct, selectedRow);

            lblTotalData.Text = $"Total Data : {totalData} | Page : {currentPage}/{totalPage}";
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
                        p.Voltage_min = Convert.ToInt32(nmuMinVoltage.Value);
                        p.Voltage_max = Convert.ToInt32(nmuMaxVoltage.Value);
                        p.Current_min = Convert.ToInt32(nmuMinCurrent.Value);
                        p.Current_max = Convert.ToInt32(nmuMaxCurrent.Value);
                        p.Save();
                    }

                }
                else
                {
                    // Update
                    if(productSeleted == null)
                    {
                        MessageBox.Show("Product not selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        p.Voltage_min = Convert.ToInt32(nmuMinVoltage.Value);
                        p.Voltage_max = Convert.ToInt32(nmuMaxVoltage.Value);
                        p.Current_min = Convert.ToInt32(nmuMinCurrent.Value);
                        p.Current_max = Convert.ToInt32(nmuMaxCurrent.Value);
                        p.Update();
                    }
                }

            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                productSeleted = null;
                btnSave.Text = "Save";
                RenderDGV();
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            txtName.Text = "";
            cmbType.SelectedIndex = 0;
            nmuMinVoltage.Value = 0;
            nmuMaxVoltage.Value = 0;
            nmuMinCurrent.Value = 0;
            nmuMaxCurrent.Value = 0;
            productSeleted = null;


            dgvProduct.ClearSelection();
            btnSave.Text = "Save";
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

            if(dgvProduct.SelectedRows.Count == 0)
            {
                MessageBox.Show("Product not selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try{

            var id = dgvProduct.SelectedRows[0].Cells["Id"].Value;
            using (var p = SQLite.Product.Get(Convert.ToInt32(id)))
            {
                p.Delete();
            }
            }catch(Exception ex)
            {
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
                return;
            }

            var id = dgvProduct.SelectedRows[0].Cells["Id"].Value;

            if (id == null)
            {
                toolStripStatusMessage.Text = "Product not selected.";
                return;
            }

            productSeleted = SQLite.Product.Get(Convert.ToInt32(id));
            if (productSeleted == null)
            {
                toolStripStatusMessage.Text = "Product not found.";
                return;
            }

            txtName.Text = productSeleted.Name;
            cmbType.SelectedIndex = productSeleted.Type;
            nmuMinVoltage.Value = productSeleted.Voltage_min;
            nmuMaxVoltage.Value = productSeleted.Voltage_max;
            nmuMinCurrent.Value = productSeleted.Current_min;
            nmuMaxCurrent.Value = productSeleted.Current_max;
            btnSave.Text = "Update";

            toolStripStatusMessage.Text = "Product selected.";
        }

        private void dgvProduct_SelectionChanged(object sender, EventArgs e)
        {
            toolStripStatusMessage.Text = "";
            if(dgvProduct.SelectedRows.Count == 0)
            {
                btnDelete.Enabled = false;
            }else{
                btnDelete.Enabled = true;
            }
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ProductType = cmbType.SelectedIndex;
            Properties.Settings.Default.Save();
        }
    }
}
