using AutoFocusCCD.Config;
using AutoFocusCCD.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoFocusCCD.Forms.Setting
{
    public partial class FileManagement : Form
    {
        private static readonly Logger Logger = Main.Logger;

        public FileManagement()
        {
            InitializeComponent();
        }

        private int current_page = 1;
        private int per_page = 20;
        private int total_pages = 0;
        private int total_data = 0;
        private int fileId = 0;

        private void FileManagement_Load(object sender, EventArgs e)
        {
            _ = RenderDGVFile();
        }

        private async Task RenderDGVFile()
        {
            string search = txtSearch.Text;
            string server_url = Main.preferences.Network.URL;
            // check last character of url = '/' or not
            if (server_url[server_url.Length - 1] != '/')
            {
                server_url += "/";
            }
            string url = $"{Main.preferences.Network.URL}/api/v1/filemanager/?search={search}&page={current_page}&per_page={per_page}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    FileApiResponse apiResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<FileApiResponse>(responseBody);
                    Console.WriteLine(apiResponse.status);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error when fetching data from server");
                }
            }
        }
    }
}
