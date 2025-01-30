using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Multi_Camera_MINI_AOI_V3.Utilities
{
    public static class Extensions
    {
        public static string ReadFileOnce(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        public static string[] ReadFile(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream))
            {
                var content = reader.ReadToEnd();
                return content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            }
        }
        public static Bitmap ReScale(Bitmap bmp, System.Drawing.Size newSize)
        {
            Bitmap scaledBitmap = new Bitmap(newSize.Width, newSize.Height);

            using (Graphics graphics = Graphics.FromImage(scaledBitmap))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

                graphics.DrawImage(bmp, 0, 0, newSize.Width, newSize.Height);
            }

            return scaledBitmap;
        }
        public static Bitmap ReScale(Bitmap bmp, float percentage)
        {

            if (bmp == null) throw new ArgumentNullException("bmp cannot be null.");
            if (percentage <= 0 || percentage > 2) throw new ArgumentOutOfRangeException("percentage must be between 0 and 1.");

            int newWidth = (int)(bmp.Width * percentage);
            int newHeight = (int)(bmp.Height * percentage);

            if (newWidth <= 0 || newHeight <= 0)
            {
                throw new InvalidOperationException("Scaled dimensions are too small or negative.");
            }

            return ReScale(bmp, new System.Drawing.Size(newWidth, newHeight));

        }

        public static int GetSelectedRowIndex(DataGridView dataGrid)
        {
            if (dataGrid.SelectedRows.Count > 0)
            {
                return dataGrid.SelectedRows[0].Index;
            }
            return -1;
        }

        public static void SelectedRow(DataGridView dataGrid, int rowIndex)
        {
            dataGrid.ClearSelection();
            if (rowIndex >= 0 && rowIndex < dataGrid.Rows.Count)
            {
                dataGrid.Rows[rowIndex].Selected = true;
            }
        }

        public static bool IsValidIP(string ip)
        {
            // Regular expression pattern to match valid IPv4 addresses.
            string pattern = @"^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            return Regex.IsMatch(ip, pattern);
        }

        /**
         *  
         */
        public static bool IsValidURL(string url)
        {
            Uri uriResult;
            return Uri.TryCreate(url, UriKind.Absolute, out uriResult) && uriResult.Scheme == Uri.UriSchemeHttp; // true: valid URL, false: invalid URL
        }

        public static bool IsValidPort(string port)
        {
            int portNumber;
            if (int.TryParse(port, out portNumber))
            {
                return portNumber >= 0 && portNumber <= 65535;
            }
            return false;
        }


        public static float? StringToFloat(string input)
        {
            float result;
            if (float.TryParse(input, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static float? StringToInt(string input)
        {
            int result;
            if (int.TryParse(input, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static bool IsFloat(string input)
        {
            float result;
            return float.TryParse(input, out result);
        }

        public static bool IsInt(string input)
        {
            int result;
            return int.TryParse(input, out result);
        }

        public static bool IsDigits(string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            return s.All(char.IsDigit);
        }
        public static bool IsHexadecimal(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex)) return false;

            return Regex.IsMatch(hex, @"^[0-9A-Fa-f]+$");
        }


        public static void SetDataSourceAndUpdateSelection(DataGridView dgv, DataTable dt, string[] visibleColumns = null, Dictionary<string, string> changeColumns = null)
        {
            try
            {
                dgv.DataSource = dt;
                if (visibleColumns != null)
                {
                    foreach (DataGridViewColumn column in dgv.Columns)
                    {
                        if (visibleColumns.Contains(column.Name))
                        {
                            column.Visible = false;
                        }

                        if (column.Name == "No")
                        {
                            column.Width = 30;
                        }
                    }
                }
                if (changeColumns != null)
                {
                    foreach (var item in changeColumns)
                    {
                        dgv.Columns[item.Key].HeaderText = item.Value;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error Dt:" + e.Message);
            }
        }

        public static void CropSaveImage(string path, Image image, Rectangle rect)
        {
            if (rect.Width == 0 || rect.Height == 0)
            {
                return;
            }
            using (Bitmap bmp = new Bitmap(rect.Width, rect.Height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(image, 0, 0, rect, GraphicsUnit.Pixel);
                }
                bmp.Save(path, ImageFormat.Jpeg);
            }
        }

        public static void CropSaveImage(string path, Image image, int x, int y, int width, int height)
        {
            if (width == 0 || height == 0)
            {
                return;
            }
            using (Bitmap bmp = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(image, 0, 0, new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
                }
                bmp.Save(path, ImageFormat.Jpeg);
            }
        }

        public static Image GetImage(Image image, Rectangle rect)
        {
            if (rect.Width == 0 || rect.Height == 0)
            {
                return null;
            }
            using (Bitmap bmp = new Bitmap(rect.Width, rect.Height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(image, 0, 0, rect, GraphicsUnit.Pixel);
                }
                return bmp;
            }
        }

        public static Image GetImage(Image image, int x, int y, int width, int height)
        {
            if (width == 0 || height == 0)
            {
                return null;
            }
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(image, 0, 0, new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
            }
            return bmp;

        }

        public static void RefreshComboBoxWithList(System.Windows.Forms.ComboBox comboBox, IList<string> items, bool selectLast = false)
        {
            int oldSelectedIndex = comboBox.SelectedIndex;
            comboBox.Items.Clear();
            comboBox.Items.AddRange(items.ToArray());
            if (comboBox.Items.Count <= 0) return;

            if (oldSelectedIndex > 0 && oldSelectedIndex < comboBox.Items.Count)
            {
                comboBox.SelectedIndex = oldSelectedIndex;
            }
            else
            {
                comboBox.SelectedIndex = selectLast ? comboBox.Items.Count - 1 : 0;
            }
        }
    }

}
