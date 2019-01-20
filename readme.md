# PoExtractor

This utility extracts translatable strings from the C# code, Razor templates and Liquid templates to POT (portable object template) files. It is designed to follow conventions used in the [OrchardCore](https://github.com/OrchardCMS/OrchardCore) project.

## Usage

`dotnet PoExtractor.dll inputpath outputpath`

Extracts all translatable strings from projects at the specified input path and saves generated POT files at the specified output path. It creates one POT file per a project.

Use `dotnet PoExtractor.dll inputpath outputpath --liquid` to include translations from .liquid files (requires Fluid.Core 1.0.0-beta-9501 or newer)

## Limitations

PoExtractor assumes, the code follows several conventions:

* `IStringLocalizer` or a derived class is accessed via property named `T`
* Liquid templates uses a filter named `t`
* context of the localizable string is full name (with namespace) of the containing class for C# code
* context of the localizable string is dot-delimited relative path the to view for Razor templates
* context of the localizable string is dot-delimited relative path the to template for Liquid templates
* 
## Example

C# code:
```csharp
namespace OrchardCore.ContentFields.Fields { 
    public class LinkFieldDisplayDriver : ContentFieldDisplayDriver<LinkField> {
        public LinkFieldDisplayDriver(IStringLocalizer<LinkFieldDisplayDriver> localizer) {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }
        
        public override async Task<IDisplayResult> UpdateAsync(LinkField field, IUpdateModel updater, UpdateFieldEditorContext context) {
            bool modelUpdated = await updater.TryUpdateModelAsync(field, Prefix, f => f.Url, f => f.Text);

            if (modelUpdated)
            {
                var settings = context.PartFieldDefinition.Settings.ToObject<LinkFieldSettings>();

                if (settings.Required && String.IsNullOrWhiteSpace(field.Url))
                {
                    updater.ModelState.AddModelError(Prefix, T["The url is required for {0}.", context.PartFieldDefinition.DisplayName()]);
                }
            }

            return Edit(field, context);
        }
    }
}
```

Razor view:
```html
@model OrchardCore.ContentFields.ViewModels.EditLinkFieldViewModel

<div class="row">
    <fieldset class="form-group col-md-12">
        <label asp-for="Url">@Model.PartFieldDefinition.DisplayName()</label>
    </fieldset>
    <fieldset class="form-group col-md-6" asp-validation-class-for="Url">
        <input asp-for="Url" class="form-control content-preview-text" placeholder="@settings.UrlPlaceholder" required="@isRequired" />
    </fieldset>
    <fieldset class="form-group col-md-6" asp-validation-class-for="Text">
        <label asp-for="Text" @if (settings.LinkTextMode == LinkTextMode.Required) { <text> class="required" </text>  }>@T["Link text"]</label>
        <input asp-for="Text" type="text" class="form-control content-preview-text" placeholder="@settings.TextPlaceholder" required="@isTextRequired" />
    </fieldset>
</div>

```

Liquid template:
```html
div class="page-heading">
   <h1>{{ "Page Not Found" | t }}</h1>
/div>

```

Generated POT file:
```
#: OrchardCore.ContentFields\Drivers\LinkFieldDriver.cs:59
#. updater.ModelState.AddModelError(Prefix, T["The url is required for {0}.", context.PartFieldDefinition.DisplayName()]);
msgctxt "OrchardCore.ContentFields.Fields.LinkFieldDisplayDriver"
msgid "The url is required for {0}."
msgstr ""

#: OrchardCore.ContentFields\Views\LinkField.Edit.cshtml:32
#. <label asp-for="Text" @if (settings.LinkTextMode == LinkTextMode.Required) { <text> class="required" </text>  }>@T["Link text"]</label>
msgctxt "OrchardCore.ContentFields.Views.LinkField.Edit"
msgid "Link text"
msgstr ""

#: TheBlogTheme\Views\Shared\NotFound.liquid:0
msgctxt "TheBlogTheme.Views.Shared.NotFound"
msgid "Page Not Found"
msgstr ""
```
