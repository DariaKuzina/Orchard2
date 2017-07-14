using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orchard.ContentFields.Fields;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Metadata.Models;

namespace Orchard.ContentFields.ViewModels
{
    public class EditDateTimeFieldViewModel
    {
        public string Value { get; set; }
        public DateTimeField Field { get; set; }
        public ContentPart Part { get; set; }
        public ContentPartFieldDefinition PartFieldDefinition { get; set; }
    }
}
