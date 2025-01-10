using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoFocusCCD.Config
{
    public class PreferencesConfig
    {
        public NetworkConfig Network { get; set; }
        public ProcessingConfig Processing { get; set; }
        public ClearMesConfig ClearMes { get; set; }
        public FileSystemConfig FileSystem { get; set; }
        public OptionNGConfig OptionNG { get; set; }

        public class NetworkConfig
        {
            public string URL { get; set; }
        }

        public class ProcessingConfig
        {
            public int Threshold { get; set; }
        }

        public class ClearMesConfig
        {
            public string Message1 { get; set; }
            public int Delay { get; set; }
            public string Message2 { get; set; }
        }

        public class FileSystemConfig
        {
            public int DeleteFileAfterDays { get; set; }
            public string Path { get; set; }
        }

        public class OptionNGConfig
        {
            public bool AllowSendNG { get; set; }
            public string Key { get; set; }
            public int Delay { get; set; }
            public string Message { get; set; }
            public string Description { get; set; }
        }
    }

    // Load config from file
    public class PreferencesConfigLoader
    {
        public static PreferencesConfig Load(string path)
        {
            try
            {
                if(!File.Exists(path))
                {
                    // load default config and save to file
                    PreferencesConfig config = LoadDefault();
                    Save(path, config);
                }
                return JsonConvert.DeserializeObject<PreferencesConfig>(File.ReadAllText(path));
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static void Save(string path, PreferencesConfig config)
        {
            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(path, json);
        }


        public static PreferencesConfig LoadDefault()
        {
            return new PreferencesConfig
            {
                Network = new PreferencesConfig.NetworkConfig
                {
                    URL = "http://127.0.0.1:11001"
                },
                Processing = new PreferencesConfig.ProcessingConfig
                {
                    Threshold = 100
                },
                ClearMes = new PreferencesConfig.ClearMesConfig
                {
                    Message1 = "clear",
                    Delay = 500,
                    Message2 = "test"
                },
                FileSystem = new PreferencesConfig.FileSystemConfig
                {
                    DeleteFileAfterDays = 10,
                    Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Assembly.GetExecutingAssembly().GetName().Name)
                },
                OptionNG = new PreferencesConfig.OptionNGConfig
                {
                    AllowSendNG = true,
                    Key = "006D",
                    Delay = 500,
                    Message = "NG",
                    Description = "Send NG to MES"
                }
            };
        }        
    }
}
