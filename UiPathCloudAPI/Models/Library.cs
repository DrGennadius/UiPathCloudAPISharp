using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Models
{
    public class Library
    {
        public string Title { get; set; }

        public string Version { get; set; }

        public string Key { get; set; }

        public string Description { get; set; }

        public DateTime Published { get; set; }

        public bool IsLatestVersion { get; set; }

        public string OldVersion { get; set; }

        public string ReleaseNotes { get; set; }

        public string Authors { get; set; }

        public string Id { get; set; }
    }
}
