using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA_Window.model
{
    public class FileAttribute
    {
        public int SerialNumber { get; set; }
        public string ID { get; set; }
        public bool IsCurrent { get; set; }
        public bool Flag { get; set; }
        public string FileName { get; set; }
        public string UpdateTime { get; set; }
        public string FilePath { get; set; }
        public string Status { get; set; }
        public bool IsExecute { get; set; }
        public string FontColor { get; set; }
    }
}
