using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Orchard.ContentFields.Fields;
using Orchard.ContentFields.Settings;
using Orchard.ContentFields.ViewModels;
using Orchard.ContentManagement.Display.ContentDisplay;
using Orchard.ContentManagement.Display.Models;
using Orchard.ContentManagement.Metadata.Models;
using Orchard.DisplayManagement.ModelBinding;
using Orchard.DisplayManagement.Views;

namespace Orchard.ContentFields.Fields
{
    public class DateTimeFieldDisplayDriver : ContentFieldDisplayDriver<DateTimeField>
    {
        public DateTimeFieldDisplayDriver(IStringLocalizer<LinkFieldDisplayDriver> localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }
        public override IDisplayResult Display(DateTimeField field, BuildFieldDisplayContext context)
        {
            return Shape<DisplayDateTimeFieldViewModel>("DateTimeField", model =>
            {
                model.Field = field;
                model.Part = context.ContentPart;
                model.PartFieldDefinition = context.PartFieldDefinition;
            })
            .Location("Content")
            .Location("SummaryAdmin", "");
            ;
        }

        public override IDisplayResult Edit(DateTimeField field, BuildFieldEditorContext context)
        {
            var settings = context.PartFieldDefinition.Settings.ToObject<DateTimeFieldSettings>();

            return Shape<EditDateTimeFieldViewModel>("DateTimeField_Edit", model =>
            {
                model.Value = field.Value.HasValue ? field.Value.Value.ToString(settings.Format, CultureInfo.InvariantCulture) : "";
                model.Field = field;
                model.Part = context.ContentPart;
                model.PartFieldDefinition = context.PartFieldDefinition;
            });
        }

        public override async Task<IDisplayResult> UpdateAsync(DateTimeField field, IUpdateModel updater, UpdateFieldEditorContext context)
        {
            var viewModel = new EditDateTimeFieldViewModel();

            bool modelUpdated = await updater.TryUpdateModelAsync(viewModel, Prefix, f => f.Value);

            if (modelUpdated)
            {
                field.Value = null;
                var settings = context.PartFieldDefinition.Settings.ToObject<DateTimeFieldSettings>();
                DateTime dateTime;
                if (DateTime.TryParseExact(viewModel.Value, settings.Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                {
                    field.Value = dateTime;
                }
                else
                {
                    updater.ModelState.AddModelError(Prefix, T["Date is invalid to formar {0}.", settings.Format]);
                }

            }

            return Edit(field, context);
        }
    }
}
