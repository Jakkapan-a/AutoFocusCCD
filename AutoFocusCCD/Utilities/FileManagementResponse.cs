using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFocusCCD.Utilities
{
    public class FileApiResponse
    {
        public Data data { get; set; } 
        public string message { get; set; }
        public string status { get; set; }
    }

    public class Data
    {
        public List<Item> items { get; set; }
        public Pagination pagination { get; set; }
    }

    public class Item
    {
        public string created_at { get; set; }
        public string description { get; set; }
        public string filename { get; set; }
        public int id { get; set; }
        public string image_name { get; set; }
        public string name { get; set; }
        public string updated_at { get; set; }
        public string file_type { get; set; }
    }

    public class Pagination
    {
        public int current_page { get; set; }
        public bool has_next { get; set; }
        public bool has_prev { get; set; }
        public int per_page { get; set; }
        public int total_items { get; set; }
        public int total_pages { get; set; }
    }

    public class FileValidApiResponse
    {
        public bool is_valid { get; set; }
        public string message { get; set; }
        public string status { get; set; }
    }
}
