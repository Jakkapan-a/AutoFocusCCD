using AutoFocusCCD.Config;
using AutoFocusCCD.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
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
            var config = Preferences();
            this.pictureBoxPredict.Visible = true;
            this.pictureBoxPredict.Image?.Dispose();
            this.pictureBoxPredict.Image = null;
            this.pictureBoxPredict.Image = Properties.Resources.Spinner_0_4s_800px;
            try
            {
                this.lbTitle.Text = "Processing...";
                this.lbTitle.ForeColor = Color.Black;
                this.lbTitle.BackColor = Color.DarkOrange;
                if (this._product == null)
                {
                    this.lbTitle.Text = "Please scan QR code";
                    this.lbTitle.BackColor = Color.Yellow;
                    this.lbTitle.ForeColor = Color.Black;
                    return;
                }
                // Process
                await Task.Delay(100);
                bool summaryResult = true;
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
                    }
                    else
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

                    if (boxes == null)
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
                            bool testRelult = true;
                            // Crop image
                            string name = box.Name;
                            Rectangle boxCut = new Rectangle(box.X, box.Y, box.Width, box.Hight);
                            // Cut image
                            using (Bitmap imgBox = CropBitmap(img, boxCut.X, boxCut.Y, boxCut.Width, boxCut.Height))
                            {
                                string filename = $"P{box.Id}{box.Name}_{Guid.NewGuid().ToString().Substring(0, 5)}.jpg";

                                imgBox.Save(System.IO.Path.Combine(path, filename));

                                // Process detect
                                if (box.YoloModelId != 0)
                                {
                                    testRelult = true;
                                    // call api
                                    string fileNamePath = Path.Combine(path, filename);
                                    DetectionResult res = await Predict(fileNamePath, box);

                                    if (res != null)
                                    {
                                        Color bg = Color.Blue;
                                        //g.DrawRectangle(new Pen(bg, 3), box.X, box.Y, box.Width, box.Hight);
                                        var topDetections = res.Result.OrderByDescending(d => d.Confidence).Take(1);
                                        foreach (var item in topDetections)
                                        {
                                            RectangleF rect = new RectangleF(boxCut.X + item.X, boxCut.Y + item.Y, item.Width, item.Height);

                                            string txtStr = item.Name.Replace("_OK", "").Replace("_ok", "").Replace("_NG", "").Replace("_ng", "");

                                            // Check result is OK or NG
                                            testRelult = txtStr.Contains("OK") || txtStr.Contains("ok");

                                            txtStr = $"{txtStr} - {item.Confidence:F2}";
                                            int w = txtStr?.Length > 1 ? txtStr.Length * 32 : 64;

                                            bg = testRelult ? Color.Green : Color.Red;
                                            g.DrawRectangle(new Pen(bg, 3), rect.X, rect.Y, rect.Width, rect.Height);
                                            g.FillRectangle(testRelult ? Brushes.Green : Brushes.Red, rect.X, rect.Y - 40, w, 40);
                                            g.DrawString(txtStr, font, Brushes.White, rect.X, rect.Y - 40);
                                        }
                                    }
                                    else
                                    {
                                        testRelult = false;
                                        LogAppendText("Service not response.");
                                    }

                                }
                                else
                                {
                                    testRelult = false;
                                }
                                // Draw box and result
                            }
                            // Draw box
                            await Task.Delay(10);


                            // Box fill
                            Color backgroundColor = testRelult ? Color.Green : Color.Red;
                            Color foregroundColor = Color.Black;
                            g.DrawRectangle(new Pen(backgroundColor, 3), box.X, box.Y, box.Width, box.Hight);

                            if (name != null)
                            {
                                int w = name?.Length > 1 ? name.Length * 32 : 64;
                                g.FillRectangle(testRelult ? Brushes.Green : Brushes.Red, box.X, box.Y - 40, w, 40);
                                g.DrawString(name, font, Brushes.White, box.X, box.Y - 40);
                            }
                            // End box

                            if (testRelult == false && summaryResult == true) 
                            {
                                summaryResult = false;
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
                LogAppendText("Error processing : " + ex.Message);
                Console.WriteLine("Error processing : " + ex.Message);
            }
            finally
            {

            }
        }

        private void LogAppendText(string txt)
        {
            if(InvokeRequired)
            {
                Invoke(new Action(() => LogAppendText(txt)));
                return;
            }

            txtLog.AppendText($"{txt}{Environment.NewLine}");
        }

        private async Task<DetectionResult> Predict(string filePath, SQLite.Boxes boxes)
        {
            try
            {
                using (var client = new System.Net.Http.HttpClient())
                using (var content = new MultipartFormDataContent())
                {
                    var fileContent = new ByteArrayContent(File.ReadAllBytes(filePath));
                    fileContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("multipart/form-data");
                    content.Add(fileContent, "file", Path.GetFileName(filePath));
                    content.Add(new StringContent(boxes.YoloModelId.ToString()), "id");

                    var response = await client.PostAsync(Preferences().Network.URL + "/api/v1/predict/", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<DetectionResult>(responseString);

                        Console.WriteLine(responseString);
                        return result;
                    }
                    else
                    {
                        Console.WriteLine("Error predict: " + response.StatusCode);
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
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
