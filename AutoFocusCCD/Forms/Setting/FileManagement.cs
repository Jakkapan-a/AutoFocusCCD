using AutoFocusCCD.Config;
using AutoFocusCCD.Utilities;
using Multi_Camera_MINI_AOI_V3.Utilities;
using NLog;
using Ookii.Dialogs.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Extensions = Multi_Camera_MINI_AOI_V3.Utilities.Extensions;

namespace AutoFocusCCD.Forms.Setting
{
    public partial class FileManagement : Form
    {
        private readonly Logger Logger = Main.Logger;
        private readonly string URL = "";

        public FileManagement()
        {
            InitializeComponent();
            string server_url = Main.Preferences().Network.URL;
            // check last character of url = '/' or not
            if (server_url[server_url.Length - 1] == '/')
            {
                // Remove last character
                server_url = server_url.Remove(server_url.Length - 1, 1);
            }
            URL = server_url;
        }

        private int current_page = 1;
        private int per_page = 20;
        private int total_pages = 0;
        private int total_data = 0;
        private int fileId = 0;

        private DataTable dtFiles;

        private void FileManagement_Load(object sender, EventArgs e)
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

            cbType.SelectedIndex = Properties.Settings.Default.File_type;
            current_type = Properties.Settings.Default.File_type;
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

                        Extensions.SetDataSourceAndUpdateSelection(dgvFile, dtFiles, visibleColumns: new string[] { "id","image_name" ,"filename" , "updated_at" });
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = false;

