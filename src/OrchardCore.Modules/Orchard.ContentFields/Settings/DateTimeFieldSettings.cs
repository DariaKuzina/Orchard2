using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Orchard.ContentFields.Settings
{
    public class DateTimeFieldSettings
    {
        public string Hint { get; set; }
        public string Editor { get; set; }

        private string _format;
        public string Format
        {
            get
            {
                return _format ?? CultureInfo.InvariantCulture.DateTimeFormat.ShortDatePattern;
            }

            set
            {
                _format = value;
            }
        }
    }
}
