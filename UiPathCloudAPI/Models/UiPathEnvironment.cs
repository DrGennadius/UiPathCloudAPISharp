using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Models
{
    public class UiPathEnvironment
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public int Id { get; set; }

        List<Robot> Robots { get; set; }
    }
}
