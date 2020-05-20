using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business
{

    public class Settings
    {
        private string[] _supportedLangs;
        public string[] SupportedLangs
        {
            get
            {
                return _supportedLangs;
            }
            set
            {
                _supportedLangs = value;
                if (value != null)
                    SupportedCultures = value.Select(c => new CultureInfo(c)).ToArray();
            }
        }
        public CultureInfo[] SupportedCultures { get; set; }
        public string Name { get; set; }
        public FirebaseConfig FirebaseConfig { get; set; }

        private static Settings _instance;
        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Settings();
                return _instance;
            }
        }
    }

    public class FirebaseConfig
    {
        public string project_id { get; set; }
    }
}
