using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Models
{
    /// <summary>
    /// Organization Unit.
    /// </summary>
    public class Folder
    {
        public string DisplayName { get; set; }

        /// <summary>
        /// Folder Id (Organization Unit Id).
        /// </summary>
        public int Id { get; set; }
    }
}
