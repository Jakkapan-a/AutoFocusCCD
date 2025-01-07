using GitHub.secile.Video;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoFocusCCD
{
    partial class Main
    {
        private GitHub.secile.Video.UsbCamera camera = null;

        private int index = -1;
        private int formatIndex = -1;

        private void ConnectCamera()
        {
            if (cmbDevices.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a device first.");
                return;
            }

            int index = cmbDevices.SelectedIndex;

            if(cmbFormats.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a format first.");
                return;
            }

            int formatIndex = cmbFormats.SelectedIndex;

            this.Start(index, formatIndex);
        }

        public void Start(int index, int formatIndex)
        {
            if (camera != null)
            {
                camera.Release();
            }
            this.index = index;
            this.formatIndex = formatIndex;


            var formats = UsbCamera.GetVideoFormat(index);

            if (formatIndex >= formats.Length)
            {
                formatIndex = 0;
            }

            var format = formats[formatIndex];
            camera = new UsbCamera(index, format);

            camera.SetPreviewControl(pictureBox.Handle, pictureBox.ClientSize);
            pictureBox.Resize += (s, ev) => camera.SetPreviewSize(pictureBox.ClientSize); // support resize.

            // start.
            camera.Start();
        }

        public void Stop()
        {
            if (camera != null)
            {
                camera.Release();
            }
        }

        public Bitmap GetBitmap()
        {
            if (camera == null) return null;

            return camera.GetBitmap();
        }

    }
}