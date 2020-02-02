using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Models
{
    public class NewQueueItemData
    {
        public NewQueueItemData()
        {
            SpecificContent = new Dictionary<string, object>();
        }

        public string Name { get; set; }

        public string Priority { get; set; }

        public Dictionary<string, object> SpecificContent { get; set; }

        public DateTime? DeferDate { get; set; }

        public DateTime? DueDate { get; set; }

        public string Reference { get; set; }

        public void AddSpecificContent(string name, string value)
        {
            SpecificContent.Add(name + "@odata.type", "#String");
            SpecificContent.Add(name, value);
        }

        public void AddSpecificContent(string name, int value)
        {
            SpecificContent.Add(name, value);
        }

        public void AddSpecificContent(string name, bool value)
        {
            SpecificContent.Add(name, value);
        }
    }
}
