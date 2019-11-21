using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp
{
    public class DateTimeRange
    {
        private DateTime? _minValue;

        private DateTime? _maxValue;

        public bool ExcludeMin { get; set; }

        public bool ExcludeMax { get; set; }

        public DateTime? GetMinValue()
        {
            return _minValue;
        }

        public void SetMinValue(DateTime minDate, bool exclude = false)
        {
            _minValue = minDate;
            ExcludeMin = exclude;
        }

        public DateTime? GetMaxValue()
        {
            return _maxValue;
        }

        public void SetMaxValue(DateTime maxDate, bool exclude = false)
        {
            _maxValue = maxDate;
            ExcludeMax = exclude;
        }

        public string GetString(string valueName)
        {
            string result = "";

            if (_minValue.HasValue && _maxValue.HasValue)
            {
                if (_minValue.Value == _maxValue.Value)
                {
                    if (ExcludeMin && ExcludeMax)
                    {
                        // (x:x)
                        result = string.Format("{0}%20ne%20{1}", valueName, _minValue.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                    }
                    else
                    {
                        // [x:x]
                        result = string.Format("{0}%20eq%20{1}", valueName, _minValue.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                    }
                }
                else if (!ExcludeMin && !ExcludeMax)
                {
                    // [x:y]
                    result = string.Format("{0}%20ge%20{1}%20and%20{0}%20le%20{2}", valueName, _minValue.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"), _maxValue.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                }
                else if (ExcludeMin && !ExcludeMax)
                {
                    // (x:y]
                    result = string.Format("{0}%20gt%20{1}%20and%20{0}%20le%20{2}", valueName, _minValue.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"), _maxValue.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                }
                else if (!ExcludeMin && ExcludeMax)
                {
                    // [x:y)
                    result = string.Format("{0}%20ge%20{1}%20and%20{0}%20lt%20{2}", valueName, _minValue.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"), _maxValue.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                }
                else
                {
                    // (x:y)
                    result = string.Format("{0}%20gt%20{1}%20and%20{0}%20lt%20{2}", valueName, _minValue.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"), _maxValue.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                }
            }
            else if (_minValue.HasValue)
            {
                if (ExcludeMin)
                {
                    // (x:infinity)
                    result = string.Format("{0}%20gt%20{1}", valueName, _minValue.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                }
                else
                {
                    // [x:infinity)
                    result = string.Format("{0}%20ge%20{1}", valueName, _minValue.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                }
            }
            else if (_maxValue.HasValue)
            {
                if (ExcludeMax)
                {
                    // (infinity:y)
                    result = string.Format("{0}%20lt%20{1}", valueName, _maxValue.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                }
                else
                {
                    // (infinity:y]
                    result = string.Format("{0}%20le%20{1}", valueName, _maxValue.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                }
            }

            return result;
        }
    }
}
