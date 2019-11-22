using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp
{
    public class SessionFilter : IFilter
    {
        public int RobotId { get; set; } = -1;

        public string RobotName { get; set; }

        public string State { get; set; }

        public DateTimeRange ReportingTimeRange { get; set; }

        private StringBuilder _resultBuilder;

        private void AppendToResult(string element)
        {
            if (_resultBuilder.Length > 0)
            {
                _resultBuilder.Append(string.Format("%20and%20", element));
            }
            else
            {
                _resultBuilder.Append(element);
            }
        }

        public string GetODataString()
        {
            if (_resultBuilder == null)
            {
                _resultBuilder = new StringBuilder();
            }
            else
            {
                _resultBuilder.Clear();
            }

            if (RobotId != -1)
            {
                AppendToResult(string.Format("Robot/Id%20eq%20%27{0}%27", RobotId));
            }
            if (!string.IsNullOrEmpty(RobotName))
            {
                AppendToResult(string.Format("Robot/Name%20eq%20%27{0}%27", RobotName));
            }
            if (!string.IsNullOrEmpty(State))
            {
                AppendToResult(string.Format("State%20eq%20%27{0}", State));
            }
            if (ReportingTimeRange != null)
            {
                AppendToResult(ReportingTimeRange.GetString("ReportingTime"));
            }

            string result = _resultBuilder.ToString();
            if (string.IsNullOrEmpty(result))
            {
                return result;
            }
            else
            {
                return "$filter=" + result;
            }
        }
    }
}
