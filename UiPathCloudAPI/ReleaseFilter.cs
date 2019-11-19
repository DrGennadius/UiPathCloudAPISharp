using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp
{
    public class ReleaseFilter : IFilter
    {
        public string Name { get; set; }

        public string Value
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Name))
                {
                    return "";
                }
                else
                {
                    return string.Format("$filter=Name%20eq%20%27{0}%27", Name);
                }
            }
        }
    }
}
