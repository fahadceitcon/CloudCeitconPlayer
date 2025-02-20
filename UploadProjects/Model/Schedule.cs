using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadProjects.Model
{
    public class Schedule
    {
        public string UniqueId { get; set; } //Must be a unique GUID which should be saved on designer side for future updates
        public string ProjectName { get; set; } // Must be with .cdp extension
        public string Version { get; set; } // Version Of the Project
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string LocationGroup { get; set; }
        public string CDPFileContent { get; set; }
        public string Location { get; set; } // Name of THe folder for the project content
        public string? ContentFiles { get; set; }
        public List<string>? Files { get; set; } // File names must be same as in the CDP project file
        public bool Selected { get; set; }
    }
}
