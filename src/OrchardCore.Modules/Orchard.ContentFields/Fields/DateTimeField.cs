using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orchard.ContentManagement;

namespace Orchard.ContentFields.Fields
{
    public class DateTimeField : ContentField
    {
        public DateTime? Value { get; set; }
    }
}
