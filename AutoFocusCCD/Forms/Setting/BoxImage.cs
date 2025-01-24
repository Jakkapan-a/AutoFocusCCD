using AutoFocusCCD.Components;
using GitHub.secile.Video;
using Multi_Camera_MINI_AOI_V3.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoFocusCCD.Forms.Setting
{
    public partial class BoxImage : Form
    {
        private static readonly Logger Logger = Main.Logger;

        private int ProductId = -1;
        private SQLite.Product product = null;
        private int boxesId = -1;
        public BoxImage(int productId)
        {
            InitializeComponent();
            ProductId = productId;


            product = SQLite.Product.Get(ProductId);
        }

        private DataTable dt;
        private int noDt = 1;
        //private int pageSize = 100;
        //private int currentPage = 1;
        //private int totalPage = 0;
        //private int totalData = 0;

        private GitHub.secile.Video.UsbCamera camera = null;

        //private int index = -1;
        //private int formatIndex = -1;

        private Image imageOriginal;

        private void BoxImage_Load(object sender, EventArgs e)
        {
            if(product == null)
            {
                MessageBox.Show("Product not found.");
                this.Close();
                return;
            }

            LoadDevices();

            if(product.ImageFile != null)
            {
                this.imagePath = System.IO.Path.Combine(Main.path, "images", product.ImageFile);
            }
            LoadImage();


            dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("No", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("X", typeof(int));
            dt.Columns.Add("Y", typeof(int));
            dt.Columns.Add("Width", typeof(int));
            dt.Columns.Add("Height", typeof(int));
            dt.Columns.Add("ProductId", typeof(int));
            dt.Columns.Add("YoloModelId", typeof(int));
            dt.Columns.Add("YoloModelName", typeof(string));
            dt.Columns.Add("Last Update", typeof(string));

            RenderDGVBoxes();

            Logger.Info($"Product: {product.Name}");

        }

        private string imagePath ="";

        private void LoadImage()
        {
            scrollablePictureBox1.Image?.Dispose();
            scrollablePictureBox1.Image = null;
            if (imagePath != "")
            {
                if (System.IO.File.Exists(imagePath))
                {
                    using (var bmpTemp = new Bitmap(imagePath))
                    {
                        scrollablePictureBox1.Image = new Bitmap(bmpTemp);
                        imageOriginal?.Dispose();
                        imageOriginal = new Bitmap(bmpTemp);
                    }
                }
            }
        }

        private void LoadDevices()
        {
            string[] devices = UsbCamera.FindDevices();
            if (devices.Length == 0) return;

            Extensions.RefreshComboBoxWithList(cmbDevices, devices);
        }

        private void cmbDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            int cameraIndex = 0;

            if (cmb.SelectedIndex >= 0)
            {
                cameraIndex = cmb.SelectedIndex;
            }

            var formats = UsbCamera.GetVideoFormat(cameraIndex);

            string[] formatList = new string[formats.Length];

            for (int i = 0; i < formats.Length; i++)
            {
                formatList[i] = formats[i].ToString();
            }

            Extensions.RefreshComboBoxWithList(cmbFormats, formatList);
        }

        private async void btnCapture_Click(object sender, EventArgs e)
        {
            // var camera = new UsbCamera(cameraIndex, format);

            int cameraIndex = cmbDevices.SelectedIndex;
            int formatIndex = cmbFormats.SelectedIndex;

            // Validate
            if (cameraIndex < 0)
            {
                MessageBox.Show("Please select camera.");
                return;
            }

            if (formatIndex < 0)
            {
                MessageBox.Show("Please select format.");
                return;
            }

            var formats = UsbCamera.GetVideoFormat(cameraIndex);

            btnCapture.Enabled = false;
            var format = formats[formatIndex];

            camera = new UsbCamera(cameraIndex, format);

            camera.Start();
            scrollablePictureBox1.Image?.Dispose();
            scrollablePictureBox1.Image = null;
            scrollablePictureBox1.Image = Properties.Resources.Spinner_0_4s_800px;

            await Task.Delay(1000);
            using (var image = camera.GetBitmap())
            {
                scrollablePictureBox1.Image?.Dispose();
                scrollablePictureBox1.Image = null;
                scrollablePictureBox1.Image = new Bitmap(image);

                // Save image

                string path = Main.path;

                if(!string.IsNullOrEmpty(path))
                {
                    path = System.IO.Path.Combine(path, "images");

                    if (!System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }

                    string fileName = $"{Guid.NewGuid().ToString()}.jpg";

                    path = System.IO.Path.Combine(path, fileName);

                    image.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);


                        using(var db = SQLite.Product.Get(product.Id))
                        {
                            string oldFile = db.ImageFile;

                            try{
                                string pathOld = System.IO.Path.Combine(Main.path, "images", oldFile);
                                if(System.IO.File.Exists(pathOld))
                                {
                                    System.IO.File.Delete(pathOld);
                                }
                            }catch(Exception ex)
                            {
                                Logger.Error(ex.Message);
                            }

                            db.ImageFile = fileName;
                            db.Update();
                        }
                }
            }

            camera.Stop();
            btnCapture.Enabled = true;
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadDevices();
        }




        private void RenderDGVBoxes()
        {
            if(dt == null)
            {
                return;
            }

            var boxes = SQLite.Boxes.GetByProductId(ProductId);

            dt.Rows.Clear();            
            noDt = 1;
            int selectedRow = Extensions.GetSelectedRowIndex(dgvBoxes);
            if(boxes != null)
            {
                foreach (var box in boxes)
                {
                    DataRow row = dt.NewRow();
                    row["Id"] = box.Id;
                    row["No"] = noDt++;
                    row["Name"] = box.Name;
                    row["X"] = box.X;
                    row["Y"] = box.Y;
                    row["Width"] = box.Width;
                    row["Height"] = box.Hight;
                    row["ProductId"] = box.ProductId;
                    row["YoloModelId"] = box.YoloModelId;
                    row["YoloModelName"] = box.YoloModelName == "" ? "-" : box.YoloModelName;
                    row["Last Update"] = box.UpdatedAt;
                    dt.Rows.Add(row);

                }
            }

            Extensions.SetDataSourceAndUpdateSelection(dgvBoxes, dt, visibleColumns: new string[] { "Id", "ProductId", "Width" , "Height","X", "Y" , "YoloModelId" });
            Extensions.SelectedRow(dgvBoxes, selectedRow);

            dgvBoxes.Columns["No"].Width = 25;

        }
        private int yoloModelId = 0;
        private void btnNew_Click(object sender, EventArgs e)
        {

            dgvBoxes.ClearSelection();

            boxesId = 0;
            txtName.Text = "";
            txtX.Text = "0";
            txtY.Text = "0";
            txtWidth.Text = "0";
            txtHeight.Text = "0";

            btnSave.Text = "Save";

            this.yoloModelId = 0;
            this.txtYoloModelName.Text = "";

            scrollablePictureBox1.Deselect();
            scrollablePictureBox1.Invalidate();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text == string.Empty || txtName.Text == "" || txtName.BackColor == Color.Red)
            {
                txtName.BackColor = Color.Red;
                return;
            }

            btnSave.Enabled = false;

            try
            {
                if (btnSave.Text == "Save")
                {
                    btnSave.Text = "Save..";

                    if (SQLite.Boxes.IsNameExit(txtName.Text, ProductId))
                    {
                        MessageBox.Show("Name already exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        btnSave.Text = "Save";
                        btnSave.Enabled = true;

                        return;
                    }

                    using (var box = new SQLite.Boxes())
                    {
                        box.Name = txtName.Text;
                        box.ProductId = ProductId;
                        box.X = int.Parse(txtX.Text);
                        box.Y = int.Parse(txtY.Text);
                        box.Width = int.Parse(txtWidth.Text);
                        box.Hight = int.Parse(txtHeight.Text);
                        box.YoloModelId = this.yoloModelId;
                        box.YoloModelName = this.txtYoloModelName.Text == "" ? "-" : this.txtYoloModelName.Text;
                        box.Save();
                    }

                }
                else if (btnSave.Text == "Update" && boxesId != 0)
                {
                    btnSave.Text = "Update..";


                    using (var box = SQLite.Boxes.Get(this.boxesId))
                    {
                        if (box != null)
                        {
                            box.Name = txtName.Text;
                            box.X = int.Parse(txtX.Text);
                            box.Y = int.Parse(txtY.Text);
                            box.Width = int.Parse(txtWidth.Text);
                            box.Hight = int.Parse(txtHeight.Text);
                            box.YoloModelId = this.yoloModelId;
                            box.YoloModelName = this.txtYoloModelName.Text == "" ? "X" : this.txtYoloModelName.Text;
                            box.Update();
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            btnSave.Enabled = true;
            txtName.BackColor = Color.White;
            btnSave.Text = this.yoloModelId == -1 ? "Save" : "Update";

            this.RenderDGVBoxes();
        }

        private void scrollablePictureBox1_Paint(object sender, PaintEventArgs e)
        {
            ScrollablePictureBox picture = (ScrollablePictureBox)sender;

            if (picture != null)
            {
                if (picture.GetRectOriginal() != System.Drawing.Rectangle.Empty)
                {
                    txtX.Text = picture.GetRectOriginal().X.ToString();
                    txtY.Text = picture.GetRectOriginal().Y.ToString();
                    txtWidth.Text = picture.GetRectOriginal().Width.ToString();
                    txtHeight.Text = picture.GetRectOriginal().Height.ToString();
                }
                else
                {
                    txtX.Text = "0";
                    txtY.Text = "0";
                    txtWidth.Text = "0";
                    txtHeight.Text = "0";
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.boxesId < 1) return;

            SQLite.Boxes.Delete(this.boxesId);
            this.RenderDGVBoxes();
            btnNew.PerformClick();
        }


        private void DrawingBoxesImage()
        {
            if (imageOriginal == null) return;

            using (var img = new Bitmap(imageOriginal))
            {
                using (Graphics g = Graphics.FromImage(img))
                {
                    foreach (DataGridViewRow row in dgvBoxes.Rows)
                    {
                        // Selection to continue
                        if (row.Selected) continue;

                        int x = int.Parse(row.Cells["X"].Value.ToString());
                        int y = int.Parse(row.Cells["Y"].Value.ToString());
                        int width = int.Parse(row.Cells["Width"].Value.ToString());
                        int height = int.Parse(row.Cells["Height"].Value.ToString());
                        string name = row.Cells["Name"].Value.ToString();

                        g.DrawRectangle(new Pen(Color.Blue, 3), x, y, width, height);
                        if (name != null)
                        {
                            int w = name?.Length > 1 ? name.Length * 32 : 64;
                            g.FillRectangle(Brushes.Blue, x, y - 40, w, 40);
                            g.DrawString(name, new Font("Arial", 15), Brushes.White, x, y - 30);
                        }
                    }
                }
                scrollablePictureBox1.Image?.Dispose();
                scrollablePictureBox1.Image = new Bitmap(img);
            }
        }

        private void dgvBoxes_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (dgv == null) return;

            if (dgv.SelectedRows.Count > 0)
            {
                var row = dgv.SelectedRows[0];
                if (row != null)
                {
                    this.boxesId = int.Parse(row.Cells["Id"].Value.ToString());
                    txtName.Text = row.Cells["Name"].Value.ToString();
                    txtX.Text = row.Cells["X"].Value.ToString();
                    txtY.Text = row.Cells["Y"].Value.ToString();
                    txtWidth.Text = row.Cells["Width"].Value.ToString();
                    txtHeight.Text = row.Cells["Height"].Value.ToString();
                    this.yoloModelId = int.Parse(row.Cells["YoloModelId"].Value.ToString());
                    this.txtYoloModelName.Text = row.Cells["YoloModelName"].Value.ToString();
                    btnSave.Text = "Update";

                    System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(int.Parse(txtX.Text), int.Parse(txtY.Text), int.Parse(txtWidth.Text), int.Parse(txtHeight.Text));

                    scrollablePictureBox1.SetRect(rectangle, true);
                    btnDelete.Enabled = true;
                }
            }
            else
            {
                this.boxesId = 0;
                txtName.Text = "";
                txtX.Text = "0";
                txtY.Text = "0";
                txtWidth.Text = "0";
                txtHeight.Text = "0";
                this.yoloModelId = 0;
                this.txtYoloModelName.Text = "";
                btnSave.Text = "Save";
                scrollablePictureBox1.SetRect(Rectangle.Empty, true);

                btnDelete.Enabled = false;
            }
            this.DrawingBoxesImage();
        }

        private SelectModel selectModelForm = null;
        private void btnSelectModel_Click(object sender, EventArgs e)
        {
            selectModelForm?.Close();
            selectModelForm?.Dispose();

            selectModelForm = new SelectModel();
            selectModelForm.OnSelect += SelectModelForm_OnSelect;
            selectModelForm.Show();
        }

        private void SelectModelForm_OnSelect(int id, string name)
        {
            txtYoloModelName.Text = name;
            this.yoloModelId = id;
        }
    }
}
