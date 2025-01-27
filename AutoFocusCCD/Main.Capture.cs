using GitHub.secile.Video;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace AutoFocusCCD
{
    partial class Main
    {
        private GitHub.secile.Video.UsbCamera camera = null;
        private System.Timers.Timer timerCapture;
        private int index = -1;
        private int formatIndex = -1;

        private readonly object _lockObject = new object();
        private bool _isDisposed = false;

        private void InitializeCapture()
        {
            timerCapture = new System.Timers.Timer();
            timerCapture.Interval = 1000 / 30; // 30fps
            timerCapture.Elapsed += TimerCapture_Elapsed;
            timerCapture.SynchronizingObject = this;
        }

        private void TimerCapture_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (camera == null) return;

            if(camera.IsReady)
            {
                UpdateCameraImage();
            }
        }

        private void UpdateCameraImage()
        {
            try
            {
                using (var bmp = camera.GetBitmap())
                {
                    if (bmp == null) return;
                    var oldImage = pictureBoxCamera.Image;
                    pictureBoxCamera.Image = new Bitmap(bmp);
                    oldImage?.Dispose();
                }
            }
            catch(Exception ex)
            {
                Logger.Error("Error updating camera image: " + ex.Message);
            }
        }

        private void ConnectCamera()
        {
            if (cmbDevices.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a device first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int index = cmbDevices.SelectedIndex;

            if(cmbFormats.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a format first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            camera.SetPreviewControl(pictureBoxCamera.Handle, pictureBoxCamera.ClientSize);
            pictureBoxCamera.Resize += (s, ev) => camera.SetPreviewSize(pictureBoxCamera.ClientSize); // support resize.

            // start.
            camera.Start();
            //timerCapture.Start();
        }

        public void Stop()
        {
            if (camera != null)
            {
                camera.Release();
            }

            if (timerCapture != null)
            {
                timerCapture.Stop();
            }

            var oldImage = pictureBoxCamera.Image;
            pictureBoxCamera.Image = null;
            oldImage?.Dispose();
        }

        public Bitmap GetBitmap()
        {
            if (camera == null) return null;

            return camera.GetBitmap();
        }

    }
}