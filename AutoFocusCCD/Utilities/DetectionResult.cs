using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFocusCCD.Utilities
{
    public class DetectionResult
    {
        public List<Detection> Result { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
    }

    public class Detection
    {
        public float[] Box { get; set; }
        public int Class { get; set; }
        public float Confidence { get; set; }
        public string Name { get; set; }

        public float X => Box[0];
        public float Y => Box[1];

        public float Width => Box[2] - Box[0];
        public float Height => Box[3] - Box[1];
    }
}
