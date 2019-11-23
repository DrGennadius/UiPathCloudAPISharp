using System;

namespace UiPathCloudAPISharp.Models
{
    public class Job
    {
        public string Key { get; set; }
        
        public DateTime? StartTime { get; set; }
        
        public DateTime? EndTime { get; set; }
        
        public string State { get; set; }
        
        public string Source { get; set; }
        
        public string BatchExecutionKey { get; set; }
        
        public string Info { get; set; }
        
        public DateTime CreationTime { get; set; }
        
        public string StartingScheduleId { get; set; }
        
        public int Id { get; set; }
    }
}
