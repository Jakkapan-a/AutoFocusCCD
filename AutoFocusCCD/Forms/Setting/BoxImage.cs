using AutoFocusCCD.Components;
using GitHub.secile.Video;
using Multi_Camera_MINI_AOI_V3.Utilities;
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
        private int ProductId = -1;
        private SQLite.Product product = null;
        public BoxImage(int productId)
        {
            InitializeComponent();
            ProductId = productId;


            product = SQLite.Product.Get(ProductId);
        }


        private GitHub.secile.Video.UsbCamera camera = null;

        //private int index = -1;
        //private int formatIndex = -1;

        private void BoxImage_Load(object sender, EventArgs e)
        {
            if(product == null)
            {
                MessageBox.Show("Product not found.");
                this.Close();
                return;
            }

            LoadDevices();
            this.imagePath = product.ImageFile;
            LoadImage();
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
                    scrollablePictureBox1.Image?.Dispose();
                    scrollablePictureBox1.Image = null;
                    scrollablePictureBox1.Image = Image.FromFile(imagePath);

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
            }

            camera.Stop();
            btnCapture.Enabled = true;
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadDevices();
        }
    }
}