            if (btnSave.Text == "Save")
            {
                if(txtName.Text == "" || txtDescription.Text == "" || txtPath.Text == "")
                {
                    MessageBox.Show("Please fill or select all fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnSave.Enabled = true;
                    return;
                }


                if (!File.Exists(filePath))
                {
                    MessageBox.Show("File not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnSave.Enabled = true;
                    return;
                }

                btnSave.Text = "Saving...";
                progressDialog.WindowTitle = $"Uploading file {txtName.Text}";
                progressDialog.ShowDialog(this);
            }
            else if (btnSave.Text == "Update")
            {
                if(id <= 0)
                {
                    MessageBox.Show("Please select a file to update", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnSave.Enabled = true;
                    return;
                }

                if (txtName.Text == "" || txtDescription.Text == "" || txtPath.Text == "")
                {
                    MessageBox.Show("Please fill or select all fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnSave.Enabled = true;
                    return;
                }

                btnSave.Text = "Updating...";
                progressDialog.WindowTitle = $"Updating file {txtName.Text}";
                progressDialog.ShowDialog(this);

            }
            btnSave.Text = "Save";

            btnSave.Enabled = true;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            vistaOpenFileDialog1.Filter = "pt files (*.pt)|*.pt|All files (*.*)|*.*";
            vistaOpenFileDialog1.FilterIndex = 1;
            vistaOpenFileDialog1.RestoreDirectory = true;

            if (vistaOpenFileDialog1.ShowDialog() == DialogResult.OK) 
            {
                txtPath.Text = vistaOpenFileDialog1.FileName; // 
            }
        }


#if false
        private async Task UploadFileInChunks(string filePath, string serverUrl, string name, string description, int chunkSize, int id = 0)
        {
            using (var client = new HttpClient())
            {
                byte[] buffer = new byte[chunkSize];
                long fileSize = new FileInfo(filePath).Length;
                int totalChunks = (int)Math.Ceiling(fileSize / (double)chunkSize);

                // ตั้งค่า ProgressBar
                //toolStripProgressBar1.Maximum = totalChunks;
                //toolStripProgressBar1.Value = 0;

                string new_filename = Path.GetFileNameWithoutExtension(filePath) + Guid.NewGuid().ToString().Replace("-", "_").Substring(0, 5);

                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    for (int chunkNumber = 0; chunkNumber < totalChunks; chunkNumber++)
                    {
                        // อ่านข้อมูลเป็น chunk
                        int bytesRead = await fileStream.ReadAsync(buffer, 0, chunkSize);
                        if (bytesRead == 0) break;

                        using (var form = new MultipartFormDataContent())
                        {
                            // สร้างเนื้อหาไฟล์ chunk
                            var fileContent = new ByteArrayContent(buffer, 0, bytesRead);
                            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                            // เพิ่มไฟล์ chunk และข้อมูลในฟอร์ม
                            form.Add(fileContent, "file", Path.GetFileName(filePath));
                            form.Add(new StringContent(name), "name");
                            form.Add(new StringContent(description), "description");
                            form.Add(new StringContent(chunkNumber.ToString()), "chunk_number");
                            form.Add(new StringContent(totalChunks.ToString()), "total_chunks");
                            form.Add(new StringContent(new_filename), "filename");
                            if (id > 0)
                            {
                                form.Add(new StringContent(id.ToString()), "id");
                            }

                            // ส่ง POST request ไปยังเซิร์ฟเวอร์
                            var response = await client.PostAsync(serverUrl, form);

                            if (!response.IsSuccessStatusCode)
                            {
                                MessageBox.Show($"Failed to upload chunk {chunkNumber + 1}. Status code: {response.StatusCode}");
                                return;
                            }
                            else
                            {
                                Console.WriteLine($"Uploaded chunk {chunkNumber + 1}/{totalChunks}");

                                // อัปเดต ProgressBar
                                //toolStripProgressBar1.Value = chunkNumber + 1;
                            }
                        }
                    }
                }
            }
        }
#endif
        private int id = 0;
        private string filePath = "";
        private string name = "";
        private string description = "";
        private int current_type = 0;
        private void progressDialog1_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
            string serverUrl = $"{this.URL}/api/v1/filemanager/upload-chunk-model";
            int chunkSize = 2 * 1024 * 1024; // 2M
            // check file exist
            if (!File.Exists(filePath) && id > 0)
            {
                serverUrl = $"{this.URL}/api/v1/filemanager/update-info";
                updateIfileInfo(serverUrl, name, description, id);
                return;
            }
            using (var client = new HttpClient())
            {
                byte[] buffer = new byte[chunkSize];
                long fileSize = new FileInfo(filePath).Length;
                int totalChunks = (int)Math.Ceiling(fileSize / (double)chunkSize);

                // ตั้งค่า ProgressBar
                //toolStripProgressBar1.Maximum = totalChunks;
                //toolStripProgressBar1.Value = 0;
                string new_filename = Path.GetFileNameWithoutExtension(filePath) + Guid.NewGuid().ToString().Replace("-", "_").Substring(0, 5);
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    for (int chunkNumber = 0; chunkNumber < totalChunks; chunkNumber++)
                    {
                        // อ่านข้อมูลเป็น chunk
                        int bytesRead = fileStream.ReadAsync(buffer, 0, chunkSize).Result;
                        if (bytesRead == 0) break;

                        using (var form = new MultipartFormDataContent())
                        {
                            // สร้างเนื้อหาไฟล์ chunk
                            var fileContent = new ByteArrayContent(buffer, 0, bytesRead);
                            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                            // เพิ่มไฟล์ chunk และข้อมูลในฟอร์ม
                            form.Add(fileContent, "file", Path.GetFileName(filePath));
                            form.Add(new StringContent(name), "name");
                            form.Add(new StringContent(description), "description");
                            form.Add(new StringContent(chunkNumber.ToString()), "chunk_number");
                            form.Add(new StringContent(totalChunks.ToString()), "total_chunks");
                            form.Add(new StringContent(new_filename), "filename");
                            
                            string type = current_type == 0 ? "cls": "detect";

                            form.Add(new StringContent(type), "file_type");
                            if (id > 0)
                            {
                                form.Add(new StringContent(id.ToString()), "id");
                            }

                            // ส่ง POST request ไปยังเซิร์ฟเวอร์
                            var response = client.PostAsync(serverUrl, form).Result;

                            if (!response.IsSuccessStatusCode)
                            {
                                MessageBox.Show($"Failed to upload chunk {chunkNumber + 1}. Status code: {response.StatusCode}");
                                return;
                            }
                            else
                            {
                                Console.WriteLine($"Uploaded chunk {chunkNumber + 1}/{totalChunks}");

                                int percent = (int)((chunkNumber + 1) * 100 / totalChunks);
                                progressDialog.ReportProgress(percent, "Upload File", string.Format(System.Globalization.CultureInfo.CurrentCulture, "Processing: {0}%", percent));
                            }
                        }
                    }
                }
            }
        }

        private void updateIfileInfo(string url, string name, string description, int id =0)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var form = new MultipartFormDataContent();
                    form.Add(new StringContent(name), "name");
                    form.Add(new StringContent(description), "description");

