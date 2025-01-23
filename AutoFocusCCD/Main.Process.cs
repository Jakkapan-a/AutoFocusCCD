using AutoFocusCCD.Config;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoFocusCCD
{
    partial class Main
    {
        private void InitializeProcess()
        {
        }

        private async void StartProcess()
        {
            this.pictureBoxPredict.Visible = true;
            this.pictureBoxPredict.Image?.Dispose();
            this.pictureBoxPredict.Image = null;
            this.pictureBoxPredict.Image = Properties.Resources.Spinner_0_4s_800px;
            try
            {
                this.lbTitle.Text = "Processing...";
                this.lbTitle.ForeColor = Color.Black;
                this.lbTitle.BackColor = Color.DarkOrange;
                if(this._product == null)
                {
                    this.lbTitle.Text = "Please scan QR code";
                    this.lbTitle.BackColor = Color.Yellow;
                    this.lbTitle.ForeColor = Color.Black;
                    return;
                }
                // Process
                await Task.Delay(100);
                bool result = true;
                using (var img = camera.GetBitmap())
                using (var displayImage = new Bitmap(img))
                {
                    // Init process
                    string date = DateTime.Now.ToString("dd-MMM-yyyy");
                    string pathSys = Preferences().FileSystem.Path;
                    string path = "";

                    string pathDefult = PreferencesConfigLoader.LoadDefault().FileSystem.Path;
                    if (pathDefult == pathSys)
                    {
                        path = Path.Combine(pathSys, "system", "images", _product.Name, date, this.txtQr.Text);
                    }
                    else
                    {
                        path = Path.Combine(pathSys, _product.Name, date, this.txtQr.Text);
                    }

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    } else
                    {
                        path = Path.Combine(path, DateTime.Now.ToString("HH-mm-ss"));
                        Directory.CreateDirectory(path);
                    }
                    // Save image
                    string imgPath = Path.Combine(path, $"ORG_${Guid.NewGuid().ToString().Substring(0, 5)}.jpg");
                    img.Save(imgPath);

                    this.pictureBoxPredict.Image?.Dispose();
                    this.pictureBoxPredict.Image = new Bitmap(displayImage);

                    List<SQLite.Boxes> boxes = SQLite.Boxes.GetByProductId(_product.Id);
                    
                    if(boxes == null)
                    {
                        Logger.Error("Box not found.");
                        return;
                    }

                    // Main process
                    using (var font = new Font("Arial", 30))
                    using (Graphics g = Graphics.FromImage(displayImage))
                    {
                        foreach (var box in boxes)
                        {
                            // Crop image
                            string name = box.Name;
                            Rectangle boxCut = new Rectangle(box.X, box.Y, box.Width, box.Hight);
                            // Cut image
                            using (Bitmap imgBox = CropBitmap(img, boxCut.X, boxCut.Y, boxCut.Width, boxCut.Height))
                            {
                                string filename = $"P{box.Id}{box.Name}_{Guid.NewGuid().ToString().Substring(0, 5)}.jpg";
                                imgBox.Save(System.IO.Path.Combine(path, filename));

                                // Process detect


                                // Draw box and result
                            }
                            // Draw box
                            await Task.Delay(10);
                            Color backgroundColor = result ? Color.Green : Color.Red;
                            Color foregroundColor = Color.Black;
                            g.DrawRectangle(new Pen(backgroundColor, 3), box.X, box.Y, box.Width, box.Hight);

                            if (name != null)
                            {
                                int w = name?.Length > 1 ? name.Length * 32 : 64;
                                g.FillRectangle(result ? Brushes.Green : Brushes.Red, box.X, box.Y - 40, w, 40);
                                g.DrawString(name, font, Brushes.White, box.X, box.Y - 40);
                            }
                            var newImage = new Bitmap(displayImage);
                            this.pictureBoxPredict.Image?.Dispose();
                            this.pictureBoxPredict.Image = newImage;

                            await Task.Delay(10);
                        }
                    }

                    // End process
                }
                // Update UI

                //
                Console.WriteLine("Complate...");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Console.WriteLine("Error processing : " +ex.Message);
            }
            finally
            {
                //if (this.pictureBoxPredict.Image != null)
                //{
                //    this.pictureBoxPredict.Image.Dispose();
                //    this.pictureBoxPredict.Image = null;
                //}
            }
        }

        private Bitmap CropBitmap(Bitmap source, int x, int y, int width, int height)
        {
            var crop = new Bitmap(width, height);
            using (var g = Graphics.FromImage(crop))
            {
                g.DrawImage(source, -x, -y);
            }
            return crop;
        }
    }
}
