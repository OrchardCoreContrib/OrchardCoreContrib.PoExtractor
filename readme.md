# PoExtractor

This utility extracts translatable strings from the C# and VB code, Razor templates and Liquid templates to POT (portable object template) files. It is designed to follow conventions used in the [OrchardCore](https://github.com/OrchardCMS/OrchardCore) project.

PoExtractor is distributed as a dotnet global tool and it is available on the official NuGet.org feed in two versions:

* `PoExtractor` - a general purpose utility for extracting translatable strings from  C#, VB code and Razor views
* `PoExtractor.OrchardCore` - the same utility with the additional support for extracting translatable strings from Liquid templates.
  * In order to be able to parse Liquid templates, it needs to reference `OrchardCore.DisplayManagement.Liquid` package, where the Liquid filters are defined

## Installation


Install with the following command:

`dotnet tool install --global PoExtractor`

or

`dotnet tool install --global PoExtractor.OrchardCore`

> The latest version of PoExtractor.OrchardCore is build against OrchardCore 1.0.0-beta3-71077. If you want to use PoExtractor with OrchardCore 1.0.0-RC1 please use version 0.2.0-rc1
>
> `dotnet tool install --global PoExtractor.OrchardCore --version 0.2.0-rc1`
> 
> If you cant to use PoExtractor with the preview version of OrchardCore, please use version 0.4.0-rc1-13272
>
> `dotnet tool install --global PoExtractor.OrchardCore --version 0.4.0-rc1-13272`

## Usage

`extractpo inputpath outputpath`

or

`extractpo-oc inputpath outputpath`

Extracts all translatable strings from projects at the specified input path and saves generated POT files at the specified output path. It creates one POT file per a project. This includes liquid views.

## Uninstallation

`dotnet tool uninstall --global PoExtractor`

or

`dotnet tool uninstall --global PoExtractor.OrchardCore`

## Limitations

PoExtractor assumes, the code follows several conventions:

* `IStringLocalizer` or a derived class is accessed via a field named `S` (This is a convention used in Orchard Core)
* `IHtmlLocalizer` or a derived class is accessed via a field named `H` (This is a convention used in Orchard Core)
* `IStringLocalizer` or `IHtmlLocalizer` is accessed via a field named `T` (This is a older convention used in Orchard Core)
* Liquid templates use the filter named `t` (This is a convention used in Fluid)
* context of the localizable string is the full name (with namespace) of the containing class for C# or VB code
* context of the localizable string is the dot-delimited relative path the to view for Razor templates
* context of the localizable string is the dot-delimited relative path the to template for Liquid templates
 
## Example

C# code:
```csharp
namespace OrchardCore.ContentFields.Fields { 
    public class LinkFieldDisplayDriver : ContentFieldDisplayDriver<LinkField> {
        private IStringLocalizer S;

        public LinkFieldDisplayDriver(IStringLocalizer<LinkFieldDisplayDriver> localizer) {
            S = localizer;
        }

        public override async Task<IDisplayResult> UpdateAsync(LinkField field, IUpdateModel updater, UpdateFieldEditorContext context) {
            bool modelUpdated = await updater.TryUpdateModelAsync(field, Prefix, f => f.Url, f => f.Text);

            if (modelUpdated)
            {
                var settings = context.PartFieldDefinition.Settings.ToObject<LinkFieldSettings>();

                if (settings.Required && String.IsNullOrWhiteSpace(field.Url))
                {
                    updater.ModelState.AddModelError(Prefix, S["The url is required for {0}.", context.PartFieldDefinition.DisplayName()]);
                }
            }

            return Edit(field, context);
        }
    }
}
```

VB code:
```vb
Namespace OrchardCore.Modules.GreetingModule 
    Public Class Greeting
        private readonly S As IStringLocalizer(Of Greeting)

        Public Sub New(ByVal localizer As IStringLocalizer(Of Greeting))
            S = localizer
        End Sub

        Public Sub Saulation(byVal name As String)
            Console.WriteLine(S("Hi {0} ...", name))
        End Sub
    End Class
End Namespace
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

#: OrchardCore.Modules.GreetingModule\Greeting.vb:94
#. Console.WriteLine(S("Hi {0} ...", name))
msgctxt "OrchardCore.Modules.GreetingModule.Greeting"
msgid "Hi {0} ..."
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
