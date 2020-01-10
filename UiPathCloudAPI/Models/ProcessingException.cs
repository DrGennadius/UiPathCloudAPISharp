using System;

namespace UiPathCloudAPISharp.Models
{
    public class ProcessingException
    {
        public string Reason { get; set; }

        public string Details { get; set; }

        public string Type { get; set; }

        public string AssociatedImageFilePath { get; set; }

        public DateTime CreationTime { get; set; }
    }
}