                    string type = current_type == 0 ? "cls" : "detect";

                    form.Add(new StringContent(type), "file_type");
                    form.Add(new StringContent(id.ToString()), "id");
                    
                    HttpResponseMessage response = client.PostAsync(url, form).Result;
                    //response.EnsureSuccessStatusCode();
                    //FileValidApiResponse
                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    //FileApiResponse apiResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<FileApiResponse>(responseBody);
                    Logger.Info(responseBody);



                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error when fetching data from server");
                    throw new Exception("Error when fetching data from server : " + ex.Message);
                }
            }
        }

        private void progressDialog1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Logger.Error(e.Error, "Error when uploading file");
                // MessageBox.Show(e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel.Text = "Error when uploading file";
            }
            else
            {
            //    MessageBox.Show("Upload completed", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                toolStripStatusLabel.Text = "Upload completed";
                _ = RenderDGVFile();
            }

        }

        private void dgvFile_SelectionChanged(object sender, EventArgs e)
        {
            if(dgvFile.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvFile.SelectedRows[0];
                id = Convert.ToInt32(row.Cells["id"].Value);
                txtName.Text = row.Cells["name"].Value.ToString();
                txtDescription.Text = row.Cells["description"].Value.ToString();
                cbType.SelectedIndex = row.Cells["file_type"].Value.ToString() == "cls" ? 0 : 1;
                current_type = cbType.SelectedIndex;
                txtPath.Text = row.Cells["filename"].Value.ToString();
                btnSave.Text = "Update";
                btnDelete.Enabled = true;
            }
            else
            {
                id = 0;
                txtName.Text = "";
                txtDescription.Text = "";
                txtPath.Text = "";
                btnSave.Text = "Save";
                btnDelete.Enabled = false;
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            id = 0;
            txtName.Text = "";
            txtDescription.Text = "";
            txtPath.Text = "";
            btnSave.Text = "Save";
            dgvFile.ClearSelection();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            this.name = txtName.Text;
            this.countValid = 0;
            timerValidName.Stop();
            timerValidName.Start();
        }

        private void txtDescription_TextChanged(object sender, EventArgs e)
        {
            this.description = txtDescription.Text;
        }

        private void txtPath_TextChanged(object sender, EventArgs e)
        {
            this.filePath = txtPath.Text;
        }

        private bool fatchValidaName(int id = 0)
        {
            string url = $"{URL}/api/v1/filemanager/validate";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                   
                    var form = new MultipartFormDataContent();
                    form.Add(new StringContent(txtName.Text), "name");
                    form.Add(new StringContent(id.ToString()), "id");

                    HttpResponseMessage response = client.PostAsync(url, form).Result;
                    response.EnsureSuccessStatusCode();
                    //FileValidApiResponse
                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    FileValidApiResponse apiResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<FileValidApiResponse>(responseBody);
                    return apiResponse?.status == "success" && apiResponse.is_valid;

                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error when fetching data from server");
                    return false;
                }
            }
        }

        int countValid = 0;
        private void timerValidName_Tick(object sender, EventArgs e)
        {
            if(countValid > 0)
            {
                countValid--;
                return;
            }

            timerValidName.Stop();

            if (fatchValidaName(id))
            {
                txtName.BackColor = Color.White;
                btnSave.Enabled = true;
            }
            else
            {
                txtName.BackColor = Color.Red;
                btnSave.Enabled = false;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (id <= 0)
                {
                    MessageBox.Show("Please select a file to delete", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string url = $"{URL}/api/v1/filemanager/delete";
                using (HttpClient client = new HttpClient())
                {
                    var form = new MultipartFormDataContent();
                    form.Add(new StringContent(id.ToString()), "id");
                    HttpResponseMessage response = client.PostAsync(url, form).Result;
                    response.EnsureSuccessStatusCode();
                    //FileValidApiResponse
                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    FileValidApiResponse apiResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<FileValidApiResponse>(responseBody);
                    if (apiResponse?.status == "success")
                    {
                        MessageBox.Show("File deleted", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _ = RenderDGVFile();
                    }
                    else
                    {
                        MessageBox.Show("Error when deleting file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Error(ex, "Error when deleting file");
                MessageBox.Show("Error when deleting file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.File_type = cbType.SelectedIndex;
            current_type = cbType.SelectedIndex;
            Properties.Settings.Default.Save();
        }
    }
}