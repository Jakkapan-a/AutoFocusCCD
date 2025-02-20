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

        private void InitializeProcess()
        { }

        /*
        private async void StartProcess()
        {
            this._stopwatch.Restart();

            var config = Preferences();
            this.pictureBoxPredict.Visible = true;
            this.pictureBoxPredict.Image?.Dispose();
            this.pictureBoxPredict.Image = null;
            this.pictureBoxPredict.Image = Properties.Resources.Spinner_0_4s_800px;
            try
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
                    path = Path.Combine(pathSys, "images" , _product.Name, date, this.txtQr.Text);
                }
                // Create Directory
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                else
                {
                    path = Path.Combine(path, DateTime.Now.ToString("HH-mm-ss"));
                    Directory.CreateDirectory(path);
                }

                this.lbTitle.Text = "Processing...";
                LogAppendText("", true);
                LogAppendText("+++ Start Testing +++");

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
                await Task.Delay(10);
                bool summaryResult = true;

                // ------------------------ Check Current ------------------------- //
                LogAppendText("Check Voltage...");
                float voltage = sensorData1.voltage_V > 0 ? sensorData1.voltage_V : 0;
                float voltage_min = (_product.Voltage_min / 1000);
                float voltage_max = (_product.Voltage_max / 1000);
                if (voltage < voltage_min || voltage > voltage_max)
                {
                    summaryResult = false;
                    LogAppendText($"Voltage is out of range {voltage} ({voltage_min} - {voltage_max}) NG");
                }
                else
                {
                    LogAppendText($"Voltage is in range {voltage} ({voltage_min} - {voltage_max}) OK");
                }
                LogAppendText("Check Current...");

                float current = sensorData1.current_mA > 0 ? sensorData1.current_mA : 0;
                float currnet_min = (_product.Current_min / 1000);
                float currnet_max = (_product.Current_max / 1000);

                if (current < currnet_min || current > currnet_max)
                {
                    summaryResult = false;
                    LogAppendText($"Current is out of range {current} ({currnet_min} - {currnet_max}) NG");
                }
                else
                {
                    LogAppendText($"Current is in range {current} ({currnet_min}  -  {currnet_max}) OK");
                }

                // -------------------------- Main process -------------------------- //

                using (var img = GetBitmap())
                using (var displayImage = new Bitmap(img))
                {                  
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
                    using (var font = new Font("Arial", 17))
                    using (Graphics g = Graphics.FromImage(displayImage))
                    {
                        if(boxes.Count() == 0)
                        {
                            LogAppendText("=== No box found ===");
                            summaryResult = false;
                        }

                        foreach (var box in boxes)
                        {
                            this.lbTitle.Text = $"=== {box.Name} ===";
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
                                    LogAppendText($"**** {box.Name} ****");
                                    testRelult = true;
                                    // call api
                                    string fileNamePath = Path.Combine(path, filename);
                                    DetectionResult res = await Predict(fileNamePath, box);
                                    if (res != null)
                                    {
                                        Color bg = Color.Blue;
                                        var topDetections = res.Result.OrderByDescending(d => d.Confidence).Take(1);
                                        if(topDetections.Count() == 0)
                                        {
                                            LogAppendText("No detection.");
                                            testRelult = false;
                                        }
                                        foreach (var item in topDetections)
                                        {
                                            
                                            float threshold = config.Processing.Threshold;
                                            float confod = item.Confidence * 100;
                                            if (confod < threshold)
                                            {
                                                testRelult = false;
                                                LogAppendText($"{item.Name} - Confidence is lower than threshold {item.Confidence:F2} < {config.Processing.Threshold:F2}");
                                                continue;
                                            }
                                            
                                            RectangleF rect = new RectangleF(boxCut.X + item.X, boxCut.Y + item.Y, item.Width, item.Height);
                                         
#if DEBUG
                                            // item.Name = item.Name + "_OK";
#endif
                                            string txtStr = item.Name.Replace("_OK", "").Replace("_ok", "").Replace("_NG", "").Replace("_ng", "");
                                            // Check result is OK or NG
                                            testRelult = IsKey("OK", item.Name);
                                            string resultStr = $"{box.Name} - {(testRelult ? "OK" : "NG")} {item.Confidence:F2}"; ;
                                            LogAppendText(resultStr);
                                            txtStr = $"{txtStr} - {item.Confidence:F2}";
                                            int w = txtStr?.Length > 1 ? txtStr.Length * 24 : 32;
                                            
                                            bg = testRelult ? Color.Green : Color.Red;
                                            g.DrawRectangle(new Pen(bg, 3), rect.X, rect.Y, rect.Width, rect.Height);
                                            g.FillRectangle(testRelult ? Brushes.Green : Brushes.Red, rect.X, rect.Y - 40, w, 40);
                                            g.DrawString(txtStr, font, Brushes.White, rect.X, rect.Y - 35);
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

                                if (testRelult == false && summaryResult == true)
                                {
                                    summaryResult = false;
                                }
                            }
                            // Draw box
                            await Task.Delay(1);

                            if (config.Other.Rectangle)
                            {
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
                            }
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
                    // Save image box
                    string filenameBox = $"BOX_{Guid.NewGuid().ToString().Substring(0, 5)}.jpg";
                    displayImage.Save(Path.Combine(path, filenameBox));

                    // End process
                }
                // Update UI

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

                    if (_product.IsByPass == 1 || config.Other.ByPass)
                    {
                        LogAppendText("***** BY PASS *****");
                        summaryResult = true;
                    }

                    this._stopwatch.Stop();
                    LogAppendText($"Time elapsed: {_stopwatch.ElapsedMilliseconds} ms");

                    if (summaryResult)
                    {
                        this.lbTitle.Text = "OK";
                        this.lbTitle.BackColor = Color.Green;
                        this.lbTitle.ForeColor = Color.White;

                        this.deviceControl.SetLED(DeviceControl.Mode2Type.LED_GREEN, true);
                        this.SendText(txtQr.Text);
                        history.result = "OK";

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
                        history.result = "NG";
                    }
                    
                    if (_product.IsByPass == 1 || config.Other.ByPass)
                    {
                        this.lbTitle.Text = "Please visual check";
                        this.lbTitle.BackColor = Color.BlueViolet;
                        this.lbTitle.ForeColor = Color.White;
                    }

                    await history.SaveSaync();
                }

                this.txtLog.SaveFile(Path.Combine(path, "log.txt"));
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
        
        */
        
        
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

                    string url = Preferences().Network.URL + "/api/v1/predict/";

                    var response = await client.PostAsync(url, content);
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

                using (var img = GetBitmap())
                using (var displayImage = new Bitmap(img))
                {
                    string imgPath = SaveImage(img, path);
                    this.pictureBoxPredict.Image?.Dispose();
                    this.pictureBoxPredict.Image = new Bitmap(displayImage);

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
