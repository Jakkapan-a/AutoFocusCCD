using AutoFocusCCD.Config;
using AutoFocusCCD.SQLite;
using AutoFocusCCD.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoFocusCCD
{
    partial class Main
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(4); // Limit 4 threads
        private HistoryUploadControl historyUploadControl = new HistoryUploadControl();
        private void InitializeProcess()
        { }

        private bool IsKey(string key , string text)
        {
            return text.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void LogAppendText(string txt , bool reset = false)
        {
            if(InvokeRequired)
            {
                Invoke(new Action(() => LogAppendText(txt)));
                return;
            }

            txtLog.AppendText($"{txt}{Environment.NewLine}");

            if(reset)
            {
                txtLog.Text = txt;
            }
        }
       
        public static Bitmap CropBitmap(Bitmap source, int x, int y, int width, int height)
        {
            var crop = new Bitmap(width, height);
            using (var g = Graphics.FromImage(crop))
            {
                g.DrawImage(source, -x, -y);
            }
            return crop;
        }
        
        // ------------------------- New Process ---------------------- //
        private async void StartProcess()
        {
            try
            {
                LogAppendText("",true);
                this._stopwatch.Restart();
                InitializeUI();

                if (!ValidateProduct()) return;
                var config = Preferences();
                string path = PrepareDirectories();
                this.lbTitle.Text = "Processing...";
                this.lbTitle.ForeColor = Color.Black;
                this.lbTitle.BackColor = Color.DarkOrange;

                bool summaryResult = await PerformChecks();
                historyUploadControl.ClearData();
                historyUploadControl.name = txtEmp.Text;
                historyUploadControl.serial_number = txtQr.Text;
                historyUploadControl.computer = Environment.MachineName;
                historyUploadControl.station = config.Other.Station;
                historyUploadControl.model = _product.Name;
                if(historyUploadControl.name != "test")
                {
                    //await historyUploadControl.Create(config.Network.URL2);
                    await historyUploadControl.Create("http://127.0.0.1:8000");
                }
                // -------------------- Process ------------------- //
                using (var img = GetBitmap())
                using (var displayImage = new Bitmap(img))
                {
                    string imgPath = SaveImage(img, path);
                    this.pictureBoxPredict.Image?.Dispose();
                    this.pictureBoxPredict.Image = new Bitmap(displayImage);
                    _ = historyUploadControl.UploadImage(displayImage);

                    List<SQLite.Boxes> boxes = SQLite.Boxes.GetByProductId(_product.Id);
                    if (boxes == null || boxes.Count == 0)
                    {
                        LogAppendText("=== No box found ===");
                        return;
                    }

                    List<Task<bool>> tasks = new List<Task<bool>>();
                    using (var font = new Font("Arial", 14))
                    using (Graphics g = Graphics.FromImage(displayImage))
                    {
                        foreach (var box in boxes)
                        {
                            tasks.Add(ProcessBoxAsync(img, displayImage, g, font, path, box));
                        }

                        bool[] results = await Task.WhenAll(tasks);
                        summaryResult = results.All(r => r);
                    }

                    UpdateeUI(displayImage,"");
                    await Task.Delay(10);

                    SaveFinalImage(displayImage, path);
                    _ = historyUploadControl.UploadImage(displayImage);
                }

                this._stopwatch.Stop();
                LogAppendText($"Time elapsed: {_stopwatch.ElapsedMilliseconds} ms");

                if (_product.IsByPass == 1 || config.Other.ByPass)
                {
                    LogAppendText("***** BY PASS *****");
                    summaryResult = true;
                }


                if (summaryResult)
                {
                    this.lbTitle.Text = "OK";
                    this.lbTitle.BackColor = Color.Green;
                    this.lbTitle.ForeColor = Color.White;

                    this.deviceControl.SetLED(DeviceControl.Mode2Type.LED_GREEN, true);
                    this.SendText(txtQr.Text);

                }
                else
                {
                    this.lbTitle.Text = "NG";
                    this.lbTitle.BackColor = Color.Red;
                    this.lbTitle.ForeColor = Color.White;

                    this.deviceControl.SetLED(DeviceControl.Mode2Type.LED_RED, true);

                    if (config.OptionNG.AllowSendNG)
                    {
                        this.SendText(config.OptionNG.Key);
                        await Task.Delay(config.OptionNG.Delay);
                        this.SendText(txtQr.Text);
                    }
                }

                if (_product.IsByPass == 1 || config.Other.ByPass)
                {
                    this.lbTitle.Text = "Please visual check";
                    this.lbTitle.BackColor = Color.BlueViolet;
                    this.lbTitle.ForeColor = Color.White;
                }

                await SaveHistory(summaryResult, path);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                LogAppendText("Error processing : " + ex.Message);
            }
        }
        
        private void InitializeUI()
        {
            this.pictureBoxPredict.Visible = true;
            this.pictureBoxPredict.Image?.Dispose();
            this.pictureBoxPredict.Image = null;
            this.pictureBoxPredict.Image = Properties.Resources.Spinner_0_4s_800px;
        }

        private void UpdateeUI(Bitmap bmp, string title)
        {
            if(InvokeRequired)
            {
                Invoke(new Action(() => UpdateeUI(bmp,title)));
                return;
            }

            var newImage = new Bitmap(bmp);
            this.pictureBoxPredict.Image?.Dispose();
            this.pictureBoxPredict.Image = newImage;

            this.lbTitle.Text = title;
        }

        private bool ValidateProduct()
        {
            if (this._product == null)
            {
                this.lbTitle.Text = "Please scan QR code";
                this.lbTitle.BackColor = Color.Yellow;
                this.lbTitle.ForeColor = Color.Black;
                return false;
            }
            return true;
        }

        private string PrepareDirectories()
        {
            string date = DateTime.Now.ToString("dd-MMM-yyyy");
            string pathSys = Preferences().FileSystem.Path;
            string path = Path.Combine(pathSys, "images", _product.Name, date, this.txtQr.Text);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                path = Path.Combine(path, DateTime.Now.ToString("HH-mm-ss"));
                Directory.CreateDirectory(path);
            }

            return path;
        }

        private async Task<bool> PerformChecks()
        {
            bool summaryResult = true;

            LogAppendText("Check Voltage...");
            float voltage = sensorData1.voltage_V > 0 ? sensorData1.voltage_V : 0;
            float voltageMin = (_product.Voltage_min / 1000);
            float voltageMax = (_product.Voltage_max / 1000);
            if (voltage < voltageMin || voltage > voltageMax)
            {
                summaryResult = false;
                LogAppendText($"Voltage is out of range {voltage} ({voltageMin} - {voltageMax}) NG");
            }
            else
            {
                LogAppendText($"Voltage is in range {voltage} ({voltageMin} - {voltageMax}) OK");
            }

            await Task.Delay(10);

            LogAppendText("Check Current...");
            float current = sensorData1.current_mA > 0 ? sensorData1.current_mA : 0;
            float currentMin = (_product.Current_min / 1000);
            float currentMax = (_product.Current_max / 1000);
            if (current < currentMin || current > currentMax)
            {
                summaryResult = false;
                LogAppendText($"Current is out of range {current} ({currentMin} - {currentMax}) NG");
            }
            else
            {
                LogAppendText($"Current is in range {current} ({currentMin} - {currentMax}) OK");
            }


            return summaryResult;
        }

        private string SaveImage(Bitmap img, string path)
        {
            string imgPath = Path.Combine(path, $"ORG_{Guid.NewGuid().ToString().Substring(0, 5)}.jpg");
            img.Save(imgPath);
            return imgPath;
        }

        private async Task<bool> ProcessBoxAsync(Bitmap img, Bitmap displayImage, Graphics g, Font font, string path, SQLite.Boxes box)
        {
            bool testResult = true;
            string name = box.Name;
            Rectangle boxCut = new Rectangle(box.X, box.Y, box.Width, box.Hight);

            using (Bitmap imgBox = CropBitmap(img, boxCut.X, boxCut.Y, boxCut.Width, boxCut.Height))
            {
                string filename = $"P{box.Id}{box.Name}_{Guid.NewGuid().ToString().Substring(0, 5)}.jpg";
                imgBox.Save(Path.Combine(path, filename));


                if (box.YoloModelId != 0)
                {

                    string fileNamePath = Path.Combine(path, filename);
                    DetectionResult res = await PredictAsync(fileNamePath, box);
                    LogAppendText($"**** {box.Name} ****");

                    if (res != null)
                    {
                        var topDetections = res.Result.OrderByDescending(d => d.Confidence).Take(1);
                        if (!topDetections.Any())
                        {
                            LogAppendText("No detection.");
                            testResult = false;
                        }

                        foreach (var item in topDetections)
                        {
                            if (item.Confidence * 100 < Preferences().Processing.Threshold)
                            {
                                testResult = false;
                                LogAppendText($"{item.Name} - Confidence too low {item.Confidence:F2}");
                                continue;
                            }

                            RectangleF rect = new RectangleF(boxCut.X + item.X, boxCut.Y + item.Y, item.Width, item.Height);
                            Color bg = IsKey("OK", item.Name) ? Color.Green : Color.Red;
                            g.DrawRectangle(new Pen(bg, 2), rect.X, rect.Y, rect.Width, rect.Height);
                            g.FillRectangle(bg == Color.Green ? Brushes.Green : Brushes.Red, rect.X, rect.Y - 35, 200, 35);
                            g.DrawString($"{item.Name} - {item.Confidence:F2}", font, Brushes.White, rect.X, rect.Y - 35);

                            testResult = IsKey("OK", item.Name);
                            
                            LogAppendText($"{item.Name}: {item.Confidence:F2}");
                            UpdateeUI(displayImage, $"**** {box.Name} ****");
                        }
                    }
                    else
                    {
                        testResult = false;
                        LogAppendText("Service not responding.");
                    }
                }
            }

            return testResult;
        }

        private async Task<DetectionResult> PredictAsync(string filePath, SQLite.Boxes box)
        {
            await _semaphore.WaitAsync(); // จำกัดจำนวนงานที่ทำพร้อมกัน
            try
            {
                using (var client = new HttpClient())
                using (var content = new MultipartFormDataContent())
                {
                    var fileContent = new ByteArrayContent(File.ReadAllBytes(filePath));
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data");
                    content.Add(fileContent, "file", Path.GetFileName(filePath));
                    content.Add(new StringContent(box.YoloModelId.ToString()), "id");

                    string url = Preferences().Network.URL + "/api/v1/predict/";
                    var response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        try
                        {
                            return Newtonsoft.Json.JsonConvert.DeserializeObject<Utilities.DetectionResult>(responseString);
                        }
                        catch (JsonSerializationException ex)
                        {
                            return null;
                        }
                    }
                    return null;
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void SaveFinalImage(Bitmap displayImage, string path)
        {
            string filenameBox = $"BOX_{Guid.NewGuid().ToString().Substring(0, 5)}.jpg";
            displayImage.Save(Path.Combine(path, filenameBox));
        }

        private async Task SaveHistory(bool summaryResult, string path)
        {
            using (var history = new SQLite.History())
            {
                history.employee = txtEmp.Text;
                history.qr_code = txtQr.Text;
                history.voltage = (int)sensorData1.voltage_V;
                history.voltage_min = _product.Voltage_min;
                history.voltage_max = _product.Voltage_max;
                history.current = (int)sensorData1.current_mA;
                history.current_min = _product.Current_min;
                history.current_max = _product.Current_max;
                history.path_folder = path;
                history.product_id = _product.Id;
                history.product_name = _product.Name;
                history.result = summaryResult ? "OK" : "NG";
                await history.SaveSaync();
            }
        }


    }
}
