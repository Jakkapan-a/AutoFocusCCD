using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AutoFocusCCD.Utilities
{
    public class HistoryUploadControl
    {
        public int id { get; set; }
        public string name { get; set; }
        public string serial_number { get; set; }
        public string computer { get; set; }
        public string station { get; set; }
        public string model { get; set; }

        private string _url = "";

        public async Task<bool> Create(string url)
        {
            string endpoint = "api/1.0.1/automation/auto-focus/create";
            this._url = url;
            if (!url.EndsWith("/")) url += "/";
            url += endpoint;


            var json = new
            {
                name,
                serial_number,
                computer,
                station,
                model
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.PostAsync(url, jsonContent);
                var responseString = await response.Content.ReadAsStringAsync();

                try
                {
                    HistoryResponse res = JsonConvert.DeserializeObject<HistoryResponse>(responseString);
                    this.id = res.data.id;
                }
                catch (Exception e)
                {
                    return false;
                }
                return response.IsSuccessStatusCode;
            }
        }


        public async Task<bool> UploadImage(Bitmap bmp)
        {
            string endpoint = "api/1.0.1/automation/auto-focus/upload-image";
            string url = this._url;
            if (!url.EndsWith("/")) url += "/";
            url += endpoint;

            using (var client = new HttpClient())
            using (var content = new MultipartFormDataContent())
            {
                // 🔽 ลดขนาดภาพให้ไม่เกิน 1.5MB 🔽
                byte[] compressedImage = CompressImage(bmp, 1.5 * 1024 * 1024); // 1.5MB

                if (compressedImage == null)
                {
                    Console.WriteLine("❌ Failed to compress image!");
                    return false;
                }

                var fileContent = new ByteArrayContent(compressedImage);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                // เพิ่มข้อมูลไปยัง MultipartFormDataContent
                content.Add(new StringContent(id.ToString()), "id");
                //content.Add(new StringContent(result), "result");
                content.Add(fileContent, "image", "upload.jpg");  // 'upload.jpg' คือชื่อไฟล์ที่ส่งไป

                // ส่ง POST Request
                var response = await client.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                Console.WriteLine(responseString);
                return response.IsSuccessStatusCode;
            }

        }

        private byte[] CompressImage(Bitmap bmp, double maxSizeInBytes)
        {
            long quality = 90; // เริ่มต้นที่คุณภาพ 90%
            byte[] imageBytes;

            using (var ms = new MemoryStream())
            {
                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                if (jpgEncoder == null) return null;

                EncoderParameters encoderParams = new EncoderParameters(1);
                EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                encoderParams.Param[0] = qualityParam;

                do
                {
                    ms.SetLength(0); // 
                    bmp.Save(ms, jpgEncoder, encoderParams);
                    imageBytes = ms.ToArray();

                    if (imageBytes.Length <= maxSizeInBytes)
                        return imageBytes;

                    quality -= 10; //
                    if (quality < 10) break; //
                    qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                    encoderParams.Param[0] = qualityParam;

                } while (imageBytes.Length > maxSizeInBytes);
            }

            return imageBytes;
        }


        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            return ImageCodecInfo.GetImageEncoders().FirstOrDefault(codec => codec.FormatID == format.Guid);
        }

        public void ClearData()
        {
            id = 0;
            name = "";
            serial_number = "";
            computer = "";
            station = "";
            model = "";
        }

        // --------------------- 
        public class Data
        {
            public string model { get; set; }
            public DateTime updated_at { get; set; }
            public DateTime created_at { get; set; }
            public int id { get; set; }
        }

        public class HistoryResponse
        {
            public string message { get; set; }
            public Data data { get; set; }
        }
    }

   
}
