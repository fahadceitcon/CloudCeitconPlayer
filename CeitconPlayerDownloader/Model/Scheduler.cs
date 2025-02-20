using System;

namespace CeitconPlayerDownloader.Model
{
    public class Scheduler
    {
        public string _Id { get; set; }
        public string _Client { get; set; }
        public string _Project { get; set; }
        public string _Version { get; set; }
        public DateTime _StartTime { get; set; }
        public DateTime _EndTime { get; set; }
    }
}
