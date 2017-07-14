using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orchard.ContentFields.Fields;
using Orchard.Indexing;

namespace Orchard.ContentFields.Indexing
{
    public class DateTimeFieldIndexHandler : ContentFieldIndexHandler<DateTimeField>
    {
        public override Task BuildIndexAsync(DateTimeField field, BuildFieldIndexContext context)
        {
            var options = context.Settings.ToOptions();
            context.DocumentIndex.Entries.Add(context.Key, new DocumentIndex.DocumentIndexEntry(field.Value, DocumentIndex.Types.DateTime, options));

            return Task.CompletedTask;
        }
    }
}
