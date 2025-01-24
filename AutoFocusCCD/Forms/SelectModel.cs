using AutoFocusCCD.Utilities;
using Multi_Camera_MINI_AOI_V3.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoFocusCCD.Forms
{
    public partial class SelectModel : Form
    {
        private readonly Logger Logger = Main.Logger;

        private readonly string URL = "";
        public SelectModel()
        {
            InitializeComponent();

            string server_url = Main.Preferences().Network.URL;
            // check last character of url = '/' or not
            if (server_url[server_url.Length - 1] == '/')
            {
                server_url = server_url.Remove(server_url.Length - 1, 1);
            }
            URL = server_url;
        }

        private DataTable dtFiles;
        private int current_page = 1;
        private int per_page = 20;
        private int total_pages = 0;
        private int total_data = 0;
        private int fileId = 0;
        public delegate void SetNameDoubleClickHandler(int id, string name);
        public event SetNameDoubleClickHandler OnSelect;


        private void SelectModel_Load(object sender, EventArgs e)
        {
            dtFiles = new DataTable();
            dtFiles.Columns.Add("id", typeof(int));
            dtFiles.Columns.Add("no", typeof(int));
            dtFiles.Columns.Add("name", typeof(string));
            dtFiles.Columns.Add("filename", typeof(string));
            dtFiles.Columns.Add("image_name", typeof(string));
            dtFiles.Columns.Add("description", typeof(string));
            dtFiles.Columns.Add("file_type", typeof(string));
            dtFiles.Columns.Add("created_at", typeof(string));
            dtFiles.Columns.Add("updated_at", typeof(string));

            _ = RenderDGVFile();

        }

        private async Task RenderDGVFile()
        {
            string search = txtSearch.Text;
            string url = $"{this.URL}/api/v1/filemanager/?search={search}&page={current_page}&per_page={per_page}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    FileApiResponse apiResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<FileApiResponse>(responseBody);
                    //Console.WriteLine(apiResponse.status);

                    if (apiResponse.status == "success")
                    {
                        int selectedRow = Extensions.GetSelectedRowIndex(dgvFile);
                        dtFiles.Clear();
                        int no = 1;
                        no = apiResponse.data.pagination.per_page * (apiResponse.data.pagination.current_page - 1) + 1;
                        foreach (Item item in apiResponse.data.items)
                        {
                            DataRow row = dtFiles.NewRow();
                            row["id"] = item.id;
                            row["no"] = no;
                            row["name"] = item.name;
                            row["filename"] = item.filename;
                            row["image_name"] = item.image_name;
                            row["description"] = item.description;
                            row["file_type"] = item.file_type;
                            row["created_at"] = item.created_at;
                            row["updated_at"] = item.updated_at;
                            dtFiles.Rows.Add(row);
                            no++;
                        }

                        Extensions.SetDataSourceAndUpdateSelection(dgvFile, dtFiles, visibleColumns: new string[] { "id", "image_name", "filename", "updated_at" });
                        Extensions.SelectedRow(dgvFile, selectedRow);

                        toolStripStatusLabel.Text = $"Total: {apiResponse.data.pagination.total_items} items, pages: {apiResponse.data.pagination.current_page}/{apiResponse.data.pagination.total_pages}";

                        btnPrevious.Enabled = apiResponse.data.pagination.has_prev;
                        btnNext.Enabled = apiResponse.data.pagination.has_next;

                        // Set header text
                        dgvFile.Columns["no"].HeaderText = "No";
                        dgvFile.Columns["no"].Width = 30;
                        dgvFile.Columns["name"].HeaderText = "Name";
                        dgvFile.Columns["filename"].HeaderText = "Filename";
                        dgvFile.Columns["image_name"].HeaderText = "Image Name";
                        dgvFile.Columns["description"].HeaderText = "Description";
                        dgvFile.Columns["created_at"].HeaderText = "Created At";
                        dgvFile.Columns["updated_at"].HeaderText = "Updated At";

                    }
                    else
                    {
                        MessageBox.Show(apiResponse.message);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error when fetching data from server");
                }
            }
        }

        private async void btnPrevious_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn == null) return;

            btn.Enabled = false;
            current_page--;
            await RenderDGVFile();
            btn.Enabled = true;
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn == null) return;

            btn.Enabled = false;
            current_page++;
            await RenderDGVFile();
            btn.Enabled = true;
        }

        private async void txtSearch_TextChanged(object sender, EventArgs e)
        {
            await RenderDGVFile();
        }

        private void dgvFile_DoubleClick(object sender, EventArgs e)
        {
            if (dgvFile.SelectedRows.Count > 0)
            {
                fileId = int.Parse(dgvFile.SelectedRows[0].Cells["id"].Value.ToString());
                string name = dgvFile.SelectedRows[0].Cells["name"].Value.ToString();
                OnSelect?.Invoke(fileId, name);
                this.Close();
            }
        }
    }
}
