using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp
{
    public class SessionFilter : IFilter
    {
        public string Value
        {
            get
            {
                if (_resultBuilder == null)
                {
                    _resultBuilder = new StringBuilder();
                }
                else
                {
                    _resultBuilder.Clear();
                }

                if (Top != -1)
                {
                    AppendToResult(string.Format("$top={0}", Top));
                }
                if (!string.IsNullOrEmpty(Select))
                {
                    AppendToResult(string.Format("$select={0}", Select));
                }
                if (!string.IsNullOrEmpty(Expand))
                {
                    AppendToResult(string.Format("$expand={0}", Expand));
                }

                return _resultBuilder.ToString();
            }
        }

        public int Top { get; set; } = -1;

        public string Select { get; set; }

        public string Expand { get; set; }

        private StringBuilder _resultBuilder;

        private void AppendToResult(string element)
        {
            if (_resultBuilder.Length > 0)
            {
                _resultBuilder.Append(string.Format("&{0}", element));
            }
            else
            {
                _resultBuilder.Append(element);
            }
        }
    }
}
