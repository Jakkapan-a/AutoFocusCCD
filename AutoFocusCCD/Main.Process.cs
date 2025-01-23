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
                using(var img = camera.GetBitmap())
                {
                    // Init process
                    string date = DateTime.Now.ToString("dd-MMM-yyyy");
                    string path = Path.Combine(Preferences().FileSystem.Path,_product.Name,date,this.txtQr.Text);
                    this.pictureBoxPredict.Image?.Dispose();
                    this.pictureBoxPredict.Image = null;
                    this.pictureBoxPredict.Image = new Bitmap(img);
                    if(!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }else
                    {
                        path = Path.Combine(path,DateTime.Now.ToString("HH-mm-ss"));
                        Directory.CreateDirectory(path);
                    }
                    // Save image
                    string imgPath = Path.Combine(path,"ORG_"+Guid.NewGuid().ToString() + ".jpg");
                    img.Save(imgPath);

                    List<SQLite.Boxes> boxes = SQLite.Boxes.GetByProductId(_product.Id);
                    
                    if(boxes == null)
                    {
                        Logger.Error("Box not found.");
                        return;
                    }

                    // Main process
                    using (Graphics g = Graphics.FromImage(img))
                    {
                        foreach (var box in boxes)
                        {
                            // Crop image
                            string name = box.Name;
                            Rectangle boxCut = new Rectangle(box.X, box.Y, box.Width, box.Hight);
                            // Call API

                            // Draw box
                            await Task.Delay(10);
                            Color backgroundColor = result ? Color.Green : Color.Red;
                            Color foregroundColor = Color.Black;
                            g.DrawRectangle(new Pen(backgroundColor, 3), box.X, box.Y, box.Width, box.Hight);

                            if (name != null)
                            {
                                int w = name?.Length > 1 ? name.Length * 32 : 64;
                                g.FillRectangle(result ? Brushes.Green : Brushes.Red, box.X, box.Y - 40, w, 40);
                                g.DrawString(name, new Font("Arial", 30), Brushes.White, box.X, box.Y - 40);
                            }
                            this.pictureBoxPredict.Image?.Dispose();
                            this.pictureBoxPredict.Image = new Bitmap(img);

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
            }
            finally
            {
                // this.pictureBoxPredict.Visible = false;
            }
        }
    }
}
